using Commercial.Sirius.Business.WeiXin;
using Log;
using Reply.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using WinService;

namespace Business
{

    /// <summary>
    /// 微信 url 授权
    /// </summary>
    public class BusinessWxUrlSigns
    {

        //日志对象
        SystemLog sLog = new SystemLog();
        /*==============================================================*/
        /*  方法主入口  --  页面参数执行方法                      */
        /*==============================================================*/
        #region ------------------方法主入口  
        /// <summary>
        ///  方法主入口  
        /// </summary>
        /// <returns></returns>
        public ReplyMessage BeforeInit(string jsonMessage)
        {
            //实例化JavaScriptSerializer类的新实例
            JavaScriptSerializer JavaScriptJson = new JavaScriptSerializer();
            System.Collections.Generic.Dictionary<string, string> obj;
            //数据接收对象
            DataSet ds = new DataSet();
            //数据返回对象
            ReplyMessage reply = new ReplyMessage();
            //数据返回内部对象
            Signs signs = new Signs();
            //返回字符串
            string result = null;
            try
            {

                //1 -数据字典对象化处理(参数较少时使用)
                //-----------------------------------------------------------------------------------
                obj = JavaScriptJson.Deserialize<System.Collections.Generic.Dictionary<string, string>>(jsonMessage);
                _return_data _data = this.ParamValidation(obj);

                //判断是否验证通过
                if (_data.msgcode > 0)
                {
                    reply.RequestStatus = "error";
                    reply.ReplyMode = Reply.Entity.Enums.ReplyModeEnum.SYNCH;
                    reply.errorCode = _data.msgcode;
                    reply.ReplyContent = _data.message;
                    reply.ReplyAlertMessage = "数据处理完成";
                    reply.error = "";
                    return reply;
                }

                string agentid = "";
                if (obj.ContainsKey("agent_id"))
                {
                    agentid = obj["agent_id"];
                }
                //获取配置信息
                BusinessWXToken _wxToken = new BusinessWXToken(obj["app_id"], agentid);
                signs.appId = _wxToken.app_id;
                signs.nonceStr = GuidTo16String();
                signs.jsapiTicket = _wxToken.jsapi_ticket;
                signs.timestamp = ConvertDateTimeInt(DateTime.Now);
                signs.url = obj["url"].ToString();
                //组装签名字符串
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendFormat("jsapi_ticket={0}", _wxToken.jsapi_ticket);
                sb.AppendFormat("&noncestr={0}", signs.nonceStr);
                sb.AppendFormat("&timestamp={0}", signs.timestamp);
                sb.AppendFormat("&url={0}", signs.url);
                signs.signature = GetSha1(sb.ToString()).ToLower();
                result = JavaScriptJson.Serialize(signs);
                //返回数据
                reply.RequestStatus = "success";
                reply.RequestStatusCode = "1";
                reply.ReplyMode = Reply.Entity.Enums.ReplyModeEnum.SYNCH;
                reply.ReplyAlertMessage = "数据处理完成";
                reply.ReplyContent = result;
                reply.error = "";
                reply.errorCode = 0;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return reply;
        }

        /*==============================================================*/
        /*  验证方法  --  页面参数验证集合                      */
        /*==============================================================*/
        public _return_data ParamValidation(System.Collections.Generic.Dictionary<string, string> obj)
        {
            //调用方法返回对象
            _return_data _return = new _return_data();
            try
            {
                if (!obj.ContainsKey("app_id"))
                {
                    _return.msgcode = 1001;
                    _return.message = "微信账号 [app_id] 不可为空！";
                    return _return;
                }
                if (!obj.ContainsKey("url"))
                {
                    _return.msgcode = 1002;
                    _return.message = "微信授权 [url] 不可为空！";
                    return _return;
                }
                _return.msgcode = 0;
                _return.message = "各项参数验证成功";
            }
            catch (Exception ex)
            {
            }
            return _return;
        }
        #endregion


        public static string GetSha1(string str)
        {
            //建立SHA1对象
            System.Security.Cryptography.SHA1 sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            //将mystr转换成byte[] 
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            byte[] dataToHash = enc.GetBytes(str);
            //Hash运算
            byte[] dataHashed = sha.ComputeHash(dataToHash);
            //将运算结果转换成string
            string hash = BitConverter.ToString(dataHashed).Replace("-", "");
            return hash;
        }

        /// <summary>  
        /// 根据GUID获取16位的唯一字符串  
        /// </summary>  
        /// <param name=\"guid\"></param>  
        /// <returns></returns>  
        public static string GuidTo16String()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
                i *= ((int)b + 1);
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }

        #region ------------------datetime转换
        /// <summary>
        /// datetime转换成unixtime
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private static int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }

        private static DateTime UnixTimeToTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
        #endregion

        /// <summary>
        /// 返回数据对象
        /// </summary>
        public class _return_data
        {
            /// <summary>
            /// 编码
            /// </summary>
            public int msgcode { get; set; }

            /// <summary>
            /// 消息
            /// </summary>
            public string message { get; set; }
        }

        /// <summary>
        /// 取出的配置数据对象
        /// </summary>
        public class WxConfig
        {
            /// <summary>
            /// 微信用户id
            /// </summary>
            public string APPID { get; set; }

            /// <summary>
            /// 公众帐号secert
            /// </summary>
            public string APPSECRET { get; set; }

            /// <summary>
            /// js 令牌信息
            /// </summary>
            public string JsTicket { get; set; }
            
        }


        /// <summary>
        /// 返回的业务类实体
        /// </summary>
        public class Signs
        {
            /// <summary>
            /// 公众号的唯一标识
            /// </summary>
            public string appId { get; set; }
            /// <summary>
            /// url
            /// </summary>
            public string url { get; set; }
            /// <summary>
            /// JsapiTicket
            /// </summary>
            public string jsapiTicket { get; set; }
            /// <summary>
            /// 生成签名的时间戳
            /// </summary>
            public int timestamp { get; set; }
            /// <summary>
            /// 生成签名的随机串
            /// </summary>
            public string nonceStr { get; set; }
            /// <summary>
            /// 签名
            /// </summary>
            public string signature { get; set; }
        }


    }
}