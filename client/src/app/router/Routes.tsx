import { createBrowserRouter, Navigate } from "react-router";
import App from "../layout/App";
import HomePage from "../../features/home/HomePage";
import NetworkDeviceList from "../../features/NetworkDevices/NetworkDeviceList";
import GateForm from "../../features/Gates/GateForm";
import TestErrors from "../../features/Errors/TestErrors";
import NotFound from "../../features/Errors/NotFound";
import ServerError from "../../features/Errors/ServerError";
import ClientDetailPage from "../../features/Clients/ClientDetailPage";
import ClientsDashboard from "../../features/Clients/ClientsDashboard";
import GatesDashboard from "../../features/Gates/GatesDashboard";
import TfPlansDashboard from "../../features/TfPlans/TfPlansDashboard";
import SPRVlansDashboard from "../../features/SPRVlans/SPRVlansDashboard";

export const router = createBrowserRouter([
  {
    path: "/",
    element: <App />,
    children: [
      { path: "", element: <HomePage /> },
      { path: "networkDevices", element: <NetworkDeviceList /> },
      { path: "gates", element: <GatesDashboard /> },
      { path: "gates/:id", element: <GateForm /> },
      { path: "createGate", element: <GateForm /> },
      { path: "clients", element: <ClientsDashboard /> },
      { path: "clients/:id", element: <ClientDetailPage /> },
      { path: "tfPlans", element: <TfPlansDashboard /> },
      { path: "sprVlans", element: <SPRVlansDashboard /> },
      { path: "errors", element: <TestErrors /> },
      { path: "not-found", element: <NotFound /> },
      { path: "server-error", element: <ServerError /> },
      { path: "*", element: <Navigate replace to="/not-found" /> },
    ],
  },
]);
