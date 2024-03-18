using System;
using System.Text;
using System.Data;
using Loogn;
using Log;
using Newtonsoft.Json;

namespace WinService
{

    /// <summary>
    /// 帐户信息实体类
    /// </summary>
    /// <author>zys</author>
    /// <date>2015-03-25</date>
    public sealed class BusinessWXToken
    {
        public string app_id { get; set; }
        //令牌
        public string access_token { get; set; }

        public string app_secret { get; set; }
        //前端令牌
        public string jsapi_ticket { get; set; }
        //日志对象
        SystemLog sLog = new SystemLog();
        public BusinessWXToken(string app_id,string agent_id)
        {
            string url = "https://wx.chinawmt.com/third_party_api/api/offiaccount/getWXToken.ashx";
            string postDataStr = "{\"app_id\":\"" + app_id + "\",\"agent_id\":\"" + agent_id + "\"}";
            string json_info = Util.HttpPost2(url, postDataStr);
            dynamic obj_data = Util.JsonTo<dynamic>(json_info);
            if (obj_data["errorCode"] == 0)
            {
                if (obj_data["ReplyContent"] != null)
                {
                    WxOAuth objContent = JsonConvert.DeserializeObject<WxOAuth>(obj_data["ReplyContent"].ToString());
                    this.access_token = objContent.access_token;
                    this.app_secret = objContent.app_secret;
                    this.app_id = app_id;
                    this.jsapi_ticket = objContent.jsapi_ticket;
                }
            }
        }

        public class WxOAuth
        {
            /// <summary>
            /// 
            /// </summary>
            public string app_id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string access_token { get; set; }

            public string app_secret { get; set; }
            
            /// <summary>
            /// 
            /// </summary>
            public string jsapi_ticket { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int msgcode { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string message { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string data { get; set; }
        }
       

    }

}
