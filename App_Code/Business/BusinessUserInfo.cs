using System;
using System.Text;
using System.Data;
using System.Web.Script.Serialization;
using System.Net;
using System.IO;
using Commercial.Sirius.WeiXin;
using System.Collections.Generic;
using Loogn;

namespace Commercial.Sirius.Business.WeiXin
{

    /// <summary>
    /// 用户信息实体类
    /// </summary>
    /// <author>zys</author>
    /// <date>2015-03-25</date>
    public sealed class BusinessUserInfo
    {

        /// <summary>
        /// 缺省构造函数
        /// </summary>
        public BusinessUserInfo()
        {
        }

        /// <summary>
        /// 缺省构造函数
        /// </summary>
        /// <param name="userinfo"></param>
        public BusinessUserInfo(OAuthUser userinfo,string touserid)
        {
            Commercial.Sirius.WeiXin.Entity_UserInfo ec = new Commercial.Sirius.WeiXin.Entity_UserInfo();
            ec.Nickname = userinfo.nickname;
            if (userinfo.sex == "0") ec.Sex = "未知";
            else ec.Sex = userinfo.sex == "1" ? "男" : "女";
            ec.Country = userinfo.country;
            ec.Province = userinfo.province;
            ec.City = userinfo.city;
            ec.HeadImgurl = userinfo.headimgurl;
            ec.LocalHead = "";
            ec.HeadDate = DateTime.MinValue.AddYears(1901); ;
            ec.AttentionDate = DateTime.MinValue.AddYears(1901);
            ec.IsState = true;//关注状态
            ec.CancelAttentionDate = DateTime.MinValue.AddYears(1901);
            ec.AttentionDate = DateTime.Now;
            //绑定信息
            ec.CustomerCode = "";
            ec.CustomerName = "";
            ec.MobilePhone = "";
            ec.IsBoundState = false;
            ec.BoundDate = DateTime.MinValue.AddYears(1901);
            //其他信息
            ec.OpenId = userinfo.openid;
            ec.FKAccount = "";
            ec.FKAccountName = "";
            ec.TouserId = touserid;
            ec.CreateDate = DateTime.Now;
            ec.UpdateDate = DateTime.Now;
            ec.IsActive = true;
            DataSave(ec);
        }

        /*==============================================================*/
        /* 用户信息实体类  单实例添加 保存                              */
        /*==============================================================*/
        #region ------------------单实例添加保存
        /// <summary>
        /// 单实例添加保存
        /// </summary>
        /// <param name="ec">微信资源信息对象</param>
        /// <returns></returns>
        public void DataSave(Entity_UserInfo ec)
        {
            //判断是否-用户存在 存在添加 不存在更新
            int userState = UserExistDayState(ec.TouserId, ec.OpenId);

            if (userState == 0)
            {
                InsertUserInfo(ec);
            }
            else
            {
                UpdateUserInfo(ec);
            }
        }

        #endregion


