DECLARE @intFlag INT
SET @intFlag = 1
WHILE (@intFlag >0)
BEGIN
   
	--delete 
	--select * 
	from Anbar
	
where Anbar.ID in

(SELECT s.ID
FROM  (
SELECT * ,
        ROW_NUMBER() OVER(PARTITION BY address, date,tel,mobile,senf_id,fname,etehadie,family
		,raste,co_national_id,national_id, postal_code,activity_sector,name,province_input,city_input,type,tag
                                ORDER BY ID ASC) AS rk
    FROM Anbar p
	where address ='951120' ) s

WHERE s.rk = 2)
;
 SET @intFlag = @@ROWCOUNT;     

END
GO

