import httpClient from "@/lib/api/httpClient";
import { apiPath } from "@/config/api.config";
import { buildRequestParams } from "@/lib/api/common/buildRequestParams";
import { fetchPaged } from "@/lib/api/common/paged";
import { FilterOperator } from "@/lib/types/Common/FilterOperator";
import {
  WorkflowDefinition,
  WorkflowSummary,
  ExecutionRequest,
  ExecutionResult,
} from "../domain/workflow/model";
import { workflowPayload } from "../domain/workflow/graph";
const endpoint = (action: string, id?: string) =>
  id ? apiPath("Workflows", action, id) : apiPath("Workflows", action);
export const workflowsApi = {
  list(page: number, search = "", exact = false) {
    return fetchPaged<WorkflowSummary>(
      endpoint("GetWorkflowsByFilter"),
      buildRequestParams(
        { pageNumber: page, pageSize: 12 },
        { property: "Name", descending: false },
        search
          ? [
              {
                PropertyPath: "Name",
                Operator: exact
                  ? FilterOperator.Equals
                  : FilterOperator.Contains,
                Value: search,
              },
            ]
          : []
      )
    );
  },
  async get(id: string): Promise<WorkflowDefinition> {
    return (
      await httpClient.get<WorkflowDefinition>(endpoint("GetWorkflowById", id))
    ).data;
  },
  async create(graph: WorkflowDefinition): Promise<WorkflowDefinition | null> {
    // CreateWorkflow's response body is the new workflow's ID (a plain GUID string) - fetched
    // by primary key below, not by re-querying for a name match, which was ambiguous under a
    // concurrent create/rename sharing that name.
    const response = await httpClient.post<string>(
      endpoint("CreateWorkflow"),
      workflowPayload(graph)
    );
    try {
      return await workflowsApi.get(response.data);
    } catch {
      return null;
    } // Creation succeeded; never retry POST because a follow-up GET failed.
  },
  async update(graph: WorkflowDefinition) {
    await httpClient.put(endpoint("UpdateWorkflow"), {
      id: graph.id,
      ...workflowPayload(graph),
    });
  },
  async publish(id: string) {
    await httpClient.post(endpoint("PublishWorkflow", id));
  },
  async archive(id: string) {
    await httpClient.post(endpoint("ArchiveWorkflow", id));
  },
  async remove(id: string) {
    await httpClient.delete(endpoint("DeleteWorkflowById", id));
  },
  async execute(
    id: string,
    request: ExecutionRequest,
    signal: AbortSignal
  ): Promise<ExecutionResult> {
    return (
      await httpClient.post<ExecutionResult>(
        endpoint("ExecuteWorkflow", id),
        request,
        { signal, timeout: 0 }
      )
    ).data;
  },
};
