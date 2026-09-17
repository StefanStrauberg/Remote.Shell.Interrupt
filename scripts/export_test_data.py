#!/usr/bin/env python3
"""Export Clients / TfPlans / SPRVlans from the docker-compose PostgreSQL
into plain-SQL dump files, ready to load into a test database in another
docker container.

Requirements:
  - Docker (with the `docker compose` v2 plugin) available on PATH.
  - The repo's `postgres` service (see docker-compose.yml) already running,
    e.g.:  docker compose up -d postgres

Usage:
  python scripts/export_test_data.py
  python scripts/export_test_data.py --combined
  python scripts/export_test_data.py --readable
  python scripts/export_test_data.py --tables Clients TfPlans SPRVlans CODs
  python scripts/export_test_data.py --service postgres --output-dir db_export
  python scripts/export_test_data.py --container testing --db testing --user admin --password '!QAZxsw2'

  The last form targets a plain `docker run` container (not part of this
  repo's `docker compose` stack) directly via `docker exec`, e.g. a
  standalone test-data container named "testing".

Notes:
  - Dumps are DATA-ONLY (`--data-only`), plain SQL, `--disable-triggers`
    (so INSERT/COPY statements bypass FK-check triggers on load, avoiding
    table-order problems) and `--no-owner --no-privileges` (portable across
    environments with different role names).
  - `Clients.Id_COD` is a required (non-nullable) foreign key into the `CODs`
    table, which is NOT one of the three requested tables. If your test
    database does not already have matching CODs rows, either:
      * seed/copy CODs data into the test DB first, or
      * re-run with `--tables Clients TfPlans SPRVlans CODs` to include it.
    `Clients.Id_TfPlan` (nullable FK to TfPlans) is covered since TfPlans is
    already part of the default export.
"""
from __future__ import annotations

import argparse
import datetime
import subprocess
import sys
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parent.parent
DEFAULT_TABLES = ["Clients", "TfPlans", "SPRVlans"]
DEFAULT_ENV = {
    "POSTGRES_DB": "postgres",
    "POSTGRES_USER": "postgres",
    "POSTGRES_PASSWORD": "postgres",
}


def load_env_defaults() -> dict[str, str]:
    """Read POSTGRES_DB/USER/PASSWORD from .env if present, else fall back
    to the defaults baked into docker-compose.yml."""
    values = dict(DEFAULT_ENV)
    env_file = REPO_ROOT / ".env"
    if env_file.exists():
        for line in env_file.read_text(encoding="utf-8").splitlines():
            line = line.strip()
            if not line or line.startswith("#") or "=" not in line:
                continue
            key, _, val = line.partition("=")
            key = key.strip()
            if key in values:
                values[key] = val.strip().strip('"').strip("'")
    return values


def compose_service_container_id(service: str) -> str | None:
    result = subprocess.run(
        ["docker", "compose", "ps", "-q", service],
        cwd=REPO_ROOT,
        capture_output=True,
        text=True,
    )
    container_id = result.stdout.strip()
    return container_id or None


def container_is_running(container: str) -> bool:
    result = subprocess.run(
        ["docker", "inspect", "-f", "{{.State.Running}}", container],
        capture_output=True,
        text=True,
    )
    return result.returncode == 0 and result.stdout.strip() == "true"


def run_pg_dump(
    exec_cmd: list[str],
    db: str,
    user: str,
    tables: list[str],
    out_file: Path,
    readable: bool,
) -> None:
    cmd = [
        *exec_cmd,
        "pg_dump",
        "-U", user,
        "-d", db,
        "--data-only",
        "--disable-triggers",
        "--no-owner",
        "--no-privileges",
    ]
    if readable:
        cmd.append("--column-inserts")
    for table in tables:
        cmd += ["--table", f'public."{table}"']

    with out_file.open("wb") as f:
        proc = subprocess.run(cmd, cwd=REPO_ROOT, stdout=f, stderr=subprocess.PIPE)

    if proc.returncode != 0:
        out_file.unlink(missing_ok=True)
        raise RuntimeError(proc.stderr.decode("utf-8", errors="replace"))


