import { lazy, Suspense, ReactNode } from "react";
import { createBrowserRouter, Navigate } from "react-router";
import App from "../layout/App";
import RouteSuspenseFallback from "../shared/components/RouteSuspenseFallback";
import ProtectedRoute from "./ProtectedRoute";
import HomePage from "../../features/home/HomePage";
import TestErrors from "../../features/Errors/TestErrors";
import NotFound from "../../features/Errors/NotFound";
import ServerError from "../../features/Errors/ServerError";
import AccessDenied from "../../features/Errors/AccessDenied";

// Feature pages are code-split so each route ships its own chunk.
const LoginPage = lazy(() => import("../../features/Auth/Login/LoginPage"));
const RegisterPage = lazy(() => import("../../features/Auth/Register/RegisterPage"));
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
const GateForm = lazy(() => import("../../features/Gates/CreateUpdate/GateForm"));
const AdminPage = lazy(() => import("../../features/Admin/AdminPage"));
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

export const router = createBrowserRouter([
  {
    path: "/",
    element: <App />,
    children: [
      // Public routes
      { path: "", element: <HomePage /> },
      { path: "login", element: page(<LoginPage />) },
      { path: "not-found", element: <NotFound /> },
      { path: "server-error", element: <ServerError /> },
      { path: "access-denied", element: <AccessDenied /> },

      // Authenticated routes (any role)
      { path: "mainPage", element: page(<ProtectedRoute><MainPageDashboard /></ProtectedRoute>) },
      { path: "mainPage/:id", element: page(<ProtectedRoute><MainPageDashboard /></ProtectedRoute>) },
      { path: "networkDevices", element: page(<ProtectedRoute><NetworkDeviceDashboard /></ProtectedRoute>) },
      {
        path: "networkDevices/:id",
        element: page(<ProtectedRoute><NetworkDeviceDetailPage /></ProtectedRoute>)
      },
      { path: "clients", element: page(<ProtectedRoute><ClientsDashboard /></ProtectedRoute>) },
      { path: "clients/:id", element: page(<ProtectedRoute><ClientDetailPage /></ProtectedRoute>) },
      { path: "tfPlans", element: page(<ProtectedRoute><TfPlansDashboard /></ProtectedRoute>) },
      { path: "sprVlans", element: page(<ProtectedRoute><SPRVlansDashboard /></ProtectedRoute>) },
      { path: "errors", element: page(<ProtectedRoute><TestErrors /></ProtectedRoute>) },

      // Admin-only routes (mirrors the backend [Authorize(Roles = "Admin")])
      { path: "register", element: page(<ProtectedRoute roles={["Admin"]}><RegisterPage /></ProtectedRoute>) },
      { path: "gates", element: page(<ProtectedRoute roles={["Admin"]}><GatesDashboard /></ProtectedRoute>) },
      { path: "gates/:id", element: page(<ProtectedRoute roles={["Admin"]}><GateForm /></ProtectedRoute>) },
      { path: "createGate", element: page(<ProtectedRoute roles={["Admin"]}><GateForm /></ProtectedRoute>) },
      { path: "admin", element: page(<ProtectedRoute roles={["Admin"]}><AdminPage /></ProtectedRoute>) },

      { path: "*", element: <Navigate replace to="/not-found" /> }
    ]
  }
]);
