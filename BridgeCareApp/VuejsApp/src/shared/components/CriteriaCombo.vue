<template>
    <v-combobox :items="items"></v-combobox>
</template>
<script lang="ts">

import Vue from 'vue';
import Component from 'vue-class-component';
import { Prop, Watch } from 'vue-property-decorator';
import { watch } from 'fs';
@Component
export default class CriteriaCombo extends Vue {
    @Prop() value: string;

    mounted() {
        if (this.value===null) this.value="temp1";
    }
    data() {
        return {
            select: 'Item1',
            items: [
            'Item1',
            'Item2',
            'Item3',
            this.value
            ]
        }
    }

    @Watch('value')
    onValuesChanged() {
        console.log(this.value);
    }
}
</script>
<style scoped>

</style>