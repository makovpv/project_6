use ulleum_home
go

create table projects (id int not null identity(1,1), name nvarchar(255) not null, code uniqueidentifier not null, StartMessage nvarchar(max), FinalMessage nvarchar(max))
go
alter table projects add constraint pk_projects primary key (id)
go
create unique index [idx_projects_code] on projects (Code)
go

--drop table projects


create table test (id int not null identity (1,1), name nvarchar(255) not null, author nvarchar(255), guid_code uniqueidentifier not null)
go
alter table test add constraint pk_test primary key (id)
go
--drop table test
--------------------------------------
create table ProjectTest_Link (Project_ID int not null, Test_ID int not null, OrdNumber tinyint)
go
alter table ProjectTest_Link add constraint FK_ProjectID foreign key (project_id) references projects (id)
go
alter table ProjectTest_Link add constraint FK_TestID foreign key (test_id) references test (id) on delete cascade
go
--drop table ProjectTest_Link
--------------------------------------

-- БЛОКИ ВОПРОСОВ
create table Test_Question (id int not null identity(1,1), test_id int not null, [text] nvarchar(512), number tinyint, instruction nvarchar(1024), comment nvarchar(512))
go
alter table Test_Question add constraint PK_TestQuestion primary key (id)
go
alter table Test_Question add constraint FK_BlockTestID foreign key (test_id) references test (id) 
go
--drop table test_question

create table Dimension_Mode (id smallint not null, name nvarchar(100))
go
alter table Dimension_Mode add constraint pk_Dimension_Mode primary key (id)
go
--drop table Dimension_Mode

create table Dimension_Type (id tinyint not null, name nvarchar(100), ForItem bit, ForAnket bit)
go
alter table Dimension_Type add constraint pk_Dimension_Type primary key (id)
go
--drop table Dimension_Type

create table Quest_Type (id tinyint not null, name nvarchar(100))
go
alter table Quest_Type add constraint pk_Quest_Type primary key (id)
go
--drop table Quest_Type

create table SubScaleDimension (id int not null identity(1,1), test_id int not null, name nvarchar(255), 
dimension_type tinyint not null, dimension_mode smallint not null, 
GradationCount tinyint, isUniqueSelect bit default 0)
go
alter table SubScaleDimension add constraint pk_SubScaleDimension primary key (id)
go
alter table SubScaleDimension add constraint FK_SubScaleDim_TestID foreign key (test_id) references test (id)
go
alter table SubScaleDimension add constraint FK_SubScaleDim_DimType foreign key (dimension_type) references dimension_type (id)
go
alter table SubScaleDimension add constraint FK_SubScaleDim_DimMode foreign key (dimension_mode) references dimension_mode (id)
go
--drop  table SubScaleDimension

create table SubScales (id int not null identity(1,1), Dimension_ID int not null, name nvarchar(255), OrderNumber tinyint not null default 1)
go
alter table SubScales add constraint PK_SubScales primary key (id)
go
alter table SubScales add constraint FK_SubScales_Dim foreign key (dimension_id) references SubScaleDimension (id)
go
--drop table subscales

create table items (id int not null identity(1,1), [text] nvarchar(255), item_type tinyint not null,
group_id int not null, number smallint, dimension_id int)
go
alter table items add constraint pk_items primary key (id)
go
alter table items add constraint FK_items_ItemType foreign key (item_type) references quest_type (id)
go
alter table items add constraint FK_items_GroupID foreign key (group_id) references test_question (id)
go
alter table items add constraint FK_items_DimensionID foreign key (dimension_id) references SubScaleDimension (id)
go
--drop table items
------------------------------------------------------------------------------------

create table Test_Subject (id int not null identity(1,1), project_id int, Nick_Name nvarchar(255), Test_Date smalldatetime, Age smallint, Gender bit)
go
alter table Test_Subject add constraint PK_Test_Subject primary key (id)
go
alter table Test_Subject add constraint FK_Test_Subject_Project foreign key (project_id) references Projects (id)
go
--drop table test_subject

