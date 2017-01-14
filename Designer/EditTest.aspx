<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EditTest.aspx.cs" Inherits="Designer_EditTest"
    MasterPageFile="~/MyMasterPage.master" Title="Редактор теста" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script type="text/javascript" language="javascript">
        function OnConfirmRecalc() {
            if (confirm('Переопределить Т-балл ?')) return true;
            return false;
        }
        function OnConfirmDelete() {
            if (confirm('Удалить строку ?')) return true;
            return false;
        }
        function OnConfirmSubjRecalc() {
            if (confirm('Пересчитать результаты по респондетнам для этого теста ?')) return true;
            return false;
        }
    </script>
    
    <div>
        <ajaxToolkit:ToolkitScriptManager runat="Server" EnablePartialRendering="true" ID="ScriptManager1" />
        <div style="display: inline-block">
        <asp:Menu ID="Menu1" runat="server" BackColor="#B5C7DE" DynamicHorizontalOffset="2"
            Font-Names="Verdana" Font-Size="0.8em" ForeColor="#284E98" Orientation="Horizontal"
            StaticSubMenuIndent="10px">
            <DynamicHoverStyle BackColor="#284E98" ForeColor="White" />
            <DynamicMenuItemStyle HorizontalPadding="5px" VerticalPadding="2px" />
            <DynamicMenuStyle BackColor="#B5C7DE" />
            <DynamicSelectedStyle BackColor="#507CD1" />
            <Items>
                <asp:MenuItem Text="Публикация" Value="Публикация">
                    <asp:MenuItem Text="Опубликовать" Value="Опубликовать"></asp:MenuItem>
                    <asp:MenuItem Text="Убрать из публикации" Value="Убрать из публикации"></asp:MenuItem>
                </asp:MenuItem>
                <asp:MenuItem Text="Блоки" Value="Блоки"></asp:MenuItem>
                <asp:MenuItem Text="Параметры" Value="Параметры"></asp:MenuItem>
                <asp:MenuItem Text="Запустить" Value="Запустить"></asp:MenuItem>
            </Items>
            <StaticHoverStyle BackColor="#284E98" ForeColor="White" />
            <StaticMenuItemStyle HorizontalPadding="5px" VerticalPadding="2px" />
            <StaticSelectedStyle BackColor="#507CD1" />
        </asp:Menu>
        </div>
        <br />
        <br />
        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Designer/TestList.aspx">Перейти к списку тестов</asp:HyperLink>
        <br/>
        <script type="text/javascript">
            function PanelClick(sender, e) {
                var Messages = $get('<%=Messages.ClientID%>');
                Highlight(Messages);
            }

            function ActiveTabChanged(sender, e) {
                var activetabindex = sender.get_activeTab().get_tabIndex();
                document.cookie = "tabIndex=" + escape(activetabindex);                
                
                var CurrentTab = $get('<%=CurrentTab.ClientID%>');
                CurrentTab.innerHTML = sender.get_activeTab().get_headerText();
                Highlight(CurrentTab);

            }

            var HighlightAnimations = {};
            function Highlight(el) {
                if (HighlightAnimations[el.uniqueID] == null) {
                    HighlightAnimations[el.uniqueID] = Sys.Extended.UI.Animation.createAnimation({
                        AnimationName: "color",
                        duration: 0.5,
                        property: "style",
                        propertyKey: "backgroundColor",
                        startValue: "#FFFF90",
                        endValue: "#FFFFFF"
                    }, el);
                }
                HighlightAnimations[el.uniqueID].stop();
                HighlightAnimations[el.uniqueID].play();
            }

            function ToggleHidden(value) {
                $find('<%=Tabs.ClientID%>').get_tabs()[2].set_enabled(value);
            }
        </script>
        
        
        <br />
        <asp:LinqDataSource ID="TestType_LDS" runat="server" ContextTypeName="TesterDataClassesDataContext"
            EntityTypeName="" TableName="Test_Types">
        </asp:LinqDataSource>
        <asp:LinqDataSource ID="ScaleLinqDataSource" runat="server" ContextTypeName="TesterDataClassesDataContext"
            EntityTypeName="" TableName="Scales" Where="test_id == @test_id" EnableUpdate="True"
            EnableDelete="True">
            <WhereParameters>
                <asp:QueryStringParameter Name="test_id" QueryStringField="TestID" Type="Int32" />
            </WhereParameters>
        </asp:LinqDataSource>
        <ajaxToolkit:TabContainer runat="server" ID="Tabs" OnClientActiveTabChanged="ActiveTabChanged"
            ActiveTabIndex="1" Width="100%" OnDemand="True" 
            OnActiveTabChanged="Tabs_ActiveTabChanged">
            <ajaxToolkit:TabPanel runat="server" ID="Panel1" HeaderText="Общее">
                <ContentTemplate>
                    <asp:LinqDataSource ID="TestInfoLinqDataSource" runat="server" ContextTypeName="TesterDataClassesDataContext"
                        EnableUpdate="True" EntityTypeName="" TableName="tests" Where="id == @id">
                        <WhereParameters>
                            <asp:QueryStringParameter DefaultValue="0" Name="id" QueryStringField="TestID" Type="Int32" />
                        </WhereParameters>
                    </asp:LinqDataSource>
                    <br />
                    <asp:DetailsView ID="DetailsView2" runat="server" AutoGenerateRows="False" BackColor="White"
                        BorderColor="#999999" BorderStyle="Double" BorderWidth="3px" CellPadding="3"
                        DataKeyNames="id" DataSourceID="TestInfoLinqDataSource" GridLines="Vertical"
                        Width="100%">
                        <AlternatingRowStyle BackColor="Gainsboro" />
                        <EditRowStyle BackColor="#CCFFCC" Font-Bold="False" ForeColor="Black" />
                        <Fields>
                            <asp:BoundField DataField="id" SortExpression="id" ReadOnly="True" Visible="False" />
                            <asp:BoundField DataField="name" HeaderText="Наименование" SortExpression="name">
                                <ControlStyle Width="90%" />
                            </asp:BoundField>
                            <asp:BoundField DataField="author" HeaderText="Автор" SortExpression="author">
                                <ControlStyle Width="90%" />
                            </asp:BoundField>
                            <asp:BoundField DataField="abbreviature" HeaderText="Аббревиатура" SortExpression="abbreviature">
                                <ControlStyle Width="90%" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Tags" HeaderText="Тэги">
                                <ControlStyle Width="90%" />
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="ИНСТРУКЦИЯ">
                                <ItemTemplate>
                                    <asp:Label ID="Lbl2" runat="server" Text='<%# Bind("instruction") %>'></asp:Label></ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBoxInstruction" runat="server" TextMode="MultiLine" Text='<%# Bind("instruction") %>'
                                        Width="90%">
                                    </asp:TextBox></EditItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="StimulSource" HeaderText="Источник стимульного материала" />
                            <asp:BoundField DataField="Comment" HeaderText="Комментарий" SortExpression="Comment">
                                <ControlStyle Width="90%" />
                            </asp:BoundField>
                            <asp:BoundField HeaderText="Версия" DataField="version_number" />
                            <asp:TemplateField ConvertEmptyStringToNull="False" HeaderText="Тип инструмента"
                                SortExpression="test_type1">
                                <EditItemTemplate>
                                    <asp:DynamicControl ID="DynamicControl1" runat="server" DataField="test_type1" Mode="Edit" />
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblInstrumentType" runat="server" Text='<%# Bind("test_type1.name") %>'></asp:Label></ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="publish_year" HeaderText="Год первой публикации" />
                            <asp:BoundField DataField="publisher" HeaderText="Издатель версии">
                                <ControlStyle Width="90%" />
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="Теоретический конструкт">
                                <ItemTemplate>
                                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("Theory_Construct_Info") %>'></asp:Label></ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox1" runat="server" TextMode="MultiLine" Text='<%# Bind("Theory_Construct_Info") %>'
                                        Width="90%">
                                    </asp:TextBox></EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Информация о надежности">
                                <ItemTemplate>
                                    <asp:Label ID="qqq" runat="server" Text='<%# Bind("reliability_info") %>' /></ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="qqq2" runat="server" TextMode="MultiLine" Text='<%# Bind("reliability_info") %>'
                                        Width="90%" /></EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Информация о валидности">
                                <ItemTemplate>
                                    <asp:Label ID="lbl_validation_info" runat="server" Text='<%# Bind("validation_info") %>' /></ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="tb_validation_info" runat="server" TextMode="MultiLine" Text='<%# Bind("validation_info") %>'
                                        Width="90%" /></EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Стандартизация и тестовые нормы">
                                <ItemTemplate>
                                    <asp:Label ID="lbl_test_norms" runat="server" Text='<%# Bind("test_norms") %>' /></ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="tb_test_norms" runat="server" TextMode="MultiLine" Text='<%# Bind("test_norms") %>'
                                        Width="90%" /></EditItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="jur_law" HeaderText="Юридические права" />
                            <asp:BoundField DataField="develop_history" HeaderText="История разработки" />
                            <asp:BoundField DataField="key_security" HeaderText="Защищенность ключей" />
                            <asp:BoundField DataField="psy_task" HeaderText="Решаемые психодиагностические задачи" />
                            <asp:BoundField DataField="diagnostic_subj" HeaderText="Предмет диагностики" />
                            <asp:BoundField DataField="ITVersionSpecific" HeaderText="Особенности электронной версии" />
                            <asp:TemplateField ConvertEmptyStringToNull="False" HeaderText="Область диагностики"
                                SortExpression="DiagnosticFieldType">
                                <ItemTemplate>
                                    <asp:Label ID="lblDiagnosticFieldType" runat="server" Text='<%# Bind("DiagnosticFieldType.name") %>'></asp:Label></ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DynamicControl ID="DynamicControl2" runat="server" DataField="DiagnosticFieldType"
                                        Mode="Edit" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="social_advisability_idx" HeaderText="индекс социальной желательности" />
                            <asp:BoundField DataField="qualification_demand" HeaderText="требования к квалификации пользователя" />
                            <asp:BoundField DataField="mthd_recomendation" HeaderText="методические рекомендации" />
                            <asp:BoundField DataField="use_restriction" HeaderText="ограничения использования инструмента" />
                            <asp:BoundField DataField="lnk_analog" HeaderText="ссылка на похожие инструменты" />
                            <asp:BoundField DataField="lnk_research" HeaderText="ссылка на примеры исследований" />
                            <asp:BoundField DataField="lnk_FullMethodInfo" HeaderText="ссылка на подробное описание методики" />
                            <asp:BoundField DataField="lnk_DeveloperInfo" HeaderText="ссылка на контакты разработчика" />
                            <asp:BoundField DataField="lnk_SaleMethodic" HeaderText="ссылка на продажу методики" />
                            <asp:TemplateField ConvertEmptyStringToNull="False" HeaderText="Язык" SortExpression="language">
                                <ItemTemplate>
                                    <asp:Label ID="lblLanguage" runat="server" Text='<%# Bind("language.name") %>'></asp:Label></ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DynamicControl ID="DynamicControl3" runat="server" DataField="language" Mode="Edit" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ConvertEmptyStringToNull="True" HeaderText="Категория" SortExpression="test_category">
                                <ItemTemplate>
                                    <asp:Label ID="lblCategory" runat="server" Text='<%# Bind("test_category.name") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DynamicControl ID="dcCat" runat="server" DataField="test_category" Mode="Edit" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Краткое описание теста">
                                <ItemTemplate>
                                    <asp:Label ID="Lbl2" runat="server" Text='<%# Bind("short_description") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="tboxDescription" runat="server" TextMode="MultiLine" Text='<%# Bind("short_description") %>'
                                        Width="90%">
                                    </asp:TextBox></EditItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="monitoring_area" HeaderText="зона контроля"/>
