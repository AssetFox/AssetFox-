<template>
    <v-btn
        id="OkButton-btn"
        @click="onOk"
        variant="outlined"
        style="margin-left: 10px;"
        class="ghd-blue-bg ghd-white ghd-button-text ghd-button ghd-outline-button-padding"
    >
        Ok
    </v-btn>
</template>
  
<script setup>
const emit = defineEmits(['ok']);

const onOk = () => {
    emit('ok');
};
</script>

<style scoped>
/* Add any component-specific styles here */
</style>