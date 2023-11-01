<template>
    <v-row>
        <div class="div-border align-start">   
            <div id="app" class="ghd-white-bg" v-cloak @drop.prevent="onSelect($event.dataTransfer.files)" @dragover.prevent>
                <v-row>
                    <div class="drag-drop-area">
                        <v-row justify="center" align="center" style="padding:10px; margin: 10px;">
                            <img :src="getUrl('assets/icons/upload.svg')" style="margin: 20px;"/>
                                <span class="span-center Montserrat-font-family">Drag & Drop Files Here </span>
                        </v-row>
                    </div>
                </v-row>
            </div>
        </div>
        <v-row justify="end">     
            <v-col cols="10">
                <v-switch
                    v-show="useTreatment"
                    label="No Treatment"
                    class="ghd-control-label ghd-md-gray Montserrat-font-family my-2"
                    v-model="applyNoTreatment"
                />
            </v-col>
        </v-row>
        <div>
            <input @update:model-value="onSelect($event.target.files)" id="file-select" type="file" hidden />
        </div>
    </v-row>        
    <v-divider/>
    <div style="margin: 10px;">
        <v-data-table-server :headers="tableHeaders"
                             :items="files"
                             :items-length="files.length"
                             class="elevation-1 fixed-header v-table__overflow Montserrat-font-family"
                             >
            <template slot="items" slot-scope="props" v-slot:item="props">
                <td>
                    {{props.item.name}}
                </td>
                <td>
                    <div><strong>{{ formatBytesSize(props.item.size) }}</strong></div>
                </td>
                <td>
                    <v-btn @click="file = null" class="ghd-blue" flat>
                        <img class='img-general' :src="getUrl('assets/icons/trash-ghd-blue.svg')"/>
                    </v-btn>
                </td>
            </template>
        </v-data-table-server>
    </div>
</template>

<script setup lang="ts">
import { ref, shallowRef, toRefs, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import {hasValue} from '@/shared/utils/has-value-util';
import {getPropertyValues} from '@/shared/utils/getter-utils';
import {clone, prop} from 'ramda';
import {DataTableHeader} from '@/shared/models/vue/data-table-header';
import { formatBytes } from '@/shared/utils/math-utils';
import { useStore } from 'vuex';
import { getUrl } from '../utils/get-url';

let store = useStore();
const emit = defineEmits(['submit','treatment'])
const props = defineProps<{
    closed: boolean,
    useTreatment: boolean
    }>()
const { useTreatment, closed } = toRefs(props);

async function addErrorNotificationAction(payload?: any): Promise<any> {await store.dispatch('addErrorNotification', payload);}
async function setIsBusyAction(payload?: any): Promise<any> {await store.dispatch('setIsBusy', payload);}

    const applyNoTreatment = ref<boolean>(true);
    const fileSelect = ref<HTMLInputElement>({} as HTMLInputElement);
    let tableHeaders: any[] = [
        {title: 'Name', key: 'name', align: 'left', sortable: false, class: '', width: '50%'},
        {title: 'Size', key: 'size', align: 'left', sortable: false, class: '', width: '35%'},
        {title: 'Action', key: 'action', align: 'left', sortable: false, class: '', width: ''}
    ];
    const files = ref<File[]>([]);
    const file = ref<File|null>(null);   
    
    function chooseFiles(){
        if(document != null)
        {
            document.getElementById('file-select')!.click();
        }
    }

    watch(file,()=>{        
        files.value = hasValue(file.value) ? [file.value as File] : [];                                   
        emit('submit', file.value);
        (<HTMLInputElement>document.getElementById('file-select')!).value = '';
    });

    watch(closed,()=>{
        if (closed.value) {
            files.value = [];
            file.value = null;
            fileSelect.value.value = '';
            (<HTMLInputElement>document.getElementById('file-select')!).value = '';
        }
    });

    watch(applyNoTreatment,()=>{
        emit('treatment', applyNoTreatment.value);
    });
    
    onMounted(() => {
        // couple fileSelect object with #file-select input element
        fileSelect.value = document.getElementById('file-select') as HTMLInputElement;        
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

        fileSelect.value.value = '';
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
