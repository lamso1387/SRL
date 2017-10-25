USE [mabadimaghased]
GO
/****** Object:  UserDefinedFunction [dbo].[GetDateFromEpoch]    Script Date: 09/12/2017 04:08:45 ب.ظ ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[GetDateFromEpoch] (@epoch bigint)
RETURNS datetime
AS BEGIN
   -- DECLARE @Work nVARCHAR(250);

   -- select @work=title  from base_values where doc_type=@base_type and [key]=@base_key;

    RETURN dateadd(s, @epoch, '19700101');
END




GO
