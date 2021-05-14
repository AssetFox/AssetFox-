import {AxiosPromise} from 'axios';
import { API, coreAxiosInstance} from '@/shared/utils/axios-instance';
import {Announcement} from '@/shared/models/iAM/announcement';

export default class AnnouncementService {
    static getAnnouncements(): AxiosPromise {
        return coreAxiosInstance.get(`${API.Announcement}/GetAnnouncements`);
    }

    static upsertAnnouncement(data: Announcement): AxiosPromise {
        return coreAxiosInstance.post(`${API.Announcement}/UpsertAnnouncement`, data);
    }

    static deleteAnnouncement(announcementId: string): AxiosPromise {
        return coreAxiosInstance.delete(`${API.Announcement}/DeleteAnnouncement/${announcementId}`);
    }
}