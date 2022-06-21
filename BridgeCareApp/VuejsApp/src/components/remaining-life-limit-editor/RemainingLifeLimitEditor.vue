<template>
    <v-layout column>
        <v-subheader class="ghd-control-label ghd-md-gray">Remaining Life Limit Library</v-subheader>
        <v-flex xs12>
            <v-layout justify-space-between>
                <v-flex row xs6>
                    <v-select
                      class="ghd-select ghd-text-field ghd-text-field-border vs-style"
                      :items="selectListItems"
                       v-model="selectItemValue"
                      outline
                      outlined
                    >
                    </v-select>
                </v-flex>
                <div>
                <v-btn class="ghd-white-bg ghd-blue ghd-button" @click="onShowCreateRemainingLifeLimitDialog" outline>Add Remaining Life Limit</v-btn>
                <v-btn class="ghd-white-bg ghd-blue ghd-button" @click="onShowCreateRemainingLifeLimitLibraryDialog(true)" v-show="!hasScenario" outline>Create New Library</v-btn>
                </div>
            </v-layout>
        </v-flex>
        <div v-show="selectItemValue != null || hasScenario">
            <v-data-table
            :headers="gridHeaders"
            :items="remainingLifeLimits"
            class="elevation-1 fixed-header v-table__overflow"
            v-model="selectedGridRows"
            >
                <template v-slot:headers="props">
                    <tr>
                        <th
                          v-for="header in props.headers"
                          :key="header.text"
                        >
                            {{header.text}}
                        </th>
                    </tr>
                </template>
                <template v-slot:items="props">
                    <tr :active="props.selected" @click="props.selected = !props.selected">
                        <td>
                            <v-edit-dialog
                                :return-value.sync="props.item.attribute"
                                large
                                lazy
                                persistent
                                @save="
                                    onEditRemainingLifeLimitProperty(
                                        props.item,
                                        'attribute',
                                        props.item.attribute,
                                    )
                                "
                            >
                                <v-text-field
                                    readonly
                                    single-line
                                    class="sm-txt"
                                    :value="props.item.attribute"
                                    :rules="[
                                        rules['generalRules'].valueIsNotEmpty,
                                    ]"
                                />
                                <template slot="input">
                                    <v-select
                                        :items="numericAttributeSelectItems"
                                        label="Select an Attribute"
                                        outline
                                        v-model="props.item.attribute"
                                        :rules="[
                                            rules['generalRules']
                                                .valueIsNotEmpty,
                                        ]"
                                    />
                                </template>
                            </v-edit-dialog>
                        </td>
                        <td>
                            <v-edit-dialog
                                :return-value.sync="props.item.value"
                                large
                                lazy
                                persistent
                                @save="
                                    onEditRemainingLifeLimitProperty(
                                        props.item,
                                        'value',
                                        props.item.value,
                                    )
                                "
                            >
                                <v-text-field
                                    readonly
                                    single-line
                                    class="sm-txt"
                                    :value="props.item.value"
                                    :rules="[
                                        rules['generalRules'].valueIsNotEmpty,
                                    ]"
                                />
                                <template slot="input">
                                    <v-text-field
                                        label="Edit"
                                        single-line
                                        :mask="'##########'"
                                        v-model.number="props.item.value"
                                        :rules="[
                                            rules['generalRules']
                                                .valueIsNotEmpty,
                                        ]"
                                    />
                                </template>
                            </v-edit-dialog>
                        </td>
                        <td v-if="props.item.criterionLibrary.mergedCriteriaExpression != '' && props.item.criterionLibrary.mergedCriteriaExpression != null" class="px-0">
                            {{ props.item.criterionLibrary.mergedCriteriaExpression}}
                        </td>
                        <td v-else class="px-0">
                        - 
                        </td>
                        <td class="px-0">
                            <v-icon class="ghd-blue" @click="onShowCriterionLibraryEditorDialog(props.item)">edit</v-icon>
                        </td>
                        <td justify-end>
                            <v-icon class="ghd-blue" @click="onRemoveRemainingLifeLimitIcon(props.item)"> delete </v-icon>
                        </td>
                    </tr>
                </template>
                </v-data-table>
                <v-layout justify-start align-center class="pa-2">
                    <v-text class="ghd-control-text" v-if="totalDataFound > 0">Showing {{ dataPerPage }} of {{ totalDataFound }} results</v-text>
                    <v-text class="ghd-control-text" v-else>No results found!</v-text>
                    <!-- <v-divider vertical class="mx-3"/>
                    <v-btn flat right
                      class="ghd-control-label ghd-blue"
                      @click="onRemoveRemainingLifeLimit"
                    > Delete Selected 
                    </v-btn> -->
                </v-layout>
                <v-divider></v-divider>
                <v-flex v-show="!hasScenario" xs12 class="px-0">
                    <v-subheader class="ghd-control-label ghd-md-gray">Description</v-subheader>
                    <v-textarea
                        class="ghd-control-text ghd-control-border"
                        v-model="selectedRemainingLifeLimitLibrary.description"
                        @input="selectedRemainingLifeLimitLibrary = {...selectedRemainingLifeLimitLibrary, description: $event}"
                        outline
                    >
                    </v-textarea>
                </v-flex>
                <v-layout justify-center row>
                    <v-btn class="ghd-blue" flat v-show="hasScenario" @click="onDiscardChanges">Cancel</v-btn>
                    <v-btn class="ghd-blue" flat v-show="!hasScenario" @click="onShowConfirmDeleteAlert">Delete Library</v-btn>
                    <v-btn class="ghd-white-bg ghd-blue ghd-button" @click="onShowCreateRemainingLifeLimitLibraryDialog(true)" outline>Create as New Library</v-btn>
                    <v-btn class="ghd-blue-bg ghd-white ghd-button" v-show="hasScenario" @click="onUpsertScenarioRemainingLifeLimits" :disabled="disableCrudButton() || !hasUnsavedChanges">Save</v-btn>
                    <v-btn class="ghd-blue-bg ghd-white ghd-button" v-show="!hasScenario" :disabled="disableCrudButton() || !hasUnsavedChanges" @click="onUpsertRemainingLifeLimitLibrary">Update Library</v-btn>
                </v-layout>
        </div>

        <ConfirmDeleteAlert 
          :dialogData="confirmDeleteAlertData"
          @submit="onSubmitConfirmDeleteAlertResult"
        />
        <CreateRemainingLifeLimitLibraryDialog
          :dialogData="createRemainingLifeLimitLibraryDialogData"
          @submit="onSubmitCreateRemainingLifeLimitLibraryDialogResult"
        />
        <CreateRemainingLifeLimitDialog 
          :dialogData="createRemainingLifeLimitDialogData"
          @submit="onAddRemainingLifeLimit"
        />
        <CriterionLibraryEditorDialog 
          :dialogData="criterionLibraryEditorDialogData"
          @submit="onEditRemainingLifeLimitCriterionLibrary"
        />
    </v-layout>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import { Action, State, Getter } from 'vuex-class';
