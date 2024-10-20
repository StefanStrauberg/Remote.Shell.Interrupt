import { useRef, useState } from 'react';
import Wrapper from '../Wrapper/Wrapper';
import classes from './Port.module.css';
import Button from '../UI/Button/Button';

export const Port = ({ port }) => {
    const [textToggleLans, setTextToggleLans] = useState('show');
    const lansRef = useRef(null);
    const handleToggleLans = () => {
        lansRef.current.classList.toggle(classes.none);
        if (textToggleLans === 'hide') {
            setTextToggleLans('show');
        } else {
            setTextToggleLans('hide');
        }
    };

    return (
        <Wrapper>
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
                    </div>{' '}
                    {port.vlaNs.length ? (
                        <Button
                            onClick={handleToggleLans}
                            classNames={classes.lansBtn}
                        >
                            {textToggleLans}
                        </Button>
                    ) : null}
                    <div ref={lansRef} className={classes.none}>
                        {port.vlaNs.map((lan) => (
                            <div key={lan.vlanName}>
                                <div className={classes.lan}>
                                    <div>vlan tag: ${lan.vlanTag}</div>
                                    <div>vlan name: ${lan.vlanName}</div>
                                </div>
                            </div>
                        ))}
                    </div>
                </div>
            </div>
        </Wrapper>
    );
};
