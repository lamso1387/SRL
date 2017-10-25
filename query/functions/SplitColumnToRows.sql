
/****** Object:  UserDefinedFunction [dbo].[SplitColumnToRows]    Script Date: 09/13/2017 06:42:12 ب.ظ ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE FUNCTION [dbo].[SplitColumnToRows] 
( 
    @string NVARCHAR(MAX), 
    @delimiter CHAR(1) 
) 
RETURNS @output TABLE(splitdata NVARCHAR(MAX) 
) 
BEGIN 
    DECLARE @start INT, @end INT 
    SELECT @start = 1, @end = CHARINDEX(@delimiter, @string) 
    WHILE @start < LEN(@string) + 1 BEGIN 
        IF @end = 0  
            SET @end = LEN(@string) + 1
       
        INSERT INTO @output (splitdata)  
        VALUES(SUBSTRING(@string, @start, @end - @start)) 
        SET @start = @end + 1 
        SET @end = CHARINDEX(@delimiter, @string, @start)
        
    END 
    RETURN 
END

GO


/*use:
column: complexes.owners
0579848876/حسن رحیمی;0579389553/عبداله یوسفی;0579959449/محمدعلی عرب

select * from
(select owners from  complexes ) t1
CROSS APPLY SplitColumnToRows(t1.owners,';') split

result:

0579848876/حسن رحیمی;0579389553/عبداله یوسفی;0579959449/محمدعلی عرب	   0579848876/حسن رحیمی
0579848876/حسن رحیمی;0579389553/عبداله یوسفی;0579959449/محمدعلی عرب	0579389553/عبداله یوسفی
0579848876/حسن رحیمی;0579389553/عبداله یوسفی;0579959449/محمدعلی عرب	0579959449/محمدعلی عرب

*/

