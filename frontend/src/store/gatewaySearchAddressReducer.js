const defaultState = {
    value: '',
};

export const GATEWAY_SEARCH_ADDRESS_ACTIONS = {
    CHANGE_VALUE: 'CHANGE_VALUE',
    RESET_VALUE: 'RESET_VALUE',
};

export const gatewaySearchAddressReducer = (state = defaultState, action) => {
    if (action.type === GATEWAY_SEARCH_ADDRESS_ACTIONS.CHANGE_VALUE) {
        return { ...state, value: action?.payload ?? '' };
    }
    if (action.type === GATEWAY_SEARCH_ADDRESS_ACTIONS.RESET_VALUE) {
        return { ...state, value: '' };
    }
    return state;
};
