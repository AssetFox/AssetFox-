// import Vue from 'vue';
// import Vuetify from 'vuetify/lib';
// import 'vuetify/src/stylus/app.styl';
// This is from forums, can be removed if not of use: // import '@mdi/font/css/materialdesignicons.css'
// This is from forums, can be removed if not of use: // import * as styles from 'vuetify/styles'
import { IconProps, IconSet, createVuetify } from 'vuetify';
import { fa } from 'vuetify/iconsets/fa'
import * as components from "vuetify/components";
import * as directives from "vuetify/directives";
import { h } from 'vue';
import GhdSearchSvg from '@/shared/icons/GhdSearchSvg.vue';
import GhdDownSvg from '@/shared/icons/GhdDownSvg.vue';
import GhdTableSortSvg from '@/shared/icons/GhdTableSortSvg.vue';

const ghdSearchIconSet: IconSet = {
    component: (props: IconProps) => {
      return h(GhdSearchSvg, {
        name: 'ghd-search'
      });
    }
  }
  
const ghdDownIconSet: IconSet = {
    component: (props: IconProps) => {
      return h(GhdDownSvg, {
        name: 'ghd-down' 
      });
    }
  }
  
const ghdTableSortIconSet: IconSet = {
    component: (props: IconProps) => {
        return h(GhdTableSortSvg, {
            name: 'ghd-table-sort'
      });
    }
  }

const vuetify = createVuetify({ 
    icons: { 
      defaultSet: 'fa', 
      sets: { fa, ghdSearchIconSet, ghdTableSortIconSet, ghdDownIconSet },  
    },
    components,
    directives,
    });

export default vuetify;

// Commented as createVuetify to be used for vue3
// Vue.use(Vuetify, {
//     theme: {
//         primary: '#002E6C',
//         secondary: '#424242',
//         accent: '#82B1FF',
//         error: '#FF5252',
//         info: '#2196F3',
//         success: '#4CAF50',
//         warning: '#FFC107'
//     },
//     customProperties: true,
//     iconfont: 'md',
// });
