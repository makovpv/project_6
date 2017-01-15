<%@ Page Title="Компетенции" Language="C#" MasterPageFile="~/MainBusinessMasterPage.master" AutoEventWireup="true" CodeFile="booklink.aspx.cs" Inherits="NSI_booklink" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    
        <h3 runat="server" id="lblBook" class="nsi"/>
        <p>
        <asp:HyperLink runat="server" Text="вернуться к списку книг" NavigateUrl="~/nsi/books.aspx" class="nsi"/>
        </p>
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    
    <%--<asp:CheckBoxList ID="ItemsCheckBoxList" runat="server" DataSourceID="sqlBooks" 
        DataTextField="name" DataValueField="idcompetence" 
        ondatabinding="ItemsCheckBoxList_DataBinding" 
        ondatabound="ItemsCheckBoxList_DataBound" >
    </asp:CheckBoxList>--%>

    <div class="nsi">
        <asp:GridView runat="server" DataSourceID="sqlBooks" AutoGenerateColumns="false" DataKeyNames="idcompetence" ID="gvLink" >
            <Columns>
                <asp:TemplateField HeaderText="Выбор">
                    <ItemTemplate>
                        <asp:CheckBox runat="server" ID="cbox" Checked='<%# Eval("ischecked") %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="name" HeaderText="Наименование компетенции"/>
                <asp:BoundField DataField="description" HeaderText="Пояснение"/>
                <asp:BoundField DataField="idcompetence" HeaderText="Код" ItemStyle-HorizontalAlign="Center" />
            </Columns>

            <AlternatingRowStyle BackColor="#ccffcc"/>

        </asp:GridView>

        <br/>
        <asp:Button ID="Button1" runat="server" Text="Сохранить" OnClick="onClick" />
    </div>

    <asp:SqlDataSource runat="server" ID="sqlBooks"
        ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>"
        SelectCommand="
        select c.idCompetence, c.name, c.description,
            cast (case when bc.idbook is null then 0 else 1 end as bit) as isChecked
        from dbo.competence c
        left join book_competence_lnk bc on bc.idcompetence = c.idcompetence and bc.idbook = @idbook
        where c.idCompany = @idcompany">

        <SelectParameters>
            <asp:Parameter Name="idcompany" DbType="Int32"/>
            <asp:QueryStringParameter Name="idbook" QueryStringField="id" Type=Int32/>
        </SelectParameters>
    </asp:SqlDataSource>


</asp:Content>

