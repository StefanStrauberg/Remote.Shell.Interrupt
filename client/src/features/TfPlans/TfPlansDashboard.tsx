import { Grid2 } from "@mui/material";
import TfPlanListPage from "./TfPlanListPage";
import TfPlansEmptyPage from "./TfPlansEmptyPage";
import { useState } from "react";
import { useTfPlans } from "../../lib/hooks/useTfPlans";

export default function TfPlansDashboard() {
  // Manage local state for pagination
  const [pageNumber, setPageNumber] = useState<number>(1); // TablePagination uses zero-based index
  const pageSize = 15; // Default page size

  // Hook for fetching data
  const { tfPlans, pagination, isPending } = useTfPlans(pageNumber, pageSize);

  return (
    <>
      {tfPlans?.length === 0 ? (
        <TfPlansEmptyPage />
      ) : (
        <Grid2 container spacing={3}>
          <Grid2 size={12}>
            <TfPlanListPage
              tfPlans={tfPlans}
              isPending={isPending}
              pageNumber={pageNumber}
              pagination={pagination}
              setPageNumber={setPageNumber}
            />
          </Grid2>
        </Grid2>
      )}
    </>
  );
}
