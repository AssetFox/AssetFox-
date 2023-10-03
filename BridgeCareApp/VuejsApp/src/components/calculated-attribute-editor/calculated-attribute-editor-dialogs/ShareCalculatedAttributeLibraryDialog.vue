<template>
  <v-dialog max-width="500px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title>
        <v-layout justify-center>
          <h3>Calculated Attribute Library Sharing</h3>
        </v-layout>
          <v-btn @click="onSubmit(false)" variant = "flat" class="ghd-close-button">
            X
          </v-btn>
      </v-card-title>
      <v-card-text>
        <v-data-table :headers="shareCalculatedAttributeLibraryUserGridHeaders"
                      :items="shareCalculatedAttributeLibraryUserGridRows"
                      sort-icon=$vuetify.icons.ghd-table-sort
                      :search="searchTerm">
          <template slot="items" slot-scope="props" v-slot:item="{item}">
            <td>
              {{ item.username }}
            </td>
            <td>
              <v-checkbox label="Is Shared" v-model="item.raw.isShared"
                          @change="removeUserModifyAccess(item.id, item.isShared)"/>
            </td>
            <td>
              <v-checkbox :disabled="!dialogData.isShared" label="Can Modify" v-model="item.raw.canModify"/>
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
          <v-btn @click="onSubmit(false)" class="ghd-white-bg ghd-blue ghd-button-text" variant = "flat">Cancel</v-btn>
          <v-btn @click="onSubmit(true)" class="ghd-white-bg ghd-blue ghd-button-text ghd-blue-border ghd-text-padding">
            Save
          </v-btn>
        </v-layout>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts" setup>
import Vue from 'vue';
import {any, find, findIndex, propEq, update, filter} from 'ramda';
import {CalculatedAttributeLibraryUser } from '@/shared/models/iAM/calculated-attribute';
import {LibraryUser } from '@/shared/models/iAM/user';
import {User} from '@/shared/models/iAM/user';
import {hasValue} from '@/shared/utils/has-value-util';
import {getUserName} from '@/shared/utils/get-user-info';
import {setItemPropertyValueInList} from '@/shared/utils/setter-utils';
import {DataTableHeader} from '@/shared/models/vue/data-table-header';
import {CalculatedAttributeLibraryUserGridRow, ShareCalculatedAttributeLibraryDialogData, emptyShareCalculatedAttributeLibraryDialogData } from '@/shared/models/modals/share-calculated-attribute-data';
import CalculatedAttributeService from '@/services/calculated-attribute.service';
import { http2XX } from '@/shared/utils/http-utils';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { CreateCalculatedAttributeLibraryDialogData } from '@/shared/models/modals/create-calculated-attribute-library-dialog-data';

const emit = defineEmits(['submit'])
let store = useStore();
const props = defineProps<{
    dialogData: ShareCalculatedAttributeLibraryDialogData
  }>()
