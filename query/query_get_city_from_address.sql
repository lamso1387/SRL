select full_address ,
 SUBSTRING(full_address,(CHARINDEX('-',full_address)+8),((CHARINDEX('-',full_address,CHARINDEX('-',full_address)+1))-(CHARINDEX('-',full_address)+8))) city
   from anbar where user_id is not null 