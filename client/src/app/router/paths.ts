export const routeSegments = {
  login: "login",
  register: "register",
  main: "mainPage",
  networkDevices: "networkDevices",
  clients: "clients",
  tariffPlans: "tfPlans",
  sprVlans: "sprVlans",
  gates: "gates",
  createGate: "createGate",
  admin: "admin",
  adminUsers: "admin/users",
  adminWorkflows: "admin/workflows",
  errors: "errors",
  notFound: "not-found",
  serverError: "server-error",
  accessDenied: "access-denied",
} as const;

const path = (segment: string) => `/${segment}`;

export const routes = {
  home: "/",
  login: path(routeSegments.login),
  register: path(routeSegments.register),
  main: path(routeSegments.main),
  networkDevices: path(routeSegments.networkDevices),
  networkDevice: (id: string) =>
    `${path(routeSegments.networkDevices)}/${encodeURIComponent(id)}`,
  clients: path(routeSegments.clients),
  client: (id: string | number) =>
    `${path(routeSegments.clients)}/${encodeURIComponent(String(id))}`,
  tariffPlans: path(routeSegments.tariffPlans),
  sprVlans: path(routeSegments.sprVlans),
  gates: path(routeSegments.gates),
  gate: (id: string) =>
    `${path(routeSegments.gates)}/${encodeURIComponent(id)}`,
  createGate: path(routeSegments.createGate),
  admin: path(routeSegments.admin),
  adminUsers: path(routeSegments.adminUsers),
  adminWorkflows: path(routeSegments.adminWorkflows),
  createWorkflow: `${path(routeSegments.adminWorkflows)}/new`,
  workflow: (id: string) =>
    `${path(routeSegments.adminWorkflows)}/${encodeURIComponent(id)}`,
  errors: path(routeSegments.errors),
  notFound: path(routeSegments.notFound),
  serverError: path(routeSegments.serverError),
  accessDenied: path(routeSegments.accessDenied),
} as const;
