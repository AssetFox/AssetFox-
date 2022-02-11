export enum NotificationType {
    Success,
    Warning,
    Error,
    Info,
}

export interface Notification {
    type: NotificationType;
    shortMessage: string;
    longMessage: string;
}
