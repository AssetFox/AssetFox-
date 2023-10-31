<template>
  <v-dialog max-width="500px" persistent v-bind:show="dialogData.showDialog">
    <v-card>
      <v-card-title>
        <v-row justify-center>
          <h5>Deficient Condition Goal Library Sharing</h5>
        </v-row>
          <v-btn @click="onSubmit(false)" variant = "flat" class="ghd-close-button">
            X
          </v-btn>
      </v-card-title>
      <v-card-text>
        <v-data-table id="ShareDeficientConditionGoalLibraryDialog-table-vdatatable" 
                      :headers="deficientConditionGoalLibraryUserGridHeaders"
                      :items="deficientConditionGoalLibraryUserGridRows"
                      :search="searchTerm">
          <template slot="items" slot-scope="props" v-slot:item="{item}">
            <td>
              {{ item.username }}
            </td>
            <td>
              <v-checkbox id="ShareDeficientConditionGoalLibraryDialog-isShared-vcheckbox" label="Is Shared" v-model="item.isShared"
                          @change="removeUserModifyAccess(item.id, item.isShared)"/>
            </td>
            <td>
              <v-checkbox id="ShareDeficientConditionGoalLibraryDialog-canModify-vcheckbox" :disabled="!item.value.isShared" label="Can Modify" v-model="item.canModify"/>
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
        <v-row row justify-center>
          <v-btn id="ShareDeficientConditionGoalLibraryDialog-cancel-vbtn" @click="onSubmit(false)" class="ghd-white-bg ghd-blue ghd-button-text" variant = "flat">Cancel</v-btn>
          <v-btn id="ShareDeficientConditionGoalLibraryDialog-save-vbtn" @click="onSubmit(true)" class="ghd-white-bg ghd-blue ghd-button-text ghd-blue-border ghd-text-padding">
            Save
          </v-btn>
        </v-row>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { watch, toRefs, ref, computed } from 'vue';
import {any, find, findIndex, propEq, update, filter} from 'ramda';
import {DeficientConditionGoalLibraryUser } from '@/shared/models/iAM/deficient-condition-goal';
import {LibraryUser } from '@/shared/models/iAM/user';
import {User} from '@/shared/models/iAM/user';
import {hasValue} from '@/shared/utils/has-value-util';
import {getUserName} from '@/shared/utils/get-user-info';
import {setItemPropertyValueInList} from '@/shared/utils/setter-utils';
import {DataTableHeader} from '@/shared/models/vue/data-table-header';
import {DeficientConditionGoalLibraryUserGridRow, ShareDeficientConditionGoalLibraryDialogData } from '@/shared/models/modals/share-deficient-condition-goal-data';
import DeficientConditionGoalService from '@/services/deficient-condition-goal.service';
import { http2XX } from '@/shared/utils/http-utils';
import { useStore } from 'vuex';

let store = useStore();

const props = defineProps<{
  dialogData: ShareDeficientConditionGoalLibraryDialogData
}>()
const { dialogData } = toRefs(props);
const emit = defineEmits(['submit']);

