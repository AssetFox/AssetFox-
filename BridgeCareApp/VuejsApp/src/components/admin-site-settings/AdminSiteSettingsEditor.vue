
<template>
    <v-layout column class="Montserrat-font-family ma-0" style="width: 25%; padding-left: 50px;">
         <v-layout align-center class="vl-style">
             <v-flex xs12>
             <v-layout column>
                 <v-subheader class="ghd-control-label ghd-md-gray Montserrat-font-family">Implementation Name</v-subheader>
               <v-text-field
                 class="ghd-select ghd-text-field ghd-text-field-border Montserrat-font-family search-icon-general"
                 v-model="implementationID"
                 type="text"
                 hide-details
                 clearable
                 single-line
                 outline      
                >
                </v-text-field>
             </v-layout>
             </v-flex>
             <v-btn class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' style="margin-top: 6%;" @click="onSaveImplementationName" outline>Save</v-btn>
         </v-layout>

         <v-layout align-center class="vl-style" style="margin-top: 5%;">
             <v-flex xs12>
             <v-layout column>
                 <v-subheader class="ghd-control-label ghd-md-gray Montserrat-font-family">Agency Logo </v-subheader>
             </v-layout>
             </v-flex>
             <input id="agencyImageUpload" type="file" accept="image/*" ref="agencyFileInput" @change="handleAgencyLogoUpload" hidden>
             <v-btn class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' style="margin-right: 40%;" @click="onUploadAgencyLogo" outline>Upload</v-btn>
         </v-layout>

         <v-layout align-center class="vl-style">
             <v-flex xs12>
             <v-layout column>
                 <v-subheader class="ghd-control-label ghd-md-gray Montserrat-font-family">Implementation Logo </v-subheader>
             </v-layout>
             </v-flex>
             <input id="implementationImageUpload" type="file" ref="implementationFileInput" accept="image/*" @change="handleImplementationLogoUpload" hidden>
             <v-btn class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' style="margin-right: 40%;" @click="onUploadImplementationLogo" outline>Upload</v-btn>
         </v-layout>
         
     </v-layout>
</template>

<script lang="ts">
import axios from 'axios';
import Vue from 'vue';
import { Component, Watch} from 'vue-property-decorator';
import { Action, State } from 'vuex-class';
import {clone, find, groupBy, propEq, uniq} from 'ramda';
import Vuex from 'vuex'
import AdminSiteSettingsService from '@/services/admin-site-settings.service';
import { hasValue } from '@/shared/utils/has-value-util';

@Component
export default class AdminSiteSettingsEditor extends Vue{

    @State(state => state.siteAdminModule.implementationName) implementationName: string;
    @State(state => state.siteAdminModule.agencyLogo) agencyLogo: string;
    @State(state => state.siteAdminModule.implementationLogo) implementationLogo: string;

     @Action('getImplementationName') getImplemetnationNameAction: string;
     @Action('setImplementationName') setImplemetnationNameAction: any; 
     @Action('setAgencyLogo') setAgencyLogoAction: any;
     @Action('setImplementationLogo') setImplementationLogoAction: any;

     ImplementationID: String = '';
     PerformanceCurvesFile: File | null;


     @Watch('implementationName')
        onImplementationLoad() {
            this.ImplementationID = this.implementationName
        }
           

 onSaveImplementationName(){
        AdminSiteSettingsService.importImplementationName(this.ImplementationID);
 }
 
 handleImplementationLogoUpload(event: { target: { files: any[]; }; }){
    const file = event.target.files[0];
    AdminSiteSettingsService.importProductLogo(file);
 }

 handleAgencyLogoUpload(event: { target: { files: any[]; }; }){
    const file = event.target.files[0];
    AdminSiteSettingsService.importAgencyLogo(file);
 }

}
</script>



