use ulleum_home
go

alter table subject_group add mail_template nvarchar(2000) default 'пройти исследование можно вот по этой ссылке #link#'
go
alter table subject_group add start_date smalldatetime
go
update user_state set name = 'сотрудник компании' where id=2
insert into user_state(id,name) values (3,'уволенный сотрудник')
insert into user_state(id,name) values (4,'в декретном отпуске')
insert into user_state(id,name) values (5,'в резерве')
insert into user_state(id,name) values (6,'сотрудник на испытательном сроке')
INSERT INTO user_state (id,name) VALUES (7,'отклоненный кандидат')
go
insert into job (name) values ('мерчендайзер')
go
alter table scales add isMain bit not null default 0
go

----------------------------------
create table dbo.report (idReport int not null, name nvarchar(255) not null, description nvarchar(255),
	constraint pk_report primary key (idReport)
)
go
alter table subject_group add idReport int
go
alter table subject_group add constraint fk_subjectgroup_report foreign key (idreport) references dbo.report (idreport)
go

insert into report (idReport,name) values (1,'стандартный групповой отчет')
insert into report (idReport,name) values (2,'групповой отчет с частотой распределения ответов (обезличинный)')
insert into report (idreport,name) values (3,'групповой отчет по объектам оценки')
--------------------

alter table dbo.Test_SubjectGroup_Link add MeasureObjectName nvarchar(255)
go

alter table dbo.test_subject add idTestLink int
alter table dbo.test_subject add constraint fk_test_measure_link foreign key (idtestlink) references Test_SubjectGroup_Link (id)
go

alter table dbo.subject_group add instruction nvarchar(max)
go

alter table user_account add RecruteDate smalldatetime, DismissalDate smalldatetime
go

insert into diagram_type (id,name) values (4, 'пузырьковая')

alter table dbo.Test_SubjectGroup_Link add isObjectRequired bit not null default 1
go

--------------------------------------------
create table dbo.rep_item_type (idItemType smallint not null, name nvarchar(250) not null,
constraint pk_report_item primary key (idItemType))
go
create table dbo.rep_item_link (idLink int not null identity, idGroup int not null, idItemType smallint not null,
constraint pk_rep_item_link primary key (idLink),
constraint fk_rep_item_group foreign key (idGroup) references subject_group (id) on delete cascade,
constraint fk_rep_item_type foreign key (idItemtype) references rep_item_type (idItemtype)
)
go

insert into rep_item_type (iditemtype, name) values (10, 'Индикатор по основной шкале')
insert into rep_item_type (iditemtype, name) values (20, 'Диаграмма по основной шкале')
insert into rep_item_type (iditemtype, name) values (30, 'Результаты по тестовому баллу для основной шкалы')
insert into rep_item_type (iditemtype, name) values (40, 'Результаты по тестовому баллу для основной шкалы в разрезе должностей')
insert into rep_item_type (iditemtype, name) values (50, 'Результаты по тестовому баллу для основной шкалы в разрезе отделов')
insert into rep_item_type (iditemtype, name) values (60, 'Диаграмма по всем шкалам (инструмент)')
insert into rep_item_type (iditemtype, name) values (70, 'Основные статистические показатели по шкалам')
insert into rep_item_type (iditemtype, name) values (80, 'Персональные результаты (тестовый балл) для всех шкал')
insert into rep_item_type (iditemtype, name) values (90, 'Персональные результаты (процентиль) для всех шкал')
insert into rep_item_type (iditemtype, name) values (100, 'Частота выбора вариантов')
insert into rep_item_type (iditemtype, name) values (110, 'Результаты по объектам оценки')

insert into rep_item_type (iditemtype, name) values (3, 'Общая информация по исследованию')
insert into rep_item_type (iditemtype, name) values (5, 'Содержание')
go

insert into rep_item_type (iditemtype, name) values (105, 'Наиболее популярный ответ')
insert into rep_item_type (iditemtype, name) values (45, 'Диаграмма по основной шкале в разрезе должностей')
insert into rep_item_type (iditemtype, name) values (55, 'Диаграмма по основной шкале в разрезе отдела')
go


alter table dbo.rep_item_link add OrdNumer tinyint
go

