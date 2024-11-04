import Router from '../../components/Router/Router';
import Wrapper from '../../components/Wrapper/Wrapper';
import { Port } from '../../components/Port/Port';
import classes from './Gateway.module.css';
import Item from '../../pages/Gatewaypage/components/Item/Item';

export default function Gateway({ data }) {
    return (
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
