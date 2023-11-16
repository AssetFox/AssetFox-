<template>
    <v-row>
        <v-dialog content content-class="centered-dialog" persistent v-model="showDialogComputed">
            <v-container>
                <v-row   justify="center">
                    <v-col align="center">
                        <v-progress-circular :size="70" :width="7" class="ara-blue-pantone-281" indeterminate></v-progress-circular>
                        <h1>Please wait</h1>
                    </v-col>
                    
                </v-row>
            </v-container>
        </v-dialog>
    </v-row>
</template>

<script lang="ts" setup>
    import Vue, { computed } from 'vue';
import { useStore } from 'vuex';


let store = useStore();
let showDialogComputed = computed<boolean>(() => store.state.busyModule.isBusy);

</script>

<style>
    .dialog.centered-dialog, .v-dialog.centered-dialog {
        background: grey;
        box-shadow: none;
        border-radius: 6px;
        width: auto;
        color: whitesmoke;
    }
</style>
