<template>
  <v-dialog max-width="600px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-padding-top-title">
        <v-row justify="space-between">
          <h5>Remaining Life Limit Library Sharing</h5>
          <XButton @click="onSubmit(false)"/>
        </v-row>
      </v-card-title>
      <v-card-text>
        <v-row>
          <v-col>
            <v-data-table-server id="ShareRemainingLifeLimitLibraryDialog-table-vdatatable"
                          :headers="remainingLifeLimitLibraryUserGridHeaders"
                          :items="remainingLifeLimitLibraryUserGridRows"
                          :items-length="remainingLifeLimitLibraryUserGridRows.length"
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
              <th style="font-weight: bold;" v-for="header in remainingLifeLimitLibraryUserGridHeaders" :key="header.title">
                  {{header.title}}
              </th>
            </tr>
          </template>
              <template v-slot:item="props">
                <tr>
                <td>
                  {{ props.item.username }}
                </td>
                <td>
                  <v-checkbox id="ShareRemainingLifeLimitLibraryDialog-isShared-vcheckbox" v-model="props.item.isShared"
                              @change="removeUserModifyAccess(props.item.id, props.item.isShared)"/>
                </td>
                <td>
                  <v-checkbox id="ShareRemainingLifeLimitLibraryDialog-canModify-vcheckbox" :disabled="!props.item.isShared" v-model="props.item.canModify"/>
                </td>
              </tr>
              </template>
              <!-- <v-alert :model-value="true"
                      class="assetFox-orange-bg"
                      icon="fas fa-exclamation"
                      slot="no-results">
                Your search for "{{ searchTerm }}" found no results.
              </v-alert> -->
            </v-data-table-server>
          </v-col>
        </v-row>
      </v-card-text>

      <v-card-actions>
        <v-row justify="center" class="ghd-dialog-padding-bottom-buttons">
          <CancelButton @cancel="onSubmit(false)"/>
          <SaveButton @save="onSubmit(true)"/>
        </v-row>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { watch, ref, toRefs, computed } from 'vue';
import {any, find, findIndex, propEq, update, filter} from 'ramda';
import {RemainingLifeLimitLibraryUser } from '@/shared/models/iAM/remaining-life-limit';
import {LibraryUser } from '@/shared/models/iAM/user';
import {User} from '@/shared/models/iAM/user';
import {hasValue} from '@/shared/utils/has-value-util';
import {getUserName} from '@/shared/utils/get-user-info';
import {setItemPropertyValueInList} from '@/shared/utils/setter-utils';
import {RemainingLifeLimitLibraryUserGridRow, ShareRemainingLifeLimitLibraryDialogData } from '@/shared/models/modals/share-remaining-life-limit-data';
import RemainingLifeLimitService from '@/services/remaining-life-limit.service';
import { http2XX } from '@/shared/utils/http-utils';
import { useStore } from 'vuex';
import SaveButton from '@/shared/components/buttons/SaveButton.vue';
import CancelButton from '@/shared/components/buttons/CancelButton.vue';
import XButton from '@/shared/components/buttons/XButton.vue';

  const props = defineProps<{
    dialogData: ShareRemainingLifeLimitLibraryDialogData
  }>();
  const { dialogData } = toRefs(props);

  const emit = defineEmits(['submit']);
  let store = useStore();
  const stateUsers  = computed<User[]>(()=>store.state.userModule.users);

  let remainingLifeLimitLibraryUserGridHeaders: any[] = [
    {title: 'Username', key: 'username', align: 'left', sortable: true, class: '', width: ''},
    {title: 'Shared With', key: 'isShared', align: 'left', sortable: true, class: '', width: ''},
    {title: 'Can Modify', key: 'canModify', align: 'left', sortable: true, class: '', width: ''}
  ];
  const remainingLifeLimitLibraryUserGridRows = ref<RemainingLifeLimitLibraryUserGridRow[]>([]);
  let currentUserAndOwner = ref<RemainingLifeLimitLibraryUser[]>([]);
  let searchTerm: string = '';

  watch(dialogData, () => {
    if (dialogData.value.showDialog) {
      onSetGridData();
      onSetUsersSharedWith();
    }
  });

  function onSetGridData() {
    const currentUser: string = getUserName();
    remainingLifeLimitLibraryUserGridRows.value = stateUsers.value
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
        RemainingLifeLimitService.getRemainingLifeLimitLibraryUsers(dialogData.value.remainingLifeLimitLibrary.id).then(response => {
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

                currentUserAndOwner.value = filter(isCurrentUserOrOwner, remainingLifeLimitLibraryUsers) as RemainingLifeLimitLibraryUser[];
                const otherUsers: RemainingLifeLimitLibraryUser[] = filter(isNotCurrentUserOrOwner, remainingLifeLimitLibraryUsers) as RemainingLifeLimitLibraryUser[];

                otherUsers.forEach((remainingLifeLimitLibraryUser: RemainingLifeLimitLibraryUser) => {
                    if (any(propEq('id', remainingLifeLimitLibraryUser.userId), remainingLifeLimitLibraryUserGridRows.value)) {
                        const remainingLifeLimitLibraryUserGridRow: RemainingLifeLimitLibraryUserGridRow = find(
                            propEq('id', remainingLifeLimitLibraryUser.userId), remainingLifeLimitLibraryUserGridRows.value) as RemainingLifeLimitLibraryUserGridRow;

                        remainingLifeLimitLibraryUserGridRows.value = update(
                            findIndex(propEq('id', remainingLifeLimitLibraryUser.userId), remainingLifeLimitLibraryUserGridRows.value),
                            { ...remainingLifeLimitLibraryUserGridRow, isShared: true, canModify: remainingLifeLimitLibraryUser.canModify },
                            remainingLifeLimitLibraryUserGridRows.value
                        );
                    }
                });
            }
        });
  }

  function removeUserModifyAccess(userId: string, isShared: boolean) {
    if (!isShared) {
      remainingLifeLimitLibraryUserGridRows.value = setItemPropertyValueInList(
          findIndex(propEq('id', userId), remainingLifeLimitLibraryUserGridRows.value),
          'canModify', false, remainingLifeLimitLibraryUserGridRows.value);
    }
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', getRemainingLifeLimitLibraryUsers());
    } else {
      emit('submit', null);
    }

    remainingLifeLimitLibraryUserGridRows.value = [];
  }

  function getRemainingLifeLimitLibraryUsers() {
    const usersSharedWith: RemainingLifeLimitLibraryUser[] = remainingLifeLimitLibraryUserGridRows.value
        .filter((remainingLifeLimitLibraryUserGridRow: RemainingLifeLimitLibraryUserGridRow) => remainingLifeLimitLibraryUserGridRow.isShared)
        .map((remainingLifeLimitLibraryUserGridRow: RemainingLifeLimitLibraryUserGridRow) => ({
          userId: remainingLifeLimitLibraryUserGridRow.id,
          username: remainingLifeLimitLibraryUserGridRow.username,
          canModify: remainingLifeLimitLibraryUserGridRow.canModify,
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
