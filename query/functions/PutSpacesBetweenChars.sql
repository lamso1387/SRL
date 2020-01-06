create FUNCTION PutSpacesBetweenChars 

(@String VARCHAR(100))

RETURNS VARCHAR(100)
AS
BEGIN
   DECLARE @pos INT, @result VARCHAR(100); 
   SET @result = @String; 
   SET @pos = 2 -- location where we want first space 
   WHILE @pos < LEN(@result)+1 
   BEGIN 
       SET @result = STUFF(@result, @pos, 0, SPACE(1)); 
       SET @pos = @pos+2; 
   END 
   RETURN @result; 
END
GO