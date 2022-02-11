<template>
     <v-menu offset-y>
        <template v-slot:activator="{ on, attrs }">
            <button v-on="on" v-bind="attrs">
                <notification-bell
                :size="30"
                :count="0"
                :upperLimit="50"
                counterLocation="right"
                fontSize="10px"
                counterStyle="roundRectangle"
                counterBackgroundColor="#FF0000"
                counterTextColor="#FFFFFF"
                iconColor="#FFFFFF"
                />
            </button>
        </template>
         <v-card
    class="mx-auto"
    max-width="344"
    outlined
  >
    <v-list-item three-line>
      <v-list-item-content>
        <div class="text-overline mb-4">
          ERROR
        </div>
        <v-list-item-subtitle>Greyhound divisely hello coldly fonwderfully</v-list-item-subtitle>
      </v-list-item-content>
    </v-list-item>
    <v-card-actions>
      <v-btn
        outlined
        rounded
        text
      >
        Expand
      </v-btn>
    </v-card-actions>
  </v-card>
  </v-menu>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component} from 'vue-property-decorator';
    import {State} from 'vuex-class';
    import {NotificationBell} from 'vue-notification-bell';

    @Component ({
    components: { NotificationBell },
    })
    export default class NotificationMenu extends Vue {
        @State(state => state.notificationModule.notifications) notifications: Notification[];
    }
</script>