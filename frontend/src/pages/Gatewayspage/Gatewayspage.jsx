import { useEffect, useState } from 'react';
import Router from '../../components/Router/Router';
import classes from './Gatewayspage.module.css';
import { getGateways } from '../../services/gateways.service';
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
                {loading ? (
                    <div className={classes.loading}>Loading...</div>
                ) : null}
                {gateways.map((gateway) => (
                    <Router key={gateway.id} data={gateway} />
                ))}
            </section>
        </>
    );
}
