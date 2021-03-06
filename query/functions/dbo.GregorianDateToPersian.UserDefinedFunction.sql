USE [mabadimaghased]
GO
/****** Object:  UserDefinedFunction [dbo].[GregorianDateToPersian]    Script Date: 09/12/2017 04:08:45 ب.ظ ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Function [dbo].[GregorianDateToPersian] (@date datetime)
Returns nvarchar(50)
as
Begin
    Declare @depoch as bigint
    Declare @cycle  as bigint
    Declare @cyear  as bigint
    Declare @ycycle as bigint
    Declare @aux1 as bigint
    Declare @aux2 as bigint
    Declare @yday as bigint
    Declare @Jofst  as Numeric(18,2)
    Declare @jdn bigint

    Declare @iYear   As Integer
    Declare @iMonth  As Integer
    Declare @iDay    As Integer

    Set @Jofst=2415020.5
    Set @jdn=Round(Cast(@date as int)+ @Jofst,0)

    Set @depoch = @jdn - [dbo].[PersianDateToJulian](475, 1, 1) 
    Set @cycle = Cast(@depoch / 1029983 as int) 
    Set @cyear = @depoch%1029983 

    If @cyear = 1029982
       Begin
         Set @ycycle = 2820 
       End
    Else
       Begin
        Set @aux1 = Cast(@cyear / 366 as int) 
        Set @aux2 = @cyear%366 
        Set @ycycle = Cast(((2134 * @aux1) + (2816 * @aux2) + 2815) / 1028522 as int) + @aux1 + 1 
      End

    Set @iYear = @ycycle + (2820 * @cycle) + 474 

    If @iYear <= 0
      Begin 
        Set @iYear = @iYear - 1 
      End
    Set @yday = (@jdn - [dbo].[PersianDateToJulian](@iYear, 1, 1)) + 1 
    If @yday <= 186 
       Begin
         Set @iMonth = CEILING(Convert(Numeric(18,4),@yday) / 31) 
       End
    Else
       Begin
          Set @iMonth = CEILING((Convert(Numeric(18,4),@yday) - 6) / 30)  
       End
       Set @iDay = (@jdn - [dbo].[PersianDateToJulian](@iYear, @iMonth, 1)) + 1 

      Return Convert(nvarchar(50),@iDay) + '-' +   Convert(nvarchar(50),@iMonth) +'-' + Convert(nvarchar(50),@iYear)
End


GO
