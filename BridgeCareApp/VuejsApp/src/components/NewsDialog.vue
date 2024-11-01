<template>
  <Dialog style="width:50%; height:auto" block-scroll modal v-model:visible="showDialog" :closable="false">
    <v-card class='announcement-dialog'>
        <v-toolbar color="#002E6C" dark>
            <!-- <v-app-bar-nav-icon></v-app-bar-nav-icon> -->
            <v-toolbar-title>Latest News</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-btn icon @click="closeDialog()">
                <v-icon>fas fa-times-circle</v-icon>
            </v-btn>
        </v-toolbar>
    <v-card style="margin: 10px;" v-if="hasAdminAccess">
        <v-container style="padding:0%">
            <v-row no-gutters>
                <v-col>
                    <v-icon v-if='isEditingAnnouncement()' class='assetFox-orange'
                            style='padding-right: 1em; padding-left: 1em;'
                            title='Stop Editing'
                            @click='onStopEditing()'>
                        fas fa-times-circle
                    </v-icon>
                    <v-col style="padding: 10px;">
                        <v-text-field class='announcement-title' label='Title' single-line variant="underlined"
                                      tabindex='1' v-model='newAnnouncementTitle' />
                        <v-card-text style='padding-top: 0; padding-bottom: 0; padding-left: 0%;'>{{ formatDate(new Date()) }}
                        </v-card-text>
                    </v-col>
                </v-col>
                <v-btn @click='onSendAnnouncement' class='assetFox-blue' flat
                           tabindex='3' title='Send Announcement'>
                        <v-icon>fas fa-paper-plane</v-icon>
                </v-btn>
            </v-row>
        </v-container>    
        <v-container style="padding:0%">
            <v-textarea
                auto-grow
                class='announcement-content'
                density="default" label='Announcement Text (HTML tags can be used for detailed formatting.)'
                rows='1' single-line
                style='padding: 0.4em;'
                tabindex='2'
                v-model='newAnnouncementContent' />
        </v-container>
    </v-card>
    <div style='display: flex; align-items: center; justify-content: center'>
        <v-btn @click='seeNewerAnnouncements()' class='assetFox-blue-bg text-white' round
               style='margin-top: 10px; margin-bottom: 0'
               v-if='announcementListOffset > 0'>
            See Newer Announcements
        </v-btn>
    </div>
        <div v-for='announcement in getVisibleAnnouncements()'>
            <v-card style="padding: 10px; margin: 10px;">
                <v-row justify="space-between" no-gutters>
                    <v-card-title class='announcement-title'>
                        <v-icon @click='onStopEditing()' class='assetFox-orange'
                                style='padding: 1em'
                                title='Stop Editing'
                                v-if='isEditingAnnouncement(announcement)'>
                            fas fa-times-circle
                    </v-icon>
                    {{ announcement.title }}
                    </v-card-title>
                    <div style="padding: 10px;">
                        <v-btn @click='onSetAnnouncementForEdit(announcement)' class='assetFox-blue'
                                   flat
                                   title='Edit Announcement' v-if='hasAdminAccess'>
                                <v-icon>fas fa-edit</v-icon>
                        </v-btn>
                        <v-btn @click='onDeleteAnnouncement(announcement.id)' class='assetFox-orange'
                                   flat
                                   title='Delete Announcement' v-if='hasAdminAccess'>
                                <v-icon>fas fa-trash</v-icon>
                        </v-btn>
                    </div>
                </v-row>
                <v-card-text class='announcement-date'>{{ formatDate(announcement.createdDate) }}
                </v-card-text>
                <v-card-text class='announcement-content'
                             v-html="announcement.content.replace(/(\r)*\n/g, '<br/>')"></v-card-text>
            </v-card>
        </div>
    <div style='display: flex; align-items: center; justify-content: center;'>
            <v-btn @click='seeOlderAnnouncements()' class='assetFox-blue-bg text-white' round
                   style='margin-top: 0; margin-bottom: 10px'
                   v-if='announcementListOffset < announcements.length - (hasAdminAccess ? 9 : 10)'>
                See Older Announcements
            </v-btn>
        </div>
    </v-card>
  </Dialog>
</template>

<script setup lang="ts">
import { Announcement, emptyAnnouncement } from '@/shared/models/iAM/announcement';
import moment from 'moment';
import { hasValue } from '@/shared/utils/has-value-util';
import { toRefs, computed, watch, ref } from 'vue';
import { useStore } from 'vuex';
import Dialog from 'primevue/dialog';
import { getNewGuid } from '@/shared/utils/uuid-utils';

