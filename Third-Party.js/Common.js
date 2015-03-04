/// <reference path="mootools1.2.js" />
//显示验证码
function valiCode(imgID, VerifyImagePage) {
    var str = "?";
    if (VerifyImagePage.indexOf("?") > -1) { str = "&"; }
    $(imgID).src = VerifyImagePage + str + 'random=' + Math.random();
    $(imgID).style.display = 'inline';
}

function ShowValiCode(imgID) {
    if ($(imgID).getStyle('display') == 'none') { $(imgID).onclick(); }
}

function AlertByEmpty(inputID, strErr) {
    var value = $(inputID).get('value').trim();
    $(inputID).set('value', value);
    if (value.length == 0) {
        alert(strErr);
        $(inputID).select();
        return false;
    }
    return true;
}

//通用的标签切换
//showTagID:要显示的标签ID
function SwitchTag(ele) {
    var Tags = $(ele).getParent().getChildren();
    Tags.each(function (item) { item.setStyle('display', 'none'); });
    $(ele).setStyle('display', 'block');
}

function HideTag(hideTagID) {
    var Tags = $(hideTagID).getElements('div');
    Tags.each(function (item) { item.setStyle('display', 'none'); });
}

//样式的显示切换
function ClassTab(ele, currentClass, defaultClass) {
    var Tags = $(ele).getParent().getChildren();
    Tags.each(function (item) { if (defaultClass) { item.set('class', defaultClass); } else { item.removeClass(currentClass); } });
    $(ele).set('class', currentClass);
}

function Loadding(ele) {
    $(ele).innerHTML = "<div style='text-align:center;'><img src='/Resources/Images/Loadding.gif' />正在加载数据，请稍等...</div>";
}



function GetParams(key) {
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


//列表全选
function CheckAll(form) {
    for (var i = 0; i < form.elements.length; i++) {
        var e = form.elements[i];
        if (e.Name != 'chkAll' && e.disabled == false)
            e.checked = form.chkAll.checked;
    }
}
///列表全选样式
var CurrentClassName;
function MouseOver(me, MouseOverCssClass) {
    CurrentClassName = me.className;
    me.className = MouseOverCssClass;
}

function MouseOut(me) {
    me.className = CurrentClassName;
}

//添加到收藏夹
function addToFav(title, url) {
    if (window.sidebar) { // Mozilla Firefox Bookmark
        window.sidebar.addPanel(title, url, "");
    } else if (window.external) { // IE Favorite
        window.external.addFavorite(url, title);
    } else if (window.opera) { // Opera 7+
        return false; // do nothing
    } else {
        alert('Unfortunately, your browser does not support this action,'
         + ' please bookmark this page manually by pressing Ctrl + D on PC or Cmd + D on Mac.');
    }
}

//设置主页
function SetHome(obj, vrl) {
    try { obj.style.behavior = 'url(#default#homepage)'; obj.setHomePage(vrl); }
    catch (e) {
        if (window.netscape) {
            try {
                netscape.security.PrivilegeManager.enablePrivilege("UniversalXPConnect");
            }
            catch (e) {
                alert("此操作被浏览器拒绝！\n请在浏览器地址栏输入“about:config”并回车\n然后将[signed.applets.codebase_principal_support]设置为'true'");
            }
            var prefs = Components.classes['@mozilla.org/preferences-service;1'].getService(Components.interfaces.nsIPrefBranch);
            prefs.setCharPref('browser.startup.homepage', vrl);
        }
    }
}