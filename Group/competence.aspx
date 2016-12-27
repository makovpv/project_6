<%@ Page Title="Компетенции" Language="C#" MasterPageFile="~/MainBusinessMasterPage.master" AutoEventWireup="true" CodeFile="competence.aspx.cs" Inherits="Group_competence" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <h3>Компетенции</h3>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <script type="text/javascript" language="javascript">
    function OnConfirmDelete() {
        if (confirm('Удалить компетенцию?')) return true;
        return false;
    }
    </script>

    
    <asp:GridView ID="GridView1" runat="server" DataSourceID="sqlCompetence" AutoGenerateColumns="false" DataKeyNames="idcompetence">
        <Columns>
            <asp:BoundField DataField="name" HeaderText="Наименование"/>
            <asp:BoundField DataField="description" HeaderText="Описание"/>

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
                        CommandName="Delete" CommandArgument='<%# Eval("idcompetence") %>' 
                        ToolTip="Удалить" OnClientClick="if (!OnConfirmDelete()) return false;" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>    

        <AlternatingRowStyle BackColor="#ccffcc"/>
    </asp:GridView>

    <p>
        <asp:Button ID="btnAdd" runat="server" Text="Новая компетенция" OnClick="btnAdd_Click" />
    </p>

    <asp:SqlDataSource runat="server" ID="sqlCompetence"
        ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>"
        SelectCommand="select * from dbo.competence where idCompany = @idcompany"
        DeleteCommand="delete from dbo.competence where idcompetence = @idcompetence"
        UpdateCommand="update dbo.competence set name=@name, description=@description where idcompetence = @idcompetence">

        <SelectParameters>
            <asp:Parameter Name="idcompany" DbType="Int32"/>
        </SelectParameters>
    </asp:SqlDataSource>

</asp:Content>

