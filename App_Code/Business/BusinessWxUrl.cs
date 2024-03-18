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
    /// 微信 url 跳转服务
    /// </summary>
    public class BusinessWxUrl
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
                string reurls = "", reurl = "", url_param = "", appid = "", type = "", sign="";
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
                        if (reurlArr[i].IndexOf("sign=") >= 0)
                        {
                            sign = reurlArr[i].Replace("sign=", "");
                            continue;
                        }
                    }
                    if (appid != "")
                    {
                        //获取配置信息
                        BusinessWXToken _wxToken = new BusinessWXToken(appid,"");
                        _config = new WxConfig();
                        _config.APPSECRET=_wxToken.app_secret;
                        _config.APPID = appid;
                    }
                    #endregion
                }
                sLog.Normal(string.Format(urlType+ " 请求: {0} \r\n {1} ", urlType, reurls));
                if (urlType == "URL")
                {
                    #region ------------------urlProcess 逻辑业务处理
                    //拼接参数
                    string param = null;
                    //获取微信回传的code
                    string code = "";
                    if (context.Request.QueryString["code"] != null && context.Request.QueryString["code"] != "")
                    {
                        //获取微信回传的code
                        code = context.Request.QueryString["code"].ToString();
                        sLog.Normal(string.Format(urlType + " code请求: {0} \r\n {1}\r\n code {2} ", urlType, reurls, context.Request.QueryString["code"]));
                        OAuth_Token Model = Get_token(code);
                        //获取token
                        OAuthUser OAuthUser_Model = new OAuthUser();
                        if (context.Request.QueryString["auth"]== "1" || sign== "1")
                        {
                            OAuthUser_Model = Get_UserInfo(Model.access_token, Model.openid);
                        }else
                        {
                            type = "1";
                            OAuthUser_Model.openid = Model.openid;
                        }
                        if (OAuthUser_Model.openid != null && OAuthUser_Model.openid != "")  //已获取得openid及其他信息
                        {
                            Encoding utf8 = Encoding.UTF8;
                            switch (type)
                            {
                                case "1":
                                    {
                                        param = "&openid=" + OAuthUser_Model.openid;
                                        break;
                                    }
                                case "2":
                                    {
                                        param = "&openid=" + OAuthUser_Model.openid;
                                        param += "&nickname=" + HttpUtility.UrlEncode(OAuthUser_Model.nickname, utf8).ToUpper();
                                        break;
                                    }
                                case "3":
                                    {
                                        param = "&openid=" + OAuthUser_Model.openid;
                                        param += "&nickname=" + HttpUtility.UrlEncode(OAuthUser_Model.nickname, utf8).ToUpper();
                                        param += "&headimgurl=" + OAuthUser_Model.headimgurl;
                                        break;
                                    }
                                case "4":
                                    {
                                        param = "&openid=" + OAuthUser_Model.openid;
                                        param += "&nickname=" + HttpUtility.UrlEncode(OAuthUser_Model.nickname, utf8).ToUpper();
                                        param += "&headimgurl=" + OAuthUser_Model.headimgurl;
                                        param += "&sex=" + OAuthUser_Model.sex;
                                        break;
                                    }
                            }
                            if (!string.IsNullOrWhiteSpace(OAuthUser_Model.unionid))
                            {
                                param += "&unionid=" + OAuthUser_Model.unionid;
                            }
                            //reurl 是否本身携带参数
                            if (reurl.IndexOf('?') > 0)
                            {
                                context.Response.Redirect(reurl + "&date=" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + param, false);
                                sLog.Normal(string.Format(urlType + " 返回: {0} \r\n {1} ", reurl, "&date=" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + param));
                            }
                            else
                            {
                                context.Response.Redirect(reurl + "?date=" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + url_param+param, false);
                                sLog.Normal(string.Format(urlType + " 返回: {0} \r\n {1} ", reurl, "&date=" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + param));
                            }
                        }
                        else
                        {
                            if(sign=="0")
                            {
                                context.Response.Redirect("urlProcessAuthorize.ashx?auth=0&reurl=" + reurls, false);
                            }
                            else
                            {
                                //未获得openid，访问弹出微信授权页面，提示用户授权
                                context.Response.Redirect("urlProcessAuthorize.ashx?auth=1&reurl=" + reurls, false);
                            }
                        }
                    }
                    #endregion
                }
                else if (urlType == "Authorize")
                {
                    #region ------------------UrlRedirectAuthorize 授权业务逻辑处理
                    //弹出授权页面(如在不弹出授权页面基础下未获得openid，弹出授权页面，提示用户授权)
                    if (context.Request.QueryString["auth"] != null && context.Request.QueryString["auth"] != "" && context.Request.QueryString["auth"] == "1")
                    {
                        string url = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + _config.APPID + "&redirect_uri=https://wx.chinawmt.com/wxUrl/api/urlProcess.ashx?reurl=" + context.Request.QueryString["reurl"].ToString() + "&response_type=code&scope=snsapi_userinfo&state=1#wechat_redirect";
                        sLog.Normal(string.Format(urlType + " 授权: {0} \r\n {1} ", url, context.Request.QueryString["auth"]));
                        context.Response.Redirect(url);
                    }
                    else
                    {
                        string url = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + _config.APPID + "&redirect_uri=https://wx.chinawmt.com/wxUrl/api/urlProcess.ashx?reurl=" + context.Request.QueryString["reurl"].ToString() + "&response_type=code&scope=snsapi_base&state=1#wechat_redirect";
                        sLog.Normal(string.Format(urlType + " 静默授权: {0} \r\n {1} ", url, context.Request.QueryString["auth"]));
                        context.Response.Redirect(url);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                sLog.Error(ex.Message);
               // throw new Exception(ex.Message);
            }
        }
     
        //根据appid，secret，code获取微信openid、access token信息
        protected OAuth_Token Get_token(string Code)
        {
            //获取微信回传的openid、access token
            string Str = GetJson("https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + _config.APPID + "&secret=" + _config.APPSECRET + "&code=" + Code + "&grant_type=authorization_code");
            //微信回传的数据为Json格式，将Json格式转化成对象
            OAuth_Token Oauth_Token_Model = JsonHelper.ParseFromJson<OAuth_Token>(Str);
            return Oauth_Token_Model;
        }

        //刷新Token(好像这个刷新Token没有实际作用)
        protected OAuth_Token refresh_token(string REFRESH_TOKEN)
        {
            string Str = GetJson("https://api.weixin.qq.com/sns/oauth2/refresh_token?appid=" + _config.APPID + "&grant_type=refresh_token&refresh_token=" + REFRESH_TOKEN);
            OAuth_Token Oauth_Token_Model = JsonHelper.ParseFromJson<OAuth_Token>(Str);
            return Oauth_Token_Model;
        }

        //根据openid，access token获得用户信息
        protected OAuthUser Get_UserInfo(string REFRESH_TOKEN, string OPENID)
        {
            string Str = GetJson("https://api.weixin.qq.com/sns/userinfo?access_token=" + REFRESH_TOKEN + "&openid=" + OPENID);
            sLog.Normal(Str);
            OAuthUser userinfo = JsonHelper.ParseFromJson<OAuthUser>(Str);
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
        }

    }
}