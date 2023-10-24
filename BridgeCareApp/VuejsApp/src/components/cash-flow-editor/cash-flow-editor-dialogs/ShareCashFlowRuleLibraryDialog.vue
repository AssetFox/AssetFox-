<template>
  <v-dialog max-width="500px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title>
        <v-layout justify-center>
          <h3>Cash Flow Rule Library Sharing</h3>
        </v-layout>
          <v-btn @click="onSubmit(false)" flat class="ghd-close-button">
            X
          </v-btn>
      </v-card-title>
      <v-card-text>
        <v-data-table id="ShareCashFlowRuleLibraryDialog-table-vdatatable" :headers="shareCashFlowRuleLibraryUserGridHeaders"
                      :items="shareCashFlowRuleLibraryUserGridRows"
                      sort-icon=$vuetify.icons.ghd-table-sort
                      :search="searchTerm">
          <template slot="items" slot-scope="props">
            <td>
              {{ props.item.username }}
            </td>
            <td>
              <v-checkbox id="ShareCashFlowRuleLibraryDialog-isShared-vcheckbox" label="Is Shared" v-model="props.item.isShared"
                          @change="removeUserModifyAccess(props.item.id, props.item.isShared)"/>
            </td>
            <td>
              <v-checkbox id="ShareCashFlowRuleLibraryDialog-canModify-vcheckbox" :disabled="!props.item.isShared" label="Can Modify" v-model="props.item.canModify"/>
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
          <v-btn id="ShareCashFlowRuleLibraryDialog-cancel-vbtn" @click="onSubmit(false)" class="ghd-white-bg ghd-blue ghd-button-text" depressed>Cancel</v-btn>
          <v-btn id="ShareCashFlowRuleLibraryDialog-save-vbtn" @click="onSubmit(true)" class="ghd-white-bg ghd-blue ghd-button-text ghd-blue-border ghd-text-padding">
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
import {CashFlowRuleLibraryUser } from '@/shared/models/iAM/cash-flow';
import {LibraryUser } from '@/shared/models/iAM/user';
import {User} from '@/shared/models/iAM/user';
import {hasValue} from '@/shared/utils/has-value-util';
import {getUserName} from '@/shared/utils/get-user-info';
import {setItemPropertyValueInList} from '@/shared/utils/setter-utils';
import {DataTableHeader} from '@/shared/models/vue/data-table-header';
import {CashFlowRuleLibraryUserGridRow, ShareCashFlowRuleLibraryDialogData } from '@/shared/models/modals/share-cash-flow-rule-data';
import CashFlowRuleService from '@/services/cash-flow.service';
import { http2XX } from '@/shared/utils/http-utils';

@Component
export default class ShareCashFlowRuleLibraryDialog extends Vue {
  @Prop() dialogData: ShareCashFlowRuleLibraryDialogData;

  @State(state => state.userModule.users) stateUsers: User[];

  shareCashFlowRuleLibraryUserGridHeaders: DataTableHeader[] = [
    {text: 'Username', value: 'username', align: 'left', sortable: true, class: '', width: ''},
    {text: 'Shared With', value: '', align: 'left', sortable: true, class: '', width: ''},
    {text: 'Can Modify', value: '', align: 'left', sortable: true, class: '', width: ''}
  ];
  shareCashFlowRuleLibraryUserGridRows: CashFlowRuleLibraryUserGridRow[] = [];
  currentUserAndOwner: CashFlowRuleLibraryUser[] = [];
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

    this.shareCashFlowRuleLibraryUserGridRows = this.stateUsers
        .filter((user: User) => user.username !== currentUser)
        .map((user: User) => ({
          id: user.id,
          username: user.username,
          isShared: false,
          canModify: false
        }));
  }

    onSetUsersSharedWith() {
        // Cash Flow Rule library users
        let cashFlowRuleLibraryUsers: CashFlowRuleLibraryUser[] = [];
        CashFlowRuleService.getCashFlowRuleLibraryUsers(this.dialogData.cashFlowRuleLibrary.id).then(response => {
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

                this.currentUserAndOwner = filter(isCurrentUserOrOwner, cashFlowRuleLibraryUsers) as CashFlowRuleLibraryUser[];
                const otherUsers: CashFlowRuleLibraryUser[] = filter(isNotCurrentUserOrOwner, cashFlowRuleLibraryUsers) as CashFlowRuleLibraryUser[];

                otherUsers.forEach((cashFlowRuleLibraryUser: CashFlowRuleLibraryUser) => {
                    if (any(propEq('id', cashFlowRuleLibraryUser.userId), this.shareCashFlowRuleLibraryUserGridRows)) {
                        const cashFlowRuleLibraryUserGridRow: CashFlowRuleLibraryUserGridRow = find(
                            propEq('id', cashFlowRuleLibraryUser.userId), this.shareCashFlowRuleLibraryUserGridRows) as CashFlowRuleLibraryUserGridRow;

                        this.shareCashFlowRuleLibraryUserGridRows = update(
                            findIndex(propEq('id', cashFlowRuleLibraryUser.userId), this.shareCashFlowRuleLibraryUserGridRows),
                            { ...cashFlowRuleLibraryUserGridRow, isShared: true, canModify: cashFlowRuleLibraryUser.canModify },
                            this.shareCashFlowRuleLibraryUserGridRows
                        );
                    }
                });
            }
        });
  }

  removeUserModifyAccess(userId: string, isShared: boolean) {
    if (!isShared) {
      this.shareCashFlowRuleLibraryUserGridRows = setItemPropertyValueInList(
          findIndex(propEq('id', userId), this.shareCashFlowRuleLibraryUserGridRows),
          'canModify', false, this.shareCashFlowRuleLibraryUserGridRows);
    }
  }

  onSubmit(submit: boolean) {
    if (submit) {
      this.$emit('submit', this.getCashFlowRuleLibraryUsers());
    } else {
      this.$emit('submit', null);
    }

    this.shareCashFlowRuleLibraryUserGridRows = [];
  }

  getCashFlowRuleLibraryUsers() {
    const usersSharedWith: CashFlowRuleLibraryUser[] = this.shareCashFlowRuleLibraryUserGridRows
        .filter((cashFlowRuleLibraryUserGridRow: CashFlowRuleLibraryUserGridRow) => cashFlowRuleLibraryUserGridRow.isShared)
        .map((cashFlowRuleLibraryUserGridRow: CashFlowRuleLibraryUserGridRow) => ({
          userId: cashFlowRuleLibraryUserGridRow.id,
          username: cashFlowRuleLibraryUserGridRow.username,
          canModify: cashFlowRuleLibraryUserGridRow.canModify,
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
