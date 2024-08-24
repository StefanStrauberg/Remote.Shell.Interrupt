import Tree from 'react-d3-tree';
import classes from './Rulespage.module.css';
import { useEffect, useRef, useState } from 'react';

const data = {
    name: 'Root',
    children: [
        {
            name: 'Child 1',
            condition: 'condition',
            children: [
                {
                    name: 'Grandchild 1',
                    condition: 'condition 2',
                    children: [{ name: '123', condition: 'last' }],
                },
                { name: 'Grandchild 2' },
            ],
        },
        {
            name: 'Child 2',
        },
    ],
};

const renderRectSvgNode = ({ nodeDatum, toggleNode }) => (
    <g>
        <rect width="20" height="20" x="-10" onClick={toggleNode} />
        <text
            fill="#222325"
            strokeWidth="0"
            x="20"
            fontSize={14}
            fontWeight={600}
        >
            {nodeDatum.name}
        </text>
        <text x={20} y={20} strokeWidth={0} fill="#1B1C1E">
            {nodeDatum.condition}
        </text>
    </g>
);

export default function Rulespage() {
    const treeRef = useRef();
    const [x, setX] = useState(0);
    const [y, setY] = useState(0);
    useEffect(() => {
        if (treeRef.current) {
            setX(treeRef.current.clientWidth);
            setY(treeRef.current.clientHeight);
        }
    }, []);
    return (
        <div
            className={classes.tree}
            style={{
                width: '100%',
                height: '30rem',
                background: '#fff',
                borderRadius: '10px',
            }}
            ref={treeRef}
        >
            <Tree
                data={data}
                renderCustomNodeElement={renderRectSvgNode}
                orientation="horizontal"
                translate={{
                    x: x / 3,
                    y: y / 2,
                }}
            />
        </div>
    );
}
