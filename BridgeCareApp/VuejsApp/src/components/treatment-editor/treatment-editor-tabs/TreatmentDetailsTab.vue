<template>
    <v-layout class="treatment-details-tab-content">
        <v-flex xs12>
            <v-layout column>                
                <v-flex>
                    <v-subheader class="ghd-control-label ghd-md-gray">Treatment Description</v-subheader>
                    <v-textarea  
                        id ="TreatmentDetailsTab-desc-vtextarea"                      
                        class='ghd-control-border ghd-control-text'
                        no-resize
                        outline
                        rows="3"
                        v-model="selectedTreatmentDetails.description"
                        @input="
                            onEditTreatmentDetails(
                                'description',
                                selectedTreatmentDetails.description,
                            )
                        "
                    />
                </v-flex>                
                <v-layout xs12 row class="ghd-left-padding ghd-right-padding">
                    <v-flex xs3>
                        <v-subheader class="ghd-control-label ghd-md-gray">Category</v-subheader>
                        <v-select id="TreatmentDetailsTab-category-vselect"
                        class='ghd-select ghd-control-text ghd-text-field ghd-text-field-border'
                            :items="Array.from(treatmentCategoryMap.keys())"
                            append-icon=$vuetify.icons.ghd-down
                            @input="
                                onEditTreatmentType(
                                    'category',
                                    treatmentCategoryBinding,
                                )
                            "
                            label="Category"
                            outline
                            v-model="treatmentCategoryBinding"
                            :rules="[rules['generalRules'].valueIsNotEmpty]"
                        />
                    </v-flex>
                    <v-flex xs3>
                        <v-subheader class="ghd-control-label ghd-md-gray">Asset type</v-subheader>
                        <v-select id="TreatmentDetailsTab-assetType-vselect"
                        class='ghd-select ghd-control-text ghd-text-field ghd-text-field-border'
                        :items="Array.from(assetTypeMap.keys())"
                        append-icon=$vuetify.icons.ghd-down
                            @input="
                                onEditAssetType(
                                    'assetType',
                                    assetTypeBinding,
                                )
                            "
                            label="Asset type"
                            outline
                            v-model="assetTypeBinding"
                            :rules="[rules['generalRules'].valueIsNotEmpty]"
                        />
                    </v-flex>
                    <v-flex xs3>
                        <v-subheader class="ghd-control-label ghd-md-gray">Years Before Any</v-subheader>
                        <v-text-field id="TreatmentDetailsTab-yearsBeforeAny-vtext"
                            class='ghd-control-border ghd-control-text ghd-control-width-sm'
                            :mask="'####'"
                            @input="
                                onEditTreatmentDetails(
                                    'shadowForAnyTreatment',
                                    selectedTreatmentDetails.shadowForAnyTreatment,
                                )
                            "
                            label="Years Before Any"
                            outline
                            v-model="
                                selectedTreatmentDetails.shadowForAnyTreatment
                            "
                            :rules="[rules['generalRules'].valueIsNotEmpty]"
                        />
                    </v-flex>
                    <v-flex xs3>
                        <v-subheader class="ghd-control-label ghd-md-gray">Years Before Same</v-subheader>
                        <v-text-field id="TreatmentDetailsTab-yearsBeforeSame-vtext"
                            class='ghd-control-border ghd-control-text ghd-control-width-sm'
                            :mask="'####'"
                            rows="4"
                            @input="
                                onEditTreatmentDetails(
                                    'shadowForSameTreatment',
                                    selectedTreatmentDetails.shadowForSameTreatment,
                                )
                            "
                            label="Years Before Same"
                            outline
                            v-model="
                                selectedTreatmentDetails.shadowForSameTreatment
                            "
                            :rules="[rules['generalRules'].valueIsNotEmpty]"
                        />
                    </v-flex>
