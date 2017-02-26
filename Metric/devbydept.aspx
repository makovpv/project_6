<%@ Page Title="Отклонения" Language="C#" MasterPageFile="~/MainBusinessMasterPage.master" AutoEventWireup="true" CodeFile="devbydept.aspx.cs" Inherits="Metric_devbydept" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <div class="nsi">
        <h4>Отклонения по отделу</h4>
        <p>
        <asp:HyperLink runat="server" Text="вернуться в личный кабинет" NavigateUrl="~/lk2.aspx#companyPage"/>
        </p>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <div class="nsi">
        <asp:GridView ID="GridView1" runat="server" DataSourceID="sqlDevByDept" AutoGenerateColumns="false" >
            <Columns>
                <asp:BoundField DataField="fio" HeaderText="ФИО" ItemStyle-Width="30%"/>
                <asp:BoundField DataField="metric_name" HeaderText="Метрика" ItemStyle-Width="40%"/>
                <asp:BoundField DataField="test_date" HeaderText="Дата" ItemStyle-Width="10%"/>
            </Columns>

            <AlternatingRowStyle BackColor="#ccffcc"/>

        </asp:GridView>
    </div>

    <asp:SqlDataSource runat="server" ID="sqlDevByDept"
        ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>"
        SelectCommand="declare @idcompany int select @idcompany = idcompany from dept where id = @iddept
        select fio, metric_name, convert (varchar(10), test_date, 104) as test_date from dbo.MetricDeviation(@idcompany, @iddept) md
        order by md.fio">
        <SelectParameters>
            <asp:QueryStringParameter Name="iddept" QueryStringField="id" DbType="Int32"/>
        </SelectParameters>
    </asp:SqlDataSource>
</asp:Content>

