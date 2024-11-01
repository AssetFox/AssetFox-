<template>
  <v-dialog max-width="200px" persistent v-model="showDialogComputed">
    <v-card>
      <v-card-title>
        <v-row justify-center>
          <h3>Set Number of Years to Add</h3>
        </v-row>
      </v-card-title>
      <v-card-text>
        <v-text-field type="number" min=1 v-maska:[mask] label="Edit" outline v-model.number="range"/>
        <label>{{rangeLabel}}</label>
      </v-card-text>
      <v-card-actions>
        <v-btn :disabled="range === 0" @click="onSubmit(true)" class="assetFox-blue-bg text-white">Save</v-btn>
        <v-btn @click="onSubmit(false)" class="assetFox-orange-bg text-white">Cancel</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import Vue, { computed } from 'vue';
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';

const emit = defineEmits(['submit'])
const props = defineProps<{
  showDialog: boolean,
  startYear: number
}>()
let showDialogComputed = computed(() => props.showDialog);
let range: number = 1;
const mask = { mask: '##########' };

function rangeLabel() {
    return 'Year Range: ' + (range <= 1 ? props.startYear : props.startYear + '-' + (props.startYear + range - 1));
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
