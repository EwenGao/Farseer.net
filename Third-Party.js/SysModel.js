/// <reference path="../Js/Main.js" />

function SysModel(w, h, icoSrc, key) {
    /*============================================================================*/
    /*==================================私有变量==================================*/
    /*============================================================================*/
    // 允许操作的行为
    var Model = { IsTree: false, IsList: false, IsIframe: false };
    // 参数名称
    var Param = { Tree: "ParentID", ParentIDs: "ParentIDs", ListID: "ID" };
    // 页面地址
    var Url = { Ajax: "ajax.ashx" };
    // 列表页索引地址
    var PageIndex = 1;
    // 初始化按钮状态
    var InitButtons = function () {
        //var ele = $(EleID.Tree);

        //var ele = Tree ? Tree.CurrentItem : null;

        //if (!Model.IsTree) { return; }
        //if (!$(EleID.BtnAdd)) { return; }
        //if (!Model.IsAdd || (Tree && ele && Tree.isRoot(ele) && !Model.IsRootAdd)) { $(EleID.BtnAdd).disable(); }
        //else { $(EleID.BtnAdd).enable(); }
    }
    // 初始化参数
    var InitParams = function (urlParams) {
        if (typeof (urlParams) == "function") { urlParams = urlParams(); }
        if (urlParams == null || urlParams == "") { urlParams = "?"; }
        if (urlParams.substr(0, 1) == "&") { urlParams = "?" + urlParams.substr(1); }
        if (urlParams.substr(0, 1) != "?") { urlParams = "?" + urlParams; }
        return urlParams;
    }
    // 初始化Url
    var InitUrl = function (url) {
        var pagePath = window.location.pathname.substr(0, window.location.pathname.lastIndexOf('/') + 1);

        if (url.substr(0, 1) != "/" && url.indexOf("http://") == -1) { return pagePath + url; }
        return url;
    }
    // 添加参数
    var AddParams = function (urlParams, key, value) {
        urlParams = InitParams(urlParams);
        if (urlParams.substr(urlParams.length - 1) != "&" && urlParams.substr(urlParams.length - 1) != "?") { urlParams += "&"; }
        urlParams += key + "=" + value;
        return urlParams;
    }
    // 获取当前树选中的节点ID
    var GetSelectedID = function () {
        var ID = Cookie.get(CookieName);
        if (!ID || ID == "0" || ID == "") { ID = "0"; }
        return ID;
    }
    // 保存当前树选中的节点ID
    var SetSelectedID = function (ID) { Cookie.set(CookieName, ID, "2100-01-01"); }
    // 简单加密
    var CookieName = function () {
        // 当前页面地址
        var pageUrl = window.location.href.substr(window.location.href.indexOf(window.location.pathname));
        if (pageUrl.indexOf("?") > -1) { pageUrl = pageUrl.substr(0, pageUrl.indexOf("?")); }
        pageUrl = pageUrl.replace(/\//g, "");

        var cookiesName = "";
        while (++i < pageUrl.length) {
            if (i % 3 != 0) { continue; }
            cookiesName += pageUrl.charCodeAt(i);
        }
        return cookiesName + key;
    }();
    // Iframe控件
    var SetIframe = function (eleID, url) {
        if (navigator.userAgent.indexOf("Firefox") > 0 || navigator.userAgent.indexOf("Chrome") > 0) { document.getElementById(eleID).contentWindow.location.href = url; }
        else { document.getElementById(eleID).src = url; }
    }

    /*==============================================================================================*/
    /*==================================在实际使用过程中被重新定义==================================*/
    /*==============================================================================================*/
    // 数据列表List.aspx的右健弹出框内容（重写使用）
    var MenuData = {}
    // 树的右健弹出框内容（重写使用）
    var TreeMenuData = {}
    // 读取树数据
    this.LoadTree = function (fun) { };
    // 获取内容列表
    this.GetList = function (pageIndex) { };
    // 读取Iframe
    this.LoadIframe = function (param) { };
    // 检测数据
    this.GetSelectData = function (caption, isRequired, isMulti) { };



    /*==============================================================================================*/
    /*===========================================公共方法===========================================*/
    /*==============================================================================================*/
    // 宽度
    this.width = w;
    // 高度
    this.height = h;
    // ico图标
    this.IcoSrc = icoSrc || "icon021";
    // 初始化树
    this.InitTree = function (eleID, url, params, paramName, treeMenuData, isRootAdd) {
        Model.IsTree = true;

        if (!eleID || eleID == "") { eleID = "tree"; }
        url = InitUrl(url);

        Param.Tree = paramName || Param.Tree;

        var s = this;
        // 读取树数据
        this.LoadTree = function (fun) {
            var param = InitParams(params);
            Server.sendRequest(url + param, null, function (Request) {
                try {
                    $(eleID).innerHTML = Request.responseText;
                    if (fun) { fun(); }

                    var node = Tree.getNode(eleID, "ID", GetSelectedID());
                    if (!node) {
                        Tree.CurrentItem = $(eleID).$T("p")[0];
                        Tree.CurrentItem.onclick.apply(Tree.CurrentItem);
                    }
                    else {
                        Tree.selectNode(node, true);
                        Tree.scrollToNode(node);
                        InitButtons(Tree.CurrentItem);
                    }
                } catch (ex) { frameElement.getTopLevelWindow().$$.dialog.alert("读取树时失败：" + ex.message); }
            });
        };
        this.LoadTree();

        // 右健树菜单
        if (treeMenuData) { TreeMenuData = treeMenuData; }
        else {
            TreeMenuData = function (menu, isRoot) {
                if (Model.IsList) {
                    if ((isRootAdd == true || isRootAdd == null || !isRoot) && s.Add) { menu.addItem("添加", s.Add, "Icons/" + s.IcoSrc + "a1.gif"); }
                }
                else {
                    if (s.Add) { menu.addItem("添加", s.Add, "Icons/" + s.IcoSrc + "a1.gif"); }
                    if (!isRoot) {
                        if (s.Move) { menu.addItem("移动", s.Move, "Icons/" + s.IcoSrc + "a7.gif"); }
                        if (s.Del) { menu.addItem("删除", s.Del, "Icons/" + s.IcoSrc + "a3.gif"); }
                    }
                }
                return menu;
            }
        }
    }
    // 初始化Iframe
    this.InitIframe = function (eleID, url, params) {
        Model.IsIframe = true;

        if (!eleID || eleID == "") { eleID = "frm"; }
        if (!url || url == "") { url = "Info.aspx"; }
        url = InitUrl(url);

        // 读取Iframe
        this.Add = this.LoadIframe = function (param) {
            if (param) { param = InitParams(params) + "&" + param; }
            else { param = InitParams(params); }

            SetIframe(eleID, url + param);
        }

        // 检测数据
        this.GetSelectData = function (caption, isRequired, isMulti) {
            if (!isRequired) { return ""; }

            var ID = GetSelectedID();
            if (isMulti && (ID == "0" || parseInt(ID) < 1)) { frameElement.getTopLevelWindow().$$.dialog.alert("请选择" + Caption + "的数据！").zindex(); return; }
            return Param.Tree + "=" + ID;
        }

        if (!Model.IsTree) { this.LoadIframe(); }
    }
    // 初始化列表
    this.InitList = function (eleID, url, params, menuData) {
        Model.IsList = true;

        if (!eleID || eleID == "") { eleID = "DataList"; }
        if (!url || url == "") { url = "List.aspx"; }
        url = InitUrl(url);

        DataGrid.init(eleID);

        var s = this;
        // 获取内容列表
        this.GetList = function (pageIndex) {
            if (pageIndex) { PageIndex = pageIndex; }
            var param = AddParams(params, "PageIndex", PageIndex);

            if (Model.IsTree) { param += "&" + Param.Tree + "=" + GetSelectedID(); }

            DataGrid.loadData(eleID, url + param);
        };

        // 检测数据
        this.GetSelectData = function (caption, isRequired, isMulti) {
            if (!isRequired) { return ""; }

            var arr = DataGrid.getSelectedValue(eleID);
            if (isRequired && (arr == null || arr.length == 0)) { frameElement.getTopLevelWindow().$$.dialog.alert("请先选择要" + caption + "的数据！").zindex(); return; }
            if (!isMulti && (arr != null && arr.length != 1)) { frameElement.getTopLevelWindow().$$.dialog.alert(caption + "，只能选择一条数据！").zindex(); return; }
            if (arr == null || arr.length == 0) { return; }
            return (isMulti ? "IDs=" : "ID=") + arr.join();
        }

        if (menuData) { MenuData = menuData; }
        else {
            MenuData = function (menu) {
                var s = S;
                if (s.Edit) { if ($("btnEdit")) { menu.addItem("编辑", s.Edit, "Icons/" + s.IcoSrc + "a2.gif"); } }
                if (s.Move) { if ($("btnMove")) { menu.addItem("移动", s.Move, "Icons/" + s.IcoSrc + "a7.gif"); } }
                if (s.View) { if ($("btnView")) { menu.addItem("查看", s.View, "Icons/" + s.IcoSrc + "a2.gif"); } }
                if (s.Del) { if ($("btnDel")) { menu.addItem("删除", s.Del, "Icons/" + s.IcoSrc + "a3.gif"); } }
                return menu;
            }
        }

        if (!Model.IsTree) { this.GetList(); }
    }
    // 初始化按扭
    this.SetButtonPage = function (eleID, url, params, caption, width, height, isRequired, isMulti, closeCallBack, okCallBack) {
        if (!url || url == "") { url = "Info.aspx"; }

        var s = this;
        var func = function () { s.ShowDialog(url, params, caption, width, height, isRequired, isMulti, closeCallBack, okCallBack); }

        if ($(eleID)) { $(eleID).onclick = func; }
        return func;
    }
    // 初始化按扭
    this.SetButtonAjax = function (eleID, url, params, caption, operateType, isRequired, isMulti, callBack) {

        var s = this;
        var func = function () { s.SendData(url, params, caption, operateType, isRequired, isMulti, callBack); }

        if ($(eleID)) { $(eleID).onclick = func; }
        return func;
    }
    // 打开Dialog窗口
    this.SetButtonDialog = function (eleID, dialogUrl, dialogParams, dialogParamsName, ajaxUrl, ajaxParams, caption, operateType, width, height, isRequired, isMulti, closeCallBack, okCallBack) {

        var s = this;
        var func = function () {
            s.ShowDialog(dialogUrl, dialogParams, caption, width, height, isRequired, isMulti, closeCallBack, function () {
                //获取子窗口的数据
                var selValue = this.content.document.getElementById('selValue').options[0].value;
                if (selValue.length == 0) { return; }
                if (GetSelectedID() == selValue) { frameElement.getTopLevelWindow().$$.dialog.alert("您所选择的内容已经在该" + caption + "下，请选择其他" + caption + "！").zindex(); return; }
                ajaxParams = AddParams(ajaxParams, dialogParamsName, selValue);
                s.SendData(ajaxUrl, ajaxParams, caption, operateType, isRequired, isMulti, okCallBack);
            });
        }
        if ($(eleID)) { $(eleID).onclick = func; }
        return func;
    }
    // 公共的打开窗口
    this.ShowDialog = function (url, params, caption, width, height, isRequired, isMulti, closeCallBack, okCallBack) {
        url = InitUrl(url);
        var data = this.GetSelectData(caption, isRequired, isMulti);
        if (isRequired && !data) { return; }

        var param = Model.IsTree ? AddParams(params, Param.Tree, GetSelectedID()) : InitParams(params);
        if (data != null && data.length > 0) { param += "&" + data; }

        if (!closeCallBack && !okCallBack) {
            var s = this;
            closeCallBack = function () {
                if (Model.IsList) { s.GetList(); } else { s.LoadTree(); }
            };
        }

        if (Model.IsList) {
            frameElement.getTopLevelWindow().$$.dialog({
                title: caption,
                content: "url:" + url + param,
                width: width,
                height: height,
                lock: true,
                parent: this,
                min: false,
                close: closeCallBack,
                ok: okCallBack
            }).zindex();
        } else {
            this.InitIframe(url + param);
        }
    }
    // 请求数据
    this.SendData = function (url, params, caption, operateType, isRequired, isMulti, callBack) {
        if (!url || url == "") { url = "ajax.ashx"; }
        url = InitUrl(url);

        if (!isRequired) { isRequired = false }
        if (!isMulti) { isMulti = false }

        var data = this.GetSelectData(caption, isRequired, isMulti);
        if (isRequired && !data) { return; }

        var param = Model.IsTree && !Model.IsIframe ? AddParams(params, Param.Tree, GetSelectedID()) : InitParams(params);
        param = AddParams(param, "OperateType", operateType);
        if (data && data.length > 0) { param += "&" + data; }

        if (!callBack) {
            var s = this;
            callBack = function () {
                if (Model.IsList) { s.GetList(); } else { s.LoadTree(); }
            };
        }

        frameElement.getTopLevelWindow().$$.dialog.confirm("确认要" + caption + "选中的数据吗？", function () {
            frameElement.getTopLevelWindow().$$.dialog.tips("正在" + caption + "数据...", 600, "loading.gif").lock().zindex();
            Server.sendRequest(url + param, null, function (Request) {
                var msg = Request.responseText;
                var icon = "fail.png";
                var closeSec = 2;
                if (msg.length == 0) { closeSec = 1; msg = caption + "成功！"; icon = "succ.png"; }
                frameElement.getTopLevelWindow().$$.dialog.tips(msg, closeSec, icon, callBack).lock().zindex();
            });
        }).lock().zindex();
    }
    // 数据列表List.aspx的右健弹出框
    this.ShowMenu = function (tr, evt) {
        if (!tr.Selected) { DataGrid.onRowClick(tr, evt); }
        evt = getEvent(evt);
        var dg = tr.parentNode.parentNode;
        Menu.close();
        if (dg.onContextMenu) {
            dg.onContextMenu(tr, evt);
        }
        else {
            evt = getEvent(evt);
            var id = dg.id;

            var menu = new Menu();
            menu.Width = 150;
            menu.setEvent(evt);
            MenuData(menu);
            menu.show();
        }
        stopEvent(evt);
    }
    // 树的右健弹出框
    this.ShowTreeMenu = function (event, ele) {
        var ID = ele.getAttribute("ID");
        Tree.selectNode(ele, true);
        var menu = new Menu();
        menu.setEvent(event);
        menu.setParam(ID);
        TreeMenuData(menu, Tree.isRoot(ele));
        menu.show();
    }
    //树型单击事件
    this.OnTreeClick = function (ele) {
        var ID = ele.getAttribute("ID");
        SetSelectedID(ID);

        try {
            if (Model.IsList) {
                this.GetList(1);
                InitButtons(ele);
            }
            if (Model.IsIframe) {
                this.LoadIframe(Model.IsTree ? (Param.Tree + "=" + GetSelectedID()) : null);
            }
        } catch (ex) { frameElement.getTopLevelWindow().$$.dialog.alert("树型单击事件出错啦：" + ex.message); }
    }

    /*=====================================================================================================================*/
    /*==================================默认实现 添加、编辑、查看、删除、移动、冻结、解冻==================================*/
    /*=====================================================================================================================*/

    // 执行该方法，可以额外检测常用功能，并自动加入
    this.Ready = function (fun) {
        if (!S) { return; }
        if (fun) { fun(); }
        S.Add = Model.IsList ? S.SetButtonPage("btnAdd", "Info.aspx", null, "添加", this.width, this.height, false, false) : S.LoadIframe;
        S.Edit = Model.IsList ? S.SetButtonPage("btnEdit", "Info.aspx", null, "编辑", this.width, this.height, true, false) : null;
        S.View = Model.IsList ? S.SetButtonPage("btnView", "View.aspx", null, "查看", this.width, this.height, false, false) : null;
        S.Del = S.SetButtonAjax("btnDel", null, null, "删除", "delete", true, true);
        S.Freeze = S.SetButtonAjax("btnFreeze", null, null, "冻结", "frozen", true, true);
        S.DisFreeze = S.SetButtonAjax("btnDisFreeze", null, null, "解冻", "disFrozen", true, true);
        if ($("btnSearch")) { $("btnSearch").onclick = function () { S.GetList(1); } }
    }
}

var S = new SysModel();

function GetList(pageIndex) { S.GetList(pageIndex); }
