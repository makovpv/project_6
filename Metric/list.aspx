﻿<%@ Page Title="Метрики" Language="C#" MasterPageFile="~/MainBusinessMasterPage.master" AutoEventWireup="true" CodeFile="list.aspx.cs" Inherits="Metric_list" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <h3 runat="server" id="lblCompany" class="nsi"/>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <script type="text/javascript" language="javascript">
        function OnConfirmDelete() {
            if (confirm('Удалить метрику?')) return true;
            return false;
        }
    </script>
    
    <div class="nsi">
        <asp:GridView ID="GridView1" runat="server" DataSourceID="sqlMetrics" AutoGenerateColumns="false" DataKeyNames="idMetric">
            <Columns>
                <asp:BoundField DataField="name" HeaderText="Наименование" ItemStyle-Width="50%" ControlStyle-Width="90%"/>
                <asp:BoundField DataField="description" HeaderText="Описание" ItemStyle-Width="30%" ControlStyle-Width="90%"/>
                <asp:BoundField DataField="datecreate" HeaderText="Дата" ReadOnly="true"/>
                <asp:BoundField DataField="index_value" HeaderText="Условие" ReadOnly="true"/>
                <asp:BoundField DataField="index_value" HeaderText="Показатель" ReadOnly="true"/>

                <asp:TemplateField HeaderText="Фильтр">
                    <ItemTemplate>
                        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# "~/metric/subjfilter.aspx?id=" + Eval("idMetric") %>' Text="Фильтр"/>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <EditItemTemplate>
                        <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/yes_small.ico"
                            CommandName="Update" ToolTip="Принять" />
                        <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/cancel_small.ico"
                            CommandName="Cancel" ToolTip="Отмена" />
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/edit_pen.ico"
                            CommandName="Edit" ToolTip="Редактировать" />
                        <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/delete_small.ico"
                            CommandName="Delete" CommandArgument='<%# Eval("idMetric") %>' 
                            ToolTip="Удалить" OnClientClick="if (!OnConfirmDelete()) return false;" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>    

            <AlternatingRowStyle BackColor="#ccffcc"/>
        </asp:GridView>

        <p>
            <asp:Button ID="btnAdd" runat="server" Text="Новая метрика" OnClick="btnAdd_Click" />
        </p>
    </div>

    <asp:SqlDataSource runat="server" ID="sqlMetrics"
        ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>"
        SelectCommand="select * from dbo.metric where idCompany = @idcompany"
        DeleteCommand="delete from dbo.metric where idMetric = @idMetric"
        UpdateCommand="update dbo.metric set name=@name, description=isnull(@description,'') where idMetric = @idMetric">

        <SelectParameters>
            <asp:Parameter Name="idcompany" DbType="Int32"/>
        </SelectParameters>
    </asp:SqlDataSource>
</asp:Content>
