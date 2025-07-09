import { Box, Pagination, Typography } from "@mui/material";
import { NetworkDevice } from "../../lib/types/NetworkDevices/NetworkDevice";
import { PaginationMetadata } from "../../lib/types/Common/PaginationMetadata";
import NetworkDeviceCard from "./NetworkDeviceCard";

type Props = {
  networkDevices: NetworkDevice[] | undefined;
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
  // Loading state
  if (!networkDevices || isLoadingNetworkDevices)
    return <Typography>Loading ...</Typography>;

  // Handle page change
  const handlePageChange = (
    _event: React.ChangeEvent<unknown>,
    value: number
  ) => {
    setPageNumber(value); // Update the page number
  };

  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
      <Box
        sx={{
          display: "grid",
          gridTemplateColumns: "repeat(2, 1fr)", // Two columns
          gap: 3,
        }}
      >
        {networkDevices.map((networkDevice) => (
          <NetworkDeviceCard
            key={networkDevice.id}
            networkDevice={networkDevice}
          />
        ))}
      </Box>

      {/* Pagination Component */}
      <Pagination
        count={pagination.TotalPages || 1} // Total pages based on pagination metadata
        page={pageNumber} // Current active page
        onChange={handlePageChange} // Handle page change
        variant="outlined"
        color="primary"
        sx={{ alignSelf: "center", mt: 2 }}
      />
    </Box>
  );
}
