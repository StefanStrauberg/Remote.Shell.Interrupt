import classes from './Rulespage.module.css';
import { useEffect, useState } from 'react';
import { getRules } from '../../services/rules.service';
import { transformInTree } from '../../utils/transformInTree';
import Loader from '../../components/Loader/Loader';
import { Tree } from 'primereact/tree';
import { Link } from 'react-router-dom';
import Button from '../../components/UI/Button/Button';
import './Rulespage.css';
export default function Rulespage() {
    const [data, setData] = useState(null);
    const [expandedKeys, setExpandedKeys] = useState({});
    const expandAll = (nodes) => {
        const _expandedKeys = {};
        nodes.forEach((node) => (_expandedKeys[node.key] = true));
        setExpandedKeys(_expandedKeys);
    };
    useEffect(() => {
        getRules()
            .then((res) => res.json())
            .then((data) => {
                setData(transformInTree(data));
                expandAll(data);
            });
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

    return data ? (
        <div className={classes.treeWrapper}>
            <Tree
                value={[data]}
                expandedKeys={expandedKeys}
                nodeTemplate={nodeTemplate}
                onToggle={() => true}
                className={`${classes.tree} custom-tree`}
            />
        </div>
    ) : (
        <Loader />
    );
}
