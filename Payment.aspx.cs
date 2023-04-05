using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;

public partial class Payment : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConsumerConnect"].ConnectionString);
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Label6.Text = (string)Session["cost"];
            LabelUid.Text = Session["Uid"].ToString();
        }
    }
    protected void Button1_Click(object sender, EventArgs e)
    {

        string pname = "";
        string pid = "";
        string qty = "";

        SqlDataAdapter da;
        DataSet ds;
        string sel = "";
        string oid = "";
        sel = "SELECT CAST(Oid AS int) AS Expr1 FROM Orders ORDER BY Expr1 desc";
        da = new SqlDataAdapter(sel, con);
        ds = new DataSet();
        da.Fill(ds);
        if (ds.Tables[0].Rows.Count == 0)
        {
            oid = "1";
        }
        else
        {
            string s = ds.Tables[0].Rows[0][0].ToString();
            int s1 = Convert.ToInt32(s) + 1;
            oid = s1.ToString();
        }

        if (Session["BuyNow"] != "Data")
        {
            sel = "select pid,productname,qty from cart where uid='" + LabelUid.Text + "'";
            da = new SqlDataAdapter(sel, con);
            ds = new DataSet();
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    pid = ds.Tables[0].Rows[i][0].ToString();
                    pname = ds.Tables[0].Rows[i][1].ToString();
                    qty = ds.Tables[0].Rows[i][2].ToString();

                    string sql2 = "insert into orders(Oid,Productname,pid,qty,cost,uid,date,time,status) values('" + oid + "','" + pname + "','" + pid + "','" + qty + "','" + Label6.Text + "','" + LabelUid.Text + "','" + System.DateTime.Now.ToString("yyyy/MM/dd") + "','" + System.DateTime.Now.ToString("HH:mm") + "','Payment Success')";
                    SqlCommand cmd2 = new SqlCommand(sql2, con);
                    con.Open();
                    cmd2.ExecuteNonQuery();
                    con.Close();
                }
            }
        }
        else
        {
            pname = Session["pname"].ToString();
            pid = Session["pid"].ToString();

            

            string sql2 = "insert into orders(Oid,Productname,pid,qty,cost,uid,date,time,status) values('" + oid + "','" + pname + "','" + pid + "','1','" + Label6.Text + "','" + LabelUid.Text + "','" + System.DateTime.Now.ToString("yyyy/MM/dd") + "','" + System.DateTime.Now.ToString("HH:mm") + "','Payment Success')";
            SqlCommand cmd2 = new SqlCommand(sql2, con);
            con.Open();
            cmd2.ExecuteNonQuery();
            con.Close();
        }

        string retData = RSA(T2.Text, T3.Text, T4.Text);
        retData = retData.TrimEnd('`');
        string[] retArr = retData.Split('`');
        string sql1 = "insert into payment values('" + T1.Text + "','" + retArr[0] + "','" + retArr[1] + "','" + retArr[2] + "')";
        SqlCommand cmd = new SqlCommand(sql1, con);
        con.Open();
        cmd.ExecuteNonQuery();
        con.Close();



        string del = "delete from cart where uid='" + Session["Uid"].ToString() + "'";
        cmd = new SqlCommand(del, con);
        con.Open();
        cmd.ExecuteNonQuery();
        con.Close();
        Session["Oid"] = oid;
        Response.Redirect("OrderPlaced.aspx");
    }

    public string RSA(string value1, string value2, string value3)
    {
        string[] valarr = { value1, value2, value3 };
        string ret = "";
        var cypherText = "";
        for (int i = 0; i < 3; i++)
        {
            //lets take a new CSP with a new 2048 bit rsa key pair
            var csp = new RSACryptoServiceProvider(2048);

            //how to get the private key
            var privKey = csp.ExportParameters(true);

            //and the public key ...
            var pubKey = csp.ExportParameters(false);

            //converting the public key into a string representation
            string pubKeyString;
            {
                //we need some buffer
                var sw = new System.IO.StringWriter();
                //we need a serializer
                var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                //serialize the key into the stream
                xs.Serialize(sw, pubKey);
                //get the string from the stream
                pubKeyString = sw.ToString();
            }

            //converting it back
            {
                //get a stream from the string
                var sr = new System.IO.StringReader(pubKeyString);
                //we need a deserializer
                var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                //get the object back from the stream
                pubKey = (RSAParameters)xs.Deserialize(sr);
            }

            //conversion for the private key is no black magic either ... omitted

            //we have a public key ... let's get a new csp and load that key
            csp = new RSACryptoServiceProvider();
            csp.ImportParameters(pubKey);

            //we need some data to encrypt
            var plainTextData = valarr[i];

            //for encryption, always handle bytes...
            var bytesPlainTextData = System.Text.Encoding.Unicode.GetBytes(plainTextData);

            //apply pkcs#1.5 padding and encrypt our data 
            var bytesCypherText = csp.Encrypt(bytesPlainTextData, false);

            //we might want a string representation of our cypher text... base64 will do
            cypherText = Convert.ToBase64String(bytesCypherText);

            ret += cypherText.ToString() + "`";
        }


        ///*
        // * some transmission / storage / retrieval
        // * 
        // * and we want to decrypt our cypherText
        // */

        ////first, get our bytes back from the base64 string ...
        //bytesCypherText = Convert.FromBase64String(cypherText);

        ////we want to decrypt, therefore we need a csp and load our private key
        //csp = new RSACryptoServiceProvider();
        //csp.ImportParameters(privKey);

        ////decrypt and strip pkcs#1.5 padding
        //bytesPlainTextData = csp.Decrypt(bytesCypherText, false);

        ////get our original plainText back...
        //plainTextData = System.Text.Encoding.Unicode.GetString(bytesPlainTextData);
        
        return ret;
    }
}