// @vitest-environment jsdom
import { describe, expect, it } from "vitest";
import { screen } from "@testing-library/react";
import HomePage from "../src/features/home/HomePage";
import { renderApp } from "./renderApp";

describe("HomePage", () => {
  it("links visitors to sign in and the authenticated workspace", () => {
    renderApp(<HomePage />);
    expect(
      screen.getByRole("heading", { name: /Clarity across/ })
    ).toBeVisible();
    expect(screen.getByRole("link", { name: /Sign in/ })).toHaveAttribute(
      "href",
      "/login"
    );
    expect(
      screen.getByRole("link", { name: /Open workspace/ })
    ).toHaveAttribute("href", "/mainPage");
  });
});
