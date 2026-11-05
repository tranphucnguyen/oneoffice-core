# OneOffice Core

.NET10 solution with Api, Core and Infrastructure. Read docs/C1-01_DESIGN_SPEC.md, docs/DECISIONS.md and docs/PROJECT_STATE.md. Owner authority is docs/APPROVAL_RECORD.md.

Build: dotnet restore OneOffice.slnx; dotnet build OneOffice.slnx --no-restore. Environment-specific verified commands are in docs/C1-02_VERIFICATION.md. Scaffold checks: powershell -NoProfile -ExecutionPolicy Bypass -File scripts/Verify-Scaffold.ps1.

C1-01/C1-02 complete. Database/auth/frontend implementation awaits subsequent tickets. Planned budget135h within120–150h. Stop for Owner review before C1-03.
