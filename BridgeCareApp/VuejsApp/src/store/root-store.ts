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
import deficientConditionGoalModule from '@/store-modules/deficient-condition-goal.module';
import budgetPriorityModule from '@/store-modules/budget-priority.module';
import targetConditionGoalModule from '@/store-modules/target-condition-goal.module';
import remainingLifeLimitModule from '@/store-modules/remaining-life-limit.module';
import pollingModule from '@/store-modules/polling.module';
import announcementModule from '@/store-modules/announcement.module';
import cashFlowModule from '@/store-modules/cash-flow.module';
import userModule from '@/store-modules/user.module';
import unsavedChangesFlagModule from '@/store-modules/unsaved-changes-flag.module';
import criterionModule from '@/store-modules/criterion-library.module';
import analysisMethodModule from '@/store-modules/analysis-method.module';
import azureB2CModule from '@/store-modules/azureB2C.module';
import calculatedAttributeModule from '@/store-modules/calculated-attribute.module';
import notificationModule from '@/store-modules/notification.module';
import datasourceModule from '@/store-modules/data-source.module';
import committedProjectsModule from '@/store-modules/committed-project.module';
import adminDataModule from '@/store-modules/admin-data.module';

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
        deficientConditionGoalModule,
        budgetPriorityModule,
        targetConditionGoalModule,
        remainingLifeLimitModule,
        pollingModule,
        announcementModule,
        cashFlowModule,
        userModule,
        unsavedChangesFlagModule,
        criterionModule,
        analysisMethodModule,
        azureB2CModule,
        calculatedAttributeModule,
        notificationModule,
        datasourceModule,
        committedProjectsModule,
        adminDataModule
    },
});
