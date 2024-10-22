<template>
    <v-btn
        v-if="show"
        id="UpdateLibraryButton-btn"
        @click="onUpdateLibrary"
        :disabled="disabled"
        class="ghd-blue-bg text-white ghd-button-text ghd-outline-button-padding ghd-button"
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
  