import { Box, Divider } from "@mui/material";
import { ReactNode } from "react";
import { Client } from "../../../lib/types/Clients/Client";
import ClientDetailCOD from "./ClientDetailCOD";
import ClientDetailCommercialContact from "./ClientDetailCommercialContact";
import ClientDetailGeneralInformation from "./ClientDetailGeneralInformation";
import ClientDetailHistory from "./ClientDetailHistory";
import ClientDetailNotes from "./ClientDetailNotes";
import ClientDetailSPRVlans from "./ClientDetailSPRVlans";
import ClientDetailTariffPlan from "./ClientDetailTariffPlan";
import ClientDetailTechnicalContact from "./ClientDetailTechnicalContact";

type Props = { client: Client };

function Section({ title, children }: { title: string; children: ReactNode }) {
  return (
    <Box component="section">
      <Divider textAlign="left" sx={{ px: 2, color: "text.secondary" }}>
        {title}
      </Divider>
      {children}
    </Box>
  );
}

export default function ClientDetailsSections({ client }: Props) {
  return (
    <>
      <Section title="General information">
        <ClientDetailGeneralInformation client={client} />
      </Section>
      <Section title="VLANs">
        <ClientDetailSPRVlans client={client} />
      </Section>
      <Section title="Tariff plan">
        <ClientDetailTariffPlan client={client} />
      </Section>
      <Section title="Notes">
        <ClientDetailNotes client={client} />
      </Section>
      <Section title="Commercial contact">
        <ClientDetailCommercialContact client={client} />
      </Section>
      <Section title="Technical contact">
        <ClientDetailTechnicalContact client={client} />
      </Section>
      <Section title="History">
        <ClientDetailHistory client={client} />
      </Section>
      <Section title="Data center">
        <ClientDetailCOD client={client} />
      </Section>
    </>
  );
}
