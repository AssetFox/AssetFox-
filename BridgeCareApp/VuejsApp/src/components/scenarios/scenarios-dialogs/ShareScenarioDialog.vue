<template>
  <v-dialog max-width="500px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title>
        <v-layout justify-center>
          <h3>Scenario Sharing</h3>
        </v-layout>
      </v-card-title>
      <v-card-text>
        <v-data-table :headers="scenarioUserGridHeaders"
                      :items="scenarioUserGridRows"
                      sort-icon=$vuetify.icons.ghd-table-sort
                      :search="searchTerm">
          <template slot="items" slot-scope="props">
            <td>
              <v-label>{{ props.item.username }}</v-label>
            </td>
            <td>
              <v-checkbox class="ghd-padding-top" label="Is Shared" v-model="props.item.isShared"
                          @change="removeUserModifyAccess(props.item.id, props.item.isShared)"/>
            </td>
            <td>
              <v-checkbox :disabled="!props.item.isShared" class="ghd-padding-top" label="Can Modify" v-model="props.item.canModify"/>
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
        <v-layout justify-space-between row>
          <v-btn @click="onSubmit(true)" class="ara-blue-bg white--text">
            Save
          </v-btn>
          <v-btn @click="onSubmit(false)" class="ara-orange-bg white--text">Cancel</v-btn>
        </v-layout>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import Vue from 'vue';
import {Component, Prop, Watch} from 'vue-property-decorator';
import {Action, State} from 'vuex-class';
import {any, find, findIndex, propEq, update, filter} from 'ramda';
import {ScenarioUser} from '@/shared/models/iAM/scenario';
import {User} from '@/shared/models/iAM/user';
import {hasValue} from '@/shared/utils/has-value-util';
import {getUserName} from '@/shared/utils/get-user-info';
import {setItemPropertyValueInList} from '@/shared/utils/setter-utils';
import {DataTableHeader} from '@/shared/models/vue/data-table-header';
import {ScenarioUserGridRow, ShareScenarioDialogData} from '@/shared/models/modals/share-scenario-dialog-data';

@Component
export default class ShareScenarioDialog extends Vue {
  @Prop() dialogData: ShareScenarioDialogData;

  @State(state => state.userModule.users) stateUsers: User[];

  scenarioUserGridHeaders: DataTableHeader[] = [
    {text: 'Username', value: 'username', align: 'left', sortable: true, class: '', width: ''},
    {text: 'Shared With', value: '', align: 'left', sortable: true, class: '', width: ''},
    {text: 'Can Modify', value: '', align: 'left', sortable: true, class: '', width: ''}
  ];
  scenarioUserGridRows: ScenarioUserGridRow[] = [];
  currentUserAndOwner: ScenarioUser[] = [];
  searchTerm: string = '';

  @Watch('dialogData')
  onDialogDataChanged() {
    if (this.dialogData.showDialog) {
      this.onSetGridData();
      this.onSetUsersSharedWith();
    }
  }

  onSetGridData() {
    const currentUser: string = getUserName();

    this.scenarioUserGridRows = this.stateUsers
        .filter((user: User) => user.username !== currentUser)
        .map((user: User) => ({
          id: user.id,
          username: user.username,
          isShared: false,
          canModify: false
        }));
  }

  onSetUsersSharedWith() {
    const currentUser: string = getUserName();
    const isCurrentUserOrOwner = (scenarioUser: ScenarioUser) => scenarioUser.username === currentUser || scenarioUser.isOwner;
    const isNotCurrentUserOrOwner = (scenarioUser: ScenarioUser) => scenarioUser.username !== currentUser && !scenarioUser.isOwner;

    this.currentUserAndOwner = filter(isCurrentUserOrOwner, this.dialogData.scenario.users) as ScenarioUser[];
    const otherUsers: ScenarioUser[] = filter(isNotCurrentUserOrOwner, this.dialogData.scenario.users) as ScenarioUser[];

    otherUsers.forEach((scenarioUser: ScenarioUser) => {
      if (any(propEq('id', scenarioUser.userId), this.scenarioUserGridRows)) {
        const scenarioUserGridRow: ScenarioUserGridRow = find(
            propEq('id', scenarioUser.userId), this.scenarioUserGridRows) as ScenarioUserGridRow;

        this.scenarioUserGridRows = update(
            findIndex(propEq('id', scenarioUser.userId), this.scenarioUserGridRows),
            {...scenarioUserGridRow, isShared: true, canModify: scenarioUser.canModify},
            this.scenarioUserGridRows
        );
      }
    });
  }

  removeUserModifyAccess(userId: string, isShared: boolean) {
    if (!isShared) {
      this.scenarioUserGridRows = setItemPropertyValueInList(
          findIndex(propEq('id', userId), this.scenarioUserGridRows),
          'canModify', false, this.scenarioUserGridRows);
    }
  }

  onSubmit(submit: boolean) {
    if (submit) {
      this.$emit('submit', this.getScenarioUsers());
    } else {
      this.$emit('submit', null);
    }

    this.scenarioUserGridRows = [];
  }

  getScenarioUsers() {
    const usersSharedWith: ScenarioUser[] = this.scenarioUserGridRows
        .filter((scenarioUserGridRow: ScenarioUserGridRow) => scenarioUserGridRow.isShared)
        .map((scenarioUserGridRow: ScenarioUserGridRow) => ({
          userId: scenarioUserGridRow.id,
          username: scenarioUserGridRow.username,
          canModify: scenarioUserGridRow.canModify,
          isOwner: false
        }));

    return [...this.currentUserAndOwner, ...usersSharedWith];
  }
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
