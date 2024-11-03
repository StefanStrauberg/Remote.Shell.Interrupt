import { RULES } from '../data/endpoints';

export function getRules() {
    return fetch(RULES.RULES + RULES.GET_ALL_RULES);
}
