export const ASSIGNMENTS = {
    ASSIGNMENTS: import.meta.env.VITE_ASSIGNMENT_API_URL + '/Assignments',
    GET_All_ASSIGNMENTS: '/GetAssignments',
    GET_BY_ID_ASSIGNMENT: '/GetAssignmentById',
    CREATE_ASSIGNMENT: '/CreateAssignment',
    DELETE_ASSIGNMENT: '/DeleteAssignmentById',
    UPDATE_ASSIGNMENT: '/UpdateAssignment',
};

export const TESTING = {
    TESTING: import.meta.env.VITE_TESTING_API_URL,
    GET_TESTING: '/Get',
    WALK_TESTING: '/Walk',
};

export const GATEWAYS = {
    GATEWAYS: import.meta.env.VITE_ASSIGNMENT_API_URL + '/NetworkDevices',
    GET_ALL_GATEWAYS: '/GetNetworkDevices',
    GET_BY_IP_GATEWAY: '/GetNetworkDevicesByIP',
    GET_BY_ID_GATEWAY: '/GetNetworkDevicesByID',
    CREATE_GATEWAY: '/CreateNetworkDevice',
    DELETE_GATEWAY: '/DeleteNetworkDeviceById',
    GET_BY_TAG: '/GetNetworkDevicesByVlanTag',
};

export const RULES = {
    RULES: import.meta.env.VITE_ASSIGNMENT_API_URL + '/BusinessRules',
    GET_ALL_RULES: '/GetBusinessRules',
    CREATE_RULE: '/CreateBusinessRule',
    DELETE_BY_ID_RULE: '/DeleteBusinessRuleById',
    UPDATE_RULE: '/UpdateBusinessRule',
};

export const ORGANIZATIONS = {
    CLIENTCODS: import.meta.env.VITE_ASSIGNMENT_API_URL + '/ClientCODs',
    GET_CODS: '/GetClientsCOD',
    GET_COD_BY_NAME: '/GetClientsCODByName',
    GET_COD_BY_TAG: '/GetClientsCODByVlanTag',
};
