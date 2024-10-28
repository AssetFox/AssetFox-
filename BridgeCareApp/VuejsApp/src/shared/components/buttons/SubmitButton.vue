<template>
    <v-btn
        v-if="show"
        id="SubmitButton-btn"
        @click="onSubmit"
        :disabled="disabled"
        variant="outlined"
        style="margin-left: 10px;"
        class="ghd-blue-bg ghd-white ghd-button-text ghd-button ghd-outline-button-padding"
    >
        Submit
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

const emit = defineEmits(['submit']);

const onSubmit = () => {
    emit('submit');
};
</script>

<style scoped>
/* Add any component-specific styles here */
</style>
  