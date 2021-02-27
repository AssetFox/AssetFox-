import Vue from 'vue';
import Vuex from 'vuex';

import busy from '@/store-modules/busy.module';
import authentication from '@/store-modules/authentication.module';
import network from '../store-modules/network.module';
import scenario from '../store-modules/scenario.module';
import inventory from '@/store-modules/inventory.module';
import investmentEditor from '@/store-modules/investment.module';
import performanceCurvesEditor from '@/store-modules/performance-curve.module';
import treatmentEditor from '@/store-modules/treatment.module';
import attribute from '@/store-modules/attribute.module';
import toastr from '@/store-modules/toastr.module';
import deficientEditor from '@/store-modules/deficient-condition-goal.module';
import priorityEditor from '@/store-modules/priority.module';
import targetEditor from '@/store-modules/target-condition-goal.module';
import remainingLifeLimitEditor from '@/store-modules/remaining-life-limit.module';
import rollup from '../store-modules/rollup.module';
import polling from '@/store-modules/polling.module';
import announcement from '@/store-modules/announcement.module';
import cashFlowEditor from '@/store-modules/cash-flow.module';
import userCriteria from '@/store-modules/user-criteria.module';
import unsavedChangesFlag from '@/store-modules/unsaved-changes-flag.module';
import criteriaEditor from '@/store-modules/criterion-library.module';

Vue.use(Vuex);

export default new Vuex.Store({
    modules: {
        busy,
        authentication,
        network,
        scenario,
        inventory,
        investmentEditor,
        performanceCurvesEditor,
        attribute,
        treatmentEditor,
        toastr,
        deficientEditor,
        priorityEditor,
        targetEditor,
        remainingLifeLimitEditor,
        rollup,
        polling,
        announcement,
        cashFlowEditor,
        userCriteria,
        unsavedChangesFlag,
        criteriaEditor
    }
});
