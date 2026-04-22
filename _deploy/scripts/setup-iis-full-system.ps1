param(
    [string]$SiteName = "KPITracker",
    [string]$Domain = "",
    [string]$FrontendPath = "C:\inetpub\kpi-ui",
    [string]$BackendPath = "C:\inetpub\kpi-api",
    [int]$Port = 80
)

Import-Module WebAdministration

$frontendPool = "${SiteName}-frontend"
$backendPool = "${SiteName}-backend"

if (-not (Test-Path $FrontendPath)) {
    throw "FrontendPath khong ton tai: $FrontendPath"
}

if (-not (Test-Path $BackendPath)) {
    throw "BackendPath khong ton tai: $BackendPath"
}

if (-not (Test-Path "IIS:\AppPools\$frontendPool")) {
    New-WebAppPool -Name $frontendPool | Out-Null
}

if (-not (Test-Path "IIS:\AppPools\$backendPool")) {
    New-WebAppPool -Name $backendPool | Out-Null
}

Set-ItemProperty "IIS:\AppPools\$frontendPool" -Name managedRuntimeVersion -Value ""
Set-ItemProperty "IIS:\AppPools\$backendPool" -Name managedRuntimeVersion -Value ""

if (-not (Test-Path "IIS:\Sites\$SiteName")) {
    New-Website -Name $SiteName -PhysicalPath $FrontendPath -Port $Port -ApplicationPool $frontendPool | Out-Null
} else {
    Set-ItemProperty "IIS:\Sites\$SiteName" -Name physicalPath -Value $FrontendPath
    Set-ItemProperty "IIS:\Sites\$SiteName" -Name applicationPool -Value $frontendPool
}

if ($Domain) {
    $existingBinding = Get-WebBinding -Name $SiteName -Protocol "http" -ErrorAction SilentlyContinue |
        Where-Object { $_.bindingInformation -eq "*:$Port:$Domain" }

    if (-not $existingBinding) {
        New-WebBinding -Name $SiteName -Protocol "http" -Port $Port -HostHeader $Domain | Out-Null
    }
}

$apiAppPath = "IIS:\Sites\$SiteName\api"
if (-not (Test-Path $apiAppPath)) {
    New-WebApplication -Site $SiteName -Name "api" -PhysicalPath $BackendPath -ApplicationPool $backendPool | Out-Null
} else {
    Set-ItemProperty $apiAppPath -Name physicalPath -Value $BackendPath
    Set-ItemProperty $apiAppPath -Name applicationPool -Value $backendPool
}

Set-WebConfigurationProperty -PSPath "MACHINE/WEBROOT/APPHOST/$SiteName/api" `
    -Filter "system.webServer/aspNetCore" `
    -Name "environmentVariables.[name='ASPNETCORE_ENVIRONMENT',value='Production'].value" `
    -Value "Production" `
    -ErrorAction SilentlyContinue | Out-Null

Write-Host "Hoan tat tao site IIS." -ForegroundColor Green
Write-Host "Frontend: http://$($Domain ? $Domain : 'localhost'):$Port/" -ForegroundColor Cyan
Write-Host "Backend : http://$($Domain ? $Domain : 'localhost'):$Port/api" -ForegroundColor Cyan
