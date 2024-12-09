import classes from './Rulespage.module.css';
import { useEffect, useState } from 'react';
import { getRules } from '../../services/rules.service';
import { transformInTree } from '../../utils/transformInTree';
import Loader from '../../components/Loader/Loader';
import { Tree } from 'primereact/tree';
import { Link } from 'react-router-dom';
import Button from '../../components/UI/Button/Button';
import './Rulespage.css';
import Error from '../../components/Error/Error';
export default function Rulespage() {
    const [data, setData] = useState(null);
    const [isLoading, setIsLoading] = useState(false);
    const [expandedKeys, setExpandedKeys] = useState({});
    useEffect(() => {
        setIsLoading(true);
        getRules()
            .then((res) => res.json())
            .then((data) => {
                if (!data?.Status) {
                    setData(transformInTree(data)[0]);
                    setExpandedKeys(transformInTree(data)[1]);
                }
            })
            .finally(() => setIsLoading(false));
    }, []);

    const nodeTemplate = (node) => {
        return (
            <div className={classes.node}>
                <div className={classes.bold}>Id: {node.id}</div>
                <div className={classes.bold}>Name: {node.name}</div>
                <div className={classes.bold}>
                    ParentId: {node.parentId ?? 'null'}
                </div>
                <div className={classes.bold}>
                    Assignment id: {node.assignmentId}
                </div>
                <div className={classes.btns}>
                    <Link to={'/rules/edit'}>
                        {' '}
                        <Button classNames={classes.btn}>Edit</Button>
                    </Link>
                    <Button classNames={`${classes.btn} ${classes.delete}`}>
                        Delete
                    </Button>
                </div>
            </div>
        );
    };
    if (isLoading) return <Loader />;
    if (!data) return <Error />;
    console.log(data, expandedKeys);
    return (
        <div className={classes.treeWrapper}>
            <Tree
                value={[data]}
                expandedKeys={expandedKeys}
                nodeTemplate={nodeTemplate}
                className={`${classes.tree} custom-tree`}
            />
        </div>
    );
}
