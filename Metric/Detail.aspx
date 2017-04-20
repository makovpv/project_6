<%@ Page Title="Анализ метрики" Language="C#" AutoEventWireup="true" CodeFile="Detail.aspx.cs" Inherits="Metric_Detail" MasterPageFile="~/MainBusinessMasterPage.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
        <div class="nsi">
        <h3 runat="server" id="lblMetric"/>
        
        <p>
        <b>Описание метрики:</b><br/>
        <asp:Literal runat="server" ID="lDescription" />
        </p>

        <p>
        <b>Описание расчета:</b><br/>
        <asp:Literal runat="server" ID="lCalcDescription" />
        </p>

        <p>
        <b>Схема устранения отклонений:</b><br/>
        <asp:Literal runat="server" ID="lEliminate_Schema" />
        </p>
        
        <p>
        <asp:HyperLink runat="server" Text="вернуться в личный кабинет" NavigateUrl="~/lk2.aspx#companyPage"/>
        </p>
        </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    
    <div class="nsi">
        <asp:GridView runat="server" DataSourceID="sqlDetail" AutoGenerateColumns="false" >
            <Columns>
                <asp:BoundField DataField="fio" HeaderText="ФИО"  ItemStyle-Width="25%"/>
                <asp:BoundField DataField="dept_name" HeaderText="Отдел"  ItemStyle-Width="15%"/>
                <asp:BoundField DataField="job_name" HeaderText="Должность"  ItemStyle-Width="20%"/>
                <asp:BoundField DataField="test_date" HeaderText="Дата" ItemStyle-Width="12%" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="test_value" HeaderText="Значение" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="12%" />
                <asp:BoundField DataField="comment" HeaderText="Комментарий" ItemStyle-Width="26%" />
            </Columns>

            <AlternatingRowStyle BackColor="#ccffcc"/>

        </asp:GridView>
    </div>

    <asp:SqlDataSource runat="server" ID="sqlDetail"
        ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>"
        SelectCommand="
        set dateformat 'dmy' 
        declare @idcompany int select @idcompany  =idcompany from dbo.metric where idmetric = @idMetric
        SELECT cast(md.test_value as int) as test_value, md.fio, convert (varchar(10), md.test_date, 104) as test_date,
            d.name as dept_name,
            j.name as job_name,
            md.comment
        FROM dbo.MetricDeviation (@idcompany, null) md
        left join dept d on d.id = md.iddept
        left join job j on j.id = md.idjob
        where idmetric = @idMetric
        order by md.fio">

        <SelectParameters>
            <asp:QueryStringParameter Name="idMetric" QueryStringField="id" Type=Int32/>
        </SelectParameters>
    </asp:SqlDataSource>


</asp:Content>
