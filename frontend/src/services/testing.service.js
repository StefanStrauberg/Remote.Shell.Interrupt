import { TESTING } from '../data/endpoints';

export function getTesting(data) {
    return fetch(TESTING.TESTING + TESTING.GET_TESTING, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
    });
}

export function walkTesting(data) {
    return fetch(TESTING.TESTING + TESTING.WALK_TESTING, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
    });
}
