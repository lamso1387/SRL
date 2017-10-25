USE [mabadimaghased]
GO
/****** Object:  UserDefinedFunction [dbo].[GetPersianDateFromEpoch]    Script Date: 09/12/2017 04:08:45 ب.ظ ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE FUNCTION [dbo].[GetPersianDateFromEpoch] (@epoch bigint)
Returns nvarchar(50)
AS BEGIN
   
  RETURN [dbo].[DateToPersian] ( dateadd(s, @epoch, '19700101'));

END





GO