const emit = defineEmits(['close'])
let store = useStore();
let props = defineProps<{
    showDialog: boolean
}>()
const { showDialog } = toRefs(props);
const announcements = computed<Announcement[]>(() => store.state.announcementModule.announcements);
const hasAdminAccess = computed<boolean>(() => store.state.authenticationModule.hasAdminAccess);
async function upsertAnnouncementAction(payload?: any): Promise<any> {await store.dispatch('upsertAnnouncement', payload);}
async function deleteAnnouncementAction(payload?: any): Promise<any> {await store.dispatch('deleteAnnouncement', payload);}
const announcementListOffset = ref<number>(0);
const newAnnouncementTitle = ref<string>("");
const newAnnouncementContent = ref<string>("");
const selectedAnnouncementForEdit = ref<Announcement | undefined>(undefined);

    watch(announcements, () => {
    });
    function getVisibleAnnouncements() {
        return announcements.value.slice(announcementListOffset.value, announcementListOffset.value + (hasAdminAccess.value ? 9 : 10));
    }

    function formatDate(announcementDate: Date) {
        return `${moment(announcementDate).format('dddd, MMMM Do, YYYY')}`;
    }

    function closeDialog() {
        emit("close", true);
    }

    function onDeleteAnnouncement(announcementId: string) {
        deleteAnnouncementAction({ deletedAnnouncementId: announcementId });
    }

    function onSendAnnouncement() {
        if (!hasValue(selectedAnnouncementForEdit.value)) {
            onCreateAnnouncement();
        } else {
            onEditAnnouncement();
        }
    }

    function onCreateAnnouncement() {
        upsertAnnouncementAction({
            announcement: {
                id: getNewGuid(),
                title: newAnnouncementTitle.value,
                content: newAnnouncementContent.value,
                createdDate: new Date(),
            },
        });
        newAnnouncementContent.value = newAnnouncementTitle.value = '';
            }

    function onEditAnnouncement() {
        if (!hasValue(selectedAnnouncementForEdit.value)) {
            return;
        }

        upsertAnnouncementAction({
            announcement: {
                ...selectedAnnouncementForEdit.value,
                title: newAnnouncementTitle.value,
                content: newAnnouncementContent.value,
            },
        });

        newAnnouncementContent.value = newAnnouncementTitle.value = '';
                selectedAnnouncementForEdit.value = undefined;
    }

    function onSetAnnouncementForEdit(announcement: Announcement) {
        selectedAnnouncementForEdit.value = announcement;
        newAnnouncementTitle.value = announcement.title;
        newAnnouncementContent.value = announcement.content;
    }

    function onStopEditing() {
        selectedAnnouncementForEdit.value = undefined;
        newAnnouncementTitle.value = newAnnouncementContent.value = '';
        isEditingAnnouncement(selectedAnnouncementForEdit.value);
    }

    function isEditingAnnouncement(announcement?: Announcement) {
        if (!hasValue(announcement)) {
            return hasValue(selectedAnnouncementForEdit.value);
        }
        return hasValue(selectedAnnouncementForEdit.value) && selectedAnnouncementForEdit.value!.id === announcement!.id;
    }

    function seeNewerAnnouncements() {
        // Admins see the announcement creation card, so they're shown one less announcement at a time to save space
        const decrement = hasAdminAccess ? 9 : 10;
        if (announcementListOffset.value > decrement) {
            announcementListOffset.value -= decrement;
        } else {
            announcementListOffset.value = 0;
        }
    }

    function seeOlderAnnouncements() {
        const increment = hasAdminAccess ? 9 : 10;
        if (announcementListOffset.value < announcements.value.length - increment) {
            announcementListOffset.value += increment;
        } else {
            announcementListOffset.value = announcementListOffset.value - increment;
        }
    }

</script>

<style scoped>
html {
    font-size: 14px;
}

body {
    font-family: var(--font-family);
    font-weight: normal;
    background: var(--surface-ground);
    color: var(--text-color);
    padding: 1rem;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
}

.card {
    background: var(--surface-card);
    padding: 2rem;
    border-radius: 10px;
    margin-bottom: 1rem;
}
    
.announcement-dialog {
    width: 100%;
    justify-content: center;
}

.announcement {
    margin: 10px 20px;
}

.announcement-title {
    font-size: 2em;
    font-weight: bold;
    padding-bottom: 0;
}

.announcement-date {
    font-size: 0.9em;
    padding-top: 0;
    padding-bottom: 0;
}

.announcement-content {
    font-size: 1.2em;
    padding-top: 0.75em;
    padding-bottom: 1em;
}

</style>