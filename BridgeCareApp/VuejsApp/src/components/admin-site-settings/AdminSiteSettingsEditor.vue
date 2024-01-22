<template>
    <v-row
      column
      class="Montserrat-font-family ma-0"
      style="width: 25%; padding-left: 50px"
    >
    <v-col></v-col>
      <v-row align-center class="vl-style">
        <v-col cols="11">
          <v-row column>
            <v-subheader
              class="ghd-control-label ghd-md-gray Montserrat-font-family"
              >Implementation Name</v-subheader
            >
          </v-row>
        </v-col>
        <v-col cols="8">
          <v-row column style="padding-right: 100px">
            <v-text-field
              variant="outlined"
              id="AdminSiteSettingsEditor-EditImplementationName-textfield"
              v-model="ImplementationID"
              type="text"
              hide-details
              clearable
              single-line           
              style="padding-left: 0px; width: 250%"
              density="compact"
            >
            </v-text-field>
            <v-col >
          <v-row column style="padding-left: 300px; margin-top: -46px; margin-left: 25px;">
            <v-btn
              id="AdminSiteSettingsEditor-SaveImplementationName-btn"
              class="ghd-blue ghd-button-text ghd-outline-button-padding ghd-button"
              style="padding-left: 10px"
              @click="onSaveImplementationName"
              variant = "outlined"
              >Save</v-btn
            >
          </v-row>
        </v-col>
          </v-row>
        </v-col>
        
      </v-row>
      <v-row align-center class="vl-style" style="margin-top: 5%">
        <v-col cols="12">
          <v-row column>
            <v-subheader
              class="ghd-control-label ghd-md-gray Montserrat-font-family"
              >Agency Logo
            </v-subheader>
          </v-row>
        </v-col>
        <v-col cols="12">
          <v-row column style="padding-left: 330px; margin-top: -45px;" >
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
          </v-row>
        </v-col>
      </v-row>
      <v-row align-center class="vl-style">
        <v-col cols="12">
          <v-row column>
            <v-subheader
              class="ghd-control-label ghd-md-gray Montserrat-font-family"
              >Implementation Logo
            </v-subheader>
          </v-row>
        </v-col>
        <v-col cols="12">
          <v-row column style="padding-left: 330px; margin-top: -35px;">
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
          </v-row>
        </v-col>
      </v-row>
    </v-row>
  </template>
  <script lang="ts" setup>
  import Vue, { computed } from 'vue';
  import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
  import { useStore } from 'vuex';
  import { useRouter } from 'vue-router';

  let store = useStore();
  let implementationName = computed<string>(()=>store.state.adminSiteSettingsModule.implementationName);
  let agencyLogo = computed<string>(()=>store.state.adminSiteSettingsModule.agencyLogo);
  let implementationLogo = computed<string>(()=>store.state.adminSiteSettingsModule.implementationLogo);
  let ImplementationID = ref('');
  async function getImplementationNameAction(payload?: any): Promise<any> {await store.dispatch('getImplementationName',payload);}
  async function importImplementationNameAction(implementationName:string): Promise<any> {await store.dispatch('importImplementationName',implementationName);}
  async function importAgencyLogoAction(payload?: any): Promise<any> {await store.dispatch('importAgencyLogo',payload);}
  async function importProductLogoAction(payload?: any): Promise<any> {await store.dispatch('importProductLogo',payload);}

  function onSaveImplementationName(){
    importImplementationNameAction(ImplementationID.value);
  }
  function onUploadImplementationLogo(){
      document.getElementById("implementationImageUpload")?.click();
   }

   function onUploadAgencyLogo(){
      document.getElementById("agencyImageUpload")?.click();
   }
   function handleImplementationLogoUpload(payload: any){
    const file = payload.target.files[0];
    importProductLogoAction(file);  
}
  function handleAgencyLogoUpload(payload: any){
    const file = payload.target.files[0];
    importAgencyLogoAction(file);
  }
  </script>
  