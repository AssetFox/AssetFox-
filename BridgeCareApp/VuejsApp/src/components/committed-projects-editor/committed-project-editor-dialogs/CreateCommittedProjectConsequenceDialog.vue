<template>
  <v-dialog max-width="450px" persistent v-model="showDialogComputed">
    <v-card>
      <v-card-title class="ghd-dialog-box-padding-top">
        <v-row justify-space-between align-center>
          <div class="ghd-control-dialog-header">Add Consequence</div>
          <v-btn 
              id="CreateCommittedProjectConsequenceDialog-close-vbtn"
              @click="onSubmit(false)" variant = "flat" class="ghd-close-button">
              X
          </v-btn>
        </v-row>
      </v-card-title>
      <v-card-text class="ghd-dialog-box-padding-center"
        id="CreateCommittedProjectConsequenceDialog-content-vCardText"
      >
        <v-row column
          id="CreateCommittedProjectConsequenceDialog-content-vLayout"
          >
          <v-col
            id="CreateCommittedProjectConsequenceDialog-attribute-vFlex">
            <v-subheader class="ghd-md-gray ghd-control-label">Attribute</v-subheader>
            <v-select :items="attributeNames"
              menu-icon=custom:GhdDownSvg
              variant="outlined"
              v-model="newConsequence.attribute" :rules="[rules['generalRules'].valueIsNotEmpty]"
              class="ghd-select ghd-text-field ghd-text-field-border">
            </v-select>
          </v-col>
          <v-col
            id="CreateCommittedProjectConsequenceDialog-changeValue-vFlex">
            <v-subheader class="ghd-md-gray ghd-control-label">Change Value</v-subheader>
            <v-text-field outline v-model="newConsequence.changeValue"
              :rules="[rules['generalRules'].valueIsNotEmpty]"
              class="ghd-text-field-border ghd-text-field"></v-text-field>
          </v-col>         
        </v-row>
      </v-card-text>
      <v-card-actions class="ghd-dialog-box-padding-bottom">
        <v-row justify-center row>
          <CancelButton @cancel="onSubmit(false)"/>
          <SaveButton 
            @save="onSubmit(true)"
            :disabled="disableSubmitButton()"
          />       
        </v-row>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts" setup>
import {watch, ref, onMounted, reactive, computed} from 'vue';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import { CommittedProjectConsequence, emptyCommittedProjectConsequence } from '@/shared/models/iAM/committed-projects';
import { Attribute } from '@/shared/models/iAM/attribute';
import { hasValue } from '@/shared/utils/has-value-util';
import { getPropertyValues } from '@/shared/utils/getter-utils';
import { createDecipheriv } from 'crypto';
import { useStore } from 'vuex';
import SaveButton from '@/shared/components/buttons/SaveButton.vue';
import CancelButton from '@/shared/components/buttons/CancelButton.vue';

  let store = useStore();
  const emit = defineEmits(['submit']);
  const props = defineProps({showDialog: Boolean});
  let showDialogComputed = computed(() => props.showDialog);
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