import { Watch } from 'vue-property-decorator';
import {
    emptyRemainingLifeLimit,
    emptyRemainingLifeLimitLibrary,
    RemainingLifeLimit,
    RemainingLifeLimitLibrary,
} from '@/shared/models/iAM/remaining-life-limit';
import { getPropertyValues } from '@/shared/utils/getter-utils';
import { prepend, clone, findIndex, isNil, propEq, update, contains } from 'ramda';
import { hasValue } from '@/shared/utils/has-value-util';
import { SelectItem } from '@/shared/models/vue/select-item';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { Attribute } from '@/shared/models/iAM/attribute';
import CreateRemainingLifeLimitDialog from '@/components/remaining-life-limit-editor/remaining-life-limit-editor-dialogs/CreateRemainingLifeLimitDialog.vue';
import {
    CriterionLibraryEditorDialogData,
    emptyCriterionLibraryEditorDialogData,
} from '@/shared/models/modals/criterion-library-editor-dialog-data';
import CriterionLibraryEditorDialog from '@/shared/modals/CriterionLibraryEditorDialog.vue';
import {
    CreateRemainingLifeLimitLibraryDialogData,
    emptyCreateRemainingLifeLimitLibraryDialogData,
} from '@/shared/models/modals/create-remaining-life-limit-library-dialog-data';
import CreateRemainingLifeLimitLibraryDialog from '@/components/remaining-life-limit-editor/remaining-life-limit-editor-dialogs/CreateRemainingLifeLimitLibraryDialog.vue';
import {
    CreateRemainingLifeLimitDialogData,
    emptyCreateRemainingLifeLimitDialogData,
} from '@/shared/models/modals/create-remaining-life-limit-dialog-data';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
import Alert from '@/shared/modals/Alert.vue';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import {
    InputValidationRules,
    rules,
} from '@/shared/utils/input-validation-rules';
import { setItemPropertyValue } from '@/shared/utils/setter-utils';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { CriterionLibrary } from '@/shared/models/iAM/criteria';
import { ScenarioRoutePaths } from '@/shared/utils/route-paths';
import { getUserName } from '@/shared/utils/get-user-info';

