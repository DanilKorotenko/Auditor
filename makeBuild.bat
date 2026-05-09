
for /f "tokens=*" %%i in ('git rev-list --count HEAD') do set buildNumber=%%i

git tag %buildNumber%

git push origin tag %buildNumber%

(
echo namespace common;
echo public class AuditorVersion
echo {
echo	public const int majorVersion = 1;
echo	public const int minorVersion = 0;
echo	public const int version = %buildNumber%;
echo }
) > common\AuditorVersion.cs


cd auditActiveWindow

CALL .\makeBuild.bat

cd ..\..
