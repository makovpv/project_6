drop procedure dbo.CalcScaleIndexValue
go
create procedure dbo.CalcScaleIndexValue (@scale_id int,  @formula nvarchar(255), @test_id int, @subject_id int) 
as
begin
	set nocount on
	declare @ABR nvarchar(255), @tval decimal(8,3), @res decimal(8,3)
	set @formula = replace(@formula,',','.')

	declare c_idx cursor fast_forward for
	select s.abreviature, isnull(rd.Raw_Value,0)
	from Scales s
	left join Raw_Data rd on rd.Scale_ID=s.id and rd.Subject_ID=@subject_id
	where test_id = @test_id and isnull(s.abreviature,'') <> ''
	
	open c_idx
	fetch next from c_idx into @abr, @tval
	while @@FETCH_STATUS=0 begin
		set @formula = REPLACE (@formula, @ABR, @tval)
		--print @abr
		fetch next from c_idx into @abr, @tval	
	end
	close c_idx
	deallocate c_idx

	--print '++++'
	declare @tmp nvarchar(255)
	set @tmp = N'select @res_out = '+@formula
	--print @tmp
	EXECUTE sp_executesql @tmp, N'@res_out decimal(8,3) OUTPUT', @res_out=@res output;
	
	--print @res
	if not exists (select 1 from Raw_Data where Scale_ID=@scale_id and Subject_ID = @subject_id )
		insert into dbo.Raw_Data (Subject_ID, Scale_ID, Raw_Value)
		values (@subject_id, @scale_id, isnull(@res,0))
	else
		update dbo.Raw_Data set Raw_Value = isnull(@res,0) 
		where Scale_ID=@scale_id and Subject_ID = @subject_id
end
go

--exec dbo.CalcScaleIndexValue 469, 'Is+Zh+RF+S_Z_h', 1136, 625

drop procedure dbo.CalcRawValues
go
create procedure dbo.CalcRawValues (@SubjectID int) as
begin
	set nocount on
	delete from Raw_Data where Subject_ID = @SubjectID

	declare @HasKeys bit = 0

	-- обычна€ шкала (ключи)
	insert into dbo.Raw_Data (Subject_ID, Scale_ID,  Raw_Value)
	select @SubjectID, isl.scale_id, isnull(sum(isl.kf*tr.SelectedValue),0)
	from Test_Results tr 
	INNER join ItemScale_Link isl on isl.subscale_id = tr.SubScale_ID and isl.item_id = tr.item_id
	INNER join scales s on s.id=isl.scale_id
	where tr.Subject_ID = @SubjectID and s.RawCalcType = 1
	group by isl.scale_id

	if @@rowcount > 0 set @HasKeys = 1
	
	-- учет по наличию ответов
	declare @ans_count int, @anstext_count int, @TestID int
	select @ans_count = count(distinct tr.item_id) from Test_Results tr where tr.Subject_ID = @SubjectID and isnull(tr.SelectedValue,0) <> 0
	select @anstext_count = count(distinct trx.item_id) from Test_Results_txt trx where trx.Subject_ID = @SubjectID and isnull(trx.text,'') <> ''
	select @TestID = test_id from test_subject where id=@SubjectID
	
	insert into dbo.Raw_Data (Subject_ID, Scale_ID,  Raw_Value)
	select @SubjectID, s.id, isnull(@ans_count,0)+isnull(@anstext_count,0)
	from scales s
	where s.test_id = @TestID and s.RawCalcType = 2 

	if @HasKeys > 0 begin
		-- индексы
		declare @ScaleID int, @Formula nvarchar(255)
		declare c_idx_scl cursor fast_forward for
		select s.id, s.Formula
		from dbo.Scales s
		where s.test_id = @TestID and isnull(s.formula,'')<>''

		open c_idx_scl
		fetch next from c_idx_scl into @scaleID, @Formula
		while @@FETCH_STATUS = 0 begin
			exec dbo.CalcScaleIndexValue @scaleID, @Formula, @TestID, @SubjectID
			fetch next from c_idx_scl into @scaleID, @Formula
		end
		close c_idx_scl
		deallocate c_idx_scl
	end
end
go

