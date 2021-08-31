import Vue from 'vue';
import VueRouter from 'vue-router';
import './register-hooks';
import EditAnalysisMethod from '@/components/scenarios/EditAnalysisMethod.vue';
import UnderConstruction from '@/components/UnderConstruction.vue';
import Logout from '@/components/Logout.vue';
import Home from '@/components/Home.vue';
import AuthenticationStart from '@/components/authentication/AuthenticationStart.vue';
import { hasValue } from '@/shared/utils/has-value-util';
import authenticationModule from '@/store-modules/authentication.module';

// Lazily-loaded pages
const Scenario = () => import(/* webpackChunkName: "scenario" */ '@/components/scenarios/Scenarios.vue');
const EditScenario = () => import(/* webpackChunkName: "editScenario" */ '@/components/scenarios/EditScenario.vue');
const InvestmentEditor = () => import(/* webpackChunkName: "investmentModule" */ '@/components/investment-editor/InvestmentEditor.vue');
const PerformanceCurveEditor = () => import(/* webpackChunkName: "performanceCurveEditor" */ '@/components/performance-curve-editor/PerformanceCurveEditor.vue');
const TreatmentEditor = () => import(/* webpackChunkName: "treatmentEditor" */ '@/components/treatment-editor/TreatmentEditor.vue');
const BudgetPriorityEditor = () => import (/* webpackChunkName: "budgetPriorityEditor" */ '@/components/budget-priority-editor/BudgetPriorityEditor.vue');
const TargetConditionGoalEditor = () => import (/* webpackChunkName: "targetConditionGoalEditor" */ '@/components/target-editor/TargetConditionGoalEditor.vue');
const DeficientConditionGoalEditor = () => import (/* webpackChunkName: "deficientConditionGoalEditor" */ '@/components/deficient-condition-goal-editor/DeficientConditionGoalEditor.vue');
const Authentication = () => import (/* webpackChunkName: "Authentication" */ '@/components/authentication/Authentication.vue');
const AuthenticationFailure = () => import (/* webpackChunkName: "authenticationFailure" */ '@/components/authentication/AuthenticationFailure.vue');
const NoRole = () => import (/*webpackChunkName: "noRole" */ '@/components/authentication/NoRole.vue');
const Inventory = () => import (/*webpackChunkName: "inventory" */ '@/components/Inventory.vue');
const UserCriteriaEditor = () => import (/*webpackChunkName: "userCriteria" */ '@/components/user-criteria/UserCriteria.vue');
const CriterionLibraryEditor = () => import(/*webpackChunkName: "criterionLibraryEditor" */ '@/components/criteria-editor/CriterionLibraryEditor.vue');
const AnalysisMethodEditor = () => import (/*webpackChunkName: editAnalysis*/ '@/components/scenarios/EditAnalysisMethod.vue');
const RemainingLifeLimitEditor = () => import (/*webpackChunkName: remainingLifeLimitEditor*/ '@/components/remaining-life-limit-editor/RemainingLifeLimitEditor.vue');
const CashFlowEditor = () => import (/*webpackChunkName: cashFlowEditor*/ '@/components/cash-flow-editor/CashFlowEditor.vue');
const CalculatedAttributeEditor = () => import (/*webpackChunkName: "CalculatedAttributeEditor" */ '@/components/calculated-attribute-editor/CalculatedAttributeEditor.vue');

Vue.use(VueRouter);

