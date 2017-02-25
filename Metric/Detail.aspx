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
                <asp:BoundField DataField="fio" HeaderText="ФИО"  ItemStyle-Width="30%"/>
                <asp:BoundField DataField="test_date" HeaderText="Дата" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="test_value" HeaderText="Значение" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="15%" />
            </Columns>

            <AlternatingRowStyle BackColor="#ccffcc"/>

        </asp:GridView>
    </div>

    <asp:SqlDataSource runat="server" ID="sqlDetail"
        ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>"
        SelectCommand="
        select cast(td.test_value as int) as test_value, ts.fio, convert (varchar(10), ts.test_date, 104) as test_date
        from metric m 
        inner join Test_Data td on m.idScale = td.Scale_ID 
        inner join test_subject ts on ts.id = td.subject_id
        inner join user_account ua on ua.iduser = ts.iduser
        where m.idMetric = @idMetric 
            and ts.actual = 1
            and td.Test_Value < m.index_value and m.condition = '<' 
            and ua.idjob in (select idjob from metric_subj_filter where idmetric = m.idmetric and idjob is not null) 
            and ua.idstate in (select idstate from metric_subj_filter where idmetric = m.idmetric and idstate is not null)
                       
        union all
        SELECT NULL AS test_value, ts.fio, NULL AS text_date 
        FROM metric m
        inner JOIN Test_Subject ts ON ts.test_id = m.idtest
        inner join user_account ua on ua.iduser = ts.iduser and ua.idcompany=m.idcompany
        WHERE m.idmetric = @idMetric
        AND m.condition = 'NE' and m.index_value = 1
        and ua.idjob in (select idjob from metric_subj_filter where idmetric = m.idmetric and idjob is not null) 
        and ua.idstate in (select idstate from metric_subj_filter where idmetric = m.idmetric and idstate is not null)
        group by m.idmetric, ts.fio
        having count (ts.test_date) = 0

        order by ts.fio">

        <SelectParameters>
            <asp:QueryStringParameter Name="idMetric" QueryStringField="id" Type=Int32/>
        </SelectParameters>
    </asp:SqlDataSource>


</asp:Content>