@Component({
    components: {
        CreateRemainingLifeLimitLibraryDialog,
        CreateRemainingLifeLimitDialog,
        CriterionLibraryEditorDialog,
        ConfirmDeleteAlert: Alert,
    },
})
export default class RemainingLifeLimitEditor extends Vue {
    @State(state => state.remainingLifeLimitModule.remainingLifeLimitLibraries)
    stateRemainingLifeLimitLibraries: RemainingLifeLimitLibrary[];
    @State(
        state =>
            state.remainingLifeLimitModule.selectedRemainingLifeLimitLibrary,
    )
    stateSelectedRemainingLifeLimitLibrary: RemainingLifeLimitLibrary;
    @State(state => state.attributeModule.numericAttributes)
    stateNumericAttributes: Attribute[];
    @State(state => state.remainingLifeLimitModule.scenarioRemainingLifeLimits)
    stateScenarioRemainingLifeLimits: RemainingLifeLimit[];
    @State(state => state.unsavedChangesFlagModule.hasUnsavedChanges)
    hasUnsavedChanges: boolean;
    @State(state => state.authenticationModule.isAdmin) isAdmin: boolean;

    @Action('getRemainingLifeLimitLibraries')
    getRemainingLifeLimitLibrariesAction: any;
    @Action('upsertRemainingLifeLimitLibrary')
    upsertRemainingLifeLimitLibraryAction: any;
    @Action('deleteRemainingLifeLimitLibrary')
    deleteRemainingLifeLimitLibraryAction: any;
    @Action('selectRemainingLifeLimitLibrary')
    selectRemainingLifeLimitLibraryAction: any;
    @Action('addErrorNotification') addErrorNotificationAction: any;
    @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;
    @Action('getScenarioRemainingLifeLimits')
    getScenarioRemainingLifeLimitsAction: any;
    @Action('upsertScenarioRemainingLifeLimits')
    upsertScenarioRemainingLifeLimitsAction: any;

    @Getter('getUserNameById') getUserNameByIdGetter: any;

