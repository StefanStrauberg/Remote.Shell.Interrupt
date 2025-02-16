import React, { useEffect, useState } from 'react';
import classes from './Gatewayspage.module.css';
import {
    getByIpGateway,
    getByTagGateway,
    getGateways,
} from '../../services/gateways.service';
import Loader from '../../components/Loader/Loader';
import { useDispatch, useSelector } from 'react-redux';
import Error from '../../components/Error/Error';
import Router from '../../components/Router/Router';
import Gateway from '../../components/Gateway/Gateway';
import { GATEWAY_SEARCH_ADDRESS_ACTIONS } from '../../store/gatewaySearchAddressReducer';
import Input from '../../components/UI/Input/Input';
import Button from '../../components/UI/Button/Button';
import Wrapper from '../../components/Wrapper/Wrapper';
import Item from '../Gatewaypage/components/Item/Item';
import Cod from '../../components/Cod';

export default function Gatewayspage() {
    const [gateways, setGateways] = useState([]);
    const [loading, setIsLoading] = useState(false);
    const [isError, setIsError] = useState(false);
    const [isClickedSearchTag, setIsClickedSearchTag] = useState(false);
    const [isLastSearchTag, setIsLastSearchTag] = useState(false);
    const [oldResultTag, setOldResultTag] = useState('');
    const [tag, setTag] = useState('');
    const [cods, setCods] = useState();
    const dispatch = useDispatch();
    const gatewaySearchAddress = useSelector(
        (state) => state.gatewaySearchAddressReducer
    );
    const fetchByTag = () => {
        setIsLoading(true);
        getByTagGateway(tag)
            .then((res) => res.json())
            .then((data) => {
                if (!data?.Status) {
                    console.log(data);
                    setGateways(data.networkDevices);
                    setCods(data.clientCODs);
                    setIsError(false);
                } else {
                    setIsError(true);
                }
            })
            .catch(() => setIsError(true))
            .finally(() => setIsLoading(false));
    };
    useEffect(() => {
        if (!gatewaySearchAddress.isSearch) return;
        setIsLoading(true);
        setIsError(false);
        setCods();
        if (gatewaySearchAddress.value === '') {
            getGateways()
                .then((res) => res.json())
                .then((data) => {
                    if (!data?.Status) {
                        setGateways(data.networkDevices);
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
                        console.log(data?.networkDevices);
                        setGateways(data?.networkDevices);
                        setCods(data?.clientCODs);
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
        setIsClickedSearchTag(false);
        setIsLastSearchTag(false);
        setOldResultTag(tag);
    }, [gatewaySearchAddress]);
    useEffect(() => {
        if (isClickedSearchTag) {
            if (tag) {
                fetchByTag();
            } else {
                setCods();
                setIsLoading(true);
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
            }
            setIsLastSearchTag(true);
            setIsClickedSearchTag(false);
            setOldResultTag(tag);
        }
    }, [isClickedSearchTag]);
    console.log(isError);
    return (
        <>
            <div className="form my">
                <Input
                    placeholder={'Поиск по vlanTag:'}
                    onChange={(e) => setTag(e.target.value)}
                />
                <Button
                    onClick={() => {
                        setIsClickedSearchTag(true);
                    }}
                >
                    Search
                </Button>
            </div>
            {isError ? <Error /> : null}
            {loading ? <Loader /> : null}
            {!loading && !isError && gateways ? (
                <section className={classes.gatewaysInfo}>
                    {(gatewaySearchAddress.oldValue === '' &&
                        !isLastSearchTag) ||
                    (isLastSearchTag && oldResultTag == '') ? (
                        <div className={classes.grid}>
                            {gateways?.length
                                ? gateways?.map((gateway) => (
                                      <React.Fragment key={gateway.id}>
                                          <Router data={gateway} />
                                      </React.Fragment>
                                  ))
                                : null}
                        </div>
                    ) : (
                        <div className={classes.wrapper}>
                            <div>
                                {cods?.map((cod, index) => (
                                    <Cod cod={cod} key={index} />
                                ))}
                            </div>
                            {gateways?.map((gateway) => (
                                <Gateway data={gateway} key={gateway.id} />
                            ))}
                        </div>
                    )}
                </section>
            ) : null}
        </>
    );
}
