<template>
  <v-dialog max-width="450px" persistent v-model="showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-box-padding-top">
        <v-layout justify-space-between align-center>
          <div class="ghd-control-dialog-header">Add Consequence</div>
          <v-btn 
              id="CreateCommittedProjectConsequenceDialog-close-vbtn"
              @click="onSubmit(false)" flat class="ghd-close-button">
              X
          </v-btn>
        </v-layout>
      </v-card-title>
      <v-card-text class="ghd-dialog-box-padding-center"
        id="CreateCommittedProjectConsequenceDialog-content-vCardText"
      >
        <v-layout column
          id="CreateCommittedProjectConsequenceDialog-content-vLayout"
          >
          <v-flex
            id="CreateCommittedProjectConsequenceDialog-attribute-vFlex">
            <v-subheader class="ghd-md-gray ghd-control-label">Attribute</v-subheader>
            <v-select :items="attributeNames"
              append-icon=$vuetify.icons.ghd-down
              outline
              v-model="newConsequence.attribute" :rules="[rules['generalRules'].valueIsNotEmpty]"
              class="ghd-select ghd-text-field ghd-text-field-border">
            </v-select>
          </v-flex>
          <v-flex
            id="CreateCommittedProjectConsequenceDialog-changeValue-vFlex">
            <v-subheader class="ghd-md-gray ghd-control-label">Change Value</v-subheader>
            <v-text-field outline v-model="newConsequence.changeValue"
              :rules="[rules['generalRules'].valueIsNotEmpty]"
              class="ghd-text-field-border ghd-text-field"></v-text-field>
          </v-flex>         
        </v-layout>
      </v-card-text>
      <v-card-actions class="ghd-dialog-box-padding-bottom">
        <v-layout justify-center row>
          <v-btn @click="onSubmit(false)" flat class='ghd-blue ghd-button-text ghd-button'>
            Cancel
          </v-btn >
          <v-btn :disabled="disableSubmitButton()" @click="onSubmit(true)" outline class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'>
            Save
          </v-btn>         
        </v-layout>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import Vue from 'vue';
import {Component, Prop} from 'vue-property-decorator';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import { CommittedProjectConsequence, emptyCommittedProjectConsequence } from '@/shared/models/iAM/committed-projects';
import { State } from 'vuex-class';
import { Attribute } from '@/shared/models/iAM/attribute';
import { hasValue } from '@/shared/utils/has-value-util';
import { getPropertyValues } from '@/shared/utils/getter-utils';

@Component
export default class CreateConsequenceDialog extends Vue {
  @Prop() showDialog: boolean;

  mounted() {
    this.setAttributes();
  }

  newConsequence: CommittedProjectConsequence = {...emptyCommittedProjectConsequence, id: getNewGuid()};
  rules: InputValidationRules = rules;
  attributeNames: string[] = [];

  @State(state => state.attributeModule.attributes) stateAttributes: Attribute[];

  disableSubmitButton() {
    return !(this.rules['generalRules'].valueIsNotEmpty(this.newConsequence.changeValue) === true);
  }

  setAttributes() {
    if (hasValue(this.stateAttributes)) {
      this.attributeNames = getPropertyValues('name', this.stateAttributes);

      if (this.showDialog) {
        this.setNewDeficientConditionGoalDefaultValues();
      }
    }
  }

  onSubmit(submit: boolean) {
    if (submit) {
      this.$emit('submit', this.newConsequence);
    } else {
      this.$emit('submit', null);
    }

    this.newConsequence = {...emptyCommittedProjectConsequence, id: getNewGuid()};
  }
}
</script>