<template>
    <button
    style="margin-top: 10px;"
        :class="['custom-icon-button', { 'is-disabled': disabled, 'ghd-red': !disabled }]"
        @click="onClick"
        :disabled="disabled"
        :style="{ '--button-size': `${size}px`, '--icon-size': `${iconSize}px` }"
    >
        <TrashCanSvg :width="iconSize" :height="iconSize"/>
    </button>
</template>

<script setup>
import { defineProps, defineEmits } from 'vue'
import TrashCanSvg from '@/shared/icons/TrashCanSvg.vue';

const props = defineProps({
    disabled: {
        type: Boolean,
        default: false
    },
    size: {
        type: [String, Number],
        default: 40 
    },
    iconSize: {
        type: [String, Number],
        default: 26 
    }
})

const emit = defineEmits(['click'])

const onClick = (event) => {
    if (!props.disabled) {
        emit('click', event)
    }
}
</script>

<style scoped>
.custom-icon-button {
    background-color: transparent;
    border: none;
    width: var(--button-size); 
    height: var(--button-size);
    padding: 0;
    border-radius: 50%;
    cursor: pointer;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    transition: background-color 0s;
    vertical-align: -7px
}

.custom-icon-button:hover {
    background-color: rgba(217, 83, 79, 0.2); 
}

.custom-icon-button.ghd-red svg {
    --svg-color: #D9534F;
    color: var(--svg-color);
    width: var(--icon-size);
    height: var(--icon-size);
}

.custom-icon-button.is-disabled {
    cursor: not-allowed;
    pointer-events: none;
}

.custom-icon-button.is-disabled svg {
    --svg-color: #B0B0B0; 
    color: var(--svg-color);
}

.custom-icon-button.is-disabled:hover {
    background-color: transparent; 
}
</style>
