<template>
    <v-row column>
        <v-row>
            <v-card
                class="mx-auto ghd-sidebar-raw-data"
                height="100%"
                elevation="0"
            >
                <div class="raw-data-list-header" style="padding-right: 125px !important">
                    Administration
                </div>
                <v-list class="ghd-navigation-list">
                    <v-list-item
                        class="settings-list ghd-control-text"
                        :key="navigationTabs.tabName"
                        :model-value="navigationTabs"
                        v-for="navigationTabs in visibleNavigationTabs()"
                    >
                        <v-list-tile :to="navigationTabs.navigation" style="border-bottom: 1px solid #CCCCCC;" @click="onNavigate(navigationTabs.navigation)">
                            <v-list-tile-action>
                                <v-list-tile-icon class="sidebar-icon">
                                    <AttributesSvg style="height: 38px; width: 34px"  class="raw-data-icon" v-if="navigationTabs.tabName === 'Security'"/>    
                                    <DataSourceSvg style="height: 30px; width: 36px" class="raw-data-icon" v-if="navigationTabs.tabName === 'Site'"/>
                                    <NetworksSvg  style="height: 34px; width: 34px" class="raw-data-icon" v-if="navigationTabs.tabName === 'Data'"/>                            
                                    <NetworksSvg  style="height: 34px; width: 34px" class="raw-data-icon" v-if="navigationTabs.tabName === 'RawData'"/>                            
                                </v-list-tile-icon>
                            </v-list-tile-action>                            
                            <v-list-tile-title style="text-decoration: none">{{navigationTabs.tabName}}</v-list-tile-title>                            
                        </v-list-tile>
                    </v-list-item>
                </v-list>
            </v-card>
            <v-col cols ="12" class="ghd-content">
                <v-container fluid grid-list-xs style="padding-left:20px;padding-right:20px;">
                    <router-view></router-view>
                </v-container>
            </v-col>
        </v-row>
    </v-row>
</template>

<script lang="ts" setup>
import Vue from 'vue';
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import router from '@/router';
import { any, clone, isNil, propEq } from 'ramda';
import { NavigationTab } from '@/shared/models/iAM/navigation-tab';
import { getBlankGuid } from '@/shared/utils/uuid-utils';
import AttributesSvg from '@/shared/icons/AttributesSvg.vue';
import DataSourceSvg from '@/shared/icons/DataSourceSvg.vue';
import NetworksSvg from '@/shared/icons/NetworksSvg.vue';

import { createDecipheriv } from 'crypto';

    let store = useStore();
    let hasAdminAccess = ref<boolean>(store.state.authenticationModule.hasAdminAccess);
    let userID = ref<string>(store.state.authenticationModule.userID);
    let networkId: string = getBlankGuid();
    let networkName: string = '';
    let navigationTabs: NavigationTab[] = [
        {
            tabName: 'Security',
            tabIcon: "",
            navigation: {
                path: '/UserCriteria/',
            },
        },
        {
            tabName: 'Site',
            tabIcon: "",
            navigation: {
                path: '/Site/',
            },
        },
        {
            tabName: 'Data',
            tabIcon: "",
            navigation: {
                path: '/AdminData/',
            },
        },
    ];
    created();
    function created(){
        (() => {
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

                        if (navigationTab.tabName === 'DataSource' 
                            || navigationTab.tabName === 'Networks' 
                            || navigationTab.tabName === 'Attributes') {
                            navigationTab['visible'] =hasAdminAccess.value;
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
        });
    }
     function visibleNavigationTabs() {
        return navigationTabs.filter(
            navigationTab =>
                navigationTab.visible === undefined || navigationTab.visible,
        );
    }
    function onNavigate(route: any) {
        if (router.currentRoute.value.path !== route.path) {
            router.push(route).catch(() => {});
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
    color: black;
}

.settings-list a:active {
    text-decoration: none;
}

.settings-list a:focus {
    text-decoration: none;
}

.raw-data-list-header {
    font-family: 'Montserrat' !important;
    font-weight: 500 !important;
    font-size: 18px !important;
    background-color: #607C9F !important;
    color: #FFFFFF !important;
    padding-top: 10px !important;
    padding-bottom: 10px !important;
    padding-left: 20px !important;
}

.text-primary .selected-sidebar-icon .v-icon{
    visibility: visible !important;
}

.selected-sidebar-icon .v-icon{
    visibility: hidden !important;
}

.text-primary .raw-data-icon{
    stroke: #FFFFFF !important;
}

.raw-data-icon {
    stroke: #999999 !important;
}

.raw-data-svg-fill {
    fill: #FFFFFF;
}

.text-primary .raw-data-svg-fill {
    fill: #2A578D;
}
</style>