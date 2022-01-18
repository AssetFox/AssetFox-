<template>
    <v-dialog max-width="450px" persistent v-model="showDialog">
        <v-card elevation="5" outlined class="modal-pop-up-padding">
            <v-card-title>
                <h3 style="display: block; color:rgb(59, 57, 65)">
                    Create new scenario
                </h3>
                <v-spacer></v-spacer>
                <v-btn @click="onSubmit(false)" color="error" icon>
                    <i class="fas fa-times-circle"></i>
                </v-btn>
            </v-card-title>

            <v-card-text>
                  <!-- <v-row class="mx-0"> -->
                    <v-text-field
                        label="Name"
                        outline
                        v-model="newScenario.name"
                    ></v-text-field>
                    <v-checkbox v-model="shared" label="Share with all?" />
                <!-- </v-layout> -->
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn
                        :disabled="newScenario.name === ''"
                        @click="onSubmit(true)"
                        class="ara-blue-bg white--text button-radius"
                    >
                        Save
                    </v-btn>
                    <v-btn
                        @click="onSubmit(false)"
                        class="ara-orange-bg white--text button-radius"
                        >Cancel</v-btn
                    >
                </v-layout>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts">
import Vue from 'vue';
import { Component, Prop, Watch } from 'vue-property-decorator';
import { Action, State } from 'vuex-class';
import { getUserName } from '@/shared/utils/get-user-info';
import { User } from '@/shared/models/iAM/user';
import {
    emptyScenario,
    Scenario,
    ScenarioUser,
} from '@/shared/models/iAM/scenario';
import { getNewGuid } from '@/shared/utils/uuid-utils';
import { hasValue } from '@/shared/utils/has-value-util';
import { find, propEq } from 'ramda';

@Component
export default class CreateScenarioDialog extends Vue {
    @Prop() showDialog: boolean;

    @State(state => state.userModule.users) stateUsers: User[];

    newScenario: Scenario = { ...emptyScenario, id: getNewGuid() };
    shared: boolean = false;

    @Watch('showDialog')
    onShowDialogChanged() {
        this.onModifyScenarioUserAccess();
    }

    @Watch('shared')
    onSetPublic() {
        this.onModifyScenarioUserAccess();
    }

    onModifyScenarioUserAccess() {
        if (this.showDialog) {
            const currentUser: User = find(
                propEq('username', getUserName()),
                this.stateUsers,
            ) as User;
            const owner: ScenarioUser = {
                userId: currentUser.id,
                username: currentUser.username,
                canModify: true,
                isOwner: true,
            };

            this.newScenario = {
                ...this.newScenario,
                users: this.shared
                    ? [
                          owner,
                          ...this.stateUsers
                              .filter(
                                  (user: User) =>
                                      user.username !== currentUser.username,
                              )
                              .map((user: User) => ({
                                  userId: user.id,
                                  username: user.username,
                                  canModify: false,
                                  isOwner: false,
                              })),
                      ]
                    : [owner],
            };
        }
    }

    onSubmit(submit: boolean) {
        if (submit) {
            this.$emit('submit', this.newScenario);
        } else {
            this.$emit('submit', null);
        }

        this.newScenario = { ...emptyScenario, id: getNewGuid() };
        this.shared = false;
    }
}
</script>
