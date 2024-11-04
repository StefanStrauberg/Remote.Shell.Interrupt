export const transformInTree = (data) => {
    const root = data.find((el) => el.isRoot === true);
    root.key = self.crypto.randomUUID();
    return BFS(data, root);
};

const BFS = (data, currNode) => {
    const queue = [...currNode.children];
    currNode.children = [];
    while (queue.length) {
        const nodeId = queue.shift();
        const node = data.find((el) => el.id === nodeId);
        node.key = self.crypto.randomUUID();
        currNode.children.push(node);
        BFS(data, node);
    }
    return currNode;
};
