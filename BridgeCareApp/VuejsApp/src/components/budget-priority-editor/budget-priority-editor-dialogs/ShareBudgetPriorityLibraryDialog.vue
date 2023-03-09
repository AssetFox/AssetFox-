<template>
  <v-dialog max-width="500px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title>
        <v-layout justify-center>
          <h3>Budget Priority Library Sharing</h3>
        </v-layout>
          <v-btn @click="onSubmit(false)" flat class="ghd-close-button">
            X
          </v-btn>
      </v-card-title>
      <v-card-text>
        <v-data-table :headers="budgetPriorityLibraryUserGridHeaders"
                      :items="budgetPriorityLibraryUserGridRows"
                      sort-icon=$vuetify.icons.ghd-table-sort
                      :search="searchTerm">
          <template slot="items" slot-scope="props">
            <td>
              {{ props.item.username }}
            </td>
            <td>
              <v-checkbox label="Is Shared" v-model="props.item.isShared"
                          @change="removeUserModifyAccess(props.item.id, props.item.isShared)"/>
            </td>
            <td>
              <v-checkbox :disabled="!props.item.isShared" label="Can Modify" v-model="props.item.canModify"/>
            </td>
          </template>
          <v-alert :value="true"
                   class="ara-orange-bg"
                   icon="fas fa-exclamation"
                   slot="no-results">
            Your search for "{{ searchTerm }}" found no results.
          </v-alert>
        </v-data-table>
      </v-card-text>
      <v-card-actions>
        <v-layout row justify-center>
          <v-btn @click="onSubmit(false)" class="ghd-white-bg ghd-blue ghd-button-text" depressed>Cancel</v-btn>
          <v-btn @click="onSubmit(true)" class="ghd-white-bg ghd-blue ghd-button-text ghd-blue-border ghd-text-padding">
            Save
          </v-btn>
        </v-layout>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import Vue from 'vue';
import {Component, Prop, Watch} from 'vue-property-decorator';
import {Action, State} from 'vuex-class';
import {any, find, findIndex, propEq, update, filter} from 'ramda';
import {BudgetPriorityLibraryUser } from '@/shared/models/iAM/budget-priority';
import {LibraryUser } from '@/shared/models/iAM/user';
import {User} from '@/shared/models/iAM/user';
import {hasValue} from '@/shared/utils/has-value-util';
import {getUserName} from '@/shared/utils/get-user-info';
import {setItemPropertyValueInList} from '@/shared/utils/setter-utils';
import {DataTableHeader} from '@/shared/models/vue/data-table-header';
import {BudgetPriorityLibraryUserGridRow, ShareBudgetPriorityLibraryDialogData } from '@/shared/models/modals/share-budget-priority-library-dialog-data';
import BudgetPriorityService from '@/services/budget-priority.service';
import { http2XX } from '@/shared/utils/http-utils';

@Component
export default class ShareBudgetPriorityLibraryDialog extends Vue {
  @Prop() dialogData: ShareBudgetPriorityLibraryDialogData;

  @State(state => state.userModule.users) stateUsers: User[];

  budgetPriorityLibraryUserGridHeaders: DataTableHeader[] = [
    {text: 'Username', value: 'username', align: 'left', sortable: true, class: '', width: ''},
    {text: 'Shared With', value: '', align: 'left', sortable: true, class: '', width: ''},
    {text: 'Can Modify', value: '', align: 'left', sortable: true, class: '', width: ''}
  ];
  budgetPriorityLibraryUserGridRows: BudgetPriorityLibraryUserGridRow[] = [];
  currentUserAndOwner: BudgetPriorityLibraryUser[] = [];
  searchTerm: string = '';

  @Watch('dialogData')
  onDialogDataChanged() {
    if (this.dialogData.showDialog) {
      this.onSetGridData();
      this.onSetUsersSharedWith();
    }
  }

  onSetGridData() {
    const currentUser: string = getUserName();

    this.budgetPriorityLibraryUserGridRows = this.stateUsers
        .filter((user: User) => user.username !== currentUser)
        .map((user: User) => ({
          id: user.id,
          username: user.username,
          isShared: false,
          canModify: false
        }));
  }

