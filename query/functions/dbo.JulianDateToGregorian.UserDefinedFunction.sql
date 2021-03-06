USE [mabadimaghased]
GO
/****** Object:  UserDefinedFunction [dbo].[JulianDateToGregorian]    Script Date: 09/12/2017 04:08:45 ب.ظ ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Secondly, convert Julian calendar date to Gregorian to achieve the target.
Create FUNCTION [dbo].[JulianDateToGregorian] (@jdn bigint)
Returns nvarchar(11)
as
Begin
    Declare @Jofst  as Numeric(18,2)
    Set @Jofst=2415020.5
    Return Convert(nvarchar(11),Convert(datetime,(@jdn- @Jofst),113),110)
End

GO
