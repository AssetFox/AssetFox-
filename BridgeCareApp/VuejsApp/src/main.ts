//import '@babel/polyfill';
import '@fortawesome/fontawesome-free/css/all.css';
import Vue, { createApp, defineComponent, watch, reactive } from 'vue';
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
import { ap } from 'ramda';
import vuetify from '@/plugins/vuetify';

const app = createApp(App);

app.use(router);
app.use(store);
app.use(vuetify);

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

app.use(VueSanitize, defaultOptions);

app.config.globalProperties.productionTip = false;
//app.use(VuejsDialog);
//app.config.globalProperties.$config = config;
app.mount('#app');
// fetch(import.meta.env.BASE_URL + 'config.json')
//     .then(response => response.json())
//     .then(config => {
      
//     });
