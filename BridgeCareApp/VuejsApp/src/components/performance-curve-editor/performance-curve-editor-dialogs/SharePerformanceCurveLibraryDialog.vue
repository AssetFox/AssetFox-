<template>
  <v-dialog max-width="600px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title>
        <v-row justify="space-between" style="margin-top: 10px;">
          <v-spacer></v-spacer>
          <h3>Performance Curve Library Sharing</h3>
          <v-spacer></v-spacer>
          <XButton @click="onSubmit(false)"/>
        </v-row>
      </v-card-title>
      <v-card-text>
        <v-data-table id="SharePerformanceCurveLibraryDialog-table-vdatatable" :headers="performanceCurveLibraryUserGridHeaders"
                      :items="performanceCurveLibraryUserGridRows"
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
              <th style="font-weight: bold;" v-for="header in performanceCurveLibraryUserGridHeaders" :key="header.title">
                  {{header.title}}
              </th>
            </tr>
          </template>
          <template slot="items" slot-scope="props" v-slot:item="props">
            <tr>
            <td>
              {{ props.item.username }}
            </td>
            <td>
              <v-checkbox id="SharePerformanceCurveLibraryDialog-isShared-vcheckbox" v-model="props.item.isShared"
                          @change="removeUserModifyAccess(props.item.id, props.item.isShared)"/>
            </td>
            <td>
              <v-checkbox id="SharePerformanceCurveLibraryDialog-canModify-vcheckbox" :disabled="!props.item.isShared" v-model="props.item.canModify"/>
            </td>
          </tr>
          </template>
          <!-- <v-alert :model-value="true"
                   class="assetFox-orange-bg"
                   icon="fas fa-exclamation"
                   slot="no-results">
            Your search for "{{ searchTerm }}" found no results.
          </v-alert> -->
        </v-data-table>
      </v-card-text>
      <v-card-actions>
        <v-row justify="center">
          <CancelButton @cancel="onSubmit(false)"/>
          <SaveButton @save="onSubmit(true)"/>
        </v-row>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts" setup>
import { watch, toRefs, computed, ref } from 'vue';
import {any, find, findIndex, propEq, update, filter} from 'ramda';
import {PerformanceCurveLibraryUser } from '@/shared/models/iAM/performance';
import {LibraryUser } from '@/shared/models/iAM/user';
import {User} from '@/shared/models/iAM/user';
import {hasValue} from '@/shared/utils/has-value-util';
import {getUserName} from '@/shared/utils/get-user-info';
import {setItemPropertyValueInList} from '@/shared/utils/setter-utils';
import {PerformanceCurveLibraryUserGridRow, SharePerformanceCurveLibraryDialogData } from '@/shared/models/modals/share-performance-curve-library-dialog-data';
import PerformanceCurveService from '@/services/performance-curve.service';
import { http2XX } from '@/shared/utils/http-utils';
import { useStore } from 'vuex';
import CancelButton from '@/shared/components/buttons/CancelButton.vue';
import SaveButton from '@/shared/components/buttons/SaveButton.vue';
import XButton from '@/shared/components/buttons/XButton.vue';

const emit = defineEmits(['submit'])
let store = useStore();
const props = defineProps<{
  dialogData: SharePerformanceCurveLibraryDialogData
    }>()
const { dialogData } = toRefs(props);
    // let showDialogComputed = computed(() => props.dialogData.showDialog);
const stateUsers = computed<User[]>(() => store.state.userModule.users);
let performanceCurveLibraryUserGridHeaders: any[] = [
    {title: 'Username', key: 'username', align: 'left', sortable: true, class: '', width: ''},
    {title: 'Shared With', key: '', align: 'left', sortable: true, class: '', width: ''},
    {title: 'Can Modify', key: '', align: 'left', sortable: true, class: '', width: ''}
  ];
const performanceCurveLibraryUserGridRows = ref<PerformanceCurveLibraryUserGridRow[]>([]);
let currentUserAndOwner: PerformanceCurveLibraryUser[] = [];
let searchTerm: string = '';

watch(dialogData,()=> {
    if (dialogData.value.showDialog) {
      onSetGridData();
      onSetUsersSharedWith();
    }
  });

  function onSetGridData() {
    const currentUser: string = getUserName();

    performanceCurveLibraryUserGridRows.value = stateUsers.value
        .filter((user: User) => user.username !== currentUser)
        .map((user: User) => ({
          id: user.id,
          username: user.username,
          isShared: false,
          canModify: false
        }));
  }

    function onSetUsersSharedWith() {
        //performance curve library users
        let performanceCurveLibraryUsers: PerformanceCurveLibraryUser[] = [];
        PerformanceCurveService.GetPerformanceCurveLibraryUsers(dialogData.value.performanceCurveLibrary.id).then(response => {
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

                currentUserAndOwner = filter(isCurrentUserOrOwner, performanceCurveLibraryUsers) as PerformanceCurveLibraryUser[];
                const otherUsers: PerformanceCurveLibraryUser[] = filter(isNotCurrentUserOrOwner, performanceCurveLibraryUsers) as PerformanceCurveLibraryUser[];

                otherUsers.forEach((performanceCurveLibraryUser: PerformanceCurveLibraryUser) => {
                    if (any(propEq('id', performanceCurveLibraryUser.userId), performanceCurveLibraryUserGridRows.value)) {
                        const performanceCurveLibraryUserGridRow: PerformanceCurveLibraryUserGridRow = find(
                            propEq('id', performanceCurveLibraryUser.userId), performanceCurveLibraryUserGridRows.value) as PerformanceCurveLibraryUserGridRow;

                        performanceCurveLibraryUserGridRows.value = update(
                            findIndex(propEq('id', performanceCurveLibraryUser.userId), performanceCurveLibraryUserGridRows.value),
                            { ...performanceCurveLibraryUserGridRow, isShared: true, canModify: performanceCurveLibraryUser.canModify },
                            performanceCurveLibraryUserGridRows.value
                        );
                    }
                });
            }
        });
  }

  function removeUserModifyAccess(userId: string, isShared: boolean) {
    if (!isShared) {
      performanceCurveLibraryUserGridRows.value = setItemPropertyValueInList(
          findIndex(propEq('id', userId), performanceCurveLibraryUserGridRows.value),
          'canModify', false, performanceCurveLibraryUserGridRows.value);
    }
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', getPerformanceCurveLibraryUsers());
    } else {
      emit('submit', null);
    }

    performanceCurveLibraryUserGridRows.value = [];
  }

  function getPerformanceCurveLibraryUsers() {
    const usersSharedWith: PerformanceCurveLibraryUser[] = performanceCurveLibraryUserGridRows.value
        .filter((performanceCurveLibraryUserGridRow: PerformanceCurveLibraryUserGridRow) => performanceCurveLibraryUserGridRow.isShared)
        .map((performanceCurveLibraryUserGridRow: PerformanceCurveLibraryUserGridRow) => ({
          userId: performanceCurveLibraryUserGridRow.id,
          username: performanceCurveLibraryUserGridRow.username,
          canModify: performanceCurveLibraryUserGridRow.canModify,
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
