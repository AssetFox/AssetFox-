export interface UserCriteria {
    username: string;
    criteria?: string;
    hasCriteria: boolean;
    hasAccess: boolean;
}

export const emptyUserCriteria: UserCriteria = {
    username: '',
    hasCriteria: false,
    hasAccess: false
};
