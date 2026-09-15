import { lazy, Suspense, ReactNode } from "react";
import { Navigate, RouteObject } from "react-router";
import App from "../layout/App";
import RouteSuspenseFallback from "../shared/components/RouteSuspenseFallback";
import ProtectedRoute from "./ProtectedRoute";
import HomePage from "../../features/home/HomePage";
import TestErrors from "../../features/Errors/TestErrors";
import NotFound from "../../features/Errors/NotFound";
import ServerError from "../../features/Errors/ServerError";
import AccessDenied from "../../features/Errors/AccessDenied";
import { routeSegments, routes } from "./paths";
import RouteErrorPage from "./RouteErrorPage";

// Feature pages are code-split so each route ships its own chunk.
const LoginPage = lazy(() => import("../../features/Auth/Login/LoginPage"));
const RegisterPage = lazy(
  () => import("../../features/Auth/Register/RegisterPage")
);
const TfPlansDashboard = lazy(
  () => import("../../features/TfPlans/TfPlansDashboard")
);
const SPRVlansDashboard = lazy(
  () => import("../../features/SPRVlans/SPRVlansDashboard")
);
const ClientsDashboard = lazy(
  () => import("../../features/Clients/List/ClientsDashboard")
);
const ClientDetailPage = lazy(
  () => import("../../features/Clients/Detail/ClientDetailPage")
);
const GatesDashboard = lazy(
  () => import("../../features/Gates/List/GatesDashboard")
);
const GateForm = lazy(
  () => import("../../features/Gates/CreateUpdate/GateForm")
);
const AdminPage = lazy(() => import("../../features/Admin/AdminPage"));
const WorkflowsPage = lazy(
  () => import("../../features/Workflows/WorkflowsPage")
);
const WorkflowEditorPage = lazy(
  () => import("../../features/Workflows/WorkflowEditorPage")
);
const UsersDashboard = lazy(
  () => import("../../features/Users/List/UsersDashboard")
);
const NetworkDeviceDashboard = lazy(
  () => import("../../features/NetworkDevices/List/NetworkDeviceDashboard")
);
const MainPageDashboard = lazy(
  () => import("../../features/MainPage/MainPageDashboard")
);
const NetworkDeviceDetailPage = lazy(
  () => import("../../features/NetworkDevices/Detail/NetworkDeviceDetailPage")
);

function page(element: ReactNode) {
  return <Suspense fallback={<RouteSuspenseFallback />}>{element}</Suspense>;
}

// Kept in its own module (imported into Routes.tsx, which calls
// createBrowserRouter(routeConfig)) so tests can inspect which role each
// route requires without going through createBrowserRouter, which needs a
// DOM (this project's Vitest setup runs in plain Node).
export const routeConfig: RouteObject[] = [
  {
    path: "/",
    element: <App />,
    errorElement: <RouteErrorPage />,
    children: [
      // Public routes
      { path: "", element: <HomePage /> },
      { path: routeSegments.login, element: page(<LoginPage />) },
      { path: routeSegments.notFound, element: <NotFound /> },
      { path: routeSegments.serverError, element: <ServerError /> },
      { path: routeSegments.accessDenied, element: <AccessDenied /> },

      // Authenticated routes (any role)
      {
        path: routeSegments.main,
        element: page(
          <ProtectedRoute>
            <MainPageDashboard />
          </ProtectedRoute>
        ),
      },
      {
        path: `${routeSegments.main}/:id`,
        element: page(
          <ProtectedRoute>
            <MainPageDashboard />
          </ProtectedRoute>
        ),
      },
      {
        path: routeSegments.networkDevices,
        element: page(
          <ProtectedRoute>
            <NetworkDeviceDashboard />
          </ProtectedRoute>
        ),
      },
      {
        path: `${routeSegments.networkDevices}/:id`,
        element: page(
          <ProtectedRoute>
            <NetworkDeviceDetailPage />
          </ProtectedRoute>
        ),
      },
      {
        path: routeSegments.clients,
        element: page(
          <ProtectedRoute>
            <ClientsDashboard />
          </ProtectedRoute>
        ),
      },
      {
        path: `${routeSegments.clients}/:id`,
        element: page(
          <ProtectedRoute>
            <ClientDetailPage />
          </ProtectedRoute>
        ),
      },
      {
        path: routeSegments.tariffPlans,
        element: page(
          <ProtectedRoute>
            <TfPlansDashboard />
          </ProtectedRoute>
        ),
      },
      {
        path: routeSegments.sprVlans,
        element: page(
          <ProtectedRoute>
            <SPRVlansDashboard />
          </ProtectedRoute>
        ),
      },
      // Admin-only routes (mirrors the backend [Authorize(Roles = "Admin")])
      {
        path: routeSegments.adminWorkflows,
        element: page(
          <ProtectedRoute roles={["Admin"]}>
            <WorkflowsPage />
          </ProtectedRoute>
        ),
      },
      {
        path: `${routeSegments.adminWorkflows}/new`,
        element: page(
          <ProtectedRoute roles={["Admin"]}>
            <WorkflowEditorPage />
          </ProtectedRoute>
        ),
      },
      {
        path: `${routeSegments.adminWorkflows}/:id`,
        element: page(
          <ProtectedRoute roles={["Admin"]}>
            <WorkflowEditorPage />
          </ProtectedRoute>
        ),
      },
      {
        path: routeSegments.register,
        element: page(
          <ProtectedRoute roles={["Admin"]}>
            <RegisterPage />
          </ProtectedRoute>
        ),
      },
      {
        path: routeSegments.gates,
        element: page(
          <ProtectedRoute roles={["Admin"]}>
            <GatesDashboard />
          </ProtectedRoute>
        ),
      },
      {
        path: `${routeSegments.gates}/:id`,
        element: page(
          <ProtectedRoute roles={["Admin"]}>
            <GateForm />
          </ProtectedRoute>
        ),
      },
      {
        path: routeSegments.createGate,
        element: page(
          <ProtectedRoute roles={["Admin"]}>
            <GateForm />
          </ProtectedRoute>
        ),
      },
      {
        path: routeSegments.admin,
        element: page(
          <ProtectedRoute roles={["Admin"]}>
            <AdminPage />
          </ProtectedRoute>
        ),
      },
      {
        path: routeSegments.adminUsers,
        element: page(
          <ProtectedRoute roles={["Admin"]}>
            <UsersDashboard />
          </ProtectedRoute>
        ),
      },
      {
        // A debug page for manually triggering backend error responses
        // (including a real POST to Gates/CreateGate) - not meant for
        // regular users to reach at all, let alone unauthenticated-of-role.
        path: routeSegments.errors,
        element: page(
          <ProtectedRoute roles={["Admin"]}>
            <TestErrors />
          </ProtectedRoute>
        ),
      },

      { path: "*", element: <Navigate replace to={routes.notFound} /> },
    ],
  },
];
