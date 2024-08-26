import Tree from 'react-d3-tree';
import classes from './Rulespage.module.css';
import { useEffect, useRef, useState } from 'react';
import Button from '../../components/UI/Button/Button';
import Text from '../../components/Text/Text';
import { Link } from 'react-router-dom';

const data = {
    name: 'Root',
    children: [
        {
            name: 'Chiiohgjfoihjfdoihjofgidhold 1',
            condition: 'condition',
            children: [
                {
                    name: 'Grandchild 1',
                    condition: 'condition 2',
                    children: [{ name: '123', condition: 'last' }],
                },
                {
                    name: 'Grandchild 2',
                    children: [
                        {
                            name: '123',
                            condition: 'last',
                            children: [
                                {
                                    name: '123',
                                    children: [
                                        { name: '123', condition: 'last' },
                                    ],
                                    condition: 'last',
                                },
                            ],
                        },
                    ],
                },
                {
                    name: 'Grandchild 1',
                    condition: 'condition 2',
                    children: [{ name: '123', condition: 'last' }],
                },
                {
                    name: 'Grandchild 1',
                    condition: 'condition 2',
                    children: [{ name: '123', condition: 'last' }],
                },
                {
                    name: 'Grandchild 1',
                    condition: 'condition 2',
                    children: [{ name: '123', condition: 'last' }],
                },
                {
                    name: 'Grandchild 1',
                    condition: 'condition 2',
                    children: [{ name: '123', condition: 'last' }],
                },
                {
                    name: 'Grandchild 1',
                    condition: 'condition 2',
                    children: [{ name: '123', condition: 'last' }],
                },
                {
                    name: 'Grandchild 1',
                    condition: 'condition 2',
                    children: [{ name: '123', condition: 'last' }],
                },
                {
                    name: 'Grandchild 1',
                    condition: 'condition 2',
                    children: [{ name: '123', condition: 'last' }],
                },
                {
                    name: 'Grandchild 1',
                    condition: 'condition 2',
                    children: [{ name: '123', condition: 'last' }],
                },
                {
                    name: 'Grandchild 1',
                    condition: 'condition 2',
                    children: [{ name: '123', condition: 'last' }],
                },
                {
                    name: 'Grandchild 1',
                    condition: 'condition 2',
                    children: [{ name: '123', condition: 'last' }],
                },
                {
                    name: 'Grandchild 1',
                    condition: 'condition 2',
                    children: [{ name: '123', condition: 'last' }],
                },
            ],
        },
        {
            name: 'Child 2adsfasdfasdfgisodfoisdfjgiosdjfgiojosidfgfgisjdgooigsodgojsgofasdfsadfasdfafdfggkafsgksdfg',
        },
    ],
};

const renderRectSvgNode = ({ nodeDatum }) => (
    <g>
        <foreignObject width={200} height={200} x={-20} y={-20}>
            <div className={classes.block}>
                <Text>
                    <span className={classes.bold}>Name:</span> {nodeDatum.name}
                </Text>
                {nodeDatum.condition && (
                    <Text>
                        <span className={classes.bold}>Condition:</span>{' '}
                        {nodeDatum.condition}
                    </Text>
                )}
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
        </foreignObject>
    </g>
);

export default function Rulespage() {
    const treeRef = useRef();
    const [x, setX] = useState(0);
    const [y, setY] = useState(0);
    useEffect(() => {
        if (treeRef.current) {
            setX(
                treeRef.current.children[0].children[1].children[0].getBBox()
                    .width
            );
            setY(
                treeRef.current.children[0].children[1].children[0].getBBox()
                    .height
            );
        }
    }, []);

    return (
        <div
            className={classes.tree}
            style={{
                width: `${1.2 * x}px`,
                height: `${1.2 * y}px`,
                background: '#fff',
                borderRadius: '10px',
            }}
            ref={treeRef}
        >
            <Tree
                data={data}
                renderCustomNodeElement={renderRectSvgNode}
                orientation="vertical"
                translate={{
                    x: x / 1.6,
                    y: y / 10,
                }}
                draggable={false}
                zoomable={false}
                separation={{
                    siblings: 2,
                }}
                pathFunc={'elbow'}
            />
        </div>
    );
}
