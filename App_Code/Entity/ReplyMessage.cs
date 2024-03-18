using Reply.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victop.Frame.CoreLibrary.Enums;

namespace Reply.Entity
{
    /// <summary>
    /// 请求成功失败 返回体 返回体
    /// </summary>
    public class ReplyMessage
    {
        private string requestStatus;
        /// <summary>
        /// 请求状态 成功： success 异常： error  失败： failure
        /// </summary>
        public virtual string RequestStatus
        {
            get { return requestStatus; }
            set { requestStatus = value; }
        }
        private string requestStatusCode;

        /// <summary>
        /// 请求状态 编码 成功： 1 成功： -1  失败： 0 失败
        /// </summary>
        public virtual string RequestStatusCode
        {
            get { return requestStatusCode; }
            set { requestStatusCode = value; }
        }

        private ReplyModeEnum replyMode;
        /// <summary>
        /// 应答类型
        /// </summary>
        public virtual ReplyModeEnum ReplyMode
        {
            get { return replyMode; }
            set { replyMode = value; }
        }

        private string replyContent;
        /// <summary>
        /// 应答消息数据字段
        /// </summary>
        public virtual string ReplyContent
        {
            get { return replyContent; }
            set { replyContent = value; }
        }

        private string replyAlertMessage;

        /// <summary>
        /// 应答提示信息
        /// </summary>
        public virtual string ReplyAlertMessage
        {
            get { return replyAlertMessage; }
            set { replyAlertMessage = value; }
        }

        /// <summary>
        /// 异常编码
        /// </summary>
        public int errorCode { get; set; }

        /// <summary>
        /// 异常消息
        /// </summary>
        public string errorMessage { get; set; }

        /// <summary>
        /// 异常记录
        /// </summary>
        public string error { get; set; }


    }
}
