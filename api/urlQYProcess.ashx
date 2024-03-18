<%@ WebHandler Language="C#" Class="urlQYProcess" %>
using System.Web;

public class urlQYProcess : IHttpHandler
{
    HttpContext context;
    public string reurl = "";

    public void ProcessRequest(HttpContext context)
    {
        this.context = context;
        Business.BusinessQYWxUrl _wxUrl = new Business.BusinessQYWxUrl();
        _wxUrl.BeforeInit(context, "URL");
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}