import { Link } from 'react-router-dom';
import classes from './Router.module.css';
import { ROUTES } from '../../data/routes';
export default function Router({ data }) {
    const titlePort = (port) => {
        const {
            interfaceName,
            interfaceNumber,
            interfaceSpeed,
            interfaceStatus,
            interfaceType,
        } = port;
        const mainPortInfo = {
            interfaceName,
            interfaceNumber,
            interfaceSpeed,
            interfaceStatus,
            interfaceType,
        };
        let res = '';
        for (let prop in mainPortInfo) {
            res += `${prop} - ${mainPortInfo[prop]}\n`;
        }

        return res;
    };
    return (
        <Link
            to={`${
                data.id ? ROUTES.GATEWAYS + `/${data.id}` : location.pathname
            }`}
            className={classes.link}
            state={data}
        >
            <div className={classes.wrapper}>
                <div className={classes.title}>{data.host}</div>
                <div className={classes.routerFlex}>
                    <div className={classes.router}>
                        <div className={classes.state}>
                            <div className={classes.off}></div>
                            <div className={classes.on}></div>
                        </div>
                        <div className={classes.ports}>
                            {data.portsOfNetworkDevice.map((port) => {
                                return (
                                    <div
                                        className={classes.port}
                                        key={port.id}
                                        title={titlePort(port)}
                                    ></div>
                                );
                            })}
                        </div>
                    </div>
                </div>
            </div>
        </Link>
    );
}
