update anbar
set anbar.national_id=str(namefamily.NationalCode) , anbar.address='951106'
 from anbar left join namefamily on

( Anbar.fname=namefamily.Name
and Anbar.family=namefamily.Family)
or
(CONCAT(namefamily.Name,namefamily.Family)=Anbar.fname)
or
(CONCAT(namefamily.Name,' ',namefamily.Family)=Anbar.fname)


or


( replace(Anbar.fname,N' ',N'')=namefamily.Name
and Anbar.family=namefamily.Family)

or
( replace(Anbar.fname,' ','')=namefamily.Name
and replace(Anbar.family,N' ',N'')=namefamily.Family)

or
( Anbar.fname=namefamily.Name
and replace(Anbar.family,N' ',N'')=namefamily.Family)

or
(CONCAT(namefamily.Name,namefamily.Family)=replace(Anbar.fname,N' ',N''))

or
(CONCAT(namefamily.Name,' ',namefamily.Family)=replace(Anbar.fname,N' ',N''))


 where Anbar.user_id is null and namefamily.NationalCode is not null
 
 