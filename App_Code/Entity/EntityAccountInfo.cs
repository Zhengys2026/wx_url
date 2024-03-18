using System;
namespace Commercial.Sirius.WeiXin
{
    /// <summary>1001_�ʺ���Ϣ</summary>
    /// <author>֣��˧��2013.08.17��</author>
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

        

        /// <summary>ȱʡ���캯��</summary>
        public EntityAccountInfo() {}

        /// <summary>����</summary>
        public string EntityOID
        {
            get { return new System.Guid(_entityOID == null ? _entityOID = Guid.NewGuid().ToString() : _entityOID).ToString(); }
            set { _entityOID = value; }
        }

        /// <summary>�ڲ�ӳ������</summary>
        public string InnerEntityOID
        {
            get { return _entityOID; }
            set { _entityOID = value; }
        }


        /// <summary>
        /// �����������ʺű��
        /// ���ȣ�50
        /// ����Ϊ�գ���
        /// </summary>
        public string AccountCode
        {
            get { return _accountCode; }
            set { _accountCode = value; }
        }

        /// <summary>
        /// �����������ʺ�����[���ڡ�����]
        /// ���ȣ�50
        /// ����Ϊ�գ���
        /// </summary>
        public string AccountType
        {
            get { return _accountType; }
            set { _accountType = value; }
        }

        /// <summary>
        /// �����������ʺ�����
        /// ���ȣ�50
        /// ����Ϊ�գ���
        /// </summary>
        public string AccountName
        {
            get { return _accountName; }
            set { _accountName = value; }
        }
        /// <summary>
        /// �����������ʺ�id
        /// ���ȣ�50
        /// ����Ϊ�գ���
        /// </summary>
        public string AccountId
        {
            get { return _accountId; }
            set { _accountId = value; }
        }

        /// <summary>
        /// �����������ʺ�����
        /// ���ȣ�50
        /// ����Ϊ�գ���
        /// </summary>
        public string AccountPassword
        {
            get { return _accountPassword; }
            set { _accountPassword = value; }
        }

        /// <summary>
        /// ����������AppID(Ӧ��ID)
        /// ���ȣ�50
        /// ����Ϊ�գ���
        /// </summary>
        public string AppId
        {
            get { return _appId; }
            set { _appId = value; }
        }

        /// <summary>
        /// ����������AppSecret(Ӧ����Կ)
        /// ���ȣ�50
        /// ����Ϊ�գ���
        /// </summary>
        public string AppSecret
        {
            get { return _appSecret; }
            set { _appSecret = value; }
        }

        /// <summary>
        /// ����������URL(��������ַ)
        /// ���ȣ�200
        /// ����Ϊ�գ���
        /// </summary>
        public string CallbackUrl
        {
            get { return _callbackUrl; }
            set { _callbackUrl = value; }
        }

        /// <summary>
        /// ����������Token(����)
        /// ���ȣ�50
        /// ����Ϊ�գ���
        /// </summary>
        public string Token
        {
            get { return _token; }
            set { _token = value; }
        }

        /// <summary>
        /// ����������EncodingAESKey(��Ϣ��Կ)
        /// ���ȣ�50
        /// ����Ϊ�գ���
        /// </summary>
        public string EncodingAesKey
        {
            get { return _encodingAesKey; }
            set { _encodingAesKey = value; }
        }

        /// <summary>
        /// ����������������΢�ź�-ԭʼID
        /// ���ȣ�50
        /// ����Ϊ�գ���
        /// </summary>
        public string TouserId
        {
            get { return _touserId; }
            set { _touserId = value; }
        }

        /// <summary>
        /// ������������ǰ������Ϣ[7200�����]
        /// ���ȣ�150
        /// ����Ϊ�գ���
        /// </summary>
        public string AccessToken
        {
            get { return _accessToken; }
            set { _accessToken = value; }
        }


        /// <summary>
        /// ������������������ʱ��
        /// ���ȣ�150
        /// ����Ϊ�գ���
        /// </summary>
        public System.DateTime UpdateATokenDate
        {
            get { return _updateATokenDate; }
            set { _updateATokenDate = value; }
        }

        /// <summary>
        /// ������������ǰ������Ϣ[7200�����]
        /// ���ȣ�150
        /// ����Ϊ�գ���
        /// </summary>
        public string JsApiToken
        {
            get { return _jsApiTicket; }
            set { _jsApiTicket = value; }
        }


        /// <summary>
        /// ������������������ʱ��
        /// ���ȣ�150
        /// ����Ϊ�գ���
        /// </summary>
        public System.DateTime UpdateJTokenDate
        {
            get { return _updateJTokenDate; }
            set { _updateJTokenDate = value; }
        }

        /// <summary>
        /// ����������������Ϣ
        /// ���ȣ�200
        /// ����Ϊ�գ���
        /// </summary>
        public string PrincipalInfo
        {
            get { return _principalInfo; }
            set { _principalInfo = value; }
        }

        /// <summary>
        /// ������������Ӫ�ߵ绰
        /// ���ȣ�50
        /// ����Ϊ�գ���
        /// </summary>
        public string PrincipalTab
        {
            get { return _principalTab; }
            set { _principalTab = value; }
        }
    }
}
