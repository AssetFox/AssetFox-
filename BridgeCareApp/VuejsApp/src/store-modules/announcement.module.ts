import { Announcement } from '@/shared/models/iAM/announcement';
import { any, append, clone, findIndex, prepend, propEq, reject, update } from 'ramda';
import AnnouncementService from '@/services/announcement.service';
import { AxiosResponse } from 'axios';
import { hasValue } from '@/shared/utils/has-value-util';
import { convertFromMongoToVue } from '@/shared/utils/mongo-model-conversion-utils';
import { http2XX } from '@/shared/utils/http-utils';
import { sortByProperty } from '@/shared/utils/sorter-utils';

const state = {
  announcements: [] as Announcement[],
  packageVersion: import.meta.env.PACKAGE_VERSION || '0',
};

const mutations = {
  announcementsMutator(state: any, announcements: Announcement[]) {
    state.announcements = clone(announcements);
  },
  addedOrUpdatedAnnouncementMutator(state: any, announcement: Announcement) {
    state.announcements = any(propEq('id', announcement.id), state.announcements)
      ? update(findIndex(propEq('id', announcement.id), state.announcements),
        announcement, state.announcements)
      : prepend(announcement, state.announcements);
  },
  deletedAnnouncementMutator(state: any, deletedAnnouncementId: string) {
    if (any(propEq('id', deletedAnnouncementId), state.announcements)) {
      state.announcements = reject((announcement: Announcement) => announcement.id === deletedAnnouncementId, state.announcements);
    }
  },
  sortAnnouncementsMutator(state: any) {
    state.announcements = sortByProperty('createdDate', state.announcements).reverse();
  },
};

const actions = {
  async getAnnouncements({ commit }: any) {
    await AnnouncementService.getAnnouncements().then((response: AxiosResponse<any[]>) => {
      if (hasValue(response, 'data')) {
        commit('announcementsMutator', response.data as Announcement[]);
      }
    });
  },
  async upsertAnnouncement({ dispatch, commit }: any, payload: any) {
    await AnnouncementService.upsertAnnouncement(payload.announcement)
      .then((response: AxiosResponse) => {
        if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
          const message: string = any(propEq('id', payload.announcement.id), state.announcements)
            ? 'Updated announcement' : 'Added announcement';
          commit('addedOrUpdatedAnnouncementMutator', payload.announcement);
          dispatch('setSuccessMessage', { message: message });
        }
      });
  },
  async deleteAnnouncement({ dispatch, commit }: any, payload: any) {
    await AnnouncementService.deleteAnnouncement(payload.deletedAnnouncementId)
      .then((response: AxiosResponse) => {
        if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
          commit('deletedAnnouncementMutator', payload.deletedAnnouncementId);
          dispatch('setSuccessMessage', { message: 'Deleted announcement' });
        }
      });
  }
};

const getters = {};

export default {
  state,
  getters,
  actions,
  mutations,
};
