export interface Notification {
    id: number;
    icon: string;
    iconColor: string;
    active: boolean;
    shortMessage: string;
    longMessage: string;
    stackTrace?: string;
}
