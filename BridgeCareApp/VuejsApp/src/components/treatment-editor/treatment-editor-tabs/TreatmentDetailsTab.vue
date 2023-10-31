<template>
    <v-row class="treatment-details-tab-content">
        <v-col cols = "12">
            <v-row column>                
                <v-col>
                    <v-subheader class="ghd-control-label ghd-md-gray">Treatment Description</v-subheader>
                    <v-textarea  
                        id ="TreatmentDetailsTab-desc-vtextarea"                      
                        class='ghd-control-border ghd-control-text'
                        no-resize
                        outline
                        rows="3"
                        item-title="text"
                        item-value="value" 
                        v-model="selectedTreatmentDetails.description"
                        @update:model-value="onEditTreatmentDetails('description', selectedTreatmentDetails.description)"
                    />
                </v-col>                
                <v-row  rows = "12" row class="ghd-left-padding ghd-right-padding">
                    <v-col cols = "3">
                        <v-subheader class="ghd-control-label ghd-md-gray">Category</v-subheader>
                        <v-select id="TreatmentDetailsTab-category-vselect"
                        class='ghd-select ghd-control-text ghd-text-field ghd-text-field-border'
                            :items="Array.from(treatmentCategoryMap.keys())"
                            append-icon=ghd-down
                            @update:model-value="onEditTreatmentType('category', treatmentCategoryBinding)"
                            label="Category"
                            variant="outlined"
                            v-model="treatmentCategoryBinding"
                            item-title="text"
                            item-value="value" 
                            :rules="[rules['generalRules'].valueIsNotEmpty]"
                        />
                    </v-col>
                    <v-col cols = "3">
                        <v-subheader class="ghd-control-label ghd-md-gray">Asset type</v-subheader>
                        <v-select id="TreatmentDetailsTab-assetType-vselect"
                        class='ghd-select ghd-control-text ghd-text-field ghd-text-field-border'
                        :items="Array.from(assetTypeMap.keys())"
                        append-icon=ghd-down
                        @update:model-value="onEditAssetType('assetType', assetTypeBinding)"
                        label="Asset type"
                        variant="outlined"
                        v-model="assetTypeBinding"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"
                        />
                    </v-col>
                    <v-col cols = "3">
                        <v-subheader class="ghd-control-label ghd-md-gray">Years Before Any</v-subheader>
                        <v-text-field id="TreatmentDetailsTab-yearsBeforeAny-vtext"
                            class='ghd-control-border ghd-control-text ghd-control-width-sm'
                            :mask="'####'"
                            @update:model-value="onEditTreatmentDetails('shadowForAnyTreatment', selectedTreatmentDetails.shadowForAnyTreatment)"
                            label="Years Before Any"
                            outline
                            v-model="
                                selectedTreatmentDetails.shadowForAnyTreatment
                            "
                            :rules="[rules['generalRules'].valueIsNotEmpty]"
                        />
                    </v-col>
                    <v-col cols = "3">
                        <v-subheader class="ghd-control-label ghd-md-gray">Years Before Same</v-subheader>
                        <v-text-field id="TreatmentDetailsTab-yearsBeforeSame-vtext"
                            class='ghd-control-border ghd-control-text ghd-control-width-sm'
                            :mask="'####'"
                            rows="4"
                            @update:model-value="onEditTreatmentDetails('shadowForSameTreatment', selectedTreatmentDetails.shadowForSameTreatment)"
                            label="Years Before Same"
                            outline
                            v-model="
                                selectedTreatmentDetails.shadowForSameTreatment
                            "
                            :rules="[rules['generalRules'].valueIsNotEmpty]"
                        />
                    </v-col>
                </v-row>                
                <v-col cols="20">
                    <v-menu
                        full-width
                        location="bottom"
                        min-height="500px"
                        min-width="1000px"
                    >   
                        <template v-slot:activator>                                                                                       
                            <v-row justify="space-between" class="ghd-left-padding">  
                                <v-row  align-center style="height:50px;">                                    
                                    <v-col cols = "9">
                                        <v-subheader class="ghd-control-label ghd-md-gray">Treatment Criteria</v-subheader>    
                                    </v-col>
                                    <v-col cols = "2">                                 
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
                                    </v-col>                                    
                                </v-row>       
                                
                                                          
                            </v-row>   
                            
                            <v-row  class="ghd-right-padding">  
                                    <v-col cols ="26">
                                        <v-textarea
                                            class="ghd-control-border sm-txt"                                            
                                            no-resize
                                            outline
                                            rows="3"
                                            readonly                                            
                                            :model-value="
                                                selectedTreatmentDetails.criterionLibrary
                                                    .mergedCriteriaExpression
                                            "
                                        >
                                        </v-textarea>         
                                    </v-col>      
                                </v-row>
                                       
                        </template>                
                        <v-card>
                            <v-card-text>
                                <v-textarea
                                    class="ghd-card-width"
                                    :model-value="
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
                </v-col>              
            </v-row>
        </v-col>   
        <GeneralCriterionEditorDialog
            :dialogData="treatmentCriterionEditorDialogData"
            @submit="onSubmitTreatmentCriterionEditorDialogResult"
        />
    </v-row>
