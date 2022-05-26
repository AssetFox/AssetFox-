<template>
  <v-dialog v-model="dialogData.showDialog" max-width="450px" persistent>
    <v-card>
      <v-card-title>
        <v-layout justify-left>
          <h3 class="ghd-dialog">New Criteria Library</h3>
        </v-layout>
      </v-card-title>
      <v-card-text>
        <v-layout column>
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
        </v-layout>
      </v-card-text>
      <v-card-actions>
          <v-layout justify-center row>
            <v-btn
                   class="ghd-white-bg ghd-blue ghd-button-text"
                   depressed
                   @click="onSubmit(false)">
              Cancel
            </v-btn>
            <v-btn :disabled="newCriterionLibrary.name === ''"
                   class="ghd-blue-bg ghd-white ghd-button-text"
                   @click="onSubmit(true)"
                   depressed                   
                   >
              Save
            </v-btn>
          </v-layout>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import Vue from 'vue';
import {Getter} from 'vuex-class';
import {Component, Prop, Watch} from 'vue-property-decorator';
import {CreateCriterionLibraryDialogData} from '@/shared/models/modals/create-criterion-library-dialog-data';
import {CriterionLibrary, emptyCriterionLibrary} from '@/shared/models/iAM/criteria';
import {getUserName} from '@/shared/utils/get-user-info';
import {getNewGuid} from '@/shared/utils/uuid-utils';

@Component
export default class CreateCriterionLibraryDialog extends Vue {
  @Prop() dialogData: CreateCriterionLibraryDialogData;

  @Getter('getIdByUserName') getIdByUserNameGetter: any;

  newCriterionLibrary: CriterionLibrary = {...emptyCriterionLibrary, id: getNewGuid(), isSingleUse: false};

  @Watch('dialogData')
  onDialogDataChanged() {
    let currentUser: string = getUserName();

    this.newCriterionLibrary = {
      ...this.newCriterionLibrary,
      mergedCriteriaExpression: this.dialogData.mergedCriteriaExpression,
      owner: this.getIdByUserNameGetter(currentUser),
    };
  }

  onSubmit(submit: boolean) {
    if (submit) {
      this.$emit('submit', this.newCriterionLibrary);
    } else {
      this.$emit('submit', null);
    }

    this.newCriterionLibrary = {...emptyCriterionLibrary, id: getNewGuid()};
  }
}
</script>