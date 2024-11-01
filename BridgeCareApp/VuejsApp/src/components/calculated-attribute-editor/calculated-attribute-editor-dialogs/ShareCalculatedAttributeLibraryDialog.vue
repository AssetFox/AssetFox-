<template>
  <v-dialog max-width="600px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title>
        <v-row justify="space-between" align="center" style="margin-top: 10px;">
          <h5>Calculated Attribute Library Sharing</h5>
          <XButton @click="onSubmit(false)"/>
        </v-row>
      </v-card-title>
      <v-card-text>
        <v-data-table id="ShareCalculatedAttributeLibraryDialog-table-vdatatable"
                      :headers="shareCalculatedAttributeLibraryUserGridHeaders"
                      :items="shareCalculatedAttributeLibraryUserGridRows"
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
              <th style="font-weight: bold;" v-for="header in shareCalculatedAttributeLibraryUserGridHeaders" :key="header.title">
                  {{header.title}}
              </th>
            </tr>
          </template>
          <template slot="items" slot-scope="props" v-slot:item="{item}">
            <tr>
            <td>
              {{ item.username }}
            </td>
            <td>
              <v-checkbox v-model="item.isShared" id="ShareCalculatedAttributeLibraryDialog-isShared-vcheckbox"
                          @change="removeUserModifyAccess(item.id, item.isShared)"/>
            </td>
            <td>
              <v-checkbox :disabled="!dialogData.showDialog" v-model="item.canModify" id="ShareCalculatedAttributeLibraryDialog-canModify-vcheckbox"/>
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
import { toRefs, ref, watch, computed } from 'vue';
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
import SaveButton from '@/shared/components/buttons/SaveButton.vue';
import CancelButton from '@/shared/components/buttons/CancelButton.vue';
import XButton from '@/shared/components/buttons/XButton.vue';

const emit = defineEmits(['submit'])
let store = useStore();
const props = defineProps<{
    dialogData: ShareCalculatedAttributeLibraryDialogData
  }>()
const {dialogData } = toRefs(props);
const stateUsers = computed<User[]>(() => store.state.userModule.users);
const shareCalculatedAttributeLibraryUserGridHeaders = ref<any[]>([
    {title: 'Username', key: 'username', align: 'left', sortable: true, class: '', width: ''},
    {title: 'Shared With', key: 'isShared', align: 'left', sortable: true, class: '', width: ''},
    {title: 'Can Modify', key: 'canModify', align: 'left', sortable: true, class: '', width: ''}
  ]);
  const shareCalculatedAttributeLibraryUserGridRows = ref<CalculatedAttributeLibraryUserGridRow[]>([]);
  let currentUserAndOwner: CalculatedAttributeLibraryUser[] = [];
  const searchTerm = ref('');

  watch(dialogData,() => {
    if (dialogData.value.showDialog) {
      onSetGridData();
      onSetUsersSharedWith();
    }
  });

  function onSetGridData() {
    const currentUser: string = getUserName();

    shareCalculatedAttributeLibraryUserGridRows.value = stateUsers.value
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
        CalculatedAttributeService.getCalculatedAttributeLibraryUsers(dialogData.value.calculatedAttributeLibrary.id).then(response => {
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
                    if (any(propEq('id', calculatedAttributeLibraryUser.userId), shareCalculatedAttributeLibraryUserGridRows.value)) {
                        const calculatedAttributeLibraryUserGridRow: CalculatedAttributeLibraryUserGridRow = find(
                            propEq('id', calculatedAttributeLibraryUser.userId), shareCalculatedAttributeLibraryUserGridRows.value) as CalculatedAttributeLibraryUserGridRow;

                        shareCalculatedAttributeLibraryUserGridRows.value = update(
                            findIndex(propEq('id', calculatedAttributeLibraryUser.userId), shareCalculatedAttributeLibraryUserGridRows.value),
                            { ...calculatedAttributeLibraryUserGridRow, isShared: true, canModify: calculatedAttributeLibraryUser.canModify },
                            shareCalculatedAttributeLibraryUserGridRows.value
                        );
                    }
                });
            }
        });
  }

  function removeUserModifyAccess(userId: string, isShared: boolean) {
    if (!isShared) {
      shareCalculatedAttributeLibraryUserGridRows.value = setItemPropertyValueInList(
          findIndex(propEq('id', userId), shareCalculatedAttributeLibraryUserGridRows.value),
          'canModify', false, shareCalculatedAttributeLibraryUserGridRows.value);
    }
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', getCalculatedAttributeLibraryUsers());
    } else {
      emit('submit', null);
    }

    shareCalculatedAttributeLibraryUserGridRows.value = [];
  }

  function getCalculatedAttributeLibraryUsers() {
    const usersSharedWith: CalculatedAttributeLibraryUser[] = shareCalculatedAttributeLibraryUserGridRows.value
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
