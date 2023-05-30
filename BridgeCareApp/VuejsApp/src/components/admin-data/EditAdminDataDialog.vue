<template>
    <v-dialog max-width="450px" persistent v-model="DialogData.showDialog">
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
                            <v-list-tile-title v-text="setting"></v-list-tile-title>
                        </v-list-tile-content>
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
                <v-btn  @click="onSubmit(true)" outline class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'>
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

    selectedSettings: string[] = [];
    settingsList: string[] = [];
    settingItems: SelectItem[] = [];
    settingSelectItemValue: string | null = null;

    @Watch('DialogData', {deep:true})
    onDialogDataChanged(){
        this.selectedSettings = clone(this.DialogData.selectedSettings);
        this.settingsList = clone(this.DialogData.settingsList);
        this.settingSelectItemValue = null;
    }

    @Watch('settingsList')
    onSettingsListChanged(){
        this.settingItems = this.settingsList.map(_ => {
            return {text: _, value: _}
        });
    }

    onDeleteSettingClick(setting:string){
        this.selectedSettings = this.selectedSettings.filter(_ => _ !== setting);
    }

    onAddClick(){
        if(!isNil(this.settingSelectItemValue)){
            this.selectedSettings.push(this.settingSelectItemValue)
        }            
    }

    isAddDisabled(){
        if(!isNil(this.settingSelectItemValue)){
            return this.selectedSettings.includes(this.settingSelectItemValue);
        }
        return true;
    }

    onSubmit(submit: boolean) {
        if (submit) {
        this.$emit('submit', this.selectedSettings);
        } else {
        this.$emit('submit', null);
        }
    }
}
</script>