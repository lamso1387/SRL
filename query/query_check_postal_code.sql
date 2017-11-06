USE [mabadimaghased]
GO

/****** Object:  UserDefinedFunction [dbo].[IsPostalCodeAlgorithm]    Script Date: 11/05/2017 01:33:04 ب.ظ ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


 

CREATE FUNCTION [dbo].[IsPostalCodeAlgorithm](@postal_code nvarchar(50))
RETURNS nvarchar(50)
AS BEGIN
    DECLARE @correct nvarchar(50)
	
	if len(@postal_code) != 10
	begin
	set @correct='false' ;
	end
	else 
	begin
	 DECLARE @left5 nvarchar(5);
	 set @left5= left(@postal_code , 5);
	 if @left5 like '%0%' or @left5 like '%2%'
	 set @correct='false' ;
	 else if substring(@left5, 5,1)='5'
	  set @correct='false' ;
	   else if substring(@postal_code, 6,1)='0'
	  set @correct='false' ;
	  else if substring(@postal_code, 6,4)='0000'
	  set @correct='false' ;
	  else
	    set @correct='true' ;
	end
    RETURN @correct
END



GO