<%--                            <asp:TemplateField HeaderText="is single page">
                                <ItemTemplate>
                                    <asp:CheckBox runat="server" ID="cboxSP" Checked='<%# Bind("issinglepage") %>'></asp:CheckBox>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:CheckBox runat="server" ID="cboxSPE" Checked='<%# Bind("issinglepage") %>'></asp:CheckBox>
                                </EditItemTemplate>
                            </asp:TemplateField>--%>
                            

                            <asp:CommandField ShowEditButton="True" ButtonType="Button" EditText="Редактор" CancelText="Отмена"
                                UpdateText="Готово" />
                        </Fields>
                        <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                        <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                        <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                    </asp:DetailsView>
                
</ContentTemplate>
            
</ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="TabPanel1" runat="server" HeaderText="Пункты" OnDemandMode="Once">
                <HeaderTemplate>Пункты</HeaderTemplate>
                <ContentTemplate>
                    <asp:Label runat="server" Text="Блок" Width="100px"/>
                    <asp:DropDownList
                        ID="BlockDropDownList" runat="server" DataSourceID="BlockLinqDataSource" DataTextField="text"
                        DataValueField="id" AutoPostBack="True" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
                    </asp:DropDownList>
                    <asp:LinqDataSource ID="BlockLinqDataSource" runat="server" ContextTypeName="TesterDataClassesDataContext"
                        EntityTypeName="" Select="new (id, number, text)" TableName="Test_Questions"
                        Where="test_id == @test_id" OnSelected="BlockLinqDataSource_Selected">
                        <WhereParameters>
                            <asp:QueryStringParameter Name="test_id" QueryStringField="TestID" Type="Int32" />
                        </WhereParameters>
                    </asp:LinqDataSource>
                    <asp:LinqDataSource ID="ItemLinqDataSource" runat="server" ContextTypeName="TesterDataClassesDataContext"
                        EnableDelete="True" EnableInsert="True" EnableUpdate="True" EntityTypeName=""
                        TableName="items" Where="group_id == @group_id" OrderBy="number">
                        <WhereParameters>
                            <asp:ControlParameter ControlID="BlockDropDownList" Name="group_id" PropertyName="SelectedValue"
                                Type="Int32" />
                        </WhereParameters>
                    </asp:LinqDataSource>
                    <br />
                    <asp:CheckBox runat="server" ID="cboxIsSinglePage" Text="отображать все вопросы сразу (на одной странице)"/>
                    <asp:Button runat="server" ID="btnSaveIsSinglePage" Text="сохранить" OnClick="OnSaveSinglePage"/>
                    <br /><br />
                    <asp:GridView ID="ItemGridView"
                        runat="server" AutoGenerateColumns="False" DataSourceID="ItemLinqDataSource"
                        Height="16px" EmptyDataText="ничего нет" AllowPaging="True" DataKeyNames="id"
                        OnRowCommand="CommonGridView_RowCommand" ShowHeaderWhenEmpty="True" Width="100%"
                        BackColor="White" BorderColor="#E7E7FF" BorderStyle="None" BorderWidth="1px"
                        CellPadding="3" GridLines="Horizontal" PageSize="25" 
                        onrowediting="ItemGridView_RowEditing" 
                        onrowdeleting="ItemGridView_RowDeleting">
                        <AlternatingRowStyle BackColor="#F7F7F7" />
                        <Columns>
                            <asp:TemplateField HeaderText="№ вопроса" SortExpression="number">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("number") %>' Width="100%"></asp:TextBox></EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("number") %>'></asp:Label></ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="TextBoxItemNumber" runat="server" Width="100%"> </asp:TextBox></FooterTemplate>
                                <ItemStyle Width="20px" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Текст вопроса" SortExpression="text">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("text") %>' Width="100%"></asp:TextBox></EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="TextBoxItemText" runat="server" Width="100%"> </asp:TextBox></FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("text") %>'></asp:Label></ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Пояснение к вопросу" >
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="TextBox33" Width="100%" Text='<%# Bind("description") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="TextBoxDescrText" runat="server" Width="100%"> </asp:TextBox>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label1222" runat="server" Text='<%# Bind("description") %>'></asp:Label></ItemTemplate>
                            </asp:TemplateField>

                            <asp:HyperLinkField NavigateUrl="~/Designer/EditDimension.aspx" DataNavigateUrlFields="ID,Dimension_ID"
                                DataNavigateUrlFormatString="~/Designer/EditDimension.aspx?ItemID={0}&amp;DimID={1}"
                                Text="варианты">
                                <ItemStyle Width="75px" />
                            </asp:HyperLinkField>
                            <asp:TemplateField ShowHeader="False">
                                <EditItemTemplate>
                                    <asp:ImageButton ID="ImageButton3" runat="server"  ImageUrl="~/Images/yes_small.ico" 
                                        CommandName="Update" ToolTip="Принять" />
                                    <asp:ImageButton ID="ImageButton1" runat="server"  ImageUrl="~/Images/cancel_small.ico" 
                                        CommandName="Cancel" ToolTip="Отмена" />
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:ImageButton runat="server" ImageUrl="~/Images/edit_pen.ico"
                                        CommandName="Edit" ToolTip="Редактировать"/>
                                    <asp:ImageButton runat="server"  ImageUrl="~/Images/delete_small.ico" 
                                        CommandName="Delete" ToolTip="Удалить" OnClientClick="if (!OnConfirmDelete()) return false;" />
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:ImageButton ID="ImageButton3" runat="server"  ImageUrl="~/Images/yes_small.ico" 
                                        CommandName="Insert" ToolTip="Принять" />
                                        <asp:ImageButton ID="ImageButton1" runat="server"  ImageUrl="~/Images/cancel_small.ico" 
                                        CommandName="InsertCancel" ToolTip="Отмена" />
                                </FooterTemplate>
                                <ItemStyle Width="100px" />
                            </asp:TemplateField>
                        </Columns>
                        <FooterStyle BackColor="#B5C7DE" ForeColor="#4A3C8C" />
                        <EmptyDataTemplate>
                            <asp:Label Text="никого нет дома" runat="server" />
                        </EmptyDataTemplate>
                        <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#F7F7F7" />
                        <PagerStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" HorizontalAlign="Right" />
                        <RowStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" />
                        <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="#F7F7F7" />
                        <SortedAscendingCellStyle BackColor="#F4F4FD" />
                        <SortedAscendingHeaderStyle BackColor="#5A4C9D" />
                        <SortedDescendingCellStyle BackColor="#D8D8F0" />
                        <SortedDescendingHeaderStyle BackColor="#3E3277" />
                    </asp:GridView>
                    <p/>
                    <asp:CustomValidator ID="CustomValidator1" runat="server" 
                        ErrorMessage="CustomValidator" BackColor="#FF3300" Width="100%">Добавление, редактирование и удаление вопросов запрещено, т.к. существуют ответы респондентов</asp:CustomValidator>
                    </p>
                    <asp:Button ID="Button4" runat="server" OnClick="Button4_Click" Text="Новый" />
                    <asp:Button
                        ID="Button5" runat="server" OnClick="Button5_Click" Text="Вставка" />
                    <asp:Button
                            ID="Button6" runat="server" Enabled="False" OnClick="Button6_Click" Text="Пронумеровать" />
                    
                    <br />
                    <asp:Label Text="Буфер" runat="server"></asp:Label><br />
                    <asp:TextBox ID="boxBuffer" runat="server" Height="61px" TextMode="MultiLine" Width="100%"
                        Wrap="False" BorderColor="#0033CC"></asp:TextBox>
                    
