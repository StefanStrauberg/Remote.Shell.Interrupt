import Wrapper from '../../components/Wrapper/Wrapper';
import classes from './Assignmentspage.module.css';
import Text from '../../components/Text/Text';
import { Link } from 'react-router-dom';
import { deleteAssignment } from '../../services/assignments.service';
export default function Assigmentspage() {
    function deleteHandler() {
        const decision = window.confirm('Are you sure you want to delete?');
        if (decision) {
            deleteAssignment()
                .then((res) =>
                    res.status === 200
                        ? alert('Assignment was deleted')
                        : alert('Something went wrong')
                )
                .catch((err) => alert(err));
        }
    }
    return (
        <div className={classes.wrapper}>
            <Wrapper>
                <div className={classes.assignmentWrapper}>
                    <div className={classes.text}>
                        <Text>
                            <span className={classes.assignmentTitle}>
                                Name:
                            </span>{' '}
                            Get Interfaces
                        </Text>
                        <Text>
                            <span className={classes.assignmentTitle}>
                                Type of request:
                            </span>{' '}
                            Walk
                        </Text>
                        <Text>
                            <span className={classes.assignmentTitle}>
                                Target field name:
                            </span>{' '}
                            Interfaces
                        </Text>
                        <Text>
                            <span className={classes.assignmentTitle}>
                                OID:
                            </span>{' '}
                            sysName - 1.3.6.1.2.1.1.5
                        </Text>
                    </div>
                    <div className={classes.action}>
                        <Link className={classes.link} to={'/assignments/edit'}>
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
            <Wrapper>
                <div className={classes.assignmentWrapper}>
                    <div className={classes.text}>
                        <Text>
                            <span className={classes.assignmentTitle}>
                                Name:
                            </span>{' '}
                            Get Interfaces
                        </Text>
                        <Text>
                            <span className={classes.assignmentTitle}>
                                Type of request:
                            </span>{' '}
                            Walk
                        </Text>
                        <Text>
                            <span className={classes.assignmentTitle}>
                                Target field name:
                            </span>{' '}
                            Interfaces
                        </Text>
                        <Text>
                            <span className={classes.assignmentTitle}>
                                OID:
                            </span>{' '}
                            sysName - 1.3.6.1.2.1.1.5
                        </Text>
                    </div>
                    <div className={classes.action}>
                        <Link className={classes.link} to={'/assignments/edit'}>
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
        </div>
    );
}
