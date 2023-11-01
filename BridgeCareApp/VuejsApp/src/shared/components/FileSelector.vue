<template>
    <v-row column>
        <v-row column>        
            <div class="div-border align-start">   
                <div id="app" class="ghd-white-bg" v-cloak @drop.prevent="onSelect($event.dataTransfer.files)" @dragover.prevent>
                    <v-row fill-height justify-center>
                        <div class="drag-drop-area">
                            <v-row fill-height align-center justify-center>
                                <img :src="getUrl('assets/icons/upload.svg')"/>
                                <v-row column align-center>
                                    <span class="span-center Montserrat-font-family">Drag & Drop Files Here </span>
                                    <!--<span class="span-center Montserrat-font-family">or</span>
                                    <v-btn class="ghd-blue Montserrat-font-family a-0 ma-0" @click="chooseFiles()" flat> Click here to select files </v-btn>-->
                                </v-row>
                            </v-row>
                        </div>
                        </v-row>
                </div>
            </div>
            <v-col cols = "12">
                <v-row justify-start>     
                    <v-switch
                        v-show="useTreatment"
                        label="No Treatment"
                        class="ghd-control-label ghd-md-gray Montserrat-font-family my-2"
                        v-model="applyNoTreatment"
                    />
                </v-row>
            </v-col>
            <div v-show="true">
                <input @change="onSelect($event.target.files)" id="file-select" type="file" hidden />
            </div>
        </v-row>        
        <div class="files-table">
            <v-data-table-server :headers="tableHeaders"
                                 :items="files"
                                 :items-length="files.length"
                                 class="elevation-1 fixed-header v-table__overflow Montserrat-font-family"
                                 sort-icon=ghd-table-sort
                                 hide-actions>
                <template slot="items" slot-scope="props" v-slot:item="props">
                    <td>
                        {{props.item.name}}
                    </td>
                    <td>
                        <div><strong>{{ formatBytesSize(props.item.size) }}</strong></div>
                    </td>
                    <td>
                        <v-btn @click="file = null" class="ghd-blue" icon>
                            <img class='img-general' :src="getUrl('assets/icons/trash-ghd-blue.svg')"/>
                        </v-btn>
                    </td>
                </template>
            </v-data-table-server>
        </div>
    </v-row>
</template>

<script lang="ts" setup>
import Vue, { shallowRef } from 'vue';
import {hasValue} from '@/shared/utils/has-value-util';
import {getPropertyValues} from '@/shared/utils/getter-utils';
import {clone, prop} from 'ramda';
import {DataTableHeader} from '@/shared/models/vue/data-table-header';
import { formatBytes } from '@/shared/utils/math-utils';
import {inject, reactive, ref, toRefs, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
import { getUrl } from '../utils/get-url';

let store = useStore();
const emit = defineEmits(['submit','treatment'])
const props = defineProps<{
    closed: boolean,
    useTreatment: boolean
    }>()
const { useTreatment } = toRefs(props);

async function addErrorNotificationAction(payload?: any): Promise<any> {await store.dispatch('addErrorNotification', payload);}
async function setIsBusyAction(payload?: any): Promise<any> {await store.dispatch('setIsBusy');}

    let applyNoTreatment= shallowRef<boolean>(true);
    let fileSelect: HTMLInputElement = {} as HTMLInputElement;
    let tableHeaders: any[] = [
        {title: 'Name', key: 'name', align: 'left', sortable: false, class: '', width: '50%'},
        {title: 'Size', key: 'size', align: 'left', sortable: false, class: '', width: '35%'},
        {title: 'Action', key: 'action', align: 'left', sortable: false, class: '', width: ''}
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

    watch(file,()=>{        
        files = hasValue(file.value) ? [file.value as File] : [];                                   
        emit('submit', file);
        (<HTMLInputElement>document.getElementById('file-select')!).value = '';
    });

    watch(closed,()=>{
        if (closed) {
            files = [];
            file.value = null;
            fileSelect.value = '';
            (<HTMLInputElement>document.getElementById('file-select')!).value = '';
        }
    });

    watch(applyNoTreatment,()=>{
        emit('treatment', applyNoTreatment);
    });
    
    onMounted(() => {
        // couple fileSelect object with #file-select input element
        fileSelect = document.getElementById('file-select') as HTMLInputElement;        
    });   

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
