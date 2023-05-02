<template>
    <v-layout class="treatment-details-tab-content">
        <v-flex xs12>
            <v-layout column>                
                <v-flex>
                    <v-subheader class="ghd-control-label ghd-md-gray">Treatment Description</v-subheader>
                    <v-textarea                        
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
                        <v-select
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
                        <v-select
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
                        <v-text-field 
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
                        <v-text-field
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
                                        <v-btn
                                            @click="
                                                onRemoveTreatmentCriterion
                                            "
                                            class="ghd-white-bg ghd-blue ghd-button-text"
                                            icon
                                        >
                                            <v-icon style="font-size:20px !important" class="ghd-blue">fas fa-eraser</v-icon>
                                        </v-btn>
                                        <v-btn
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
                </v-flex>      
            </v-layout>
        </v-flex>

        <GeneralCriterionEditorDialog
            :dialogData="treatmentCriterionEditorDialogData"
            @submit="onSubmitTreatmentCriterionEditorDialogResult"
        />
    </v-layout>
</template>

<script lang="ts">
import Vue from 'vue';
import { Component, Prop, Watch } from 'vue-property-decorator';
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
    assetTypeReverseMap
} from '@/shared/models/iAM/treatment';
import {
    CriterionLibrary,
    emptyCriterionLibrary,
} from '@/shared/models/iAM/criteria';
import { setItemPropertyValue } from '@/shared/utils/setter-utils';
import GeneralCriterionEditorDialog from '@/shared/modals/GeneralCriterionEditorDialog.vue';
import { emptyGeneralCriterionEditorDialogData, GeneralCriterionEditorDialogData } from '@/shared/models/modals/general-criterion-editor-dialog-data';

@Component({
    components: {
        GeneralCriterionEditorDialog,
    },
})
export default class TreatmentDetailsTab extends Vue {
    @Prop() selectedTreatmentDetails: TreatmentDetails;
    @Prop() rules: InputValidationRules;
    @Prop() callFromScenario: boolean;
    @Prop() callFromLibrary: boolean;

    treatmentCriterionEditorDialogData: GeneralCriterionEditorDialogData = clone(
        emptyGeneralCriterionEditorDialogData,
    );
    uuidNIL: string = getBlankGuid();

    treatmentCategoryMap: Map<string, TreatmentCategory> = clone(treatmentCategoryMap);
    treatmentCategoryReverseMap: Map<TreatmentCategory, string> = clone(treatmentCategoryReverseMap);
    assetTypeReverseMap: Map<AssetType, string> = clone(assetTypeReverseMap);
    treatmentCategoryBinding: string = '';
    assetTypeMap: Map<string, AssetType> = clone(assetTypeMap);
    assetTypeBinding: string = '';

    @Watch('selectedTreatmentDetails')
    onSelectedTreatmentDetailsChanged(){
        this.treatmentCategoryBinding = treatmentCategoryReverseMap.get(this.selectedTreatmentDetails.category)!;
        this.assetTypeBinding = this.assetTypeReverseMap.get(this.selectedTreatmentDetails.assetType)!;
    }

    onShowTreatmentCriterionEditorDialog() {
        this.treatmentCriterionEditorDialogData = {
            showDialog: true,
            CriteriaExpression: this.selectedTreatmentDetails.criterionLibrary.mergedCriteriaExpression
        };
    }

    onSubmitTreatmentCriterionEditorDialogResult(
        criterionExpression: string,
    ) {
        this.treatmentCriterionEditorDialogData = clone(
            emptyGeneralCriterionEditorDialogData,
        );

        if (!isNil(criterionExpression)) {
            if(this.selectedTreatmentDetails.criterionLibrary.id === getBlankGuid())
                this.selectedTreatmentDetails.criterionLibrary.id = getNewGuid();
            this.$emit(
                'onModifyTreatmentDetails',
                setItemPropertyValue(
                    'criterionLibrary',
                    {...this.selectedTreatmentDetails.criterionLibrary, mergedCriteriaExpression: criterionExpression} as CriterionLibrary,
                    this.selectedTreatmentDetails,
                ),
            );
        }
    }

    onEditTreatmentType(property: string, key: any){
        var category = this.treatmentCategoryMap.get(key);
        this.onEditTreatmentDetails(property, category);
    }
    onEditAssetType(property: string, key: any){
        var asset = this.assetTypeMap.get(key);
        this.onEditTreatmentDetails(property, asset);
    }

    onEditTreatmentDetails(property: string, value: any) {
        this.$emit(
            'onModifyTreatmentDetails',
            setItemPropertyValue(
                property,
                value,
                this.selectedTreatmentDetails,
            ),
        );
    }

    onRemoveTreatmentCriterion() {
        this.$emit(
            'onModifyTreatmentDetails',
            setItemPropertyValue(
                'criterionLibrary',
                clone(emptyCriterionLibrary),
                this.selectedTreatmentDetails,
            ),
        );
    }
}
</script>

<style>
.criteria-flex {
    padding-bottom: 0px !important;
}
</style>
