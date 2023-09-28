<template>
    <v-dialog v-model="dialogData.showDialog" max-width="450px" persistent>
        <v-card>
            <v-card-title class="ghd-dialog-box-padding-top">
                <v-layout justify-space-between align-center >
                    <div class="ghd-control-dialog-header">New Calculated Attribute Library</div>
                    <v-btn @click="onSubmit(false)" variant = "flat" class="ghd-close-button">
                        X
                    </v-btn>
                </v-layout>
            </v-card-title>
            <v-card-text class="ghd-dialog-box-padding-center">
                <v-layout column>
                    <v-subheader class="ghd-md-gray ghd-control-label">Name</v-subheader>
                    <v-text-field
                        v-model="newCalculatedAttributeLibrary.name"
                        :rules="rules['generalRules'].valueIsNotEmpty"
                        outline
                        class="ghd-text-field-border ghd-text-field"/>
                    <v-subheader class="ghd-md-gray ghd-control-label">Description</v-subheader>
                    <v-textarea
                        v-model="newCalculatedAttributeLibrary.description"
                        no-resize
                      outline
                        rows="3"
                        class="ghd-text-field-border"/>
                </v-layout>
            </v-card-text>
            <v-card-actions class="ghd-dialog-box-padding-bottom">
                <v-layout justify-center>
                    <v-btn
                    variant = "outline" 
                        class='ghd-blue ghd-button-text ghd-button'
                        @click="onSubmit(false)">
                        Cancel
                    </v-btn>
                    <v-btn
                        :disabled="newCalculatedAttributeLibrary.name === ''"
                        variant = "outline" 
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                        @click="onSubmit(true)">
                        Save
                    </v-btn> 
                </v-layout>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts" setup>
import Vue from 'vue';
import {
    InputValidationRules,
    rules as validationRules,
} from '@/shared/utils/input-validation-rules';
import { clone } from 'ramda';
import { getNewGuid } from '@/shared/utils/uuid-utils';
import { CreateCalculatedAttributeLibraryDialogData, emptyCreateCalculatedAttributeLibraryDialogData } from '@/shared/models/modals/create-calculated-attribute-library-dialog-data';
import {
    CalculatedAttributeLibrary,
    emptyCalculatedAttributeLibrary,
} from '@/shared/models/iAM/calculated-attribute';
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';

const emit = defineEmits(['submit'])
const props = defineProps<{

    dialogData: CreateCalculatedAttributeLibraryDialogData

}>()
let newCalculatedAttributeLibrary: CalculatedAttributeLibrary = {
        ...emptyCalculatedAttributeLibrary,
        id: getNewGuid(),
    };
    let rules: InputValidationRules = validationRules;
    
    watch(()=>props.dialogData,() => onDialogDataChanged)
    function onDialogDataChanged() {
        newCalculatedAttributeLibrary = {
            ...newCalculatedAttributeLibrary,
            calculatedAttributes: props.dialogData.calculatedAttributes,
        };
    }
    function onSubmit(submit: boolean) {
        if (submit) {
            emit('submit', newCalculatedAttributeLibrary);
        } else {
            emit('submit', null);
        }

        newCalculatedAttributeLibrary = {
            ...emptyCalculatedAttributeLibrary,
            id: getNewGuid(),
        };
    }
    

</script>
