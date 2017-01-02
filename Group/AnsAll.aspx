<%@ Page Title="" Language="C#" MasterPageFile="~/PlayerBusinessMasterPage.master" AutoEventWireup="true" CodeFile="AnsAll.aspx.cs" Inherits="Player_AnsAll" %>
<%@ MasterType VirtualPath="~/PlayerBusinessMasterPage.master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <ajaxToolkit:ToolkitScriptManager runat="Server" EnablePartialRendering="true" />
    <script src="../Scripts/jquery-1.10.2.min.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        function empClick(ctrID) {
            var sl;
            sl = "#" + ctrID;
            $(sl).toggle();
        }
    </script>

    <div class="workarea">

        <div runat="server" id="divApproveTitle">
            <h4>Утверждение квартальной оценки</h3>
        </div>

        <div runat="server" id="divInfo" class="subjInfo"/>
        <hr />
        <div runat="server" id="divInstruction"/>
        <hr />

        <div runat="server" id="divAnonHeader">Режим анонимности<br/></div>
        <asp:RadioButtonList ID="rblAnon" runat="server" BackColor="#ff9933" Width="60%">
            <asp:ListItem Text="Ответить анонимно" Value="1" Selected="True"/>
            <asp:ListItem Text="Ответить НЕ анонимно" Value="0"/>
        </asp:RadioButtonList>

        <br />
        <div runat="server" id="divContent"/>
        <hr />
        <div runat="server" id="divApprove">
            <h5>Рекомендации сотруднику на следующий квартал</h5>
            <asp:TextBox runat="server" ID="tboxApproveComment" TextMode="MultiLine" Width="70%"/>
        </div>
        <hr />
        <asp:Button runat="server" CssClass="green" Text="Сохранить"/>
        <asp:HyperLink runat="server" NavigateUrl="~/lk2.aspx" Text="Перейти в личный кабинет"/>
        <br/>
        <br/>
    </div>
</asp:Content>

