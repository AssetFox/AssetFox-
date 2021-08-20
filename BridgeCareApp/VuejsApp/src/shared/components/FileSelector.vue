<template>
    <v-layout column>
        <v-layout column>           
            <div id="app" class="ara-light-gray-bg" v-cloak @drop.prevent="onSelect($event.dataTransfer.files)" @dragover.prevent>
                <v-layout align-center fill-height justify-center>
                    <div class="drag-drop-area">Drag & Drop File Here</div>
                </v-layout>                
            </div>
            <v-flex xs12>
                <v-layout justify-start>                  
                    <v-btn @click="chooseFiles()" class="ara-blue-bg white--text">
                        Select File
                    </v-btn>
                </v-layout>
            </v-flex>
            <div v-show="true">
                <input @change="onSelect($event.target.files)" id="file-select" type="file" hidden/>
            </div>
        </v-layout>        
        <div class="files-table">
            <v-data-table :headers="tableHeaders" :items="files" class="elevation-1 fixed-header v-table__overflow"
                          hide-actions>
                <template slot="items" slot-scope="props">
                    <td>
                        <v-layout column>
                            <span>{{props.item.name}}</span>
                            <div><strong>{{ formatBytesSize(props.item.size) }}</strong></div>
                        </v-layout>
                    </td>
                    <td>
                        <v-btn @click="file = null" class="ara-orange" icon>
                            <v-icon>fas fa-trash</v-icon>
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
import { formatBytes } from '@/shared/utils/math-utils.ts';

@Component
export default class FileSelector extends Vue {
    @Prop() closed: boolean = false;

    @Action('setErrorMessage') setErrorMessageAction: any;
    @Action('setIsBusy') setIsBusyAction: any;
    
    fileSelect: HTMLInputElement = {} as HTMLInputElement;
    tableHeaders: DataTableHeader[] = [
        {text: 'Selected File', value: 'name', align: 'left', sortable: false, class: '', width: '150px'},
        {text: '', value: '', align: 'center', sortable: false, class: '', width: '25px'}
    ];
    files: File[] = [];
    file: File | null = null;   

    chooseFiles(){
        if(document != null)
        {
            document.getElementById("file-select").click();
        }
    }

    @Watch('file')
    onFileChanged() {        
        this.files = hasValue(this.file) ? [this.file as File] : [];                                   
        this.$emit('submit', this.file);
        document.getElementById("file-select").value = '';
    }

    @Watch('closed')
    onClose() {
        if (this.closed) {
            this.files = [];
            this.file = null;
            this.fileSelect.value = '';
            document.getElementById("file-select").value = '';
        }
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
                this.setErrorMessageAction({message: 'Only .xlsx file types are allowed'});
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
    overflow-y: auto;
}

.drag-drop-area{
    height: 100px;
    border-radius: 4px;
    padding-top: 40px;
}
</style>
