$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
[xml]$solution = Get-Content (Join-Path $root 'OneOffice.slnx')
$expected = @{
    'OneOffice.Api' = @('OneOffice.Core', 'OneOffice.Infrastructure')
    'OneOffice.Infrastructure' = @('OneOffice.Core')
    'OneOffice.Core' = @()
}
$paths = @($solution.Solution.Project | ForEach-Object { $_.Path })
if (@($paths | Where-Object { $_ -notlike 'tests/*' }).Count -ne 3) { throw 'Expected exactly three product projects' }
foreach ($name in $expected.Keys) {
    if ($paths -notcontains "$name/$name.csproj") { throw "Missing $name" }
    [xml]$project = Get-Content (Join-Path $root "$name/$name.csproj")
    if ($project.Project.PropertyGroup.TargetFramework -ne 'net10.0') { throw "Wrong framework $name" }
    $actual = @($project.SelectNodes('//ProjectReference') | ForEach-Object { [IO.Path]::GetFileNameWithoutExtension($_.Include) })
    if ((($actual | Sort-Object) -join ',') -ne (($expected[$name] | Sort-Object) -join ',')) { throw "Wrong references $name" }
}
if (Get-ChildItem (Join-Path $root 'OneOffice.Api') -Recurse -Filter '*WeatherForecast*') { throw 'Demo remains' }
Write-Output 'PASS: solution membership, frameworks, directed references, Core independence, no WeatherForecast'