    remainingLifeLimitLibraries: RemainingLifeLimitLibrary[] = [];
    selectedRemainingLifeLimitLibrary: RemainingLifeLimitLibrary = clone(
        emptyRemainingLifeLimitLibrary,
    );
    selectedGridRows: RemainingLifeLimit[] = [];
    selectedRemainingLifeIds: string[] = [];
    selectedScenarioId: string = getBlankGuid();
    selectItemValue: string | null = '';
    selectListItems: SelectItem[] = [];
    hasSelectedLibrary: boolean = false;
    gridHeaders: DataTableHeader[] = [
        {
            text: 'Remaining Life Attribute',
            value: 'attribute',
            align: 'left',
            sortable: true,
            class: '',
            width: '',
        },
        {
            text: 'Limit',
            value: 'value',
            align: 'left',
            sortable: true,
            class: '',
            width: '',
        },
        {
            text: 'Criteria',
            value: 'criteria',
            align: 'left',
            sortable: false,
            class: '',
            width: '50%',
        },
        {
            text: '',
            value: '',
            align: 'left',
            sortable: false,
            class:'',
            width: ''
        },
        {
            text: 'Actions',
            value: 'Actions',
            align: 'right',
            sortable: false,
            class: '',
            width: ''
        }
    ];
    itemsPerPage:number = 5;
    dataPerPage: number = 0;
    totalDataFound: number = 5;
    remainingLifeLimits: RemainingLifeLimit[] = [];
    numericAttributeSelectItems: SelectItem[] = [];
    createRemainingLifeLimitDialogData: CreateRemainingLifeLimitDialogData = clone(
        emptyCreateRemainingLifeLimitDialogData,
    );
    selectedRemainingLifeLimit: RemainingLifeLimit = clone(
        emptyRemainingLifeLimit,
    );
    criterionLibraryEditorDialogData: CriterionLibraryEditorDialogData = clone(
        emptyCriterionLibraryEditorDialogData,
    );
    createRemainingLifeLimitLibraryDialogData: CreateRemainingLifeLimitLibraryDialogData = clone(
        emptyCreateRemainingLifeLimitLibraryDialogData,
    );
    confirmDeleteAlertData: AlertData = clone(emptyAlertData);
    rules: InputValidationRules = rules;
    uuidNIL: string = getBlankGuid();
    hasScenario: boolean = false;
    currentUrl: string = window.location.href;
    hasCreatedLibrary: boolean = false;

    onRemoveRemainingLifeLimitIcon(remainingLifeLimit: RemainingLifeLimit) {
        this.remainingLifeLimits = this.remainingLifeLimits.filter((life: RemainingLifeLimit) =>
        !contains(life.id, remainingLifeLimit.id));
    }

    onRemoveRemainingLifeLimit() {
        this.remainingLifeLimits = this.remainingLifeLimits.filter((life: RemainingLifeLimit) =>
            !contains(life.id, this.selectedRemainingLifeIds),
        );
    }
    @Watch('selectedGridRows')
        onSelectedGridRowsChanged() {
            this.selectedRemainingLifeIds = getPropertyValues('id', this.selectedGridRows,) as string[];
        }
    beforeRouteEnter(to: any, from: any, next: any) {
        next((vm: any) => {
            vm.selectItemValue = null;
            vm.getRemainingLifeLimitLibrariesAction();
            if (to.path.indexOf(ScenarioRoutePaths.RemainingLifeLimit) !== -1) {
                vm.selectedScenarioId = to.query.scenarioId;
                if (vm.selectedScenarioId === vm.uuidNIL) {
                    vm.addErrorNotificationAction({
                        message: 'Found no selected scenario for edit',
                    });
                    vm.$router.push('/Scenarios/');
                }
                vm.hasScenario = true;
                vm.getScenarioRemainingLifeLimitsAction(vm.selectedScenarioId);
            }
        });
    }

    beforeDestroy() {
        this.setHasUnsavedChangesAction({ value: false });
    }

    @Watch('stateRemainingLifeLimitLibraries')
    onStateRemainingLifeLimitLibrariesChanged() {
        this.selectListItems = this.stateRemainingLifeLimitLibraries.map(
            (remainingLifeLimitLibrary: RemainingLifeLimitLibrary) => ({
                text: remainingLifeLimitLibrary.name,
                value: remainingLifeLimitLibrary.id,
            }),
        );
    }

    @Watch('selectItemValue')
    onSelectItemValueChanged() {
        this.selectRemainingLifeLimitLibraryAction({
            libraryId: this.selectItemValue,
        });
    }

