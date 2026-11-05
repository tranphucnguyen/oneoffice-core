# Decisions — 05/11/2026

Authority: APPROVAL_RECORD.md, ONEOFFICE-C1-APPROVAL-001. Implementation contract: C1-01_DESIGN_SPEC.md.

| Decision | Status | Impact |
|---|---|---|
| ADR-001–008 | Baseline retained | Single company, modular monolith, .NET10/EF10/PostgreSQL18, React SPA, UUID/UTC, adjacency list, JWT/RBAC |
| ADR-009 | Superseded by ADR-024 | Leader can lead multiple departments outside membership |
| ADR-010–012 | Superseded by B01–B03 | Read-only managers, explicit scope, dependencies, keep valid cross-department manager |
| ADR-013 | Superseded by PLAN-001 | Old 105h calendar retired |
| ADR-014 | Confirmed | Root solution, three projects |
| ADR-015 | Approved | Rename SharedKernel project to Core; SharedKernel becomes common folder |
| ADR-016 | Approved | Assignment-specific permission/scope pairs |
| ADR-018 | Approved | JWT15min, absolute family7days, rotation/revocation |
| ADR-023 | Approved | Cross-department active manager, no cycle or automatic RBAC |
| ADR-024 | Approved | Multiple department leadership, nonunique LeaderId |
| PLAN-001 | Approved | 120–150h budget; 135h reference |
| B01 | Approved | Roots, required department/position, explicit manager exemption |
| B02 | Approved | Least privilege, protected privileged accounts, scoped projections |
| B03 | Approved | Disable, Version, transaction locks |
| B04 | Approved | Strict replay, CSRF/Origin, bootstrap and recovery |
| B05 | Approved | Durable AuditLogs, atomic mutation audit |

Technical resolutions delegated by directive: ManagerExempt + reason and audit, least-privilege catalog, ordered locks, maintenance recovery, AuditLogs dictionary are documented in spec. They are implementation proposals under approved principles, not additional Owner approvals.
Pending: audit retention proposed365days, deployment domains/secret provider and backup schedule, brand. No automatic audit purge. Deferred: multi-leader-per-department, editable role catalog, hard delete, file uploads, workflow.

## ADR-025 — Approved by C1-03 Owner directive
PostgreSQL18 native Windows replaces PostgreSQL16 baseline. Existing instance only, observed port5433. Development uses no PostgreSQL Docker. Server/service/firewall/global settings require separate Owner approval to change. PostgreSQL17 also present and remains outside project scope.
Runtime oneoffice_app is not superuser/CREATEDB/CREATEROLE. Proposed provisioning keeps database/schema ownership with maintenance owner, grants runtime only CONNECT/USAGE initially and specific DML with C1-06. No provisioning executed pending authenticated inventory. Migrations execution deferred to C1-06.
