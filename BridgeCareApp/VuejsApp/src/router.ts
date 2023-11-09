import EditAnalysisMethod from '@/components/scenarios/EditAnalysisMethod.vue';
import UnderConstruction from '@/components/UnderConstruction.vue';
import Logout from '@/components/Logout.vue';
import AuthenticationStart from '@/components/authentication/AuthenticationStart.vue';
import { hasValue } from '@/shared/utils/has-value-util';
import { UnsecuredRoutePathNames } from '@/shared/utils/route-paths';
import { SecurityTypes } from '@/shared/utils/security-types';
import store from '@/store/root-store';
import {
    isAuthenticatedUser,
    onHandleLogout,
} from '@/shared/utils/authentication-utils';
import { createRouter, createWebHistory } from 'vue-router';

// Lazily-loaded pages
const Scenario = () =>
    import(
        /* webpackChunkName: "scenario" */ '@/components/scenarios/Scenarios.vue'
    );
const EditScenario = () =>
    import(
        /* webpackChunkName: "editScenario" */ '@/components/scenarios/EditScenario.vue'
    );
const EditLibrary = () =>
    import(
        /* webpackChunkName: "editLibrary" */ '@/components/libraries/EditLibrary.vue'
    );
const InvestmentEditor = () =>
    import(
        /* webpackChunkName: "investmentModule" */ '@/components/investment-editor/InvestmentEditor.vue'
    );
const PerformanceCurveEditor = () =>
    import(
        /* webpackChunkName: "performanceCurveEditor" */ '@/components/performance-curve-editor/PerformanceCurveEditor.vue'
    );
const TreatmentEditor = () =>
    import(
        /* webpackChunkName: "treatmentEditor" */ '@/components/treatment-editor/TreatmentEditor.vue'
    );
const ReportsAndOutputs = () =>
    import(
        '@/components/reports/ReportsAndOutputs.vue'
    );
const BudgetPriorityEditor = () =>
    import(
        /* webpackChunkName: "budgetPriorityEditor" */ '@/components/budget-priority-editor/BudgetPriorityEditor.vue'
    );
const TargetConditionGoalEditor = () =>
    import(
        /* webpackChunkName: "targetConditionGoalEditor" */ '@/components/target-editor/TargetConditionGoalEditor.vue'
    );
const DeficientConditionGoalEditor = () =>
    import(
        /* webpackChunkName: "deficientConditionGoalEditor" */ '@/components/deficient-condition-goal-editor/DeficientConditionGoalEditor.vue'
    );
const Authentication = () =>
    import(
        /* webpackChunkName: "Authentication" */ '@/components/authentication/Authentication.vue'
    );
const AuthenticationFailure = () =>
    import(
        /* webpackChunkName: "authenticationFailure" */ '@/components/authentication/AuthenticationFailure.vue'
    );
const NoRole = () =>
    import(
        /*webpackChunkName: "noRole" */ '@/components/authentication/NoRole.vue'
    );
const Inventory = () =>
    import(/*webpackChunkName: "inventory" */ '@/components/Inventory.vue');
const UserCriteriaEditor = () =>
    import(
        /*webpackChunkName: "userCriteria" */ '@/components/user-criteria/UserCriteria.vue'
    );
const CriterionLibraryEditor = () =>
    import(
        /*webpackChunkName: "criterionLibraryEditor" */ '@/components/criteria-editor/CriterionLibraryEditor.vue'
    );
const AnalysisMethodEditor = () =>
    import(
        /*webpackChunkName: editAnalysis*/ '@/components/scenarios/EditAnalysisMethod.vue'
    );
const RemainingLifeLimitEditor = () =>
    import(
        /*webpackChunkName: remainingLifeLimitEditor*/ '@/components/remaining-life-limit-editor/RemainingLifeLimitEditor.vue'
    );
const CashFlowEditor = () =>
    import(
        /*webpackChunkName: cashFlowEditor*/ '@/components/cash-flow-editor/CashFlowEditor.vue'
    );
