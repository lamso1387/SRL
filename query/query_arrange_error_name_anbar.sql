delete from ErrorName;
insert into ErrorName (output_result, error_farsi)
select output_result, error_farsi from Anbar where (api_key is not null and api_key !='') and (user_id is null or user_id ='')
and (output_result is not null and output_result !='')
and (error_farsi is not null and error_farsi !='')
group by output_result ,error_farsi;