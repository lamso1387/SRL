using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;



namespace SRL
{
    public class Projects
    {
        public class PmsNtsm
        {
            public class ResultClass<T>
            {
                public string ErrorMessage { get; set; }
                public int Value { get; set; }
                public T Result { get; set; } = SRL.ClassManagement.CreateInstance<T>();

            }

            public class Issues
            {
                public List<Issue> issues { get; set; }
                public int total_count { get; set; }
                public int offset { get; set; }
                public int limit { get; set; }


                public class Project
                {
                    public int id { get; set; }
                    public string name { get; set; }
                }

                public class Tracker
                {
                    public int id { get; set; }
                    public string name { get; set; }
                }

                public class Status
                {
                    public int id { get; set; }
                    public string name { get; set; }
                }

                public class Priority
                {
                    public int id { get; set; }
                    public string name { get; set; }
                }

                public class Author
                {
                    public int id { get; set; }
                    public string name { get; set; }
                }

                public class AssignedTo
                {
                    public int id { get; set; }
                    public string name { get; set; }
                }

                public class CustomField
                {
                    public int id { get; set; }
                    public string name { get; set; }
                    public string value { get; set; }
                }

                public class Issue
                {
                    public int id { get; set; }
                    public Project project { get; set; }
                    public Tracker tracker { get; set; }
                    public Status status { get; set; }
                    public Priority priority { get; set; }
                    public Author author { get; set; }
                    public AssignedTo assigned_to { get; set; }
                    public string subject { get; set; }
                    public string description { get; set; }
                    public int done_ratio { get; set; }
                    public List<CustomField> custom_fields { get; set; }
                    public string created_on { get; set; }
                    public string updated_on { get; set; }
                    public string start_date { get; set; }
                }




            }
            public class Projects
            {
                public List<Project> projects { get; set; }
                public int total_count { get; set; }
                public int offset { get; set; }
                public int limit { get; set; }


                public class Project
                {
                    public int id { get; set; }
                    public string name { get; set; }
                    public string identifier { get; set; }
                    public string description { get; set; }
                    public int status { get; set; }
                    public string created_on { get; set; }
                    public string updated_on { get; set; }
                    public Parent parent { get; set; }
                }
                public class Parent
                {
                    public int id { get; set; }
                    public string name { get; set; }
                }

            }

            public class IssueStatus
            {
                public int id { get; set; }
                public string name { get; set; }
                public bool? is_closed { get; set; }
            }

            public class IssuePriority
            {
                public int id { get; set; }
                public string name { get; set; }
                public bool? is_default { get; set; }
            }

            public static ResultClass<List<Issues.Issue>> GetIssueList(string base_address, string key, int? project_id = null, int? priority_id = null, string create_on = null, string updated_on = null, string filter = "&status_id=*")
            {
                //base_address = "https://pms.ntsw.ir";
                /*
                  To fetch issues for a date range (uncrypted filter is "><2012-03-01|2012-03-07") : created_on=%3E%3C2012-03-01|2012-03-07
                  To fetch issues created after a certain date (uncrypted filter is ">=2012-03-01") : created_on=%3E%3D2012-03-01
                  Or before a certain date (uncrypted filter is "<= 2012-03-07") :created_on=%3C%3D2012-03-07
                  To fetch issues created after a certain timestamp (uncrypted filter is ">=2014-01-02T08:12:32Z") :created_on=%3E%3D2014-01-02T08:12:32Z
                  To fetch issues updated after a certain timestamp (uncrypted filter is ">=2014-01-02T08:12:32Z") :updated_on=%3E%3D2014-01-02T08:12:32Z
                */

                ResultClass<List<Issues.Issue>> response = new ResultClass<List<Issues.Issue>>();

                string method = "/issues.json?key=" + key;

                if (project_id != null)
                    method += "&project_id=" + project_id;
                if (priority_id != null)
                    method += "&priority_id=" + priority_id;
                if (create_on != null)
                    method += "&create_on=" + create_on;
                if (updated_on != null)
                    method += "&updated_on=" + updated_on;

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(base_address);


                int offset = 0;

                Issues output = new Issues();
                do
                {
                    string range = "&offset=" + offset + "&limit=100";
                    HttpResponseMessage res = client.GetAsync(method + filter + range).Result;
                    string result = res.Content.ReadAsStringAsync().Result;

                    if (res.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        response.ErrorMessage = res.StatusCode + ". " + result;
                        response.Result = null;
                        break;
                    }

                    else
                    {
                        output = Newtonsoft.Json.JsonConvert.DeserializeObject<Issues>(result);
                        response.Result.AddRange(output.issues);
                        offset += 100;

                    }

                } while (output.issues.Any());



                return response;
            }





            public static ResultClass<Projects> GetProjectList(string key, string filter = "&status_id=*", string base_address = "https://pms.ntsw.ir")
            {
                ResultClass<Projects> response = new ResultClass<Projects>();

                string method = "/projects.json?key=" + key;
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(base_address);
                HttpResponseMessage res = client.GetAsync(method + filter).Result;
                string result = res.Content.ReadAsStringAsync().Result;

                if (res.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    response.ErrorMessage = res.StatusCode + ". " + result;
                    response.Result = null;
                }
                else
                {
                    var output = Newtonsoft.Json.JsonConvert.DeserializeObject<Projects>(result);
                    response.Result = output;

                }

                return response;
            }
            public static ResultClass<List<IssueStatus>> GetIssueStatuses(string key, string base_address = "https://pms.ntsw.ir")
            {
                ResultClass<List<IssueStatus>> response = new ResultClass<List<IssueStatus>>();

                string method = "/issue_statuses.json?key=" + key;
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(base_address);
                HttpResponseMessage res = client.GetAsync(method).Result;
                string result = res.Content.ReadAsStringAsync().Result;

                if (res.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    response.ErrorMessage = res.StatusCode + ". " + result;
                    response.Result = null;
                }
                else
                {
                    Dictionary<string, object> output = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result);

                    List<IssueStatus> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<IssueStatus>>(output["issue_statuses"].ToString());

                    response.Result = list;

                }

                return response;
            }

            public static ResultClass<List<IssuePriority>> GetIssuePriorities(string key, string base_address = "https://pms.ntsw.ir")
            {
                ResultClass<List<IssuePriority>> response = new ResultClass<List<IssuePriority>>();

                string method = "/issue_priorities.json?key=" + key;
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(base_address);
                HttpResponseMessage res = client.GetAsync(method).Result;
                string result = res.Content.ReadAsStringAsync().Result;

                if (res.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    response.ErrorMessage = res.StatusCode + ". " + result;
                    response.Result = null;
                }
                else
                {
                    Dictionary<string, object> output = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result);

                    List<IssuePriority> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<IssuePriority>>(output["issue_priorities"].ToString());

                    response.Result = list;

                }

                return response;
            }