const CommittedProjectsEditor = () =>
    import(
        '@/components/committed-projects-editor/CommittedProjectsEditor.vue'
    );
const CalculatedAttributeEditor = () =>
    import(
        /*webpackChunkName: "CalculatedAttributeEditor" */ '@/components/calculated-attribute-editor/CalculatedAttributeEditor.vue'
    );
const EditRawData = () =>
    import(
        /*webpackChunkName: "EditRawData" */ '@/components/raw-data/EditRawData.vue'
    );
const DataSource = () =>
    import(
        /*webpackChunkName: "DataSource" */ '@/components/data-source/DataSource.vue'
    );
const Attributes = () =>
    import(
        /*webpackChunkName: "Attributes" */ '@/components/attributes/Attributes.vue'
    );
const Networks = () =>
    import(
        /*webpackChunkName: "Networks" */ '@/components/networks/Networks.vue'
    );
const EditAdmin = () =>
import(
    /*webpackChunkName: "EditAdmin" */ '@/components/admin/EditAdmin.vue'
);
const Site = () =>
import(
    /*webpackChunkName: "Site" */ '@/components/admin-site-settings/AdminSiteSettingsEditor.vue'
);
const AdminData = () =>
import(
    /*webpackChunkName: "AdminData" */ '@/components/admin-data/AdminData.vue'
);

const onHandlingUnsavedChanges = (to: any, next: any): void => {
    // @ts-ignore
    if (store.state.unsavedChangesFlagModule.hasUnsavedChanges) {
        next();
        // @ts-ignore
        // Vue.dialog
        //     .confirm(
        //         'You have unsaved changes. Are you sure you wish to continue?',
        //         { reverse: true },
        //     )
        //     .then(() => next())
        //     .catch(() => next(false));
    } else {
        next();
    }
};

const beforeEachFunc = (to: any, from: any, next: any) => {
    if (UnsecuredRoutePathNames.indexOf(to.name) === -1) {
        const hasAuthInfo: boolean =
            // @ts-ignore
            store.state.authenticationModule.securityType === SecurityTypes.esec
                ? hasValue(localStorage.getItem('UserTokens'))
                : hasValue(localStorage.getItem('LoggedInUser'));
        // @ts-ignore
        if (!store.state.authenticationModule.authenticated && !hasAuthInfo) {
            next('/AuthenticationStart/');
        } else if (
            // @ts-ignore
            !store.state.authenticationModule.authenticated &&
            hasAuthInfo
        ) {
            /*const isAuthenticatedUser: Promise<boolean | void> =
                // @ts-ignore
                store.state.authenticationModule.securityType ===
                SecurityTypes.esec
                  // @ts-ignore
                    ? isAuthenticatedEsecUser(onHandlingUnsavedChanges(to, next))
                    : isAuthenticatedAzureUser();*/

            isAuthenticatedUser().then((isAuthenticated: boolean | void) => {
                if (isAuthenticated) {
                    onHandlingUnsavedChanges(to, next);
                } else {
                    onHandleLogout();
                }
            });
        } else {
            onHandlingUnsavedChanges(to, next);
        }
    } else {
        next();
    }
};

const beforeEnterFunc = (to: any, from: any, next: any) => {
    if (
        // @ts-ignore
        !store.state.authenticationModule.hasRole ||
        // @ts-ignore
        (!store.state.userModule.currentUserCriteriaFilter.hasAccess &&
            // @ts-ignore
            !store.state.authenticationModule.hasAdminAccess)
    ) {
        next('/NoRole/');
    } else {
        next();
    }
};


