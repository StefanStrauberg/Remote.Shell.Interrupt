import { ASSIGNMENTS } from '../data/endpoints';

export function getAssignments() {
    return fetch(ASSIGNMENTS.ASSIGNMENTS + ASSIGNMENTS.GET_All_ASSIGNMENTS);
}

export function createAssignment(data) {
    return fetch(ASSIGNMENTS.ASSIGNMENTS + ASSIGNMENTS.CREATE_ASSIGNMENT, {
        method: 'POST',
        body: JSON.stringify(data),
    });
}

export function getByIdAssignment(id) {
    return fetch(
        ASSIGNMENTS.ASSIGNMENTS + ASSIGNMENTS.GET_BY_ID_ASSIGNMENT + `/${id}`
    );
}

export function deleteAssignment(id) {
    return fetch(
        ASSIGNMENTS.ASSIGNMENTS + ASSIGNMENTS.DELETE_ASSIGNMENT + `/${id}`,
        {
            method: 'DELETE',
        }
    );
}

export function updateAssignment(data) {
    console.log(data);
    return fetch(ASSIGNMENTS.ASSIGNMENTS + ASSIGNMENTS.UPDATE_ASSIGNMENT, {
        method: 'PUT',
        body: JSON.stringify(data),
    });
}
