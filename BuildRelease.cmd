#
for /F "tokens=*" %%i in ('git describe --tags HEAD') do set VERSION=%%i
rd rglikeworkname\rglikeworkname\bin\x86\Release /s /q
del %VERSION%.tar
del %VERSION%.tar.gz
C:\windows\microsoft.net\framework\v4.0.30319\MSBuild.exe rglikeworkname.sln /p:Configuration=Release
xcopy rglikeworkname\rglikeworknamecontent\Data rglikeworkname\rglikeworkname\bin\x86\Release\Content\Data\ /H /E /G /Q /R /Y
xcopy rglikeworkname\rglikeworknamecontent\Textures rglikeworkname\rglikeworkname\bin\x86\Release\Content\Textures\ /H /E /G /Q /R /Y
"C:\program files\7-zip\7z.exe" a -ttar %VERSION%.tar "%CD%\rglikeworkname\rglikeworkname\bin\x86\Release"
"C:\program files\7-zip\7z.exe" a -tgzip %VERSION%.tar.gz %VERSION%.tar
"C:\program files (x86)\7-zip\7z.exe" a -ttar %VERSION%.tar "%CD%\rglikeworkname\rglikeworkname\bin\x86\Release"
"C:\program files (x86)\7-zip\7z.exe" a -tgzip %VERSION%.tar.gz %VERSION%.tar