const router = createRouter({
    history: createWebHistory(),
    routes: [
        {
            path: '/Inventory/',
            name: 'Inventory',
            component: Inventory,
            beforeEnter: beforeEnterFunc,
        },
        {
            path: '/Scenarios/',
            name: 'Scenarios',
            component: Scenario,
            beforeEnter: beforeEnterFunc,
        },
        {
            path: '/EditScenario/',
            name: 'EditScenario',
            component: EditScenario,
            children: [
                {
                    path: '/EditAnalysisMethod/',
                    name: 'EditAnalysisMethod',
                    component: AnalysisMethodEditor,
                    beforeEnter: beforeEnterFunc,
                },
                {
                    path: '/InvestmentEditor/Scenario/',
                    component: InvestmentEditor,
                    props: true,
                    beforeEnter: beforeEnterFunc,
                },
                {
                    path: '/PerformanceCurveEditor/Scenario/',
                    component: PerformanceCurveEditor,
                    props: true,
                    beforeEnter: beforeEnterFunc,
                },
                {
                    path: '/CalculatedAttributeEditor/Scenario/',
                    component: CalculatedAttributeEditor,
                    props: true,
                },
                {
                    path: '/TreatmentEditor/Scenario/',
                    component: TreatmentEditor,
                    props: true,
                    beforeEnter: beforeEnterFunc,
                },
                {
                    path: '/BudgetPriorityEditor/Scenario/',
                    component: BudgetPriorityEditor,
                    props: true,
                    beforeEnter: beforeEnterFunc,
                },
                {
                    path: '/TargetConditionGoalEditor/Scenario/',
                    component: TargetConditionGoalEditor,
                    props: true,
                    beforeEnter: beforeEnterFunc,
                },
                {
                    path: '/DeficientConditionGoalEditor/Scenario/',
                    component: DeficientConditionGoalEditor,
                    props: true,
                    beforeEnter: beforeEnterFunc,
                },
                {
                    path: '/RemainingLifeLimitEditor/Scenario',
                    component: RemainingLifeLimitEditor,
                    props: true,
                    beforeEnter: beforeEnterFunc,
                },
                {
                    path: '/CashFlowEditor/Scenario',
                    component: CashFlowEditor,
                    props: true,
                    beforeEnter: beforeEnterFunc,
                },
                {
                    path: '/CommittedProjectsEditor/Scenario',
                    component: CommittedProjectsEditor,
                    props: true,
                    beforeEnter: beforeEnterFunc
                },
                {
                    path: '/ReportsAndOutputs/Scenario',
                    component: ReportsAndOutputs,
                    props: true,
                    beforeEnter: beforeEachFunc,
                },
            ],
        },
        {
            path: '/EditLibrary/',
            name: 'EditLibrary',
            component: EditLibrary,
            children: [
                {
                    path: '/InvestmentEditor/Library/',
                    component: InvestmentEditor,
                },
                {
                    path: '/PerformanceCurveEditor/Library/',
                    component: PerformanceCurveEditor,
                },
                {
                    path: '/CalculatedAttributeEditor/Library/',
                    component: CalculatedAttributeEditor,
                },
                {
                    path: '/TreatmentEditor/Library/',
                    component: TreatmentEditor,
                },
                {
                    path: '/BudgetPriorityEditor/Library/',
                    component: BudgetPriorityEditor,
                },
                {
                    path: '/TargetConditionGoalEditor/Library/',
                    component: TargetConditionGoalEditor,
                },
                {
                    path: '/DeficientConditionGoalEditor/Library/',
                    component: DeficientConditionGoalEditor,
                },
                {
                    path: '/RemainingLifeLimitEditor/Library/',
                    component: RemainingLifeLimitEditor,
                },
                {
                    path: '/CashFlowEditor/Library/',
                    component: CashFlowEditor,
                },
                {
                    path: '/CommittedProjectsEditor/Library',
                    component: CommittedProjectsEditor,
                },
            ],
        },
        {
            path: '/EditRawData/',
            name: 'EditRawData',
            component: EditRawData,
            children: [
                {
                    path: '/DataSource/',
                    component: DataSource,
                },
                {
                    path: '/Attributes/',
                    component: Attributes,
                },
                {
                    path: '/Networks/',
                    component: Networks,
                },
            ],
        },
        {
            path: '/EditAdmin/',
            name: 'EditAdmin',
            component: EditAdmin,
            children: [
                {
                    path: '/UserCriteria/',
                    component: UserCriteriaEditor,
                },
                {
                    path: '/AdminData/',
                    component: AdminData,
                },
                {
                    path: '/Site/',
                    component: Site,
                },
            ],
        },
        {
            path: '/DataSource/',
            name: 'DataSource',
            component: DataSource,
            props: true,
        },
        {
            path: '/Attributes/',
            name: 'Attributes',
            component: Attributes,
            props: true,
        },
        {
            path: '/Site/',
            name: 'Site',
            component: Site,
            props: true,
        },
        {
            path: '/AdminData/',
            name: 'AdminData',
            component: AdminData,
            props: true,
        },
        {
            path: '/Networks/',
            name: 'Networks',
            component: Networks,
            props: true,
        },
        {
            path: '/InvestmentEditor/Library/',
            name: 'InvestmentEditor',
            component: InvestmentEditor,
            props: true,
            beforeEnter: beforeEnterFunc,
        },
        {
            path: '/PerformanceCurveEditor/Library/',
            name: 'PerformanceEditor',
            component: PerformanceCurveEditor,
            props: true,
            beforeEnter: beforeEnterFunc,
        },
        {
            path: '/TreatmentEditor/Library/',
            name: 'TreatmentEditor',
            component: TreatmentEditor,
            props: true,
            beforeEnter: beforeEnterFunc,
        },
        {
            path: '/BudgetPriorityEditor/Library/',
            name: 'BudgetPriorityEditor',
            component: BudgetPriorityEditor,
            props: true,
            beforeEnter: beforeEnterFunc,
        },
        {
            path: '/TargetConditionGoalEditor/Library/',
            name: 'TargetConditionGoalEditor.vue',
            component: TargetConditionGoalEditor,
            props: true,
            beforeEnter: beforeEnterFunc,
        },
        {
            path: '/DeficientConditionGoalEditor/Library/',
            name: 'DeficientConditionGoalEditor',
            component: DeficientConditionGoalEditor,
            props: true,
            beforeEnter: beforeEnterFunc,
        },
        {
            path: '/RemainingLifeLimitEditor/Library/',
            name: 'RemainingLifeLimitEditor',
            component: RemainingLifeLimitEditor,
            props: true,
            beforeEnter: beforeEnterFunc,
        },
        {
            path: '/CashFlowEditor/Library/',
            name: 'CashFlowEditor',
            component: CashFlowEditor,
            props: true,
            beforeEnter: beforeEnterFunc,
        },
        {
            path: '/CommittedProjectsEditor/Library/',
            name: 'CommittedProjectsEditor',
            component: CommittedProjectsEditor,
            props: true,
            beforeEnter: beforeEnterFunc
        },
        {
            path: '/CriterionLibraryEditor/Library/',
            name: 'CriterionLibraryEditor.vue',
            component: CriterionLibraryEditor,
            beforeEnter: beforeEnterFunc,
        },
        {
            path: '/Authentication/',
            name: 'Authentication',
            component: Authentication,
        },
        {
            path: '/AuthenticationStart/',
            name: 'AuthenticationStart',
            component: AuthenticationStart,
        },
        {
            path: '/AuthenticationFailure/',
            name: 'AuthenticationFailure',
            component: AuthenticationFailure,
        },
        {
            path: '/NoRole/',
            name: 'NoRole',
            component: NoRole,
        },
        {
            path: '/UnderConstruction/',
            name: 'UnderConstruction',
            component: UnderConstruction,
        },
        {
            path: '/iAM/',
            name: 'iAM',
            component: Logout,
        },
        {
            path: '/UserCriteria/',
            name: 'UserCriteria',
            component: UserCriteriaEditor,
        },
        {
            path: '/CalculatedAttributeEditor/Library/',
            name: 'CalculatedAttributeEditor',
            component: CalculatedAttributeEditor,
            props: true,
        },
        {
            path: '/',
            component: Scenario,
        }
        // TODO: Does below need to be here?
        // {
        //     path: '*',
        //     redirect: '/AuthenticationStart/',
        // },
    ],
});

router.beforeEach(beforeEachFunc);

export default router;
