<template>
    <v-dialog :model-value="modelValue" max-width="400px" @update:modelValue="updateModelValue">
        <v-card>
            <v-card-text class="text-center">
                {{ message }}
            </v-card-text>
            <v-card-actions>
                <v-row justify="center" class="w-100">
                    <OkButton @ok="closeDialog" />
                </v-row>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script setup>
import OkButton from '../buttons/OkButton.vue';

const props = defineProps({
    modelValue: {
        type: Boolean,
        required: true,
    },
    message: {
        type: String,
        required: true,
    },
});

const emit = defineEmits(['update:modelValue']);

const updateModelValue = (value) => {
    emit('update:modelValue', value);
};

const closeDialog = () => {
    emit('update:modelValue', false);
};
</script>
