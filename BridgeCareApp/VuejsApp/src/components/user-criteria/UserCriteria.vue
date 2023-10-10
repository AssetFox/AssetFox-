<template>
  <v-row column style='width: 90%; margin: auto'>
    <v-flex xs12>
      <v-card class='test1'>
        <div v-if='unassignedUsersCriteriaFilter.length > 0'>
          <v-card-title class='userCriteriaTableHeader'>
            Unassigned Users
          </v-card-title>
          <v-card-text style='justify-content: center; text-align: center; padding-top: 0'>
            The following users have no access to any inventory items.
          </v-card-text>
          <v-divider style='margin: 0' />
          <div v-for='user in unassignedUsersCriteriaFilter'>
            <v-row row class='unassigned-user-layout'>
              <v-flex class='unassigned-user-layout-username-flex' xs3>
                {{ user.userName }}
              </v-flex>
              <v-flex xs3>
                {{ user.description }}
              </v-flex>
              <v-flex style='padding: 0' xs4>
                <v-row align-center>
                  <v-btn @click='onEditCriteria(user)' class='ara-blue-bg text-white' 
                          title='Give the user limited access to the bridge inventory'>
                    <v-icon size='1.5em' style='padding-right: 0.5em'>fas fa-edit</v-icon>
                    Assign Criteria Filter
                  </v-btn>
                </v-row>
              </v-flex>
              <v-flex justify-center style='padding: 0' xs2>
                <v-row align-center>
                  <v-btn @click='onDeleteUser(user)' class='ara-orange' icon title='Delete User'>
                    <v-icon>fas fa-trash</v-icon>
                  </v-btn>
                </v-row>
              </v-flex>
            </v-row>
            <v-divider style='margin: 0' />
          </div>
        </div>
        <v-card-title class='userCriteriaTableHeader'>
          Inventory Access Criteria
        </v-card-title>
        <div>
          <v-text-field
            v-model='search'
            append-icon='mdi-magnify'
            label='Search'
            single-line
            hide-details
          ></v-text-field>
        </div>
        <div>
          <v-data-table
            :headers='userCriteriaGridHeaders'
            :items='filteredUsersCriteriaFilter'
            :items-per-page='5'
            class='elevation-1'
            hide-actions
            @sort='onSort'
          >
            <template v-slot:header="{ header, index }">
              <span style='cursor: pointer'>
                {{ header.text }}
                <v-icon v-if='sortKey === header.value'>
                  {{ sortOrder === 'asc' ? 'arrow_upward' : 'arrow_downward' }}
                </v-icon>
              </span>
            </template>
            <template v-slot:item="props" slot='items' slot-scope='props'>
              <td style='width: 15%; font-size: 1.2em; padding-top: 0.4em'>{{ props.item.userName }}</td>
              <td style='width: 35%'>
                <v-row align-center style='flex-wrap:nowrap; margin-left: 0; width: 100%'>
                  <v-menu bottom 
                      min-height='500px' min-width='500px' v-if='props.item.hasCriteria' style='width: 100%'>
                    <template v-slot:activator>
                      <v-text-field class='sm-txt' :model-value='props.item.criteria' readonly style='width: 100%' type='text' />
                    </template>
                    <v-card>
                      <v-card-text>
                        <v-textarea :model-value='props.item.criteria' full-width no-resize outline 
                        readonly 
                        rows='5'>
                      </v-textarea>
                      </v-card-text>
                    </v-card>
                  </v-menu>
                  <div style='font-size: 1.2em; font-weight: bold; padding-top: 0.4em; padding-right: 1em' 
                      v-if='!props.item.hasCriteria'>
                    All Assets
                  </div>
                  <v-btn @click='onEditCriteria(props.item)' class='edit-icon' icon 
                          title='Edit Criteria' 
                          v-if='props.item.hasCriteria'>
                    <v-icon>fas fa-edit</v-icon>
                  </v-btn>
                </v-row>
              </td>
              <td class='d-flex'>
                <v-btn @click='onEditCriteria(props.item)' class='ara-blue' icon 
                        title='Restrict Access with Criteria Filter' 
                        v-if='!props.item.hasCriteria'>
                  <v-icon>fas fa-lock</v-icon>
                </v-btn>
                <v-btn @click='onGiveUnrestrictedAccess(props.item)' class='ara-blue' icon 
                        title='Allow All Assets' 
                        v-if='props.item.hasCriteria'>
                  <v-icon>fas fa-lock-open</v-icon>
                </v-btn>
                <v-btn @click='onRevokeAccess(props.item)' class='ara-orange' icon 
                        title='Revoke Access'>
                  <v-icon>fas fa-times-circle</v-icon>
                </v-btn>
                <v-btn @click='onDeleteUser(props.item)' class='ara-orange' icon 
                        title='Delete User'>
                  <v-icon>fas fa-trash</v-icon>
                </v-btn>
              </td>
              <td>
                <v-menu bottom min-height='200px' min-width='200px'>
                  <template v-slot:activator="{ props }">
                    <v-text-field v-model='props.item.name' :readonly='!props.item.hasCriteria' style='width: 15em' type='text' class='text-center' />
                  </template>
                  <v-card>
                    <v-card-text>
                      <v-text-field v-model='props.item.name' label='Edit Name' single-line @click.stop />
                      <v-btn @click='updateName(props.item)'>Update</v-btn>
                    </v-card-text>
                  </v-card>
                </v-menu>
              </td>
              <td>
                <v-menu bottom min-height='200px' min-width='200px'>
                  <template v-slot:activator>
                    <v-text-field v-model='props.item.description' :readonly='!props.item.hasCriteria' style='width: 15em' type='text' class='text-center' />
                  </template>
                  <v-card>
                    <v-card-text>
                      <v-text-field v-model='props.item.description' label='Edit Description' single-line @click.stop />
                      <v-btn @click='updateDescription(props.item)'>Update</v-btn>
                    </v-card-text>
                  </v-card>
                </v-menu>
              </td>
            </template>
          </v-data-table>
        </div>
      </v-card>
    </v-flex>
    <CriteriaFilterEditorDialog :dialogData='criteriaFilterEditorDialogData' 
                                @submitCriteriaEditorDialogResult='onSubmitCriteria' />

    <Alert :dialogData='beforeDeleteAlertData' @submit='onSubmitDeleteUserResponse' />
  </v-row>
