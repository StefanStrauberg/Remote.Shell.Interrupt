import { useState } from "react";
import { FilterDescriptor } from "../../lib/types/Common/FilterDescriptor";
import { useTfPlansQuery } from "./api/tfPlansQueries";
import EmptyPage from "../../app/shared/components/EmptyPage";
import { Grid2, Box } from "@mui/material";
import TfPlanListPage from "./TfPlanListPage";
import PriceCheckIcon from "@mui/icons-material/PriceCheck";
import { DEFAULT_PAGINATION } from "../../lib/constants/pagination";
import PageHeader from "../../app/shared/components/PageHeader";
import {
  PageError,
  PageLoading,
} from "../../app/shared/components/PageFeedback";

export default function TfPlansDashboard() {
  const [pageNumber, setPageNumber] = useState<number>(1);
  const [orderBy] = useState<string>("idTfPlan");
  const [orderByDescending] = useState<boolean>(false);
  const pageSize = 12;
  const filters: FilterDescriptor[] = [];

  const tfPlansQuery = useTfPlansQuery({
    pagination: { pageNumber, pageSize },
    filters,
    orderBy: { property: orderBy, descending: orderByDescending },
  });
  const tfPlans = tfPlansQuery.data?.data ?? [];
  const pagination = tfPlansQuery.data?.pagination ?? DEFAULT_PAGINATION;
  const isLoading = tfPlansQuery.isPending;
  const error = tfPlansQuery.error;

  if (isLoading) {
    return <PageLoading message="Loading tariff plans…" />;
  }

  if (error) {
    return <PageError title="Unable to load tariff plans" error={error} />;
  }

  return (
    <Box>
      <PageHeader
        title="Tariff plans"
        description="Browse service plans synchronized from the billing system"
        icon={PriceCheckIcon}
      />

      {tfPlans.length === 0 ? (
        <EmptyPage input="No tariff plans found" />
      ) : (
        <Grid2 container spacing={3}>
          <Grid2 size={12}>
            <TfPlanListPage
              tfPlans={tfPlans}
              isLoading={isLoading}
              pageNumber={pageNumber}
              pagination={pagination}
              setPageNumber={setPageNumber}
            />
          </Grid2>
        </Grid2>
      )}
    </Box>
  );
}
