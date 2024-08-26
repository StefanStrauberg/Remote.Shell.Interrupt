import { testing } from '../data/endpoints';

export function getTesting(data) {
    return fetch(testing.testing + testing.getTesting, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
    });
}

export function walkTesting(data) {
    return fetch(testing.testing + testing.walkTesting, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
    });
}
