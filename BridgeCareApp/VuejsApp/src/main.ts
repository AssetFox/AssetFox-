import '@babel/polyfill';
import '@fortawesome/fontawesome-free/css/all.css';
import Vue from 'vue';
import 'vuetify/dist/vuetify.min.css';
import 'bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import Vuetify from 'vuetify';
import App from './App.vue';
import router from './router';
import store from './store/root-store';
import './assets/css/main.css';
import 'izitoast/dist/css/iziToast.min.css';
import 'izitoast/dist/js/iziToast.min';
import VueScreen from 'vue-screen';
// @ts-ignore
import VueWorker from 'vue-worker';
import '@progress/kendo-ui';
import '@progress/kendo-theme-default/dist/all.css';
import { KendoChartInstaller } from '@progress/kendo-charts-vue-wrapper';
import VueCurrencyInput from 'vue-currency-input';
import connectionHub from './connectionHub';
// @ts-ignore
import VueSanitize from 'vue-sanitize';
// @ts-ignore
import VuejsDialog from 'vuejs-dialog';
// @ts-ignore
import VuejsDialogMixin from 'vuejs-dialog/dist/vuejs-dialog-mixin.min.js';
import 'vuejs-dialog/dist/vuejs-dialog.min.css';

Vue.use(Vuetify, {
    iconfont: 'fa',
});

Vue.use(VueWorker);

Vue.use(KendoChartInstaller);

Vue.use(VueCurrencyInput);
Vue.use(connectionHub);

Vue.use(VueScreen, {
    sm: 576,
    md: 768,
    lg: 992,
    xl: 1200,
    xxl: 1400,
    freeRealEstate: 1700,
    breakpointsOrder: ['sm', 'md', 'lg', 'xl', 'xxl', 'freeRealEstate'],
});

var defaultOptions = {
    allowedTags: VueSanitize.defaults.allowedTags.concat([
        'html',
        'head',
        'body',
        'link',
    ]),
    allowedAttributes: false,
};

Vue.use(VueSanitize, defaultOptions);

Vue.config.productionTip = false;

Vue.use(VuejsDialog);

fetch(process.env.BASE_URL + "config.json")
.then((response) => response.json())
.then((config) => {
    Vue.prototype.$config = config;
    new Vue({
        store,
        router,
        render: h => h(App),
    }).$mount('#app');
});