</template>

<script lang='ts' setup>
import Vue, { getCurrentInstance } from 'vue';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
import Alert from '@/shared/modals/Alert.vue';
import {
  CriterionFilterEditorDialogData,
  emptyCriterionFilterEditorDialogData,
} from '@/shared/models/modals/criterion-filter-editor-dialog-data';
import { User } from '@/shared/models/iAM/user';
import { itemsAreEqual } from '@/shared/utils/equals-utils';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { emptyUserCriteriaFilter, UserCriteriaFilter } from '@/shared/models/iAM/user-criteria-filter';

import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';

let store = useStore();
const instance = getCurrentInstance();

const emit = defineEmits(['submit'])

    function filteredUsersCriteriaFilter() {
    if (loading || !search) {
      return assignedUsersCriteriaFilter;
    }
    const lowerCaseSearch = search.toLowerCase();
    
    return assignedUsersCriteriaFilter.filter((item: { [s: string]: any; } | ArrayLike<unknown>) => {
      return Object.values(item).some(val => String(val).toLowerCase().includes(lowerCaseSearch));
    });
  }
  
  let stateUsers = ref<User[]>(store.state.userModule.users);
  let stateUsersCriteriaFilter = ref<UserCriteriaFilter[]>(store.state.userModule.usersCriteriaFilter);

  async function getAllUserCriteriaAction(payload?: any): Promise<any> {await store.dispatch('getAllUsers');}
  async function deleteUserAction(payload?: any): Promise<any> {await store.dispatch('deleteUser');}
  async function getAllUserCriteriaFilterAction(payload?: any): Promise<any> {await store.dispatch('getAllUserCriteriaFilter');}
  async function updateUserCriteriaFilterAction(payload?: any): Promise<any> {await store.dispatch('updateUserCriteriaFilter');}
  async function revokeUserCriteriaFilterAction(payload?: any): Promise<any> {await store.dispatch('revokeUserCriteriaFilter');}
  
  let beforeDeleteAlertData: AlertData = { ...emptyAlertData };
  let userCriteriaGridHeaders: object[] = [
    { text: 'User', align: 'left', sortable: true, value: 'userName' },
    { text: 'Criteria Filter', sortable: true, value: 'hasCriteria' },
    { text: '', align: 'right', sortable: false, value: 'actions' },
    { text: 'Name', align: 'Left', sortable: false, value: 'name' },
    { text: 'Description', align: 'Left', sortable: false, value: 'description' }
  ];

  let unassignedUsers: User[] = [];
  let assignedUsers: User[] = [];

  let assignedUsersCriteriaFilter: UserCriteriaFilter[]=[] ;
  let unassignedUsersCriteriaFilter: UserCriteriaFilter[] = [];
  
  let criteriaFilterEditorDialogData: CriterionFilterEditorDialogData = { ...emptyCriterionFilterEditorDialogData };
  let selectedUser: UserCriteriaFilter = { ...emptyUserCriteriaFilter };
  let uuidNIL: string = getBlankGuid();
  let nameValue: string = '';
  let descriptionValue: string = '';
  let sortKey: string = "userName";
  let sortOrder: string = "asc";
  let search: string = '';
  let loading: boolean = true;
  const $forceUpdate = inject('$forceUpdate') as any
  created();
  function created() {
  criteriaFilterEditorDialogData = { ...emptyCriterionFilterEditorDialogData };
  unassignedUsersCriteriaFilter = [];
  assignedUsersCriteriaFilter = [];

  getAllUserCriteriaAction().then(() => {
    loading = false;
  });

  watch(stateUsersCriteriaFilter, () => {
    assignedUsersCriteriaFilter.forEach((userCriteriaFilter: UserCriteriaFilter) => {
      if (userCriteriaFilter.hasCriteria) {
        nameValue = userCriteriaFilter.name;
        descriptionValue = userCriteriaFilter.description;
      }
    });
  });
}

