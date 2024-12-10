export const transformInTree = (data) => {
    const keys = {};
    data.key = self.crypto.randomUUID();
    keys[data.key] = true;
    data.label = 'Test';
    BFS(data, keys);
    return [data, keys];
};

const BFS = (currNode, keys) => {
    if (!currNode || !currNode?.children) return;
    const queue = [...currNode.children];
    for (let i = 0; i < queue.length; i++) {
        const node = queue[i];
        node.key = self.crypto.randomUUID();
        node.label = 'test';
        keys[node.key] = true;
        BFS(node, keys);
    }
};
