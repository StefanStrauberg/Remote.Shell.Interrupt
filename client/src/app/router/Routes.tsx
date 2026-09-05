import { lazy, Suspense } from "react";
import { createBrowserRouter, Navigate } from "react-router";
import App from "../layout/App";
import RouteSuspenseFallback from "../shared/components/RouteSuspenseFallback";
import HomePage from "../../features/home/HomePage";
import TestErrors from "../../features/Errors/TestErrors";
import NotFound from "../../features/Errors/NotFound";
import ServerError from "../../features/Errors/ServerError";

// Feature pages are code-split so each route ships its own chunk.
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

function page(element: React.ReactNode) {
  return <Suspense fallback={<RouteSuspenseFallback />}>{element}</Suspense>;
}

export const router = createBrowserRouter([
  {
    path: "/",
    element: <App />,
    children: [
      { path: "", element: <HomePage /> },
      { path: "mainPage", element: page(<MainPageDashboard />) },
      { path: "mainPage/:id", element: page(<MainPageDashboard />) },
      { path: "networkDevices", element: page(<NetworkDeviceDashboard />) },
      {
        path: "networkDevices/:id",
        element: page(<NetworkDeviceDetailPage />),
      },
      { path: "gates", element: page(<GatesDashboard />) },
      { path: "gates/:id", element: page(<GateForm />) },
      { path: "createGate", element: page(<GateForm />) },
      { path: "clients", element: page(<ClientsDashboard />) },
      { path: "clients/:id", element: page(<ClientDetailPage />) },
      { path: "tfPlans", element: page(<TfPlansDashboard />) },
      { path: "sprVlans", element: page(<SPRVlansDashboard />) },
      { path: "admin", element: page(<AdminPage />) },
      // Error pages stay eager: they must render even if other chunks fail.
      { path: "errors", element: <TestErrors /> },
      { path: "not-found", element: <NotFound /> },
      { path: "server-error", element: <ServerError /> },
      { path: "*", element: <Navigate replace to="/not-found" /> },
    ],
  },
]);
