<template>
  <v-row column style='width: 90%; margin: auto'>
    
    <v-col cols = "12">
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
              <v-col id="UserCriteria-unassignedUser-flex" class='unassigned-user-layout-username-flex' cols="2">
                {{ user.userName }}
              </v-col>
              <v-col cols = "3">
                {{ user.description }}
              </v-col>
              <v-col style='padding: 0' cols="3">
                <v-row align-center>               
                <v-btn id="UserCriteria-assignUnassignedUserCriteria-btn" @click='onGiveUnrestrictedAccess(user)'  class='assetFox-blue-bg text-white' 
                          title='Allow All Assets'>
                          <v-icon size='1.5em' style='padding-right: 0.5em'>fas fa-lock-open</v-icon>                   
                    Allow All Assets
                </v-btn>
                </v-row>
              </v-col>
              <v-col style='padding: 0' cols="3">
                <v-row align-center>
                  <v-btn id="UserCriteria-assignUnassignedUserCriteria-btn" @click='onEditCriteria(user)'  class='assetFox-blue-bg text-white' 
                          title='Give the user limited access to the asset inventory'>
                    <v-icon size='1.5em' style='padding-right: 0.5em'>fas fa-edit</v-icon>
                    Assign Criteria Filter
                  </v-btn>
                </v-row>
              </v-col>
              <v-col justify-center style='padding: 0' xs2>
                <v-row align-center>
                  <v-btn @click='onDeactivateUser(user)' class='assetFox-orange' flat title='Delete User'>
                    <v-icon size='1.5em'>fas fa-trash</v-icon>
                  </v-btn>
                </v-row>
              </v-col>
            </v-row>
            <v-divider style='margin: 0' />
          </div>
        </div>
        <v-card-title class='userCriteriaTableHeader'>
          Inventory Access Criteria
        </v-card-title>
        <div>
          <v-text-field
            id="UserCriteria-search-textfield"
            v-model='search'
            append-icon='mdi-magnify'
            label='Search'
            single-line
            hide-details
          ></v-text-field>
        </div>
        <div>
          <v-data-table id="UserCriteria-users-datatable"
            :header='userCriteriaGridHeaders'
            :items='filteredUsersCriteriaFilter'
            :rows-per-page-items=[5,10,25]
            item-key="userId"
            class='elevation-1'
            sort-asc-icon="custom:GhdTableSortAscSvg"
            sort-desc-icon="custom:GhdTableSortDescSvg"
          >
          <template v-slot:headers="props">
              <tr>
                  <th
                    v-for="header in userCriteriaGridHeaders"
                    :key="header.title"
                    @click="onActiveSort(header.key)"
                  >
                    <span style='cursor: pointer'>
                      {{ header.title }}
                      <v-icon v-if='sortKey === header.key'>
                        {{ sortOrder === 'asc' ? 'arrow_upward' : 'arrow_downward' }}
                      </v-icon>
                    </span>
                  </th>
              </tr>
            </template>
            <template v-slot:item="{item}" >
              <tr>
              <td style='width: 15%; font-size: 1.2em; padding-top: 0.4em'>{{ item.userName }}</td>
              <td style='width: 35%'>
                <v-row align-center style='flex-wrap:nowrap; margin-left: 0; width: 100%'>
                  <v-menu bottom 
                      min-height='500px' min-width='500px' v-if='item.hasCriteria' style='width: 100%'>
                    <template v-slot:activator="{props}" >
                      <v-text-field variant="underlined"  v-bind="props" id="UserCriteria-userCriteria-textfield" class='sm-txt' :model-value='item.criteria' readonly style='width: 100%' type='text' />
                    </template>
                    <v-card>
                      <v-card-text>
                        <v-textarea :model-value='item.criteria' full-width no-resize variant="underlined"
                        readonly 
                        rows='5'>
                      </v-textarea>
                      </v-card-text>
                    </v-card>
                  </v-menu>
                  <div id="UserCriteria-userHasAllAssets-div" style='font-size: 1.2em; font-weight: bold; padding-top: 0.4em; padding-right: 1em' 
                      v-if='!item.hasCriteria'>
                    All Assets
                  </div>
                  <v-btn id="UserCriteria-editUserCriteria-btn" @click='onEditCriteria(item)' class='edit-icon' flat 
                          title='Edit Criteria' 
                          v-if='item.hasCriteria'>
                    <v-icon>fas fa-edit</v-icon>
                  </v-btn>
                </v-row>
              </td>
              <td class='d-flex'>
                <v-btn id="UserCriteria-restrictUserAccess-btn" @click='onEditCriteria(item)'  class='assetFox-blue' flat 
                        title='Restrict Access with Criteria Filter' 
                        v-if='!item.hasCriteria'>
                  <v-icon>fas fa-lock</v-icon>
                </v-btn>
                <v-btn id="UserCriteria-allowUserAllAccess-btn" @click='onGiveUnrestrictedAccess(item)' class='assetFox-blue' flat 
                        title='Allow All Assets' 
                        v-if='item.hasCriteria'>
                  <v-icon>fas fa-lock-open</v-icon>
                </v-btn>
                <v-btn id="UserCriteria-revokeUserAccess-btn" @click='onRevokeAccess(item)' class='assetFox-orange' flat 
                        title='Revoke Access'>
                  <v-icon>fas fa-times-circle</v-icon>
                </v-btn>
                <v-btn id="UserCriteria-deactivateUser-btn" @click='onDeactivateUser(item)' class='assetFox-orange' flat 
                        title='Deactivate User'>
                  <v-icon>fas fa-trash</v-icon>
                </v-btn>
              </td>
              <td>
                <v-menu bottom min-height='200px' min-width='200px'>
                  <template v-slot:activator="{ props }" >
                    <v-text-field v-bind="props" id="UserCriteria-userName-textfield" v-model='item.name' variant="underlined" :readonly='!item.hasCriteria' style='width: 15em' type='text' class='text-center' />
                  </template>
                  <v-card style="top: -60px;" >
                    <v-card-text>
                      <v-text-field id="UserCriteria-editUserName-textfield" variant="underlined" v-model='tempName' label='Edit Name' single-line @click.stop />
                      <v-btn id="UserCriteria-updateUserName-btn" @click='updateName(item)' class="mr-2">Update</v-btn>
                      <v-btn id="UserCriteria-cancelUserName-btn" @click='cancelNameEdit'>Cancel</v-btn>
                    </v-card-text>
                  </v-card>
                </v-menu>
              </td>
              <td>
                <v-menu bottom min-height='200px' min-width='200px'>
                  <template v-slot:activator="{ props }">
                    <v-text-field v-bind="props" id="UserCriteria-userDescription-textfield" variant="underlined" v-model='item.description' :readonly='!item.hasCriteria' style='width: 15em' type='text' class='text-center' />
                  </template>
                  <v-card style="top: -60px;">
                    <v-card-text>
                      <v-text-field id="UserCriteria-editUserDescription-textfield" variant="underlined" v-model='tempDescription' label='Edit Description' single-line @click.stop />
                      <v-btn id="UserCriteria-updateUserDescription-btn" @click='updateDescription(item)' class="mr-2">Update</v-btn>
                      <v-btn id="UserCriteria-cancelUpdateDescription-btn" @click='cancelDescriptionEdit'>Cancel</v-btn>
                    </v-card-text>
                  </v-card>
                </v-menu>
              </td>
            </tr>
            </template>
          </v-data-table>
        </div>
      </v-card>
    </v-col>
    <v-col cols = "12">
      <v-card class='test1'>
        <v-card-title class='userCriteriaTableHeader'>
          Inventory Deactivated Users
        </v-card-title>
        <div>
          <v-text-field
            id="UserCriteria-search-textfield"
            v-model='inactiveSearch'
            append-icon='mdi-magnify'
            label='Search'
            single-line
            hide-details
          ></v-text-field>
        </div>
        <div>
          <v-data-table id="UserCriteria-users-datatable"
            :header='userCriteriaGridHeaders'
            :items='inactiveFilteredUsersCriteriaFilter'
            :rows-per-page-items=[5,10,25]
            item-key="userId"
            class='elevation-1'
            sort-asc-icon="custom:GhdTableSortAscSvg"
            sort-desc-icon="custom:GhdTableSortDescSvg"
          >
          <template v-slot:headers="props">
              <tr>
                  <th
                    v-for="header in userCriteriaGridHeaders"
                    :key="header.title"
                    @click="onInactiveSort(header.key)"
                  >
                    <span style='cursor: pointer'>
                      {{ header.title }}
                      <v-icon v-if='sortKey === header.key'>
                        {{ sortOrder === 'asc' ? 'arrow_upward' : 'arrow_downward' }}
                      </v-icon>
                    </span>
                  </th>
              </tr>
            </template>
            <template v-slot:item="{item}" >
              <tr>
              <td style='width: 15%; font-size: 1.2em; padding-top: 0.4em'>{{ item.userName }}</td>
              <td style='width: 35%'>
                <v-row align-center style='flex-wrap:nowrap; margin-left: 0; width: 100%'>
                  <v-menu bottom 
                      min-height='500px' min-width='500px' v-if='item.hasCriteria' style='width: 100%'>
                    <template v-slot:activator="{props}" >
                      <v-text-field variant="underlined"  v-bind="props" id="UserCriteria-userCriteria-textfield" class='sm-txt' :model-value='item.criteria' readonly style='width: 100%' type='text' />
                    </template>
                    <v-card>
                      <v-card-text>
                        <v-textarea :model-value='item.criteria' full-width no-resize variant="underlined"
                        readonly 
                        rows='5'>
                      </v-textarea>
                      </v-card-text>
                    </v-card>
                  </v-menu>
                  <div id="UserCriteria-userHasAllAssets-div" style='font-size: 1.2em; font-weight: bold; padding-top: 0.4em; padding-right: 1em' 
                      v-if='!item.hasCriteria'>
                    All Assets
                  </div>
                  <v-btn id="UserCriteria-editUserCriteria-btn" @click='onEditCriteria(item)' class='edit-icon' flat 
                          title='Edit Criteria' 
                          v-if='item.hasCriteria'>
                    <v-icon>fas fa-edit</v-icon>
                  </v-btn>
                </v-row>
              </td>
              <td class='d-flex'>
                <v-btn id="UserCriteria-restrictUserAccess-btn" @click='onEditCriteria(item)'  class='assetFox-blue' flat 
                        title='Restrict Access with Criteria Filter' 
                        v-if='!item.hasCriteria'>
                  <v-icon>fas fa-lock</v-icon>
                </v-btn>
                <v-btn id="UserCriteria-allowUserAllAccess-btn" @click='onGiveUnrestrictedAccess(item)' class='assetFox-blue' flat 
                        title='Allow All Assets' 
                        v-if='item.hasCriteria'>
                  <v-icon>fas fa-lock-open</v-icon>
                </v-btn>
                <v-btn id="UserCriteria-revokeUserAccess-btn" @click='onReactivateUser(item)' class='assetFox-orange' flat 
                        title='Reactivate User'>
                  <v-icon>fas fa-check-circle</v-icon>
                </v-btn>
              </td>
              <td>
                <v-menu bottom min-height='200px' min-width='200px'>
                  <template v-slot:activator="{ props }" >
                    <v-text-field v-bind="props" id="UserCriteria-userName-textfield" v-model='item.name' variant="underlined" :readonly='!item.hasCriteria' style='width: 15em' type='text' class='text-center' />
                  </template>
                  <v-card style="top: -60px;" >
                    <v-card-text>
                      <v-text-field id="UserCriteria-editUserName-textfield" variant="underlined" v-model='tempName' label='Edit Name' single-line @click.stop />
                      <v-btn id="UserCriteria-updateUserName-btn" @click='updateName(item)' class="mr-2">Update</v-btn>
                      <v-btn id="UserCriteria-cancelUserName-btn" @click='cancelNameEdit'>Cancel</v-btn>
                    </v-card-text>
                  </v-card>
                </v-menu>
              </td>
              <td>
                <v-menu bottom min-height='200px' min-width='200px'>
                  <template v-slot:activator="{ props }">
                    <v-text-field v-bind="props" id="UserCriteria-userDescription-textfield" variant="underlined" v-model='item.description' :readonly='!item.hasCriteria' style='width: 15em' type='text' class='text-center' />
                  </template>
                  <v-card style="top: -60px;">
                    <v-card-text>
                      <v-text-field id="UserCriteria-editUserDescription-textfield" variant="underlined" v-model='tempDescription' label='Edit Description' single-line @click.stop />
                      <v-btn id="UserCriteria-updateUserDescription-btn" @click='updateDescription(item)' class="mr-2">Update</v-btn>
                      <v-btn id="UserCriteria-cancelUpdateDescription-btn" @click='cancelDescriptionEdit'>Cancel</v-btn>
                    </v-card-text>
                  </v-card>
                </v-menu>
              </td>
            </tr>
            </template>
          </v-data-table>
        </div>
      </v-card>
    </v-col>

    <EditCriteriaDialog :dialogData='criteriaFilterEditorDialogData' 
                                @submitCriteriaEditorDialogResult='onSubmitCriteria' />

    <Alert :dialogData='beforeDeactivateAlertData' @submit='onSubmitDeactivateUserResponse' />
    <Alert :dialogData='beforeReactivateAlertData' @submit='onSubmitReactivateUserResponse' />
  </v-row>