</template>

<script lang="ts" setup>
import Vue, { computed } from 'vue';
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
    const props = defineProps<{
        selectedTreatmentDetails: TreatmentDetails,
        rules: InputValidationRules,
        callFromScenario: boolean,
        callFromLibrary: boolean
    }>();
    let TreatmentIsUnSelectable = ref(false);



    let treatmentCriterionEditorDialogData = ref(clone(
        emptyGeneralCriterionEditorDialogData,
    ));
    let uuidNIL: string = getBlankGuid();

    let treatmentCategoryMapValue: Map<string, TreatmentCategory> = clone(treatmentCategoryMap);
    let treatmentCategoryReverseMapValue: Map<TreatmentCategory, string> = clone(treatmentCategoryReverseMap);
    let assetTypeReverseMapValue: Map<AssetType, string> = clone(assetTypeReverseMap);
    let treatmentCategoryBinding = ref('');
    let assetTypeMapValue: Map<string, AssetType> = clone(assetTypeMap);
    let assetTypeBinding = ref('');

    watch(() => props.selectedTreatmentDetails, () => onSelectedTreatmentDetailsChanged())
    function onSelectedTreatmentDetailsChanged(){
        treatmentCategoryBinding.value = treatmentCategoryReverseMap.get(props.selectedTreatmentDetails.category)!;
        assetTypeBinding.value = assetTypeReverseMap.get(props.selectedTreatmentDetails.assetType)!;
        TreatmentIsUnSelectable.value = props.selectedTreatmentDetails.isUnselectable;
    }

    watch(TreatmentIsUnSelectable, (newValue, oldValue) => onToggleIsUnSelectable(newValue))
    function onToggleIsUnSelectable(value: boolean) {
        console.log('onToggleIsUnSelectable called with value:', value);
                emit(
                'onModifyTreatmentDetails',
                 setItemPropertyValue(
                 'isUnselectable',
                 value,
                props.selectedTreatmentDetails,
    ),
  );
}

    function onShowTreatmentCriterionEditorDialog() {
        treatmentCriterionEditorDialogData.value = {
            showDialog: true,
            CriteriaExpression: props.selectedTreatmentDetails.criterionLibrary.mergedCriteriaExpression
        };
    }

    function onSubmitTreatmentCriterionEditorDialogResult(
        criterionExpression: string,
    ) {
        treatmentCriterionEditorDialogData.value = clone(
            emptyGeneralCriterionEditorDialogData,
        );

        if (!isNil(criterionExpression)) {
            if(props.selectedTreatmentDetails.criterionLibrary.id === getBlankGuid())
                props.selectedTreatmentDetails.criterionLibrary.id = getNewGuid();
            emit(
                'onModifyTreatmentDetails',
                setItemPropertyValue(
                    'criterionLibrary',
                    {...props.selectedTreatmentDetails.criterionLibrary, mergedCriteriaExpression: criterionExpression} as CriterionLibrary,
                     props.selectedTreatmentDetails,
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
                props.selectedTreatmentDetails,
            ),
        );
    }

    function onRemoveTreatmentCriterion() {
        emit(
            'onModifyTreatmentDetails',
            setItemPropertyValue(
                'criterionLibrary',
                clone(emptyCriterionLibrary),
                props.selectedTreatmentDetails,
            ),
        );
    }

</script>

<style>
.criteria-flex {
    padding-bottom: 0px !important;
}
</style>