--drop table Test_Results
create table Test_Results (id int not null identity, Subject_ID int not null, item_id int not null, SubScale_ID int not null, SelectedValue smallint)
go
alter table Test_Results add constraint PK_Test_Results primary key (ID) 
go
alter table Test_Results add constraint FK_Test_Results_SubScale foreign key (SubScale_ID) references SubScales (id)
go

alter table Test_Results add constraint FK_Test_Results_Subject foreign key (subject_ID) references test_subject (id)
go


create table Scales (id int not null identity (1,1), 
test_id int not null,
name nvarchar(255) not null, 
min_value smallint, 
max_value smallint,
abreviature nvarchar(10),
sumtype tinyint not null default 1, 
use_raw_data bit default 0, 
ScoreCalcType tinyint not null default 2, 
kf1 numeric(8,4) not null default 1, 
kf2 numeric(8,4) not null default 1, 
[precision] numeric(8,4), 
[description] ntext )
go
alter table Scales add constraint PK_Scales primary key (id)
go
alter table Scales add constraint FK_Scales_Test foreign key (test_id) references test (id)
go

create table ItemScale_Link (id int not null identity(1,1), item_id int not null, scale_id int not null, subscale_id int not null, kf decimal(8,3) not null default 1)
go
alter table ItemScale_Link add constraint PK_ItemScale_Link primary key (id)
go
alter table ItemScale_Link add constraint FK_ItemScale_Link_item foreign key (item_id) references items (id)
go
alter table ItemScale_Link add constraint FK_ItemScale_Link_scale foreign key (scale_id) references scales (id)
go
alter table ItemScale_Link add constraint FK_ItemScale_Link_SubScale foreign key (subscale_id) references subscales (id)
go


create table interpretation (id int not null identity, test_id int not null, [text] ntext )
go
alter table interpretation add constraint PK_interpretation primary key (id)
go
alter table interpretation add constraint FK_interpretation_test foreign key (test_id) references test (id)
go

create table ConditionRangeType (id tinyint not null, name nvarchar(255))
go
alter table ConditionRangeType add constraint PK_ConditionRangeType primary key (id)
go
insert into ConditionRangeType (id, name) values (1, 'абсолютное значение')
insert into ConditionRangeType (id, name) values (2, 'рейтинг')
go

create table inter_condition (id int not null identity, inter_id int not null, 
scale_id int not null, min_value numeric(6,3), max_value numeric (6,3), range_type tinyint not null default 1)
go
alter table inter_condition add constraint PK_inter_condition primary key (id)
go
alter table inter_condition add constraint FK_inter_condition_inter foreign key (inter_id) references interpretation (id)
go
alter table inter_condition add constraint FK_inter_condition_scale foreign key (scale_id) references scales (id)
go
alter table inter_condition add constraint FK_inter_condition_rangetype foreign key (range_type) references ConditionRangeType (id)
go


create table Resume_Item_Type (id tinyint not null, name nvarchar(255))
go
alter table Resume_Item_Type add constraint PK_Resume_Item_Type primary key (id)
go
insert into Resume_Item_Type (id, name) values (1, 'статичный текст')
insert into Resume_Item_Type (id, name) values (2, 'диаграмма')
insert into Resume_Item_Type (id, name) values (3, 'динамический текст')
insert into Resume_Item_Type (id, name) values (4, 'биографическая информация')
insert into Resume_Item_Type (id, name) values (5, 'изображение')
insert into Resume_Item_Type (id, name) values (6, 'таблица')

insert into Resume_Item_Type (id, name) values (7, 'интерпретация')
insert into Resume_Item_Type (id, name) values (8, 'разделитель')
insert into Resume_Item_Type (id, name) values (9, 'коды ответов')
go


