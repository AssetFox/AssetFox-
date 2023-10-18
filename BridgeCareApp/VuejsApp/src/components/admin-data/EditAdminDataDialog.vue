<template>
    <v-dialog max-width="600px" persistent v-bind:show="DialogData.showDialog">
        <v-card>
            <v-card-title class="ghd-dialog-box-padding-top">
            <v-row justify-space-between align-center>
                <div class="ghd-control-dialog-header">Change {{DialogData.settingName}}</div>
            </v-row>
            </v-card-title>
            <v-card-text class="ghd-dialog-box-padding-center">
                <v-row>
                    <v-select :items='settingItems'
                    variant="outlined"
                    v-model='settingSelectItemValue'                         
                    class="ghd-select ghd-text-field ghd-text-field-border">
                    </v-select>   
                    <v-btn style="margin-top: 2px !important; margin-left: 10px !important"
                    class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined"
                    @click="onAddClick"
                    :disabled='isAddDisabled()'>
                        Add
                    </v-btn>
                </v-row>
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
            <v-row justify-center row>
                <v-btn @click="onSubmit(false)" variant = "flat" class='ghd-blue ghd-button-text ghd-button'>
                Cancel
                </v-btn >
                <v-btn @click="onSubmit(true)" variant = "outlined" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'>
                Save
                </v-btn>         
            </v-row>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts" setup>
import Vue, { shallowRef } from 'vue';
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import { EditAdminDataDialogData, emptyEditAdminDataDialogData } from '@/shared/models/modals/edit-data-dialog-data';
import { clone, empty, isNil } from 'ramda';
import { SelectItem } from '@/shared/models/vue/select-item';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
import Dialog from 'primevue/dialog';

    let InputRules: InputValidationRules = rules;
    let DialogData: EditAdminDataDialogData = emptyEditAdminDataDialogData ;
    
    let selectedSettings: {value:string, networkType:string}[] = [];
    let settingsList= shallowRef<string[]>([]);
    let settingItems: SelectItem[] = [];
    const emit = defineEmits(['submit'])
    let settingSelectItemValue: string | null = null;
    let primarySuffix: string = "(P)"
    let rawDataSuffix: string = "(R)"
    let primaryType: string  = "PRIMARY"
    let rawType: string = "RAW"

    //watchers
    watch(DialogData, () => onDialogDataChanged)
    function onDialogDataChanged() {
        selectedSettings = DialogData.selectedSettings.map(_ => {
            let toReturn: {value: string, networkType: string} 
            let type = "";
            let value = "";
            const suffix = _.substring(_.length - 3);
            if(DialogData.settingName == "InventoryReports"){
                if(suffix === primarySuffix){
                    type = primarySuffix;
                    value = _.substring(0, _.length - 3);
                }
                else if(suffix === rawDataSuffix){
                    type = rawDataSuffix;
                    value = _.substring(0, _.length - 3);
                }
                else{
                    value = _;
                    type = primarySuffix;
                }
            }
            else
                value = _;

            toReturn = {value: value, networkType: type};
            return toReturn
        });
        settingsList.value = clone(DialogData.settingsList);
        settingSelectItemValue = null;
    }
    watch(settingsList,() => onSettingsListChanged)
    function onSettingsListChanged(){
        settingItems = settingsList.value.map(_ => {
            return {text: _, value: _}
        });
    }

    function onDeleteSettingClick(setting:any){
        selectedSettings = selectedSettings.filter(_ => _.value !== setting.value);
    }
    function onAddClick(){
        if(!isNil(settingSelectItemValue)){
            selectedSettings.push({value: settingSelectItemValue, 
            networkType: DialogData.settingName === "InventoryReports" ? primarySuffix : ""})
        }            
    }

    function isAddDisabled(){
        if(!isNil(settingSelectItemValue)){
            return !isNil(selectedSettings.find(_ => _.value === settingSelectItemValue!));
        }
        return true;
    }

    function onSubmit(submit: boolean) {
        if (submit) {
        emit('submit', selectedSettings.map(_ => _.value + _.networkType));
        } else {
        emit('submit', null);
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