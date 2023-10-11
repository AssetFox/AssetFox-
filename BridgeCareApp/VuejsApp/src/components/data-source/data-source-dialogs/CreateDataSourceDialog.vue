<template>
  <v-dialog max-width="450px" persistent v-bind:show="dialogData.showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-box-padding-top">
         <v-layout justify-space-between align-center>
            <div class="ghd-control-dialog-header">New Data Source</div>
            <v-btn @click="onSubmit(false)" variant = "flat" class="ghd-close-button">
              X
            </v-btn>
          </v-layout>
        </v-card-title>           
      <v-card-text class="ghd-dialog-box-padding-center">
        <v-layout column>
          <v-subheader class="ghd-md-gray ghd-control-label">Name</v-subheader>
          <v-text-field outline id="CreateDataSourceDialog-Name-vtextField"
            v-model="datasourceName"
            class="ghd-text-field-border ghd-text-field"/>
        </v-layout>
      </v-card-text>
      <v-card-actions class="ghd-dialog-box-padding-bottom">
        <v-layout justify-center row>
          <v-btn id="CreateDataSourceDialog-Cancel-vbtn" @click="onSubmit(false)" class='ghd-blue ghd-button-text ghd-button' variant = "flat">Cancel</v-btn>
          <v-btn id="CreateDataSourceDialog-Save-vbtn" @click="onSubmit(true)"
                 class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined">
            Save
          </v-btn>          
        </v-layout>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import Vue, { ref, Ref, shallowRef, ShallowRef, watch } from 'vue';
import {InputValidationRules, rules as validationRules} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import { Datasource, emptyDatasource, DSSQL } from '../../../shared/models/iAM/data-source';
import { CreateDataSourceDialogData} from '@/shared/models/modals/data-source-dialog-data'
import { getUserName } from '@/shared/utils/get-user-info';
import { useStore } from 'vuex';

  let store = useStore();
  let getIdByUserNameGetter: any = store.getters.getIdByUserName ;

  const props = defineProps<{
    dialogData: CreateDataSourceDialogData
  }>()
  const emit = defineEmits(['submit'])

  let newDataSource: ShallowRef<Datasource> = shallowRef(emptyDatasource);
  let rules: InputValidationRules = validationRules;
  let datasourceName: Ref<string> = ref('New Data Source');

  watch(datasourceName, () => onDataSourceNameChanged)
  function onDataSourceNameChanged() {
      newDataSource.value.name = datasourceName.value;
  }

  watch(() => props.dialogData, () => onDialogDataChanged)
  function onDialogDataChanged() {
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
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', newDataSource);
    } else {
      emit('submit', null);
    }

    newDataSource.value = emptyDatasource;
    props.dialogData.showDialog = false;
  }

</script>