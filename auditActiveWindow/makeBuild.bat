
for /f "tokens=*" %%i in ('git rev-list --count HEAD') do set buildNumber=%%i

set zipName="auditActiveWindow-%buildNumber%.zip"

dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true --self-contained true

powershell -Command "Compress-Archive -Path 'bin\Release\net9.0-windows\win-x64\publish\auditActiveWindow.exe' -DestinationPath %zipName%"

move %zipName% "../"