    @Watch('stateSelectedRemainingLifeLimitLibrary')
    onStateSelectedRemainingLifeLimitLibraryChanged() {
        this.selectedRemainingLifeLimitLibrary = clone(
            this.stateSelectedRemainingLifeLimitLibrary,
        );
    }

    @Watch('selectedRemainingLifeLimitLibrary')
    onSelectedRemainingLifeLimitLibraryChanged() {
        this.hasSelectedLibrary =
            this.selectedRemainingLifeLimitLibrary.id !== this.uuidNIL;

        if (this.hasScenario) {
            this.remainingLifeLimits = this.selectedRemainingLifeLimitLibrary.remainingLifeLimits.map((remainingLifeLimit: RemainingLifeLimit) => ({
                ...remainingLifeLimit, id: getNewGuid()
            }));
        } else {
            this.remainingLifeLimits = clone(
                this.selectedRemainingLifeLimitLibrary.remainingLifeLimits,
            );
        }
    }

    @Watch('stateScenarioRemainingLifeLimits')
    onStateScenarioRemainingLifeLimitsChanged() {
        if (this.hasScenario) {
            this.remainingLifeLimits = clone(this.stateScenarioRemainingLifeLimits);
        }
    }
    @Watch('remainingLifeLimits')
    onGridDataChanged() {
        const hasUnsavedChanges: boolean = this.hasScenario
            ? hasUnsavedChangesCore('', this.remainingLifeLimits, this.stateScenarioRemainingLifeLimits)
            : hasUnsavedChangesCore('',
                { ...clone(this.selectedRemainingLifeLimitLibrary), remainingLifeLimits: clone(this.remainingLifeLimits) },
                this.stateSelectedRemainingLifeLimitLibrary);

        this.setHasUnsavedChangesAction({ value: hasUnsavedChanges });
        // Update total data found and "showing results portion"
        this.totalDataFound = this.remainingLifeLimits.length;
        (this.totalDataFound < this.itemsPerPage) ? this.dataPerPage = this.totalDataFound : this.dataPerPage = this.itemsPerPage;
    }

    @Watch('stateNumericAttributes')
    onStateNumericAttributesChanged() {
        this.setAttributesSelectListItems();
    }

    mounted() {
        this.setAttributesSelectListItems();
    }

    setAttributesSelectListItems() {
        if (hasValue(this.stateNumericAttributes)) {
            this.numericAttributeSelectItems = this.stateNumericAttributes.map(
                (attribute: Attribute) => ({
                    text: attribute.name,
                    value: attribute.name,
                }),
            );
        }
    }

    getOwnerUserName(): string {

        if (!this.hasCreatedLibrary) {
        return this.getUserNameByIdGetter(this.selectedRemainingLifeLimitLibrary.owner);
        }
        
        return getUserName();
    }

    onShowCreateRemainingLifeLimitLibraryDialog(createAsNewLibrary: boolean) {
        this.createRemainingLifeLimitLibraryDialogData = {
            showDialog: true,
            remainingLifeLimits: createAsNewLibrary
                ? this.selectedRemainingLifeLimitLibrary.remainingLifeLimits
                : [],
        };
    }

    onSubmitCreateRemainingLifeLimitLibraryDialogResult(library: RemainingLifeLimitLibrary) {
        this.createRemainingLifeLimitLibraryDialogData = clone(emptyCreateRemainingLifeLimitLibraryDialogData);

        if (!isNil(library)) {
            this.upsertRemainingLifeLimitLibraryAction({library: library});
            this.hasCreatedLibrary = true;
            this.selectItemValue = library.name;
        }
    }

    onShowCreateRemainingLifeLimitDialog() {
        this.createRemainingLifeLimitDialogData = {
            showDialog: true,
            numericAttributeSelectItems: this.numericAttributeSelectItems,
        };
    }

