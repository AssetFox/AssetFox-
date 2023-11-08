<template>
  <v-dialog max-width="450px" persistent v-model="showDialogComputed">
    <v-card>
      <v-card-title class="ghd-dialog-box-padding-top">
         <v-row justify-space-between align-center>
            <div class="ghd-control-dialog-header">New Data Source</div>
          </v-row>
        </v-card-title>           
      <v-card-text class="ghd-dialog-box-padding-center">
        <v-row column>
          <v-text-field label="Name" outline id="CreateDataSourceDialog-Name-vtextField"
            v-model="datasourceName"
            class="ghd-text-field-border ghd-text-field"/>
        </v-row>
      </v-card-text>
      <v-card-actions class="ghd-dialog-box-padding-bottom">
        <v-row justify-center row>
          <v-btn id="CreateDataSourceDialog-Cancel-vbtn" @click="onSubmit(false)" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' style="margin-right:auto; margin-left:auto;" variant = "flat">Cancel</v-btn>
          <v-btn id="CreateDataSourceDialog-Save-vbtn" @click="onSubmit(true)"
                 class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' style="margin-right:auto; margin-left:auto;" variant = "outlined">
            Save
          </v-btn>          
        </v-row>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import Vue, { computed, ref, Ref, shallowRef, ShallowRef, watch } from 'vue';
import {InputValidationRules, rules as validationRules} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import { Datasource, emptyDatasource, DSSQL } from '../../../shared/models/iAM/data-source';
import { CreateDataSourceDialogData} from '@/shared/models/modals/data-source-dialog-data'
import { getUserName } from '@/shared/utils/get-user-info';
import { useStore } from 'vuex';
import { clone } from 'ramda';

  let store = useStore();
  let getIdByUserNameGetter: any = store.getters.getIdByUserName ;

  const props = defineProps<{
    dialogData: CreateDataSourceDialogData
  }>()
  let showDialogComputed = computed(() => props.dialogData.showDialog);
  const emit = defineEmits(['submit'])

   const newDataSource = ref<Datasource>(emptyDatasource);
  let rules: InputValidationRules = validationRules;
  let datasourceName = ref<string>('New Data Source');

  watch(datasourceName, () => { 
      newDataSource.value.name = datasourceName.value;
  })

  watch(() => props.dialogData, () => {  
    newDataSource.value = {
        id: getNewGuid(),
        createdBy: getIdByUserNameGetter(getUserName()),
        name: datasourceName.value,
        type: DSSQL,
        connectionString: '',
        dateColumn: '', 
        locationColumn: '',
        secure: false
    };
  })

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', newDataSource.value);
    } else {
      emit('submit', null);
    }

    props.dialogData.showDialog = false;
  }

</script>