update rep_item_type set name = 'Основные статистические показатели по шкалам' where iditemtype = 70
go
alter table interpretation add idInterKind tinyint
go
--alter table test_subject add idInterpretation int -- потенциальная угроза (иначе жесткий запрос в лк получается)
--go
--alter table test_subject add constraint fk_subject_interpretation foreign key (idInterpretation) references interpretation (id) on delete cascade
--go
alter table dbo.test add monitoring_area nvarchar(255)
go

create table dbo.rep_test_link (idLink int not null identity, idTest int not null, idItemType smallint not null, OrdNumber tinyint,
constraint pk_rep_test_link primary key (idLink),
constraint fk_rep_test_test foreign key (idTest) references dbo.test (id) on delete cascade,
constraint fk_rep_test_type foreign key (idItemtype) references rep_item_type (idItemtype)
)
go

create index idx_test_res_item on test_results (item_id)
create index idx_test_res_subject on test_results (subject_id)
go

create table dbo.indicator (idIndicator int not null identity, name nvarchar(255), idType smallint not null, idScale int,
	idCompany int not null,
	constraint pk_indicator primary key (idIndicator),
	constraint fk_indicator_scale foreign key (idScale) references dbo.scales (id) on delete cascade,
	constraint fk_indicator_company foreign key (idCompany) references dbo.company (id) on delete cascade
)
go
-------------------------------------------------------

create table dbo.test_measure_link (
	idLink int not null identity,
	idTestMeasure int not null,
	idUser uniqueidentifier null,
	constraint pk_subjectmeasure primary key (idLink),
	constraint fk_subjectmeasure_lnk foreign key (idTestMeasure) references dbo.test_subjectgroup_link (id) on delete cascade,
	constraint fk_subjectmeasure_user foreign key (idUser) references dbo.user_account (idUser) on delete cascade
)
go

alter table dbo.subject_group add isForYearPlan bit not null default 0
go

----------------------
alter table dbo.indicator add idDiagram int null
go
alter table dbo.indicator add constraint fk_indicator_diagram foreign key (idDiagram) references dbo.test_diagram (id) on delete cascade
go


------------------------------------------------
create table dbo.sch_frequency (idFreq tinyint not null, name nvarchar(200) not null,
	constraint pk_freq primary key (idFreq))
go

insert into sch_frequency (idfreq, name) values (0, 'однократно')
insert into sch_frequency (idfreq, name) values (1, 'ежемесячно')
insert into sch_frequency (idfreq, name) values (2, 'ежеквартально')
insert into sch_frequency (idfreq, name) values (3, 'каждые полгода')
insert into sch_frequency (idfreq, name) values (4, 'ежегодно')
go

create table dbo.subj_fill_type (idFillType tinyint not null, name nvarchar(255) not null,
	constraint pk_subj_fill_type primary key (idFillType)
)
go

insert into subj_fill_type (idfilltype, name) values (0, 'по персонам')
insert into subj_fill_type (idfilltype, name) values (1, 'по отделу')
insert into subj_fill_type (idfilltype, name) values (2, 'по должности')
insert into subj_fill_type (idfilltype, name) values (3, 'по статусу')
go

create table dbo.schedule (
	idShedule int not null identity, 
	idBaseGroup int not null, 
	idFreq tinyint not null,
	idFillType tinyint not null,
	constraint pk_schedule primary key (idShedule),
	constraint fk_schedule_base_group foreign key (idBaseGroup) references dbo.subject_group (id),
	constraint fk_schedule_frequency foreign key (idFreq) references dbo.sch_frequency (idfreq),
	constraint fk_schedule_fill_type foreign key (idFillType) references dbo.subj_fill_type (idFillType)
)
go
---------------------------------------
insert into subj_fill_type (idfilltype,name) values (4,'самооценка с контролем начальника')
insert into subj_fill_type (idfilltype,name) values (5,'не заполнять')
---------------------------------------
alter table subject_group add isAutoSubjAdd bit not null default 0 -- автоматическое повторное добавление респондента, который только что прошел исследование
go

insert into rep_item_type (idItemType, name)
values (120, 'Первичные данные (выбор респондента)')
go

