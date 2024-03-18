<%@ WebHandler Language="C#" Class="urlXCXGetOpenId" %>

using System;
using System.Web;
using System.Web.Script.Serialization;
using Log;
using Reply.Entity;

public class urlXCXGetOpenId : IHttpHandler {

    HttpContext context;
    JavaScriptSerializer JavaScriptJson = new JavaScriptSerializer();
    public void ProcessRequest (HttpContext context)
    {
        this.context = context;
        //日志对象
        SystemLog sLog = new SystemLog();
        //返回对象
        ReplyMessage reply = new ReplyMessage();
        //数据接收对象
        string result = null;
        Business.BusinessWxXCXOpenId _signs = new Business.BusinessWxXCXOpenId();
        try
        {
            //发送 回复 消息
            System.IO.Stream s = context.Request.InputStream;
            byte[] b = new byte[s.Length];
            s.Read(b, 0, (int)s.Length);
            string jsonMessage = System.Text.Encoding.UTF8.GetString(b);
           //jsonMessage = "{ \"app_id\":\"wx09c952bc1398bf17\",\"code\":\"001OSlDq1a72Xi0yL6Fq1kHoDq1OSlDF\"}   ";
            if (jsonMessage != null && jsonMessage != "")
            {
                sLog.Normal(string.Format("urlXCXGetOpenId 请求: \r\n {0} ", jsonMessage));
                //主方法实例化 返回
                jsonMessage = jsonMessage.Replace("'", "’");
                reply =_signs.BeforeInit(jsonMessage);
                //json格式化
                result = JavaScriptJson.Serialize(reply);
                //数据流入 流出 写入日志
                sLog.Normal(string.Format(" urlXCXGetOpenId 获取数据 请求: \r\n\r\n {0} \r\n\r\n api返回数据 请求: \r\n\r\n {1} ", jsonMessage, result));
            }
        }
        catch (Exception ex)
        {
            reply.RequestStatus = "error";
            reply.RequestStatusCode = "0";
            reply.error = ex.ToString();
            reply.errorCode = 1001;
            reply.errorMessage = "Service try catch 区域 发生Exception 异常,请联系管理员";
            result = JavaScriptJson.Serialize(reply);
            sLog.Errors(result, ex);
        }
        context.Response.Write(result);
        context.Response.End();
    }

    public bool IsReusable {
        get {
            return false;
        }
    }
}