<template>
    <v-layout column class="Montserrat-font-family">
        <v-flex xs12>
            <v-layout justify-space-between>
                <v-flex xs4 class="ghd-constant-header">
                    <v-subheader class="ghd-md-gray ghd-control-label">Select a Cash Flow Library</v-subheader>
                    <v-select
                        :items="librarySelectItems"
                        append-icon=$vuetify.icons.ghd-down
                        outline
                        v-model="librarySelectItemValue"
                        class="ghd-select ghd-text-field ghd-text-field-border">
                    </v-select>
                    <div class="ghd-md-gray ghd-control-subheader budget-parent" v-if='hasScenario'>Based on: {{parentLibraryName}}<span v-if="scenarioLibraryIsModified">&nbsp;(Modified)</span></div>  
                </v-flex>
                <v-flex xs4 class="ghd-constant-header">    
                    <div v-if="hasScenario" style="padding-top: 18px !important">
                        <v-btn  
                            class='ghd-blue-bg white--text ghd-button-text ghd-outline-button-padding ghd-button'
                            @click="importLibrary()"
                            :disabled="importLibraryDisabled">
                            Import
                        </v-btn>
                    </div>               
                    <v-layout row v-show='hasSelectedLibrary || hasScenario' style="padding-top: 28px !important">
                        <div v-if='hasSelectedLibrary && !hasScenario' class="header-text-content" style="padding-top: 7px !important">
                            Owner: {{ getOwnerUserName() || '[ No Owner ]' }}
                        </div>
                        <v-divider class="owner-shared-divider" inset vertical
                            v-if='hasSelectedLibrary && selectedScenarioId === uuidNIL'>
                        </v-divider>
                         <v-badge v-show="isShared" style="padding: 10px">
                    <template v-slot: badge>
                        <span>Shared</span>
                        </template>
                        </v-badge>
                        <v-btn @click='onShowShareCashFlowRuleLibraryDialog(selectedCashFlowRuleLibrary)' class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline
                            v-show='!hasScenario'>
                            Share Library
                    </v-btn>
                    </v-layout>  
                </v-flex>
                <v-flex xs4 class="ghd-constant-header">                   
                    <v-layout row align-end style="padding-top: 22px !important">
                        <v-spacer></v-spacer>
                        <v-btn @click="showAddCashFlowRuleDialog = true" v-show="hasSelectedLibrary || hasScenario"
                            id="CashFlowEditor-addCashFlowRule-btn" 
                            outline class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'>
                            Add Cash Flow Rule
                        </v-btn>
                        <v-btn @click="onShowCreateCashFlowRuleLibraryDialog(false)"
                            outline class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                            v-show="!hasScenario">
                            Create New Library
                        </v-btn>
                    </v-layout>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex v-show="hasSelectedLibrary || hasScenario" xs12>
            <div class="cash-flow-library-tables">
                <v-data-table
                    id="CashFlowEditor-cashFlowRules-table"
                    :headers="cashFlowRuleGridHeaders"
                    :items="currentPage"  
                    :pagination.sync="pagination"
                    :must-sort='true'
                    :total-items="totalItems"
                    :rows-per-page-items=[5,10,25]
                    sort-icon=$vuetify.icons.ghd-table-sort
                    v-model='selectedCashRuleGridRows'
                    class="ghd-table v-table__overflow"
                    item-key="id"
                    select-all>
                    <template slot="items" slot-scope="props">
                        <td>
                            <v-checkbox hide-details primary v-model='props.selected'></v-checkbox>
                        </td>
                        <td>
                            <v-edit-dialog
                                :return-value.sync="props.item.name"
                                large
                                lazy
                                @save="onEditSelectedLibraryListData(props.item,'description')"
                                >
                                <v-text-field
                                    id="CashFlowEditor-ruleName-text"
                                    readonly
                                    single-line
                                    class="sm-txt"
                                    :value="props.item.name"
                                    :rules="[rules.generalRules.valueIsNotEmpty]"/>
                                <template slot="input">
                                    <v-textarea
                                        label="Description"
                                        no-resize
                                        outline
                                        rows="5"
                                        :rules="[rules.generalRules.valueIsNotEmpty]"
                                        v-model="props.item.name"/>
                                </template>
                            </v-edit-dialog>
                        </td>
                        <td>
                            <v-layout align-center style='flex-wrap:nowrap'>
                                <v-menu
                                bottom
                                min-height="500px"
                                min-width="500px">
                                <template slot="activator">
                                    <v-text-field
                                        id="CashFlowEditor-criteria-text"
                                        readonly
                                        single-line
                                        class="sm-txt"
                                        :value=" props.item
                                                    .criterionLibrary
                                                    .mergedCriteriaExpression"/>
                                </template>
                                <v-card>
                                    <v-card-text>
                                        <v-textarea
                                            :value="
                                                props.item
                                                    .criterionLibrary
                                                    .mergedCriteriaExpression"
                                            full-width
                                            no-resize
                                            outline
                                            readonly
                                            rows="5"/>
                                    </v-card-text>
                                </v-card>
                            </v-menu>
                            <v-btn
                                @click="onEditCashFlowRuleCriterionLibrary(props.item)"
                                id="CashFlowEditor-editCashFlowRule-btn"
                                class="ghd-blue"
                                icon>
                                <img class='img-general' :src="require('@/assets/icons/edit.svg')"/>
                            </v-btn>
                            </v-layout>
                                                   
                        </td>
                        <td>
                            <v-layout style='flex-wrap:nowrap'>
                                <v-btn
                                @click="onDeleteCashFlowRule(props.item.id)"
                                id="CashFlowEditor-deleteCashFlowRule-btn"
                                class="ghd-blue"
                                icon>
                                <img class='img-general' :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
                            </v-btn>
                            <v-btn
                                @click="onSelectCashFlowRule(props.item.id)"
                                id="CashFlowEditor-editCashFlowRuleDistribution-btn"
                                class="ghd-blue"
                                icon>
                                <img class='img-general' :src="require('@/assets/icons/edit-cash.svg')"/>
                            </v-btn>
                            </v-layout>                          
                        </td>
                    </template>
                </v-data-table>

                <v-btn :disabled='selectedCashRuleGridRows.length === 0' @click='onDeleteSelectedCashFlowRules'
                    class='ghd-blue ghd-button' flat>
                    Delete Selected
                </v-btn>
            </div>
        </v-flex>
        <v-flex v-show="hasSelectedLibrary && !hasScenario" xs12>
            <v-layout justify-center>
                <v-flex>
                    <v-subheader class="ghd-subheader ">Description</v-subheader>
                    <v-textarea
                        class="ghd-text-field-border"
                        no-resize
                        outline
                        rows="4"
                        v-model="selectedCashFlowRuleLibrary.description"
                        @input='checkHasUnsavedChanges()'>
                    </v-textarea>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex xs12>
            <v-layout
                justify-center
                row
                v-show="hasSelectedLibrary || hasScenario">
                <v-btn
                    @click="onDeleteCashFlowRuleLibrary"
                    flat class='ghd-blue ghd-button-text ghd-button'
                    v-show="!hasScenario"
                    :disabled="!hasLibraryEditPermission">
                    Delete Library
                </v-btn>   
                <v-btn
                    @click="onDiscardChanges"
                    v-show="hasScenario"
                    :disabled="!hasUnsavedChanges" flat class='ghd-blue ghd-button-text ghd-button'>
                    Cancel
                </v-btn>
                <v-btn
                    :disabled="disableCrudButtons()"
                    @click="onShowCreateCashFlowRuleLibraryDialog(true)"
                    class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline> 
                    Create as New Library
                </v-btn>
                <v-btn
                    id="CashFlowEditor-save-btn"
                    :disabled="disableCrudButtonsResult || !hasUnsavedChanges"
                    @click="onUpsertScenarioCashFlowRules"
                    class='ghd-blue-bg white--text ghd-button-text ghd-button'
                    v-show="hasScenario">
                    Save
                </v-btn>
                <v-btn
                    :disabled="disableCrudButtonsResult || !hasLibraryEditPermission || !hasUnsavedChanges"
                    @click="onUpsertCashFlowRuleLibrary"
                    class='ghd-blue-bg white--text ghd-button-text ghd-outline-button-padding ghd-button'
                    v-show="!hasScenario">
                    Update Library
                </v-btn>                                       
            </v-layout>
        </v-flex>

        <ConfirmDeleteAlert
            :dialogData="confirmDeleteAlertData"
            @submit="onSubmitConfirmDeleteAlertResult"
        />

        <CreateCashFlowRuleLibraryDialog
            :dialogData="createCashFlowRuleLibraryDialogData"
            @submit="onSubmitCreateCashFlowRuleLibraryDialogResult"
        />

        <GeneralCriterionEditorDialog
            :dialogData="criterionEditorDialogData"
            @submit="onSubmitCriterionLibraryEditorDialogResult"
        />

        <CashFlowRuleEditDialog            
            :showDialog="showRuleEditorDialog"
            :selectedCashFlowRule="selectedCashFlowRule"
            @submit="onSubmitCashFlowRuleEdit"
        />
        <ShareCashFlowRuleLibraryDialog :dialogData="shareCashFlowRuleLibraryDialogData"
            @submit="onShareCashFlowRuleDialogSubmit" 
        />
        <AddCashFlowRuleDialog
            :showDialog="showAddCashFlowRuleDialog"
            @submit="onSubmitAddCashFlowRule"/>
    </v-layout>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import { Watch } from 'vue-property-decorator';
