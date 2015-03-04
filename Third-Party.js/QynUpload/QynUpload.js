var QynUpload = {};

// 打开运行窗口
QynUpload.FileType = "";
QynUpload.Path = "";

/// <summary>
/// 显示页面，使用js.dialog方式
/// </summary>
/// <param name="width">弹出框的长度</param>
/// <param name="height">弹出框的高度</param>
/// <param name="swfOption">swf配置</param>
/// <param name="callBack">窗口关闭时回调函数(this.content.document.getElementById("lstItem"), this.content)</param>
/// <param name="fileType">文件类型样式</param>
/// <param name="dig">对话框变量</param>
QynUpload.Dialog = function (width, height, swfOption, callBack, fileType, dig) {
    if (!dig) { dig = this; }
    if (fileType) { QynUpload.FileType = fileType; }

    dig({
        width: width,
        height: height,
        content: 'url:' + QynUpload.Path + 'dialog.html?FileType=' + QynUpload.FileType,
        close: function () {
            if (callBack) { callBack(this.content.document.getElementById("lstItem"), this.content); }
        },
        init: function () {
            this.content.setting(swfOption);
        }
    });
}

/// <summary>
/// 显示页面，使用window.open方式
/// </summary>
/// <param name="width">弹出框的长度</param>
/// <param name="height">弹出框的高度</param>
/// <param name="swfOption">swf配置</param>
/// <param name="callBack">窗口关闭时回调函数</param>
/// <param name="fileType">文件类型样式</param>
QynUpload.Show = function (width, height, swfOption, callBack, fileType) {
    if (fileType) { QynUpload.FileType = fileType; }

    var newwin = window.open(QynUpload.Path + 'dialog.html?FileType=' + QynUpload.FileType, '', '');
    newwin.opener = null;
    newwin.onload = function () {
        this.setting(swfOption);
    }
    newwin.onunload = function () {
        if (callBack) { callBack(this.document.getElementById("lstItem"), this); }
    }
}

/*!
 * _path 获取组件核心文件lhgdialog.js所在的绝对路径
 * _args 获取lhgdialog.js文件后的url参数组，如：lhgdialog.js?self=true&skin=aero中的?后面的内容
 */
_args = function (script, i, me) {
    var l = script.length;

    for (; i < l; i++) {
        me = !!document.querySelector ? script[i].src : script[i].getAttribute('src', 4);

        if (me.substr(me.lastIndexOf('/')).indexOf('QynUpload.js') !== -1) { break; }
    }

    me = me.split('?');
    _args = me[1];
    return _args;
    //return me[0].substr(0, me[0].lastIndexOf('/') + 1);
}(document.getElementsByTagName('script'), 0);

/// <summary>
/// 获取url参数值函数
/// </summary>
/// <param name="name">参数名称</param>
GetParam = function (name) {
    name = name.toLowerCase()
    if (_args) {
        var p = _args.toLowerCase().split('&'), i = 0, l = p.length, a;
        for (; i < l; i++) {
            a = p[i].split('=');
            if (name === a[0]) { return a[1]; }
        }
    }
    return null;
}

/// <summary>
/// 读取脚本代码
/// </summary>
/// <param name="scriptPath">脚本地址</param>
function LoadScript(scriptPath) {
    document.write('<script type="text/javascript" src="' + scriptPath + '"></script>');
}

var Init = function () {
    QynUpload.FileType = GetParam("FileType") || "";

    // 得到当前脚本的地址
    var script = document.getElementsByTagName('script');
    for (var i = 0; i < script.length; i++) {
        me = !!document.querySelector ? script[i].src : script[i].getAttribute('src', 4);
        if (me.substr(me.lastIndexOf('/')).indexOf('QynUpload.js') !== -1) { QynUpload.Path = me.substr(0, me.lastIndexOf('/') + 1); break; }
    }
}();