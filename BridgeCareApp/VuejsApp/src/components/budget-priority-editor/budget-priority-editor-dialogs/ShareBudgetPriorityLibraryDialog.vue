<template>
  <v-dialog  max-width="600px" persistent v-model ="dialogData.showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-padding-top-title">
        <v-row justify="space-between">
          <div class="ghd-control-dialog-header"><h5>Budget Priority Library Sharing</h5></div>
          <XButton @click="onSubmit(false)"/>
        </v-row>
      </v-card-title>
      <v-card-text>
        <v-data-table id="ShareBudgetPriorityLibraryDialog-table-vdatatable"
                      :headers="budgetPriorityLibraryUserGridHeaders"
                      :items="budgetPriorityLibraryUserGridRows"
                      :items-length="budgetPriorityLibraryUserGridRows.length"
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
              <th style="font-weight: bold;" v-for="header in budgetPriorityLibraryUserGridHeaders" :key="header.title">
                  {{header.title}}
              </th>
            </tr>
          </template>
          <template v-slot:item="{item}" slot="items" slot-scope="props">
            <tr>
            <td>
              {{ item.username }}
            </td>
            <td>
              <v-checkbox id="ShareBudgetPriorityLibraryDialog-isShared-vcheckbox" v-model="item.isShared"
                          @change="removeUserModifyAccess(item.id, item.isShared)"/>
            </td>
            <td>
              <v-checkbox id="ShareBudgetPriorityLibraryDialog-canModify-vcheckbox" :disabled="!item.isShared" v-model="item.canModify"/>
            </td>
          </tr>
          </template>
          <!-- <v-alert :model-value="true"
                   class="assetFox-orange-bg"
                   icon="fas fa-exclamation"
                   slot="no-results">
            Your search for "{{ searchTerm }}" found no results.
          </v-alert> -->
        </v-data-table>
      </v-card-text>
      <v-card-actions>
        <v-row justify="center">
          <CancelButton @cancel="onSubmit(false)"/>
          <SaveButton @save="onSubmit(true)"/>          
        </v-row>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { computed, toRefs, ref, watch } from 'vue';
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
import { useStore } from 'vuex';
import SaveButton from '@/shared/components/buttons/SaveButton.vue';
import CancelButton from '@/shared/components/buttons/CancelButton.vue';
import XButton from '@/shared/components/buttons/XButton.vue';

  let store = useStore();
  const stateUsers  = computed<User[]>(()=>store.state.userModule.users);
  const props = defineProps<{
    dialogData: ShareBudgetPriorityLibraryDialogData
  }>();
  const { dialogData } = toRefs(props);

  const emit = defineEmits(['submit']);

  let budgetPriorityLibraryUserGridHeaders: any[] = [
    {title: 'Username', key: 'username', align: 'left', sortable: true, class: '', width: ''},
    {title: 'Shared With', key: '', align: 'left', sortable: true, class: '', width: ''},
    {title: 'Can Modify', key: '', align: 'left', sortable: true, class: '', width: ''}
  ];
  let budgetPriorityLibraryUserGridRows = ref<BudgetPriorityLibraryUserGridRow[]>([]);
  let currentUserAndOwner = ref<BudgetPriorityLibraryUser[]>([]);
  let searchTerm: string = '';

  watch(dialogData, ()=> {
    if (dialogData.value.showDialog) {
      onSetGridData();
      onSetUsersSharedWith();
    }
  });

  function onSetGridData() {
    const currentUser: string = getUserName();

    budgetPriorityLibraryUserGridRows.value = stateUsers.value
        .filter((user: User) => user.username !== currentUser)
        .map((user: User) => ({
          id: user.id,
          username: user.username,
          isShared: false,
          canModify: false
        }));
  }

    function onSetUsersSharedWith() {
        //budget priority library users
        let budgetPriorityLibraryUsers: BudgetPriorityLibraryUser[] = [];
        BudgetPriorityService.GetBudgetPriorityLibraryUsers(props.dialogData.budgetPriorityLibrary.id).then(response => {
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

                currentUserAndOwner.value = filter(isCurrentUserOrOwner, budgetPriorityLibraryUsers) as BudgetPriorityLibraryUser[];
                const otherUsers: BudgetPriorityLibraryUser[] = filter(isNotCurrentUserOrOwner, budgetPriorityLibraryUsers) as BudgetPriorityLibraryUser[];

                otherUsers.forEach((budgetPriorityLibraryUser: BudgetPriorityLibraryUser) => {
                    if (any(propEq('id', budgetPriorityLibraryUser.userId), budgetPriorityLibraryUserGridRows.value)) {
                        const budgetPriorityLibraryUserGridRow: BudgetPriorityLibraryUserGridRow = find(
                            propEq('id', budgetPriorityLibraryUser.userId), budgetPriorityLibraryUserGridRows.value) as BudgetPriorityLibraryUserGridRow;

                        budgetPriorityLibraryUserGridRows.value = update(
                            findIndex(propEq('id', budgetPriorityLibraryUser.userId), budgetPriorityLibraryUserGridRows.value),
                            { ...budgetPriorityLibraryUserGridRow, isShared: true, canModify: budgetPriorityLibraryUser.canModify },
                            budgetPriorityLibraryUserGridRows.value
                        );
                    }
                });
            }
        });
  }

  function removeUserModifyAccess(userId: string, isShared: boolean) {
    if (!isShared) {
      budgetPriorityLibraryUserGridRows.value = setItemPropertyValueInList(
          findIndex(propEq('id', userId), budgetPriorityLibraryUserGridRows.value),
          'canModify', false, budgetPriorityLibraryUserGridRows.value);
    }
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', getBudgetPriorityLibraryUsers());
    } else {
      emit('submit', null);
    }

    budgetPriorityLibraryUserGridRows.value = [];
  }

  function getBudgetPriorityLibraryUsers() {
    const usersSharedWith: BudgetPriorityLibraryUser[] = budgetPriorityLibraryUserGridRows.value
        .filter((budgetPriorityLibraryUserGridRow: BudgetPriorityLibraryUserGridRow) => budgetPriorityLibraryUserGridRow.isShared)
        .map((budgetPriorityLibraryUserGridRow: BudgetPriorityLibraryUserGridRow) => ({
          userId: budgetPriorityLibraryUserGridRow.id,
          username: budgetPriorityLibraryUserGridRow.username,
          canModify: budgetPriorityLibraryUserGridRow.canModify,
          isOwner: false
        }));

    return [...currentUserAndOwner.value, ...usersSharedWith];
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
