<%@ Page Title="" Language="C#" 
    AutoEventWireup="true" CodeFile="ThreatDetail.aspx.cs" Inherits="Analyse_ThreatDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../CSS/Ulleum.css" rel="stylesheet" type="text/css" />
    <meta name="robots" content="noindex,nofollow" />
    <title>Ulleum. Аналитика</title>
</head>
<body style="background-color: White; width: 100%; margin: 0; height: 100%;">
    <form id="form1" runat="server">
    <div style="position: absolute; height: 100%; width: 100%; vertical-align: top; background-position: inherit;
        background-image: url(../Images/sky.jpg);
        -webkit-background-size: cover;
        -moz-background-size: cover;
        -o-background-size: cover;
        background-size: cover;" />

    <div style="position: absolute; height: 100%; width: 100%; vertical-align: bottom;
        background-position: bottom; background-image: url(../Images/grass.gif); background-repeat: repeat-x;" />
    <asp:Table ID="TableMain" runat="server" Width="100%" Height="100%" BackColor="Transparent">
        <asp:TableRow Height="30px">
            <asp:TableCell>
                <asp:Table ID="Table1" runat="server">
                    <asp:TableRow>
                        <asp:TableCell HorizontalAlign="Left">
                            <a href="../lk2.aspx"><img src="../Images/logo3.png" style="margin: 0px; height: 50px;" alt="" /></a>
                        </asp:TableCell>
                        <asp:TableCell Width="100%" HorizontalAlign="Right">
                            <asp:Label runat="server" ID="lblUserNameTitle" Font-Size="Small" />
                        </asp:TableCell>
                        <asp:TableCell> 
                            <asp:Label runat="server" ID="lblRoleTitle" Font-Size="Small" style="margin: 0px 10px 0px 10px;" />
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:LoginStatus ID="LoginStatus2" runat="server" LogoutText="Выйти" LogoutImageUrl="../Images/exit.png"
                                BorderStyle="None" BackColor="Transparent" Width="25" Height="30" Style="margin: 0px 10px 0px 10px;" />
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell VerticalAlign="Top">

                <%-- Шапка --%>
                <asp:Panel ID="Panel3" runat="server" CssClass="mainPanelTitle_alone" ScrollBars="None">
                    <asp:Table ID="Table7" runat="server" CssClass="mainPanelTitleTable">
                        <asp:TableRow>
                            <asp:TableCell Width="90px" VerticalAlign="Middle" HorizontalAlign="Center">
                            <img src="../Images/reports_logo.png"  height="70" alt=""/>
                            </asp:TableCell>
                            <asp:TableCell HorizontalAlign="Justify">
                                <asp:Label ID="lblInterKind" runat="server" CssClass="titleText" Text="Стандартный групповой отчет по проекту: "/>
                                <br />
                                <asp:Label ID="lblProjName" runat="server"  CssClass="titleText"/>
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </asp:Panel>
                
                <%-- Данные--%>
                <asp:Panel ID="Panel4" runat="server" CssClass="mainPanel_alone" ScrollBars="Vertical">
                    <asp:Table ID="Table2" runat="server" Style="width: 100%;">

                        
                        
                        
                        <asp:TableRow>
                            <asp:TableCell VerticalAlign="Top" HorizontalAlign="Center">
                                <dx:ASPxGridView ID="dxgEmp" runat="server" DataSourceID="sqlActiveTreatsEmp"  
                                                AutoGenerateColumns="false" Width="98%" Theme="Office2010Silver" SettingsPager-PageSize="30">
                                    <Columns>
                                        <dx:GridViewDataHyperLinkColumn Width="20%"
                                            FieldName="id" Caption="Сотрудник" Settings-AllowHeaderFilter="False"
                                            PropertiesHyperLinkEdit-NavigateUrlFormatString="~/Player/TestResult.aspx?SubjId={0}"
                                            PropertiesHyperLinkEdit-TextField="fio" CellStyle-HorizontalAlign="Left" />
                                        <dx:GridViewDataColumn FieldName="relevance" Caption="Актуальность" />
                                        <dx:GridViewDataColumn FieldName="dept_name" Caption="Отдел" />
                                        <dx:GridViewDataColumn FieldName="job_name" Caption="Должность" />
                                        <dx:GridViewDataHyperLinkColumn Width="40%"
                                            FieldName="group_id" Caption="Исследование" Settings-AllowHeaderFilter="False"
                                            PropertiesHyperLinkEdit-NavigateUrlFormatString="~/Analyse/Common.aspx?g={0}"
                                            PropertiesHyperLinkEdit-TextField="research_name" CellStyle-HorizontalAlign="Left" />
                                    </Columns>
                                    <Settings ShowHeaderFilterButton="true" ShowFooter="true" />
                                        <SettingsPopup>
                                            <HeaderFilter Height="200" />
                                        </SettingsPopup>
                                        <SettingsBehavior ConfirmDelete="true" />
                                    </dx:ASPxGridView>
                            </asp:TableCell>
                        </asp:TableRow>

                        <%--<asp:TableRow>
                            <asp:TableCell VerticalAlign="Top" HorizontalAlign="Justify">
                                <asp:GridView ID="GridView1" runat="server" DataSourceID="sqlActiveTreatsEmp" AutoGenerateColumns="False"
                                    BackColor="White" BorderColor="#E7E7FF" BorderStyle="None" BorderWidth="1px"
                                    CellPadding="3" EnableSortingAndPagingCallbacks="true" GridLines="Horizontal"
                                    AllowSorting="True">
                                    <AlternatingRowStyle BackColor="#F7F7F7" />
                                    <Columns>
                                        <asp:HyperLinkField DataTextField="fio" SortExpression="fio" DataNavigateUrlFormatString="~/Player/TestResult.aspx?SubjId={0}"
                                            HeaderText="Сотрудник" DataNavigateUrlFields="id" />
                                        <asp:BoundField DataField="relevance" HeaderText="Актуальность" ReadOnly="true" SortExpression="test_date" />
                                        <asp:BoundField DataField="dept_name" HeaderText="Отдел" ReadOnly="true" SortExpression="dept_name" />
                                        <asp:BoundField DataField="job_name" HeaderText="Должность" ReadOnly="true" SortExpression="job_name" />
                                        <asp:HyperLinkField DataTextField="research_name" SortExpression="research_name"
                                            DataNavigateUrlFormatString="~/Analyse/Common.aspx?g={0}" HeaderText="Исследование"
                                            DataNavigateUrlFields="group_id" />
                                    </Columns>
                                    <FooterStyle BackColor="#B5C7DE" ForeColor="#4A3C8C" />
                                    <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#F7F7F7" />
                                    <PagerStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" HorizontalAlign="Right" />
                                    <RowStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" />
                                    <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="#F7F7F7" />
                                    <SortedAscendingCellStyle BackColor="#F4F4FD" />
                                    <SortedAscendingHeaderStyle BackColor="#5A4C9D" />
                                    <SortedDescendingCellStyle BackColor="#D8D8F0" />
                                    <SortedDescendingHeaderStyle BackColor="#3E3277" />
                                </asp:GridView>
                            </asp:TableCell>
                        </asp:TableRow>--%>


                    </asp:Table>
                </asp:Panel>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow Height="0">
            <asp:TableCell>
                <%--SQL дата--%>
                <asp:SqlDataSource ID="sqlActiveTreatsEmp" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>">
                    <SelectParameters>
                        <asp:Parameter Name="idCompany" DbType="Int32" />
                        <asp:Parameter Name="idTest" DbType="Int32" />
                        <asp:Parameter Name="idDept" DbType="Int32" />
                        <asp:Parameter Name="ishr" DbType="Boolean" />
                        <asp:QueryStringParameter Name="idInterpetation" QueryStringField="i" DbType="Int32" />
                    </SelectParameters>
                </asp:SqlDataSource>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    </form>
</body>
</html>
