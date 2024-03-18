<%@ WebHandler Language="C#" Class="urlProcessAuthorize" %>

using System.Web;

public class urlProcessAuthorize : IHttpHandler
{
    HttpContext context;
    public void ProcessRequest(HttpContext context)
    {
       this.context = context;
        Business.BusinessWxUrl _wxUrl = new Business.BusinessWxUrl();
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