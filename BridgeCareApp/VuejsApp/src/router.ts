﻿import Vue from 'vue';
import VueRouter from 'vue-router';
import './register-hooks';
import EditAnalysis from '@/components/scenarios/EditAnalysis.vue';
import UnderConstruction from '@/components/UnderConstruction.vue';
import Logout from '@/components/Logout.vue';
import Home from '@/components/Home.vue';
import AuthenticationStart from '@/components/authentication/AuthenticationStart.vue';

// Lazily-loaded pages
const Scenario = () => import(/* webpackChunkName: "scenario" */ '@/components/scenarios/Scenarios.vue');
const EditScenario = () => import(/* webpackChunkName: "editScenario" */ '@/components/scenarios/EditScenario.vue');
const InvestmentEditor = () => import(/* webpackChunkName: "investmentEditor" */ '@/components/investment-editor/InvestmentEditor.vue');
const PerformanceCurvesEditor = () => import(/* webpackChunkName: "performanceCurvesEditor" */ '@/components/performance-curve-editor/PerformanceCurvesEditor.js');
const TreatmentEditor = () => import(/* webpackChunkName: "treatmentEditor" */ '@/components/treatment-editor/TreatmentEditor.vue');
const PriorityEditor = () => import (/* webpackChunkName: "priorityEditor" */ '@/components/priority-editor/PriorityEditor.vue');
const TargetConditionGoalEditor = () => import (/* webpackChunkName: "targetEditor" */ '@/components/target-editor/TargetConditionGoalEditor.vue');
const DeficientConditionGoalEditor = () => import (/* webpackChunkName: "deficientEditor" */ '@/components/deficient-condition-goal-editor/DeficientConditionGoalEditor.vue');
const Authentication = () => import (/* webpackChunkName: "Authentication" */ '@/components/authentication/Authentication.vue');
const AuthenticationFailure = () => import (/* webpackChunkName: "authenticationFailure" */ '@/components/authentication/AuthenticationFailure.vue');
const NoRole = () => import (/*webpackChunkName: "noRole" */ '@/components/authentication/NoRole.vue');
const Inventory = () => import (/*webpackChunkName: "inventory" */ '@/components/Inventory.vue');
const UserCriteriaEditor = () => import (/*webpackChunkName: "userCriteria" */ '@/components/user-criteria/UserCriteria.vue');
const CriterionLibraryEditor = () => import(/*webpackChunkName: "criterionLibraryEditor" */ '@/components/criteria-editor/CriterionLibraryEditor.vue');
const AnalysisEditor = () => import (/*webpackChunkName: editAnalysis*/ '@/components/scenarios/EditAnalysis.vue');
const RemainingLifeLimitEditor = () => import (/*webpackChunkName: remainingLifeLimitEditor*/ '@/components/remaining-life-limit-editor/RemainingLifeLimitEditor.vue');
const CashFlowEditor = () => import (/*webpackChunkName: cashFlowEditor*/ '@/components/cash-flow-editor/CashFlowEditor.vue');

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
                    path: '/EditAnalysis/',
                    name: 'EditAnalysis',
                    component: AnalysisEditor,
                },
                {
                    path: '/InvestmentEditor/Scenario/',
                    component: InvestmentEditor,
                    props: true
                },
                {
                    path: '/PerformanceCurveEditor/Scenario/',
                    component: PerformanceCurvesEditor,
                    props: true
                },
                {
                    path: '/TreatmentEditor/Scenario/',
                    component: TreatmentEditor,
                    props: true
                },
                {
                    path: '/PriorityEditor/Scenario/',
                    component: PriorityEditor,
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
            component: PerformanceCurvesEditor,
            props: true
        },
        {
            path: '/TreatmentEditor/Library/',
            name: 'TreatmentEditor',
            component: TreatmentEditor,
            props: true
        },
        {
            path: '/PriorityEditor/Library/',
            name: 'PriorityEditor',
            component: PriorityEditor,
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
            path: '*',
            redirect: '/AuthenticationStart/'
        }
    ]
});


export default router;