insert into dimension_mode (id,name)
select 0, 'не задано' union all
select 1, 'горизонтально, с полюсами' union all
select 2, 'кружочки' union all
select 3, 'выпадающий список'
go

insert into Dimension_Type (id, name, ForItem, ForAnket)
select 1, 'одиночный выбор', 1, 1 union all
select 2, 'множественный выбор',	1, 1 union all
select 3, 'открытый вопрос',	1, 1 union all
select 7, 'ранжирование',	1, 0 union all
select 5, 'попарное ранжирование',	1, 0 union all
select 6, 'шкалирование',	1, 0 union all
select 8, 'пол', 0,	1 union all
select 9, 'год рождения',	0,	1 union all
select 10, 'диапазон',	0,	0 union all
select 11, 'дата',	0,	1 union all
select 12, 'число',	0,	1
go

insert into Quest_Type (id, name) values (1, 'текст')
insert into Quest_Type (id, name) values (2, 'изображение')
go

create table Diagram_Type (id tinyint not null, name nvarchar(255))
go
alter table Diagram_Type add constraint PK_Diagram_Type primary key (id)
go

insert into Diagram_Type (id, name) values (0, '<без диаграмм>')
insert into Diagram_Type (id, name) values (1, 'столбчатая')
insert into Diagram_Type (id, name) values (2, 'график')
insert into Diagram_Type (id, name) values (3, 'горизонтальная')
insert into Diagram_Type (id, name) values (20, '''паутинка''')
go


create table test_diagram (id int not null identity, test_id int not null, name nvarchar(255),
diagram_type tinyint not null, NormLow numeric (6,2), NormHigh numeric (6,2))
go

alter table test_diagram add constraint PK_test_diagram primary key (id)
go
alter table test_diagram add constraint FK_test_diagram_test foreign key (test_id) references test (id)
go
alter table test_diagram add constraint FK_test_diagram_type foreign key (diagram_type) references diagram_type (id)
go

create table Resume_Items (id int not null identity, test_id int not null, 
OrderNumber tinyint not null,
Resume_Item_Type tinyint not null, item_text ntext, 
scale_id int,
diagram_id int,
is_scale_exclusive bit,
resume_image image)
go
alter table Resume_Items add constraint PK_Resume_Items primary key (id)
go
alter table Resume_Items add constraint FK_Resume_Items_test foreign key (test_id) references test (id)
go
alter table Resume_Items add constraint FK_Resume_Items_type foreign key (Resume_Item_type) references Resume_Item_type (id)
go
alter table Resume_Items add constraint FK_Resume_Items_Scale foreign key (scale_id) references Scales (id)
go
alter table Resume_Items add constraint FK_Resume_Items_diagram foreign key (diagram_id) references test_diagram (id)
go

create table Test_Data (Subject_ID int not null, Scale_ID int not null, Test_Value smallint not null)
go
alter table Test_Data add constraint PK_Test_Data primary key (Subject_ID, Scale_ID)
go
alter table Test_Data add constraint FK_Test_Data_Subj foreign key (Subject_ID) references Test_Subject (id)
go
alter table Test_Data add constraint FK_Test_Scale foreign key (Scale_ID) references Scales (id)
go


alter table Test add abbreviature nvarchar(20), instruction ntext
go


create table Params (id int identity(1,1) not null, Test_ID int not null, name nvarchar(255), Param_Type tinyint not null default 0)
go
alter table Params add constraint PK_Params primary key (ID)
go
alter table Params add constraint FK_Params_Test foreign key (Test_ID) references Test (id)
go

create table Param_Values (id int not null identity(0,1), Param_ID int not null, str_value nvarchar(255) not null, ivalue_1 tinyint, ivalue_2 tinyint)
go
alter table Param_Values add constraint PK_Param_Values primary key (ID)
go

alter table Param_Values add constraint FK_Param_Values_Param foreign key (Param_ID) references Params (id) on delete cascade
go

