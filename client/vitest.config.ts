import { defineConfig, mergeConfig } from "vitest/config";
import viteConfig from "./vite.config";

export default mergeConfig(
  viteConfig,
  defineConfig({
    test: {
      environment: "node",
      setupFiles: ["./tests/setup.ts"],
      restoreMocks: true,
      coverage: {
        provider: "v8",
        reporter: ["text", "html", "json-summary"],
        include: ["src/**/*.{ts,tsx}"],
        exclude: ["src/**/*.d.ts", "src/lib/types/**", "src/main.tsx"],
      },
    },
  })
);