</ContentTemplate>
            
</ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="TabPanel2" runat="server" HeaderText="Шкалы">
                <ContentTemplate>
                    <asp:GridView ID="ScalesGridView" runat="server" AutoGenerateColumns="False" DataSourceID="ScaleLinqDataSource"
                        ShowHeaderWhenEmpty="True" Width="100%" OnRowCommand="ScalesGridView_RowCommand"
                        DataKeyNames="id">
                        <Columns>
                            <asp:TemplateField HeaderText="name" SortExpression="name">
                                <EditItemTemplate>
                                    <asp:TextBox ID="tbName" runat="server" Width="90%" BackColor="LightBlue" Text='<%# Bind("name") %>' /></EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="tbName" runat="server">
                                    </asp:TextBox></FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("name") %>'></asp:Label></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="abreviature" SortExpression="abreviature">
                                <EditItemTemplate>
                                    <asp:TextBox ID="tbAbbr" Width="50" runat="server" Text='<%# Bind("abreviature") %>'></asp:TextBox></EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="tbAbbr" runat="server">
                                    </asp:TextBox></FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("abreviature") %>'></asp:Label></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ConvertEmptyStringToNull="False" HeaderText="Тип расчета Т-балла"
                                SortExpression="ScoreCalcType" ControlStyle-Width="75px">
                                <ItemTemplate>
                                    <asp:Label ID="DynamicControl1" Text='<%# Bind("ScoreCalcType1.name") %>' runat="server">
                                    </asp:Label></ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DynamicControl ID="DynamicControl1" runat="server" DataField="ScoreCalcType1"
                                        Mode="Edit" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Параметр" SortExpression="Param1">
                                <EditItemTemplate>
                                    <asp:DropDownList ID="ParamDDL" runat="server" DataSourceID="Params_SDS" DataTextField="name"
                                        DataValueField="ID" SelectedValue='<%# Bind("Param_ID") %>' />
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblParam" runat="server" Text='<%# Bind("Param1.name") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Параметр 2">
                                <EditItemTemplate>
                                    <asp:DropDownList ID="Param2DDL" runat="server" DataSourceID="Params2_SDS" 
                                    DataTextField="name" DataValueField="ID" SelectedValue='<%# Bind("Param2_ID") %>'/>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblParam2" runat="server" Text='<%# Bind("Param.name") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Formula" HeaderText="Formula" SortExpression="Formula" />
                            <asp:BoundField DataField="AVG_Value" HeaderText="Средн. значение">
                                <ControlStyle Width="50px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Standard_Dev" HeaderText="Ст. отклонение">
                                <ControlStyle Width="50px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="min_value" HeaderText="Мин. Т-балл">
                                <ControlStyle Width="50px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="max_value" HeaderText="Макс. Т-балл">
                                <ControlStyle Width="50px" />
                            </asp:BoundField>
                            <asp:CheckBoxField DataField="isMain" HeaderText="Главная" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="description" ControlStyle-Width="100px" HeaderText="Описание" SortExpression="description" />
                            <asp:TemplateField ShowHeader="False">
                                <EditItemTemplate>
                                    <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="True" CommandName="Update"
                                        Text="Update">
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="LinkButton2" runat="server"
                                            CausesValidation="False" CommandName="Cancel" Text="Cancel">
                                    </asp:LinkButton>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:LinkButton ID="LinkButton11" runat="server" CausesValidation="True" CommandName="Insert"
                                        Text="Insert"></asp:LinkButton><asp:LinkButton ID="LinkButton22" runat="server" CausesValidation="False"
                                            CommandName="InsertCancel" Text="Cancel"></asp:LinkButton></FooterTemplate>
                                <ItemTemplate>
                                    <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Edit"
                                        Text="Edit"></asp:LinkButton><asp:LinkButton ID="LinkButton3" runat="server" CausesValidation="False"
                                            CommandName="Delete" Text="Delete"></asp:LinkButton></ItemTemplate>
                            </asp:TemplateField>
                            <asp:HyperLinkField DataNavigateUrlFields="id" 
                                DataNavigateUrlFormatString="~/Designer/ScaleNorm.aspx?id={0}" 
                                NavigateUrl="~/Designer/ScaleNorm.aspx" Text="Нормы" />
                        </Columns>
                        <EditRowStyle BorderColor="#6600FF" BorderStyle="Solid" />
                    </asp:GridView>
                    <br />
                    <asp:Button ID="btnNewScale" runat="server" OnClick="btnNewScale_Click" Text="Новая" />
                    <asp:SqlDataSource ID="Params_SDS" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>"
                        SelectCommand="SELECT [id], [name] FROM [Params] WHERE ([Test_ID] = @Test_ID) union all select null, '[пусто]'">
                        <SelectParameters>
                            <asp:QueryStringParameter Name="Test_ID" QueryStringField="TestID" Type="Int32" />
                        </SelectParameters>
                    </asp:SqlDataSource>
                    <asp:SqlDataSource ID="Params2_SDS" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>"
                        SelectCommand="SELECT [id], [name] FROM [Params] WHERE ([Test_ID] = @Test_ID) union all select null, '[пусто]'">
                        <SelectParameters>
                            <asp:QueryStringParameter Name="Test_ID" QueryStringField="TestID" Type="Int32" />
                        </SelectParameters>
                    </asp:SqlDataSource>
                