create table Scale_Range (Scale_ID int not null, Max_Value smallint not null, Score decimal (6,3) not null)
go
alter table Scale_Range add constraint FK_Scale_Range_Scale foreign key (Scale_ID) references Scales (id)
go

alter table Scale_Range add Param_Value_ID int
go
alter table Scale_Range add constraint FK_Scale_Range_Param_Value foreign key (Param_Value_ID) references Param_Values (id)
go
alter table Scale_Range add id int not null identity
go
alter table Scale_Range add constraint PK_Scale_Range primary key (id)
go

alter table Scales add Param_ID int, Formula nvarchar (255)
go
alter table Scales add constraint FK_Scales_Param foreign key (param_id) references Params (id)
go

create table Param_Results (id int not null identity(1,1), Param_Value_ID int not null, Subject_ID int not null)
go
alter table Param_Results add constraint FK_Param_Results_value 
foreign key (Param_Value_ID) references Param_Values (id)
go
alter table Param_Results add constraint FK_Param_Results_Subject
foreign key (Subject_ID) references Test_Subject (id) on delete cascade
go
alter table Param_Results add constraint PK_Param_Results primary key (id)
go

alter table Test_Subject add Test_Id int
go
alter table Test_Subject add constraint FK_TestSubject_Test foreign key (test_id)
references Test (id) on delete cascade
go

alter table Test_Subject add Test_Date_Start smalldatetime
go

create table dbo.Test_Diagram_Scales (id int not null identity, diagram_id int not null, scale_id int not null, group_id int)
go
alter table dbo.Test_Diagram_Scales add constraint PK_Test_Diagram_Scales primary key (id)
go
alter table dbo.Test_Diagram_Scales add constraint FK_Diagram_Scales_Diagram
foreign key (Diagram_ID) references Test_Diagram (id) on delete cascade
go
alter table dbo.Test_Diagram_Scales add constraint FK_Diagram_Scales_Scales
foreign key (Scale_ID) references Scales (id) 
go

create table dbo.Test_Results_Txt (id int not null identity, subject_id int not null, item_id int not null, [text] varchar(max))
go
alter table dbo.Test_Results_Txt add constraint PK_Test_Results_Txt primary key (id)
go
alter table dbo.Test_Results_Txt add constraint FK_Test_Results_Txt_Subject foreign key (subject_id)
references dbo.test_subject (id)
go

alter table dbo.Test add Tags nvarchar(255)
alter table dbo.Test add StimulSource nvarchar(255)
go

alter table dbo.Test add ins_date smalldatetime --not null default getdate()
go

alter table dbo.Test add Comment nvarchar(255)
go

create table dbo.Raw_Data (Subject_ID int not null, Scale_ID int not null, Raw_Value smallint not null)
go
alter table Raw_Data add constraint PK_Raw_Data primary key (Subject_ID, Scale_ID)
go
alter table Raw_Data add constraint FK_Raw_Data_Subj foreign key (Subject_ID) references Test_Subject (id)
go
alter table Raw_Data add constraint FK_Raw_Data_Scale foreign key (Scale_ID) references Scales (id)
go

alter table dbo.test add version_number nvarchar(20), 
test_type tinyint,				-- Тип инструмента (анкета, опросник, стандартизированный тест, ...)
publish_year smallint,			-- Год первой публикации
publisher nvarchar(255),		-- Издатель версии
Theory_Construct_Info ntext,	-- Теоретический конструкт
reliability_info ntext,			-- Информация о надежности
validation_info ntext,			-- Информация о валидности
test_norms ntext,				-- Стандартизация и тестовые нормы
jur_law ntext,					-- Юридические права
develop_history ntext,			-- История разработки 
key_security ntext,				-- Защищенность ключей
psy_task ntext,					-- Решаемые психодиагностические задачи
--practic_application			-- практика применения
diagnostic_subj nvarchar(255),	-- предмет диагностики
ITVersionSpecific ntext,		-- Особенности электронной версии
diagnostic_field tinyint,		-- область диагностики
social_advisability_idx ntext,	-- индекс социальной желательности
qualification_demand ntext,		-- требования к квалификации пользователя
mthd_recomendation ntext,		-- методические рекомендации
use_restriction ntext,			-- ограничения использования инструмента

