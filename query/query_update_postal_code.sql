insert into PostalCode(postal_code,province,city, [address], township, last_sent) 
select  postal_code,province,city, full_address ,
 SUBSTRING(full_address,(CHARINDEX('-',full_address)+8),((CHARINDEX('-',full_address,CHARINDEX('-',full_address)+1))-(CHARINDEX('-',full_address)+8))) township ,
 GETDATE()
   from anbar where user_id is not null and
   anbar.postal_code not in (select PostalCode.postal_code from PostalCode)
   ;

   -- run multiple to delete duplicates
   delete from PostalCode
where PostalCode.ID in

(SELECT s.ID
FROM  (
SELECT * ,
        ROW_NUMBER() OVER(PARTITION BY postal_code 
                                ORDER BY ID ASC) AS rk
    FROM PostalCode p ) s

WHERE s.rk = 2);