const stateUsers = computed(() => store.state.userModule.users as User[]);

  let deficientConditionGoalLibraryUserGridHeaders: any[] = [
    {title: 'Username', key: 'username', align: 'left', sortable: true, class: '', width: ''},
    {title: 'Shared With', key: '', align: 'left', sortable: true, class: '', width: ''},
    {title: 'Can Modify', key: '', align: 'left', sortable: true, class: '', width: ''}
  ];
  const deficientConditionGoalLibraryUserGridRows = ref<DeficientConditionGoalLibraryUserGridRow[]>([]);
  const currentUserAndOwner = ref<DeficientConditionGoalLibraryUser[]>([]);
  let searchTerm: string = '';

  watch(dialogData, () => {
    if (dialogData.value.showDialog) {
      onSetGridData();
      onSetUsersSharedWith();
    }
  });

  function onSetGridData() {
    const currentUser: string = getUserName();

    deficientConditionGoalLibraryUserGridRows.value = stateUsers.value
        .filter((user: User) => user.username !== currentUser)
        .map((user: User) => ({
          id: user.id,
          username: user.username,
          isShared: false,
          canModify: false
        }));
  }

  function onSetUsersSharedWith() {
        // Deficient Condition Goal library users
        let deficientConditionGoalLibraryUsers: DeficientConditionGoalLibraryUser[] = [];
        DeficientConditionGoalService.getDeficientConditionGaolLibraryUsers(dialogData.value.deficientConditionGoalLibrary.id).then(response => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString()) && response.data)
            {
                let libraryUsers = response.data as LibraryUser[];
                if (libraryUsers !== null && libraryUsers !== undefined) {

                    libraryUsers.forEach((libraryUser, index) => {
                        //determine access level
                        let canModify = false; if (libraryUser.accessLevel == 1) { canModify = true; }
                        let isOwner = false; if (libraryUser.accessLevel == 2) { isOwner = true; }

                        //create library user object
                        let deficientConditionGoalLibraryUser: DeficientConditionGoalLibraryUser = {
                            userId: libraryUser.userId,
                            username: libraryUser.userName,
                            canModify: canModify,
                            isOwner: isOwner,
                        }

                        //add library user to an array
                        deficientConditionGoalLibraryUsers.push(deficientConditionGoalLibraryUser);
                    });
                }

                //fill grid
                const currentUser: string = getUserName();
                const isCurrentUserOrOwner = (deficientConditionGoalLibraryUser: DeficientConditionGoalLibraryUser) => deficientConditionGoalLibraryUser.username === currentUser || deficientConditionGoalLibraryUser.isOwner;
                const isNotCurrentUserOrOwner = (deficientConditionGoalLibraryUser: DeficientConditionGoalLibraryUser) => deficientConditionGoalLibraryUser.username !== currentUser && !deficientConditionGoalLibraryUser.isOwner;

                currentUserAndOwner.value = filter(isCurrentUserOrOwner, deficientConditionGoalLibraryUsers) as DeficientConditionGoalLibraryUser[];
                const otherUsers: DeficientConditionGoalLibraryUser[] = filter(isNotCurrentUserOrOwner, deficientConditionGoalLibraryUsers) as DeficientConditionGoalLibraryUser[];

                otherUsers.forEach((deficientConditionGoalLibraryUser: DeficientConditionGoalLibraryUser) => {
                    if (any(propEq('id', deficientConditionGoalLibraryUser.userId), deficientConditionGoalLibraryUserGridRows.value)) {
                        const deficientConditionGoalLibraryUserGridRow: DeficientConditionGoalLibraryUserGridRow = find(
                            propEq('id', deficientConditionGoalLibraryUser.userId), deficientConditionGoalLibraryUserGridRows.value) as DeficientConditionGoalLibraryUserGridRow;

                        deficientConditionGoalLibraryUserGridRows.value = update(
                            findIndex(propEq('id', deficientConditionGoalLibraryUser.userId), deficientConditionGoalLibraryUserGridRows.value),
                            { ...deficientConditionGoalLibraryUserGridRow, isShared: true, canModify: deficientConditionGoalLibraryUser.canModify },
                            deficientConditionGoalLibraryUserGridRows.value
                        );
                    }
                });
            }
        });
  }

  function removeUserModifyAccess(userId: string, isShared: boolean) {
    if (!isShared) {
      deficientConditionGoalLibraryUserGridRows.value = setItemPropertyValueInList(
          findIndex(propEq('id', userId), deficientConditionGoalLibraryUserGridRows.value),
          'canModify', false, deficientConditionGoalLibraryUserGridRows.value);
    }
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', getDeficientConditionGoalLibraryUsers());
    } else {
      emit('submit', null);
    }

    deficientConditionGoalLibraryUserGridRows.value = [];
  }

  function getDeficientConditionGoalLibraryUsers() {
    const usersSharedWith: DeficientConditionGoalLibraryUser[] = deficientConditionGoalLibraryUserGridRows.value
        .filter((deficientConditionGoalLibraryUserGridRow: DeficientConditionGoalLibraryUserGridRow) => deficientConditionGoalLibraryUserGridRow.isShared)
        .map((deficientConditionGoalLibraryUserGridRow: DeficientConditionGoalLibraryUserGridRow) => ({
          userId: deficientConditionGoalLibraryUserGridRow.id,
          username: deficientConditionGoalLibraryUserGridRow.username,
          canModify: deficientConditionGoalLibraryUserGridRow.canModify,
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
