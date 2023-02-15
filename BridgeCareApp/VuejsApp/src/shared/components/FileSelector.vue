<template>
    <v-layout column>
        <v-layout column>        
            <div class="div-border align-start">   
                <div id="app" class="ghd-white-bg" v-cloak @drop.prevent="onSelect($event.dataTransfer.files)" @dragover.prevent>
                    <v-layout fill-height justify-center>
                        <div class="drag-drop-area">
                            <v-layout fill-height align-center justify-center>
                                <img :src="require('@/assets/icons/upload.svg')"/>
                                <v-layout column align-center>
                                    <span class="span-center Montserrat-font-family">Drag & Drop Files Here </span>
                                    <!--<span class="span-center Montserrat-font-family">or</span>
                                    <v-btn class="ghd-blue Montserrat-font-family a-0 ma-0" @click="chooseFiles()" flat> Click here to select files </v-btn>-->
                                </v-layout>
                            </v-layout>
                        </div>
                        </v-layout>
                </div>
            </div>
            <v-flex xs12>
                <v-layout justify-start>     
                    <v-switch
                        v-show="useTreatment"
                        label="No Treatment"
                        class="ghd-control-label ghd-md-gray Montserrat-font-family my-2"
                        v-model="applyNoTreatment"
                    />
                </v-layout>
            </v-flex>
            <div v-show="true">
                <input @change="onSelect($event.target.files)" id="file-select" type="file" hidden />
            </div>
        </v-layout>        
        <div class="files-table">
            <v-data-table :headers="tableHeaders" :items="files" class="elevation-1 fixed-header v-table__overflow Montserrat-font-family"
                        sort-icon=$vuetify.icons.ghd-table-sort
                          hide-actions>
                <template slot="items" slot-scope="props">
                    <td>
                        {{props.item.name}}
                    </td>
                    <td>
                        <div><strong>{{ formatBytesSize(props.item.size) }}</strong></div>
                    </td>
                    <td>
                        <v-btn @click="file = null" class="ghd-blue" icon>
                            <img class='img-general' :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
                        </v-btn>
                    </td>
                </template>
            </v-data-table>
        </div>
    </v-layout>
</template>

<script lang="ts">
import Vue from 'vue';
import {Component, Watch, Prop} from 'vue-property-decorator';
import {Action} from 'vuex-class';
import {hasValue} from '@/shared/utils/has-value-util';
import {getPropertyValues} from '@/shared/utils/getter-utils';
import {clone, prop} from 'ramda';
import {DataTableHeader} from '@/shared/models/vue/data-table-header';
import { formatBytes } from '@/shared/utils/math-utils';

@Component
export default class FileSelector extends Vue {
    @Prop() closed: boolean = false;
    @Prop() useTreatment: boolean = false;
    @Action('addErrorNotification') addErrorNotificationAction: any;
    @Action('setIsBusy') setIsBusyAction: any;

    applyNoTreatment: boolean = true;
    fileSelect: HTMLInputElement = {} as HTMLInputElement;
    tableHeaders: DataTableHeader[] = [
        {text: 'Name', value: 'name', align: 'left', sortable: false, class: '', width: '50%'},
        {text: 'Size', value: 'size', align: 'left', sortable: false, class: '', width: '35%'},
        {text: 'Action', value: 'action', align: 'left', sortable: false, class: '', width: ''}
    ];
    files: File[] = [];
    file: File | null = null;   

    chooseFiles(){
        if(document != null)
        {
            document.getElementById('file-select')!.click();
        }
    }

    @Watch('file')
    onFileChanged() {        
        this.files = hasValue(this.file) ? [this.file as File] : [];                                   
        this.$emit('submit', this.file);
        (<HTMLInputElement>document.getElementById('file-select')!).value = '';
    }

    @Watch('closed')
    onClose() {
        if (this.closed) {
            this.files = [];
            this.file = null;
            this.fileSelect.value = '';
            (<HTMLInputElement>document.getElementById('file-select')!).value = '';
        }
    }
    @Watch('applyNoTreatment')
    onTreatmentChanged() {
        this.$emit('treatment', this.applyNoTreatment);
    }
    mounted() {
        // couple fileSelect object with #file-select input element
        this.fileSelect = document.getElementById('file-select') as HTMLInputElement;        
    }    

    /**
     * File input change event handler
     */
    onSelect(fileList: FileList) {
        if (hasValue(fileList)) {
            const fileName: string = prop('name', fileList[0]) as string;

            if (fileName.indexOf('xlsx') === -1) {
                this.addErrorNotificationAction({
                    message: 'Only .xlsx file types are allowed',
                });
            }

            this.file = clone(fileList[0]);
        }

        this.fileSelect.value = '';
    }

    /**
     * Returns a formatted string of a file's bytes
     */
    formatBytesSize(bytes: number) {
        return formatBytes(bytes);
    }
}
</script>

<style>
.files-table {
    height: 125px;
    overflow-x: hidden;
    overflow-y: auto;
}
.drag-drop-area {
    height: 100px;
    border-radius: 4px;
    padding-top: 20px;
}
.div-border {
    border: dashed;
    border-radius: 4px;
    border-width: 1px;
}
.span-center {
    justify-content: center;
    align-content: center;
}
</style>