lnk_analog nvarchar(255),
lnk_research nvarchar(255),
lnk_FullMethodInfo nvarchar(255),
lnk_DeveloperInfo nvarchar(255),
lnk_SaleMethodic nvarchar(255)
go
alter table dbo.test add language_type tinyint
go


--drop table dbo.Passport_Template

create table dbo.Passport_Template (id smallint not null identity, template_text ntext)
go
alter table dbo.Passport_Template add constraint PK_Passport_Template primary key (id)
go

--insert into Passport_Template (id) values (1)

alter table dbo.Passport_Template add [name] nvarchar(255), [comment] nvarchar(255)
go
--alter table dbo.Passport_Template alter column id smallint not null identity (2,1)


create table dbo.Test_Type (id tinyint not null, name nvarchar(255))
go
alter table dbo.Test_Type add constraint PK_Test_Type primary key (id)
go
alter table dbo.test add constraint FK_Test_Type foreign key (test_type) references dbo.test_type (id)
go
insert into Test_Type (id, name) values (1, 'анкета')
insert into Test_Type (id, name) values (2, 'опросник')
insert into Test_Type (id, name) values (3, 'стандартизированный тест')
insert into Test_Type (id, name) values (4, 'экспертная методика')
insert into Test_Type (id, name) values (5, 'собеседование')
insert into Test_Type (id, name) values (6, 'интервью')
insert into Test_Type (id, name) values (7, 'контент-анализ')
insert into Test_Type (id, name) values (8, 'приборная методика')
go

--------- VIEWS -----------------------------------------------------------

--drop view dbo.vItemScaleLink

create view dbo.vItemScaleLink as
select distinct i.id as item_id, i.number, i.text, lnk.scale_id
from ItemScale_Link lnk
inner join items i on i.id = lnk.item_id
go

alter table dbo.test add isPublished bit not null default 0
go
--------------------------------------------------
alter table dbo.Resume_Items add txt_template_id smallint 
go
alter table dbo.Resume_Items add constraint FK_Resume_Template foreign key (txt_template_id) references dbo.Passport_Template (id)
go

-------------------------------
alter table dbo.test_diagram_scales add OrderNumber tinyint
go
alter table dbo.scales add max_limit decimal (8,3), min_limit decimal (8,3)
go
-----------------------------

create table dbo.ScoreCalcType (id tinyint not null, name nvarchar(255))
go
alter table dbo.ScoreCalcType add constraint PK_ScoreCalcType primary key (id)
go

insert into dbo.ScoreCalcType (id, name) values (1, 'обычный способ (диапазон)')
insert into dbo.ScoreCalcType (id, name) values (2, 'тестовый равен сырому')
insert into dbo.ScoreCalcType (id, name) values (3, 'проценты')
insert into dbo.ScoreCalcType (id, name) values (4, 'среднее отклонение')

alter table dbo.Scales add constraint FK_Scale_CalcType foreign key (ScoreCalcType) references ScoreCalcType (id)
go

insert into dimension_mode (id, name) values (4, 'интервальная шкала')
insert into dimension_mode (id, name) values (5, 'дискретная шкала')
go


alter table SubScaleDimension add min_value int, max_value int, step_value decimal (8,3)
go


