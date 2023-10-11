//import '@babel/polyfill';
import '@fortawesome/fontawesome-free/css/all.css';
import Vue, { h, createApp, defineComponent, watch, reactive } from 'vue';
import 'vuetify/dist/vuetify.min.css';
import 'bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import App from './App.vue';
import router from './router';
import store from './store/root-store';
import './assets/css/main.css';
import VueScreen from 'vue-screen';
// @ts-ignore
import VueWorker from 'vue-worker';
import '@progress/kendo-ui';
import '@progress/kendo-theme-default/dist/all.css';
import { KendoChartInstaller } from '@progress/kendo-charts-vue-wrapper';
import VueCurrencyInput from 'vue-currency-input';
import connectionHub from './connectionHub';
// @ts-ignore
import VueSanitize from 'vue-3-sanitize';
// @ts-ignore
import VuejsDialog from 'vuejs-dialog';
// @ts-ignore
import 'vuejs-dialog/dist/vuejs-dialog.min.css';
import GhdSearchSvg from '@/shared/icons/GhdSearchSvg.vue';
import GhdDownSvg from '@/shared/icons/GhdDownSvg.vue';
import GhdTableSortSvg from '@/shared/icons/GhdTableSortSvg.vue';
import authenticationModule from './store-modules/authentication.module';
import { ap } from 'ramda';
import { IconProps, IconSet, createVuetify } from 'vuetify';
import { fa } from 'vuetify/iconsets/fa'
import PrimeVue from 'primevue/config';
import Dialog from 'primevue/dialog';
import Button from 'primevue/button';
import ConfirmationService from 'primevue/confirmationservice';
import "primevue/resources/themes/saga-blue/theme.css"; 
import 'primevue/resources/primevue.min.css'
import 'primeicons/primeicons.css';

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
const app = createApp(App);
const vuetify = createVuetify({ 
icons: { 
  defaultSet: 'fa', 
  sets: { fa, ghdSearchIconSet, ghdTableSortIconSet, ghdDownIconSet },
},
});
app.use(router);
app.use(store);
app.use(vuetify);
app.use(PrimeVue);
//app.use(VueWorker);
app.use(KendoChartInstaller);

//app.use(VueCurrencyInput);
//authenticationModule.state.securityType = config.securityType as string;
//app.use(connectionHub);

// app.use(VueScreen, {
//     sm: 576,
//     md: 768,
//     lg: 992,
//     xl: 1200,
//     xxl: 1400,
//     freeRealEstate: 1700,
//     breakpointsOrder: ['sm', 'md', 'lg', 'xl', 'xxl', 'freeRealEstate'],
// });

var defaultOptions = {
    allowedTags: VueSanitize.defaults.allowedTags.concat([
        'html',
        'head',
        'body',
        'link',
    ]),
    allowedAttributes: false,
};

app.use(VueSanitize, defaultOptions);
app.use(ConfirmationService);
app.component("Dialog",Dialog)
   .component("Button", Button);
app.config.globalProperties.productionTip = false;
//app.use(VuejsDialog);
//app.config.globalProperties.$config = config;
app.mount('#app');
// fetch(import.meta.env.BASE_URL + 'config.json')
//     .then(response => response.json())
//     .then(config => {
      
//     });
