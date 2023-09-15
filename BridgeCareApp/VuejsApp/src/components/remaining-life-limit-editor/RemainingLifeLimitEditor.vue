<template>
    <v-layout column>
      <v-flex xs12>
        <v-layout justify-space-between>
          <v-flex xs3 class="ghd-constant-header">
              <v-layout column>
                  <v-subheader class="ghd-control-label ghd-md-gray">Remaining Life Limit Library</v-subheader>
                  <v-select id="RemainingLifeLimitEditor-lifeLimitLibrary-select"
                            class="ghd-select ghd-text-field ghd-text-field-border vs-style"
                            :items="selectListItems"
                            append-icon=$vuetify.icons.ghd-down
                            v-model="librarySelectItemValue"
                            outline
                            >
                  </v-select>
                  <div class="ghd-md-gray ghd-control-subheader budget-parent" v-if='hasScenario'><b>Library Used: {{parentLibraryName}}<span v-if="scenarioLibraryIsModified">&nbsp;(Modified)</span></b></div>
              </v-layout>
          </v-flex>
          <v-flex xs4 class="ghd-constant-header">
                    <v-layout v-if="hasSelectedLibrary && !hasScenario" style="padding-top: 18px; padding-left: 5px" align-center>
                        <div class="header-text-content owner-padding">
                            Owner: {{ getOwnerUserName() || '[ No Owner ]' }} | Date Modified: {{ dateModified }}
                        </div>
                        <v-divider  vertical 
                            v-if="hasSelectedLibrary && !hasScenario">
                        </v-divider>
                        <v-badge v-show="isShared" style="padding: 7px">
                            <template v-slot: badge>
                                <span>Shared</span>
                            </template>
                        </v-badge>
                        <v-btn @click='onShowShareRemainingLifeLimitLibraryDialog(selectedRemainingLifeLimitLibrary)' class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline
                            v-show='!hasScenario'>
                            Share Library
                        </v-btn>
                    </v-layout>
                </v-flex>
                <v-flex xs4 class="ghd-constant-header">
                <v-layout justify-end align-end style="padding-top: 18px !important;">
                    <div>
                        <v-btn id="RemainingLifeLimitEditor-addRemainingLifeLimit-btn" class="ghd-white-bg ghd-blue ghd-button" @click="onShowCreateRemainingLifeLimitDialog" v-show="librarySelectItemValue != null || hasScenario" outline>Add Remaining Life Limit</v-btn>
                        <v-btn class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' style ="ri"  @click="onShowCreateRemainingLifeLimitLibraryDialog(false)" v-show="!hasScenario" outline>Create New Library</v-btn>
                    </div>
                </v-layout>
                </v-flex>
            </v-layout>
        </v-flex>
        <div v-show="librarySelectItemValue != null || hasScenario">
            <v-data-table
            id="RemainingLifeLimitEditor-attributes-dataTable"
            :headers="gridHeaders"
            :items="currentPage"  
            :pagination.sync="pagination"
            :must-sort='true'
            :total-items="totalItems"
            :rows-per-page-items=[5,10,25]
            sort-icon=$vuetify.icons.ghd-table-sort
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
                                        append-icon=$vuetify.icons.ghd-down
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
                        <td v-if="props.item.criterionLibrary.mergedCriteriaExpression != '' && props.item.criterionLibrary.mergedCriteriaExpression != null" >
                            {{ props.item.criterionLibrary.mergedCriteriaExpression}}
                        </td>
                        <td v-else>-
                        </td>
                        <td class="px-0">
                            <v-btn @click="onShowCriterionLibraryEditorDialog(props.item)" icon>
                                <img class='img-general' :src="require('@/assets/icons/edit.svg')"/>
                            </v-btn>   
                        </td>
                        <td justify-end>
                            <v-btn @click="onRemoveRemainingLifeLimitIcon(props.item)" icon>
                                <img class='img-general' :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
                            </v-btn>                          
                        </td>
                    </tr>
                </template>
                </v-data-table>
                <v-layout justify-start align-center class="pa-2">
                </v-layout>
                <v-divider></v-divider>
                <v-flex v-show="!hasScenario" xs12 class="px-0">
                    <v-subheader class="ghd-control-label ghd-md-gray">Description</v-subheader>
                    <v-textarea
                        class="ghd-control-text ghd-control-border"
                        v-model="selectedRemainingLifeLimitLibrary.description"
                        @input="checkHasUnsavedChanges()"
                        outline
                    >
                    </v-textarea>
                </v-flex>
                <v-layout justify-center row>
                    <v-btn id="RemainingLifeLimitEditor-cancel-btn" class="ghd-blue" outline v-show="hasScenario" @click="onDiscardChanges" :disabled="!hasUnsavedChanges">Cancel</v-btn>
                    <v-btn id="RemainingLifeLimitEditor-deleteLibrary-btn" class="ghd-blue" outline v-show="!hasScenario" @click="onShowConfirmDeleteAlert">Delete Library</v-btn>
                    <v-btn id="RemainingLifeLimitEditor-createAsNewLibrary-btn" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' @click="onShowCreateRemainingLifeLimitLibraryDialog(true)" outline>Create as New Library</v-btn>
                    <v-btn id="RemainingLifeLimitEditor-save-btn" class="ghd-blue-bg ghd-white ghd-button" v-show="hasScenario" @click="onUpsertScenarioRemainingLifeLimits" :disabled="disableCrudButton() || !hasUnsavedChanges">Save</v-btn>
                    <v-btn id="RemainingLifeLimitEditor-updateLibrary-btn" class="ghd-blue-bg ghd-white ghd-button" v-show="!hasScenario" :disabled="disableCrudButton() || !hasUnsavedChanges" @click="onUpsertRemainingLifeLimitLibrary">Update Library</v-btn>
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
        <ShareRemainingLifeLimitLibraryDialog :dialogData="shareRemainingLifeLimitLibraryDialogData"
            @submit="onShareRemainingLifeLimitDialogSubmit" 
        />
        <GeneralCriterionEditorDialog 
          :dialogData="criterionEditorDialogData"
          @submit="onEditRemainingLifeLimitCriterionLibrary"
        />
    </v-layout>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import { Action, State, Getter, Mutation } from 'vuex-class';