def main() -> None:
    parser = argparse.ArgumentParser(
        description="Export Clients/TfPlans/SPRVlans to plain-SQL dump files.",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog=__doc__,
    )
    parser.add_argument(
        "--tables", nargs="+", default=DEFAULT_TABLES,
        help=f"Tables to export (default: {' '.join(DEFAULT_TABLES)})",
    )
    parser.add_argument(
        "--output-dir", default="db_export",
        help="Directory (relative to repo root) to write dump files into (default: db_export)",
    )
    parser.add_argument(
        "--service", default="postgres",
        help="docker-compose service name for PostgreSQL (default: postgres). "
             "Ignored if --container is given.",
    )
    parser.add_argument(
        "--container",
        help="Name/ID of a plain `docker run` PostgreSQL container to export from "
             "directly via `docker exec`, bypassing `docker compose` entirely "
             "(use this for a standalone test-data container that isn't part of "
             "this repo's compose stack, e.g. --container testing).",
    )
    parser.add_argument(
        "--db", help="Database name (overrides POSTGRES_DB from .env/defaults)",
    )
    parser.add_argument(
        "--user", help="Database user (overrides POSTGRES_USER from .env/defaults)",
    )
    parser.add_argument(
        "--password",
        help="Database password (overrides POSTGRES_PASSWORD from .env/defaults). "
             "Only needed if pg_dump inside the container requires it interactively; "
             "usually the container's own POSTGRES_PASSWORD env var already covers this.",
    )
    parser.add_argument(
        "--combined", action="store_true",
        help="Write a single combined .sql file instead of one file per table",
    )
    parser.add_argument(
        "--readable", action="store_true",
        help="Use --column-inserts (verbose, human-readable INSERT statements) "
             "instead of the default compact COPY format",
    )
    args = parser.parse_args()

    env = load_env_defaults()
    if args.db:
        env["POSTGRES_DB"] = args.db
    if args.user:
        env["POSTGRES_USER"] = args.user
    if args.password:
        env["POSTGRES_PASSWORD"] = args.password

    out_dir = REPO_ROOT / args.output_dir
    out_dir.mkdir(parents=True, exist_ok=True)

    if args.container:
        if not container_is_running(args.container):
            print(
                f"Container '{args.container}' is not running.\n"
                f"Start it first:\n  docker start {args.container}",
                file=sys.stderr,
            )
            sys.exit(1)
        exec_cmd = ["docker", "exec", args.container]
    else:
        container_id = compose_service_container_id(args.service)
        if not container_id:
            print(
                f"Postgres service '{args.service}' is not running.\n"
                f"Start it first:\n  docker compose up -d {args.service}\n"
                f"(If your test data lives in a standalone container instead of this "
                f"repo's compose stack, use --container <name> instead of --service.)",
                file=sys.stderr,
            )
            sys.exit(1)
        exec_cmd = ["docker", "compose", "exec", "-T", args.service]

    timestamp = datetime.datetime.now().strftime("%Y%m%d_%H%M%S")
    written: list[Path] = []

    try:
        if args.combined:
            out_file = out_dir / f"export_{timestamp}.sql"
            run_pg_dump(exec_cmd, env["POSTGRES_DB"], env["POSTGRES_USER"], args.tables, out_file, args.readable)
            written.append(out_file)
        else:
            for table in args.tables:
                out_file = out_dir / f"{table}_{timestamp}.sql"
                run_pg_dump(exec_cmd, env["POSTGRES_DB"], env["POSTGRES_USER"], [table], out_file, args.readable)
                written.append(out_file)
    except RuntimeError as exc:
        print(f"pg_dump failed:\n{exc}", file=sys.stderr)
        sys.exit(1)

    print("Done. Wrote:")
    for f in written:
        print(f"  {f.relative_to(REPO_ROOT)}  ({f.stat().st_size} bytes)")

    print()
    print("To load into your test DB container (schema must already exist there,")
    print("e.g. via the same EF Core migrations / `docker compose up`):")
    print()
    for f in written:
        print(
            f"  Get-Content {f.relative_to(REPO_ROOT)} | "
            f"docker exec -i <test-container-name> psql -U {env['POSTGRES_USER']} -d {env['POSTGRES_DB']}"
        )
    print()
    if "CODs" not in args.tables and "Clients" in args.tables:
        print(
            "NOTE: Clients.Id_COD is a required FK into the CODs table (not exported here).\n"
            "If the test DB has no matching CODs rows, either seed CODs there first, or re-run with:\n"
            "  python scripts/export_test_data.py --tables Clients TfPlans SPRVlans CODs"
        )


if __name__ == "__main__":
    main()
