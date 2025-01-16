import { ORGANIZATIONS } from '../data/endpoints';

console.log(ORGANIZATIONS);
export const getCODS = () => {
    return fetch(ORGANIZATIONS.CLIENTCODS + ORGANIZATIONS.GET_CODS);
};

export const getCodByName = (name) => {
    return fetch(
        ORGANIZATIONS.CLIENTCODS + ORGANIZATIONS.GET_COD_BY_NAME + `/${name}`
    );
};
export const getCodByTag = (id) => {
    return fetch(
        ORGANIZATIONS.CLIENTCODS + ORGANIZATIONS.GET_COD_BY_TAG + `/${id}`
    );
};
