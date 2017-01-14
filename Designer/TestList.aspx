<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TestList.aspx.cs" Inherits="Designer_Projects" MasterPageFile="~/MyMasterPage.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server"> 
    <script type="text/javascript" language="javascript">
        function OnConfirmDelete() {
            if (confirm('Удалить тест ?')) return true;
            return false;
        }
    </script>   
    <asp:Label ID="Label1" runat="server" Font-Bold="True" Font-Size="Larger" 
        ForeColor="#3333CC" Text='<%$Resources: GlobalRes, String1%>'></asp:Label>
    <br />
    <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" 
        CellPadding="4" DataSourceID="TestLinqDataSource" 
        DataKeyNames="id" onrowcommand="GridView2_RowCommand" EmptyDataText="пока нет никаких тестов" 
        ForeColor="#333333" Width="100%" PageSize="15" 
        ondatabound="GridView2_DataBound" onrowdatabound="GridView2_RowDataBound">
        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Image ID="Image1" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:HyperLinkField DataTextField="name" DataNavigateUrlFields="id" 
                DataNavigateUrlFormatString="~/Designer/EditTest.aspx?TestID={0}" 
                NavigateUrl="~/Designer/EditTest.aspx" HeaderText="Наименование" />
            <asp:BoundField DataField="version_number" HeaderText="Версия" ReadOnly="True"/>
            <asp:BoundField DataField="Comment" HeaderText="Комментарий" ReadOnly="True" 
                SortExpression="Comment" />
            <asp:HyperLinkField DataNavigateUrlFields="id" 
                DataNavigateUrlFormatString="~/Analyse/TestSubjectResult.aspx?TestID={0}" 
                DataTextField="id" DataTextFormatString="Результаты" 
                NavigateUrl="~/Analyse/TestSubjectResult.aspx" ShowHeader="False" 
                Visible="False" >

            <ItemStyle Width="100px" />
            </asp:HyperLinkField>

            <asp:TemplateField ShowHeader="False">
                <ItemTemplate>
                    <asp:ImageButton ID="ImageButton2" runat="server"  ImageUrl="~/Images/delete_small.ico" 
                                        CommandName="Delete" ToolTip="Удалить" 
                                        OnClientClick="if (!OnConfirmDelete()) return false;" />    

                </ItemTemplate>
            </asp:TemplateField>

        </Columns>
        <EditRowStyle BackColor="#999999" />
        <FooterStyle BackColor="#5D7B9D" ForeColor="White" Font-Bold="True" />
        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
        <SortedAscendingCellStyle BackColor="#E9E7E2" />
        <SortedAscendingHeaderStyle BackColor="#506C8C" />
        <SortedDescendingCellStyle BackColor="#FFFDF8" />
        <SortedDescendingHeaderStyle BackColor="#6F8DAE" />

<SortedAscendingCellStyle BackColor="#E9E7E2"></SortedAscendingCellStyle>

<SortedAscendingHeaderStyle BackColor="#506C8C"></SortedAscendingHeaderStyle>

<SortedDescendingCellStyle BackColor="#FFFDF8"></SortedDescendingCellStyle>

<SortedDescendingHeaderStyle BackColor="#6F8DAE"></SortedDescendingHeaderStyle>
    </asp:GridView>
    <asp:LinqDataSource ID="TestLinqDataSource" runat="server" 
        ContextTypeName="TesterDataClassesDataContext" EntityTypeName="" 
        OrderBy="name" TableName="tests" EnableDelete="True" >
        <DeleteParameters>
            <asp:ControlParameter ControlID="GridView2" Name="Test_ID" 
                PropertyName="SelectedValue" />
        </DeleteParameters>
    </asp:LinqDataSource>
    <br />
    <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="Новый" 
        ToolTip="Создать новый тест" />

    

</asp:Content>


