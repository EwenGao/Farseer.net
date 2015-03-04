/// <summary>
/// 初始化操作
/// </summary>
/// <param name="swfuOption">swf配置参数</param>
function skinInit(swfuOption) {
    swfuOption["button_image_url"] = "FileType/PicSingle/button.jpg";
    swfuOption["button_placeholder_id"] = "btnUpload";
    swfuOption["button_width"] = 196;
    swfuOption["button_height"] = 51;
}

/// <summary>
/// 对话框选择完成时执行
/// </summary>
/// <param name="numFilesSelected">选择的文件数量</param>
/// <param name="numFilesQueued">当前队列数量</param>
function fileDialogCompleted(numFilesSelected, numFilesQueued) {
}

/// <summary>
/// 开始上传
/// </summary>
/// <param name="file">当前上传的文件</param>
function uploadStarted(file) {
    var dom = QueueDom(file.id);
    dom.Msg.innerHTML = "正在准备上传...";

    var index = file.index - 1;
    while (index > -1) {
        deleteQueued("SWFUpload_0_" + index);
        --index;
    }
    return true;
}

/// <summary>
/// 上传的过程定时执行
/// </summary>
/// <param name="file">当前上传的文件</param>
/// <param name="bytesLoaded">已传文件节点数据</param>
/// <param name="percent">进度</param>
/// <param name="useTime">使用时间</param>
/// <param name="uSpeed">当前时间速度</param>
/// <param name="reTime">剩余时间</param>
function uploadProgressed(file, bytesLoaded, percent, useTime, uSpeed, reTime) {
    var dom = QueueDom(file.id);

    dom.Rate.innerHTML = percent + "%";
    dom.Status.style.width = percent + "%";
    dom.SaveSize.innerHTML = getSize(bytesLoaded);

    if (percent < 100) {
        var Timeleft = "计算中...";
        if (reTime != "Infinity") { Timeleft = minsec("m", reTime) + "分:" + minsec("s", reTime) + '秒'; }
        dom.Msg.innerHTML = "上传中，剩余时间: " + Timeleft;
    }
    else {
        dom.Msg.innerHTML = "正在进去加密存储";
    }
}

/// <summary>
/// 上传成功时执行
/// </summary>
/// <param name="file">当前上传的文件</param>
/// <param name="savePath">保存的路径</param>
/// <param name="useTime">使用时间</param>
function uploadSuccessed(file, savePath, useTime) {

    addNode(file.index + "|" + file.name, savePath);
    var selValue = document.getElementById('lstItem');
    selValue.options[selValue.options.length - 1].selected = true;

    var dom = QueueDom(file.id);
    var useTimeString = "，用时: " + minsec("m", useTime) + "分:" + minsec("s", useTime) + "秒";

    dom.Msg.innerHTML = "上传成功" + useTimeString + "！"
    dom.Rate.innerHTML = "100%";
    setTimeout(dom.Img.src = savePath, 2000);

    if (frameElement && frameElement.api) { frameElement.api.close(); }
}

/// <summary>
/// 文件失败加入队列时执行
/// </summary>
/// <param name="file">当前选择的文件</param>
/// <param name="error">错误代码</param>
/// <param name="message">错误提示</param>
function uploadErrored(file, error, message) {
    var dom = QueueDom(file.id);
    dom.Container.className = "progressContainer red";
    dom.Rate.innerHTML = "";
    dom.Del.innerHTML = "";
    dom.Del.removeAttribute("onclick");
    dom.Del.removeAttribute("href");
    dom.Del.removeAttribute("class");
    dom.Msg.innerHTML = message;
}

/// <summary>
/// 删除队列
/// </summary>
/// <param name="ID">选择的文件数量</param>
function deleteQueued(ID) {
    var dom = QueueDom(ID);
    document.getElementById(ID).style.display = "none";
}

/// <summary>
/// 插入html队列
/// </summary>
/// <param name="file">当前文件</param>
function insertHtml(file) {
    var div = '<div class="progressWrapper" id="' + file.id + '">';
    div += '    <div class="progressContainer green">';
    div += '       <div class="progressName" title="' + file.name + '">' + file.name + '</div>';
    div += '       <div class="pic"><img src="FileType/PicList/loadding.jpg" /></div>';
    div += '       <div class="progressFileSize">';
    div += '           <span>0 KB</span> / ' + getSize(file.size);
    div += '           <span>0%</span>';
    div += '       </div>';
    div += '       <div class="progressBarStatus">等待上传</div>';
    div += '       <div class="progressBarInProgress" style="width: 0%;"></div>';
    div += '       <div class="progressCancel"><a href="#" onclick="deleteQueue(\'' + file.id + '\') "><img src="FileType/PicList/del.png" /></a></div>';
    div += '    </div>';
    div += '</div>';
    document.getElementById("QynUploadQueue").innerHTML += div;
}

/// <summary>
/// 队列dom
/// </summary>
/// <param name="id">文件ID</param>
var QueueDom = function (id) {
    this.Container = document.getElementById(id).children[0];                       // 容器
    this.Img = Container.children[1].children[0];                                   // 图片
    this.SaveSize = Container.children[2].children[0];                              // 已传大小
    this.Rate = Container.children[2].children[1];                                  // 进度百分比
    this.Msg = Container.children[3];                                               // 消息
    this.Status = Container.children[4];                                            // 进度状态
    this.Del = Container.children[5];                                               // 删除链接
    return this;
}