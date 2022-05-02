<template>
    <v-layout>
        <v-dialog max-width="450px" persistent v-model="dialogData.showDialog">
            <v-card>
                <v-card-title>
                    <v-layout justify-center>
                        <h3>New Treatment Library</h3>
                    </v-layout>
                </v-card-title>
                <v-card-text>
                    <v-layout column>
                        <v-text-field
                            label="Name"
                            outline
                            v-model="newTreatmentLibrary.name"
                        ></v-text-field>
                        <v-textarea
                            label="Description"
                            no-resize
                            outline
                            rows="3"
                            v-model="newTreatmentLibrary.description"
                        >
                        </v-textarea>
                    </v-layout>
                </v-card-text>
                <v-card-actions>
                    <v-layout justify-space-between row>
                        <v-btn
                            :disabled="newTreatmentLibrary.name === ''"
                            @click="onSubmit(true)"
                            color="info"
                        >
                            Save
                        </v-btn>
                        <v-btn @click="onSubmit(false)" color="error"
                            >Cancel</v-btn
                        >
                    </v-layout>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-layout>
</template>

<script lang="ts">
import Vue from 'vue';
import {Getter} from 'vuex-class';
import { Component, Prop, Watch } from 'vue-property-decorator';
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

@Component
export default class CreateTreatmentLibraryDialog extends Vue {
    @Prop() dialogData: CreateTreatmentLibraryDialogData;

    @Getter('getIdByUserName') getIdByUserNameGetter: any;

    newTreatmentLibrary: TreatmentLibrary = {
        ...emptyTreatmentLibrary,
        id: getNewGuid(),
    };

    @Watch('dialogData')
    onDialogDataChanged() {
        let currentUser: string = getUserName();

        this.newTreatmentLibrary = {
            ...this.newTreatmentLibrary,
            treatments: this.dialogData.selectedTreatmentLibraryTreatments.map(
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
            owner: this.getIdByUserNameGetter(currentUser),
        };
    }

    /**
     * Emits the newTreatmentLibrary object or a null value to the parent component and resets the
     * newTreatmentLibrary object
     * @param submit Whether or not to emit the newTreatmentLibrary object
     */
    onSubmit(submit: boolean) {
        if (submit) {
            this.$emit('submit', this.newTreatmentLibrary);
        } else {
            this.$emit('submit');
        }

        this.newTreatmentLibrary = {
            ...emptyTreatmentLibrary,
            id: getNewGuid(),
        };
    }
}
</script>
