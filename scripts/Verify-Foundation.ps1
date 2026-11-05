$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$outDir = Join-Path $root '.local-dotnet/smoke'
New-Item -ItemType Directory -Force $outDir | Out-Null
function Request($port, $path, $expected) {
    try { $r=Invoke-WebRequest "http://127.0.0.1:$port$path" -UseBasicParsing; $status=[int]$r.StatusCode; $body=$r.Content }
    catch [System.Net.WebException] {
        $response=$_.Exception.Response
        if (-not $response) { throw }
        $status=[int]$response.StatusCode
        $reader=New-Object IO.StreamReader($response.GetResponseStream()); $body=$reader.ReadToEnd(); $reader.Dispose()
    }
    if ($status -ne $expected) { throw "Unexpected HTTP status for $path : $status" }
    if ($body -match 'SMOKE_SECRET|SMOKE_HOST|Password=|ConnectionString|StackTrace') { throw 'Sensitive response detected' }
    if ($expected -ge 400 -and $path -notlike '/health*') {
        $problem=$body | ConvertFrom-Json
        if (-not $problem.code -or -not $problem.traceId -or $problem.status -ne $expected) { throw 'Invalid ProblemDetails' }
    }
    Write-Output "PASS $path HTTP$status"
}
foreach ($scenario in @('Development','Production')) {
    $port=if($scenario -eq 'Development'){5191}else{5192}
    $env:ASPNETCORE_ENVIRONMENT=$scenario
    $env:DOTNET_ENVIRONMENT=$scenario
    $env:ASPNETCORE_URLS="http://127.0.0.1:$port"
    if($scenario -eq 'Production'){$env:ConnectionStrings__OneOfficeDb='Host=127.0.0.1;Port=1;Database=oneoffice_dev;Username=oneoffice_app;Password=SMOKE_SECRET;Timeout=1;Command Timeout=1'}
    $stdout=Join-Path $outDir "$scenario.out.log"; $stderr=Join-Path $outDir "$scenario.err.log"
    $process=Start-Process 'C:/Program Files/dotnet/dotnet.exe' -ArgumentList @('OneOffice.Api/bin/Debug/net10.0/OneOffice.Api.dll','--foundation-smoke') -WorkingDirectory $root -WindowStyle Hidden -RedirectStandardOutput $stdout -RedirectStandardError $stderr -PassThru
    try {
        $ready=$false
        for($i=0;$i -lt 30;$i++){try{Invoke-WebRequest "http://127.0.0.1:$port/health" -UseBasicParsing | Out-Null;$ready=$true;break}catch{Start-Sleep -Milliseconds 300}}
        if(-not $ready){throw 'API failed to start'}
        Request $port '/health' 200
        if($scenario -eq 'Development'){
            Request $port '/health/ready' 200
            Request $port '/openapi/v1.json' 200
            Request $port '/_smoke/error?password=SMOKE_SECRET' 500
            Request $port '/_smoke/validation?number=SMOKE_SECRET' 400
        }else{
            Request $port '/health/ready' 503
            Request $port '/openapi/v1.json' 404
            Request $port '/_smoke/error' 404
        }
        Request $port '/missing?token=SMOKE_SECRET' 404
    } finally {
        if(-not $process.HasExited){Stop-Process -Id $process.Id; $process.WaitForExit()}
        Remove-Item Env:ConnectionStrings__OneOfficeDb -ErrorAction SilentlyContinue
    }
    $logs=(Get-Content -Raw $stdout)+(Get-Content -Raw $stderr)
    if($logs -match 'SMOKE_SECRET|SMOKE_HOST|Password=|ConnectionString|Host='){throw 'Sensitive logs detected'}
    if($logs -notmatch 'TraceId'){throw 'Missing structured trace log'}
    Write-Output "PASS $scenario sanitized structured logs"
}
