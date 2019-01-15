using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web.UI;

namespace WebApplication1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "AddMunicipality")]
        string AddMunicipality();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "AddSchedule/{Municipality}/{TaxType}/{Date}")]
        string AddSchedule(string Municipality, string TaxType,string Date);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "GetTax/{Municipality}/{Date}")]
        string GetTax(string Municipality, string Date);

    }
}
