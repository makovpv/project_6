<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SelItem.aspx.cs" Inherits="Designer_SelItem" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="height: 259px">
    <form id="form1" runat="server">
    <div>
    
    </div>
    
    <asp:Label ID="Label1" runat="server" Text="Блок" Width="100"></asp:Label>
    <asp:DropDownList ID="BlockDropDownList" runat="server" AutoPostBack="True" 
        DataSourceID="BlockLDS" DataTextField="text" DataValueField="id">
    </asp:DropDownList>
    <asp:LinqDataSource ID="BlockLDS" runat="server" 
        ContextTypeName="TesterDataClassesDataContext" EntityTypeName="" 
        Select="new (id, text)" TableName="Test_Questions" Where="test_id == @test_id">
        <WhereParameters>
            <asp:SessionParameter Name="test_id" SessionField="TestID" Type="Int32" />
        </WhereParameters>
    </asp:LinqDataSource>
    <br />
    <br />
    <asp:CheckBoxList ID="ItemsCheckBoxList" runat="server" BorderColor="#3333CC" 
        BorderStyle="Solid" BorderWidth="3px" DataSourceID="ItemsLDS" 
        DataTextField="text" DataValueField="id" Width="100%">


    </asp:CheckBoxList>
    
    
    <asp:LinqDataSource ID="ItemsLDS" runat="server" 
        ContextTypeName="TesterDataClassesDataContext" EntityTypeName="" 
        Select="new (id, text, number)" TableName="items" 
        Where="group_id == @group_id" >
        <WhereParameters>
            <asp:ControlParameter ControlID="BlockDropDownList" Name="group_id" 
                PropertyName="SelectedValue" Type="Int32" />
        </WhereParameters>
    </asp:LinqDataSource>
    <br />
    <br />
    <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="Ok" />
    <asp:Button ID="Button2" runat="server" Text="Отмена" />
    
    
    </form>
</body>
</html>
