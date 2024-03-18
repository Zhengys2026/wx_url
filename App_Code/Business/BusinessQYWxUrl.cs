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
    /// 微信 url 跳转服务
    /// </summary>
    public class BusinessQYWxUrl
    {
        //全局执行对象
        public WxConfig _config;

        //日志对象
        SystemLog sLog = new SystemLog();

        /*==============================================================*/
        /*  方法主入口  --  页面参数执行方法                      */
        /*==============================================================*/
        /// <summary>
        ///  方法主入口  
        /// </summary>
        /// <returns></returns>
        public void BeforeInit(HttpContext context, string urlType)
        {
            try
            {

                //取出页面要跳转到的URL地址
                string reurls = "", reurl = "", url_param = "", appid = "", type = "", agentid="", scope = "";
                if (context.Request.QueryString["reurl"] != null && context.Request.QueryString["reurl"] != "")
                {
                    #region ------------------取出页面要跳转到的URL  appid type

                    reurls = context.Request.QueryString["reurl"].ToString();
                    //分解参数 
                    string[] reurlArr = reurls.Split(',');
                    for (int i = 0; i < reurlArr.Length; i++)
                    {
                        if (i == 0)
                        {
                            //首对象 取URL
                            reurl = reurlArr[0];
                            continue;
                        }
                        //取对应的param
                        if (reurlArr[i].IndexOf("param=") >= 0)
                        {
                            url_param = "&"+reurlArr[i].Replace("param=", "").Replace('*','&');
                            continue;
                        }
                        //取对应的appid
                        if (reurlArr[i].IndexOf("appid=") >= 0)
                        {
                            appid = reurlArr[i].Replace("appid=", "");
                            continue;
                        }
                        //取对应的type
                        if (reurlArr[i].IndexOf("type=") >= 0)
                        {
                            type = reurlArr[i].Replace("type=", "");
                            continue;
                        }
                        if (reurlArr[i].IndexOf("agentid=") >= 0)
                        {
                            agentid = reurlArr[i].Replace("agentid=", "");
                            continue;
                        }
                        if (reurlArr[i].IndexOf("scope=") >= 0)
                        {
                            agentid = reurlArr[i].Replace("scope=", "");
                            continue;
                        }

                    }
                    if (appid != "")
                    {
                        BusinessWXToken _wxToken = new BusinessWXToken(appid, agentid);
                        _config = new WxConfig();
                        _config.APPSECRET = _wxToken.app_secret;
                        _config.APPID = appid;
                        _config.AGENTID = agentid;
                    }
                    #endregion
                }
                sLog.Normal(string.Format("QYURL 请求: {0} \r\n {1} ", urlType, reurls));
                var uri = new Uri(reurls);
                var QYURLHost = uri.Scheme+"//"+uri.Host+"";
                if (urlType == "URL")
                {
                    #region ------------------urlProcess 逻辑业务处理
                    //拼接参数
                    string param = null;
                    //获取微信回传的code
                    string code = "";
                    sLog.Normal(string.Format("QYURLcode 请求: {0} \r\n {1} code\r\n {2} ", urlType, reurls, code));
                    if (context.Request.QueryString["code"] != null && context.Request.QueryString["code"] != "")
                    {
                        //获取微信回传的code
                        code = context.Request.QueryString["code"].ToString();
                        sLog.Normal(string.Format("QYURLcode 请求: {0} \r\n {1} code\r\n {2} ", urlType, reurls, code));
                        OAuth_Token Model = Get_token(code);
                        //获取 用户信息
                        OAuthUser OAuthUser_Model = Get_UserInfo(Model.access_token, code);
                        if(OAuthUser_Model.user_ticket != null && OAuthUser_Model.user_ticket != "")
                        {
                            //----换取用户信息----
                            OAuthUserDetail OAuthUser_Detail = Get_UserInfoDetail(Model.access_token, OAuthUser_Model.user_ticket);
                            Encoding utf8 = Encoding.UTF8;
                            string DeviceId = "";
                            if (!string.IsNullOrWhiteSpace(OAuthUser_Model.DeviceId))
                            {
                                DeviceId = OAuthUser_Model.DeviceId;
                            }else
                            {
                                DeviceId = OAuthUser_Model.openid;
                            }
                            if (string.IsNullOrWhiteSpace(DeviceId))
                            {
                                DeviceId = OAuthUser_Detail.userid;
                            }
                            switch (type)
                            {
                                case "1":
                                    {
                                        param = "&mobile=" + OAuthUser_Detail.mobile;
                                        break;
                                    }
                                case "2":
                                    {
                                        param = "&mobile=" + OAuthUser_Detail.mobile;
                                        param += "&deviceid=" + DeviceId;
                                        break;
                                    }
                                case "3":
                                    {
                                        param = "&mobile=" + OAuthUser_Detail.mobile;
                                        param += "&deviceid=" + DeviceId;
                                        if (!string.IsNullOrWhiteSpace(OAuthUser_Detail.name))
                                        {
                                            param += "&name=" + HttpUtility.UrlEncode(OAuthUser_Detail.name, utf8).ToUpper();
                                        }
                                        else
                                        {
                                            param += "&name=";
                                        }
                                        break;
                                    }
                                case "4":
                                    {
                                        sLog.Normal(string.Format("QYURLcode 请求: 换取用户信息41\r\n " + OAuthUser_Model.DeviceId));
                                        param = "&mobile=" + OAuthUser_Detail.mobile;
                                        param += "&deviceid=" + DeviceId;
                                        sLog.Normal(string.Format("QYURLcode 请求: 换取用户信息41\r\n " +OAuthUser_Detail.name));
                                        if(!string.IsNullOrWhiteSpace(OAuthUser_Detail.name))
                                        {
                                            param += "&name=" + HttpUtility.UrlEncode(OAuthUser_Detail.name, utf8).ToUpper();
                                        }else
                                        {
                                            param += "&name=";
                                        }
                                  
                                        param += "&userid=" + OAuthUser_Detail.userid;
                                        sLog.Normal(string.Format("QYURLcode 请求: 换取用户信息42\r\n "));
                                        break;
                                    }
                                case "5":
                                    {
                                        param = "&mobile=" + OAuthUser_Detail.mobile;
                                        param += "&deviceid=" + DeviceId;
                                        if (!string.IsNullOrWhiteSpace(OAuthUser_Detail.name))
                                        {
                                            param += "&name=" + HttpUtility.UrlEncode(OAuthUser_Detail.name, utf8).ToUpper();
                                        }
                                        else
                                        {
                                            param += "&name=";
                                        }
                                        param += "&userid=" + OAuthUser_Detail.userid;
                                        param += "&gender=" + OAuthUser_Detail.gender;
                                        break;
                                    }
                                case "6":
                                    {
                                        param = "&mobile=" + OAuthUser_Detail.mobile;
                                        param += "&deviceid=" + DeviceId;
                                        if (!string.IsNullOrWhiteSpace(OAuthUser_Detail.name))
                                        {
                                            param += "&name=" + HttpUtility.UrlEncode(OAuthUser_Detail.name, utf8).ToUpper();
                                        }
                                        else
                                        {
                                            param += "&name=";
                                        }
                                        param += "&userid=" + OAuthUser_Detail.userid;
                                        param += "&gender=" + OAuthUser_Detail.gender;
                                        param += "&email=" + OAuthUser_Detail.email;
                                        break;
                                    }
                                case "7":
                                    {
                                        param = "&mobile=" + OAuthUser_Detail.mobile;
                                        param += "&deviceid=" + DeviceId;
                                        if (!string.IsNullOrWhiteSpace(OAuthUser_Detail.name))
                                        {
                                            param += "&name=" + HttpUtility.UrlEncode(OAuthUser_Detail.name, utf8).ToUpper();
                                        }
                                        else
                                        {
                                            param += "&name=";
                                        }
                                        param += "&userid=" + OAuthUser_Detail.userid;
                                        param += "&gender=" + OAuthUser_Detail.gender;
                                        param += "&email=" + OAuthUser_Detail.email;
                                        param += "&avatar=" + OAuthUser_Detail.avatar;
                                        break;
                                    }
                                case "8":
                                    {
                                        param = "&mobile=" + OAuthUser_Detail.mobile;
                                        param += "&deviceid=" + DeviceId;
                                        if (!string.IsNullOrWhiteSpace(OAuthUser_Detail.name))
                                        {
                                            param += "&name=" + HttpUtility.UrlEncode(OAuthUser_Detail.name, utf8).ToUpper();
                                        }
                                        else
                                        {
                                            param += "&name=";
                                        }
                                        param += "&userid=" + OAuthUser_Detail.userid;
                                        param += "&gender=" + OAuthUser_Detail.gender;
                                        param += "&email=" + OAuthUser_Detail.email;
                                        param += "&avatar=" + OAuthUser_Detail.avatar;
                                        param += "&qr_code=" + OAuthUser_Detail.avatar;
                                        break;
                                    }
                            }
                            if (!string.IsNullOrWhiteSpace(OAuthUser_Model.external_userid))
                            {
                                param += "&euserid=" + OAuthUser_Model.external_userid;
                            }
                            if (!string.IsNullOrWhiteSpace(OAuthUser_Model.openid))
                            {
                                param += "&openid=" + OAuthUser_Model.openid;
                            }
                            //reurl 是否本身携带参数
                            if (reurl.IndexOf('?') > 0)
                            {
                                sLog.Normal(string.Format("QYURL 返回: {0} \r\n 接收： {1} ", reurl + "&date=" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + param, reurls));
                                context.Response.Redirect(reurl + "&date=" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + param, false);
                            }
                            else
                            {
                                sLog.Normal(string.Format("QYURL 返回: {0} \r\n 接收： {1} ", reurl + "&date=" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + url_param, reurls));
                                context.Response.Redirect(reurl + "?date=" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + url_param + param, false);
                            }
                        }
                        else
                        {
                            sLog.Normal(string.Format("QYURL 返回: {0} \r\n 接收： {1} ", "urlQYProcessAuthorize.ashx ? auth = 1 & reurl = " + reurls, reurls));
                            //未获得openid，访问弹出微信授权页面，提示用户授权
                            context.Response.Redirect("urlQYProcessAuthorize.ashx?auth=1&reurl=" + reurls, false);
                        }
                    }
                    #endregion
                }
                else if (urlType == "Authorize")
                {
                    #region ------------------UrlRedirectAuthorize 授权业务逻辑处理
                    //1 走全部 snsapi_privateinfo：手动授权，可获取成员的详细信息，包含手机、邮箱
                    if (context.Request.QueryString["auth"] != null && context.Request.QueryString["auth"] != "" && context.Request.QueryString["auth"] == "1")
                    {
                        string url = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + _config.APPID + "&redirect_uri" + QYURLHost + "/wxUrl/api/urlQYProcess.ashx?reurl=" + context.Request.QueryString["reurl"].ToString() + "&response_type=code&scope=snsapi_privateinfo&agentid=" + _config.AGENTID + "&state=1#wechat_redirect";
                        sLog.Normal(string.Format("QYURL 返回: {0} \r\n 接收： {1} ",url, reurls));
                        context.Response.Redirect(url);
                    }
                    // 2 snsapi_userinfo：静默授权，不弹出授权页面 ,可获取成员的详细信息，但不包含手机、邮箱；
                    else if (context.Request.QueryString["auth"] != null && context.Request.QueryString["auth"] != "" && context.Request.QueryString["auth"] == "2")
                    {
                        string url = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + _config.APPID + "&redirect_uri=" + QYURLHost + "/wxUrl/api/urlQYProcess.ashx?reurl=" + context.Request.QueryString["reurl"].ToString() + "&response_type=code&scope=snsapi_userinfo&agentid=" + _config.AGENTID + "&state=1#wechat_redirect";
                        sLog.Normal(string.Format("QYURL 返回: {0} \r\n 接收： {1} ", url, reurls));
                        context.Response.Redirect(url);
                    }
                    //snsapi_base：静默授权，不弹出授权页面 ,可获取成员的的基础信息（UserId与DeviceId）；
                    else
                    {
                        //不弹出授权页面
                        sLog.Normal(string.Format("QYURL 返回: {0} \r\n 接收： {1} ", "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + _config.APPID + "&redirect_uri="+ QYURLHost + "/wxUrl/api/urlQYProcess.ashx?reurl=" + context.Request.QueryString["reurl"].ToString() + "&response_type=code&scope=snsapi_base&agentid=" + _config.AGENTID + "&&state=1#wechat_redirect", reurls));
                        context.Response.Redirect("https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + _config.APPID + "&redirect_uri=" + QYURLHost + "/wxUrl/api/urlQYProcess.ashx?reurl=" + context.Request.QueryString["reurl"].ToString() + "&response_type=code&scope=snsapi_base&agentid=" + _config.AGENTID + "&&state=1#wechat_redirect");
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message);
                sLog.Normal(string.Format("QYURLException 请求: {0} \r\n ", ex.Message));
            }
        }

        //根据appid，secret，code获取微信用户成员信息信息
        protected OAuth_Token Get_token(string Code)
        {
            //获取微信回传的openid、access token
            string Str = GetJson("https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid=" + _config.APPID + "&corpsecret=" + _config.APPSECRET);

            sLog.Normal(string.Format("QYURLAStr 请求: {0}\r\n 返回{1} ", "https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid=" + _config.APPID + "&corpsecret=" + _config.APPSECRET, Str));
            //微信回传的数据为Json格式，将Json格式转化成对象
            OAuth_Token Oauth_Token_Model = JsonHelper.ParseFromJson<OAuth_Token>(Str);
            return Oauth_Token_Model;
        }


        //根据openid，access token获得用户信息
        protected OAuthUser Get_UserInfo(string REFRESH_TOKEN, string CODE)
        {     
            string urlstr = "https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo?access_token=";
            urlstr = "https://qyapi.weixin.qq.com/cgi-bin/auth/getuserinfo?access_token=";
            string Str = GetJson(urlstr + REFRESH_TOKEN + "&code=" + CODE);

            OAuthUser userinfo = JsonHelper.ParseFromJson<OAuthUser>(Str);
            sLog.Normal(string.Format("QYURLStr 请求: {0} ", Str));
            return userinfo;
        }
        
        //获取用户信息
        protected OAuthUserDetail Get_UserInfoDetail(string REFRESH_TOKEN,string USER_TICKET)
        {
            string URL = "https://qyapi.weixin.qq.com/cgi-bin/user/getuserdetail?access_token=" + REFRESH_TOKEN;
            URL= "https://qyapi.weixin.qq.com/cgi-bin/auth/getuserdetail?access_token=" + REFRESH_TOKEN;
            var dataString = "{\"user_ticket\":\"" + USER_TICKET + "\"}";
            sLog.Normal(string.Format("QYURLStr 请求: {0} ", dataString));
            var Str = Util.HttpPost2(URL, dataString);
            sLog.Normal(string.Format("QYURLStr 返回: {0} ", Str));
            OAuthUserDetail userinfo = JsonHelper.ParseFromJson<OAuthUserDetail>(Str);
            return userinfo;
        }

        //访问微信url并返回微信信息
        protected string GetJson(string url)
        {
            WebClient wc = new WebClient();
            wc.Credentials = CredentialCache.DefaultCredentials;
            wc.Encoding = System.Text.Encoding.UTF8;
            string returnText = wc.DownloadString(url);

            if (returnText.Contains("errcode"))
            {
                //可能发生错误
            }
            return returnText;
        }

        /// <summary>
        /// token类
        /// </summary>
        public class OAuth_Token
        {
            public OAuth_Token()
            {
                //TODO: 在此处添加构造函数逻辑
            }
            //access_token	网页授权接口调用凭证,注意：此access_token与基础支持的access_token不同
            //expires_in	access_token接口调用凭证超时时间，单位（秒）
            //refresh_token	用户刷新access_token
            //openid	用户唯一标识，请注意，在未关注公众号时，用户访问公众号的网页，也会产生一个用户和公众号唯一的OpenID
            //scope	用户授权的作用域，使用逗号（,）分隔
            public string _access_token;
            public string _expires_in;
            public string _refresh_token;
            public string _openid;
            public string _scope;
            public string access_token
            {
                set { _access_token = value; }
                get { return _access_token; }
            }
            public string expires_in
            {
                set { _expires_in = value; }
                get { return _expires_in; }
            }

            public string refresh_token
            {
                set { _refresh_token = value; }
                get { return _refresh_token; }
            }
            public string openid
            {
                set { _openid = value; }
                get { return _openid; }
            }
            public string scope
            {
                set { _scope = value; }
                get { return _scope; }
            }

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
        /// 返回数据对象
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
            ///  应用id  agent_id
            /// </summary>
            public string AGENTID { get; set; }
            
        }

        /// <summary>
        /// 用户信息类
        /// </summary>
        public class OAuthUser
        {
            public OAuthUser()
            { }
            #region 数据库字段
            private string _errcode;
            private string _errmsg;
            private string _userid;
            private string _deviceId;
            private string _openid;
            private string _user_ticket;
            private string _expires_in;
            private string _external_userid; 
            #endregion

            #region 字段属性
            /// <summary>
            /// 返回码
            /// </summary>
            public string errcode
            {
                set { _errcode = value; }
                get { return _errcode; }
            }
            /// <summary>
            /// 返回码说明
            /// </summary>
            public string errmsg
            {
                set { _errmsg = value; }
                get { return _errmsg; }
            }
            /// <summary>
            /// 用户id
            /// </summary>
            public string UserId
            {
                set { _userid = value; }
                get { return _userid; }
            }
            /// <summary>
            /// 成员票据，最大为512字节
            /// </summary>
            public string DeviceId
            {
                set { _deviceId = value; }
                get { return _deviceId; }
            }
            /// <summary>
            /// 成员票据，最大为512字节
            /// </summary>
            public string openid
            {
                set { _openid = value; }
                get { return _openid; }
            }
            /// <summary>
            /// 成员票据，最大为512字节
            /// </summary>
            public string user_ticket
            {
                set { _user_ticket = value; }
                get { return _user_ticket; }
            }

            /// <summary>
            /// 外部联系人id
            /// </summary>
            public string external_userid
            {
                set { _external_userid = value; }
                get { return _external_userid; }
            }
            /// <summary>
            /// 票据时间
            /// </summary>
            public string expires_in
            {
                set { _expires_in = value; }
                get { return _expires_in; }
            }

            #endregion
        }

        public class OAuthUserDetail
        {
            /// <summary>
            /// 编码
            /// </summary>
            public int errcode { get; set; }

            /// <summary>
            /// 消息
            /// </summary>
            public string errmsg { get; set; }


            /// <summary>
            /// 成员UserID
            /// </summary>
            public string userid { get; set; }

            /// <summary>
            /// 成员姓名
            /// </summary>
            public string name { get; set; }

            /// <summary>
            /// 成员手机号
            /// </summary>
            public string mobile { get; set; }

            /// <summary>
            /// 性别。0表示未定义，1表示男性，2表示女性
            /// </summary>
            public string gender { get; set; }

            /// <summary>
            /// 成员邮箱
            /// </summary>
            public string email { get; set; }

            /// <summary>
            /// 头像url
            /// </summary>
            public string avatar { get; set; }

            /// <summary>
            /// 员工个人二维码（扫描可添加为外部联系人）
            /// </summary>
            public string qr_code { get; set; }


        }

    }
}