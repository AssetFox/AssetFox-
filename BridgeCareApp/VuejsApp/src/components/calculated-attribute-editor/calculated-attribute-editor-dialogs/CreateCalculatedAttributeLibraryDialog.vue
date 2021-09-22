<template>
    <v-dialog v-model="dialogData.showDialog" max-width="450px" persistent>
        <v-card>
            <v-card-title>
                <v-layout justify-center>
                    <h3>New Calculated Attribute Library</h3>
                </v-layout>
            </v-card-title>
            <v-card-text>
                <v-layout column>
                    <v-text-field
                        v-model="newCalculatedAttributeLibrary.name"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"
                        label="Name"
                        outline
                    />
                    <v-textarea
                        v-model="newCalculatedAttributeLibrary.description"
                        label="Description"
                        no-resize
                        outline
                        rows="3"
                    />
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn
                        :disabled="newCalculatedAttributeLibrary.name === ''"
                        class="ara-blue-bg white--text"
                        @click="onSubmit(true)"
                    >
                        Save
                    </v-btn>
                    <v-btn
                        class="ara-orange-bg white--text"
                        @click="onSubmit(false)"
                    >
                        Cancel
                    </v-btn>
                </v-layout>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts">
import Vue from 'vue';
import { Component, Prop, Watch } from 'vue-property-decorator';
import {
    InputValidationRules,
    rules,
} from '@/shared/utils/input-validation-rules';
import { clone } from 'ramda';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { CreateCalculatedAttributeLibraryDialogData } from '@/shared/models/modals/create-calculated-attribute-library-dialog-data';
import {
    CalculatedAttribute,
    CalculatedAttributeLibrary,
    CriterionAndEquationSet,
    emptyCalculatedAttributeLibrary,
    Timing,
} from '@/shared/models/iAM/calculated-attribute';
import { getUserName } from '@/shared/utils/get-user-info';

@Component
export default class CreateCalculatedAttributeLibraryDialog extends Vue {
    @Prop() dialogData: CreateCalculatedAttributeLibraryDialogData;

    newCalculatedAttributeLibrary: CalculatedAttributeLibrary = {
        ...emptyCalculatedAttributeLibrary,
        id: getNewGuid(),
    };
    rules: InputValidationRules = clone(rules);

    @Watch('dialogData')
    onDialogDataChanged() {
        this.newCalculatedAttributeLibrary = {
            ...this.newCalculatedAttributeLibrary,
            calculatedAttributes: this.dialogData.calculatedAttributes,
        };
    }
    onSubmit(submit: boolean) {
        if (submit) {
            this.$emit('submit', this.newCalculatedAttributeLibrary);
        } else {
            this.$emit('submit', null);
        }

        this.newCalculatedAttributeLibrary = {
            ...emptyCalculatedAttributeLibrary,
            id: getNewGuid(),
        };
    }
}
</script>
