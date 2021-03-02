<template>
  <v-dialog v-model="dialogData.showDialog" max-width="450px" persistent>
    <v-card>
      <v-card-title>
        <v-layout justify-center>
          <h3>New Criteria Library</h3>
        </v-layout>
      </v-card-title>
      <v-card-text>
        <v-layout column>
          <v-text-field v-model="newCriterionLibrary.name" label="Name" outline>
          </v-text-field>
          <v-textarea v-model="newCriterionLibrary.description" label="Description" no-resize outline rows="3">
          </v-textarea>
        </v-layout>
      </v-card-text>
      <v-card-actions>
        <v-layout justify-space-between row>
          <v-btn @click="onSubmit(true)" :disabled="newCriterionLibrary.name === ''"
                 class="ara-blue-bg white--text">
            Save
          </v-btn>
          <v-btn @click="onSubmit(false)" class="ara-orange-bg white--text">Cancel</v-btn>
        </v-layout>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import Vue from 'vue';
import {Component, Prop, Watch} from 'vue-property-decorator';
import {CreateCriterionLibraryDialogData} from '@/shared/models/modals/create-criterion-library-dialog-data';
import {CriterionLibrary, emptyCriterionLibrary} from '@/shared/models/iAM/criteria';
import {getUserName} from '@/shared/utils/get-user-info';
import {getNewGuid} from '@/shared/utils/uuid-utils';

@Component
export default class CreateCriterionLibraryDialog extends Vue {
  @Prop() dialogData: CreateCriterionLibraryDialogData;

  newCriterionLibrary: CriterionLibrary = {...emptyCriterionLibrary, id: getNewGuid()};

  @Watch('dialogData')
  onDialogDataChanged() {
    this.newCriterionLibrary = {
      ...this.newCriterionLibrary,
      mergedCriteriaExpression: this.dialogData.mergedCriteriaExpression,
      owner: getUserName()
    };
  }

  onSubmit(submit: boolean) {
    if (submit) {
      this.$emit('submit', this.newCriterionLibrary);
    } else {
      this.$emit('submit', null);
    }

    this.newCriterionLibrary = {...emptyCriterionLibrary, id: getNewGuid()};
  }
}
</script>