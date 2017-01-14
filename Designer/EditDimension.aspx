<%@ Page Language="C#" AutoEventWireup="true" Inherits="Designer_EditDimension" CodeFile="EditDimension.aspx.cs" MasterPageFile="~/MyMasterPage.master" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">    
        <div>
        <asp:HyperLink ID="hlGotoTest" runat="server">Перейти к тесту</asp:HyperLink>
        <hr />
        <br />
        <asp:Label ID="Label6" runat="server" Text="Текущий вопрос (item):"></asp:Label>
        <br />
        <div>
            <asp:Label ID="lblCurrentItem" runat="server" BackColor="#FFFFCC" 
                Font-Bold="True" Font-Italic="True"></asp:Label>
            <br/>
            <br/>
            <asp:Image ID="Image1" runat="server" 
                AlternateText="нет изображения для вопроса" />
        </div>
        <br/>


    <asp:Label ID="Label1" runat="server" Text="Варианты:  "></asp:Label>
            <%--ondatabound="DimensionVarsDropDownList_SelectedIndexChanged"--%>
    <asp:DropDownList ID="DimensionVarsDropDownList" runat="server" AutoPostBack="True" 
        DataSourceID="DimensionVarsSQL" DataTextField="name" DataValueField="id" 
            onselectedindexchanged="DimensionVarsDropDownList_SelectedIndexChanged" ondatabound="DimensionVarsDropDownList_DataBound" 
            
            >
    </asp:DropDownList>
            <asp:SqlDataSource ID="DimensionVarsSQL" runat="server" 
                ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>" SelectCommand="select distinct ssd.name, ssd.id
