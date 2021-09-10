<template>
    <v-layout class="treatment-details-tab-content">
        <v-flex xs12>
            <v-layout column justify-center>
                <v-flex xs10 class="criteria-flex">
                    <v-menu
                        full-width
                        bottom
                        min-height="500px"
                        min-width="800px"
                    >
                        <template slot="activator">
                            <v-text-field
                                readonly
                                full-width
                                class="sm-txt"
                                label="Treatment Criteria"
                                :value="
                                    selectedTreatmentDetails.criterionLibrary
                                        .mergedCriteriaExpression
                                "
                            >
                                <template slot="append-outer">
                                    <v-layout align-center fill-height row>
                                        <v-btn
                                            @click="
                                                onShowTreatmentCriterionLibraryEditorDialog
                                            "
                                            class="edit-icon"
                                            icon
                                        >
                                            <v-icon>fas fa-edit</v-icon>
                                        </v-btn>
                                        <v-btn
                                            @click="
                                                onRemoveTreatmentCriterionLibrary
                                            "
                                            class="ara-orange"
                                            icon
                                        >
                                            <v-icon>fas fa-minus-square</v-icon>
                                        </v-btn>
                                    </v-layout>
                                </template>
                            </v-text-field>
                        </template>
                        <v-card>
                            <v-card-text>
                                <v-textarea
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

                <div class="shadow-inputs-div">
                    <v-spacer></v-spacer>
                    <v-layout justify-space-between row>
                        <v-flex xs3>
                            <v-select
                                :items="Array.from(treatmentCategoryMap.keys())"
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
                            <v-select
                            :items="Array.from(assetTypeMap.keys())"
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
                        <v-flex xs2>
                            <v-text-field
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
                        <v-flex xs2>
                            <v-text-field
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
                    </v-layout>
                    <v-spacer></v-spacer>
                </div>

                <v-flex xs6 class="treatment-description-flex">
                    <v-textarea
                        label="Treatment Description"
                        no-resize
                        outline
                        rows="2"
                        v-model="selectedTreatmentDetails.description"
                        @input="
                            onEditTreatmentDetails(
                                'description',
                                selectedTreatmentDetails.description,
                            )
                        "
                    />
                </v-flex>
            </v-layout>
        </v-flex>

        <TreatmentCriterionLibraryEditorDialog
            :dialogData="treatmentCriterionLibraryEditorDialogData"
            @submit="onSubmitTreatmentCriterionLibraryEditorDialogResult"
        />
    </v-layout>
</template>

<script lang="ts">
import Vue from 'vue';
import { Component, Prop, Watch } from 'vue-property-decorator';
import CriterionLibraryEditorDialog from '../../../shared/modals/CriterionLibraryEditorDialog.vue';
import {
    CriterionLibraryEditorDialogData,
    emptyCriterionLibraryEditorDialogData,
} from '@/shared/models/modals/criterion-library-editor-dialog-data';
import { clone, isNil } from 'ramda';
import { InputValidationRules } from '@/shared/utils/input-validation-rules';
import { getBlankGuid } from '@/shared/utils/uuid-utils';
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

@Component({
    components: {
        TreatmentCriterionLibraryEditorDialog: CriterionLibraryEditorDialog,
    },
})
export default class TreatmentDetailsTab extends Vue {
    @Prop() selectedTreatmentDetails: TreatmentDetails;
    @Prop() rules: InputValidationRules;
    @Prop() callFromScenario: boolean;
    @Prop() callFromLibrary: boolean;

    treatmentCriterionLibraryEditorDialogData: CriterionLibraryEditorDialogData = clone(
        emptyCriterionLibraryEditorDialogData,
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

    onShowTreatmentCriterionLibraryEditorDialog() {
        this.treatmentCriterionLibraryEditorDialogData = {
            showDialog: true,
            libraryId: this.selectedTreatmentDetails.criterionLibrary.id,
            isCallFromScenario: this.callFromScenario,
            isCriterionForLibrary: this.callFromLibrary,
        };
    }

    onSubmitTreatmentCriterionLibraryEditorDialogResult(
        criterionLibrary: CriterionLibrary,
    ) {
        this.treatmentCriterionLibraryEditorDialogData = clone(
            emptyCriterionLibraryEditorDialogData,
        );

        if (!isNil(criterionLibrary)) {
            this.$emit(
                'onModifyTreatmentDetails',
                setItemPropertyValue(
                    'criterionLibrary',
                    criterionLibrary,
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

    onRemoveTreatmentCriterionLibrary() {
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
.treatment-details-content {
    height: 185px;
}

.criteria-flex {
    padding-bottom: 0px !important;
}

.shadow-inputs-div {
    height: 78px;
}

.treatment-description-flex {
    padding-bottom: 0px !important;
}
</style>
