using System;
namespace Commercial.Sirius.WeiXin
{
    /// <summary>1001_帐号信息</summary>
    /// <author>郑勇帅（2013.08.17）</author>
    /// <date>2013-01-08</date>
    [System.Serializable]
    public class EntityAccountInfo
    {
        private string _entityOID;
        private string _accountCode;
        private string _accountType;
        private string _accountName;
        private string _accountId;
        private string _accountPassword;
        private string _appId;
        private string _appSecret;
        private string _callbackUrl;
        private string _token;
        private string _encodingAesKey;
        private string _touserId;
        private string _accessToken;
        private System.DateTime _updateATokenDate;
        private string _jsApiTicket;
        private System.DateTime _updateJTokenDate;
        private string _principalInfo;
        private string _principalTab;

        

        /// <summary>缺省构造函数</summary>
        public EntityAccountInfo() {}

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
        /// 功能描述：帐号编号
        /// 长度：50
        /// 不能为空：否
        /// </summary>
        public string AccountCode
        {
            get { return _accountCode; }
            set { _accountCode = value; }
        }

        /// <summary>
        /// 功能描述：帐号类型[公众、服务]
        /// 长度：50
        /// 不能为空：否
        /// </summary>
        public string AccountType
        {
            get { return _accountType; }
            set { _accountType = value; }
        }

        /// <summary>
        /// 功能描述：帐号名称
        /// 长度：50
        /// 不能为空：否
        /// </summary>
        public string AccountName
        {
            get { return _accountName; }
            set { _accountName = value; }
        }
        /// <summary>
        /// 功能描述：帐号id
        /// 长度：50
        /// 不能为空：否
        /// </summary>
        public string AccountId
        {
            get { return _accountId; }
            set { _accountId = value; }
        }

        /// <summary>
        /// 功能描述：帐号密码
        /// 长度：50
        /// 不能为空：否
        /// </summary>
        public string AccountPassword
        {
            get { return _accountPassword; }
            set { _accountPassword = value; }
        }

        /// <summary>
        /// 功能描述：AppID(应用ID)
        /// 长度：50
        /// 不能为空：否
        /// </summary>
        public string AppId
        {
            get { return _appId; }
            set { _appId = value; }
        }

        /// <summary>
        /// 功能描述：AppSecret(应用密钥)
        /// 长度：50
        /// 不能为空：否
        /// </summary>
        public string AppSecret
        {
            get { return _appSecret; }
            set { _appSecret = value; }
        }

        /// <summary>
        /// 功能描述：URL(服务器地址)
        /// 长度：200
        /// 不能为空：否
        /// </summary>
        public string CallbackUrl
        {
            get { return _callbackUrl; }
            set { _callbackUrl = value; }
        }

        /// <summary>
        /// 功能描述：Token(令牌)
        /// 长度：50
        /// 不能为空：否
        /// </summary>
        public string Token
        {
            get { return _token; }
            set { _token = value; }
        }

        /// <summary>
        /// 功能描述：EncodingAESKey(消息密钥)
        /// 长度：50
        /// 不能为空：否
        /// </summary>
        public string EncodingAesKey
        {
            get { return _encodingAesKey; }
            set { _encodingAesKey = value; }
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
        /// 功能描述：当前令牌信息[7200秒更新]
        /// 长度：150
        /// 不能为空：否
        /// </summary>
        public string AccessToken
        {
            get { return _accessToken; }
            set { _accessToken = value; }
        }


        /// <summary>
        /// 功能描述：更新令牌时间
        /// 长度：150
        /// 不能为空：否
        /// </summary>
        public System.DateTime UpdateATokenDate
        {
            get { return _updateATokenDate; }
            set { _updateATokenDate = value; }
        }

        /// <summary>
        /// 功能描述：当前令牌信息[7200秒更新]
        /// 长度：150
        /// 不能为空：否
        /// </summary>
        public string JsApiToken
        {
            get { return _jsApiTicket; }
            set { _jsApiTicket = value; }
        }


        /// <summary>
        /// 功能描述：更新令牌时间
        /// 长度：150
        /// 不能为空：否
        /// </summary>
        public System.DateTime UpdateJTokenDate
        {
            get { return _updateJTokenDate; }
            set { _updateJTokenDate = value; }
        }

        /// <summary>
        /// 功能描述：主体信息
        /// 长度：200
        /// 不能为空：否
        /// </summary>
        public string PrincipalInfo
        {
            get { return _principalInfo; }
            set { _principalInfo = value; }
        }

        /// <summary>
        /// 功能描述：运营者电话
        /// 长度：50
        /// 不能为空：否
        /// </summary>
        public string PrincipalTab
        {
            get { return _principalTab; }
            set { _principalTab = value; }
        }
    }
}
