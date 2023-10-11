<template>
  <v-dialog max-width="500px" persistent v-bind:show="dialogData.showDialog">
    <v-card>
      <v-card-title>
        <v-layout justify-center>
          <h3>Remaining Life Limit Library Sharing</h3>
        </v-layout>
          <v-btn @click="onSubmit(false)" variant = "flat" class="ghd-close-button">
            X
          </v-btn>
      </v-card-title>
      <v-card-text>
        <v-data-table :headers="remainingLifeLimitLibraryUserGridHeaders"
                      :items="remainingLifeLimitLibraryUserGridRows"
                      sort-icon=$vuetify.icons.ghd-table-sort
                      :search="searchTerm">
          <template slot="items" slot-scope="props" v-slot:item="props">
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
          <v-alert :model-value="true"
                   class="ara-orange-bg"
                   icon="fas fa-exclamation"
                   slot="no-results">
            Your search for "{{ searchTerm }}" found no results.
          </v-alert>
        </v-data-table>
      </v-card-text>
      <v-card-actions>
        <v-layout row justify-center>
          <v-btn @click="onSubmit(false)" class="ghd-white-bg ghd-blue ghd-button-text" variant = "flat">Cancel</v-btn>
          <v-btn @click="onSubmit(true)" class="ghd-white-bg ghd-blue ghd-button-text ghd-blue-border ghd-text-padding">
            Save
          </v-btn>
        </v-layout>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import Vue, { watch } from 'vue';