</ContentTemplate>
            
</ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="TabPanel3" runat="server" HeaderText="Ключи">
                <ContentTemplate>
<br /><br /><asp:Panel ID="pnlKeys" runat="server" Height="246px" ScrollBars="Auto" 
                        onload="pnlKeys_Load"></asp:Panel><br /><asp:Button ID="btnAddKeys" runat="server" OnClick="btnAddKeys_Click" Text="Добавить" /><asp:Button ID="btnCopyKF_To" runat="server" Text="Применть к" 
                        OnClick="btnCopyKF_To_Click" Enabled="False" /><asp:TextBox runat="server" ID="NumberListTBox" Enabled="False" />
</ContentTemplate>
            
</ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="TabPanel4" runat="server" HeaderText="Диапазоны" ScrollBars="Auto">
                <ContentTemplate>
<asp:DropDownList ID="ScalesDropDownList" runat="server" DataSourceID="ScaleLinqDataSource"
                        DataTextField="name" DataValueField="id" AutoPostBack="True"></asp:DropDownList>
<br /><br />
                        <asp:GridView ID="RangeGridView" runat="server" AutoGenerateColumns="False" DataSourceID="ScaleRange_LDS"
                        ShowHeaderWhenEmpty="True" Width="100%" DataKeyNames="id" onrowcommand="RangeGridView_RowCommand" 
                        onrowediting="RangeGridView_RowEditing" 
                        onrowupdating="RangeGridView_RowUpdating"><Columns>
