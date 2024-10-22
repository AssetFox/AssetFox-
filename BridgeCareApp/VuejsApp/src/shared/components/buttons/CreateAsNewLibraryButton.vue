<template>
    <v-btn
        id="CreateAsNewLibraryButton-btn"
        @click="onCreateAsNewLibrary"
        :disabled="disabled"
        variant="outlined"
        style="margin-left: 10px; margin-right: 10px"
        class="ghd-blue ghd-button-text ghd-outline-button-padding ghd-button"
    >
        Create as New Library
    </v-btn>
</template>
  
<script setup>
const props = defineProps({
disabled: {
    type: Boolean,
    default: false,
}
});

const emit = defineEmits(['createAsNewLibrary']);

const onCreateAsNewLibrary = () => {
emit('createAsNewLibrary');
};
</script>

<style scoped>
/* Add any component-specific styles here */
</style>
  