import { describe, expect, it } from "vitest";
import { buildRequestParams } from "../src/lib/api/common/buildRequestParams";
import { parsePaginationHeader } from "../src/lib/api/common/paged";
import { FilterOperator } from "../src/lib/types/Common/FilterOperator";

describe("API request helpers", () => {
  it("serializes pagination, sorting, and indexed filters", () => {
    expect(
      buildRequestParams(
        { pageNumber: 2, pageSize: 12 },
        { property: "Name", descending: true },
        [
          {
            PropertyPath: "Working",
            Operator: FilterOperator.Equals,
            Value: "true",
          },
        ]
      )
    ).toEqual({
      pageNumber: 2,
      pageSize: 12,
      orderBy: "Name",
      orderByDescending: true,
      "Filters[0].PropertyPath": "Working",
      "Filters[0].Operator": "Equals",
      "Filters[0].Value": "true",
    });
  });

  it("accepts a valid pagination header", () => {
    const metadata = {
      TotalCount: 21,
      PageSize: 10,
      CurrentPage: 2,
      TotalPages: 3,
      HasNext: true,
      HasPrevious: true,
    };

    expect(parsePaginationHeader(JSON.stringify(metadata))).toEqual(metadata);
  });

  it("falls back safely for missing or structurally invalid metadata", () => {
    expect(parsePaginationHeader(undefined)).toMatchObject({ TotalCount: 0 });
    expect(
      parsePaginationHeader(JSON.stringify({ TotalCount: "twenty" }))
    ).toMatchObject({ TotalCount: 0, TotalPages: 0 });
  });
});