import { Action, State, Getter, Mutation } from 'vuex-class';
import { SelectItem } from '@/shared/models/vue/select-item';
import {
    clone,
    find,
    isNil,
    propEq,
    any,
} from 'ramda';
import {
    CashFlowDistributionRule,
    CashFlowRule,
    CashFlowRuleLibrary,
    CashFlowRuleLibraryUser,
    emptyCashFlowDistributionRule,
    emptyCashFlowRule,
    emptyCashFlowRuleLibrary,
    emptyCashFlowRuleLibraryUsers
} from '@/shared/models/iAM/cash-flow';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import CriterionLibraryEditorDialog from '@/shared/modals/CriterionLibraryEditorDialog.vue';
import {
    CriterionLibraryEditorDialogData,
    emptyCriterionLibraryEditorDialogData,
} from '@/shared/models/modals/criterion-library-editor-dialog-data';
import { emptyShareCashFlowRuleLibraryDialogData, ShareCashFlowRuleLibraryDialogData } from '@/shared/models/modals/share-cash-flow-rule-data';
import {
    CreateCashFlowRuleLibraryDialogData,
    emptyCreateCashFlowLibraryDialogData,
} from '@/shared/models/modals/create-cash-flow-rule-library-dialog-data';
import CreateCashFlowRuleLibraryDialog from '@/components/cash-flow-editor/cash-flow-editor-dialogs/CreateCashFlowRuleLibraryDialog.vue';
import CashFlowRuleEditDialog from '@/components/cash-flow-editor/cash-flow-editor-dialogs/CashFlowRuleEditDialog.vue';
import AddCashFlowRuleDialog from '@/components/cash-flow-editor/cash-flow-editor-dialogs/AddCashFlowRuleDialog.vue';
import { formatAsCurrency } from '@/shared/utils/currency-formatter';
import { hasValue } from '@/shared/utils/has-value-util';
import { getLastPropertyValue } from '@/shared/utils/getter-utils';
import ShareCashFlowRuleLibraryDialog from '@/components/cash-flow-editor/cash-flow-editor-dialogs/ShareCashFlowRuleLibraryDialog.vue';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
import Alert from '@/shared/modals/Alert.vue';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import {
    InputValidationRules,
    rules,
} from '@/shared/utils/input-validation-rules';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { ScenarioRoutePaths } from '@/shared/utils/route-paths';
import { getUserName } from '@/shared/utils/get-user-info';
import { emptyGeneralCriterionEditorDialogData, GeneralCriterionEditorDialogData } from '@/shared/models/modals/general-criterion-editor-dialog-data';
import GeneralCriterionEditorDialog from '@/shared/modals/GeneralCriterionEditorDialog.vue';
import { emptyPagination, Pagination } from '@/shared/models/vue/pagination';
import { LibraryUpsertPagingRequest, PagingPage, PagingRequest } from '@/shared/models/iAM/paging';
import CashFlowService from '@/services/cash-flow.service';
import { AxiosResponse } from 'axios';
import { http2XX } from '@/shared/utils/http-utils';
import { isNullOrUndefined } from 'util';
import { LibraryUser } from '@/shared/models/iAM/user';

