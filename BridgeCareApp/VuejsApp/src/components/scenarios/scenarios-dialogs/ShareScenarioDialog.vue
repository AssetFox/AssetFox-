<template>
  <v-dialog max-width="500px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title>
        <v-row justify="space-between">
          <h3>Scenario Sharing</h3>
          <v-btn @click="onSubmit(false)" flat>
            <i class="fas fa-times fa-2x"></i>
          </v-btn>
        </v-row>
      </v-card-title>
      <v-card-text>
        <v-data-table id="ShareScenarioDialog-table-vdatatable"
                      :headers="scenarioUserGridHeaders"
                      :items="scenarioUserGridRows"
                      sort-asc-icon="custom:GhdTableSortAscSvg"
                      sort-desc-icon="custom:GhdTableSortDescSvg"
                      :search="searchTerm">
          <template slot="items" slot-scope="props" v-slot:item="{item}">
            <tr>
            <td>
              <v-label>{{ item.username }}</v-label>
            </td>
            <td>
              <v-checkbox id="ShareScenarioDialog-isShared-vcheckbox" 
                  class="ghd-padding-top bottom-margin-zero" label="Is Shared" v-model="item.isShared"
                  @change="removeUserModifyAccess(item.id, item.isShared)"/>
            </td>
            <td>
              <v-checkbox id="ShareScenarioDialog-canModify-vcheckbox" 
                :disabled="!item.isShared" 
                class="ghd-padding-top bottom-margin-zero" 
                label="Can Modify" 
                v-model="item.canModify"/>
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
          <v-btn id="ShareScenarioDialog-cancel-vbtn" @click="onSubmit(false)" class="ghd-button ghd-blue">Cancel</v-btn>
          <v-btn id="ShareScenarioDialog-save-vbtn" @click="onSubmit(true)" class="ghd-white-bg ghd-blue" variant="outlined">
            Save
          </v-btn>
        </v-row>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts" setup>
import { ref, watch, onMounted, toRefs, computed } from 'vue'; 
import {any, find, findIndex, propEq, update, filter} from 'ramda';
import {ScenarioUser} from '@/shared/models/iAM/scenario';
import {User} from '@/shared/models/iAM/user';
import {getUserName} from '@/shared/utils/get-user-info';
import {setItemPropertyValueInList} from '@/shared/utils/setter-utils';
import {ScenarioUserGridRow, ShareScenarioDialogData} from '@/shared/models/modals/share-scenario-dialog-data';
import { useStore } from 'vuex'; 

  let store = useStore(); 

  const props = defineProps<{dialogData: ShareScenarioDialogData}>();
  const emit = defineEmits(['submit'])
  const { dialogData } = toRefs(props);
  const stateUsers = computed<User[]>(() => store.state.userModule.users)

  let scenarioUserGridHeaders: any[] = [
    {title: 'Username', key: 'username', align: 'left', sortable: true, class: '', width: ''},
    {title: 'Shared With', key: '', align: 'left', sortable: true, class: '', width: ''},
    {title: 'Can Modify', key: '', align: 'left', sortable: true, class: '', width: ''}
  ];
  let scenarioUserGridRows = ref<ScenarioUserGridRow[]>([]);
  let currentUserAndOwner: ScenarioUser[] = [];
  let searchTerm = ref('');

  watch(dialogData,()=> {
    if (dialogData.value.showDialog) {
      onSetGridData();
      onSetUsersSharedWith();
    }
  });

  function onSetGridData() {
    const currentUser: string = getUserName();

     scenarioUserGridRows.value =  stateUsers.value
        .filter((user: User) => user.username !== currentUser)
        .map((user: User) => ({
          id: user.id,
          username: user.username,
          isShared: false,
          canModify: false
        }));
  }

  function onSetUsersSharedWith() {
    const currentUser: string = getUserName();
    const isCurrentUserOrOwner = (scenarioUser: ScenarioUser) => scenarioUser.username === currentUser || scenarioUser.isOwner;
    const isNotCurrentUserOrOwner = (scenarioUser: ScenarioUser) => scenarioUser.username !== currentUser && !scenarioUser.isOwner;

    currentUserAndOwner = filter(isCurrentUserOrOwner, props.dialogData.scenario.users) as ScenarioUser[];
    const otherUsers: ScenarioUser[] = filter(isNotCurrentUserOrOwner, props.dialogData.scenario.users) as ScenarioUser[];

    otherUsers.forEach((scenarioUser: ScenarioUser) => {
      if (any(propEq('id', scenarioUser.userId),  scenarioUserGridRows.value)) {
        const scenarioUserGridRow: ScenarioUserGridRow = find(
            propEq('id', scenarioUser.userId),  scenarioUserGridRows.value) as ScenarioUserGridRow;

         scenarioUserGridRows.value = update(
            findIndex(propEq('id', scenarioUser.userId),  scenarioUserGridRows.value),
            {...scenarioUserGridRow, isShared: true, canModify: scenarioUser.canModify},
             scenarioUserGridRows.value
        );
      }
    });
  }

  function removeUserModifyAccess(userId: string, isShared: boolean) {
    if (!isShared) {
       scenarioUserGridRows.value = setItemPropertyValueInList(
          findIndex(propEq('id', userId),  scenarioUserGridRows.value),
          'canModify', false,  scenarioUserGridRows.value);
    }
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', getScenarioUsers());
    } else {
      emit('submit', null);
    }

     scenarioUserGridRows.value = [];
  }

  function getScenarioUsers() {
    const usersSharedWith: ScenarioUser[] =  scenarioUserGridRows.value
        .filter((scenarioUserGridRow: ScenarioUserGridRow) => scenarioUserGridRow.isShared)
        .map((scenarioUserGridRow: ScenarioUserGridRow) => ({
          userId: scenarioUserGridRow.id,
          username: scenarioUserGridRow.username,
          canModify: scenarioUserGridRow.canModify,
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