insert into subj_fill_type (idfilltype,name) values (6,'все действующие сотрудники компании')
go

alter table indicator add isPersonal bit not null default 0
go
alter table indicator add idGroup int null -- исследование
go
alter table indicator add constraint FK_IndicatorSurvey foreign key (idGroup) references subject_group (id) on delete cascade
go

alter table subject_group add isAutoEmpNew bit not null default 0 -- автоматическое добавление новых сотрудников
go

alter table indicator add category_header varchar(255)
go

insert into dimension_type (id,name, foritem)
values (13,'действующие сотрудники компании', 1)

alter table indicator add abstract varchar(255)
go

update dimension_type set foritem=1 where id = 11
go
--------------------------------------------------
alter table scales alter column [description] varchar(max)
go

------------ IDEA --------------------------------------
create table idea_state (
	idState tinyint not null, 
	name nvarchar(255) not null,
	constraint pk_idea_state primary key (idstate)
)
go

insert into idea_state (idstate,name) values (0, 'на рассмотрении')
insert into idea_state (idstate,name) values (1, 'Внедрить во всей компании')
insert into idea_state (idstate,name) values (2, 'Внедрить в некоторых отделах')
insert into idea_state (idstate,name) values (3, 'Внедрить в отделе')
insert into idea_state (idstate,name) values (4, 'Отправить на доработку')
insert into idea_state (idstate,name) values (5, 'Отклонено')


create table idea (
	id int not null identity,
	idState tinyint not null default 0,
	idSubject int null, -- link to execution fact
	constraint pk_idea primary key (id),
	constraint fk_idea_state foreign key (idstate) references idea_state (idstate),
	constraint fk_idea_subject foreign key (idsubject) references test_subject (id)
)
go

create table idea_generator (
	idTest int not null,
	constraint uk_idea_generator_test unique (idTest),
	constraint fk_idea_generator_test foreign key (idTest) references test (id)
)

insert into idea_generator (idtest) values (1225) -- !!

insert into idea (idSubject) -- !!!!
select ts.id
from test_subject ts
where test_id = 1225 and test_date is not null

----------------------------------
alter table idea add [resume] nvarchar(max) null
go
------------------------------------------------
update dimension_type set ForItem = 1 where id = 11
go
alter table dbo.test add isSinglePage bit not null default 0
go

insert into idea_state (idstate,name) values (6, 'Реализовано')
go

alter table indicator add 
	repeat_lnk bit not null default 0,
	full_report_lnk bit not null default 0
go


create table test_subject_approved (
	id int not null identity, 
	idSubject int not null,  
	isApproved bit not null default 0,
	ApprovedByUser uniqueidentifier, 
	ApprovedDate SmallDateTime, 

	constraint pk_test_subj_approved primary key (id),
	constraint fk_test_subj_approved_by_user foreign key (ApprovedByUser) references user_account (idUser),
	constraint fk_test_subj_approved_subject foreign key (idSubject) references test_subject (id)
)
go

create table dbo.generator_type (
	id tinyint not null, 
	name varchar(255) not null,
	constraint pk_generator_type primary key (id),
)
go

insert into dbo.generator_type (id, name) values (1,'идея')
insert into dbo.generator_type (id, name) values (2,'подтверждение квартальной оценки')
go

alter table idea_generator add idGeneratorType tinyint 
alter table idea_generator add constraint fk_generator_type foreign key (idGeneratorType) references generator_type (id)
go
update idea_generator set idGeneratorType = 1
go
alter table idea_generator alter column idGeneratorType tinyint not null
go

insert into idea_state (idstate,name) values (7, 'К внедрению')
insert into idea_state (idstate,name) values (8, 'Не оценивается')
go

update idea set idstate = 1 where idstate in (2,3)
update idea_state set name = 'Внедрить' where idstate = 1
delete idea_state  where idstate in (2,3)
go
update idea set idstate = 7 where idstate = 1
delete idea_state where idstate = 1
go

alter table idea add implement_date datetime
go
alter table test_subject_approved add commentary varchar(max) not null default ''
go

alter table subject_group add isAnonymous bit not null default 0
go

alter table indicator add link_URL varchar(255), link_title varchar(255)
go

