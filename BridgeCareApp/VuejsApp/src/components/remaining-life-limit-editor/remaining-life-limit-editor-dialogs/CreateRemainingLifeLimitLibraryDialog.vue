<template>
  <v-dialog width="50%" persistent v-model ="dialogData.showDialog">
    <v-card>
       <v-card-title class="ghd-dialog-padding-top-title">
        <v-row justify="space-between">
          <div class="dialog-header"><h5>Create New Remaining Life Limit Library</h5></div>        
          <XButton @click="onSubmit(false)"/>
        </v-row>
      </v-card-title>
      <v-card-text class="ghd-dialog-text-field-padding">
        <v-row>
          <v-col>
          <v-subheader class="ghd-control-label ghd-md-gray">Name</v-subheader>
        
          <v-text-field id="CreateRemainingLifeLimitLibraryDialog-nane-vtextarea"
                        v-model="newRemainingLifeLimitLibrary.name"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"
                        variant="outlined"
                        density="compact" 
                        class="ghd-control-text ghd-control-border"/>
        
          <v-subheader class="ghd-control-label ghd-md-gray">Description</v-subheader>
       
          <v-textarea id="CreateRemainingLifeLimitLibraryDialog-description-vtextarea"
                      no-resize
                      variant="outlined"
                      rows="3"
                      height="100px"
                      class="ghd-control-text ghd-control-border"
                      v-model="newRemainingLifeLimitLibrary.description"/>
          </v-col>
        </v-row>
      </v-card-text>
      <v-card-actions class="py-0">
        <v-row justify="center" row class="ghd-dialog-padding-bottom-buttons">
          <CancelButton @cancel="onSubmit(false)"/>
          <SaveButton 
            @save="onSubmit(true)"
            :disabled="newRemainingLifeLimitLibrary.name === ''"  
          />
        </v-row>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import Vue, { watch, toRefs, ref } from 'vue';
import {CreateRemainingLifeLimitLibraryDialogData} from '@/shared/models/modals/create-remaining-life-limit-library-dialog-data';
import {
  emptyRemainingLifeLimitLibrary,
  RemainingLifeLimit,
  RemainingLifeLimitLibrary
} from '@/shared/models/iAM/remaining-life-limit';
import {rules as validationRules, InputValidationRules} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import {hasValue} from '@/shared/utils/has-value-util';
import SaveButton from '@/shared/components/buttons/SaveButton.vue';
import CancelButton from '@/shared/components/buttons/CancelButton.vue';
import XButton from '@/shared/components/buttons/XButton.vue';

  const props = defineProps<{
    dialogData: CreateRemainingLifeLimitLibraryDialogData
  }>();
  const { dialogData } = toRefs(props);

  const emit = defineEmits(['submit']);

  let newRemainingLifeLimitLibrary = ref<RemainingLifeLimitLibrary>({...emptyRemainingLifeLimitLibrary, id: getNewGuid()});
  let rules: InputValidationRules = {...validationRules};

  watch((() => props.dialogData), ()=> onDialogDataChanged())
  function onDialogDataChanged() {
    newRemainingLifeLimitLibrary.value = {
      ...newRemainingLifeLimitLibrary.value,
      remainingLifeLimits: hasValue(props.dialogData.remainingLifeLimits)
          ? props.dialogData.remainingLifeLimits.map((remainingLifeLimit: RemainingLifeLimit) => ({
            ...remainingLifeLimit,
            id: getNewGuid()
          }))
          : []
    };
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', newRemainingLifeLimitLibrary.value);
    } else {
      emit('submit', null);
    }

    newRemainingLifeLimitLibrary.value = {...emptyRemainingLifeLimitLibrary, id: getNewGuid()};
  }
</script>
