import { useMutation } from "@tanstack/react-query";
import { useState } from "react";
import agent from "../../lib/api/agent";
import { Alert, Button, ButtonGroup, Typography } from "@mui/material";

export default function TestErrors() {
  const [validationErrors, setValidationErrors] = useState<string[]>([]);

  const { mutate } = useMutation({
    mutationFn: async ({
      path,
      method = "get",
    }: {
      path: string;
      method: string;
    }) => {
      if (method === "post") {
        await agent.post(path, {});
      } else {
        await agent.get(path);
      }
    },
    onError: (err) => {
      if (Array.isArray(err)) {
        setValidationErrors(err);
      } else {
        setValidationErrors([]);
      }
    },
  });

  const handleError = (path: string, method = "get") => {
    mutate({ path, method });
  };

  return (
    <>
      <Typography variant="h4">Test errors component</Typography>

      <ButtonGroup variant="contained" sx={{ mt: 4 }}>
        <Button onClick={() => handleError("Buggy/GetNotFound")}>
          Not found
        </Button>
        <Button onClick={() => handleError("Buggy/GetBadRequest")}>
          Bad request
        </Button>
        <Button onClick={() => handleError("Gates/CreateGate", "post")}>
          Validation error
        </Button>
        <Button onClick={() => handleError("Buggy/GetServerError")}>
          Server error
        </Button>
        <Button onClick={() => handleError("Buggy/GetUnauthorized")}>
          Unauthorised
        </Button>
      </ButtonGroup>

      {validationErrors.map((err, i) => (
        <Alert key={i} severity="error">
          {err}
        </Alert>
      ))}
    </>
  );
}
