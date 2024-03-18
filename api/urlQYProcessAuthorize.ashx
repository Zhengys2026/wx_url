<%@ WebHandler Language="C#" Class="urlQYProcessAuthorize" %>

using System.Web;

public class urlQYProcessAuthorize : IHttpHandler
{
    HttpContext context;
    public void ProcessRequest(HttpContext context)
    {
       this.context = context;
        Business.BusinessQYWxUrl _wxUrl = new Business.BusinessQYWxUrl();
        _wxUrl.BeforeInit(context, "Authorize");
    }
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}