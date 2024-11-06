const defaultState = {
    value: '',
    oldValue: '',
    isSearch: false,
};

export const GATEWAY_SEARCH_ADDRESS_ACTIONS = {
    CHANGE_VALUE: 'CHANGE_VALUE',
    RESET_VALUE: 'RESET_VALUE',
    SET_SEARCH: 'SET_SEARCH',
    RESET_SEARCH: 'RESET_SEARCH',
    TRANSFER_VALUE_TO_OLD_VALUE: 'TRANSFER_VALUE_TO_OLD_VALUE',
};

export const gatewaySearchAddressReducer = (state = defaultState, action) => {
    switch (action.type) {
        case GATEWAY_SEARCH_ADDRESS_ACTIONS.CHANGE_VALUE:
            return { ...state, value: action?.payload ?? '' };
        case GATEWAY_SEARCH_ADDRESS_ACTIONS.RESET_VALUE:
            return { ...state, value: '', oldValue: '' };
        case GATEWAY_SEARCH_ADDRESS_ACTIONS.SET_SEARCH:
            return { ...state, isSearch: true };
        case GATEWAY_SEARCH_ADDRESS_ACTIONS.RESET_SEARCH:
            return { ...state, isSearch: false };
        case GATEWAY_SEARCH_ADDRESS_ACTIONS.TRANSFER_VALUE_TO_OLD_VALUE:
            return { ...state, oldValue: state.value };
        default:
            return state;
    }
};
