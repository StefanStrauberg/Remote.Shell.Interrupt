import { useMutation } from "@tanstack/react-query";
import { useState } from "react";
import httpClient from "../../lib/api/httpClient";
import { ApiError } from "../../lib/api/ApiError";
import { Alert, Button, ButtonGroup, Typography } from "@mui/material";
import { API_V1_PREFIX } from "../../config/api.config";

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
        await httpClient.post(path, {});
      } else {
        await httpClient.get(path);
      }
    },
    onError: (err) => {
      if (err instanceof ApiError) {
        setValidationErrors(err.validationErrors);
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
        <Button
          onClick={() => handleError(`${API_V1_PREFIX}/Buggy/GetNotFound`)}
        >
          Not found
        </Button>
        <Button
          onClick={() => handleError(`${API_V1_PREFIX}/Buggy/GetBadRequest`)}
        >
          Bad request
        </Button>
        <Button
          onClick={() =>
            handleError(`${API_V1_PREFIX}/Gates/CreateGate`, "post")
          }
        >
          Validation error
        </Button>
        <Button
          onClick={() => handleError(`${API_V1_PREFIX}/Buggy/GetServerError`)}
        >
          Server error
        </Button>
        <Button
          onClick={() => handleError(`${API_V1_PREFIX}/Buggy/GetUnauthorized`)}
        >
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
