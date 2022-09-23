<template>
    <v-layout column>
        <!-- top row -->
        <v-flex xs12>
            <v-layout justify-space-between>
                <v-flex xs4 class="ghd-constant-header">
                    <v-layout column>
                        <v-subheader class="ghd-md-gray ghd-control-label">Calculated Attribute</v-subheader>
                        <v-select
                            :items="librarySelectItems"
                            append-icon=$vuetify.icons.ghd-down
                            outline
                            v-model="librarySelectItemValue"
                            class="ghd-select ghd-text-field ghd-text-field-border">
                        </v-select>                       
                    </v-layout>
                </v-flex>
                <v-flex xs4 class="ghd-constant-header">
                    <v-layout v-if='hasSelectedLibrary && !hasScenario' style="padding-top: 24px !important" class="shared-owner-flex-padding">
                        <div class="header-text-content owner-padding" style="padding-top: 7px !important">
                            Owner: {{ getOwnerUserName() || '[ No Owner ]' }}
                        </div>
                        <v-divider class="owner-shared-divider" inset vertical></v-divider>
                        <v-switch
                            class='sharing header-text-content'
                            label="Default Calculation"
                            v-model="isDefaultBool"
                            />
                    </v-layout>
                </v-flex>
                <v-flex xs4 class="ghd-constant-header">
                    <v-layout align-end>
                        <v-text-field
                                    prepend-inner-icon=$vuetify.icons.ghd-search
                                    hide-details
                                    lablel="Search"
                                    placeholder="Search Calcultated Attribute"
                                    single-line
                                    v-model="gridSearchTerm"
                                    outline
                                    class="ghd-text-field-border ghd-text-field search-icon-general"
                                    style="margin-top:20px !important"
                                >
                                </v-text-field>
                        <v-btn
                            @click="onShowCreateCalculatedAttributeLibraryDialog(false)"
                            class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                            outline
                            v-show="!hasScenario"
                            style="top: 4px !important">
                            Create New Library
                        </v-btn>
                    </v-layout>
                </v-flex>
            </v-layout>
        </v-flex>
        <!-- attributes and timing -->
        <v-flex xs12 v-show="hasSelectedLibrary || hasScenario">
            <v-layout justify-space-between>
                <v-flex xs6>
                <v-layout column style="float:left; width: 100%">
                    <v-subheader class="ghd-md-gray ghd-control-label">Attribute</v-subheader>
                    <v-select
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
                        :items="attributeTimingSelectItems"
                        append-icon=$vuetify.icons.ghd-down
                        outline
                        v-model="attributeTimingSelectItemValue"
                        :disabled="!isAdmin"
                        class="ghd-select ghd-text-field ghd-text-field-border"
                        v-on:change="setTiming">
                    </v-select>
                </v-layout>
                </v-flex>
            </v-layout>
        </v-flex>
        <!-- data table -->
        <v-flex xs12 v-show="hasSelectedLibrary || hasScenario">
            <v-data-table
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
                            readonly
                            class="sm-txt"
                            :value="props.item.equation"
                            :disabled="!isAdmin">
                            <template slot="append-outer">
                                <v-btn
                                    @click="onShowEquationEditorDialog(props.item.id) "
                                    class="ghd-blue"
                                    icon
                                    v-if="isAdmin">
                                    <img class='img-general img-shift' :src="require('@/assets/icons/edit.svg')"/>
                                </v-btn>
                            </template>
                        </v-text-field>
                    </td>
                    <td class="text-xs-center">
                        <v-text-field
                            readonly
                            class="sm-txt"
                            :value="props.item.criteriaExpression"
                            :disabled="!isAdmin">
                            <template slot="append-outer">
                                <v-btn
                                    @click="onEditCalculatedAttributeCriterionLibrary(props.item.id)"
                                    class="ghd-blue"
                                    icon
                                    v-if="isAdmin">
                                    <img class='img-general img-shift' :src="require('@/assets/icons/edit.svg')"/>
                                </v-btn>
                            </template>
                        </v-text-field>
                    </td>
                    <td class="text-xs-center">
                        <v-btn
                            @click="
                                onRemoveCalculatedAttribute(props.item.id)"
                            class="ghd-blue"
                            icon
                            :disabled="!isAdmin">
                            <img class='img-general' :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
                        </v-btn>
                    </td>
                </template>
            </v-data-table>
            <v-btn
                @click="onAddCriterionEquationSet()"
                class='ghd-blue ghd-button'
                outline
                v-if="isAdmin"
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
                @input="
                    selectedCalculatedAttributeLibrary = {
                        ...selectedCalculatedAttributeLibrary,
                        description: $event,
                    }"/>
        </v-flex>
        <!-- buttons -->
        <v-flex xs12 v-show="hasSelectedLibrary || hasScenario">
            <v-layout justify-center v-show='hasSelectedLibrary || hasScenario'>
                <v-btn
                    :disabled="!hasUnsavedChanges"
                    v-if="isAdmin && hasScenario"
                    @click="onDiscardChanges"
                    class='ghd-blue ghd-button-text ghd-button'
                    flat
                    v-show="hasSelectedLibrary || hasScenario">
                    Cancel
                </v-btn>
                <v-btn
                    @click="onUpsertScenarioCalculatedAttribute"
                    class='ghd-blue-bg white--text ghd-button-text ghd-button'
                    v-show="hasScenario && isAdmin"
                    :disabled="disableCrudButton() || !hasUnsavedChanges">
                    Save
                </v-btn>
                <v-btn
                    @click="onShowConfirmDeleteAlert"
                    class='ghd-blue ghd-button-text ghd-button'
                    flat
                    v-show="!hasScenario"
                    :disabled="!hasSelectedLibrary">
                    Delete Library
                </v-btn>
                <v-btn
                    :disabled="disableCrudButton()"
                    v-if="isAdmin"
                    @click="onShowCreateCalculatedAttributeLibraryDialog(true)"
                    outline
                    class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'>
                    Create as New Library
                </v-btn>
                <v-btn
                    :disabled="disableCrudButton() || !hasUnsavedChanges"
                    @click="onUpsertCalculatedAttributeLibrary"
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
        <CreateCalculatedAttributeLibraryDialog
            :dialogData="createCalculatedAttributeLibraryDialogData"
            @submit="onSubmitCreateCalculatedAttributeLibraryDialogResult"
        />

        <CreateCalculatedAttributeDialog
            :showDialog="showCreateCalculatedAttributeDialog"
            @submit="onSubmitCreateCalculatedAttributeDialogResult"
        />
        <EquationEditorDialog
            :dialogData="equationEditorDialogData"
            @submit="onSubmitEquationEditorDialogResult"
        />
        <CriterionLibraryEditorDialog
            :dialogData="criterionLibraryEditorDialogData"
            @submit="onSubmitCriterionLibraryEditorDialogResult"
        />
    </v-layout>
