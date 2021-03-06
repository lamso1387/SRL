USE [mabadimaghased]
GO
/****** Object:  UserDefinedFunction [dbo].[GetTitleOfBaseValue]    Script Date: 09/12/2017 04:08:45 ب.ظ ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[GetTitleOfBaseValue] (@base_key VARCHAR(250), @base_type VARCHAR(250))
RETURNS nVARCHAR(250)
AS BEGIN
    DECLARE @Work nVARCHAR(250);

    select @work=title  from base_values where doc_type=@base_type and [key]=@base_key;

    RETURN @work;
END
GO
