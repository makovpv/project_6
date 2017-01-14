<%@ Page Title="Библиотека" Language="C#" MasterPageFile="~/MainBusinessMasterPage.master" AutoEventWireup="true" CodeFile="books.aspx.cs" Inherits="NSI_books" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <h3 runat="server" id="lblCompany"/>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <script type="text/javascript" language="javascript">
        function OnConfirmDelete() {
            if (confirm('Удалить книгу?')) return true;
            return false;
        }
    </script>
    
    <asp:GridView ID="GridView1" runat="server" DataSourceID="sqlBooks" AutoGenerateColumns="false" DataKeyNames="idBook">
        <Columns>
            <asp:BoundField DataField="title" HeaderText="Наименование книги" ItemStyle-Width="50%" ControlStyle-Width="90%"/>
            <asp:BoundField DataField="author" HeaderText="Автор" ItemStyle-Width="30%" ControlStyle-Width="90%"/>
            <asp:BoundField DataField="pages" HeaderText="Кол-во страниц" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="10%"/>
            <%--<asp:TemplateField>
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox1" runat="server"  Text='<%# Bind("pages") %>' />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Eval("pages") %>'/>
                </ItemTemplate>
            </asp:TemplateField>--%>


            <asp:TemplateField HeaderText="Компетенции">
                <ItemTemplate>
                    <asp:HyperLink runat="server" NavigateUrl='<%# "~/nsi/booklink.aspx?id=" + Eval("idBook") %>' Text="Компетенции"/>
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
                        CommandName="Delete" CommandArgument='<%# Eval("idBook") %>' 
                        ToolTip="Удалить" OnClientClick="if (!OnConfirmDelete()) return false;" />
<%--                    <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/yes_small.ico"
                        CommandName="link" ToolTip="Компетенции" />--%>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>    

        <AlternatingRowStyle BackColor="#ccffcc"/>
    </asp:GridView>

    <p>
        <asp:Button ID="btnAdd" runat="server" Text="Новая книга" OnClick="btnAdd_Click" />
    </p>

    <asp:SqlDataSource runat="server" ID="sqlBooks"
        ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>"
        SelectCommand="select * from dbo.book where idCompany = @idcompany"
        DeleteCommand="delete from dbo.book where idbook = @idbook"
        UpdateCommand="update dbo.book set title=@title, author=isnull(@author,''), pages=@pages where idbook = @idbook">

        <SelectParameters>
            <asp:Parameter Name="idcompany" DbType="Int32"/>
        </SelectParameters>
    </asp:SqlDataSource>
    
        
</asp:Content>

