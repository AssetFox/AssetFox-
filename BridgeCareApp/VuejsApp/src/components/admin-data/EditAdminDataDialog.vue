<template>
    <v-dialog max-width="600px" persistent v-model="DialogData.showDialog">
        <v-card>
            <v-card-title class="ghd-dialog-box-padding-top">
            <v-layout justify-space-between align-center>
                <div class="ghd-control-dialog-header">Change {{DialogData.settingName}}</div>
            </v-layout>
            </v-card-title>
            <v-card-text class="ghd-dialog-box-padding-center">
                <v-layout>
                    <v-select :items='settingItems'
                    outline  
                    v-model='settingSelectItemValue'                         
                    class="ghd-select ghd-text-field ghd-text-field-border">
                    </v-select>   
                    <v-btn style="margin-top: 2px !important; margin-left: 10px !important"
                    class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline
                    @click="onAddClick"
                    :disabled='isAddDisabled()'>
                        Add
                    </v-btn>
                </v-layout>
                <v-list>
                    <v-list-tile
                    v-for="setting in selectedSettings"
                    :key="setting">
                        <v-list-tile-content >
                            <v-list-tile-title v-text="setting.value"></v-list-tile-title>
                        </v-list-tile-content>
                        <v-radio-group v-if="DialogData.settingName == 'InventoryReports'" class="admin-radio" v-model="setting.networkType" row>
                            <v-radio  label="RAW" value="(R)"></v-radio>
                            <v-radio  label="PRIMARY" value="(P)"></v-radio>
                        </v-radio-group>
                        <v-btn @click="onDeleteSettingClick(setting)"  class="ghd-blue" icon>
                            <img class='img-general' :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
                        </v-btn>
                    </v-list-tile>
                </v-list>
            </v-card-text>
            <v-card-actions class="ghd-dialog-box-padding-bottom">
            <v-layout justify-center row>
                <v-btn @click="onSubmit(false)" flat class='ghd-blue ghd-button-text ghd-button'>
                Cancel
                </v-btn >
                <v-btn @click="onSubmit(true)" outline class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'>
                Save
                </v-btn>         
            </v-layout>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts">
import Vue from 'vue';
import {Component, Prop, Watch} from 'vue-property-decorator';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import { EditAdminDataDialogData } from '@/shared/models/modals/edit-data-dialog-data';
import { clone, isNil } from 'ramda';
import { SelectItem } from '@/shared/models/vue/select-item';

@Component
export default class EditAdminDataDialog extends Vue {
    @Prop() DialogData: EditAdminDataDialogData;
        
    rules: InputValidationRules = rules;

    selectedSettings: {value:string, networkType:string}[] = [];
    settingsList: string[] = [];
    settingItems: SelectItem[] = [];
    settingSelectItemValue: string | null = null;
    primarySuffix: string = "(P)"
    rawDataSuffix: string = "(R)"
    primaryType: string  = "PRIMARY"
    rawType: string = "RAW"

    @Watch('DialogData', {deep:true})
    onDialogDataChanged(){
        this.selectedSettings = this.DialogData.selectedSettings.map(_ => {
            let toReturn: {value: string, networkType: string} 
            let type = "";
            let value = "";
            const suffix = _.substring(_.length - 3);
            if(this.DialogData.settingName == "InventoryReports"){
                if(suffix === this.primarySuffix){
                    type = this.primarySuffix;
                    value = _.substring(0, _.length - 3);
                }
                else if(suffix === this.rawDataSuffix){
                    type = this.rawDataSuffix;
                    value = _.substring(0, _.length - 3);
                }
                else{
                    value = _;
                    type = this.primarySuffix;
                }
            }
            else
                value = _;

            toReturn = {value: value, networkType: type};
                
            return toReturn
        });
        this.settingsList = clone(this.DialogData.settingsList);
        this.settingSelectItemValue = null;
    }

    @Watch('settingsList')
    onSettingsListChanged(){
        this.settingItems = this.settingsList.map(_ => {
            return {text: _, value: _}
        });
    }

    onDeleteSettingClick(setting:any){
        this.selectedSettings = this.selectedSettings.filter(_ => _.value !== setting.value);
    }

    onAddClick(){
        if(!isNil(this.settingSelectItemValue)){
            this.selectedSettings.push({value: this.settingSelectItemValue, 
            networkType: this.DialogData.settingName === "InventoryReports" ? this.primarySuffix : ""})
        }            
    }

    isAddDisabled(){
        if(!isNil(this.settingSelectItemValue)){
            return !isNil(this.selectedSettings.find(_ => _.value === this.settingSelectItemValue!));
        }
        return true;
    }

    onSubmit(submit: boolean) {
        if (submit) {
        this.$emit('submit', this.selectedSettings.map(_ => _.value + _.networkType));
        } else {
        this.$emit('submit', null);
        }
    }
}
</script>

<style>
    .v-input--radio-group {
        padding-top: 0 !important;
        margin-top: 0 !important;
    }

    .admin-radio label{
        margin-bottom: 0 !important;
        font-weight: 500 !important;
    }

    .admin-radio .v-input__slot {
        margin-bottom: -15px !important;
    }

</style>