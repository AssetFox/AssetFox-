<template>
    <v-layout column>
        <!-- top row -->
        <v-flex xs12>
            <v-layout justify-space-between>
                <v-flex xs5 class="ghd-constant-header" style="margin-right: 10px">
                    <v-layout column>
                        <v-subheader class="ghd-md-gray ghd-control-label">Calculated Attribute</v-subheader>
                        <v-select
                                  id="CalculatedAttribute-CalculatedAttribute-select"
                                  :items="librarySelectItems"
                                  append-icon=$vuetify.icons.ghd-down
                                  outline
                                  v-model="librarySelectItemValue"
                                  class="ghd-select ghd-text-field ghd-text-field-border">
                        </v-select>
                        <div class="ghd-md-gray ghd-control-subheader" v-if="hasScenario"><b>Library Used: {{parentLibraryName}} <span v-if="scenarioLibraryIsModified">&nbsp;(Modified)</span></b></div>
                    </v-layout>
                </v-flex>
                <v-flex xs7 class="ghd-constant-header" style="margin-right: 10px">
                    <v-layout align-end>
                        <v-text-field
                                    id="CalculatedAttribute-search-textField"
                                    prepend-inner-icon=$vuetify.icons.ghd-search
                                    hide-details
                                    lablel="Search"
                                    placeholder="Search Calcultated Attribute"
                                    single-line
                                    v-model="gridSearchTerm"
                                    outline
                                    clearable
                                    @click:clear="onClearClick()"
                                    class="ghd-text-field-border ghd-text-field search-icon-general"
                                    style="margin-top:20px !important">
                        </v-text-field>
                        <v-btn id="CalculatedAttribute-search-btn" style="position: relative; top: 3px; margin-right: 1px" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline @click="onSearchClick()">Search</v-btn>
                        <v-btn id="CalculatedAttribute-createNewLibrary-btn"
                            @click="onShowCreateCalculatedAttributeLibraryDialog(false)"
                            class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                            outline
                            v-show="!hasScenario"
                            style="top: 2px !important; position: relative">
                            Create New Library
                        </v-btn>
                    </v-layout>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex xs6 class="ghd-constant-header" style="margin-bottom: 15px">
            <v-layout v-if='hasSelectedLibrary && !hasScenario' align-center>
                <div class="header-text-content owner-padding">
                     Owner: {{ getOwnerUserName() || '[ No Owner ]' }} | Date Modified: {{ modifiedDate }}
                </div>
                <v-divider class="owner-shared-divider" inset vertical></v-divider>
                <v-badge v-show="isShared" style="padding: 10px">
                    <template v-slot:badge>
                        <span>Shared</span>
                    </template>
                    </v-badge>
                    <v-btn id="CalculatedAttribute-shareLibrary-btn" @click='onShowShareCalculatedAttributeLibraryDialog(selectedCalculatedAttributeLibrary)' class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline
                v-show='!hasScenario'>
                Share Library
            </v-btn>
        </v-layout>
        </v-flex>
        <!-- attributes and timing -->
        <v-flex xs12 v-show="hasSelectedLibrary || hasScenario">
            <v-layout justify-space-between>
                <v-flex xs6>
                <v-layout column style="float:left; width: 100%">
                    <v-subheader class="ghd-md-gray ghd-control-label">Attribute</v-subheader>
                    <v-select
                        id="CalculatedAttribute-Attribute-select"   
                        :items="attributeSelectItems"
                        append-icon=$vuetify.icons.ghd-down
                        outline
                        class="ghd-select ghd-text-field ghd-text-field-border"
                        v-model="attributeSelectItemValue">
                    </v-select>
                </v-layout>
                <v-flex xs2>
                </v-flex>
                </v-flex>
                <v-flex xs6>
                <v-layout column style="float:right; width: 100%">
                    <v-subheader class="ghd-md-gray ghd-control-label">Timing</v-subheader>
                    <v-select
                        id="CalculatedAttribute-Timing-select"
                        :items="attributeTimingSelectItems"
                        append-icon=$vuetify.icons.ghd-down
                        outline
                        v-model="attributeTimingSelectItemValue"
                        :disabled="!hasAdminAccess"
                        class="ghd-select ghd-text-field ghd-text-field-border"
                        v-on:change="setTiming">
                    </v-select>
                </v-layout>
                </v-flex>
            </v-layout>
        </v-flex>
        <!-- Default Equation -->
        <v-flex xs12 v-show="hasSelectedLibrary || hasScenario">
            <v-layout justify-center>
                <v-flex xs6>
                    <v-layout column>
                        <v-subheader class="ghd-md-gray ghd-control-label">Default Equation</v-subheader>
                        <v-text-field
                            id="CalculatedAttribute-defaultEquation-textfield"
                            readonly
                            class="sm-txt"
                            v-model="defaultEquation.equation.expression"
                            :disabled="!hasAdminAccess">
                            <template slot="append-outer">
                                <v-btn
                                    id="CalculatedAttribute-defaultEquationEditor-btn"
                                    @click="onShowEquationEditorDialogForDefaultEquation()"
                                    class="ghd-blue"
                                    icon
                                    v-if="hasAdminAccess">
                                    <img class='img-general img-shift' :src="require('@/assets/icons/edit.svg')"/>
                                </v-btn>
                            </template>
                        </v-text-field>
                    </v-layout>
                </v-flex>
            </v-layout>
        </v-flex>
        <!-- data table -->
        <v-flex xs12 v-show="hasSelectedLibrary || hasScenario">
            <v-data-table
                id="CalculatedAttribute-equation-table"    
                :headers="calculatedAttributeGridHeaders"
                :items="selectedGridItem"
                :pagination.sync="pagination"
                :total-items="totalItems"
                :rows-per-page-items=[5,10,25]
                class="v-table__overflow ghd-table"
                sort-icon=$vuetify.icons.ghd-table-sort
                item-key="calculatedAttributeLibraryEquationId">
                <template slot="items" slot-scope="props">
                    <td class="text-xs-center">
                        <v-text-field
                            id="CalculatedAttribute-equation-textfield"
                            readonly
                            class="sm-txt"
                            :value="props.item.equation"
                            :disabled="!hasAdminAccess">
                            <template slot="append-outer">
                                <v-btn
                                    id="CalculatedAttribute-editEquation-btn"
                                    @click="onShowEquationEditorDialog(props.item.id)"
                                    class="ghd-blue"
                                    icon
                                    v-if="hasAdminAccess">
                                    <img class='img-general img-shift' :src="require('@/assets/icons/edit.svg')"/>
                                </v-btn>
                            </template>
                        </v-text-field>
                    </td>
                    <td class="text-xs-center">
                        <v-text-field
                            id="CalculatedAttribute-EquationCriteria-textfield"
                            readonly
                            class="sm-txt"
                            :value="props.item.criteriaExpression"
                            :disabled="!hasAdminAccess">
                            <template slot="append-outer">
                                <v-btn
                                    id="CalculatedAttribute-changeEquationCriteria-btn"
                                    @click="onEditCalculatedAttributeCriterionLibrary(props.item.id)"
                                    class="ghd-blue"
                                    icon
                                    v-if="hasAdminAccess">
                                    <img class='img-general img-shift' :src="require('@/assets/icons/edit.svg')"/>
                                </v-btn>
                            </template>
                        </v-text-field>
                    </td>
                    <td class="text-xs-center">
                        <v-btn
                            id="CalculatedAttribute-RemoveEquation-btn"
                            @click="
                                onRemoveCalculatedAttribute(props.item.id)"
                            class="ghd-blue"
                            icon
                            :disabled="!hasAdminAccess">
                            <img class='img-general' :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
                        </v-btn>
                    </td>
                </template>
            </v-data-table>
            <v-btn
                id="CalculatedAttribute-AddNewEquation-btn"
                @click="onAddCriterionEquationSet()"
                class='ghd-blue ghd-button'
                outline
                v-if="hasAdminAccess"
                :disabled="
                    attributeSelectItemValue == null ||
                    attributeSelectItemValue == ''">
                Add New Equation
            </v-btn>
        </v-flex>
        <!-- description -->
        <v-flex v-show='hasSelectedLibrary && !hasScenario' xs12>
            <v-subheader class="ghd-subheader ">Description</v-subheader>
            <v-textarea
                no-resize
                outline
                class="ghd-text-field-border"
                rows="4"
                v-model="selectedCalculatedAttributeLibrary.description"
                @input='checkHasUnsavedChanges()'/>
        </v-flex>
        <!-- buttons -->
        <v-flex xs12 v-show="hasSelectedLibrary || hasScenario">
            <v-layout justify-center v-show='hasSelectedLibrary || hasScenario'>
                <v-btn
                    :disabled="!hasUnsavedChanges"
                    v-if="hasAdminAccess && hasScenario"
                    @click="onDiscardChanges"
                    class='ghd-blue ghd-button-text ghd-button'
                    flat
                    v-show="hasSelectedLibrary || hasScenario">
                    Cancel
                </v-btn>
                
                <v-btn id="CalculatedAttribute-deleteLibrary-btn"
                    @click="onShowConfirmDeleteAlert"
                    class='ghd-blue ghd-button-text ghd-button'
                    flat
                    v-show="!hasScenario"
                    :disabled="!hasSelectedLibrary">
                    Delete Library
                </v-btn>
                <v-btn id="CalculatedAttribute-createAsNewLibrary-btn"
                    :disabled="disableCrudButton()"
                    v-if="hasAdminAccess"
                    @click="onShowCreateCalculatedAttributeLibraryDialog(true)"
                    outline
                    class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'>
                    Create as New Library
                </v-btn>
                <v-btn id="CalculatedAttribute-updateLibrary-btn"
                    :disabled="disableCrudButton() || !hasUnsavedChanges"
                    @click="onUpsertCalculatedAttributeLibrary"
                    class='ghd-blue-bg white--text ghd-button-text ghd-outline-button-padding ghd-button'
                    v-show="!hasScenario">
                    Update Library
                </v-btn>
                <v-btn
                    @click="onUpsertScenarioCalculatedAttribute"
                    class='ghd-blue-bg white--text ghd-button-text ghd-button'
                    v-show="hasScenario && hasAdminAccess"
                    :disabled="disableCrudButton() || !hasUnsavedChanges">
                    Save
                </v-btn> 
            </v-layout>
        </v-flex>
     
        <ConfirmDeleteAlert
            :dialogData="confirmDeleteAlertData"
            @submit="onSubmitConfirmDeleteAlertResult"
        />
        <CreateCalculatedAttributeLibraryDialog
            :dialogData="createCalculatedAttributeLibraryDialogData"
            @submit="onSubmitCreateCalculatedAttributeLibraryDialogResult"
        />
        <ShareCalculatedAttributeLibraryDialog :dialogData="shareCalculatedAttributeLibraryDialogData"
            @submit="onShareCalculatedAttributeDialogSubmit" 
        />
        <CreateCalculatedAttributeDialog
            :showDialog="showCreateCalculatedAttributeDialog"
            @submit="onSubmitCreateCalculatedAttributeDialogResult"
        />
        <EquationEditorDialog
            :dialogData="equationEditorDialogData"
            @submit="onSubmitEquationEditorDialogResult"
        />
        <GeneralCriterionEditorDialog
            :dialogData="criterionEditorDialogData"
            @submit="onSubmitCriterionEditorDialogResult"
        />
    </v-layout>
