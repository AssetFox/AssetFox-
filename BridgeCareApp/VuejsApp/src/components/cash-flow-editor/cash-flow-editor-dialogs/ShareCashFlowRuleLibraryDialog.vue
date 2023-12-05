<template>
  <v-dialog max-width="500px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title>
        <v-row justify="space-between" style="margin-top: 10px;">
          <h5>Cash Flow Rule Library Sharing</h5>
          <v-btn @click="onSubmit(false)" variant = "flat" class="ghd-close-button">
            X
          </v-btn>
        </v-row>
      </v-card-title>
      <v-card-text>
        <v-data-table id="ShareCashFlowRuleLibraryDialog-table-vdatatable" :headers="shareCashFlowRuleLibraryUserGridHeaders"
                      :items="shareCashFlowRuleLibraryUserGridRows"
                      sort-asc-icon="custom:GhdTableSortAscSvg"
                      sort-desc-icon="custom:GhdTableSortDescSvg"
                      :search="searchTerm">
          <template v-slot:item="{item}" slot="items" slot-scope="props">
            <tr>
            <td>
              {{ item.username }}
            </td>
            <td>
              <v-checkbox id="ShareCashFlowRuleLibraryDialog-isShared-vcheckbox" label="Is Shared" v-model="item.isShared"
                          @change="removeUserModifyAccess(item.id, item.isShared)"/>
            </td>
            <td>
              <v-checkbox id="ShareCashFlowRuleLibraryDialog-canModify-vcheckbox" :disabled="!item.isShared" label="Can Modify" v-model="item.canModify"/>
            </td>
          </tr>
          </template>
          <!-- <v-alert :model-value="true"
                   class="ara-orange-bg"
                   icon="fas fa-exclamation"
                   slot="no-results">
            Your search for "{{ searchTerm }}" found no results.
          </v-alert> -->
        </v-data-table>
      </v-card-text>
      <v-card-actions>
        <v-row justify="center">
          <v-btn id="ShareCashFlowRuleLibraryDialog-cancel-vbtn" @click="onSubmit(false)" class="ghd-white-bg ghd-blue ghd-button-text" variant = "flat">Cancel</v-btn>
          <v-btn id="ShareCashFlowRuleLibraryDialog-save-vbtn" @click="onSubmit(true)" class="ghd-white-bg ghd-blue ghd-button-text ghd-blue-border ghd-text-padding" variant="outlined">
            Save
          </v-btn>
        </v-row>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts" setup>
