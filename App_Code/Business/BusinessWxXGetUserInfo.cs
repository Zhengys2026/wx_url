using Commercial.Sirius.Business.WeiXin;
using Commercial.Sirius.WeiXin;
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
    public class BusinessWxXGetUserInfo
    {

        //全局执行对象
        public WxConfig _config;
        //日志对象
        SystemLog sLog = new SystemLog();

        /// <summary>
        ///  方法主入口  
        /// </summary>
        /// <returns></returns>
        public ReplyMessage BeforeInit(string jsonMessage)
        {
            //实例化JavaScriptSerializer类的新实例
            JavaScriptSerializer JavaScriptJson = new JavaScriptSerializer();
            Dictionary<string, string> obj;
            //数据返回对象
            ReplyMessage reply = new ReplyMessage();
            //数据返回内部对象
            //返回字符串
            string result = null;
            try
            {
                //1 -数据字典对象化处理(参数较少时使用)
                //-----------------------------------------------------------------------------------
                obj = JavaScriptJson.Deserialize<Dictionary<string, string>>(jsonMessage);
                BusinessWXToken _wxToken = new BusinessWXToken(obj["app_id"],"");
                string url = "https://api.weixin.qq.com/cgi-bin/user/info?access_token=";
                string access_token = _wxToken.access_token;
                url = url + access_token + "&openid=" + obj["open_id"] + "&lang=" + LangType.zh_CN.ToString();
                var json = Util.HttpGet2(url);
                if (json.IndexOf("errcode") > 0)
                {
                    var ui = new UserInfo();
                    ui.error = Util.JsonTo<ReturnCode>(json);
                    result = JavaScriptJson.Serialize(ui);
                }
                else
                {
                   result = JavaScriptJson.Serialize(Util.JsonTo<UserInfo>(json));
                }
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

        /// <summary>
        /// 用户基本信息
        /// </summary>
        public class UserInfo
        {
            /// <summary>
            /// 用户是否订阅该公众号标识，值为0时，代表此用户没有关注该公众号，拉取不到其余信息。
            /// </summary>
            public byte subscribe { get; set; }
            /// <summary>
            /// 用户的标识，对当前公众号唯一
            /// </summary>
            public string openid { get; set; }
            /// <summary>
            /// 用户的昵称
            /// </summary>
            public string nickname { get; set; }
            /// <summary>
            /// 用户的性别，值为1时是男性，值为2时是女性，值为0时是未知
            /// </summary>
            public byte sex { get; set; }
            /// <summary>
            /// 用户的语言，简体中文为zh_CN
            /// </summary>
            public LangType language { get; set; }
            /// <summary>
            /// 用户所在城市
            /// </summary>
            public string city { get; set; }
            /// <summary>
            /// 用户所在省份
            /// </summary>
            public string province { get; set; }
            /// <summary>
            /// 用户所在国家
            /// </summary>
            public string country { get; set; }
            /// <summary>
            /// 用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像），用户没有头像时该项为空
            /// </summary>
            public string headimgurl { get; set; }
            /// <summary>
            /// 用户关注时间，为时间戳。如果用户曾多次关注，则取最后关注时间
            /// </summary>
            public long subscribe_time { get; set; }

            public ReturnCode error { get; set; }
        }
        public enum LangType
        {
            /// <summary>
            /// 中文简体
            /// </summary>
            zh_CN,

            /// <summary>
            /// 中文繁体
            /// </summary>
            zh_TW,

            /// <summary>
            /// 英文
            /// </summary>
            en
        }
        /// <summary>
        /// 全局返回码请看
        /// http://mp.weixin.qq.com/wiki/index.php?title=%E5%85%A8%E5%B1%80%E8%BF%94%E5%9B%9E%E7%A0%81%E8%AF%B4%E6%98%8E
        /// </summary>
        [Serializable]
        public class ReturnCode
        {
            public int errcode { get; set; }
            public string errmsg { get; set; }
            public override string ToString()
            {
                return "{ \"errcode\":" + errcode + ",\"errmsg\":\"" + errmsg + "\"}";
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

            /// <summary>
            /// 公众帐号access_token
            /// </summary>
            public string ACCESSTOKEN { get; set; }

            public string TOUSERID { get; set; }

        }

       
    }
}