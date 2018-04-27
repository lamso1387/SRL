alter FUNCTION DiffCharsCount
(@String1 nVARCHAR(100),@String2 nVARCHAR(100) )

RETURNS int
AS
BEGIN  
   Declare @count_diff int
     
select @count_diff=len(replace(dbo.DiffChars(@String1,@String2),',',''))
 

   RETURN @count_diff
END
GO
