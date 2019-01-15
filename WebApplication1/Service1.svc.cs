using System;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace WebApplication1
{

    public class Service1 : IService1
    {
        public string AddMunicipality()
        {
            string body = "";
            string[] MunicipalityArray;

            if (OperationContext.Current.RequestContext.RequestMessage.IsEmpty)
                return "Err body";
            body = Encoding.UTF8.GetString(OperationContext.Current.RequestContext.RequestMessage.GetBody<byte[]>());

            if (body.Length < 4)
                return "Err body";

            MunicipalityArray = body.Split(new[] { "\r\n"}, StringSplitOptions.None);

            try
            {
                SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connectas"].ConnectionString);
                conn.Open();

                for (int i = 0; i < MunicipalityArray.Count(); i++)
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO [Municipality] values ('" + MunicipalityArray[i] + "')", conn))
                    {
                        if (cmd.ExecuteNonQuery() < 0)
                            return "Insert failed!";
                    }
                }

            }
            catch (SqlException ex)
            {
                return ex.ToString();
            }

            return MunicipalityArray.Count().ToString() + " record(s) inserted";
        }
        public string AddSchedule(string Municipality, string TaxType, string Date)
        {
            string MunID,TaxID;
            DateTime DateFrom, DateTo;

            if (!DateTime.TryParse(Date, out DateFrom))
            {
                return "Date value invalid";
            }

            switch (TaxType.ToLower())
            {
                case "yearly":
                    DateTo = DateFrom.AddYears(1);
                    break;
                case "monthly":
                    DateTo = DateFrom.AddMonths(1);
                    break;
                case "weekly":
                    DateTo = DateFrom.AddDays(7);
                    break;
                case "daily":
                    DateTo = DateFrom;
                    break;
                default:
                    return "Tax Type parameter invalid";
            }

          
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connectas"].ConnectionString);
            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                SqlDataAdapter konektas = new SqlDataAdapter("Select ID from Municipality where [MunicipalityName] = '" + Municipality + "'", conn);
                konektas.Fill(ds);                    
                konektas.Dispose();
                if (ds.Tables[0].Rows.Count == 0) return "Municipality not defined";
                MunID = ds.Tables[0].Rows[0]["ID"].ToString();

                ds = new DataSet();
                konektas = new SqlDataAdapter("Select ID from TaxesConfig where [TaxTypeString] = '" + TaxType + "'", conn);
                konektas.Fill(ds);
                konektas.Dispose();
                if (ds.Tables[0].Rows.Count == 0) return "TaxesConfig not defined";
                TaxID = ds.Tables[0].Rows[0]["ID"].ToString();

                SqlCommand cmd = new SqlCommand("INSERT INTO [Taxes] values (" + MunID + "," + TaxID + ",'" + DateFrom.ToString("yyyy-MM-dd") + "','" + DateTo.ToString("yyyy-MM-dd") + "')", conn);
                cmd.ExecuteNonQuery();

                conn.Close();

                return "Schedule Added";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string GetTax(string Municipality, string Date)
        {
            DateTime dtDate;
            double taxRate = 0.0;

            if (!DateTime.TryParse(Date, out dtDate))
            {
                return "Date value invalid";
            }

            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Connectas"].ConnectionString);
            try
            {
                conn.Open();
                DataSet ds = new DataSet();

                StringBuilder SQ = new StringBuilder("select top 1 [TaxValue] from TaxesConfig tc inner join Taxes t on tc.Id = t.TaxID ");
                SQ.Append("inner join Municipality m on m.id = t.MunID ");
                SQ.Append("where m.MunicipalityName = '" + Municipality + "' and t.DateFrom <= '" + dtDate.ToString("yyyy-MM-dd") + "' and t.DateTo>='" + dtDate.ToString("yyyy-MM-dd") + "' order by TaxTypeNo desc");
                SqlDataAdapter konektas = new SqlDataAdapter(SQ.ToString(), conn);

                konektas.Fill(ds);
                konektas.Dispose();
                if (ds.Tables[0].Rows.Count == 0) return "No taxes found!";
                return ds.Tables[0].Rows[0]["TaxValue"].ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
          
            //return taxRate.ToString("N2");
        }
    }
}