drop procedure dbo.CalcSubjectTestValues
go
create procedure dbo.CalcSubjectTestValues (@SubjectID int) as
begin
	set nocount on
	--exec dbo.CalcRawValues @SubjectID
	declare @TestValue decimal(8,3), @ParamValueID int, @Param2ValueID int
	delete from Test_Data where Subject_ID = @SubjectID

	exec dbo.CalcRawValues @SubjectID

	--select top 1 @ParamValueID = Param_Value_ID
	--from Param_Results where Subject_id = @SubjectID

	declare @ScaleID int, @RawScore decimal(8,3), @ScoreCalcType tinyint, @MaxValue smallint
	declare c cursor fast_forward for
	select rd.scale_id, rd.raw_value, s.ScoreCalcType, s.max_value
	from dbo.Raw_Data rd
	inner join scales s on s.id=rd.scale_id
	where rd.Subject_ID = @SubjectID

	open c
	fetch next from c into @scaleID, @RawScore, @ScoreCalcType, @MaxValue
	while @@FETCH_STATUS = 0 begin
		set @TestValue = 0
		if @ScoreCalcType = 2 set @TestValue = @RawScore -- тестовый равен сырому
		else if @ScoreCalcType = 1 begin -- “-балл
			select top 1 @TestValue = sr.Score 
			from Scale_Range sr 
			left join Scales s on s.id = sr.Scale_ID
			where sr.Max_Value >= @RawScore and sr.Scale_ID = @ScaleID 
			and (s.Param_ID is null or sr.Param_Value_ID in (select pv.Param_Value_ID from Param_Results pv where pv.Subject_ID=@SubjectID))
			and (s.Param2_ID is null or sr.Param2_Value_ID in (select pv.Param_Value_ID from Param_Results pv where pv.Subject_ID=@SubjectID))
			order by sr.Max_Value
		end
		else if @ScoreCalcType = 3 begin -- проценты
			if isnull(@MaxValue,0)<>0
				set @TestValue = cast(100.0 * @RawScore / @MaxValue as int)
			else set @TestValue = @RawScore 
		end
		else begin
			close c
			deallocate c
			raiserror ('неверный тип расчета бала', 16, 1)
			return
		end
		
		insert into dbo.Test_Data (Subject_ID, Scale_ID, Test_Value)
		values (@SubjectID, @ScaleID, isnull(@TestValue,0))

		fetch next from c into @scaleID, @RawScore, @ScoreCalcType, @MaxValue
	end
	close c
	deallocate c

end
go

drop procedure dbo.CalcTestValues
go
create procedure dbo.CalcTestValues (@BaseSubjectID int) as
begin
	set nocount on
	declare @idSubj int
	declare c_subj cursor fast_forward for
	select id
	from test_subject 
	where id = @BaseSubjectID or (basesubject_id is not null and basesubject_id = @BaseSubjectID)

	open c_subj
	fetch next from c_subj into @idSubj
	while @@FETCH_STATUS = 0 begin
		if exists (select top 1 1 from test_results where subject_id = @idSubj)
			exec dbo.CalcSubjectTestValues @idSubj
		
		fetch next from c_subj into @idSubj
	end
	close c_subj
	deallocate c_subj
end
go


-----------------------------------------------------------
drop procedure dbo.Delete_Subject
go

create procedure dbo.Delete_Subject (@SubjectID int) as
begin
	delete Test_Data where Subject_ID = @SubjectID
	delete Param_Results where Subject_ID = @SubjectID
	delete Test_Results where Subject_ID = @SubjectID
	delete Test_Results_Txt where subject_id = @SubjectID
	delete Test_Subject where id = @SubjectID
end
go
-----------------------------------------------------------

--select rank() over (order by test_value desc) as Rank, *
--from test_data td
--where td.Subject_ID = 25


--select * 
--from inter_condition ic
--inner join Test_Data td on td.Subject_ID = 127
--where ic.inter_id = 10

--select * 
--from inter_condition ic
--inner join (
--select rank() over (order by test_value desc) as scale_rank, Scale_ID, td.Test_Value
--from Test_Data td 
--where  td.Subject_ID = 127
--) q on ic.scale_id = q.Scale_ID
--where ic.inter_id = 10 
--	AND ((ic.range_type=1 and q.Test_Value not between ic.min_value and ic.max_value)
--			OR
--		(ic.range_type=2 and q.scale_rank not between ic.min_value and ic.max_value))

drop function dbo.IsInterpretationFor
go
create function dbo.IsInterpretationFor (@inter_id int, @SubjectID int) returns bit
as
begin
	declare @res bit
	if not exists (
		select 1 from inter_condition ic
		inner join (
			select rank() over (order by test_value desc) as scale_rank, Scale_ID, td.Test_Value
			from Test_Data td 
			where  td.Subject_ID = @SubjectID
			) q on ic.scale_id = q.Scale_ID
			where ic.inter_id = @inter_id
				AND ((ic.range_type=1 and q.Test_Value not between ic.min_value and ic.max_value)
						OR
					(ic.range_type=2 and q.scale_rank not between ic.min_value and ic.max_value))
		)
		set @res = 1
	else
		set @res = 0
	return @res
end
go
-----------------------------------------------------------
--drop function dbo.GetInterpretation
--go
--create function dbo.GetInterID_For (@TestID int, @SubjectID int) returns 
--table (id int)
--as
--begin
--	declare @res ntext, @txt ntext
--	declare @interpretation_id int
--	declare c cursor fast_forward for
--	select i.id, i.text from interpretation i where i.test_id = @TestID
--	open c
--	fetch next from c into @interpretation_id, @txt
--	while @@FETCH_STATUS = 0 begin
		
--		if not exists (
--			select 1 from inter_condition ic 
--			where ic.inter_id = @interpretation_id 
--				and	dbo.GetTestValue (@SubjectID, @TestID, ic.scale_id) not between ic.min_value and ic.max_value
--		)
--		--if dbo.IsInterpretationFor (@interpretation_id, @SubjectID) = 1 
--			set @res = isnull(@res,'') +CHAR (13)+@txt

--		fetch next from c into @interpretation_id, @txt
--	end
--	close c
--	deallocate c

--	return @res
--end
--go
-----------------------------------------------------------

------------------------------------------------------------------------

create procedure dbo.Delete_ItemScaleLink (@ItemID int, @ScaleID int) as
begin
	delete from ItemScale_Link where item_id = @ItemID and scale_id = @ScaleID
end
go

