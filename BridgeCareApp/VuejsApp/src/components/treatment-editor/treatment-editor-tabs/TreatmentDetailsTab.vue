<template>
    <v-row class="treatment-details-tab-content">
        <v-col cols="12">
            <v-textarea  
                id ="TreatmentDetailsTab-desc-vtextarea"                      
                class='ghd-control-border ghd-control-text'
                no-resize
                label="Treatment Description"
                variant="outlined"
                rows="3"
                item-title="text"
                item-value="value" 
                v-model="selectedTreatmentDetails.description"
                @update:model-value="onEditTreatmentDetails('description', selectedTreatmentDetails.description)"
            />
        </v-col>                
        <v-row class="ghd-left-padding ghd-right-padding" style="margin: 10px;">
            <v-col cols="3">
                <v-select id="TreatmentDetailsTab-category-vselect"
                class='ghd-select ghd-control-text ghd-text-field ghd-text-field-border'
                    :items="Array.from(treatmentCategoryMap.keys())"
                    menu-icon=custom:GhdDownSvg  
                    label="Category"
                    variant="outlined"
                    density="compact"
                    v-model="treatmentCategoryBinding"
                    item-title="text"
                    item-value="value" 
                    :rules="[rules['generalRules'].valueIsNotEmpty]"
                />
            </v-col>
            <v-col >
                <v-select id="TreatmentDetailsTab-assetType-vselect"
                class='ghd-select ghd-control-text ghd-text-field ghd-text-field-border'
                :items="stateAssetType"
                append-icon=ghd-down
                label="Asset type"
                variant="outlined"
                density="compact"
                v-model="assetTypeBinding"
                :rules="[rules['generalRules'].valueIsNotEmpty]"
                />
            </v-col>
            <v-col >
                <v-text-field id="TreatmentDetailsTab-yearsBeforeAny-vtext"
                    class='ghd-control-border ghd-control-text ghd-control-width-sm'
                    v-maska:[mask]
                    @update:model-value="onEditTreatmentDetails('shadowForAnyTreatment', $event)"
                    label="Years Before Any"
                    variant="outlined"
                    density="compact"
                    v-model="
                        selectedTreatmentDetails.shadowForAnyTreatment
                    "
                    :rules="[rules['generalRules'].valueIsNotEmpty]"
                />
            </v-col>
            <v-col >
                <v-text-field id="TreatmentDetailsTab-yearsBeforeSame-vtext"
                    class='ghd-control-border ghd-control-text ghd-control-width-sm'
                    v-maska:[mask]
                    rows="4"
                    @update:model-value="onEditTreatmentDetails('shadowForSameTreatment', $event)"
                    label="Years Before Same"
                    variant="outlined"
                    density="compact"
                    v-model="
                        selectedTreatmentDetails.shadowForSameTreatment
                    "
                    :rules="[rules['generalRules'].valueIsNotEmpty]"
                />
            </v-col>
        </v-row>                
        <v-col cols="12">
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
                            <v-col>                                 
                                <v-btn id="TreatmentDetailsTab-RemoveCriteria-vbtn"
                                    @click="
                                        onRemoveTreatmentCriterion
                                    "
                                    class="ghd-white-bg ghd-blue ghd-button-text"
                                    flat
                                >
                                    <v-icon style="font-size:20px !important" class="ghd-blue">fas fa-eraser</v-icon>
                                </v-btn>
                                <v-btn id="TreatmentDetailsTab-EditCriteria-vbtn"
                                    @click="
                                        onShowTreatmentCriterionEditorDialog
                                    "
                                    class="edit-icon"
                                    flat
                                    style="left:25px"
                                >
                                    <img class='img-general' :src="getUrl('assets/icons/edit.svg')"/>
                                </v-btn>   
                            </v-col>                                    
                        </v-row>       
                    </v-row>   
                    <v-row  class="ghd-right-padding">  
                            <v-col cols ="26">
                                <v-textarea
                                    class="ghd-control-border sm-txt"                                            
                                    no-resize
                                    variant="outlined"
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
                            variant="outlined"
                            readonly
                            rows="5"
                        />
                    </v-card-text>
                </v-card>
            </v-menu>
            <v-switch
                color="#2A578D"
                v-model="TreatmentIsUnSelectable"
                label="Mark treatment as unselectable by scenario engine"
                style="margin-left: 0px; margin-top: 10px;"
            ></v-switch>    
        </v-col>              
    </v-row>
    <GeneralCriterionEditorDialog
        :dialogData="treatmentCriterionEditorDialogData"
        @submit="onSubmitTreatmentCriterionEditorDialogResult"
    />
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { ref, onMounted, onBeforeUnmount, watch, toRefs } from 'vue';
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
import { getUrl } from '@/shared/utils/get-url';

    let store = useStore();
    let stateAssetType = computed<string[]>(()=>store.state.adminDataModule.assetType);
    async function getAssetTypeAction(payload?: any): Promise<any> {await store.dispatch('getAssetType',payload);}

    const emit = defineEmits(['submit', 'onModifyTreatmentDetails'])
    const props = defineProps<{
        selectedTreatmentDetails: TreatmentDetails,
        rules: InputValidationRules,
        callFromScenario: boolean,
        callFromLibrary: boolean
    }>();
    const { rules, callFromScenario, callFromLibrary, selectedTreatmentDetails } = toRefs(props);
    const TreatmentIsUnSelectable = ref(false);
    const treatmentCriterionEditorDialogData = ref(clone(emptyGeneralCriterionEditorDialogData));
    let uuidNIL: string = getBlankGuid();
    let treatmentCategoryMapValue: Map<string, TreatmentCategory> = clone(treatmentCategoryMap);
    let treatmentCategoryReverseMapValue: Map<TreatmentCategory, string> = clone(treatmentCategoryReverseMap);
    let assetTypeReverseMapValue: Map<AssetType, string> = clone(assetTypeReverseMap);
    let treatmentCategoryBinding = ref('');
    let categories = Array.from(treatmentCategoryMap.keys());
    let assetTypeMapValue: Map<string, AssetType> = clone(assetTypeMap);
    let assetTypeBinding = ref('');

    const mask = { mask: '##########' };

    created();
    function created() {
        getAssetTypeAction();
    }

    watch(assetTypeBinding, () => {
        onEditAssetType('assetType', assetTypeBinding.value)
    })

    watch(treatmentCategoryBinding, () => {
        onEditTreatmentType('category', treatmentCategoryBinding.value)
    })
    watch(selectedTreatmentDetails, () => {
        treatmentCategoryBinding.value = treatmentCategoryReverseMap.get(selectedTreatmentDetails.value.category)!;
        assetTypeBinding.value = assetTypeReverseMap.get(selectedTreatmentDetails.value.assetType)!;
        TreatmentIsUnSelectable.value = selectedTreatmentDetails.value.isUnselectable;
    });

    watch(TreatmentIsUnSelectable, (newValue, oldValue) => onToggleIsUnSelectable(newValue))
    function onToggleIsUnSelectable(value: boolean) {
        emit(
        'onModifyTreatmentDetails',
         setItemPropertyValue(
         'isUnselectable',
         value,
        selectedTreatmentDetails.value,
        ),
        );
    }

    function onShowTreatmentCriterionEditorDialog() {
        treatmentCriterionEditorDialogData.value = {
            showDialog: true,
            CriteriaExpression: selectedTreatmentDetails.value.criterionLibrary.mergedCriteriaExpression
        };
    }

    function onSubmitTreatmentCriterionEditorDialogResult(
        criterionExpression: string,
    ) {
        treatmentCriterionEditorDialogData.value = clone(
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
                     selectedTreatmentDetails.value,
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
                selectedTreatmentDetails.value,
            ),
        );
    }

    function onRemoveTreatmentCriterion() {
        emit(
            'onModifyTreatmentDetails',
            setItemPropertyValue(
                'criterionLibrary',
                clone(emptyCriterionLibrary),
                selectedTreatmentDetails.value,
            ),
        );
    }

</script>

<style>
.criteria-flex {
    padding-bottom: 0px !important;
}
</style>
