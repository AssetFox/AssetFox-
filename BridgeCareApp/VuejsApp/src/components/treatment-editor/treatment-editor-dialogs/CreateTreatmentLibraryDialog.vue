<template>
    <v-row>
        <v-dialog max-width="450px" persistent v-bind:show="dialogData.showDialog">
            <v-card class="ghd-padding">
                <v-card-title>
                    <v-row justify-left>
                        <h3 class="ghd-title">Create New Treatment Library</h3>
                    </v-row>
                    <v-btn @click="onSubmit(false)" variant = "flat" class="ghd-close-button">
                        X
                    </v-btn>
                </v-card-title>
                <v-card-text>
                    <v-row column>
                        <v-subheader class="ghd-control-label ghd-md-gray">Name</v-subheader>
                        <v-text-field id="CreateTreatmentLibraryDialog-name-textField" class="ghd-control-border ghd-control-text ghd-control-width-lg"
                            outline
                            v-model="newTreatmentLibrary.name"
                        ></v-text-field>
                        <v-subheader class="ghd-control-label ghd-md-gray">Description</v-subheader>
                        <v-textarea id="CreateTreatmentLibraryDialog-desc-textareaField" class="ghd-control-border ghd-control-text ghd-control-width-lg"
                            no-resize
                            outline
                            rows="3"
                            v-model="newTreatmentLibrary.description"
                        >
                        </v-textarea>
                    </v-row>
                </v-card-text>
                <v-card-actions>
                    <v-row row justify-center>
                        <v-btn outline @click="onSubmit(false)" class="ghd-white-bg ghd-blue ghd-button-text" variant = "flat"
                            >Cancel</v-btn
                        >
                        <v-btn
                            id="CreateTreatmentLibraryDialog-addLibrary-btn"
                            :disabled="newTreatmentLibrary.name === ''"
                            @click="onSubmit(true)"
                            class="ghd-white-bg ghd-blue ghd-button-text ghd-blue-border ghd-text-padding"
                        >
                            Save
                        </v-btn>                        
                    </v-row>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-row>
</template>

<script lang="ts" setup>
import Vue from 'vue';
import { inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { CreateTreatmentLibraryDialogData } from '@/shared/models/modals/create-treatment-library-dialog-data';
import {
    emptyTreatmentLibrary,
    Treatment,
    TreatmentConsequence,
    TreatmentCost,
    TreatmentLibrary,
} from '@/shared/models/iAM/treatment';
import { getUserName } from '@/shared/utils/get-user-info';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';

    const props = defineProps<{dialogData: CreateTreatmentLibraryDialogData}>()
    const dialogData = props.dialogData;
    const emit = defineEmits(['submit'])
    let store = useStore();

    async function getIdByUserNameGetter(payload?: any): Promise<any> {
        await store.dispatch('getIdByUserName');
    }

    let newTreatmentLibrary: TreatmentLibrary = {
        ...emptyTreatmentLibrary,
        id: getNewGuid(),
    };

    
  watch(() => props.dialogData, () => onDialogDataChanged)
  async function onDialogDataChanged() {
        let currentUser: string = getUserName();

        newTreatmentLibrary = {
            ...newTreatmentLibrary,
            treatments: props.dialogData.selectedTreatmentLibraryTreatments.map(
                (treatment: Treatment) => ({
                    ...treatment,
                    id: getNewGuid(),
                    costs: treatment.costs.map((cost: TreatmentCost) => {
                        cost.id = getNewGuid();
                        return cost;
                    }),
                    consequences: treatment.consequences.map(
                        (consequence: TreatmentConsequence) => {
                            consequence.id = getNewGuid();
                            return consequence;
                        },
                    ),
                }),
            ),
            owner: await getIdByUserNameGetter(currentUser),
        };
    }

    /**
     * Emits the newTreatmentLibrary object or a null value to the parent component and resets the
     * newTreatmentLibrary object
     * @param submit Whether or not to emit the newTreatmentLibrary object
     */
   function onSubmit(submit: boolean) {
        if (submit) {
            emit('submit', newTreatmentLibrary);
        } else {
            emit('submit');
        }

        newTreatmentLibrary = {
            ...emptyTreatmentLibrary,
            id: getNewGuid(),
        };
    }
</script>
