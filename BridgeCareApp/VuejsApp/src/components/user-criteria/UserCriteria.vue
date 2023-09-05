<template>
  <v-layout column style='width: 90%; margin: auto'>
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
            <v-layout row class='unassigned-user-layout'>
              <v-flex class='unassigned-user-layout-username-flex' xs3>
                {{ user.userName }}
              </v-flex>
              <v-flex xs3>
                {{ user.description }}
              </v-flex>
              <v-flex style='padding: 0' xs4>
                <v-layout align-center>
                  <v-btn @click='onEditCriteria(user)' class='ara-blue-bg white--text' 
                          title='Give the user limited access to the bridge inventory'>
                    <v-icon size='1.5em' style='padding-right: 0.5em'>fas fa-edit</v-icon>
                    Assign Criteria Filter
                  </v-btn>
                </v-layout>
              </v-flex>
              <v-flex justify-center style='padding: 0' xs2>
                <v-layout align-center>
                  <v-btn @click='onDeleteUser(user)' class='ara-orange' icon title='Delete User'>
                    <v-icon>fas fa-trash</v-icon>
                  </v-btn>
                </v-layout>
              </v-flex>
            </v-layout>
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
                <v-icon v-if='sortKeyUserCriteria === header.value'>
                  {{ sortOrderUserCriteria === 'asc' ? 'arrow_upward' : 'arrow_downward' }}
                </v-icon>
              </span>
            </template>
            <template slot='items' 
            slot-scope='props'>
              <td style='width: 15%; font-size: 1.2em; padding-top: 0.4em'>{{ props.item.userName }}</td>
              <td style='width: 35%'>
                <v-layout align-center style='flex-wrap:nowrap; margin-left: 0; width: 100%'>
                  <v-menu bottom 
                      min-height='500px' min-width='500px' v-if='props.item.hasCriteria' style='width: 100%'>
                    <template slot='activator'>
                      <v-text-field class='sm-txt' :value='props.item.criteria' readonly style='width: 100%' type='text' />
                    </template>
                    <v-card>
                      <v-card-text>
                        <v-textarea :value='props.item.criteria' full-width no-resize outline 
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
                </v-layout>
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
                  <template v-slot:activator="{ on }">
                    <v-text-field v-model='props.item.name' :readonly='!props.item.hasCriteria' style='width: 15em' type='text' v-on='on' class='text-center' />
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
                  <template slot='activator'>
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
  </v-layout>
</template>

<script lang='ts'>
import Vue from 'vue';
import { Component, Watch } from 'vue-property-decorator';
import { Action, State } from 'vuex-class';
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
import CriterionFilterEditorDialog from '@/shared/modals/CriterionFilterEditorDialog.vue';

@Component({
  components: {
    CriteriaFilterEditorDialog: CriterionFilterEditorDialog, Alert,
  },
  computed: {
    filteredUsersCriteriaFilter() {
    if (this.loading || !this.search) {
      return this.assignedUsersCriteriaFilter;
    }
    const lowerCaseSearch = this.search.toLowerCase();
    return this.assignedUsersCriteriaFilter.filter((item: { [s: string]: unknown; } | ArrayLike<unknown>) => {
      return Object.values(item).some(val => String(val).toLowerCase().includes(lowerCaseSearch));
    });
  },
},
})

export default class UserCriteriaEditor extends Vue {
  @State(state => state.userModule.users) stateUsers: User[];
  @State(state => state.userModule.usersCriteriaFilter) stateUsersCriteriaFilter: UserCriteriaFilter[];

  @Action('getAllUsers') getAllUserCriteriaAction: any;
  @Action('deleteUser') deleteUserAction: any;

  @Action('getAllUserCriteriaFilter') getAllUserCriteriaFilterAction: any;
  @Action('updateUserCriteriaFilter') updateUserCriteriaFilterAction: any;
  @Action('revokeUserCriteriaFilter') revokeUserCriteriaFilterAction: any;

  beforeDeleteAlertData: AlertData = { ...emptyAlertData };
  userCriteriaGridHeaders: object[] = [
    { text: 'User', align: 'left', sortable: true, value: 'userName' },
    { text: 'Criteria Filter', sortable: true, value: 'hasCriteria' },
    { text: '', align: 'right', sortable: false, value: 'actions' },
    { text: 'Name', align: 'Left', sortable: false, value: 'name' },
    { text: 'Description', align: 'Left', sortable: false, value: 'description' }
  ];

  unassignedUsers: User[] = [];
  assignedUsers: User[] = [];

  assignedUsersCriteriaFilter: UserCriteriaFilter[] = [];
  unassignedUsersCriteriaFilter: UserCriteriaFilter[] = [];
  
  criteriaFilterEditorDialogData: CriterionFilterEditorDialogData = { ...emptyCriterionFilterEditorDialogData };
  selectedUser: UserCriteriaFilter = { ...emptyUserCriteriaFilter };
  uuidNIL: string = getBlankGuid();
  nameValue: string = '';
  descriptionValue: string = '';
  sortKey: string = "userName";
  sortOrder: string = "asc";
  search: string = '';
  loading: boolean = true;

