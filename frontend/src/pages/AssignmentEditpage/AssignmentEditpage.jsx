import { useLocation } from 'react-router-dom';
import Form from '../../components/UI/Form/Form';
import { useState } from 'react';
import { updateAssignment } from '../../services/assignments.service';

export const AssignmentEditpage = () => {
    const location = useLocation();
    const { name, typeOfRequest, OID, targetFieldName, id } = location.state;
    const [data, setData] = useState({
        id,
        name: name,
        typeOfRequest: typeOfRequest,
        targetFieldName: targetFieldName,
        oid: OID,
    });
    const handleSubmit = () => {
        updateAssignment(data)
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
        <div>
            <Form
                types={[
                    {
                        name: 'Name',
                        input: 'input',
                        defaultValue: name,
                        onChangeName: 'name',
                    },
                    {
                        name: 'Type of request',
                        input: 'input',
                        defaultValue: typeOfRequest,
                        onChangeName: 'typeOfRequest',
                    },
                    {
                        name: 'Target field name',
                        input: 'input',
                        defaultValue: targetFieldName,
                        onChangeName: 'targetFieldName',
                    },
                    {
                        name: 'OID',
                        input: 'input',
                        defaultValue: OID,
                        onChangeName: 'oid',
                    },
                ]}
                onChange={handleChange}
                onSubmit={handleSubmit}
            />
        </div>
    );
};
