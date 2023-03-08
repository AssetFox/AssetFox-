<template>
  <v-dialog max-width="65%" min persistent v-model="showDialog">
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
                            <v-btn @click='seeNewerAnnouncements()' class='ara-blue-bg white--text' round
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
                            <v-btn @click='seeOlderAnnouncements()' class='ara-blue-bg white--text' round
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

<script lang="ts">
import Vue from 'vue';
import { Action, State } from 'vuex-class';
import { Component, Prop } from 'vue-property-decorator';
import { Announcement, emptyAnnouncement } from '@/shared/models/iAM/announcement';
import moment from 'moment';
import { hasValue } from '@/shared/utils/has-value-util';

@Component
export default class NewsDialog extends Vue {
    @Prop() showDialog: boolean;
  
    @State(state => state.announcementModule.announcements) announcements: Announcement[];
    @State(state => state.authenticationModule.hasAdminAccess) hasAdminAccess: boolean;
    
    @Action('upsertAnnouncement') upsertAnnouncementAction: any;
    @Action('deleteAnnouncement') deleteAnnouncementAction: any;

    announcementListOffset: number = 0;

    newAnnouncementTitle: string = '';
    newAnnouncementContent: string = '';

    selectedAnnouncementForEdit?: Announcement = undefined;

    getVisibleAnnouncements() {
        return this.announcements.slice(this.announcementListOffset, this.announcementListOffset + (this.hasAdminAccess ? 9 : 10));
    }

    formatDate(announcementDate: Date) {
        return `${moment(announcementDate).format('dddd, MMMM Do, YYYY')}`;
    }

    closeDialog() {
        this.$emit("close", true);
    }

    onDeleteAnnouncement(announcementId: string) {
        this.deleteAnnouncementAction({ deletedAnnouncementId: announcementId });
    }

    onSendAnnouncement() {
        if (!hasValue(this.selectedAnnouncementForEdit)) {
            this.onCreateAnnouncement();
        } else {
            this.onEditAnnouncement();
        }
    }

    onCreateAnnouncement() {
        this.upsertAnnouncementAction({
            announcement: {
                ...emptyAnnouncement,
                title: this.newAnnouncementTitle,
                content: this.newAnnouncementContent,
                createdDate: new Date(),
            },
        });

        this.newAnnouncementContent = this.newAnnouncementTitle = '';
    }

    onEditAnnouncement() {
        if (!hasValue(this.selectedAnnouncementForEdit)) {
            return;
        }

        this.upsertAnnouncementAction({
            announcement: {
                ...this.selectedAnnouncementForEdit,
                title: this.newAnnouncementTitle,
                content: this.newAnnouncementContent,
            },
        });

        this.newAnnouncementContent = this.newAnnouncementTitle = '';
        this.selectedAnnouncementForEdit = undefined;
    }

    onSetAnnouncementForEdit(announcement: Announcement) {
        this.selectedAnnouncementForEdit = announcement;
        this.newAnnouncementTitle = announcement.title;
        this.newAnnouncementContent = announcement.content;
    }

    onStopEditing() {
        this.selectedAnnouncementForEdit = undefined;
        this.newAnnouncementTitle = this.newAnnouncementContent = '';
    }

    isEditingAnnouncement(announcement?: Announcement) {
        if (!hasValue(announcement)) {
            return hasValue(this.selectedAnnouncementForEdit);
        }
        return hasValue(this.selectedAnnouncementForEdit) && this.selectedAnnouncementForEdit!.id === announcement!.id;
    }

    seeNewerAnnouncements() {
        // Admins see the announcement creation card, so they're shown one less announcement at a time to save space
        const decrement = this.hasAdminAccess ? 9 : 10;
        if (this.announcementListOffset > decrement) {
            this.announcementListOffset -= decrement;
        } else {
            this.announcementListOffset = 0;
        }
    }

    seeOlderAnnouncements() {
        const increment = this.hasAdminAccess ? 9 : 10;
        if (this.announcementListOffset < this.announcements.length - increment) {
            this.announcementListOffset += increment;
        } else {
            this.announcementListOffset = this.announcementListOffset - increment;
        }
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