const router = new VueRouter({
    mode: 'history',
    routes: [
        {
            path: '/Inventory/',
            name: 'Inventory',
            component: Inventory
        },
        {
            path: '/Scenarios/',
            name: 'Scenarios',
            component: Scenario,
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
                },
                {
                    path: '/InvestmentEditor/Scenario/',
                    component: InvestmentEditor,
                    props: true
                },
                {
                    path: '/PerformanceCurveEditor/Scenario/',
                    component: PerformanceCurveEditor,
                    props: true
                },
                {
                    path: '/CalculatedAttributeEditor/Scenario/',
                    component: CalculatedAttributeEditor,
                    props: true
                },
                {
                    path: '/TreatmentEditor/Scenario/',
                    component: TreatmentEditor,
                    props: true
                },
                {
                    path: '/BudgetPriorityEditor/Scenario/',
                    component: BudgetPriorityEditor,
                    props: true
                },
                {
                    path: '/TargetConditionGoalEditor/Scenario/',
                    component: TargetConditionGoalEditor,
                    props: true
                },
                {
                    path: '/DeficientConditionGoalEditor/Scenario/',
                    component: DeficientConditionGoalEditor,
                    props: true
                },
                {
                    path: '/RemainingLifeLimitEditor/Scenario/',
                    component: RemainingLifeLimitEditor,
                    props: true
                },
                {
                    path: '/CashFlowEditor/Scenario',
                    component: CashFlowEditor,
                    props: true
                }
            ]
        },
        {
            path: '/InvestmentEditor/Library/',
            name: 'InvestmentEditor',
            component: InvestmentEditor,
            props: true
        },
        {
            path: '/PerformanceCurveEditor/Library/',
            name: 'PerformanceEditor',
            component: PerformanceCurveEditor,
            props: true
        },
        {
            path: '/TreatmentEditor/Library/',
            name: 'TreatmentEditor',
            component: TreatmentEditor,
            props: true
        },
        {
            path: '/BudgetPriorityEditor/Library/',
            name: 'BudgetPriorityEditor',
            component: BudgetPriorityEditor,
            props: true
        },
        {
            path: '/TargetConditionGoalEditor/Library/',
            name: 'TargetConditionGoalEditor.vue',
            component: TargetConditionGoalEditor,
            props: true
        },
        {
            path: '/DeficientConditionGoalEditor/Library/',
            name: 'DeficientConditionGoalEditor',
            component: DeficientConditionGoalEditor,
            props: true
        },
        {
            path: '/RemainingLifeLimitEditor/Library/',
            name: 'RemainingLifeLimitEditor',
            component: RemainingLifeLimitEditor,
            props: true
        },
        {
            path: '/CashFlowEditor/Library/',
            name: 'CashFlowEditor',
            component: CashFlowEditor,
            props: true
        },
        {
            path: '/CriterionLibraryEditor/Library/',
            name: 'CriterionLibraryEditor.vue',
            component: CriterionLibraryEditor,
        },
        {
            path: '/Authentication/',
            name: 'Authentication',
            component: Authentication
        },
        {
            path: '/AuthenticationStart/',
            name: 'AuthenticationStart',
            component: AuthenticationStart
        },
        {
            path: '/AuthenticationFailure/',
            name: 'AuthenticationFailure',
            component: AuthenticationFailure
        },
        {
            path: '/NoRole/',
            name: 'NoRole',
            component: NoRole
        },
        {
            path: '/UnderConstruction/',
            name: 'UnderConstruction',
            component: UnderConstruction
        },
        {
            path: '/iAM/',
            name: 'iAM',
            component: Logout
        },
        {
            path: '/Home/',
            name: 'Home',
            component: Home
        },
        {
            path: '/UserCriteria/',
            name: 'UserCriteria',
            component: UserCriteriaEditor
        },
        {
            path: '/CalculatedAttributeEditor/Library/',
            name: 'CalculatedAttributeEditor',
            component: CalculatedAttributeEditor,
            props: true
        },
        {
            path: '*',
            redirect: '/AuthenticationStart/'
        }
    ]
});

const routesToIgnore: string[] = ['AuthenticationStart', 'iAM', 'Authentication'];

router.beforeEach((to: any, from: any, next) => {
    if (authenticationModule.state.securityType === authenticationModule.state.azureSecurityType &&
      !hasValue(localStorage.getItem('LoggedInUser')) && routesToIgnore.indexOf(to.name) === -1) {
        next('/AuthenticationStart');
    } else {
        next();
    }
});

export default router;
