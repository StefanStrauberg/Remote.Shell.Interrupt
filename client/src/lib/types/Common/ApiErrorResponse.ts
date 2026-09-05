/**
 * Shape of the JSON error payload returned by the API's
 * ExceptionHandlingMiddleware.
 *
 * The backend serializes this payload with a plain
 * `JsonSerializer.Serialize` call (not through MVC), so property names
 * keep their PascalCase declaration names — unlike regular DTO bodies,
 * which are camelCase.
 */
export type ApiErrorResponse = {
  Status: number;
  Title: string;
  Detail: string;
  Errors?: { [key: string]: string[] };
};
