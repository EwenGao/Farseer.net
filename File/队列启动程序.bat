@echo off
:check

tasklist|find /i "Tag.Tasker.exe" >null
if %errorlevel%==1 echo "��⵽������������������ʽ����" && start E:\WebSite\��ǩ����ϵͳ\��ʽ\UpLoadFile\��ǩ��������ִ�г���/Tag.Tasker.exe

tasklist|find /i "Tag.Tasker.Test.exe" >null
if %errorlevel%==1 echo "��⵽�������������������Ի���" && start E:\WebSite\��ǩ����ϵͳ\����\UpLoadFile\��ǩ��������ִ�г���/Tag.Tasker.Test.exe


timeout /T 3 /nobreak >null

goto check