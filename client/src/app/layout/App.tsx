import { Box, Container, CssBaseline } from "@mui/material";
import NavBar from "./NavBar";
import NetworkDeviceDashboard from "../../features/NetworkDevices/Dashboard/NetworkDeviceDashboard";
import "./styles.css";

const testNetworkDevices = [
  {
    id: "638345cc-02df-408b-af43-23cdde48929a",
    host: "192.168.101.2",
    typeOfNetworkDevice: "Juniper",
    networkDeviceName: "Test Name 1",
    generalInformation: "Test Information 1",
    portsOfNetworkDevice: [
      {
        id: "5fa79cdf-6d4c-4e39-aff1-5ac44b3583e4",
        interfaceNumber: 723,
        interfaceName: "ae0",
        interfaceType: "ieee8023adLag",
        interfaceStatus: "down",
        interfaceSpeed: 0,
        isAggregated: false,
        macAddress: "40:A6:77:42:93:C0",
        description: "",
        aggregatedPorts: [],
        macTable: [],
        arpTableOfPort: {},
        networkTableOfPort: {},
        vlaNs: [
          {
            vlanTag: 141,
            vlanName: "z141_ITV",
          },
        ],
      },
    ],
  },
  {
    id: "638345cc-02df-408b-af43-23cdde48922a",
    host: "192.168.101.5",
    typeOfNetworkDevice: "Extreme",
    networkDeviceName: "Test Name 2",
    generalInformation: "Test Information 2",
    portsOfNetworkDevice: [
      {
        id: "5fa79cdf-6d4c-4e39-aff1-5ac44b3583e4",
        interfaceNumber: 723,
        interfaceName: "ae0",
        interfaceType: "ieee8023adLag",
        interfaceStatus: "down",
        interfaceSpeed: 0,
        isAggregated: false,
        macAddress: "40:A6:77:42:93:C0",
        description: "",
        aggregatedPorts: [],
        macTable: [],
        arpTableOfPort: {},
        networkTableOfPort: {},
        vlaNs: [
          {
            vlanTag: 829,
            vlanName: "829_ZuZu",
          },
          {
            vlanTag: 515,
            vlanName: "515_MuMu",
          },
        ],
      },
    ],
  },
];

function App() {
  return (
    <Box>
      <CssBaseline />
      <NavBar />
      <Container>
        <NetworkDeviceDashboard networkDevices={testNetworkDevices} />
      </Container>
    </Box>
  );
}

export default App;
