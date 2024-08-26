import Router from '../../components/Router/Router';
import Wrapper from '../../components/Wrapper/Wrapper';
import classes from './Gatewaypage.module.css';

export default function Gatewaypage() {
    return (
        <>
            <div className={classes.routerWrapper}>
                <Router />
            </div>
            <Wrapper>
                <div className={classes.routerInfoWrapper}>
                    <div className={classes.routerInfoTitle}>Gateways</div>
                    <div className={classes.routerInfoDescr}>
                        <div className={classes.routerInfoText}>
                            lorem ipsum
                        </div>
                        <div className={classes.routerInfoText}>
                            lorem ipsum
                        </div>
                        <div className={classes.routerInfoText}>
                            lorem ipsum
                        </div>
                        <div className={classes.routerInfoText}>
                            lorem ipsum
                        </div>
                    </div>
                </div>
                <div className={classes.routerInfoWrapper}>
                    <div className={classes.routerInfoTitle}>Gateways</div>
                    <div className={classes.routerInfoDescr}>
                        <div className={classes.routerInfoText}>
                            lorem ipsum
                        </div>
                        <div className={classes.routerInfoText}>
                            lorem ipsum
                        </div>
                        <div className={classes.routerInfoText}>
                            lorem ipsum
                        </div>
                        <div className={classes.routerInfoText}>
                            lorem ipsum
                        </div>
                    </div>
                </div>
                <div className={classes.routerInfoWrapper}>
                    <div className={classes.routerInfoTitle}>Gateways</div>
                    <div className={classes.routerInfoDescr}>
                        <div className={classes.routerInfoText}>
                            lorem ipsum
                        </div>
                        <div className={classes.routerInfoText}>
                            lorem ipsum
                        </div>
                        <div className={classes.routerInfoText}>
                            lorem ipsum
                        </div>
                        <div className={classes.routerInfoText}>
                            lorem ipsum
                        </div>
                    </div>
                </div>
            </Wrapper>
        </>
    );
}
