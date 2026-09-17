import {
  Box,
  Pagination,
  Typography,
  CircularProgress,
  Grid2,
  Button,
  Stack,
  Table,
  TableHead,
  TableBody,
  TableRow,
  TableCell,
  TableContainer,
  Paper,
  ToggleButton,
  ToggleButtonGroup,
} from "@mui/material";
import { useState } from "react";
import { Link } from "react-router";
import { routes } from "@/app/router/paths";
import CopyValue from "@/app/shared/components/CopyValue";
import { NetworkDevice } from "../../../lib/types/NetworkDevices/NetworkDevice";
import { PaginationMetadata } from "../../../lib/types/Common/PaginationMetadata";
import NetworkDeviceCard from "./NetworkDeviceCard";

type Props = {
  networkDevices: NetworkDevice[];
  isLoadingNetworkDevices: boolean;
  pageNumber: number;
  pagination: PaginationMetadata;
  setPageNumber: (value: React.SetStateAction<number>) => void;
};

export default function NetworkDeviceListPage({
  networkDevices,
  isLoadingNetworkDevices,
  pageNumber,
  pagination,
  setPageNumber,
}: Props) {
  const [view, setView] = useState("table");
  // Handle page change
  const handlePageChange = (
    _event: React.ChangeEvent<unknown>,
    value: number
  ) => {
    setPageNumber(value);
  };

  // Show loading state
  if (isLoadingNetworkDevices) {
    return (
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        minHeight="200px"
      >
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
      <Stack
        direction="row"
        justifyContent="space-between"
        alignItems="center"
        flexWrap="wrap"
        gap={1}
      >
        <Typography variant="h6" component="h2" gutterBottom>
          {pagination.TotalCount || 0} network devices found
        </Typography>
        <ToggleButtonGroup
          value={view}
          exclusive
          size="small"
          onChange={(_, next: string | null) => {
            if (next) setView(next);
          }}
          aria-label="Device view"
        >
          <ToggleButton value="table">Table</ToggleButton>
          <ToggleButton value="cards">Cards</ToggleButton>
        </ToggleButtonGroup>
      </Stack>

      {view === "table" ? (
        <TableContainer component={Paper} variant="outlined">
          <Table aria-label="Network devices">
            <TableHead>
              <TableRow>
                <TableCell>Device</TableCell>
                <TableCell>Host</TableCell>
                <TableCell>Vendor</TableCell>
                <TableCell>Information</TableCell>
                <TableCell>Details</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {networkDevices.map((device) => (
                <TableRow key={device.id} hover>
                  <TableCell>
                    <Typography variant="subtitle2">
                      {device.networkDeviceName}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    <Stack direction="row" alignItems="center">
                      <Typography
                        variant="body2"
                        sx={{ fontFamily: "monospace", whiteSpace: "nowrap" }}
                      >
                        {device.host}
                      </Typography>
                      <CopyValue value={device.host} />
                    </Stack>
                  </TableCell>
                  <TableCell>{device.typeOfNetworkDevice}</TableCell>
                  <TableCell sx={{ maxWidth: 320 }}>
                    {device.generalInformation || "No information"}
                  </TableCell>
                  <TableCell>
                    <Button
                      component={Link}
                      to={routes.networkDevice(device.id)}
                      size="small"
                    >
                      View
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      ) : (
        <Grid2 container spacing={2}>
          {networkDevices.map((networkDevice) => (
            <Grid2 size={{ xs: 12, sm: 6, lg: 4 }} key={networkDevice.id}>
              <NetworkDeviceCard networkDevice={networkDevice} />
            </Grid2>
          ))}
        </Grid2>
      )}

      {/* Pagination Component */}
      {pagination.TotalPages > 1 && (
        <Box display="flex" justifyContent="center" mt={3}>
          <Pagination
            count={pagination.TotalPages}
            page={pageNumber}
            onChange={handlePageChange}
            color="primary"
            showFirstButton
            showLastButton
          />
        </Box>
      )}
    </Box>
  );
}
