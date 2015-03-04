:ff
@cls
@echo                             ╔======================╗
@echo                             ‖   常用服务项批处理   ‖
@echo                             ╚======================╝
@echo      ╔┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉╗
@echo      ┋在下面输入您要启动或者停止的服务项                                  ┋
@echo      ┋1 重启mssqlserver、重启IIS服务                                      ┋
@echo      ┋2 停止mssqlserver、停止IIS服务                                      ┋
@echo      ┋3 重装.net 4.0 32位                                                 ┋
@echo      ┋4 重装.net 4.0 64位                                                 ┋
@echo      ╚┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉╝
:aa
@echo 请输入:
@set/p sr= >nul
@IF NOT "%sr%"=="" SET sr=%sr:~0,1%
@if /i "%sr%"=="1" goto item1
@if /i "%sr%"=="2" goto item2
@if /i "%sr%"=="3" goto item3
@if /i "%sr%"=="4" goto item4
@echo 选择无效，请重新输入
@echo.
@goto aa

:item1
@net stop mssqlserver
@net start mssqlserver
iisreset
@goto ff

:item2
@net stop mssqlserver
iisreset /STOP
@goto ff

:item3
C:\Windows\Microsoft.NET\Framework\v4.0.30319\aspnet_regiis.exe -i
@goto ff

:item4
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\aspnet_regiis.exe -i
iisreset
@goto ff