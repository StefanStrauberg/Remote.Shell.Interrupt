import { assignments } from '../data/endpoints';

export function getAssignments() {
    return fetch(assignments.assignments + assignments.getAllAssignments);
}

export function createAssignment(data) {
    return fetch(assignments.assignments + assignments.createAssignment, {
        method: 'POST',
        body: JSON.stringify(data),
    });
}

export function getByIdAssignment(id) {
    return fetch(assignments.assignments + assignments.getByIdAssignment + id);
}

export function deleteAssignment(id) {
    return fetch(
        assignments.assignments + assignments.deleteAssignment + `/${id}`,
        {
            method: 'DELETE',
        }
    );
}
