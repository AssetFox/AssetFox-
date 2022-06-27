<template>
    <v-layout column class="Montserrat-font-family">
        Data Source
        <v-btn @click="onClick">test</v-btn>
        <v-icon x-large color="green darken-2">$vuetify.icons.product</v-icon>
    </v-layout>
</template>

<script lang='ts'>
import Vue from 'vue';
import Component from 'vue-class-component';
import Vuetify from 'vuetify';

@Component({

})
export default class DataSource extends Vue {
    onClick(){
        let foos = this.$vuetify.theme.primary
        
    }
}
</script>

<style>

</style>