drop procedure dbo.Recalc_Scale_Range
go
-- переопределение значений дл€ расчета тестовго бала дл€ шкалы
create procedure dbo.Recalc_Scale_Range (@ScaleID int) as
begin
	delete from Scale_Range where Scale_ID = @ScaleID

	declare @max_value smallint, @min_value smallint, @i smallint
	select @max_value = SUM(max_value), @min_value = SUM (min_value) 
	from (
		select MAX(isl.kf) as max_value, MIN(isl.kf) as min_value
		from ItemScale_Link isl --inner join SubScales ss on isl.subscale_id = ss.id
		where isl.scale_id = @ScaleID
		group by isl.item_ID) q

	set @i = @min_value
	while @i <= @max_value begin
		insert into Scale_Range (Scale_ID, Max_Value, Score)
		values (@ScaleID, @i, 1)

		set @i = @i+1
	end

end
go


create procedure dbo.CopyItemScaleKF_To (@ScaleID int, @SourceItemId int, @DestItemNumber int) as
begin
	declare @GroupId int, @SourceNumber int, @DestItemID int, @DestDimensionID int
	select @SourceNumber = i.number, @GroupId = i.group_id from items i where i.id = @SourceItemId 
	if @SourceNumber = @DestItemNumber return

	declare @kf_count int, @dest_kf_count int
	select @kf_count =  COUNT(*) 
	from items i
	left join SubScales ss on i.dimension_id = ss.Dimension_ID
	where i.id = @SourceItemId	
	
	select @DestItemID = i.id, @DestDimensionID = i.dimension_id
	from items i where i.group_id = @GroupId and i.number = @DestItemNumber
	
	select @dest_kf_count = COUNT(*) from SubScales where Dimension_ID = @DestDimensionID
	if @dest_kf_count <> @kf_count return
	
	delete from ItemScale_Link where scale_id = @ScaleID and item_id = @DestItemID

	insert into ItemScale_Link (item_id, kf, scale_id, subscale_id) -- хм... а... dimension_id кака€ должна быть ?
	select @DestItemID, lnk.kf, @ScaleID, ss_d.id 
	from ItemScale_Link lnk
	inner join SubScales ss_s on ss_s.id = lnk.subscale_id
	inner join SubScales ss_d on ss_d.Dimension_ID = @DestDimensionID and ss_d.OrderNumber = ss_s.OrderNumber
	where lnk.scale_id = @ScaleID and lnk.item_id = @SourceItemId

end
go

--create function dbo.GetDynamicText (@TestID int) returns ntext
--as
--begin
--	declare @res ntext
--	select @res = pt.template_text from Passport_Template pt where id = 1
	
--	declare @Author as nvarchar(255)
--	select @Author = t.author
--	from test t where id = @TestID

--	set @res = REPLACE (@res, '[јвтор]', @Author)

--	return @res
--end
--go
--------------------------------------------------------------------------------------------------------------

--select * from test order by name -- 10
--select * from test_question where test_id = 12 --16
--select * from SubScaleDimension where  test_id= 10

--select * from itemscale_link

--exec dbo.Test_Delete 18
-----------------------------------------------------------------------
drop procedure dbo.Test_Delete
go
create procedure dbo.Test_Delete (@Test_ID int) as
begin
	set nocount on
	
	delete from param_results
	from param_results pr
	inner join test_subject ts on ts.id = pr.subject_id
	where ts.test_id=@Test_ID
	
	delete from resume_items where test_id = @Test_ID
	
	delete from inter_condition
	from inter_condition ic
	inner join interpretation i on i.id=ic.inter_id
	where i.test_id=@Test_ID
	delete from interpretation where test_id = @Test_ID

	delete ItemScale_Link
	from ItemScale_Link lnk
	inner join items i on i.id = lnk.item_id
	inner join test_question tq on tq.id=i.group_id
	inner join scales s on s.test_id=@Test_ID and lnk.scale_id=s.id
	where tq.test_id=@Test_ID

	delete from items where group_id in (select id from test_question where test_id = @Test_ID)
	delete from subscales where dimension_id in (select id from subscaledimension where test_id=@Test_ID)
	delete from subscaledimension where test_id=@Test_ID

	delete from test_question where test_id = @Test_ID
	
	delete from scale_range where scale_id in (select id from scales where test_id = @Test_ID)
	
	delete from raw_data
	from raw_data rd inner join scales s on s.id=rd.scale_id
	where s.test_id=@Test_ID
	delete from test_data
	from test_data td inner join scales s on s.id=td.scale_id
	where s.test_id=@Test_ID
	delete from test_diagram_scales
	from test_diagram_scales  tds inner join scales s on s.id = tds.scale_id
	where s.test_id = @Test_ID

	delete from scales where test_id=@Test_ID
	delete from params where test_id = @Test_ID
	delete from resume_items where test_id = @Test_ID
	delete from test_diagram where test_id = @Test_ID
	delete from test where id = @Test_ID
end
go

-----------------------------------------------------------
drop procedure dbo.SetItemDimension
go

