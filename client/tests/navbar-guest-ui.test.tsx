// @vitest-environment jsdom
import { describe, expect, it } from "vitest";
import { screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import NavBar from "../src/app/layout/NavBar";
import { renderApp } from "./renderApp";

describe("NavBar for a signed-out visitor", () => {
  it("hides every workspace link and offers a sign-in link instead", () => {
    renderApp(<NavBar />);
    expect(
      screen.queryByRole("navigation", { name: "Main navigation" })
    ).toBeEmptyDOMElement();
    expect(
      screen.getAllByRole("link", { name: "Sign in" }).length
    ).toBeGreaterThan(0);
    expect(
      screen.queryByRole("button", { name: "Sign out" })
    ).not.toBeInTheDocument();
  });

  it("opens and closes the mobile navigation drawer", async () => {
    renderApp(<NavBar />);
    await userEvent.click(screen.getByRole("button", { name: "Open menu" }));
    const closeButton = await screen.findByRole("button", {
      name: "Close menu",
    });
    expect(closeButton).toBeVisible();
    await userEvent.click(closeButton);
  });
});