        /*==============================================================*/
        /* 判断用户  是否存在，用户是否需要进行更新  
         * 0 用户不存在 添加
         * 1 用户存在 更新
        /*==============================================================*/
        #region ------------------判断用户 是否存在，用户是否需要进行更新
        /// <summary>
        /// 判断用户  是否存在，用户是否需要进行更新
        /// </summary>
        /// <param name="touser_id">微信 所属公众号标识</param>
        /// <param name="open_id">微信 用户标识</param>
        /// <returns></returns>
        public int UserExistDayState(string touser_id,string open_id)
        {
            //数据接收对象
            DataSet ds = new DataSet();
            //数据返回对象
            int state = 0;

            try
            {
                //用户信息 1天内 是否更新过 
                StringBuilder sql = new StringBuilder();
                sql.Append("\r\n");
                sql.Append("SELECT [pk_id],update_date");
                sql.Append(" FROM [dbo].[pm_wx_user_info]");
                sql.Append(" where  touser_id='" + touser_id + "' and open_id='" + open_id + "'  ");
                sql.Append("\r\n");

                ds = SQLHelper.SqlHelper.ExecuteDataset(SQLHelper.StrCon.StrConn, SQLHelper.StrCon.commandType, sql.ToString());
                if (ds.Tables[0].Rows.Count > 0)
                {
                    state = 1;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
            return state;
        }

        #endregion

        /*==============================================================*/
        /* 用户信息 更新                                                */
        /*==============================================================*/
        #region ------------------用户信息 更新
        /// <summary>
        ///  用户信息 更新
        /// </summary>
        /// <param name="from_user_id">用户id</param>
        /// <returns></returns>
        public void UpdateUserInfo(Entity_UserInfo ec)
        {

            DataSet ds = new DataSet();
            //数据返回对象
            string result = null;
            //数据库语句
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("\r\n");
                sql.Append(" UPDATE pm_wx_user_info  ");
                sql.Append(" SET [nickname]='" + ec.Nickname + "' ");
                sql.AppendFormat(",[sex]='{0}'", ec.Sex);
                sql.Append(" ,[country]='" + ec.Country + "' ");
                sql.Append(" ,[province]='" + ec.Province + "' ");
                sql.Append(" ,[city]='" + ec.City + "' ");
                sql.Append(" ,[head_imgurl]='" + ec.HeadImgurl + "' ");
                sql.Append(" ,[attention_date]='" + ec.AttentionDate + "' ");
                sql.Append(" ,[update_date]='" + ec.UpdateDate + "' ");
                sql.AppendFormat(",[is_state]={0}", ec.IsState == false ? "0" : "1");
                sql.Append(" where open_id='" + ec.OpenId + "' ");
                sql.Append("\r\n");
                SQLHelper.SqlHelper.ExecuteNonQuery(SQLHelper.StrCon.StrConn, SQLHelper.StrCon.commandType, sql.ToString());


            }
            catch (Exception)
            {

            }

        }

        #endregion

        /*==============================================================*/
        /* 用户信息 添加                                                */
        /*==============================================================*/
        #region ------------------用户信息 添加
        /// <summary>
        ///  用户信息 添加
        /// </summary>
        /// <param name="from_user_id">用户id</param>
        /// <returns></returns>
        public void InsertUserInfo(Entity_UserInfo ec)
        {

            DataSet ds = new DataSet();
            //数据返回对象
            string result = null;
            //数据库语句
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("\r\n");
                sql.Append("INSERT INTO [pm_wx_user_info]");
                sql.Append("([pk_id]");
                sql.Append(",[touser_id]");
                sql.Append(",[open_id]");
                sql.Append(",[customer_code]");
                sql.Append(",[customer_name]");
                sql.Append(",[mobile_phone]");
                sql.Append(",[is_bound_state]");
                sql.Append(",[bound_date]");
                sql.Append(",[nickname]");
                sql.Append(",[sex]");
                sql.Append(",[country]");
                sql.Append(",[province]");
                sql.Append(",[city]");
                sql.Append(",[head_imgurl]");
                sql.Append(",[local_head]");
                sql.Append(",[head_date]");
                sql.Append(",[attention_date]");
                sql.Append(",[is_state]");
                sql.Append(",[cancel_attention_date]");
                sql.Append(",[create_date]");
                sql.Append(",[update_date]");
                sql.Append(",[is_active]");
                sql.Append(")");
                sql.Append("\r\n");
                sql.Append("select");
                sql.Append("'" + ec.EntityOID.ToString() + "',");
                sql.AppendFormat("{0},", ec.TouserId == null ? "null" : "'" + ec.TouserId + "'");
                sql.AppendFormat("{0},", ec.OpenId == null ? "null" : "'" + ec.OpenId + "'");
                sql.AppendFormat("{0},", ec.CustomerCode == null ? "null" : "'" + ec.CustomerCode + "'");
                sql.AppendFormat("{0},", ec.CustomerName == null ? "null" : "'" + ec.CustomerName + "'");
                sql.AppendFormat("{0},", ec.MobilePhone == null ? "null" : "'" + ec.MobilePhone + "'");
                sql.AppendFormat("{0},", ec.IsBoundState == false ? "0" : "1" + "");
                sql.AppendFormat("{0},", ec.BoundDate == null ? "null" : "'" + ec.BoundDate + "'");
                sql.AppendFormat("{0},", ec.Nickname == null ? "null" : "'" + ec.Nickname + "'");
                sql.AppendFormat("{0},", ec.Sex == null ? "null" : "'" + ec.Sex + "'");
                sql.AppendFormat("{0},", ec.Country == null ? "null" : "'" + ec.Country + "'");
                sql.AppendFormat("{0},", ec.Province == null ? "null" : "'" + ec.Province + "'");
                sql.AppendFormat("{0},", ec.City == null ? "null" : "'" + ec.City + "'");
                sql.AppendFormat("{0},", ec.HeadImgurl == null ? "null" : "'" + ec.HeadImgurl + "'");
                sql.AppendFormat("{0},", ec.LocalHead == null ? "null" : "'" + ec.LocalHead + "'");
                sql.AppendFormat("{0},", ec.HeadDate == null ? "null" : "'" + ec.HeadDate + "'");
                sql.AppendFormat("{0},", ec.AttentionDate == null ? "null" : "'" + ec.AttentionDate + "'");
                sql.AppendFormat("{0},", ec.IsState == false ? "0" : "1" + "");
                sql.AppendFormat("{0},", ec.CancelAttentionDate == null ? "null" : "'" + ec.CancelAttentionDate + "'");
                sql.AppendFormat("{0},", ec.CreateDate == null ? "null" : "'" + ec.CreateDate + "'");
                sql.AppendFormat("{0},", ec.UpdateDate == null ? "null" : "'" + ec.UpdateDate + "'");
                sql.AppendFormat("{0}", ec.IsActive == false ? "0" : "1" + "");
                sql.Append("");
                sql.Append("\r\n");

                SQLHelper.SqlHelper.ExecuteNonQuery(SQLHelper.StrCon.StrConn, SQLHelper.StrCon.commandType, sql.ToString());


            }
            catch (Exception)
            {

            }

        }

