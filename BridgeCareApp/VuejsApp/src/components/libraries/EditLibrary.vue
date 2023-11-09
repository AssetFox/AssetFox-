<template>
        <v-row>
            <v-col class="p-0">
            <v-card
                class="ghd-sidebar-libary"
                height="100%"
                elevation="0"
                style="border-top-left-radius: 10px; border-bottom-left-radius: 10px; border: 1px solid #999999;"
            >
                <v-list class="ghd-navigation-list">
                    <v-list-item
                        class="settings-list ghd-control-text"
                        :key="navigationTab.tabName"
                        :model-value="navigationTab"
                        v-for="navigationTab in visibleNavigationTabs()"                        
                    >
                        <v-list-item :to="navigationTab.navigation" style="border-bottom: 1px solid #CCCCCC;">
                            <template v-slot:prepend>
                                    <!-- <v-icon class="mx-2" slot="prependIcon" v-text="navigationTab.tabIcon"></v-icon> -->
                                    <TreatmentSvg style="height: 38px; width: 34px"  class="library-icon" v-if="navigationTab.tabName === 'Treatment'"/>  
                                    <TargetConditionGoalSvg style="height: 38px; width: 34px"  class="library-icon" v-if="navigationTab.tabName === 'Target Condition Goal'"/>  
                                    <RemainingLifeLimitSvg style="height: 38px; width: 34px"  class="library-icon" v-if="navigationTab.tabName === 'Remaining Life Limit'"/>  
                                    <PerformanceCurveSvg style="height: 34px; width: 36px"  class="library-icon" v-if="navigationTab.tabName === 'Deterioration Model'"/>  
                                    <DeficientConditionGoalSvg style="height: 38px; width: 34px"  class="library-icon" v-if="navigationTab.tabName === 'Deficient Condition Goal'"/>  
                                    <InvestmentSvg style="height: 38px; width: 34px"  class="library-icon" v-if="navigationTab.tabName === 'Investment'"/>  
                                    <CashFlowSvg style="height: 38px; width: 34px"  class="library-icon" v-if="navigationTab.tabName === 'Cash Flow'"/>  
                                    <BudgetPrioritySvg style="height: 38px; width: 34px"  class="library-icon" v-if="navigationTab.tabName === 'Budget Priority'"/>  
                                    <CalculatedAttributeSvg style="height: 32px; width: 32px"  class="library-icon-stroke" v-if="navigationTab.tabName === 'Calculated Attribute'"/>  
                            </template>
                                <v-list-item-title style="text-decoration: none">{{navigationTab.tabName}}</v-list-item-title>
                        </v-list-item>
                    </v-list-item>
                </v-list>
            </v-card>
            <v-col cols = "12" class="ghd-content">
                <v-container fluid grid-list-xs style="padding-left:20px;padding-right:20px;">
                    <router-view></router-view>
                </v-container>
            </v-col>
        </v-col>
        </v-row>
</template>

