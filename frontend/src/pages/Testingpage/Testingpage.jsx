import { useState } from 'react';
import Form from '../../components/UI/Form/Form';
import Wrapper from '../../components/Wrapper/Wrapper';
import Text from '../../components/Text/Text';
import classes from './Testingpage.module.css';
import { getTesting, walkTesting } from '../../services/testing.service';
export default function Testingpage() {
    const [info, setInfo] = useState({});
    const [data, setData] = useState();
    function onChangeHandler(e, reqName) {
        setInfo((prev) => ({ ...prev, [reqName]: e.target.value }));
        console.log(info);
    }
    function infoWithoutType() {
        const tempInfo = { ...info };
        delete tempInfo?.type;
        return tempInfo;
    }
    function onSubmitHandler() {
        console.log(infoWithoutType());
        if (info.type === 'Walk' || info.type == '') {
            walkTesting(infoWithoutType())
                .then((res) => res.json())
                .then((data) => setData(data))
                .catch((err) => setData(err));
        } else {
            getTesting(infoWithoutType())
                .then((res) => res.json())
                .then((data) => setData(data))
                .catch((err) => setData(err));
        }
    }

    return (
        <div>
            <Form
                placeholder={'Enter info'}
                types={[
                    { name: 'Host', input: 'input', requestName: 'Host' },
                    {
                        name: 'Community',
                        input: 'input',
                        requestName: 'Community',
                    },
                    { name: 'OID', input: 'input', requestName: 'OID' },
                    {
                        name: 'Type of request',
                        input: 'select',
                        options: [
                            { option: 'Walk', value: 'Walk' },
                            { option: 'Get', value: 'Get' },
                        ],
                        requestName: 'type',
                    },
                ]}
                onChange={onChangeHandler}
                submitText={'Get info'}
                onSubmit={onSubmitHandler}
            />
            <Wrapper>
                <h1 className={classes.title}>
                    <Text>Received information:</Text>
                </h1>
                <Text>{JSON.stringify(data)}</Text>
            </Wrapper>
        </div>
    );
}