alter table ItemScale_Link drop constraint FK_ItemScale_Link_SubScale
go
alter table ItemScale_Link add constraint FK_ItemScale_Link_SubScale foreign key (subscale_id) references subscales (id) on delete cascade
go
alter table Test_Results drop constraint FK_Test_Results_SubScale
go
alter table Test_Results add constraint FK_Test_Results_SubScale foreign key (SubScale_ID) references SubScales (id) on delete cascade
go
alter table Scale_Range drop constraint FK_Scale_Range_Scale
go
alter table Scale_Range add constraint FK_Scale_Range_Scale foreign key (Scale_ID) references Scales (id) on delete cascade
go
alter table ItemScale_Link drop constraint FK_ItemScale_Link_item
go
alter table ItemScale_Link add constraint FK_ItemScale_Link_item foreign key (item_id) references items (id) on delete cascade
go


create table dbo.DiagnosticFieldType (id tinyint not null, name nvarchar(255))
alter table dbo.DiagnosticFieldType add constraint PK_DiagnosticFieldType primary key (id)
alter table dbo.test add constraint FK_Test_DiagnosticField foreign key (diagnostic_field) references DiagnosticFieldType (id)
go

create table dbo.language (id tinyint not null, name nvarchar(255))
alter table dbo.language add constraint PK_language primary key (id)
alter table dbo.test add constraint FK_Test_language foreign key (language_type) references dbo.language (id)
go

insert into dbo.language (id,name) values (1,'английский')
insert into dbo.language (id,name) values (2,'русский')
go

insert into DiagnosticFieldType (id, name) values (1,'клиническая')
insert into DiagnosticFieldType (id, name) values (2,'образовательная')
insert into DiagnosticFieldType (id, name) values (3,'профессиональная')
insert into DiagnosticFieldType (id, name) values (4,'общая')
go

alter table Scale_Range add constraint FK_ScaleRange_ParamValue foreign key (param_value_id) references param_values (id) on delete cascade
go

--alter table Test_Results drop constraint FK_Test_Results_Item
alter table Test_Results add constraint FK_Test_Results_Item foreign key (item_ID) references items (id) on delete cascade
go

--alter table dbo.Test_Results_Txt drop constraint FK_Test_Results_Txt_Item
alter table dbo.Test_Results_Txt add constraint FK_Test_Results_Txt_Item foreign key (item_id)
references dbo.items (id) on delete cascade
go

create table dbo.test_category (id tinyint not null, name nvarchar(255))
alter table dbo.test_category add constraint PK_Test_Category primary key (id)

alter table dbo.test add category_id tinyint
alter table dbo.test add constraint FK_Test_Category foreign key (category_id)
references dbo.test_category (id)
go

insert into dbo.test_category (id, name) values (1, 'Развлекательный')
insert into dbo.test_category (id, name) values (2, 'Научный и неактуальный')
insert into dbo.test_category (id, name) values (3, 'Разрабатываемый')
insert into dbo.test_category (id, name) values (4, 'Научный и актуальный')
go

alter table dbo.test add Short_Description nvarchar(2000)
go

alter table ItemScale_Link add dimension_id int
go
alter table ItemScale_Link drop constraint FK_ItemScale_Link_item
go
alter table items add constraint UK_Items_Item_Dim unique (id, dimension_id)
go
alter table ItemScale_Link add constraint FK_ItemScale_Link_item_dim foreign key (item_id, dimension_id) references items (id, dimension_id)
go


alter table ItemScale_Link drop constraint FK_ItemScale_Link_SubScale
go
alter table subscales add constraint UK_Subscale_Sub_Dim unique (id, dimension_id)
go
alter table ItemScale_Link add constraint FK_ItemScale_Link_Subscale_dim foreign key (subscale_id, dimension_id) 
references subscales (id, dimension_id)
go

alter table ItemScale_Link alter column dimension_id int not null
go

alter table Param_Results add txt_value nvarchar(255)
alter table Param_Results alter column Param_Value_ID int
go

alter table Scales add AVG_Value float, Standard_Dev float
go

alter table scales add Param2_ID int
--alter table params add constraint UK_Params unique (id, test_id)
--alter table scales add constraint FK_Param2 foreign key (param2_id, test_id) references params (id, test_id)

