import {
  Box,
  Pagination,
  Typography,
  CircularProgress,
  Grid2,
} from "@mui/material";
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
      <Typography variant="h6" component="h2" gutterBottom>
        {pagination.TotalCount || 0} network devices found
      </Typography>

      <Grid2 container spacing={2}>
        {networkDevices.map((networkDevice) => (
          <Grid2 size={{ xs: 12, sm: 6, lg: 4 }} key={networkDevice.id}>
            <NetworkDeviceCard networkDevice={networkDevice} />
          </Grid2>
        ))}
      </Grid2>

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