watch(stateUsers,()=>onUserCriteriaChanged())
  function onUserCriteriaChanged() {
    unassignedUsers = stateUsers.value.filter((user: User) => !user.hasInventoryAccess);
    assignedUsers = stateUsers.value.filter((user: User) => user.hasInventoryAccess);

    unassignedUsersCriteriaFilter = [{ ...emptyUserCriteriaFilter }];
    unassignedUsers.forEach((value) => {
      var tempL: UserCriteriaFilter = {
        userId: value.id,
        userName: value.username,
        hasAccess: value.hasInventoryAccess,
        hasCriteria: false,
        name: '',
        description: '',
        criteria: '',
        criteriaId: '',
      };
      unassignedUsersCriteriaFilter.push(tempL);
    });
    unassignedUsersCriteriaFilter.shift(); // removes the 1st element, which is always bank in case
    $forceUpdate();
  }

  watch(stateUsersCriteriaFilter,()=>onUserCriteriaFilterChanged())
  function onUserCriteriaFilterChanged() {
    assignedUsersCriteriaFilter = stateUsersCriteriaFilter.value;
    $forceUpdate();
  }

  onMounted(()=>mounted())
  function mounted() {
    getAllUserCriteriaFilterAction();
  }

  function onEditCriteria(userFilter: UserCriteriaFilter) {
    selectedUser = userFilter;
    var currentUser = stateUsers.value.filter((user: User) => user.id == userFilter.userId)[0];

    criteriaFilterEditorDialogData = {
      showDialog: true,
      userId: currentUser.id,
      name: userFilter.name,
      criteriaId: userFilter.criteriaId,
      description: currentUser.description,
      criteria: userFilter.criteria,
      userName: currentUser.username,
      hasCriteria: userFilter.hasCriteria,
      hasAccess: userFilter.hasAccess,
    };
  }

  function onSort(sortBy: string, sortDesc: any) {
  sortKey = sortBy;
  sortOrder = sortDesc ? 'desc' : 'asc';
}

  function onSubmitCriteria(userCriteriaFilter: UserCriteriaFilter) {
    criteriaFilterEditorDialogData = { ...emptyCriterionFilterEditorDialogData };
    if (userCriteriaFilter != null) {
      if (userCriteriaFilter.criteriaId == '') {
        userCriteriaFilter.criteriaId = getNewGuid();
      }
      updateUserCriteriaFilterAction({ userCriteriaFilter: userCriteriaFilter });
    }

    selectedUser = { ...emptyUserCriteriaFilter };
  }

  function updateName(userCriteriaFilter: UserCriteriaFilter) {
  const updatedUserCriteriaFilter = {
    ...userCriteriaFilter,
    name: userCriteriaFilter.name
  }
  updateUserCriteriaFilterAction({ userCriteriaFilter: updatedUserCriteriaFilter })
}

  function updateDescription(userCriteriaFilter: UserCriteriaFilter) {
  const updatedUserCriteriaFilter = {
    ...userCriteriaFilter,
    description: userCriteriaFilter.description,
  };
  updateUserCriteriaFilterAction({ userCriteriaFilter: updatedUserCriteriaFilter });
}

  function onRevokeAccess(targetUser: UserCriteriaFilter) {
    const userCriteriaFilter = {
      ...targetUser,
      criteria: undefined,
      hasAccess: false,
      hasCriteria: false,
    };
    revokeUserCriteriaFilterAction({ userCriteriaId: userCriteriaFilter.criteriaId });
  }

  function onGiveUnrestrictedAccess(targetUser: UserCriteriaFilter) {
    const userFilterCriteria = {
      ...targetUser,
      criteria: '',
      hasAccess: true,
      hasCriteria: false,
    };
    if (userFilterCriteria.criteriaId == '') {
      userFilterCriteria.criteriaId = getNewGuid();
    }
    updateUserCriteriaFilterAction({ userCriteriaFilter: userFilterCriteria });
  }

  function onDeleteUser(user: UserCriteriaFilter) {
    selectedUser = user;

    beforeDeleteAlertData = {
      choice: true,
      heading: 'Delete User',
      message: `Are you sure you want to delete user ${selectedUser.userName}?`,
      showDialog: true,
    };
  }

  function onSubmitDeleteUserResponse(doDelete: boolean) {
    beforeDeleteAlertData = { ...emptyAlertData };

    if (doDelete && !itemsAreEqual(selectedUser, emptyUserCriteriaFilter)) {

      deleteUserAction({ userId: selectedUser.userId })
          .then(() => selectedUser = { ...emptyUserCriteriaFilter });
    }
  }

</script>

<style>
.unassigned-user-layout {
  margin: auto !important;
  width: 75%;
  text-align: center
}

.unassigned-user-layout-username-flex {
  display: flex;
  flex-direction: column;
  justify-content: center;
  font-size: 1.2em;
  padding-top: 0;
  padding-bottom: 0
}

.userCriteriaTableHeader {
  justify-content: center;
  font-size: 1.5em;
  font-weight: bold;
}
</style>