-- нужны составные ключи (FK) на уровне БД!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! (в ключ добавить ID теста)

alter table scales add constraint FK_Param2 foreign key (param2_id) references params (id)
go

alter table Scale_Range add Param2_Value_ID int
go
alter table Scale_Range add constraint FK_Scale_Range_Param2_Value foreign key (Param2_Value_ID) references Param_Values (id)
go

create table subject_group (id int not null identity, [name] nvarchar(255))
alter table subject_group add constraint PK_Subject_group primary key (id)

alter table test_subject add group_id int
alter table test_subject add constraint FK_Subject_Group foreign key (group_id) references subject_group (id)
go

insert into subject_group (name) values ('Дизайнер')
insert into subject_group (name) values ('Эра и Ко')
go

---------------------------------------------------------
alter table Scales alter column min_value decimal (6,3)
alter table Scales alter column max_value decimal (6,3)
go
--alter table Scales alter column min_value smallint
--alter table Scales alter column max_value smallint
---------------------------------------------------------
create table Scale_Norm (id int not null identity, NormType_id tinyint not null, Scale_ID int not null, 
LowRange decimal(10,4) not null default 0, HighRange decimal(10,4) not null default 0)
go

alter table Scale_Norm add constraint PK_Scale_Norm primary key (id)
go
alter table Scale_Norm add constraint FK_Scale_Norm_Scale
foreign key (Scale_id) references Scales (id) on delete cascade
go
ALTER TABLE dbo.Scale_Norm ADD CONSTRAINT
	CK_Scale_Norm_Type_ CHECK (([normtype_id]>=(1) AND [normtype_id]<=(5)))
go

ALTER TABLE dbo.Scale_Norm ADD CONSTRAINT
	IX_Scale_Norm_Type UNIQUE NONCLUSTERED 
	(
	Scale_ID,
	NormType_id
	)
go
--alter table Scale_Norm add constraint UK_Scale_Norm_Type unique key (scale_id, normtype_id)

alter table dbo.Raw_Data alter column Raw_Value decimal(8,3) not null
alter table dbo.Test_Data alter column Test_Value decimal(8,3) not null
go

create table dbo.Norm_Type (id tinyint not null, name nvarchar(20))
go
alter table dbo.Norm_Type add constraint PK_Norm_Type primary key (id)
go
insert into dbo.Norm_Type (id, name)
select 1, 'очень низкое' union all
select 2, 'низкое' union all
select 3, 'нормальное' union all
select 4, 'высокое' union all
select 5, 'очень высокое' 
go

alter table Scale_Norm add constraint FK_Scale_Norm_Type foreign key ([normtype_id]) references Norm_Type (id)
go
----------------------------
alter table Test_Question add isShuffledItem bit not null default 0

alter table subject_group add project_id int 
alter table subject_group add constraint FK_SubjectGroup_Proj foreign key (project_id) references projects (id)
go

alter table test_subject add fio nvarchar(255)
go

alter table Test_Question add isTimeRestrict bit not null default 0
go

alter table test_results alter column SubScale_ID int null
go

alter table Test_Results drop constraint FK_Test_Results_Subject
alter table Test_Results add constraint FK_Test_Results_Subject foreign key (subject_ID) references test_subject (id) on delete cascade
alter table Test_Data drop constraint FK_Test_Data_Subj
alter table Test_Data add constraint FK_Test_Data_Subj foreign key (Subject_ID) references Test_Subject (id) on delete cascade
alter table Raw_Data drop constraint FK_Raw_Data_Subj
alter table Raw_Data add constraint FK_Raw_Data_Subj foreign key (Subject_ID) references Test_Subject (id) on delete cascade
go
-------------------------------------
alter table test_subject add BaseSubject_id int
go
--alter table test_subject drop constraint FK_Base_Subj
alter table test_subject add constraint FK_Base_Subj foreign key (BaseSubject_ID) references Test_Subject (id) --on delete cascade
go
ALTER TABLE dbo.Test_Subject ADD
	MeasureNumber  AS case when basesubject_id is null then 1 else 2 end