</template>

<script lang='ts' setup>
import Vue, { computed, getCurrentInstance, nextTick } from 'vue';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
import Alert from '@/shared/modals/Alert.vue';
import EditCriteriaDialog from '@/shared/modals/CriterionFilterEditorDialog.vue';
import{CriterionFilterEditorDialogData,
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
let edit = ref<boolean>(false)
const emit = defineEmits(['submit'])

let search = ref('');
let inactiveSearch = ref('');
const tempDeactivateUser = ref<any>([]);
const tempDeactivateUserFilter = ref<any>([]);
const tempReactivateUserFilter = ref<any>([]);
const tempReactivateUser = ref<any>([]);

const filteredUsersCriteriaFilter = computed(() => {
  if (loading || !search.value || search.value.trim() === '') {
    return assignedUsersCriteriaFilter.value;
  }

  const lowerCaseSearch = search.value.toLowerCase();

  return assignedUsersCriteriaFilter.value.filter((item: { [s: string]: any; } | ArrayLike<unknown>) => {
    return Object.values(item).some(val => String(val).toLowerCase().includes(lowerCaseSearch));
  });
});

const inactiveFilteredUsersCriteriaFilter = computed(() => {
  if (loading || !inactiveSearch.value || inactiveSearch.value.trim() === '') {
    return inactiveUsersCriteriaFilter.value;
  }

  const lowerInactiveCaseSearch = inactiveSearch.value.toLowerCase();

  return inactiveUsersCriteriaFilter.value.filter((item: { [s: string]: any; } | ArrayLike<unknown>) => {
    return Object.values(item).some(val => String(val).toLowerCase().includes(lowerInactiveCaseSearch));
  });
});
  
  let stateUsers = computed<User[]>(()=>store.state.userModule.users);
  let stateUsersCriteriaFilter = computed<UserCriteriaFilter[]>(()=>store.state.userModule.usersCriteriaFilter);

  async function getAllUserCriteriaAction(payload?: any): Promise<any> {await store.dispatch('getAllUsers',payload);}
  async function deactivateUserAction(payload?: any): Promise<any> {await store.dispatch('deactivateUser',payload);}
  async function reactivateUserAction(payload?: any): Promise<any> {await store.dispatch('reactivateUser',payload);}
  async function getAllUserCriteriaFilterAction(payload?: any): Promise<any> {await store.dispatch('getAllUserCriteriaFilter',payload);}
  async function updateUserCriteriaFilterAction(payload?: any): Promise<any> {await store.dispatch('updateUserCriteriaFilter',payload);}
  async function revokeUserCriteriaFilterAction(payload?: any): Promise<any> {await store.dispatch('revokeUserCriteriaFilter',payload);}
  
  let beforeDeactivateAlertData=ref<AlertData> (emptyAlertData);
  let beforeReactivateAlertData=ref<AlertData> (emptyAlertData);
  let userCriteriaGridHeaders: any = [
    { title: 'User', align: 'left', sortable: true, key: 'userName' },
    { title: 'Criteria Filter', sortable: true, key: 'criteria' },
    { title: '', align: 'right', sortable: false, key: 'actions' },
    { title: 'Name', align: 'Left', sortable: true, key: 'name' },
    { title: 'Description', align: 'Left', sortable: true, key: 'description' }
  ];

  let unassignedUsers: User[]=[];
  let assignedUsers:User[] =[];
  let inactiveUsers:User[] =[];

  const assignedUsersCriteriaFilter= ref<any>([]) ;
  const unassignedUsersCriteriaFilter=ref <UserCriteriaFilter[]> ([]);
  const inactiveUsersCriteriaFilter= ref<any>([]) ;
  
  const criteriaFilterEditorDialogData= ref<any>  (emptyCriterionFilterEditorDialogData);
  let selectedUser: UserCriteriaFilter = { ...emptyUserCriteriaFilter };
  let uuidNIL: string = getBlankGuid();
  let nameValue: string = '';
  let tempName = ref('');
  let descriptionValue: string = '';
  let tempDescription = ref('');
  let sortKey: string = "userName";
  let sortOrder: string = "asc";
  // let search: string = '';
  let loading: boolean = true;
  const $forceUpdate = inject('$forceUpdate') as any
  created();
  function created() {
  // Create the variables for unassigned. assigned. and inactive users  
  unassignedUsersCriteriaFilter.value = [];
  assignedUsersCriteriaFilter.value = [];
  inactiveUsersCriteriaFilter.value = [];

  getAllUserCriteriaAction().then(() => {
    loading = false;
  });

  watch(stateUsersCriteriaFilter, () => {
    stateUsersCriteriaFilter.value.forEach((userCriteriafilter: UserCriteriaFilter)=>{
      let tempUserCriteria = userCriteriafilter;
      if(tempUserCriteria.hasCriteria)
      {
          nameValue = tempUserCriteria.name;
          descriptionValue = tempUserCriteria.description;
          assignedUsersCriteriaFilter.value.push(tempUserCriteria);
      }

      // If the user is in the list of inactive users, add it to the CriteriaFilter inactive list
      if (inactiveUsers.some(user => user.username === tempUserCriteria.userName)) {
          inactiveUsersCriteriaFilter.value.push(tempUserCriteria);
      }
    })
  });
}
watch(search, () => {
  if (loading || !search.value || search.value.trim() === '') {
    return assignedUsersCriteriaFilter.value;
  }

  const lowerCaseSearch = search.value.toLowerCase();

  return assignedUsersCriteriaFilter.value.filter((item: { [s: string]: any; } | ArrayLike<unknown>) => {
    return Object.values(item).some(val => String(val).toLowerCase().includes(lowerCaseSearch));
  });
});

watch(inactiveSearch, () => {
  // Sort for search
  if (loading || !inactiveSearch.value || inactiveSearch.value.trim() === '') {
    return inactiveUsersCriteriaFilter.value;
  }

  const lowerInactiveCaseSearch = inactiveSearch.value.toLowerCase();

  const filteredData = inactiveUsersCriteriaFilter.value.filter((item: { [s: string]: any }) => {
    return Object.values(item).some(val => String(val).toLowerCase().includes(lowerInactiveCaseSearch));
  });

  // Return the filtered results
  return filteredData;
});


watch(stateUsers,()=>onUserCriteriaChanged())
  function onUserCriteriaChanged() {
    // Filter and Assign unassgined users, assigned users, and inactive users
    unassignedUsers = stateUsers.value.filter((user: User) => !user.hasInventoryAccess && user.activeStatus);
    assignedUsers = stateUsers.value.filter((user: User) => user.hasInventoryAccess && user.activeStatus);
    inactiveUsers = stateUsers.value.filter((user: User) => !user.activeStatus);

    unassignedUsersCriteriaFilter.value = [{ ...emptyUserCriteriaFilter }];
    unassignedUsers.forEach((value) => {
      var tempL: UserCriteriaFilter = {
        userId: value.id,
        userName: value.username,
        hasAccess: value.hasInventoryAccess,
        hasCriteria: false,
        activeStatus: false,
        name: '',
        description: value.description,
        criteria: '',
        criteriaId: '',
      };
      unassignedUsersCriteriaFilter.value.push(tempL);
    });
    unassignedUsersCriteriaFilter.value.shift(); // removes the 1st element, which is always bank in case
  }

  // First watch: stateUsers
  watch(stateUsers, () => {
    onUserCriteriaChanged();
    // Ensure the second watch runs only after the first watch completes
    nextTick(() => {
      // Second watch: stateUsersCriteriaFilter
      watch(stateUsersCriteriaFilter, () => {
        onUserCriteriaFilterChanged();
      }, { immediate: true });
    });
  });

  watch(stateUsersCriteriaFilter, () => onUserCriteriaFilterChanged());
    function onUserCriteriaFilterChanged() {
    // Create a set of inactive usernames
    const inactiveUsernamesSet = new Set(inactiveUsers.map(user => user.username));

    // Filter and assign active users
    assignedUsersCriteriaFilter.value = stateUsersCriteriaFilter.value.filter(userCriteria => {
      return !inactiveUsernamesSet.has(userCriteria.userName);
    });

    // Filter and assign inactive users
    inactiveUsersCriteriaFilter.value = stateUsersCriteriaFilter.value.filter(userCriteria => {
      return inactiveUsernamesSet.has(userCriteria.userName);
    });
  }

  onMounted(()=>mounted())
  function mounted() {
    getAllUserCriteriaFilterAction();
  }

  function onEditCriteria(userFilter: UserCriteriaFilter) {
    selectedUser = userFilter;
    tempDescription.value = userFilter.description;
    tempName.value = userFilter.name;
    var currentUser = stateUsers.value.filter((user: User) => user.id == userFilter.userId)[0];

    criteriaFilterEditorDialogData.value = {
      showDialog: true,
      userId: currentUser.id,
      name: currentUser.name,
      criteriaId: userFilter.criteriaId,
      description: currentUser.description,
      criteria: userFilter.criteria,
      userName: currentUser.username,
      hasCriteria: userFilter.hasCriteria,
      hasAccess: userFilter.hasAccess,
    };
  }

  function onActiveSort(sortBy: string) {
    sortKey = sortBy;
    sortOrder = sortOrder === 'asc' ? 'desc' : 'asc';
    filteredUsersCriteriaFilter.value.sort((a: { [x: string]: number; }, b: { [x: string]: number; }) => {
      if (a[sortKey] < b[sortKey]) return sortOrder === 'asc' ? -1 : 1;
      if (a[sortKey] > b[sortKey]) return sortOrder === 'asc' ? 1 : -1;
      return 0;
    });
  }

  function onInactiveSort(sortBy: string) {
    sortKey = sortBy;
    sortOrder = sortOrder === 'asc' ? 'desc' : 'asc';
    inactiveFilteredUsersCriteriaFilter.value.sort((a: { [x: string]: number; }, b: { [x: string]: number; }) => {
      if (a[sortKey] < b[sortKey]) return sortOrder === 'asc' ? -1 : 1;
      if (a[sortKey] > b[sortKey]) return sortOrder === 'asc' ? 1 : -1;
      return 0;
    });
  }

  function onSubmitCriteria(userCriteriaFilter: UserCriteriaFilter) {
    criteriaFilterEditorDialogData.value = { ...emptyCriterionFilterEditorDialogData };
    if (userCriteriaFilter != null) {
      if (userCriteriaFilter.criteriaId == '') {
        userCriteriaFilter.criteriaId = getNewGuid();
      }
      updateUserCriteriaFilterAction({ userCriteriaFilter: userCriteriaFilter });
      
    }
    getAllUserCriteriaAction().then(() => {
    loading = false;
  });
    selectedUser = { ...emptyUserCriteriaFilter };
  }

  function updateName(userCriteriaFilter: UserCriteriaFilter) {
    userCriteriaFilter.name = tempName.value;
    const updatedUserCriteriaFilter = {
      ...userCriteriaFilter,
      name: userCriteriaFilter.name
    }
    updateUserCriteriaFilterAction({ userCriteriaFilter: updatedUserCriteriaFilter })
  }

  function cancelNameEdit() {
    tempName.value = '';
  }

  function updateDescription(userCriteriaFilter: UserCriteriaFilter) {
    userCriteriaFilter.description = tempDescription.value
    const updatedUserCriteriaFilter = {
      ...userCriteriaFilter,
      description: userCriteriaFilter.description,
    };
    updateUserCriteriaFilterAction({ userCriteriaFilter: updatedUserCriteriaFilter });
  }

  function cancelDescriptionEdit() {
    tempDescription.value = '';
  }

  function onRevokeAccess(targetUser: UserCriteriaFilter) {
    const userCriteriaFilter = {
      ...targetUser,
      criteria: undefined,
      hasAccess: false,
      hasCriteria: false,
      descriptionValue: targetUser.description,
      nameValue:targetUser.name
    };
    revokeUserCriteriaFilterAction({ userCriteriaId: userCriteriaFilter.criteriaId });
  }

  function onGiveUnrestrictedAccess(targetUser: UserCriteriaFilter) {
    const userFilterCriteria = {
      ...targetUser,
      criteria: '',
      hasAccess: true,
      hasCriteria: false,
      nameValue:targetUser.name,
      descriptionValue:targetUser.description
    };
    if (userFilterCriteria.criteriaId == '') {
      userFilterCriteria.criteriaId = getNewGuid();
    }
    updateUserCriteriaFilterAction({ userCriteriaFilter: userFilterCriteria });
  }

  function onDeactivateUser(user: UserCriteriaFilter) {
    selectedUser = user;

    beforeDeactivateAlertData.value = {
      choice: true,
      heading: 'Deactivate User',
      message: `Are you sure you want to deactivate user ${selectedUser.userName}?`,
      showDialog: true,
    };
  }

  async function onSubmitDeactivateUserResponse(doDelete: boolean) {
  beforeDeactivateAlertData.value = { ...emptyAlertData };
  tempDeactivateUserFilter.value = assignedUsersCriteriaFilter.value;
  if (doDelete && !itemsAreEqual(selectedUser, emptyUserCriteriaFilter)) {
    // Set tempDeactivateUser equal to the current value of inactiveUsersCriteriaFilter
    tempDeactivateUser.value = inactiveUsersCriteriaFilter.value;
    await deactivateUserAction({ userId: selectedUser.userId });

    const inactiveUsernamesSet = new Set(inactiveUsers.map(user => user.username));
      // Filter and assign active users
        assignedUsersCriteriaFilter.value = assignedUsersCriteriaFilter.value.filter((userCriteria: { userName: string; }) => {
      return !inactiveUsernamesSet.has(userCriteria.userName);
    });

    // Set inactiveUsersCriteriaFilter equal to the value of tempDeactivateUser
    inactiveUsersCriteriaFilter.value = tempDeactivateUser.value;

    // Remove duplicates based on username
    const uniqueUsers = new Set();
    inactiveUsersCriteriaFilter.value = inactiveUsersCriteriaFilter.value.filter((userCriteria: { userName: unknown; }) => {
      if (!uniqueUsers.has(userCriteria.userName)) {
        uniqueUsers.add(userCriteria.userName);
        return true;
      }
      return false;
    });

      // Add to the list of inactive users
    inactiveUsersCriteriaFilter.value.push(selectedUser);
    selectedUser = { ...emptyUserCriteriaFilter };
  }
  beforeDeactivateAlertData.value.showDialog = false;
}

  function onReactivateUser(user: UserCriteriaFilter) {
    selectedUser = user;

    beforeReactivateAlertData.value = {
      choice: true,
      heading: 'Reactivate User',
      message: `Are you sure you want to reactivate user ${selectedUser.userName}?`,
      showDialog: true,
    };
  }
async function onSubmitReactivateUserResponse(doReactivate: boolean) {
  beforeReactivateAlertData.value = { ...emptyAlertData };

  tempReactivateUserFilter.value = inactiveUsersCriteriaFilter.value;
  if (doReactivate && !itemsAreEqual(selectedUser, emptyUserCriteriaFilter)) {
    // Set tempReactivateUser equal to the current value of assignedUsersCriteriaFilter
    tempReactivateUser.value = assignedUsersCriteriaFilter.value;
    await reactivateUserAction({ userId: selectedUser.userId })
      .then(() => {

        inactiveUsersCriteriaFilter.value = tempReactivateUserFilter.value;

        const inactiveUsernamesSet = new Set(inactiveUsers.map(user => user.username));

        // Filter and assign inactive users
          inactiveUsersCriteriaFilter.value = inactiveUsersCriteriaFilter.value.filter((userCriteria: { userName: string; }) => {
            return inactiveUsernamesSet.has(userCriteria.userName);
        });
        // Set assignedUsersCriteriaFilter equal to the value of tempReactivateUser
        assignedUsersCriteriaFilter.value = tempReactivateUser.value;
        
        // Remove Duplicates
      const uniqueAssignedUsers = new Set();
      inactiveUsersCriteriaFilter.value = inactiveUsersCriteriaFilter.value.filter((userCriteria: { userName: unknown; }) => {
        if (!uniqueAssignedUsers.has(userCriteria.userName)) {
          uniqueAssignedUsers.add(userCriteria.userName);
          return true;
        }
        return false;
      });

        // Add to the list of inactive users
        assignedUsersCriteriaFilter.value.unshift(selectedUser);

        // Reset selectedUser
        selectedUser = { ...emptyUserCriteriaFilter };
      });
  }
  
  beforeReactivateAlertData.value.showDialog = false;
}
</script>

<style>
.unassigned-user-layout {
  margin: auto !important;
  width: 75%;
  text-align: center
}
.d-flex{
  margin-top: 29px;
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
