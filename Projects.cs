using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace SRL
{
    public class Projects
    {
        public class Nwms
        {
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

                            //try
                            //{
                            HttpResponseMessage response = new HttpResponseMessage();
                            if (item.code == null) continue;
                            if (item.code.Length > 10)
                            {
                                var get = SRL.Projects.Nwms.GetCompanyByCoNationalId(item.code, out response);

                                if (get == null)
                                {
                                    string query = "update " + table_name + " set status='" + response.StatusCode.ToString() + "'  where code='" + item.code + "'";
                                    SRL.AccessManagement.ExecuteToAccess(query, access_file_name, true);
                                }
                                else if (string.IsNullOrWhiteSpace(get.error_name))
                                {
                                    string query = "update " + table_name + " set status='" + response.StatusCode.ToString() + "' , name='" + get.name + "' where code='" + item.code + "'";
                                    SRL.AccessManagement.ExecuteToAccess(query, access_file_name, true);
                                }
                                else
                                {
                                    string query = "update " + table_name + " set status='" + response.StatusCode.ToString() + "' , error='" + get.error_name + "' where code='" + item.code + "'";
                                    SRL.AccessManagement.ExecuteToAccess(query, access_file_name, true);
                                }
                            }
                            else
                            {
                                var get = SRL.Projects.Nwms.GetPersonByNationalId(item.code, out response);
                                if (string.IsNullOrWhiteSpace(get.ErrorDescription))
                                {
                                    string query = "update " + table_name + " set status='" + response.StatusCode.ToString() + "' , name='" + get.FirstName + "', family='" + get.LastName + "' where code='" + item.code + "'";
                                    SRL.AccessManagement.ExecuteToAccess(query, access_file_name, true);
                                }
                                else
                                {
                                    string query = "update " + table_name + " set status='" + response.StatusCode.ToString() + "' , error='" + get.ErrorDescription + "' where code='" + item.code + "'";
                                    SRL.AccessManagement.ExecuteToAccess(query, access_file_name, true);
                                }
                            }
                            //}
                            //catch (Exception ex)
                            //{
                            //    MessageBox.Show(ex.Message);
                            //}
                        }
                    }

                    public static void CheckCoOrPersonIsCorrect(string access_file_name, string table_name, int paralel)
                    {
                        SRL.AccessManagement.AddColumnToAccess("name", table_name, SRL.AccessManagement.AccessDataType.nvarcharmax, access_file_name, true);
                        SRL.AccessManagement.AddColumnToAccess("family", table_name, SRL.AccessManagement.AccessDataType.nvarcharmax, access_file_name, true);
                        SRL.AccessManagement.AddColumnToAccess("error", table_name, SRL.AccessManagement.AccessDataType.nvarcharmax, access_file_name, true);
                        SRL.AccessManagement.AddColumnToAccess("status", table_name, SRL.AccessManagement.AccessDataType.nvarcharmax, access_file_name, true);

                        DataTable dt = SRL.AccessManagement.GetDataTableFromAccess(access_file_name, table_name);
                        var list_ = SRL.Convertor.ConvertDataTableToList<CoOrPerson>(dt);
                        var list = list_.Where(x => x.status == "" || x.status == null || x.status != "OK").ToList();

                        SRL.AccessManagement.ExecuteToAccess("update " + table_name + " set code=Trim(code)", access_file_name, true);

                        SRL.ActionManagement.MethodCall.ParallelMethodCaller.ParallelCall<CoOrPerson>(list, paralel.ToString(), StartParallelCallCoOrPerson, () => { MessageBox.Show("done"); }, null, table_name, access_file_name);
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
                public static void Estelam(string file_full_path, string table_name, string api_key)
                {
                    DataTable table = SRL.AccessManagement.GetDataTableFromAccess(file_full_path, table_name);

                    List<PostCodeEstelamResult> list = SRL.Convertor.ConvertDataTableToList<PostCodeEstelamResult>(table);

                    foreach (var item in list)
                    {
                        if (item.status == "OK") continue;

                        HttpResponseMessage response = new HttpResponseMessage();
                        string message = "";
                        SRL.Projects.Nwms.ComplexByPostCodeResult war =
                        SRL.Projects.Nwms.GetComplexByPostalCode(item.postal_code, api_key,  out message);
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            try
                            {
                                string query = "update " + table_name + " set status='OK' , exist_anbar='" + war.warehouse_server + "' , correct='" + war.postal_code_server + "' "
                                    + " , province='" + war.province + "'  , township='" + war.township + "'  , city='" + war.city + "'"
                                    + "  , address='" + war.full_address + "'"
                                    + " where postal_code='" + item.postal_code + "' ;";
                                SRL.AccessManagement.ExecuteToAccess(query, file_full_path, true);


                            }
                            catch (Exception ex)
                            {
                                string query = "update " + table_name + " set  error='" + ex.Message + "' where ID=" + item.ID + " ;";
                                SRL.AccessManagement.ExecuteToAccess(query, file_full_path, true);
                            }
                        }
                        else
                        {
                            string query = "update " + table_name + " set status='" + response.StatusCode.ToString() + "' , error='" + message + "' where ID=" + item.ID + " ;";
                            SRL.AccessManagement.ExecuteToAccess(query, file_full_path, true);
                        }

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

                            if (result.ErrorCode ==0)

                            {
                                address += "استان " + result.State.Trim() + "- شهرستان " + result.TownShip.Trim() + "- بخش " +result.Zone.Trim()+ "- " + result.LocationType.Trim() + " " + result.Location.Trim()
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

                            var exce = SRL.AccessManagement.ExecuteToAccess(query, args[3].ToString(), false);


                        }
                        catch (Exception ex)
                        {
                            string query = "update " + args[4] + " set  status='" + ex.Message + "' where ID=" + item.ID + " ;";
                            SRL.AccessManagement.ExecuteToAccess(query, args[3].ToString(), true);
                        }
                    }
                }

                public static void EstelamFromPost(string file_full_path, string table_name, string api_key, string parallel, string password, string username)
                {
                    DataTable table = SRL.AccessManagement.GetDataTableFromAccess(file_full_path, table_name);
                    List<GetAddressByPostServerResult> list = SRL.Convertor.ConvertDataTableToList<GetAddressByPostServerResult>(table).Where(x => x.status != "OK" || x.status == null || x.status == "").ToList();
                    PostCodeServiceReference.PostCodeClient client = new PostCodeServiceReference.PostCodeClient();
                    SRL.ActionManagement.MethodCall.ParallelMethodCaller.ParallelCall<GetAddressByPostServerResult>(list, parallel, ParallelAddressByPostServer, null, null, password, client, username, file_full_path, table_name);
                }
            }

            public class ShahkarInputClass
            {
                public string requestId { get; set; }
                public string serviceNumber { get; set; }
                public int serviceType { get; set; }
                public int identificationType { get; set; }
                public string identificationNo { get; set; }
            }

            public class ShahkarOutputClass
            {
                public int response { get; set; }
                public string requestId { get; set; }
                public string result { get; set; }
                public string comment { get; set; }
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

            public static List<SearchResult.SearchWarehouseResult> SearchWarehouse(Dictionary<string, object> input_json, string api_key)
            {
                List<SearchResult.SearchWarehouseResult> warehouses = new List<SearchResult.SearchWarehouseResult>();
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://app.nwms.ir/v2/b2b-api/" + api_key + "/admin/warehouse/_search/");
                List<SearchResult.SearchWarehouseResult> warehouse_list = new List<SearchResult.SearchWarehouseResult>();
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
                    warehouse_list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SearchResult.SearchWarehouseResult>>(data);

                    warehouses.AddRange(warehouse_list);
                    from += 10;
                } while (warehouse_list.Count == 10);

                return warehouses;
            }


            public static List<SearchResult.SearchComplexResult> SearchComplex(Dictionary<string, object> input_json, string api_key)
            {
                List<SearchResult.SearchComplexResult> complexes = new List<SearchResult.SearchComplexResult>();
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://app.nwms.ir/v2/b2b-api/" + api_key + "/admin/complex/_search/");

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

            public static List<SearchResult.SearchComplexResult> GetAllWarComplexByNationalId(string api_key, string national_id)
            {

                List<SearchResult.SearchComplexResult> complexes = new List<SearchResult.SearchComplexResult>();
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://app.nwms.ir");

                HttpResponseMessage response = client.GetAsync("/v2/b2b-api-imp/" + api_key + "/" + national_id + "/complex/_all").Result;
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return null;
                }

                string result = response.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result_json = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                string data = result_json["data"].ToString();
                complexes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SearchResult.SearchComplexResult>>(data);


                return complexes;
            }

            public class ComplexByPostCodeResult
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

            }

            public class AnbarPersonClass
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
            public class CompanyClass
            {
                public long ID { get; set; }
                public string co_national_id { get; set; }
                public string name { get; set; }
                public string error_name { get; set; }
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


            public static PersonClass GetPersonByNationalId(string national_id, out HttpResponseMessage response)
            {
                PersonClass person = new PersonClass();
                person = null;
                response = null;
                HttpClient client_ = new HttpClient();
                client_.BaseAddress = new Uri("https://admin-app.nwms.ir/v2/b2b-api/2050130318/admin/ext-service/");
                national_id = SRL.Convertor.NationalId(national_id);
                if (string.IsNullOrWhiteSpace(national_id)) return null;
                Dictionary<string, object> input = new Dictionary<string, object>();
                input["national_id"] = national_id;
                response = client_.PostAsJsonAsync("person_by_national_id", input).Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    person = new PersonClass();
                    string result = response.Content.ReadAsStringAsync().Result;
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                    if (data["FirstName"] != null)
                    {
                        person.FirstName = data["FirstName"].ToString();
                        person.LastName = data["LastName"].ToString();
                        person.last_sent = DateTime.Now.ToString();
                        person.national_id = national_id;
                    }
                    else
                    {
                        person.ErrorDescription = data["ErrorDescription"].ToString();
                        person.national_id = national_id;
                    }

                }

                return person;
            }

            public static AnbarPersonClass GetAnbarPersonByNationalId(string national_id, string api_key)
            {

                HttpClient client1 = new HttpClient();
                client1.BaseAddress = new Uri("https://app.nwms.ir");
                // Dictionary<string, object> input = new Dictionary<string, object>();
                // input.Add("account_status", "1");
                // input.Add("national_id", national_id);
                //result = client1.PostAsJsonAsync("/v2/b2b-api/2050130318/admin/users/_search/0/1", input).Result;
                HttpResponseMessage response = client1.GetAsync("/v2/b2b-api-imp/" + api_key + "/" + national_id + "/user").Result;
                string result = response.Content.ReadAsStringAsync().Result;
                AnbarPersonClass person = Newtonsoft.Json.JsonConvert.DeserializeObject<AnbarPersonClass>(result);
                return person;
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
            public static PostalCodeFromPostClass EstelamPostalCodeFromPost(string postal_code, out HttpResponseMessage response)
            {
                PostalCodeFromPostClass post = new PostalCodeFromPostClass();
                post = null;
                response = null;
                HttpClient client_ = new HttpClient();
                client_.BaseAddress = new Uri("https://admin-app.nwms.ir/v2/b2b-api/2050130318/admin/ext-service/");
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
            public static CompanyClass GetCompanyByCoNationalId(string co_national_id, out HttpResponseMessage response)
            {
                CompanyClass company = new CompanyClass();
                company = null;
                response = null;

                HttpClient client_ = new HttpClient();
                client_.BaseAddress = new Uri("https://admin-app.nwms.ir/v2/b2b-api/2050130318/admin/ext-service/");


                if (string.IsNullOrWhiteSpace(co_national_id)) return null;

                Dictionary<string, object> input = new Dictionary<string, object>();
                input["cmp_national_code"] = co_national_id;
                response = client_.PostAsJsonAsync("co_inq", input).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    company = new CompanyClass();
                    string result = response.Content.ReadAsStringAsync().Result;
                    Dictionary<string, object> data1 = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                    if (data1["Successful"].ToString() == "true" || data1["Successful"].ToString() == "True")
                    {

                        company.name = data1["Name"].ToString();
                        company.co_national_id = co_national_id;
                    }
                    else
                    {
                        company.error_name = data1["Message"].ToString();
                        company.co_national_id = co_national_id;
                    }
                }
                return company;


            }

            public static List<CompanyListClass> GetCompanySeoByCoNationalId(string co_national_id, string api_key, out HttpResponseMessage response)
            {
                List<CompanyListClass> result_list = new List<CompanyListClass>();

                response = null;

                HttpClient client_list = new HttpClient();
                client_list.BaseAddress = new Uri("https://admin-app.nwms.ir/v2/b2b-api/" + api_key + "/admin/company/_search/");


                if (string.IsNullOrWhiteSpace(co_national_id)) return null;

                Dictionary<string, object> input = new Dictionary<string, object>();
                input["national_id"] = co_national_id;
                response = client_list.PostAsJsonAsync("0/99", input).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {


                    string result = response.Content.ReadAsStringAsync().Result;

                    string data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result)["data"].ToString();

                    List<CompanyListClass> co_list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CompanyListClass>>(data);

                    foreach (var item in co_list)
                    {
                        if (item.account_status == "3") continue;

                        item.ceo.ceo_mobile = Nwms.GetAnbarPersonByNationalId(item.ceo.national_id, api_key)?.mobile;
                        result_list.Add(item);

                    }
                }
                else
                {
                    result_list = null;
                }

                return result_list;


            }

            public static ComplexByPostCodeResult GetComplexByPostalCode(string postal_code, string api_key, out string result)
            {
                ComplexByPostCodeResult estelam_result = new ComplexByPostCodeResult();
                estelam_result = null;

                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                client.BaseAddress = new Uri("https://app.nwms.ir/v2/b2b-api/");

                var call = client.GetAsync(api_key + "/complex_by_post_code/" + postal_code);
                HttpResponseMessage response = call.Result;
                string result_ = response.Content.ReadAsStringAsync().Result;
                result = System.Text.RegularExpressions.Regex.Unescape(result_);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    estelam_result = Newtonsoft.Json.JsonConvert.DeserializeObject<ComplexByPostCodeResult>(result);

                }
                return estelam_result;

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
                    public string postal_code { get; set; }
                    public List<Person> contractors { get; set; }
                    public string org_creator_national_id { get; set; }
                    public string name { get; set; }
                    public string id { get; set; }

                }


            }


        }
    }
}
