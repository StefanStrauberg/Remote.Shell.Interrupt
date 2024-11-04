import { useParams } from 'react-router-dom';
import { useEffect, useState } from 'react';
import Loader from '../../components/Loader/Loader';
import { getByIdGateway } from '../../services/gateways.service';
import Error from '../../components/Error/Error';
import Gateway from '../../components/Gateway/Gateway';

export default function Gatewaypage() {
    const [isError, setIsError] = useState(false);
    const [data, setData] = useState(null);
    const params = useParams();

    useEffect(() => {
        getByIdGateway(params.id)
            .then((res) => {
                if (res.status !== 200) throw 'Error';
                return res.json();
            })
            .then((data) => {
                setData(data);
            })
            .catch(() => {
                setIsError(true);
            });
    }, [params.id]);
    if (isError) return <Error />;
    if (!data) return <Loader />;
    if (data) return <Gateway data={data} />;
}
