# Architecture Decision Register — OneOffice Core

Date: 2026-11-04. Every change must be accompanied by rationale, tradeoffs, impact on tests, and owner approval.

| ADR | Decision | State | Why |
|---|---|---|---|
| ADR-001 | Internal single-company product; no Tenant table | Baseline | user-selected deployment model |
| ADR-002 | Clean Modular Monolith, modules Identity / Organization / AccessControl | Baseline | solo developer, future modularity |
| ADR-003 | .NET 10 LTS + EF Core 10 + Npgsql compatible; PostgreSQL 16 | Baseline | .NET 8/9 EOS 2026-11-10 |
| ADR-004 | React + TypeScript + Vite + shadcn/ui + Tailwind | Baseline | polished SPA, lower overhead than SSR |
| ADR-005 | UUID PKs, UTC timestamps, NO ACTION/RESTRICT domain FKs | Baseline | robust hierarchy and traceability |
| ADR-006 | Department adjacency list + recursive CTE | Baseline | avoids premature materialized-path complexity |
| ADR-007 | JWT short-lived access + rotated hashed refresh token (HttpOnly cookie) | Baseline | secure web sessions |
| ADR-008 | RBAC permission + record scopes All/Department/DepartmentTree/Self | Baseline | future approvals require correct org authorization |
| ADR-009 | Business rule: leader belongs to exact department | Proposed — needs owner approval | safe deterministic resolver |
| ADR-010 | Manager only views scoped employees; HR admin edits | Proposed — needs owner approval | avoid unintended permissions |
| ADR-011 | Block inactivation with active dependencies | Proposed — needs owner approval | avoid orphaned workflow identities |
| ADR-012 | Move user requires explicit manager reassignment | Proposed — needs owner approval | avoid wrong approval path |
| ADR-013 | Phase 1 six weeks / 105h, 15–20h/week | Planning baseline | reduced dev hours + polished UI |

Policy: mark Proposed ADRs Approved/Rejected only after direct owner decision. No module should assume approval implicitly.
