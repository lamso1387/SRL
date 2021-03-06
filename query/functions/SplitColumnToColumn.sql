
/****** Object:  UserDefinedFunction [dbo].[Wordparser]    Script Date: 09/13/2017 06:56:46 ب.ظ ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create FUNCTION [dbo].[SpliteColumnToColumn]
(
  @multiwordstring nVARCHAR(255),
  @delimiter nVARCHAR(10),
  @wordnumber      NUMERIC
)
returns nVARCHAR(255)
AS
  BEGIN
      DECLARE @remainingstring nVARCHAR(255)
      SET @remainingstring=@multiwordstring

      DECLARE @numberofwords NUMERIC
      SET @numberofwords=(LEN(@remainingstring) - LEN(REPLACE(@remainingstring, @delimiter, '')) + 1)

      DECLARE @word nVARCHAR(50)
      DECLARE @parsedwords TABLE
      (
         line NUMERIC IDENTITY(1, 1),
         word nVARCHAR(255)
      )

      WHILE @numberofwords > 1
        BEGIN
            SET @word=LEFT(@remainingstring, CHARINDEX(@delimiter, @remainingstring) - 1)

            INSERT INTO @parsedwords(word)
            SELECT @word

            SET @remainingstring= REPLACE(@remainingstring, Concat(@word, @delimiter), '')
            SET @numberofwords=(LEN(@remainingstring) - LEN(REPLACE(@remainingstring, @delimiter, '')) + 1)

            IF @numberofwords = 1
              BREAK

            ELSE
              CONTINUE
        END

      IF @numberofwords = 1
        SELECT @word = @remainingstring
      INSERT INTO @parsedwords(word)
      SELECT @word

      RETURN
        (SELECT word
         FROM   @parsedwords
         WHERE  line = @wordnumber)

  END