  created() {
  this.criteriaFilterEditorDialogData = { ...emptyCriterionFilterEditorDialogData };
  this.unassignedUsersCriteriaFilter = [];
  this.assignedUsersCriteriaFilter = [];

  this.getAllUserCriteriaAction().then(() => {
    this.loading = false;
  });

  this.$watch('stateUsersCriteriaFilter', () => {
    this.assignedUsersCriteriaFilter.forEach((userCriteriaFilter: UserCriteriaFilter) => {
      if (userCriteriaFilter.hasCriteria) {
        this.nameValue = userCriteriaFilter.name;
        this.descriptionValue = userCriteriaFilter.description;
      }
    });
  });
}

@Watch('stateUsers')
  onUserCriteriaChanged() {
    this.unassignedUsers = this.stateUsers.filter((user: User) => !user.hasInventoryAccess);
    this.assignedUsers = this.stateUsers.filter((user: User) => user.hasInventoryAccess);

    this.unassignedUsersCriteriaFilter = [{ ...emptyUserCriteriaFilter }];
    this.unassignedUsers.forEach((value) => {
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
      this.unassignedUsersCriteriaFilter.push(tempL);
    });
    this.unassignedUsersCriteriaFilter.shift(); // removes the 1st element, which is always bank in this case
    this.$forceUpdate();
  }

  @Watch('stateUsersCriteriaFilter')
  onUserCriteriaFilterChanged() {
    this.assignedUsersCriteriaFilter = this.stateUsersCriteriaFilter;
    this.$forceUpdate();
  }

  mounted() {
    this.getAllUserCriteriaFilterAction();
  }

  onEditCriteria(userFilter: UserCriteriaFilter) {
    this.selectedUser = userFilter;
    var currentUser = this.stateUsers.filter((user: User) => user.id == userFilter.userId)[0];

    this.criteriaFilterEditorDialogData = {
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

  onSort(sortBy: string, sortDesc: any) {
  this.sortKey = sortBy;
  this.sortOrder = sortDesc ? 'desc' : 'asc';
}

  onSubmitCriteria(userCriteriaFilter: UserCriteriaFilter) {
    this.criteriaFilterEditorDialogData = { ...emptyCriterionFilterEditorDialogData };
    if (userCriteriaFilter != null) {
      if (userCriteriaFilter.criteriaId == '') {
        userCriteriaFilter.criteriaId = getNewGuid();
      }
      this.updateUserCriteriaFilterAction({ userCriteriaFilter: userCriteriaFilter });
    }

    this.selectedUser = { ...emptyUserCriteriaFilter };
  }

  updateName(userCriteriaFilter: UserCriteriaFilter) {
  const updatedUserCriteriaFilter = {
    ...userCriteriaFilter,
    name: userCriteriaFilter.name
  }
  this.updateUserCriteriaFilterAction({ userCriteriaFilter: updatedUserCriteriaFilter })
}

  updateDescription(userCriteriaFilter: UserCriteriaFilter) {
  const updatedUserCriteriaFilter = {
    ...userCriteriaFilter,
    description: userCriteriaFilter.description,
  };
  this.updateUserCriteriaFilterAction({ userCriteriaFilter: updatedUserCriteriaFilter });
}

  onRevokeAccess(targetUser: UserCriteriaFilter) {
    const userCriteriaFilter = {
      ...targetUser,
      criteria: undefined,
      hasAccess: false,
      hasCriteria: false,
    };
    this.revokeUserCriteriaFilterAction({ userCriteriaId: userCriteriaFilter.criteriaId });
  }

  onGiveUnrestrictedAccess(targetUser: UserCriteriaFilter) {
    const userFilterCriteria = {
      ...targetUser,
      criteria: '',
      hasAccess: true,
      hasCriteria: false,
    };
    if (userFilterCriteria.criteriaId == '') {
      userFilterCriteria.criteriaId = getNewGuid();
    }
    this.updateUserCriteriaFilterAction({ userCriteriaFilter: userFilterCriteria });
  }

  onDeleteUser(user: UserCriteriaFilter) {
    this.selectedUser = user;

    this.beforeDeleteAlertData = {
      choice: true,
      heading: 'Delete User',
      message: `Are you sure you want to delete user ${this.selectedUser.userName}?`,
      showDialog: true,
    };
  }

  onSubmitDeleteUserResponse(doDelete: boolean) {
    this.beforeDeleteAlertData = { ...emptyAlertData };

    if (doDelete && !itemsAreEqual(this.selectedUser, emptyUserCriteriaFilter)) {

      this.deleteUserAction({ userId: this.selectedUser.userId })
          .then(() => this.selectedUser = { ...emptyUserCriteriaFilter });
    }
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
