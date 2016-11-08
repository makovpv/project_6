<%@ Page Title="Ulleum" Language="C#" MasterPageFile="~/MainBusinessMasterPage.master"
    AutoEventWireup="true" CodeFile="lk2.aspx.cs" Inherits="lk2" UICulture="ru-RU" %>

<%@ Register Assembly="DevExpress.XtraCharts.v12.1.Web, Version=12.1.12.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.XtraCharts.Web" TagPrefix="dxchartsui" %>
<%@ Register Assembly="DevExpress.XtraCharts.v12.1, Version=12.1.12.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.XtraCharts" TagPrefix="cc1" %>
<%@ Register Assembly="DevExpress.Web.ASPxGridView.v12.1, Version=12.1.12.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dx" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script type="text/javascript" language="javascript">

        var showedItem = 'tableCompany';

        function OnConfirmDelete() {
            if (confirm('Удалить исследование?')) return true;
            return false;
        }

        function compPanelsClose() {
            compActivePanel.SetVisible(false);
            compPotencPanel.SetVisible(false);
            compMonitPanel.SetVisible(false);
            compIndicatorPanel.SetVisible(false);
            compIssledPanel.SetVisible(false);
        }

        function sotrPanelsClose() {
            sotrStuffPanel.SetVisible(false);
            sotrGaugePanel.SetVisible(false);
            sotrPersonsListPanel.SetVisible(false);
        }

        function InitMenu() {
            HideAllMenu();
            if (showedItem == 'tableCompany') {
                tableCompany.SetVisible(true);
                menuASPxPanelCompany.SetVisible(true);
            }
            else if (showedItem == 'tableHR') {
                tableHR.SetVisible(true);
                HRInfoPanel.SetVisible(false);
                menuASPxPanelHR.SetVisible(true);
            }
            else if (showedItem == 'tableSotr') {
                tableSotr.SetVisible(true);
                menuASPxPanelSotr.SetVisible(true);
            }
            else if (showedItem == 'tableUnFinishedTest') {
                tableUnFinishedTest.SetVisible(true);
                menuASPxPanelUnFinished.SetVisible(true);
            }
            else if (showedItem == 'tableLC') {
                tableLC.SetVisible(true);
                menuASPxPanelLC.SetVisible(true);
            }
        }


        function HideAllMenu() {
            tableHR.SetVisible(false);

            tableCompany.SetVisible(false);
            tableSotr.SetVisible(false);
            tableUnFinishedTest.SetVisible(false);
            tableLC.SetVisible(false);

            menuASPxPanelCompany.SetVisible(false);
            menuASPxPanelHR.SetVisible(false);
            menuASPxPanelSotr.SetVisible(false);
            menuASPxPanelUnFinished.SetVisible(false);
            menuASPxPanelLC.SetVisible(false);
            HRComplitedPanel.SetVisible(false);
        }

        function HRInfoPanelShow() {
            HideAllMenu();
            tableHR.SetVisible(true);
            HRInfoPanel.SetVisible(true);
            HRComplitedPanel.SetVisible(false);
        }

        function HRInfoPanelHide() {
            HRInfoPanel.SetVisible(false);
            InitMenu();
        }

        function tableHRShow(s) {
            HideAllMenu();
            tableHR.SetVisible(true);
            showedItem = 'tableHR';
            menuASPxPanelHR.SetVisible(true);
            HRComplitedPanel.SetVisible(false);
        }

        function tableCompanyShow(s) {
            HideAllMenu();
            tableCompany.SetVisible(true);
            showedItem = 'tableCompany';
            menuASPxPanelCompany.SetVisible(true);
        }

        function tableSotrShow(s) {
            HideAllMenu();
            tableSotr.SetVisible(true);
            showedItem = 'tableSotr';
            menuASPxPanelSotr.SetVisible(true);
        }

        function tableUnFinishedTestShow(s) {
            HideAllMenu();
            tableUnFinishedTest.SetVisible(true);
            showedItem = 'tableUnFinishedTest';
            menuASPxPanelUnFinished.SetVisible(true);
        }

        function tableLCShow(s) {
            HideAllMenu();
            tableLC.SetVisible(true);
            showedItem = 'tableLC';
            menuASPxPanelLC.SetVisible(true);
        }

        function companyHint(p) {
            menuCompanyHint.SetVisible(p);
        }

        function hrHint(p) {
            menuHRHint.SetVisible(p);
        }

        function sotrHint(p) {
            menuSotrHint.SetVisible(p);
        }

        function unFinishHint(p) {
            menuUnFinishHint.SetVisible(p);
        }

        function accountHint(p) {
            menuAccountHint.SetVisible(p);
        }
        

    </script>
    <asp:Table runat="server" Style="width: 100%; height: 100%; margin-top: -2px; background-repeat: repeat;
                background-image: url(Images/sky.jpg);">
        <asp:TableRow Height="20px" 
            style="    
            background: #FAF4DC; 
            background: -moz-linear-gradient(top, rgba(250,245,220,1) 0%, rgba(230,160,10,1) 100%); 
            background: -webkit-gradient(linear, left top, left bottom, color-stop(0%,rgba(250,245,220,1) ), color-stop(100%,rgba(230,160,10,1))); 
            background: -webkit-linear-gradient(top, rgba(250,245,220,1)  0%,rgba(230,160,10,1) 100%);
            background: -o-linear-gradient(top, rgba(250,245,220,1)  0%,rgba(230,160,10,1) 100%);
            background: linear-gradient(top, rgba(250,245,220,1) 0%,rgba(230,160,10,1) 100%);">
            <asp:TableCell HorizontalAlign="Left">
                <%--<asp:Table runat="server" style="height: 20px;">
                    <asp:TableRow>
                        <asp:TableCell CssClass="menuButtonBackPanelCell">
                            <asp:LinkButton ID="LinkButton3" runat="server" Text="Ulleum" CssClass="menuLinkButton"/>
                        </asp:TableCell>
                        <asp:TableCell CssClass="menuButtonBackPanelCell">
                            <asp:LinkButton ID="LinkButton5" runat="server" Text="О программе" CssClass="menuLinkButton" />
                        </asp:TableCell>
                        <asp:TableCell CssClass="menuButtonBackPanelCell">
                            <asp:LinkButton ID="LinkButton6" runat="server" Text="FAQ" CssClass="menuLinkButton" />
                        </asp:TableCell>
                        <asp:TableCell CssClass="menuButtonBackPanelCell">
                            <asp:LinkButton ID="LinkButton7" runat="server" Text="Написать нам" CssClass="menuLinkButton" />
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>--%>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow Height="20px">  
            <asp:TableCell HorizontalAlign="Right">
                <asp:Table ID="Table20" runat="server" style="height: 20px; margin-top: -40px;">
                    <asp:TableRow>
                        <asp:TableCell>
                            <asp:Label runat="server" ID="lblUserNameTitle" Font-Size="Small" />
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:Label runat="server" ID="lblRoleTitle" Font-Size="Small" />
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:LoginStatus ID="LoginStatus2" runat="server" LogoutText="Выйти" LogoutImageUrl="Images/exit.png"
                                BorderStyle="None" BackColor="Transparent" Width="25" Height="30" Style="margin: 0px 0px 0px 10px;" />
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </asp:TableCell>
        </asp:TableRow>
        
        <asp:TableRow >  
            <asp:TableCell style="width: 100%; height: 30px;" VerticalAlign="Top">
                <div style="margin: 0% 10% 0% 10%; width: 80%;">
                    <ul class="tabs left">
                        <li class="current first"><a href="#Tests">Задачи</a></li>
                        <li runat="server" id="liCompanyPage"><a href="#companyPage">Компания</a></li>
                        <li runat="server" id="liHRProcess"><a href="#HRProcess">HR процесс</a></li>
			            <li runat="server" id="liEmployess"><a href="#Employees">Сотрудники</a></li>
			            <li><a href="#LC">Личный кабинет</a></li>
                        <li class="last"><a href="#MyProfile">Мой профиль</a></li>
                    </ul>
                </div>
            </asp:TableCell>
        </asp:TableRow>
        
        <asp:TableRow >  
            <asp:TableCell style="width: 100%;" ColumnSpan="2" VerticalAlign="Top">
            <%-- HR --%>
            <div id="HRProcess" class="tab-content" style="display: none;">
                <asp:Panel ID="Panel4" runat="server">
                    <asp:Table ID="Table2" runat="server" Style="width: 100%;">
                        <%-- чарт и мини инфо окно--%>
                        <asp:TableRow>
                            <asp:TableCell VerticalAlign="Top">
                                <%-- чарт --%>
                                <asp:Table ID="TableChart" runat="server" Width="80%" Style="vertical-align: top; display: none;">
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <div style="position: absolute; width: 80%; top: 370px; margin: 0px; padding: 0px; vertical-align: top;
                                                left: 0px;">
                                                <asp:Table ID="TableHRYear" Width="700px" runat="server" HorizontalAlign="Left">
                                                    <asp:TableRow>
                                                        <asp:TableCell HorizontalAlign="Center">
                                                            <asp:Label ID="HRLabelYear" CssClass="titleText" Text="2016 год" runat="server"></asp:Label>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                </asp:Table>
                                            </div>
                                                        
                                            <%-- внешний бублик --%>
                                            <div style="position: absolute; width: 80%; margin: 0px 0px 0px 0px; padding: 0px;
                                                vertical-align: top; left: 0px;">
                                                <asp:Table ID="TableHRChartMonth" runat="server">
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <asp:Chart ID="ChartMonth" runat="server" ImageType="Png" ImageUrl="~/TempImages/ChartPic_#SEQ(300,3)"
                                                                Palette="EarthTones" BackColor="Transparent" Width="700px" Height="600px">
                                                                <Series>
                                                                    <asp:Series Name="MonthSeries" ChartType="Doughnut" BorderColor="180, 26, 59, 105"
                                                                        BorderWidth="1" Font="Verdana, 14pt">
                                                                        <Points>
                                                                            <asp:DataPoint Color="200, 160, 220, 150" BackSecondaryColor="250, 0, 220, 150" Label="Апрель"
                                                                                LabelAngle="-75" CustomProperties="OriginalPointIndex=3" YValues="1" />
                                                                            <asp:DataPoint Color="200, 130, 190, 130" BackSecondaryColor="250, 0, 190, 130" Label="Май"
                                                                                LabelAngle="-45" CustomProperties="OriginalPointIndex=4" YValues="1" />
                                                                            <asp:DataPoint Color="200, 100, 160, 100" BackSecondaryColor="250, 0, 160, 100" Label="Июнь"
                                                                                LabelAngle="-15" CustomProperties="OriginalPointIndex=5" YValues="1" />
                                                                            <asp:DataPoint Color="200, 230, 235, 130" BackSecondaryColor="250, 230, 235, 0" Label="Июль"
                                                                                LabelAngle="15" CustomProperties="OriginalPointIndex=6" YValues="1" />
                                                                            <asp:DataPoint Color="200, 215, 235, 090" BackSecondaryColor="250, 215, 235, 0" Label="Август"
                                                                                LabelAngle="45" CustomProperties="OriginalPointIndex=7" YValues="1" />
                                                                            <asp:DataPoint Color="200, 200, 235, 050" BackSecondaryColor="250, 200, 235, 0" Label="Сентябрь"
                                                                                LabelAngle="75" CustomProperties="OriginalPointIndex=8" YValues="1" />
                                                                            <asp:DataPoint Color="200, 250, 200, 0" BackSecondaryColor="250, 250, 200, 160" Label="Октябрь"
                                                                                LabelAngle="-75" CustomProperties="OriginalPointIndex=9" YValues="1" />
                                                                            <asp:DataPoint Color="200, 250, 170, 0" BackSecondaryColor="250, 250, 170, 130" Label="Ноябрь"
                                                                                LabelAngle="-45" CustomProperties="OriginalPointIndex=10" YValues="1" />
                                                                            <asp:DataPoint Color="200, 250, 140, 0" BackSecondaryColor="250, 250, 140, 100" Label="Декабрь"
                                                                                LabelAngle="-15" CustomProperties="OriginalPointIndex=11" YValues="1" />
                                                                            <asp:DataPoint Color="200, 160, 220, 230" BackSecondaryColor="250, 0, 200, 230" Label="Январь"
                                                                                LabelAngle="15" CustomProperties="OriginalPointIndex=0" YValues="1" />
                                                                            <asp:DataPoint Color="200, 130, 190, 230" BackSecondaryColor="250, 0, 160, 230" Label="Февраль"
                                                                                LabelAngle="45" CustomProperties="OriginalPointIndex=1" YValues="1" />
                                                                            <asp:DataPoint Color="200, 100, 160, 230" BackSecondaryColor="250, 0, 100, 230" Label="Март"
                                                                                LabelAngle="75" CustomProperties="OriginalPointIndex=2" YValues="1" />
                                                                        </Points>
                                                                    </asp:Series>
                                                                </Series>
                                                                <ChartAreas>
                                                                    <asp:ChartArea Name="ChartAreaMonth" BackColor="Transparent" ShadowColor="Transparent"
                                                                        BorderWidth="0" Area3DStyle-Enable3D="false" />
                                                                </ChartAreas>
                                                            </asp:Chart>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                </asp:Table>
                                            </div>
                                            <%-- внутренний бублик --%>
                                            <div style="position: absolute; width: 80%; margin: 0px 0px 0px 0px; padding: 0px;
                                                vertical-align: top; left: 0px;">
                                                <asp:Table ID="TableHRChartData" runat="server">
                                                    <asp:TableRow>
                                                        <asp:TableCell HorizontalAlign="Right" Width="60px">
                                                            <%--<asp:ImageButton ID="ImageButton7" runat="server" src="Images/year_left.png" alt="-1"
                                                                BorderStyle="None" BackColor="Transparent" ToolTip="Предыдущий год" />--%>
                                                        </asp:TableCell>
                                                        <asp:TableCell HorizontalAlign="Center">
                                                            <asp:Chart ID="ChartData" runat="server" BackColor="Transparent" Width="578px" Height="460"
                                                                ImageType="Png" ImageUrl="~/TempImages/ChartPic_#SEQ(300,3)" Style="margin: 70px 0px 10px 0px;">
                                                                <Series>
                                                                    <asp:Series Name="DataSeries" ChartType="Doughnut" CustomProperties="DrawingStyle=Emboss"
                                                                        BorderColor="180, 26, 59, 105" Color="220, 65, 140, 240" Font="Verdana, 10pt">
                                                                        <Points>
                                                                            <asp:DataPoint Color="200, 160, 220, 150" BackSecondaryColor="150, 0, 220, 150" ToolTip="Апрель"
                                                                                CustomProperties="OriginalPointIndex=3" YValues="1" Label="" LabelAngle="15" />
                                                                            <asp:DataPoint Color="200, 130, 190, 130" BackSecondaryColor="150, 0, 190, 130" ToolTip="Май"
                                                                                CustomProperties="OriginalPointIndex=4" YValues="1" Label="" LabelAngle="45" />
                                                                            <asp:DataPoint Color="200, 100, 160, 100" BackSecondaryColor="150, 0, 160, 100" ToolTip="Июнь"
                                                                                CustomProperties="OriginalPointIndex=5" YValues="1" Label="" LabelAngle="75" />
                                                                            <asp:DataPoint Color="200, 230, 235, 130" BackSecondaryColor="150, 230, 235, 0" ToolTip="Июль"
                                                                                CustomProperties="OriginalPointIndex=6" YValues="1" Label="" LabelAngle="-75" />
                                                                            <asp:DataPoint Color="200, 215, 235, 090" BackSecondaryColor="150, 215, 235, 0" ToolTip="Август"
                                                                                CustomProperties="OriginalPointIndex=7" YValues="1" Label="" LabelAngle="-45" />
                                                                            <asp:DataPoint Color="200, 200, 235, 050" BackSecondaryColor="150, 200, 235, 0" ToolTip="Сентябрь"
                                                                                CustomProperties="OriginalPointIndex=8" YValues="1" Label="" LabelAngle="-15" />
                                                                            <asp:DataPoint Color="200, 250, 200, 0" BackSecondaryColor="150, 250, 200, 160" ToolTip="Октябрь"
                                                                                CustomProperties="OriginalPointIndex=9" YValues="1" Label="" LabelAngle="15" />
                                                                            <asp:DataPoint Color="200, 250, 170, 0" BackSecondaryColor="150, 250, 170, 130" ToolTip="Ноябрь"
                                                                                CustomProperties="OriginalPointIndex=10" YValues="1" Label="" LabelAngle="45" />
                                                                            <asp:DataPoint Color="200, 250, 140, 0" BackSecondaryColor="150, 250, 140, 100" ToolTip="Декабрь"
                                                                                CustomProperties="OriginalPointIndex=11" YValues="1" Label="" LabelAngle="75" />
                                                                            <asp:DataPoint Color="200, 160, 220, 230" BackSecondaryColor="150, 0, 200, 230" ToolTip="Январь"
                                                                                CustomProperties="OriginalPointIndex=0" YValues="1" Label="" LabelAngle="-75" />
                                                                            <asp:DataPoint Color="200, 130, 190, 230" BackSecondaryColor="150, 0, 160, 230" ToolTip="Февраль"
                                                                                CustomProperties="OriginalPointIndex=1" YValues="1" Label="" LabelAngle="-45" />
                                                                            <asp:DataPoint Color="200, 100, 160, 230" BackSecondaryColor="150, 0, 100, 230" ToolTip="Март"
                                                                                CustomProperties="OriginalPointIndex=2" YValues="1" Label="" LabelAngle="-15" />
                                                                        </Points>
                                                                    </asp:Series>
                                                                </Series>
                                                                <ChartAreas>
                                                                    <asp:ChartArea Name="ChartAreaData" BackColor="Transparent" ShadowColor="Transparent"
                                                                        BorderWidth="0" Area3DStyle-Enable3D="false" />
                                                                </ChartAreas>
                                                            </asp:Chart>
                                                        </asp:TableCell>
                                                        <asp:TableCell HorizontalAlign="Left" Width="60px">
                                                            <%--<asp:Button ID="ImageButton8" runat="server" Text=">" Font-Size="26"
                                                                BorderStyle="None" BackColor="Transparent" ToolTip="Следующий год" />--%>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                </asp:Table>
                                            </div>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                                <%-- инфо панельки --%>
                                <asp:Table ID="TableInfo" BackColor="Transparent" runat="server" Style="margin-top: 70px; display: none;
                                    position: absolute;" Width="80%">
                                    <asp:TableRow>
                                        <asp:TableCell VerticalAlign="Top" HorizontalAlign="Left" BackColor="Transparent">
                                            <dx:ASPxRoundPanel ID="infoPanel" Style="box-shadow: 0 0 10px rgba(0,0,0,0.5); border-radius: 10px;"
                                                runat="server" Theme="Office2010Silver" ClientInstanceName="HRInfoPanel">
                                                <HeaderTemplate>
                                                    <asp:Table ID="Table3" runat="server" Width="100%">
                                                        <asp:TableRow>
                                                            <asp:TableCell HorizontalAlign="Left">
                                                                    <asp:Image ID="Image2" runat="server" src="Images/calendar.png" alt="" style="padding: 0px; margin: 0px;"/>
                                                            </asp:TableCell>
                                                            <asp:TableCell HorizontalAlign="Center">
                                                                <asp:Label ID="infoPanelHeader" runat="server" Text="" Font-Size="Large" Font-Bold="true" />
                                                            </asp:TableCell>
                                                            <asp:TableCell HorizontalAlign="Right">
                                                                <asp:ImageButton ID="ButtonInfoPanelLeftClose" runat="server" BackColor="Transparent"
                                                                    Style="padding: 0px; margin: 0px; border: none; background: none;" BorderStyle="None"
                                                                    ImageUrl="~/Images/delete_small.ico" OnClientClick="HRInfoPanel.SetVisible(false); return false;"/>
                                                            </asp:TableCell>
                                                        </asp:TableRow>
                                                    </asp:Table>
                                                </HeaderTemplate>
                                                <PanelCollection>
                                                    <dx:PanelContent>
                                                        <div>
                                                            <b>Текущие исследования</b><br />
                                                            <br />
                                                            <asp:Repeater ID="RepeaterHRNowResearch" runat="server" DataSourceID="SqlHRNowResearch">
                                                                <ItemTemplate>
                                                                    <dt><i class="icon-ok"></i>
                                                                        <asp:HyperLink ID="HyperLink3" runat="server" Text='<%# Eval("name_and_count") %>' 
                                                                            NavigateUrl='<%# "~/analyse/common.aspx?g="+Eval("ID") %>' />
                                                                        <asp:Label ID="Label3" Text='<%# Eval("stop_date") %>' runat="server" />
                                                                        <asp:ImageButton ID="ImageButton1" BorderStyle="None" BackColor="Transparent" ImageUrl="~/Images/edit_pen.ico"
                                                                            runat="server" PostBackUrl='<%# "~/group/edit.aspx?g="+Eval("ID") %>' ToolTip="Менеджер исследования" />
                                                                        <asp:ImageButton ID="ImageButton2" BorderStyle="None" BackColor="Transparent" ImageUrl="~/Images/delete_small.ico"
                                                                            runat="server" OnCommand="DeleteCurrentResearch_Command" CommandArgument='<%# Eval("id") %>'
                                                                            ToolTip="Удалить исследование" OnClientClick="if (!OnConfirmDelete()) return false;" />
                                                                        <dt />
                                                                        <%--<hr class="alt2">--%>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                            <br />
                                                            <b>Постоянные исследования</b><br />
                                                            <asp:Repeater runat="server" DataSourceID="SqlHRConstResearchAll">
                                                                <ItemTemplate>
                                                                    <dt><i class="icon-ok"></i>
                                                                        <asp:HyperLink ID="HyperLink3" runat="server" Text='<%# Eval("name_and_count") %>' 
                                                                            NavigateUrl='<%# "~/analyse/common.aspx?g="+Eval("ID") %>' />
                                                                        <asp:Label ID="Label3" Text='<%# Eval("stop_date") %>' runat="server" />
                                                                        <asp:ImageButton ID="ImageButton1" BorderStyle="None" BackColor="Transparent" ImageUrl="~/Images/edit_pen.ico"
                                                                            runat="server" PostBackUrl='<%# "~/group/edit.aspx?g="+Eval("ID") %>' ToolTip="Менеджер исследования" />
                                                                        <asp:ImageButton ID="ImageButton2" BorderStyle="None" BackColor="Transparent" ImageUrl="~/Images/delete_small.ico"
                                                                            runat="server" OnCommand="DeleteCurrentResearch_Command" CommandArgument='<%# Eval("id") %>'
                                                                            ToolTip="Удалить исследование" OnClientClick="if (!OnConfirmDelete()) return false;" />
                                                                        <dt />
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                            <br />
                                                            <b>Запланированные исследования</b><br />
                                                            <asp:Repeater ID="RepeaterHRPlanResearch" runat="server" DataSourceID="SqlHRPlanResearch">
                                                                <ItemTemplate>
                                                                    <dt><i class="icon-ok"></i>
                                                                        <asp:HyperLink ID="HyperLink3" runat="server" Text='<%# Eval("name") %>' NavigateUrl='<%# "~/analyse/common.aspx?g="+Eval("ID") %>'>
                                                                        </asp:HyperLink>
                                                                        <asp:Label ID="Label4" Text='<%# Eval("start_date") %>' runat="server" />
                                                                        <asp:ImageButton ID="ImageButton3" BorderStyle="None" BackColor="Transparent" ImageUrl="~/Images/edit_pen.ico"
                                                                            runat="server" PostBackUrl='<%# "~/group/edit.aspx?g="+Eval("ID") %>' ToolTip="Менеджер исследования" />
                                                                        <asp:ImageButton ID="ImageButton4" BorderStyle="None" BackColor="Transparent" runat="server"
                                                                            ImageUrl="~/Images/delete_small.ico" OnCommand="DeletePlanResearch_Command" CommandArgument='<%# Eval("id") %>'
                                                                            ToolTip="Удалить исследование" OnClientClick="if (!OnConfirmDelete()) return false;" />
                                                                        <dt />
                                                                        <%--<hr class="alt2">--%>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                            <br />
                                                            <b>Завершенные исследования</b><br />
                                                            <asp:Repeater ID="RepeaterHRClosedResearch" runat="server" DataSourceID="SqlHRClosedResearch">
                                                                <ItemTemplate>
                                                                    <dt><i class="icon-ok"></i>
                                                                        <asp:HyperLink ID="HyperLink3" runat="server" Text='<%# Eval("name_and_count") %>'
                                                                            NavigateUrl='<%# "~/analyse/common.aspx?g="+Eval("ID") %>'>
                                                                        </asp:HyperLink>
                                                                        <asp:Label ID="Label5" Text='<%# Eval("stop_date") %>' runat="server" />
                                                                        <asp:ImageButton ID="ImageButton5" BorderStyle="None" BackColor="Transparent" ImageUrl="~/Images/edit_pen.ico"
                                                                            runat="server" PostBackUrl='<%# "~/group/edit.aspx?g="+Eval("ID") %>' ToolTip="Менеджер исследования" />
                                                                        <asp:ImageButton ID="ImageButton6" BorderStyle="None" BackColor="Transparent" runat="server"
                                                                            ImageUrl="~/Images/delete_small.ico" OnCommand="DeleteClosedResearch_Command" CommandArgument='<%# Eval("id") %>'
                                                                            ToolTip="Удалить исследование" OnClientClick="if (!OnConfirmDelete()) return false;" />
                                                                        <dt />
                                                                        <%--<hr class="alt2">--%>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                        </div>
                                                    </dx:PanelContent>
                                                </PanelCollection>
                                            </dx:ASPxRoundPanel>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                                        
                                <asp:Table ID="Table19" BackColor="Transparent" runat="server" Style="margin-top: 0px;
                                    position: absolute;" Width="80%">
                                    <asp:TableRow>
                                        <asp:TableCell VerticalAlign="Top" HorizontalAlign="Left" BackColor="Transparent">
                                            <div style="position: absolute; width: 100%; margin: 20px 0px 0px 0px; padding: 0px;
                                                vertical-align: top; left: 0px;">
                                                <%--<asp:Table ID="TableHRCreate" runat="server" HorizontalAlign="Right">
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <asp:LoginView ID="LoginView1" runat="server">
                                                                <RoleGroups>
                                                                    <asp:RoleGroup Roles="HR">
                                                                        <ContentTemplate>
                                                                            <asp:Button ID="btnNewResearch" runat="server" 
                                                                            style="background: rgb(229,238,207); box-shadow: 0 0 2px rgba(0,0,0,0.5);" ForeColor="black"
                                                                            OnClick="btnNewResearch_Click" Text="Создать исследование" />
                                                                        </ContentTemplate>
                                                                    </asp:RoleGroup>
                                                                </RoleGroups>
                                                            </asp:LoginView>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                </asp:Table>--%>
                                            </div>                                        
                                            </asp:TableCell>
                                        </asp:TableRow>
                                    </asp:Table>
                                        
                            </asp:TableCell>
                        </asp:TableRow>
                        <%-- Исследования года --%>
                        <asp:TableRow>
                            <asp:TableCell Width="100%">
                                <asp:Table ID="Table4" BackColor="Transparent" runat="server" Width="100%" Style="margin-top: 0px;">
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Table ID="TableHRCreate" runat="server" HorizontalAlign="Center">
                                                <asp:TableRow>
                                                    <asp:TableCell>
                                                        <asp:LoginView ID="LoginView1" runat="server">
                                                            <RoleGroups>
                                                                <asp:RoleGroup Roles="HR">
                                                                    <ContentTemplate>
                                                                        <asp:Button ID="btnNewResearch" runat="server" CssClass="medium orange"
                                                                        ForeColor="black" style="width: 200px;"
                                                                        OnClick="btnNewResearch_Click" Text="Создать исследование" />
                                                                    </ContentTemplate>
                                                                </asp:RoleGroup>
                                                            </RoleGroups>
                                                        </asp:LoginView>
                                                    </asp:TableCell>
                                                </asp:TableRow>
                                            </asp:Table>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell VerticalAlign="Top" HorizontalAlign="Left" BackColor="Transparent">
                                            <div style="margin-left: 20px;">
                                            <b>Текущие исследования</b><br />
                                            <asp:Repeater ID="Repeater1" runat="server" DataSourceID="SqlHRNowResearchAll">
                                                <ItemTemplate>
                                                    <dt>
                                                        <i class="icon-ok"></i>
                                                        <asp:HyperLink ID="HyperLink3" runat="server" Text='<%# Eval("name_and_count") %>'
                                                            Visible='<%# Eval("isnotempty") %>'
                                                            NavigateUrl='<%# "~/analyse/common.aspx?g="+Eval("ID") %>' />
                                                        <asp:Label ID="Label2" Text='<%# Eval("name_and_count") %>' runat="server" Visible='<%# Eval("isempty") %>' />
                                                        <asp:Label ID="Label3" Text='<%# Eval("stop_date") %>' runat="server" />
                                                        <asp:ImageButton ID="ImageButton1" BorderStyle="None" BackColor="Transparent" ImageUrl="~/Images/edit_pen.ico"
                                                            runat="server" PostBackUrl='<%# "~/group/edit.aspx?g="+Eval("ID") %>' ToolTip="Менеджер исследования"  style="margin-left: 10px;"/>
                                                        <asp:ImageButton ID="ImageButton2" BorderStyle="None" BackColor="Transparent" ImageUrl="~/Images/delete_small.ico"
                                                            runat="server" OnCommand="DeleteCurrentResearch_Command" CommandArgument='<%# Eval("id") %>'
                                                            ToolTip="Удалить исследование" OnClientClick="if (!OnConfirmDelete()) return false;"/>
                                                                                
                                                        <dt />
                                                        <%--<hr class="alt2">--%>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                            <br />
                                            <b>Постоянные исследования</b><br />
                                            <asp:Repeater ID="Repeater2" runat="server" DataSourceID="SqlHRConstResearchAll">
                                                <ItemTemplate>
                                                    <dt>
                                                        <i class="icon-ok"></i>
                                                        <asp:HyperLink ID="HyperLink3" runat="server" Text='<%# Eval("name_and_count") %>'
                                                            Visible='<%# Eval("isnotempty") %>'
                                                            NavigateUrl='<%# "~/analyse/common.aspx?g="+Eval("ID") %>' />
                                                        <asp:Label ID="Label2" Text='<%# Eval("name_and_count") %>' runat="server" Visible='<%# Eval("isempty") %>' />
                                                        <asp:Label ID="Label3" Text='<%# Eval("stop_date") %>' runat="server" />
                                                        <asp:ImageButton ID="ImageButton1" BorderStyle="None" BackColor="Transparent" ImageUrl="~/Images/edit_pen.ico"
                                                            runat="server" PostBackUrl='<%# "~/group/edit.aspx?g="+Eval("ID") %>' ToolTip="Менеджер исследования"  style="margin-left: 10px;"/>
                                                        <asp:ImageButton ID="ImageButton2" BorderStyle="None" BackColor="Transparent" ImageUrl="~/Images/delete_small.ico"
                                                            runat="server" OnCommand="DeleteCurrentResearch_Command" CommandArgument='<%# Eval("id") %>'
                                                            ToolTip="Удалить исследование" OnClientClick="if (!OnConfirmDelete()) return false;"/>
                                                                                
                                                        <dt />
                                                </ItemTemplate>
                                            </asp:Repeater>
                                            <br />
                                            <b>Запланированные исследования</b><br />
                                            <asp:Repeater ID="Repeater7" runat="server" DataSourceID="SqlHRPlanResearchAll">
                                                <ItemTemplate>
                                                    <dt>
                                                        <i class="icon-ok"></i>
                                                        <asp:Label ID="Label4" Text='<%# Eval("name") +" "+ Eval("start_date") %>' runat="server" />

                                                        <asp:ImageButton ID="ImageButton3" BorderStyle="None" BackColor="Transparent" ImageUrl="~/Images/edit_pen.ico"
                                                            runat="server" PostBackUrl='<%# "~/group/edit.aspx?g="+Eval("ID") %>' ToolTip="Менеджер исследования"  style="margin-left: 10px;"/>
                                                        <asp:ImageButton ID="ImageButton4" BorderStyle="None" BackColor="Transparent" runat="server"
                                                            ImageUrl="~/Images/delete_small.ico" OnCommand="DeletePlanResearch_Command" CommandArgument='<%# Eval("id") %>'
                                                            ToolTip="Удалить исследование" OnClientClick="if (!OnConfirmDelete()) return false;"/>
                                                        <dt />
                                                        <%--<hr class="alt2">--%>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                            <br />
                                            </div>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>

                                            <!--Завершенные исследования-->
                                            <dx:ASPxRoundPanel ID="ASPxRoundPanel1" Width="100%" runat="server" Theme="Office2010Silver">
                                                <HeaderTemplate>
                                                    <asp:Table ID="Table6" runat="server" Width="100%">
                                                        <asp:TableRow>
                                                            <asp:TableCell HorizontalAlign="Left">
                                                                <asp:Label ID="infoPanelHeader" runat="server" Text="Завершенные исследования" Font-Bold="true" Font-Size="10" />
                                                            </asp:TableCell>
                                                            <asp:TableCell HorizontalAlign="Right">
                                                                        <asp:ImageButton ID="ImageButton1" runat="server" BackColor="Transparent" BorderStyle="None" style="padding: 0px; margin: 0px;"
                                                                        ImageUrl="~/Images/collapse.png" Width="20px" Height="20px" ToolTip="Свернуть/Развернуть"  
                                                                        OnClientClick="HRComplitedPanel.SetVisible(!HRComplitedPanel.GetVisible()); return false;"/>
                                                            </asp:TableCell>
                                                        </asp:TableRow>
                                                    </asp:Table>
                                                </HeaderTemplate>
                                                <PanelCollection>
                                                    <dx:PanelContent>
                                                        <dx:ASPxPanel CssClass="companyPanelContent" ID="ASPxPanel8" runat="server" Width="100%" ClientInstanceName="HRComplitedPanel">
                                                            <PanelCollection>
                                                                <dx:PanelContent ID="PanelContent13" runat="server">
                                                                    <asp:Repeater ID="Repeater3" runat="server" DataSourceID="SqlHRClosedResearchAll">
                                                                        <ItemTemplate>
                                                                            <dt>
                                                                                <i class="icon-ok"></i>
                                                                                <asp:HyperLink ID="HyperLink3" runat="server" Text='<%# Eval("name_and_count") %>' 
                                                                                    Visible='<%# Eval("isnotempty") %>'
                                                                                    NavigateUrl='<%# "~/analyse/common.aspx?g="+Eval("ID") %>'>
                                                                                </asp:HyperLink>
                                                                                <asp:Label ID="Label1" Text='<%# Eval("name_and_count") %>' runat="server" Visible='<%# Eval("isempty") %>' />
                                                                                <asp:Label ID="Label5" Text='<%# Eval("stop_date") %>' runat="server" />
                                                                                <asp:ImageButton ID="ImageButton5" BorderStyle="None" BackColor="Transparent" ImageUrl="~/Images/edit_pen.ico"
                                                                                    runat="server" PostBackUrl='<%# "~/group/edit.aspx?g="+Eval("ID") %>' ToolTip="Менеджер исследования" style="margin-left: 10px;"/>
                                                                                <asp:ImageButton ID="ImageButton6" BorderStyle="None" BackColor="Transparent" runat="server"
                                                                                    ImageUrl="~/Images/delete_small.ico" OnCommand="DeleteClosedResearch_Command" CommandArgument='<%# Eval("id") %>'
                                                                                    ToolTip="Удалить исследование" OnClientClick="if (!OnConfirmDelete()) return false;"/>
                                                                                <dt />

                                                                                <%--<hr class="alt2">--%>
                                                                        </ItemTemplate>
                                                                    </asp:Repeater>
                                                                </dx:PanelContent>
                                                            </PanelCollection>
                                                        </dx:ASPxPanel>
                                                    </dx:PanelContent>
                                                </PanelCollection>
                                            </dx:ASPxRoundPanel>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </asp:Panel>
            </div>
            
            <!-- Личный кабинет -->
            <div id="LC" class="tab-content clearfix">
                <asp:Table ID="Table8" runat="server" Style="width: 100%;">
                    <asp:TableRow>
                        <asp:TableCell VerticalAlign="Top">
                            <dl class="icons">
                                Пользователь:
                                <asp:Label runat="server" ID="lblUserName" Font-Bold="true" /><br />
                                Роль:
                                <asp:Label runat="server" ID="lblRole" Font-Bold="true" /><br />
                                Компания:
                                <asp:Label runat="server" ID="lblCompany" Font-Bold="true" /><br />
                                Отдел:
                                <asp:Label runat="server" ID="lblDept" Font-Bold="true" /><br />
                                Должность:
                                <asp:Label runat="server" ID="lblJob" Font-Bold="true" /><br />
                                ФИО:
                                <asp:Label runat="server" ID="lblFIO" Font-Bold="true" />
                                <br />
                                Статус:
                                <asp:Label runat="server" ID="lblStatus" Font-Bold="true" /><br />
                                <hr />
                                <a href="Admin/Password.aspx">Сменить пароль</a>
                                <br />
                                <asp:LoginStatus ID="LoginStatus1" runat="server" LogoutText="Выйти" />
                            </dl>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </div>

            <!-- Сотрудники -->
            <div id="Employees" class="tab-content clearfix" style="display: none;">
                <asp:Table ID="Table9" runat="server" Style="width: 100%;">
                    <%-- В компании работают --%>
                    <asp:TableRow HorizontalAlign="Center">
                        <asp:TableCell VerticalAlign="Top">
                            <dx:ASPxRoundPanel ID="ASPxRoundPanel7" runat="server" Width="100%" Theme="Office2010Silver" style="padding: 0px;">
                                <HeaderTemplate>
                                    <asp:Table ID="Table5" runat="server" Width="100%">
                                        <asp:TableRow>
                                            <%--<asp:TableCell HorizontalAlign="Left" Width="45">
                                                <asp:Image ID="Image5" ImageUrl="Images/sotrStuffPanel.png" runat="server" alt="" Width="45" Height="30" style="padding: 0px; margin: -5px;"/>
                                            </asp:TableCell>--%>
                                            <asp:TableCell HorizontalAlign="Left">
                                                <asp:Label ID="Label6" runat="server" Text='<%$Resources: GlobalRes, lk_emp_1%>' Font-Size="10pt"/>
                                            </asp:TableCell>
                                            <asp:TableCell HorizontalAlign="Right">
                                                    <asp:ImageButton ID="ImageButton7" runat="server" BackColor="Transparent" BorderStyle="None" 
                                                    style="padding: 0px; margin: 0px; background: none; border: none;"
                                                    ImageUrl="~/Images/collapse.png" Width="20px" Height="20px" ToolTip="Свернуть/Развернуть"  
                                                    OnClientClick="sotrStuffPanel.SetVisible(!sotrStuffPanel.GetVisible()); return false;"/>
                                            </asp:TableCell>
                                        </asp:TableRow>
                                    </asp:Table>
                                </HeaderTemplate>
                                <PanelCollection>
                                    <dx:PanelContent>
                                        <dx:ASPxPanel CssClass="companyPanelContent" ID="ASPxPanel1" runat="server" Width="100%" ClientInstanceName="sotrStuffPanel">
                                            <PanelCollection>
                                                <dx:PanelContent ID="PanelContent1" runat="server">
                                                    <asp:Table ID="Table5" runat="server" BackColor="Transparent" Width="100%">
                                                        <asp:TableRow>
                                                            <asp:TableCell VerticalAlign="Top" Width="70%">
                                                                В компании работают
                                                                <asp:Label runat="server" ID="lblStaffCount" />
                                                                сотрудников
                                                                <br />
                                                                <asp:Repeater ID="Repeater4" runat="server" DataSourceID="sqlCompanyDept">
                                                                    <ItemTemplate>
                                                                        <br />
                                                                        <img src="Images/green_check.png" alt="" />
                                                                        <asp:Label ID="Label7" Text='<%# "Отдел \""+Eval("dept_name") +"\" " +Eval("emp_count") +" человек" %>'
                                                                            runat="server" />
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                            </asp:TableCell>
                                                            <asp:TableCell VerticalAlign="Bottom" Width="30%">
                                                                <img src="Images/newUser.png" alt="Новый сотрудник"/><a href="Admin/UserAdd.aspx" style="margin: 5px 10px 15px 10px;">Новый сотрудник</a>
                                                            </asp:TableCell>
                                                        </asp:TableRow>
                                                    </asp:Table>
                                                </dx:PanelContent>
                                            </PanelCollection>
                                        </dx:ASPxPanel>
                                    </dx:PanelContent>
                                </PanelCollection>
                            </dx:ASPxRoundPanel>
                        </asp:TableCell>
                    </asp:TableRow>
                    <%-- графики --%>
                    <asp:TableRow HorizontalAlign="Center">
                        <asp:TableCell VerticalAlign="Top">
                            <dx:ASPxRoundPanel ID="ASPxRoundPanel8" runat="server" Width="100%" Theme="Office2010Silver">
                                <HeaderTemplate>
                                    <asp:Table ID="Table7" runat="server" Width="100%">
                                        <asp:TableRow>
                                            <%--<asp:TableCell HorizontalAlign="Left" Width="45">
                                                <asp:Image ID="Image6" ImageUrl="Images/sotrGaugePanel.png" runat="server" alt="" Width="45" Height="30" style="padding: 0px; margin: -5px;"/>
                                            </asp:TableCell>--%>
                                            <asp:TableCell HorizontalAlign="Left">
                                                <asp:Label ID="Label8" runat="server" Text='Показатели' Font-Size="10pt"/>
                                            </asp:TableCell>
                                            <asp:TableCell HorizontalAlign="Right">
                                                    <asp:ImageButton ID="ImageButton8" runat="server" BackColor="Transparent" BorderStyle="None" style="padding: 0px; margin: 0px; background: none; border: none;"
                                                    ImageUrl="~/Images/collapse.png" Width="20px" Height="20px" ToolTip="Свернуть/Развернуть"  
                                                    OnClientClick="sotrGaugePanel.SetVisible(!sotrGaugePanel.GetVisible()); return false;"/>
                                            </asp:TableCell>
                                        </asp:TableRow>
                                    </asp:Table>
                                </HeaderTemplate>
                                <PanelCollection>
                                    <dx:PanelContent>
                                        <dx:ASPxPanel CssClass="companyPanelContent" ID="ASPxPanel9" runat="server" Width="100%" ClientInstanceName="sotrGaugePanel">
                                            <PanelCollection>
                                                <dx:PanelContent ID="PanelContent2" runat="server">
                                                    <asp:Chart ID="ChartCompanyDept" runat="server" Height="350px" Width="700px" BackColor="Transparent"
                                                        Style="background: none;">
                                                        <Series>
                                                            <asp:Series ChartType="Doughnut"
                                                                CustomProperties="PieLabelStyle = Outside" XValueMember="DeptName" YValueMembers="EmpCount"
                                                                IsValueShownAsLabel="true" LabelFormat="{0} ч." Legend="Legend1" BorderColor="180, 26, 59, 105">
                                                            </asp:Series>
                                                        </Series>
                                                        <ChartAreas>
                                                            <asp:ChartArea Name="ChartArea1" BackColor="Transparent" >
                                                            </asp:ChartArea>
                                                        </ChartAreas>
                                                        <Legends>
                                                            <asp:Legend Name="Legend1"/>
                                                        </Legends>
                                                    </asp:Chart>
                                                    <%--<asp:Chart ID="ChartCompanyJob" runat="server" Height="350px" Width="700px" BackColor="Transparent">
                                                        <Series>
                                                            <asp:Series CustomProperties="PieLabelStyle = Outside" 
                                                                ChartType="Doughnut" XValueMember="JobName" YValueMembers="EmpCount"
                                                                IsValueShownAsLabel="true" LabelFormat="{0} ч." Legend="Legend1" BorderColor="180, 26, 59, 105" />
                                                        </Series>
                                                        <ChartAreas>
                                                            <asp:ChartArea Name="ChartArea1" BackColor="Transparent">
                                                            </asp:ChartArea>
                                                        </ChartAreas>
                                                        <Legends>
                                                            <asp:Legend Name="Legend1" />
                                                        </Legends>
                                                    </asp:Chart>--%>
                                                </dx:PanelContent>
                                            </PanelCollection>
                                        </dx:ASPxPanel>
                                    </dx:PanelContent>
                                </PanelCollection>
                            </dx:ASPxRoundPanel>
                        </asp:TableCell>
                    </asp:TableRow>
                    <%-- таблица --%>
                    <asp:TableRow HorizontalAlign="Center">
                        <asp:TableCell VerticalAlign="Top">
                            <dx:ASPxRoundPanel ID="ASPxRoundPanel9" runat="server" Width="100%" Theme="Office2010Silver">
                                <HeaderTemplate>
                                    <asp:Table ID="Table9" runat="server" Width="100%">
                                        <asp:TableRow>
                                            <%--<asp:TableCell HorizontalAlign="Left" Width="45">
                                                <asp:Image ID="Image7" ImageUrl="Images/sotrPersonsListPanel.png" runat="server" alt="" Width="45" Height="30" style="padding: 0px; margin: -5px;"/>
                                            </asp:TableCell>--%>
                                            <asp:TableCell HorizontalAlign="Left">
                                                <asp:Label ID="Label9" runat="server" Text='<%$Resources: GlobalRes, lk_emp_1%>' Font-Size="10pt"/>
                                            </asp:TableCell>
                                            <asp:TableCell HorizontalAlign="Right">
                                                    <asp:ImageButton ID="ImageButton9" runat="server" BackColor="Transparent" BorderStyle="None" style="padding: 0px; margin: 0px; background: none; border: none;"
                                                    ImageUrl="~/Images/collapse.png" Width="20px" Height="20px" ToolTip="Свернуть/Развернуть"  
                                                    OnClientClick="sotrPersonsListPanel.SetVisible(!sotrPersonsListPanel.GetVisible()); return false;"/>
                                            </asp:TableCell>
                                        </asp:TableRow>
                                    </asp:Table>
                                </HeaderTemplate>
                                <PanelCollection>
                                    <dx:PanelContent>
                                        <dx:ASPxPanel CssClass="companyPanelContent" ID="ASPxPanel10" runat="server" Width="100%" ClientInstanceName="sotrPersonsListPanel">
                                            <PanelCollection>
                                                <dx:PanelContent ID="PanelContent14" runat="server">
                                                    <dx:ASPxGridView ID="dxgEmp" runat="server" DataSourceID="sqlUsers" KeyFieldName="iduser" SettingsPager-PageSize="30"
                                                        AutoGenerateColumns="false" Width="100%" Theme="Office2010Silver">
                                                        <Columns>
                                                            <dx:GridViewDataHyperLinkColumn FieldName="iduser" Caption="ФИО" Width="35%" Settings-AllowHeaderFilter="False"
                                                                PropertiesHyperLinkEdit-NavigateUrlFormatString="~/Group/UserInfo.aspx?id={0}"
                                                                PropertiesHyperLinkEdit-TextField="fio">
                                                            </dx:GridViewDataHyperLinkColumn>
                                                            <dx:GridViewDataColumn FieldName="dept_name" Caption="Отдел" Width="20%" Settings-HeaderFilterMode="CheckedList" />
                                                            <dx:GridViewDataColumn FieldName="job_name" Caption="Должность" Width="20%" Settings-HeaderFilterMode="CheckedList" />
                                                            <dx:GridViewDataColumn FieldName="state_name" Caption="Статус" Width="20%" Settings-HeaderFilterMode="CheckedList" />
                                                            <dx:GridViewDataHyperLinkColumn FieldName="iduser" Caption="" Settings-AllowHeaderFilter="False"
                                                                PropertiesHyperLinkEdit-NavigateUrlFormatString="~/Admin/UserEdit.aspx?id={0}"
                                                                PropertiesHyperLinkEdit-Text="Редактор">
                                                            </dx:GridViewDataHyperLinkColumn>
                                                            <dx:GridViewCommandColumn DeleteButton-Image-Url="~/Images/delete_small.ico" DeleteButton-Visible="true"
                                                                VisibleIndex="5" ButtonType="Image">
                                                            </dx:GridViewCommandColumn>
                                                        </Columns>
                                                        <Settings ShowHeaderFilterButton="true" ShowFooter="true" />
                                                        <SettingsPopup>
                                                            <HeaderFilter Height="200" />
                                                        </SettingsPopup>
                                                        <SettingsBehavior ConfirmDelete="true" />
                                                    </dx:ASPxGridView>
                                                </dx:PanelContent>
                                            </PanelCollection>
                                        </dx:ASPxPanel>
                                    </dx:PanelContent>
                                </PanelCollection>
                            </dx:ASPxRoundPanel>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>    
            </div>

            <%--Компания--%>
            <div id="companyPage" class="tab-content clearfix" style="display: none;">
                <asp:Table ID="Table1" runat="server" Style="width: 100%;">
                    <%-- Активные угрозы --%>
                    <asp:TableRow HorizontalAlign="Center">
                        <asp:TableCell VerticalAlign="Top">
                            <dx:ASPxRoundPanel ID="ASPxRoundPanel2" runat="server" Width="100%" Theme="Office2010Silver">
                                <HeaderTemplate>
                                    <asp:Table ID="Table10" runat="server" Width="100%">
                                        <asp:TableRow>
                                            <%--<asp:TableCell HorizontalAlign="Left" Width="45">
                                                <asp:Image ID="Image1" ImageUrl="Images/compActivePanel.png" runat="server" alt=""
                                                    Width="45" Height="30" Style="padding: 0px; margin: -5px;" />
                                            </asp:TableCell>--%>
                                            <asp:TableCell HorizontalAlign="Left">
                                                <asp:Label ID="Label19" runat="server" Text='<%$Resources: GlobalRes, lk_section1%>' Font-Size="10pt"/>
                                            </asp:TableCell>
                                            <asp:TableCell HorizontalAlign="Right">
                                                <asp:ImageButton ID="ImageButton17" runat="server" BackColor="Transparent" BorderStyle="None"
                                                    Style="padding: 0px; margin: 0px; background: none; border: none;" ImageUrl="~/Images/collapse.png"
                                                    Width="20px" Height="20px" ToolTip="Свернуть/Развернуть" OnClientClick="compActivePanel.SetVisible(!compActivePanel.GetVisible()); return false;" />
                                            </asp:TableCell>
                                        </asp:TableRow>
                                    </asp:Table>
                                </HeaderTemplate>
                                <PanelCollection>
                                    <dx:PanelContent ID="PanelContent3" runat="server">
                                        <dx:ASPxPanel CssClass="companyPanelContent" ID="ASPxPanel2" runat="server" ClientInstanceName="compActivePanel">
                                            <PanelCollection>
                                                <dx:PanelContent ID="PanelContent4" runat="server">
                                                    <%-- тут динамическое создание в табличку --%>
                                                    <asp:Table runat="server" Width="100%" ID="companyActiveTable" />
                                                </dx:PanelContent>
                                            </PanelCollection>
                                        </dx:ASPxPanel>
                                    </dx:PanelContent>
                                </PanelCollection>
                            </dx:ASPxRoundPanel>
                        </asp:TableCell>
                    </asp:TableRow>
                    <%-- Потенциалы --%>
                    <asp:TableRow HorizontalAlign="Center">
                        <asp:TableCell VerticalAlign="Top">
                            <dx:ASPxRoundPanel ID="ASPxRoundPanel3" runat="server" Width="100%" Theme="Office2010Silver">
                                <HeaderTemplate>
                                    <asp:Table ID="Table11" runat="server" Width="100%">
                                        <asp:TableRow>
                                            <%--<asp:TableCell HorizontalAlign="Left" Width="45">
                                                <asp:Image ID="Image3" ImageUrl="Images/compPotencPanel.png" runat="server" alt=""
                                                    Width="45" Height="30" Style="padding: 0px; margin: -5px;" />
                                            </asp:TableCell>--%>
                                            <asp:TableCell HorizontalAlign="Left">
                                                <asp:Label ID="Label20" runat="server" Text='<%$Resources: GlobalRes, lk_section2%>'
                                                    Font-Size="10pt"/>
                                            </asp:TableCell>
                                            <asp:TableCell HorizontalAlign="Right">
                                                <asp:ImageButton ID="ImageButton18" runat="server" BackColor="Transparent" BorderStyle="None"
                                                    Style="padding: 0px; margin: 0px; background: none; border: none;" ImageUrl="~/Images/collapse.png"
                                                    Width="20px" Height="20px" ToolTip="Свернуть/Развернуть" OnClientClick="compPotencPanel.SetVisible(!compPotencPanel.GetVisible()); return false;" />
                                            </asp:TableCell>
                                        </asp:TableRow>
                                    </asp:Table>
                                </HeaderTemplate>
                                <PanelCollection>
                                    <dx:PanelContent ID="PanelContent5" runat="server">
                                        <dx:ASPxPanel CssClass="companyPanelContent" ID="ASPxPanel3" runat="server" Width="100%" ClientInstanceName="compPotencPanel">
                                            <PanelCollection>
                                                <dx:PanelContent ID="PanelContent6" runat="server">
                                                    <%-- тут динамическое создание в табличку --%>
                                                    <asp:Table runat="server" Width="100%" ID="companyPotencTable" />
                                                </dx:PanelContent>
                                            </PanelCollection>
                                        </dx:ASPxPanel>
                                    </dx:PanelContent>
                                </PanelCollection>
                            </dx:ASPxRoundPanel>
                        </asp:TableCell>
                    </asp:TableRow>
                    <%-- Мониторинг персонала --%>
                    <asp:TableRow HorizontalAlign="Center">
                        <asp:TableCell VerticalAlign="Top">
                            <dx:ASPxRoundPanel ID="ASPxRoundPanel4" runat="server" Width="100%" Theme="Office2010Silver">
                                <HeaderTemplate>
                                    <asp:Table ID="Table12" runat="server" Width="100%">
                                        <asp:TableRow>
                                            <%--<asp:TableCell HorizontalAlign="Left" Width="45">
                                                <asp:Image ID="Image4" ImageUrl="Images/compMonitPanel.png" runat="server" alt=""
                                                    Width="45" Height="30" Style="padding: 0px; margin: -5px;" />
                                            </asp:TableCell>--%>
                                            <asp:TableCell HorizontalAlign="Left">
                                                <asp:Label ID="Label21" runat="server" Text='<%$Resources: GlobalRes, lk_section3%>'
                                                    Font-Size="10pt"/>
                                            </asp:TableCell>
                                            <asp:TableCell HorizontalAlign="Right">
                                                <asp:ImageButton ID="ImageButton19" runat="server" BackColor="Transparent" BorderStyle="None"
                                                    Style="padding: 0px; margin: 0px; background: none; border: none;" ImageUrl="~/Images/collapse.png"
                                                    Width="20px" Height="20px" ToolTip="Свернуть/Развернуть" OnClientClick="compMonitPanel.SetVisible(!compMonitPanel.GetVisible()); return false;" />
                                            </asp:TableCell>
                                        </asp:TableRow>
                                    </asp:Table>
                                </HeaderTemplate>
                                <PanelCollection>
                                    <dx:PanelContent ID="PanelContent7" runat="server">
                                        <dx:ASPxPanel CssClass="companyPanelContent" ID="ASPxPanel4" runat="server" Width="100%" ClientInstanceName="compMonitPanel">
                                            <PanelCollection>
                                                <dx:PanelContent ID="PanelContent8" runat="server">
                                                    <asp:Table ID="Table13" runat="server" Width="100%">
                                                        <asp:TableRow>
                                                            <asp:TableCell>
                                                                <asp:Repeater ID="Repeater9" runat="server" DataSourceID="sqlMonitoring">
                                                                    <ItemTemplate>
                                                                        <img src="Images/rep_ico.png" alt="" width="20" height="21" style="margin-top: 8px;
                                                                            margin-bottom: -8px;" />
                                                                        <asp:HyperLink ID="HyperLink6" runat="server" Text='<%# Eval("monitoring_area") %>'
                                                                            NavigateUrl='<%# "~/Analyse/Common.aspx?ma=" +Eval("monitoring_area") %>' Style="margin: 0px 15px 10px 0px;" />
                                                                        <br />
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                            </asp:TableCell>
                                                        </asp:TableRow>
                                                    </asp:Table>
                                                </dx:PanelContent>
                                            </PanelCollection>
                                        </dx:ASPxPanel>
                                    </dx:PanelContent>
                                </PanelCollection>
                            </dx:ASPxRoundPanel>
                        </asp:TableCell>
                    </asp:TableRow>
                    <%-- Индикаторы --%>
                    <asp:TableRow HorizontalAlign="Center">
                        <asp:TableCell VerticalAlign="Top">
                            <dx:ASPxRoundPanel ID="ASPxRoundPanel5" runat="server" Width="100%" Theme="Office2010Silver">
                                <HeaderTemplate>
                                    <asp:Table ID="Table14" runat="server" Width="100%">
                                        <asp:TableRow>
                                            <%--<asp:TableCell HorizontalAlign="Left" Width="45">
                                                <asp:Image ID="Image1" ImageUrl="Images/compIndicatorPanel.png" runat="server" alt=""
                                                    Width="45" Height="30" Style="padding: 0px; margin: -5px;" />
                                            </asp:TableCell>--%>
                                            <asp:TableCell HorizontalAlign="Left">
                                                <asp:Label ID="Label22" runat="server" Text='<%$Resources: GlobalRes, lk_section4%>'
                                                    Font-Size="10pt" />
                                            </asp:TableCell>
                                            <asp:TableCell HorizontalAlign="Right">
                                                <asp:ImageButton ID="ImageButton20" runat="server" BackColor="Transparent" BorderStyle="None"
                                                    Style="padding: 0px; margin: 0px; background: none; border: none;" ImageUrl="~/Images/collapse.png"
                                                    Width="20px" Height="20px" ToolTip="Свернуть/Развернуть" OnClientClick="compIndicatorPanel.SetVisible(!compIndicatorPanel.GetVisible()); return false;" />
                                            </asp:TableCell>
                                        </asp:TableRow>
                                    </asp:Table>
                                </HeaderTemplate>
                                <PanelCollection>
                                    <dx:PanelContent ID="PanelContent9" runat="server">
                                        <dx:ASPxPanel CssClass="companyPanelContent" ID="ASPxPanel5" runat="server" Width="100%" ClientInstanceName="compIndicatorPanel">
                                            <PanelCollection>
                                                <dx:PanelContent ID="PanelContent10" runat="server">
                                                    <asp:Table ID="Table15" runat="server" Width="100%">
                                                        <asp:TableRow>
                                                            <asp:TableCell ID="compIndicatorData"> 
                                                                <%--В компании открыты
                                                                <asp:Label runat="server" ID="lblVacancyCount" />
                                                                вакансии<br />
                                                                На рассмотрении
                                                                <asp:Label runat="server" ID="lblKandidatCount" />
                                                                находятся кандидатов<br />
                                                                <asp:Label runat="server" ID="lblIspitatCount" />
                                                                сотрудников находятся на испытательном сроке--%>
                                                            </asp:TableCell>
                                                        </asp:TableRow>
                                                    </asp:Table>
                                                </dx:PanelContent>
                                            </PanelCollection>
                                        </dx:ASPxPanel>
                                    </dx:PanelContent>
                                </PanelCollection>
                            </dx:ASPxRoundPanel>
                        </asp:TableCell>
                    </asp:TableRow>
                    <%-- Исследования --%>
                    <asp:TableRow HorizontalAlign="Center">
                        <asp:TableCell VerticalAlign="Top">
                            <dx:ASPxRoundPanel ID="ASPxRoundPanel6" runat="server" Width="100%" Theme="Office2010Silver">
                                <HeaderTemplate>
                                    <asp:Table ID="Table16" runat="server" Width="100%">
                                        <asp:TableRow>
                                            <%--<asp:TableCell HorizontalAlign="Left" Width="45">
                                                <asp:Image ID="Image1" ImageUrl="Images/compIssledPanel.png" runat="server" alt=""
                                                    Width="45" Height="30" Style="padding: 0px; margin: -5px;" />
                                            </asp:TableCell>--%>
                                            <asp:TableCell HorizontalAlign="Left">
                                                <asp:Label ID="Label23" runat="server" Text='<%$Resources: GlobalRes, lk_section5%>'
                                                    Font-Size="10pt" />
                                            </asp:TableCell>
                                            <asp:TableCell HorizontalAlign="Right">
                                                <asp:ImageButton ID="ImageButton21" runat="server" BackColor="Transparent" BorderStyle="None"
                                                    Style="padding: 0px; margin: 0px; background: none; border: none;" ImageUrl="~/Images/collapse.png"
                                                    Width="20px" Height="20px" ToolTip="Свернуть/Развернуть" OnClientClick="compIssledPanel.SetVisible(!compIssledPanel.GetVisible()); return false;" />
                                            </asp:TableCell>
                                        </asp:TableRow>
                                    </asp:Table>
                                </HeaderTemplate>
                                <PanelCollection>
                                    <dx:PanelContent ID="PanelContent11" runat="server">
                                        <dx:ASPxPanel CssClass="companyPanelContent" ID="ASPxPanel6" runat="server" Width="100%" ClientInstanceName="compIssledPanel">
                                            <PanelCollection>
                                                <dx:PanelContent ID="PanelContent12" runat="server">
                                                    <asp:Table ID="Table17" runat="server" Width="100%">
                                                        <asp:TableRow>
                                                            <asp:TableCell>
                                                                <asp:Label ID="Label24" runat="server" Text='<%$Resources: GlobalRes, lk_section5_1%>' />
                                                                <asp:Repeater ID="Repeater5" runat="server" DataSourceID="SqlCurrExamination">
                                                                    <ItemTemplate>
                                                                        <br />
                                                                        <asp:HyperLink ID="HyperLink4" NavigateUrl='<%# "~\\Analyse\\Common.aspx?g=" +Eval("id") %>'
                                                                            Text='<%# Eval("name") + " " +Eval("stop_date") %>' runat="server" />
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                                <%--<br>
                                                                Запланированы исследования:
                                                                <asp:Repeater ID="Repeater6" runat="server" DataSourceID="SqlPlanExamination">
                                                                    <ItemTemplate>
                                                                        <br />
                                                                        <asp:HyperLink ID="HyperLink4" NavigateUrl='<%# "~\\Analyse\\Common.aspx?g=" +Eval("id") %>'
                                                                            Text='<%# Eval("name") + " (старт " +Eval("start_date") +")" %>' runat="server"
                                                                            Enabled="false" />
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                                <br />
                                                                Завершенные исследования:
                                                                <asp:Repeater ID="Repeater7" runat="server" DataSourceID="SqlPastExamination">
                                                                    <ItemTemplate>
                                                                        <br />
                                                                        <asp:HyperLink ID="HyperLink4" NavigateUrl='<%# "~\\Analyse\\Common.aspx?g=" +Eval("id") %>'
                                                                            Text='<%# Eval("name") %>' runat="server" />
                                                                    </ItemTemplate>
                                                                </asp:Repeater>--%>
                                                            </asp:TableCell>
                                                        </asp:TableRow>
                                                    </asp:Table>
                                                </dx:PanelContent>
                                            </PanelCollection>
                                        </dx:ASPxPanel>
                                    </dx:PanelContent>
                                </PanelCollection>
                            </dx:ASPxRoundPanel>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </div>

            <!-- Задачи -->
            <div id="Tests" class="tab-content clearfix" style="display: none;">
                <asp:Table ID="Table18" runat="server" Style="width: 100%;">
                    <asp:TableRow>
                        <asp:TableCell VerticalAlign="Top">
                            <dl class="icons">
                                <asp:Repeater runat="server" DataSourceID="SubjectAuto_lds">
                                    <ItemTemplate>
                                        <dt><i class="icon-star"></i>
                                        <asp:HyperLink runat="server" 
                                            NavigateUrl='<%# "~/Player/testtrack.aspx?s="+Eval("ID") %>'
                                            Text='<%# Eval("groupname") %>'>
                                        </asp:HyperLink><dt />
                                        <hr class="alt2">
                                    </ItemTemplate>
                                </asp:Repeater>
                                
                                <asp:Repeater ID="Repeater6" runat="server" DataSourceID="Subject_lds">
                                    <ItemTemplate>
                                        <dt><i class="icon-ok"></i>
                                            <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl='<%# "~/Player/testtrack.aspx?s="+Eval("ID") %>'
                                                Text='<%# Eval("groupname") %>'>
                                            </asp:HyperLink><dt />
                                            <hr class="alt2">
                                    </ItemTemplate>
                                </asp:Repeater>

                                <asp:Repeater runat="server" DataSourceID="NewIdeas_lds">
                                    <ItemTemplate>
                                        <dt><i class="icon-question-sign"></i>
                                            <asp:HyperLink runat="server" 
                                                NavigateUrl='<%# "~/Group/idea.aspx?id="+Eval("ID") %>'
                                                Text='<%# "Оценить идею "+Eval("fio")+" от "+Eval("test_date") %>'>
                                            </asp:HyperLink><dt />
                                            <hr class="alt2">
                                    </ItemTemplate>
                                </asp:Repeater>

                                <asp:Repeater runat="server" DataSourceID="ApprovedSubject_lds">
                                    <ItemTemplate>
                                        <dt><i class="icon-ok"></i>
                                            <asp:HyperLink runat="server" 
                                                NavigateUrl='<%# "~/Group/ansall.aspx?ap="+Eval("ID") %>'
                                                Text='<%# "Утвердить оценку "+Eval("fio") %>'>
                                            </asp:HyperLink><dt />
                                            <hr class="alt2">
                                    </ItemTemplate>
                                </asp:Repeater>
                            </dl>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </div>

            <%--MyProfile--%>
            <div id="MyProfile"  class="tab-content clearfix" style="display: none;">
                <asp:Table runat="server" Style="width: 100%;">
                    <%-- Индикаторы --%>
                    <asp:TableRow HorizontalAlign="Center">
                        <asp:TableCell VerticalAlign="Top">
                            <dx:ASPxRoundPanel runat="server" Width="100%" Theme="Office2010Silver">
                                <HeaderTemplate>
                                    <asp:Table runat="server" Width="100%">
                                        <asp:TableRow>
                                            <asp:TableCell HorizontalAlign="Left">
                                                <asp:Label runat="server" Text='<%$Resources: GlobalRes, lk_section4%>'
                                                    Font-Size="10pt" />
                                            </asp:TableCell>
                                            <asp:TableCell HorizontalAlign="Right">
                                                <asp:ImageButton ID="ImageButton20" runat="server" BackColor="Transparent" BorderStyle="None"
                                                    Style="padding: 0px; margin: 0px; background: none; border: none;" ImageUrl="~/Images/collapse.png"
                                                    Width="20px" Height="20px" ToolTip="Свернуть/Развернуть" 
                                                    />
                                            </asp:TableCell>
                                        </asp:TableRow>
                                    </asp:Table>
                                </HeaderTemplate>
                                <PanelCollection>
                                    <dx:PanelContent runat="server">
                                        <dx:ASPxPanel CssClass="companyPanelContent" runat="server" Width="100%" ClientInstanceName="compPersonalIndicatorPanel">
                                            <PanelCollection>
                                                <dx:PanelContent runat="server">
                                                    <asp:Table runat="server" Width="100%">
                                                        <asp:TableRow>
                                                            <asp:TableCell runat="server" ID="compPersonalIndicatorData"> 
                                                            </asp:TableCell>
                                                        </asp:TableRow>
                                                    </asp:Table>
                                                </dx:PanelContent>
                                            </PanelCollection>
                                        </dx:ASPxPanel>
                                    </dx:PanelContent>
                                </PanelCollection>
                            </dx:ASPxRoundPanel>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </div>

            </asp:TableCell>
        </asp:TableRow>
        



        <asp:TableRow Height="0">
            <asp:TableCell>
                <%--SQL дата--%>
                <asp:SqlDataSource ID="sqlUsers" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>">
                    <SelectParameters>
                        <asp:Parameter Name="idcompany" Type="Int16" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="sqlActiveTreats" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>">
                    <SelectParameters>
                        <asp:Parameter Name="idcompany" DbType="Int32" />
                        <asp:Parameter Name="iddept" DbType="Int32" />
                        <asp:Parameter Name="isHR" DbType="Boolean" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="sqlPotential" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>">
                    <SelectParameters>
                        <asp:Parameter Name="idcompany" DbType="Int32" />
                        <asp:Parameter Name="iddept" DbType="Int32" />
                        <asp:Parameter Name="isHR" DbType="Boolean" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="sqlCompanyDept" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>">
                    <SelectParameters>
                        <asp:Parameter Name="idcompany" DbType="Int32" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlCurrExamination" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>">
                    <SelectParameters>
                        <asp:Parameter Name="idcompany" DbType="Int32" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="sqlMySubject" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>">
                    <SelectParameters>
                        <asp:Parameter Name="iduser" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="Subject_lds" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>">
                    <SelectParameters>
                        <asp:Parameter Name="iduser" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SubjectAuto_lds" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>">
                    <SelectParameters>
                        <asp:Parameter Name="iduser" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="sqlMonitoring" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>">
                    <SelectParameters>
                        <asp:Parameter Name="idcompany" DbType="Int32" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="NewIdeas_lds" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>">
                    <SelectParameters>
                        <asp:Parameter Name="idcompany" DbType="Int32" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="ApprovedSubject_lds" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>">
                    <SelectParameters>
                        <asp:Parameter Name="idUser" DbType="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <%-- HR --%>
                <asp:SqlDataSource ID="SqlHRNowResearch" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>">
                    <SelectParameters>
                        <asp:Parameter Name="idcompany" Type="String" />
                        <asp:Parameter Name="StartDate" Type="String" />
                        <asp:Parameter Name="EndDate" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlHRNowResearchAll" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>">
                    <SelectParameters>
                        <asp:Parameter Name="idcompany" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlHRConstResearchAll" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>">
                    <SelectParameters>
                        <asp:Parameter Name="idcompany" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlHRPlanResearch" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>">
                    <SelectParameters>
                        <asp:Parameter Name="idcompany" DbType="Int32" />
                        <asp:Parameter Name="StartDate" Type="String" />
                        <asp:Parameter Name="EndDate" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlHRMonth" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>">
                    <SelectParameters>
                        <asp:Parameter Name="idcompany" DbType="Int32" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlHRPlanResearchAll" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>">
                    <SelectParameters>
                        <asp:Parameter Name="idcompany" DbType="Int32" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlHRClosedResearch" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>">
                    <SelectParameters>
                        <asp:Parameter Name="idcompany" Type="String" />
                        <asp:Parameter Name="StartDate" Type="String" />
                        <asp:Parameter Name="EndDate" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlHRClosedResearchAll" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>">
                    <SelectParameters>
                        <asp:Parameter Name="idcompany" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Content>
