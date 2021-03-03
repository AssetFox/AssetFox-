import Vue from 'vue';
import Vuex from 'vuex';

import busyModule from '@/store-modules/busy.module';
import authenticationModule from '@/store-modules/authentication.module';
import networkModule from '../store-modules/network.module';
import scenarioModule from '../store-modules/scenario.module';
import inventoryModule from '@/store-modules/inventory.module';
import investmentModule from '@/store-modules/investment.module';
import performanceCurveModule from '@/store-modules/performance-curve.module';
import treatmentModule from '@/store-modules/treatment.module';
import attributeModule from '@/store-modules/attribute.module';
import toastrModule from '@/store-modules/toastr.module';
import deficientConditionGoalModule from '@/store-modules/deficient-condition-goal.module';
import budgetPriorityModule from '@/store-modules/budget-priority.module';
import targetConditionGoalModule from '@/store-modules/target-condition-goal.module';
import remainingLifeLimitModule from '@/store-modules/remaining-life-limit.module';
import rollupModule from '../store-modules/rollup.module';
import pollingModule from '@/store-modules/polling.module';
import announcementModule from '@/store-modules/announcement.module';
import cashFlowModule from '@/store-modules/cash-flow.module';
import userCriteriaModule from '@/store-modules/user-criteria.module';
import unsavedChangesFlagModule from '@/store-modules/unsaved-changes-flag.module';
import criterionModule from '@/store-modules/criterion-library.module';

Vue.use(Vuex);

export default new Vuex.Store({
    modules: {
        busyModule,
        authenticationModule,
        networkModule,
        scenarioModule,
        inventoryModule,
        investmentModule,
        performanceCurveModule,
        attributeModule,
        treatmentModule,
        toastrModule,
        deficientConditionGoalModule,
        budgetPriorityModule,
        targetConditionGoalModule,
        remainingLifeLimitModule,
        rollupModule,
        pollingModule,
        announcementModule,
        cashFlowModule,
        userCriteriaModule,
        unsavedChangesFlagModule,
        criterionModule
    }
});
