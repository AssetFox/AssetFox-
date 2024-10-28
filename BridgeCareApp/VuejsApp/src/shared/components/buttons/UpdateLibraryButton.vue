<template>
    <v-btn
        v-if="show"
        id="UpdateLibraryButton-btn"
        @click="onUpdateLibrary"
        :disabled="disabled"
        variant="outlined"
        style="margin-left: 10px;"
        class="ghd-blue-bg ghd-white ghd-button-text ghd-button ghd-outline-button-padding"
    >
        Update Library
    </v-btn>
</template>
  
<script setup>
const props = defineProps({
    disabled: {
        type: Boolean,
        default: false,
    },
    show: {
        type: Boolean,
        default: true, 
    },
});

const emit = defineEmits(['updateLibrary']);

const onUpdateLibrary = () => {
    emit('updateLibrary');
};
</script>
  
<style scoped>
/* Add any component-specific styles here */
</style>
  