<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Common.aspx.cs" Inherits="Analyse_Common" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../CSS/Ulleum.css" rel="stylesheet" type="text/css" />
    <link href="../CSS/style.css" rel="stylesheet" type="text/css" />
    <meta name="robots" content="noindex,nofollow" />
    <title>Ulleum. Групповой отчет</title>
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
    
    
    
    
    <asp:Table ID="Table3" runat="server" Style="width: 100%; height: 100%; margin-top: -2px;">
        <asp:TableRow Height="20px" style="    
            background: #FAF4DC; 
            background: -moz-linear-gradient(top, rgba(250,245,220,1) 0%, rgba(230,160,10,1) 100%); 
            background: -webkit-gradient(linear, left top, left bottom, color-stop(0%,rgba(250,245,220,1) ), color-stop(100%,rgba(230,160,10,1))); 
            background: -webkit-linear-gradient(top, rgba(250,245,220,1)  0%,rgba(230,160,10,1) 100%);
            background: -o-linear-gradient(top, rgba(250,245,220,1)  0%,rgba(230,160,10,1) 100%);
            background: linear-gradient(top, rgba(250,245,220,1)  0%,rgba(230,160,10,1) 100%);">
            <asp:TableCell HorizontalAlign="Left">
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
                            <asp:LoginStatus ID="LoginStatus1" runat="server" LogoutText="Выйти" LogoutImageUrl="../Images/exit.png"
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
                        <li id="Li1" runat="server" style="width: 150px;" ><a href="../">Назад</a></li>
                        <li id="Li2" runat="server" ><a href="../#Tests">Задачи</a></li>
                        <li runat="server" id="liCompanyPage"><a href="../#companyPage">Компания</a></li>
                        <li runat="server" id="liHRProcess"><a href="../#HRProcess">HR процесс</a></li>
			            <li runat="server" id="liEmployess"><a href="../#Employees">Сотрудники</a></li>
			            <li><a href="../#LC">Личный кабинет</a></li>
                        <li class="last"><a href="../#MyProfile">Мой профиль</a></li>
                    </ul>
                </div>
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow >  
            <asp:TableCell style="width: 100%;" ColumnSpan="2" VerticalAlign="Top" HorizontalAlign="Center">
                <div class="tab-content">
                    <asp:Table ID="Table2" runat="server" Style="width: 100%;">
                        <asp:TableRow>
                            <asp:TableCell Width="20px" ></asp:TableCell>
                                <asp:TableCell VerticalAlign="Top" HorizontalAlign="Justify">
                                    <br />
                                    <asp:Label ID="Label5" runat="server" Text="Стандартный групповой отчет по проекту: " style="font-weight: bold;"/>
                                    <asp:Label runat="server" ID="lblProjName" style="font-weight: bold;"/>

                                
                                <div runat="server" id="divInfo">
                                    <b>Компания:</b><asp:Label ID="Label1" runat="server" /><br />
                                    Период тестирования:<asp:Label ID="Label2" runat="server" /><br />
                                    Число участников:<asp:Label ID="Label3" runat="server" /><br />
                                    Название теста:<asp:Label ID="Label4" Font-Bold="true" runat="server" /><br />
                                    <br />
                                </div>
                                <div runat="server" id="divContent">
                                    <h3>
                                        Содержание:</h3>
                                    <a href="#main_test" runat="server" id="ref_main_test">Результаты по тестовому баллу
                                        для основной шкалы</a><br />
                                    <a href="#main_test_job" runat="server" id="ref_main_test_job">Результаты по тестовому
                                        баллу для основной шкалы в разрезе должностей</a><br />
                                    <a href="#main_test_dept" runat="server" id="ref_main_test_dept">Результаты по тестовому
                                        баллу для основной шкалы в разрезе отделов</a><br />
                                    <a href="#summary_all" runat="server" id="ref_summary_all">Основные статистические показатели
                                        по шкалам</a><br />
                                    <a href="#personal_test_all" runat="server" id="ref_personal_test_all">Персональные
                                        результаты (тестовый балл) для всех шкал</a><br />
                                    <a href="#personal_proc_all" runat="server" id="ref_personal_proc_all">Персональные
                                        результаты (процентиль) для всех шкал</a><br />
                                    <a href="#ans_freq" runat="server" id="ref_ans_freq">Частота выбора вариантов</a>
                                    <a href="#measure_obj" runat="server" id="ref_measure_obj">Средние значения по шкалам
                                        (по объектам оценки)</a>
                                </div>
                                <p />
                                <div id="div00" runat="server">
                                    <div id="divIndicator" runat="server">
                                        <br />
                                        <div style="display: inline-block; vertical-align: top; height: 150px; width: 150px;
                                            margin-top: 50px">
                                            <br />
                                            <div id="divCirc" runat="server" style="width: 100px; height: 100px; border-radius: 50px;
                                                font-size: 20px; color: #000; line-height: 100px; text-align: center; background: #ABC;">
                                                75%</div>
                                            <br />
                                            <div id="divLowUser" runat="server" style="display: inline; width: 100px; text-align: center;">
                                                12%</div>
                                        </div>
                                    </div>
                                    <asp:Chart ID="Chart1" runat="server" Height="441px" Width="750px" OnDataBound="Chart1_DataBound">
                                        <Series>
                                            <asp:Series ChartType="Doughnut" CustomProperties="PieLabelStyle = Outside" IsValueShownAsLabel="True" Legend="Legend1" XValueMember="name"
                                                YValueMembers="subj_count" Name="Series1" LabelFormat="{0} чел">
                                            </asp:Series>
                                        </Series>
                                        <ChartAreas>
                                            <asp:ChartArea Name="ChartArea1">
                                            </asp:ChartArea>
                                        </ChartAreas>
                                        <Legends>
                                            <asp:Legend Name="Legend1">
                                            </asp:Legend>
                                        </Legends>
                                        <Titles>
                                            <asp:Title Font="Microsoft Sans Serif, 11.25pt" Name="Title1">
                                            </asp:Title>
                                        </Titles>
                                    </asp:Chart>
                                    
                                    <a name="main_test" />
                                    <asp:Label runat="server" Text="Результаты по тестовому баллу для основной шкалы"
                                        ID="lblGridView1" Visible="false" />
                                    <asp:GridView ID="Gridview1" runat="server" AllowSorting="False" AutoGenerateColumns="False"
                                        EnableSortingAndPagingCallbacks="true" OnSorting="Gridview1_Sorting" OnRowDataBound="Gridview1_RowDataBound">
                                        <Columns>
                                            <asp:BoundField DataField="fio" HeaderText="ФИО" ReadOnly="true" SortExpression="fio" />
                                            <%--<asp:BoundField DataField="rate" ReadOnly="True" SortExpression="rate" HeaderText="Рейтинг">
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>--%>
                                            <asp:BoundField DataField="test_value" HeaderText="Балл" ReadOnly="True" SortExpression="test_value"
                                                DataFormatString="{0:G0}">
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <%--<asp:BoundField DataField="pp" ReadOnly="True" SortExpression="pp" DataFormatString="{0:P0}"
                                                HeaderText="Процентиль">
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>--%>
                                            <asp:BoundField DataField="dept_name" HeaderText="Отдел" ReadOnly="true" SortExpression="dept_name" />
                                            <asp:BoundField DataField="job_name" HeaderText="Должность" ReadOnly="true" SortExpression="job_name" />
                                            <asp:BoundField DataField="test_date" HeaderText="Дата прохождения" ReadOnly="true" SortExpression="test_date" />
                                            <asp:BoundField DataField="number" HeaderText="Кол-во прохождений" ReadOnly="true" SortExpression="number" ItemStyle-HorizontalAlign="Center" />
                                        </Columns>
                                    </asp:GridView>
                                    <p />
                                    
                                    <a name="main_test_job" />
                                    <asp:Panel runat="server" ID="pnlBarJob">
                                        <asp:Label Text="Результаты по тестовому баллу для основной шкалы в разрезе должностей"
                                            runat="server" ID="lblByJob" />
                                        <asp:GridView ID="gridByJob" runat="server" AutoGenerateColumns="False" OnRowDataBound="gridByJob_RowDataBound">
                                            <Columns>
                                                <asp:BoundField DataField="name" HeaderText="Должность" ReadOnly="true" />
                                                <asp:BoundField DataField="rate" ReadOnly="true" HeaderText="Рейтинг">
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="avg_score" HeaderText="Балл" ReadOnly="true" DataFormatString="{0:G0}">
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="avg_pp" HeaderText="Процентиль" ReadOnly="true" DataFormatString="{0:p0}">
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="subj_count" HeaderText="Кол-во" ReadOnly="true">
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:BoundField>
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>
                                    <p />
                                    
                                    <a name="main_test_dept" />
                                    <asp:Panel runat="server" ID="pnlBarDept">
                                        <asp:Label Text="Результаты по тестовому баллу для основной шкалы в разрезе отделов"
                                            runat="server" ID="lblByDept" />
                                        <asp:GridView ID="gridByDept" runat="server" AutoGenerateColumns="False" DataSourceID="sqlDataByDept"
                                            OnRowDataBound="gridByJob_RowDataBound">
                                            <Columns>
                                                <asp:BoundField DataField="name" HeaderText="Должность" ReadOnly="true" />
                                                <asp:BoundField DataField="rate" ReadOnly="True" SortExpression="rate" HeaderText="Рейтинг">
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="avg_score" HeaderText="Балл" ReadOnly="True" DataFormatString="{0:G0}">
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="avg_pp" HeaderText="Процентиль" ReadOnly="True" DataFormatString="{0:p0}">
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="subj_count" HeaderText="Кол-во" ReadOnly="true">
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:BoundField>
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>

                                    <asp:Panel runat="server" ID="pnlDiagramAVG" />
                                    <p />
                                    <a name="summary_all" />
                                    <label runat="server" id="lbl_summary_all" title="Основные статистические показатели по шкалам" />
                                    <asp:GridView ID="Gridview2" runat="server" AllowSorting="True" AutoGenerateColumns="true"
                                        EnableSortingAndPagingCallbacks="true">
                                    </asp:GridView>
                                    
                                    <a name="personal_test_all" />
                                    <asp:Label runat="server" Text="Персональные результаты (тестовый балл) для всех шкал"
                                        ID="lblGridView6" Visible="false" />
                                    <asp:GridView ID="Gridview6" runat="server" AllowSorting="True" AutoGenerateColumns="false"
                                        EnableSortingAndPagingCallbacks="true" OnRowDataBound="gridView6_RowDataBound">
                                        <Columns>
                                            <asp:BoundField DataField="fio" HeaderText="ФИО" ReadOnly="true" SortExpression="fio" />
                                        </Columns>
                                    </asp:GridView>
                                    
                                    <a name="personal_proc_all" />
                                    <asp:Label runat="server" Text="Персональные результаты (процентиль) для всех шкал"
                                        ID="lblGridView7" Visible="false" />
                                    <asp:GridView ID="Gridview7" runat="server" AllowSorting="True" AutoGenerateColumns="false"
                                        OnRowDataBound="Gridview7_RowDataBound">
                                        <Columns>
                                            <asp:BoundField DataField="fio" HeaderText="ФИО" ReadOnly="true" SortExpression="fio" />
                                        </Columns>
                                    </asp:GridView>
                                    
                                    <a name="ans_freq" />
                                    <asp:Label runat="server" Text="Частота выбора вариантов" ID="lblGridViewFreq" Visible="false" />
                                    <dx:ASPxGridView ID="ASPxGridviewFreq" runat="server" KeyFieldName="iduser" AutoGenerateColumns="false" Width="100%" Theme="Office2010Silver">
                                        <Columns>
                                            <dx:GridViewDataColumn FieldName="text" Caption="Вопрос" Width="70%" Settings-HeaderFilterMode="CheckedList" />
                                        </Columns>
                                        <Settings ShowHeaderFilterButton="true" ShowFooter="true" />
                                        <SettingsPopup>
                                            <HeaderFilter Height="200" />
                                        </SettingsPopup>
                                        <SettingsBehavior ConfirmDelete="true" />
                                    </dx:ASPxGridView>
                                    <%--
                                    <asp:GridView ID="GridviewFreq" runat="server" AutoGenerateColumns="false" OnRowDataBound="GridviewFreq_RowDataBound">
                                        <Columns>
                                            <asp:BoundField DataField="text" HeaderText="Вопрос" ReadOnly="true" />
                                        </Columns>
                                    </asp:GridView>
                                    <asp:Label runat="server" ID="lblLegend" />
                                    --%>
                                    <br />
                                    
                                    <asp:GridView ID="GridviewMostPopular" runat="server" AutoGenerateColumns="false">
                                        <Columns>
                                            <asp:BoundField DataField="text" HeaderText="Вопрос" ReadOnly="true" />
                                            <asp:BoundField DataField="name" HeaderText="Ответ" ReadOnly="true" />
                                            <asp:BoundField DataField="freq" HeaderText="Частота выбора" ReadOnly="true" DataFormatString="{0:p0}">
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>
                                        </Columns>
                                    </asp:GridView>
                                    <br />
                                    <a name="measure_obj" />
                                    <asp:Label runat="server" ID="lblMeasureObj" Text="Средние значения по шкалам (по объектам оценки)" />
                                    <asp:GridView ID="gridMeasure" runat="server" AutoGenerateColumns="true" AllowSorting="false"/>
                                    
                                    <a name="raw_data"/>
                                    <asp:Label runat="server" Text="Первичные данные" />
                                    <asp:GridView ID="gridRawData" runat="server" 
                                        AutoGenerateColumns="false" AllowSorting="false" 
                                        RowStyle-BorderWidth="1px">
                                    </asp:GridView>
                                    
                                    <br />
                                    <a href="../lk2.aspx">Перейти к списку исследований</a>
                                </div>
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </div>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow Height="0">
            <asp:TableCell>
                <%--SQL дата--%>
                <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>">
                    <SelectParameters>
                        <asp:Parameter Name="ScaleID" Type="Int32" />
                        <asp:Parameter Name="TestID" Type="Int32" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDiagram2" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>">
                    <SelectParameters>
                        <asp:Parameter Name="ScaleID" Type="Int32" />
                        <asp:Parameter Name="TestID" Type="Int32" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDiagram" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>">
                    <SelectParameters>
                        <asp:Parameter Name="ScaleID" DbType="Int32" />
                        <asp:Parameter Name="TestID" DbType="Int32" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>">
                    <SelectParameters>
                        <asp:Parameter Name="TestID" Type="Int32" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="sqlDataByJob" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>">
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataByDept" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>">
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="sqlFreq" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>">
                    <SelectParameters>
                        <%--<asp:Parameter Name="idGroup" Type="Int32" />--%>
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="sqlMeasure" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>"
                    SelectCommand="">
                    <SelectParameters>
                        <asp:QueryStringParameter DefaultValue="0" Name="GroupID" QueryStringField="g" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="sqlRawData" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>">
                </asp:SqlDataSource>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    </form>
</body>
</html>
