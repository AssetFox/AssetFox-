<template>
    <v-layout column justify-start class="Montserrat-font-family pa-2">
        <!-- <v-flex xs6> -->
            <v-layout align-center class="vl-style" row>
                <v-layout column>
                    <v-subheader class="ghd-control-label ghd-md-gray">Data Source</v-subheader>
                    <v-select
                      class="ghd-select ghd-text-field ghd-text-field-border ds-style"
                      outline
                      outlined
                    >
                    </v-select>
                </v-layout>
                <v-btn class="ghd-white-bg ghd-blue" outline>Add Data Source</v-btn>
            </v-layout>
        <!-- </v-flex> -->
        <v-layout column>
            <v-subheader class="ghd-control-label ghd-md-gray">Source Type</v-subheader>
            <v-select
              class="ghd-select ghd-text-field ghd-text-field-border ds-style"
              outline
              outlined
            >
            </v-select>
        </v-layout>
        <v-layout column>
            <v-subheader class="ghd-control-label ghd-md-gray">Connection String</v-subheader>
            <v-layout justify-start>
                    <v-flex xs6>
                        <v-textarea
                          label="Description"
                          no-resize
                          outline
                        >
                        </v-textarea>
                    </v-flex>
            </v-layout>
        </v-layout>
        <v-layout justify-center> 
            <v-flex xs6>
                <v-btn class="ghd-white-bg ghd-blue" flat>Cancel</v-btn>
                <v-btn class="ghd-white-bg ghd-blue" outline>Test</v-btn>
                <v-btn class="ghd-white-bg ghd-blue" outline>Save</v-btn>
            </v-flex>
        </v-layout>
    </v-layout>
</template>

<script lang='ts'>
import Vue from 'vue';
import Component from 'vue-class-component';

@Component({

})
export default class DataSource extends Vue {
    
}
</script>

<style>
.ds-style {
    width: 50%;
}
.vl-style {
    width: 800px;
}
</style>
