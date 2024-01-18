//import '@babel/polyfill';
import '@fortawesome/fontawesome-free/css/all.css';
import { library } from '@fortawesome/fontawesome-svg-core';
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import { faSignOutAlt, faSignInAlt } from '@fortawesome/free-solid-svg-icons';
import Vue, { createApp, h, defineComponent, watch, reactive } from 'vue';
import vuetify from '@/plugins/vuetify';
import 'vuetify/dist/vuetify.min.css';
import 'bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import App from './App.vue';
import router from './router';
import { useRoute, useRouter } from 'vue-router';
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
import { ap } from 'ramda';
import { IconProps, IconSet, createVuetify } from 'vuetify';
import { fa } from 'vuetify/iconsets/fa'
import PrimeVue from 'primevue/config';
import Dialog from 'primevue/dialog';
import Button from 'primevue/button';
import DataTable from 'primevue/datatable';
import ConfirmationService from 'primevue/confirmationservice';
import ConfirmDialog from 'primevue/dialog';
import "primevue/resources/themes/saga-blue/theme.css"; 
import "primevue/resources/themes/bootstrap4-light-blue/theme.css"
import 'primevue/resources/primevue.min.css'
import 'primeicons/primeicons.css';
import "primeflex/primeflex.css";
const app = createApp(App);


app.use(router);
app.use(store);
app.use(vuetify);
app.use(PrimeVue);
//app.use(VueWorker);
app.use(KendoChartInstaller);

//app.use(VueCurrencyInput);
//authenticationModule.state.securityType = config.securityType as string;

app.use(connectionHub);

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

// Font awesome (free) library
library.add(faSignInAlt);
library.add(faSignOutAlt);

app.use(VueSanitize, defaultOptions);
app.use(ConfirmationService);
app.component("Dialog",Dialog)
   .component("Button", Button)
   .component("DataTable", DataTable)
   .component("ConfirmDialog", ConfirmDialog)
   .component('font-awesome-icon', FontAwesomeIcon);

app.config.globalProperties.productionTip = false;
//app.use(VuejsDialog);
//app.config.globalProperties.$config = config;
app.mount('#app');
// fetch(import.meta.env.BASE_URL + 'config.json')
//     .then(response => response.json())
//     .then(config => {
      
//     });
