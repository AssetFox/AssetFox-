<script setup lang="ts">
import { useCurrencyInput } from 'vue-currency-input';
import { watch } from 'vue';

const props = defineProps({
  modelValue: Number,
  errorMessages: {
    type: Array as () => string[],
    default: () => [],
  }
});

const { inputRef, formattedValue, numberValue, setValue } = useCurrencyInput({
  currency: 'USD',
  hideCurrencySymbolOnFocus: false,
  hideGroupingSeparatorOnFocus: false,
  hideNegligibleDecimalDigitsOnFocus: true,
  precision: 2,
  valueRange: { min: -1 },
});

watch(
  () => props.modelValue,
  (value) => {
    setValue(value);
  }
);
</script>

<template>
  <v-text-field
    v-model="formattedValue"
    variant="solo"
    ref="inputRef"
    :error-messages="props.errorMessages"
  >
  </v-text-field>
</template>
