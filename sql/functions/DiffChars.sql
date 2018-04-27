alter FUNCTION [dbo].[DiffChars]
(@String1 nVARCHAR(100),@String2 nVARCHAR(100) )

RETURNS nVARCHAR(100)
AS
BEGIN 

set @String1 = case when len(@String1) > len(@String2) then @String1 else @String2 end;
set @String2 = case when len(@String1) > len(@String2) then @String2 else @String1 end;

  Declare @result as varchar(100);

set @String1 =dbo.PutCommaBetweenChars(@String1);

set @String2=dbo.PutCommaBetweenChars(@String2);

WITH FirstStringSplit(S1) AS
(
    SELECT CAST('<x>' + REPLACE(@String1,',','</x><x>') + '</x>' AS XML)
)
,SecondStringSplit(S2) AS
(
    SELECT CAST('<x>' + REPLACE(@String2,',','</x><x>') + '</x>' AS XML)
)

SELECT @result= STUFF(
(
    SELECT ',' + part1.value('.','nvarchar(max)')
    FROM FirstStringSplit
    CROSS APPLY S1.nodes('/x') AS A(part1)
    WHERE part1.value('.','nvarchar(max)') NOT IN(SELECT B.part2.value('.','nvarchar(max)')
                                                  FROM SecondStringSplit 
                                                  CROSS APPLY S2.nodes('/x') AS B(part2)
                                                  ) 
    FOR XML PATH('')

),1,1,'')
set @result=case when   @result is NULL then '' else @result end ;
   RETURN @result; 
END
 
GO
