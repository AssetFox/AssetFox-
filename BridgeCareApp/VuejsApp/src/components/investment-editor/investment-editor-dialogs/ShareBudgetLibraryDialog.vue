<template>
  <v-dialog max-width="500px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title>
        <v-row justify-center>
          <h3>Budget Library Sharing</h3>
        </v-row>
      </v-card-title>
      <v-card-text>
        <v-data-table id="ShareBudgetLibraryDialog-table-vdatatable" :headers="budgetLibraryUserGridHeaders"
                      :items="budgetLibraryUserGridRows"
                      sort-icon=$vuetify.icons.ghd-table-sort
                      :search="searchTerm">
          <template slot="items" slot-scope="props" v-slot:item="{item}">
            <td>
              {{ item.value.username }}
            </td>
            <td>
              <v-checkbox id="ShareBudgetLibraryDialog-isShared-vcheckbox" label="Is Shared" v-model="item.raw.isShared"
                          @change="removeUserModifyAccess(item.value.id, item.value.isShared)"/>
            </td>
            <td>
              <v-checkbox id="ShareBudgetLibraryDialog-canModify-vcheckbox" :disabled="!item.value.isShared" label="Can Modify" v-model="item.raw.canModify"/>
            </td>
          </template>
          <v-alert :model-value="true"
                   class="ara-orange-bg"
                   icon="fas fa-exclamation"
                   slot="no-results">
            Your search for "{{ searchTerm }}" found no results.
          </v-alert>
        </v-data-table>
      </v-card-text>
      <v-card-actions>
        <v-row justify-space-between row>
          <v-btn id="ShareBudgetLibraryDialog-save-vbtn" @click="onSubmit(true)" class="ara-blue-bg text-white">
            Save
          </v-btn>
          <v-btn id="ShareBudgetLibraryDialog-cancel-vbtn" @click="onSubmit(false)" class="ara-orange-bg text-white">Cancel</v-btn>
        </v-row>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts" setup>
import Vue from 'vue';
import {any, find, findIndex, propEq, update, filter} from 'ramda';
import { BudgetLibraryUser } from '@/shared/models/iAM/investment';
import { LibraryUser } from '@/shared/models/iAM/user';
import {User} from '@/shared/models/iAM/user';
import {hasValue} from '@/shared/utils/has-value-util';
import {getUserName} from '@/shared/utils/get-user-info';
import {setItemPropertyValueInList} from '@/shared/utils/setter-utils';
import {DataTableHeader} from '@/shared/models/vue/data-table-header';
import { BudgetLibraryUserGridRow, ShareBudgetLibraryDialogData } from '@/shared/models/modals/share-budget-library-dialog-data';
import InvestmentService from '@/services/investment.service';
    import { AxiosResponse } from 'axios';
    import { http2XX } from '@/shared/utils/http-utils';
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';

let store = useStore();
const emit = defineEmits(['submit'])
const props = defineProps<{
  dialogData: ShareBudgetLibraryDialogData
}>()
let stateUsers = ref<User[]>(store.state.userModule.users);

  let budgetLibraryUserGridHeaders: DataTableHeader[] = [
    {text: 'Username', value: 'username', align: 'left', sortable: true, class: '', width: ''},
    {text: 'Shared With', value: '', align: 'left', sortable: true, class: '', width: ''},
    {text: 'Can Modify', value: '', align: 'left', sortable: true, class: '', width: ''}
  ];
  let budgetLibraryUserGridRows: BudgetLibraryUserGridRow[] = [];
  let currentUserAndOwner: BudgetLibraryUser[] = [];
  let searchTerm: string = '';

  watch(props.dialogData,()=>onDialogDataChanged)
  function onDialogDataChanged() {
    if (props.dialogData.showDialog) {
      onSetGridData();
      onSetUsersSharedWith();
    }
  }

  function onSetGridData() {
    const currentUser: string = getUserName();

    budgetLibraryUserGridRows = stateUsers.value
        .filter((user: User) => user.username !== currentUser)
        .map((user: User) => ({
          id: user.id,
          username: user.username,
          isShared: false,
          canModify: false
        }));
  }

    function onSetUsersSharedWith() {
        //budget library users
        let budgetLibraryUsers: BudgetLibraryUser[] = [];
        InvestmentService.getBudgetLibraryUsers(props.dialogData.budgetLibrary.id).then(response => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString()) && response.data)
            {
                let libraryUsers = response.data as LibraryUser[];
                if (libraryUsers !== null && libraryUsers !== undefined) {
                    //fill budget library user collection
                    libraryUsers.forEach((libraryUser, index) => {
                        //determine access level
                        let canModify = false; if (libraryUser.accessLevel == 1) { canModify = true; }
                        let isOwner = false; if (libraryUser.accessLevel == 2) { isOwner = true; }

                        //create library user object
                        let budgetLibraryUser: BudgetLibraryUser = {
                            userId: libraryUser.userId,
                            username: libraryUser.userName,
                            canModify: canModify,
                            isOwner: isOwner,
                        }

                        //add library user to an array
                        budgetLibraryUsers.push(budgetLibraryUser);
                    });
                }

                //fill grid
                const currentUser: string = getUserName();
                const isCurrentUserOrOwner = (budgetLibraryUser: BudgetLibraryUser) => budgetLibraryUser.username === currentUser || budgetLibraryUser.isOwner;
                const isNotCurrentUserOrOwner = (budgetLibraryUser: BudgetLibraryUser) => budgetLibraryUser.username !== currentUser && !budgetLibraryUser.isOwner;

                currentUserAndOwner = filter(isCurrentUserOrOwner, budgetLibraryUsers) as BudgetLibraryUser[];
                const otherUsers: BudgetLibraryUser[] = filter(isNotCurrentUserOrOwner, budgetLibraryUsers) as BudgetLibraryUser[];

                otherUsers.forEach((budgetLibraryUser: BudgetLibraryUser) => {
                    if (any(propEq('id', budgetLibraryUser.userId), budgetLibraryUserGridRows)) {
                        const budgetLibraryUserGridRow: BudgetLibraryUserGridRow = find(
                            propEq('id', budgetLibraryUser.userId), budgetLibraryUserGridRows) as BudgetLibraryUserGridRow;

                        budgetLibraryUserGridRows = update(
                            findIndex(propEq('id', budgetLibraryUser.userId), budgetLibraryUserGridRows),
                            { ...budgetLibraryUserGridRow, isShared: true, canModify: budgetLibraryUser.canModify },
                            budgetLibraryUserGridRows
                        );
                    }
                });
            }
        });
  }

  function removeUserModifyAccess(userId: string, isShared: boolean) {
    if (!isShared) {
      budgetLibraryUserGridRows = setItemPropertyValueInList(
          findIndex(propEq('id', userId), budgetLibraryUserGridRows),
          'canModify', false, budgetLibraryUserGridRows);
    }
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', getBudgetLibraryUsers());
    } else {
      emit('submit', null);
    }

    budgetLibraryUserGridRows = [];
  }

  function getBudgetLibraryUsers() {
    const usersSharedWith: BudgetLibraryUser[] = budgetLibraryUserGridRows
        .filter((budgetLibraryUserGridRow: BudgetLibraryUserGridRow) => budgetLibraryUserGridRow.isShared)
        .map((budgetLibraryUserGridRow: BudgetLibraryUserGridRow) => ({
          userId: budgetLibraryUserGridRow.id,
          username: budgetLibraryUserGridRow.username,
          canModify: budgetLibraryUserGridRow.canModify,
          isOwner: false
        }));

    return [...currentUserAndOwner, ...usersSharedWith];
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
