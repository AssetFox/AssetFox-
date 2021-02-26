<template>
  <v-layout class="treatment-details-tab-content">
    <v-flex xs12>
      <v-layout column justify-center>
        <v-flex xs10 class="criteria-flex">
          <!--          <v-text-field readonly full-width class="sm-txt" label="Treatment Criteria"
                                  :value="selectedTreatmentDetails.criterionLibrary.mergedCriteriaExpression">
                      <template slot="append-outer">
                        <v-layout align-center fill-height row>
                          <v-btn @click="onShowTreatmentCriterionLibraryEditorDialog" class="edit-icon" icon>
                            <v-icon>fas fa-edit</v-icon>
                          </v-btn>
                          <v-btn @click="onRemoveTreatmentCriterionLibrary" class="ara-orange" icon>
                            <v-icon>fas fa-minus-square</v-icon>
                          </v-btn>
                        </v-layout>
                      </template>
                    </v-text-field>-->

          <v-menu full-width bottom min-height="500px" min-width="800px">
            <template slot="activator">
              <v-text-field readonly full-width class="sm-txt" label="Treatment Criteria"
                            :value="selectedTreatmentDetails.criterionLibrary.mergedCriteriaExpression">
                <template slot="append-outer">
                  <v-layout align-center fill-height row>
                    <v-btn @click="onShowTreatmentCriterionLibraryEditorDialog" class="edit-icon" icon>
                      <v-icon>fas fa-edit</v-icon>
                    </v-btn>
                    <v-btn @click="onRemoveTreatmentCriterionLibrary" class="ara-orange" icon>
                      <v-icon>fas fa-minus-square</v-icon>
                    </v-btn>
                  </v-layout>
                </template>
              </v-text-field>
            </template>
            <v-card>
              <v-card-text>
                <v-textarea :value="selectedTreatmentDetails.criterionLibrary.mergedCriteriaExpression" full-width
                            no-resize
                            outline
                            readonly
                            rows="5"/>
              </v-card-text>
            </v-card>
          </v-menu>
        </v-flex>

        <div class="shadow-inputs-div">
          <v-spacer></v-spacer>
          <v-layout justify-space-between row>
            <v-flex xs5>
              <v-text-field :mask="'####'"
                            @change="onEditTreatmentDetails('shadowForAnyTreatment', selectedTreatmentDetails.shadowForAnyTreatment)"
                            label="Years Before Any"
                            outline v-model="selectedTreatmentDetails.shadowForAnyTreatment"
                            :rules="[rules['generalRules'].valueIsNotEmpty]"/>
            </v-flex>
            <v-flex xs5>
              <v-text-field :mask="'####'" rows="4"
                            @change="onEditTreatmentDetails('shadowForSameTreatment', selectedTreatmentDetails.shadowForSameTreatment)"
                            label="Years Before Same"
                            outline v-model="selectedTreatmentDetails.shadowForSameTreatment"
                            :rules="[rules['generalRules'].valueIsNotEmpty]"/>
            </v-flex>
          </v-layout>
          <v-spacer></v-spacer>
        </div>

        <v-flex xs6 class="treatment-description-flex">
          <v-textarea label="Treatment Description" no-resize outline rows="2"
                      v-model="selectedTreatmentDetails.description"/>
        </v-flex>
      </v-layout>
    </v-flex>

    <TreatmentCriterionLibraryEditorDialog :dialogData="treatmentCriterionLibraryEditorDialogData"
                                           @submit="onSubmitTreatmentCriterionLibraryEditorDialogResult"/>
  </v-layout>
</template>

<script lang="ts">
import Vue from 'vue';
import {Component, Prop} from 'vue-property-decorator';
import CriterionLibraryEditorDialog from '../../../shared/modals/CriterionLibraryEditorDialog.vue';
import {
  CriterionLibraryEditorDialogData,
  emptyCriterionLibraryEditorDialogData
} from '@/shared/models/modals/criterion-library-editor-dialog-data';
import {clone, isNil} from 'ramda';
import {InputValidationRules} from '@/shared/utils/input-validation-rules';
import {getBlankGuid} from '@/shared/utils/uuid-utils';
import {TreatmentDetails} from '@/shared/models/iAM/treatment';
import {CriterionLibrary, emptyCriterionLibrary} from '@/shared/models/iAM/criteria';
import {setItemPropertyValue} from '@/shared/utils/setter-utils';

@Component({
  components: {TreatmentCriterionLibraryEditorDialog: CriterionLibraryEditorDialog}
})
export default class TreatmentDetailsTab extends Vue {
  @Prop() selectedTreatmentDetails: TreatmentDetails;
  @Prop() rules: InputValidationRules;

  treatmentCriterionLibraryEditorDialogData: CriterionLibraryEditorDialogData = clone(emptyCriterionLibraryEditorDialogData);
  uuidNIL: string = getBlankGuid();

  onShowTreatmentCriterionLibraryEditorDialog() {
    this.treatmentCriterionLibraryEditorDialogData = {
      showDialog: true,
      libraryId: this.selectedTreatmentDetails.criterionLibrary.id
    };
  }

  onSubmitTreatmentCriterionLibraryEditorDialogResult(criterionLibrary: CriterionLibrary) {
    this.treatmentCriterionLibraryEditorDialogData = clone(emptyCriterionLibraryEditorDialogData);

    if (!isNil(criterionLibrary)) {
      this.$emit('onModifyTreatmentDetails', setItemPropertyValue('criterionLibrary', criterionLibrary, this.selectedTreatmentDetails));
    }
  }

  onEditTreatmentDetails(property: string, value: any) {
    this.$emit('onModifyTreatmentDetails', setItemPropertyValue(property, value, this.selectedTreatmentDetails));
  }

  onRemoveTreatmentCriterionLibrary() {
    this.$emit('onModifyTreatmentDetails', setItemPropertyValue('criterionLibrary', clone(emptyCriterionLibrary), this.selectedTreatmentDetails));
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
