<%@ Page Title="Благодарности" Language="C#" MasterPageFile="~/MainBusinessMasterPage.master" AutoEventWireup="true" CodeFile="gratitude.aspx.cs" Inherits="Analyse_gratitude" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <h3>Благодарности</h3>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <ajaxToolkit:ToolkitScriptManager runat="Server" EnablePartialRendering="true" ID="ScriptManager1" />    

    <asp:HyperLink ID="HyperLink1" runat="server" Text="вернуться в личный кабинет" NavigateUrl="~/lk2.aspx"/>
    <br/>
    <br/>
    <asp:Button ID="Button1" runat="server" Text="Фильтр" />
    <hr/>
    кто поблагодарил: 
    <asp:DropDownList ID="ddlSubject" runat="server" 
        DataSourceID="sqlUser" DataTextField="fio" DataValueField="iduser"/>
    кого поблагодарили: 
    <asp:DropDownList ID="ddlObject" runat="server" 
        DataSourceID="sqlUser" DataTextField="fio" DataValueField="iduser"/>

    <br/>
    период с: 
    <asp:TextBox ID="tboxBeginDate" runat="server" Width="100px" style="margin: 2px 0px 2px 0px;"
        Text="01.01.2016"/>
    <asp:Button runat="server" ID="btnBeginDate" Text="..."/>
    <ajaxToolkit:CalendarExtender runat="server" Enabled="true" ID="calendar1"
            TargetControlID="tboxBeginDate" PopupButtonID="btnBeginDate" Format="dd.MM.yyyy">
    </ajaxToolkit:CalendarExtender>
    период по: 
    <asp:TextBox ID="tboxEndDate" runat="server" Width="100px" style="margin: 2px 0px 2px 0px;"/>
    <asp:Button runat="server" ID="btnEndDate" Text="..."/>
    <ajaxToolkit:CalendarExtender runat="server" ID="CalendarExtender2" Enabled="True" 
            TargetControlID="tboxEndDate" PopupButtonID="btnEndDate" Format="dd.MM.yyyy">
    </ajaxToolkit:CalendarExtender>
    <br/>
    <hr/>
    <br/>

    <asp:GridView runat="server" DataSourceID="sqlGratitude" AutoGenerateColumns="false" ID="gridGratitude"
        
        OnRowCommand="grid_RowCommand">
        <Columns>
            <asp:BoundField DataField="SubjectName" HeaderText="Кто поблагодарил" ItemStyle-Width="15%" />
            <asp:BoundField DataField="test_date" HeaderText="Дата" ItemStyle-Width="13%"/>
            <asp:BoundField DataField="ObjectName" HeaderText="Кого поблагодарили" ItemStyle-Width="15%"/>
            <asp:BoundField DataField="gratitude_text" HeaderText="Благодарность" ItemStyle-Width="57%"/>
            
        </Columns>    
        <AlternatingRowStyle BackColor="#ccffcc"/>
    </asp:GridView>

    <asp:SqlDataSource runat="server" ID="sqlGratitude"
        ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>"
        SelectCommand="select ts.Test_Date, ts.fio as subjectName, txt.text as gratitude_text, ss.name as objectname
            from subject_group sg
            join test_subject ts on ts.group_id = sg.id and ts.Test_Date is not null
            join Test_Results_Txt txt on txt.subject_id = ts.id
            join Test_Results tr on tr.Subject_ID = ts.id
            join SubScales ss on ss.id = tr.SubScale_ID and tr.SelectedValue = 1
            where sg.id = 1111 
            	and ts.Test_Date between case @dtbeg when '' then '19000101' else @dtbeg end and case @dtend when '' then '21000101' else dateadd(dd,1,@dtend) end
                and ts.fio = case @subj when '<все>' then ts.fio else @subj end
                and ss.name = case @obj when '<все>' then ss.name else @obj end
                and ts.fio = case @subj_filter when '<все>' then ts.fio else @subj_filter end
                and ss.name = case @obj_filter when '<все>' then ss.name else @obj_filter end
            order by ts.Test_Date desc, ts.id">
        <SelectParameters>
            <asp:ControlParameter ControlID="ddlSubject" Name="subj" PropertyName="SelectedItem.Text" Type="String"/>
            <asp:ControlParameter ControlID="ddlObject" Name="obj" PropertyName="SelectedItem.Text" Type="String" />
            <asp:ControlParameter ControlID="tboxBeginDate" Name="dtbeg" PropertyName="Text" Type="DateTime" />
            <asp:ControlParameter ControlID="tboxEndDate" Name="dtend" PropertyName="Text" Type="DateTime" />
            <asp:Parameter Name="subj_filter" DefaultValue="<все>" />
            <asp:Parameter Name="obj_filter" DefaultValue="<все>" />
        </SelectParameters>

            <%--<asp:QueryStringParameter Name="subj_filter" QueryStringField="s" DefaultValue="<все>" />
            <asp:QueryStringParameter Name="obj_filter" QueryStringField="o" DefaultValue="<все>" />--%>
        
    </asp:SqlDataSource>

    <asp:SqlDataSource runat="server" ID="SqlUser"
        ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>"
        SelectCommand=
        "select 0 as ord, '00000000-0000-0000-0000-000000000000' iduser, '<все>' fio
        union all
        select 1 as ord, idUser, FIO from user_account where idcompany = @idcompany 
        order by 1,3">
        <SelectParameters>
            <asp:Parameter Name="idcompany" DbType="Int32"/>
        </SelectParameters>
    </asp:SqlDataSource>
</asp:Content>

