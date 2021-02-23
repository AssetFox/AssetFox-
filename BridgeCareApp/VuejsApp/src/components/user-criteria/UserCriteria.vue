<template>
    <v-layout column style="width: 66%; margin: auto">
        <v-flex xs12>
            <v-card class="test1">
                <div v-if="unassignedUsers.length > 0">
                    <v-card-title class="userCriteriaTableHeader">
                        Unassigned Users
                    </v-card-title>
                    <v-card-text style="justify-content: center; text-align: center; padding-top: 0">
                        The following users have no access to any inventory items.
                    </v-card-text>
                    <v-divider style="margin: 0"/>
                    <div v-for="user in unassignedUsers">
                        <v-layout row class="unassigned-user-layout">
                            <v-flex class="unassigned-user-layout-username-flex" xs3>
                                {{user.username}}
                            </v-flex>
                            <v-flex style="padding: 0" xs4>
                                <v-btn @click="onEditCriteria(user)" class="ara-blue-bg white--text"
                                       title="Give the user limited access to the bridge inventory">
                                    <v-icon size="1.5em" style="padding-right: 0.5em">fas fa-edit</v-icon>
                                    Assign Criteria Filter
                                </v-btn>
                            </v-flex>
                            <v-flex justify-center style="padding: 0" xs3>
                                <v-btn @click="onGiveUnrestrictedAccess(user)" class="ara-blue-bg white--text"
                                       title="Allow the user to access the full bridge inventory">
                                    <v-icon size="1.5em" style="padding-right: 0.5em">fas fa-lock-open</v-icon>
                                    Allow All Assets
                                </v-btn>
                            </v-flex>
                            <v-flex justify-center style="padding: 0" xs2>
                                <v-btn @click="onDeleteUser(user)" class="ara-orange" icon title="Delete User">
                                    <v-icon>fas fa-trash</v-icon>
                                </v-btn>
                            </v-flex>
                        </v-layout>
                        <v-divider style="margin: 0"/>
                    </div>
                </div>
                <v-card-title class="userCriteriaTableHeader">
                    Inventory Access Criteria
                </v-card-title>
                <div>
                    <v-data-table :headers="userCriteriaGridHeaders"
                                  :items="assignedUsers"
                                  :items-per-page="5"
                                  class="elevation-1"
                                  hide-actions>
                        <template slot="items" slot-scope="props">
                            <td style="width: 10%; font-size: 1.2em; padding-top: 0.4em">{{ props.item.username }}</td>
                            <td style="width: 25%">
                                <v-layout align-center style="flex-wrap:nowrap; margin-left: 0">
                                    <v-menu bottom
                                            min-height="500px" min-width="500px" v-if="props.item.hasCriteria">
                                        <template slot="activator">
                                            <v-text-field class="sm-txt" :value="props.item.criteria" readonly
                                                          style="width: 25em" type="text"/>
                                        </template>
                                        <v-card>
                                            <v-card-text>
                                                <v-textarea :value="props.item.criteria" full-width no-resize outline
                                                            readonly
                                                            rows="5">
                                                </v-textarea>
                                            </v-card-text>
                                        </v-card>
                                    </v-menu>
                                    <div style="font-size: 1.2em; font-weight: bold; padding-top: 0.4em; padding-right: 1em"
                                         v-if="!props.item.hasCriteria">
                                        All Assets
                                    </div>
                                    <v-btn @click="onEditCriteria(props.item)" class="edit-icon" icon
                                           title="Edit Criteria"
                                           v-if="props.item.hasCriteria">
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </v-layout>
                            </td>
                            <td>
                                <v-btn @click="onEditCriteria(props.item)" class="ara-blue" icon
                                       title="Restrict Access with Criteria Filter"
                                       v-if="!props.item.hasCriteria">
                                    <v-icon>fas fa-lock</v-icon>
                                </v-btn>
                                <v-btn @click="onGiveUnrestrictedAccess(props.item)" class="ara-blue" icon
                                       title="Allow All Assets"
                                       v-if="props.item.hasCriteria">
                                    <v-icon>fas fa-lock-open</v-icon>
                                </v-btn>
                                <v-btn @click="onRevokeAccess(props.item)" class="ara-orange" icon
                                       title="Revoke Access">
                                    <v-icon>fas fa-times-circle</v-icon>
                                </v-btn>
                                <v-btn @click="onDeleteUser(props.item)" class="ara-orange" icon
                                       title="Delete User">
                                    <v-icon>fas fa-trash</v-icon>
                                </v-btn>
                            </td>
                        </template>
                    </v-data-table>
                </div>
            </v-card>
        </v-flex>

        <CriteriaEditorDialog :dialogData="criteriaEditorDialogData"
                              @submitCriteriaEditorDialogResult="onSubmitCriteria"/>

        <Alert :dialogData="beforeDeleteAlertData" @submit="onSubmitDeleteUserResponse"/>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Watch} from 'vue-property-decorator';
    import {Action, State} from 'vuex-class';
    import {AlertData, emptyAlertData} from '@/shared/models/modals/alert-data';
    import Alert from '@/shared/modals/Alert.vue';
    import CriterionLibraryEditorDialog from '@/shared/modals/CriterionLibraryEditorDialog.vue';
    import {
        CriterionLibraryEditorDialogData,
        emptyCriterionLibraryEditorDialogData
    } from '@/shared/models/modals/criterion-library-editor-dialog-data';
    import {isNil} from 'ramda';
    import {emptyUserCriteria, UserCriteria} from '@/shared/models/iAM/user-criteria';
    import {itemsAreEqual} from '@/shared/utils/equals-utils';
    import {getBlankGuid} from '@/shared/utils/uuid-utils';

    @Component({
        components: {
            CriteriaEditorDialog: CriterionLibraryEditorDialog, Alert
        }
    })
    export default class UserCriteriaEditor extends Vue {
        @State(state => state.userCriteria.allUserCriteria) allUserCriteria: UserCriteria[];

        @Action('getAllUserCriteria') getAllUserCriteriaAction: any;
        @Action('setUserCriteria') setUserCriteriaAction: any;
        @Action('deleteUser') deleteUserAction: any;

        beforeDeleteAlertData: AlertData = {...emptyAlertData};
        userCriteriaGridHeaders: object[] = [
            {text: 'User', align: 'left', sortable: false, value: 'username'},
            {text: 'Criteria Filter', sortable: false, value: 'criteria'},
            {text: '', align: 'right', sortable: false, value: 'actions'}
        ];

        unassignedUsers: UserCriteria[] = [];
        assignedUsers: UserCriteria[] = [];
        criteriaEditorDialogData: CriterionLibraryEditorDialogData = {...emptyCriterionLibraryEditorDialogData};
        selectedUser: UserCriteria = {...emptyUserCriteria};
        uuidNIL: string = getBlankGuid();

        created() {
            this.getAllUserCriteriaAction();
        }

        @Watch('allUserCriteria')
        onUserCriteriaChanged() {
            this.unassignedUsers = this.allUserCriteria.filter((user: UserCriteria) => !user.hasAccess);
            this.assignedUsers = this.allUserCriteria.filter((user: UserCriteria) => user.hasAccess);
            this.$forceUpdate();
        }

        onEditCriteria(user: UserCriteria) {
            this.selectedUser = user;

            // TODO: use actual criterion library object id
            this.criteriaEditorDialogData = {
                showDialog: true,
                libraryId: this.uuidNIL//user.criteria === undefined ? '' : user.criteria
            };
        }

        onSubmitCriteria(criteria: string) {
            this.criteriaEditorDialogData = {...emptyCriterionLibraryEditorDialogData};

            if (!isNil(criteria)) {
                const userCriteria = {
                    ...this.selectedUser,
                    criteria,
                    hasAccess: true,
                    hasCriteria: true
                };
                this.setUserCriteriaAction({userCriteria});
            }

            this.selectedUser = {...emptyUserCriteria};
        }

        onRevokeAccess(targetUser: UserCriteria) {
            const userCriteria = {
                ...targetUser,
                criteria: undefined,
                hasAccess: false,
                hasCriteria: false
            };
            this.setUserCriteriaAction({userCriteria});
        }

        onGiveUnrestrictedAccess(targetUser: UserCriteria) {
            const userCriteria = {
                ...targetUser,
                criteria: undefined,
                hasAccess: true,
                hasCriteria: false
            };
            this.setUserCriteriaAction({userCriteria});
        }

        onDeleteUser(user: UserCriteria) {
            this.selectedUser = user;

            this.beforeDeleteAlertData = {
                choice: true,
                heading: 'Delete User',
                message: `Are you sure you want to delete user ${this.selectedUser.username}?`,
                showDialog: true
            };
        }

        onSubmitDeleteUserResponse(doDelete: boolean) {
            this.beforeDeleteAlertData = {...emptyAlertData};

            if (doDelete && !itemsAreEqual(this.selectedUser, emptyUserCriteria)) {
                this.deleteUserAction({user: this.selectedUser.username})
                    .then(() => this.selectedUser = {...emptyUserCriteria});
            }
        }
    }
</script>

<style>
    .unassigned-user-layout {
        margin: auto !important;
        width: 75%;
        text-align: center
    }

    .unassigned-user-layout-username-flex {
        display: flex;
        flex-direction: column;
        justify-content: center;
        font-size: 1.2em;
        padding-top: 0;
        padding-bottom: 0
    }

    .userCriteriaTableHeader {
        justify-content: center;
        font-size: 1.5em;
        font-weight: bold;
    }
</style>