create procedure dbo.SetItemDimension (@ItemID int, @DimensionID int, @SetMode tinyint, 
	@DimensionType tinyint, @DimensionMode tinyint, @GradationCount tinyint,
	@MinValue int, @MaxValue int, @StepValue decimal (8,3)
) as
begin
	if @SetMode = 1
		update items set dimension_id = @DimensionID where id = @ItemID
	else
		if @SetMode = 2 
			update items set dimension_id = @DimensionID 
			where group_id = (select i.group_id  from items i where i.id = @ItemID)
		else
			if @SetMode = 3 begin
				declare @tid int
				select @tid = tq.test_id
				from items i
				inner join Test_Question tq on i.group_id = tq.id
				where i.id = @ItemID

				update items set dimension_id = @DimensionID
				from items i
				inner join Test_Question tq on tq.id = i.group_id
				where tq.test_id = @tid
			end

	-- обновление названи€ dimension
	declare c cursor fast_forward for select s.name from SubScales s where s.Dimension_ID = @DimensionID order by s.OrderNumber
	declare @Str nvarchar(255), @DimStr nvarchar(255) set @DimStr = ''
	open c
	fetch next from c into @Str
	while @@FETCH_STATUS = 0 begin
		set @DimStr = @DimStr + Substring (@Str, 1, 3) +'/'
		fetch next from c into @Str
	end
	close c
	deallocate c
	update SubScaleDimension set name = @DimStr, dimension_type = @DimensionType, dimension_mode = @DimensionMode, 
		GradationCount = @GradationCount, min_value = @MinValue, max_value = @MaxValue, step_value = @StepValue
	where id = @DimensionID
end
go

drop procedure dbo.Add_ItemScaleLink
go

create procedure dbo.Add_ItemScaleLink (@ItemID int, @ScaleID int) as
begin
	insert into ItemScale_Link (item_id, scale_id, subscale_id, dimension_id, kf)
	select @ItemID, @ScaleID, s.id, i.Dimension_ID, 1
	from SubScales s
	inner join items i on s.Dimension_ID = i.dimension_id
	where i.id = @ItemID
end
go

-----------------------------------------------------------
---------------------------------------------------
drop function dbo.SubjectAnswers
go
create function dbo.SubjectAnswers (@SubjectID int) returns @tbl table (answer nvarchar(255)) 
AS
begin
	insert into @tbl (answer)
	select q.answer
	from (
	select i.group_id, i.number as item_number, ss.OrderNumber as var_number, cast (tr.SelectedValue as nvarchar(255)) as answer, i.id as item_id, tr.SubScale_ID as var_id
	from Test_Results tr 
	inner join items i on i.id = tr.item_id
	inner join SubScaleDimension ssd on ssd.id = i.dimension_id and ssd.dimension_type in (2,5)
	inner join SubScales ss on ss.Dimension_ID = i.dimension_id and tr.SubScale_ID = ss.id
	where tr.Subject_ID = @SubjectID
	UNION ALL
	select i.group_id, i.number, null, cast (ss.OrderNumber as nvarchar(255)), i.id, tr.SubScale_ID
	from Test_Results tr 
	inner join items i on i.id = tr.item_id
	inner join SubScaleDimension ssd on ssd.id = i.dimension_id and ssd.dimension_type in (1,6)
	inner join SubScales ss on ss.Dimension_ID = i.dimension_id and tr.SubScale_ID = ss.id
	where tr.Subject_ID = @SubjectID
	UNION ALL
	select i.group_id, i.number, null, tr.text, i.id, null
	from Test_Results_Txt tr
	inner join items i on i.id = tr.item_id
	where tr.subject_id = @SubjectID
	) q
	inner join Test_Question tq on tq.id = q.group_id
	order by tq.number, tq.id, q.item_number, q.item_id, q.var_number, q.var_id
	return
end
go

-- »Ћ» такой вариант (по плану хуже, по смыслу ... может быть более правильный)
drop function dbo.SubjectAnswers
go
create function dbo.SubjectAnswers (@SubjectID int) returns @tbl table (answer nvarchar(255)) 
AS
begin
	declare @TestID int
	select top 1 @TestID = test_id from test_subject where id=@SubjectID
	
	insert into @tbl (answer)
	select q.answer
	from Test_Question tq 
	inner join
	( 
		select i.group_id, i.number as item_number, ss.OrderNumber as var_number, i.id as item_id, ss.id as var_id, cast (tr.selectedvalue as nvarchar(255)) as answer
		from items i
		inner join SubScaleDimension ssd on ssd.id = i.dimension_id and ssd.dimension_type in (2,5)
		inner join SubScales ss on ss.Dimension_ID = i.dimension_id 
		left join test_results tr on tr.item_id=i.id and tr.subject_id = @SubjectID and tr.subscale_id=ss.id
		
		UNION ALL
		select i.group_id, i.number, null, i.id, null, cast (max(ss.OrderNumber) as nvarchar(255))
		from items i
		inner join SubScaleDimension ssd on ssd.id = i.dimension_id and ssd.dimension_type in (1,6)
		left join test_results tr on tr.item_id=i.id and tr.subject_id = @SubjectID
		left join SubScales ss on ss.Dimension_ID = i.dimension_id and tr.subscale_id=ss.id
		group by i.group_id, i.number, i.id
		
		UNION ALL
		select i.group_id, i.number, null, i.id, null, tr.text
		from items i
		inner join SubScaleDimension ssd on ssd.id = i.dimension_id and ssd.dimension_type in (3)
		left join test_results_txt tr on tr.item_id=i.id and tr.subject_id = @SubjectID	
	) q on q.group_id = tq.id
	where tq.test_id = @TestID
	order by tq.number, tq.id, q.item_number, q.item_id, q.var_number, q.var_id
	
	return
end
go