<asp:BoundField DataField="Max_Value" HeaderText="Сырой балл (верхняя граница)"></asp:BoundField>
<asp:BoundField DataField="Score" HeaderText="Тестовый бал"/>
<asp:TemplateField  HeaderText="Параметр">
    <EditItemTemplate>
                                    <%--<asp:TextBox runat="server" ID="tb1" Text='<%# Bind("Param_Value_ID") %>'>
                                    </asp:TextBox>--%>
                                    <asp:DropDownList ID="ddlParamValues" runat="server" 
                                        DataSourceID="ParamValues_SQL" DataTextField="str_value" DataValueField="id"
                                        SelectedValue='<%# Bind("Param_Value_ID") %>'>
                                    </asp:DropDownList>
                                
</EditItemTemplate>
<ItemTemplate>
                                    <asp:Label ID="lblParamValue" runat="server" Text="<%# Bind('Param_Value1.str_value') %>">
                                    </asp:Label>
                                
</ItemTemplate>
</asp:TemplateField>
<asp:TemplateField  HeaderText="Параметр 2">
    <EditItemTemplate>
                                    <%--<asp:TextBox runat="server" ID="tb1" Text='<%# Bind("Param2_Value_ID") %>'/>--%>
                                        <asp:DropDownList ID="ddlParamValues2" runat="server" 
                                        DataSourceID="Param2Values_SQL" 
                                        DataTextField="str_value" 
                                        DataValueField="id"
                                        SelectedValue='<%# Bind("Param2_Value_ID") %>'/>

                                