import { Watch } from 'vue-property-decorator';
import {
    emptyRemainingLifeLimit,
    emptyRemainingLifeLimitLibrary,
    RemainingLifeLimit,
    RemainingLifeLimitLibrary,
    RemainingLifeLimitLibraryUser
} from '@/shared/models/iAM/remaining-life-limit';
import { getPropertyValues } from '@/shared/utils/getter-utils';
import { clone, isNil, propEq, any, find } from 'ramda';
import { hasValue } from '@/shared/utils/has-value-util';
import { SelectItem } from '@/shared/models/vue/select-item';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { Attribute } from '@/shared/models/iAM/attribute';
import CreateRemainingLifeLimitDialog from '@/components/remaining-life-limit-editor/remaining-life-limit-editor-dialogs/CreateRemainingLifeLimitDialog.vue';
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
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { ScenarioRoutePaths } from '@/shared/utils/route-paths';
import { getUserName } from '@/shared/utils/get-user-info';
import GeneralCriterionEditorDialog from '@/shared/modals/GeneralCriterionEditorDialog.vue';
import { emptyGeneralCriterionEditorDialogData, GeneralCriterionEditorDialogData } from '@/shared/models/modals/general-criterion-editor-dialog-data';
import { emptyPagination, Pagination } from '@/shared/models/vue/pagination';
import { LibraryUpsertPagingRequest, PagingPage, PagingRequest } from '@/shared/models/iAM/paging';
import ShareRemainingLifeLimitLibraryDialog from '@/components/remaining-life-limit-editor/remaining-life-limit-editor-dialogs/ShareRemainingLifeLimitLibraryDialog.vue'; 
import RemainingLifeLimitService from '@/services/remaining-life-limit.service';
import { emptyShareRemainingLifeLimitLibraryDialogData, ShareRemainingLifeLimitLibraryDialogData } from '@/shared/models/modals/share-remaining-life-limit-data';
import { AxiosResponse } from 'axios';
import { http2XX } from '@/shared/utils/http-utils';
import { isNullOrUndefined } from 'util';
import { LibraryUser } from '@/shared/models/iAM/user';