-------------------------------------------------------------------------------------
--3280
--drop function dbo.GetMoreTestFor
--alter function dbo.GetMoreTestFor (@SubjectID int) returns @tbl table ( TestId int, TestName nvarchar(255))
create procedure dbo.GetMoreTestFor (@SubjectID int)
as
begin
	declare @Nick nvarchar(255), @TestID int, @Tags nvarchar(255)
	
	select @Nick = ts.nick_name, @TestID = ts.test_id, @Tags = t.tags
	from test_subject ts inner join test t on t.id = ts.test_id
	where ts.id = @SubjectID

--	insert into @tbl (testid, testname)
	select top 3 t.id, t.name
	from test t
	where isPublished = 1 and
	(
		(patindex('%личность%', @tags)>0 and t.tags like '%личность%') or
		(patindex('%психическое здоровье%', @tags)>0 and t.tags like '%психическое здоровье%') or
		(patindex('%интеллект%', @tags)>0 and t.tags like '%интеллект%') or
		(patindex('%работа%', @tags)>0 and t.tags like '%работа%') or
		(patindex('%отношени€%', @tags)>0 and t.tags like '%отношени€%') or
		(patindex('%отношени€%', @tags)=0 and patindex('%работа%', @tags)=0 and patindex('%интеллект%', @tags)=0 and patindex('%психическое здоровье%', @tags)=0 and patindex('%личность%', @tags)=0)
	)
	and id not in (
		select test_id from test_subject where nick_name =@Nick
		)
	order by (ABS(CHECKSUM(NewId())) % 14)

--	return
end
go

drop function dbo.GetSubjLowScales
go
create function dbo.GetSubjLowScales (@aSubjID int) returns nvarchar(1024)
as
begin
	declare @n nvarchar(255), @res nvarchar(1024)
	declare c cursor fast_forward for
	select s.name from Test_Data td
	inner join Scales s on td.Scale_ID = s.id
	where td.Subject_ID = @aSubjID and td.Test_Value < 50
	open c
	fetch next from c into @n
	while @@FETCH_STATUS = 0 begin
		set @res = isnull(@res,'') +', '+@n
		fetch next from c into @n
	end
	close c
	deallocate c

	if LEN(@res)>1 set @res = RIGHT(@res, LEN(@res)-2)
	return @res
end
go

drop procedure dbo.Add_Param_Gender
go
create procedure dbo.Add_Param_Gender (@TestID int, @GroupID int) as
begin
	declare @param_id int
	insert into Params (Test_ID, name, Param_Type, Group_ID)
	values (@TestID, 'ѕол', 1, @GroupID)
	set @param_id = @@IDENTITY
	insert into Param_Values (Param_ID, str_value, ivalue_1, ivalue_2)
	values (@param_id, 'мужской', 1, 1)
	insert into Param_Values (Param_ID, str_value, ivalue_1, ivalue_2)
	values (@param_id, 'женский', 0, 0)
end
go

drop procedure dbo.Add_Param_Age
go
create procedure dbo.Add_Param_Age (@TestID int, @GroupID int) as
begin
	declare @param_id int
	insert into Params (Test_ID, name, Param_Type, Group_ID)
	values (@TestID, '¬озраст', 2, @GroupID)
	set @param_id = @@IDENTITY
	insert into Param_Values (Param_ID, str_value, ivalue_1, ivalue_2)
	select @param_id, '0-10', 0, 10 union all
	select @param_id, '11-20', 11, 20 union all
	select @param_id, '21-30', 21, 30 union all
	select @param_id, '31-40', 31, 40 union all
	select @param_id, '41-50', 41, 50 union all
	select @param_id, '51-..', 51, 255 
end
go


drop function dbo.next_test_version
go
create function dbo.next_test_version (@test_version nvarchar(20)) returns nvarchar(20)
as
begin
	declare @res nvarchar(20), @dgt char, @old_minor nvarchar(20), @pos int

	if (isnull(@test_version,'')='') set @res = '2'
	else if CHARINDEX ('.',@test_version)=0 set @res=@test_version+'.2'
	else begin
		set @old_minor=''
		set @pos = LEN (@test_version)
		while 1=1 begin
			set @dgt = SUBSTRING (@test_version, @pos, 1)
			if @dgt not in ('1','2','3','4','5','6','7','8','9','0') begin
				set @res = LEFT(@test_version,@pos)+ CAST( cast (@old_minor as int) +1 as nvarchar(20))
				break
			end
			else begin
				set @old_minor = @dgt+@old_minor
				set @pos=@pos-1
			end
		end
	end
	
	return @res
end
go