</EditItemTemplate>
<ItemTemplate>
                                    <asp:Label ID="lblParamValue_2" runat="server" Text="<%# Bind('Param_Value.str_value') %>">
                                    </asp:Label>
                                
</ItemTemplate>
</asp:TemplateField>
<asp:TemplateField ShowHeader="False">
    <EditItemTemplate>
                                    <asp:ImageButton ID="ImageButton3" runat="server"  ImageUrl="~/Images/yes_small.ico" 
                                        CommandName="Update" ToolTip="Принять" />
                                    <asp:ImageButton ID="ImageButton1" runat="server"  ImageUrl="~/Images/cancel_small.ico" 
                                        CommandName="Cancel" ToolTip="Отмена" />
                                
</EditItemTemplate>
<ItemTemplate>
                                    <asp:ImageButton ID="ImageButton1" runat="server"  ImageUrl="~/Images/edit_pen.ico" 
                                        CommandName="Edit" ToolTip="Редактировать" />
                                    <asp:ImageButton ID="ImageButton2" runat="server"  ImageUrl="~/Images/delete_small.ico" 
                                        CommandName="Delete" ToolTip="Удалить"/>
                                
</ItemTemplate>
</asp:TemplateField>
</Columns>
</asp:GridView>
<asp:LinqDataSource ID="ScaleRange_LDS" 
                        runat="server" ContextTypeName="TesterDataClassesDataContext"
                        EntityTypeName="" TableName="Scale_Ranges" Where="Scale_ID == @Scale_ID" EnableUpdate="True"
                        EnableDelete="True" onselecting="ScaleRange_LDS_Selecting"><WhereParameters>
<asp:ControlParameter ControlID="ScalesDropDownList" Name="Scale_ID" PropertyName="SelectedValue"
                                Type="Int32" />
</WhereParameters>
</asp:LinqDataSource>

                                
                            <asp:SqlDataSource ID="ParamValues_SQL" runat="server" ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>"
                        SelectCommand="select pv.id, pv.str_value
from param_values pv
inner join scales s on s.param_id=pv.Param_id
where s.id=@ScaleID and s.test_id=@TestID
union all
select null as id, '[пусто]' as str_value"><SelectParameters>
<asp:QueryStringParameter Name="TestID" QueryStringField="TestID" />
<asp:ControlParameter ControlID="ScalesDropDownList" Name="ScaleID" PropertyName="SelectedValue" />
</SelectParameters>
</asp:SqlDataSource>


                            <asp:SqlDataSource ID="Param2Values_SQL" runat="server" 
                            ConnectionString="<%$ ConnectionStrings:tester_dataConnectionString %>"
                        SelectCommand="select pv.id, pv.str_value
from param_values pv
inner join scales s on s.param2_id=pv.Param_id
where s.id= @ScaleID and s.test_id= @TestID
union all
select null as id, '[пусто]' as str_value"
><SelectParameters>
<asp:QueryStringParameter Name="TestID" QueryStringField="TestID" />
<asp:ControlParameter ControlID="ScalesDropDownList" Name="ScaleID" PropertyName="SelectedValue" />
</SelectParameters>
</asp:SqlDataSource>


                                
                                <br />
<asp:Button ID="btnAdd" runat="server" OnClick="btnAdd_Click" Text="Добавить" />
<asp:Button
                        ID="RecalcButton" runat="server" Text="Переопределить" OnClick="RecalcButton_Click"
                        OnClientClick="if (!OnConfirmRecalc()) return false;" />
<asp:Button ID="btnAddRange"
                            runat="server" Text="Добавить диапазон" OnClick="btnAddRange_Click" Enabled="False" />
