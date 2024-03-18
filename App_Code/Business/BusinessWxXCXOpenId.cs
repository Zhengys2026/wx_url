using Commercial.Sirius.Business.WeiXin;
using Log;
using Loogn;
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
    /// 微信小程序获取 openid
    /// </summary>
    public class BusinessWxXCXOpenId
    {
        //全局执行对象
        public WxConfig _config;

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
                BusinessWXToken _wxToken = new BusinessWXToken(obj["app_id"],"");
                string url = "https://api.weixin.qq.com/sns/jscode2session";
                sLog.Normal(string.Format("urlXCXGetOpenIdstc 请求: \r\n {0} ", url + "?appid=" + _wxToken.app_id + "&secret=" + _wxToken.app_secret + "&js_code=" + obj["code"].ToString() + "&secret=authorization_code"));
                string Str = Util.HttpGet2(url+ "?appid=" + _wxToken.app_id + "&secret=" + _wxToken.app_secret + "&js_code=" + obj["code"].ToString() + "&secret=authorization_code");
                sLog.Normal(string.Format("urlXCXGetOpenIdstcStr 请求: \r\n {0} \r\n {1} ", url + "?appid=" + _wxToken.app_id + "&secret=" + _wxToken.app_secret + "&js_code=" + obj["code"].ToString() + "&secret=authorization_code", Str));

                OpenOBJ _open_obj = JsonHelper.ParseFromJson<OpenOBJ>(Str);
                result = JavaScriptJson.Serialize(_open_obj);

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
                if (!obj.ContainsKey("code"))
                {
                    _return.msgcode = 1002;
                    _return.message = "微信换取码 [code] 不可为空！";
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
        /// 将Json格式数据转化成对象
        /// </summary>
        public class JsonHelper
        {
            /// <summary>  
            /// 生成Json格式  
            /// </summary>  
            /// <typeparam name="T"></typeparam>  
            /// <param name="obj"></param>  
            /// <returns></returns>  
            public static string GetJson<T>(T obj)
            {
                DataContractJsonSerializer json = new DataContractJsonSerializer(obj.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    json.WriteObject(stream, obj);
                    string szJson = System.Text.Encoding.UTF8.GetString(stream.ToArray()); return szJson;
                }
            }
            /// <summary>  
            /// 获取Json的Model  
            /// </summary>  
            /// <typeparam name="T"></typeparam>  
            /// <param name="szJson"></param>  
            /// <returns></returns>  
            public static T ParseFromJson<T>(string szJson)
            {
                T obj = Activator.CreateInstance<T>();
                using (MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(szJson)))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                    return (T)serializer.ReadObject(ms);
                }
            }
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


        }


        /// <summary>
        /// 返回的业务类实体
        /// </summary>
        public class OpenOBJ
        {
            /// <summary>
            /// 公众号的唯一标识
            /// </summary>
            public int errcode { get; set; }
            /// <summary>
            /// url
            /// </summary>
            public string hints { get; set; }
            /// <summary>
            /// JsapiTicket
            /// </summary>
            public string openid { get; set; }

            public string unionid { get; set; }

            /// <summary>
            /// JsapiTicket
            /// </summary>
            public string session_key { get; set; }

        }


    }
}