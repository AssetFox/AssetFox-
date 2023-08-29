export interface AlertData {
    showDialog: boolean;
    heading: string;
    message: string;
    choice: boolean;
}

export const emptyAlertData: AlertData = {
    showDialog: false,
    heading: '',
    message: '',
    choice: false
};

export interface AlertDataWithButtons {
    showDialog: boolean;
    heading: string;
    message: string;
    choice: boolean;
    buttons: string[];
}

export const emptyAlertDataWithButtons: AlertDataWithButtons = {
    showDialog: false,
    heading: '',
    message: '',
    choice: false,
    buttons : ['']
};

export interface SampleButton {
    label: string;
}

export const emptySampleButton: SampleButton = {
    label: '',
}