</template>
<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import { Watch } from 'vue-property-decorator';
import { Action, State, Getter, Mutation } from 'vuex-class';
import Alert from '@/shared/modals/Alert.vue';
import EquationEditorDialog from '../../shared/modals/EquationEditorDialog.vue';
import CreateCalculatedAttributeLibraryDialog from './calculated-attribute-editor-dialogs/CreateCalculatedAttributeLibraryDialog.vue';
import CreateCalculatedAttributeDialog from './calculated-attribute-editor-dialogs/CreateCalculatedAttributeDialog.vue';
import ShareCalculatedAttributeLibraryDialog from '@/components/calculated-attribute-editor/calculated-attribute-editor-dialogs/ShareCalculatedAttributeLibraryDialog.vue';
import { emptyShareCalculatedAttributeLibraryDialogData, ShareCalculatedAttributeLibraryDialogData } from '@/shared/models/modals/share-calculated-attribute-data';
import {
    InputValidationRules,
    rules,
} from '@/shared/utils/input-validation-rules';
import {
  any,
    clone,
    find,
    findIndex,
    isNil,
    propEq,
    update,
} from 'ramda';
import {
    CalculatedAttribute,
    CalculatedAttributeGridModel,
    CalculatedAttributeLibrary,
    CalculatedAttributeLibraryUser,
    CriterionAndEquationSet,
    emptyCalculatedAttribute,
    emptyCalculatedAttributeLibrary,
    emptyCriterionAndEquationSet,
    Timing,
} from '@/shared/models/iAM/calculated-attribute';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { Attribute } from '@/shared/models/iAM/attribute';
import { hasValue } from '@/shared/utils/has-value-util';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
import {
    CreateCalculatedAttributeLibraryDialogData,
    emptyCreateCalculatedAttributeLibraryDialogData,
} from '@/shared/models/modals/create-calculated-attribute-library-dialog-data';
import {
    emptyEquationEditorDialogData,
    EquationEditorDialogData,
} from '@/shared/models/modals/equation-editor-dialog-data';
import { Equation } from '@/shared/models/iAM/equation';
import {
    emptyCriterionLibrary,
} from '@/shared/models/iAM/criteria';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { SelectItem } from '@/shared/models/vue/select-item';
import { ScenarioRoutePaths } from '@/shared/utils/route-paths';
import { getUserName } from '@/shared/utils/get-user-info';
import { emptyPagination, Pagination } from '@/shared/models/vue/pagination';
import { CalculatedAttributeLibraryUpsertPagingRequestModel, CalculatedAttributePagingRequestModel, CalculatedAttributePagingSyncModel, calculcatedAttributePagingPageModel} from '@/shared/models/iAM/paging';
import { mapToIndexSignature } from '@/shared/utils/conversion-utils';
import CalculatedAttributeService from '@/services/calculated-attribute.service';
import { AxiosResponse } from 'axios';
import { http2XX } from '@/shared/utils/http-utils';
import GeneralCriterionEditorDialog from '@/shared/modals/GeneralCriterionEditorDialog.vue';
import { emptyGeneralCriterionEditorDialogData, GeneralCriterionEditorDialogData } from '@/shared/models/modals/general-criterion-editor-dialog-data';
import { isNullOrUndefined } from 'util';
import { LibraryUser } from '@/shared/models/iAM/user';

@Component({
    components: {
        CreateCalculatedAttributeLibraryDialog,
        CreateCalculatedAttributeDialog,
        ShareCalculatedAttributeLibraryDialog,
        EquationEditorDialog,
        GeneralCriterionEditorDialog,
        ConfirmDeleteAlert: Alert,
    },
})
export default class CalculatedAttributeEditor extends Vue {
    @State(state => state.calculatedAttributeModule.calculatedAttributeLibraries) stateCalculatedAttributeLibraries: CalculatedAttributeLibrary[];
    @State(state =>state.calculatedAttributeModule.selectedCalculatedAttributeLibrary) stateSelectedCalculatedAttributeLibrary: CalculatedAttributeLibrary;
    @State(state => state.calculatedAttributeModule.scenarioCalculatedAttributes) stateScenarioCalculatedAttributes: CalculatedAttribute[];
    @State(state => state.calculatedAttributeModule.selectedLibraryCalculatedAttributes) stateSelectedLibraryCalculatedAttributes: CalculatedAttribute[];
    @State(state => state.calculatedAttributeModule.calculatedAttributes) stateCalculatedAttributes: Attribute[];
    @State(state => state.unsavedChangesFlagModule.hasUnsavedChanges) hasUnsavedChanges: boolean;
    @State(state => state.authenticationModule.hasAdminAccess) hasAdminAccess: boolean;
    @State(state => state.calculatedAttributeModule.isSharedLibrary) isSharedLibrary: boolean;
    @Action('getIsSharedCalculatedAttributeLibrary') getIsSharedLibraryAction: any;
    @Action('upsertScenarioCalculatedAttribute')
    upsertScenarioCalculatedAttributeAction: any;
    @Action('deleteCalculatedAttributeLibrary')
    deleteCalculatedAttributeLibraryAction: any;
    @Action('getCalculatedAttributeLibraries')
    getCalculatedAttributeLibrariesAction: any;
    @Action('getScenarioCalculatedAttribute') getScenarioCalculatedAttributeAction: any;
    @Action('getSelectedLibraryCalculatedAttributes') getSelectedLibraryCalculatedAttributesAction: any;
    @Action('selectCalculatedAttributeLibrary')
    selectCalculatedAttributeLibraryAction: any;
    @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;
    @Action('getCalculatedAttributes') getCalculatedAttributesAction: any;
    @Action('addErrorNotification') addErrorNotificationAction: any;
    @Action('addSuccessNotification') addSuccessNotificationAction: any;
    @Action('getCurrentUserOrSharedScenario') getCurrentUserOrSharedScenarioAction: any;
    @Action('selectScenario') selectScenarioAction: any;

