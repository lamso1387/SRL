USE [mabadimaghased]
GO

/****** Object:  UserDefinedFunction [dbo].[CheckNationalCode]    Script Date: 11/05/2017 01:36:45 ب.ظ ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:        <Haajit>
-- Create date: <Create Date, ,>
-- Description:    <Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[CheckCoNationalCode](@NC nvarchar(11)) 

RETURNS nvarchar(5)
AS
BEGIN

    DECLARE @c int
    DECLARE @n int
    DECLARE @r int
	DECLARE @dp2 int
    DECLARE @NC1 char(1)
    DECLARE @NC2 char(1)
    DECLARE @NC3 char(1)
    DECLARE @NC4 char(1)
    DECLARE @NC5 char(1)
    DECLARE @NC6 char(1)
    DECLARE @NC7 char(1)
    DECLARE @NC8 char(1)
    DECLARE @NC9 char(1)
    DECLARE @NC10 char(1)
	DECLARE @NC11 char(1)
    set @NC= LTRIM(RTRIM(@NC))
    if (len(@NC) <> 11)
    begin
        SET @NC = NULL
    end
    else
    if (@NC LIKE '%[^0-9]%')
    begin
        SET @NC = NULL
    end
    else
    begin
        set @NC1 = substring(@NC,1,1)
        set @NC2 = substring(@NC,2,1)
        set @NC3 = substring(@NC,3,1)
        set @NC4 = substring(@NC,4,1)
        set @NC5 = substring(@NC,5,1)
        set @NC6 = substring(@NC,6,1)
        set @NC7 = substring(@NC,7,1)
        set @NC8 = substring(@NC,8,1)
        set @NC9 = substring(@NC,9,1)
        set @NC10 = substring(@NC,10,1)
		set @NC11 = substring(@NC,11,1)

        if (@NC1 not in ('1','2','3','4','5','6','7','8','9','0'))
            or (@NC2 not in ('1','2','3','4','5','6','7','8','9','0'))
            or (@NC3 not in ('1','2','3','4','5','6','7','8','9','0'))
            or (@NC4 not in ('1','2','3','4','5','6','7','8','9','0'))
            or (@NC5 not in ('1','2','3','4','5','6','7','8','9','0'))
            or (@NC6 not in ('1','2','3','4','5','6','7','8','9','0'))
            or (@NC7 not in ('1','2','3','4','5','6','7','8','9','0'))
            or (@NC8 not in ('1','2','3','4','5','6','7','8','9','0'))
            or (@NC9 not in ('1','2','3','4','5','6','7','8','9','0'))
            or (@NC10 not in ('1','2','3','4','5','6','7','8','9','0'))
			or (@NC11 not in ('1','2','3','4','5','6','7','8','9','0'))
        begin
            set @NC= Null
        end
        else
        if (@NC='11111111111' OR
            @NC='00000000000' OR
            @NC='22222222222' OR
            @NC='33333333333' OR
            @NC='44444444444' OR
            @NC='55555555555' OR
            @NC='66666666666' OR
            @NC='77777777777' OR
            @NC='88888888888' OR
            @NC='99999999999' )
        begin
            SET @NC = NULL
        end
        else
        begin
            set @c = cast(@NC11 as int);
			set @dp2 =  cast(@NC10 as int) +2;
            set @n = (cast(@NC1 as int)+@dp2)*29 +
                     (cast(@NC2 as int)+@dp2)*27 +
                     (cast(@NC3 as int)+@dp2)*23 +
                     (cast(@NC4 as int)+@dp2)*19 +
                     (cast(@NC5 as int)+@dp2)*17 +
                     (cast(@NC6 as int)+@dp2)*29 +
                     (cast(@NC7 as int)+@dp2)*27 +
                     (cast(@NC8 as int)+@dp2)*23 +
                     (cast(@NC9 as int)+@dp2)*19 +
					 (cast(@NC10 as int)+@dp2)*17
            set @r = @n % 11 
            if  NOT((@r = 10 AND @c =0) or (@r !=10 AND @r = @c))
            begin
                SET @NC = NULL
            end    
        end
    end
	declare @check nvarchar(5);
	set @check=case when @NC is null then 'False' else 'True' end
    Return  @check

END
GO