go

alter table items alter column [text] nvarchar(300)
go


alter table Norm_Type add bk_color char(6) not null default '00FF00'
go
-- бледный светофор
update Norm_Type set bk_color = 'FF6600' where id=1 -- очень низкий
update Norm_Type set bk_color = 'FFCC99' where id=2 -- низкий
update Norm_Type set bk_color = 'FFFF99' where id=3 -- норм.
update Norm_Type set bk_color = 'CCFF99' where id=4 -- высокий
update Norm_Type set bk_color = '99FF99' where id=5 -- очень высокий

-- дефолтные
update Norm_Type set bk_color = '418CF0' where id=1 -- очень низкий
update Norm_Type set bk_color = 'FCB441' where id=2 -- низкий
update Norm_Type set bk_color = 'E0400A' where id=3 -- норм.
update Norm_Type set bk_color = '056492' where id=4 -- высокий
update Norm_Type set bk_color = 'BFBFBF' where id=5 -- очень высокий

--------------------------------------------------
alter table Test_Question add isShuffledAns bit not null default 0

alter table subject_group add UserKey uniqueidentifier
go


alter table Params alter column Test_ID int null
go
alter table Params add Group_ID int null
alter table Params add constraint FK_Params_Group foreign key (Group_ID) references subject_group (id)
go

-- ProjectTest_Link более не актуальна drop table ProjectTest_Link
create table Test_SubjectGroup_Link (id int not null identity, idTest int not null, idGroup int not null, OrdNumber tinyint,
constraint PK_TestSubjectGroupLink primary key (id),
constraint FK_TestSubjectGroupLink_Test foreign key (idTest) references test (id),
constraint FK_TestSubjectGroupLink_Subject foreign key (idGroup) references Subject_Group (id)
)
go

create table dbo.Company (id int not null identity, name nvarchar(255),
	constraint PK_Company primary key (id))
go

alter table subject_group add idCompany int
go
alter table subject_group add constraint FK_SubjGroupCompany foreign key (idCompany) references company (id)
go

create table dbo.dept (id int not null identity, idCompany int not null, name nvarchar(255),
	constraint pk_Dept primary key (id),
	constraint fk_dept_company foreign key (idCompany) references Company (id)
	--unique uk_dept (id, idCom)
)

create table dbo.user_account (idUser uniqueidentifier not null, 
	idCompany int, idDept int,
	login_name nvarchar(255), fio nvarchar(255), tags nvarchar(255),
	constraint pk_UserAcc primary key (iduser),
	constraint fk_UserAcc_Company foreign key (idCompany) references company (id),
	constraint fk_UserAcc_Dept foreign key (idDept) references dept (id) -- complex key ?
)
go
---------------------------------------------------------------------------

--drop table TestSubjectGroupLink


create table Job (id int not null identity, name nvarchar(255) not null,
constraint pk_job primary key (id)
)
go
create table user_state (id tinyint not null, name nvarchar(255) not null,
constraint pk_user_state primary key (id))
go
insert into user_state (id,name) values (1,'кандидат')
insert into user_state(id,name) values (2,'действующий сотрудник')
go

alter table user_account add comment nvarchar(255), idJob int, idState tinyint
go
alter table user_account add constraint fk_user_job foreign key (idjob) references job (id)
go
alter table user_account add constraint fk_user_state foreign key (idstate) references user_state (id)
go

alter table user_account add birth_year smallint, gender bit
go

alter table test_subject add idUser uniqueidentifier
go

--alter table test_subject drop constraint fk_subject_user
alter table test_subject add constraint fk_subject_user foreign key (iduser) references user_account (iduser) on delete cascade
go

alter table subject_group add create_date datetime not null default getdate(), stop_date smalldatetime
go

alter table test_subject add MailWasSent bit not null default 0
go