drop procedure dbo.Test_Copy
go
create procedure dbo.Test_Copy (@idTest int) as
begin
	set nocount on
	declare @NewIdTest int, 
		@IdBlock int, @NewIdBlock int, 
		@idItem int, @NewIdItem int,
		@idDimension int, @NewIdDimension int,
		@idSubScale int, @NewIdSubScale int,
		@idParam int, @NewIdParam int,
		@idParamVal int, @NewIdParamVal int,
		@idDiagram int, @NewIdDiagram int,
		@idInter int, @NewIdInter int,
		@idScale int, @NewIdScale int
	
	BEGIN TRAN mytran

	insert into dbo.test (name, author, guid_code, abbreviature, instruction, isPublished, Tags, StimulSource, ins_date,
	Comment, version_number, test_type, publish_year, publisher, Theory_Construct_Info, reliability_info,
	validation_info, test_norms, jur_law, develop_history, key_security, psy_task, diagnostic_subj,
	ITVersionSpecific, diagnostic_field, social_advisability_idx, qualification_demand, mthd_recomendation,
	use_restriction, lnk_analog, lnk_research, lnk_FullMethodInfo, lnk_DeveloperInfo, lnk_SaleMethodic,
	language_type, category_id, Short_Description)
	select ' опи€ '+name, author, NEWID(), abbreviature, instruction, 0, Tags, StimulSource, GETDATE(),
	Comment, dbo.next_test_version (version_number), 
	test_type, publish_year, publisher, Theory_Construct_Info, reliability_info,
	validation_info, test_norms, jur_law, develop_history, key_security, psy_task, diagnostic_subj,
	ITVersionSpecific, diagnostic_field, social_advisability_idx, qualification_demand, mthd_recomendation,
	use_restriction, lnk_analog, lnk_research, lnk_FullMethodInfo, lnk_DeveloperInfo, lnk_SaleMethodic,
	language_type, category_id, Short_Description
	from test where id = @idTest
	set @NewIdTest = SCOPE_IDENTITY()

	--dimensions
	declare @DimTbl table (idOld int, idNew int)
	declare @SubScaleTbl table (idOld int, idNew int)
	declare c_dim cursor fast_forward for select id from subscaledimension where test_id=@idTest
	open c_dim
	fetch next from c_dim into @idDimension
	while @@FETCH_STATUS = 0 begin
		insert into subscaledimension (test_id, name, dimension_type, dimension_mode, 
			gradationcount, isuniqueselect, min_value, max_value, step_value)
		select @NewIdTest, name, dimension_type, dimension_mode, 
			gradationcount, isuniqueselect, min_value, max_value, step_value
		from subscaledimension where id = @idDimension

		set @NewIDDimension = SCOPE_IDENTITY()
		insert into @DimTbl (idOld, idNew) values (@idDimension, @NewIDDimension)

		--SubScales
		declare c_subscale cursor fast_forward for select id from subscales where dimension_id = @idDimension
		open c_subscale
		fetch next from c_subscale into @idSubScale
		while @@fetch_status=0 begin
			insert into subscales (dimension_id,name,orderNumber)
			select @NewIDDimension, name, orderNumber
			from subscales where id = @idSubscale
			set @NewIdSubScale=SCOPE_IDENTITY()
			insert into @SubScaleTbl (idOld, idNew) values (@IdSubScale, @NewIDSubScale)

			fetch next from c_subscale into @idSubScale
		end
		close c_subscale
		deallocate c_subscale

		fetch next from c_dim into @idDimension
	end
	close c_dim
	deallocate c_dim

	--parameters
	declare @ParamTbl table (idOld int, idNew int)
	declare @ParamValTbl table (idOld int, idNew int)
	declare c_param cursor fast_forward for select id from params where test_id = @idTest
	open c_param
	fetch next from c_param into @idParam
	while @@fetch_status = 0 begin
		insert into params (test_id, name, param_type)
		select @NewIdTest, name, param_type
		from params
		where id = @idParam
		
		set @newIdParam = scope_identity()
		insert into @ParamTbl (idOld, idNew) values (@idParam, @NewIdParam)

		declare c_ParamVal cursor fast_forward for
		select id from param_values where param_id = @idParam
		open c_ParamVal
		fetch next from c_ParamVal into @idParamVal
		while @@fetch_status = 0 begin
			insert into Param_Values (param_id, str_value, ivalue_1, ivalue_2)
			select @NewIdParam, str_value, ivalue_1, ivalue_2
			from param_values
			where id = @idParamVal
			set @NewIdParamVal = scope_identity()

			insert into @ParamValTbl (idOld, idNew) values (@idParamVal, @NewIdParamVal)
			fetch next from c_ParamVal into @idParamVal
		end
		close c_ParamVal
		deallocate c_ParamVal

		fetch next from c_param into @idParam
	end
	close c_param
	deallocate c_param

	--scales
	declare @ScaleTbl table (idOld int, idNew int)
	declare c_scale cursor fast_forward for select id from scales where test_id=@idTest
	open c_scale
	fetch next from c_scale into @idScale
	while @@FETCH_STATUS = 0 begin
		insert into scales (test_id, name, min_value, max_value, abreviature, sumtype, use_raw_data,
			ScoreCalcType, kf1, kf2, [precision], Param_ID, formula, max_limit, min_limit, avg_value,
			standard_dev, Param2_ID)
		select @NewIdTest, name, min_value, max_value, abreviature, sumtype, use_raw_data,
			ScoreCalcType, kf1, kf2, [precision], tbl_1.idNew, formula, max_limit, min_limit, avg_value,
			standard_dev, tbl_2.idNew
		from scales s
		left join @ParamTbl tbl_1 on tbl_1.idOld = s.param_id
		left join @ParamTbl tbl_2 on tbl_1.idOld = s.param2_id
		where id = @idScale

		set @NewIDScale = SCOPE_IDENTITY()
		insert into @ScaleTbl (idOld, idNew) values (@idScale, @NewIdScale)

		insert into scale_norm (normtype_id, scale_id, lowrange, highrange)
		select normtype_id, @NewIdScale, lowrange, highrange
		from scale_norm where scale_id= @idScale

		fetch next from c_scale into @idScale
	end
	close c_scale
	deallocate c_scale

	--diagram
	declare @DiagramTbl table (idOld int, idNew int)
	declare c_diagram cursor fast_forward for select id from test_diagram where test_id=@idtest
	open c_diagram
	fetch next from c_diagram into @idDiagram
	while @@fetch_status = 0 begin
		insert into test_diagram (test_id,name,diagram_type, normlow,normHigh)
		select @NewIdTest,name,diagram_type,normLow,normHigh
		from test_diagram where id = @idDiagram
		set @NewIdDiagram = SCOPE_IDENTITY()

		insert into @DiagramTbl (idold, idNew) values (@idDiagram, @NewIdDiagram)

		insert into test_diagram_scales (diagram_id, scale_id,group_id,OrderNumber)
		select @NewIdDiagram, tbl.idNew, group_id, orderNumber
		from test_diagram_scales tds
		left join @ScaleTbl tbl on tbl.idOld = tds.Scale_id
		where tds.diagram_id = @idDiagram

		fetch next from c_diagram into @idDiagram
	end
	close c_diagram
	deallocate c_diagram

	--resume
	insert into Resume_Items (test_id, OrderNumber,Resume_Item_type, item_text, scale_id, 
		diagram_id, is_Scale_Exclusive,resume_image, txt_template_id)
	select @NewIdTest, OrderNumber, Resume_Item_Type, Item_Text, s.idNew,
		d.idNew, is_Scale_Exclusive,resume_image,txt_template_id
	from Resume_Items ri
	left join @ScaleTbl s on s.idold=ri.scale_id
	left join @DiagramTbl d on d.idOld = ri.diagram_id
	where test_id = @idTest

	--scale_range
	insert into scale_range (scale_id, max_value, score, Param_Value_id, param2_value_id)
	select s.idNew, max_value, score, pv1.idNew, pv2.idNew
	from @ScaleTbl s
	inner join scale_range sr on sr.scale_id=s.idOld
	left join @ParamValTbl pv1 on pv1.idOld=sr.Param_Value_ID
	left join @ParamValTbl pv2 on pv2.idOld=sr.Param2_Value_ID

	-- блоки
	declare c_block cursor fast_forward for select id from Test_Question where test_id = @idTest
	open c_block
	fetch next from c_block into @idBlock
	while @@FETCH_STATUS = 0 begin
		insert into Test_Question (test_id, text, number, instruction, comment, isShuffledItem, isTimeRestrict, isShuffledAns)
		select @NewIdTest, text, number, instruction, comment, isShuffledItem, isTimeRestrict, isShuffledAns
		from Test_Question where id = @IdBlock
		set @NewIdBlock = SCOPE_IDENTITY()

		-- items
		declare c_item cursor fast_forward for select ID from items 
		where group_id = @IdBlock
		open c_item
		fetch next from c_item into @idItem
		while @@FETCH_STATUS = 0 begin
			insert into items (dimension_id, group_id, item_type, number, text)
			select tbl.idNew, @NewIdBlock, item_type, number, text
			from items i
			left join @DimTbl tbl on tbl.idOld = i.dimension_id
			where id = @idItem

			set @NewIdItem = scope_identity()

			--itemscale_link 
			insert into itemscale_link (item_id, scale_id, subscale_id, kf, dimension_id)
			select @NewIdItem, s.idNew, sub.idNew, kf, d.idNew
			from itemscale_link isl
			left join @ScaleTbl s on s.idOld = isl.scale_id
			left join @SubScaleTbl sub on sub.idOld = isl.subscale_id
			left join @DimTbl d on d.idOld = isl.dimension_id
			where isl.item_id = @idItem

			fetch next from c_item into @idItem
		end
		close c_item
		deallocate c_item

		fetch next from c_block into @idBlock
	end
	close c_block
	deallocate c_block

	--interpretation
	declare c_inter cursor fast_forward for	select id from interpretation where test_id=@idTest
	open c_inter
	fetch next from c_inter into @idInter
	while @@fetch_status=0 begin
		insert into interpretation (test_id,[text])
		select @NewIdTest, [text] from interpretation where id = @idInter
		
		set @NewIdInter=scope_identity()

		insert into inter_condition (inter_id,scale_id,min_value,max_value,range_type)
		select @NewIdInter, s.idNew, min_value, max_value, range_type
		from inter_condition ic
		left join @ScaleTbl s on s.idOld = ic.Scale_ID
		where inter_id = @idInter

		fetch next from c_inter into @idInter
	end
	close c_inter
	deallocate c_inter

	COMMIT tran mytran

	return @NewIdTest

