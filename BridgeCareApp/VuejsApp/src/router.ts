import Vue from 'vue';
import VueRouter from 'vue-router';
import './register-hooks';
import EditAnalysisMethod from '@/components/scenarios/EditAnalysisMethod.vue';
import UnderConstruction from '@/components/UnderConstruction.vue';
import Logout from '@/components/Logout.vue';
import Home from '@/components/Home.vue';
import AuthenticationStart from '@/components/authentication/AuthenticationStart.vue';
import { hasValue } from '@/shared/utils/has-value-util';
import { UnsecuredRoutePathNames } from '@/shared/utils/route-paths';
import { SecurityTypes } from '@/shared/utils/security-types';
import store from '@/store/root-store';
import {
    isAuthenticatedUser,
    onHandleLogout,
} from '@/shared/utils/authentication-utils';

// Lazily-loaded pages
const Scenario = () =>
    import(
        /* webpackChunkName: "scenario" */ '@/components/scenarios/Scenarios.vue'
    );
const EditScenario = () =>
    import(
        /* webpackChunkName: "editScenario" */ '@/components/scenarios/EditScenario.vue'
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

const onHandlingUnsavedChanges = (to: any, next: any): void => {
    // @ts-ignore
    if (store.state.unsavedChangesFlagModule.hasUnsavedChanges) {
        // @ts-ignore
        Vue.dialog
            .confirm(
                'You have unsaved changes. Are you sure you wish to continue?',
                { reverse: true },
            )
            .then(() => next())
            .catch(() => next(false));
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
            !store.state.authenticationModule.isAdmin)
    ) {
        next('/NoRole/');
    } else {
        next();
    }
};

Vue.use(VueRouter);

const router = new VueRouter({
    mode: 'history',
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
                    path: '/RemainingLifeLimitEditor/Scenario/',
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
            ],
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
            path: '/Home/',
            name: 'Home',
            component: Home,
        },
        {
            path: '/UserCriteria/',
            name: 'UserCriteria',
            component: UserCriteriaEditor,
        },
        {
            path: '*',
            redirect: '/AuthenticationStart/',
        },
    ],
});

router.beforeEach(beforeEachFunc);

export default router;
