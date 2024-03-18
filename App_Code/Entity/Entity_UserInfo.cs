using System;
namespace Commercial.Sirius.WeiXin
{
    /// <summary>1001_�û���Ϣ</summary>
    /// <author>֣��˧��2013.08.17��</author>
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
        

        /// <summary>ȱʡ���캯��</summary>
        public Entity_UserInfo() {}

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
        /// ������������������[�ʺ���Ϣ]
        /// ���ȣ�50
        /// ����Ϊ�գ���
        /// </summary>
        public string FKAccount
        {
            get { return _fk_account; }
            set { _fk_account = value; }
        }
        /// <summary>
        /// ������������������[�ʺ���Ϣ]
        /// ���ȣ�50
        /// ����Ϊ�գ���
        /// </summary>
        public string FKAccountName
        {
            get { return _fk_account_name; }
            set { _fk_account_name = value; }
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
        /// �����������ͻ����
        /// ���ȣ�50
        /// ����Ϊ�գ���
        /// </summary>
        public string CustomerCode
        {
            get { return _customerCode; }
            set { _customerCode = value; }
        }

        /// <summary>
        /// �����������ͻ�����
        /// ���ȣ�50
        /// ����Ϊ�գ���
        /// </summary>
        public string CustomerName
        {
            get { return _customerName; }
            set { _customerName = value; }
        }

        /// <summary>
        /// ����������[�û�]���ͷ��ʺţ�һ��OpenID��
        /// ���ȣ�100
        /// ����Ϊ�գ���
        /// </summary>
        public string OpenId
        {
            get { return _open_id; }
            set { _open_id = value; }
        }

        /// <summary>
        /// �������������ֻ�
        /// ���ȣ�50
        /// ����Ϊ�գ���
        /// </summary>
        public string MobilePhone
        {
            get { return _mobilePhone; }
            set { _mobilePhone = value; }
        }

        /// <summary>
        /// ������������״̬
        /// ����Ϊ�գ���
        /// </summary>
        public bool IsBoundState
        {
            get { return _isBoundState; }
            set { _isBoundState = value; }
        }

        /// <summary>
        /// ������������ʱ��
        /// ����Ϊ�գ���
        /// </summary>
        public System.DateTime BoundDate
        {
            get { return _boundDate; }
            set { _boundDate = value; }
        }

        /// <summary>
        /// �����������û����ǳ�
        /// ���ȣ�50
        /// ����Ϊ�գ���
        /// </summary>
        public string Nickname
        {
            get { return _nickname; }
            set { _nickname = value; }
        }

        /// <summary>
        /// �����������û����Ա�
        /// ���ȣ�50
        /// ����Ϊ�գ���
        /// </summary>
        public string Sex
        {
            get { return _sex; }
            set { _sex = value; }
        }

        /// <summary>
        /// �����������û����ڹ���
        /// ���ȣ�50
        /// ����Ϊ�գ���
        /// </summary>
        public string Country
        {
            get { return _country; }
            set { _country = value; }
        }

        /// <summary>
        /// �����������û�����ʡ��
        /// ���ȣ�50
        /// ����Ϊ�գ���
        /// </summary>
        public string Province
        {
            get { return _province; }
            set { _province = value; }
        }

        /// <summary>
        /// �����������û����ڳ���
        /// ���ȣ�50
        /// ����Ϊ�գ���
        /// </summary>
        public string City
        {
            get { return _city; }
            set { _city = value; }
        }

        /// <summary>
        /// �����������û�ͷ��[����][һ���û�һ���ڸ���һ��]
        /// ���ȣ�200
        /// ����Ϊ�գ���
        /// </summary>
        public string HeadImgurl
        {
            get { return _headImgurl; }
            set { _headImgurl = value; }
        }

        /// <summary>
        /// ��������������ͷ��
        /// ���ȣ�200
        /// ����Ϊ�գ���
        /// </summary>
        public string LocalHead
        {
            get { return _localHead; }
            set { _localHead = value; }
        }

        /// <summary>
        /// ����������ͷ�񴴽�ʱ��
        /// ����Ϊ�գ���
        /// </summary>
        public System.DateTime HeadDate
        {
            get { return _headDate; }
            set { _headDate = value; }
        }

        /// <summary>
        /// �����������û���עʱ��
        /// ����Ϊ�գ���
        /// </summary>
        public System.DateTime AttentionDate
        {
            get { return _attentionDate; }
            set { _attentionDate = value; }
        }

        /// <summary>
        /// ������������ע״̬[��ע��δ��ע]
        /// ����Ϊ�գ���
        /// </summary>
        public bool IsState
        {
            get { return _isState; }
            set { _isState = value; }
        }

        /// <summary>
        /// ����������ȡ����עʱ��
        /// ����Ϊ�գ���
        /// </summary>
        public System.DateTime CancelAttentionDate
        {
            get { return _cancelAttentionDate; }
            set { _cancelAttentionDate = value; }
        }

        /// <summary>
        /// �����������Ƿ񼤻�״̬
        /// ����Ϊ�գ���
        /// </summary>
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }

        /// <summary>
        /// ��������������ʱ��
        /// ����Ϊ�գ���
        /// </summary>
        public System.DateTime CreateDate
        {
            get { return _createDate; }
            set { _createDate = value; }
        }

        /// <summary>
        /// ��������������ʱ��
        /// ����Ϊ�գ���
        /// </summary>
        public System.DateTime UpdateDate
        {
            get { return _updateDate; }
            set { _updateDate = value; }
        }
    }
}
