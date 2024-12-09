import { useEffect, useState } from 'react';
import classes from './Gatewayspage.module.css';
import { getByIpGateway, getGateways } from '../../services/gateways.service';
import Loader from '../../components/Loader/Loader';
import { useDispatch, useSelector } from 'react-redux';
import Error from '../../components/Error/Error';
import Router from '../../components/Router/Router';
import Gateway from '../../components/Gateway/Gateway';
import { GATEWAY_SEARCH_ADDRESS_ACTIONS } from '../../store/gatewaySearchAddressReducer';

export default function Gatewayspage() {
    const [gateways, setGateways] = useState([]);
    const [loading, setIsLoading] = useState(false);
    const [isError, setIsError] = useState(false);
    const dispatch = useDispatch();
    const gatewaySearchAddress = useSelector(
        (state) => state.gatewaySearchAddressReducer
    );

    useEffect(() => {
        if (!gatewaySearchAddress.isSearch) return;
        setIsLoading(true);
        setIsError(false);
        if (gatewaySearchAddress.value === '') {
            getGateways()
                .then((res) => res.json())
                .then((data) => {
                    if (!data?.Status) {
                        setGateways(data);
                        setIsError(false);
                    } else {
                        setIsError(true);
                    }
                })
                .catch(() => {
                    setIsError(true);
                })
                .finally(() => setIsLoading(false));
        } else {
            getByIpGateway(gatewaySearchAddress.value)
                .then((res) => res.json())
                .then((data) => {
                    if (!data?.Status) {
                        setGateways(data);
                        setIsError(false);
                    } else {
                        setIsError(true);
                    }
                })
                .catch(() => {
                    setIsError(true);
                })
                .finally(() => setIsLoading(false));
        }
        dispatch({ type: GATEWAY_SEARCH_ADDRESS_ACTIONS.RESET_SEARCH });
        dispatch({
            type: GATEWAY_SEARCH_ADDRESS_ACTIONS.TRANSFER_VALUE_TO_OLD_VALUE,
        });
    }, [gatewaySearchAddress]);
    if (isError) return <Error />;
    if (loading) return <Loader />;

    return (
        <>
            <section className={classes.gatewaysInfo}>
                {gatewaySearchAddress.oldValue === '' ? (
                    <div className={classes.grid}>
                        {gateways.length
                            ? gateways?.map((gateway) => (
                                  <Router key={gateway.id} data={gateway} />
                              ))
                            : null}
                    </div>
                ) : (
                    <div className={classes.wrapper}>
                        {gateways?.map((gateway) => (
                            <Gateway data={gateway} key={gateway.id} />
                        ))}
                    </div>
                )}
            </section>
        </>
    );
}
