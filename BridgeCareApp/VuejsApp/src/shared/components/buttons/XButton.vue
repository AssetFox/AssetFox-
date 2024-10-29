<template>
<v-btn
    @click="onClick"
    variant="text"  
    icon  
>
    <i class="fas fa-times fa-2x"></i>
</v-btn>
</template>

<script setup>
const props = defineProps({
    
});

const emit = defineEmits(['submit']);

const onClick = () => {
    emit('submit');
};
</script>

<style scoped>
.icon-button {
    padding: 0;
}
</style>
  