</template>
<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import { Watch } from 'vue-property-decorator';
import { Action, State, Getter } from 'vuex-class';
import Alert from '@/shared/modals/Alert.vue';
import EquationEditorDialog from '../../shared/modals/EquationEditorDialog.vue';
import CriterionLibraryEditorDialog from '../../shared/modals/CriterionLibraryEditorDialog.vue';
import CreateCalculatedAttributeLibraryDialog from './calculated-attribute-editor-dialogs/CreateCalculatedAttributeLibraryDialog.vue';
import CreateCalculatedAttributeDialog from './calculated-attribute-editor-dialogs/CreateCalculatedAttributeDialog.vue';
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
    map,
    prepend,
    propEq,
    reject,
    update,
} from 'ramda';
import {
    CalculatedAttribute,
    CalculatedAttributeGridModel,
    CalculatedAttributeLibrary,
    CriterionAndEquationSet,
    emptyCalculatedAttribute,
    emptyCalculatedAttributeLibrary,
    emptyCalculatedAttributeGridModel,
    emptyCriterionAndEquationSet,
    Timing,
    TimingMap,
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
import { emptyEquation, Equation } from '@/shared/models/iAM/equation';
import {
    CriterionLibraryEditorDialogData,
    emptyCriterionLibraryEditorDialogData,
} from '@/shared/models/modals/criterion-library-editor-dialog-data';
import {
    CriterionLibrary,
    emptyCriteria,
    emptyCriterionLibrary,
} from '@/shared/models/iAM/criteria';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { SelectItem } from '@/shared/models/vue/select-item';
import { ScenarioRoutePaths } from '@/shared/utils/route-paths';
import { emptySelectItem } from '@/shared/models/vue/select-item';
import { getUserName } from '@/shared/utils/get-user-info';
import { emptyPagination, Pagination } from '@/shared/models/vue/pagination';
import { CalculatedAttributeLibraryUpsertPagingRequestModel, CalculatedAttributePagingRequestModel, CalculatedAttributePagingSyncModel, calculcatedAttributePagingPageModel, PagingPage } from '@/shared/models/iAM/paging';
import { mapToIndexSignature } from '@/shared/utils/conversion-utils';
import CalculatedAttributeService from '@/services/calculated-attribute.service';
import { AxiosResponse } from 'axios';
import { http2XX } from '@/shared/utils/http-utils';

@Component({
    components: {
        CreateCalculatedAttributeLibraryDialog,
        CreateCalculatedAttributeDialog,
        EquationEditorDialog,
        CriterionLibraryEditorDialog,
        ConfirmDeleteAlert: Alert,
    },
})
export default class CalculatedAttributeEditor extends Vue {
    @State(
        state => state.calculatedAttributeModule.calculatedAttributeLibraries,
    )
    stateCalculatedAttributeLibraries: CalculatedAttributeLibrary[];
    @State(
        state =>
            state.calculatedAttributeModule.selectedCalculatedAttributeLibrary,
    )
    stateSelectedCalculatedAttributeLibrary: CalculatedAttributeLibrary;

    @State(
        state => state.calculatedAttributeModule.scenarioCalculatedAttributes,
    )
    stateScenarioCalculatedAttributes: CalculatedAttribute[];
    @State(state => state.calculatedAttributeModule.calculatedAttributes)
    stateCalculatedAttributes: Attribute[];
    @State(state => state.unsavedChangesFlagModule.hasUnsavedChanges)
    hasUnsavedChanges: boolean;
    @State(state => state.authenticationModule.isAdmin) isAdmin: boolean;

    @Action('upsertScenarioCalculatedAttribute')
    upsertScenarioCalculatedAttributeAction: any;
    @Action('upsertCalculatedAttributeLibrary')
    upsertCalculatedAttributeLibraryAction: any;
    @Action('deleteCalculatedAttributeLibrary')
    deleteCalculatedAttributeLibraryAction: any;
    @Action('getCalculatedAttributeLibraries')
    getCalculatedAttributeLibrariesAction: any;
    @Action('getScenarioCalculatedAttribute')
    getScenarioCalculatedAttributeAction: any;
    @Action('selectCalculatedAttributeLibrary')
    selectCalculatedAttributeLibraryAction: any;
    @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;
    @Action('getCalculatedAttributes') getCalculatedAttributesAction: any;
    @Action('addErrorNotification') addErrorNotificationAction: any;

    @Action('addSuccessNotification') addSuccessNotificationAction: any;

    @Getter('getUserNameById') getUserNameByIdGetter: any;

    updatedCalcAttrMap:Map<string, [CalculatedAttribute, CalculatedAttribute]> = 
        new Map<string, [CalculatedAttribute, CalculatedAttribute]>();//0: original value | 1: updated value
    CalcAttrCache: CalculatedAttribute = emptyCalculatedAttribute;
    pairsCache: CriterionAndEquationSet[] = [];
    updatedPairsMaps:Map<string, [CriterionAndEquationSet, CriterionAndEquationSet]> = 
        new Map<string, [CriterionAndEquationSet, CriterionAndEquationSet]>();//0: original value | 1: updated value 
    addedPairs: Map<string, CriterionAndEquationSet[]> = new  Map<string, CriterionAndEquationSet[]>();
    deletionPairsIds: Map<string, string[]> = new Map<string, string[]>();
    updatedPairs:  Map<string, CriterionAndEquationSet[]> = new  Map<string, CriterionAndEquationSet[]>();
    gridSearchTerm = '';
    currentSearch = '';
    pagination: Pagination = clone(emptyPagination);
    isPageInit = false;
    totalItems = 0;
    currentPage: CalculatedAttribute = clone(emptyCalculatedAttribute);
    initializing: boolean = true;

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
    attributeSelectItems: SelectItem[] = [];
    attributeSelectItemValue: string | null = '';
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
    criterionLibraryEditorDialogData: CriterionLibraryEditorDialogData = clone(
        emptyCriterionLibraryEditorDialogData,
    );
    calculatedAttributeGridData: CalculatedAttribute[] = [];
    activeCalculatedAttributeId: string = getBlankGuid();
    selectedGridItem: CalculatedAttributeGridModel[] = [];
    selectedAttribute: CalculatedAttribute = clone(emptyCalculatedAttribute)
    hasCreatedLibrary: boolean = false;

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

            vm.getCalculatedAttributesAction();
            vm.getCalculatedAttributeLibrariesAction();

            vm.setAttributeSelectItems();
            vm.setAttributeTimingSelectItems();

            if (
                to.path.indexOf(ScenarioRoutePaths.CalculatedAttribute) !== -1
            ) {
                vm.selectedScenarioId = to.query.scenarioId;

                if (vm.selectedScenarioId === vm.uuidNIL) {
                    vm.addErrorNotificationAction({
                        message: 'Unable to identify selected scenario.',
                    });
                    vm.$router.push('/Scenarios/');
                }

                vm.hasScenario = true;
                vm.getScenarioCalculatedAttributeAction(vm.selectedScenarioId);
            }
            vm.initializePages();
        });
    }
    beforeDestroy() {
        this.calculatedAttributeGridData = [] as CalculatedAttribute[];
        this.selectedAttribute = clone(emptyCalculatedAttribute);
    }

    // Watchers
    @Watch('pagination')
    onPaginationChanged() {
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
                addedPairs: this.mapToIndexSignature(this.addedPairs) 
            },           
            sortColumn: sortBy === '' ? 'year' : sortBy,
            isDescending: descending != null ? descending : false,
            search: this.currentSearch,
            attributeId: this.selectedAttribute.id
        };
        
        if((!this.hasSelectedLibrary || this.hasScenario) && this.selectedScenarioId !== this.uuidNIL){
            CalculatedAttributeService.getScenarioCalculatedAttrbiutetPage(this.selectedScenarioId, request).then(response => {
                if(response.data){
                    let data = response.data as calculcatedAttributePagingPageModel;
                    this.currentPage.equations = data.items;
                    this.currentPage.calculationTiming = data.calculationTiming
                    this.CalcAttrCache = this.currentPage
                    this.pairsCache = this.currentPage.equations;
                    this.totalItems = data.totalItems;
                }
            });
        }            
        else if(this.hasSelectedLibrary)
             CalculatedAttributeService.getLibraryCalculatedAttributePage(this.librarySelectItemValue !== null ? this.librarySelectItemValue : '', request).then(response => {
                if(response.data){
                    let data = response.data as calculcatedAttributePagingPageModel;
                    this.currentPage.equations = data.items;
                    this.currentPage.calculationTiming = data.calculationTiming
                    this.CalcAttrCache = this.currentPage
                    this.pairsCache = this.currentPage.equations;
                    this.totalItems = data.totalItems;
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

    @Watch('selectedAttribute')
    onSelectedAttributeChanged(){
        this.selectedGridItem = this.calculatedAttributeGridModelConverter(this.currentPage)
    }

    @Watch('isDefaultBool')
    onIsDefaultBoolChanged(){
        this.selectedCalculatedAttributeLibrary.isDefault = this.isDefaultBool;
        this.onSelectedCalculatedAttributeLibraryChanged();
    }
    @Watch('stateCalculatedAttributes')
    onStateCalculatedAttributesChanged() {
        this.setAttributeSelectItems();
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
    @Watch('librarySelectItemValue')
    onLibrarySelectItemValueChanged() {
        this.selectCalculatedAttributeLibraryAction(
            this.librarySelectItemValue,
        );
    }
    @Watch('attributeSelectItemValue')
    onAttributeSelectItemValueChanged() {//check if is possibe to add calc attr(touched)
        // selection change in calculated attribute multi select
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
                item.equations.forEach(_ => {
                    if (isNil(_.criteriaLibrary)) {
                        _.criteriaLibrary = clone(emptyCriterionLibrary);
                    }
                    if (isNil(_.equation)) {
                        _.equation = clone(emptyEquation);
                    }
                });
                this.activeCalculatedAttributeId = item.id;
                this.selectedAttribute = item;
                this.setTimingsMultiSelect(item.calculationTiming);
                this.resetPage();
            } else {
                // if the selected Calculated attribute data is not present in the grid
                // Add a new object for it. Because we cannot loop over a object, which is null
                var newAttributeObject: CalculatedAttribute = {
                    id: getNewGuid(),
                    attribute: this.attributeSelectItemValue,
                    name: this.attributeSelectItemValue,
                    calculationTiming: Timing.OnDemand,
                    equations: [] as CriterionAndEquationSet[],
                };
                this.calculatedAttributeGridData.push(newAttributeObject);
                this.activeCalculatedAttributeId = newAttributeObject.id;
                this.selectedAttribute = newAttributeObject;
                this.setTimingsMultiSelect(Timing.OnDemand);
            }
        }
    }
    @Watch('attributeTimingSelectItemValue')
    onAttributeTimingSelectItemValue() {//(touched)
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
        if (this.isAdmin) {
            const hasUnsavedChanges: boolean = this.hasScenario
                ? hasUnsavedChangesCore(
                      '',
                      this.calculatedAttributeGridData,
                      this.stateScenarioCalculatedAttributes,
                  )
                : this.stateSelectedCalculatedAttributeLibrary.id !=
                  getBlankGuid()
                ? hasUnsavedChangesCore(
                      '',
                      {
                          ...clone(this.selectedCalculatedAttributeLibrary),
                          calculatedAttributes: clone(
                              this.calculatedAttributeGridData,
                          ),
                      },
                      this.stateSelectedCalculatedAttributeLibrary,
                  )
                : false;
            this.setHasUnsavedChangesAction({ value: hasUnsavedChanges });
        }
    }
    @Watch('selectedCalculatedAttributeLibrary')
    onSelectedCalculatedAttributeLibraryChanged() {//library change, needs to call on pagination(touched)
        // change in library multiselect
        if (
            this.selectedCalculatedAttributeLibrary.id !== this.uuidNIL &&
            this.selectedCalculatedAttributeLibrary.id != getBlankGuid()
        ) {
            this.hasSelectedLibrary = true;
        } else {
            this.hasSelectedLibrary = false;
        }

        // If grid data is null, then add dummy objects on the fly, to show the rows to users.
        // So that, the users can add criteria and equations
        // if (
        //     this.calculatedAttributeGridData == undefined ||
        //     this.calculatedAttributeGridData.length == 0
        // ) {
        //     this.attributeSelectItems.forEach(_ => {
        //         var tempItem: CalculatedAttribute = {
        //             id: getNewGuid(),
        //             attribute: _.text,
        //             name: _.text,
        //             calculationTiming: Timing.OnDemand,
        //             equations: [] as CriterionAndEquationSet[],
        //         };
        //         this.calculatedAttributeGridData.push(tempItem);
        //         this.setDefaultAttributeOnLoad(
        //             this.calculatedAttributeGridData[0],
        //         );
        //     });
        // }

        if (this.hasScenario && this.hasSelectedLibrary) {
            // we need new ids for the object which is assigned to a scenario.
            this.calculatedAttributeGridData = this.selectedCalculatedAttributeLibrary.calculatedAttributes.map(
                (value: CalculatedAttribute) => ({
                    ...value,
                    id: getNewGuid(),
                }),
            );
            // If a user in scenario page, then selecting a library should generate new Ids.
            // Because this data will be saved against the scenario
            // this.calculatedAttributeGridData.forEach(att => {
            //     att.equations.map((value: CriterionAndEquationSet) => ({
            //         ...value,
            //         id: getNewGuid(),
            //     }));
            //     att.equations.forEach(eq => {
            //         eq.id = getNewGuid();
            //         if (isNil(eq.criteriaLibrary)) {
            //             eq.criteriaLibrary = clone(emptyCriterionLibrary);
            //             eq.criteriaLibrary.id = getNewGuid();
            //             eq.criteriaLibrary.isSingleUse = true;
            //         } else {
            //             eq.criteriaLibrary.id = getNewGuid();
            //         }
            //         eq.equation.id = getNewGuid();
            //     });
            // });

            // Set the default values in Calculated attribute multi select, if we have data in calculatedAttributeGridData
            if (
                this.calculatedAttributeGridData != undefined &&
                this.calculatedAttributeGridData.length > 0
            ) {
                this.setDefaultAttributeOnLoad(
                    this.calculatedAttributeGridData[0],
                );
                this.onPaginationChanged();
            } else {
                this.isAttributeSelectedItemValue = false;
            }
        } else if (this.hasScenario && !this.hasSelectedLibrary) {
            // If a user un select a Library, then reset the grid data from the scenario calculated attribute state
            this.resetGridData();
        } else if (!this.hasScenario) {
            // If a user is in Lirabry page
            this.calculatedAttributeGridData = clone(
                this.stateSelectedCalculatedAttributeLibrary.calculatedAttributes,
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
                this.onPaginationChanged();
            } else {
                this.isAttributeSelectedItemValue = false;
                this.attributeSelectItemValue = null;
                this.attributeTimingSelectItemValue = null;
                this.isTimingSelectedItemValue = false;
            }
        }
        this.onCalculatedAttributeGridDataChanged();
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

    onUpsertScenarioCalculatedAttribute() {//scenario upsert things(touched)
        const syncModel: CalculatedAttributePagingSyncModel = {
                libraryId: this.selectedCalculatedAttributeLibrary.id === this.uuidNIL ? null : this.selectedCalculatedAttributeLibrary.id,
                updatedCalculatedAttributes: Array.from(this.updatedCalcAttrMap.values()).map(r => r[1]),
                deletedPairs: mapToIndexSignature(this.deletionPairsIds),
                updatedPairs: mapToIndexSignature( this.updatedPairs),
                addedPairs: mapToIndexSignature(this.addedPairs) 
        }
        CalculatedAttributeService.upsertScenarioCalculatedAttribute(syncModel, this.selectedScenarioId).then(((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                this.clearChanges()
                this.resetPage();
                this.addSuccessNotificationAction({message: "Modified calculated attrbutes"});
                this.librarySelectItemValue = null
            }   
        }))
    }

    onUpsertCalculatedAttributeLibrary() {//library upsert things(touched)
        const syncModel: CalculatedAttributePagingSyncModel = {
                libraryId: this.selectedCalculatedAttributeLibrary.id === this.uuidNIL ? null : this.selectedCalculatedAttributeLibrary.id,
                updatedCalculatedAttributes: Array.from(this.updatedCalcAttrMap.values()).map(r => r[1]),
                deletedPairs: mapToIndexSignature(this.deletionPairsIds),
                updatedPairs: mapToIndexSignature( this.updatedPairs),
                addedPairs: mapToIndexSignature(this.addedPairs) 
        }
        const request: CalculatedAttributeLibraryUpsertPagingRequestModel = {
            syncModel: syncModel,
            isNewLibrary: false,
            library: this.selectedCalculatedAttributeLibrary
        }
        CalculatedAttributeService.upsertCalculatedAttributeLibrary(request).then(((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                this.clearChanges()
                this.resetPage();
                //this.selectedBudgetLibraryMutator(this.selectedBudgetLibrary);
                this.addSuccessNotificationAction({message: "Updated calculated attribute library",});
            }   
        }))
    }
    onShowCreateCalculatedAttributeLibraryDialog(createAsNewLibrary: boolean) {//might need stuff
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
    onSubmitCreateCalculatedAttributeLibraryDialogResult(//new library upsert stuff(touched)
        calculatedAttributeLibrary: CalculatedAttributeLibrary,
    ) {
        this.createCalculatedAttributeLibraryDialogData = clone(
            emptyCreateCalculatedAttributeLibraryDialogData,
        );

        if (!isNil(calculatedAttributeLibrary)) {
            this.upsertCalculatedAttributeLibraryAction(
                calculatedAttributeLibrary,
            );
            const syncModel: CalculatedAttributePagingSyncModel = {
                libraryId: calculatedAttributeLibrary.calculatedAttributes.length === 0 ? null : this.selectedCalculatedAttributeLibrary.id,
                updatedCalculatedAttributes: calculatedAttributeLibrary.calculatedAttributes.length === 0 ? [] : Array.from(this.updatedCalcAttrMap.values()).map(r => r[1]),
                deletedPairs: calculatedAttributeLibrary.calculatedAttributes.length === 0 ? {} : mapToIndexSignature(this.deletionPairsIds),
                updatedPairs: calculatedAttributeLibrary.calculatedAttributes.length === 0 ? {} : mapToIndexSignature( this.updatedPairs),
                addedPairs: calculatedAttributeLibrary.calculatedAttributes.length === 0 ? {} : mapToIndexSignature(this.addedPairs) 
            }
            const request: CalculatedAttributeLibraryUpsertPagingRequestModel = {
                syncModel: syncModel,
                isNewLibrary: false,
                library: this.selectedCalculatedAttributeLibrary
            }
            CalculatedAttributeService.upsertCalculatedAttributeLibrary(request).then(((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    this.clearChanges()
                    this.resetPage();
                    //this.selectedBudgetLibraryMutator(this.selectedBudgetLibrary);
                    this.addSuccessNotificationAction({message: "Updated calculated attribute library",});
                    this.librarySelectItemValue = calculatedAttributeLibrary.id;
                    this.hasCreatedLibrary = true;
                }   
            }))
           
        }
    }
    onSubmitCreateCalculatedAttributeDialogResult(//might be deprecated
        newCalculatedAttribute: CalculatedAttribute[],
    ) {
        this.showCreateCalculatedAttributeDialog = false;

        if (!isNil(newCalculatedAttribute)) {
            this.calculatedAttributeGridData = clone(newCalculatedAttribute);
        }
    }

    disableCrudButton() {//gonna have to do something with this
        if (this.calculatedAttributeGridData == undefined) {
            return false;
        }
        const dataIsValid = this.calculatedAttributeGridData.every(_ =>
             this.rules['generalRules'].valueIsNotEmpty(_.equations) === true &&
             (
                _.equations.length < 2 || 
                    (
                        _.equations.filter((set: CriterionAndEquationSet) => 
                        this.rules['generalRules'].valueIsNotEmpty(set.criteriaLibrary) === true &&
                        this.rules['generalRules'].valueIsNotEmpty(set.criteriaLibrary.mergedCriteriaExpression) !== true).length < 2
                    )
             ) &&
            _.equations.every((set: CriterionAndEquationSet) => {
                return (
                    this.rules['generalRules'].valueIsNotEmpty(set.id) === true &&
                    this.rules['generalRules'].valueIsNotEmpty(set.criteriaLibrary) === true &&
                    this.rules['generalRules'].valueIsNotEmpty(set.equation.expression) === true                 
                );
            }),
        );

        if (this.hasSelectedLibrary) {
            return !(
                this.rules['generalRules'].valueIsNotEmpty(
                    this.selectedCalculatedAttributeLibrary.name,
                ) === true && dataIsValid
            );
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
    onAddCriterionEquationSet() { //adds new pair(touched)
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
        // if (this.selectedAttribute.equations == undefined) {
        //     this.selectedAttribute.equations = [];
        //     this.onSelectedAttributeChanged()
        // }
        // this.selectedAttribute.equations.push(newSet);
        // this.calculatedAttributeGridData = update(
        //     findIndex(
        //         propEq('id', this.selectedAttribute.id),
        //         this.calculatedAttributeGridData,
        //     ),
        //     { ...this.selectedAttribute },
        //     this.calculatedAttributeGridData,
        // );
        // this.onSelectedAttributeChanged()
    }
    onEditCalculatedAttributeCriterionLibrary(criterionEquationSetId: string) {//opens criterion editor
        var currItem = this.calculatedAttributeGridData.find(
            _ => _.id == this.activeCalculatedAttributeId,
        )!;
        var currentCriteria = currItem.equations.find(
            _ => _.id == criterionEquationSetId,
        )!;
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

            this.criterionLibraryEditorDialogData = {
                showDialog: true,
                libraryId: currentCriteria.criteriaLibrary.id,
                isCallFromScenario: this.hasScenario,
                isCriterionForLibrary: !this.hasScenario,
            };
        }
    }

    onSubmitCriterionLibraryEditorDialogResult(//edits criterion of pair
        criterionLibrary: CriterionLibrary,
    ) {
        this.criterionLibraryEditorDialogData = clone(
            emptyCriterionLibraryEditorDialogData,
        );

        var currItem = this.calculatedAttributeGridData.find(
            _ => _.id == this.activeCalculatedAttributeId,
        )!;
        if (!isNil(criterionLibrary) && this.hasSelectedCalculatedAttribute) {
            currItem.equations.map(item => {
                item.id == this.currentCriteriaEquationSetSelectedId
                    ? (item.criteriaLibrary.mergedCriteriaExpression =
                          criterionLibrary.mergedCriteriaExpression)
                    : item.criteriaLibrary;
            });

            this.calculatedAttributeGridData = update(
                findIndex(
                    propEq('id', currItem.id),
                    this.calculatedAttributeGridData,
                ),
                { ...currItem },
                this.calculatedAttributeGridData,
            );
            this.onSelectedAttributeChanged();
        }

        this.selectedCalculatedAttribute = clone(emptyCalculatedAttribute);
        this.hasSelectedCalculatedAttribute = false;
    }
    onShowEquationEditorDialog(criterionEquationSetId: string) {//edits equation of pair
        var currItem = this.calculatedAttributeGridData.find(
            _ => _.id == this.activeCalculatedAttributeId,
        )!;
        var currentEquation = currItem.equations.find(
            _ => _.id == criterionEquationSetId,
        );
        this.currentCriteriaEquationSetSelectedId = criterionEquationSetId;
        if (!isNil(currentEquation)) {
            this.hasSelectedCalculatedAttribute = true;

            this.equationEditorDialogData = {
                showDialog: true,
                equation: currentEquation.equation,
            };
        }
    }
    onSubmitEquationEditorDialogResult(equation: Equation) {//opens equation editor
        this.equationEditorDialogData = clone(emptyEquationEditorDialogData);

        if (!isNil(equation) && this.hasSelectedCalculatedAttribute) {
            var currItem = this.calculatedAttributeGridData.find(
                _ => _.id == this.activeCalculatedAttributeId,
            )!;
            currItem.equations.map(item => {
                item.id == this.currentCriteriaEquationSetSelectedId
                    ? (item.equation = equation)
                    : item.equation;
            });
            this.onSelectedAttributeChanged();
        }

        this.selectedCalculatedAttribute = clone(emptyCalculatedAttribute);
        this.hasSelectedCalculatedAttribute = false;
    }
    onRemoveCalculatedAttribute(criterionEquationSetId: string) {//removes pair
        var currItem = this.calculatedAttributeGridData.find(
            _ => _.id == this.activeCalculatedAttributeId,
        )!;
        currItem.equations = reject(
            propEq('id', criterionEquationSetId),
            currItem.equations,
        );
        this.selectedAttribute.equations = reject(
            propEq('id', criterionEquationSetId),
            this.selectedAttribute.equations,
        );
        this.onSelectedAttributeChanged()
    }
    onDiscardChanges() {//needs work
        this.librarySelectItemValue = null;
        this.selectedCalculatedAttributeLibrary = clone(
            emptyCalculatedAttributeLibrary,
        );
        setTimeout(() => {
            if (this.hasScenario) {
                this.resetGridData();
                this.onAttributeSelectItemValueChanged()
            }
        });
    }

    resetGridData() {//take a look at this
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
    setDefaultAttributeOnLoad(localCalculatedAttribute: CalculatedAttribute) {//might want to look at
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
        let mapEntry = this.updatedCalcAttrMap.get(rowId)

        if(isNil(mapEntry)){
            const row = this.CalcAttrCache;
            if(!isNil(row) && hasUnsavedChangesCore('', updatedRow, row))
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
            // this.updatedCalcAttrMap.delete(rowId)
            let amounts = this.updatedPairs.get(this.selectedCalculatedAttribute.id)
            if(!isNil(amounts))
                amounts.splice(amounts.findIndex(r => r.id == updatedRow.id), 1)
        }
            

        this.checkHasUnsavedChanges();
    }

    clearChanges(){
        this.updatedPairs.clear();
        this.updatedPairsMaps.clear
        this.addedPairs.clear();
        this.deletionPairsIds.clear();
        this.updatedCalcAttrMap.clear();
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
            (this.hasScenario && this.hasSelectedLibrary) 
        this.setHasUnsavedChangesAction({ value: hasUnsavedChanges });
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
                addedPairs: this.mapToIndexSignature(this.addedPairs) 
            },           
            sortColumn: '',
            isDescending: false,
            search: '',
            attributeId: this.selectedAttribute.id
        };
        
        if((!this.hasSelectedLibrary || this.hasScenario) && this.selectedScenarioId !== this.uuidNIL){
            CalculatedAttributeService.getScenarioCalculatedAttrbiutetPage(this.selectedScenarioId, request).then(response => {
                if(response.data){
                    let data = response.data as calculcatedAttributePagingPageModel;
                    this.currentPage.equations = data.items;
                    this.currentPage.calculationTiming = data.calculationTiming
                    this.CalcAttrCache = this.currentPage
                    this.pairsCache = this.currentPage.equations;
                    this.totalItems = data.totalItems;
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
                    this.CalcAttrCache = this.currentPage
                    this.pairsCache = this.currentPage.equations;
                    this.totalItems = data.totalItems;
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
}
</style>