<template>
  <v-dialog v-model="showDialogComputed" max-width="444px" persistent>
    <v-card height="437px" class="ghd-dialog">
      <v-card-title >
        <v-row justify-left>
          <h3 class="ghd-dialog">Create New Deterioration Model Library</h3>
        </v-row>
      </v-card-title>
      <v-card-text >
        <v-row >
          <v-col>
          <v-subheader class="ghd-control-label ghd-md-gray">Name</v-subheader>             
          <v-text-field id="CreatePerformanceCurveLibraryDialog-Name-vtextfield" class="ghd-control-text ghd-control-border"
                        v-model="newPerformanceCurveLibrary.name"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"
                        variant="outlined"/>
          <v-subheader class="ghd-control-label ghd-md-gray">Description</v-subheader>             
          <v-textarea id="CreatePerformanceCurveLibraryDialog-Description-vtextfield" class="ghd-control-text ghd-control-border"
                      v-model="newPerformanceCurveLibrary.description"
                      no-resize
                      variant="outlined"
                      rows="3"/>
            </v-col>
        </v-row>
      </v-card-text>
      <v-card-actions>
          <v-row justify="center" row>
            <v-btn id="CreatePerformanceCurveLibraryDialog-Cancel-vbtn" variant = "outlined"
                   class="ghd-white-bg ghd-blue ghd-button-text"
                   
                   @click="onSubmit(false)">
              Cancel
            </v-btn>
            <v-btn id="CreatePerformanceCurveLibraryDialog-Save-vbtn" :disabled="newPerformanceCurveLibrary.name === ''"
                   class="ghd-blue-bg ghd-white ghd-button-text"
                   @click="onSubmit(true)"
                   variant = "flat"                   
                   >
              Save
            </v-btn>
          </v-row>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts" setup>
import Vue, { computed } from 'vue';
import {emptyPerformanceCurveLibrary, PerformanceCurve, PerformanceCurveLibrary} from '@/shared/models/iAM/performance';
import {CreatePerformanceCurveLibraryDialogData} from '@/shared/models/modals/create-performance-curve-library-dialog-data';
import {getUserName} from '@/shared/utils/get-user-info';
import {InputValidationRules, rules as validationRules} from '@/shared/utils/input-validation-rules';
import {clone} from 'ramda';
import {getBlankGuid, getNewGuid} from '@/shared/utils/uuid-utils';
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';

const emit = defineEmits(['submit'])
let store = useStore();
const props = defineProps<{
  dialogData: CreatePerformanceCurveLibraryDialogData
    }>()
    let showDialogComputed = computed(() => props.dialogData.showDialog);
    let getIdByUserNameGetter: any = store.getters.getIdByUserName
    let newPerformanceCurveLibrary = ref<PerformanceCurveLibrary>({...emptyPerformanceCurveLibrary, id: getNewGuid()});
    let rules: InputValidationRules = validationRules;

  watch(()=>props.dialogData,()=>onDialogDataChanged())
  function onDialogDataChanged() {
    let currentUser: string = getUserName();

    newPerformanceCurveLibrary.value = {
      ...newPerformanceCurveLibrary.value,
      performanceCurves: props.dialogData.performanceCurves.map((performanceCurve: PerformanceCurve) => {
        performanceCurve.id = getNewGuid();
        if (performanceCurve.equation.id !== getBlankGuid()) {
          performanceCurve.equation.id = getNewGuid();
        }
        return performanceCurve;
      }),
      owner: getIdByUserNameGetter(currentUser),
    };
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', newPerformanceCurveLibrary.value);
    } else {
      emit('submit', null);
    }

    newPerformanceCurveLibrary.value = {...emptyPerformanceCurveLibrary, id: getNewGuid()};
  }
</script>
