<template>
  <v-dialog max-width="500px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title>
        <v-row justify="space-between" style="margin: 5px;">
          <h3>Treatment Library Sharing</h3>
          <v-btn @click="onSubmit(false)" variant = "flat" class="ghd-close-button">
            X
          </v-btn>
        </v-row>
      </v-card-title>
      <v-card-text>
        <v-data-table :headers="treatmentLibraryUserGridHeaders"
                      :items="treatmentLibraryUserGridRows"
                      sort-icon=ghd-table-sort
                      :search="searchTerm">
          <template slot="items" slot-scope="props" v-slot:item="props">
            <tr>
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
          </tr>
          </template>
        </v-data-table>
      </v-card-text>
        <v-row justify="center" style="margin-bottom: 10px; margin-top: 10px;">
          <v-btn @click="onSubmit(false)" class="ghd-white-bg ghd-blue ghd-button-text" variant = "flat">Cancel</v-btn>
          <v-btn @click="onSubmit(true)" class="ghd-white-bg ghd-blue ghd-button-text ghd-blue-border ghd-text-padding">
            Save
          </v-btn>
        </v-row>
    </v-card>
  </v-dialog>
</template>

<script lang="ts" setup>
import { ref, toRefs, watch, computed } from 'vue';
import { useStore } from 'vuex';
import {any, find, findIndex, propEq, update, filter} from 'ramda';
import {TreatmentLibraryUser } from '@/shared/models/iAM/treatment';
import {LibraryUser } from '@/shared/models/iAM/user';
import {User} from '@/shared/models/iAM/user';
import {hasValue} from '@/shared/utils/has-value-util';
import {getUserName} from '@/shared/utils/get-user-info';
import {setItemPropertyValueInList} from '@/shared/utils/setter-utils';
import {TreatmentLibraryUserGridRow, ShareTreatmentLibraryDialogData } from '@/shared/models/modals/share-treatment-library-dialog-data';
import TreatmentService from '@/services/treatment.service';
import { http2XX } from '@/shared/utils/http-utils';

    const props = defineProps<{dialogData: ShareTreatmentLibraryDialogData}>()
    const { dialogData } = toRefs(props);
    const emit = defineEmits(['submit'])
    let store = useStore();
    const stateUsers = computed<User[]>(() => store.state.userModule.users);

  const treatmentLibraryUserGridHeaders: any[] = [
    {title: 'Username', key: 'username', align: 'left', sortable: true, class: '', width: ''},
    {title: 'Shared With', key: '', align: 'left', sortable: true, class: '', width: ''},
    {title: 'Can Modify', key: '', align: 'left', sortable: true, class: '', width: ''}
  ];
  const treatmentLibraryUserGridRows = ref<TreatmentLibraryUserGridRow[]>([]);
  let currentUserAndOwner: TreatmentLibraryUser[] = [];
  const searchTerm = ref<string>('');

  watch(dialogData, () => {
    if (dialogData.value.showDialog) {
      onSetGridData();
      onSetUsersSharedWith();
    }
  });

  function onSetGridData() {
    const currentUser: string = getUserName();

    treatmentLibraryUserGridRows.value = stateUsers.value
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
        TreatmentService.getTreatmentLibraryUsers(props.dialogData.treatmentLibrary.id).then(response => {
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
                    if (any(propEq('id', treatmentLibraryUser.userId), treatmentLibraryUserGridRows.value)) {
                        const treatmentLibraryUserGridRow: TreatmentLibraryUserGridRow = find(
                            propEq('id', treatmentLibraryUser.userId), treatmentLibraryUserGridRows.value) as TreatmentLibraryUserGridRow;

                        treatmentLibraryUserGridRows.value = update(
                            findIndex(propEq('id', treatmentLibraryUser.userId), treatmentLibraryUserGridRows.value),
                            { ...treatmentLibraryUserGridRow, isShared: true, canModify: treatmentLibraryUser.canModify },
                            treatmentLibraryUserGridRows.value
                        );
                    }
                });
            }
        });
  }

  function removeUserModifyAccess(userId: string, isShared: boolean) {
    if (!isShared) {
      treatmentLibraryUserGridRows.value = setItemPropertyValueInList(
          findIndex(propEq('id', userId), treatmentLibraryUserGridRows.value),
          'canModify', false, treatmentLibraryUserGridRows.value);
    }
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', getTreatmentLibraryUsers());
    } else {
      emit('submit', null);
    }

    treatmentLibraryUserGridRows.value = [];
  }

  function getTreatmentLibraryUsers() {
    const usersSharedWith: TreatmentLibraryUser[] = treatmentLibraryUserGridRows.value
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
