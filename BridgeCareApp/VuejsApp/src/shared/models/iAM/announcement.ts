import { getNewGuid } from '@/shared/utils/uuid-utils';

export interface Announcement {
    id: string;
    title: string;
    content: string;
    createdDate: Date;
}

export const emptyAnnouncement: Announcement = {
    id: getNewGuid(),
    title: '',
    content: '',
    createdDate: new Date()
};