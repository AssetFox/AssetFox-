<template>
    <v-layout
      column
      class="Montserrat-font-family ma-0"
      style="width: 25%; padding-left: 50px"
    >
      <v-layout align-center class="vl-style">
        <v-flex xs12>
          <v-layout column>
            <v-subheader
              class="ghd-control-label ghd-md-gray Montserrat-font-family"
              >Implementation Name</v-subheader
            >
          </v-layout>
        </v-flex>
        <v-flex xs12>
          <v-layout column style="padding-right: 100px">
            <v-text-field
              class="ghd-select ghd-text-field ghd-text-field-border Montserrat-font-family search-icon-general"
              v-model="ImplementationID"
              type="text"
              hide-details
              clearable
              single-line
              outline
              style="padding-left: 10px; width: 250%"
            >
            </v-text-field>
          </v-layout>
        </v-flex>
        <v-flex xs12>
          <v-layout column>
            <v-btn
              class="ghd-blue ghd-button-text ghd-outline-button-padding ghd-button"
              style="padding-left: 10px"
              @click="onSaveImplementationName"
              variant = "outlined"
              >Save</v-btn
            >
          </v-layout>
        </v-flex>
      </v-layout>
      <v-layout align-center class="vl-style" style="margin-top: 5%">
        <v-flex xs12>
          <v-layout column>
            <v-subheader
              class="ghd-control-label ghd-md-gray Montserrat-font-family"
              >Agency Logo
            </v-subheader>
          </v-layout>
        </v-flex>
        <v-flex xs12>
          <v-layout column>
            <input
              id="agencyImageUpload"
              type="file"
              accept="image/*"
              ref="agencyFileInput"
              @change="handleAgencyLogoUpload"
              hidden
            />
            <v-btn
              class="ghd-blue ghd-button-text ghd-outline-button-padding ghd-button"
              style="margin-right: 40%"
              @click="onUploadAgencyLogo"
              variant = "outlined"
              >Upload</v-btn
            >
          </v-layout>
        </v-flex>
      </v-layout>
      <v-layout align-center class="vl-style">
        <v-flex xs12>
          <v-layout column>
            <v-subheader
              class="ghd-control-label ghd-md-gray Montserrat-font-family"
              >Implementation Logo
            </v-subheader>
          </v-layout>
        </v-flex>
        <v-flex xs12>
          <v-layout column>
            <input
              id="implementationImageUpload"
              type="file"
              ref="implementationFileInput"
              accept="image/*"
              @change="handleImplementationLogoUpload"
              hidden
            />
            <v-btn
              class="ghd-blue ghd-button-text ghd-outline-button-padding ghd-button"
              style="margin-right: 40%"
              @click="onUploadImplementationLogo"
              variant = "outlined"
              >Upload</v-btn
            >
          </v-layout>
        </v-flex>
      </v-layout>
    </v-layout>
  </template>
  <script lang="ts" setup>
  import Vue from 'vue';
  import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
  import { useStore } from 'vuex';
  import { useRouter } from 'vue-router';

  let store = useStore();
  let implementationName = ref<string>(store.state.adminDataModule.implementationName);
  let agencyLogo = ref<string>(store.state.adminDataModule.agencyLogo);
  let implementationLogo = ref<string>(store.state.adminDataModule.implementationLogo);
  let ImplementationID:string = '';
  async function getImplementationNameAction(payload?: any): Promise<any> {await store.dispatch('getImplementationName');}
  async function importImplementationNameAction(implementationName:string): Promise<any> {await store.dispatch('getImplementationName');}
  async function importAgencyLogoAction(payload?: any): Promise<any> {await store.dispatch('importAgencyLogo');}
  async function importProductLogoAction(payload?: any): Promise<any> {await store.dispatch('importProductLogo');}

  function onSaveImplementationName(){
    importImplementationNameAction(ImplementationID);
  }
  function onUploadImplementationLogo(){
      document.getElementById("implementationImageUpload")?.click();
   }

   function onUploadAgencyLogo(){
      document.getElementById("agencyImageUpload")?.click();
   }
   function handleImplementationLogoUpload(payload: any){
    const file = payload.files[0];
    importProductLogoAction(file);  
}
  function handleAgencyLogoUpload(payload: any){
    const file = payload.files[0];
    importAgencyLogoAction(file);
  }
  </script>
  