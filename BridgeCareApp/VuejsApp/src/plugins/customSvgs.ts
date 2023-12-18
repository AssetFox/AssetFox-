import { h } from "vue";
import type { IconSet, IconProps } from "vuetify";
import GhdSearchSvg from '@/shared/icons/GhdSearchSvg.vue';
import GhdDownSvg from '@/shared/icons/GhdDownSvg.vue';
import GhdTableSortSvg from '@/shared/icons/GhdTableSortSvg.vue';
import GhdTableSortAscSvg from '@/shared/icons/GhdTableSortAscSvg.vue'
import GhdTableSortDescSvg from '@/shared/icons/GhdTableSortDescSvg.vue'

const customSvgNameToComponent: any = {
    GhdDownSvg,
    GhdSearchSvg,
    GhdTableSortSvg,
    GhdTableSortAscSvg,
    GhdTableSortDescSvg
};

const customSVGs: IconSet = {
  component: (props: IconProps) => h(customSvgNameToComponent[props.icon]),
};

export { customSVGs /* aliases */ };