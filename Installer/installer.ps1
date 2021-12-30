Import-Module "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\Common7\Tools\Microsoft.VisualStudio.DevShell.dll"
Enter-VsDevShell -VsInstallPath "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional" -StartInPath (Get-Location)
$env:Path+=";C:\Program Files (x86)\Inno Setup 5"

dotnet publish -c Release -r win-x86 --self-contained true -p:PublishReadyToRun=true -o ./Embedded/Release ..\DesktopApp\upforthis\upforthis.csproj
msbuild -r -t:publish -p:Configuration=Release -p:SelfContained=true -p:RuntimeIdentifier=win-x86 -p:PublishReadyToRun=True -p:OutDir=../../Installer/Embedded/Release ..\DesktopApp\DesktopApp\DesktopApp.csproj

Remove-Item ./Embedded/**/*.pdb -Recurse
Remove-Item ./Embedded/**/*.json -Recurse

Remove-Item ./Embedded/Release/x64/ -Recurse 

Remove-Item ./Embedded/Release/libvlc/win-x64/ -Recurse
Remove-Item ./Embedded/Release/libvlc/win-x86/locale/ -Recurse
Remove-Item ./Embedded/Release/libvlc/win-x86/plugins/audio_mixer/ -Recurse
Remove-Item ./Embedded/Release/libvlc/win-x86/plugins/gui/ -Recurse
Remove-Item ./Embedded/Release/libvlc/win-x86/plugins/control/ -Recurse
Remove-Item ./Embedded/Release/libvlc/win-x86/plugins/packetizer/ -Recurse
Remove-Item ./Embedded/Release/libvlc/win-x86/plugins/services_discovery/ -Recurse
Remove-Item ./Embedded/Release/libvlc/win-x86/plugins/text_renderer/ -Recurse
Remove-Item ./Embedded/Release/libvlc/win-x86/plugins/visualization/ -Recurse

#move "db.db"
Move-Item ./Embedded/Release/db/db.db ./Embedded/db.db

signtool sign /debug /n "北京东大正保科技有限公司" /d "会计下载课堂" /t http://timestamp.digicert.com ./Embedded/Release/CdelCourse.exe ./Embedded/Release/upforthis.exe ./Embedded/Release/Framework.dll ./Embedded/Release/DownloadClass.Toolkit.dll

iscc ./installer-script.iss

signtool sign /debug /n "北京东大正保科技有限公司" /d "会计下载课堂" /t http://timestamp.digicert.com ./Output/ChinaaccCourse.exe

#move back "db.db"
Move-Item ./Embedded/db.db ./Embedded/Release/db/db.db