    onAddRemainingLifeLimit(newRemainingLifeLimit: RemainingLifeLimit) {
        this.createRemainingLifeLimitDialogData = clone(emptyCreateRemainingLifeLimitDialogData);

        if (!isNil(newRemainingLifeLimit)) {
            this.remainingLifeLimits = prepend(newRemainingLifeLimit, this.remainingLifeLimits);
        }
    }

    onEditRemainingLifeLimitProperty(remainingLifeLimit: RemainingLifeLimit, property: string, value: any) {
        this.remainingLifeLimits = update(
            findIndex(propEq('id', remainingLifeLimit.id), this.remainingLifeLimits),
            setItemPropertyValue(property, value, remainingLifeLimit),
            this.remainingLifeLimits,
        );
    }

    onShowCriterionLibraryEditorDialog(remainingLifeLimit: RemainingLifeLimit) {
        this.selectedRemainingLifeLimit = remainingLifeLimit;

        this.criterionLibraryEditorDialogData = {
            showDialog: true,
            libraryId: remainingLifeLimit.criterionLibrary.id,
            isCallFromScenario: this.hasScenario,
            isCriterionForLibrary: !this.hasScenario,
        };
    }

    onEditRemainingLifeLimitCriterionLibrary(
        criterionLibrary: CriterionLibrary,
    ) {
        this.criterionLibraryEditorDialogData = clone(emptyCriterionLibraryEditorDialogData);

        if (!isNil(criterionLibrary) && this.selectedRemainingLifeLimit.id !== this.uuidNIL) {
            this.remainingLifeLimits = update(
                findIndex(
                    propEq('id', this.selectedRemainingLifeLimit.id),
                    this.remainingLifeLimits,
                ),
                {
                    ...this.selectedRemainingLifeLimit,
                    criterionLibrary: criterionLibrary,
                },
                this.remainingLifeLimits,
            );
        }

        this.selectedRemainingLifeLimit = clone(emptyRemainingLifeLimit);
    }

    onUpsertRemainingLifeLimitLibrary() {
        const library: RemainingLifeLimitLibrary = {
            ...clone(this.selectedRemainingLifeLimitLibrary),
            remainingLifeLimits: clone(this.remainingLifeLimits)
        };
        this.upsertRemainingLifeLimitLibraryAction({ library: library, });
    }

    onUpsertScenarioRemainingLifeLimits() {
        this.upsertScenarioRemainingLifeLimitsAction({
            scenarioRemainingLifeLimits: this.remainingLifeLimits,
            scenarioId: this.selectedScenarioId,
        }).then(() => (this.selectItemValue = null));
    }

    onDiscardChanges() {
        this.selectItemValue = null;
        setTimeout(() => {
            if (this.hasScenario) {
                this.remainingLifeLimits = clone(this.stateScenarioRemainingLifeLimits);
            }
        });
    }

    onShowConfirmDeleteAlert() {
        this.confirmDeleteAlertData = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message: 'Are you sure you want to delete?',
        };
    }

    onSubmitConfirmDeleteAlertResult(submit: boolean) {
        this.confirmDeleteAlertData = clone(emptyAlertData);

        if (submit) {
            this.selectItemValue = null;
            this.deleteRemainingLifeLimitLibraryAction({
                libraryId: this.selectedRemainingLifeLimitLibrary.id,
            });
        }
    }

    disableCrudButton() {
        const dataIsValid: boolean = this.remainingLifeLimits.every(
            (remainingLife: RemainingLifeLimit) => {
                return (
                    this.rules['generalRules'].valueIsNotEmpty(
                        remainingLife.value,
                    ) === true &&
                    this.rules['generalRules'].valueIsNotEmpty(
                        remainingLife.attribute,
                    ) === true
                );
            },
        );

        if (this.hasSelectedLibrary) {
            return !(
                this.rules['generalRules'].valueIsNotEmpty(
                    this.selectedRemainingLifeLimitLibrary.name,
                ) === true &&
                dataIsValid);
        }

        return !dataIsValid;
    }
}
</script>
<style scoped>
.vs-style {
    width: 50%;
}
</style>