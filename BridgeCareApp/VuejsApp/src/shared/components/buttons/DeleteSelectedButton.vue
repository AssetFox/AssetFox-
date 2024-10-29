<template>
    <v-btn
      id="DeleteSelectedButton-btn"
      @click="onDeleteSelected"
      :disabled="disabled"
      variant="text"
      class="ghd-red ghd-button"
    >
      Delete Selected
    </v-btn>
  </template>
  
  <script setup>
  const props = defineProps({
    disabled: {
      type: Boolean,
      default: false, // Determines whether the button is disabled
    },
  });
  
  const emit = defineEmits(['deleteSelected']);
  
  const onDeleteSelected = () => {
    emit('deleteSelected'); // Emits the event to the parent when the button is clicked
  };
  </script>
  
  <style scoped>
  /* Add any component-specific styles here */
  </style>
  