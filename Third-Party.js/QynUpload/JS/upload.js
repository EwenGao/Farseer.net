//  计算上传文件开始时间
var iTime = new Date();
//  swfupload组件
var swfu;

/// <summary>
/// 初始化操作
/// </summary>
/// <param name="rootPath">当前组件目录</param>
/// <param name="skinName">主题路径</param>
/// <param name="swfuOption">swf配置参数</param>
function init(swfuOption) {
    if (!swfuOption) { swfuOption = {}; }
    swfuOption["flash_url"] = "swf/swfupload.swf";
    swfuOption["file_dialog_complete_handler"] = fileDialogComplete;
    swfuOption["file_queued_handler"] = fileQueued;
    swfuOption["file_queue_error_handler"] = fileQueueError;
    swfuOption["upload_start_handler"] = uploadStart;
    swfuOption["upload_progress_handler"] = uploadProgress;
    swfuOption["upload_error_handler"] = uploadError;
    swfuOption["upload_success_handler"] = uploadSuccess;
    swfuOption["upload_complete_handler"] = uploadComplete;

    // 从主题中，初始化操作
    skinInit(swfuOption);

    swfu = new SWFUpload(swfuOption);
}

/// <summary>
/// 文件加入队列成功时执行
/// </summary>
/// <param name="file">当前选择的文件</param>
function fileQueued(file) {
    insertHtml(file);
}

/// <summary>
/// 文件加入队列失败时执行
/// </summary>
/// <param name="file">当前选择的文件</param>
/// <param name="error">错误代码</param>
/// <param name="message">错误提示</param>
function fileQueueError(file, error, message) {
    if (file != null) { insertHtml(file); }
    uploadError(file, error, message);
}

/// <summary>
/// 对话框选择完成时执行
/// </summary>
/// <param name="numFilesSelected">选择的文件数量</param>
/// <param name="numFilesQueued">当前队列数量</param>
function fileDialogComplete(numFilesSelected, numFilesQueued) {
    fileDialogCompleted(numFilesSelected, numFilesQueued);
    if (numFilesQueued > 0) { this.startUpload(); }
}

/// <summary>
/// 开始上传
/// </summary>
/// <param name="file">当前上传的文件</param>
function uploadStart(file) {
    // 当前时间
    iTime = new Date();
    uploadStarted(file);
    return true;
}

/// <summary>
/// 上传的过程定时执行
/// </summary>
/// <param name="file">当前上传的文件</param>
/// <param name="bytesLoaded">已传文件节点数据</param>
function uploadProgress(file, bytesLoaded) {
    // 进度
    var percent = Math.ceil((bytesLoaded / file.size) * 100);
    if (percent > 100) { percent = 100; }

    var currentTime = new Date();
    // 使用时间
    var useTime = (Math.ceil(currentTime - iTime) / 1000);
    //当前时间速度
    var uSpeed = bytesLoaded > 0 ? Math.floor(roundNumber((bytesLoaded / useTime), 2)) : 0;
    // 剩余时间
    var reTime = file.size - bytesLoaded > 0 ? roundNumber((file.size - bytesLoaded) / uSpeed, 2) : 0;

    uploadProgressed(file, bytesLoaded, percent, useTime, uSpeed, reTime);
}

/// <summary>
/// 上传成功时执行
/// </summary>
/// <param name="file">当前上传的文件</param>
/// <param name="savePath">保存的路径</param>
function uploadSuccess(file, savePath) {
    // 花费使用时间
    var currentTime = new Date();
    var useTime = (Math.ceil(currentTime - iTime) / 1000);

    uploadSuccessed(file, savePath, useTime);
}

/// <summary>
/// 上传完成时
/// </summary>
/// <param name="file">当前上传的文件</param>
function uploadComplete(file) {
    // 队列还有文件时，继续上传
    if (this.getStats().files_queued > 0) { this.startUpload(); }
    else { }
}

/// <summary>
/// 文件失败加入队列时执行
/// </summary>
/// <param name="file">当前选择的文件</param>
/// <param name="error">错误代码</param>
/// <param name="message">错误提示</param>
function uploadError(file, error, message) {
    switch (error) {
        case -100:
            {
                message = "你最多只能上传" + swfu.settings.file_upload_limit + "张文件！"; break;
            }
        case -110:
            {
                message = "文件超出限制大小" + swfu.settings.file_size_limit + "！"; break;
            }
        case -120:
            {
                message = "文件大小，不能为空！";
                break;
            }
        case -130:
            {
                message = "文件类型不允许上传！";
                break;
            }
        case -200:
            {
                message = "服务器报错了！";
                break;
            }
        case -210:
            {
                message = "没有找到服务器接收文件的地址！";
                break;
            }
        case -220:
            {
                message = "无法保存文件！";
                break;
            }
        case -230:
            {
                message = "安全错误，上传违反了安全约束！";
                break;
            }
        case -240:
            {
                message = "上传的文件数量限制已达到最大！";
                break;
            }
        case -250:
            {
                message = "初始化失败，请重试！";
                break;
            }
        case -260:
            {
                message = "您选择的文件不存在！";
                break;
            }
        case -270:
            {
                message = "服务器连接成功，但无法上传！";
                break;
            }
        case -280:
            {
                message = "取消上传！";
                break;
            }
        case -290:
            {
                message = "暂停上传！";
                break;
            }
        default:
            {
                message = error + "：" + message;
                break;
            }
    }
    if (file == null) { alert(message); return; }
    uploadErrored(file, error, message);
}

