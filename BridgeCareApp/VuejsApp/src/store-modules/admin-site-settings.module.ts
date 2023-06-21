import {AxiosResponse} from 'axios';
import AdminSiteSettingsService from '@/services/admin-site-settings.service';
import {hasValue} from '@/shared/utils/has-value-util';

const state = {
    agencyLogo: '',
    productLogo: '',
    implementationName: '',
    isSuccessfulImport: false as boolean,
};

const mutations = {
    agencyLogoMutator(state: any, agencyLogo: string) {
        state.agencyLogo = agencyLogo;
    },
    implementationNameMutator(state: any, implementationName: String) {
        state.implementationName = implementationName;
    },
    productLogoMutator(state: any, productLogo: string) {
        state.productLogo = productLogo;
    },
    isSuccessfulImportMutator(state: any, isSuccessful: boolean) {
        state.isSuccessfulImport = isSuccessful;
    }
};

const actions = {
    async getAgencyLogo({commit}: any) {
        await AdminSiteSettingsService.getAgencyLogo()
        .then((response: AxiosResponse<string>) => {
            if (hasValue(response, 'data')) {
                commit('agencyLogoMutator', response.data);
            }
        });
    },
    async getImplementationName({commit}: any) {
        await AdminSiteSettingsService.getImplementationName()
        .then((response: AxiosResponse<string>) => {
            if (hasValue(response, 'data')) {
                commit('implementationNameMutator', response.data);
            }
        });
    },
    async getProductLogo({commit}: any) {
        await AdminSiteSettingsService.getProductLogo()
        .then((response: AxiosResponse<string>) => {
            if (hasValue(response, 'data')) {
                commit('productLogoMutator', response.data);
            }
        });
    },
    async importAgencyLogo({commit, dispatch}: any, payload: File) {
        await AdminSiteSettingsService.importAgencyLogo(payload)
        .then(async (response: AxiosResponse) => {
            if (response.status >= 200 && response.status < 300) {
                const base64 = await convertFileToBase64(payload);
                commit('agencyLogoMutator', base64);
                commit('isSuccessfulImportMutator', true);
                dispatch('addSuccessNotification',{
                    message: 'Agency logo imported'
                });
            }
        });
    },
    async importImplementationName({commit, dispatch}: any, payload: any) {
        await AdminSiteSettingsService.importImplementationName(payload)
        .then((response: AxiosResponse) => {
            if (response.status >= 200 && response.status < 300) {
                const payloadString = payload.toString();
    
                  commit('implementationNameMutator', payloadString);
                  commit('isSuccessfulImportMutator', true);
                dispatch('addSuccessNotification', {
                  message: 'Implementation Name imported'
                });
              }
            });
        },
    async importProductLogo({commit, dispatch}: any, payload: File) {
        await AdminSiteSettingsService.importProductLogo(payload)
        .then(async (response: AxiosResponse) => {
            if (response.status >= 200 && response.status < 300) {
                const base64 = await convertFileToBase64(payload);
                commit('productLogoMutator', base64);
                commit('isSuccessfulImportMutator', true);
                dispatch('addSuccessNotification',{
                    message: 'Product logo imported'
                });
            }
        });
    },
    async convertAndStoreAgencyLogo({commit}: any, agencyLogo: File) {
        const base64 = await convertFileToBase64(agencyLogo);
        commit('agencyLogoMutator', base64);
    },
    async convertAndStoreProductLogo({commit}: any, productLogo: File) {
        const base64 = await convertFileToBase64(productLogo);
        commit('productLogoMutator', base64);
    },
    
};

function convertFileToBase64(file: File): Promise<string> {
    return new Promise<string>((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = () => {
        const image = new Image();
        image.onload = () => {
          const targetHeight = 50;
          const ratio = image.width / image.height;
          const targetWidth = Math.round(targetHeight * ratio);
  
          const canvas = document.createElement('canvas');
          canvas.width = targetWidth;
          canvas.height = targetHeight;
  
          const context = canvas.getContext('2d');
          context?.drawImage(image, 0, 0, targetWidth, targetHeight);
  
          const resizedBase64 = canvas.toDataURL(file.type);
          resolve(resizedBase64);
        };
        image.onerror = error => reject(error);
        image.src = reader.result as string;
      };
      reader.onerror = error => reject(error);
      reader.readAsDataURL(file);
    });
  }
  
const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
