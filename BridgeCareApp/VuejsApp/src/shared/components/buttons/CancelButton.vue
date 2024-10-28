<template>
    <v-btn
        v-if="show"
        id="CancelButton-btn"
        @click="onCancel"
        :disabled="disabled"
        variant="outlined"
        style="margin-right: 10px;"
        class="ghd-white-bg ghd-blue ghd-button-text ghd-button ghd-outline-button-padding"
    >
     Cancel
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
        default: true
    },
});

const emit = defineEmits(['cancel']);

const onCancel = () => {
    emit('cancel');
};
</script>

<style scoped>
/* Add any component-specific styles here */
</style>
  