#
rd rglikeworkname\rglikeworkname\bin\x86\Release /s /q
del release.tar
del release.tar.gz
C:\windows\microsoft.net/framework/v4.0.30319/MSBuild.exe rglikeworkname.sln /p:Configuration=Release
"C:\program files\7-zip\7z.exe" a -ttar release.tar D:\roguelikeworkname\rglikeworkname\rglikeworkname\bin\x86\Release\
"C:\program files\7-zip\7z.exe" a -tgzip release.tar.gz release.tar