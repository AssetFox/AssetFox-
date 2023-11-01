export const getUrl = (url: string) => {
    return new URL("../../"+url, import.meta.url).href
};