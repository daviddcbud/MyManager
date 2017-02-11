using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.ServiceBusServices
{
     [ServiceContract]
    public interface IMyService
    {
        [OperationContract]
        bool Ping();
        [OperationContract]
        List<Login> GetLogins(string password);

    }
    [DataContract]
    public class Login
    {
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string Url { get; set; }

    }
    public class MyService : IMyService
    {
        public List<Login> GetLogins(string password)
        {
            var list = new List<Login>();
            using(var conn=new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("select * FROM useraccounts order by description", conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new Login
                    {
                        UserName=reader["UserName"].ToString(),
                        Description=reader["Description"].ToString(),
                        Url= reader["url"].ToString(),
                        Password = EncDec.Decrypt(reader["password"].ToString(), password),
                });

                }
                reader.Close();
            }
            return list;
        }

        public bool Ping()
        {
            return true;
        }
    }
}