end
go

------------------------------------------------------
drop procedure dbo.User_Delete 
go
create procedure dbo.User_Delete (@iduser uniqueidentifier) as
begin
	delete from user_account where idUser = @iduser --and 1=2
end
go
-------------------------------------------------------------------------
drop function dbo.relevance_for_date
go
create function dbo.relevance_for_date (@action_date datetime, @for_date datetime) returns varchar(255)
with schemabinding
as
begin
	declare @delta int, @res varchar(255)

	if (@for_date is null) 
		set @for_date = getdate()
	set @delta = datediff (dd, @action_date, @for_date)
	if (@delta <= 31)
		set @res= cast(@delta as varchar(10)) +' дн. назад'
	else begin
		set @delta = datediff (mm, @action_date, @for_date)
		if (@delta <= 12)
			set @res= cast(@delta as varchar(10)) +' мес. назад'
		else begin
			set @delta = datediff (yy, @action_date, @for_date)
			set @res= cast(@delta as varchar(10)) +' г. назад'
		end
	end
	return @res
end



go


if object_id(N'[dbo].[DateToPeriodID]') is not null
drop function [dbo].[DateToPeriodID]
go

create function [dbo].[DateToPeriodID]( @p_dt as datetime)
returns smallint
with schemabinding -- дл€ детерминизма
begin
  if @p_dt is null
    set @p_dt = '19010101'
    
  return datediff(mm, 0, @p_dt)