            public static ResultClass<int?> CreateIssue(object issue, string key, string base_address = "https://pms.ntsw.ir")
            {

                ResultClass<int?> response = new ResultClass<int?>();
                string method = "/issues.json?key=" + key;

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(base_address);

                HttpResponseMessage res = client.PostAsJsonAsync(method, issue).Result;
                string result = res.Content.ReadAsStringAsync().Result;

                if (res.StatusCode != System.Net.HttpStatusCode.Created)
                {
                    response.ErrorMessage = res.StatusCode + ". " + result;
                    response.Result = null;
                }

                else
                {
                    dynamic issue_res = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);
                    response.Value = issue_res.issue.id;
                    response.Result = response.Value;

                }
                return response;


            }
        }
        public class Nwms
        {

            public enum UserRoleType
            {
                none = 0,
                contractor,
                agent,
                both
            }
            public enum GetCompanyInAnbarResult
            {
                OK,
                EmptyInput,
                NotFound,
                Error,
                NotActiveFound,
                OKNotInCeos,
                OKNotCeos
            }
            public enum GetPersonResult
            {
                OK,
                EmptyInput,
                ServiceError,
                Error
            }

            public static bool UpdateWarehouseAddress(string api_key, string war_id, string post_code, Action<string> error_call, Action<PostAddress> call_back)
            {

                Dictionary<string, object> input = new Dictionary<string, object>();

                string error = "";
                var address = SRL.Projects.Nwms.GetAddress(api_key, post_code, ref error);
                if (address == null)
                {
                    if (error_call != null) error_call(error);
                    return false;
                }

                string[] updater = { $"province:{address.province}", $"city:{address.city}", $"township:{address.township}", $"full_address:{address.address}", $"village:{address.village}", $"country:", $"zone:", $"rural:" };
                input["st"] = updater;
                if (SRL.Projects.Nwms.PutWarehouse(api_key, war_id, input, "https", out error) == false)
                {
                    if (error_call != null) error_call(error);
                    return false;
                }

                if (call_back != null) call_back(address);
                return true;
            }

            public class BaseValueTypes
            {
                public enum Base
                {
                    activity_sectors,
                    supervisor_orgs,
                    warehouse_types,
                    org_creators,
                }

            }
            public enum RegCompanyResult
            {
                OK,
                EmptyInput,
                NotFound,
                Error,
                CoNationalIdEmpty,
                NationalIdEmpty
            }


            public enum NwmsResultType
            {
                OK,
                HttpError,
                WarhouseNotFound,
                MultiFoundSetId,
                NotUniqueFound
            }
            public class SearchResult
            {

                public class Polygon
                {
                    public double lat { get; set; }
                    public double lng { get; set; }
                }

                public class Person
                {
                    public string national_id { get; set; }
                    public string name { get; set; }
                    public string id { get; set; }
                }
                public class SearchComplexResult
                {
                    public int? create_date { get; set; }
                    public string telephone_number { get; set; }
                    public string account_status { get; set; }
                    public string postal_code { get; set; }
                    public string village { get; set; }
                    public string id { get; set; }
                    public string city { get; set; }
                    public IList<Polygon> polygon { get; set; }
                    public string zone { get; set; }
                    public object area { get; set; }
                    public string warehouse_usage_type { get; set; }
                    public string province { get; set; }
                    public object gov_wh_number { get; set; }
                    public string complex_set_type { get; set; }
                    public string full_address { get; set; }
                    public object address { get; set; }
                    public string township { get; set; }
                    public string org_creator_national_id { get; set; }
                    public string warehouse_ownership_type { get; set; }
                    public IList<Person> owners { get; set; }
                    public Person agent { get; set; }
                    public string name { get; set; }
                    public string country { get; set; }
                    public string rural { get; set; }
                }

                public class SearchWarehouseResult
                {
                    public string account_status { get; set; }
                    public string postal_code { get; set; }
                    public List<Person> contractors { get; set; }
                    public string org_creator_national_id { get; set; }
                    public string name { get; set; }
                    public string id { get; set; }
                    public long create_date { get; set; }
                    public List<string> supervisor_org { get; set; }
                    public List<string> activity_sector { get; set; }
                    public string type { get; set; }
                    public string[] st { get; set; }

                }

                public class SearchUserResult
                {
                    public int create_date { get; set; }
                    public string national_id { get; set; }
                    public object phonenumber { get; set; }
                    public string account_status { get; set; }
                    public string alivestatus { get; set; }
                    public string password { get; set; }
                    public object creator_national_id { get; set; }
                    public string lastname { get; set; }
                    public string SSNN_serial { get; set; }
                    public object email { get; set; }
                    public string firstname { get; set; }
                    public string using_two_phase_password { get; set; }
                    public string SSNN_no { get; set; }
                    public object address { get; set; }
                    public string postalcode { get; set; }
                    public object org_creator_national_id { get; set; }
                    public object birthepoch { get; set; }
                    public string mobile { get; set; }
                    public string gender { get; set; }
                    public string userid { get; set; }
                    public string fathername { get; set; }
                }

                public class SearchGoodResult
                {
                    public JsonData json_data { get; set; }
                    public string id { get; set; }

                    public class JsonData
                    {
                        public string key { get; set; }
                        public string title { get; set; }
                    }

                }
            }

            public static void UpdateWarehouseAddressGroup(string api_key, string table_name, string access_file_name, int paralel, Action call_back, Action<Exception> call_error)
            {

                SRL.AccessManagement.AddColumnToAccess("error", table_name, SRL.AccessManagement.AccessDataType.nvarcharmax, access_file_name);
                SRL.AccessManagement.AddColumnToAccess("status", table_name, SRL.AccessManagement.AccessDataType.nvarcharmax, access_file_name);
                SRL.AccessManagement.AddColumnToAccess("address", table_name, SRL.AccessManagement.AccessDataType.nvarcharmax, access_file_name);

                DataTable dt = SRL.AccessManagement.GetDataTableFromAccess(access_file_name, table_name);
                var list_ = SRL.Convertor.ConvertDataTableToList<AddressUpdator>(dt);
                var list = list_.Where(x => x.status == "" || x.status == null || x.status != "OK").ToList();

                SRL.AccessManagement.ExecuteToAccess("update " + table_name + " set id=Trim(id)", access_file_name);

                SRL.ActionManagement.MethodCall.Parallel.ParallelCall<AddressUpdator>(list, paralel.ToString(),
                    UpdateWarehouseAddressParallel, null, call_error, call_back, null, table_name, access_file_name, api_key);
            }
            public class AddressUpdator
            {
                public string id { get; set; }
                public string postal_code { get; set; }
                public string error { get; set; }
                public string status { get; set; }
                public string address { get; set; }

            }
            public static void UpdateWarehouseAddressParallel(List<AddressUpdator> list, BackgroundWorker worker, params object[] args)
            {
                string table_name = args[0].ToString();
                string access_file_name = args[1].ToString();
                string api_key = args[2].ToString();

                foreach (var item in list)
                {
                    string query = "";
                    if (string.IsNullOrWhiteSpace(item.id))
                    {
                        query = $"update {table_name} set status='OK', error='id is null'  where id='' or id is null";
                    }
                    else
                    {
                        AddressUpdator war = new AddressUpdator();
                        UpdateWarehouseAddress(api_key, item.id, item.postal_code, (error) =>
                           {
                               query = $"update { table_name } set status='NOK' , error='{ error}' where id='{ item.id }'";
                           }, (address) =>
                           {
                               query = $"update { table_name } set status='OK', address='{address.address}' where id='{ item.id }'";

                           });

                    }

                    SRL.AccessManagement.ExecuteToAccess(query, access_file_name);
                }
            }

            public static string SendSms(string sms_user, string sms_pass, string esb_user, string esb_pass, string message, string sms_from, string to)
            {
                using (SmsNtswService.SendSMSFromPortTypeClient client = SRL.Web.CreateWcfClient<SmsNtswService.SendSMSFromPortTypeClient>("https://sr-cix.ntsw.ir/services/SendSMSFrom?wsdl"))
                {
                    SRL.Web.AddBasicAuthToSoapHeader(client, esb_user, esb_pass, 0);
                    SmsNtswService.CsOperationManagmentSendSMSFromInfo info = new SmsNtswService.CsOperationManagmentSendSMSFromInfo();
                    info.Message = message;
                    info.To = to;
                    info.From = sms_from;
                    var r = client.SendSMSFrom(sms_user, sms_pass, info);

                    if (r.RecID < 1000)
                    {
                        return r.ErrorMessage;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            public static GetComplexResult GetComplex(string api_key, string postal_code, string protocol, ref string result)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api/" + api_key + "/complex/by-postal-code/");
                    HttpResponseMessage response = client.GetAsync(postal_code).Result;
                    result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        GetComplexResult complex = Newtonsoft.Json.JsonConvert.DeserializeObject<GetComplexResult>(result);
                        return complex;
                    }
                    else
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return null;
                    }
                }
            }
            public static GetComplexResult GetComplexById(string api_key, string id, string protocol, ref string result)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api/" + api_key + "/complex/");
                    HttpResponseMessage response = client.GetAsync(id).Result;
                    result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        GetComplexResult complex = Newtonsoft.Json.JsonConvert.DeserializeObject<GetComplexResult>(result);
                        return complex;
                    }
                    else
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return null;
                    }
                }
            }

            public static string GetUserMobile(string api_key, string national_id, string protocol, ref string error)
            {
                Dictionary<string, object> input = new Dictionary<string, object>();
                input["national_id"] = national_id;
                List<SearchResult.SearchUserResult> list = SearchUserBySize(input, api_key, 0, 1, protocol, out error);
                if (list == null)
                {
                    return null;
                }
                else if (list.Count < 1)
                {
                    error = @"{""user"":""not_found""}";
                    return "";
                }
                else
                {
                    return list[0].mobile;
                }
            }

            public class VirtClass
            {
                public string id { get; set; }
                public string contractor_national_id { get; set; }
                public string name { get; set; }
                public string warehouse_id { get; set; }
                public string account_status { get; set; }
                public string postal_code { get; set; }
                public string org_creator_national_id { get; set; }
                public ST st { get; set; }
            }
            public class ST
            {
                public string province { get; set; }
                public string township { get; set; }
                public string city { get; set; }
            }
            public class EstelamAccessFile
            {
                public class CoOrPersonClass
                {
                    public class CoOrPerson
                    {
                        public string code { get; set; }
                        public string name { get; set; }
                        public string family { get; set; }
                        public string error { get; set; }
                        public string status { get; set; }
                    }

                    public static void StartParallelCallCoOrPerson(List<CoOrPerson> list, BackgroundWorker worker, params object[] args)
                    {
                        string table_name = args[0].ToString();
                        string access_file_name = args[1].ToString();

                        foreach (var item in list)
                        {
                            string query = "";
                            if (string.IsNullOrWhiteSpace(item.code))
                            {
                                query = $"update {table_name} set status='OK', error='code is null'  where code='{item.code}'";
                            }

                            else if (item.code.Length > 10)
                            {

                                string result = "";
                                CompanyClass company = new CompanyClass();
                                var is_ok = SRL.Projects.Nwms.GetCixCompanyByCoNationalId("2050130318", item.code, out company, out result);

                                if (is_ok == false)
                                {
                                    query = $"update {table_name} set status='OK', error='{result}'  where code='{item.code}'";
                                }
                                else
                                {
                                    query = $"update {table_name} set status='OK', name='{company.name}' ,CoAddress='{company.address}'   where code='" + item.code + "'";
                                }
                            }
                            else
                            {
                                PersonClass person = new PersonClass();
                                var get = SRL.Projects.Nwms.GetCixPersonByNationalId("2050130318", item.code, out person);
                                if (get == GetPersonResult.OK)
                                {
                                    query = $"update { table_name } set status='OK' , name='{ person.FirstName }', family='{ person.LastName }', birth='{person.BirthDate}' where code='{ item.code }'";

                                }
                                else
                                {
                                    query = $"update { table_name } set status='OK' , error='{ person.ErrorDescription}' where code='{ item.code }'";

                                }

                            }
                            SRL.AccessManagement.ExecuteToAccess(query, access_file_name);
                        }
                    }

                    public static void CheckCoOrPersonIsCorrect(string access_file_name, string table_name, int paralel, Action<Exception> call_error, Action final_call_back)
                    {
                        SRL.AccessManagement.AddColumnToAccess("name", table_name, SRL.AccessManagement.AccessDataType.nvarcharmax, access_file_name);
                        SRL.AccessManagement.AddColumnToAccess("family", table_name, SRL.AccessManagement.AccessDataType.nvarcharmax, access_file_name);
                        SRL.AccessManagement.AddColumnToAccess("birth", table_name, SRL.AccessManagement.AccessDataType.nvarcharmax, access_file_name);
                        SRL.AccessManagement.AddColumnToAccess("error", table_name, SRL.AccessManagement.AccessDataType.nvarcharmax, access_file_name);
                        SRL.AccessManagement.AddColumnToAccess("status", table_name, SRL.AccessManagement.AccessDataType.nvarcharmax, access_file_name);
                        SRL.AccessManagement.AddColumnToAccess("CoAddress", table_name, SRL.AccessManagement.AccessDataType.nvarcharmax, access_file_name);

                        DataTable dt = SRL.AccessManagement.GetDataTableFromAccess(access_file_name, table_name);
                        var list_ = SRL.Convertor.ConvertDataTableToList<CoOrPerson>(dt);
                        var list = list_.Where(x => x.status == "" || x.status == null || x.status != "OK").ToList();

                        SRL.AccessManagement.ExecuteToAccess("update " + table_name + " set code=Trim(code)", access_file_name);

                        SRL.ActionManagement.MethodCall.Parallel.ParallelCall<CoOrPerson>(list, paralel.ToString(),
                            StartParallelCallCoOrPerson, null, call_error, final_call_back, null, table_name, access_file_name);
                    }
                }
                public class NationalCodeEstelamInput
                {
                    public long ID { get; set; }
                    public string national_code { get; set; }
                    public string status { get; set; }
                    public string error { get; set; }
                    public string correct { get; set; }
                    public string name_family { get; set; }
                    public string exist_anbar { get; set; }
                    public string war_id { get; set; }
                    public string postal_code { get; set; }
                    public string name { get; set; }
                    public string type { get; set; }
                    public string isic { get; set; }
                    public string creator { get; set; }
                }

                public class GetAddressByPostServerResult
                {
                    public long ID { get; set; }
                    public string status { get; set; }
                    public string correct { get; set; }
                    public int ErrorCode { get; set; }
                    public string ErrorMessage { get; set; }
                    public string Location { get; set; }
                    public int LocationCode { get; set; }
                    public string LocationType { get; set; }
                    public string PostCode { get; set; }
                    public string State { get; set; }
                    public string TownShip { get; set; }
                    public string Village { get; set; }
                }
                public static void Estelam(string file_full_path, string table_name, string api_key, string parallel, Action callback)
                {
                    string[] exist_anbar_head = { "ID", "postal_code", "status", "exist_anbar", "correct", "province", "township", "city", "address", "error" };
                    foreach (var item in exist_anbar_head)
                    {
                        SRL.AccessManagement.AddColumnToAccess(item, "table1", SRL.AccessManagement.AccessDataType.nvarcharmax, file_full_path);
                    }
                    try
                    {
                        SRL.AccessManagement.CreateIndex("postal_code", "postal_code", table_name, file_full_path);

                    }
                    catch (Exception ex)
                    {
                    }

                    DataTable table = SRL.AccessManagement.GetDataTableFromAccess(file_full_path, table_name);

                    List<PostCodeEstelamResult> list = SRL.Convertor.ConvertDataTableToList<PostCodeEstelamResult>(table).Where(x => x.status != "OK").ToList();
                    SRL.ActionManagement.MethodCall.Parallel.ParallelCall<PostCodeEstelamResult>(list, parallel, EstelamParallel, null, null, callback, null, api_key, table_name, file_full_path);
                }

                private static void EstelamParallel(List<PostCodeEstelamResult> list, BackgroundWorker bg, params object[] args)
                {
                    string api_key = args[0].ToString();
                    string table_name = args[1].ToString();
                    string file_full_path = args[2].ToString();


                    foreach (var item in list)
                    {
                        if (item.status == "OK") continue;

                        HttpResponseMessage response = new HttpResponseMessage();
                        string message = "";
                        SRL.Projects.Nwms.ComplexByPostCodeResult war =
                        SRL.Projects.Nwms.GetComplexByPostalCode(item.postal_code, api_key, true, "https", out message);
                        string query = "";
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            try
                            {
                                query = "update " + table_name + " set status='OK' , exist_anbar='" + war.warehouse_server + "' , correct='" + war.postal_code_server + "' "
                                    + " , province='" + war.province + "'  , township='" + war.township + "'  , city='" + war.city + "'"
                                    + "  , address='" + war.full_address + "'"
                                    + " where postal_code='" + item.postal_code + "' ;";

                            }
                            catch (Exception ex)
                            {
                                query = "update " + table_name + " set  error='" + ex.Message + "' where ID=" + item.ID + " ;";
                            }
                        }
                        else
                        {
                            query = "update " + table_name + " set status='" + response.StatusCode.ToString() + "' , error='" + message + "' where ID=" + item.ID + " ;";

                        }

                        SRL.AccessManagement.ExecuteToAccess(query, file_full_path);

                    }
                }

                public static void ParallelAddressByPostServer(List<GetAddressByPostServerResult> list, BackgroundWorker bg, params object[] args)
                {
                    //args0: password, args1: client, args2: username, args3: file_full_path, args4: table_name
                    foreach (var item in list)
                    {
                        if (item.status == "OK") continue;
                        var time = new System.Diagnostics.Stopwatch();
                        time.Start();
                        var result = EstelamFromPostServer((PostCodeServiceReference.PostCodeClient)args[1], args[2].ToString(), item.PostCode, args[0].ToString());
                        time.Stop();
                        var sec = time.Elapsed.TotalSeconds;
                        time = new System.Diagnostics.Stopwatch();

                        try
                        {
                            string address = "";

                            if (result.ErrorCode == 0)

                            {
                                address += "استان " + result.State.Trim() + "- شهرستان " + result.TownShip.Trim() + "- بخش " + result.Zone.Trim() + "- " + result.LocationType.Trim() + " " + result.Location.Trim()
                                + "- " + result.Parish.Trim() + "- " + result.PreAvenue.Trim() + "- " + result.Avenue.Trim();
                                if (result.HouseNo != 0) address += "- پلاک " + result.HouseNo;
                                if (!string.IsNullOrWhiteSpace(result.BuildingName)) address += "- ساختمان " + result.BuildingName.Trim();
                                address += "- طبقه " + result.FloorNo.Trim();
                                if (!string.IsNullOrWhiteSpace(result.SideFloor)) address += "- واحد " + result.SideFloor.Trim();
                            }

                            string query = "update " + args[4] + " set status='OK', correct='" + (result.ErrorCode == 0 ?
                       "true" : "false") + "', ErrorCode=" + result.ErrorCode + " , ErrorMessage='" + result.ErrorMessage + "', "
                       + " Location='" + result.Location + "' , LocationCode=" + result.LocationCode + " "
                                + " , LocationType='" + result.LocationType + "'  , State='" + result.State + "'  , TownShip='" + result.TownShip + "'"
                                + "  , Village='" + result.Village + "' , Address='" + address + "'  where PostCode='" + item.PostCode + "' ";

                            var exce = SRL.AccessManagement.ExecuteToAccess(query, args[3].ToString());


                        }
                        catch (Exception ex)
                        {
                            string query = "update " + args[4] + " set  status='" + ex.Message + "' where ID=" + item.ID + " ;";
                            SRL.AccessManagement.ExecuteToAccess(query, args[3].ToString());
                        }
                    }
                }

                public static void ParallelAddressByPostESB(List<GetAddressByPostServerResult> list, BackgroundWorker bg, params object[] args)
                {
                    string service_password = args[0].ToString();
                    var client = (CIXGetAddressByPostcode.GetAddressByPostCodePortTypeClient)args[1];
                    string file_full_path = args[2].ToString();
                    string table_name = args[3].ToString();

                    foreach (var item in list)
                    {
                        if (item.status == "OK") continue;
                        var time = new System.Diagnostics.Stopwatch();
                        time.Start();
                        var result = EstelamFromPostESBServer(client, item.PostCode, service_password);
                        time.Stop();
                        var sec = time.Elapsed.TotalSeconds;
                        time = new System.Diagnostics.Stopwatch();
                        time.Start();
                        try
                        {
                            string address = "";

                            if (result.ErrorCode == 0)

                            {
                                address += "استان " + result.State.Trim() + "- شهرستان " + result.TownShip.Trim() + "- بخش " + result.Zone.Trim() + "- " + result.LocationType.Trim() + " " + result.Location.Trim()
                                + "- " + result.Parish.Trim() + "- " + result.PreAvenue.Trim() + "- " + result.Avenue.Trim();
                                if (result.HouseNo != 0) address += "- پلاک " + result.HouseNo;
                                if (!string.IsNullOrWhiteSpace(result.BuildingName)) address += "- ساختمان " + result.BuildingName.Trim();
                                address += "- طبقه " + result.FloorNo.Trim();
                                if (!string.IsNullOrWhiteSpace(result.SideFloor)) address += "- واحد " + result.SideFloor.Trim();

                                while (address.Contains("  "))
                                {
                                    address = address.Replace("  ", " ");
                                }

                            }

                            string query = "update " + table_name + " set status='OK', correct='" + (result.ErrorCode == 0 ?
                       "true" : "false") + "', ErrorCode=" + result.ErrorCode + " , ErrorMessage='" + result.ErrorMessage + "', "
                       + " Location='" + result.Location + "' , LocationCode=" + result.LocationCode + " "
                                + " , LocationType='" + result.LocationType + "'  , State='" + result.State + "'  , TownShip='" + result.TownShip + "'"
                                + "  , Village='" + result.Village + "' , Address='" + address + "'  where PostCode='" + item.PostCode + "' ";

                            var exce = SRL.AccessManagement.ExecuteToAccess(query, file_full_path);


                        }
                        catch (Exception ex)
                        {
                            string query = "update " + table_name + " set  status='" + ex.Message + "' where ID=" + item.ID + " ;";
                            SRL.AccessManagement.ExecuteToAccess(query, file_full_path);
                        }
                        time.Stop();
                        sec = time.Elapsed.TotalSeconds;
                        time = new System.Diagnostics.Stopwatch();
                    }
                }
                public static void EstelamByNationalCode(string file_full_path, string table_name, string api_key, string parallel, Action callback, Action<Exception> call_error, UserRoleType role_type)
                {
                    // string[] exist_anbar_head = { "ID", "national_code", "status", "error", "correct", "first_name", "last_name", "exist_anbar", "id", "postal_code", "name", "type", "isic", "creator", "province", "township", "city", "address" };
                    foreach (var item in typeof(NationalCodeEstelamInput).GetProperties())
                    {
                        SRL.AccessManagement.AddColumnToAccess(item.Name, "table1", SRL.AccessManagement.AccessDataType.nvarcharmax, file_full_path);
                    }
                    try
                    {
                        SRL.AccessManagement.CreateIndex("national_code", "national_code", table_name, file_full_path);
                    }
                    catch (Exception ex)
                    {
                    }
                    SRL.Database.SqliteEF base_value_db = new SRL.Database.SqliteEF($"Data Source={ Path.Combine(SRL.FileManagement.GetCurrentDirectory(), "BaseValues.sqlite")};Version=3;");
                    List<BaseValueTB> types = base_value_db.Where<BaseValueTB>(new List<SRL.Database.SqliteEF.WhereClause>() {
                            new SRL.Database.SqliteEF.WhereClause { key = nameof(BaseValueTB.type), opt = "=", value = SRL.Projects.Nwms.BaseValueTypes.Base.warehouse_types.ToString() } }).ToList();
                    List<BaseValueTB> orgs = base_value_db.Where<BaseValueTB>(new List<SRL.Database.SqliteEF.WhereClause>() {
                            new SRL.Database.SqliteEF.WhereClause { key = nameof(BaseValueTB.type), opt = "=", value = SRL.Projects.Nwms.BaseValueTypes.Base.org_creators.ToString() } }).ToList();


                    DataTable table = SRL.AccessManagement.GetDataTableFromAccess(file_full_path, table_name);

                    List<NationalCodeEstelamInput> list = SRL.Convertor.ConvertDataTableToList<NationalCodeEstelamInput>(table).Where(x => x.status != "OK").ToList();
                    SRL.ActionManagement.MethodCall.Parallel.ParallelCall<NationalCodeEstelamInput>(list, parallel, EstelamByNationalCodeParallel, null,
                        call_error, callback, null, api_key, table_name, file_full_path, types, orgs);

                }

                private static void EstelamByNationalCodeParallel(List<NationalCodeEstelamInput> list, BackgroundWorker bg, object[] args)
                {
                    string api_key = args[0].ToString();
                    string table_name = args[1].ToString();
                    string file_full_path = args[2].ToString();
                    List<BaseValueTB> types = (List<BaseValueTB>)args[3];
                    List<BaseValueTB> orgs = (List<BaseValueTB>)args[4];

                    foreach (var item in list)
                    {
                        if (item.status == "OK") continue;

                        Dictionary<string, object> input = new Dictionary<string, object>();
                        input["account_status"] = "1";
                        input["person"] = item.national_code;
                        string message = "";
                        List<SearchResult.SearchWarehouseResult> war =
                        SRL.Projects.Nwms.SearchWarehouse(input, api_key, 0, 100, "https", ref message);
                        string query = "";
                        if (war != null)
                        {
                            if (war.Any())
                            {
                                var warehouse = war[0];
                                var contractor_has = warehouse.contractors?.Where(x => x.national_id == item.national_code);
                                if (contractor_has?.Count() > 0)
                                {
                                    var contractor = contractor_has.First();
                                    query = $"update {table_name} set status='OK' , correct='true' ,name_family='{contractor.name}', exist_anbar='{war.Count}', war_id='{warehouse.id}', " +
                                        $"postal_code='{warehouse.postal_code}', name='{warehouse.name}', type='{types.Where(x => x.code == warehouse.type).DefaultIfEmpty(new BaseValueTB { title = "" }).FirstOrDefault().title}', " +
                                        $"isic='{string.Join(", ", warehouse.activity_sector)}', creator='{orgs.Where(x => x.code == warehouse.org_creator_national_id).DefaultIfEmpty(new BaseValueTB { title = "" }).FirstOrDefault().title}'  where national_code='{item.national_code}' ;";
                                }
                                else
                                {
                                    query = $"update {table_name} set status='OK' , correct='true' , exist_anbar='{war.Count}', war_id='{warehouse.id}', " +
                                       $"postal_code='{warehouse.postal_code}', name='{warehouse.name}', type='{types.Where(x => x.code == warehouse.type).DefaultIfEmpty(new BaseValueTB { title = "" }).FirstOrDefault().title}', " +
                                       $"isic='{string.Join(", ", warehouse.activity_sector)}', creator='{orgs.Where(x => x.code == warehouse.org_creator_national_id).DefaultIfEmpty(new BaseValueTB { title = "" }).FirstOrDefault().title}'  where national_code='{item.national_code}' ;";

                                }
                            }
                            else
                            {
                                query = "update " + table_name + $" set  status='OK', exist_anbar='0' where national_code='{item.national_code}' ;";
                            }

                        }
                        else
                        {
                            query = "update " + table_name + " set status='NOK' , error='" + message + "' where ID=" + item.ID + " ;";

                        }

                        SRL.AccessManagement.ExecuteToAccess(query, file_full_path);



                    }
                }



                public static void EstelamFromPost(string file_full_path, string table_name, string api_key, string parallel, string password, string username)
                {
                    DataTable table = SRL.AccessManagement.GetDataTableFromAccess(file_full_path, table_name);
                    List<GetAddressByPostServerResult> list = SRL.Convertor.ConvertDataTableToList<GetAddressByPostServerResult>(table).Where(x => x.status != "OK" || x.status == null || x.status == "").ToList();
                    PostCodeServiceReference.PostCodeClient client = new PostCodeServiceReference.PostCodeClient();
                    SRL.ActionManagement.MethodCall.Parallel.ParallelCall<GetAddressByPostServerResult>(list, parallel, ParallelAddressByPostServer, null, null, null, null, password, client, username, file_full_path, table_name);
                }

                public static void EstelamFromPostESB(string file_full_path, string table_name, string api_key, string parallel, string service_password, string esb_username, string esb_pass, Action callback)
                {
                    SRL.AccessManagement.AddColumnToAccess("ErrorCode", "table1", SRL.AccessManagement.AccessDataType.integer, file_full_path);
                    SRL.AccessManagement.AddColumnToAccess("ErrorMessage", "table1", SRL.AccessManagement.AccessDataType.nvarcharmax, file_full_path);
                    SRL.AccessManagement.AddColumnToAccess("Location", "table1", SRL.AccessManagement.AccessDataType.nvarcharmax, file_full_path);
                    SRL.AccessManagement.AddColumnToAccess("LocationCode", "table1", SRL.AccessManagement.AccessDataType.integer, file_full_path);
                    SRL.AccessManagement.AddColumnToAccess("LocationType", "table1", SRL.AccessManagement.AccessDataType.nvarcharmax, file_full_path);
                    SRL.AccessManagement.AddColumnToAccess("State", "table1", SRL.AccessManagement.AccessDataType.nvarcharmax, file_full_path);
                    SRL.AccessManagement.AddColumnToAccess("TownShip", "table1", SRL.AccessManagement.AccessDataType.nvarcharmax, file_full_path);
                    SRL.AccessManagement.AddColumnToAccess("Village", "table1", SRL.AccessManagement.AccessDataType.nvarcharmax, file_full_path);
                    SRL.AccessManagement.AddColumnToAccess("Address", "table1", SRL.AccessManagement.AccessDataType.nvarcharmax, file_full_path);
                    SRL.AccessManagement.AddColumnToAccess("status", "table1", SRL.AccessManagement.AccessDataType.nvarcharmax, file_full_path);
                    SRL.AccessManagement.AddColumnToAccess("correct", "table1", SRL.AccessManagement.AccessDataType.nvarcharmax, file_full_path);

                    DataTable table = SRL.AccessManagement.GetDataTableFromAccess(file_full_path, table_name);
                    List<GetAddressByPostServerResult> list = SRL.Convertor.ConvertDataTableToList<GetAddressByPostServerResult>(table).Where(x => x.status != "OK" || x.status == null || x.status == "").ToList();
                    CIXGetAddressByPostcode.GetAddressByPostCodePortTypeClient client = new CIXGetAddressByPostcode.GetAddressByPostCodePortTypeClient("GetAddressByPostCodeHttpsSoap11Endpoint");
                    client.ClientCredentials.UserName.UserName = esb_username;
                    client.ClientCredentials.UserName.Password = esb_pass;
                    SRL.ActionManagement.MethodCall.Parallel.ParallelCall<GetAddressByPostServerResult>(list, parallel, ParallelAddressByPostESB, callback, null, null, null, service_password, client, file_full_path, table_name);
                }
                public class EditByIdClass
                {
                    public string code { get; set; }
                    public string status { get; set; }
                    public string error { get; set; }
                }

                public static void EditById(string access_file_name, string table_name, int paralel, string method, string api_key, object filter)
                {
                    SRL.AccessManagement.AddColumnToAccess("status", table_name, SRL.AccessManagement.AccessDataType.nvarcharmax, access_file_name);
                    SRL.AccessManagement.AddColumnToAccess("error", table_name, SRL.AccessManagement.AccessDataType.nvarcharmax, access_file_name);

                    DataTable dt = SRL.AccessManagement.GetDataTableFromAccess(access_file_name, table_name);
                    var list_ = SRL.Convertor.ConvertDataTableToList<EditByIdClass>(dt);
                    var list = list_.Where(x => x.status != "OK" && x.status != "NOK").ToList();

                    SRL.AccessManagement.ExecuteToAccess("update " + table_name + " set code=Trim(code)", access_file_name);

                    SRL.ActionManagement.MethodCall.Parallel.ParallelCall<EditByIdClass>(list, paralel.ToString(), StartParallelEditById,
                        () => { MessageBox.Show("done"); }, null, null, null, table_name, access_file_name, method, api_key, filter);

                }
                public static void StartParallelEditById(List<EditByIdClass> list, BackgroundWorker worker, params object[] args)
                {
                    string table_name = args[0].ToString();
                    string access_file_name = args[1].ToString();
                    string method = args[2].ToString();
                    string api_key = args[3].ToString();
                    object filter = args[4];

                    foreach (var item in list)
                    {
                        if (item.code.Length > 0)
                        {
                            HttpClient client = new HttpClient();
                            client.BaseAddress = new Uri($"https://app.nwms.ir/v2/b2b-api/{api_key}/admin/{method}/");
                            string error = "";
                            var complex = SRL.Projects.Nwms.GetComplexById(api_key, item.code, "https", ref error);
                            if (complex == null) continue;
                            string query = "";
                            if (complex.name.Contains("?"))
                            {
                                query = $"update {table_name} set status='NOK' , error='name contains ?' where code='{item.code}'";

                            }
                            else
                            {
                                var response = client.PutAsJsonAsync(item.code, filter).Result;
                                var result = response.Content.ReadAsStringAsync().Result;
                                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    query = $"update {table_name} set status='OK'  where code='{item.code}'";
                                }
                                else
                                {
                                    query = $"update {table_name} set status='{response.StatusCode.ToString()}' , error='{result}' where code='{item.code}'";
                                }

                            }
                            SRL.AccessManagement.ExecuteToAccess(query, access_file_name);

                        }

                    }
                }

            }

            public class PostCodeEstelamResult
            {
                public long ID { get; set; }
                public string postal_code { get; set; }
                public string status { get; set; }
                public string exist_anbar { get; set; }
                public string correct { get; set; }
                public string province { get; set; }
                public string township { get; set; }
                public string city { get; set; }
                public string address { get; set; }
                public string error { get; set; }
            }
            public static void EstelamFromNwms(string file_full_path, string table_name, string api_key, string parallel, Action callback, Action<Exception> func_error)
            {
                string[] exist_anbar_head = { "status", "exist_anbar", "error" };
                foreach (var item in exist_anbar_head)
                {
                    SRL.AccessManagement.AddColumnToAccess(item, "table1", SRL.AccessManagement.AccessDataType.nvarcharmax, file_full_path);
                }
                try
                {
                    SRL.AccessManagement.CreateIndex("postal_code", "postal_code", table_name, file_full_path);

                }
                catch (Exception ex)
                {
                }

                DataTable table = SRL.AccessManagement.GetDataTableFromAccess(file_full_path, table_name);

                List<PostCodeEstelamResult> list = SRL.Convertor.ConvertDataTableToList<PostCodeEstelamResult>(table).Where(x => x.status != "OK" || x.status == "" || x.status == null).ToList();

                SRL.ActionManagement.MethodCall.Parallel.ParallelCall<PostCodeEstelamResult>(list, parallel, EstelamFromNwmsParallel, null, func_error, callback, null, api_key, table_name, file_full_path);

            }

            private static void EstelamFromNwmsParallel(List<PostCodeEstelamResult> list, BackgroundWorker bg, params object[] args)
            {
                string api_key = args[0].ToString();
                string table_name = args[1].ToString();
                string file_full_path = args[2].ToString();
                Dictionary<string, object> input = new Dictionary<string, object>();
                input["account_status"] = "1";


                foreach (var item in list)
                {
                    if (item.status == "OK") continue;

                    string message = "";
                    input["postal_code"] = item.postal_code;
                    System.Data.OleDb.OleDbParameter[] parameters = null;

                    var war = SRL.Projects.Nwms.SearchWarehouseBySize(api_key,0,1, input,"https", ref message);
                    
                    string query = "";
                    if (war == null)
                    {
                        System.Data.OleDb.OleDbParameter parameter = new System.Data.OleDb.OleDbParameter { ParameterName = "@error", Value = message };
                        parameters = new OleDbParameter[] { parameter };
                        query = $"update {table_name} set status='NOK' , error=@error where ID={item.ID};";

                    }
                    else if (war.Count == 0)
                    {
                        query = $"update {table_name} set status='OK' , exist_anbar='false' where ID={item.ID};";

                    }
                    else
                    {
                        query = $"update {table_name} set status='OK' , exist_anbar='true'  where ID={item.ID};"; ;


                    }
                    
                    SRL.AccessManagement.ExecuteToAccess(query, file_full_path, parameters);

                }
            }


            public static bool DeleteAllContractors(string api_key, string warehouse_id, string protocol, out string error)
            {

                Dictionary<string, object> input = new Dictionary<string, object>();
                input["contractors"] = new List<ContractorListClass>();
                return PutWarehouse(api_key, warehouse_id, input, protocol, out error);
            }
            public static bool DeleteContractor(string api_key, string national_id, GetWarehouseClass warehouse, string protocol, out string error)
            {
                var contractors = new List<ContractorListClass>();
                foreach (var item in warehouse.contractors)
                {
                    if (item.national_id == national_id) continue;
                    contractors.Add(new ContractorListClass { national_id = item.national_id, name = item.name });
                }
                Dictionary<string, object> input = new Dictionary<string, object>();
                input["contractors"] = contractors;
                return PutWarehouse(api_key, warehouse.id, input, protocol, out error);
            }
            public static bool DeleteOwner(string api_key, string national_id, ComplexByPostCodeResult complex, string protocol, ref string result)
            {
                var new_owners = new ComplexOwnerClass();
                new_owners.agent = new ComplexOwnerClass.person();
                foreach (var item in complex?.owners)
                {
                    if (item.national_id == national_id) continue;
                    new_owners.owners.Add(new ComplexOwnerClass.person { national_id = item.national_id, name = item.name });
                }
                if (complex?.agent?.national_id == national_id || complex?.agent?.national_id == null)
                {
                    bool any_owner = new_owners.owners.Any();
                    new_owners.agent.national_id = any_owner ? new_owners.owners.First().national_id : "0000000000";
                    new_owners.agent.name = any_owner ? new_owners.owners.First().name : "UNKNOWN";
                }
                else
                {
                    bool has_agent = !(complex?.agent == null);
                    new_owners.agent.national_id = has_agent ? complex.agent.national_id : "0000000000";
                    new_owners.agent.name = has_agent ? complex.agent.name : "UNKNOWN";

                }
                return SetOwners(api_key, new_owners, complex.id, protocol, ref result);
            }

            public static bool DeleteWarehouse(string api_key, string war_id, string protocol, ref string mes)
            {
                Dictionary<string, object> inp = new Dictionary<string, object>();
                inp["account_status"] = "3";
                mes = "";
                return PutWarehouse(api_key, war_id, inp, protocol, out mes);
            }

            public class ShahkarInputClass
            {
                public string requestId { get; set; }
                public string serviceNumber { get; set; }
                public int serviceType { get; set; }
                public int identificationType { get; set; }
                public string identificationNo { get; set; }
            }

            public static List<CardexResult> GetCardex(string api_key, string contractor_national_id, int start, int size, CardexFilter filters, string protocol, ref string result)
            {
                result = "";
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{ protocol}://app.nwms.ir/v2/b2b-api-imp/{api_key}/{contractor_national_id}/cardex/_search/");
                    Dictionary<string, object> input = new Dictionary<string, object>();
                    Dictionary<string, object> filter = new Dictionary<string, object>();
                    input["filter"] = filter;
                    if (filters != null)
                    {
                        if (!string.IsNullOrWhiteSpace(filters.warehouse_id)) filter["warehouse_id"] = filters.warehouse_id;
                        if (!string.IsNullOrWhiteSpace(filters.user_id)) filter["user_id"] = filters.user_id;
                        if (!string.IsNullOrWhiteSpace(filters.good_id)) filter["good_id"] = filters.good_id;
                        if (!string.IsNullOrWhiteSpace(filters.postal_code)) filter["additional_data.postal_code"] = filters.postal_code;
                        if (!string.IsNullOrWhiteSpace(filters.province)) filter["additional_data.province"] = filters.province;
                        if (!string.IsNullOrWhiteSpace(filters.township)) filter["additional_data.township"] = filters.township;
                        if (!string.IsNullOrWhiteSpace(filters.city)) filter["additional_data.city"] = filters.city;

                    }

                    HttpResponseMessage response = client.PostAsJsonAsync($"{start}/{size}", input).Result;
                    result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result)["data"].ToString();
                        List<CardexResult> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CardexResult>>(data);
                        return list;
                    }
                    else
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return null;
                    }
                }
            }
            public static List<CardexResult> SearchCardex(string api_key, int start, int size, CardexFilter filters, string protocol, ref string result)
            {
                result = "";
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{ protocol}://app.nwms.ir/v2/b2b-api/{api_key}/admin/cardex/_search/");
                    Dictionary<string, object> input = new Dictionary<string, object>();
                    Dictionary<string, object> filter = new Dictionary<string, object>();
                    input["filter"] = filter;
                    if (filters != null)
                    {
                        if (!string.IsNullOrWhiteSpace(filters.warehouse_id)) filter["warehouse_id"] = filters.warehouse_id;
                        if (!string.IsNullOrWhiteSpace(filters.user_id)) filter["user_id"] = filters.user_id;
                        if (!string.IsNullOrWhiteSpace(filters.good_id)) filter["good_id"] = filters.good_id;
                        if (!string.IsNullOrWhiteSpace(filters.postal_code)) filter["additional_data.postal_code"] = filters.postal_code;
                        if (!string.IsNullOrWhiteSpace(filters.province)) filter["additional_data.province"] = filters.province;
                        if (!string.IsNullOrWhiteSpace(filters.township)) filter["additional_data.township"] = filters.township;
                        if (!string.IsNullOrWhiteSpace(filters.city)) filter["additional_data.city"] = filters.city;
                    }

                    HttpResponseMessage response = client.PostAsJsonAsync($"{start}/{size}", input).Result;
                    result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result)["data"].ToString();
                        List<CardexResult> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CardexResult>>(data);
                        return list;
                    }
                    else
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return null;
                    }
                }
            }

            public class ShahkarOutputClass
            {
                public int response { get; set; }
                public string requestId { get; set; }
                public string result { get; set; }
                public string comment { get; set; }
            }
            public class RegUserInput
            {
                public string fathername { get; set; }
                public string firstname { get; set; }
                public string gender { get; set; }
                public string lastname { get; set; }
                public string mobile { get; set; }
                public string password { get; set; }
                public string postalcode { get; set; }
                public string SSNN_no { get; set; }
                public string SSNN_serial { get; set; }
                public string using_two_phase_password { get; set; }
                public string national_id { get; set; }
                public string account_status { get; set; }
            }
            public class ComplexByPostCodeResult
            {

                public bool warehouse_server { get; set; }
                public bool postal_code_server { get; set; }
                public string postal_code { get; set; }
                public string province { get; set; }
                public string village { get; set; }
                public string township { get; set; }
                public string city { get; set; }
                public string full_address { get; set; }

                public string name { get; set; }
                public string id { get; set; }
                public string org_creator_national_id { get; set; }
                public string gov_wh_number { get; set; }

                public Agent agent { get; set; }
                public List<Owner> owners { get; set; }
                public List<Warehouse> warehouses { get; set; }

                public class Agent
                {
                    public string name { get; set; }
                    public string id { get; set; }
                    public string national_id { get; set; }
                    public string mobile { get; set; }
                }


                public class Warehouse
                {
                    public string name { get; set; }
                    public string id { get; set; }
                    public string postal_code { get; set; }
                    public string org_creator_national_id { get; set; }
                    public string creator_national_id { get; set; }
                    public string type { get; set; }
                    public List<string> activity_sector { get; set; }
                    public List<string> supervisor_org { get; set; }
                    public List<Contractor> contractors { get; set; }
                }

                public class Owner
                {

                    public string name { get; set; }
                    public string national_id { get; set; }
                    public string mobile { get; set; }
                }

                public class Contractor
                {

                    public string name { get; set; }
                    public string id { get; set; }
                    public string national_id { get; set; }
                    public string mobile { get; set; }
                }
            }

            public class GetComplexResult
            {
                public object create_date { get; set; }
                public string telephone_number { get; set; }
                public Agent agent { get; set; }
                public string account_status { get; set; }
                public string id { get; set; }
                public string city { get; set; }
                public List<Warehouse> warehouses { get; set; }
                public string province { get; set; }
                public string full_address { get; set; }
                public string township { get; set; }
                public string org_creator_national_id { get; set; }
                public List<Owner> owners { get; set; }
                public string name { get; set; }
                public string creator_national_id { get; set; }
                public List<string> st { get; set; }

                public class Agent
                {
                    public string name { get; set; }
                    public string national_id { get; set; }
                    public string id { get; set; }
                }

                public class Contractor
                {
                    public string name { get; set; }
                    public string national_id { get; set; }
                    public int start_epoch { get; set; }
                    public string account_status { get; set; }
                    public int expire_epoch { get; set; }
                    public string id { get; set; }
                }

                public class Warehouse
                {
                    public int create_date { get; set; }
                    public List<string> supervisor_org { get; set; }
                    public string account_status { get; set; }
                    public string id { get; set; }
                    public List<string> activity_sector { get; set; }
                    public string type { get; set; }
                    public List<Contractor> contractors { get; set; }
                    public string org_creator_national_id { get; set; }
                    public object gov_warehouse_no { get; set; }
                    public string name { get; set; }
                    public string creator_national_id { get; set; }
                }

                public class Owner
                {
                    public string national_id { get; set; }
                    public string name { get; set; }
                    public string id { get; set; }
                }

            }

            public static bool DeleteApiKey(string api_key, string key_id, string protocol, out string result)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{protocol}://admin-app.nwms.ir/v2/b2b-api/" + api_key + "/json_store/");
                    HttpResponseMessage response = client.DeleteAsync(key_id).Result;
                    result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return true;
                    }
                    else
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return false;
                    }
                }
            }

            public class PersonClass
            {
                public string national_id { get; set; }
                public string City { get; set; }
                public string FirstName { get; set; }
                public string LastName { get; set; }
                public string IdentitySerial { get; set; }
                public string IdentityNo { get; set; }
                public string BirthDate { get; set; }
                public string FatherName { get; set; }
                public string SupervisorNationalCode { get; set; }
                public string IdentitySeries { get; set; }
                public string last_sent { get; set; }
                public string Town { get; set; }
                public string Gender { get; set; }
                public string ErrorDescription { get; set; }
                public string HttpCode { get; set; }

            }
            public class AnbarUserClassResult
            {
                public ResultType result_type { get; set; } = new ResultType();
                public AnbarUserClass user { get; set; } = new AnbarUserClass();
                public string error { get; set; }


                public enum ResultType
                {
                    OK,
                    Error,
                    Inactive

                }
            }
            public class AnbarUserClass
            {

                public int create_date { get; set; }
                public string national_id { get; set; }
                public string modifier_national_id { get; set; }
                public string phonenumber { get; set; }
                public string account_status { get; set; }
                public string alivestatus { get; set; }

                public string password { get; set; }
                public string creator_national_id { get; set; }
                public string lastname { get; set; }
                public object SSNN_serial { get; set; }
                public object email { get; set; }
                public object files { get; set; }
                public string firstname { get; set; }
                public string using_two_phase_password { get; set; }
                public string SSNN_no { get; set; }
                public object address { get; set; }
                public string postalcode { get; set; }
                public object org_creator_national_id { get; set; }
                public string birthepoch { get; set; }
                public object user_national_card_image { get; set; }
                public object user_profile_image { get; set; }
                public string mobile { get; set; }
                public string gender { get; set; }
                public string userid { get; set; }
                public string fathername { get; set; }


            }
            public class SahaUserClass
            {
                public string Town { get; set; }
                public string City { get; set; }
                public string NationalCode { get; set; }
                public string FirstName { get; set; }
                public string IdentityNo { get; set; }
                public string ErrorDescription { get; set; }
                public string Gender { get; set; }
                public string IdentitySeries { get; set; }
                public bool raw_result { get; set; }
                public string BirthDate { get; set; }
                public int ErrorCode { get; set; }
                public string FatherName { get; set; }
                public string LastName { get; set; }
                public string IsLive { get; set; }
                public object DeathDate { get; set; }
                public string IdentitySerial { get; set; }
                public object Vilage { get; set; }
                public string SupervisorNationalCode { get; set; }
            }
            public class CompanyClass
            {
                public long ID { get; set; }
                public string co_national_id { get; set; }
                public string name { get; set; }
                public string error_name { get; set; }
                public string address { get; set; }
                public string RegisterNumber { get; set; }
            }
            public class GetWarehouseClass
            {
                public int create_date { get; set; }
                public List<string> supervisor_org { get; set; } = new List<string>();
                public string account_status { get; set; }
                public string postal_code { get; set; }
                public string id { get; set; }
                public Complex complex { get; set; }
                public List<string> activity_sector { get; set; } = new List<string>();
                public string type { get; set; }
                public List<Contractor> contractors { get; set; } = new List<Contractor>();
                public string org_creator_national_id { get; set; }
                public List<Polygon> polygon { get; set; } = new List<Polygon>();
                public string gov_warehouse_no { get; set; }
                public string name { get; set; }
                public string creator_national_id { get; set; }
                public List<string> st { get; set; } = new List<string>();

                public class Agent
                {
                    public string name { get; set; }
                    public string national_id { get; set; }
                }

                public class Polygon
                {
                    public double lat { get; set; }
                    public double lng { get; set; }
                }

                public class Complex
                {
                    public string province { get; set; }
                    public string city { get; set; }
                    public string name { get; set; }
                    public string village { get; set; }
                    public Agent agent { get; set; }
                    public string full_address { get; set; }
                    public string township { get; set; }
                    public IList<Polygon> polygon { get; set; }
                }

                public class Contractor
                {
                    public object org_creator_national_id { get; set; }
                    public string name { get; set; }
                    public string title { get; set; }
                    public string national_id { get; set; }
                    public object creator_national_id { get; set; }
                    public int start_epoch { get; set; }
                    public string common_name { get; set; }
                    public string account_status { get; set; }
                    public object modifier_national_id { get; set; }
                    public int expire_epoch { get; set; }
                    public string id { get; set; }
                }

            }
            public class CompanyListClass
            {
                public object website { get; set; }
                public string register_code { get; set; }
                public string national_id { get; set; }
                public string commercial_code { get; set; }
                public string register_epoch { get; set; }
                public string account_status { get; set; }
                public string phonenumber { get; set; }
                public string modifier_national_id { get; set; }
                public object roozname_rasmi_url { get; set; }
                public string id { get; set; }
                public string en_name { get; set; }
                public object register_city { get; set; }
                public object email { get; set; }
                public object files { get; set; }
                public string fax { get; set; }
                public object ceo_expire_epoch { get; set; }
                public string ceo_national_id { get; set; }
                public object address { get; set; }
                public object org_creator_national_id { get; set; }
                public string name { get; set; }
                public string creator_national_id { get; set; }
                public string postal_code { get; set; }
                public Ceo ceo { get; set; }

                public class Ceo
                {
                    public object org_creator_national_id { get; set; }
                    public string name { get; set; }
                    public string title { get; set; }
                    public string national_id { get; set; }
                    public object creator_national_id { get; set; }
                    public int start_epoch { get; set; }
                    public string common_name { get; set; }
                    public string account_status { get; set; }
                    public object modifier_national_id { get; set; }
                    public int expire_epoch { get; set; }
                    public string id { get; set; }
                    public string ceo_mobile { get; set; }
                }


            }

            public class PostalCodeFromPostClass
            {
                public int ErrorCode { get; set; }
                public object ErrorMessage { get; set; }
                public string BuildingName { get; set; }
                public string Zone { get; set; }
                public string LocationType { get; set; }
                public int Error { get; set; }
                public bool raw_result { get; set; }
                public string Description { get; set; }
                public string FloorNo { get; set; }
                public string State { get; set; }
                public object SideFloor { get; set; }
                public string PostCode { get; set; }
                public double HouseNo { get; set; }
                public object Village { get; set; }
                public string PreAvenue { get; set; }
                public string Avenue { get; set; }
                public string TownShip { get; set; }
                public string Parish { get; set; }
                public string LocationCode { get; set; }
                public string Location { get; set; }
            }

            public class AnbarOperation
            {

                public class AdditionalData
                {
                    public string good_owneragent { get; set; }
                    public string owner_phone { get; set; }
                    public double? bol_date { get; set; }
                    public string bol_tid { get; set; }
                    public string bol_series { get; set; }
                    public string bol_serial { get; set; }
                    public string draft_tid { get; set; }
                    public double? draft_date { get; set; }
                    public string org_creator_nationa_id { get; set; }
                    public string postal_code { get; set; }



                }

                public class ReceiptItem
                {
                    public string category_id { get; set; }
                    public string taxonomy_id { get; set; }
                    public string good_id { get; set; }
                    public string good_desc { get; set; }
                    public string measurement_unit { get; set; }
                    public double count { get; set; }
                    public string production_type { get; set; }
                    public double? total_weight { get; set; }
                    public string package_type { get; set; }
                    public double? package_count { get; set; }
                    public double? item_value { get; set; }
                    public string location { get; set; }
                    public double? production_date { get; set; }
                    public double? expire_date { get; set; }
                    public string description { get; set; }
                    public List<object> receipt_shares = new List<object>();
                    public List<object> tracking_list = new List<object>();

                }

                public class SimpleReceipt
                {
                    public string number { get; set; }
                    public string owner_name { get; set; }
                    public string owner { get; set; }
                    public Nullable<double> rcp_date { get; set; }
                    public string warehouse_id { get; set; }//virt_id
                    public string postal_code { get; set; }
                    public string doc_type { get; set; }
                    public string vehicle_number { get; set; }
                    public string driver_national_id { get; set; }
                    public string driver { get; set; }
                    public string insurance_name { get; set; }
                    public double? insurance_date { get; set; }
                    public string insurance_number { get; set; }
                    public double? weight_impure { get; set; }
                    public double? weight_pure { get; set; }
                    public AdditionalData additional_data { get; set; }
                    public List<ReceiptItem> receipt_items = new List<ReceiptItem>();
                    public string rcp_creator { get; set; }
                    public double? create_date { get; set; }
                    public string id { get; set; }
                    public string contractor { get; set; }
                    public string status { get; set; }//TEMPORARY,PERMANENT,CANCELED  
                    public string warehouse_parent_id { get; set; }// warehouse_id


                }

                public class SimpleGoodIssue
                {
                    public string doc_type { get; set; }
                    public double? weight_pure { get; set; }
                    public string vehicle_number { get; set; }
                    public string number { get; set; }
                    public string warehouse_id { get; set; }
                    public string postal_code { get; set; }
                    public string owner { get; set; }
                    public string keeper { get; set; }
                    public double goods_issue_date { get; set; }
                    public double? weight_impure { get; set; }
                    public string driver_national_id { get; set; }
                    public string driver { get; set; }
                    public string insurance_name { get; set; }
                    public double? insurance_date { get; set; }
                    public string insurance_number { get; set; }
                    public string owner_name { get; set; }
                    public AdditionalData additional_data { get; set; } = new AdditionalData();
                    public List<GoodsIssueItem> goods_issue_items = new List<GoodsIssueItem>();
                    public string goods_issue_creator { get; set; }
                    public double? create_date { get; set; }
                    public string id { get; set; }
                    public string contractor { get; set; }
                    public string status { get; set; }//TEMPORARY,PERMANENT,CANCELED 
                    public string warehouse_parent_id { get; set; }// warehouse_id


                }

                public class GoodsIssueItem
                {
                    public string category_id { get; set; }
                    public string taxonomy_id { get; set; }
                    public string good_id { get; set; }
                    public string good_desc { get; set; }
                    public string measurement_unit { get; set; }
                    public double count { get; set; }
                    public string production_type { get; set; }
                    public double? total_weight { get; set; }
                    public string package_type { get; set; }
                    public double? package_count { get; set; }
                    public double? production_date { get; set; }
                    public double? expire_date { get; set; }
                    public string location { get; set; }

                }


                public static bool Receipt(string api_key, string contractor_national_id, AnbarOperation.SimpleReceipt simple_receipt, string protocol, out string id, ref object input_sent)
                {
                    System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api-imp/");
                    input_sent = simple_receipt;

                    input_sent = SRL.Json.RemoveEmptyKeys(simple_receipt, true, true);
                    HttpResponseMessage response = client.PostAsJsonAsync(api_key + "/" + contractor_national_id + "/receipt/simple", input_sent).Result;

                    id = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        id = SRL.Json.IsJson(id) ? id : response.StatusCode.ToString();
                        return false;
                    }
                    else
                    {
                        //ثبت سند بصورت موقت انجام شد
                        Dictionary<string, object> output = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(id);
                        id = output["id"].ToString();
                        int version = int.Parse(output["version"].ToString());
                        return true;
                    }


                }
                public static bool ReceiptFinal(string api_key, string contractor_national_id, AnbarOperation.SimpleReceipt simple_receipt, string protocol, bool final_if_EXISTS, out string id, ref object input_sent)
                {
                    System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api-imp/");
                    input_sent = simple_receipt;

                    input_sent = SRL.Json.RemoveEmptyKeys(simple_receipt, true, true);
                    HttpResponseMessage response = client.PostAsJsonAsync(api_key + "/" + contractor_national_id + "/receipt/receipt_finalize", input_sent).Result;

                    id = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        id = SRL.Json.IsJson(id) ? id : response.StatusCode.ToString();
                        if (id == @"{""number"": ""EXISTS_BEFORE""}" && final_if_EXISTS)
                        {
                            var filters = new SearchFilters { number = simple_receipt.number };
                            if (!string.IsNullOrWhiteSpace(simple_receipt.postal_code)) filters.postal_code = simple_receipt.postal_code;
                            else if (!string.IsNullOrWhiteSpace(simple_receipt.warehouse_id)) filters.warehouse_id = simple_receipt.warehouse_id;
                            var list = SearchReceipt(api_key, 0, 1, filters, protocol, ref id);
                            if (list == null)
                            {
                                return false;
                            }
                            else if (!list.Any())
                            {
                                return false;
                            }
                            else
                            {
                                string to_final = list.First().id;
                                if (FinalizeReceipt(to_final, api_key, contractor_national_id, protocol, out id))
                                {
                                    id = to_final;
                                    return true;
                                }
                                else
                                {
                                    if (id == @"{""status"": ""already permanent""}")
                                    {
                                        id = to_final;
                                        return true;
                                    }
                                    else return false;
                                }
                            }

                        }
                        else return false;
                    }
                    else
                    {
                        //ثبت سند بصورت موقت انجام شد
                        Dictionary<string, object> output = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(id);
                        id = output["id"].ToString();
                        int version = int.Parse(output["version"].ToString());
                        return true;
                    }


                }

                public static SimpleReceipt CreateSimpleReceipt(string postal_code, string warehouse_id)
                {
                    AnbarOperation.SimpleReceipt simple_receipt = new AnbarOperation.SimpleReceipt();
                    //شماره داخلی رسید - این شماره توسط انبار وارد می شود و باید غیرتکراری باشد  *
                    simple_receipt.number = Guid.NewGuid().ToString();
                    //تاریخ صدور رسید از نوع epoch:*
                    //تاریخ صدور باید به میلادی تبدیل و سپس از 1970/1/1 کم شود و تعداد ثانیه ها بدست آید
                    simple_receipt.rcp_date = (new System.Globalization.PersianCalendar().ToDateTime(1396, 8, 15, 0, 0, 0, 0) - new DateTime(1970, 1, 1)).TotalSeconds;

                    //کد پستی انبار*
                    simple_receipt.postal_code = postal_code;


                    //در صورتی که برای کدملی و کدپستی بیش از یک انبار وجود داشته باشد باید متغیر زیر که کدانبار می باشد
                    //ارسال گردد. در صورتی که یک انبار ثبت شده است همان کدپستی کافی است
                    if (!string.IsNullOrWhiteSpace(warehouse_id)) simple_receipt.warehouse_id = warehouse_id;

                    //نوع رسید*
                    //0 برای بدون مرجع
                    //1 برای بارنامه
                    //2 برای حواله
                    simple_receipt.doc_type = "0";
                    //کدملی راننده:*
                    simple_receipt.driver_national_id = "";
                    //نام و نام خانوادگی راننده:
                    simple_receipt.driver = "سهیل رمضان زاده";
                    //شماره پلاک*
                    simple_receipt.vehicle_number = "45ب874ایران65";
                    //تاریخ صدور بیمه
                    simple_receipt.insurance_date = 1640118600;
                    //نام بیمه نامه
                    simple_receipt.insurance_name = "بیمه آسیا";
                    //شماره بیمه نامه
                    simple_receipt.insurance_number = "1254";
                    //کدملی یا شناسه ملی مالک کالا:*
                    simple_receipt.owner = "";
                    //نام کامل مالک کالا حقیقی یا حقوقی
                    simple_receipt.owner_name = "سهیل رمضان زاده";
                    //وزن بار بهمراه ناوگان- وزن ناخالص به کیلوگرم*
                    simple_receipt.weight_impure = 2500;
                    //وزن بار بدون ناوگان-وزن خالص به کیلوگرم*
                    simple_receipt.weight_pure = 500;

                    //اقلام کالایی رسید بصورت زیر بدست می آید
                    AnbarOperation.ReceiptItem receipt_item = new AnbarOperation.ReceiptItem();
                    //شناسه کالا:*
                    receipt_item.good_id = "0002";
                    //واحد اندازگیری:
                    // 0002	کیلوگرم	برای
                    //0003	مثقال	برای مثقال
                    //0004	لیتر	برای
                    //0005	تن	برای
                    //0001	برای عدد
                    //؟
                    receipt_item.measurement_unit = "0001";
                    //مقدار برحسب واحد اندازه گیری:*
                    receipt_item.count = 10;
                    //نوع بسته بندی:
                    //002	کارتن	
                    //001	پاکت	
                    //003	پالت	
                    //004	قراصه	
                    //005	گونی	
                    //006	بدون بسته بندی	
                    //007	فلّه
                    receipt_item.package_type = "002";
                    //تعداد بسته
                    receipt_item.package_count = 2;

                    receipt_item.location = "محل نگهداری";
                    //تاریخ تولید epoch:
                    receipt_item.production_date = 1324499400;
                    //تاریخ انقضا epoch:*
                    //باید بعد از زمان جاری و بعد از تاریخ تولید باشد
                    receipt_item.expire_date = 1684771400;
                    //وزن کل ردیف کالا به کیلوگرم
                    receipt_item.total_weight = 5000;

                    //می تواند چندین ریف کالایی در یک سند افزود
                    simple_receipt.receipt_items.Add(receipt_item);

                    //اطلاعات اختیاری:
                    AnbarOperation.AdditionalData additional_data = new AnbarOperation.AdditionalData();
                    additional_data.good_owneragent = "نام و نام خانوادگی نماینده مالک";
                    additional_data.owner_phone = "تلفن مالک یا نماینده مالک";

                    simple_receipt.additional_data = additional_data;

                    return simple_receipt;


                }

                public static bool FinalizeReceipt(string id, string api_key, string contractor_national_id, string protocol, out string result_final)
                {

                    //جهت نهایی سازی سند:
                    System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api-imp/");
                    HttpResponseMessage response_final = client.PostAsync(api_key + "/" + contractor_national_id + "/receipt/" + id + "/finalize", null).Result;

                    result_final = response_final.Content.ReadAsStringAsync().Result;
                    if (response_final.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        result_final = SRL.Json.IsJson(result_final) ? result_final : response_final.StatusCode.ToString();
                        return false;
                    }
                    else
                    {//در این صورت سند قطعی شده است
                        Dictionary<string, object> output_final = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result_final);
                        string status = output_final["status"].ToString(); // status == "PERMANENT"
                        if (status == "PERMANENT") return true;
                        else return false;

                    }

                }

                private void btnHavale_Click(object sender, EventArgs e)
                {//متد ثبت موقت حواله
                    HttpResponseMessage response = SendGoodIssueSimple("https");
                    string result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        Console.WriteLine(result);

                    }
                    else
                    {
                        //ثبت سند بصورت موقت انجام شد
                        Dictionary<string, object> output = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                        string id = output["id"].ToString();
                        int version = int.Parse(output["version"].ToString());
                        //جهت نهایی سازی سند:
                        HttpResponseMessage response_final = FinalizeGoodIsuue(id, "https");
                        string result_final = response_final.Content.ReadAsStringAsync().Result;
                        if (response_final.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            Console.WriteLine(result_final);
                        }
                        else
                        {//در این صورت سند قطعی شده است
                            Dictionary<string, object> output_final = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result_final);
                            string status = output_final["status"].ToString(); // status == "PERMANENT"
                        }
                    }

                }

                private HttpResponseMessage FinalizeGoodIsuue(string id, string protocol)
                {
                    System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api-imp/");
                    return client.PostAsync("2050130000/2050130000/goods_issue/" + id + "/finalize", null).Result;
                }

                private HttpResponseMessage SendGoodIssueSimple(string protocol)
                {
                    AnbarOperation.SimpleGoodIssue simple_good_issue = new AnbarOperation.SimpleGoodIssue();
                    //شماره داخلی حواله- این شماره توسط انبار وارد می شود و باید غیرتکراری باشد  *
                    //simple_good_issue.number = Guid.NewGuid().ToString();
                    ////تاریخ صدور رسید از نوع epoch:*
                    ////تاریخ صدور باید به میلادی تبدیل و سپس از 1970/1/1 کم شود و تعداد ثانیه ها بدست آید
                    //simple_good_issue.goods_issue_date = (new System.Globalization.PersianCalendar().ToDateTime(1395, 12, 2, 14, 12, 01, 0) - new DateTime(1970, 1, 1)).TotalSeconds;

                    ////کد پستی انبار*
                    //simple_good_issue.postal_code = "5691947637";
                    ////در صورتی که برای کدملی و کدپستی بیش از یک انبار وجود داشته باشد باید متغیر زیر که کدانبار می باشد
                    ////ارسال گردد. در صورتی که یک انبار ثبت شده است همان کدپستی کافی است
                    ////simple_receipt.warehouse_id = "a8647d57dd784867ba5c172eb59156f4";

                    ////نوع رسید*
                    ////0 برای بدون مرجع
                    ////1 برای رسید
                    ////2 برای معرفی نامه
                    //simple_good_issue.doc_type = "0";
                    ////کدملی انباردار
                    //simple_good_issue.keeper = "";
                    ////کدملی راننده:*
                    //simple_good_issue.driver_national_id = "";
                    ////نام و نام خانوادگی راننده:
                    //simple_good_issue.driver = "سهیل رمضان زاده";
                    ////شماره پلاک*
                    //simple_good_issue.vehicle_number = "45ب874ایران65";
                    ////تاریخ صدور بیمه
                    //simple_good_issue.insurance_date = 1640118600;
                    ////نام بیمه نامه
                    //simple_good_issue.insurance_name = "بیمه آسیا";
                    ////شماره بیمه نامه
                    //simple_good_issue.insurance_number = "1254";
                    ////کدملی یا شناسه ملی مالک کالا:*
                    //simple_good_issue.owner = "2050130351";
                    ////نام کامل مالک کالا حقیقی یا حقوقی
                    //simple_good_issue.owner_name = "سهیل رمضان زاده";
                    ////وزن بار بهمراه ناوگان- وزن ناخالص به کیلوگرم*
                    //simple_good_issue.weight_impure = 2500;
                    ////وزن بار بدون ناوگان-وزن خالص به کیلوگرم*
                    //simple_good_issue.weight_pure = 500;

                    ////اقلام کالایی حواله بصورت زیر بدست می آید
                    //AnbarOperation.GoodsIssueItem good_issue_item = new AnbarOperation.GoodsIssueItem();
                    ////شناسه کالا:*
                    //good_issue_item.good_id = "0002";
                    ////واحد اندازگیری:
                    //// 0002	کیلوگرم	برای
                    ////0003	مثقال	برای مثقال
                    ////0004	لیتر	برای
                    ////0005	تن	برای
                    ////0001	برای عدد
                    ////؟
                    //good_issue_item.measurement_unit = "0001";
                    ////مقدار برحسب واحد اندازه گیری:*
                    //good_issue_item.count = 10;
                    ////نوع بسته بندی:
                    ////002	کارتن	
                    ////001	پاکت	
                    ////003	پالت	
                    ////004	قراصه	
                    ////005	گونی	
                    ////006	بدون بسته بندی	
                    ////007	فلّه
                    //good_issue_item.package_type = "002";
                    ////تعداد بسته
                    //good_issue_item.package_count = 2;

                    //good_issue_item.location = "محل نگهداری";
                    ////تاریخ تولید epoch:
                    //good_issue_item.production_date = 1324499400;
                    ////تاریخ انقضا epoch:*
                    ////باید بعد از زمان جاری و بعد از تاریخ تولید باشد
                    //good_issue_item.expire_date = 1684771400;
                    ////وزن کل ردیف کالا به کیلوگرم
                    //good_issue_item.total_weight = 5000;

                    ////می تواند چندین ریف کالایی در یک سند افزود
                    //simple_good_issue.goods_issue_items.Add(good_issue_item);


                    System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api-imp/");
                    string input = JsonConvert.SerializeObject(simple_good_issue, Formatting.Indented);
                    return client.PostAsJsonAsync("2050130000/2050130000/goods_issue/simple", simple_good_issue).Result;
                }

                public static bool Havale(string api_key, string contractor_national_id, AnbarOperation.SimpleGoodIssue simple_havale, string protocol, out string result, ref object sent_data)
                {
                    System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api-imp/");
                    sent_data = simple_havale;
                    sent_data = SRL.Json.RemoveEmptyKeys(simple_havale, true, true);
                    HttpResponseMessage response = client.PostAsJsonAsync(api_key + "/" + contractor_national_id + "/goods_issue/simple", sent_data).Result;

                    result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return false;
                    }
                    else
                    {
                        //ثبت سند بصورت موقت انجام شد
                        Dictionary<string, object> output = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                        string id = output["id"].ToString();
                        int version = int.Parse(output["version"].ToString());
                        result = id;
                        return true;
                    }

                }

                public static bool HavaleFinal(string api_key, string contractor_national_id, AnbarOperation.SimpleGoodIssue simple_havale, string protocol, out string result, ref object sent_data)
                {
                    System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api-imp/");
                    sent_data = simple_havale;
                    sent_data = SRL.Json.RemoveEmptyKeys(simple_havale, true, true);
                    HttpResponseMessage response = client.PostAsJsonAsync(api_key + "/" + contractor_national_id + "/goods_issue/goods_issue_finalize", sent_data).Result;

                    result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return false;
                    }
                    else
                    {
                        Dictionary<string, object> output = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                        string id = output["id"].ToString();
                        int version = int.Parse(output["version"].ToString());
                        result = id;
                        return true;
                    }

                }
                public static bool FinalizeGoodIssue(string id, string api_key, string contractor_national_id, string protocol, out string finalization)
                {
                    //جهت نهایی سازی سند:
                    System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api-imp/");
                    HttpResponseMessage response_final = client.PostAsync(api_key + "/" + contractor_national_id + "/goods_issue/" + id + "/finalize", null).Result;

                    finalization = response_final.Content.ReadAsStringAsync().Result;
                    if (response_final.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        finalization = SRL.Json.IsJson(finalization) ? finalization : response_final.StatusCode.ToString();
                        return false;
                    }
                    else
                    {//در این صورت سند قطعی شده است
                        Dictionary<string, object> output_final = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(finalization);
                        string status = output_final["status"].ToString(); // status == "PERMANENT"
                        if (status == "PERMANENT") return true;
                        else return false;

                    }
                }

                public static bool ExchangePre(string api_key, string contractor_national_id, AnbarOperation.ExchangeInput deal, string protocol, out string result)
                {
                    System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api-imp/");
                    object input = deal;
                    if (string.IsNullOrWhiteSpace(deal.goods_issue.warehouse_id))
                    {
                        input = SRL.Json.RemoveEmptyKeys(deal, true, true);
                    }
                    HttpResponseMessage response = client.PostAsJsonAsync(api_key + "/" + contractor_national_id + "/exchange", input).Result;

                    result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return false;
                    }
                    else
                    {
                        //ثبت سند بصورت موقت انجام شد
                        Dictionary<string, object> output = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                        string id = output["op_id"].ToString();
                        result = id;
                        return true;
                    }
                }

                public static bool FinalizeExchange(string id, string api_key, string contractor_national_id, string protocol, out string finalization)
                {
                    //جهت نهایی سازی سند:
                    using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
                    {

                        client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api-imp/");
                        HttpResponseMessage response_final = client.PostAsync(api_key + "/" + contractor_national_id + "/exchange/" + id + "/finalize", null).Result;

                        finalization = response_final.Content.ReadAsStringAsync().Result;
                        if (response_final.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            finalization = SRL.Json.IsJson(finalization) ? finalization : response_final.StatusCode.ToString();
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }

                public static bool CancelReceipt(string id, string api_key, string contractor, string protocol, out string result_final)
                {
                    System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api-imp/");
                    HttpResponseMessage response_final = client.PostAsync(api_key + "/" + contractor + "/receipt/" + id + "/cancel", null).Result;

                    result_final = response_final.Content.ReadAsStringAsync().Result;
                    if (response_final.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        result_final = SRL.Json.IsJson(result_final) ? result_final : response_final.StatusCode.ToString();
                        return false;
                    }
                    else
                    {
                        Dictionary<string, object> output_final = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result_final);
                        string status = output_final["status"].ToString();
                        if (status == "CANCELED") return true;
                        else return false;

                    }
                }

                public static bool CancelHavale(string id, string api_key, string contractor, string protocol, out string result_final)
                {
                    System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api-imp/");
                    HttpResponseMessage response_final = client.PostAsync(api_key + "/" + contractor + "/goods_issue/" + id + "/cancel", null).Result;

                    result_final = response_final.Content.ReadAsStringAsync().Result;
                    if (response_final.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        result_final = SRL.Json.IsJson(result_final) ? result_final : response_final.StatusCode.ToString();
                        return false;
                    }
                    else
                    {
                        Dictionary<string, object> output_final = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result_final);
                        string status = output_final["status"].ToString();
                        if (status == "CANCELED") return true;
                        else return false;

                    }
                }

                public static List<SimpleReceipt> GetReceiptList(string api_key, string contractor_national_id, int start, int size, SearchFilters _filters, string protocol, ref string result)
                {
                    using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
                    {
                        Dictionary<string, object> input = new Dictionary<string, object>();

                        Dictionary<string, object> filter = new Dictionary<string, object>();
                        input["filter"] = filter;
                        if (_filters != null)
                        {
                            if (!string.IsNullOrWhiteSpace(_filters.number)) filter["number"] = _filters.number;
                            if (!string.IsNullOrWhiteSpace(_filters.owner)) filter["owner"] = _filters.owner;
                            if (_filters.status != OptStatus.None) filter["status"] = _filters.status.ToString();
                            if (_filters.date_from != null || _filters.date_to != null)
                            {
                                Dictionary<string, object> date_filter = new Dictionary<string, object>();
                                if (_filters.date_from != null) date_filter["$gte"] = SRL.Convertor.EnglishDateTimeToUnixEpoch((DateTime)_filters.date_from);
                                if (_filters.date_to != null) date_filter["$lte"] = SRL.Convertor.EnglishDateTimeToUnixEpoch((DateTime)_filters.date_to);
                                filter["rcp_date"] = date_filter;
                            }
                        }
                        if (string.IsNullOrWhiteSpace(contractor_national_id))
                        {
                            string virt = "";
                            List<ContractorListClass> cons = new List<ContractorListClass>();
                            GetVirtualWarehouse(api_key, "", ref contractor_national_id, "", _filters.postal_code, protocol, false, out virt, ref cons);
                        }

                        client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api-imp/{api_key}/{contractor_national_id}/receipt/_search/");
                        HttpResponseMessage response = client.PostAsJsonAsync($"{start}/{size}", input).Result;
                        result = response.Content.ReadAsStringAsync().Result;

                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                            return null;
                        }
                        else
                        {
                            string data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result)["data"].ToString();
                            List<SimpleReceipt> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SimpleReceipt>>(data);
                            return list;
                        }
                    }
                }
                public static List<SimpleReceipt> SearchReceipt(string api_key, int start, int size, SearchFilters _filters, string protocol, ref string result)
                {
                    using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
                    {
                        Dictionary<string, object> input = new Dictionary<string, object>();

                        Dictionary<string, object> filter = new Dictionary<string, object>();
                        input["filter"] = filter;
                        if (_filters != null)
                        {
                            if (!string.IsNullOrWhiteSpace(_filters.number)) filter["number"] = _filters.number;
                            if (!string.IsNullOrWhiteSpace(_filters.owner)) filter["owner"] = _filters.owner;
                            if (_filters.status != OptStatus.None) filter["status"] = _filters.status.ToString();

                            if (_filters.date_from != null || _filters.date_to != null)
                            {
                                Dictionary<string, object> date_filter = new Dictionary<string, object>();
                                if (_filters.date_from != null) date_filter["$gte"] = SRL.Convertor.EnglishDateTimeToUnixEpoch((DateTime)_filters.date_from);
                                if (_filters.date_to != null) date_filter["$lte"] = SRL.Convertor.EnglishDateTimeToUnixEpoch((DateTime)_filters.date_to);
                                filter["rcp_date"] = date_filter;
                            }
                            if (!string.IsNullOrWhiteSpace(_filters.postal_code)) filter["additional_data.postal_code"] = _filters.postal_code;
                            if (!string.IsNullOrWhiteSpace(_filters.province)) filter["additional_data.province"] = _filters.province;
                            if (!string.IsNullOrWhiteSpace(_filters.city)) filter["additional_data.city"] = _filters.city;
                            if (!string.IsNullOrWhiteSpace(_filters.warehouse_id)) filter["warehouse_id"] = _filters.warehouse_id;
                        }
                        client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api/{api_key}/admin/receipt/_search/");
                        HttpResponseMessage response = client.PostAsJsonAsync($"{start}/{size}", input).Result;
                        result = response.Content.ReadAsStringAsync().Result;

                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                            return null;
                        }
                        else
                        {
                            string data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result)["data"].ToString();
                            List<SimpleReceipt> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SimpleReceipt>>(data);
                            return list;
                        }
                    }
                }

                public static List<SimpleGoodIssue> GetHavaleList(string api_key, string contractor_national_id, int start, int size, SearchFilters _filters, string protocol, ref string result)
                {
                    using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
                    {
                        Dictionary<string, object> input = new Dictionary<string, object>();

                        Dictionary<string, object> filter = new Dictionary<string, object>();
                        input["filter"] = filter;
                        if (_filters != null)
                        {
                            if (!string.IsNullOrWhiteSpace(_filters.number)) filter["number"] = _filters.number;
                            if (!string.IsNullOrWhiteSpace(_filters.owner)) filter["owner"] = _filters.owner;
                            if (_filters.status != OptStatus.None) filter["status"] = _filters.status.ToString();
                            if (_filters.date_from != null || _filters.date_to != null)
                            {
                                Dictionary<string, object> date_filter = new Dictionary<string, object>();
                                if (_filters.date_from != null) date_filter["$gte"] = SRL.Convertor.EnglishDateTimeToUnixEpoch((DateTime)_filters.date_from);
                                if (_filters.date_to != null) date_filter["$lte"] = SRL.Convertor.EnglishDateTimeToUnixEpoch((DateTime)_filters.date_to);
                                filter["goods_issue_date"] = date_filter;
                            }
                        }
                        if (string.IsNullOrWhiteSpace(contractor_national_id))
                        {
                            string virt = "";
                            List<ContractorListClass> cons = new List<ContractorListClass>();
                            GetVirtualWarehouse(api_key, "", ref contractor_national_id, "", _filters.postal_code, protocol, false, out virt, ref cons);
                        }
                        client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api-imp/{api_key}/{contractor_national_id}/goods_issue/_search/");
                        HttpResponseMessage response = client.PostAsJsonAsync($"{start}/{size}", input).Result;
                        result = response.Content.ReadAsStringAsync().Result;
                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                            return null;
                        }
                        else
                        {
                            string data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result)["data"].ToString();
                            List<SimpleGoodIssue> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SimpleGoodIssue>>(data);
                            return list;
                        }
                    }
                }
                public static List<SimpleGoodIssue> SearchHavale(string api_key, int start, int size, SearchFilters _filters, string protocol, ref string result)
                {

                    using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
                    {
                        Dictionary<string, object> input = new Dictionary<string, object>();

                        Dictionary<string, object> filter = new Dictionary<string, object>();
                        input["filter"] = filter;
                        if (_filters != null)
                        {
                            if (!string.IsNullOrWhiteSpace(_filters.number)) filter["number"] = _filters.number;
                            if (!string.IsNullOrWhiteSpace(_filters.owner)) filter["owner"] = _filters.owner;
                            if (_filters.status != OptStatus.None) filter["status"] = _filters.status.ToString();
                            if (_filters.date_from != null || _filters.date_to != null)
                            {
                                Dictionary<string, object> date_filter = new Dictionary<string, object>();
                                if (_filters.date_from != null) date_filter["$gte"] = SRL.Convertor.EnglishDateTimeToUnixEpoch((DateTime)_filters.date_from);
                                if (_filters.date_to != null) date_filter["$lte"] = SRL.Convertor.EnglishDateTimeToUnixEpoch((DateTime)_filters.date_to);
                                filter["goods_issue_date"] = date_filter;
                            }
                            if (!string.IsNullOrWhiteSpace(_filters.postal_code)) filter["additional_data.postal_code"] = _filters.postal_code;
                            if (!string.IsNullOrWhiteSpace(_filters.province)) filter["additional_data.province"] = _filters.province;
                            if (!string.IsNullOrWhiteSpace(_filters.city)) filter["additional_data.city"] = _filters.city;
                            if (!string.IsNullOrWhiteSpace(_filters.warehouse_id)) filter["warehouse_id"] = _filters.warehouse_id;
                        }
                        client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api/{api_key}/admin/goods_issue/_search/");
                        HttpResponseMessage response = client.PostAsJsonAsync($"{start}/{size}", input).Result;
                        result = response.Content.ReadAsStringAsync().Result;
                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                            return null;
                        }
                        else
                        {
                            string data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result)["data"].ToString();
                            List<SimpleGoodIssue> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SimpleGoodIssue>>(data);
                            return list;
                        }
                    }
                }
                public class ExchangeInput
                {
                    public GoodsIssue goods_issue { get; set; } = new GoodsIssue();
                    public Receipt receipt { get; set; } = new Receipt();

                    public class GoodsIssueItem
                    {
                        public string good_id { get; set; }
                        public string measurement_unit { get; set; }
                        public int count { get; set; }
                        public List<ReceiptShare> receipt_shares { get; set; } = new List<ReceiptShare>();

                    }

                    public class Receipt
                    {
                        public string number { get; set; }
                        public string owner { get; set; }
                        public double rcp_date { get; set; }
                        [DefaultValue("")]
                        public string warehouse_id { get; set; }
                        public string postal_code { get; set; }
                        public List<ReceiptItem> receipt_items { get; set; } = new List<ReceiptItem>();
                    }
                    public class GoodsIssue
                    {
                        public double goods_issue_date { get; set; }
                        public string owner { get; set; }
                        [DefaultValue("")]
                        public string warehouse_id { get; set; }
                        public string postal_code { get; set; }
                        public List<GoodsIssueItem> goods_issue_items { get; set; } = new List<GoodsIssueItem>();
                    }

                    public class ReceiptItem
                    {
                        public string category_id { get; set; }
                        public string taxonomy_id { get; set; }
                        public string good_id { get; set; }
                        public string measurement_unit { get; set; }
                        public int count { get; set; }
                        public int total_weight { get; set; }
                        public string package_type { get; set; }
                        public int package_count { get; set; }
                        public int production_date { get; set; }
                    }
                    public class ReceiptShare
                    {
                        public int count { get; set; }
                        public string receipt_item_id { get; set; }
                    }
                }
                public enum OptStatus
                {
                    None,
                    TEMPORARY,
                    PERMANENT,
                    CANCELED
                }

                public class SearchFilters
                {
                    public string number { get; set; }
                    public string warehouse_id { get; set; }//virt_id
                    public string postal_code { get; set; }
                    public string province { get; set; }
                    public string city { get; set; }
                    public string owner { get; set; }
                    public DateTime? date_from { get; set; } = null;
                    public DateTime? date_to { get; set; } = null;
                    public OptStatus status { get; set; } = OptStatus.None;
                }
            }


            public class ContractorListClass
            {
                public string national_id { get; set; }
                public string name { get; set; }
                public long start_epoch { get; set; } = 1519072200;
                public long expire_epoch { get; set; } = 1834605000;
            }
            public class ComplexOwnerClass
            {
                public person agent = new person();
                public List<person> owners = new List<person>();

                public class person
                {
                    public string national_id { get; set; }
                    public string name { get; set; }
                }

            }
            public class CompanyInAnbarClass
            {

                public string website { get; set; }
                public string register_code { get; set; }
                public string national_id { get; set; }
                public string commercial_code { get; set; }
                public string register_epoch { get; set; }
                public string account_status { get; set; }
                public string phonenumber { get; set; }
                public string modifier_national_id { get; set; }
                public string roozname_rasmi_url { get; set; }
                public string id { get; set; }
                public string en_name { get; set; }
                public string register_city { get; set; }
                public string email { get; set; }
                public string fax { get; set; }
                public object ceo_expire_epoch { get; set; }
                public string ceo_national_id { get; set; }
                public string address { get; set; }
                public string org_creator_national_id { get; set; }
                public string name { get; set; }
                public string creator_national_id { get; set; }
                public string postal_code { get; set; }
                public List<CeoAgent> ceo_agents { get; set; } = new List<CeoAgent>();
                public Ceo ceo { get; set; }
                public object doc_taghirate_sabt_shode { get; set; }
                public object doc_ruzname_rasmi { get; set; }
                public class CeoAgent
                {
                    public string org_creator_national_id { get; set; }
                    public string name { get; set; }
                    public string title { get; set; }
                    public string national_id { get; set; }
                    public string creator_national_id { get; set; }
                    public long start_epoch { get; set; }
                    public string common_name { get; set; }
                    public string account_status { get; set; }
                    public string modifier_national_id { get; set; }
                    public long expire_epoch { get; set; }
                    public string id { get; set; }
                }
                public class Ceo
                {
                    public string org_creator_national_id { get; set; }
                    public string name { get; set; }
                    public string title { get; set; }
                    public string national_id { get; set; }
                    public string creator_national_id { get; set; }
                    public int start_epoch { get; set; }
                    public string common_name { get; set; }
                    public string account_status { get; set; }
                    public object modifier_national_id { get; set; }
                    public int expire_epoch { get; set; }
                    public string id { get; set; }
                }

            }
            public class RegCoInput
            {
                public string name { get; set; }
                public List<object> ceo_agents { get; set; } = new List<object>();
                public string register_code { get; set; }
                public string en_name { get; set; }
                public Ceo ceo { get; set; } = new Ceo();
                public string national_id { get; set; }
                public class Ceo
                {
                    public string national_id { get; set; }
                    public string name { get; set; }
                }

            }
            public class LatLng
            {
                public float lat { get; set; }
                public float lng { get; set; }
            }
            public class RegWarehouseInput
            {
                public IEnumerable<string> isRegValid()
                {
                    IEnumerable<string> r = SRL.ClassManagement.CheckValidationAttribute(this);
                    if (r.Any())
                    {
                        return r;
                    }
                    else
                    {
                        if (contractor_or_agent == UserRoleType.none)
                        {
                            return new string[] { "contractor_or_agent is required" };
                        }
                        string[] types = new string[] { "0", "1", "2", "3", "4", "6", "7" };
                        if (!types.Contains(type))
                        {
                            return new string[] { "type must be in" + SRL.Json.ClassObjectToJson(types) };
                        }
                        if (!activity_sector.Any())
                        {
                            return new string[] { "activity_sector is required" };
                        }
                        if (!supervisor_org.Any())
                        {
                            return new string[] { "supervisor_org is required" };
                        }

                        return new string[] { };

                    }
                }

                [Required]
                public string national_id { get; set; }
                [Required]
                public string mobile { get; set; }
                [Required]
                public string name { get; set; }
                [DefaultValue("")]
                public string co_national_id { get; set; }
                [Required]
                public string postal_code { get; set; }
                [Required]
                public string warehouse_usage_type { get; set; }
                [Required]
                public string area_type { get; set; }
                [Required]
                public string area { get; set; }
                public string province { get; set; }
                public string city { get; set; }
                [Required]
                public UserRoleType contractor_or_agent { get; set; } = UserRoleType.none;
                [Required]
                public string type { get; set; }
                [Required]
                public List<string> activity_sector { get; set; } = new List<string>();
                [Required]
                public List<string> supervisor_org { get; set; } = new List<string>();
                [DefaultValue("")]
                public string gov_warehouse_no { get; set; }

                [DefaultValue("")]
                public string birth_date { get; set; }
                public bool on_error_data_send_sms_to_user { get; set; } = false;


            }
            public class RegWarehouseOut
            {
                public string province { get; set; }
                public string city { get; set; }
                public string co_id { get; set; }
                public string user_id { get; set; }
                public string user_first_name { get; set; }
                public string full_address { get; set; }
                public string warehouse_id { get; set; }
                public string user_last_name { get; set; }
                public string complex_id { get; set; }
                public string company_name { get; set; }
            }
            public static RegWarehouseOut RegWarehouse(string api_key, RegWarehouseInput input_, string protocol, ref string result, ref object sent_data, string prefix = "app")
            {
                using (HttpClient client = new HttpClient())
                {
                    sent_data = SRL.Json.RemoveEmptyKeys(input_, true, true);
                    client.BaseAddress = new Uri($"{protocol}://{prefix}.nwms.ir/v2/b2b-api/{ api_key }/");
                    HttpResponseMessage response = client.PostAsJsonAsync("reg_warehouse", sent_data).Result;
                    result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        RegWarehouseOut output = Newtonsoft.Json.JsonConvert.DeserializeObject<RegWarehouseOut>(result);
                        return output;
                    }
                    else
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return null;
                    }
                }

            }
            public class WarehouseSearchResult
            {
                public long create_date { get; set; }
                public List<string> supervisor_org { get; set; }
                public string account_status { get; set; }
                public string postal_code { get; set; }
                public string id { get; set; }
                public List<string> activity_sector { get; set; }
                public string type { get; set; }
                public List<Contractor> contractors { get; set; }
                public string org_creator_national_id { get; set; }
                public string name { get; set; }

                public class Contractor
                {
                    public string name { get; set; }
                    public string national_id { get; set; }
                }



            }
            public class PostAddress
            {
                public string province { get; set; }
                public string township { get; set; }
                public string city { get; set; }
                public string village { get; set; }
                public string address { get; set; }
                public string error { get; set; }
            }
            public static PostAddress GetAddress(string api_key, string postal_code, ref string result)
            {

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"https://app.nwms.ir/v2/b2b-api/{ api_key }/iran-post/");
                    HttpResponseMessage response = client.GetAsync(postal_code).Result;
                    result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        PostAddress output = Newtonsoft.Json.JsonConvert.DeserializeObject<PostAddress>(result);
                        if (string.IsNullOrWhiteSpace(output.error))
                        {
                            return output;
                        }
                        else
                        {
                            result = output.error;
                            return null;
                        }
                    }
                    else
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return null;
                    }
                }

            }
            public static List<WarehouseSearchResult> WarehouseSearch(string api_key, long date_from, long date_to, int start, int size, string protocol)
            {
                try
                {

                    Dictionary<string, object> filter = new Dictionary<string, object>();
                    filter["account_status"] = "1";
                    Dictionary<string, object> date_filter = new Dictionary<string, object>();
                    date_filter["$gte"] = date_from;
                    date_filter["$lte"] = date_to;
                    filter["create_date"] = date_filter;

                    HttpClient client = new HttpClient();

                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api/" + api_key + "/admin/warehouse/_search/");
                    client.Timeout = new TimeSpan(0, 30, 0);
                    var result = client.PostAsJsonAsync(start + "/" + size, filter).Result;
                    if (result.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        Console.Write(result.Content.ReadAsStringAsync().Result);
                        return null;
                    }
                    else
                    {
                        var response = result.Content.ReadAsStringAsync().Result;

                        string data =
                                    Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(response)["data"].ToString();
                        // data = System.Text.RegularExpressions.Regex.Unescape(data);
                        List<WarehouseSearchResult> list =
                            Newtonsoft.Json.JsonConvert.DeserializeObject<List<WarehouseSearchResult>>(data);

                        return list;

                    }


                }
                catch (Exception ex)
                {

                    return null;
                }
            }

            public class EstelamResult
            {

                public bool warehouse_server { get; set; }
                public bool postal_code_server { get; set; }
                public string province { get; set; }
                public string village { get; set; }
                public string township { get; set; }
                public string city { get; set; }
                public string full_address { get; set; }

                public string name { get; set; }
                public string id { get; set; }
                public string org_creator_national_id { get; set; }
                public string gov_wh_number { get; set; }

                public Agent agent { get; set; }
                public List<Owner> owners { get; set; }
                public List<Warehouse> warehouses { get; set; }

                public class Agent
                {
                    public string name { get; set; }
                    public string id { get; set; }
                    public string national_id { get; set; }
                    public string mobile { get; set; }
                }


                public class Warehouse
                {
                    public string name { get; set; }
                    public string id { get; set; }
                    public string org_creator_national_id { get; set; }
                    public string type { get; set; }
                    public List<string> activity_sector { get; set; }
                    public List<string> supervisor_org { get; set; }
                    public List<Contractor> contractors { get; set; }
                }

                public class Owner
                {

                    public string name { get; set; }
                    public string national_id { get; set; }
                    public string mobile { get; set; }
                }

                public class Contractor
                {

                    public string name { get; set; }
                    public string id { get; set; }
                    public string national_id { get; set; }
                    public string mobile { get; set; }
                }

            }
            public static void GetComplexByPostalCodeSample(string postal_code, string api_key, string protocol)
            {

                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api/");

                HttpResponseMessage message = client.GetAsync(api_key + "/complex_by_post_code/" + postal_code).Result;
                string result = message.Content.ReadAsStringAsync().Result;

                string result_encode = System.Text.RegularExpressions.Regex.Unescape(result);
                //  MessageBox.Show(result_encode);

                if (message.StatusCode == System.Net.HttpStatusCode.OK)
                {// در این حالت استعلام انجام می شود و خروجی بصورت زیر استخراج می گردد:

                    //add 'Newtonsoft.Json.dll' reference:
                    EstelamResult estelam_result = Newtonsoft.Json.JsonConvert.DeserializeObject<EstelamResult>(result_encode);

                    //اگر کدپستی در سامانه انبار ثبت نام شده باشد مقدار برابر ترو در غیر اینصورت فالس است:
                    //اگر مقدار زیر برابر ترو باشد، یعنی در سامانه انبار ثبت شده باشد، حتما در شرکت پست هم وجود دارد و صحیح است
                    bool warehouse_server = estelam_result.warehouse_server;


                    if (warehouse_server == false)
                    {
                        //اگر کدپستی در شرکت پست وجود داشته باشد مقدار برابر ترو در غیر اینصورت فالس است
                        bool postal_code_server = estelam_result.postal_code_server;
                    }

                    //اگر کدپستی در یکی از سامانه ها (سامانه انبار یا شرکت پست) وجود داشته باشند اطلاعات آدرسی مقدار دارند

                    //استان
                    string province = estelam_result.province;

                    //شهرستان
                    string township = estelam_result.province;

                    //شهر
                    string city = estelam_result.city;

                    //دهستان در صورت وجود
                    string village = estelam_result.village;

                    //ادرس کامل
                    string full_address = estelam_result.full_address;


                    //اگر کدپستی در سامانه انبار ثبت نام شده باشد اطلاعات واحدها و مالکین و بهره برداران قابل دریافت است:
                    if (warehouse_server)
                    {
                        //نام مجتمع ثبت نام شده -مجتمع معادل کدپستی ثبت نام شده است
                        string name = estelam_result.name;

                        //کد یکتای مجتمع :
                        string id = estelam_result.id;

                        //کد سازمان ثبت کننده مجتمع بصورت وب سرویسی که میتواند نال باشد:
                        string org_creator_national_id = estelam_result.org_creator_national_id;

                        // شماره مجتمع در سازمان که می تواند نال باشد
                        string gov_wh_number = estelam_result.gov_wh_number;

                        //اگر برای مجتمع مالک یا مالکین ثبت شده باشد:
                        if (estelam_result.agent != null)
                        {//یکی از مالکین بعنوان مالک اصلی یا نماینده مالکین شناخته می شود

                            //مالک اصلی
                            string agent_name = estelam_result.agent.name;

                            //کدیکتای نماینده
                            string agent_id = estelam_result.agent.id;

                            //کدملی نماینده
                            string agent_national_id = estelam_result.agent.national_id;

                            //موبایل نماینده:
                            string agent_mobile = estelam_result.agent.mobile;

                            //اگر مجتمع دارای بیش از یک مالک باشد می توان اطلاعات همه مالکین را دریافت کرد:
                            foreach (var owner in estelam_result.owners)
                            {//نماینده مالکین جز یکی از مالکان است

                                //مالک
                                string owner_name = owner.name;

                                //کدملی مالک
                                string owner_national_id = owner.national_id;

                                //موبایل مالک
                                string owner_mobile = owner.mobile;
                            }
                        }

                        //در یک کدپستی یا مجتمع می تواند چندین واحد ثبت شود برای مثال در یک کدپستی دو فرد می توانند واحد جداگانه ثبت کنند.

                        //اگر برای یک مجتمع یا کدپستی واحدهای زیر مجموعه ای ثبت شده باشد:
                        if (estelam_result.warehouses != null)
                            foreach (var warehouse in estelam_result.warehouses)
                            {
                                //نام واحد:
                                string warehouse_name = warehouse.name;

                                //کدیکتای واحد
                                string warehouse_id = warehouse.id;

                                //کدسازمان ثبت کننده واحد بصورت وب سرویسی که می تواند نال باشد
                                string warehouse_org_creator_national_id = warehouse.org_creator_national_id;

                                //نوع واحد دارای یکی از مقادیر زیر:
                                //0 برای اصناف
                                //1 برای باغ و مزرعه
                                //2 برای معدن
                                // 3 برای تولیدی ها
                                //4 برای انبار و مراکز نگهداری کالا
                                //5 برای مراکز ارائه خدمات
                                string warehouse_type = warehouse.type;

                                //حوزه فعالیت واحد که دو رقم اول کد  ایسیک است و یک واحد می تواند چند حوزه فعالیت داشته باشد
                                foreach (string activity_sector in warehouse.activity_sector) { string warehouse_activity_sector = activity_sector; }

                                //کد دولتی یا خصوصی ناظر واحد که میتواند نال یا چند مورد باشد
                                //نام متناظر با کد را از پشتیبان سامانه تحویل بگیرید
                                if (warehouse.supervisor_org != null)
                                    foreach (string supervisor_org in warehouse.supervisor_org) { string warehouse_supervisor_org = supervisor_org; }

                                //در سامانه انبار یک مجتمع یا کدپستی دارای مالک /مالکین است و یک واحد دارای بهره بردار (مستاجر) است 
                                //مالک مجتمع میتواند همان بهره بردار باشد
                                // یک واحد میتواند بهره بردار نداشته باشد یا چندین بهره بردار داشته باشد
                                if (warehouse.contractors != null)
                                    foreach (var contractor in warehouse.contractors)
                                    {
                                        //بهره بردار
                                        string contractor_name = contractor.name;

                                        //کدیکتای بهره بردار
                                        string contractor_id = contractor.id;

                                        //کدملی بهره بردار
                                        string contractor_national_id = contractor.national_id;

                                        //موبایل بهره بردار
                                        string contractor_mobile = contractor.mobile;
                                    }
                            }
                    }

                }
                else
                {// در این حالت استعلام انجام نشده و خروجی خطا بصورت زیر قابل مشاهده است:
                    Console.Write(System.Text.RegularExpressions.Regex.Unescape(result));
                }

            }

            public static bool PutComplex(string api_key, string complex_id, object body_json, string protocol, out string result)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api/" + api_key + "/admin/complex/");
                    HttpResponseMessage response = client.PutAsJsonAsync(complex_id, body_json).Result;
                    result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return true;
                    }
                    else
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return false;
                    }
                }
            }

            public static RegCompanyResult RegCo(string api_key, string national_id, string mobile, string postal_code, string co_national_id, string protocol,
               out string ceo_name, out string co_error)
            {
                co_error = ceo_name = "";
                SRL.Projects.Nwms.GetCompanyInAnbarResult co_ok = new SRL.Projects.Nwms.GetCompanyInAnbarResult();
                SRL.Projects.Nwms.GetPersonResult person_ok = new SRL.Projects.Nwms.GetPersonResult();
                var person = new SRL.Projects.Nwms.PersonClass();
                var company = new SRL.Projects.Nwms.CompanyInAnbarClass();
                co_ok = SRL.Projects.Nwms.GetCompanyInAnbar(api_key, national_id, co_national_id, protocol, out company, out co_error);
                switch (co_ok)
                {
                    case SRL.Projects.Nwms.GetCompanyInAnbarResult.EmptyInput:
                        return RegCompanyResult.CoNationalIdEmpty;
                    case SRL.Projects.Nwms.GetCompanyInAnbarResult.Error:
                        return RegCompanyResult.Error;
                    case SRL.Projects.Nwms.GetCompanyInAnbarResult.NotFound:
                    case SRL.Projects.Nwms.GetCompanyInAnbarResult.NotActiveFound:
                        var company_ext = new SRL.Projects.Nwms.CompanyClass();
                        if (SRL.Projects.Nwms.GetCompanyByCoNationalId(api_key, co_national_id, protocol, out company_ext, out co_error) == false)
                        {
                            return RegCompanyResult.Error;
                        }
                        else
                        {
                            person_ok = SRL.Projects.Nwms.GetPersonByNationalId(api_key, national_id, protocol, out person);
                            switch (person_ok)
                            {
                                case GetPersonResult.OK:
                                    RegCoInput inp = new RegCoInput();
                                    inp.ceo.name = person.FirstName + " " + person.LastName;
                                    inp.ceo.national_id = national_id;
                                    inp.en_name = company_ext.name;
                                    inp.name = company_ext.name;
                                    inp.national_id = co_national_id;
                                    inp.register_code = company_ext.RegisterNumber;
                                    ceo_name = person.FirstName + " " + person.LastName;
                                    if (SRL.Projects.Nwms.AddNewCompany(api_key, inp, protocol, out company, out co_error) == false)
                                    {
                                        return RegCompanyResult.Error;
                                    }
                                    break;
                                case GetPersonResult.EmptyInput:
                                    return RegCompanyResult.NationalIdEmpty;
                                case GetPersonResult.Error:
                                    co_error = person.ErrorDescription;
                                    return RegCompanyResult.Error;
                            }


                        }
                        break;
                    case SRL.Projects.Nwms.GetCompanyInAnbarResult.OKNotCeos:
                    case SRL.Projects.Nwms.GetCompanyInAnbarResult.OKNotInCeos:
                        if (AddCeoAgent(api_key, company, national_id, mobile, postal_code, protocol, out ceo_name) == false)
                        {
                            co_error = ceo_name;
                            return RegCompanyResult.Error;
                        }
                        break;
                    case SRL.Projects.Nwms.GetCompanyInAnbarResult.OK:
                        break;

                }
                co_error = company.name;
                return RegCompanyResult.OK;
            }

            public static bool AddCeoAgent(string api_key, CompanyInAnbarClass company,
                string national_id, string mobile, string postal_code, string protocol, out string name_error, long start_epoch = 1524252600, long expire_epoch = 1839785400)
            {
                string error = "";
                name_error = AddUser(api_key, national_id, mobile, postal_code, protocol, out error);
                if (name_error == null)
                {
                    name_error = error;
                    return false;
                }
                using (HttpClient client = new HttpClient())
                {
                    List<Dictionary<string, object>> agent_list = new List<Dictionary<string, object>>();
                    Dictionary<string, object> agent = new Dictionary<string, object>();
                    agent["national_id"] = national_id;
                    agent["name"] = name_error;
                    agent["start_epoch"] = start_epoch;
                    agent["expire_epoch"] = expire_epoch;
                    agent_list.Add(agent);
                    foreach (var item in company.ceo_agents)
                    {
                        if (item.national_id == national_id) return true;
                        agent = new Dictionary<string, object>();
                        agent["national_id"] = item.national_id;
                        agent["name"] = item.name;
                        agent["start_epoch"] = item.start_epoch;
                        agent["expire_epoch"] = item.expire_epoch;
                        agent_list.Add(agent);

                    }

                    Dictionary<string, object> input = new Dictionary<string, object>();
                    input["ceo_agents"] = agent_list;
                    bool put_mode = PutCompany(api_key, company.id, input, protocol, out error);
                    if (put_mode)
                    {
                        return true;
                    }
                    else
                    {
                        name_error = error;
                        return false;
                    }
                }
            }

            public static string AddUser(string api_key, string national_id, string mobile, string postal_code, string protocol, out string error)
            {
                error = "";
                var user = GetAnbarUser(national_id, api_key, protocol, out error);
                if (user == null)
                {
                    var saha_user = GetSahaUser(national_id, api_key, protocol, out error);
                    if (saha_user == null) return null;
                    RegUserInput inp = new RegUserInput();
                    inp.account_status = "1";
                    inp.fathername = saha_user.FatherName;
                    inp.firstname = saha_user.FirstName;
                    inp.gender = saha_user.Gender == "0" ? "male" : "female";
                    inp.lastname = saha_user.LastName;
                    inp.mobile = mobile;
                    inp.national_id = national_id;
                    inp.password = national_id;
                    inp.postalcode = string.IsNullOrWhiteSpace(postal_code) ? "4713644457" : postal_code;
                    inp.SSNN_no = saha_user.IdentityNo == "0" ? national_id : saha_user.IdentityNo;
                    inp.SSNN_serial = saha_user.IdentitySerial + "|" + saha_user.IdentitySeries + "|الف";
                    inp.using_two_phase_password = "0";

                    var reged_user = RegisterUser(inp, api_key, protocol, out error);
                    if (reged_user != null)
                    {
                        return saha_user.FirstName + " " + saha_user.LastName;
                    }
                    else
                    {
                        return null;
                    }

                }
                else
                {
                    return user.firstname + " " + user.lastname;

                }
            }

            public static string RegisterUser(RegUserInput input, string api_key, string protocol, out string result)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api/" + api_key + "/admin/");
                    HttpResponseMessage response = client.PostAsJsonAsync("users", input).Result;
                    result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string id = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result)["userid"].ToString();
                        return id;
                    }
                    else
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return null;
                    }
                }
            }

            public static bool PutCompany(string api_key, string co_id, object input, string protocol, out string result)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api/" + api_key + "/admin/company/");
                    HttpResponseMessage response = client.PutAsJsonAsync(co_id, input).Result;
                    result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return true;
                    }
                    else
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return false;
                    }
                }
            }
            public static bool AddNewCompany(string api_key, RegCoInput inp, string protocol, out CompanyInAnbarClass company, out string result)
            {
                company = null;
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api/" + api_key + "/");
                    HttpResponseMessage response = client.PostAsJsonAsync("admin/company", inp).Result;
                    result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        company = Newtonsoft.Json.JsonConvert.DeserializeObject<CompanyInAnbarClass>(result);
                        return true;
                    }
                    else
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return false;
                    }
                }
            }

            public static string GetUserName(string api_key, string national_id, string protocol)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api/" + api_key + "/inquiry/national_id/");
                    HttpResponseMessage response = client.GetAsync(national_id).Result;
                    string result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string name = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result)["name"].ToString();
                        if (string.IsNullOrWhiteSpace(result)) return null;
                        else return name;
                    }
                    else
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return null;
                    }
                }
            }

            public static bool SetOwners(string api_key, ComplexOwnerClass complexOwnerClass, string complex_id, string protocol, ref string result)
            {
                result = "";
                if (complexOwnerClass.owners.Count < 1)
                {
                    return DeleteAllOwners(api_key, complex_id, protocol);
                }

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api/" + api_key + "/admin/complex/");
                    HttpResponseMessage response = client.PutAsJsonAsync(complex_id, complexOwnerClass).Result;
                    result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            public static bool SetOwnersByPostcode(string api_key, ComplexOwnerClass complexOwnerClass, string postal_code, string protocol, ref string result)
            {
                string com_id = SearchComplex(new Dictionary<string, object> { ["account_status"] = "1", ["postal_code"] = postal_code }, api_key, protocol).First().id;
                return SetOwners(api_key, complexOwnerClass, com_id, protocol, ref result);
            }
            public static bool AddOwner(string api_key, string national_id, string name, ComplexByPostCodeResult complex, string protocol)
            {
                ComplexOwnerClass complexOwnerClass = new ComplexOwnerClass();
                bool exist = false;
                foreach (var item in complex.owners)
                {
                    exist = item.national_id == national_id;
                    complexOwnerClass.owners.Add(new ComplexOwnerClass.person { national_id = item.national_id, name = item.name });
                }
                if (!exist)
                {
                    complexOwnerClass.owners.Add(new ComplexOwnerClass.person { national_id = national_id, name = name });
                }
                if (complex.agent != null)
                {
                    if (complex.agent.national_id != "0000000000")
                    {
                        complexOwnerClass.agent.national_id = complex.agent.national_id;
                        complexOwnerClass.agent.name = complex.agent.name;
                    }
                    else
                    {
                        complexOwnerClass.agent.national_id = national_id;
                        complexOwnerClass.agent.name = name;
                    }
                }
                else
                {
                    complexOwnerClass.agent.national_id = national_id;
                    complexOwnerClass.agent.name = name;
                }

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api/" + api_key + "/admin/complex/");
                    HttpResponseMessage response = client.PutAsJsonAsync(complex.id, complexOwnerClass).Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            public static bool SetContractors(string api_key, List<ContractorListClass> contractors, string warehouse_id, string protocol)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api/" + api_key + "/admin/warehouse/");
                    Dictionary<string, object> input = new Dictionary<string, object>();
                    input["contractors"] = contractors;
                    HttpResponseMessage response = client.PutAsJsonAsync(warehouse_id, input).Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return true;

                    }
                    else return false;

                }

            }
            public static bool AddContractor(string api_key, string national_id, string name, GetWarehouseClass warehouse, string protocol)
            {
                List<ContractorListClass> contractors = new List<ContractorListClass>();
                bool exist = false;
                foreach (var item in warehouse.contractors)
                {
                    exist = item.national_id == national_id;
                    contractors.Add(new ContractorListClass { national_id = item.national_id, name = item.name });
                }
                if (!exist)
                {
                    contractors.Add(new ContractorListClass { national_id = national_id, name = name });
                }
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api/" + api_key + "/admin/warehouse/");
                    Dictionary<string, object> input = new Dictionary<string, object>();
                    input["contractors"] = contractors;
                    HttpResponseMessage response = client.PutAsJsonAsync(warehouse.id, input).Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return true;

                    }
                    else return false;

                }

            }

            public static NwmsResultType GetVirtualWarehouseWithPost(string api_key, string warehouse_id, string postal_code, string contractor_national_id, string contractor_co_national_id, string protocol, out string result)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api-imp/" + api_key + "/" + contractor_national_id + "/virt_warehouse/");
                    HttpResponseMessage response = client.GetAsync("_all").Result;
                    result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result)["data"].ToString();
                        List<VirtClass> virt = Newtonsoft.Json.JsonConvert.DeserializeObject<List<VirtClass>>(data);
                        string contractor = string.IsNullOrWhiteSpace(contractor_co_national_id) ? contractor_national_id : contractor_co_national_id;
                        if (string.IsNullOrWhiteSpace(warehouse_id))
                        {
                            var query = virt.Where(x => (x.postal_code == postal_code && x.contractor_national_id == contractor) && x.account_status == "1");
                            if (query.Count() == 1)
                            {
                                result = query.First().id;
                                return NwmsResultType.OK;
                            }
                            else if (query.Count() > 1)
                            {
                                return NwmsResultType.MultiFoundSetId;
                            }
                            else
                            {
                                return NwmsResultType.WarhouseNotFound;
                            }
                        }
                        else
                        {
                            var query = virt.Where(x => ((x.warehouse_id == warehouse_id && x.contractor_national_id == contractor) || x.id == warehouse_id) && x.account_status == "1");
                            if (query.Any())
                            {
                                result = query.First().id;
                                return NwmsResultType.OK;
                            }
                            else
                            {
                                return NwmsResultType.WarhouseNotFound;
                            }
                        }

                    }
                    else
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return NwmsResultType.HttpError;
                    }

                }

            }
            public static List<VirtClass> GetAllVirtWarehouse(string api_key, string contractor_national_id, string protocol, ref string result)
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = $"{protocol}://app.nwms.ir/v2/b2b-api-imp/{ api_key }/{ contractor_national_id }/virt_warehouse/";
                    client.BaseAddress = new Uri(url);
                    HttpResponseMessage response = client.GetAsync("_all").Result;
                    result = response.Content.ReadAsStringAsync().Result;

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result)["data"].ToString();
                        List<VirtClass> virts = Newtonsoft.Json.JsonConvert.DeserializeObject<List<VirtClass>>(data);
                        return virts;
                    }
                    else
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return null;
                    }

                }
            }

            public static NwmsResultType GetVirtualWarehouse(string api_key, string warehouse_id, ref string contractor_national_id, string contractor_co_national_id,
               string postal_code, string protocol, bool only_contractor, out string virt_result, ref List<ContractorListClass> contractors)
            {
                contractors = new List<ContractorListClass>();
                //try get contractor_national_id if empty

                if (string.IsNullOrWhiteSpace(contractor_national_id))
                {
                    CompanyInAnbarClass company = new CompanyInAnbarClass();

                    //first from company
                    if (!string.IsNullOrWhiteSpace(contractor_co_national_id))
                    {
                        var anbar_co = GetCompanyInAnbar(api_key, contractor_national_id, contractor_co_national_id, protocol, out company, out virt_result);
                        if (anbar_co == GetCompanyInAnbarResult.OK || anbar_co == GetCompanyInAnbarResult.OKNotInCeos)
                        {
                            contractor_national_id = company.ceo.national_id;
                        }
                    }
                    //from warehouse_id
                    if (only_contractor == false) if (string.IsNullOrWhiteSpace(contractor_national_id) && !string.IsNullOrWhiteSpace(warehouse_id))
                        {
                            var warehouse = GetWarehouse(api_key, warehouse_id, protocol, out virt_result);
                            if (warehouse?.contractors?.Count() > 0)
                            {

                                if (warehouse.contractors.Select(x => x.national_id)?.Distinct()?.Count() == 1)
                                {
                                    contractor_national_id = warehouse.contractors.Select(x => x.national_id).First();
                                    //if it is company
                                    if (contractor_national_id.Length == 11)
                                    {
                                        var anbar_co = GetCompanyInAnbar(api_key, "", contractor_national_id, protocol, out company, out virt_result);
                                        if (anbar_co == GetCompanyInAnbarResult.OK || anbar_co == GetCompanyInAnbarResult.OKNotInCeos)
                                        {
                                            contractor_national_id = company.ceo.national_id;
                                        }
                                        else
                                        {
                                            contractor_national_id = "";
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var item in warehouse.contractors)
                                    {
                                        contractors.Add(new ContractorListClass { name = item.name, national_id = item.national_id });
                                    }
                                    virt_result = "";
                                    return NwmsResultType.NotUniqueFound;
                                }
                            }
                        }
                    //from postal_code
                    if (only_contractor == false) if (string.IsNullOrWhiteSpace(contractor_national_id) && !string.IsNullOrWhiteSpace(postal_code))
                        {
                            var complex = GetComplexByPostalCode(postal_code, api_key, true, protocol, out virt_result);
                            if (complex != null)
                            {
                                if (complex?.warehouses?.Count < 1 || complex?.warehouses == null)
                                {
                                    return NwmsResultType.WarhouseNotFound;
                                }
                                else if (complex?.warehouses?.SelectMany(x => x.contractors)?.Select(y => y.national_id)?.Distinct()?.Count() == 1)
                                {
                                    contractor_national_id = complex.warehouses.SelectMany(x => x.contractors).Select(y => y.national_id).First();
                                    //if it is company
                                    if (contractor_national_id.Length == 11)
                                    {
                                        var anbar_co = GetCompanyInAnbar(api_key, "", contractor_national_id, protocol, out company, out virt_result);
                                        if (anbar_co == GetCompanyInAnbarResult.OK || anbar_co == GetCompanyInAnbarResult.OKNotInCeos)
                                        {
                                            contractor_national_id = company.ceo.national_id;
                                        }
                                        else
                                        {
                                            contractor_national_id = "";
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var item in complex?.warehouses?.SelectMany(x => x.contractors))
                                    {
                                        contractors.Add(new ContractorListClass { name = item.name, national_id = item.national_id });
                                    }
                                    virt_result = "";
                                    return NwmsResultType.NotUniqueFound;
                                }
                            }
                        }
                    if (string.IsNullOrWhiteSpace(contractor_national_id))
                    {
                        virt_result = "";
                        return NwmsResultType.WarhouseNotFound;
                    }
                }

                virt_result = "";

                List<VirtClass> virt = GetAllVirtWarehouse(api_key, contractor_national_id, protocol, ref virt_result);

                if (virt != null)
                {
                    if (virt.Count < 1)
                    {
                        return NwmsResultType.WarhouseNotFound;
                    }
                    else if (virt.Count == 1)
                    {
                        virt_result = virt.First().id;
                        return NwmsResultType.OK;
                    }
                    else
                    {
                        string contractor = string.IsNullOrWhiteSpace(contractor_co_national_id) ? contractor_national_id : contractor_co_national_id;
                        if (!string.IsNullOrWhiteSpace(warehouse_id))
                        {
                            var query_exact = virt.Where(x => ((x.warehouse_id == warehouse_id && x.contractor_national_id == contractor) || x.id == warehouse_id) && x.account_status != "3");
                            if (query_exact.Any())
                            {
                                var first_wirt = query_exact.First();
                                virt_result = first_wirt.id;
                                return NwmsResultType.OK;
                            }
                            else return GetNearestId(virt, postal_code, contractor, ref virt_result, ref contractors);
                        }
                        else
                        {
                            return GetNearestId(virt, postal_code, contractor, ref virt_result, ref contractors);
                        }
                    }
                }
                else
                {
                    return NwmsResultType.HttpError;
                }

            }

            private static NwmsResultType GetNearestId(List<VirtClass> virt, string postal_code, string contractor, ref string virt_id, ref List<ContractorListClass> contractors)
            {
                contractors = new List<ContractorListClass>();
                // by postal_code and national_id
                var query_by_pc_nc = virt.Where(x => (x.postal_code == postal_code && x.contractor_national_id == contractor) && x.account_status != "3");
                if (query_by_pc_nc.Count() > 0)
                {
                    var first_wirt = query_by_pc_nc.First();
                    virt_id = first_wirt.id;
                    return NwmsResultType.OK;
                }
                else
                {  // by national_id 
                    var query_by_nc = virt.Where(x => x.contractor_national_id == contractor && x.account_status != "3");
                    if (query_by_nc.Count() == 1)
                    {
                        var first_wirt = query_by_nc.First();
                        virt_id = first_wirt.id;
                        return NwmsResultType.OK;
                    }
                    else if (query_by_nc.Count() > 1)
                    {
                        foreach (var item in query_by_nc)
                        {
                            contractors.Add(new ContractorListClass { name = item.name, national_id = item.contractor_national_id });
                        }
                        virt_id = "";
                        return NwmsResultType.NotUniqueFound;
                    }
                    else
                    {// only postal_code
                        var query_by_pc = virt.Where(x => x.postal_code == postal_code && x.account_status != "3");
                        if (query_by_pc.Count() == 1)
                        {
                            var first_wirt = query_by_pc.First();
                            virt_id = first_wirt.id;
                            return NwmsResultType.OK;
                        }
                        else if (query_by_pc.Count() > 1)
                        {
                            foreach (var item in query_by_pc)
                            {
                                contractors.Add(new ContractorListClass { name = item.name, national_id = item.contractor_national_id });
                            }
                            virt_id = "";
                            return NwmsResultType.NotUniqueFound;
                        }
                        else
                        {
                            return NwmsResultType.WarhouseNotFound;
                        }
                    }

                }
            }

            public static bool DeleteAllOwners(string api_key, string complex_id, string protocol)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api/" + api_key + "/admin/complex/");
                    Dictionary<string, object> input = new Dictionary<string, object>();
                    List<Dictionary<string, object>> owners = new List<Dictionary<string, object>>();
                    Dictionary<string, object> unknown = new Dictionary<string, object>();
                    unknown["national_id"] = "0000000000";
                    unknown["name"] = "UNKNOWN";
                    owners.Add(unknown);
                    input["owners"] = owners;
                    HttpResponseMessage response = client.PutAsJsonAsync(complex_id, input).Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            public static GetWarehouseClass GetWarehouse(string api_key, string warehouse_id, string protocol, out string result)
            {
                result = "";
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api/" + api_key + "/admin/warehouse/");
                    HttpResponseMessage response = client.GetAsync(warehouse_id).Result;
                    result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var output = Newtonsoft.Json.JsonConvert.DeserializeObject<GetWarehouseClass>(result);
                        if (output.account_status == "1")
                        {
                            return output;
                        }
                        else
                        {
                            result = @"{""warehouse"":""not_active_found""}";
                            return null;
                        }
                        return output;
                    }
                    else
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return null;
                    }
                }

            }

            public static bool PutWarehouse(string api_key, string warehouse_id, object body_json, string protocol, out string result)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api/" + api_key + "/admin/warehouse/");
                    HttpResponseMessage response = client.PutAsJsonAsync(warehouse_id, body_json).Result;
                    result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return true;
                    }
                    else
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return false;
                    }
                }
            }
            public static bool PutUser(string api_key, string national_id, object body_json, string protocol, out string error)
            {
                error = "";
                Dictionary<string, object> inp = new Dictionary<string, object>();
                inp["account_status"] = "1";
                inp["national_id"] = national_id;
                var users = GetAnbarUser(inp, api_key, protocol, out error);
                if (users == null)
                {
                    return false;
                }
                if (!users.Any())
                {
                    error = "کاربر یافت نشد";
                    return false;
                }
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api/" + api_key + "/admin/users/");
                    HttpResponseMessage response = client.PutAsJsonAsync(users.First().userid, body_json).Result;
                    error = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return true;
                    }
                    else
                    {
                        error = SRL.Json.IsJson(error) ? error : response.StatusCode.ToString();
                        return false;
                    }
                }
            }

            public static ShahkarOutputClass CallShahkar(string client_id, string national_id, string mobile, string basic_auth_value = "c2FuYXRfbWFkYW5fZ3NiOjcxc2RsdTU2")
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://sr-cix.ntsw.ir/");
                ShahkarInputClass input = new ShahkarInputClass();

                input.identificationNo = national_id;
                input.identificationType = 0;
                var date = DateTime.Now;

                string time = String.Format("{0:yyyy}{0:MM}{0:dd}{0:HH}{0:mm}{0:ss}{0:FFFFFF}", date);

                string id = client_id + time;

                input.requestId = id;
                input.serviceNumber = mobile;
                input.serviceType = 2;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basic_auth_value);
                HttpResponseMessage response = client.PostAsJsonAsync("Services/GetIDmatching-NWMS", input).Result;

                string result = response.Content.ReadAsStringAsync().Result;
                ShahkarOutputClass output = Newtonsoft.Json.JsonConvert.DeserializeObject<ShahkarOutputClass>(result);

                return output;
            }
            
            public static List<SearchResult.SearchWarehouseResult> SearchWarehouse(Dictionary<string, object> input_json, string api_key, int? from_, int? size_, string protocol, ref string result)
            {
                List<SearchResult.SearchWarehouseResult> warehouses = new List<SearchResult.SearchWarehouseResult>();
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api/" + api_key + "/admin/warehouse/_search/");
                List<SearchResult.SearchWarehouseResult> warehouse_list = new List<SearchResult.SearchWarehouseResult>();
                int from = 0;
                if (from_ != null) from = (int)from_;
                do
                {
                    int size = 10;
                    if (size_ != null) size = (int)size_;

                    warehouse_list = SearchWarehouseBySize(api_key, from, size, input_json, protocol, ref result);
                    if (warehouse_list == null) return null;

                    warehouses.AddRange(warehouse_list);
                    from += size;
                } while (warehouse_list.Count == 10 && (from_ == null || size_ == null));

                return warehouses;
            }

            public static List<SearchResult.SearchWarehouseResult> SearchWarehouseBySize(string api_key, int from, int size, Dictionary<string, object> input_json, string protocol, ref string result)
            {
                List<SearchResult.SearchWarehouseResult> warehouses = new List<SearchResult.SearchWarehouseResult>();
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api/" + api_key + "/admin/warehouse/_search/");


                HttpResponseMessage response = client.PostAsJsonAsync(from + $"/{size}", input_json).Result;
                result = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                    return null;
                }

                Dictionary<string, object> result_json = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                string data = result_json["data"].ToString();
                warehouses = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SearchResult.SearchWarehouseResult>>(data);
                 
                return warehouses;
            }
            public static List<SearchResult.SearchGoodResult> SearchGoods(string api_key, string protocol, out string result, Dictionary<string, object> input_json = null)
            {
                result = "";
                if (input_json == null) input_json = new Dictionary<string, object> { ["filter"] = new Object() };

                List<SearchResult.SearchGoodResult> goods = new List<SearchResult.SearchGoodResult>();
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri($"{protocol}://admin-app.nwms.ir/v2/b2b-api/" + api_key + "/json_store/goods/_search/");
                List<SearchResult.SearchGoodResult> goods_list = new List<SearchResult.SearchGoodResult>();
                int from = 0;
                int to = 90;
                do
                {

                    HttpResponseMessage response = client.PostAsJsonAsync(from + "/" + to, input_json).Result;
                    result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return null;
                    }


                    Dictionary<string, object> result_json = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                    string data = result_json["data"].ToString();
                    if (string.IsNullOrWhiteSpace(data)) break;
                    goods_list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SearchResult.SearchGoodResult>>(data);

                    goods.AddRange(goods_list);
                    from += to;
                } while (goods_list.Count == to);

                return goods;
            }

            public static List<SearchResult.SearchGoodResult> SearchGoods(string api_key, string taxonomy, int start, int size, string protocol, ref string result)
            {
                Dictionary<string, object> filter = new Dictionary<string, object>();
                Dictionary<string, object> json = new Dictionary<string, object>();
                json["json_data.taxonomy"] = taxonomy;
                json["json_data.custom_pagination_start"] = start;
                json["json_data.custom_pagination_size"] = size;

                filter["filter"] = json;

                return SearchGoods(api_key, protocol, out result, filter);

            }

            public static string DeleteGood(string api_key, string good_id, string protocol)
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri($"{protocol}://admin-app.nwms.ir/v2/b2b-api/" + api_key + "/json_store/");
                HttpResponseMessage response = client.DeleteAsync(good_id).Result;
                string result = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return null;
                }
                else
                {
                    result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                    return result;

                }
            }

            public static List<SearchResult.SearchUserResult> GetAnbarUser(Dictionary<string, object> input_json, string api_key, string protocol, out string result)
            {
                result = "";
                List<SearchResult.SearchUserResult> users = new List<SearchResult.SearchUserResult>();
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri($"{protocol}://admin-app.nwms.ir/v2/b2b-api/" + api_key + "/admin/users/_search/");
                List<SearchResult.SearchUserResult> user_list = new List<SearchResult.SearchUserResult>();
                int from = 0;
                do
                {

                    HttpResponseMessage response = client.PostAsJsonAsync(from + "/10", input_json).Result;
                    result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return null;
                    }


                    Dictionary<string, object> result_json = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                    string data = result_json["data"].ToString();
                    if (string.IsNullOrWhiteSpace(data)) break;
                    user_list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SearchResult.SearchUserResult>>(data);

                    users.AddRange(user_list);
                    from += 10;
                } while (user_list.Count == 10);

                return users;
            }
            public static AnbarUserClass GetAnbarUserPass(string national_id, string api_key, string protocol, ref string result)
            {
                AnbarUserClass user = GetAnbarUser(national_id, api_key, protocol, out result);
                if (user == null) return null;

                string pass = GetUserPass(user.userid, api_key, protocol, ref result);
                if (string.IsNullOrWhiteSpace(pass)) return null;
                user.password = pass;
                return user;
            }

            public static string GetUserPass(string userid, string api_key, string protocol, ref string result)
            {
                result = "";
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api/" + api_key + "/admin/users/");
                    HttpResponseMessage response = client.GetAsync(userid).Result;
                    result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var user = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                        if (user["account_status"].ToString() == "1")
                        {
                            return user["password"].ToString();
                        }
                        else
                        {
                            result = @"{""user"":""not_active_found""}";
                            return "";
                        }
                    }
                    else
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return null;
                    }
                }
            }

            public static AnbarUserClassResult GetUserData(string userid, string api_key, string protocol)
            {
                using (HttpClient client = new HttpClient())
                {
                    AnbarUserClassResult user_result = new AnbarUserClassResult();
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api/" + api_key + "/admin/users/");
                    HttpResponseMessage response = client.GetAsync(userid).Result;
                    string result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        AnbarUserClass user = Newtonsoft.Json.JsonConvert.DeserializeObject<AnbarUserClass>(result);
                        if (user.account_status == "1")
                        {
                            user_result.result_type = AnbarUserClassResult.ResultType.OK;
                            user_result.user = user;
                            return user_result;
                        }
                        else
                        {
                            user_result.result_type = AnbarUserClassResult.ResultType.Inactive;
                            user_result.user = null;
                            return user_result;
                        }
                    }
                    else
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        user_result.result_type = AnbarUserClassResult.ResultType.Error;
                        user_result.user = null;
                        user_result.error = result;
                        return user_result;
                    }
                }
            }

            public static List<SearchResult.SearchUserResult> SearchUserBySize(Dictionary<string, object> input_json, string api_key, int from, int size, string protocol, out string result)
            {
                result = "";
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api/" + api_key + "/admin/users/_search/");
                List<SearchResult.SearchUserResult> user_list = new List<SearchResult.SearchUserResult>();
                HttpResponseMessage response = client.PostAsJsonAsync(from + "/" + size, input_json).Result;
                result = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                    return null;
                }
                Dictionary<string, object> result_json = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                string data = result_json["data"].ToString();
                user_list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SearchResult.SearchUserResult>>(data);
                return user_list;
            }

            public static List<SearchResult.SearchComplexResult> SearchComplex(Dictionary<string, object> input_json, string api_key, string protocol)
            {
                List<SearchResult.SearchComplexResult> complexes = new List<SearchResult.SearchComplexResult>();
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api/" + api_key + "/admin/complex/_search/");

                List<SearchResult.SearchComplexResult> data_list = new List<SearchResult.SearchComplexResult>();
                int from = 0;
                do
                {
                    HttpResponseMessage response = client.PostAsJsonAsync(from + "/10", input_json).Result;
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        return null;
                    }

                    string result = response.Content.ReadAsStringAsync().Result;
                    Dictionary<string, object> result_json = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                    string data = result_json["data"].ToString();
                    if (string.IsNullOrWhiteSpace(data)) break;
                    data_list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SearchResult.SearchComplexResult>>(data);

                    complexes.AddRange(data_list);

                    from += 10;
                } while (data_list.Any());

                return complexes;
            }

            public static List<SearchResult.SearchComplexResult> GetAllWarComplexByNationalId(string api_key, string national_id, string protocol, out string result)
            {
                result = "";
                List<SearchResult.SearchComplexResult> complexes = new List<SearchResult.SearchComplexResult>();
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri($"{protocol}://app.nwms.ir");
                HttpResponseMessage response = client.GetAsync("/v2/b2b-api-imp/" + api_key + "/" + national_id + "/complex/_all").Result;
                result = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                    return null;
                }
                Dictionary<string, object> result_json = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                string data = result_json["data"].ToString();
                complexes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SearchResult.SearchComplexResult>>(data);


                return complexes;
            }

            public static GetPersonResult GetPersonByNationalId(string api_key, string national_id, string protocol, out PersonClass person)
            {

                person = new PersonClass();
                HttpClient client_ = new HttpClient();
                client_.BaseAddress = new Uri($"{ protocol}://admin-app.nwms.ir/v2/b2b-api/" + api_key + "/admin/ext-service/");
                person = new PersonClass();

                national_id = SRL.Convertor.NationalId(national_id);
                Dictionary<string, object> input = new Dictionary<string, object>();
                input["national_id"] = national_id;
                HttpResponseMessage response = client_.PostAsJsonAsync("person_by_national_id", input).Result;
                if (string.IsNullOrWhiteSpace(national_id))
                {
                    person.ErrorDescription = "کدملی خالی است";
                    return GetPersonResult.EmptyInput;
                }
                string result = response.Content.ReadAsStringAsync().Result;
                person.HttpCode = response.StatusCode.ToString();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                    if (data["FirstName"] != null)
                    {
                        person.FirstName = data["FirstName"].ToString();
                        person.LastName = data["LastName"].ToString();
                        person.last_sent = DateTime.Now.ToString();
                        person.national_id = national_id;
                        return GetPersonResult.OK;
                    }
                    else
                    {
                        person.ErrorDescription = data["ErrorDescription"].ToString();
                        return GetPersonResult.ServiceError;
                    }

                }
                else
                {
                    person.ErrorDescription = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                    return GetPersonResult.Error;
                }
            }

            public static GetPersonResult GetCixPersonByNationalId(string api_key, string national_id, out PersonClass person)
            {
                person = new PersonClass();

                national_id = SRL.Convertor.NationalId(national_id);

                if (string.IsNullOrWhiteSpace(national_id))
                {
                    person.ErrorDescription = "کدملی خالی است";
                    return GetPersonResult.EmptyInput;
                }

                CixGetPersonInfoServiceReference.GetPersonInfoPortTypeClient client_ =
                    new CixGetPersonInfoServiceReference.GetPersonInfoPortTypeClient("GetPersonInfoHttpsSoap11Endpoint");
                SRL.Web.AddBasicAuthToSoapHeader(client_, "nwms_user", "2867%plfa@", 0);

                CixGetPersonInfoServiceReference.SabtAhvalSAHAPersonInfoStract response = client_.getPersonInfoSAHA96M(national_id);
                if (response.ErrorCode == 0)
                {

                    person.FirstName = response.FirstName;
                    person.LastName = response.LastName;
                    person.last_sent = DateTime.Now.ToString();
                    person.national_id = national_id;
                    person.BirthDate = response.BirthDate;
                    return GetPersonResult.OK;
                }
                else
                {
                    person.ErrorDescription = response.ErrorDescription;
                    return GetPersonResult.ServiceError;
                }


            }

            public static AnbarUserClass GetAnbarUser(string national_id, string api_key, string protocol, out string result)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api-imp/" + api_key + "/" + national_id + "/");
                    HttpResponseMessage response = client.GetAsync("user").Result;
                    result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        AnbarUserClass person = Newtonsoft.Json.JsonConvert.DeserializeObject<AnbarUserClass>(result);
                        return person;

                    }
                    else
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return null;
                    }
                }
            }
            public static SahaUserClass GetSahaUser(string national_id, string api_key, string protocol, out string result)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{protocol}://admin-app.nwms.ir/v2/b2b-api/" + api_key + "/admin/ext-service/");
                    Dictionary<string, object> input = new Dictionary<string, object>();
                    input["national_id"] = national_id;
                    HttpResponseMessage response = client.PostAsJsonAsync("person_by_national_id", input).Result;
                    result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        SahaUserClass person = Newtonsoft.Json.JsonConvert.DeserializeObject<SahaUserClass>(result);
                        if (person.ErrorCode == 0)
                        {
                            return person;
                        }
                        else
                        {
                            result = person.ErrorDescription;
                            return null;
                        }

                    }
                    else
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return null;
                    }
                }
            }
            public partial class BaseValueTB
            {
                public long ID { get; set; }
                public string title { get; set; }
                public string code { get; set; }
                public string type { get; set; }
            }
            public class BaseValueData
            {

                public string key { get; set; }
                public string value { get; set; }

                public string GetTitle()
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(value)["title"].ToString();
                }

            }


            public class OrgCreator
            {
                public JsonData json_data { get; set; }

                public class JsonData
                {
                    public string user_id { get; set; }
                    public string key { get; set; }
                    public string title { get; set; }
                    public string user_national_id { get; set; }
                }
            }
            public static List<BaseValueData> GetOrgCreators(string api_key, BaseValueTypes.Base base_type, string protocol, out string result)
            {
                using (HttpClient client = new HttpClient())
                {
                    List<BaseValueData> base_values = new List<BaseValueData>();
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api/{ api_key }/json_store/b2b_api_key/_search/");
                    int from = 0;
                    Dictionary<string, object> input = new Dictionary<string, object>();
                    Dictionary<string, object> filter = new Dictionary<string, object>();
                    List<OrgCreator> all_org_creators = new List<OrgCreator>();
                    List<OrgCreator> each_org_creators = new List<OrgCreator>();
                    input["filter"] = filter;
                    do
                    {
                        HttpResponseMessage response = client.PostAsJsonAsync($"{from}/100", input).Result;
                        result = response.Content.ReadAsStringAsync().Result;
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            string data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result)["data"].ToString();
                            each_org_creators = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OrgCreator>>(data);
                            all_org_creators.AddRange(each_org_creators);
                        }
                        else
                        {
                            result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                            return null;
                        }

                        from += 100;

                    } while (each_org_creators.Count == 100);

                    foreach (var item in all_org_creators)
                    {
                        AnbarUserClassResult user = GetUserData(item.json_data.user_id, api_key, protocol);
                        switch (user.result_type)
                        {
                            case AnbarUserClassResult.ResultType.OK:
                                base_values.Add(new BaseValueData { key = user.user.national_id, value = SRL.Json.ClassObjectToJson(new Dictionary<string, object> { ["title"] = item.json_data.title }) });
                                break;
                            case AnbarUserClassResult.ResultType.Error:
                                result = user.error;
                                return null;
                            case AnbarUserClassResult.ResultType.Inactive:
                                continue;
                        }
                    }
                    return base_values;
                }
            }

            public static List<BaseValueData> GetBaseValue(string api_key, BaseValueTypes.Base base_type, string protocol, out string result)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api/" + api_key + "/admin/setting_keyvalue/");
                    HttpResponseMessage response = client.GetAsync(base_type.ToString()).Result;
                    result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Dictionary<string, object> data_status = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                        List<BaseValueData> data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BaseValueData>>(data_status["data"].ToString());
                        return data;
                    }
                    else
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return null;
                    }
                }
            }
            public static string ComputePostCodeHash(string password, string param1 = null, string param2 = null, string param3 = null)
            {
                StringBuilder sb = new StringBuilder(password + "#");
                if (!string.IsNullOrEmpty(param1))
                    sb.Append(param1 + "#");
                if (!string.IsNullOrEmpty(param2))
                    sb.Append(param2 + "#");
                if (!string.IsNullOrEmpty(param3))
                    sb.Append(param3 + "#");
                sb.Append(DateTime.Now.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture));

                return SRL.Security.GetHashString(sb.ToString(), SRL.Security.HashAlgoritmType.Sha1);
            }

            public static PostCodeServiceReference.AddressResult EstelamFromPostServer
                   (PostCodeServiceReference.PostCodeClient client, string username, string post_code, string password)
            {
                string hash = ComputePostCodeHash(password, post_code);
                var res = client.GetAddressByPostcode(username, hash, post_code, "", "", "");
                return res;
            }

            public static CIXGetAddressByPostcode.AddressResult EstelamFromPostESBServer
                  (CIXGetAddressByPostcode.GetAddressByPostCodePortTypeClient client, string post_code, string service_password)
            {
                string hash = ComputePostCodeHash(service_password, post_code);
                var res = client.GetAddressByPostcode(hash, post_code, "?", "?", "?");
                return res;
            }
            public static PostalCodeFromPostClass EstelamPostalCodeFromPost(string postal_code, out HttpResponseMessage response)
            {
                PostalCodeFromPostClass post = new PostalCodeFromPostClass();
                post = null;
                response = null;
                HttpClient client_ = new HttpClient();
                client_.BaseAddress = new Uri($"https://admin-app.nwms.ir/v2/b2b-api/2050130318/admin/ext-service/");
                if (string.IsNullOrWhiteSpace(postal_code)) return null;
                Dictionary<string, object> input = new Dictionary<string, object>();
                input["postal_code"] = postal_code;
                response = client_.PostAsJsonAsync("postal_code", input).Result;
                string result = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    post = Newtonsoft.Json.JsonConvert.DeserializeObject<PostalCodeFromPostClass>(result);

                }
                return post;
            }
            public static bool? GetCompanyByCoNationalId(string api_key, string co_national_id, string protocol, out CompanyClass company, out string result)
            {
                company = new CompanyClass();

                HttpClient client_ = new HttpClient();
                client_.BaseAddress = new Uri($"{ protocol}://admin-app.nwms.ir/v2/b2b-api/" + api_key + "/admin/ext-service/");


                Dictionary<string, object> input = new Dictionary<string, object>();
                input["cmp_national_code"] = co_national_id;
                HttpResponseMessage response = client_.PostAsJsonAsync("co_inq", input).Result;
                result = response.Content.ReadAsStringAsync().Result;
                if (string.IsNullOrWhiteSpace(co_national_id))
                {
                    result = @"{""co_national_id"":""empty""}";
                    return false;
                }
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Dictionary<string, object> data1 = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                    if (data1["Successful"].ToString() == "true" || data1["Successful"].ToString() == "True")
                    {
                        company.name = data1["Name"].ToString();
                        company.co_national_id = co_national_id;
                        company.address = data1["Address"].ToString();
                        company.RegisterNumber = data1["RegisterNumber"].ToString();
                        result = company.name;
                        return true;
                    }
                    else
                    {
                        result = data1["Message"].ToString();
                        company.co_national_id = co_national_id;
                        return false;
                    }
                }
                else
                {
                    result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                    return null;
                }


            }

            public static bool GetCixCompanyByCoNationalId(string api_key, string co_national_id, out CompanyClass company, out string result)
            {
                company = new CompanyClass();
                if (string.IsNullOrWhiteSpace(co_national_id))
                {
                    result = @"{""co_national_id"":""empty""}";
                    return false;
                }

                CixGetLegalPerson.GetLegalPersonInfoDBPortTypeClient client_ = SRL.Web.CreateWcfClient<CixGetLegalPerson.GetLegalPersonInfoDBPortTypeClient>("https://sr-cix.ntsw.ir/services/GetLegalPersonInfoDB?wsdl");


                SRL.Web.AddBasicAuthToSoapHeader(client_, "nwms_user", "2867%plfa@", 0);
                var inp = new CixGetLegalPerson.Parameter();
                inp.NationalCode = co_national_id;
                CixGetLegalPerson.Result response = client_.InquiryByNationalCode(inp);
                if (response.Message == null)
                {
                    company.name = response.Name;
                    company.co_national_id = co_national_id;
                    company.address = response.Address;
                    company.RegisterNumber = response.RegisterNumber.ToString();
                    result = company.name;
                    return true;
                }
                else
                {
                    result = response.Message;
                    return false;
                }


            }


            public static GetCompanyInAnbarResult GetCompanyInAnbar(string api_key, string national_id, string co_national_id, string protocol, out CompanyInAnbarClass company, out string error)
            {
                GetCompanyInAnbarResult company_result = new GetCompanyInAnbarResult();
                company = null;
                error = "";
                if (string.IsNullOrWhiteSpace(co_national_id))
                {
                    company_result = GetCompanyInAnbarResult.EmptyInput;
                    return company_result;
                }
                using (HttpClient client_ = new HttpClient())
                {
                    client_.BaseAddress = new Uri($"{protocol}://admin-app.nwms.ir/v2/b2b-api/" + api_key + "/admin/company/_search/");
                    Dictionary<string, object> input = new Dictionary<string, object>();
                    input["national_id"] = co_national_id;
                    HttpResponseMessage response = client_.PostAsJsonAsync("0/99", input).Result;
                    string result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result)["data"].ToString();
                        List<CompanyInAnbarClass> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CompanyInAnbarClass>>(data);
                        if (list.Count < 1)
                        {
                            company_result = GetCompanyInAnbarResult.NotFound;
                            return company_result;
                        }
                        foreach (var item in list)
                        {
                            if (item.account_status == "3") continue;
                            company = item;
                            if (item.ceo_national_id != national_id)
                            {
                                if (item.ceo_agents != null)
                                {
                                    if (item.ceo_agents.Where(x => x.national_id == national_id).Any())
                                    {
                                        company_result = GetCompanyInAnbarResult.OK;
                                        return company_result;
                                    }
                                    else
                                    {
                                        company_result = GetCompanyInAnbarResult.OKNotInCeos;
                                        return company_result;
                                    }
                                }
                                else
                                {
                                    company_result = GetCompanyInAnbarResult.OKNotCeos;
                                    return company_result;
                                }

                            }
                            else
                            {
                                company_result = GetCompanyInAnbarResult.OK;
                                return company_result;
                            }

                        }
                        company_result = GetCompanyInAnbarResult.NotActiveFound;
                        return company_result;

                    }
                    else
                    {
                        error = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        company_result = GetCompanyInAnbarResult.Error;
                        return company_result;
                    }

                }
            }

            public static CompanyListClass GetCompanySeoByCoNationalId(string co_national_id, string api_key, string protocol, out string result)
            {
                result = "";
                HttpClient client_list = new HttpClient();
                client_list.BaseAddress = new Uri($"{protocol}://admin-app.nwms.ir/v2/b2b-api/" + api_key + "/admin/company/_search/");

                if (string.IsNullOrWhiteSpace(co_national_id)) return null;

                Dictionary<string, object> input = new Dictionary<string, object>();
                input["national_id"] = co_national_id;
                HttpResponseMessage response = client_list.PostAsJsonAsync("0/99", input).Result;
                result = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result)["data"].ToString();

                    List<CompanyListClass> co_list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CompanyListClass>>(data);

                    foreach (var item in co_list)
                    {
                        if (item.account_status == "3") continue;

                        item.ceo.ceo_mobile = Nwms.GetAnbarUser(item.ceo.national_id, api_key, protocol, out result)?.mobile;
                        return item;
                    }
                    return null;
                }
                else
                {
                    return null;
                }

            }
            public static ComplexByPostCodeResult GetComplexByPostalCode(string postal_code, string api_key, bool remove_unknown, string protocol, out string result, string prefix = "app")
            {
                ComplexByPostCodeResult estelam_result = new ComplexByPostCodeResult();

                using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
                {
                    client.BaseAddress = new Uri($"{protocol}://{prefix}.nwms.ir/v2/b2b-api/");

                    HttpResponseMessage response = client.GetAsync(api_key + "/complex_by_post_code/" + postal_code).Result;
                    result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        estelam_result = Newtonsoft.Json.JsonConvert.DeserializeObject<ComplexByPostCodeResult>(result);

                        if (remove_unknown)
                        {
                            if (estelam_result.owners != null)
                            {
                                var un = estelam_result.owners.Where(x => x.national_id == "0000000000");
                                if (un.Any())
                                {
                                    estelam_result.owners.Remove(un.First());
                                }
                            }
                            if (estelam_result.agent != null)
                            {
                                if (estelam_result.agent.national_id == "0000000000")
                                {
                                    estelam_result.agent = null;
                                }
                            }
                        }

                        return estelam_result;
                    }
                    else
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        if (result == "NotFound") result = @"{""postal_code"":""NotFound""}";
                        return null;
                    }
                }

            }

            public static DataTable GetListFromNWMSByFilter(Dictionary<string, object> filter, string api, int? from = null, int? len = null, string org_creator_oposite = null)
            {
                DataTable dt = new DataTable();
                int from_api = 0;
                if (from != null) from_api = (int)from;
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(api + from_api + "/");
                int? len_api = len == null ? 100 : (len < 100 ? len : 100);
                var response_ = client.PostAsJsonAsync(len_api.ToString(), filter).Result;
                var response = response_.Content.ReadAsStringAsync().Result;

                string data =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(response)["data"].ToString();
                List<Dictionary<string, object>> list =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(data);

                int all_ = list.Count;
                while (list.Count > 0)
                {

                    DataTable dt_ = SRL.Json.ConvertJsonToDataTable(list);

                    if (!string.IsNullOrWhiteSpace(org_creator_oposite))
                    {
                        var query = dt_.AsEnumerable().Where(x => x.Field<string>("org_creator_national_id") != org_creator_oposite);
                        if (query.Any())
                        {
                            dt_ = query.CopyToDataTable<DataRow>();
                            SRL.Convertor.CopyDataTableToDataTable(dt_, dt);
                        }

                    }
                    else
                    {
                        SRL.Convertor.CopyDataTableToDataTable(dt_, dt);
                    }

                    from_api += 100;
                    client = new HttpClient();
                    client.BaseAddress = new Uri(api + from_api + "/");
                    list = new List<Dictionary<string, object>>();
                    if (all_ == len_api && (len == null ? true : from_api < len + from))
                    {
                        len_api = (len + from - from_api) > 100 ? 100 : len + from - from_api;
                        response_ = client.PostAsJsonAsync((len_api).ToString(), filter).Result;
                        response = response_.Content.ReadAsStringAsync().Result;
                        data =
                            Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(response)["data"].ToString();
                        list =
                           Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(data);
                        all_ = list.Count;
                    }
                }
                return dt;
            }


            public static int? UpdateBaseValueTable<T>(DbContext db, string api_key, ref string error) where T : class
            {
                SRL.Database.EntityRemoveAll<T>(db);

                foreach (BaseValueTypes.Base base_type in Enum.GetValues(typeof(BaseValueTypes.Base)))
                {
                    if (InsertBaseValue<T>(api_key, db, base_type, "http", ref error) == false)
                    {
                        return null;
                    }
                }

                int exe = db.SaveChanges();
                return exe;
            }

            public static bool InsertBaseValue<T>(string api_key, DbContext db, BaseValueTypes.Base base_type, string protocol, ref string error) where T : class
            {
                List<SRL.Projects.Nwms.BaseValueData> list = SRL.Projects.Nwms.GetBaseValue(api_key, base_type, protocol, out error);
                if (list == null)
                {
                    return false;
                }
                else
                {
                    foreach (var item in list)
                    {
                        T b = SRL.ClassManagement.CreateInstance<T>();
                        SRL.ClassManagement.SetProperty<T>("code", b, item.key);
                        SRL.ClassManagement.SetProperty<T>("title", b, item.GetTitle());
                        SRL.ClassManagement.SetProperty<T>("type", b, base_type.ToString());
                        db.Set<T>().Add(b);
                    }
                    return true;
                }
            }

            public static bool CreateApiKey(string api_key, string userid, string title, string key, string protocol, out string result)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{protocol}://admin-app.nwms.ir/v2/b2b-api/" + api_key + "/json_store/");
                    Dictionary<string, object> input = new Dictionary<string, object>();
                    Dictionary<string, object> user_key = new Dictionary<string, object>();
                    user_key["user_id"] = userid;
                    user_key["title"] = title;
                    user_key["key"] = key;
                    input["json_data"] = user_key;
                    HttpResponseMessage response = client.PostAsJsonAsync("b2b_api_key", input).Result;
                    result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        result = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result)["id"].ToString();
                        return true;
                    }
                    else
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return false;
                    }
                }
            }

            public static string AddCategory(string api_key, string title, string key, string protocol, out string result, string prefix = "admin-app")
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{protocol}://{prefix}.nwms.ir/v2/b2b-api/{ api_key }/admin/setting_keyvalue/");
                    Dictionary<string, string> title_obj = new Dictionary<string, string>();
                    title_obj["title"] = title;
                    Dictionary<string, string> input = new Dictionary<string, string>();
                    input["key"] = key;
                    input["value"] = SRL.Json.ClassObjectToJson(title_obj, Newtonsoft.Json.Formatting.None);
                    HttpResponseMessage response = client.PostAsJsonAsync("categories", input).Result;
                    result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string id = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result)["id"].ToString();
                        return id;
                    }
                    else
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return null;
                    }
                }
            }

            public static string AddTaxonomy(string api_key, string title, string key, string category_key, string protocol, out string result, string prefix = "app")
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{protocol}://{prefix}.nwms.ir/v2/b2b-api/{ api_key }/json_store/");
                    Dictionary<string, bool> categories = new Dictionary<string, bool>();
                    categories[category_key] = true;
                    Dictionary<string, object> json_data = new Dictionary<string, object>();
                    json_data["title"] = title;
                    json_data["key"] = key;
                    json_data["categories"] = categories;
                    Dictionary<string, object> input = new Dictionary<string, object>();
                    input["json_data"] = json_data;
                    HttpResponseMessage response = client.PostAsJsonAsync("taxonomies", input).Result;
                    result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string id = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result)["id"].ToString();
                        return id;
                    }
                    else
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return null;
                    }
                }
            }

            public static string AddGood(string api_key, string title, string key, string taxonomy_key, string protocol, out string result, string prefix = "app")
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{protocol}://{prefix}.nwms.ir/v2/b2b-api/{ api_key }/json_store/");
                    Dictionary<string, object> json_data = new Dictionary<string, object>();
                    json_data["title"] = title;
                    json_data["key"] = key;
                    json_data["taxonomy"] = taxonomy_key;
                    Dictionary<string, object> input = new Dictionary<string, object>();
                    input["json_data"] = json_data;
                    HttpResponseMessage response = client.PostAsJsonAsync("goods", input).Result;
                    result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string id = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result)["id"].ToString();
                        return id;
                    }
                    else
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return null;
                    }
                }
            }

            public static string DeleteJsonStore(string api_key, string id, string protocol, string prefix = "app")
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{protocol}://{prefix}.nwms.ir/v2/b2b-api/{ api_key }/json_store/");
                    HttpResponseMessage response = client.DeleteAsync(id).Result;
                    string result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return null;
                    }
                    else
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return result;
                    }
                }
            }
            public static bool DeleteComplexById(string id, string api_key, string protocol, out string message)
            {
                Dictionary<string, object> inp = new Dictionary<string, object>();
                inp["account_status"] = "3";
                message = "";
                return PutComplex(api_key, id, inp, protocol, out message);

            }

            public static bool DeleteComplex(string postal_code, string api_key, string protocol, out string message)
            {
                Dictionary<string, object> inp = new Dictionary<string, object>();
                inp["account_status"] = "3";
                message = "";
                var list = GetComplex(api_key, postal_code, protocol, ref message);
                if (list == null)
                {
                    return false;
                }
                else
                {
                    string id = list.id;
                    return PutComplex(api_key, id, inp, protocol, out message);
                }

            }

            public static string DeleteCategory(string api_key, string uid, int version, string protocol, string prefix = "app")
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{protocol}://{prefix}.nwms.ir/v2/b2b-api/{ api_key }/admin/setting_keyvalue/");
                    HttpResponseMessage response = client.DeleteAsync($"{uid}?version={version}").Result;
                    string result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return null;
                    }
                    else
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return result;
                    }
                }
            }

            public static List<JsonDataResult> SearchTaxonomy(string api_key, string category, int start, int size, string protocol, ref string result)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{protocol}://app.nwms.ir/v2/b2b-api/{ api_key }/json_store/taxonomies/_search/0/");
                    Dictionary<string, object> filter = new Dictionary<string, object>();
                    Dictionary<string, object> json = new Dictionary<string, object>();
                    string json_categories = $"json_data.categories.{category}";
                    json[json_categories] = true;
                    json["json_data.custom_pagination_start"] = start;
                    json["json_data.custom_pagination_size"] = size;

                    filter["filter"] = json;
                    HttpResponseMessage response = client.PostAsJsonAsync("10", filter).Result;
                    result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result)["data"].ToString();
                        List<JsonDataResult> json_result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<JsonDataResult>>(data);
                        return json_result;
                    }
                    else
                    {
                        result = SRL.Json.IsJson(result) ? result : response.StatusCode.ToString();
                        return null;
                    }
                }
            }
            private static void StartDeleteComplexById(List<DeleteComplexByIdClass> list, BackgroundWorker worker, params object[] args)
            {
                string table_name = args[0].ToString();
                string access_file_name = args[1].ToString();
                string api_key = args[2].ToString();

                foreach (var item in list)
                {
                    string query = "";


                    string result = "";
                    CompanyClass company = new CompanyClass();
                    var is_ok = SRL.Projects.Nwms.DeleteComplexById(item.complex_id, api_key, "https", out result);

                    if (is_ok == false)
                    {
                        query = $"update {table_name} set status='NOK', error='{result}'  where complex_id='{item.complex_id}'";
                    }
                    else
                    {
                        query = $"update {table_name} set status='OK'  where complex_id='" + item.complex_id + "'";
                    }


                    SRL.AccessManagement.ExecuteToAccess(query, access_file_name);
                }
            }

            public static void DeleteComplexByIdGroup(string api_key, string access_file_name, string table_name, int paralel, Action<Exception> call_error, Action final_call_back)
            {
                SRL.AccessManagement.AddColumnToAccess("error", table_name, SRL.AccessManagement.AccessDataType.nvarcharmax, access_file_name);
                SRL.AccessManagement.AddColumnToAccess("status", table_name, SRL.AccessManagement.AccessDataType.nvarcharmax, access_file_name);

                DataTable dt = SRL.AccessManagement.GetDataTableFromAccess(access_file_name, table_name);
                var list_ = SRL.Convertor.ConvertDataTableToList<DeleteComplexByIdClass>(dt);
                var list = list_.Where(x => x.status == "" || x.status == null || x.status != "OK").ToList();

                SRL.AccessManagement.ExecuteToAccess("update " + table_name + " set complex_id=Trim(complex_id)", access_file_name);

                SRL.ActionManagement.MethodCall.Parallel.ParallelCall<DeleteComplexByIdClass>(list, paralel.ToString(),
                    StartDeleteComplexById, null, call_error, final_call_back, null, table_name, access_file_name, api_key);
            }
            public class DeleteComplexByIdClass
            {
                public string complex_id { get; set; }
                public string error { get; set; }
                public string status { get; set; }
            }


            public class JsonDataResult
            {
                public string doc_type { get; set; }
                public JsonData json_data { get; set; }
                public string id { get; set; }
                public class JsonData
                {
                    public string id { get; set; }
                    public string key { get; set; }
                    public string title { get; set; }
                }
            }

            public class CardexFilter
            {
                public string warehouse_id { get; set; }//virt_id
                public string user_id { get; set; }//owner_nc
                public string good_id { get; set; }
                public string postal_code { get; set; }

                public string province { get; set; }
                public string township { get; set; }
                public string city { get; set; }

            }

            public class CardexResult
            {
                public string user_id { get; set; }
                public string data_owner { get; set; }
                public string good_id { get; set; }
                public string good_desc { get; set; }
                public Creator creator { get; set; }
                public double? removable_count { get; set; }
                public double? total_count { get; set; }
                public string warehouse_id { get; set; }
                public double? modify_date { get; set; }
                public AdditionalData additional_data { get; set; }
                public double? create_date { get; set; }
                public object modifier { get; set; }
                public string id { get; set; }
                public string production_type { get; set; }

                public class AdditionalData
                {
                    public object province { get; set; }
                    public object city { get; set; }
                    public object township { get; set; }
                    public string postal_code { get; set; }
                    public string org_creator_nationa_id { get; set; }
                }

                public class Creator
                {
                    public object create_date { get; set; }
                    public string national_id { get; set; }
                    public string modifier_national_id { get; set; }
                    public object phonenumber { get; set; }
                    public string account_status { get; set; }
                    public string alivestatus { get; set; }
                    public object password { get; set; }
                    public string creator_national_id { get; set; }
                    public string lastname { get; set; }
                    public object SSNN_serial { get; set; }
                    public string email { get; set; }
                    public object files { get; set; }
                    public string firstname { get; set; }
                    public string using_two_phase_password { get; set; }
                    public string SSNN_no { get; set; }
                    public object address { get; set; }
                    public string postalcode { get; set; }
                    public object org_creator_national_id { get; set; }
                    public string birthepoch { get; set; }
                    public object user_national_card_image { get; set; }
                    public object user_profile_image { get; set; }
                    public string mobile { get; set; }
                    public string gender { get; set; }
                    public string userid { get; set; }
                    public string fathername { get; set; }
                }

            }
        }

        public class MeliSms
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string FromNumber { get; set; }
            public ServiceReferenceSendSms.SendSoapClient send_client;

            public MeliSms(string username, string password, string from_number)
            {
                //"09114452764", "2275",from_number = "500010604260"
                Username = username;
                Password = password;
                FromNumber = from_number;
                send_client = new ServiceReferenceSendSms.SendSoapClient("SendSoap12");
            }

            public Dictionary<string, string> SendSms(string text, string[] mobiles_to)
            {
                string[] result = send_client.SendSimpleSMS(Username, Password, mobiles_to, FromNumber, text, false);
                Dictionary<string, string> mobile_res = new Dictionary<string, string>();
                for (int i = 0; i < mobiles_to.Count(); i++)
                {
                    mobile_res[mobiles_to[i]] = GetErrorName(result[i]);
                }

                return mobile_res;
            }

            public double GetCredit()
            {
                double credit = send_client.GetCredit(Username, Password);
                return Math.Ceiling(credit);
            }

            private string GetErrorName(string code)
            {
                Dictionary<string, string> error_name = new Dictionary<string, string>();
                error_name["0"] = "نام کاربری یا رمز عبور اشتباه است";
                error_name["1"] = "درخواست با موفقیت انجام شد";
                error_name["2"] = "اعتبار کافی نمی باشد";
                error_name["3"] = "محدودیت در ارسال روزانه";
                error_name["4"] = "محدودیت در حجم ارسال";
                error_name["5"] = "شماره فرستنده معتبر نمی باشد";
                error_name["6"] = "سامانه در حال بروزرسانی می باشد";
                error_name["7"] = "متن حاوی کلمه فیلتر شده می باشد";
                error_name["9"] = "ارسال از خطوط عمومی از طریق وب سرویس مکانپذیر نمی باشد";
                error_name["10"] = "کاربر مورد نظر فعال نمی باشد";
                error_name["11"] = "ارسال نشده";
                error_name["0"] = "مدارک کابر کامل نمی باشد";
                if (error_name.ContainsKey(code))
                    return error_name[code];
                else return code;
            }

            public void ShowCreditInControl(Control control_to_show)
            {
                control_to_show.Text = "در حال بروزرسانی...";
                double credit = 0;
                SRL.ActionManagement.MethodCall.RunAsyncByWorker(() =>
                {
                    credit = GetCredit();
                }, () =>
                {
                    control_to_show.Text = credit.ToString();
                }, null, null, ProgressBarStyle.Blocks);

            }
        }
    }


}
