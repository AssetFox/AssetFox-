<template>
    <v-btn
      v-if="show"
      id="CreateNewLibraryButton-btn"
      @click="onCreateNewLibrary"
      variant="outlined"
      class="ghd-blue ghd-button-text ghd-outline-button-padding ghd-button"
    >
      Create New Library
    </v-btn>
  </template>
  
  <script setup>
  const props = defineProps({
    show: {
      type: Boolean,
      default: true, 
    },
  });
  
  const emit = defineEmits(['createNewLibrary']);
  
  const onCreateNewLibrary = () => {
    emit('createNewLibrary'); 
  };
  </script>
  
  <style scoped>
  /* Add any component-specific styles here */
  </style>
  