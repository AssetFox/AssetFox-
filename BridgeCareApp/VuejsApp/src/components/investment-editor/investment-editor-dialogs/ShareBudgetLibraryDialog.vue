<template>
  <v-dialog max-width="600px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title>
        <v-row justify="space-between" style="margin-top: 10px;">
          <v-spacer></v-spacer>
          <h3>Budget Library Sharing</h3>
          <v-spacer></v-spacer>
          <v-btn @click="onSubmit(false)" variant="text" class="ghd-close-button">
            X
          </v-btn>
        </v-row>
      </v-card-title>
      <v-card-text>
        <v-data-table id="ShareBudgetLibraryDialog-table-vdatatable" :headers="budgetLibraryUserGridHeaders"
                      :items="budgetLibraryUserGridRows"
                      sort-asc-icon="custom:GhdTableSortAscSvg"
                      sort-desc-icon="custom:GhdTableSortDescSvg"
                      :search="searchTerm" 
                      :items-per-page="5"        
                      :items-per-page-options="[
                        {value: 5, title: '5'},
                        {value: 10, title: '10'},
                        {value: 25, title: '25'},
                    ]">
          <template v-slot:headers="props">
            <tr>
              <th style="font-weight: bold;" v-for="header in budgetLibraryUserGridHeaders" :key="header.title">
                  {{header.title}}
              </th>
            </tr>
          </template>

          <template slot="items" slot-scope="props" v-slot:item="{item}">
            <tr>
            <td>
              {{ item.username }}
            </td>
            <td>
              <v-checkbox id="ShareBudgetLibraryDialog-isShared-vcheckbox" v-model="item.isShared"
                          @change="removeUserModifyAccess(item.id, item.isShared)"></v-checkbox>
            </td>
            <td>
              <v-checkbox id="ShareBudgetLibraryDialog-canModify-vcheckbox" :disabled="!item.isShared" v-model="item.canModify"/>
            </td>
          </tr>
          </template>
        </v-data-table>
      </v-card-text>
      <v-card-actions>
        <v-row>
          <v-spacer></v-spacer>          
          <CancelButton @cancel="onSubmit(false)"/>
          <SaveButton @save="onSubmit(true)"/>
          <v-spacer></v-spacer>
        </v-row>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts" setup>
import {any, find, findIndex, propEq, update, filter} from 'ramda';
import { BudgetLibraryUser } from '@/shared/models/iAM/investment';
import { LibraryUser } from '@/shared/models/iAM/user';
import {User} from '@/shared/models/iAM/user';
import {hasValue} from '@/shared/utils/has-value-util';
import {getUserName} from '@/shared/utils/get-user-info';
import {setItemPropertyValueInList} from '@/shared/utils/setter-utils';
import { BudgetLibraryUserGridRow, ShareBudgetLibraryDialogData } from '@/shared/models/modals/share-budget-library-dialog-data';
import InvestmentService from '@/services/investment.service';
import { http2XX } from '@/shared/utils/http-utils';
import {ref, watch, toRefs, computed} from 'vue';
import { useStore } from 'vuex';
import SaveButton from '@/shared/components/buttons/SaveButton.vue';
import CancelButton from '@/shared/components/buttons/CancelButton.vue';

let store = useStore();
const props = defineProps<{dialogData: ShareBudgetLibraryDialogData}>()
const { dialogData } = toRefs(props);
const stateUsers = computed<User[]>(() => store.state.userModule.users);
  const emit = defineEmits(['submit']);

  let budgetLibraryUserGridHeaders: any[] = [
    {title: 'Username', key: 'username', align: 'left', sortable: true, class: '', width: ''},
    {title: 'Shared With', key: 'isShared', align: 'left', sortable: true, class: '', width: ''},
    {title: 'Can Modify', key: 'canModify', align: 'left', sortable: true, class: '', width: ''}
  ];
  let budgetLibraryUserGridRows = ref<BudgetLibraryUserGridRow[]>([]);
  let currentUserAndOwner: BudgetLibraryUser[] = [];
  let searchTerm: string = '';

  watch(dialogData,()=> {
    if (dialogData.value.showDialog) {
      onSetGridData();
      onSetUsersSharedWith();
    }
  });

  function onSetGridData() {
    const currentUser: string = getUserName();

    budgetLibraryUserGridRows.value = stateUsers.value
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
        InvestmentService.getBudgetLibraryUsers(dialogData.value.budgetLibrary.id).then(response => {
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
                    if (any(propEq('id', budgetLibraryUser.userId), budgetLibraryUserGridRows.value)) {
                        const budgetLibraryUserGridRow: BudgetLibraryUserGridRow = find(
                            propEq('id', budgetLibraryUser.userId), budgetLibraryUserGridRows.value) as BudgetLibraryUserGridRow;

                        budgetLibraryUserGridRows.value = update(
                            findIndex(propEq('id', budgetLibraryUser.userId), budgetLibraryUserGridRows.value),
                            { ...budgetLibraryUserGridRow, isShared: true, canModify: budgetLibraryUser.canModify },
                            budgetLibraryUserGridRows.value
                        );
                    }
                });
            }
        });
  }

  function removeUserModifyAccess(userId: string, isShared: boolean) {
    if (!isShared) {
      budgetLibraryUserGridRows.value = setItemPropertyValueInList(
          findIndex(propEq('id', userId), budgetLibraryUserGridRows.value),
          'canModify', false, budgetLibraryUserGridRows.value);
    }
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', getBudgetLibraryUsers());
    } else {
      emit('submit', null);
    }

    budgetLibraryUserGridRows.value = [];
  }

  function getBudgetLibraryUsers() {
    const usersSharedWith: BudgetLibraryUser[] = budgetLibraryUserGridRows.value
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
.header {
  font-weight: bold;
}
</style>
