<%@ Page Title="Идея" Language="C#" AutoEventWireup="true" CodeFile="idea.aspx.cs" Inherits="Group_idea" 
    MasterPageFile="~/MainBusinessMasterPage.master" %>

<%--<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>--%>
<%--<%@ Register TagPrefix="ctr" TagName="lnkFiles" Src="~/Group/LinkedFilesControl.ascx" %>--%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    
    <%--<ajaxToolkit:ToolkitScriptManager runat="Server" EnablePartialRendering="true" ID="ScriptManager1" />--%>
    <script type="text/javascript" language="javascript">
        function OnConfirmDelete() {
            if (confirm('Удалить файл?')) return true;
            return false;
        }
    </script>

    <p>
    <asp:Label ID="lFIO" runat="server"/><br/>
    <asp:Label ID="lDate" runat="server"/><br/>
    <asp:Label ID="lDept" runat="server"/><br/>
    </p>

    <div runat="server" id="divContent">
        <asp:Repeater DataSourceID="sqlSubjResults" runat="server">
            <ItemTemplate>
                <asp:Label runat="server" Text='<%# Eval("header") %>' Font-Bold="true" />
                <br/>
                <asp:Label runat="server" Text='<%# Eval("text") %>'  Font-Italic="true"/>
                <br/><br/>
            </ItemTemplate>
        </asp:Repeater>
    </div>

    <%--<ctr:lnkFiles runat="server" ID="lnkFiles" />--%>
    <asp:Panel ID="Panel1" BackColor="LightGray" runat="server">
        <asp:Repeater runat="server" ID="rptFiles">
            <ItemTemplate>
                <asp:Label ID="Label2" runat="server" Text='<%# Eval("Name") %>' Width="30%" />
                <asp:Button runat="server" Text="Скачать" CssClass="menuLinkButton"
                    CommandArgument='<%# Eval("Name") %>' CommandName="Download" OnCommand="CommandBtn_Click" />
                <asp:Button runat="server" Text="Удалить" CssClass="menuLinkButton"
                    CommandArgument='<%# Eval("Name") %>' CommandName="Delete" 
                    OnCommand="CommandBtn_Click" OnClientClick="if (!OnConfirmDelete()) return false;" />

                <br/>
            </ItemTemplate>
        </asp:Repeater>
        <br/>
        <asp:HiddenField runat="server" ID="hididea"/>
        <asp:Label ID="Label3" runat="server" Text="Что бы прикрепить документ" Width="30%"/>
        <asp:FileUpload runat="server" ID="uplControl"  ToolTip="Добавляет новый файл" />
        <asp:Button ID="Button2" runat="server" Text="Отправить" CommandName="Upload" OnCommand="CommandBtn_Click"/>
    </asp:Panel>

    <br/>
    краткое резюме оценочной комиссии
    <br/>
    <asp:TextBox runat="server" ID="tbResume"
        BorderStyle="Solid" Height="100px" 
        TextMode="MultiLine" Width="50%" BorderColor="#0033CC"/>
    <br/>

    <p>
    <asp:Label ID="Label1" Text="Статус" runat="server" /><br/>
    <asp:DropDownList ID="ddlState" runat="server"  >
        <asp:ListItem Value="0">На рассмотрении</asp:ListItem>
        <asp:ListItem Value="4">Отправить на доработку</asp:ListItem>
        <asp:ListItem Value="5">Отклонено</asp:ListItem>
        <asp:ListItem Value="6">Реализовано</asp:ListItem>
        <asp:ListItem Value="7">К внедрению</asp:ListItem>
        <asp:ListItem Value="8">Не оценивается</asp:ListItem>
    </asp:DropDownList>

    <asp:HyperLink ID="lnkEditor" Text="Редактировать" runat="server" />
    </p>

    <asp:LoginView runat="server">
        <RoleGroups>
            <asp:RoleGroup Roles="Expert">
                <ContentTemplate>
                    <asp:Button ID="btnSave" runat="server" Text="Сохранить" OnClick="btnSaveClick"/>
                </ContentTemplate>
            </asp:RoleGroup>
        </RoleGroups>
    </asp:LoginView>
    
    
    <p>
    <asp:HyperLink NavigateUrl="~/lk2.aspx" Text="Вернуться в Личный Кабинет" runat="server" />
    <br/>
    <br/>
    <asp:HyperLink NavigateUrl="~/Analyse/IdeaList.aspx" Text="База идей" runat="server" />
    </p>

    <asp:SqlDataSource ID="sqlSubjResults" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>" 
        SelectCommand="SELECT [text], header FROM ( 
        select txt.[text], i.[text] as header , i.id as item_id, i.number 
        from test_results_txt txt 
        inner join items i on i.id = txt.item_id 
        where txt.Subject_id = @SubjID 
        union all 
        select dbo.GetMultyAnswerLine (i.id, ts.id), 
         i.[text] as header, i.id, i.number 
		from test_subject ts 
		INNER join subscaledimension ssd on ssd.test_id = ts.test_id and ssd.dimension_type in (13,1,2)  
		left join items i on i.dimension_id = ssd.id  
        where ts.id = @SubjID) q
        order by number, item_id">
        <SelectParameters>
            <asp:Parameter Name="SubjID" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
</asp:Content>