<!--                     <v-flex xs3>
                        <v-subheader class="ghd-control-label ghd-md-gray">Performance Factor</v-subheader>
                        <v-text-field
                            class='ghd-control-border ghd-control-text ghd-control-width-sm'
                            @input="
                                onEditTreatmentDetails(
                                    'performanceFactor',
                                    selectedTreatmentDetails.performanceFactor,
                                )
                            "
                            label="Performance Factor"
                            outline
                            :value='parseFloat(selectedTreatmentDetails.performanceFactor).toFixed(2)'
                            v-model.number="
                                selectedTreatmentDetails.performanceFactor
                            "
                            :rules="[rules['generalRules'].valueIsNotEmpty]"
                        />
                    </v-flex>
 -->                </v-layout>                
                <v-flex class="criteria-flex">
                    <v-menu
                        full-width
                        bottom
                        min-height="500px"
                        min-width="1000px"
                    >   
                        <template slot="activator">                                                                                       
                            <v-layout column class="ghd-left-padding">  
                                <v-layout xs12 align-center style="height:50px;">                                    
                                    <v-flex xs11>
                                        <v-subheader class="ghd-control-label ghd-md-gray">Treatment Criteria</v-subheader>    
                                    </v-flex>
                                    <v-flex xs2>                                 
                                        <v-btn id="TreatmentDetailsTab-RemoveCriteria-vbtn"
                                            @click="
                                                onRemoveTreatmentCriterion
                                            "
                                            class="ghd-white-bg ghd-blue ghd-button-text"
                                            icon
                                        >
                                            <v-icon style="font-size:20px !important" class="ghd-blue">fas fa-eraser</v-icon>
                                        </v-btn>
                                        <v-btn id="TreatmentDetailsTab-EditCriteria-vbtn"
                                            @click="
                                                onShowTreatmentCriterionEditorDialog
                                            "
                                            class="edit-icon"
                                            icon
                                            style="left:25px"
                                        >
                                            <img class='img-general' :src="require('@/assets/icons/edit.svg')"/>
                                        </v-btn>   
                                    </v-flex>                                    
                                </v-layout>       
                                <v-layout align-center class="ghd-right-padding">  
                                    <v-flex>
                                        <v-textarea
                                            class="ghd-control-border sm-txt"                                            
                                            no-resize
                                            outline
                                            rows="3"
                                            readonly                                            
                                            :value="
                                                selectedTreatmentDetails.criterionLibrary
                                                    .mergedCriteriaExpression
                                            "
                                        >
                                        </v-textarea>         
                                    </v-flex>      
                                </v-layout>                            
                            </v-layout>        
                        </template>                
                        <v-card>
                            <v-card-text>
                                <v-textarea
                                    class="ghd-card-width"
                                    :value="
                                        selectedTreatmentDetails
                                            .criterionLibrary
                                            .mergedCriteriaExpression
                                    "
                                    full-width
                                    no-resize
                                    outline
                                    readonly
                                    rows="5"
                                />
                            </v-card-text>
                        </v-card>
                    </v-menu>
                    <v-switch
                        v-model="TreatmentIsUnSelectable"
                        label="Mark treatment as unselectable by scenario engine"
                        style="margin-left: 10px; margin-top: 30px;"
                    ></v-switch>    
                </v-flex>              
            </v-layout>
        </v-flex>   
        <GeneralCriterionEditorDialog
            :dialogData="treatmentCriterionEditorDialogData"
            @submit="onSubmitTreatmentCriterionEditorDialogResult"
        />
    </v-layout>
</template>

