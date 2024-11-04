import { useEffect, useState } from 'react';
import classes from './Gatewayspage.module.css';
import { getByIpGateway, getGateways } from '../../services/gateways.service';
import Loader from '../../components/Loader/Loader';
import { useSelector } from 'react-redux';
import Error from '../../components/Error/Error';
import Router from '../../components/Router/Router';
import Gateway from '../../components/Gateway/Gateway';

export default function Gatewayspage() {
    const [gateways, setGateways] = useState([]);
    const [loading, setIsLoading] = useState(false);
    const [isError, setIsError] = useState(false);
    const gatewaySearchAddress = useSelector(
        (state) => state.gatewaySearchAddressReducer
    );
    useEffect(() => {
        setIsLoading(true);
        setIsError(false);
        if (gatewaySearchAddress.value === '') {
            getGateways()
                .then((res) => {
                    if (res.status !== 200) {
                        setIsError(true);
                    }
                    return res.json();
                })
                .then((data) => {
                    setGateways(data);
                })
                .catch((err) => {
                    console.log(err);
                    setIsError(true);
                })
                .finally(() => setIsLoading(false));
        } else {
            getByIpGateway(gatewaySearchAddress.value)
                .then((res) => {
                    if (res.status !== 200) {
                        setIsError(true);
                    }
                    return res.json();
                })
                .then((data) => {
                    setGateways(data);
                })
                .catch((err) => {
                    console.log(err);
                    setIsError(true);
                })
                .finally(() => setIsLoading(false));
        }
    }, [gatewaySearchAddress]);
    if (isError) return <Error />;
    return (
        <>
            <section className={classes.gatewaysInfo}>
                {loading ? (
                    <Loader />
                ) : gatewaySearchAddress.value === '' ? (
                    <div className={classes.grid}>
                        {gateways.length
                            ? gateways.map((gateway) => (
                                  <Router key={gateway.id} data={gateway} />
                              ))
                            : null}
                    </div>
                ) : (
                    <div className={classes.wrapper}>
                        {gateways.map((gateway) => (
                            <Gateway data={gateway} key={gateway.id} />
                        ))}
                    </div>
                )}
            </section>
        </>
    );
}