import { reactive, watch, ref, computed, toRefs } from 'vue';
import {any, find, findIndex, propEq, update, filter} from 'ramda';
import {CashFlowRuleLibraryUser } from '@/shared/models/iAM/cash-flow';
import {LibraryUser } from '@/shared/models/iAM/user';
import {User} from '@/shared/models/iAM/user';
import {hasValue} from '@/shared/utils/has-value-util';
import {getUserName} from '@/shared/utils/get-user-info';
import {setItemPropertyValueInList} from '@/shared/utils/setter-utils';
import {CashFlowRuleLibraryUserGridRow, ShareCashFlowRuleLibraryDialogData } from '@/shared/models/modals/share-cash-flow-rule-data';
import CashFlowRuleService from '@/services/cash-flow.service';
import { http2XX } from '@/shared/utils/http-utils';
import { useStore } from 'vuex';

  let store = useStore();
  const props = defineProps<{dialogData: ShareCashFlowRuleLibraryDialogData}>()
  const { dialogData } = toRefs(props);
  const stateUsers = computed<User[]>(() => store.state.userModule.users);

  const emit = defineEmits(['submit']);

  const shareCashFlowRuleLibraryUserGridHeaders: any[]= [
    {title: 'Username', key: 'username', align: 'left', sortable: true, class: '', width: ''},
    {title: 'Shared With', key: 'isShared', align: 'left', sortable: true, class: '', width: ''},
    {title: 'Can Modify', key: 'canModify', align: 'left', sortable: true, class: '', width: ''}
  ] ;
  const shareCashFlowRuleLibraryUserGridRows = ref<CashFlowRuleLibraryUserGridRow[]>([]);
  let currentUserAndOwner: CashFlowRuleLibraryUser[] = [];
  let searchTerm: string = '';

  watch(dialogData, () => {
    if (dialogData.value.showDialog) {
      onSetGridData();
      onSetUsersSharedWith();
    }
  });

  function onSetGridData() {
    const currentUser: string = getUserName();

    shareCashFlowRuleLibraryUserGridRows.value = stateUsers.value
        .filter((user: User) => user.username !== currentUser)
        .map((user: User) => ({
          id: user.id,
          username: user.username,
          isShared: false,
          canModify: false
        }));
  }

    function onSetUsersSharedWith() {
        // Cash Flow Rule library users
        let cashFlowRuleLibraryUsers: CashFlowRuleLibraryUser[] = [];
        CashFlowRuleService.getCashFlowRuleLibraryUsers(dialogData.value.cashFlowRuleLibrary.id).then(response => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString()) && response.data)
            {
                let libraryUsers = response.data as LibraryUser[];
                if (libraryUsers !== null && libraryUsers !== undefined) {

                    libraryUsers.forEach((libraryUser, index) => {
                        //determine access level
                        let canModify = false; if (libraryUser.accessLevel == 1) { canModify = true; }
                        let isOwner = false; if (libraryUser.accessLevel == 2) { isOwner = true; }

                        //create library user object
                        let cashFlowRuleLibraryUser: CashFlowRuleLibraryUser = {
                            userId: libraryUser.userId,
                            username: libraryUser.userName,
                            canModify: canModify,
                            isOwner: isOwner,
                        }

                        //add library user to an array
                        cashFlowRuleLibraryUsers.push(cashFlowRuleLibraryUser);
                    });
                }
                //fill grid
                const currentUser: string = getUserName();
                const isCurrentUserOrOwner = (cashFlowRuleLibraryUser: CashFlowRuleLibraryUser) => cashFlowRuleLibraryUser.username === currentUser || cashFlowRuleLibraryUser.isOwner;
                const isNotCurrentUserOrOwner = (cashFlowRuleLibraryUser: CashFlowRuleLibraryUser) => cashFlowRuleLibraryUser.username !== currentUser && !cashFlowRuleLibraryUser.isOwner;

                currentUserAndOwner = filter(isCurrentUserOrOwner, cashFlowRuleLibraryUsers) as CashFlowRuleLibraryUser[];
                const otherUsers: CashFlowRuleLibraryUser[] = filter(isNotCurrentUserOrOwner, cashFlowRuleLibraryUsers) as CashFlowRuleLibraryUser[];

                otherUsers.forEach((cashFlowRuleLibraryUser: CashFlowRuleLibraryUser) => {
                    if (any(propEq('id', cashFlowRuleLibraryUser.userId), shareCashFlowRuleLibraryUserGridRows.value)) {
                        const cashFlowRuleLibraryUserGridRow: CashFlowRuleLibraryUserGridRow = find(
                            propEq('id', cashFlowRuleLibraryUser.userId), shareCashFlowRuleLibraryUserGridRows.value) as CashFlowRuleLibraryUserGridRow;

                        shareCashFlowRuleLibraryUserGridRows.value = update(
                            findIndex(propEq('id', cashFlowRuleLibraryUser.userId), shareCashFlowRuleLibraryUserGridRows.value),
                            { ...cashFlowRuleLibraryUserGridRow, isShared: true, canModify: cashFlowRuleLibraryUser.canModify },
                            shareCashFlowRuleLibraryUserGridRows.value
                        );
                    }
                });
            }
        });
  }

  function removeUserModifyAccess(userId: string, isShared: boolean) {
    if (!isShared) {
      shareCashFlowRuleLibraryUserGridRows.value = setItemPropertyValueInList(
          findIndex(propEq('id', userId), shareCashFlowRuleLibraryUserGridRows.value),
          'canModify', false, shareCashFlowRuleLibraryUserGridRows.value);
    }
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', getCashFlowRuleLibraryUsers());
    } else {
      emit('submit', null);
    }

    shareCashFlowRuleLibraryUserGridRows.value = [];
  }

  function getCashFlowRuleLibraryUsers() {
    const usersSharedWith: CashFlowRuleLibraryUser[] = shareCashFlowRuleLibraryUserGridRows.value
        .filter((cashFlowRuleLibraryUserGridRow: CashFlowRuleLibraryUserGridRow) => cashFlowRuleLibraryUserGridRow.isShared)
        .map((cashFlowRuleLibraryUserGridRow: CashFlowRuleLibraryUserGridRow) => ({
          userId: cashFlowRuleLibraryUserGridRow.id,
          username: cashFlowRuleLibraryUserGridRow.username,
          canModify: cashFlowRuleLibraryUserGridRow.canModify,
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
