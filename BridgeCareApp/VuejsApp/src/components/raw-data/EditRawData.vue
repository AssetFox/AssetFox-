<template>
    <v-row column>
        <v-row>
            <v-card
                class="mx-auto ghd-sidebar-raw-data"
                height="100%"
                elevation="0"
            >
                <div class="raw-data-list-header" style="padding-right: 175px !important">
                    Raw Data
                </div>
                <v-list class="ghd-navigation-list">
                    <v-list-item
                        class="settings-list ghd-control-text"
                        :key="navigationTab.tabName"
                        v-for="navigationTab in visibleNavigationTabs()"
                    >
                    <v-list-item id="EditScenario-tabs-vListTile" :to="navigationTab.navigation" style="border-bottom: 1px solid #CCCCCC;">
                            <template v-slot:prepend>
                                <AttributesSvg id="EditRawData-attributes-button" style="height: 38px; width: 34px"  class="raw-data-icon"  v-if="navigationTab.tabName === 'Attributes'"/>    
                                    <DataSourceSvg id="EditRawData-dataSource-button" style="height: 30px; width: 36px" class="raw-data-icon"  v-if="navigationTab.tabName === 'DataSource'"/>
                                    <NetworksSvg  id="EditRawData-networks-button" style="height: 34px; width: 34px" class="raw-data-icon"  v-if="navigationTab.tabName === 'Networks'"/>                                                  
                            </template>
                            <v-list-item-title style="width: auto; padding-left: 5px;" v-text="navigationTab.tabName"></v-list-item-title>
                        </v-list-item>
                        
                    </v-list-item>
                </v-list>
            </v-card>
            <v-col cols = "12" class="ghd-content">
                <v-container fluid grid-list-xs style="padding-left:20px;padding-right:20px;">
                    <router-view></router-view>
                </v-container>
            </v-col>
        </v-row>
    </v-row>
</template>

<script setup lang="ts">
import Vue, { ref } from 'vue';
import { any, clone, isNil, propEq } from 'ramda';
import { Network } from '@/shared/models/iAM/network';
import { NavigationTab } from '@/shared/models/iAM/navigation-tab';
import { getBlankGuid } from '@/shared/utils/uuid-utils';
import AttributesSvg from '@/shared/icons/AttributesSvg.vue';
import DataSourceSvg from '@/shared/icons/DataSourceSvg.vue';
import NetworksSvg from '@/shared/icons/NetworksSvg.vue';
import { useStore } from 'vuex';
import {useRouter} from 'vue-router'
import router from '@/router';


    let store = useStore();
    let hasAdminAccess: boolean = store.state.authenticationModule.hasAdminAccess;
    let userId: string = store.state.authenticationModule.userId;


    let networkId: string = getBlankGuid();
    let networkName: string = '';
    let navigationTabs: NavigationTab[] = [
        {
            tabName: 'DataSource',
            tabIcon: "",
            navigation: {
                path: '/DataSource/',
            },
        },
        {
            tabName: 'Attributes',
            tabIcon: "",
            navigation: {
                path: '/Attributes/',
            },
        },
        {
            tabName: 'Networks',
            tabIcon: "",
            navigation: {
                path: '/Networks/',
            },
        },
    ];
    
    beforeRouteEnter();

    function beforeRouteEnter() {
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
                    navigationTab['visible'] = hasAdminAccess;
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

    }
    
    function onNavigate(route: any) {
        if (router.currentRoute.value.path !== route.path) {
            router.push(route).catch(() => {});
        }
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
