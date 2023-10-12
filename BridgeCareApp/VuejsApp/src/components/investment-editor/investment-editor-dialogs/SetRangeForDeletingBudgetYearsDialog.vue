<template>
  <Dialog max-width="200px" persistent v-bind:show="showDialog">
    <v-card>
      <v-card-title>
        <v-row justify-center>
          <h3>Set Number of Years to Delete</h3>
        </v-row>
      </v-card-title>
      <v-card-text>
        <v-text-field type="number" :mask="'##########'" label="Edit" single-line
          v-model.number="range"
          :min="1"
          :max="maxRange"
          :rules="[rules['generalRules'].valueIsNotEmpty, rules['generalRules'].valueIsWithinRange(range, [1,maxRange])]"/>
        <label>{{rangeLabel}}</label>
      </v-card-text>
      <v-card-actions>
        <v-btn :disabled="range === 0 || range > maxRange" @click="onSubmit(true)" class="ara-blue-bg text-white">Save</v-btn>
        <v-btn @click="onSubmit(false)" class="ara-orange-bg text-white">Cancel</v-btn>
      </v-card-actions>
    </v-card>
  </Dialog>
</template>

<script lang="ts" setup >
import Vue from 'vue';
import {InputValidationRules, rules as validationRules} from '@/shared/utils/input-validation-rules';
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
import Dialog from 'primevue/dialog';

const emit = defineEmits(['submit'])
const props = defineProps<{
  showDialog: boolean,
  endYear: number,
  maxRange : number
}>()
let range : number =1;
let rules: InputValidationRules = validationRules;
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
