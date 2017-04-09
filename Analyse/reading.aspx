<%@ Page Title="Отчет по книгам" Language="C#" MasterPageFile="~/MainBusinessMasterPage.master" AutoEventWireup="true" CodeFile="reading.aspx.cs" Inherits="Analyse_reading" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <div class="nsi">
        <h3>Отчет по книгам</h3>
        <p>
        <asp:HyperLink runat="server" Text="вернуться в личный кабинет" NavigateUrl="~/lk2.aspx#companyPage"/>
        </p>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="nsi">
        <asp:GridView ID="GridView1" runat="server" DataSourceID="sqlReading" AutoGenerateColumns="false" >
            <Columns>
                <asp:BoundField DataField="fio" HeaderText="ФИО"  ItemStyle-Width="30%"/>
                <asp:BoundField DataField="dept_name" HeaderText="Отдел" ItemStyle-Width="30%"/>
                <asp:BoundField DataField="name" HeaderText="Наименование книги" ItemStyle-Width="25%" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="text" HeaderText="Планируемая дата" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="15%" />
            </Columns>

            <AlternatingRowStyle BackColor="#ccffcc"/>

        </asp:GridView>
    </div>

    <asp:SqlDataSource runat="server" ID="sqlReading"
        ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>"
        SelectCommand="
        -- выгрузка по книгам

select ts.fio, ss.name, txt.text, dept.name as dept_name
from test_subject ts
join user_account ua on ua.iduser = ts.iduser
join test_results tr on ts.id = tr.subject_id and tr.item_id = 13371
join subscales ss on ss.id = tr.subscale_id
join test_results_txt txt on txt.subject_id = ts.id and txt.item_id = 13372
left join dept on dept.id = ua.iddept
where ts.test_id = 1235 and ts.test_date is not null
order by ts.fio, ss.name, txt.text">

        <SelectParameters>
    
        </SelectParameters>
    </asp:SqlDataSource>
</asp:Content>

