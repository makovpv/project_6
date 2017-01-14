<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EditBlock.aspx.cs" Inherits="Designer_TestBlocks" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="height: 664px">
        
        <asp:HyperLink ID="hlGotoTest" runat="server">Перейти к тесту</asp:HyperLink>
        <asp:Label ID="Label5" runat="server" Width="50px"></asp:Label>
        <asp:HyperLink ID="hlGotoBlocks" runat="server">Перейти к блокам</asp:HyperLink>
        <br/>
        <br />
        <asp:Label ID="Label4" runat="server" Text="Номер" Width="150px"></asp:Label>
        <asp:TextBox ID="tbNomer" runat="server"></asp:TextBox>
        <br />
        <asp:Label ID="Label3" runat="server" Text="Наименование" Width="150px"></asp:Label>
        <asp:TextBox ID="tbName" runat="server"></asp:TextBox>
        <br/><br/>
        <asp:CheckBox ID="cbxIsShuffledItm" runat="server" Text="Случайный порядок вопросов"/>
        <br/>
        <asp:CheckBox ID="cbxIsShuffledAns" runat="server" Text="Случайный порядок ответов"/>
        <br/>
        <asp:CheckBox ID="cbxIsTimeRestricted" runat="server" Text="Ограничение по времени"/>
        
        <hr/>
        <asp:Button ID="Button1" runat="server" Text="Ok" OnClick="Button1_Click" />
        <asp:Button ID="Button2" runat="server" Text="Cancel" OnClick="Button2_Click" />
        <hr/>

        <br />
        <asp:Label ID="Label1" runat="server" Text="Пояснение" Width="490px"></asp:Label>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <asp:TextBox runat="server" ID="txtBoxComment" TextMode="MultiLine" Columns="50"
            Rows="10" Width="716px" />
        <%--Text=  '<%# DataBinder.Eval(BlockLinqDataSource, "instruction") %>'--%>
        <br />
        <ajaxToolkit:HtmlEditorExtender ID="htmlEditorExtender2" TargetControlID="txtBoxComment"
            runat="server" DisplaySourceTab="True" OnImageUploadComplete="ajaxFileUpload_OnUploadComplete">
            <Toolbar>
                <ajaxToolkit:Bold />
                <ajaxToolkit:Italic />
                <ajaxToolkit:Underline />
                <ajaxToolkit:HorizontalSeparator />
                <ajaxToolkit:JustifyLeft />
                <ajaxToolkit:JustifyCenter />
                <ajaxToolkit:JustifyRight />
                <ajaxToolkit:JustifyFull />
                <ajaxToolkit:CreateLink />
                <ajaxToolkit:UnLink />
                <ajaxToolkit:InsertImage />
                <ajaxToolkit:FontNameSelector />
                <ajaxToolkit:FontSizeSelector />
                <ajaxToolkit:InsertOrderedList />
                <ajaxToolkit:InsertUnorderedList />
                <ajaxToolkit:InsertHorizontalRule />
                <ajaxToolkit:HorizontalSeparator />
            </Toolbar>
        </ajaxToolkit:HtmlEditorExtender>
        <br />
        <asp:Label ID="Label2" runat="server" Text="Инструкция"></asp:Label>
        <br />
        <br />
        <asp:TextBox runat="server" ID="txtBoxInstruction" TextMode="MultiLine" Columns="50"
            Rows="10" Width="716px" />
        <ajaxToolkit:HtmlEditorExtender ID="txtBoxInstruction_HtmlEditorExtender" TargetControlID="txtBoxInstruction"
            runat="server" DisplaySourceTab="True" OnImageUploadComplete="ajaxFileUpload_OnUploadComplete">
            <Toolbar>
                <ajaxToolkit:Bold />
                <ajaxToolkit:Italic />
                <ajaxToolkit:Underline />
                <ajaxToolkit:HorizontalSeparator />
                <ajaxToolkit:JustifyLeft />
                <ajaxToolkit:JustifyCenter />
                <ajaxToolkit:JustifyRight />
                <ajaxToolkit:JustifyFull />
                <ajaxToolkit:CreateLink />
                <ajaxToolkit:UnLink />
                <ajaxToolkit:InsertImage />
                <ajaxToolkit:FontNameSelector />
                <ajaxToolkit:FontSizeSelector />
                <ajaxToolkit:InsertOrderedList />
                <ajaxToolkit:InsertUnorderedList />
                <ajaxToolkit:InsertHorizontalRule />
                <ajaxToolkit:HorizontalSeparator />
            </Toolbar>
        </ajaxToolkit:HtmlEditorExtender>
    </div>
    </form>
</body>
</html>