<asp:TextBox
                                ID="tbNewRange" runat="server" BorderStyle="Solid" BorderWidth="1px" BorderColor="Blue"></asp:TextBox>
<br /><br />
</ContentTemplate>
            
</ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="TabPanel5" runat="server" HeaderText="Интерпретация" OnDemandMode="Once">
                <ContentTemplate>
                <asp:GridView ID="InterGridView" runat="server" 
                        AutoGenerateColumns="False" DataSourceID="InterLinqDataSource"
                        AllowPaging="True" Width="70%"
                        DataKeyNames="id">
                        <Columns>
                            <asp:HyperLinkField DataNavigateUrlFields="id"
                                HeaderText = "Код интерпретации"
                                DataNavigateUrlFormatString="~/Designer/Interpretation.aspx?InterID={0}"
                                DataTextField="id" NavigateUrl="~/Designer/Interpretation.aspx" >
                            
							<ItemStyle HorizontalAlign="Center" />
                            </asp:HyperLinkField>
                            
							<asp:BoundField DataField="idInterKind" HeaderText="Вид интерпретации" >
							    <ItemStyle HorizontalAlign="Center" />
                            </asp:BoundField>
						
                            <asp:CommandField ShowDeleteButton="True" HeaderText="Действие" >
                            <ItemStyle HorizontalAlign="Center" />
                            </asp:CommandField>
                        </Columns>
                </asp:GridView>
                <asp:LinqDataSource ID="InterLinqDataSource" runat="server" ContextTypeName="TesterDataClassesDataContext"
                        EntityTypeName="" TableName="interpretations" Where="test_id == @test_id" EnableDelete="True"><WhereParameters><asp:QueryStringParameter Name="test_id" QueryStringField="TestID" Type="Int32" /></WhereParameters></asp:LinqDataSource><br /><asp:Button ID="btnAddInterpretation" runat="server" OnClick="btnAddInterpretation_Click"
                        Text="Новая" />
                
                    
                
</ContentTemplate>


            
</ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="TabPanel6" runat="server" HeaderText="Диаграммы">
                <ContentTemplate>
                    <asp:GridView ID="DiagramGridView" runat="server" AutoGenerateColumns="False" DataKeyNames="id"
                        DataSourceID="Diagram_LDS" Width="100%" AutoGenerateDeleteButton="True" AutoGenerateEditButton="True"
                        ShowHeaderWhenEmpty="True">
                        <Columns>
                            <asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" SortExpression="ID" />
                            <asp:TemplateField HeaderText="Наименование" SortExpression="name">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox1" runat="server" Width="90%" Text='<%# Bind("name") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:HyperLink runat="server" ID="lnkDiagramName" NavigateUrl='<%# "~/Designer/EditDiagram.aspx?DiagramId="+Eval("id") %>'
                                        Text='<%# Bind("name") %>'>
                                    </asp:HyperLink></ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="TextBox1" runat="server"> 
                                    </asp:TextBox></FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Тип диаграммы" SortExpression="diagram_type">
                                <EditItemTemplate>
                                    <asp:DynamicControl ID="DynamicControl2" runat="server" DataField="diagram_type1"
                                        Mode="Edit" />
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("diagram_type1.name") %>'></asp:Label></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Нижняя граница нормы" SortExpression="NormLow">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("NormLow") %>'></asp:TextBox></EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label3" runat="server" Text='<%# Bind("NormLow") %>'></asp:Label></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Верхняя граница нормы" SortExpression="NormHigh">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("NormHigh") %>'></asp:TextBox></EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label4" runat="server" Text='<%# Bind("NormHigh") %>'></asp:Label></ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EditRowStyle BackColor="#99FF99" />
                    </asp:GridView>
                    <br />
                    <asp:Button ID="btnNewDiagram" runat="server" OnClick="btnNewDiagram_Click" Text="Новая" /><asp:LinqDataSource
                        ID="Diagram_LDS" runat="server" ContextTypeName="TesterDataClassesDataContext"
                        EntityTypeName="" TableName="test_diagrams" Where="test_id == @test_id" EnableDelete="True"
                        EnableUpdate="True">
                        <WhereParameters>
                            <asp:QueryStringParameter Name="test_id" QueryStringField="TestID" Type="Int32" />
                        </WhereParameters>
                    </asp:LinqDataSource>
                </ContentTemplate>
            
</ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="TabPanel7" runat="server" HeaderText="Резюме">
                <ContentTemplate>
                    <asp:GridView ID="ResumeGridView" runat="server" DataSourceID="ResumeLinqDataSource"
                        AutoGenerateColumns="False" Width="100%" OnRowCommand="CommonGridView_RowCommand"
                        AllowPaging="True" DataKeyNames="id">
                        <Columns>
                            <asp:BoundField DataField="ordernumber" HeaderText="№№" />
                            <asp:TemplateField HeaderText="Текст">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("item_text") %>'></asp:TextBox>
                                </EditItemTemplate><FooterTemplate><asp:TextBox ID="tbItem_Text" runat="server" /></FooterTemplate><ItemTemplate><asp:Label ID="Label1" runat="server" Text='<%# Bind("item_text") %>'></asp:Label></ItemTemplate></asp:TemplateField><asp:TemplateField ConvertEmptyStringToNull="False" HeaderText="Тип" SortExpression="resume_item_type1"><EditItemTemplate><asp:DynamicControl ID="DynamicControl1" runat="server" DataField="resume_item_type1"
                                        Mode="Edit" /></EditItemTemplate><FooterTemplate>
                                    <asp:DropDownList runat="server" ID="ddlItem_Type" DataSourceID="ResumeItemType_LDS"
                                        DataTextField="Name" DataValueField="id" /></FooterTemplate>
                            <ItemTemplate><asp:Label ID="DynamicControl33" runat="server" Text='<%# Eval("resume_item_type1.Name") %>'></asp:Label></ItemTemplate></asp:TemplateField><asp:TemplateField ConvertEmptyStringToNull="False" HeaderText="Динамич. шаблон"
                                SortExpression="passport_template"><EditItemTemplate><asp:DynamicControl ID="DynamicControl2" runat="server" DataField="passport_template"
                                        Mode="Edit" /></EditItemTemplate><ItemTemplate><asp:Label ID="DynamicControl3" runat="server" Text='<%# Eval("passport_template.name") %>'></asp:Label></ItemTemplate></asp:TemplateField><asp:BoundField DataField="Diagram_ID" HeaderText="Диаграмма" />
                            <asp:TemplateField ShowHeader="False">
                                        <EditItemTemplate>
                                        <asp:ImageButton ID="ImageButton3" runat="server"  ImageUrl="~/Images/yes_small.ico" 
                                        CommandName="Update" ToolTip="Принять" />
                                        <asp:ImageButton ID="ImageButton1" runat="server"  ImageUrl="~/Images/cancel_small.ico" 
                                        CommandName="Cancel" ToolTip="Отмена" />
                                        </EditItemTemplate>
                            <FooterTemplate>
                                        <asp:ImageButton ID="ImageButton3" runat="server"  ImageUrl="~/Images/yes_small.ico" 
                                        CommandName="Insert" ToolTip="Принять" />
                                        <asp:ImageButton ID="ImageButton1" runat="server"  ImageUrl="~/Images/cancel_small.ico" 
                                        CommandName="InsertCancel" ToolTip="Отмена" />
                            </FooterTemplate>
                            <ItemTemplate>
                                        <asp:ImageButton ID="ImageButton1" runat="server"  ImageUrl="~/Images/edit_pen.ico" 
                                        CommandName="Edit" ToolTip="Редактировать" />
                                        <asp:ImageButton ID="ImageButton2" runat="server"  ImageUrl="~/Images/delete_small.ico" 
                                        CommandName="Delete" ToolTip="Удалить" OnClientClick="if (!OnConfirmDelete()) return false;" />
                            </ItemTemplate>
                            </asp:TemplateField>
                                        </Columns>
                        </asp:GridView>
                                    
                                    <br /><asp:Button ID="btnAddResumeItem" runat="server" OnClick="btnAddResumeItem_Click"
                        Text="Добавить" /><br /><asp:LinqDataSource ID="ResumeLinqDataSource" 
                        runat="server" ContextTypeName="TesterDataClassesDataContext"
                        EntityTypeName="" TableName="Resume_Items" Where="test_id == @test_id" EnableDelete="True"
                        EnableUpdate="True" EnableInsert="True" OrderBy="OrderNumber"><WhereParameters><asp:QueryStringParameter Name="test_id" QueryStringField="TestID" Type="Int32" /></WhereParameters></asp:LinqDataSource><asp:LinqDataSource ID="ResumeItemType_LDS" runat="server" ContextTypeName="TesterDataClassesDataContext"
                        EntityTypeName="" Select="new (id, name)" TableName="Resume_Item_Types"></asp:LinqDataSource>
</ContentTemplate>
            
</ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="TabPanel8" runat="server" HeaderText="Профили">
            </ajaxToolkit:TabPanel>
        </ajaxToolkit:TabContainer>
        <br />
        <asp:Button ID="BlockButton" runat="server" Text="Блоки" OnClick="Button1_Click" />
        <asp:Button ID="ParamsButton" runat="server" Text="Параметры" OnClick="ParamsButton_Click" />
        <br />
        <asp:Label runat="server" ID="CurrentTab" />
        <br />
        <asp:Label runat="server" ID="Messages" />
        <br />
        <asp:Button ID="btnTestTest" runat="server" Text="Запустить" OnClick="btnTestTest_Click"
            BackColor="#CCFFCC" BorderColor="#3366FF" />
        <asp:Button ID="Button7" runat="server" OnClick="Button7_Click" Text="Опубликовать" />
        <asp:Button ID="Button8" runat="server" OnClick="Button8_Click" Text="Убрать из публикации" />
        <asp:Button ID="Button9" runat="server" onclick="Button9_Click" Text="Результаты" BackColor="#99CCFF" />
        <asp:Button runat="server" onclick="btnCSV_Click" Text="CSV" BackColor="#99CCFF" />
        
        <asp:Button ID="Button10" runat="server" BackColor="#FFCC99" Text="Пересчет" 
            ToolTip="Пересчет баллов по респондентам" onclick="Button10_Click" 
            onclientclick="if (!OnConfirmSubjRecalc()) return false;" />
        
        <asp:Button ID="btnCopy" runat="server" onclick="btnCopy_Click" Text="Копия" />
        
        <br />
        <br />
    </div>
</asp:Content>
