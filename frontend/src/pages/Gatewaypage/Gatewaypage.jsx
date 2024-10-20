import { useLocation } from 'react-router-dom';
import Router from '../../components/Router/Router';
import Wrapper from '../../components/Wrapper/Wrapper';
import classes from './Gatewaypage.module.css';
import { Port } from '../../components/Port/Port';

export default function Gatewaypage() {
    const location = useLocation();
    const data = location.state;

    return (
        <>
            <div className={classes.routerWrapper}>
                <Router data={data} />
            </div>
            <Wrapper>
                <div className={classes.routerInfoWrapper}>
                    <div className={classes.routerInfoTitle}>General info:</div>
                    <div className={classes.routerInfoDescr}>
                        <div className={classes.routerInfoText}>
                            <span className={classes.bold}>
                                General information:
                            </span>
                            {' ' + data.generalInformation}
                        </div>
                        <div className={classes.routerInfoText}>
                            <span className={classes.bold}>Host:</span>{' '}
                            {data.host}
                        </div>
                        <div className={classes.routerInfoText}>
                            <span className={classes.bold}>
                                Network device name:
                            </span>
                            {' ' + data.networkDeviceName}
                        </div>
                    </div>
                </div>
            </Wrapper>
            {data.portsOfNetworkDevice.map((port) => (
                <Port port={port} key={port.id} />
            ))}
        </>
    );
}
