--update anbar
--set national_id = null
--where tag='8' and national_id ='NULL';

--update anbar
--set co_national_id = null
--where tag='8' and co_national_id ='NULL';

--update anbar
--set mobile = null
--where tag='8' and mobile ='NULL';

--update anbar
--set postal_code = null
--where tag='8' and postal_code ='NULL';

--update anbar
--set activity_sector = '01'
--where tag='8';

--update anbar
--set name = etehadie
-- where tag='8' and (name='' or  name='NULL' or name is null);

--update anbar
--set contractor_or_agent = 'agent'
--where tag='8';

--alter view VaheddamiType as
--select raste ,type from anbar 
--where tag='8' and (type ='0' or type ='1' or type ='2' or type ='3' or type ='4' or type ='5')
--group by raste, type;

--update anbar
--set type = null
--where tag='8' and (type !='0' and type !='1' and type !='2' and type !='3' and type !='4' and type !='5');

--update anbar
--set anbar.type=VaheddamiType.type
--from anbar join VaheddamiType
--on anbar.province_input= VaheddamiType.raste
--where  anbar.tag='8' and anbar.type is null;

--update anbar
--set anbar.type='3'
--where  anbar.tag='8' and (anbar.type is null or anbar.type='NULL');

--update anbar
--set anbar.type=VaheddamiType.type
--from anbar left join VaheddamiType
--on anbar.raste= VaheddamiType.raste
--where  anbar.tag='8';

--delete from anbar where tag is null;

--update anbar
--set anbar.explain=N'سازمان دامپزشکی کشور'
--where anbar.tag='8';





