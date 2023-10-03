<template>
  <v-dialog max-width="65%" min persistent v-bind:show="showDialog">
    <v-container fluid grid-list-xl>
        <v-layout>
            <v-flex xs12>
                <v-layout justify-center>
                    <v-card class='announcement-dialog'>
                        <v-toolbar
                        color="#002E6C"
                        dark
                        >
                            <v-app-bar-nav-icon></v-app-bar-nav-icon>

                            <v-toolbar-title>Latest News</v-toolbar-title>

                            <v-spacer></v-spacer>
                            <v-btn icon @click="closeDialog()">
                                <v-icon>fas fa-times-circle</v-icon>
                            </v-btn>
                        </v-toolbar>    
                        <div class='announcement' style='padding-bottom: 0; margin-bottom: 0' v-if='hasAdminAccess'>
                            <v-card style='margin-bottom: 0; padding-bottom: 0'>
                                <v-card-title style='padding-top: 0; padding-bottom: 0'>
                                    <v-icon v-if='isEditingAnnouncement()' class='ara-orange'
                                            style='padding-right: 1em'
                                            title='Stop Editing'
                                            @click='onStopEditing()'>
                                        fas fa-times-circle
                                    </v-icon>
                                    <v-text-field class='announcement-title' label='Title' single-line
                                                  tabindex='1' v-model='newAnnouncementTitle' />
                                    <v-spacer />
                                    <v-btn @click='onSendAnnouncement' class='ara-blue' icon
                                           tabindex='3' title='Send Announcement'>
                                        <v-icon>fas fa-paper-plane</v-icon>
                                    </v-btn>
                                </v-card-title>
                                <v-card-text style='padding-top: 0; padding-bottom: 0'>{{ formatDate(new Date()) }}
                                </v-card-text>
                                <v-textarea
                                    auto-grow
                                    class='announcement-content'
                                    dense label='Announcement Text (HTML tags can be used for detailed formatting.)'
                                    rows='1' single-line
                                    style='padding-left: 1em; padding-right: 1em; padding-top: 0.2em'
                                    tabindex='2'
                                    v-model='newAnnouncementContent' />
                            </v-card>
                        </div>
                        <div style='display: flex; align-items: center; justify-content: center'>
                            <v-btn @click='seeNewerAnnouncements()' class='ara-blue-bg text-white' round
                                   style='margin-top: 10px; margin-bottom: 0'
                                   v-if='announcementListOffset > 0'>
                                See Newer Announcements
                            </v-btn>
                        </div>
                        <div class='announcement' v-for='announcement in getVisibleAnnouncements()'>
                            <v-card>
                                <v-card-title class='announcement-title'>
                                    <v-icon @click='onStopEditing()' class='ara-orange'
                                            style='padding-right: 1em'
                                            title='Stop Editing'
                                            v-if='isEditingAnnouncement(announcement)'>
                                        fas fa-times-circle
                                    </v-icon>
                                    {{ announcement.title }}
                                    <v-spacer />
                                    <v-btn @click='onSetAnnouncementForEdit(announcement)' class='ara-blue'
                                           icon
                                           title='Edit Announcement' v-if='hasAdminAccess'>
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                    <v-btn @click='onDeleteAnnouncement(announcement.id)' class='ara-orange'
                                           icon
                                           title='Delete Announcement' v-if='hasAdminAccess'>
                                        <v-icon>fas fa-trash</v-icon>
                                    </v-btn>
                                </v-card-title>
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
                </v-layout>
            </v-flex>
        </v-layout>
    </v-container>
  </v-dialog>
</template>

<script lang="ts" setup>
import Vue from 'vue';
import { Announcement, emptyAnnouncement } from '@/shared/models/iAM/announcement';
import moment from 'moment';
import { hasValue } from '@/shared/utils/has-value-util';
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';

const emit = defineEmits(['close'])
let store = useStore();
const props = defineProps<{
    showDialog: boolean
    }>()

let announcements = ref<Announcement[]>(store.state.announcementModule.announcements);
let hasAdminAccess = ref<boolean>(store.state.authenticationModule.hasAdminAccess);

async function upsertAnnouncementAction(payload?: any): Promise<any> {await store.dispatch('upsertAnnouncement');}
async function deleteAnnouncementAction(payload?: any): Promise<any> {await store.dispatch('deleteAnnouncement');}
 
    let announcementListOffset: number = 0;

    let newAnnouncementTitle: string = '';
    let newAnnouncementContent: string = '';

    let selectedAnnouncementForEdit: Announcement|undefined = undefined;

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

<style>
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