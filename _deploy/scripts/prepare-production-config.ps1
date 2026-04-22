param(
    [Parameter(Mandatory = $true)]
    [string]$BackendPath,
    [Parameter(Mandatory = $true)]
    [string]$SqlConnectionString,
    [Parameter(Mandatory = $true)]
    [string]$JwtKey,
    [Parameter(Mandatory = $true)]
    [string]$Domain
)

$configPath = Join-Path $BackendPath "appsettings.Production.json"
if (-not (Test-Path $configPath)) {
    throw "Khong tim thay file: $configPath"
}

$config = Get-Content $configPath -Raw | ConvertFrom-Json
$config.ConnectionStrings.DefaultConnection = $SqlConnectionString
$config.Jwt.Key = $JwtKey
$config.App.ClientUrl = "https://$Domain"
$config.Cors.AllowedOrigins = @("https://$Domain")

$json = $config | ConvertTo-Json -Depth 10
Set-Content -Path $configPath -Value $json -Encoding UTF8

Write-Host "Da cap nhat appsettings.Production.json tai $configPath" -ForegroundColor Green
