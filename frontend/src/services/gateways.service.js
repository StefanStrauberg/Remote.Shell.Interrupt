import { GATEWAYS } from '../data/endpoints';

export function getGateways() {
    return fetch(GATEWAYS.GATEWAYS + GATEWAYS.GET_ALL_GATEWAYS);
}

export function createGateway(data) {
    return fetch(GATEWAYS.GATEWAYS + GATEWAYS.CREATE_GATEWAY, {
        method: 'POST',
        body: JSON.stringify(data),
    });
}

export function getByIdGateway(id) {
    return fetch(GATEWAYS.GATEWAYS + GATEWAYS.GET_BY_ID_GATEWAY + `/${id}`);
}

export function deleteAssignment(id) {
    return fetch(GATEWAYS.GATEWAYS + GATEWAYS.DELETE_GATEWAY + `/${id}`, {
        method: 'DELETE',
    });
}

export function getByIpGateway(address) {
    return fetch(
        GATEWAYS.GATEWAYS + GATEWAYS.GET_BY_IP_GATEWAY + `/${address}`
    );
}
export const getByTagGateway = (tag) => {
    return fetch(GATEWAYS.GATEWAYS + GATEWAYS.GET_BY_TAG + `/${tag}`);
};
