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
              id="AdminSiteSettingsEditor-EditImplementationName-textfield" class="ghd-select ghd-text-field ghd-text-field-border Montserrat-font-family search-icon-general"
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
              id="AdminSiteSettingsEditor-SaveImplementationName-btn"
              class="ghd-blue ghd-button-text ghd-outline-button-padding ghd-button"
              style="padding-left: 10px"
              @click="onSaveImplementationName"
              outline
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
              outline
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
              outline
              >Upload</v-btn
            >
          </v-layout>
        </v-flex>
      </v-layout>
    </v-layout>
  </template>
  <script lang="ts">
    import Vue from 'vue';
  import { Component, Watch} from 'vue-property-decorator';
  import { Action, State } from 'vuex-class';
  
  @Component
  export default class AdminSiteSettingsEditor extends Vue{
      @State(state => state.siteAdminModule.implementationName) implementationName: string;
      @State(state => state.siteAdminModule.agencyLogo) agencyLogo: string;
      @State(state => state.siteAdminModule.implementationLogo) implementationLogo: string;
       @Action('getImplementationName') getImplementationNameAction: string;
       @Action('importImplementationName') importImplementationNameAction:(implementationName: string) => void;
       @Action('importAgencyLogo') importAgencyLogoAction: any;
       @Action('importProductLogo') importProductLogoAction: any;
       ImplementationID: string = '';
          
 onSaveImplementationName(){
    this.importImplementationNameAction(this.ImplementationID);
  }
  onUploadImplementationLogo(){
      document.getElementById("implementationImageUpload")?.click();
   }

   onUploadAgencyLogo(){
      document.getElementById("agencyImageUpload")?.click();
   }
   handleImplementationLogoUpload(event: { target: { files: any[]; }; }){
    const file = event.target.files[0];
    this.importProductLogoAction(file);   
  }
  handleAgencyLogoUpload(event: { target: { files: any[]; }; }){
    const file = event.target.files[0];
    this.importAgencyLogoAction(file);
  }
  }
  </script>
  