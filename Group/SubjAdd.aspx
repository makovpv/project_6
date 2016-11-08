<%@ Page Title="" Language="C#" MasterPageFile="~/MainBusinessMasterPage.master"
    AutoEventWireup="true" CodeFile="SubjAdd.aspx.cs" Inherits="Group_SubjAdd" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:Table ID="Table1" runat="server" Style="width: 100%; height: 100%; margin-top: -2px; background-repeat: repeat; background-image: url(../Images/sky.jpg);">
        <asp:TableRow Height="20px" style="    
            background: #FAF4DC; 
            background: -moz-linear-gradient(top, rgba(250,245,220,1) 0%, rgba(230,160,10,1) 100%); 
            background: -webkit-gradient(linear, left top, left bottom, color-stop(0%,rgba(250,245,220,1) ), color-stop(100%,rgba(230,160,10,1))); 
            background: -webkit-linear-gradient(top, rgba(250,245,220,1)  0%,rgba(230,160,10,1) 100%);
            background: -o-linear-gradient(top, rgba(250,245,220,1)  0%,rgba(230,160,10,1) 100%);
            background: linear-gradient(top, rgba(250,245,220,1)  0%,rgba(230,160,10,1) 100%);">
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
                            <asp:LoginStatus ID="LoginStatus2" runat="server" LogoutText="Выйти" LogoutImageUrl="../Images/exit.png"
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
            <asp:TableCell style="width: 100%;" ColumnSpan="2" VerticalAlign="Top">
                <div class="tab-content">
                    <br />
                    Проставьте галочки напротив респондентов, которые должны пройти это исследование
                    <p/>
                    <asp:Button runat="server" Text="Выбрать всех активных" onclick="Unnamed1_Click"/>
                    <p/>
                    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
                        DataSourceID="SqlGroupSubject" AllowSorting="True" DataKeyNames="id">
                        <Columns>
                                <asp:TemplateField HeaderText="Выбор">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="cbChecked" runat="server" Checked='<%# Bind("isSel") %>' Enabled="true" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="fio" ReadOnly="true" HeaderText="ФИО" SortExpression="fio"/>
                                <asp:BoundField DataField="dept_name" HeaderText="Отдел" SortExpression="dept_name"/>
                                <asp:BoundField DataField="job_name" HeaderText="Должность" SortExpression="job_name"/>
                                <asp:BoundField DataField="state_name" HeaderText="Статус" SortExpression="state_name" />
                        </Columns>
                        <AlternatingRowStyle BackColor="#d6d6c2"/>
                    </asp:GridView>
                    <p/>
                    Если нужных респондентов нет в списке, перечислите их email, через запятую, в поле ниже
                    <asp:TextBox runat="server" ID="tboxSubjAdd" Width="50%"></asp:TextBox>
                    <p/>
                    <asp:Button ID="Button1" runat="server" Text="OK" onclick="Button1_Click" style="margin-right: 15px;" />
                    <asp:Button ID="Button2" runat="server" Text="Отмена" />
                    <br/>

                </div>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>
                <asp:SqlDataSource ID="SqlGroupSubject" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>" 
                    SelectCommand="select cast (0 as bit) as issel, ua.login_name, ua.fio, d.name as dept_name, j.name as job_name,
                    ua.birth_year, case when ua.gender is null then '' when ua.gender = 1 then 'мужской' when ua.gender = 0 then 'женский' end as gender,
                    s.name as state_name, ua.comment, ua.iduser as id
                    from subject_group sg 
                    inner join user_account ua on ua.idcompany=sg.idcompany
                    left join dept d on d.id=ua.iddept
                    left join job j on j.id=ua.idJob
                    left join user_state s on s.id=ua.idstate
                    where sg.id = @g and s.id not in (1,3,4)">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="g" QueryStringField="g" />
                    </SelectParameters>
                </asp:SqlDataSource>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Content>

