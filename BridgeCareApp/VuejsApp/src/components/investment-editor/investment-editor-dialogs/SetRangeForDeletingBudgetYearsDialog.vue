<template>
  <v-dialog max-width="200px" persistent v-model="showDialogComputed">
    <v-card>
      <v-card-title>
        <v-row justify-center>
          <h3>Set Number of Years to Delete</h3>
        </v-row>
      </v-card-title>
      <v-card-text>
        <v-text-field type="number" v-maska:[mask] label="Edit" single-line
          v-model.number="range"
          :min="1"
          :max="maxRange"
          :rules="[rules['generalRules'].valueIsNotEmpty, rules['generalRules'].valueIsWithinRange(range, [1,maxRange])]"/>
        <label>{{rangeLabel}}</label>
      </v-card-text>
      <v-card-actions>
        <v-btn :disabled="range === 0 || range > maxRange" @click="onSubmit(true)" class="assetFox-blue-bg text-white">Save</v-btn>
        <v-btn @click="onSubmit(false)" class="assetFox-orange-bg text-white">Cancel</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts" setup >
import Vue, { computed } from 'vue';
import {InputValidationRules, rules as validationRules} from '@/shared/utils/input-validation-rules';
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';

const emit = defineEmits(['submit'])
const props = defineProps<{
  showDialog: boolean,
  endYear: number,
  maxRange : number
}>()
let showDialogComputed = computed(() => props.showDialog);
let range : number =1;
let rules: InputValidationRules = validationRules;
const mask = { mask: '##########' };

function rangeLabel() {
    return 'Year Range: ' + (range <= 1 ? props.endYear : (props.endYear - range + 1) + '-' + props.endYear);
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', range);
    } else {
      emit('submit', 0);
    }

    range = 1;
  }

</script>
