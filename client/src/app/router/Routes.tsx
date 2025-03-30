import { createBrowserRouter, Navigate } from "react-router";
import App from "../layout/App";
import HomePage from "../../features/home/HomePage";
import NetworkDeviceList from "../../features/NetworkDevices/NetworkDeviceList";
import GateListPage from "../../features/Gates/GateListPage";
import GateDetailPage from "../../features/Gates/GateDetailPage";
import GateForm from "../../features/Gates/GateForm";
import TestErrors from "../../features/Errors/TestErrors";
import NotFound from "../../features/Errors/NotFound";
import ServerError from "../../features/Errors/ServerError";

export const router = createBrowserRouter([
  {
    path: "/",
    element: <App />,
    children: [
      { path: "", element: <HomePage /> },
      { path: "networkDevices", element: <NetworkDeviceList /> },
      { path: "gates", element: <GateListPage /> },
      { path: "gates/:id", element: <GateDetailPage /> },
      { path: "editGate/:id", element: <GateForm /> },
      { path: "createGate", element: <GateForm /> },
      { path: "errors", element: <TestErrors /> },
      { path: "not-found", element: <NotFound /> },
      { path: "server-error", element: <ServerError /> },
      { path: "*", element: <Navigate replace to="/not-found" /> },
    ],
  },
]);