    @Mutation('selectedCalculatedAttributeLibraryMutator') selectedCalculatedAttributeLibraryMutation: any;
    @Mutation('calculatedAttributeLibraryMutator') calculatedAttributeLibraryMutateActions: any;

    @Getter('getUserNameById') getUserNameByIdGetter: any;

    updatedCalcAttrMap:Map<string, [CalculatedAttribute, CalculatedAttribute]> = 
        new Map<string, [CalculatedAttribute, CalculatedAttribute]>();//0: original value | 1: updated value
    addedCalcAttr: CalculatedAttribute[] = [];
    CalcAttrCache: CalculatedAttribute = emptyCalculatedAttribute;
    pairsCache: CriterionAndEquationSet[] = [];
    updatedPairsMaps:Map<string, [CriterionAndEquationSet, CriterionAndEquationSet]> = 
        new Map<string, [CriterionAndEquationSet, CriterionAndEquationSet]>();//0: original value | 1: updated value 
    addedPairs: Map<string, CriterionAndEquationSet[]> = new  Map<string, CriterionAndEquationSet[]>();
    deletionPairsIds: Map<string, string[]> = new Map<string, string[]>();
    updatedPairs:  Map<string, CriterionAndEquationSet[]> = new  Map<string, CriterionAndEquationSet[]>();
    defaultEquations: Map<string, CriterionAndEquationSet> = new Map<string, CriterionAndEquationSet>()
    gridSearchTerm = '';
    currentSearch = '';
    pagination: Pagination = clone(emptyPagination);
    isPageInit = false;
    totalItems = 0;
    currentPage: CalculatedAttribute = clone(emptyCalculatedAttribute);
    initializing: boolean = true;
    uuidNIL: string = getBlankGuid();
    isShared: boolean = false;
    modifiedDate: string;

    shareCalculatedAttributeLibraryDialogData: ShareCalculatedAttributeLibraryDialogData = clone(emptyShareCalculatedAttributeLibraryDialogData);


    defaultEquation: CriterionAndEquationSet = emptyCriterionAndEquationSet;
    defaultEquationCache: CriterionAndEquationSet = emptyCriterionAndEquationSet;
    defaultSelected: boolean = false;

    hasSelectedLibrary: boolean = false;
    isDefaultBool: boolean = true;//this exists because isDefault can't be tracked so this bool is tracked for the switch and is then synced with isDefault
    hasScenario: boolean = false;
    rules: InputValidationRules = clone(rules);
    confirmDeleteAlertData: AlertData = clone(emptyAlertData);
    showCreateCalculatedAttributeDialog = false;
    hasSelectedCalculatedAttribute: boolean = false;
    selectedCalculatedAttribute: CalculatedAttribute = clone(
        emptyCalculatedAttribute,
    );
    selectedScenarioId: string = getBlankGuid();
    librarySelectItems: SelectItem[] = [];

    librarySelectItemValue: string | null = '';
    trueLibrarySelectItemValue: string | null = ''
    librarySelectItemValueAllowedChanged: boolean = true;

    unsavedDialogAllowed: boolean = true;

    attributeSelectItems: SelectItem[] = [];

    attributeSelectItemValue: string | null = '';
    trueAttributeSelectItemValue: string | null = '';
    attributeSelectItemValueAllowedChanged: boolean = true;

    isAttributeSelectedItemValue: boolean = false;
    isTimingSelectedItemValue: boolean = false;
    attributeTimingSelectItems: SelectItem[] = [];
    attributeTimingSelectItemValue: string | number | null = '';
    currentCriteriaEquationSetSelectedId: string | null = '';

    selectedCalculatedAttributeLibrary: CalculatedAttributeLibrary = clone(
        emptyCalculatedAttributeLibrary,
    );
    createCalculatedAttributeLibraryDialogData: CreateCalculatedAttributeLibraryDialogData = clone(
        emptyCreateCalculatedAttributeLibraryDialogData,
    );
    equationEditorDialogData: EquationEditorDialogData = clone(
        emptyEquationEditorDialogData,
    );
    criterionEditorDialogData: GeneralCriterionEditorDialogData = clone(
        emptyGeneralCriterionEditorDialogData,
    );
    calculatedAttributeGridData: CalculatedAttribute[] = [];
    activeCalculatedAttributeId: string = getBlankGuid();
    selectedGridItem: CalculatedAttributeGridModel[] = [];
    selectedAttribute: CalculatedAttribute = clone(emptyCalculatedAttribute)
    hasCreatedLibrary: boolean = false;

    parentLibraryName: string = "None";
    parentLibraryId: string = "";
    scenarioLibraryIsModified: boolean = false;
    loadedParentName: string = "";
    loadedParentId: string = "";
    newLibrarySelection: boolean = false;

    calculatedAttributeGridHeaders: DataTableHeader[] = [
        {
            text: 'Equation',
            value: 'equation',
            align: 'left',
            sortable: true,
            class: '',
            width: '',
        },
        {
            text: 'Criterion',
            value: 'criteriaExpression',
            align: 'left',
            sortable: true,
            class: '',
            width: '',
        },
        {
            text: '',
            value: '',
            align: 'left',
            sortable: false,
            class: '',
            width: '',
        },
    ];

    beforeRouteEnter(to: any, from: any, next: any) {
        next((vm: any) => {
            vm.librarySelectItemValue = null;
            vm.attributeSelectItemValue = null;

            vm.getCalculatedAttributesAction().then( () => {
                vm.getCalculatedAttributeLibrariesAction().then(() => {
                    vm.setAttributeSelectItems()
                    vm.setAttributeTimingSelectItems();
                    if (to.path.indexOf(ScenarioRoutePaths.CalculatedAttribute) !== -1) {
                        vm.selectedScenarioId = to.query.scenarioId;

                        if (vm.selectedScenarioId === vm.uuidNIL) {
                            vm.addErrorNotificationAction({
                                message: 'Unable to identify selected scenario.',
                            });
                            vm.$router.push('/Scenarios/');
                        }

                        vm.hasScenario = true;
                        vm.getScenarioCalculatedAttributeAction(vm.selectedScenarioId).then(()=> {
                            vm.getCurrentUserOrSharedScenarioAction({simulationId: vm.selectedScenarioId}).then(() => {         
                                vm.selectScenarioAction({ scenarioId: vm.selectedScenarioId });        
                            });
                        });
                    }
                });                
            });
        });
    }
    beforeDestroy() {
        this.calculatedAttributeGridData = [] as CalculatedAttribute[];
        this.selectedAttribute = clone(emptyCalculatedAttribute);
    }

    // Watchers
    @Watch('pagination')
    async onPaginationChanged() {
        if(this.initializing)
            return;
        this.checkHasUnsavedChanges();
        const { sortBy, descending, page, rowsPerPage } = this.pagination;
        const request: CalculatedAttributePagingRequestModel= {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId: this.selectedCalculatedAttributeLibrary.id === this.uuidNIL ? null : this.selectedCalculatedAttributeLibrary.id,
                updatedCalculatedAttributes: Array.from(this.updatedCalcAttrMap.values()).map(r => r[1]),
                deletedPairs: mapToIndexSignature(this.deletionPairsIds),
                updatedPairs: mapToIndexSignature( this.updatedPairs),
                addedPairs: mapToIndexSignature(this.addedPairs),
                addedCalculatedAttributes: this.addedCalcAttr,
                defaultEquations: mapToIndexSignature(this.defaultEquations),
                isModified: this.scenarioLibraryIsModified
            },           
            sortColumn: sortBy === '' ? 'year' : sortBy,
            isDescending: descending != null ? descending : false,
            search: this.currentSearch,
            attributeId: this.stateCalculatedAttributes.find(_ => _.name === this.selectedAttribute.attribute)!.id
        };
        if((!this.hasSelectedLibrary && this.hasScenario) && this.selectedScenarioId !== this.uuidNIL){
           await CalculatedAttributeService.getScenarioCalculatedAttrbiutetPage(this.selectedScenarioId, request).then(response => {
                if(response.data){
                    let data = response.data as calculcatedAttributePagingPageModel;
                    this.currentPage.equations = data.items;
                    this.currentPage.calculationTiming = data.calculationTiming
                    this.pairsCache = this.currentPage.equations;
                    this.totalItems = data.totalItems;
                    this.defaultEquation = data.defaultEquation;
                    this.selectedGridItem = this.calculatedAttributeGridModelConverter(this.currentPage)
                }
            });
        }            
        else if(this.hasSelectedLibrary)
        this.initializing = true;
        await CalculatedAttributeService.getCalculatedLibraryModifiedDate(this.selectedCalculatedAttributeLibrary.id).then(response => {
                  if (hasValue(response, 'status') && http2XX.test(response.status.toString()) && response.data)
                   {
                      var data = response.data as string;
                      this.modifiedDate = data.slice(0, 10);
                   }
             });

