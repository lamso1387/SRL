using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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

                    public static void CheckCoOrPersonIsCorrect(string access_file_name, string table_name)
                    {
                        SRL.AccessManagement.AddColumnToAccess("name", table_name, SRL.AccessManagement.AccessDataType.nvarcharmax, access_file_name, true);
                        SRL.AccessManagement.AddColumnToAccess("family", table_name, SRL.AccessManagement.AccessDataType.nvarcharmax, access_file_name, true);
                        SRL.AccessManagement.AddColumnToAccess("error", table_name, SRL.AccessManagement.AccessDataType.nvarcharmax, access_file_name, true);
                        SRL.AccessManagement.AddColumnToAccess("status", table_name, SRL.AccessManagement.AccessDataType.nvarcharmax, access_file_name, true);

                        DataTable dt = SRL.AccessManagement.GetDataTableFromAccess(access_file_name, table_name);
                        var list_ = SRL.Convertor.ConvertDataTableToList<CoOrPerson>(dt);
                        var list = list_.Where(x => x.status == "" || x.status == null || x.status != "OK").ToList();

                        int count = list.Count();
                        foreach (var item in list)
                        { 

                            try
                            {
                                HttpResponseMessage response = new HttpResponseMessage();

                                if (item.code.Length > 10)
                                {
                                    var get = SRL.Projects.Nwms.GetCompanyByCoNationalId(item.code, out response);

                                    if (string.IsNullOrWhiteSpace(get.error_name))
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
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
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
                        SRL.Projects.Nwms.GetComplexByPostalCode(item.postal_code, api_key, out response, out message);
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

            public class CompanyClass
            {
                public long ID { get; set; }
                public string co_national_id { get; set; }
                public string name { get; set; }
                public string error_name { get; set; }
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
                    if (data1["Successful"].ToString() == "true")
                    {
                        string data3 = data1["data"].ToString();
                        Dictionary<string, object> data4 = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(data3);

                        company.name = data4["Name"].ToString();
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

            public static ComplexByPostCodeResult GetComplexByPostalCode(string postal_code, string api_key, out HttpResponseMessage response, out string result)
            {
                ComplexByPostCodeResult estelam_result = new ComplexByPostCodeResult();
                estelam_result = null;

                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                client.BaseAddress = new Uri("https://app.nwms.ir/v2/b2b-api/");

                var call = client.GetAsync(api_key + "/complex_by_post_code/" + postal_code);
                response =call .Result;
                string result_ = response.Content.ReadAsStringAsync().Result;
                result = System.Text.RegularExpressions.Regex.Unescape(result_);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    estelam_result = Newtonsoft.Json.JsonConvert.DeserializeObject<ComplexByPostCodeResult>(result);

                }

                return estelam_result;

            }

        }
    }
}
