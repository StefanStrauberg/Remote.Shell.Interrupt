import { useLocation } from 'react-router-dom';
import Router from '../../components/Router/Router';
import Wrapper from '../../components/Wrapper/Wrapper';
import classes from './Gatewaypage.module.css';

export default function Gatewaypage() {
    const location = useLocation();
    const data = location.state;
    console.log(data);
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
                <Wrapper key={port.id}>
                    <div className={classes.routerInfoWrapper}>
                        <div className={classes.routerInfoTitle}>
                            {port.interfaceName}
                        </div>
                        <div className={classes.routerInfoDescr}>
                            <div className={classes.routerInfoText}>
                                Interface number: {port.interfaceNumber}
                            </div>
                            <div className={classes.routerInfoText}>
                                Interface speed: {port.interfaceSpeed}
                            </div>
                            <div className={classes.routerInfoText}>
                                Interface status: {port.interfaceStatus}
                            </div>
                            <div className={classes.routerInfoText}>
                                Interface type: {port.interfaceType}
                            </div>
                            {port.vlaNs.map((lan) => (
                                <div key={lan.vlanName} className={classes.lan}>
                                    <div>vlan tag: ${lan.vlanTag}</div>
                                    <div>vlan name: ${lan.vlanName}</div>
                                </div>
                            ))}
                            <div></div>
                        </div>
                    </div>
                </Wrapper>
            ))}
        </>
    );
}
