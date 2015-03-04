window.addEvent("domready", function () {
    CreateIframe();
});



// 获取登陆状态
function LoadLoginStatus() {
    //iframe.reload(); // 登陆窗口的Iframe刷新，重新获取登陆状态
}



// 创建iframe，该窗口主要用于创建本地Cookies
function CreateIframe() {
    var iframe = document.createElement("iframe");
    iframe.src = "CreateCookies.ashx";
    iframe.width = 0;
    iframe.height = 0;
    document.body.appendChild(iframe);

    if (iframe.attachEvent) {
        iframe.attachEvent("onload", function () { LoadLoginStatus(); });
    }
    else {
        iframe.onload = function () { LoadLoginStatus(); };
    }
}