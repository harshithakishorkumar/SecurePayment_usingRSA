using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Security.Cryptography;

public partial class UserDetails : System.Web.UI.Page
{
    SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConsumerConnect"].ConnectionString);
    protected void Page_Load(object sender, EventArgs e)
    {
        string k = "SELECT uid,username,email,contact,address,gender,age FROM [User]";
        SqlDataAdapter da = new SqlDataAdapter(k, conn);
        DataSet ds = new DataSet();
        da.Fill(ds);
        int c = ds.Tables[0].Rows.Count;
        if (c > 0)
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[7] {
                            new DataColumn("Uid", typeof(string)),
                            new DataColumn("Username", typeof(string)),
                            new DataColumn("Email", typeof(string)),
                            new DataColumn("Contact",typeof(string)),
                            new DataColumn("Address",typeof(string)),
                            new DataColumn("Gender",typeof(string)),
                            new DataColumn("Age",typeof(string))
            });
            for (int i=0; i<c;i++)
            {
                string id = ds.Tables[0].Rows[0][0].ToString();
                string name = Decryption(ds.Tables[0].Rows[0][1].ToString());
                string email = Decryption(ds.Tables[0].Rows[0][2].ToString());
                string contact = Decryption(ds.Tables[0].Rows[0][3].ToString());
                string address = Decryption(ds.Tables[0].Rows[0][4].ToString());
                string gender = Decryption(ds.Tables[0].Rows[0][5].ToString());
                string age = Decryption(ds.Tables[0].Rows[0][6].ToString());
                dt.Rows.Add(id, name, email, contact,address,gender,age);
            }
            GridView1.DataSource = dt;
            GridView1.DataBind();
            GridView1.Visible = true;
        }
        else
        {
            GridView1.Visible = false;
            Page.ClientScript.RegisterStartupScript(GetType(), "msgtype", "alert('No Data Available.')", true);
        }
    }

    public static string Decryption(string strText)
    {
        var privateKey = "<RSAKeyValue><Modulus>21wEnTU+mcD2w0Lfo1Gv4rtcSWsQJQTNa6gio05AOkV/Er9w3Y13Ddo5wGtjJ19402S71HUeN0vbKILLJdRSES5MHSdJPSVrOqdrll/vLXxDxWs/U0UT1c8u6k/Ogx9hTtZxYwoeYqdhDblof3E75d9n2F0Zvf6iTb4cI7j6fMs=</Modulus><Exponent>AQAB</Exponent><P>/aULPE6jd5IkwtWXmReyMUhmI/nfwfkQSyl7tsg2PKdpcxk4mpPZUdEQhHQLvE84w2DhTyYkPHCtq/mMKE3MHw==</P><Q>3WV46X9Arg2l9cxb67KVlNVXyCqc/w+LWt/tbhLJvV2xCF/0rWKPsBJ9MC6cquaqNPxWWEav8RAVbmmGrJt51Q==</Q><DP>8TuZFgBMpBoQcGUoS2goB4st6aVq1FcG0hVgHhUI0GMAfYFNPmbDV3cY2IBt8Oj/uYJYhyhlaj5YTqmGTYbATQ==</DP><DQ>FIoVbZQgrAUYIHWVEYi/187zFd7eMct/Yi7kGBImJStMATrluDAspGkStCWe4zwDDmdam1XzfKnBUzz3AYxrAQ==</DQ><InverseQ>QPU3Tmt8nznSgYZ+5jUo9E0SfjiTu435ihANiHqqjasaUNvOHKumqzuBZ8NRtkUhS6dsOEb8A2ODvy7KswUxyA==</InverseQ><D>cgoRoAUpSVfHMdYXW9nA3dfX75dIamZnwPtFHq80ttagbIe4ToYYCcyUz5NElhiNQSESgS5uCgNWqWXt5PnPu4XmCXx6utco1UVH8HGLahzbAnSy6Cj3iUIQ7Gj+9gQ7PkC434HTtHazmxVgIR5l56ZjoQ8yGNCPZnsdYEmhJWk=</D></RSAKeyValue>";

        var testData = Encoding.UTF8.GetBytes(strText);

        using (var rsa = new RSACryptoServiceProvider(1024))
        {
            try
            {
                var base64Encrypted = strText;

                // server decrypting data with private key                    
                rsa.FromXmlString(privateKey);

                var resultBytes = Convert.FromBase64String(base64Encrypted);
                var decryptedBytes = rsa.Decrypt(resultBytes, true);
                var decryptedData = Encoding.UTF8.GetString(decryptedBytes);
                return decryptedData.ToString();
            }
            finally
            {
                rsa.PersistKeyInCsp = false;
            }
        }
    }
}