import { useState } from "react";
import { FilterDescriptor } from "../../lib/types/Common/FilterDescriptor";
import { useTfPlans } from "../../lib/hooks/useTfPlans";
import EmptyPage from "../../app/shared/components/EmptyPage";
import { Grid2 } from "@mui/material";
import TfPlanListPage from "./TfPlanListPage";

export default function TfPlansDashboard() {
  const [pageNumber, setPageNumber] = useState<number>(1);
  const pageSize = 15;

  const filters: FilterDescriptor[] = [];

  const { tfPlans, pagination, isLoading } = useTfPlans(
    { pageNumber, pageSize },
    filters
  );

  return (
    <>
      {tfPlans?.length === 0 ? (
        <EmptyPage input="Тарифные планы не найдены" />
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
    </>
  );
}
