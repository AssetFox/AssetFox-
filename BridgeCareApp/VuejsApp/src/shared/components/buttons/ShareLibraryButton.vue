<template>
    <v-btn
        v-if="show"
        id="ShareLibraryButton-btn"
        @click="onShareLibrary"
        variant="outlined"
        class="ghd-blue ghd-button-text ghd-outline-button-padding ghd-button"
        style="margin-left: 10px;"
    >
        Share Library
    </v-btn>
</template>

<script setup>
const props = defineProps({
show: {
    type: Boolean,
    default: true,
}
});

const emit = defineEmits(['shareLibrary']);

const onShareLibrary = () => {
emit('shareLibrary'); 
};
</script>

<style scoped>
/* Add any component-specific styles here */
</style>
