using System;
namespace Commercial.Sirius.WeiXin
{
    /// <summary>1001_用户信息</summary>
    /// <author>郑勇帅（2013.08.17）</author>
    /// <date>2013-01-08</date>
    [System.Serializable]
    public class Entity_UserInfo
    {
   
        private string _entityOID;
        private string _fk_account;
        private string _fk_account_name;
        private string _touserId;
        private string _customerCode;
        private string _customerName;
        private string _open_id;
        private string _mobilePhone;
        private bool _isBoundState;
        private System.DateTime _boundDate;
        private string _nickname;
        private string _sex;
        private string _country;
        private string _province;
        private string _city;
        private string _headImgurl;
        private string _localHead;
        private System.DateTime _headDate;
        private System.DateTime _attentionDate;
        private bool _isState;
        private System.DateTime _cancelAttentionDate;
        private bool _isActive;
        private System.DateTime _createDate;
        private System.DateTime _updateDate;
        

        /// <summary>缺省构造函数</summary>
        public Entity_UserInfo() {}

        /// <summary>主键</summary>
        public string EntityOID
        {
            get { return new System.Guid(_entityOID == null ? _entityOID = Guid.NewGuid().ToString() : _entityOID).ToString(); }
            set { _entityOID = value; }
        }

        /// <summary>内部映射主键</summary>
        public string InnerEntityOID
        {
            get { return _entityOID; }
            set { _entityOID = value; }
        }

        /// <summary>
        /// 功能描述：所属主体[帐号信息]
        /// 长度：50
        /// 不能为空：否
        /// </summary>
        public string FKAccount
        {
            get { return _fk_account; }
            set { _fk_account = value; }
        }
        /// <summary>
        /// 功能描述：所属主体[帐号信息]
        /// 长度：50
        /// 不能为空：否
        /// </summary>
        public string FKAccountName
        {
            get { return _fk_account_name; }
            set { _fk_account_name = value; }
        }
        /// <summary>
        /// 功能描述：开发者微信号-原始ID
        /// 长度：50
        /// 不能为空：否
        /// </summary>
        public string TouserId
        {
            get { return _touserId; }
            set { _touserId = value; }
        }

        /// <summary>
        /// 功能描述：客户编号
        /// 长度：50
        /// 不能为空：否
        /// </summary>
        public string CustomerCode
        {
            get { return _customerCode; }
            set { _customerCode = value; }
        }

        /// <summary>
        /// 功能描述：客户名称
        /// 长度：50
        /// 不能为空：否
        /// </summary>
        public string CustomerName
        {
            get { return _customerName; }
            set { _customerName = value; }
        }

        /// <summary>
        /// 功能描述：[用户]发送方帐号（一个OpenID）
        /// 长度：100
        /// 不能为空：否
        /// </summary>
        public string OpenId
        {
            get { return _open_id; }
            set { _open_id = value; }
        }

        /// <summary>
        /// 功能描述：绑定手机
        /// 长度：50
        /// 不能为空：否
        /// </summary>
        public string MobilePhone
        {
            get { return _mobilePhone; }
            set { _mobilePhone = value; }
        }

        /// <summary>
        /// 功能描述：绑定状态
        /// 不能为空：否
        /// </summary>
        public bool IsBoundState
        {
            get { return _isBoundState; }
            set { _isBoundState = value; }
        }

        /// <summary>
        /// 功能描述：绑定时间
        /// 不能为空：否
        /// </summary>
        public System.DateTime BoundDate
        {
            get { return _boundDate; }
            set { _boundDate = value; }
        }

        /// <summary>
        /// 功能描述：用户的昵称
        /// 长度：50
        /// 不能为空：否
        /// </summary>
        public string Nickname
        {
            get { return _nickname; }
            set { _nickname = value; }
        }

        /// <summary>
        /// 功能描述：用户的性别
        /// 长度：50
        /// 不能为空：否
        /// </summary>
        public string Sex
        {
            get { return _sex; }
            set { _sex = value; }
        }

        /// <summary>
        /// 功能描述：用户所在国家
        /// 长度：50
        /// 不能为空：否
        /// </summary>
        public string Country
        {
            get { return _country; }
            set { _country = value; }
        }

        /// <summary>
        /// 功能描述：用户所在省份
        /// 长度：50
        /// 不能为空：否
        /// </summary>
        public string Province
        {
            get { return _province; }
            set { _province = value; }
        }

        /// <summary>
        /// 功能描述：用户所在城市
        /// 长度：50
        /// 不能为空：否
        /// </summary>
        public string City
        {
            get { return _city; }
            set { _city = value; }
        }

        /// <summary>
        /// 功能描述：用户头像[网络][一个用户一天内更新一次]
        /// 长度：200
        /// 不能为空：否
        /// </summary>
        public string HeadImgurl
        {
            get { return _headImgurl; }
            set { _headImgurl = value; }
        }

        /// <summary>
        /// 功能描述：本地头像
        /// 长度：200
        /// 不能为空：否
        /// </summary>
        public string LocalHead
        {
            get { return _localHead; }
            set { _localHead = value; }
        }

        /// <summary>
        /// 功能描述：头像创建时间
        /// 不能为空：否
        /// </summary>
        public System.DateTime HeadDate
        {
            get { return _headDate; }
            set { _headDate = value; }
        }

        /// <summary>
        /// 功能描述：用户关注时间
        /// 不能为空：否
        /// </summary>
        public System.DateTime AttentionDate
        {
            get { return _attentionDate; }
            set { _attentionDate = value; }
        }

        /// <summary>
        /// 功能描述：关注状态[关注、未关注]
        /// 不能为空：否
        /// </summary>
        public bool IsState
        {
            get { return _isState; }
            set { _isState = value; }
        }

        /// <summary>
        /// 功能描述：取消关注时间
        /// 不能为空：否
        /// </summary>
        public System.DateTime CancelAttentionDate
        {
            get { return _cancelAttentionDate; }
            set { _cancelAttentionDate = value; }
        }

        /// <summary>
        /// 功能描述：是否激活状态
        /// 不能为空：否
        /// </summary>
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }

        /// <summary>
        /// 功能描述：创建时间
        /// 不能为空：否
        /// </summary>
        public System.DateTime CreateDate
        {
            get { return _createDate; }
            set { _createDate = value; }
        }

        /// <summary>
        /// 功能描述：更新时间
        /// 不能为空：否
        /// </summary>
        public System.DateTime UpdateDate
        {
            get { return _updateDate; }
            set { _updateDate = value; }
        }
    }
}
