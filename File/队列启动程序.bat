@echo off
:check

tasklist|find /i "Tag.Tasker.exe" >null
if %errorlevel%==1 echo "检测到不正常，尝试启动正式环境" && start E:\WebSite\标签管理系统\正式\UpLoadFile\标签分配任务执行程序/Tag.Tasker.exe

tasklist|find /i "Tag.Tasker.Test.exe" >null
if %errorlevel%==1 echo "检测到不正常，尝试启动测试环境" && start E:\WebSite\标签管理系统\测试\UpLoadFile\标签分配任务执行程序/Tag.Tasker.Test.exe


timeout /T 3 /nobreak >null

goto check