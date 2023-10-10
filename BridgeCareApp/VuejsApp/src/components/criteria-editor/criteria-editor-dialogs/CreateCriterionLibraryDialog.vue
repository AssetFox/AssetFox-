<template>
  <v-dialog v-model="dialogData.showDialog" max-width="450px" persistent>
    <v-card>
      <v-card-title>
        <v-row justify-left>
          <h3 class="ghd-dialog">New Criteria Library</h3>
        </v-row>
      </v-card-title>
      <v-card-text>
        <v-row column>
          <v-subheader class="ghd-control-label ghd-md-gray">Name</v-subheader>            
          <v-text-field
            class="ghd-control-text ghd-control-border"
            v-model="newCriterionLibrary.name"
            outline
          >
          </v-text-field>
          <v-subheader class="ghd-control-label ghd-md-gray">Description</v-subheader>            
          <v-textarea
            class="ghd-control-text ghd-control-border"
            v-model="newCriterionLibrary.description"
            no-resize
            outline
            rows="3">
          </v-textarea>
        </v-row>
      </v-card-text>
      <v-card-actions>
          <v-row justify-center row>
            <v-btn
                   class="ghd-white-bg ghd-blue ghd-button-text"
                   variant = "flat"
                   @click="onSubmit(false)">
              Cancel
            </v-btn>
            <v-btn :disabled="newCriterionLibrary.name === ''"
                   class="ghd-blue-bg ghd-white ghd-button-text"
                   @click="onSubmit(true)"
                   variant = "flat"                   
                   >
              Save
            </v-btn>
          </v-row>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts" setup>
import { inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import {CreateCriterionLibraryDialogData} from '@/shared/models/modals/create-criterion-library-dialog-data';
import {CriterionLibrary, emptyCriterionLibrary} from '@/shared/models/iAM/criteria';
import {getUserName} from '@/shared/utils/get-user-info';
import {getNewGuid} from '@/shared/utils/uuid-utils';

const props = defineProps<{dialogData: CreateCriterionLibraryDialogData}>()
let newCriterionLibrary: CriterionLibrary = {...emptyCriterionLibrary, id: getNewGuid(), isSingleUse: false};
let store = useStore();
const emit = defineEmits(['submit'])

const dialogData = reactive(props.dialogData);

async function getIdByUserNameGetter(payload?: any): Promise<any> {
        await store.dispatch('getIdByUserName');
}

  watch(dialogData, () => onDialogDataChanged)
  async function onDialogDataChanged() {
    let currentUser: string = getUserName();

    newCriterionLibrary = {
      ...newCriterionLibrary,
      mergedCriteriaExpression: dialogData.mergedCriteriaExpression,
      owner: await getIdByUserNameGetter(currentUser),
    };
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', newCriterionLibrary);
    } else {
      emit('submit', null);
    }

    newCriterionLibrary = {...emptyCriterionLibrary, id: getNewGuid()};
  }


</script>