/// <summary>
/// 删除队列
/// </summary>
/// <param name="ID">选择的文件数量</param>
function deleteQueue(ID) {
    var index = Number(ID.split("_")[2]);
    var file = swfu.getFile(index);
    if (file == null) { return; }

    swfu.cancelUpload(ID, false);
    switch (file.filestatus) {
        case -4: // 成功上传 
            {
                swfu.setStats({ successful_uploads: swfu.getStats().successful_uploads - 1 });
                // 删除ListBox项
                removeNode(file.index);
                break;
            }
    }

    deleteQueued(ID);
}

/// <summary>
/// 转换成计算机容量表达方式
/// </summary>
/// <param name="size">文件容量大小，单位byte</param>
function getSize(size) {
    if (size == 0) { return ""; }

    var sizeType = 0;               // 容量单位类型
    while (size >= 1024) {
        size = size /= 1024;
        sizeType++;
    }

    size = size.toFixed(2);
    switch (sizeType) {
        case 0: return size + " byte";
        case 1: return size + " KB";
        case 2: return size + " MB";
        case 3: return size + " GB";
        default:

    }
}

/// <summary>
/// 四舍五入求值
/// </summary>
/// <param name="num">值</param>
/// <param name="dec">放大倍数幂</param>
function roundNumber(num, dec) {
    return Math.round(num * Math.pow(10, dec)) / Math.pow(10, dec);
}

/// <summary>
/// 得出时、分、秒的值
/// </summary>
/// <param name="text">文本</param>
/// <param name="value">值</param>
function minsec(time, tempTime) {
    var ztime;
    if (time == "m") {
        ztime = Math.floor(tempTime / 60);
        if (ztime < 10) {
            ztime = "0" + ztime;
        }
    } else if (time == "s") {
        ztime = Math.ceil(tempTime % 60);
        if (ztime < 10) {
            ztime = "0" + ztime;
        }
    } else {
        ztime = "计算错误...";
    }
    return ztime;
}

/// <summary>
/// 将上传的路径添加到列表中（用于其它窗口取值）
/// </summary>
/// <param name="text">文本</param>
/// <param name="value">值</param>
function addNode(text, value) {
    var selValue = document.getElementById('lstItem');
    if (!selValue) { return; }

    selValue.options.add(new Option(text, value));
}

/// <summary>
/// 将上传的路径添加到列表中（用于其它窗口取值）
/// </summary>
/// <param name="value">值</param>
function removeNode(fileIndex) {
    var selValue = document.getElementById('lstItem');
    if (!selValue) { return; }

    for (var index = 0; index < selValue.options.length; index++) {
        if (selValue.options[index].text.split("|")[0] == fileIndex) { selValue.remove(index); break; }
    }
}

function GetParam(key) {
    ///<summary> 
    /// 获取页面URL的参数，返回包含参数的值 
    ///</summary> 
    ///<param name="key" type="String">参数名</param> 
    ///<returns type="String" /> 
    try {
        var requestStr = location.search.length > 0 ? location.search.substr(1) : "";
        if (requestStr) {
            //params = requestStr.toHash(true);
            var params = requestStr.split('&');
            for (var i = 0; i < params.length; i++) {
                var keyValue = params[i].split('=');
                var left = keyValue[0];
                var right = keyValue.length == 2 ? keyValue[1] : "";

                if (key.toLowerCase() == left.toLowerCase()) { return right; }
            }
        }
    }
    catch (e) {
    }
    return params;
}


/// <summary>
/// 读取脚本代码
/// </summary>
/// <param name="scriptPath">脚本地址</param>
function LoadScript(scriptPath) {
    document.write('<script type="text/javascript" src="' + scriptPath + '"></script>');
}

/// <summary>
/// 读取脚本代码
/// </summary>
/// <param name="scriptPath">脚本地址</param>
function LoadStyle(scriptPath) {
    document.write('<link href="' + scriptPath + '" rel="stylesheet" type="text/css" \/>');
}