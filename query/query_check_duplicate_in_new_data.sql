--delete from Anbar where Anbar.ID in
(select distinct new_data.ID 
from
(select * from anbar where tag='93') new_data,
(select * from anbar where tag !='93') main
where new_data.national_id=main.national_id and  new_data.postal_code=main.postal_code
and new_data.mobile=main.mobile and main.user_id is not null )