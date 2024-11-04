import { useState } from 'react';
import Form from '../../components/UI/Form/Form';
import { createAssignment } from '../../services/assignments.service';

export const AssignmentCreatepage = () => {
    const [data, setData] = useState({
        name: '',
        typeOfRequest: '',
        targetFieldName: '',
        oid: '',
    });
    const handleSubmit = () => {
        createAssignment(data)
            .then((res) => res.json())
            .then((data) => console.log(data));
    };
    const handleChange = (e, type) => {
        setData((prev) => ({
            ...prev,
            [type]: e.target.value,
        }));
    };
    return (
        <>
            <Form
                types={[
                    {
                        name: 'Name',
                        input: 'input',
                        onChangeName: 'name',
                    },
                    {
                        name: 'Type of request',
                        input: 'input',
                        onChangeName: 'typeOfRequest',
                    },
                    {
                        name: 'Target field name',
                        input: 'input',
                        onChangeName: 'targetFieldName',
                    },
                    {
                        name: 'OID',
                        input: 'input',
                        onChangeName: 'oid',
                    },
                ]}
                onChange={handleChange}
                onSubmit={handleSubmit}
            />
        </>
    );
};
