<template>
  <v-dialog max-width="500px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title>
        <v-layout justify-center>
          <h3>Performance Curve Library Sharing</h3>
        </v-layout>
          <v-btn @click="onSubmit(false)" flat class="ghd-close-button">
            X
          </v-btn>
      </v-card-title>
      <v-card-text>
        <v-data-table id="SharePerformanceCurveLibraryDialog-table-vdatatable" :headers="performanceCurveLibraryUserGridHeaders"
                      :items="performanceCurveLibraryUserGridRows"
                      sort-icon=$vuetify.icons.ghd-table-sort
                      :search="searchTerm">
          <template slot="items" slot-scope="props">
            <td>
              {{ props.item.username }}
            </td>
            <td>
              <v-checkbox id="SharePerformanceCurveLibraryDialog-isShared-vcheckbox" label="Is Shared" v-model="props.item.isShared"
                          @change="removeUserModifyAccess(props.item.id, props.item.isShared)"/>
            </td>
            <td>
              <v-checkbox id="SharePerformanceCurveLibraryDialog-canModify-vcheckbox" :disabled="!props.item.isShared" label="Can Modify" v-model="props.item.canModify"/>
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
          <v-btn id="SharePerformanceCurveLibraryDialog-cancel-vbtn" @click="onSubmit(false)" class="ghd-white-bg ghd-blue ghd-button-text" depressed>Cancel</v-btn>
          <v-btn id="SharePerformanceCurveLibraryDialog-save-vbtn" @click="onSubmit(true)" class="ghd-white-bg ghd-blue ghd-button-text ghd-blue-border ghd-text-padding">
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
import {PerformanceCurveLibraryUser } from '@/shared/models/iAM/performance';
import {LibraryUser } from '@/shared/models/iAM/user';
import {User} from '@/shared/models/iAM/user';
import {hasValue} from '@/shared/utils/has-value-util';
import {getUserName} from '@/shared/utils/get-user-info';
import {setItemPropertyValueInList} from '@/shared/utils/setter-utils';
import {DataTableHeader} from '@/shared/models/vue/data-table-header';
import {PerformanceCurveLibraryUserGridRow, SharePerformanceCurveLibraryDialogData } from '@/shared/models/modals/share-performance-curve-library-dialog-data';
import PerformanceCurveService from '@/services/performance-curve.service';
import { http2XX } from '@/shared/utils/http-utils';

@Component
export default class SharePerformanceCurveLibraryDialog extends Vue {
  @Prop() dialogData: SharePerformanceCurveLibraryDialogData;

  @State(state => state.userModule.users) stateUsers: User[];

  performanceCurveLibraryUserGridHeaders: DataTableHeader[] = [
    {text: 'Username', value: 'username', align: 'left', sortable: true, class: '', width: ''},
    {text: 'Shared With', value: '', align: 'left', sortable: true, class: '', width: ''},
    {text: 'Can Modify', value: '', align: 'left', sortable: true, class: '', width: ''}
  ];
  performanceCurveLibraryUserGridRows: PerformanceCurveLibraryUserGridRow[] = [];
  currentUserAndOwner: PerformanceCurveLibraryUser[] = [];
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

    this.performanceCurveLibraryUserGridRows = this.stateUsers
        .filter((user: User) => user.username !== currentUser)
        .map((user: User) => ({
          id: user.id,
          username: user.username,
          isShared: false,
          canModify: false
        }));
  }

    onSetUsersSharedWith() {
        //performance curve library users
        let performanceCurveLibraryUsers: PerformanceCurveLibraryUser[] = [];
        PerformanceCurveService.GetPerformanceCurveLibraryUsers(this.dialogData.performanceCurveLibrary.id).then(response => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString()) && response.data)
            {
                let libraryUsers = response.data as LibraryUser[];
                if (libraryUsers !== null && libraryUsers !== undefined) {

                    libraryUsers.forEach((libraryUser, index) => {
                        //determine access level
                        let canModify = false; if (libraryUser.accessLevel == 1) { canModify = true; }
                        let isOwner = false; if (libraryUser.accessLevel == 2) { isOwner = true; }

                        //create library user object
                        let performanceCurveLibraryUser: PerformanceCurveLibraryUser = {
                            userId: libraryUser.userId,
                            username: libraryUser.userName,
                            canModify: canModify,
                            isOwner: isOwner,
                        }

                        //add library user to an array
                        performanceCurveLibraryUsers.push(performanceCurveLibraryUser);
                    });
                }

                //fill grid
                const currentUser: string = getUserName();
                const isCurrentUserOrOwner = (performanceCurveLibraryUser: PerformanceCurveLibraryUser) => performanceCurveLibraryUser.username === currentUser || performanceCurveLibraryUser.isOwner;
                const isNotCurrentUserOrOwner = (performanceCurveLibraryUser: PerformanceCurveLibraryUser) => performanceCurveLibraryUser.username !== currentUser && !performanceCurveLibraryUser.isOwner;

                this.currentUserAndOwner = filter(isCurrentUserOrOwner, performanceCurveLibraryUsers) as PerformanceCurveLibraryUser[];
                const otherUsers: PerformanceCurveLibraryUser[] = filter(isNotCurrentUserOrOwner, performanceCurveLibraryUsers) as PerformanceCurveLibraryUser[];

                otherUsers.forEach((performanceCurveLibraryUser: PerformanceCurveLibraryUser) => {
                    if (any(propEq('id', performanceCurveLibraryUser.userId), this.performanceCurveLibraryUserGridRows)) {
                        const performanceCurveLibraryUserGridRow: PerformanceCurveLibraryUserGridRow = find(
                            propEq('id', performanceCurveLibraryUser.userId), this.performanceCurveLibraryUserGridRows) as PerformanceCurveLibraryUserGridRow;

                        this.performanceCurveLibraryUserGridRows = update(
                            findIndex(propEq('id', performanceCurveLibraryUser.userId), this.performanceCurveLibraryUserGridRows),
                            { ...performanceCurveLibraryUserGridRow, isShared: true, canModify: performanceCurveLibraryUser.canModify },
                            this.performanceCurveLibraryUserGridRows
                        );
                    }
                });
            }
        });
  }

  removeUserModifyAccess(userId: string, isShared: boolean) {
    if (!isShared) {
      this.performanceCurveLibraryUserGridRows = setItemPropertyValueInList(
          findIndex(propEq('id', userId), this.performanceCurveLibraryUserGridRows),
          'canModify', false, this.performanceCurveLibraryUserGridRows);
    }
  }

  onSubmit(submit: boolean) {
    if (submit) {
      this.$emit('submit', this.getPerformanceCurveLibraryUsers());
    } else {
      this.$emit('submit', null);
    }

    this.performanceCurveLibraryUserGridRows = [];
  }

  getPerformanceCurveLibraryUsers() {
    const usersSharedWith: PerformanceCurveLibraryUser[] = this.performanceCurveLibraryUserGridRows
        .filter((performanceCurveLibraryUserGridRow: PerformanceCurveLibraryUserGridRow) => performanceCurveLibraryUserGridRow.isShared)
        .map((performanceCurveLibraryUserGridRow: PerformanceCurveLibraryUserGridRow) => ({
          userId: performanceCurveLibraryUserGridRow.id,
          username: performanceCurveLibraryUserGridRow.username,
          canModify: performanceCurveLibraryUserGridRow.canModify,
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
