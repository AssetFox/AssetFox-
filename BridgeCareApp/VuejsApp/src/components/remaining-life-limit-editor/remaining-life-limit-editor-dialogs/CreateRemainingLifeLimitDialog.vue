<template>
  <v-dialog width="50%" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-padding-top-title">
        <v-row justify="space-between">
          <div class="dialog-header"><h5>Add New Remaining Life Limit</h5></div>          
          <XButton @click="onSubmit(false)"/>
        </v-row>
      </v-card-title>

      <v-card-text class="ghd-dialog-text-field-padding">
        <v-row>
          <v-col>
            <v-subheader class="ghd-control-label ghd-md-gray">Select an Attribute</v-subheader>        
            <v-select id="CreateRemainingLifeLimitDialog-selectAnAttribute-select"
                      :items="dialogData.numericAttributeSelectItems"
                      item-title="text"
                      item-value="value"
                      menu-icon=custom:GhdDownSvg
                      v-model="newRemainingLifeLimit.attribute"
                      :rules="[rules['generalRules'].valueIsNotEmpty]"
                      variant="outlined"
                      density="compact"
                      class="ghd-select ghd-control-text ghd-text-field ghd-text-field-border"/>
        
            <v-subheader class="ghd-control-label ghd-md-gray">Limit</v-subheader>
          
            <v-text-field id="CreateRemainingLifeLimitDialog-limit-textField"
                          variant="outlined"
                          density="compact"
                          v-maska:[mask]
                          v-model.number="newRemainingLifeLimit.value"
                          :rules="[rules['generalRules'].valueIsNotEmpty]"
                          class="ghd-control-text ghd-control-border"/>
          </v-col>
        </v-row>
      </v-card-text>

      <v-card-actions>    
        <v-row justify="center" class="ghd-dialog-padding-bottom-buttons">
          <CancelButton @cancel="onSubmit(false)"/>
          <SaveButton 
            @save="onSubmit(true)"
            :disabled="disableSubmitAction()"
          />
        </v-row>
      </v-card-actions>

    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import Vue, { ref, toRefs, watch } from 'vue';
import {emptyRemainingLifeLimit, RemainingLifeLimit} from '@/shared/models/iAM/remaining-life-limit';
import {CreateRemainingLifeLimitDialogData} from '@/shared/models/modals/create-remaining-life-limit-dialog-data';
import {InputValidationRules, rules as validationRules} from '@/shared/utils/input-validation-rules';
import {hasValue} from '@/shared/utils/has-value-util';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import {clone} from 'ramda';
import SaveButton from '@/shared/components/buttons/SaveButton.vue';
import CancelButton from '@/shared/components/buttons/CancelButton.vue';
import XButton from '@/shared/components/buttons/XButton.vue';

  const props = defineProps<{
    dialogData: CreateRemainingLifeLimitDialogData
  }>();
  const { dialogData } = toRefs(props);

  const emit = defineEmits(['submit']);

  let newRemainingLifeLimit = ref<RemainingLifeLimit>({...emptyRemainingLifeLimit, id: getNewGuid()});
  let rules: InputValidationRules = clone(validationRules);
  const mask = { mask: '##########' };
  
  watch(dialogData,()=> {
    newRemainingLifeLimit.value.attribute = hasValue(dialogData.value.numericAttributeSelectItems)
        ? props.dialogData.numericAttributeSelectItems[0].value.toString() : '';
  });

  function disableSubmitAction() {
    return rules['generalRules'].valueIsNotEmpty(newRemainingLifeLimit.value.attribute) !== true ||
        rules['generalRules'].valueIsNotEmpty(newRemainingLifeLimit.value) !== true;
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', newRemainingLifeLimit.value);
    } else {
      emit('submit', null);
    }

    newRemainingLifeLimit.value = {...emptyRemainingLifeLimit, id: getNewGuid()};
  }

</script>
