<%@ Page Title="Метрика" Language="C#" MasterPageFile="~/MainBusinessMasterPage.master" AutoEventWireup="true" CodeFile="SubjFilter.aspx.cs" Inherits="Metric_SubjFilter" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <div class="nsi">
        <h3>Фильтр для метрики</h3>
        <a href="list.aspx">Вернуться к списку</a>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <script type="text/javascript" language="javascript">
        function OnConfirmDelete() {
            if (confirm('Удалить фильтр?')) return true;
            return false;
        }
    </script>

    <div class="nsi">
        <p>
            Тест:
            <asp:DropDownList runat="server" AutoPostBack="true" DataSourceID="SqlTest" DataTextField="Name" DataValueField="ID" ID="ddlTest"/>
            <br/>
            Шкала:
            <asp:DropDownList ID="ddlScale" runat="server" DataSourceID="SqlScales" 
                DataTextField="Name" DataValueField="ID" ondatabound="ddlScale_DataBound"/>
        </p>
        <asp:Button runat="server" Text="Применить" OnClick="btnApplyClick"/>
        
        <hr/>

        <h5>Статус</h5>
        <asp:GridView runat="server" DataSourceID="sqlState" AutoGenerateColumns="false" DataKeyNames="idFilter" ID="gv_State">
            <Columns>
                <asp:BoundField DataField="name" HeaderText="Статус"/>
                <asp:ButtonField ButtonType="Image" ImageUrl="~/Images/delete_small.ico" 
                    CommandName="Delete" 
                    Text="Удалить"/>

            </Columns>
        </asp:GridView>
        <asp:Button Text="Добавить все статусы" runat="server" OnClick = "btnAddStateFilterClick"/>


        <h5>Должность</h5>
        <asp:GridView runat="server" DataSourceID="sqlJob" AutoGenerateColumns="false" DataKeyNames="idFilter" ID="gv_job">
            <Columns>
                <asp:BoundField DataField="name" HeaderText="Статус"/>
                <asp:ButtonField ButtonType="Image" ImageUrl="~/Images/delete_small.ico" 
                    CommandName="Delete" 
                    Text="Удалить"/>

            </Columns>
        </asp:GridView>
        <asp:Button ID="Button1" Text="Добавить все должности" runat="server" OnClick = "btnAddJobFilterClick"/>

        <h5>Отдел</h5>
        <asp:GridView runat="server" DataSourceID="sqlDept" AutoGenerateColumns="false" DataKeyNames="idFilter" ID="gv_dept">
            <Columns>
                <asp:BoundField DataField="name" HeaderText="Статус"/>
                <asp:ButtonField ButtonType="Image" ImageUrl="~/Images/delete_small.ico" 
                    CommandName="Delete" 
                    Text="Удалить"/>

            </Columns>
        </asp:GridView>
        <asp:Button ID="Button2" Text="Добавить все отделы" runat="server" OnClick = "btnAddDeptFilterClick"/>
    </div>

    <asp:SqlDataSource
        runat="server" ID="sqlState"
        ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>"
        SelectCommand="select us.name, f.idfilter from dbo.metric_subj_filter f join user_state us on us.id = f.idState
        where idMetric = @idMetric and f.idState is not null
        order by us.name"
        DeleteCommand="delete from dbo.metric_subj_filter where idFilter = @idFilter">

        <SelectParameters>
            <asp:QueryStringParameter  QueryStringField="id" Name="idMetric" DbType="Int32"/>
        </SelectParameters>

    </asp:SqlDataSource>
    <asp:SqlDataSource
        runat="server" ID="SqlJob"
        ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>"
        SelectCommand="select j.name, f.idfilter from dbo.metric_subj_filter f join job j on j.id = f.idjob
        where idMetric = @idMetric and f.idjob is not null
        order by j.name"
        DeleteCommand="delete from dbo.metric_subj_filter where idFilter = @idFilter">

        <SelectParameters>
            <asp:QueryStringParameter  QueryStringField="id" Name="idMetric" DbType="Int32"/>
        </SelectParameters>

    </asp:SqlDataSource>
    <asp:SqlDataSource
        runat="server" ID="sqlDept"
        ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>"
        SelectCommand="select d.name, f.idfilter from dbo.metric_subj_filter f join dept d on d.id = f.idDept
        where idMetric = @idMetric and f.idDept is not null
        order by d.name"
        DeleteCommand="delete from dbo.metric_subj_filter where idFilter = @idfilter">

        <SelectParameters>
            <asp:QueryStringParameter  QueryStringField="id" Name="idMetric" DbType="Int32"/>
        </SelectParameters>

    </asp:SqlDataSource>

    <asp:SqlDataSource
        runat="server" ID="SqlTest"
        ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>"
        SelectCommand="select cast (null as int) as id, '<не указан>' as name union all select id, name from test where ispublished = 1 order by name">
    </asp:SqlDataSource>
    <asp:SqlDataSource
        runat="server" ID="SqlScales"
        ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>"
        SelectCommand="select cast (null as int) as id, '<не указана>' as name union all select id, name from scales where test_id = @test_id order by name">
        <SelectParameters>
            <asp:ControlParameter ControlID="ddlTest" PropertyName="SelectedValue" DbType="Int32" Name="test_id" />
        </SelectParameters>
    </asp:SqlDataSource>

</asp:Content>

