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
                <input @change="onSelect($event.target?.files)" id="file-select" type="file" hidden />
            </div>
        </v-layout>        
        <div class="files-table">
            <v-data-table :headers="tableHeaders" :items="files" class="elevation-1 fixed-header v-table__overflow Montserrat-font-family"
                        sort-icon=$vuetify.icons.ghd-table-sort
                          hide-actions>
                <template slot="items" slot-scope="props" v-slot:items="props">
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

<script lang="ts" setup>
import Vue, { shallowRef } from 'vue';
import {hasValue} from '@/shared/utils/has-value-util';
import {getPropertyValues} from '@/shared/utils/getter-utils';
import {clone, prop} from 'ramda';
import {DataTableHeader} from '@/shared/models/vue/data-table-header';
import { formatBytes } from '@/shared/utils/math-utils';
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';

let store = useStore();
const emit = defineEmits(['submit','treatment'])
const props = defineProps<{
    closed: boolean,
    useTreatment: boolean
    }>()

async function addErrorNotificationAction(payload?: any): Promise<any> {await store.dispatch('addErrorNotification');}
async function setIsBusyAction(payload?: any): Promise<any> {await store.dispatch('setIsBusy');}

    let applyNoTreatment= shallowRef<boolean>(true);
    let fileSelect: HTMLInputElement = {} as HTMLInputElement;
    let tableHeaders: DataTableHeader[] = [
        {text: 'Name', value: 'name', align: 'left', sortable: false, class: '', width: '50%'},
        {text: 'Size', value: 'size', align: 'left', sortable: false, class: '', width: '35%'},
        {text: 'Action', value: 'action', align: 'left', sortable: false, class: '', width: ''}
    ];
    let files: File[] = [];
    let file= shallowRef<File|null>(null);   
    let closed = shallowRef<boolean>(true);

    function chooseFiles(){
        if(document != null)
        {
            document.getElementById('file-select')!.click();
        }
    }

    watch(file,()=>onFileChanged())
    function onFileChanged() {        
        files = hasValue(file.value) ? [file.value as File] : [];                                   
        emit('submit', file);
        (<HTMLInputElement>document.getElementById('file-select')!).value = '';
    }

    watch(closed,()=>onClose())
    function onClose() {
        if (closed) {
            files = [];
            file.value = null;
            fileSelect.value = '';
            (<HTMLInputElement>document.getElementById('file-select')!).value = '';
        }
    }
    watch(applyNoTreatment,()=>onTreatmentChanged())
    function onTreatmentChanged() {
        emit('treatment', applyNoTreatment);
    }
    onMounted(()=>mounted())
    function mounted() {
        // couple fileSelect object with #file-select input element
        fileSelect = document.getElementById('file-select') as HTMLInputElement;        
    }    

    /**
     * File input change event handler
     */
    function onSelect(fileList: FileList) {
        if (hasValue(fileList)) {
            const fileName: string = prop('name', fileList[0]) as string;

            if (fileName.indexOf('xlsx') === -1) {
                addErrorNotificationAction({
                    message: 'Only .xlsx file types are allowed',
                });
            }

            file.value = clone(fileList[0]);
        }

        fileSelect.value = '';
    }

    /**
     * Returns a formatted string of a file's bytes
     */
    function formatBytesSize(bytes: number) {
        return formatBytes(bytes);
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
