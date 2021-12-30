[Setup]
AppName=会计下载课堂
AppId={{B137F1CD-63BD-4BC6-B298-D98763D8F26C}
AppVersion=2.0.0.1
VersionInfoVersion=2.0.0.1
AppPublisher=北京东大正保科技有限公司
AppPublisherURL=http://www.chinaacc.com/
AppSupportURL=http://www.chinaacc.com/
AppUpdatesURL=http://www.chinaacc.com/
DefaultDirName={pf}\ChinaaccCourse
DefaultGroupName=会计下载课堂
OutputBaseFilename=ChinaaccCourse
Compression=lzma
SetupIconFile=Embedded\favorite.ico
UninstallDisplayIcon=Embedded\favorite.ico
MinVersion=6.1.7601
RestartIfNeededByRun=yes
AlwaysRestart=true

[Files]
Source: Embedded\Release\*; DestDir: {app}; Flags: ignoreversion recursesubdirs
Source: Embedded\db.db; DestDir: {app}\db; Flags: ignoreversion recursesubdirs onlyifdoesntexist
Source: Embedded\VC_redist.x86.exe; DestDir: {app}; Flags: ignoreversion
Source: Embedded\Windows6.1-KB3063858-x86.msu; DestDir: {app}; Flags: ignoreversion
Source: Embedded\Microsoft.Expression.Drawing.dll; DestDir: {app}; Flags: ignoreversion
Source: Embedded\Microsoft.WindowsAPICodePack.dll; DestDir: {app}; Flags: ignoreversion
Source: Embedded\Microsoft.WindowsAPICodePack.Shell.dll; DestDir: {app}; Flags: ignoreversion
Source: Embedded\MFC71.dll; DestDir: {app}; Flags: ignoreversion
Source: Embedded\msvcp71.dll; DestDir: {app}; Flags: ignoreversion
Source: Embedded\msvcr71.dll; DestDir: {app}; Flags: ignoreversion
Source: Embedded\vcredist_x86.exe; DestDir: {app}; Flags: ignoreversion
Source: Embedded\vc_runtime_x86.cab; DestDir: {app}; Flags: ignoreversion
Source: Embedded\vc_runtime_x86.msi; DestDir: {app}; Flags: ignoreversion
Source: Embedded\CDEL\cm.dll; DestDir: {cf32}\CDEL; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\map.dll; DestDir: {cf32}\CDEL; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\MFC71.dll; DestDir: {cf32}\CDEL; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\msvcp71.dll; DestDir: {cf32}\CDEL; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\msvcr71.dll; DestDir: {cf32}\CDEL; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\Boost_Software_License_1.0.txt; DestDir: {cf32}\CDEL\ffdshow; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\ffdshow.ax; DestDir: {cf32}\CDEL\ffdshow; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\ffmpeg.dll; DestDir: {cf32}\CDEL\ffdshow; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\ff_kernelDeint.dll; DestDir: {cf32}\CDEL\ffdshow; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\ff_liba52.dll; DestDir: {cf32}\CDEL\ffdshow; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\ff_libdts.dll; DestDir: {cf32}\CDEL\ffdshow; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\ff_libfaad2.dll; DestDir: {cf32}\CDEL\ffdshow; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\ff_libmad.dll; DestDir: {cf32}\CDEL\ffdshow; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\ff_samplerate.dll; DestDir: {cf32}\CDEL\ffdshow; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\ff_unrar.dll; DestDir: {cf32}\CDEL\ffdshow; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\ff_wmv9.dll; DestDir: {cf32}\CDEL\ffdshow; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\gnu_license.txt; DestDir: {cf32}\CDEL\ffdshow; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\libmpeg2_ff.dll; DestDir: {cf32}\CDEL\ffdshow; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\openIE.js; DestDir: {cf32}\CDEL\ffdshow; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\TomsMoComp_ff.dll; DestDir: {cf32}\CDEL\ffdshow; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\xvidcore.dll; DestDir: {cf32}\CDEL\ffdshow; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\languages\ffdshow.1026.bg; DestDir: {cf32}\CDEL\ffdshow\languages; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\languages\ffdshow.1028.tc; DestDir: {cf32}\CDEL\ffdshow\languages; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\languages\ffdshow.1029.cs; DestDir: {cf32}\CDEL\ffdshow\languages; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\languages\ffdshow.1031.de; DestDir: {cf32}\CDEL\ffdshow\languages; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\languages\ffdshow.1033.en; DestDir: {cf32}\CDEL\ffdshow\languages; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\languages\ffdshow.1034.es; DestDir: {cf32}\CDEL\ffdshow\languages; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\languages\ffdshow.1035.fi; DestDir: {cf32}\CDEL\ffdshow\languages; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\languages\ffdshow.1036.fr; DestDir: {cf32}\CDEL\ffdshow\languages; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\languages\ffdshow.1038.hu; DestDir: {cf32}\CDEL\ffdshow\languages; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\languages\ffdshow.1040.it; DestDir: {cf32}\CDEL\ffdshow\languages; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\languages\ffdshow.1041.ja; DestDir: {cf32}\CDEL\ffdshow\languages; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\languages\ffdshow.1042.ko; DestDir: {cf32}\CDEL\ffdshow\languages; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\languages\ffdshow.1045.pl; DestDir: {cf32}\CDEL\ffdshow\languages; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\languages\ffdshow.1046.br; DestDir: {cf32}\CDEL\ffdshow\languages; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\languages\ffdshow.1049.ru; DestDir: {cf32}\CDEL\ffdshow\languages; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\languages\ffdshow.1051.sk; DestDir: {cf32}\CDEL\ffdshow\languages; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\languages\ffdshow.1053.sv; DestDir: {cf32}\CDEL\ffdshow\languages; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\languages\ffdshow.1069.eu; DestDir: {cf32}\CDEL\ffdshow\languages; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\CDEL\ffdshow\languages\ffdshow.2052.sc; DestDir: {cf32}\CDEL\ffdshow\languages; Flags: uninsneveruninstall onlyifdoesntexist
Source: Embedded\Fonts\arial.ttf; DestDir: {fonts}; Check: IsXP; Flags: uninsneveruninstall onlyifdoesntexist

[Dirs]
Name: {app}\update\
Name: {app}\web\

[Registry]
Root: HKCU; Subkey: Software\GNU\ffdshow; ValueName: trayIcon; ValueType: Dword; ValueData: $0; Check: IsXP
Root: HKCU; Subkey: Software\GNU\ffdshow; ValueName: trayIconExt; ValueType: Dword; ValueData: $0; Check: IsXP
Root: HKCU; Subkey: Software\GNU\ffdshow; ValueName: h264; ValueType: Dword; ValueData: $1; Check: IsXP
Root: HKCU; Subkey: Software\GNU\ffdshow; ValueName: isWhitelist; ValueType: Dword; ValueData: $0; Check: IsXP
Root: HKCU; Subkey: Software\GNU\ffdshow_audio; ValueName: trayIcon; ValueType: Dword; ValueData: $0; Check: IsXP
Root: HKCU; Subkey: Software\GNU\ffdshow_audio; ValueName: trayIconExt; ValueType: Dword; ValueData: $0; Check: IsXP
Root: HKCU; Subkey: Software\GNU\ffdshow_audio; ValueName: aac; ValueType: Dword; ValueData: $1; Check: IsXP
Root: HKCU; Subkey: Software\GNU\ffdshow_audio; ValueName: isWhitelist; ValueType: Dword; ValueData: $0; Check: IsXP

[Run]
Filename: {sys}\msiexec.exe; Parameters: "/quiet /i ""{app}\vc_runtime_x86.msi"""; StatusMsg: 安装程序正在完成，可能需要3-5分钟的时间,请耐心等待...; Flags: skipifdoesntexist
Filename: {app}\vcredist_x86.exe; Parameters: "/q /norestart"""; WorkingDir: {tmp}; StatusMsg: 安装程序正在完成，可能需要3-5分钟的时间,请耐心等待...; Flags: skipifdoesntexist
Filename: {app}\VC_redist.x86.exe; Parameters: "/install /quiet /norestart"""; WorkingDir: {tmp}; StatusMsg: 安装程序正在完成，可能需要3-5分钟的时间,请耐心等待...; Flags: skipifdoesntexist
Filename: {sys}\regsvr32.exe; Parameters: "/s ""{cf32}\cdel\cm.dll"""
Filename: {sys}\regsvr32.exe; Parameters: "/s ""{cf32}\cdel\ffdshow\ffdshow.ax"""; Check: IsXP
Filename: {sys}\regsvr32.exe; Parameters: "/s ""{cf32}\cdel\map.dll"""
Filename: {sys}\wusa.exe; Parameters: """{app}\Windows6.1-KB3063858-x86.msu"" /quiet /norestart"; OnlyBelowVersion: 6.2.9200; StatusMsg: 安装程序正在完成，可能需要3-5分钟的时间,请耐心等待...; Flags: skipifdoesntexist

[Icons]
Name: {group}\会计下载课堂; Filename: {app}\CdelCourse.exe
Name: {group}\{cm:ProgramOnTheWeb,中华会计网校}; Filename: http://www.chinaacc.com/
Name: {group}\{cm:UninstallProgram,会计下载课堂}; Filename: {uninstallexe}
Name: {userdesktop}\会计下载课堂; Filename: {app}\CdelCourse.exe; WorkingDir: {app}

[CustomMessages]
chinesesimp.NameAndVersion=%1 版本 %2
chinesesimp.AdditionalIcons=附加快捷方式:
chinesesimp.CreateDesktopIcon=创建桌面快捷方式(&D)
chinesesimp.CreateQuickLaunchIcon=创建快速运行栏快捷方式(&Q)
chinesesimp.ProgramOnTheWeb=%1 网站
chinesesimp.UninstallProgram=卸载 %1
chinesesimp.LaunchProgram=运行 %1
chinesesimp.AssocFileExtension=将 %2 文件扩展名与 %1 建立关联(&A)
chinesesimp.AssocingFileExtension=正在将 %2 文件扩展名与 %1 建立关联...
chinesesimp.AutoStartProgramGroupDescription=启动组:
chinesesimp.AutoStartProgram=自动启动 %1
chinesesimp.AddonHostProgramNotFound=%1无法找到您所选择的文件夹。%n你想继续吗？

[Languages]
; These files are stubs
; To achieve better results after recompilation, use the real language files
Name: chinesesimp; MessagesFile: compiler:Languages\ChineseSimplified.isl

[Code]
function IsXP: Boolean;
   {function to check if we are running on Windows XP}
   var
      version: TWindowsVersion;
   begin
      GetWindowsVersionEx(version);
      result := false;
      if Version.NTPlatform and
         (Version.Major = 5) and
         (Version.Minor = 1) then
         result := true;
   end;

function IsDotNetDetected(version: string; service: cardinal): boolean;
// Indicates whether the specified version and service pack of the .NET Framework is installed.
//
// version -- Specify one of these strings for the required .NET Framework version:
//    'v1.1.4322'     .NET Framework 1.1
//    'v2.0.50727'    .NET Framework 2.0
//    'v3.0'          .NET Framework 3.0
//    'v3.5'          .NET Framework 3.5
//    'v4\Client'     .NET Framework 4.0 Client Profile
//    'v4\Full'       .NET Framework 4.0 Full Installation
//
// service -- Specify any non-negative integer for the required service pack level:
//    0               No service packs required
//    1, 2, etc.      Service pack 1, 2, etc. required
var
    key: string;
    install, serviceCount: cardinal;
    success: boolean;
begin
    key := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\' + version;
    // .NET 3.0 uses value InstallSuccess in subkey Setup
    if Pos('v3.0', version) = 1 then begin
        success := RegQueryDWordValue(HKLM, key + '\Setup', 'InstallSuccess', install);
    end else begin
        success := RegQueryDWordValue(HKLM, key, 'Install', install);
    end;
    // .NET 4.0 uses value Servicing instead of SP
    if Pos('v4', version) = 1 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Servicing', serviceCount);
    end else begin
        success := success and RegQueryDWordValue(HKLM, key, 'SP', serviceCount);
    end;
    result := success and (install = 1) and (serviceCount >= service);
end;

function IsDotNet40Detected(): boolean;
begin
  Result := IsDotNetDetected('v4\Full', 0);
end;
