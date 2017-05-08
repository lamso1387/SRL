update anbar
set anbar.activity_sector=isicmapper.isic
from anbar ,isicmapper
where anbar.tag='7' and anbar.raste=isicmapper.name