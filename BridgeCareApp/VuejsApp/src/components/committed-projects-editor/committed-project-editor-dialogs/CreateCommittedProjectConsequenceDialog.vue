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
              variant="outlined"
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

<script lang="ts" setup>
import {watch, ref, onMounted, reactive} from 'vue';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import { CommittedProjectConsequence, emptyCommittedProjectConsequence } from '@/shared/models/iAM/committed-projects';
import { Attribute } from '@/shared/models/iAM/attribute';
import { hasValue } from '@/shared/utils/has-value-util';
import { getPropertyValues } from '@/shared/utils/getter-utils';
import { createDecipheriv } from 'crypto';
import { useStore } from 'vuex';

  let store = useStore();
  const emit = defineEmits(['submit']);
  const props = defineProps({showDialog: Boolean});
  let showDialog = ref<boolean>(props.showDialog);

  onMounted(() => mounted())
  function mounted() {
    setAttributes();
  }

  let newConsequence: CommittedProjectConsequence = {...emptyCommittedProjectConsequence, id: getNewGuid()};
  let inputRules: InputValidationRules = rules;
  let attributeNames: string[] = [];

  // @State(state => state.attributeModule.attributes) stateAttributes: Attribute[];
  // async function stateAttributes(payload?: any): Promise<any>{ await store.dispatch('');}
  const stateAttributes = reactive<Attribute[]>(store.state.attributeModule.attributes);

  function disableSubmitButton() {
    return !(inputRules['generalRules'].valueIsNotEmpty(newConsequence.changeValue) === true);
  }

  function setAttributes() {
    if (hasValue(stateAttributes)) {
      attributeNames = getPropertyValues('name', stateAttributes);

      // if (showDialog) {
      //   setNewDeficientConditionGoalDefaultValues();
      // }
    }
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', newConsequence);
    } else {
      emit('submit', null);
    }

    newConsequence = {...emptyCommittedProjectConsequence, id: getNewGuid()};
  }
</script>