end
go

create function dbo.GetMultyAnswerLine (@idItem int, @idSubj int) returns varchar (max)
as
begin
	declare @res varchar(max) set @res = ''
	select @res = @res + name +'; ' from test_results tr
	join subscales ss on ss.id = tr.subscale_id
	where item_id = @idItem and subject_id = @idSubj and selectedvalue=1

	return @res

end
go


if object_id(N'[dbo].[MetricDeviation]') is not null
drop function [dbo].MetricDeviation
go
-- отклонени€ по метрикам
create function dbo.MetricDeviation (@idCompany int, @idDept int = null) returns 
	@res table (idmetric int, metric_name varchar(255), description varchar(max), iddept int, fio varchar(255), test_value decimal(8,3), test_date datetime)
as 
begin
	--set dateformat 'dmy'

	INSERT into @res (idmetric, metric_name, description, iddept, fio, test_value, test_date)
	SELECT m.idMetric, m.name as metric_name, m.description, ua.idDept, ts.fio, td.Test_Value, ts.test_date
	from metric m
	join Test_Data td on m.idScale = td.Scale_ID
	join test_subject ts on ts.id = td.subject_id 
	join user_account ua on ua.iduser = ts.iduser 
	--left join dept on dept.id = ua.idDept
	where m.idcompany = @idcompany and ua.idDept = isnull(@iddept, ua.iddept)
		and m.idtest not in (1235)
		and ts.actual = 1 and ts.test_date >= dateadd (mm, -3, getdate())
		and td.Test_Value < m.index_value and m.condition = '<'
		and ua.idjob in (select idjob from metric_subj_filter where idmetric = m.idmetric and idjob is not null) 
        and ua.idstate in (select idstate from metric_subj_filter where idmetric = m.idmetric and idstate is not null) 
	
	union all
	--отсутствие идей / прохождений теста
	select m.idMetric, m.name as metric_name, m.description, ua.idDept, ts.fio, null as Test_Value, null as test_date
	from metric m
	join test_subject ts on ts.test_id = m.idtest
	inner join user_account ua on ua.iduser = ts.iduser and ua.idcompany = m.idcompany
	--left join dept on dept.id = ua.idDept
	where m.idcompany = @idcompany and ua.idDept = isnull(@iddept, ua.iddept)
		and m.condition = 'NE'  and m.index_value = 1 
		and m.idtest not in (1235)
		and ua.idjob in (select idjob from metric_subj_filter where idmetric = m.idmetric and idjob is not null)   
        and ua.idstate in (select idstate from metric_subj_filter where idmetric = m.idmetric and idstate is not null)  
	GROUP BY m.idMetric, m.name, m.description, ua.idDept, ts.iduser, ts.fio
	--having count (ts.test_date) = 0
	having max (ts.test_date) < dateadd (mm, -3, GETDATE()) OR max (ts.test_date) IS NULL

	union all
	-- чтение книг
	select m.idMetric, m.name as metric_name, m.description, ua.idDept, ts.fio, null as Test_Value, 
		max(cast (case when isdate(txt.text)=1 then txt.text else null end as datetime)) as test_date
	from metric m
	join test_subject ts on ts.test_id = m.idtest
	inner join user_account ua on ua.iduser = ts.iduser and ua.idcompany = m.idcompany
	inner join test_results_txt txt on txt.subject_id = ts.id
	where m.idtest = 1235 
		and ua.idjob in (select idjob from metric_subj_filter where idmetric = m.idmetric and idjob is not null)   
        and ua.idstate in (select idstate from metric_subj_filter where idmetric = m.idmetric and idstate is not null)  
	group by m.idMetric, m.name, m.description, ua.idDept, ts.iduser, ts.fio
	having max(cast (case when isdate(txt.text)=1 then txt.text else null end as datetime)) < dateadd (mm, -3, getdate())

	union all
	-- подтверждение кварт.оценки
	select m.idMetric, m.name as metric_name, m.description, ua.idDept, ua.fio, null as Test_Value, ts.test_date
	from metric m
	join test_subject ts on ts.test_id = m.idtest
	join test_subject_approved tsa on tsa.idSubject = ts.id
	join user_account ua on ua.iduser = tsa.ApprovedByUser and ua.idcompany = m.idcompany
	where tsa.isapproved = 0 and m.condition = 'AP'
	and m.idcompany = @idcompany and ua.idDept = isnull(@iddept, ua.iddept)
	and ua.idjob in (select idjob from metric_subj_filter where idmetric = m.idmetric and idjob is not null)   
	and ua.idstate in (select idstate from metric_subj_filter where idmetric = m.idmetric and idstate is not null)  

	return
end
go



