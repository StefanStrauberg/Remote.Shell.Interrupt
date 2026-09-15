import { describe, expect, it } from "vitest";
import { isValidElement, ReactElement } from "react";
import { RouteObject } from "react-router";
import { routeConfig } from "../src/app/router/routeConfig";
import { routeSegments } from "../src/app/router/paths";

/**
 * Regression coverage for the ProtectedRoute wrapper every route element is
 * built with — imports routeConfig (a plain array) rather than the `router`
 * export, since createBrowserRouter() needs a DOM this project's Vitest
 * setup (plain Node, no jsdom) doesn't have.
 */
function rolesOf(route: RouteObject | undefined): string[] | undefined {
  if (!route || !isValidElement(route.element)) {
    throw new Error(`Route "${route?.path}" has no element to inspect.`);
  }

  // element is <Suspense><ProtectedRoute roles={...}>...</ProtectedRoute></Suspense>
  // (see the `page()` helper in routeConfig.tsx) for every guarded route.
  const suspenseChild = (
    route.element as ReactElement<{ children: ReactElement }>
  ).props.children;

  if (!isValidElement(suspenseChild)) {
    throw new Error(`Route "${route.path}" is not wrapped in ProtectedRoute.`);
  }

  return (suspenseChild.props as { roles?: string[] }).roles;
}

function findChild(path: string): RouteObject | undefined {
  return routeConfig[0].children?.find((c) => c.path === path);
}

describe("route guards", () => {
  it("restricts the debug error-trigger page to Admin", () => {
    // Regression for the specific bug: this route used to have no `roles`
    // prop at all, so any authenticated (non-Admin) user could reach a page
    // that fires real backend calls, including a POST to Gates/CreateGate.
    expect(rolesOf(findChild(routeSegments.errors))).toEqual(["Admin"]);
  });

  it.each([
    routeSegments.admin,
    routeSegments.adminUsers,
    routeSegments.adminWorkflows,
    routeSegments.gates,
    routeSegments.createGate,
    routeSegments.register,
  ])("restricts %s to Admin", (path) => {
    expect(rolesOf(findChild(path))).toEqual(["Admin"]);
  });

  it.each([
    routeSegments.main,
    routeSegments.networkDevices,
    routeSegments.clients,
    routeSegments.tariffPlans,
    routeSegments.sprVlans,
  ])("leaves %s open to any authenticated role", (path) => {
    expect(rolesOf(findChild(path))).toBeUndefined();
  });
});
