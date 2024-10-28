<template>
    <v-btn
      v-if="show"
      id="DeleteLibraryButton-btn"
      @click="onDeleteLibrary"
      :disabled="disabled"
      variant="outlined"
      style="margin-right: 10px;"
      class="ghd-red ghd-button-text ghd-outline-button-padding ghd-button"
    >
      Delete Library
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

const emit = defineEmits(['deleteLibrary']);

const onDeleteLibrary = () => {
    emit('deleteLibrary');
};
</script>
  
<style scoped>
/* Add any component-specific styles here */
</style>
  