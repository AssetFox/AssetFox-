<template>
    <v-row>
        <v-dialog max-width="450px" persistent v-model="dialogData.showDialog">
            <v-card class="ghd-padding">
                    <v-row justify="space-between" style="margin: 10px;">
                        <h3 class="ghd-title">Create New Treatment Library</h3>
                        <v-btn @click="onSubmit(false)" variant = "flat" class="ghd-close-button">
                        X
                    </v-btn>
                    </v-row>
                    <v-col>
                        <v-subheader class="ghd-control-label ghd-md-gray">Name</v-subheader>
                        <v-text-field id="CreateTreatmentLibraryDialog-name-textField" class="ghd-control-border ghd-control-text ghd-control-width-lg"
                            variant="outlined"
                            v-model="newTreatmentLibrary.name"
                        ></v-text-field>
                        <v-subheader class="ghd-control-label ghd-md-gray">Description</v-subheader>
                        <v-textarea id="CreateTreatmentLibraryDialog-desc-textareaField" class="ghd-control-border ghd-control-text ghd-control-width-lg"
                            no-resize
                            variant="outlined"
                            rows="3"
                            v-model="newTreatmentLibrary.description"
                        >
                        </v-textarea>
                    </v-col>
                    <v-row justify="center" style="margin-bottom: 10px;">
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
            </v-card>
        </v-dialog>
    </v-row>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { toRefs, watch, ref} from 'vue';
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

    const props = defineProps<{dialogData: CreateTreatmentLibraryDialogData}>()
    const { dialogData } = toRefs(props);
    const emit = defineEmits(['submit'])
    let store = useStore();

    function getIdByUserNameGetter(payload?: any): Promise<any> {
        store.dispatch('getIdByUserName');
    }

    const newTreatmentLibrary = ref<TreatmentLibrary>({...emptyTreatmentLibrary, id: getNewGuid(),});

    
  watch(dialogData, () => {
        let currentUser: string = getUserName();

        newTreatmentLibrary.value = {
            ...newTreatmentLibrary.value,
            treatments: dialogData.value.selectedTreatmentLibraryTreatments.map(
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
            owner: getIdByUserNameGetter(currentUser),
        };
    });

    /**
     * Emits the newTreatmentLibrary object or a null value to the parent component and resets the
     * newTreatmentLibrary object
     * @param submit Whether or not to emit the newTreatmentLibrary object
     */
   function onSubmit(submit: boolean) {
        if (submit) {
            emit('submit', newTreatmentLibrary.value);
        } else {
            emit('submit');
        }

        newTreatmentLibrary.value = {
            ...emptyTreatmentLibrary,
            id: getNewGuid(),
        };
    }
</script>
