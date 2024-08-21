<template>
    <v-dialog max-width="600px" persistent v-model="DialogData.showDialog">
        <v-card>
            <v-card-title class="ghd-dialog-box-padding-top">
            <v-row justify="space-between" align="center">
                <div id="EditAdminDataDialog-header-div" class="ghd-control-dialog-header">Change {{DialogData.settingName}}</div>
            </v-row>
            </v-card-title>
            <v-card-text class="ghd-dialog-box-padding-center">
                <v-row v-if="DialogData.settingName != 'AssetType'">
                    <v-select :items='DialogData.settingsList'
                    variant="outlined"
                    item-title="text"
                    item-value="value"
                    menu-icon=custom:GhdDownSvg
                    v-model='DialogData.selectedItem'                         
                    id="EditAdminDataDialog-addNewItems-select"
                    class="ghd-select ghd-text-field ghd-text-field-border"
                    density="compact">
                    </v-select>   
                    <v-btn style="margin-top: 2px !important; margin-left: 10px !important"
                    id="EditAdminDataDialog-addNewItems-btn"
                    class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined"
                    @click="onAddClick"
                    :disabled='isAddDisabled()'>
                        Add
                    </v-btn>
                </v-row>
                <v-row v-else="DialogData.settingName == 'AssetType'">
                    <v-text-field :items='DialogData.settingsList'
                    variant="outlined"
                    item-title="text"
                    item-value="value"
                    menu-icon=custom:GhdDownSvg
                    v-model='DialogData.selectedItem'                         
                    id="EditAdminDataDialog-addNewItems-select"
                    class="ghd-select ghd-text-field ghd-text-field-border"
                    density="compact">
                    </v-text-field>   
                    <v-btn style="margin-top: 2px !important; margin-left: 10px !important"
                    id="EditAdminDataDialog-addNewItems-btn"
                    class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined"
                    @click="onAddClick"
                    :disabled='isAddDisabled()'>
                        Add
                    </v-btn>
                </v-row>
                
                <v-list style="overflow: hidden; max-height: 100%; ">
                    <v-list-tile
                    v-for="setting in DialogData.AddedItems"
                    :key="setting" >
                    <v-row justify="end">
                        <v-col cols = "21" >
                        <v-list-tile-content >
                            <v-list-tile-title id="EditAdminDataDialog-itemName-title" v-text="setting.value"></v-list-tile-title>
                        </v-list-tile-content>
                    </v-col>
                    <v-col>
                        <input type ="radio" v-if="DialogData.settingName == 'InventoryReports'" v-model="setting.networkType" value ="(R)"/>
                        <label v-if="DialogData.settingName == 'InventoryReports'" style="margin-right: 10px;">RAW</label>
                        <input type ="radio" id="EditAdminDataDialog-primary-radio" v-if="DialogData.settingName == 'InventoryReports'" v-model="setting.networkType" value ="(P)"/>
                        <label v-if="DialogData.settingName == 'InventoryReports'">PRIMARY</label>
                    </v-col>
                        <v-btn @click="onDeleteSettingClick(setting)"  class="ghd-blue" flat>
                            <img class='img-general' :src="getUrl('assets/icons/trash-ghd-blue.svg')"/>
                        </v-btn>
                    
                        
                    </v-row>
                    </v-list-tile>
                </v-list>
            </v-card-text>
            <v-card-actions class="ghd-dialog-box-padding-bottom">
            <v-row justify="center" row>
                <v-btn id="EditAdminDataDialog-cancel-btn" @click="onSubmit(false)" variant = "flat" class='ghd-blue ghd-button-text ghd-button'>
                Cancel
                </v-btn >
                <v-btn id="EditAdminDataDialog-save-btn" @click="onSubmit(true)" variant = "outlined" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'>
                Save
                </v-btn>         
            </v-row>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts" setup>
import Vue, { shallowRef, toRefs } from 'vue';
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import { EditAdminDataDialogData, emptyEditAdminDataDialogData } from '@/shared/models/modals/edit-data-dialog-data';
import { clone, empty, isNil } from 'ramda';
import { SelectItem } from '@/shared/models/vue/select-item';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
import { AdminSelectItem } from '@/shared/models/vue/admin-select-item';
import Dialog from 'primevue/dialog';
import { getUrl } from '@/shared/utils/get-url';

    let InputRules: InputValidationRules = rules;
     const props = defineProps<{
        DialogData: EditAdminDataDialogData
     }>()
    
    const selectedSettings=  ref<AdminSelectItem[]>([]);
    const settingsList= ref<string[]>([]);
    const settingItems=ref<SelectItem[]>([]);
    const emit = defineEmits(['submit'])
    const settingSelectItemValue =  ref<string>('');
    const networkTypeSelected = ref<string>('')
    let primarySuffix: string = "(P)"
    let rawDataSuffix: string = "(R)"
    let primaryType: string  = "PRIMARY"
    let rawType: string = "RAW"
    const { DialogData } = toRefs(props);
    //watchers
    watch(DialogData, () =>  {
        
        
       DialogData.value.AddedItems = DialogData.value.selectedSettings.map(_ => {
            let toReturn: {value: string, networkType: string} 
            let type = "";
            let value = "";
            
            const suffix = _.substring(_.length - 3);
            if(DialogData.value.settingName == "InventoryReports"){
                if(suffix == primarySuffix){
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
        settingsList.value = clone(DialogData.value.settingsList);
        
    })
    watch(settingsList,() => {
        settingItems.value = settingsList.value.map(_ => {
            return {text: _, value: _}
        });
    })
    
    function onDeleteSettingClick(setting:any){
        DialogData.value.AddedItems = DialogData.value.AddedItems.filter(_ => _.value !== setting.value);
    }
    function onAddClick(){
        if(!isNil(DialogData.value.selectedItem)){
            //DialogData.value.selectedSettings.push(settingSelectItemValue.value,DialogData.value.settingName === "InventoryReports" ? primarySuffix : "")
            DialogData.value.AddedItems.push({value: DialogData.value.selectedItem, 
            networkType: DialogData.value.settingName === "InventoryReports" ? primarySuffix : ""})
        }
        DialogData.value.selectedItem = '';            
    }

    function isAddDisabled(){
        if(!isNil(DialogData.value.selectedItem)){
            return !isNil(DialogData.value.AddedItems.find(_ => _.value === DialogData.value.selectedItem!));
        }
        return true;
    }

    function onSubmit(submit: boolean) {
        if (submit) {
        emit('submit', DialogData.value.AddedItems.map(_ => _.value + _.networkType));
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