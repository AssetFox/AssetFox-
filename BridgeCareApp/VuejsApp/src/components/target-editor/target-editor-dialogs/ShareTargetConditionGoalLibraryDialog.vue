<template>
  <v-dialog max-width="500px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title>
        <v-layout justify-center>
          <h3>Target Condition Goal Library Sharing</h3>
        </v-layout>
          <v-btn @click="onSubmit(false)" flat class="ghd-close-button">
            X
          </v-btn>
      </v-card-title>
      <v-card-text>
        <v-data-table :headers="targetConditionGoalLibraryUserGridHeaders"
                      :items="targetConditionGoalLibraryUserGridRows"
                      sort-icon=$vuetify.icons.ghd-table-sort
                      :search="searchTerm">
          <template slot="items" slot-scope="props">
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
          <v-btn @click="onSubmit(false)" class="ghd-white-bg ghd-blue ghd-button-text" depressed>Cancel</v-btn>
          <v-btn @click="onSubmit(true)" class="ghd-white-bg ghd-blue ghd-button-text ghd-blue-border ghd-text-padding">
            Save
          </v-btn>
        </v-layout>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts" setup>
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';

import {any, find, findIndex, propEq, update, filter} from 'ramda';
import {TargetConditionGoalLibraryUser } from '@/shared/models/iAM/target-condition-goal';
import {LibraryUser } from '@/shared/models/iAM/user';
import {User} from '@/shared/models/iAM/user';
import {hasValue} from '@/shared/utils/has-value-util';
import {getUserName} from '@/shared/utils/get-user-info';
import {setItemPropertyValueInList} from '@/shared/utils/setter-utils';
import {DataTableHeader} from '@/shared/models/vue/data-table-header';
import {TargetConditionGoalLibraryUserGridRow, ShareTargetConditionGoalLibraryDialogData } from '@/shared/models/modals/share-target-condition-goals-data';
import TargetConditionGoalService from '@/services/target-condition-goal.service';
import { http2XX } from '@/shared/utils/http-utils';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';

  let store = useStore();
  const emit = defineEmits(['submit'])
  const props = defineProps<{dialogData: ShareTargetConditionGoalLibraryDialogData}>()
  let stateUsers = ref<User[]>(store.state.userModule.users);

  let targetConditionGoalLibraryUserGridHeaders: DataTableHeader[] = [
    {text: 'Username', value: 'username', align: 'left', sortable: true, class: '', width: ''},
    {text: 'Shared With', value: '', align: 'left', sortable: true, class: '', width: ''},
    {text: 'Can Modify', value: '', align: 'left', sortable: true, class: '', width: ''}
  ];
  let targetConditionGoalLibraryUserGridRows: TargetConditionGoalLibraryUserGridRow[] = [];
  let currentUserAndOwner: TargetConditionGoalLibraryUser[] = [];
  let searchTerm: string = '';
  

  watch(props.dialogData,()=>onDialogDataChanged)
  function onDialogDataChanged() {
    if (props.dialogData.showDialog) {
      onSetGridData();
      onSetUsersSharedWith();
    }
  }

  function onSetGridData() {
    const currentUser: string = getUserName();

    targetConditionGoalLibraryUserGridRows = stateUsers.value
        .filter((user: User) => user.username !== currentUser)
        .map((user: User) => ({
          id: user.id,
          username: user.username,
          isShared: false,
          canModify: false
        }));
  }

    function onSetUsersSharedWith() {
        // Target Condition Goal library users
        let targetConditionGoalLibraryUsers: TargetConditionGoalLibraryUser[] = [];
        TargetConditionGoalService.getTargetConditionGoalLibraryUsers(props.dialogData.targetConditionGoalLibrary.id).then(response => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString()) && response.data)
            {
                let libraryUsers = response.data as LibraryUser[];
                if (libraryUsers !== null && libraryUsers !== undefined) {

                    libraryUsers.forEach((libraryUser, index) => {
                        //determine access level
                        let canModify = false; if (libraryUser.accessLevel == 1) { canModify = true; }
                        let isOwner = false; if (libraryUser.accessLevel == 2) { isOwner = true; }

                        //create library user object
                        let targetConditionGoalLibraryUser: TargetConditionGoalLibraryUser = {
                            userId: libraryUser.userId,
                            username: libraryUser.userName,
                            canModify: canModify,
                            isOwner: isOwner,
                        }

                        //add library user to an array
                        targetConditionGoalLibraryUsers.push(targetConditionGoalLibraryUser);
                    });
                }

                //fill grid
                const currentUser: string = getUserName();
                const isCurrentUserOrOwner = (targetConditionGoalLibraryUser: TargetConditionGoalLibraryUser) => targetConditionGoalLibraryUser.username === currentUser || targetConditionGoalLibraryUser.isOwner;
                const isNotCurrentUserOrOwner = (targetConditionGoalLibraryUser: TargetConditionGoalLibraryUser) => targetConditionGoalLibraryUser.username !== currentUser && !targetConditionGoalLibraryUser.isOwner;

                currentUserAndOwner = filter(isCurrentUserOrOwner, targetConditionGoalLibraryUsers) as TargetConditionGoalLibraryUser[];
                const otherUsers: TargetConditionGoalLibraryUser[] = filter(isNotCurrentUserOrOwner, targetConditionGoalLibraryUsers) as TargetConditionGoalLibraryUser[];

                otherUsers.forEach((targetConditionGoalLibraryUser: TargetConditionGoalLibraryUser) => {
                    if (any(propEq('id', targetConditionGoalLibraryUser.userId), targetConditionGoalLibraryUserGridRows)) {
                        const targetConditionGoalLibraryUserGridRow: TargetConditionGoalLibraryUserGridRow = find(
                            propEq('id', targetConditionGoalLibraryUser.userId), targetConditionGoalLibraryUserGridRows) as TargetConditionGoalLibraryUserGridRow;

                        targetConditionGoalLibraryUserGridRows = update(
                            findIndex(propEq('id', targetConditionGoalLibraryUser.userId), targetConditionGoalLibraryUserGridRows),
                            { ...targetConditionGoalLibraryUserGridRow, isShared: true, canModify: targetConditionGoalLibraryUser.canModify },
                            targetConditionGoalLibraryUserGridRows
                        );
                    }
                });
            }
        });
  }

  function removeUserModifyAccess(userId: string, isShared: boolean) {
    if (!isShared) {
      targetConditionGoalLibraryUserGridRows = setItemPropertyValueInList(
          findIndex(propEq('id', userId), targetConditionGoalLibraryUserGridRows),
          'canModify', false, targetConditionGoalLibraryUserGridRows);
    }
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', getTargetConditionGoalLibraryUsers());
    } else {
       emit('submit', null);
    }

    targetConditionGoalLibraryUserGridRows = [];
  }

  function getTargetConditionGoalLibraryUsers() {
    const usersSharedWith: TargetConditionGoalLibraryUser[] = targetConditionGoalLibraryUserGridRows
        .filter((targetConditionGoalLibraryUserGridRow: TargetConditionGoalLibraryUserGridRow) => targetConditionGoalLibraryUserGridRow.isShared)
        .map((targetConditionGoalLibraryUserGridRow: TargetConditionGoalLibraryUserGridRow) => ({
          userId: targetConditionGoalLibraryUserGridRow.id,
          username: targetConditionGoalLibraryUserGridRow.username,
          canModify: targetConditionGoalLibraryUserGridRow.canModify,
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