alter table items add [description] varchar(2000)
go

CREATE TABLE dbo.competence (
	idCompetence SMALLINT NOT NULL IDENTITY, 
	idCompany INT NOT NULL,
	[name] NVARCHAR (255) NOT NULL,
	[description] NVARCHAR (2000),
	CONSTRAINT pk_competence PRIMARY KEY (idCompetence),
	CONSTRAINT fk_competence_company FOREIGN KEY (idCompany) REFERENCES dbo.Company (id) ON DELETE cascade
)
go

create table dbo.book (
	idBook smallint not null identity, 
	title nvarchar(255) not null, 
	author nvarchar(255) not null default '', 
	pages smallint not null default 1,
	idCompany int not null,
	CONSTRAINT pk_book PRIMARY KEY (idBook),
	constraint fk_book_company foreign key (idCompany) REFERENCES dbo.Company (id) 
	)
go

create table dbo.book_competence_lnk (
	idBook smallint not null, 
	idCompetence smallint not null,
	constraint pk_book_competence_lnk primary key (idBook, idCompetence),
	constraint fk_book_competence_book foreign key (idBook) references dbo.book (idbook) on delete cascade,
	constraint fk_book_competence_competence foreign key (idCompetence) references dbo.competence (idCompetence) 
)
go


insert into dimension_type (id,name, foritem)
values (14,'компетенция', 1)
insert into dimension_type (id,name, foritem)
values (15,'книга', 1)
go
-------------------------------------------------------------
create table dbo.metric (
	idMetric int not null identity,
	name nvarchar(255) not null,
	[description] nvarchar(max),
	DateCreate datetime not null default getdate(),

	idSubjectGroup int,
	idTest int,
	idScale int,

	index_value decimal (8,0),
	condition char(2),
	idCompany int not null,

	constraint pk_metric primary key (idMetric),
	constraint fk_metric_subjgroup foreign key (idSubjectGroup) references subject_group (id),
	constraint fk_metric_test foreign key (idTest) references test (id),
	constraint fk_metric_scale foreign key (idScale) references Scales (id),
	constraint fk_metric_company foreign key (idCompany) REFERENCES dbo.Company (id) 
)
go

create table dbo.metric_subj_filter (
	idFilter int not null identity,
	idMetric int not null,
	--idFilterType int not null default 1, 
	idJob int,
	idState tinyint,
	constraint pk_ms_filter primary key (idfilter),
	constraint fk_ms_filter foreign key (idMetric) references metric (idMetric) on delete cascade,
	constraint fk_ms_filter_job foreign key (idjob) references job (id),
	constraint fk_ms_filter_state foreign key (idState) references user_state (id)
)
go

alter table scales add RawCalcType tinyint not null default 1
-- 1- calculation by keys
-- 2- just number of answered questions
go


alter table dbo.metric add eliminate_scheme nvarchar(max), calc_description nvarchar(max)
go

alter table dbo.metric add [weight] decimal (4,1) not null default 1
go

alter table dbo.test_subject add actual bit not null default 1
go

declare c cursor for
select ts.Test_Id, ts.idUser, MAX(ts.Test_Date)
from test_subject ts
where Test_Id is not null and idUser is not null and Test_Date is not null
group by ts.Test_Id, ts.idUser
having COUNT (*)>1

declare @testid int, @iduser uniqueidentifier, @maxdate datetime

open c
fetch next from c into @testid, @iduser, @maxdate
while @@FETCH_STATUS = 0 begin
	update test_subject set actual = 0 where test_id = @testid and idUser = @iduser and Test_Date < @maxdate

	fetch next from c into @testid, @iduser, @maxdate
end
close c
deallocate c
go

CREATE TABLE dbo.metric_hist (
	idMetric INT NOT NULL,
	mDate DATETIME NOT NULL,
	mNumber int NOT NULL,
	idDept INT,
	CONSTRAINT fk_metric_hist_metric FOREIGN KEY (idmetric) REFERENCES dbo.metric (idmetric) ON DELETE CASCADE,
	CONSTRAINT fk_metric_hist_dept FOREIGN KEY (iddept) REFERENCES dbo.dept (id) ON DELETE CASCADE
)
GO