from items i
inner join Test_Question tq on tq.id=i.group_id
inner join SubScaleDimension ssd on ssd.test_id = tq.test_id
where i.id = @item_id">
                <SelectParameters>
                    <asp:QueryStringParameter DefaultValue="0" Name="item_id" 
                        QueryStringField="ItemID" />
                </SelectParameters>
            </asp:SqlDataSource>
    <br />
    <asp:Button ID="Button3" runat="server" Text="Новая" 
        ToolTip="Новая группа вариантов" onclick="Button3_Click" />
    <br />
    <br />
    <br />
    <asp:Label ID="Label2" runat="server" Text="Тип:  "></asp:Label>
    <asp:DropDownList ID="ddlDimensionType" runat="server" 
        DataSourceID="DimensionTypeLDS" DataTextField="name" DataValueField="id"> 
    </asp:DropDownList>
    <asp:LinqDataSource ID="DimensionTypeLDS" runat="server" 
        ContextTypeName="TesterDataClassesDataContext" EntityTypeName="" 
        Select="new (id, name)" TableName="Dimension_Types" Where="ForItem == @ForItem">
        <WhereParameters>
            <asp:Parameter DefaultValue="True" Name="ForItem" Type="Boolean" />
        </WhereParameters>
    </asp:LinqDataSource>
            
    <br />
    <asp:Label ID="Label3" runat="server" Text="Вид:  "></asp:Label>
            <%--ondatabound="DropDownList3_DataBound"> --%>
    <asp:DropDownList ID="ddlDimensionMode" runat="server" 
        DataSourceID="DimensionModeLDS" DataTextField="name" DataValueField="id">
        
    </asp:DropDownList>
    <asp:LinqDataSource ID="DimensionModeLDS" runat="server" 
        ContextTypeName="TesterDataClassesDataContext" EntityTypeName="" 
        Select="new (id, name)" TableName="Dimension_Modes">
    </asp:LinqDataSource>
    <br />
    <asp:Label ID="Label4" runat="server" Text="Кол-во градаций:  " ForeColor="#CCCCCC" 
                Width="130px"></asp:Label>
    <asp:TextBox ID="GradationCountTextBox" runat="server">0</asp:TextBox>
            <br />
    <asp:Label ID="Label7" runat="server" Text="Минимум" Width="130px"/>
    <asp:TextBox ID="MinValueTextBox" runat="server">0</asp:TextBox>
            <br />
    <asp:Label ID="Label8" runat="server" Text="Максимум" Width="130px"/>
    <asp:TextBox ID="MaxValueTextBox" runat="server">0</asp:TextBox>
            <br />
    <asp:Label ID="Label9" runat="server" Text="Шаг" Width="130px"/>
    <asp:TextBox ID="StepValueTextBox" runat="server">0</asp:TextBox>
    <br /><br />
    <asp:CheckBox ID="CheckBox1" Text="уникальность выбора" runat="server" />
            <br />
            <br />
            <asp:FileUpload ID="FileUpload1" runat="server"/>
    <br />
    
    <br />
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
        CellPadding="4" DataKeyNames="id" DataSourceID="SubScalesLDS" 
        ForeColor="#333333" GridLines="None" Width="50%" 
                onrowediting="GridView1_RowEditing" onrowdeleting="GridView1_RowDeleting" >
        <AlternatingRowStyle BackColor="White" />
        <Columns>
            <asp:BoundField DataField="OrderNumber" HeaderText="OrderNumber" 
                SortExpression="OrderNumber" />
            <asp:BoundField DataField="name" HeaderText="name" SortExpression="name" />
            <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" />
        </Columns>
        <EditRowStyle BackColor="#7C6F57" />
        <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
        <RowStyle BackColor="#E3EAEB" />
        <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
        <SortedAscendingCellStyle BackColor="#F8FAFA" />
        <SortedAscendingHeaderStyle BackColor="#246B61" />
        <SortedDescendingCellStyle BackColor="#D4DFE1" />
        <SortedDescendingHeaderStyle BackColor="#15524A" />
    </asp:GridView>
    <p>
        <asp:CustomValidator runat="server" ID="vldExistResult" BackColor="#FF3300" Width="100%" Text="Изменение вариантов ответов запрещено, т.к. существуют ответы респондентов" />
    </p>
    <asp:LinqDataSource ID="SubScalesLDS" runat="server" 
        ContextTypeName="TesterDataClassesDataContext" EnableDelete="True" 
        EnableInsert="True" EnableUpdate="True" EntityTypeName="" TableName="SubScales" 
        Where="Dimension_ID == @Dimension_ID" OrderBy="OrderNumber">
        <WhereParameters>
            <asp:ControlParameter ControlID="DimensionVarsDropDownList" DefaultValue="0" 
                Name="Dimension_ID" PropertyName="SelectedValue" Type="Int32" />
        </WhereParameters>
    </asp:LinqDataSource>
    <asp:Button ID="Button1" runat="server" Text="Доб." Enabled="False" />
    <asp:Button ID="Button2" runat="server" onclick="Button2_Click" 
        Text="Вставить, применить и перейти к след. вопросу" />
    <asp:Button ID="btnNextItem" runat="server" onclick="btnNextItem_Click" 
        Text="След. вопрос" ToolTip="Перейти к следующему вопросу" />
    <br />
    <asp:TextBox ID="tbBuffer" runat="server" BorderStyle="Solid" Height="120px" 
        TextMode="MultiLine" Width="100%" BorderColor="#0033CC"></asp:TextBox>
    <br />
    <br />
    <asp:Label ID="Label5" runat="server" Text="Применить к"></asp:Label>
    <br />
    <br />
    <asp:RadioButton ID="CurrentItemRadioButton" runat="server" Checked="True" 
        Text="текущему элементу" GroupName="rbGroup"  />
    <br />
    <asp:RadioButton ID="CurrentBlockRadioButton" runat="server" GroupName="rbGroup" 
        Text="к текущему блоку" />
    <br />
    <asp:RadioButton ID="AllTestItemRadioButton" runat="server" GroupName="rbGroup" 
        Text="ко всем элементам" />
    <br />
    <asp:Button ID="btnApply" runat="server" Text="Применить" 
        onclick="btnApply_Click" />
</div>

</asp:Content>