             await CalculatedAttributeService.getLibraryCalculatedAttributePage(this.librarySelectItemValue !== null ? this.librarySelectItemValue : '', request).then(response => {
                if(response.data){
                    let data = response.data as calculcatedAttributePagingPageModel;
                    this.currentPage.equations = data.items;
                    this.currentPage.calculationTiming = data.calculationTiming
                    this.pairsCache = this.currentPage.equations;
                    this.totalItems = data.totalItems;
                    this.defaultEquation = data.defaultEquation;
                    this.selectedGridItem = this.calculatedAttributeGridModelConverter(this.currentPage);
                    if (!isNullOrUndefined(this.selectedCalculatedAttributeLibrary.id) ) {
                        this.getIsSharedLibraryAction(this.selectedCalculatedAttributeLibrary).then(this.isShared = this.isSharedLibrary);
                    }
                }
            });     
    }

    @Watch('deletionPairsIds')
    onDeletionPairsIdsChanged(){
        this.checkHasUnsavedChanges();
    }

    @Watch('addedPairs', {deep: true})
    onAddedPairsChanged(){
        this.checkHasUnsavedChanges();
    }

    onSelectedAttributeChanged(){
        this.selectedGridItem = this.calculatedAttributeGridModelConverter(this.currentPage)
    }

    @Watch('isDefaultBool')
    onIsDefaultBoolChanged(){
        this.selectedCalculatedAttributeLibrary.isDefault = this.isDefaultBool;
        this.checkHasUnsavedChanges()
    }
    @Watch('stateCalculatedAttributes')
    onStateCalculatedAttributesChanged() {
        this.setAttributeSelectItems();
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
    setAttributeSelectItems() {
        if (hasValue(this.stateCalculatedAttributes)) {
            this.attributeSelectItems = this.stateCalculatedAttributes.map(
                (attribute: Attribute) => ({
                    text: attribute.name,
                    value: attribute.name,
                }),
            );

            this.attributeSelectItems.forEach(_ => {
                var tempItem: CalculatedAttribute = {
                    id: getNewGuid(),
                    attribute: _.text,
                    name: _.text,
                    libraryId: getNewGuid(),
                    isModified: false,
                    calculationTiming: Timing.OnDemand,
                    equations: [] as CriterionAndEquationSet[],
                };
                if (this.calculatedAttributeGridData == undefined) {
                    this.calculatedAttributeGridData = [] as CalculatedAttribute[];
                }
                this.calculatedAttributeGridData.push(tempItem);
            });
        }
    }
    setAttributeTimingSelectItems() {
        this.attributeTimingSelectItems = [
            { text: 'Pre Deterioration', value: Timing.PreDeterioration },
            { text: 'Post Deterioration', value: Timing.PostDeterioration },
            { text: 'On Demand', value: Timing.OnDemand },
        ];
    }
    @Watch('stateCalculatedAttributeLibraries')
    onStateCalculatedAttributeLibrariesChanged() {
        this.librarySelectItems = this.stateCalculatedAttributeLibraries.map(
            (library: CalculatedAttributeLibrary) => ({
                text: library.name,
                value: library.id,
            }),
        );
    }
    //this is so that a user is asked wether or not to continue when switching libraries after they have made changes
    //but only when in libraries
    @Watch('librarySelectItemValue')
    onLibrarySelectItemValueChangedCheckUnsaved(){
        this.defaultEquation.equation.expression = "";
        if(this.hasScenario){
            this.onLibrarySelectItemValueChanged();
            this.unsavedDialogAllowed = false;
        }           
        else if(this.librarySelectItemValueAllowedChanged) {
            this.CheckUnsavedDialog(this.onLibrarySelectItemValueChanged, () => {
                this.librarySelectItemValueAllowedChanged = false;
                this.librarySelectItemValue = this.trueLibrarySelectItemValue;               
            });
        }
        this.parentLibraryId = this.librarySelectItemValue ? this.librarySelectItemValue : "";
        this.setParentLibraryName(this.parentLibraryId);
        this.newLibrarySelection = true;
        this.librarySelectItemValueAllowedChanged = true;
    }
    onLibrarySelectItemValueChanged() {
        this.trueLibrarySelectItemValue = this.librarySelectItemValue;
        this.selectCalculatedAttributeLibraryAction(
            this.librarySelectItemValue,
        );
    }

    //this is so that users are asked wether or not to continue when switching attributes after making changes
    @Watch('attributeSelectItemValue')
    onAttributeSelectItemValueChangedCheckUnsaved(){
        if(this.attributeSelectItemValueAllowedChanged)
            this.CheckUnsavedDialog(this.onAttributeSelectItemValueChanged, () => {
                this.attributeSelectItemValueAllowedChanged = false;
                this.attributeSelectItemValue = this.trueAttributeSelectItemValue;               
            })
        this.attributeSelectItemValueAllowedChanged = true;
    }
    onAttributeSelectItemValueChanged() {
        // selection change in calculated attribute multi select
        this.clearChanges();
        this.trueAttributeSelectItemValue = this.attributeSelectItemValue;
        this.checkHasUnsavedChanges();
        if (
            isNil(this.attributeSelectItemValue) ||
            this.attributeSelectItemValue == ''
        ) {
            this.isAttributeSelectedItemValue = false;
            this.isTimingSelectedItemValue = false;
            this.selectedAttribute = clone(emptyCalculatedAttribute);
        } else {
            this.isAttributeSelectedItemValue = true;
            this.isTimingSelectedItemValue = true;
            var item = this.calculatedAttributeGridData.find(
                _ => _.attribute == this.attributeSelectItemValue,
            );
            if (item != undefined) {
                this.activeCalculatedAttributeId = item.id;
                this.selectedAttribute = item;               
                this.initializePages();
            } else {
                // if the selected Calculated attribute data is not present in the grid
                // Add a new object for it. Because we cannot loop over a object, which is null
                var newAttributeObject: CalculatedAttribute = {
                    id: getNewGuid(),
                    libraryId: getNewGuid(),
                    isModified: false,
                    attribute: this.attributeSelectItemValue,
                    name: this.attributeSelectItemValue,
                    calculationTiming: Timing.OnDemand,
                    equations: [] as CriterionAndEquationSet[],
                };
                this.calculatedAttributeGridData.push(newAttributeObject);
                this.activeCalculatedAttributeId = newAttributeObject.id;
                this.selectedAttribute = newAttributeObject;
                this.setTimingsMultiSelect(Timing.OnDemand);
                this.addedCalcAttr.push(clone(newAttributeObject));
                this.initializePages();
            }
        }
    }
    @Watch('attributeTimingSelectItemValue')
    onAttributeTimingSelectItemValue() {
        // Change in timings select box
        if (
            isNil(this.attributeTimingSelectItemValue) ||
            this.attributeTimingSelectItemValue == ''
        ) {
            this.isTimingSelectedItemValue = false;
        } else {
            this.isTimingSelectedItemValue = true;
            var localTiming = this.attributeTimingSelectItemValue as Timing;
            var item = this.calculatedAttributeGridData.find(
                _ => _.attribute == this.attributeSelectItemValue,
            );
            if (item != undefined) {
                this.CalcAttrCache = clone(item)
                item.calculationTiming = localTiming;
                this.selectedAttribute = item;
                this.onUpdateCalcAttr(item.id, clone(item))
                this.onPaginationChanged();
            }
        }
    }

    @Watch('stateSelectedCalculatedAttributeLibrary')
    onStateSelectedCalculatedAttributeLibraryChanged() {
        this.selectedCalculatedAttributeLibrary = clone(
            this.stateSelectedCalculatedAttributeLibrary,
        );
        this.isDefaultBool = this.selectedCalculatedAttributeLibrary.isDefault;
    }
    @Watch('stateScenarioCalculatedAttributes')
    onStateScenarioCalculatedAttributeChanged() {
        if (this.hasScenario) {
            if (
                !isNil(this.stateScenarioCalculatedAttributes) &&
                this.stateScenarioCalculatedAttributes.length > 0
            ) {
                this.activeCalculatedAttributeId = this.stateScenarioCalculatedAttributes[0].id;
            }
            this.resetGridData();
        }
    }
    @Watch('calculatedAttributeGridData', {deep: true})
    onCalculatedAttributeGridDataChanged() {
        if (this.hasAdminAccess) {
            this.checkHasUnsavedChanges();
        }
    }
    @Watch('selectedCalculatedAttributeLibrary')
    onSelectedCalculatedAttributeLibraryChanged() {
        
        // change in library multiselect
        if (
            this.selectedCalculatedAttributeLibrary.id !== this.uuidNIL 
        ) {
            this.hasSelectedLibrary = true;
        } 
        else {
            this.hasSelectedLibrary = false;
        }

        this.clearChanges();
        if (this.hasScenario && this.hasSelectedLibrary) {
            this.getSelectedLibraryCalculatedAttributesAction(this.selectedCalculatedAttributeLibrary.id).then(() =>{
                // we need new ids for the object which is assigned to a scenario.
                this.calculatedAttributeGridData = clone(
                    this.stateSelectedLibraryCalculatedAttributes,
                );
                // Set the default values in Calculated attribute multi select, if we have data in calculatedAttributeGridData           
                if (
                    this.calculatedAttributeGridData != undefined &&
                    this.calculatedAttributeGridData.length > 0
                ) {
                    this.setDefaultAttributeOnLoad(
                        this.calculatedAttributeGridData[0],
                    );
                } 
                else {
                    this.isAttributeSelectedItemValue = false;
                    this.selectedGridItem = [];
                }
                this.onCalculatedAttributeGridDataChanged();
            })

            
        } else if (this.hasScenario && !this.hasSelectedLibrary) {
            // If a user un select a Library, then reset the grid data from the scenario calculated attribute state
            this.resetGridData();
            this.onCalculatedAttributeGridDataChanged();
        } 
        else if (!this.hasScenario && this.hasSelectedLibrary) {
            // If a user is in Lirabry page
            this.getSelectedLibraryCalculatedAttributesAction(this.selectedCalculatedAttributeLibrary.id).then(() =>{
                this.calculatedAttributeGridData = clone(
                    this.stateSelectedLibraryCalculatedAttributes,
                );
                if (
                    this.calculatedAttributeGridData != undefined &&
                    this.calculatedAttributeGridData.length > 0
                ) {
                    this.attributeSelectItemValue = clone(
                        this.calculatedAttributeGridData[0].attribute,
                    );
                    this.isAttributeSelectedItemValue = true;

                    this.setTimingsMultiSelect(
                        this.calculatedAttributeGridData[0].calculationTiming,
                    );
                    this.activeCalculatedAttributeId = this.calculatedAttributeGridData[0].id;
                    this.selectedAttribute =
                        this.calculatedAttributeGridData[0] != undefined
                            ? this.calculatedAttributeGridData[0]
                            : this.selectedCalculatedAttribute;
                } else {
                    this.isAttributeSelectedItemValue = false;
                    this.attributeSelectItemValue = null;
                    this.attributeTimingSelectItemValue = null;
                    this.isTimingSelectedItemValue = false;
                    this.selectedGridItem = [];
                }
                this.onCalculatedAttributeGridDataChanged();
            })          
        }         
    }
    @Watch('isSharedLibrary')
    onStateSharedAccessChanged() {
        this.isShared = this.isSharedLibrary;
        if (!isNullOrUndefined(this.selectCalculatedAttributeLibrary)) {
            this.selectCalculatedAttributeLibrary.isShared = this.isShared;
        } 
    }
    setTiming(selectedItem: number) {
        this.setTimingsMultiSelect(selectedItem);
    }

    getOwnerUserName(): string {

        if (!this.hasCreatedLibrary) {
        return this.getUserNameByIdGetter(this.selectedCalculatedAttributeLibrary.owner);
        }
        
        return getUserName();
    }

    onUpsertScenarioCalculatedAttribute() {

        if (this.selectedCalculatedAttributeLibrary.id === this.uuidNIL || this.hasUnsavedChanges && this.newLibrarySelection ===false) {this.scenarioLibraryIsModified = true;}
        else { this.scenarioLibraryIsModified = false; }

        const syncModel: CalculatedAttributePagingSyncModel = {
                libraryId: this.selectedCalculatedAttributeLibrary.id === this.uuidNIL ? null : this.selectedCalculatedAttributeLibrary.id,
                updatedCalculatedAttributes: Array.from(this.updatedCalcAttrMap.values()).map(r => r[1]),
                deletedPairs: mapToIndexSignature(this.deletionPairsIds),
                updatedPairs: mapToIndexSignature( this.updatedPairs),
                addedPairs: mapToIndexSignature(this.addedPairs) ,
                addedCalculatedAttributes: this.addedCalcAttr,
                defaultEquations: mapToIndexSignature(this.defaultEquations),
                isModified: this.scenarioLibraryIsModified
        }
        CalculatedAttributeService.upsertScenarioCalculatedAttribute(syncModel, this.selectedScenarioId).then(((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                this.parentLibraryId = this.librarySelectItemValue ? this.librarySelectItemValue : "";
                this.getScenarioCalculatedAttributeAction(this.selectedScenarioId);
                this.clearChanges()
                this.resetPage();
                this.addSuccessNotificationAction({message: "Modified calculated attrbutes"});
                this.librarySelectItemValue = null
            }   
        }))
    }

    onUpsertCalculatedAttributeLibrary() {
        const syncModel: CalculatedAttributePagingSyncModel = {
                libraryId: this.selectedCalculatedAttributeLibrary.id === this.uuidNIL ? null : this.selectedCalculatedAttributeLibrary.id,
                updatedCalculatedAttributes: Array.from(this.updatedCalcAttrMap.values()).map(r => r[1]),
                deletedPairs: mapToIndexSignature(this.deletionPairsIds),
                updatedPairs: mapToIndexSignature( this.updatedPairs),
                addedPairs: mapToIndexSignature(this.addedPairs),
                addedCalculatedAttributes: this.addedCalcAttr,
                defaultEquations: mapToIndexSignature(this.defaultEquations),
                isModified: false
        }
        const request: CalculatedAttributeLibraryUpsertPagingRequestModel = {
            syncModel: syncModel,
            isNewLibrary: false,
            library: this.selectedCalculatedAttributeLibrary,
            scenarioId: null
        }
        CalculatedAttributeService.upsertCalculatedAttributeLibrary(request).then(((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                this.clearChanges()
                this.resetPage();
                this.calculatedAttributeLibraryMutateActions(this.selectedCalculatedAttributeLibrary)
                this.selectedCalculatedAttributeLibraryMutation(this.selectedCalculatedAttributeLibrary.id);
                this.addSuccessNotificationAction({message: "Updated calculated attribute library",});
            }   
        }))
    }
    onShowCreateCalculatedAttributeLibraryDialog(createAsNewLibrary: boolean) {
        var localCalculatedAttributes = [] as CalculatedAttribute[];
        if (createAsNewLibrary) {
            // if library is getting created from a scenario. Assign new Ids
            localCalculatedAttributes = clone(this.calculatedAttributeGridData);
            localCalculatedAttributes.forEach(val => {
                val.id = getNewGuid();
                val.equations.forEach(eq => {
                    eq.id = getNewGuid();
                    if (!isNil(eq.criteriaLibrary)) {
                        eq.criteriaLibrary.id = getNewGuid();
                        eq.criteriaLibrary.isSingleUse = false;
                    }
                    eq.equation.id = getNewGuid();
                });
            });
        }
        this.createCalculatedAttributeLibraryDialogData = {
            showDialog: true,
            calculatedAttributes: createAsNewLibrary
                ? localCalculatedAttributes
                : ([] as CalculatedAttribute[]),
            attributeSelectItems: this.attributeSelectItems,
        };
    }
    onSubmitCreateCalculatedAttributeLibraryDialogResult(
        calculatedAttributeLibrary: CalculatedAttributeLibrary,
    ) {
        this.createCalculatedAttributeLibraryDialogData = clone(
            emptyCreateCalculatedAttributeLibraryDialogData,
        );

        if (!isNil(calculatedAttributeLibrary)) {
            const syncModel: CalculatedAttributePagingSyncModel = {
                libraryId: calculatedAttributeLibrary.calculatedAttributes.length === 0 || !this.hasSelectedLibrary ? null : this.selectedCalculatedAttributeLibrary.id,
                updatedCalculatedAttributes: calculatedAttributeLibrary.calculatedAttributes.length === 0 ? [] : Array.from(this.updatedCalcAttrMap.values()).map(r => r[1]),
                deletedPairs: calculatedAttributeLibrary.calculatedAttributes.length === 0 ? {} : mapToIndexSignature(this.deletionPairsIds),
                updatedPairs: calculatedAttributeLibrary.calculatedAttributes.length === 0 ? {} : mapToIndexSignature( this.updatedPairs),
                addedPairs: calculatedAttributeLibrary.calculatedAttributes.length === 0 ? {} : mapToIndexSignature(this.addedPairs),
                addedCalculatedAttributes: calculatedAttributeLibrary.calculatedAttributes.length === 0 ? [] : this.addedCalcAttr,
                defaultEquations: calculatedAttributeLibrary.calculatedAttributes.length === 0 ? {} : mapToIndexSignature(this.defaultEquations),
                isModified: false
            }
            const request: CalculatedAttributeLibraryUpsertPagingRequestModel = {
                syncModel: syncModel,
                isNewLibrary: true,
                library: calculatedAttributeLibrary,
                scenarioId: this.hasScenario ? this.selectedScenarioId : null
            }
            CalculatedAttributeService.upsertCalculatedAttributeLibrary(request).then(((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    this.clearChanges()
                    this.resetPage();
                    this.calculatedAttributeLibraryMutateActions(calculatedAttributeLibrary);
                    this.selectedCalculatedAttributeLibraryMutation(calculatedAttributeLibrary.id);
                    this.addSuccessNotificationAction({message: "Updated calculated attribute library",});
                    this.librarySelectItemValue = calculatedAttributeLibrary.id;
                    this.hasCreatedLibrary = true;
                }   
            }))          
        }
    }
    onSubmitCreateCalculatedAttributeDialogResult(
        newCalculatedAttribute: CalculatedAttribute[],
    ) {
        this.showCreateCalculatedAttributeDialog = false;

        if (!isNil(newCalculatedAttribute)) {
            this.calculatedAttributeGridData = clone(newCalculatedAttribute);
        }
    }

    disableCrudButton() {
        if (this.calculatedAttributeGridData == undefined) {
            return false;
        }

        if(this.defaultEquation.id === getBlankGuid() || this.defaultEquation.equation.expression.trim() === '')
            return true;   

        var updatePairs = clone(this.updatedPairs.get(this.selectedAttribute.id));
        var addedPairs = clone(this.addedPairs.get(this.selectedAttribute.id));
        var equations: CriterionAndEquationSet[] = [];
        if(!isNil(updatePairs))
            equations = updatePairs;
        if(!isNil(addedPairs))
            equations = equations.concat(addedPairs);

        if(equations.length === 0)
            return false;

        var dataIsValid = false
        if(!isNil(equations)){
            if(equations.every(_ =>(!isNil(_.criteriaLibrary) && _.criteriaLibrary.mergedCriteriaExpression!.trim() !== '') && 
            (!isNil(_.equation) && _.equation.expression.trim() !== '')))
                dataIsValid = true;
        }


        return !dataIsValid;
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
            this.librarySelectItemValue = null;
            this.deleteCalculatedAttributeLibraryAction(
                this.selectedCalculatedAttributeLibrary.id,
            );
        }
    }
    onAddCriterionEquationSet() { 
        var newSet = clone(emptyCriterionAndEquationSet);
        newSet.id = getNewGuid();
        newSet.criteriaLibrary.id = getNewGuid();
        newSet.equation.id = getNewGuid();
        newSet.criteriaLibrary.isSingleUse = true;

        let pairs = this.addedPairs.get(this.selectedAttribute.id);
        if(!isNil(pairs)){
            pairs.push(newSet)
        }
        else
            this.addedPairs.set(this.selectedAttribute.id, [newSet])
        
        if (this.selectedAttribute.equations == undefined) {
            this.selectedAttribute.equations = [];
            this.onSelectedAttributeChanged()
        }
        this.selectedAttribute.equations.push(newSet);
        this.calculatedAttributeGridData = update(
            findIndex(
                propEq('id', this.selectedAttribute.id),
                this.calculatedAttributeGridData,
            ),
            { ...this.selectedAttribute },
            this.calculatedAttributeGridData,
        );
        this.onPaginationChanged();
    }
    onEditCalculatedAttributeCriterionLibrary(criterionEquationSetId: string) {
        var currentCriteria = clone(this.currentPage.equations.find(
            _ => _.id == criterionEquationSetId,
        )!);
        this.currentCriteriaEquationSetSelectedId = criterionEquationSetId;
        if (currentCriteria.criteriaLibrary.id == getBlankGuid()) {
            currentCriteria.criteriaLibrary = {
                id: getNewGuid(),
                name: '',
                mergedCriteriaExpression: '',
                description: '',
                isSingleUse: true,
                isShared: false,
            };
        }
        if (!isNil(currentCriteria)) {
            this.hasSelectedCalculatedAttribute = true;

            this.criterionEditorDialogData = {
                showDialog: true,
                CriteriaExpression: currentCriteria.criteriaLibrary.mergedCriteriaExpression,
            };
        }
    }

    onSubmitCriterionEditorDialogResult(
        criterionExpression: string,
    ) {
        this.criterionEditorDialogData = clone(
            emptyGeneralCriterionEditorDialogData,
        );

        var currItem = this.calculatedAttributeGridData.find(
            _ => _.id == this.activeCalculatedAttributeId,
        )!;
        if (!isNil(criterionExpression) && this.hasSelectedCalculatedAttribute) {

            if(!isNil(currItem)){
                var set = clone(this.currentPage.equations.find(_ => _.id === this.currentCriteriaEquationSetSelectedId));
                if(!isNil(set)){
                    if(set.criteriaLibrary.id === getBlankGuid())
                        set.criteriaLibrary.id = getNewGuid();
                    set.criteriaLibrary.mergedCriteriaExpression = criterionExpression;
                    this.onUpdatePair(set.id, set);
                    this.onPaginationChanged();
                }
            }
            this.onSelectedAttributeChanged();
        }

        this.selectedCalculatedAttribute = clone(emptyCalculatedAttribute);
        this.hasSelectedCalculatedAttribute = false;
    }
    onShowEquationEditorDialog(criterionEquationSetId: string) {
        var currentEquation = clone(this.currentPage.equations.find(
            _ => _.id == criterionEquationSetId,
        ));
        this.currentCriteriaEquationSetSelectedId = criterionEquationSetId;
        if (!isNil(currentEquation)) {
            this.hasSelectedCalculatedAttribute = true;

            this.equationEditorDialogData = {
                showDialog: true,
                equation: currentEquation.equation,
            };
        }
    }

    onShowEquationEditorDialogForDefaultEquation() {
        this.defaultSelected = true;
        this.equationEditorDialogData = {
            showDialog: true,
            equation: this.defaultEquation.equation,
        };
    }
    onSubmitEquationEditorDialogResult(equation: Equation) {
        this.equationEditorDialogData = clone(emptyEquationEditorDialogData);

        if (!isNil(equation) && this.hasSelectedCalculatedAttribute) {
            var currItem = this.calculatedAttributeGridData.find(
                _ => _.id == this.activeCalculatedAttributeId,
            )!;

            if(!isNil(currItem)){
                var pair = clone(this.currentPage.equations.find(_ => _.id == this.currentCriteriaEquationSetSelectedId));
                if(!isNil(pair)){
                    pair.equation.expression = equation.expression;
                    this.onUpdatePair(pair.id, pair);
                    this.onPaginationChanged();
                }
            }
            this.onSelectedAttributeChanged();
        }
        else if (!isNil(equation) && this.defaultSelected) {

            var defaultPair = this.defaultEquation;
            if(!isNil(defaultPair)){
                defaultPair.equation.expression = equation.expression;
                this.updatedDefaultEquation(defaultPair);
                this.checkHasUnsavedChanges();
            }           
        }
        this.selectedCalculatedAttribute = clone(emptyCalculatedAttribute);
        this.hasSelectedCalculatedAttribute = false;
        this.defaultSelected = false;
    }
    onRemoveCalculatedAttribute(criterionEquationSetId: string) {
        var currItem = this.calculatedAttributeGridData.find(
            _ => _.id == this.activeCalculatedAttributeId,
        );
        if(!isNil(currItem)){
            var pair = clone(this.currentPage.equations.find(_ => _.id === criterionEquationSetId))
            if(!isNil(pair)){
                var addPairs = this.addedPairs.get(currItem.id);
                var updatePairs = this.updatedPairs.get(currItem.id);
                if(!isNil(addPairs)){
                    if(!isNil(find(propEq('id', criterionEquationSetId), addPairs))){
                        addPairs = addPairs.filter(_ => _.id !== criterionEquationSetId);
                        this.addedPairs.set(currItem.id, addPairs);
                    }                       
                    else{
                        this.removePair(currItem.id, criterionEquationSetId);
                    }                  
                }  
                else if (!isNil(updatePairs)){
                    if(!isNil(find(propEq('id', criterionEquationSetId), updatePairs))){
                        updatePairs = updatePairs.filter(_ => _.id !== criterionEquationSetId);
                        this.updatedPairs.set(currItem.id, updatePairs);
                    }                       
                    this.removePair(currItem.id, criterionEquationSetId);                        
                }                 
                else
                    this.removePair(currItem.id, criterionEquationSetId);
                this.onPaginationChanged();
            }
        }
        
        this.onSelectedAttributeChanged()
    }

    removePair(calcAttriId:string, pairId: string){
        var deletionPair = this.deletionPairsIds.get(calcAttriId);
        if(!isNil(deletionPair))
            deletionPair.push(pairId);
        else 
            this.deletionPairsIds.set(calcAttriId, [pairId]);
    }
    onDiscardChanges() {
        this.librarySelectItemValue = null;
        this.selectedCalculatedAttributeLibrary = clone(
            emptyCalculatedAttributeLibrary,
        );
        setTimeout(() => {
            if (this.hasScenario) {
                this.resetGridData();
                this.clearChanges();
                this.resetPage();
            }
        });
        this.parentLibraryName = this.loadedParentName;
        this.parentLibraryId = this.loadedParentId;
    }

    resetGridData() {
        this.calculatedAttributeGridData = clone(
            this.stateScenarioCalculatedAttributes,
        );

        if (this.calculatedAttributeGridData != undefined) {
            var currItem = this.calculatedAttributeGridData.find(
                _ => _.id == this.activeCalculatedAttributeId,
            )!;

            if (currItem != undefined) {
                this.attributeSelectItemValue = clone(currItem.attribute);
                this.isAttributeSelectedItemValue = true;

                this.setTimingsMultiSelect(currItem.calculationTiming);
                this.selectedAttribute = currItem;
                // Setting up default values for null object, because API is sending it as null.
                this.selectedAttribute.equations.forEach(_ => {
                    if (isNil(_.criteriaLibrary)) {
                        _.criteriaLibrary = clone(emptyCriterionLibrary);
                        _.criteriaLibrary.id = getNewGuid();
                        _.criteriaLibrary.isSingleUse = true;
                    }
                });
                this.onSelectedAttributeChanged();
            } else if (this.calculatedAttributeGridData.length > 0) {
                this.attributeSelectItemValue = this.calculatedAttributeGridData[0].attribute;
                this.isAttributeSelectedItemValue = true;
                this.setTimingsMultiSelect(
                    this.calculatedAttributeGridData[0].calculationTiming,
                );
            } else {
                this.attributeSelectItemValue = null;
                this.isAttributeSelectedItemValue = false;
            }
        }
    }
    setTimingsMultiSelect(selectedItem: number) {
        if (selectedItem == undefined) {
            selectedItem = Timing.OnDemand;
        }
        var localTiming = this.attributeTimingSelectItems.find(
            _ => _.value == selectedItem,
        )!.text;
        this.attributeTimingSelectItemValue = selectedItem;
        this.isTimingSelectedItemValue = true;
    }
    setDefaultAttributeOnLoad(localCalculatedAttribute: CalculatedAttribute) {
        this.attributeSelectItemValue = clone(
            localCalculatedAttribute.attribute,
        );
        this.isAttributeSelectedItemValue = true;

        this.setTimingsMultiSelect(localCalculatedAttribute.calculationTiming);
        this.activeCalculatedAttributeId = localCalculatedAttribute.id;
        this.selectedAttribute =
            localCalculatedAttribute != undefined
                ? localCalculatedAttribute
                : this.selectedAttribute;
    }

    calculatedAttributeGridModelConverter(item: CalculatedAttribute): CalculatedAttributeGridModel[]{
        let toReturn: CalculatedAttributeGridModel[] = []
        if(!isNil(item.equations))
        {
            item.equations.forEach(_ =>{
                toReturn.push({
                    id: _.id,
                    equation: isNil(_.equation) ? "" : _.equation.expression,
                    criteriaExpression: isNil(_.criteriaLibrary) || isNil(_.criteriaLibrary.mergedCriteriaExpression) ? "" : _.criteriaLibrary.mergedCriteriaExpression
                    })
            })
        }
        return toReturn
    }

    onUpdateCalcAttr(rowId: string, updatedRow: CalculatedAttribute){
        var addedrow = this.addedCalcAttr.find(_ => _.id === rowId);
        if(!isNil(addedrow)){
            addedrow = updatedRow;
            return;
        }
            
        let mapEntry = this.updatedCalcAttrMap.get(rowId)

        if(isNil(mapEntry)){
            const row = this.CalcAttrCache;
            if(!isNil(row) && hasUnsavedChangesCore('', updatedRow, row) && row.attribute === updatedRow.attribute)
                this.updatedCalcAttrMap.set(rowId, [row , updatedRow])
        }
        else if(hasUnsavedChangesCore('', updatedRow, mapEntry[0])){
            mapEntry[1] = updatedRow;
        }
        else
            this.updatedCalcAttrMap.delete(rowId)

        this.checkHasUnsavedChanges();
    }

    onUpdatePair(rowId: string, updatedRow: CriterionAndEquationSet){
        if(!isNil(this.addedPairs.get(this.selectedAttribute.id)))
            if(any(propEq('id', rowId), this.addedPairs.get(this.selectedAttribute.id)!)){
                let amounts = this.addedPairs.get(this.selectedAttribute.id)!
                amounts[amounts.findIndex(b => b.id == rowId)] = updatedRow;
                return;
            }               

        let mapEntry = this.updatedPairsMaps.get(rowId)

        if(isNil(mapEntry)){
            const row = this.pairsCache.find(r => r.id === rowId);
            if(!isNil(row) && hasUnsavedChangesCore('', updatedRow, row)){
                this.updatedPairsMaps.set(rowId, [row , updatedRow])
                if(!isNil(this.updatedPairs.get(this.selectedAttribute.id)))
                    this.updatedPairs.get(this.selectedAttribute.id)!.push(updatedRow)
                else
                    this.updatedPairs.set(this.selectedAttribute.id, [updatedRow])
            }               
        }
        else if(hasUnsavedChangesCore('', updatedRow, mapEntry[0])){
            mapEntry[1] = updatedRow;
            let amounts = this.updatedPairs.get(this.selectedAttribute.id)
            if(!isNil(amounts))
                amounts[amounts.findIndex(r => r.id == updatedRow.id)] = updatedRow
        }
        else{
            let amounts = this.updatedPairs.get(this.selectedCalculatedAttribute.id)
            if(!isNil(amounts))
                amounts.splice(amounts.findIndex(r => r.id == updatedRow.id), 1)
        }
            

        this.checkHasUnsavedChanges();
    }

    updatedDefaultEquation(defaultEq: CriterionAndEquationSet){
        if(!isNil(defaultEq)){
            var mapEntry = this.defaultEquations.get(this.selectedAttribute.id)
            if(isNil(mapEntry)){
                if(hasUnsavedChangesCore('', defaultEq, this.defaultEquationCache)){
                    this.defaultEquation = clone(defaultEq);
                    this.defaultEquations.set(this.selectedAttribute.id, clone(defaultEq));
                }
            }               
            else{               
                if(hasUnsavedChangesCore('', defaultEq, this.defaultEquationCache)){
                    this.defaultEquation = clone(defaultEq);
                    mapEntry = clone(defaultEq);
                }
            }
        }
    }

    clearChanges(){
        this.updatedPairs.clear();
        this.updatedPairsMaps.clear();
        this.addedPairs.clear();
        this.deletionPairsIds.clear();
        this.updatedCalcAttrMap.clear();
        this.defaultEquations.clear();
        if(this.addedCalcAttr.length > 0){
            var addedIds = this.addedCalcAttr.map(_ => _.id);
            this.calculatedAttributeGridData = this.calculatedAttributeGridData.filter(_ => !addedIds.includes(_.id))
            this.addedCalcAttr = [];
        }  
    }

    resetPage(){
        this.pagination.page = 1;
        this.onPaginationChanged();
    }

    checkHasUnsavedChanges(){
        const hasUnsavedChanges: boolean = 
            this.deletionPairsIds.size > 0 || 
            this.addedPairs.size > 0 ||
            this.updatedCalcAttrMap.size > 0 || 
            this.updatedPairs.size > 0 || 
            this.addedCalcAttr.length > 0 ||
            this.defaultEquations.size > 0 ||
            (this.hasScenario && this.hasSelectedLibrary) ||
            (this.hasSelectedLibrary && hasUnsavedChangesCore('', this.selectedCalculatedAttributeLibrary, this.stateSelectedCalculatedAttributeLibrary))
        this.setHasUnsavedChangesAction({ value: hasUnsavedChanges });
    }

    onSearchClick(){
        this.currentSearch = this.gridSearchTerm;
        this.resetPage();
    }

    onClearClick(){
        this.gridSearchTerm = '';
        this.onSearchClick();
    }

    CheckUnsavedDialog(next: any, otherwise: any) {
        const hasUnsavedChanges: boolean = 
            this.deletionPairsIds.size > 0 || 
            this.addedPairs.size > 0 ||
            this.updatedCalcAttrMap.size > 0 || 
            this.updatedPairs.size > 0 || 
            this.addedCalcAttr.length > 0 ||
            (this.hasSelectedLibrary && !this.hasScenario && hasUnsavedChangesCore('', this.selectedCalculatedAttributeLibrary, this.stateSelectedCalculatedAttributeLibrary))
        if (hasUnsavedChanges && this.unsavedDialogAllowed) {
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

    onShowShareCalculatedAttributeLibraryDialog(calculatedAttributeLibrary: CalculatedAttributeLibrary) {
        this.shareCalculatedAttributeLibraryDialogData = {
            showDialog:true,
            calculatedAttributeLibrary: clone(calculatedAttributeLibrary)
        }
    }

    onShareCalculatedAttributeDialogSubmit(calculatedAttributeLibraryUsers: CalculatedAttributeLibraryUser[]) {
        this.shareCalculatedAttributeLibraryDialogData = clone(emptyShareCalculatedAttributeLibraryDialogData);

                if (!isNil(calculatedAttributeLibraryUsers) && this.selectedCalculatedAttributeLibrary.id !== getBlankGuid())
                {
                    let libraryUserData: LibraryUser[] = [];

                    //create library users
                    calculatedAttributeLibraryUsers.forEach((calculatedAttributeLibraryUser, index) =>
                    {   
                        //determine access level
                        let libraryUserAccessLevel: number = 0;
                        if (libraryUserAccessLevel == 0 && calculatedAttributeLibraryUser.isOwner == true) { libraryUserAccessLevel = 2; }
                        if (libraryUserAccessLevel == 0 && calculatedAttributeLibraryUser.canModify == true) { libraryUserAccessLevel = 1; }

                        //create library user object
                        let libraryUser: LibraryUser = {
                            userId: calculatedAttributeLibraryUser.userId,
                            userName: calculatedAttributeLibraryUser.username,
                            accessLevel: libraryUserAccessLevel
                        }

                        //add library user to an array
                        libraryUserData.push(libraryUser);
                    });
                    if (!isNullOrUndefined(this.selectedCalculatedAttributeLibrary.id) ) {
                        this.getIsSharedLibraryAction(this.selectedCalculatedAttributeLibrary).then(this.isShared = this.isSharedLibrary);
                    }
                    //update calculated attribute library sharing
                    CalculatedAttributeService.upsertOrDeleteCalculatedAttributeLibraryUsers(this.selectedCalculatedAttributeLibrary.id, libraryUserData).then((response: AxiosResponse) => {
                        if (hasValue(response, 'status') && http2XX.test(response.status.toString()))
                        {
                            this.resetPage();
                        }
                    });
                }
    }

    setParentLibraryName(libraryId: string) {
        if (libraryId === "None") {
            this.parentLibraryName = "None";
            return;
        }
        let foundLibrary: CalculatedAttributeLibrary = emptyCalculatedAttributeLibrary;
        this.stateCalculatedAttributeLibraries.forEach(library => {
            if (library.id === libraryId ) {
                foundLibrary = clone(library);
            }
        });
        this.parentLibraryId = foundLibrary.id;
        this.parentLibraryName = foundLibrary.name;
    }

    initializePages(){
        const request: CalculatedAttributePagingRequestModel= {
            page: 1,
            rowsPerPage: 5,
            syncModel: {
                libraryId: this.selectedCalculatedAttributeLibrary.id === this.uuidNIL ? null : this.selectedCalculatedAttributeLibrary.id,
                updatedCalculatedAttributes: Array.from(this.updatedCalcAttrMap.values()).map(r => r[1]),
                deletedPairs: mapToIndexSignature(this.deletionPairsIds),
                updatedPairs: mapToIndexSignature( this.updatedPairs),
                addedPairs: mapToIndexSignature(this.addedPairs),
                addedCalculatedAttributes: this.addedCalcAttr,
                defaultEquations: mapToIndexSignature(this.defaultEquations),
                isModified: false
            },           
            sortColumn: '',
            isDescending: false,
            search: '',
            attributeId: this.stateCalculatedAttributes.find(_ => _.name === this.selectedAttribute.attribute)!.id
        };
        
        if((!this.hasSelectedLibrary && this.hasScenario) && this.selectedScenarioId !== this.uuidNIL){
            CalculatedAttributeService.getScenarioCalculatedAttrbiutetPage(this.selectedScenarioId, request).then(response => {
                if(response.data){
                    let data = response.data as calculcatedAttributePagingPageModel;
                    this.currentPage.equations = data.items;
                    this.currentPage.calculationTiming = data.calculationTiming
                    this.pairsCache = this.currentPage.equations;
                    this.totalItems = data.totalItems;
                    this.defaultEquation = data.defaultEquation;
                    this.defaultEquationCache = clone(this.defaultEquation);
                    this.selectedGridItem = this.calculatedAttributeGridModelConverter(this.currentPage)
                    this.setTimingsMultiSelect(this.currentPage.calculationTiming);

                    this.setParentLibraryName(data.libraryId);
                    this.loadedParentId = data.libraryId;
                    this.loadedParentName = this.parentLibraryName; //store original
                    this.scenarioLibraryIsModified = data.isModified;
                }
                this.initializing = false;
            });
        }            
        else if(this.hasSelectedLibrary)
            CalculatedAttributeService.getLibraryCalculatedAttributePage(this.librarySelectItemValue !== null ? this.librarySelectItemValue : '', request).then(response => {
                if(response.data){
                    let data = response.data as calculcatedAttributePagingPageModel;
                    this.currentPage.equations = data.items;
                    this.currentPage.calculationTiming = data.calculationTiming
                    this.pairsCache = this.currentPage.equations;
                    this.totalItems = data.totalItems;
                    this.defaultEquation = data.defaultEquation;
                    this.defaultEquationCache = clone(this.defaultEquation);
                    this.selectedGridItem = this.calculatedAttributeGridModelConverter(this.currentPage)
                    this.setTimingsMultiSelect(this.currentPage.calculationTiming);
                }
                this.initializing = false;
            });
        else
            this.initializing = false;
    }
}
</script>
<style>
.sharing {
    padding-top: 0;
    margin: 0;
}
.sharing .v-input__slot{
    top: 5px !important;
}
.sharing .v-label{
    margin-bottom: 0;
    position: relative;
    top: -4px !important;
}
</style>