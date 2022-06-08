<template>
    <v-layout column>
        <v-layout>
            <v-card
                class="mx-auto ghd-sidebar-libary"
                height="100%"
                elevation="0"
                style="border-top-left-radius: 10px; border-bottom-left-radius: 10px; border: 1px solid #999999;"
            >
                <v-list class="ghd-navigation-list">
                    <v-list-item-group
                        class="settings-list ghd-control-text"
                        :key="navigationTab.tabName"
                        v-for="navigationTab in visibleNavigationTabs()"
                    >
                        <v-list-tile :to="navigationTab.navigation" style="border-bottom: 1px solid #CCCCCC;">
                            <v-list-tile-action>
                                <v-list-tile-icon>
                                    <v-icon class="mx-2" slot="prependIcon" v-text="navigationTab.tabIcon"></v-icon>
                                </v-list-tile-icon>
                            </v-list-tile-action>
                            <v-list-tile-content>
                                <v-list-tile-title style="text-decoration: none">{{navigationTab.tabName}}</v-list-tile-title>
                            </v-list-tile-content>
                        </v-list-tile>
                    </v-list-item-group>
                </v-list>
            </v-card>
            <v-flex xs12 class="ghd-content">
                <v-container fluid grid-list-xs style="padding-left:20px;padding-right:20px;">
                    <router-view></router-view>
                </v-container>
            </v-flex>
        </v-layout>
    </v-layout>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import { Action, State } from 'vuex-class';
import { any, clone, isNil, propEq } from 'ramda';
import { Network } from '@/shared/models/iAM/network';
import { NavigationTab } from '@/shared/models/iAM/navigation-tab';
import { getBlankGuid } from '@/shared/utils/uuid-utils';

@Component({
})
export default class EditLibrary extends Vue {
    @State(state => state.authenticationModule.isAdmin) isAdmin: boolean;
    @State(state => state.authenticationModule.userId) userId: string;

    networkId: string = getBlankGuid();
    networkName: string = '';
    navigationTabs: NavigationTab[] = [
        {
            tabName: 'Investment',
            tabIcon: 'fas fa-dollar-sign',
            navigation: {
                path: '/InvestmentEditor/Library',
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

    
    beforeRouteEnter(to: any, from: any, next: any) {
        next((vm: any) => {
                vm.navigationTabs = vm.navigationTabs.map(
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
                            navigationTab['visible'] = vm.isAdmin;
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
                    vm.navigationTabs,
                );
                // if no matching navigation path was found in the href, then route with path of first navigationTabs entry
                if (!hasChildPath) {
                    vm.$router.push(vm.navigationTabs[0].navigation);
                }
        });
    }


    visibleNavigationTabs() {
        return this.navigationTabs.filter(
            navigationTab =>
                navigationTab.visible === undefined || navigationTab.visible,
        );
    }
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

</style>
