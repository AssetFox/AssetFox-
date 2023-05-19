import {AxiosResponse} from 'axios';
import {clone} from 'ramda';
import {hasValue} from '@/shared/utils/has-value-util';
import AnalysisMethodService from '@/services/analysis-method.service';
import {AnalysisMethod, emptyAnalysisMethod} from '@/shared/models/iAM/analysis-method';
import {http2XX} from '@/shared/utils/http-utils';

const state = {
    simulationReportNames: [] as string[],
    inventoryReportNames: [] as string[],
    primaryNetwork: '' as string,
    keyFields: [] as string[],
};

const mutations = {
};

const actions = {
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};