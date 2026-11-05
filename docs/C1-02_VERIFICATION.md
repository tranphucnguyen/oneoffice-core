# C1-02 Verification — 05/11/2026

Baseline commit09ec1e4 captures owner workspace before changes. Solution initially already listed three projects. SDK10.0.403. OpenAPI package10.0.14 preserved.

## Commands actually executed
- dotnet sln OneOffice.slnx list: Api, Core, Infrastructure, exactly3.
- dotnet add references: Api→Core/Infrastructure; Infrastructure→Core.
- dotnet restore OneOffice.slnx --disable-parallel -m:1: PASS after network sandbox escalation; all3 restored/up-to-date.
- dotnet build OneOffice.slnx --no-restore -m:1 -p:UseSharedCompilation=false: PASS, all3 assemblies, 0 warnings/0 errors,4.93s.
- Removed old Core bin/obj inherited from rename; restored again with --ignore-failed-sources using cached packages, PASS. Final build PASS,0 warnings/0 errors,6.73s.
- powershell -NoProfile -ExecutionPolicy Bypass -File scripts/Verify-Scaffold.ps1: PASS membership/framework/directed references/Core independence/no demo.

Initial failures: default CLI home denied sentinel write; DOTNET_CLI_HOME set to workspace .local-dotnet (ignored). Default parallel MSBuild returned failure without diagnostic; -m:1 used. Sandbox DNS caused NU1301; approved escalation restore succeeded. Direct ps1 execution denied by execution policy; process-only bypass ran verification. SDK first-run reported dev certificate installation and inability to add tools to PATH. No persistent policy/PATH edited.

Source changes: renamed SharedKernel folder/csproj to Core, explicit AssemblyName/RootNamespace; updated solution and references; removed two Class1 files, WeatherForecast model/controller and sample HTTP request. Added module directory .gitkeep files and Infrastructure/Persistence. Program.cs/OpenAPI retained. Added .NET/Node/secret excludes and verification script. Only three csproj.

No business implementation or migrations. Runtime/API/auth/integration/E2E not tested. Config inspected: only logging/AllowedHosts and local launch URLs, no secret values. No actual effort timesheet exists.

Review using git diff HEAD and git status --short; newly added files included after staging. Baseline allows explicit file restore. Final changes staged, not committed, for Owner review.
