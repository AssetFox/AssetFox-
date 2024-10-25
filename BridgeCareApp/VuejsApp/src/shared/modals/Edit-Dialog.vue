<template>
    <div class="text-center">
      <div 
        style="cursor:pointer;"
        color="primary"
      >
        <slot></slot>
  
        <v-dialog
          v-model="dialog"
          activator="parent"
          max-width="290"
        >
          <v-card>
            <v-card-text>
                <v-col>

                <slot style="cursor:pointer;" name="input"></slot>          
                <v-row >
                  <v-col align="center">
                    <CancelButton @cancel="onCancel" />
                    <SaveButton @save="onSave" />                      
                  </v-col>                   
                </v-row>
                </v-col>
                
            </v-card-text>
          </v-card>
        </v-dialog>
    </div>
    </div>
  </template>
<script setup lang="ts">

import { clone } from 'ramda';
import { computed, ref, watch } from 'vue';
import SaveButton from '@/shared/components/buttons/SaveButton.vue';
import CancelButton from '@/shared/components/buttons/CancelButton.vue';
    const emit = defineEmits(['save','open', 'update:returnValue'])
    let dialog = ref(false)
    let prevVal:any;
    const props = defineProps<{
        returnValue: any
    }>()

    const value = computed({
      get() {
        return props.returnValue
      },
      set(value) {
        emit('update:returnValue', value)
      }
    })

    watch(dialog, (newVal, oldVal) =>
    {
        if(newVal = true){
            emit('open')
            prevVal = clone(props.returnValue)
        }
    })

    function onSave(){
        emit('save');
        dialog.value = false;
    }

    function onCancel(){
      value.value = prevVal;
      dialog.value = false;
    }
</script>