import {any, find, findIndex, propEq, update, filter} from 'ramda';
import {RemainingLifeLimitLibraryUser } from '@/shared/models/iAM/remaining-life-limit';
import {LibraryUser } from '@/shared/models/iAM/user';
import {User} from '@/shared/models/iAM/user';
import {hasValue} from '@/shared/utils/has-value-util';
import {getUserName} from '@/shared/utils/get-user-info';
import {setItemPropertyValueInList} from '@/shared/utils/setter-utils';
import {DataTableHeader} from '@/shared/models/vue/data-table-header';
import {RemainingLifeLimitLibraryUserGridRow, ShareRemainingLifeLimitLibraryDialogData } from '@/shared/models/modals/share-remaining-life-limit-data';
import RemainingLifeLimitService from '@/services/remaining-life-limit.service';
import { http2XX } from '@/shared/utils/http-utils';
import { useStore } from 'vuex';

  const props = defineProps<{
    dialogData: ShareRemainingLifeLimitLibraryDialogData
  }>();
  const emit = defineEmits(['submit']);
  let store = useStore();
  let stateUsers :User[] = (store.state.userModule.users);

  let remainingLifeLimitLibraryUserGridHeaders: DataTableHeader[] = [
    {text: 'Username', value: 'username', align: 'left', sortable: true, class: '', width: ''},
    {text: 'Shared With', value: '', align: 'left', sortable: true, class: '', width: ''},
    {text: 'Can Modify', value: '', align: 'left', sortable: true, class: '', width: ''}
  ];
  let remainingLifeLimitLibraryUserGridRows: RemainingLifeLimitLibraryUserGridRow[] = [];
  let currentUserAndOwner: RemainingLifeLimitLibraryUser[] = [];
  let searchTerm: string = '';

  watch((() => props.dialogData), onDialogDataChanged )
  function onDialogDataChanged() {
    if (props.dialogData.showDialog) {
      onSetGridData();
      onSetUsersSharedWith();
    }
  }

  function onSetGridData() {
    const currentUser: string = getUserName();

    remainingLifeLimitLibraryUserGridRows = stateUsers
        .filter((user: User) => user.username !== currentUser)
        .map((user: User) => ({
          id: user.id,
          username: user.username,
          isShared: false,
          canModify: false
        }));
  }

  function onSetUsersSharedWith() {
        // Remaining Life Limit library users
        let remainingLifeLimitLibraryUsers: RemainingLifeLimitLibraryUser[] = [];
        RemainingLifeLimitService.getRemainingLifeLimitLibraryUsers(props.dialogData.remainingLifeLimitLibrary.id).then(response => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString()) && response.data)
            {
                let libraryUsers = response.data as LibraryUser[];
                if (libraryUsers !== null && libraryUsers !== undefined) {

                    libraryUsers.forEach((libraryUser, index) => {
                        //determine access level
                        let canModify = false; if (libraryUser.accessLevel == 1) { canModify = true; }
                        let isOwner = false; if (libraryUser.accessLevel == 2) { isOwner = true; }

                        //create library user object
                        let remainingLifeLimitLibraryUser: RemainingLifeLimitLibraryUser = {
                            userId: libraryUser.userId,
                            username: libraryUser.userName,
                            canModify: canModify,
                            isOwner: isOwner,
                        }

                        //add library user to an array
                        remainingLifeLimitLibraryUsers.push(remainingLifeLimitLibraryUser);
                    });
                }

                //fill grid
                const currentUser: string = getUserName();
                const isCurrentUserOrOwner = (remainingLifeLimitLibraryUser: RemainingLifeLimitLibraryUser) => remainingLifeLimitLibraryUser.username === currentUser || remainingLifeLimitLibraryUser.isOwner;
                const isNotCurrentUserOrOwner = (remainingLifeLimitLibraryUser: RemainingLifeLimitLibraryUser) => remainingLifeLimitLibraryUser.username !== currentUser && !remainingLifeLimitLibraryUser.isOwner;

                currentUserAndOwner = filter(isCurrentUserOrOwner, remainingLifeLimitLibraryUsers) as RemainingLifeLimitLibraryUser[];
                const otherUsers: RemainingLifeLimitLibraryUser[] = filter(isNotCurrentUserOrOwner, remainingLifeLimitLibraryUsers) as RemainingLifeLimitLibraryUser[];

                otherUsers.forEach((remainingLifeLimitLibraryUser: RemainingLifeLimitLibraryUser) => {
                    if (any(propEq('id', remainingLifeLimitLibraryUser.userId), remainingLifeLimitLibraryUserGridRows)) {
                        const remainingLifeLimitLibraryUserGridRow: RemainingLifeLimitLibraryUserGridRow = find(
                            propEq('id', remainingLifeLimitLibraryUser.userId), remainingLifeLimitLibraryUserGridRows) as RemainingLifeLimitLibraryUserGridRow;

                        remainingLifeLimitLibraryUserGridRows = update(
                            findIndex(propEq('id', remainingLifeLimitLibraryUser.userId), remainingLifeLimitLibraryUserGridRows),
                            { ...remainingLifeLimitLibraryUserGridRow, isShared: true, canModify: remainingLifeLimitLibraryUser.canModify },
                            remainingLifeLimitLibraryUserGridRows
                        );
                    }
                });
            }
        });
  }

  function removeUserModifyAccess(userId: string, isShared: boolean) {
    if (!isShared) {
      remainingLifeLimitLibraryUserGridRows = setItemPropertyValueInList(
          findIndex(propEq('id', userId), remainingLifeLimitLibraryUserGridRows),
          'canModify', false, remainingLifeLimitLibraryUserGridRows);
    }
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', getRemainingLifeLimitLibraryUsers());
    } else {
      emit('submit', null);
    }

    remainingLifeLimitLibraryUserGridRows = [];
  }

  function getRemainingLifeLimitLibraryUsers() {
    const usersSharedWith: RemainingLifeLimitLibraryUser[] = remainingLifeLimitLibraryUserGridRows
        .filter((remainingLifeLimitLibraryUserGridRow: RemainingLifeLimitLibraryUserGridRow) => remainingLifeLimitLibraryUserGridRow.isShared)
        .map((remainingLifeLimitLibraryUserGridRow: RemainingLifeLimitLibraryUserGridRow) => ({
          userId: remainingLifeLimitLibraryUserGridRow.id,
          username: remainingLifeLimitLibraryUserGridRow.username,
          canModify: remainingLifeLimitLibraryUserGridRow.canModify,
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
