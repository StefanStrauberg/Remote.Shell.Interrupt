import { combineReducers, createStore } from 'redux';
import { gatewaySearchAddressReducer } from './gatewaySearchAddressReducer';

const combineReducer = combineReducers({
    gatewaySearchAddressReducer,
});

export const store = createStore(combineReducer);
