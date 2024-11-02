import { useEffect, useState } from 'react';
import Router from '../../components/Router/Router';
import classes from './Gatewayspage.module.css';
import { getGateways } from '../../services/gateways.service';
import Loader from '../../components/Loader/Loader';
export default function Gatewayspage() {
    const [gateways, setGateways] = useState([]);
    const [loading, setIsLoading] = useState(false);
    useEffect(() => {
        setIsLoading(true);
        getGateways()
            .then((res) => res.json())
            .then((data) => {
                setIsLoading(false);
                setGateways(data);
                console.log(data);
            })
            .catch((err) => {
                console.log(err);
                setIsLoading(false);
            });
    }, []);
    return (
        <>
            <section className={classes.gatewaysInfo}>
                {loading ? <Loader /> : null}
                <div className={classes.grid}>
                    {gateways.map((gateway) => (
                        <Router key={gateway.id} data={gateway} />
                    ))}
                </div>
            </section>
        </>
    );
}
