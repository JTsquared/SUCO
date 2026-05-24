param(
    [Parameter(Mandatory=$true)]
    [string]$Version
)

Write-Host "Building NativeChat..." -ForegroundColor Cyan
Push-Location .\NativeChatClient
npm run build
if ($LASTEXITCODE -ne 0) { Write-Host "npm build failed." -ForegroundColor Red; exit 1 }
Pop-Location

Write-Host "Publishing .NET app..." -ForegroundColor Cyan
dotnet publish -p:PublishProfile=FolderProfile
if ($LASTEXITCODE -ne 0) { Write-Host "dotnet publish failed." -ForegroundColor Red; exit 1 }

Write-Host "Packaging with vpk..." -ForegroundColor Cyan
vpk pack --packId TransparentTwitchChat --packVersion $Version --packDir .\publish --mainExe TransparentTwitchChatWPF.exe
if ($LASTEXITCODE -ne 0) { Write-Host "vpk packaging failed." -ForegroundColor Red; exit 1 }

Write-Host "Done! Published version $Version" -ForegroundColor Green