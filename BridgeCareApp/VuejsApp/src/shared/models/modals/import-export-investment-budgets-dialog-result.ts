import { ImportExportFilesDialogResult } from '@/shared/models/modals/import-export-files-dialog-result';

export interface ImportExportInvestmentBudgetsDialogResult
    extends ImportExportFilesDialogResult {
    overwriteBudgets: boolean;
}
