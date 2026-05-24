$date = Get-Date -Format "yyyy-MM-dd"
$logPath = Join-Path $PSScriptRoot "bin\Debug\log\bbmath_$date.log"

if (-not (Test-Path $logPath)) {
    Write-Warning "Log file not found: $logPath"
    exit 1
}

Write-Host "Tailing: $logPath" -ForegroundColor Green
Write-Host "Press Ctrl+C to stop" -ForegroundColor DarkGray

Get-Content $logPath -Wait -Tail 100