<script lang="ts" setup>
import Vue from 'vue';
import { inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import { clone, isNil } from 'ramda';
import { InputValidationRules } from '@/shared/utils/input-validation-rules';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import {
  AssetType,
    TreatmentCategory,
    TreatmentDetails,
    treatmentCategoryMap,
    assetTypeMap,
    treatmentCategoryReverseMap,
    assetTypeReverseMap,
} from '@/shared/models/iAM/treatment';
import {
    CriterionLibrary,
    emptyCriterionLibrary,
} from '@/shared/models/iAM/criteria';
import { setItemPropertyValue } from '@/shared/utils/setter-utils';
import GeneralCriterionEditorDialog from '@/shared/modals/GeneralCriterionEditorDialog.vue';
import { emptyGeneralCriterionEditorDialogData, GeneralCriterionEditorDialogData } from '@/shared/models/modals/general-criterion-editor-dialog-data';

    const emit = defineEmits(['submit', 'onModifyTreatmentDetails'])
    let store = useStore();
    let selectedTreatmentDetails = ref<TreatmentDetails>(store.state.TreatmentDetailsTab.selectedTreatmentDetails);
    let  rules: InputValidationRules;
    let  callFromScenario: boolean;
    let  callFromLibrary: boolean;
    let TreatmentIsUnSelectable: boolean = false;



    let treatmentCriterionEditorDialogData: GeneralCriterionEditorDialogData = clone(
        emptyGeneralCriterionEditorDialogData,
    );
    let uuidNIL: string = getBlankGuid();

    let treatmentCategoryMapValue: Map<string, TreatmentCategory> = clone(treatmentCategoryMap);
    let treatmentCategoryReverseMapValue: Map<TreatmentCategory, string> = clone(treatmentCategoryReverseMap);
    let assetTypeReverseMapValue: Map<AssetType, string> = clone(assetTypeReverseMap);
    let treatmentCategoryBinding: string = '';
    let assetTypeMapValue: Map<string, AssetType> = clone(assetTypeMap);
    let assetTypeBinding: string = '';

    watch(selectedTreatmentDetails, () => onSelectedTreatmentDetailsChanged)
    function onSelectedTreatmentDetailsChanged(){
        treatmentCategoryBinding = treatmentCategoryReverseMap.get(selectedTreatmentDetails.value.category)!;
        assetTypeBinding = assetTypeReverseMap.get(selectedTreatmentDetails.value.assetType)!;
        TreatmentIsUnSelectable = selectedTreatmentDetails.value.isUnselectable;
    }

    function onShowTreatmentCriterionEditorDialog() {
        treatmentCriterionEditorDialogData = {
            showDialog: true,
            CriteriaExpression: selectedTreatmentDetails.value.criterionLibrary.mergedCriteriaExpression
        };
    }

    function onSubmitTreatmentCriterionEditorDialogResult(
        criterionExpression: string,
    ) {
        treatmentCriterionEditorDialogData = clone(
            emptyGeneralCriterionEditorDialogData,
        );

        if (!isNil(criterionExpression)) {
            if(selectedTreatmentDetails.value.criterionLibrary.id === getBlankGuid())
                selectedTreatmentDetails.value.criterionLibrary.id = getNewGuid();
            emit(
                'onModifyTreatmentDetails',
                setItemPropertyValue(
                    'criterionLibrary',
                    {...selectedTreatmentDetails.value.criterionLibrary, mergedCriteriaExpression: criterionExpression} as CriterionLibrary,
                     selectedTreatmentDetails,
                ),
            );
        }
    }

    function onEditTreatmentType(property: string, key: any){
        var category = treatmentCategoryMap.get(key);
        onEditTreatmentDetails(property, category);
    }
    function onEditAssetType(property: string, key: any){
        var asset = assetTypeMap.get(key);
        onEditTreatmentDetails(property, asset);
    }

    function onEditTreatmentDetails(property: string, value: any) {
        emit(
            'onModifyTreatmentDetails',
            setItemPropertyValue(
                property,
                value,
                selectedTreatmentDetails,
            ),
        );
    }

    function onRemoveTreatmentCriterion() {
        emit(
            'onModifyTreatmentDetails',
            setItemPropertyValue(
                'criterionLibrary',
                clone(emptyCriterionLibrary),
                selectedTreatmentDetails,
            ),
        );
    }

</script>

<style>
.criteria-flex {
    padding-bottom: 0px !important;
}
</style>
