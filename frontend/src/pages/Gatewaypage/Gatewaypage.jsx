import { useParams } from 'react-router-dom';
import Router from '../../components/Router/Router';
import Wrapper from '../../components/Wrapper/Wrapper';
import classes from './Gatewaypage.module.css';
import { Port } from '../../components/Port/Port';
import Item from './components/Item/Item';
import { useEffect, useState } from 'react';
import Loader from '../../components/Loader/Loader';
import { getByIdGateway } from '../../services/gateways.service';
import Error from '../../components/Error/Error';

export default function Gatewaypage() {
    const [isError, setIsError] = useState(false);
    const [data, setData] = useState(null);
    const params = useParams();
    console.log(data);

    useEffect(() => {
        getByIdGateway(params.id)
            .then((res) => {
                if (res.status !== 200) throw 'Error';
                return res.json();
            })
            .then((data) => {
                setData(data);
                console.log(data);
            })
            .catch(() => {
                setIsError(true);
            });
    }, [params.id]);
    return !data ? (
        isError ? (
            <Error />
        ) : (
            <Loader />
        )
    ) : (
        <div>
            <div className={classes.routerWrapper}>
                <Router data={data} />
            </div>
            <Wrapper>
                <div className={classes.routerInfoWrapper}>
                    <div className={classes.routerInfoTitle}>General info:</div>
                    <div className={classes.routerInfoDescr}>
                        <Item
                            title={'General information'}
                            descr={data.generalInformation}
                        />
                        <Item title={'Host'} descr={data.host} />
                        <Item
                            title={'Network device name'}
                            descr={data.networkDeviceName}
                        />
                        <Item
                            title={'Type of network device'}
                            descr={data.typeOfNetworkDevice}
                        />
                    </div>
                </div>
            </Wrapper>
            <Wrapper>
                <div className={classes.ports}>
                    {data.portsOfNetworkDevice.map((port) => (
                        <Port port={port} key={port.id} />
                    ))}
                </div>
            </Wrapper>
        </div>
    );
}
