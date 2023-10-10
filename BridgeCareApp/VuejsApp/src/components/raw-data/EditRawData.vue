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
                        :model-value="navigationTab"
                        v-for="navigationTab in visibleNavigationTabs()"
                    >
                        <v-list-tile :to="navigationTab.navigation" style="border-bottom: 1px solid #CCCCCC;">
                            <v-list-tile-action>
                                <v-list-tile-icon class="sidebar-icon">
                                    <AttributesSvg id="EditRawData-attributes-button" style="height: 38px; width: 34px"  class="raw-data-icon" v-if="navigationTab.tabName === 'Attributes'"/>    
                                    <DataSourceSvg id="EditRawData-dataSource-button" style="height: 30px; width: 36px" class="raw-data-icon" v-if="navigationTab.tabName === 'DataSource'"/>
                                    <NetworksSvg  id="EditRawData-networks-button" style="height: 34px; width: 34px" class="raw-data-icon" v-if="navigationTab.tabName === 'Networks'"/>                            
                                </v-list-tile-icon>
                            </v-list-tile-action>
                            <v-list-tile-title style="text-decoration: none">{{navigationTab.tabName}}</v-list-tile-title>                         
                        </v-list-tile>
                    </v-list-item>
                </v-list>
            </v-card>
            <v-flex xs12 class="ghd-content">
                <v-container fluid grid-list-xs style="padding-left:20px;padding-right:20px;">
                    <router-view></router-view>
                </v-container>
            </v-flex>
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
