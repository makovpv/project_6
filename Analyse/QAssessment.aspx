<%@ Page Title="Квартальная оценка" Language="C#" MasterPageFile="~/MainBusinessMasterPage.master" AutoEventWireup="true" CodeFile="QAssessment.aspx.cs" Inherits="Analyse_QAssessment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <div class="nsi">
        <h3>Отчет по квартальной оценке</h3>
        <p>
        <asp:HyperLink runat="server" Text="вернуться в личный кабинет" NavigateUrl="~/lk2.aspx#companyPage"/>
        </p>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="nsi">
        <asp:GridView ID="GridView1" runat="server" DataSourceID="sql_QA" AutoGenerateColumns="false" >
            <Columns>
                <asp:BoundField DataField="fio" HeaderText="ФИО" ItemStyle-Width="15%"/>
                <asp:BoundField DataField="dept_name" HeaderText="Отдел" ItemStyle-Width="15%"/>
                <asp:BoundField DataField="test_date" HeaderText="Пройдена" ItemStyle-Width="10%"/>
                <asp:BoundField DataField="isapproved" HeaderText="Состояние" ItemStyle-Width="10%"/>
                <asp:BoundField DataField="commentary" HeaderText="Комментарий" ItemStyle-Width="50%"/>
            </Columns>

            <AlternatingRowStyle BackColor="#ccffcc"/>
        </asp:GridView>
    </div>

    <asp:SqlDataSource runat="server" ID="sql_QA"
        ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>"
        SelectCommand="
select 
ua.fio, 
cast (ts.test_date as date) as test_date, 
case tsa.isapproved when 1 then 'утверждена' else 'не утверждена' end as isapproved, 
tsa.commentary, job.name, dept.name as dept_name
from user_account ua
inner join Test_Subject ts on ua.iduser = ts.iduser
inner join idea_generator g on g.idtest = ts.test_id and g.idgeneratortype = 2
left join test_subject_approved tsa on tsa.idsubject = ts.id
left join job on job.id = ua.idjob
left join dept on dept.id = ua.iddept
where ua.idcompany = @idCompany and ua.idstate not in (1,3,4) and ts.test_date >= dateadd (mm, -3, getdate())
order by dept.name, ua.fio">

        <SelectParameters>
            <asp:Parameter Name="idcompany" DbType="Int32"/>
        </SelectParameters>
    </asp:SqlDataSource>


</asp:Content>

