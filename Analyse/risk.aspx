<%@ Page Title="Риски и происшествия" Language="C#" MasterPageFile="~/MainBusinessMasterPage.master" AutoEventWireup="true" CodeFile="risk.aspx.cs" Inherits="Analyse_risk" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<h3>Риски и происшествия</h3>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <ajaxToolkit:ToolkitScriptManager runat="Server" EnablePartialRendering="true" ID="ScriptManager1" />    
    
    <asp:HyperLink ID="HyperLink1" runat="server" Text="вернуться в личный кабинет" NavigateUrl="~/lk2.aspx"/>
    <br/>    
    <hr/>
    <br/>
    период с: 
    <asp:TextBox ID="tboxBeginDate" runat="server" Width="100px" style="margin: 2px 0px 2px 0px;"
        Text="01.01.2016"/>
    <asp:Button runat="server" ID="btnBeginDate" Text="..."/>
    <ajaxToolkit:CalendarExtender runat="server" Enabled="true" ID="calendar1"
            FirstDayOfWeek="Monday"
            TargetControlID="tboxBeginDate" PopupButtonID="btnBeginDate" Format="dd.MM.yyyy">
    </ajaxToolkit:CalendarExtender>
    период по: 
    <asp:TextBox ID="tboxEndDate" runat="server" Width="100px" style="margin: 2px 0px 2px 0px;"/>
    <asp:Button runat="server" ID="btnEndDate" Text="..."/>
    <ajaxToolkit:CalendarExtender runat="server" ID="CalendarExtender2" Enabled="True" FirstDayOfWeek="Monday"
            TargetControlID="tboxEndDate" PopupButtonID="btnEndDate" Format="dd.MM.yyyy">
    </ajaxToolkit:CalendarExtender>

    <br/>
    <hr/>
    
    <asp:Button ID="Button1" runat="server" Text="применить фильтр" />
    <br/><br/>

    <asp:GridView ID="GridView1" runat="server" DataSourceID="sqlRisk" AutoGenerateColumns="false" Width="100%">
        <Columns>
            <asp:BoundField DataField="subjectName" HeaderText="ФИО" ItemStyle-Width="20%"/>
            <asp:BoundField DataField="test_date" HeaderText="Дата" ItemStyle-Width="15%"/>
            <asp:BoundField DataField="risk_text" HeaderText="Риск или происшествие" ItemStyle-Width="65%"/>

        </Columns>    
        <AlternatingRowStyle BackColor="#ff9980"/>
    </asp:GridView>

    <asp:SqlDataSource runat="server" ID="sqlRisk"
     ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>"
     SelectCommand="select ts.Test_Date, 
     txt.text as risk_text, ts.fio as subjectName
from subject_group sg
join test_subject ts on ts.group_id = sg.id and ts.Test_Date is not null
join Test_Results_Txt txt on txt.subject_id = ts.id
where sg.id = 2126
and ts.Test_Date between case @dtbeg when '' then '19000101' else @dtbeg end and case @dtend when '' then '21000101' else dateadd(dd,1,@dtend) end
order by ts.Test_Date desc, ts.id">

        <SelectParameters>
            <asp:ControlParameter ControlID="tboxBeginDate" Name="dtbeg" PropertyName="Text" Type="DateTime" />
            <asp:ControlParameter ControlID="tboxEndDate" Name="dtend" PropertyName="Text" Type="DateTime" />
        </SelectParameters>

    </asp:SqlDataSource>





</asp:Content>

