export const assignments = {
    assignments: import.meta.env.VITE_ASSIGNMENT_API_URL + '/Assignments',
    getAllAssignments: '/GetAssignments',
    getByIdAssignment: '/GetAssignmentById',
    createAssignment: '/CreateAssignment',
    deleteAssignment: '/DeleteAssignmentById',
};

export const testing = {
    testing: import.meta.env.VITE_TESTING_API_URL,
    getTesting: '/Get',
    walkTesting: '/Walk',
};
