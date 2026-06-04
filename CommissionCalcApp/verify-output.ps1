# Verify that the CommissionCalcApp outputs the expected commission value.
# Exit 0 if the expected value is found, exit 1 otherwise.
param(
    [string]$ExpectedValue = "4,000.00"
)

$output = & dotnet run --project CommissionCalcApp.csproj 2>&1 | Out-String

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: dotnet run failed with exit code $LASTEXITCODE"
    Write-Host $output
    exit 1
}

if ($output -match [regex]::Escape($ExpectedValue)) {
    Write-Host "PASS: Found expected value '$ExpectedValue' in output"
    Write-Host $output.Trim()
    exit 0
} else {
    Write-Host "FAIL: Expected '$ExpectedValue' in output but got:"
    Write-Host $output.Trim()
    exit 1
}
