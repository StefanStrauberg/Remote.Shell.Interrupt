import classes from './Assignmentspage.module.css';
import { getAssignments } from '../../services/assignments.service';
import { useEffect, useState } from 'react';
import { AssignmentItem } from '../../components/AssignmentItem/AssignmentItem';
export default function Assigmentspage() {
    const [assignments, setAssignments] = useState([]);
    useEffect(() => {
        getAssignments()
            .then((res) => res.json())
            .then((data) => {
                if (!data?.Status) {
                    setAssignments(data);
                }
            });
    }, []);
    return (
        <div className={classes.wrapper}>
            {assignments?.map((assignment) => (
                <AssignmentItem
                    key={assignment.id}
                    id={assignment.id}
                    name={assignment.name}
                    typeOfRequest={assignment.typeOfRequest}
                    targetFieldName={assignment.targetFieldName}
                    OID={assignment?.oid}
                />
            ))}
        </div>
    );
}