<script lang="ts" setup>
import { ref, computed } from 'vue'
import { any } from 'ramda';
import { NavigationTab } from '@/shared/models/iAM/navigation-tab';
import { getBlankGuid } from '@/shared/utils/uuid-utils';
import BudgetPrioritySvg from '@/shared/icons/BudgetPrioritySvg.vue';
import CashFlowSvg from '@/shared/icons/CashFlowSvg.vue';
import InvestmentSvg from '@/shared/icons/InvestmentSvg.vue';
import DeficientConditionGoalSvg from '@/shared/icons/DeficientConditionGoalSvg.vue';
import PerformanceCurveSvg from '@/shared/icons/PerformanceCurveSvg.vue';
import RemainingLifeLimitSvg from '@/shared/icons/RemainingLifeLimitSvg.vue';
import TargetConditionGoalSvg from '@/shared/icons/TargetConditionGoalSvg.vue';
import TreatmentSvg from '@/shared/icons/TreatmentSvg.vue';
import CalculatedAttributeSvg from '@/shared/icons/CalculatedAttributeSvg.vue';
import { useStore } from 'vuex'; 

    let store = useStore(); 

    const hasAdminAccess = computed<boolean>(() => store.state.authenticationModule.hasAdminAccess) ; 
    let userId = ref<string>(store.state.authenticationModule.userId);

    let networkId: string = getBlankGuid();
    let networkName: string = '';
    let navigationTabs: NavigationTab[] = [
        {
            tabName: 'Investment',
            tabIcon: 'fas fa-dollar-sign',
            navigation: {
                path: '/InvestmentEditor/Library/',
            },
        },
        {
            tabName: 'Deterioration Model',
            tabIcon: 'fas fa-chart-line',
            navigation: {
                path: '/PerformanceCurveEditor/Library/',
            },
        },
        {
            tabName: 'Calculated Attribute',
            tabIcon: 'fas fa-plus-square',
            navigation: {
                path: '/CalculatedAttributeEditor/Library/',
            },
        },
        {
            tabName: 'Treatment',
            tabIcon: 'fas fa-tools',
            navigation: {
                path: '/TreatmentEditor/Library/',
            },
        },
        {
            tabName: 'Budget Priority',
            tabIcon: 'fas fa-copy',
            navigation: {
                path: '/BudgetPriorityEditor/Library/',
            },
        },
        {
            tabName: 'Target Condition Goal',
            tabIcon: 'fas fa-bullseye',
            navigation: {
                path: '/TargetConditionGoalEditor/Library/',
            },
        },
        {
            tabName: 'Deficient Condition Goal',
            tabIcon: 'fas fa-level-down-alt',
            navigation: {
                path: '/DeficientConditionGoalEditor/Library/',
            },
        },
        {
            tabName: 'Remaining Life Limit',
            tabIcon: 'fas fa-business-time',
            navigation: {
                path: '/RemainingLifeLimitEditor/Library/',
            },
        },
        {
            tabName: 'Cash Flow',
            tabIcon: 'fas fa-money-bill-wave',
            navigation: {
                path: '/CashFlowEditor/Library/',
            },
        },
    ];

    created();
    //beforeRouteEnter(to: any, from: any, next: any) {   
    function created() { 
        //next((vm: any) => {
                navigationTabs = navigationTabs.map(
                    (navTab: NavigationTab) => {
                        const navigationTab = {
                            ...navTab,
                            navigation: {
                                ...navTab.navigation,
                                query: {
                                },
                            },
                        };

                        if (navigationTab.tabName === 'Remaining Life Limit' 
                            || navigationTab.tabName === 'Target Condition Goal' 
                            || navigationTab.tabName === 'Deficient Condition Goal' 
                            || navigationTab.tabName === 'Calculated Attribute') {
                            navigationTab['visible'] = hasAdminAccess.value;
                        }

                        return navigationTab;
                    },
                );

                // get the window href
                const href = window.location.href;
                // check each NavigationTab object to see if it has a matching navigation path with the href
                const hasChildPath = any(
                    (navigationTab: NavigationTab) =>
                        href.indexOf(navigationTab.navigation.path) !== -1,
                    navigationTabs,
                );
        //});
    }


    function visibleNavigationTabs() {
        return navigationTabs.filter(
            navigationTab =>
                navigationTab.visible === undefined || navigationTab.visible,
        );
    }

</script>

<style>
.child-router-div {
    height: 100%;
    overflow: auto;
}

.edit-scenario-btns-div {
    display: flex;
}
.settings-list a:hover {
    text-decoration: none;
}

.text-primary .library-icon{
    fill: #FFFFFF !important;
}

.library-icon {
    fill: #999999 !important;
}

.text-primary .library-icon-stroke{
    stroke: #FFFFFF !important;
}

.library-icon-stroke {
    stroke: #999999 !important;
}

</style>