    onSetUsersSharedWith() {
        //budget priority library users
        let budgetPriorityLibraryUsers: BudgetPriorityLibraryUser[] = [];
        BudgetPriorityService.GetBudgetPriorityLibraryUsers(this.dialogData.budgetPriorityLibrary.id).then(response => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString()) && response.data)
            {
                let libraryUsers = response.data as LibraryUser[];
                if (libraryUsers !== null && libraryUsers !== undefined) {

                    libraryUsers.forEach((libraryUser, index) => {
                        //determine access level
                        let canModify = false; if (libraryUser.accessLevel == 1) { canModify = true; }
                        let isOwner = false; if (libraryUser.accessLevel == 2) { isOwner = true; }

                        //create library user object
                        let budgetPriorityLibraryUser: BudgetPriorityLibraryUser = {
                            userId: libraryUser.userId,
                            username: libraryUser.userName,
                            canModify: canModify,
                            isOwner: isOwner,
                        }

                        //add library user to an array
                        budgetPriorityLibraryUsers.push(budgetPriorityLibraryUser);
                    });
                }

                //fill grid
                const currentUser: string = getUserName();
                const isCurrentUserOrOwner = (budgetPriorityLibraryUser: BudgetPriorityLibraryUser) => budgetPriorityLibraryUser.username === currentUser || budgetPriorityLibraryUser.isOwner;
                const isNotCurrentUserOrOwner = (budgetPriorityLibraryUser: BudgetPriorityLibraryUser) => budgetPriorityLibraryUser.username !== currentUser && !budgetPriorityLibraryUser.isOwner;

                this.currentUserAndOwner = filter(isCurrentUserOrOwner, budgetPriorityLibraryUsers) as BudgetPriorityLibraryUser[];
                const otherUsers: BudgetPriorityLibraryUser[] = filter(isNotCurrentUserOrOwner, budgetPriorityLibraryUsers) as BudgetPriorityLibraryUser[];

                otherUsers.forEach((budgetPriorityLibraryUser: BudgetPriorityLibraryUser) => {
                    if (any(propEq('id', budgetPriorityLibraryUser.userId), this.budgetPriorityLibraryUserGridRows)) {
                        const budgetPriorityLibraryUserGridRow: BudgetPriorityLibraryUserGridRow = find(
                            propEq('id', budgetPriorityLibraryUser.userId), this.budgetPriorityLibraryUserGridRows) as BudgetPriorityLibraryUserGridRow;

                        this.budgetPriorityLibraryUserGridRows = update(
                            findIndex(propEq('id', budgetPriorityLibraryUser.userId), this.budgetPriorityLibraryUserGridRows),
                            { ...budgetPriorityLibraryUserGridRow, isShared: true, canModify: budgetPriorityLibraryUser.canModify },
                            this.budgetPriorityLibraryUserGridRows
                        );
                    }
                });
            }
        });
  }

  removeUserModifyAccess(userId: string, isShared: boolean) {
    if (!isShared) {
      this.budgetPriorityLibraryUserGridRows = setItemPropertyValueInList(
          findIndex(propEq('id', userId), this.budgetPriorityLibraryUserGridRows),
          'canModify', false, this.budgetPriorityLibraryUserGridRows);
    }
  }

  onSubmit(submit: boolean) {
    if (submit) {
      this.$emit('submit', this.getBudgetPriorityLibraryUsers());
    } else {
      this.$emit('submit', null);
    }

    this.budgetPriorityLibraryUserGridRows = [];
  }

  getBudgetPriorityLibraryUsers() {
    const usersSharedWith: BudgetPriorityLibraryUser[] = this.budgetPriorityLibraryUserGridRows
        .filter((budgetPriorityLibraryUserGridRow: BudgetPriorityLibraryUserGridRow) => budgetPriorityLibraryUserGridRow.isShared)
        .map((budgetPriorityLibraryUserGridRow: BudgetPriorityLibraryUserGridRow) => ({
          userId: budgetPriorityLibraryUserGridRow.id,
          username: budgetPriorityLibraryUserGridRow.username,
          canModify: budgetPriorityLibraryUserGridRow.canModify,
          isOwner: false
        }));

    return [...this.currentUserAndOwner, ...usersSharedWith];
  }
}
</script>

<style>
.sharing-username {
  margin: auto;
  padding-left: 5%;
  padding-right: 5%;
  flex: 2;
  font-size: 1.2em;
  font-weight: bold;
}

.sharing-checkbox {
  margin: 1.3em auto auto 1em;
  flex: 0;
}

.sharing-row {
  white-space: nowrap;
  display: flex;
  flex-direction: row;
  justify-content: flex-start;
}

.sharing-text {
  margin: auto auto auto 0;
  padding-left: 0;
  padding-right: 1.5em;
  flex: 2;
}

.sharing-button {
  flex: 0.55;
  margin: 1.3em 1.5em auto auto;
}
</style>
