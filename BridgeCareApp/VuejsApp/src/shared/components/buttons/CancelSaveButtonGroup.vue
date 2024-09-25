<template>
    <div class="d-flex">
        <v-btn
            id="CancelSaveButtonGroup-cancel-btn"
            @click="onCancel"
            :disabled="cancelDisabled"
            flat
            class="ghd-white-bg ghd-blue ghd-button-text ghd-button"
        >
        Cancel
        </v-btn>

        <div :style="{ width: customSpacing + 'px' }"></div>

        <v-btn
            id="CancelSaveButtonGroup-save-btn"
            @click="onSave"
            :disabled="saveDisabled"
            flat
            class="ghd-blue-bg ghd-white ghd-button-text ghd-button"
        >
        Save
        </v-btn>
        
    </div>
  </template>
  
<script setup>
    import { computed } from 'vue';

    const props = defineProps({
        spacing: {
            type: Number,
            default: 50, 
        },
        cancelDisabled: {
            type: Boolean,
            default: false
        },
        saveDisabled: {
            type: Boolean, 
            default: false
        }
    });

    const emit = defineEmits(['cancel', 'save']);

    const onCancel = () => {
        emit('cancel');
    };

    const onSave = () => {
        emit('save');
    };

    const customSpacing = computed(() => props.spacing);
</script>

<style scoped>

</style>
  