@Component({
    components: {
        CreateCashFlowRuleLibraryDialog,
        GeneralCriterionEditorDialog,
        ConfirmDeleteAlert: Alert,
        CashFlowRuleEditDialog,
        ShareCashFlowRuleLibraryDialog,
        AddCashFlowRuleDialog
    },
})
export default class CashFlowEditor extends Vue {
    @State(state => state.cashFlowModule.cashFlowRuleLibraries)
    stateCashFlowRuleLibraries: CashFlowRuleLibrary[];
    @State(state => state.cashFlowModule.selectedCashFlowRuleLibrary)
    stateSelectedCashRuleFlowLibrary: CashFlowRuleLibrary;
    @State(state => state.cashFlowModule.scenarioCashFlowRules)
    stateScenarioCashFlowRules: CashFlowRule[];
    @State(state => state.unsavedChangesFlagModule.hasUnsavedChanges)
    hasUnsavedChanges: boolean;
    @State(state => state.authenticationModule.hasAdminAccess) hasAdminAccess: boolean;
    @State(state => state.cashFlowModule.hasPermittedAccess) hasPermittedAccess: boolean;
    @State(state => state.cashFlowModule.isSharedLibrary) isSharedLibrary: boolean;
    @Action('getIsSharedCashFlowRuleLibrary') getIsSharedLibraryAction: any;
    @Action('getHasPermittedAccess') getHasPermittedAccessAction: any;
    @Action('getCashFlowRuleLibraries') getCashFlowRuleLibrariesAction: any;
    @Action('selectCashFlowRuleLibrary') selectCashFlowRuleLibraryAction: any;
    @Action('upsertCashFlowRuleLibrary') upsertCashFlowRuleLibraryAction: any;
    @Action('deleteCashFlowRuleLibrary') deleteCashFlowRuleLibraryAction: any;
    @Action('addErrorNotification') addErrorNotificationAction: any;
    @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;
    @Action('getScenarioCashFlowRules') getScenarioCashFlowRulesAction: any;
    @Action('upsertScenarioCashFlowRules') upsertScenarioCashFlowRulesAction: any;
    @Action('addSuccessNotification') addSuccessNotificationAction: any;
    @Action('getCurrentUserOrSharedScenario') getCurrentUserOrSharedScenarioAction: any;
    @Action('selectScenario') selectScenarioAction: any;

    @Mutation('cashFlowRuleLibraryMutator') cashFlowRuleLibraryMutator: any;
    @Mutation('selectedCashFlowRuleLibraryMutator') selectedCashFlowRuleLibraryMutator: any;

    @Getter('getUserNameById') getUserNameByIdGetter: any;

    addedRows: CashFlowRule[] = [];
    updatedRowsMap:Map<string, [CashFlowRule, CashFlowRule]> = new Map<string, [CashFlowRule, CashFlowRule]>();//0: original value | 1: updated value
    deletionIds: string[] = [];
    rowCache: CashFlowRule[] = [];
    gridSearchTerm = '';
    currentSearch = '';
    pagination: Pagination = clone(emptyPagination);
    isPageInit = false;
    totalItems = 0;
    currentPage: CashFlowRule[] = [];
    initializing: boolean = true;
    isShared: boolean = false;

    shareCashFlowRuleLibraryDialogData: ShareCashFlowRuleLibraryDialogData = clone(emptyShareCashFlowRuleLibraryDialogData);

    unsavedDialogAllowed: boolean = true;
    trueLibrarySelectItemValue: string | null = ''
    librarySelectItemValueAllowedChanged: boolean = true;
    librarySelectItemValue: string | null = null;
    
