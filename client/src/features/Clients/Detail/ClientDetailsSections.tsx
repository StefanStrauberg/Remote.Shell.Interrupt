import { Box, Divider, Tab, Tabs, Typography } from "@mui/material";
import { ReactNode, useId, useState } from "react";
import { Client } from "../../../lib/types/Clients/Client";
import ClientDetailCOD from "./ClientDetailCOD";
import ClientDetailCommercialContact from "./ClientDetailCommercialContact";
import ClientDetailGeneralInformation from "./ClientDetailGeneralInformation";
import ClientDetailHistory from "./ClientDetailHistory";
import ClientDetailNotes from "./ClientDetailNotes";
import ClientDetailSPRVlans from "./ClientDetailSPRVlans";
import ClientDetailTariffPlan from "./ClientDetailTariffPlan";
import ClientDetailTechnicalContact from "./ClientDetailTechnicalContact";

function Section({ title, children }: { title: string; children: ReactNode }) {
  return (
    <Box component="section" sx={{ minWidth: 0, py: 1 }}>
      <Divider textAlign="left" sx={{ px: 2, color: "text.secondary" }}>
        {title}
      </Divider>
      {children}
    </Box>
  );
}
export default function ClientDetailsSections({ client }: { client: Client }) {
  const [tab, setTab] = useState(0);
  const id = useId();
  return (
    <>
      <Tabs
        value={tab}
        onChange={(_, next: number) => setTab(next)}
        variant="scrollable"
        scrollButtons="auto"
        aria-label="Customer detail sections"
        sx={{ borderBottom: 1, borderColor: "divider" }}
      >
        {["Overview", "Network & plan", "Contacts", "Notes & history"].map(
          (label, index) => (
            <Tab
              key={label}
              label={label}
              id={`${id}-tab-${index}`}
              aria-controls={`${id}-panel-${index}`}
            />
          )
        )}
      </Tabs>
      <Box
        role="tabpanel"
        id={`${id}-panel-${tab}`}
        aria-labelledby={`${id}-tab-${tab}`}
        sx={{ py: 2 }}
      >
        {tab === 0 && (
          <Box
            sx={{
              display: "grid",
              gridTemplateColumns: { xs: "1fr", md: "1fr 1fr" },
              gap: 2,
            }}
          >
            <Section title="General information">
              <ClientDetailGeneralInformation client={client} />
            </Section>
            <Section title="Data center">
              <ClientDetailCOD client={client} />
            </Section>
          </Box>
        )}
        {tab === 1 && (
          <>
            <Section title="VLANs">
              <ClientDetailSPRVlans client={client} />
            </Section>
            <Section title="Tariff plan">
              {client.tfPlan ? (
                <ClientDetailTariffPlan client={client} />
              ) : (
                <Typography sx={{ p: 2 }} color="text.secondary">
                  No tariff plan available.
                </Typography>
              )}
            </Section>
          </>
        )}
        {tab === 2 && (
          <Box
            sx={{
              display: "grid",
              gridTemplateColumns: { xs: "1fr", md: "1fr 1fr" },
              gap: 2,
            }}
          >
            <Section title="Technical contact">
              <ClientDetailTechnicalContact client={client} />
            </Section>
            <Section title="Commercial contact">
              <ClientDetailCommercialContact client={client} />
            </Section>
          </Box>
        )}
        {tab === 3 && (
          <>
            <Section title="Notes">
              <ClientDetailNotes client={client} />
            </Section>
            <Section title="History">
              <ClientDetailHistory client={client} />
            </Section>
          </>
        )}
      </Box>
    </>
  );
}
