
@echo off
for /F "tokens=*" %%i in ('git describe --tags HEAD') do set VERSION=%%i
for /F "tokens=*" %%i in ('git show -s --format"=""%%ct"') do set VERSION2=%%i
>..\..\..\..\..\rglikeworknamelib\AutoVersion.cs echo namespace rglikeworknamelib{public static class AutoVersion{public static string Version = "%VERSION%"; public static string Time = %VERSION2%";}}

