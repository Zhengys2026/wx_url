<%@ WebHandler Language="C#" Class="urlProcess" %>
using System.Web;

public class urlProcess : IHttpHandler
{
    HttpContext context;
    public string reurl = "";

    public void ProcessRequest(HttpContext context)
    {
        this.context = context;
        Business.BusinessWxUrl _wxUrl = new Business.BusinessWxUrl();
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