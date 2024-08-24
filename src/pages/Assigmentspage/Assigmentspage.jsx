import Wrapper from '../../components/Wrapper/Wrapper';
import classes from './Assigmentspage.module.css';
import Text from '../../components/Text/Text';
import { Link } from 'react-router-dom';
export default function Assigmentspage() {
    return (
        <div className={classes.wrapper}>
            <div className={classes.wrapperCreate}>
                {' '}
                <Link
                    className={`${classes.link} ${classes.createLink}`}
                    to={'/assigments/create'}
                >
                    <Text>Add</Text>{' '}
                </Link>
            </div>

            <Wrapper>
                <div className={classes.assigmentWrapper}>
                    <div className={classes.text}>
                        <Text>
                            <span className={classes.assigmentTitle}>
                                Name:
                            </span>{' '}
                            Get Interfaces
                        </Text>
                        <Text>
                            <span className={classes.assigmentTitle}>
                                Type of request:
                            </span>{' '}
                            Walk
                        </Text>
                        <Text>
                            <span className={classes.assigmentTitle}>
                                Target field name:
                            </span>{' '}
                            Interfaces
                        </Text>
                        <Text>
                            <span className={classes.assigmentTitle}>OID:</span>{' '}
                            sysName - 1.3.6.1.2.1.1.5
                        </Text>
                    </div>
                    <div className={classes.action}>
                        <Link
                            className={classes.link}
                            to={'/assigments/update'}
                        >
                            <Text>Edit</Text>
                        </Link>
                        <Link className={classes.link}>
                            <Text>Delete</Text>
                        </Link>
                    </div>
                </div>
            </Wrapper>
            <Wrapper>
                <div className={classes.assigmentWrapper}>
                    <div className={classes.text}>
                        <Text>
                            <span className={classes.assigmentTitle}>
                                Name:
                            </span>{' '}
                            Get Interfaces
                        </Text>
                        <Text>
                            <span className={classes.assigmentTitle}>
                                Type of request:
                            </span>{' '}
                            Walk
                        </Text>
                        <Text>
                            <span className={classes.assigmentTitle}>
                                Target field name:
                            </span>{' '}
                            Interfaces
                        </Text>
                        <Text>
                            <span className={classes.assigmentTitle}>OID:</span>{' '}
                            sysName - 1.3.6.1.2.1.1.5
                        </Text>
                    </div>
                    <div className={classes.action}>
                        <Link className={classes.link}>
                            <Text>Edit</Text>
                        </Link>
                        <Link className={classes.link}>
                            <Text>Delete</Text>
                        </Link>
                    </div>
                </div>
            </Wrapper>
            <Wrapper>
                <div className={classes.assigmentWrapper}>
                    <div className={classes.text}>
                        <Text>
                            <span className={classes.assigmentTitle}>
                                Name:
                            </span>{' '}
                            Get Interfaces
                        </Text>
                        <Text>
                            <span className={classes.assigmentTitle}>
                                Type of request:
                            </span>{' '}
                            Walk
                        </Text>
                        <Text>
                            <span className={classes.assigmentTitle}>
                                Target field name:
                            </span>{' '}
                            Interfaces
                        </Text>
                        <Text>
                            <span className={classes.assigmentTitle}>OID:</span>{' '}
                            sysName - 1.3.6.1.2.1.1.5
                        </Text>
                    </div>
                    <div className={classes.action}>
                        <Link className={classes.link}>
                            <Text>Edit</Text>
                        </Link>
                        <Link className={classes.link}>
                            <Text>Delete</Text>
                        </Link>
                    </div>
                </div>
            </Wrapper>
            <Wrapper>
                <div className={classes.assigmentWrapper}>
                    <div className={classes.text}>
                        <Text>
                            <span className={classes.assigmentTitle}>
                                Name:
                            </span>{' '}
                            Get Interfaces
                        </Text>
                        <Text>
                            <span className={classes.assigmentTitle}>
                                Type of request:
                            </span>{' '}
                            Walk
                        </Text>
                        <Text>
                            <span className={classes.assigmentTitle}>
                                Target field name:
                            </span>{' '}
                            Interfaces
                        </Text>
                        <Text>
                            <span className={classes.assigmentTitle}>OID:</span>{' '}
                            sysName - 1.3.6.1.2.1.1.5
                        </Text>
                    </div>
                    <div className={classes.action}>
                        <Link className={classes.link}>
                            <Text>Edit</Text>
                        </Link>
                        <Link className={classes.link}>
                            <Text>Delete</Text>
                        </Link>
                    </div>
                </div>
            </Wrapper>
            <Wrapper>
                <div className={classes.assigmentWrapper}>
                    <div className={classes.text}>
                        <Text>
                            <span className={classes.assigmentTitle}>
                                Name:
                            </span>{' '}
                            Get Interfaces
                        </Text>
                        <Text>
                            <span className={classes.assigmentTitle}>
                                Type of request:
                            </span>{' '}
                            Walk
                        </Text>
                        <Text>
                            <span className={classes.assigmentTitle}>
                                Target field name:
                            </span>{' '}
                            Interfaces
                        </Text>
                        <Text>
                            <span className={classes.assigmentTitle}>OID:</span>{' '}
                            sysName - 1.3.6.1.2.1.1.5
                        </Text>
                    </div>
                    <div className={classes.action}>
                        <Link className={classes.link}>
                            <Text>Edit</Text>
                        </Link>
                        <Link className={classes.link}>
                            <Text>Delete</Text>
                        </Link>
                    </div>
                </div>
            </Wrapper>
            <Wrapper>
                <div className={classes.assigmentWrapper}>
                    <div className={classes.text}>
                        <Text>
                            <span className={classes.assigmentTitle}>
                                Name:
                            </span>{' '}
                            Get Interfaces
                        </Text>
                        <Text>
                            <span className={classes.assigmentTitle}>
                                Type of request:
                            </span>{' '}
                            Walk
                        </Text>
                        <Text>
                            <span className={classes.assigmentTitle}>
                                Target field name:
                            </span>{' '}
                            Interfaces
                        </Text>
                        <Text>
                            <span className={classes.assigmentTitle}>OID:</span>{' '}
                            sysName - 1.3.6.1.2.1.1.5
                        </Text>
                    </div>
                    <div className={classes.action}>
                        <Link className={classes.link}>
                            <Text>Edit</Text>
                        </Link>
                        <Link className={classes.link}>
                            <Text>Delete</Text>
                        </Link>
                    </div>
                </div>
            </Wrapper>
        </div>
    );
}
