<template>
  <v-dialog max-width="500px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title>
        <v-layout justify-center>
          <h3>Treatment Library Sharing</h3>
        </v-layout>
          <v-btn @click="onSubmit(false)" variant = "flat" class="ghd-close-button">
            X
          </v-btn>
      </v-card-title>
      <v-card-text>
        <v-data-table :headers="treatmentLibraryUserGridHeaders"
                      :items="treatmentLibraryUserGridRows"
                      sort-icon=$vuetify.icons.ghd-table-sort
                      :search="searchTerm">
          <template slot="items" slot-scope="props" v-slot:items="props">
            <td>
              {{ props.item.username }}
            </td>
            <td>
              <v-checkbox label="Is Shared" v-model="props.item.isShared"
                          @change="removeUserModifyAccess(props.item.id, props.item.isShared)"/>
            </td>
            <td>
              <v-checkbox :disabled="!props.item.isShared   " label="Can Modify" v-model="props.item.canModify"/>
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

<script lang="ts" setup>
import Vue from 'vue';
import { inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import {any, find, findIndex, propEq, update, filter} from 'ramda';
import {TreatmentLibraryUser } from '@/shared/models/iAM/treatment';
import {LibraryUser } from '@/shared/models/iAM/user';
import {User} from '@/shared/models/iAM/user';
import {hasValue} from '@/shared/utils/has-value-util';
import {getUserName} from '@/shared/utils/get-user-info';
import {setItemPropertyValueInList} from '@/shared/utils/setter-utils';
import {DataTableHeader} from '@/shared/models/vue/data-table-header';
import {TreatmentLibraryUserGridRow, ShareTreatmentLibraryDialogData } from '@/shared/models/modals/share-treatment-library-dialog-data';
import TreatmentService from '@/services/treatment.service';
import { http2XX } from '@/shared/utils/http-utils';


    const props = defineProps<{dialogData: ShareTreatmentLibraryDialogData}>()
    const dialogData = reactive(props.dialogData);
    const emit = defineEmits(['submit'])
    let store = useStore();
    let stateUsers = ref<User[]>(store.state.userModule.users);

  let treatmentLibraryUserGridHeaders: DataTableHeader[] = [
    {text: 'Username', value: 'username', align: 'left', sortable: true, class: '', width: ''},
    {text: 'Shared With', value: '', align: 'left', sortable: true, class: '', width: ''},
    {text: 'Can Modify', value: '', align: 'left', sortable: true, class: '', width: ''}
  ];
  let treatmentLibraryUserGridRows: TreatmentLibraryUserGridRow[] = [];
  let currentUserAndOwner: TreatmentLibraryUser[] = [];
  let searchTerm: string = '';

  watch(() => props.dialogData, () => onDialogDataChanged)
  async function onDialogDataChanged() {
    if (dialogData.showDialog) {
      onSetGridData();
      onSetUsersSharedWith();
    }
  }

  function onSetGridData() {
    const currentUser: string = getUserName();

    treatmentLibraryUserGridRows = stateUsers.value
        .filter((user: User) => user.username !== currentUser)
        .map((user: User) => ({
          id: user.id,
          username: user.username,
          isShared: false,
          canModify: false
        }));
  }

   function onSetUsersSharedWith() {
        //treatment library users
        let treatmentLibraryUsers: TreatmentLibraryUser[] = [];
        TreatmentService.getTreatmentLibraryUsers(dialogData.treatmentLibrary.id).then(response => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString()) && response.data)
            {
                let libraryUsers = response.data as LibraryUser[];
                if (libraryUsers !== null && libraryUsers !== undefined) {
                    //fill treatment library user collection
                    libraryUsers.forEach((libraryUser, index) => {
                        //determine access level
                        let canModify = false; if (libraryUser.accessLevel == 1) { canModify = true; }
                        let isOwner = false; if (libraryUser.accessLevel == 2) { isOwner = true; }

                        //create library user object
                        let treatmentLibraryUser: TreatmentLibraryUser = {
                            userId: libraryUser.userId,
                            username: libraryUser.userName,
                            canModify: canModify,
                            isOwner: isOwner,
                        }

                        //add library user to an array
                        treatmentLibraryUsers.push(treatmentLibraryUser);
                    });
                }

                //fill grid
                const currentUser: string = getUserName();
                const isCurrentUserOrOwner = (treatmentLibraryUser: TreatmentLibraryUser) => treatmentLibraryUser.username === currentUser || treatmentLibraryUser.isOwner;
                const isNotCurrentUserOrOwner = (treatmentLibraryUser: TreatmentLibraryUser) => treatmentLibraryUser.username !== currentUser && !treatmentLibraryUser.isOwner;

                currentUserAndOwner = filter(isCurrentUserOrOwner, treatmentLibraryUsers) as TreatmentLibraryUser[];
                const otherUsers: TreatmentLibraryUser[] = filter(isNotCurrentUserOrOwner, treatmentLibraryUsers) as TreatmentLibraryUser[];

                otherUsers.forEach((treatmentLibraryUser: TreatmentLibraryUser) => {
                    if (any(propEq('id', treatmentLibraryUser.userId), treatmentLibraryUserGridRows)) {
                        const treatmentLibraryUserGridRow: TreatmentLibraryUserGridRow = find(
                            propEq('id', treatmentLibraryUser.userId), treatmentLibraryUserGridRows) as TreatmentLibraryUserGridRow;

                        treatmentLibraryUserGridRows = update(
                            findIndex(propEq('id', treatmentLibraryUser.userId), treatmentLibraryUserGridRows),
                            { ...treatmentLibraryUserGridRow, isShared: true, canModify: treatmentLibraryUser.canModify },
                            treatmentLibraryUserGridRows
                        );
                    }
                });
            }
        });
  }

  function removeUserModifyAccess(userId: string, isShared: boolean) {
    if (!isShared) {
      treatmentLibraryUserGridRows = setItemPropertyValueInList(
          findIndex(propEq('id', userId), treatmentLibraryUserGridRows),
          'canModify', false, treatmentLibraryUserGridRows);
    }
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', getTreatmentLibraryUsers());
    } else {
      emit('submit', null);
    }

    treatmentLibraryUserGridRows = [];
  }

  function getTreatmentLibraryUsers() {
    const usersSharedWith: TreatmentLibraryUser[] = treatmentLibraryUserGridRows
        .filter((treatmentLibraryUserGridRow: TreatmentLibraryUserGridRow) => treatmentLibraryUserGridRow.isShared)
        .map((treatmentLibraryUserGridRow: TreatmentLibraryUserGridRow) => ({
          userId: treatmentLibraryUserGridRow.id,
          username: treatmentLibraryUserGridRow.username,
          canModify: treatmentLibraryUserGridRow.canModify,
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
