<%@ Page Title="База идей" Language="C#" MasterPageFile="~/MainBusinessMasterPage.master" AutoEventWireup="true" CodeFile="IdeaList.aspx.cs" Inherits="Analyse_IdeaList" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <h3>База идей</h3>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    
    <ajaxToolkit:ToolkitScriptManager runat="Server" EnablePartialRendering="true" ID="ScriptManager1" />    
    
    <asp:HyperLink runat="server" Text="вернуться в личный кабинет" NavigateUrl="~/lk2.aspx"/>
    <br/>    
    <hr/>
    статус: 
    <asp:DropDownList ID="ddlIdeaState" runat="server" 
        DataSourceID="sqlIdeaState" DataTextField="name" DataValueField="idState"/>
    рубрика:
    <asp:DropDownList ID="ddlCategory" runat="server">
        <asp:ListItem Text="<все>"/>
        <asp:ListItem Text="Корпоративная культура, климат"/>
        <asp:ListItem Text="Эффективность продаж"/>
        <asp:ListItem Text="Повышение сервиса"/>
        <asp:ListItem Text="Обучение и развитие"/>
        <asp:ListItem Text="Склад и логистика"/>
        <asp:ListItem Text="Другое"/>
    </asp:DropDownList>
    автор: 
    <asp:DropDownList ID="ddlFIO" runat="server" 
        DataSourceID="sqlUser" DataTextField="fio" DataValueField="iduser"/>
    отдел:
    <asp:DropDownList ID="ddlDept" runat="server" 
        DataSourceID="sqlDept" DataTextField="name" DataValueField="id"/>

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

    реализовано в квартале:
    <asp:DropDownList ID="ddlQuartImplement" runat="server"/>

    <br/>
    <hr/>
    
    <asp:Button ID="Button1" runat="server" Text="применить фильтр" />
    <br/><br/>

    <asp:GridView runat="server" DataSourceID="sqlIdea" AutoGenerateColumns="false">
        <Columns>
            <asp:BoundField DataField="rn" HeaderText="№№" ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Center"/>
            <asp:BoundField DataField="FIO" HeaderText="ФИО" ItemStyle-Width="15%" />
            <asp:BoundField DataField="test_date" HeaderText="Дата" ItemStyle-Width="12%" />
            <asp:BoundField DataField="state_name" HeaderText="Статус" ItemStyle-Width="12%"/>
            <asp:BoundField DataField="dept_name" HeaderText="Отдел" ItemStyle-Width="10%"/>
            <asp:BoundField DataField="category" HeaderText="Рубрика" ItemStyle-Width="10%"  />
            <asp:TemplateField HeaderText="Проблематика" ItemStyle-Width="36%">
                <ItemTemplate>
                    <asp:HyperLink runat="server" 
                        NavigateUrl='<%# "~/Group/idea.aspx?id=" +Eval("id") %>'  
                        Text='<%# Eval("ans_text") %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>    
        <AlternatingRowStyle BackColor="#ccffcc"/>
    </asp:GridView>

    <asp:SqlDataSource runat="server" ID="sqlIdea"
        ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>"
        SelectCommand="select row_number() over (order by test_date desc) as rn, q.* from (
            select ts.fio, ts.test_date, st.name as state_name, ua.idjob, ua.iddept, idea.idSubject, dept.name as dept_name
            ,ss.name as category
            ,case when isnull(trx.text,'')='' then 'не указана' else left(trx.text, 100) end as ans_text
            ,idea.id
            from idea
            join test_subject ts on ts.id = idea.idsubject
            join user_account ua on ua.iduser = ts.iduser
            join idea_state st on st.idstate = idea.idstate
            left join dept on dept.id = ua.iddept
            
            join test_results tr on tr.subject_id = ts.id
            join items i on i.id = tr.item_id 
            join subscales ss on ss.id = tr.subscale_id

			join test_results_txt trx on trx.subject_id = ts.id
            join items it on it.id = trx.item_id 

            where ua.idcompany = @idcompany and it.number='2' and i.[number] = '1'
            and idea.idstate = case @idstate when 99 then idea.idstate else @idstate end
            and ss.name = case @cat when '<все>' then ss.name else @cat end
            and ts.iduser = case @iduser when '00000000-0000-0000-0000-000000000000' then ts.iduser else @iduser end
            and ua.iddept = case @iddept when -1 then ua.iddept else @iddept end
            and ts.test_date >= case @dtbeg when '' then '19000101' else @dtbeg end
            and ts.test_date <= case @dtend when '' then '21000101' else dateadd(dd,1,@dtend) end
            and (@quart = '19000101' or idea.implement_date between @quart and dateadd (QQ, 1, @quart))
            ) q">
        <SelectParameters>
            <asp:Parameter Name="idcompany" DbType="Int32"/>
            <asp:ControlParameter ControlID="ddlIdeaState" Name="idstate" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="ddlCategory" Name="cat" PropertyName="SelectedItem.Text" Type="String" />
            <asp:ControlParameter ControlID="ddlFIO" Name="iduser" PropertyName="SelectedValue" Type="String" />
            <asp:ControlParameter ControlID="ddlDept" Name="iddept" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="tboxBeginDate" Name="dtbeg" PropertyName="Text" Type="DateTime" />
            <asp:ControlParameter ControlID="tboxEndDate" Name="dtend" PropertyName="Text" Type="DateTime" />
            <asp:ControlParameter ControlID="ddlQuartImplement" Name="quart" PropertyName="SelectedValue" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>

    <asp:SqlDataSource runat="server" ID="SqlIdeaState"
        ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>"
        SelectCommand="select 99 idstate, '<все>' name union all select idstate as idstate, name from idea_state">
    </asp:SqlDataSource>
    <asp:SqlDataSource runat="server" ID="SqlDept"
        ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>"
        SelectCommand="select -1 id, '<все>' name union all select id, name from dept where idcompany = @idcompany">
        <SelectParameters>
            <asp:Parameter Name="idcompany" DbType="Int32"/>
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource runat="server" ID="SqlUser"
        ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>"
        SelectCommand=
        "select 0 ord, '00000000-0000-0000-0000-000000000000' iduser, '<все>' fio
        union all
        select 1 ord, idUser, FIO from user_account where idcompany = @idcompany 
        order by 1,3">
        <SelectParameters>
            <asp:Parameter Name="idcompany" DbType="Int32"/>
        </SelectParameters>
    </asp:SqlDataSource>
</asp:Content>