@Component({
    components: {
        CreateRemainingLifeLimitLibraryDialog,
        CreateRemainingLifeLimitDialog,
        GeneralCriterionEditorDialog,
        ConfirmDeleteAlert: Alert,
        ShareRemainingLifeLimitLibraryDialog
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
    @State(state => state.authenticationModule.hasAdminAccess) hasAdminAccess: boolean;
    @State(state => state.remainingLifeLimitModule.isSharedLibrary) isSharedLibrary: boolean;
    @Action('getIsSharedRemainingLifeLimitLibrary') getIsSharedLibraryAction: any;

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
    @Action('addSuccessNotification') addSuccessNotificationAction: any;
    @Action('getCurrentUserOrSharedScenario') getCurrentUserOrSharedScenarioAction: any;
    @Action('selectScenario') selectScenarioAction: any;
    
    @Mutation('addedOrUpdatedRemainingLifeLimitLibraryMutator') addedOrUpdatedRemainingLifeLimitLibraryMutator: any;
    @Mutation('selectedRemainingLifeLimitLibraryMutator') selectedRemainingLifeLimitLibraryMutator: any;

    @Getter('getUserNameById') getUserNameByIdGetter: any;

    remainingLifeLimitLibraries: RemainingLifeLimitLibrary[] = [];
    selectedRemainingLifeLimitLibrary: RemainingLifeLimitLibrary = clone(
        emptyRemainingLifeLimitLibrary,
    );
    selectedGridRows: RemainingLifeLimit[] = [];
    selectedRemainingLifeIds: string[] = [];
    selectedScenarioId: string = getBlankGuid();
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

    addedRows: RemainingLifeLimit[] = [];
    updatedRowsMap:Map<string, [RemainingLifeLimit, RemainingLifeLimit]> = new Map<string, [RemainingLifeLimit, RemainingLifeLimit]>();//0: original value | 1: updated value
    deletionIds: string[] = [];
    rowCache: RemainingLifeLimit[] = [];
    gridSearchTerm = '';
    currentSearch = '';
    pagination: Pagination = clone(emptyPagination);
    isPageInit = false;
    totalItems = 0;
    currentPage: RemainingLifeLimit[] = [];
    initializing: boolean = true;
    dateModified: string;

    unsavedDialogAllowed: boolean = true;
    trueLibrarySelectItemValue: string | null = ''
    librarySelectItemValueAllowedChanged: boolean = true;
    librarySelectItemValue: string | null = null;
    isShared: boolean = false;

    shareRemainingLifeLimitLibraryDialogData: ShareRemainingLifeLimitLibraryDialogData = clone(emptyShareRemainingLifeLimitLibraryDialogData);

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
    criterionEditorDialogData: GeneralCriterionEditorDialogData = clone(
        emptyGeneralCriterionEditorDialogData,
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
    parentLibraryName: string = "None";
    parentLibraryId: string = "";
    scenarioLibraryIsModified: boolean = false;
    loadedParentName: string = "";
    loadedParentId: string = "";
    newLibrarySelection: boolean = false;

    beforeRouteEnter(to: any, from: any, next: any) {
        next((vm: any) => {
            vm.librarySelectItemValue = null;
            vm.getRemainingLifeLimitLibrariesAction().then(() => {
                if (to.path.indexOf(ScenarioRoutePaths.RemainingLifeLimit) !== -1) {
                    vm.selectedScenarioId = to.query.scenarioId;
                    if (vm.selectedScenarioId === vm.uuidNIL) {
                        vm.addErrorNotificationAction({
                            message: 'Found no selected scenario for edit',
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
            
        });
    }

    beforeDestroy() {
        this.setHasUnsavedChangesAction({ value: false });
    }
    @Watch('selectedGridRows')
        onSelectedGridRowsChanged() {
            this.selectedRemainingLifeIds = getPropertyValues('id', this.selectedGridRows,) as string[];
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

    @Watch('librarySelectItemValue')
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
        this.parentLibraryId = this.librarySelectItemValue ? this.librarySelectItemValue : "";
        this.newLibrarySelection = true;

    }
    onLibrarySelectItemValueChanged() {
        this.trueLibrarySelectItemValue = this.librarySelectItemValue
        this.selectRemainingLifeLimitLibraryAction({
            libraryId: this.librarySelectItemValue,
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
        this.hasSelectedLibrary =  this.selectedRemainingLifeLimitLibrary.id !== this.uuidNIL;
        this.clearChanges();
        this.initializing = false;
        if(this.hasSelectedLibrary)
            this.onPaginationChanged();
    }

    @Watch('stateScenarioRemainingLifeLimits')
    onStateScenarioRemainingLifeLimitsChanged() {
        if (this.hasScenario) {
            this.currentPage = clone(this.stateScenarioRemainingLifeLimits);
        }
    }
    @Watch('currentPage')
    onGridDataChanged() {
           // Get parent name from library id
        this.selectListItems.forEach(library => {
            if (library.value === this.parentLibraryId) {
                this.parentLibraryName = library.text;
            }
        });
    }
    
    @Watch('isSharedLibrary')
    onStateSharedAccessChanged() {
        this.isShared = this.isSharedLibrary;
    }
    
    @Watch('stateNumericAttributes')
    onStateNumericAttributesChanged() {
        this.setAttributesSelectListItems();
    }

    @Watch('pagination')
    async onPaginationChanged() {
        if(this.initializing)
            return;
        this.checkHasUnsavedChanges();
        const { sortBy, descending, page, rowsPerPage } = this.pagination;
        const request: PagingRequest<RemainingLifeLimit>= {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId: this.librarySelectItemValue !== null ? this.librarySelectItemValue : null,
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
            await RemainingLifeLimitService.getScenarioRemainingLifeLimitPage(this.selectedScenarioId, request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<RemainingLifeLimit>;
                    this.currentPage = data.items;
                    this.rowCache = clone(this.currentPage)
                    this.totalItems = data.totalItems;
                }
            });
        else if(this.hasSelectedLibrary)
            await RemainingLifeLimitService.getRemainingLibraryDate(this.librarySelectItemValue !== null ? this.librarySelectItemValue : '').then(response => {
                  if (hasValue(response, 'status') && http2XX.test(response.status.toString()) && response.data)
                   {
                      var data = response.data as string;
                      this.dateModified = data.slice(0, 10);
                   }
             }),     
             await RemainingLifeLimitService.getLibraryRemainingLifeLimitPage(this.librarySelectItemValue !== null ? this.librarySelectItemValue : '', request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<RemainingLifeLimit>;
                    this.currentPage = data.items;
                    this.rowCache = clone(this.currentPage)
                    this.totalItems = data.totalItems;
                    if (!isNullOrUndefined(this.selectedRemainingLifeLimitLibrary.id) ) {
                        this.getIsSharedLibraryAction(this.selectedRemainingLifeLimitLibrary).then(this.isShared = this.isSharedLibrary);
                    }
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
    onRemoveRemainingLifeLimitIcon(remainingLifeLimit: RemainingLifeLimit) {
        this.removeRowLogic(remainingLifeLimit.id);
        this.onPaginationChanged();
    }

    onRemoveRemainingLifeLimit() {
        this.selectedRemainingLifeIds.forEach(_ => {
            this.removeRowLogic(_);
        });

        this.selectedRemainingLifeIds = [];
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

    onShowCreateRemainingLifeLimitLibraryDialog(createAsNewLibrary: boolean) {
        this.createRemainingLifeLimitLibraryDialogData = {
            showDialog: true,
            remainingLifeLimits: createAsNewLibrary
                ? this.currentPage
                : [],
        };
    }

    onSubmitCreateRemainingLifeLimitLibraryDialogResult(library: RemainingLifeLimitLibrary) {
        this.createRemainingLifeLimitLibraryDialogData = clone(emptyCreateRemainingLifeLimitLibraryDialogData);

        if (!isNil(library)) {
            const upsertRequest: LibraryUpsertPagingRequest<RemainingLifeLimitLibrary, RemainingLifeLimit> = {
                library: library,    
                isNewLibrary: true,           
                 syncModel: {
                    libraryId: library.remainingLifeLimits.length == 0 || !this.hasSelectedLibrary ? null : this.selectedRemainingLifeLimitLibrary.id,
                    rowsForDeletion: library.remainingLifeLimits.length == 0 ? [] : this.deletionIds,
                    updateRows: library.remainingLifeLimits.length == 0 ? [] : Array.from(this.updatedRowsMap.values()).map(r => r[1]),
                    addedRows: library.remainingLifeLimits.length == 0 ? [] : this.addedRows,
                    isModified: false
                 },
                 scenarioId: this.hasScenario ? this.selectedScenarioId : null
            }
            RemainingLifeLimitService.upsertRemainingLifeLimitLibrary(upsertRequest).then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    this.hasCreatedLibrary = true;
                    this.librarySelectItemValue = library.id;
                    
                    if(library.remainingLifeLimits.length == 0){
                        this.clearChanges();
                    }

                    this.addedOrUpdatedRemainingLifeLimitLibraryMutator(library);
                    this.selectedRemainingLifeLimitLibraryMutator(library.id);
                    this.addSuccessNotificationAction({message:'Added remaining life limit library'})
                }               
            })
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
            this.addedRows.push(newRemainingLifeLimit);
            this.onPaginationChanged()
        }
    }

    onEditRemainingLifeLimitProperty(remainingLifeLimit: RemainingLifeLimit, property: string, value: any) {
        this.onUpdateRow(remainingLifeLimit.id, clone(remainingLifeLimit))
        this.onPaginationChanged();
    }

    onShowCriterionLibraryEditorDialog(remainingLifeLimit: RemainingLifeLimit) {
        this.selectedRemainingLifeLimit = remainingLifeLimit;

        this.criterionEditorDialogData = {
            showDialog: true,
            CriteriaExpression: remainingLifeLimit.criterionLibrary.mergedCriteriaExpression,           
        };
    }

    onEditRemainingLifeLimitCriterionLibrary(
        criteriaExpression: string | null,
    ) {
        this.criterionEditorDialogData = clone(emptyGeneralCriterionEditorDialogData);

        if (!isNil(criteriaExpression) && this.selectedRemainingLifeLimit.id !== this.uuidNIL) {
            if(this.selectedRemainingLifeLimit.criterionLibrary.id === getBlankGuid())
                this.selectedRemainingLifeLimit.criterionLibrary.id = getNewGuid();

            this.onUpdateRow(this.selectedRemainingLifeLimit.id, 
            {
                ...this.selectedRemainingLifeLimit,
                criterionLibrary: {...this.selectedRemainingLifeLimit.criterionLibrary, mergedCriteriaExpression: criteriaExpression}
            })
                
            this.onPaginationChanged();
        }

        this.selectedRemainingLifeLimit = clone(emptyRemainingLifeLimit);
    }

    onUpsertRemainingLifeLimitLibrary() {
        const library: RemainingLifeLimitLibrary = {
            ...clone(this.selectedRemainingLifeLimitLibrary),
            remainingLifeLimits: clone(this.currentPage)
        };
        const upsertRequest: LibraryUpsertPagingRequest<RemainingLifeLimitLibrary, RemainingLifeLimit> = {
                library: this.selectedRemainingLifeLimitLibrary,
                isNewLibrary: false,
                syncModel: {
                libraryId: this.selectedRemainingLifeLimitLibrary.id === this.uuidNIL ? null : this.selectedRemainingLifeLimitLibrary.id,
                rowsForDeletion: this.deletionIds,
                updateRows: Array.from(this.updatedRowsMap.values()).map(r => r[1]),
                addedRows: this.addedRows,
                isModified: false
                },
                scenarioId: null
        }
        RemainingLifeLimitService.upsertRemainingLifeLimitLibrary(upsertRequest).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                this.clearChanges()
                this.addedOrUpdatedRemainingLifeLimitLibraryMutator(this.selectedRemainingLifeLimitLibrary);
                this.selectedRemainingLifeLimitLibraryMutator(this.selectedRemainingLifeLimitLibrary.id)
                this.addSuccessNotificationAction({message: "Updated remaining life limit library",});               
            }
        });
    }

    onUpsertScenarioRemainingLifeLimits() {
        if (this.selectedRemainingLifeLimitLibrary.id === this.uuidNIL || this.hasUnsavedChanges && this.newLibrarySelection ===false) {this.scenarioLibraryIsModified = true;}
        else { this.scenarioLibraryIsModified = false; }

        RemainingLifeLimitService.upsertScenarioRemainingLifeLimits({
            libraryId: this.selectedRemainingLifeLimitLibrary.id === this.uuidNIL ? null : this.selectedRemainingLifeLimitLibrary.id,
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
                this.addSuccessNotificationAction({message: "Modified scenario's remaining life limits"});
            }           
        });
    } 

    onDiscardChanges() {
        this.librarySelectItemValue = null;
        setTimeout(() => {
            if (this.hasScenario) {
                this.clearChanges();
                this.resetPage();
            }
        });
        this.parentLibraryName = this.loadedParentName;
        this.parentLibraryId = this.loadedParentId;
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
            this.deleteRemainingLifeLimitLibraryAction({
                libraryId: this.selectedRemainingLifeLimitLibrary.id,
            });
        }
    }

    disableCrudButton() {
        const rows = this.addedRows.concat(Array.from(this.updatedRowsMap.values()).map(r => r[1]));
        const dataIsValid: boolean = rows.every(
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

    //paging

    onUpdateRow(rowId: string, updatedRow: RemainingLifeLimit){
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
            (this.hasSelectedLibrary && hasUnsavedChangesCore('', this.stateSelectedRemainingLifeLimitLibrary, this.selectedRemainingLifeLimitLibrary))
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

    onShowShareRemainingLifeLimitLibraryDialog(remainingLifeLimitLibrary: RemainingLifeLimitLibrary) {
        this.shareRemainingLifeLimitLibraryDialogData = {
            showDialog:true,
            remainingLifeLimitLibrary: clone(remainingLifeLimitLibrary)
        }
    }

    onShareRemainingLifeLimitDialogSubmit(remainingLifeLimitLibraryUsers: RemainingLifeLimitLibraryUser[]) {
        this.shareRemainingLifeLimitLibraryDialogData = clone(emptyShareRemainingLifeLimitLibraryDialogData);

                if (!isNil(remainingLifeLimitLibraryUsers) && this.selectedRemainingLifeLimitLibrary.id !== getBlankGuid())
                {
                    let libraryUserData: LibraryUser[] = [];

                    //create library users
                    remainingLifeLimitLibraryUsers.forEach((remainingLifeLimitLibraryUser, index) =>
                    {   
                        //determine access level
                        let libraryUserAccessLevel: number = 0;
                        if (libraryUserAccessLevel == 0 && remainingLifeLimitLibraryUser.isOwner == true) { libraryUserAccessLevel = 2; }
                        if (libraryUserAccessLevel == 0 && remainingLifeLimitLibraryUser.canModify == true) { libraryUserAccessLevel = 1; }

                        //create library user object
                        let libraryUser: LibraryUser = {
                            userId: remainingLifeLimitLibraryUser.userId,
                            userName: remainingLifeLimitLibraryUser.username,
                            accessLevel: libraryUserAccessLevel
                        }

                        //add library user to an array
                        libraryUserData.push(libraryUser);
                    });
                    if (!isNullOrUndefined(this.selectedRemainingLifeLimitLibrary.id) ) {
                        this.getIsSharedLibraryAction(this.selectedRemainingLifeLimitLibrary).then(this.isShared = this.isSharedLibrary);
                    }
                    //update budget library sharing
                    RemainingLifeLimitService.upsertOrDeleteRemainingLifeLimitLibraryUsers(this.selectedRemainingLifeLimitLibrary.id, libraryUserData).then((response: AxiosResponse) => {
                        if (hasValue(response, 'status') && http2XX.test(response.status.toString()))
                        {
                            this.resetPage();
                        }
                    });
                }
    }

    setParentLibraryName(libraryId: string) {
        if (libraryId === "") {
            this.parentLibraryName = "None";
            return;
        }
        let foundLibrary: RemainingLifeLimitLibrary = emptyRemainingLifeLimitLibrary;
        this.stateRemainingLifeLimitLibraries.forEach(library => {
            if (library.id === libraryId ) {
                foundLibrary = clone(library);
            }
        });
        this.parentLibraryId = foundLibrary.id;
        this.parentLibraryName = foundLibrary.name;
    }

    initializePages(){
        const request: PagingRequest<RemainingLifeLimit>= {
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
            RemainingLifeLimitService.getScenarioRemainingLifeLimitPage(this.selectedScenarioId, request).then(response => {
                this.initializing = false
                if(response.data){
                    let data = response.data as PagingPage<RemainingLifeLimit>;
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
}
</script>
<style scoped>
.vs-style {
    width: 100%;
}
</style>