    hasSelectedLibrary: boolean = false;
    selectedScenarioId: string = getBlankGuid();
    librarySelectItems: SelectItem[] = [];
    selectedCashFlowRuleLibrary: CashFlowRuleLibrary = clone(
        emptyCashFlowRuleLibrary,
    );
    cashFlowRuleGridHeaders: DataTableHeader[] = [
        {
            text: 'Rule Name',
            value: 'name',
            align: 'left',
            sortable: false,
            class: '',
            width: '25%',
        },
        {
            text: 'Criteria',
            value: 'criterionLibrary',
            align: 'left',
            sortable: false,
            class: '',
            width: '65%',
        },
        {
            text: 'Action',
            value: '',
            align: 'left',
            sortable: false,
            class: '',
            width: '10%',
        },
    ];
    cashFlowRuleGridData: CashFlowRule[] = [];
    selectedCashRuleGridRows: CashFlowRule[] = [];
    cashFlowRuleRadioBtnValue: string = '';
    selectedCashFlowRule: CashFlowRule = clone(emptyCashFlowRule);
    selectedCashFlowRuleForCriteriaEdit: CashFlowRule = clone(
        emptyCashFlowRule,
    );
    cashFlowRuleDistributionGridHeaders: DataTableHeader[] = [
        {
            text: 'Duration (yr)',
            value: 'durationInYears',
            align: 'left',
            sortable: false,
            class: '',
            width: '31.6%',
        },
        {
            text: 'Cost Ceiling',
            value: 'costCeiling',
            align: 'left',
            sortable: false,
            class: '',
            width: '31.6%',
        },
        {
            text: 'Yearly Distribution (%)',
            value: 'yearlyPercentages',
            align: 'left',
            sortable: false,
            class: '',
            width: '31.6%',
        },
        {
            text: '',
            value: '',
            align: 'left',
            sortable: false,
            class: '',
            width: '4.2%',
        },
    ];
    cashFlowDistributionRuleGridData: CashFlowDistributionRule[] = [];
    createCashFlowRuleLibraryDialogData: CreateCashFlowRuleLibraryDialogData = clone(
        emptyCreateCashFlowLibraryDialogData,
    );
    criterionEditorDialogData: GeneralCriterionEditorDialogData = clone(
        emptyGeneralCriterionEditorDialogData,
    );
    confirmDeleteAlertData: AlertData = clone(emptyAlertData);
    rules: InputValidationRules = clone(rules);
    uuidNIL: string = getBlankGuid();
    hasScenario: boolean = false;
    hasCreatedLibrary: boolean = false;
    disableCrudButtonsResult: boolean = false;
    hasLibraryEditPermission: boolean = false;
    showRuleEditorDialog: boolean = false;
    showAddCashFlowRuleDialog: boolean = false;
    importLibraryDisabled: boolean = true;
    scenarioHasCreatedNew: boolean = false;
    loadedParentName: string = "";
    loadedParentId: string = "";
    parentLibraryName: string = "None";
    parentLibraryId: string = "";
    scenarioLibraryIsModified: boolean = false;
    libraryImported: boolean = false;

    beforeRouteEnter(to: any, from: any, next: any) {
        next((vm: any) => {
            vm.librarySelectItemValue = null;
            vm.getCashFlowRuleLibrariesAction().then(() => {
                vm.getHasPermittedAccessAction().then(() => {
                    if (to.path.indexOf(ScenarioRoutePaths.CashFlow) !== -1) {
                        vm.selectedScenarioId = to.query.scenarioId;

                        if (vm.selectedScenarioId === vm.uuidNIL) {
                            vm.addErrorNotificationAction({
                                message: 'Unable to identify selected scenario.',
                            });
                            vm.$router.push('/Scenarios/');
                        }

                        vm.hasScenario = true;
                        vm.getCurrentUserOrSharedScenarioAction({simulationId: vm.selectedScenarioId}).then(() => {         
                            vm.selectScenarioAction({ scenarioId: vm.selectedScenarioId });        
                            vm.initializePages();
                        });                                                
                    }
                });
            })  
        });
    }

    beforeDestroy() {
        this.setHasUnsavedChangesAction({ value: false });
    }

    @Watch('stateCashFlowRuleLibraries')
    onStateCashFlowRuleLibrariesChanged() {
        this.librarySelectItems = this.stateCashFlowRuleLibraries.map(
            (library: CashFlowRuleLibrary) => ({
                text: library.name,
                value: library.id,
            }),
        );
    }

    @Watch('librarySelectItemValue')//import button might break something
    onLibrarySelectItemValueChangedCheckUnsaved(){
        if(this.hasScenario){
            this.onLibrarySelectItemValueChanged();
            this.unsavedDialogAllowed = false;
        }           
        else if(this.librarySelectItemValueAllowedChanged)
            this.CheckUnsavedDialog(this.onLibrarySelectItemValueChanged, () => {
                this.librarySelectItemValueAllowedChanged = false;
                this.librarySelectItemValue = this.trueLibrarySelectItemValue;               
            })
        this.librarySelectItemValueAllowedChanged = true;
        this.librarySelectItems.forEach(library => {
            if (library.value === this.librarySelectItemValue) {
                this.parentLibraryName = library.text;
            }
        });
    }
    onLibrarySelectItemValueChanged() {
        this.trueLibrarySelectItemValue = this.librarySelectItemValue;
        if(!this.hasScenario || isNil(this.librarySelectItemValue))
        {    
            this.selectCashFlowRuleLibraryAction(this.librarySelectItemValue);
        }
        else
        {
            if(!isNil(this.librarySelectItemValue) && !this.scenarioHasCreatedNew)
            {
                this.importLibraryDisabled = false;
            }

            this.scenarioHasCreatedNew = false;
        }
    }

    importLibrary() {
        this.setParentLibraryName(this.librarySelectItemValue ? this.librarySelectItemValue : "");
        this.selectCashFlowRuleLibraryAction(this.librarySelectItemValue);
        this.importLibraryDisabled = true;
        this.scenarioLibraryIsModified = false;
        this.libraryImported = true;
    }

    @Watch('stateSelectedCashRuleFlowLibrary')
    onStateSelectedCashFlowRuleLibraryChanged() {
        this.selectedCashFlowRuleLibrary = clone(
            this.stateSelectedCashRuleFlowLibrary,
        );
    }