        #endregion

        #region 获取缓存值
        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <param name="appid"></param>
        /// <returns></returns>
        public string GetWxToken(string appid)
        {
            JavaScriptSerializer JavaScriptJson = new JavaScriptSerializer();
            string url = "http://wx.chinawmt.com/jnh/api/wx_token.ashx";
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("appid", appid);
            var json = Util.HttpPost2(url, JavaScriptJson.Serialize(dic));
            return Util.JsonTo<return_info>(json).message;
        }


        #endregion

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
    }
    public class return_info
    {
        /// <summary>
        /// 编码
        /// </summary>
        public int code { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string message { get; set; }

    }
    /// <summary>
    /// 用户信息类
    /// </summary>
    public class OAuthUser
    {
        public OAuthUser()
        { }
        #region 数据库字段
        private string _openID;
        private string _searchText;
        private string _nickname;
        private string _sex;
        private string _province;
        private string _city;
        private string _country;
        private string _headimgUrl;
        private string _privilege;
        private string _unionID;
        #endregion

        #region 字段属性

        /// <summary>
        /// 用户的唯一标识
        /// </summary>
        public string openid
        {
            set { _openID = value; }
            get { return _openID; }
        }

        /// <summary>
        /// 普通用户个人资料填写的城市 
        /// </summary>
        public string unionid
        {
            set { _unionID = value; }
            get { return _unionID; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string SearchText
        {
            set { _searchText = value; }
            get { return _searchText; }
        }
        /// <summary>
        /// 用户昵称 
        /// </summary>
        public string nickname
        {
            set { _nickname = value; }
            get { return _nickname; }
        }
        /// <summary>
        /// 用户的性别，值为1时是男性，值为2时是女性，值为0时是未知 
        /// </summary>
        public string sex
        {
            set { _sex = value; }
            get { return _sex; }
        }
        /// <summary>
        /// 用户个人资料填写的省份
        /// </summary>
        public string province
        {
            set { _province = value; }
            get { return _province; }
        }
        /// <summary>
        /// 普通用户个人资料填写的城市 
        /// </summary>
        public string city
        {
            set { _city = value; }
            get { return _city; }
        }
        /// <summary>
        /// 国家，如中国为CN 
        /// </summary>
        public string country
        {
            set { _country = value; }
            get { return _country; }
        }
        /// <summary>
        /// 用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像），用户没有头像时该项为空
        /// </summary>
        public string headimgurl
        {
            set { _headimgUrl = value; }
            get { return _headimgUrl; }
        }
        /// <summary>
        /// 用户特权信息，json 数组，如微信沃卡用户为
        /// </summary>
        public List<string> privilege { get; set; }
        #endregion
    }


}
