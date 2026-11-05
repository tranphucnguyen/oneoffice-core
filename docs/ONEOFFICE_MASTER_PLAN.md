# OneOffice Master Plan — v2.0

05/11/2026. Authority ONEOFFICE-C1-APPROVAL-001. Root solution, Api/Core/Infrastructure. Core owns module Domain/Application and small SharedKernel folder.

## Current contract
C1-01_DESIGN_SPEC.md is the normative ERD, data dictionary, RBAC, API, auth, audit and test contract. DECISIONS.md records supersession; APPROVAL_RECORD.md preserves Owner directive. TASKS.md is the single current hour allocation (135h); PROJECT_STATE.md records evidence.
Earlier v1.0 schema/UserRoles, same-department leader, bootstrap-unassigned exception, 105h/six-week calendar and /backend location are superseded. Historical original is preserved in baseline Git commit09ec1e4. Source PDF is not present locally; traceability below is inherited, not independently verified in this session.

### A1. Directly grounded in source PDF
- p.1: Clean Modular Monolith, C#/.NET, PostgreSQL JSONB + GIN, React / Next.js + Tailwind + shadcn/ui, sequential/conditional workflow.
- p.2: Core groups: `Departments`, `Positions`, `Users` with `DirectManagerId`; `FormDefinitions`/`FormSubmissions`; workflow definitions/instances/tasks. Original broad schedule: 11 weeks across six phases, **assuming a different unspecified effort level**.
- p.3: Clean Architecture layers, EF Core + Npgsql, FluentValidation, optional MediatR, MiniExcel/QuestPDF; solo-dev requirement, avoid overengineering.
- p.4: Phase 1: parent-child departments; each user in one department, one position, direct manager; department leader; JWT + refresh; Fluent API avoid cascades; demo 3-level org tree; `GET` tree endpoint.
- p.4–6: Later phases dynamic form with schema/JSONB, sequential conditional workflow, UI renderer/inbox, JSONB reports/export. **These are NOT Phase 1 deliverables.**


## F. FRONTEND UX / DESIGN CONTRACT

**Visual intent:** modern enterprise SaaS; white/light neutral background, slate text, blue primary (`#2563EB`), clean cards, 8–12px corner radii, restrained iconography, accessible contrast, consistent keyboard states. Use shadcn/ui tokens rather than scattered color literals. Desktop first, usable mobile and tablet. Support Vietnamese UI text.

**Routes:** `/login`, `/`, `/employees`, `/employees/new`, `/employees/:id`, `/departments`, `/positions`, `/roles`, `/profile`. Protected app shell with sidebar/topbar/breadcrumbs; page-level action authorization. A dashboard with basic counts, no advanced analytics.

**Required screens:** login; shell; employees table (search, pagination, filters); create/edit and detail; department tree and edit form; positions; roles listing and assignment; profile; dashboard. For each P0 page: loading/empty/error/success states, confirmation for destructive/disable action, field-level validation, responsive layout, keyboard and focus support.

**Frontend tech choices:** React+TypeScript+Vite, React Router, TanStack Query for async cache, react-hook-form + Zod for form validation, Tailwind + shadcn/ui. Validate on server even if UI validates. Do not store refresh token in localStorage.


## Execution gates
Design approval → scaffold/build → DB/constraints/CI → auth/security → organization/scopes → polished UI → E2E/security → staging/handover. Capacity15–20h/week, budget120–150h, reference135h, about7–9weeks. No fixed completion date committed.
Work ticket by ticket, record commands/tests, preserve owner edits. Domain free of EF/HTTP. No secrets in source. Integration tests use PostgreSQL, browser tests cover denied access and session expiry. Scope excludes workflow/dynamic forms/SSO/payroll/multitenancy.
After C1-02 stop for Owner/reviewer. C1-03 requires a separate assignment.
