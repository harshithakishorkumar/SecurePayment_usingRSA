using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Text;
using System.Security.Cryptography;

public partial class Registration : System.Web.UI.Page
{
    SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConsumerConnect"].ConnectionString);
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string k = "SELECT TOP 1 Uid from [User] ORDER BY Uid DESC";
            SqlDataAdapter da = new SqlDataAdapter(k,conn);
            DataSet ds = new DataSet();
            da.Fill(ds);
            int i = 101;
            int c = ds.Tables[0].Rows.Count;
            if (c > 0)
            {
                i = Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString())+1;
            }
            else
            {
                i = 101;
            }
            TextBox1.Text = i.ToString();
        }
    }
    protected void ButtonSubmit_Click(object sender, EventArgs e)
    {
        if (TextBox2.Text != "" && TextBox3.Text != "" && TextBox4.Text != "" && TextBox5.Text != "" && TextBox6.Text != "" && TextBox7.Text != "")
        {
            string encval = Encryption(TextBox2.Text, TextBox3.Text, TextBox4.Text, TextBox5.Text, RadioButtonList1.Text,TextBox6.Text,TextBox7.Text);

            string[] encdata = encval.Split('~');
            string k = "INSERT INTO dbo.[User] VALUES('" + TextBox1.Text + "','" + encdata[0] + "','" + encdata[1] + "','" + encdata[2] + "','" + encdata[3] + "','" + encdata[4] + "','" + encdata[5] + "','" + encdata[6] + "')";
            SqlCommand cmd = new SqlCommand(k,conn);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            Response.Redirect("UserLogin.aspx");
        }
        else
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "msgtype", "alert('Field cannot be left blank.')", true);
        }
    }

    public static string Encryption(string name, string email,string contact,string address,string gender,string age,string pass)
    {
        var publicKey = "<RSAKeyValue><Modulus>21wEnTU+mcD2w0Lfo1Gv4rtcSWsQJQTNa6gio05AOkV/Er9w3Y13Ddo5wGtjJ19402S71HUeN0vbKILLJdRSES5MHSdJPSVrOqdrll/vLXxDxWs/U0UT1c8u6k/Ogx9hTtZxYwoeYqdhDblof3E75d9n2F0Zvf6iTb4cI7j6fMs=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        string data0 = name + "~" + email + "~" + contact + "~" + address + "~" + gender + "~" + age + "~" + pass;
        string[] data1 = data0.Split('~');
        string returndata = "";
        for (int i = 0; i < data1.Length; i++)
        {
            var testData = Encoding.UTF8.GetBytes(data1[i]);

            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                try
                {
                    // client encrypting data with public key issued by server                    
                    rsa.FromXmlString(publicKey.ToString());

                    var encryptedData = rsa.Encrypt(testData, true);

                    var base64Encrypted = Convert.ToBase64String(encryptedData);

                    returndata += base64Encrypted + "~";
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }
        return returndata.TrimEnd('~');
    }
}