    @Watch('selectedCashFlowRuleLibrary')
    onSelectedCashFlowRuleLibraryChanged() {
        this.hasSelectedLibrary =
            this.selectedCashFlowRuleLibrary.id !== this.uuidNIL;

        if (this.hasSelectedLibrary) {
            this.checkLibraryEditPermission();
            this.hasCreatedLibrary = false;
        }
        this.initializing = false;

        if(this.hasSelectedLibrary)
            this.onPaginationChanged();
    }

    @Watch('stateScenarioCashFlowRules')
    onStateScenarioCashFlowRulesChanged() {
        if (this.hasScenario) {
            this.cashFlowRuleGridData = clone(this.stateScenarioCashFlowRules);
        }
    }

    @Watch('cashFlowRuleGridData')
    onCashFlowRuleGridDataChanged() {

    }

    @Watch('selectedCashFlowRule')
    onSelectedSplitTreatmentIdChanged() {
        this.cashFlowDistributionRuleGridData = hasValue(
            this.selectedCashFlowRule.cashFlowDistributionRules,
        )
            ? clone(this.selectedCashFlowRule.cashFlowDistributionRules)
            : [];
    }
    @Watch('isSharedLibrary')
    onStateSharedAccessChanged() {
        this.isShared = this.isSharedLibrary;
        if (!isNullOrUndefined(this.selectCashFlowRuleLibrary)) {
            this.selectCashFlowRuleLibrary.isShared = this.isShared;
        } 
    }
    @Watch('pagination')
    onPaginationChanged() {
        if(this.initializing)
            return;
        this.checkHasUnsavedChanges();
        const { sortBy, descending, page, rowsPerPage } = this.pagination;
        const request: PagingRequest<CashFlowRule>= {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId: this.librarySelectItemValue !== null && this.importLibraryDisabled ? this.librarySelectItemValue : null,
                updateRows: Array.from(this.updatedRowsMap.values()).map(r => r[1]),
                rowsForDeletion: this.deletionIds,
                addedRows: this.addedRows,
                isModified: this.scenarioLibraryIsModified
            },           
            sortColumn: sortBy,
            isDescending: descending != null ? descending : false,
            search: this.currentSearch
        };
        if((!this.hasSelectedLibrary || this.hasScenario) && this.selectedScenarioId !== this.uuidNIL)
            CashFlowService.getScenarioCashFlowRulePage(this.selectedScenarioId, request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<CashFlowRule>;
                    this.currentPage = data.items;
                    this.rowCache = clone(this.currentPage)
                    this.totalItems = data.totalItems;
                }
            });
        else if(this.hasSelectedLibrary)
             CashFlowService.getLibraryCashFlowRulePage(this.librarySelectItemValue !== null ? this.librarySelectItemValue : '', request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<CashFlowRule>;
                    this.currentPage = data.items;
                    this.rowCache = clone(this.currentPage)
                    this.totalItems = data.totalItems;
                    if (!isNullOrUndefined(this.selectedCashFlowRuleLibrary.id) ) {
                        this.getIsSharedLibraryAction(this.selectedCashFlowRuleLibrary).then(this.isShared = this.isSharedLibrary);
                    }

                }
            });     
    }

    @Watch('currentPage')
    onCurrentPageChanged() {
        // Get parent name from library id
        this.librarySelectItems.forEach(library => {
            if (library.value === this.parentLibraryId) {
                this.parentLibraryName = library.text;
            }
        });
    }
    @Watch('deletionIds')
    onDeletionIdsChanged(){
        this.checkHasUnsavedChanges();
    }

    @Watch('addedRows')
    onAddedRowsChanged(){
        this.checkHasUnsavedChanges();
    }

    onSelectCashFlowRule(id:string) {
        const cashFlowRule: CashFlowRule = find(
            propEq('id', id),
            this.currentPage,
        ) as CashFlowRule;

        if (hasValue(cashFlowRule)) {
            this.selectedCashFlowRule = clone(cashFlowRule);
        } else {
            this.selectedCashFlowRule = clone(emptyCashFlowRule);
        }

        this.showRuleEditorDialog = true;
    }

    onShowCreateCashFlowRuleLibraryDialog(createAsNewLibrary: boolean) {
        this.createCashFlowRuleLibraryDialogData = {
            showDialog: true,
            cashFlowRules: createAsNewLibrary ? this.currentPage : [],
        };
    }

    onSubmitCreateCashFlowRuleLibraryDialogResult(
        cashFlowRuleLibrary: CashFlowRuleLibrary,
    ) {
        this.createCashFlowRuleLibraryDialogData = clone(
            emptyCreateCashFlowLibraryDialogData,
        );

        if (!isNil(cashFlowRuleLibrary)) {
            const upsertRequest: LibraryUpsertPagingRequest<CashFlowRuleLibrary, CashFlowRule> = {
                library: cashFlowRuleLibrary,    
                isNewLibrary: true,           
                 syncModel: {
                    libraryId: cashFlowRuleLibrary.cashFlowRules.length == 0 || !this.hasSelectedLibrary ? null : this.selectedCashFlowRuleLibrary.id,
                    rowsForDeletion: cashFlowRuleLibrary.cashFlowRules.length == 0 ? [] : this.deletionIds,
                    updateRows: cashFlowRuleLibrary.cashFlowRules.length == 0 ? [] : Array.from(this.updatedRowsMap.values()).map(r => r[1]),
                    addedRows: cashFlowRuleLibrary.cashFlowRules.length == 0 ? [] : this.addedRows,
                    isModified: false
                 },
                 scenarioId: this.hasScenario ? this.selectedScenarioId : null
            }
            CashFlowService.upsertCashFlowRuleLibrary(upsertRequest).then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    this.hasCreatedLibrary = true;
                    this.librarySelectItemValue = cashFlowRuleLibrary.id;
                    
                    if(cashFlowRuleLibrary.cashFlowRules.length == 0){
                        this.clearChanges();
                    }

                    if(this.hasScenario){
                        this.scenarioHasCreatedNew = true;
                        this.importLibraryDisabled = true;
                    }

                    this.cashFlowRuleLibraryMutator(cashFlowRuleLibrary);
                    this.selectedCashFlowRuleLibraryMutator(cashFlowRuleLibrary.id);
                    this.addSuccessNotificationAction({message:'Added cash flow rule library'})
                }               
            })
        }
    }

    onSubmitCashFlowRuleEdit(CashFlowDistributionRules:CashFlowDistributionRule[])
    {
        this.showRuleEditorDialog = false;
        if(!isNil(CashFlowDistributionRules))
        {
            let selectedRule = this.currentPage.find(o => o.id == this.selectedCashFlowRule.id) 
            if(!isNil(selectedRule))
            {
                selectedRule.cashFlowDistributionRules = hasValue(CashFlowDistributionRules) ? clone(CashFlowDistributionRules) : [];  
                this.onUpdateRow(selectedRule.id, clone(selectedRule))
                this.onPaginationChanged();
            }                
        }              
    }

    onAddCashFlowRule() {
        const newCashFlowRule: CashFlowRule = {
            ...emptyCashFlowRule,
            name: `Unnamed Rule ${this.totalItems + 1}`,
            id: getNewGuid(),
        };

        this.addedRows.push(newCashFlowRule);
        this.onPaginationChanged()
    }

    onSubmitAddCashFlowRule(newCashFlowRule: CashFlowRule){
        if(!isNil(newCashFlowRule))
        {
            this.addedRows.push(newCashFlowRule);
            this.onPaginationChanged()
        }
        this.showAddCashFlowRuleDialog = false;
    }

    onDeleteCashFlowRule(cashFlowRuleId: string) {
        this.removeRowLogic(cashFlowRuleId);
        this.onPaginationChanged();
    }

    onDeleteSelectedCashFlowRules() {
        this.selectedCashRuleGridRows.forEach(_ => {
            this.removeRowLogic(_.id);
        });

        this.selectedTargetConditionGoalIds = [];
        this.onPaginationChanged();
    }

    removeRowLogic(id: string){
        if(isNil(find(propEq('id', id), this.addedRows))){
            this.deletionIds.push(id);
            if(!isNil(this.updatedRowsMap.get(id)))
                this.updatedRowsMap.delete(id)
        }           
        else{          
            this.addedRows = this.addedRows.filter((row) => row.id !== id)
        }  
    }

    checkLibraryEditPermission() {
        this.hasLibraryEditPermission = this.hasAdminAccess || (this.hasPermittedAccess && this.checkUserIsLibraryOwner());
    }

    checkUserIsLibraryOwner() {
        return this.getUserNameByIdGetter(this.selectedCashFlowRuleLibrary.owner) == getUserName();
    }

    getOwnerUserName(): string {

        if (!this.hasCreatedLibrary) {
        return this.getUserNameByIdGetter(this.selectedCashFlowRuleLibrary.owner);
        }
        
        return getUserName();
    }


    onEditCashFlowRuleCriterionLibrary(cashFlowRule: CashFlowRule) {
        this.selectedCashFlowRuleForCriteriaEdit = clone(cashFlowRule);

        this.criterionEditorDialogData = {
            showDialog: true,
            CriteriaExpression: this.selectedCashFlowRuleForCriteriaEdit.criterionLibrary.mergedCriteriaExpression,
        };
    }

    onSubmitCriterionLibraryEditorDialogResult(
        criterionExpression: string,
    ) {
        this.criterionEditorDialogData = clone(
            emptyGeneralCriterionEditorDialogData,
        );

        if (!isNil(criterionExpression) && this.selectedCashFlowRuleForCriteriaEdit.id !== this.uuidNIL) {
            if(this.selectedCashFlowRuleForCriteriaEdit.criterionLibrary.id === getBlankGuid())
                this.selectedCashFlowRuleForCriteriaEdit.criterionLibrary.id = getNewGuid();
            this.onUpdateRow(this.selectedCashFlowRuleForCriteriaEdit.id, 
            {
                ...this.selectedCashFlowRuleForCriteriaEdit,
                criterionLibrary: {...this.selectedCashFlowRuleForCriteriaEdit.criterionLibrary, mergedCriteriaExpression: criterionExpression},
            })
            this.onPaginationChanged();

            this.selectedCashFlowRuleForCriteriaEdit = clone(emptyCashFlowRule);
        }
    }

    onEditSelectedLibraryListData(data: any, property: string) {
        switch (property) {
            case 'description':
                this.onUpdateRow(data.id, clone(data))
                this.onPaginationChanged();
                break;
        }
    }

    onOpenCostCeilingEditDialog(distributionRuleId: string) {
        this.$nextTick(() => {
            const editDialogInputElement: HTMLElement = document.getElementById(
                distributionRuleId,
            ) as HTMLElement;
            if (hasValue(editDialogInputElement)) {
                setTimeout(() => {
                    editDialogInputElement.blur();
                    setTimeout(() => editDialogInputElement.click());
                }, 250);
            }
        });
    }

    onUpsertScenarioCashFlowRules() {
        if (this.selectedCashFlowRuleLibrary.id === this.uuidNIL || this.hasUnsavedChanges && this.libraryImported === false) {this.scenarioLibraryIsModified = true;}
        else { this.scenarioLibraryIsModified = false; }

        CashFlowService.upsertScenarioCashFlowRules({
            libraryId: this.selectedCashFlowRuleLibrary.id === this.uuidNIL ? null : this.selectedCashFlowRuleLibrary.id,
            rowsForDeletion: this.deletionIds,
            updateRows: Array.from(this.updatedRowsMap.values()).map(r => r[1]),
            addedRows: this.addedRows,
            isModified: this.scenarioLibraryIsModified
        }, this.selectedScenarioId).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                this.parentLibraryId = this.librarySelectItemValue ? this.librarySelectItemValue : "";
                this.clearChanges();
                this.librarySelectItemValue = null;
                this.resetPage();
                this.addSuccessNotificationAction({message: "Modified scenario's cash flow rules"});
                this.importLibraryDisabled = true;
                this.libraryImported = false;
            }           
        });
    }

    onUpsertCashFlowRuleLibrary() {
        const cashFlowRuleLibrary: CashFlowRuleLibrary = {
            ...clone(this.selectedCashFlowRuleLibrary),
            cashFlowRules: clone(this.currentPage),
        };

        const upsertRequest: LibraryUpsertPagingRequest<CashFlowRuleLibrary, CashFlowRule> = {
                library: this.selectedCashFlowRuleLibrary,
                isNewLibrary: false,
                syncModel: {
                libraryId: this.selectedCashFlowRuleLibrary.id === this.uuidNIL ? null : this.selectedCashFlowRuleLibrary.id,
                rowsForDeletion: this.deletionIds,
                updateRows: Array.from(this.updatedRowsMap.values()).map(r => r[1]),
                addedRows: this.addedRows,
                isModified: false
                },
                scenarioId: null
        }
        CashFlowService.upsertCashFlowRuleLibrary(upsertRequest).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                this.clearChanges()
                this.resetPage();
                this.cashFlowRuleLibraryMutator(this.selectedCashFlowRuleLibrary);
                this.selectedCashFlowRuleLibraryMutator(this.selectedCashFlowRuleLibrary.id);
                this.addSuccessNotificationAction({message: "Updated cash flow rule library",});
            }
        });
    }

    onDiscardChanges() {
        this.librarySelectItemValue = null;
        this.parentLibraryName = this.loadedParentName;
        this.parentLibraryId = this.loadedParentId;

        setTimeout(() => {
            if (this.hasScenario) {
                this.clearChanges();
                this.resetPage();
                this.importLibraryDisabled = true;
            }
        });
    }

    formatAsCurrency(value: any) {
        if (hasValue(value)) {
            return formatAsCurrency(value);
        }
        return null;
    }

    disableCrudButtons() {
        const rows = this.addedRows.concat(Array.from(this.updatedRowsMap.values()).map(r => r[1]));
        const allDataIsValid = rows.every(
            (rule: CashFlowRule) => {
                const allSubDataIsValid = rule.cashFlowDistributionRules.every(
                    (
                        distributionRule: CashFlowDistributionRule,
                        index: number,
                    ) => {
                        let isValid: boolean =
                            this.rules['generalRules'].valueIsNotEmpty(
                                distributionRule.durationInYears,
                            ) === true &&
                            this.rules['generalRules'].valueIsNotEmpty(
                                distributionRule.costCeiling,
                            ) === true &&
                            this.rules['generalRules'].valueIsNotEmpty(
                                distributionRule.yearlyPercentages,
                            ) === true &&
                            this.rules[
                                'cashFlowRules'
                            ].doesTotalOfPercentsEqualOneHundred(
                                distributionRule.yearlyPercentages,
                            ) === true;

                        if (index !== 0) {
                            isValid =
                                isValid &&
                                this.rules[
                                    'cashFlowRules'
                                ].isDurationGreaterThanPreviousDuration(
                                    distributionRule,
                                    rule,
                                ) === true &&
                                this.rules[
                                    'cashFlowRules'
                                ].isAmountGreaterThanOrEqualToPreviousAmount(
                                    distributionRule,
                                    rule,
                                ) === true;
                        }

                        return isValid;
                    },
                );

                return (
                    this.rules['generalRules'].valueIsNotEmpty(rule.name) ===
                        true && allSubDataIsValid
                );
            },
        );

        if (!this.hasScenario && this.hasSelectedLibrary) {
            return !(
                this.rules['generalRules'].valueIsNotEmpty(
                    this.selectedCashFlowRuleLibrary.name,
                ) === true && allDataIsValid
            );
        }
        this.disableCrudButtonsResult = !allDataIsValid;
        return !allDataIsValid;
    }

    onDeleteCashFlowRuleLibrary() {
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
            this.librarySelectItemValue = null;
            this.deleteCashFlowRuleLibraryAction(
                this.selectedCashFlowRuleLibrary.id,
            );
        }
    }

    //paging

    onUpdateRow(rowId: string, updatedRow: CashFlowRule){
        if(any(propEq('id', rowId), this.addedRows)){
            const index = this.addedRows.findIndex(item => item.id == updatedRow.id)
            this.addedRows[index] = updatedRow;
            return;
        }

        let mapEntry = this.updatedRowsMap.get(rowId)

        if(isNil(mapEntry)){
            const row = this.rowCache.find(r => r.id === rowId);
            if(!isNil(row) && hasUnsavedChangesCore('', updatedRow, row))
                this.updatedRowsMap.set(rowId, [row , updatedRow])
        }
        else if(hasUnsavedChangesCore('', updatedRow, mapEntry[0])){
            mapEntry[1] = updatedRow;
        }
        else
            this.updatedRowsMap.delete(rowId)

        this.checkHasUnsavedChanges();
    }

    clearChanges(){
        this.updatedRowsMap.clear();
        this.addedRows = [];
        this.deletionIds = [];
    }

    resetPage(){
        this.pagination.page = 1;
        this.onPaginationChanged();
    }

    checkHasUnsavedChanges(){
        const hasUnsavedChanges: boolean = 
            this.deletionIds.length > 0 || 
            this.addedRows.length > 0 ||
            this.updatedRowsMap.size > 0 || 
            (this.hasScenario && this.hasSelectedLibrary) ||
            (this.hasSelectedLibrary && hasUnsavedChangesCore('', this.stateSelectedCashRuleFlowLibrary, this.selectedCashFlowRuleLibrary))
        this.setHasUnsavedChangesAction({ value: hasUnsavedChanges });
    }

    CheckUnsavedDialog(next: any, otherwise: any) {
        if (this.hasUnsavedChanges && this.unsavedDialogAllowed) {
            // @ts-ignore
            Vue.dialog
                .confirm(
                    'You have unsaved changes. Are you sure you wish to continue?',
                    { reverse: true },
                )
                .then(() => next())
                .catch(() => otherwise())
        } 
        else {
            this.unsavedDialogAllowed = true;
            next();
        }
    };

    setParentLibraryName(libraryId: string) {
        if (libraryId === "") {
            this.parentLibraryName = "None";
            return;
        }
        let foundLibrary: CashFlowRuleLibrary = emptyCashFlowRuleLibrary;
        this.stateCashFlowRuleLibraries.forEach(library => {
            if (library.id === libraryId ) {
                foundLibrary = clone(library);
            }
        });
        this.parentLibraryId = foundLibrary.id;
        this.parentLibraryName = foundLibrary.name;
    }

    initializePages(){
        const request: PagingRequest<CashFlowRule>= {
            page: 1,
            rowsPerPage: 5,
            syncModel: {
                libraryId: null,
                updateRows: [],
                rowsForDeletion: [],
                addedRows: [],
                isModified: false
            },           
            sortColumn: '',
            isDescending: false,
            search: ''
        };
        if((!this.hasSelectedLibrary || this.hasScenario) && this.selectedScenarioId !== this.uuidNIL)
            CashFlowService.getScenarioCashFlowRulePage(this.selectedScenarioId, request).then(response => {
                this.initializing = false
                if(response.data){
                    let data = response.data as PagingPage<CashFlowRule>;
                    this.currentPage = data.items;
                    this.rowCache = clone(this.currentPage)
                    this.totalItems = data.totalItems;
                    this.setParentLibraryName(this.currentPage.length > 0 ? this.currentPage[0].libraryId : "None");
                    this.loadedParentId = this.currentPage.length > 0 ? this.currentPage[0].libraryId : "";
                    this.loadedParentName = this.parentLibraryName; //store original
                    this.scenarioLibraryIsModified = this.currentPage.length > 0 ? this.currentPage[0].isModified : false;
                }
            });
    }

    onShowShareCashFlowRuleLibraryDialog(cashFlowRuleLibrary: CashFlowRuleLibrary) {
        this.shareCashFlowRuleLibraryDialogData = {
            showDialog:true,
            cashFlowRuleLibrary: clone(cashFlowRuleLibrary)
        }
    }

    onShareCashFlowRuleDialogSubmit(cashFlowRuleLibraryUsers: CashFlowRuleLibraryUser[]) {
        this.shareCashFlowRuleLibraryDialogData = clone(emptyShareCashFlowRuleLibraryDialogData);

                if (!isNil(cashFlowRuleLibraryUsers) && this.selectedCashFlowRuleLibrary.id !== getBlankGuid())
                {
                    let libraryUserData: LibraryUser[] = [];

                    //create library users
                    cashFlowRuleLibraryUsers.forEach((cashFlowRuleLibraryUser, index) =>
                    {   
                        //determine access level
                        let libraryUserAccessLevel: number = 0;
                        if (libraryUserAccessLevel == 0 && cashFlowRuleLibraryUser.isOwner == true) { libraryUserAccessLevel = 2; }
                        if (libraryUserAccessLevel == 0 && cashFlowRuleLibraryUser.canModify == true) { libraryUserAccessLevel = 1; }

                        //create library user object
                        let libraryUser: LibraryUser = {
                            userId: cashFlowRuleLibraryUser.userId,
                            userName: cashFlowRuleLibraryUser.username,
                            accessLevel: libraryUserAccessLevel
                        }

                        //add library user to an array
                        libraryUserData.push(libraryUser);
                    });
                    if (!isNullOrUndefined(this.selectedCashFlowRuleLibrary.id) ) {
                        this.getIsSharedLibraryAction(this.selectedCashFlowRuleLibrary).then(this.isShared = this.isSharedLibrary);
                    }
                    //update budget library sharing
                    CashFlowService.upsertOrDeleteCashFlowRuleLibraryUsers(this.selectedCashFlowRuleLibrary.id, libraryUserData).then((response: AxiosResponse) => {
                        if (hasValue(response, 'status') && http2XX.test(response.status.toString()))
                        {
                            this.resetPage();
                        }
                    });
                }
    }
}
</script>

<style>
.cash-flow-library-tables {
    height: 425px;
    overflow-y: auto;
    overflow-x: hidden;
}

.cash-flow-library-tables .v-menu--inline {
    width: 100%;
}

.cash-flow-library-tables .v-menu__activator a,
.cash-flow-library-tables .v-menu--inline input {
    width: 100%;
}

.cash-flow-radio-group .v-input--radio-group__input {
    padding-top: 25px;
}

.output {
    border-bottom: 1px solid;
}

.cash-flow-library-card {
    height: 330px;
    overflow-y: auto;
    overflow-x: hidden;
}

.invalid-input {
    color: red;
}

.amount-div {
    width: 208px;
}

.split-treatment-limit-currency-input {
    border: 1px solid;
    width: 100%;
}

.split-treatment-limit-amount-rule-span {
    font-size: 0.8em;
}

.sharing label {
    padding-top: 0.5em;
}

.sharing {
    padding-top: 0;
    margin: 0;
}
</style>
