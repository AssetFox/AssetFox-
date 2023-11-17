<template>
    <v-dialog v-model="dialogData.showDialog" max-width="450px" persistent>
        <v-card>
            <v-card-title class="ghd-dialog-box-padding-top">
                <v-row justify-space-between align-center >
                    <div class="ghd-control-dialog-header">New Calculated Attribute Library</div>
                    <v-spacer></v-spacer>
                    <v-btn @click="onSubmit(false)" variant = "flat" class="ghd-close-button">
                        X
                    </v-btn>
                </v-row>
            </v-card-title>
            <v-card-text class="ghd-dialog-box-padding-center">
                <v-row>
                    <v-subheader class="ghd-md-gray ghd-control-label">Name</v-subheader>
                </v-row>
                <v-row>
                    <v-text-field id="CreateCalculatedAttributeLibraryDialog-name-textfield"
                        v-model="newCalculatedAttributeLibrary.name"
                        :rules="rules['generalRules'].valueIsNotEmpty"
                        variant="outlined"
                        class="ghd-text-field-border ghd-text-field"/>
                </v-row>
                <v-row>
                    <v-subheader class="ghd-md-gray ghd-control-label">Description</v-subheader>
                </v-row>
                <v-row>
                    <v-textarea id="CreateCalculatedAttributeLibraryDialog-description-textfield"
                        v-model="newCalculatedAttributeLibrary.description"
                        no-resize
                        variant="outlined"
                        rows="3"
                        class="ghd-text-field-border"/>
                </v-row>
            </v-card-text>
            <v-card-actions class="ghd-dialog-box-padding-bottom">
                <v-spacer></v-spacer>
                    <v-btn id="CreateCalculatedAttributeLibraryDialog-cancel-btn"
                    variant = "outlined" 
                        class='ghd-blue ghd-button-text ghd-button'
                        @click="onSubmit(false)">
                        Cancel
                    </v-btn>
                    <v-btn id="CreateCalculatedAttributeLibraryDialog-save-btn"
                        :disabled="newCalculatedAttributeLibrary.name === ''"
                        variant = "outlined" 
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                        @click="onSubmit(true)">
                        Save
                    </v-btn> 
                    <v-spacer></v-spacer>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts" setup>
import Vue, { computed } from 'vue';
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
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref, toRefs} from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';

const emit = defineEmits(['submit'])
const props = defineProps<{

    dialogData: CreateCalculatedAttributeLibraryDialogData

}>()
const {dialogData} = toRefs(props);
const newCalculatedAttributeLibrary = ref<CalculatedAttributeLibrary>({
        ...emptyCalculatedAttributeLibrary,
        id: getNewGuid(),
    })
    let rules: InputValidationRules = validationRules;
    
    watch(dialogData,() =>{
        newCalculatedAttributeLibrary.value = {
            ...newCalculatedAttributeLibrary.value,
            calculatedAttributes: dialogData.value.calculatedAttributes,
        };
    });
    function onSubmit(submit: boolean) {
        if (submit) {
            emit('submit', newCalculatedAttributeLibrary.value);
        } else {
            emit('submit', null);
        }

        newCalculatedAttributeLibrary.value = {
            ...emptyCalculatedAttributeLibrary,
            id: getNewGuid(),
        };
    }
    

</script>