let stateUsers = ref<User[]>(store.state.userModule.users);
let shareCalculatedAttributeLibraryUserGridHeaders: DataTableHeader[] = [
    {text: 'Username', value: 'username', align: 'left', sortable: true, class: '', width: ''},
    {text: 'Shared With', value: '', align: 'left', sortable: true, class: '', width: ''},
    {text: 'Can Modify', value: '', align: 'left', sortable: true, class: '', width: ''}
  ];
  let shareCalculatedAttributeLibraryUserGridRows: CalculatedAttributeLibraryUserGridRow[] = [];
  let currentUserAndOwner: CalculatedAttributeLibraryUser[] = [];
  let searchTerm: string = '';

  watch(()=>props.dialogData,() => onDialogDataChanged)
  function onDialogDataChanged() {
    if (props.dialogData.showDialog) {
      onSetGridData();
      onSetUsersSharedWith();
    }
  }

  function onSetGridData() {
    const currentUser: string = getUserName();

    shareCalculatedAttributeLibraryUserGridRows = stateUsers.value
        .filter((user: User) => user.username !== currentUser)
        .map((user: User) => ({
          id: user.id,
          username: user.username,
          isShared: false,
          canModify: false
        }));
  }

    function onSetUsersSharedWith() {
        // Calculated Attribute library users
        let calculatedAttributeLibraryUsers: CalculatedAttributeLibraryUser[] = [];
        CalculatedAttributeService.getCalculatedAttributeLibraryUsers(props.dialogData.calculatedAttributeLibrary.id).then(response => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString()) && response.data)
            {
                let libraryUsers = response.data as LibraryUser[];
                if (libraryUsers !== null && libraryUsers !== undefined) {

                    libraryUsers.forEach((libraryUser, index) => {
                        //determine access level
                        let canModify = false; if (libraryUser.accessLevel == 1) { canModify = true; }
                        let isOwner = false; if (libraryUser.accessLevel == 2) { isOwner = true; }

                        //create library user object
                        let calculatedAttributeLibraryUser: CalculatedAttributeLibraryUser = {
                            userId: libraryUser.userId,
                            username: libraryUser.userName,
                            canModify: canModify,
                            isOwner: isOwner,
                        }

                        //add library user to an array
                        calculatedAttributeLibraryUsers.push(calculatedAttributeLibraryUser);
                    });
                }

                //fill grid
                const currentUser: string = getUserName();
                const isCurrentUserOrOwner = (calculatedAttributeLibraryUser: CalculatedAttributeLibraryUser) => calculatedAttributeLibraryUser.username === currentUser || calculatedAttributeLibraryUser.isOwner;
                const isNotCurrentUserOrOwner = (calculatedAttributeLibraryUser: CalculatedAttributeLibraryUser) => calculatedAttributeLibraryUser.username !== currentUser && !calculatedAttributeLibraryUser.isOwner;

                currentUserAndOwner = filter(isCurrentUserOrOwner, calculatedAttributeLibraryUsers) as CalculatedAttributeLibraryUser[];
                const otherUsers: CalculatedAttributeLibraryUser[] = filter(isNotCurrentUserOrOwner, calculatedAttributeLibraryUsers) as CalculatedAttributeLibraryUser[];

                otherUsers.forEach((calculatedAttributeLibraryUser: CalculatedAttributeLibraryUser) => {
                    if (any(propEq('id', calculatedAttributeLibraryUser.userId), shareCalculatedAttributeLibraryUserGridRows)) {
                        const calculatedAttributeLibraryUserGridRow: CalculatedAttributeLibraryUserGridRow = find(
                            propEq('id', calculatedAttributeLibraryUser.userId), shareCalculatedAttributeLibraryUserGridRows) as CalculatedAttributeLibraryUserGridRow;

                        shareCalculatedAttributeLibraryUserGridRows = update(
                            findIndex(propEq('id', calculatedAttributeLibraryUser.userId), shareCalculatedAttributeLibraryUserGridRows),
                            { ...calculatedAttributeLibraryUserGridRow, isShared: true, canModify: calculatedAttributeLibraryUser.canModify },
                            shareCalculatedAttributeLibraryUserGridRows
                        );
                    }
                });
            }
        });
  }

  function removeUserModifyAccess(userId: string, isShared: boolean) {
    if (!isShared) {
      shareCalculatedAttributeLibraryUserGridRows = setItemPropertyValueInList(
          findIndex(propEq('id', userId), shareCalculatedAttributeLibraryUserGridRows),
          'canModify', false, shareCalculatedAttributeLibraryUserGridRows);
    }
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', getCalculatedAttributeLibraryUsers());
    } else {
      emit('submit', null);
    }

    shareCalculatedAttributeLibraryUserGridRows = [];
  }

  function getCalculatedAttributeLibraryUsers() {
    const usersSharedWith: CalculatedAttributeLibraryUser[] = shareCalculatedAttributeLibraryUserGridRows
        .filter((calculatedAttributeLibraryUserGridRow: CalculatedAttributeLibraryUserGridRow) => calculatedAttributeLibraryUserGridRow.isShared)
        .map((calculatedAttributeLibraryUserGridRow: CalculatedAttributeLibraryUserGridRow) => ({
          userId: calculatedAttributeLibraryUserGridRow.id,
          username: calculatedAttributeLibraryUserGridRow.username,
          canModify: calculatedAttributeLibraryUserGridRow.canModify,
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
