<template>
  <v-dialog style="width: 40%;" persistent v-model="dialogData.showDialog">
    <v-card style="padding: 10px;">
      <v-card-title>
        <v-row justify="space-between" align="center">
          <h5>Target Condition Goal Library Sharing</h5>
          <v-btn @click="onSubmit(false)" variant = "flat" class="ghd-close-button">
            X
          </v-btn>
        </v-row>
      </v-card-title>
      <v-card-text>
        <v-data-table id="ShareTargetConditionGoalLibraryDialog-table-vdatatable" :headers="targetConditionGoalLibraryUserGridHeaders"
                      :items="targetConditionGoalLibraryUserGridRows"
                      :search="searchTerm"
                      :rows-per-page-items=[5,10,25]
                      :items-per-page-options="[
                                        {value: 5, title: '5'},
                                        {value: 10, title: '10'},
                                        {value: 25, title: '25'},
                                    ]">
          <template v-slot:item="{item}">
            <tr>
            <td>
              {{ item.username }}
            </td>
            <td>
              <v-checkbox 
                id="ShareTargetConditionGoalLibraryDialog-isShared-vcheckbox" 
                label="Is Shared" 
                v-model="item.isShared" 
                @update:model-value="removeUserModifyAccess(item.id, item.isShared)" />
            </td>
            <td>
              <v-checkbox id="ShareTargetConditionGoalLibraryDialog-canModify-vcheckbox" :disabled="!item.value.isShared" label="Can Modify" v-model="item.raw.canModify"/>
            </td>
          </tr>
          </template>
          <!-- <v-alert value="true"
                   class="ara-orange-bg"
                   icon="fas fa-exclamation"
                   slot="no-results">
            Your search for "{{ searchTerm }}" found no results.
          </v-alert> -->
        </v-data-table>
      </v-card-text>
        <v-row justify="center">
          <div style="margin: 10px; padding: 10px;">
          <v-btn
            id="ShareTargetConditionGoalLibraryDialog-cancel-vbtn" 
            @click="onSubmit(false)" 
            class="ghd-white-bg ghd-blue ghd-button-text" 
            flat>
              Cancel
          </v-btn>
          <v-btn 
            id="ShareTargetConditionGoalLibraryDialog-save-vbtn"
            variant="outlined"
            @click="onSubmit(true)" 
            class="ghd-white-bg ghd-blue ghd-button-text ghd-blue-border ghd-text-padding">
            Save
          </v-btn>
        </div>
        </v-row>
    </v-card>
  </v-dialog>
</template>

<script lang="ts" setup>
import { ref, onMounted, toRefs, watch, computed } from 'vue';

import {any, find, findIndex, propEq, update, filter} from 'ramda';
import {TargetConditionGoalLibraryUser } from '@/shared/models/iAM/target-condition-goal';
import {LibraryUser } from '@/shared/models/iAM/user';
import {User} from '@/shared/models/iAM/user';
import {hasValue} from '@/shared/utils/has-value-util';
import {getUserName} from '@/shared/utils/get-user-info';
import {setItemPropertyValueInList} from '@/shared/utils/setter-utils';
import {TargetConditionGoalLibraryUserGridRow, ShareTargetConditionGoalLibraryDialogData } from '@/shared/models/modals/share-target-condition-goals-data';
import TargetConditionGoalService from '@/services/target-condition-goal.service';
import { http2XX } from '@/shared/utils/http-utils';
import { useStore } from 'vuex';

  let store = useStore();
  const emit = defineEmits(['submit'])
  const props = defineProps<{dialogData: ShareTargetConditionGoalLibraryDialogData}>()
  const { dialogData } = toRefs(props);

  const stateUsers = computed<User[]>(() => store.state.userModule.users);

  let targetConditionGoalLibraryUserGridHeaders: any[] = [
    {title: 'Username', key: 'username', align: 'start', sortable: true},
    {title: 'Shared With', key: 'Shared With', align: 'left', sortable: true},
    {title: 'Can Modify', key: 'Can Modify', align: 'left', sortable: true}
  ];
  const targetConditionGoalLibraryUserGridRows = ref<TargetConditionGoalLibraryUserGridRow[]>([]);
  const currentUserAndOwner = ref<TargetConditionGoalLibraryUser[]>([]);
  let searchTerm: string = '';
  

  watch(dialogData, () => {
    if (dialogData.value.showDialog) {
      onSetGridData();
      onSetUsersSharedWith();
    }
  });

  function onSetGridData() {
    const currentUser: string = getUserName();

    targetConditionGoalLibraryUserGridRows.value = stateUsers.value
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

                currentUserAndOwner.value = filter(isCurrentUserOrOwner, targetConditionGoalLibraryUsers) as TargetConditionGoalLibraryUser[];
                const otherUsers: TargetConditionGoalLibraryUser[] = filter(isNotCurrentUserOrOwner, targetConditionGoalLibraryUsers) as TargetConditionGoalLibraryUser[];

                otherUsers.forEach((targetConditionGoalLibraryUser: TargetConditionGoalLibraryUser) => {
                    if (any(propEq('id', targetConditionGoalLibraryUser.userId), targetConditionGoalLibraryUserGridRows.value)) {
                        const targetConditionGoalLibraryUserGridRow: TargetConditionGoalLibraryUserGridRow = find(
                            propEq('id', targetConditionGoalLibraryUser.userId), targetConditionGoalLibraryUserGridRows.value) as TargetConditionGoalLibraryUserGridRow;

                        targetConditionGoalLibraryUserGridRows.value = update(
                            findIndex(propEq('id', targetConditionGoalLibraryUser.userId), targetConditionGoalLibraryUserGridRows.value),
                            { ...targetConditionGoalLibraryUserGridRow, isShared: true, canModify: targetConditionGoalLibraryUser.canModify },
                            targetConditionGoalLibraryUserGridRows.value
                        );
                    }
                });
            }
        });
  }

  function removeUserModifyAccess(userId: string, isShared: boolean) {
    if (!isShared) {
      targetConditionGoalLibraryUserGridRows.value = setItemPropertyValueInList(
          findIndex(propEq('id', userId), targetConditionGoalLibraryUserGridRows.value),
          'canModify', false, targetConditionGoalLibraryUserGridRows.value);
    }
  }

  function getTargetConditionGoalLibraryUsers() {
    const usersSharedWith: TargetConditionGoalLibraryUser[] = targetConditionGoalLibraryUserGridRows.value
        .filter((targetConditionGoalLibraryUserGridRow: TargetConditionGoalLibraryUserGridRow) => targetConditionGoalLibraryUserGridRow.isShared)
        .map((targetConditionGoalLibraryUserGridRow: TargetConditionGoalLibraryUserGridRow) => ({
          userId: targetConditionGoalLibraryUserGridRow.id,
          username: targetConditionGoalLibraryUserGridRow.username,
          canModify: targetConditionGoalLibraryUserGridRow.canModify,
          isOwner: false
        }));

    return [...currentUserAndOwner.value, ...usersSharedWith];
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', getTargetConditionGoalLibraryUsers());
    } else {
       emit('submit', null);
    }

    targetConditionGoalLibraryUserGridRows.value = [];
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
