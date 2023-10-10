<template>
    <v-row>
        <v-dialog content content-class="centered-dialog" persistent v-model="loading">
            <v-container>
                <v-row align-center column justify-center>
                    <v-progress-circular :size="70" :width="7" class="ara-blue-pantone-281"
                                         indeterminate></v-progress-circular>
                    <h1>Please wait</h1>
                </v-row>
            </v-container>
        </v-dialog>
    </v-row>
</template>

<script lang="ts" setup>
    import Vue from 'vue';
    import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';

let store = useStore();
let loading = ref<boolean>(store.state.busyModule.isBusy);

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
