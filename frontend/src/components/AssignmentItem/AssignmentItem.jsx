import Wrapper from '../Wrapper/Wrapper';
import Text from '../Text/Text';
import { Link } from 'react-router-dom';
import { deleteAssignment } from '../../services/assignments.service';
import classes from './AssignmentItem.module.css';
import { ROUTES } from '../../data/routes';
export const AssignmentItem = ({
    id,
    name,
    typeOfRequest,
    targetFieldName,
    OID,
}) => {
    function deleteHandler() {
        //TODO FIX IT
        const decision = window.confirm('Are you sure you want to delete?');
        if (decision) {
            deleteAssignment(id)
                .then((res) => res.json())
                .then((data) => {
                    alert(data.Detail);
                })
                .catch((err) => console.log(err));
        }
    }
    return (
        <Wrapper>
            <div className={classes.assignmentWrapper}>
                <div className={classes.text}>
                    <Text>
                        <span className={classes.assignmentTitle}>Name:</span>{' '}
                        {name}
                    </Text>
                    <Text>
                        <span className={classes.assignmentTitle}>
                            Type of request:
                        </span>{' '}
                        {typeOfRequest}
                    </Text>
                    <Text>
                        <span className={classes.assignmentTitle}>
                            Target field name:
                        </span>{' '}
                        {targetFieldName}
                    </Text>
                    <Text>
                        <span className={classes.assignmentTitle}>OID:</span>{' '}
                        {OID}
                    </Text>
                </div>
                <div className={classes.action}>
                    <Link
                        className={classes.link}
                        to={`${ROUTES.ASSIGNMENTS}/edit`}
                        state={{
                            id,
                            name,
                            typeOfRequest,
                            targetFieldName,
                            OID,
                        }}
                    >
                        <Text>Edit</Text>
                    </Link>
                    <Link
                        className={`${classes.link} ${classes.delete}`}
                        onClick={deleteHandler}
                    >
                        <Text>Delete</Text>
                    </Link>
                </div>
            </div>
        </Wrapper>
    );
};
