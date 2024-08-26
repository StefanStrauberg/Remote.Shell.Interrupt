import { Link } from 'react-router-dom';
import classes from './Router.module.css';
export default function Router() {
    return (
        <Link to={'/gateways/2'}>
            <div className={classes.wrapper}>
                <div className={classes.title}>
                    Cisco CE6863E-48S6CQ - 192.168.1.1
                </div>
                <div className={classes.routerFlex}>
                    <div className={classes.router}>
                        <div className={classes.state}>
                            <div className={classes.off}></div>
                            <div className={classes.on}></div>
                        </div>
                        <div className={classes.ports}>
                            <div className={classes.portWrapper}>
                                <div
                                    className={`${classes.port} ${classes.activePort}`}
                                ></div>
                                <div className={classes.port}></div>
                            </div>
                            <div className={classes.portWrapper}>
                                <div
                                    className={`${classes.port} ${classes.activePort}`}
                                ></div>
                                <div className={classes.port}></div>
                            </div>
                            <div className={classes.portWrapper}>
                                <div className={classes.port}></div>
                                <div className={classes.port}></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </Link>
    );
}
