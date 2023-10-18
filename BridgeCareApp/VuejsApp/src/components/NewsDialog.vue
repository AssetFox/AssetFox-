<template>
  <Dialog :style="{ width: '50vw', height: '50vw'}" block-scroll modal v-model:visible="showDialog" :closable="false">
                    <v-card class='announcement-dialog'>
                        <v-toolbar color="#002E6C" dark>
                            <v-app-bar-nav-icon></v-app-bar-nav-icon>
                            <v-toolbar-title>Latest News</v-toolbar-title>
                            <v-spacer></v-spacer>
                            <v-btn icon @click="closeDialog()">
                                <v-icon>fas fa-times-circle</v-icon>
                            </v-btn>
                        </v-toolbar>
                    <v-card style="margin: 10px;" v-if="hasAdminAccess">
                        <v-container>
                            <v-row no-gutters>
                                <v-col>
                                    <v-icon v-if='isEditingAnnouncement()' class='ara-orange'
                                            style='padding-right: 1em'
                                            title='Stop Editing'
                                            @click='onStopEditing()'>
                                        fas fa-times-circle
                                    </v-icon>
                                    <v-col>
                                        <v-text-field class='announcement-title' label='Title' single-line variant="underlined"
                                                      tabindex='1' :model-value='newAnnouncementTitle' />
                                        <v-card-text style='padding-top: 0; padding-bottom: 0'>{{ formatDate(new Date()) }}
                                        </v-card-text>
                                    </v-col>
                                </v-col>
                                <v-btn @click='onSendAnnouncement' class='ara-blue' icon
                                           tabindex='3' title='Send Announcement'>
                                        <v-icon>fas fa-paper-plane</v-icon>
                                </v-btn>
                            </v-row>
                        </v-container>    
                        <v-container>
                            <v-textarea
                                auto-grow
                                class='announcement-content'
                                density="default" label='Announcement Text (HTML tags can be used for detailed formatting.)'
                                rows='1' single-line
                                style='padding-left: 1em; padding-right: 1em; padding-top: 0.2em'
                                tabindex='2'
                                v-model='newAnnouncementContent' />
                        </v-container>
                    </v-card>
                    <div style='display: flex; align-items: center; justify-content: center'>
                        <v-btn @click='seeNewerAnnouncements()' class='ara-blue-bg text-white' round
                               style='margin-top: 10px; margin-bottom: 0'
                               v-if='announcementListOffset > 0'>
                            See Newer Announcements
                        </v-btn>
                    </div>
                        <div v-for='announcement in getVisibleAnnouncements()'>
                            <v-card style="padding: 10px; margin: 10px;">
                                <v-row justify="space-between">
                                    <v-card-title class='announcement-title'>
                                        <v-icon @click='onStopEditing()' class='ara-orange'
                                                style='padding-right: 1em'
                                                title='Stop Editing'
                                                v-if='isEditingAnnouncement(announcement)'>
                                    fas fa-times-circle
                                    </v-icon>
                                    {{ announcement.title }}
                                    </v-card-title>
                                    <div style="padding:10px;">
                                        <v-btn @click='onSetAnnouncementForEdit(announcement)' class='ara-blue'
                                                   icon
                                                   style="margin: 10px;"
                                                   title='Edit Announcement' v-if='hasAdminAccess'>
                                                <v-icon>fas fa-edit</v-icon>
                                        </v-btn>
                                        <v-btn @click='onDeleteAnnouncement(announcement.id)' class='ara-orange'
                                                   icon
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
                            <v-btn @click='seeOlderAnnouncements()' class='ara-blue-bg text-white' round
                                   style='margin-top: 0; margin-bottom: 10px'
                                   v-if='announcementListOffset < announcements.length - (hasAdminAccess ? 9 : 10)'>
                                See Older Announcements
                            </v-btn>
                        </div>
                    </v-card>
  </Dialog>
</template>

<script lang="ts" setup>
import { Announcement, emptyAnnouncement } from '@/shared/models/iAM/announcement';
import moment from 'moment';
import { hasValue } from '@/shared/utils/has-value-util';
import { toRefs, computed, watch} from 'vue';
import { useStore } from 'vuex';
import Dialog from 'primevue/dialog';

const emit = defineEmits(['close'])
let store = useStore();
let props = defineProps<{
    showDialog: boolean
}>()
const { showDialog } = toRefs(props);
let announcements = computed(() => store.state.announcementModule.announcements);
let hasAdminAccess = computed(() => store.state.authenticationModule.hasAdminAccess);
async function upsertAnnouncementAction(payload?: any): Promise<any> {await store.dispatch('upsertAnnouncement');}
async function deleteAnnouncementAction(payload?: any): Promise<any> {await store.dispatch('deleteAnnouncement');}
 
let announcementListOffset: number = 0;
let newAnnouncementTitle: string = '';
let newAnnouncementContent: string = '';
let selectedAnnouncementForEdit: Announcement|undefined = undefined;

    watch(announcements, () => {
    });
    function getVisibleAnnouncements() {
        return announcements.value.slice(announcementListOffset, announcementListOffset + (hasAdminAccess ? 9 : 10));
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
        if (!hasValue(selectedAnnouncementForEdit)) {
            onCreateAnnouncement();
        } else {
            onEditAnnouncement();
        }
    }

    function onCreateAnnouncement() {
        upsertAnnouncementAction({
            announcement: {
                ...emptyAnnouncement,
                title: newAnnouncementTitle,
                content: newAnnouncementContent,
                createdDate: new Date(),
            },
        });

        newAnnouncementContent = newAnnouncementTitle = '';
    }

    function onEditAnnouncement() {
        if (!hasValue(selectedAnnouncementForEdit)) {
            return;
        }

        upsertAnnouncementAction({
            announcement: {
                ...selectedAnnouncementForEdit,
                title: newAnnouncementTitle,
                content: newAnnouncementContent,
            },
        });

        newAnnouncementContent = newAnnouncementTitle = '';
        selectedAnnouncementForEdit = undefined;
    }

    function onSetAnnouncementForEdit(announcement: Announcement) {
        selectedAnnouncementForEdit = announcement;
        newAnnouncementTitle = announcement.title;
        newAnnouncementContent = announcement.content;
    }

    function onStopEditing() {
        selectedAnnouncementForEdit = undefined;
        newAnnouncementTitle = newAnnouncementContent = '';
    }

    function isEditingAnnouncement(announcement?: Announcement) {
        if (!hasValue(announcement)) {
            return hasValue(selectedAnnouncementForEdit);
        }
        return hasValue(selectedAnnouncementForEdit) && selectedAnnouncementForEdit!.id === announcement!.id;
    }

    function seeNewerAnnouncements() {
        // Admins see the announcement creation card, so they're shown one less announcement at a time to save space
        const decrement = hasAdminAccess ? 9 : 10;
        if (announcementListOffset > decrement) {
            announcementListOffset -= decrement;
        } else {
            announcementListOffset = 0;
        }
    }

    function seeOlderAnnouncements() {
        const increment = hasAdminAccess ? 9 : 10;
        if (announcementListOffset < announcements.value.length - increment) {
            announcementListOffset += increment;
        } else {
            announcementListOffset = announcementListOffset - increment;
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