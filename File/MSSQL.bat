:ff
@cls
@echo                             �X======================�[
@echo                             ��   ���÷�����������   ��
@echo                             �^======================�a
@echo      �X���������������������������������������������������������������������[
@echo      ��������������Ҫ��������ֹͣ�ķ�����                                  ��
@echo      ��1 ����mssqlserver������IIS����                                      ��
@echo      ��2 ֹͣmssqlserver��ֹͣIIS����                                      ��
@echo      ��3 ��װ.net 4.0 32λ                                                 ��
@echo      ��4 ��װ.net 4.0 64λ                                                 ��
@echo      �^���������������������������������������������������������������������a
:aa
@echo ������:
@set/p sr= >nul
@IF NOT "%sr%"=="" SET sr=%sr:~0,1%
@if /i "%sr%"=="1" goto item1
@if /i "%sr%"=="2" goto item2
@if /i "%sr%"=="3" goto item3
@if /i "%sr%"=="4" goto item4
@echo ѡ����Ч������������
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