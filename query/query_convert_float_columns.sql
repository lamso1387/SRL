
--ALTER TABLE anbar2
--ADD national_id_str nvarchar(255),
--mobile_str nvarchar(255),
--postal_code_str nvarchar(255),
--co_national_id_str nvarchar(255),
--senf_id_str nvarchar(255);


--update anbar2 set national_id_str = str(national_id,15), 
--mobile_str = str(mobile,15),
--postal_code_str = str(postal_code,15),
--co_national_id_str = str(co_national_id,15),
--senf_id_str = str(senf_id,15);


--alter table anbar2
--    drop column national_id, mobile, postal_code, co_national_id, senf_id;


	--EXEC sp_rename 'anbar2.national_id_str', 'national_id', 'COLUMN';
	--EXEC sp_rename 'anbar2.mobile_str', 'mobile', 'COLUMN' ;
	--EXEC sp_rename 'anbar2.postal_code_str', 'postal_code', 'COLUMN' ;
	--EXEC sp_rename 'anbar2.co_national_id_str', 'co_national_id', 'COLUMN' ;
	--EXEC sp_rename 'anbar2.senf_id_str', 'senf_id', 'COLUMN' ;


--update anbar2 set national_id = ltrim(rtrim(national_id)), 
--mobile = ltrim(rtrim(mobile)), 
--postal_code = ltrim(rtrim(postal_code)), 
--co_national_id = ltrim(rtrim(co_national_id)), 
--senf_id = ltrim(rtrim(senf_id));