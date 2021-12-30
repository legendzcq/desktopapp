# 下载课堂安装包构建说明

安装包由Inno Setup 5构建而成，支持从Windows XP到Windows 10的Windows操作系统上进行安装。

## 快速开始

**确认满足环境要求**后，执行installer.ps1脚本即可开始自动打包进程，打包好的安装文件在Output目录可见：

```powershell
./installer.ps1
```

> 在执行打包脚本之前**必须在设备上插入证书USB Key**，负责会打包失败。
> 打包过程中会自动对相关文件进行签名，会有弹窗提示输入令牌密码，令牌密码请从相关人员处获取。

## 环境要求

脚本调用了MSBuild、SignTool、Inno Setup，所以运行脚本需准备好相关环境：

- [Inno Setup 5.5.9](https://files.jrsoftware.org/is/5/)，默认路径安装。
- [Inno Setup 5简体中文语言包](https://github.com/jrsoftware/issrc/tree/is-5_5_9/Files/Languages/Unofficial)，将中文语言包下载到本地后，拖入Inno Setup安装目录的Languages目录中。
- [Visual Studio 2019社区版](https://visualstudio.microsoft.com/zh-hans/downloads/)，默认路径安装。
- [DigiCert证书安装](https://www.itrus.cn/service_view_1322.html)，**安装及打包时需要插入USB Key**，请从相关人员处获取。

> Visual Studio 2019社区版已经包含MSBuild及SignTool，所以无需额外安装。

## 构建原理

installer-script.iss为供Inno Setup使用的脚本文件，installer.ps1为PowerShell脚本，installer.ps1通过在脚本内部调用MSBuild执行应用构建进程，构建完毕后，调用iscc（Inno Setup的命令行工具）读取installer-script.iss来执行安装包的构建。

## 可能会遇到的问题

- 提示：“SignTool Error: No certificates were found that met all the given criteria.”

> 在PowerShell 5环境下，installer.ps1脚本文件的编码格式需为gb2312，否则识别不出脚本中的中文导致签名失败，推荐使用[PowerShell 7](https://www.microsoft.com/en-us/p/powershell/9mz1snwt0n5d)。

- 提示：“Resource update error: EndUpdateResource failed (110)”

> 安全软件导致的资源无法访问，关闭MicroSoft Defender、360安全卫士等安全软件即可。

- 提示：“Can't sign app: The specified timestamp server either could not be reached or returned an invalid response.”

>  无法得到时间戳，其中一种原因是时间戳服务器的访问问题，可能是当前的时间戳服务器拥挤，换个时间再进行签名一般能够解决该问题。

## 更新

下载课堂更新采用了一个开源库[AutoUpdater](https://github.com/ravibpatel/AutoUpdater.NET)，该库提供了一个灵活的更新策略，比如通过安装程序更新或者通过解压ZIP压缩包更新。

Release模式配置文件链接为：

```http
http://game.chinaacc.com/CourseClientUpdate/Chinaacc/AutoUpdate.xml
```

Debug模式配置文件链接为：（可以搭建本地服务进行调试：lite-serve -c config.xml，需要nodejs、lite serve）

```http
http://localhost:8080/AutoUpdate.xml
```

### 触发更新

可通过调用代码库desktopapp/DesktopApp/DesktopApp/Utils/Updater.cs相关API触发更新，目前会在应用启动时被动触发更新，或在检查更新时主动触发更新。

### 原理

通过比对当前入口程序集版本号（项目实际配置的版本号）同AutoUpdate.xml中的版本号（服务器上xml文件中的版本号）做对比，如果远端的版本更新，则启动更新流程，代码库examples/AutoUpdate项目可作为参考示例。

### 注册表

AutoUpdater默认状态存在系统注册表："\HKEY_CURRENT_USER\SOFTWARE\CDEL\会计下载课堂\AutoUpdater"下（也可自定到用户目录下的json文件）