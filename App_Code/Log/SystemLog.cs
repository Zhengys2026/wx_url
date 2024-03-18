using System;
/********************************************************************
created:	2012/01/17
created:	17:1:2012   10:16
filename: 	Common\Log5.cs
file base:	Log5
file ext:	cs
author:		Cupid
purpose:	写日志的类，模仿log4写的，所以起了个名字叫log5，但比log4要简单。
* 一个类实例默认会创建三个日志，错误日志，普通日志和调试日志。你也可以创建自己的
* 日志。理论上支持多线程，但没有经过测试。
*********************************************************************/
namespace Log
{

    public class SystemLog : IDisposable
    {
        private string _dateformat = "yyyy-MM-dd HH:mm:ss";
       
        private System.Text.Encoding _encoding = System.Text.Encoding.Default;
        private int _buffersize = 30 * 1024;
        private static string _appSiteName = null;
        private string _errfile = "";
        private string _normalfile = "";
        private string _debugfile = "";

        /// <summary>
        /// 默认错误日志路径
        /// </summary>
        public string ErrorFile
        {
            get
            {
                return _errfile;
            }
            set
            {
                _errfile = value;
                Log5Info info = GetLogInfo(_sys_err_log_name);
                if (info != null)
                    info.Logfile = _errfile;
            }
        }

        /// <summary>
        /// 默认普通日志路径
        /// </summary>
        public string NormalFile
        {
            get
            {
                return _normalfile;
            }
            set
            {
                _normalfile = value;
                Log5Info info = GetLogInfo(_sys_normal_log_name);
                if (info != null)
                    info.Logfile = _normalfile;
            }
        }

        /// <summary>
        /// 默认调试日志路径
        /// </summary>
        public string DebugFile
        {
            get
            {
                return _debugfile;
            }
            set
            {
                _debugfile = value;
                Log5Info info = GetLogInfo(_sys_debug_log_name);
                if (info != null)
                    info.Logfile = _debugfile;
            }
        }

        /// <summary>
        /// 是否启用调试，暂时没用到
        /// </summary>
        public bool EnableDebug { get; set; }

        /// <summary>
        /// 缓存大小，单位字节。
        /// </summary>
        public int BufferSize { get { return _buffersize; } set { _buffersize = value; } }

        /// <summary>
        /// 默认是否在日志中显示行号
        /// </summary>
        public bool ShowLineNo { get; set; }
        /// <summary>
        /// 默认是否在日志中显示文件名
        /// </summary>
        public ShowFileType ShowFile { get; set; }
        /// <summary>
        /// 默认是否在日志中显示方法名
        /// </summary>
        public bool ShowMethod { get; set; }
        /// <summary>
        /// 默认日志中的日期格式，例如yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string Dateformat { get { return _dateformat; } set { _dateformat = value; } }
        /// <summary>
        /// 默认的日志编码
        /// </summary>
        public System.Text.Encoding LogEncoding { get { return _encoding; } set { _encoding = value; } }

        /// <summary>
        /// 错误日志名
        /// </summary>
        public const string _sys_err_log_name = "__sys_err_log__";
        /// <summary>
        /// 普通日志名
        /// </summary>
        public const string _sys_normal_log_name = "__sys_normal_log__";
        /// <summary>
        /// 调试日志名
        /// </summary>
        public const string _sys_debug_log_name = "__sys_debug_log__";


        private System.Collections.Hashtable _loghash = new System.Collections.Hashtable();
        public SystemLog()
        {
            initVars();

            ErrorFile = Log5Info.GetLogFilePath()+"\\log\\error.log";
           
            AddDefaultLogs();

            SetBufferSize(_sys_err_log_name, 10);
            SetBufferSize(_sys_normal_log_name, 10);
            //设置普通日志的文件名为自动生成
            //%y:年 %M:月%d:日 %H:时 %m: 分 %s:秒 %f:日志名
            SetAutoGenerateFilename(_sys_normal_log_name, true, "log\\", "%y\\SOAResponseLog-%y-%M-%d.log");
           
            //设置错误日志中显示文件名
            SetShowFilePath(_sys_err_log_name, ShowFileType.filename);
   

   
        }

        /*---------------------------------------------------
         * 指定各种日志  路径  进行初始化
         * ------------------------------------------------*/
        #region ------------------指定各种日志 路径 进行初始化
        /// <summary>
        /// 指定错误日志路径的构造函数
        /// </summary>
        /// <param name="errfile">错误日志路径</param>
        public SystemLog(string errfile)
        {
            try
            {
                initVars();
                ErrorFile = errfile;
                AddDefaultLogs();
            }
            catch (Exception)
            {
                
            }
        }

        /// <summary>
        /// 指定错误日志路径和普通日志路径的构造函数
        /// </summary>
        /// <param name="errfile">错误日志路径</param>
        /// <param name="normalfile">普通日志路径</param>
        public SystemLog(string errfile, string normalfile)
        {
            try
            {
                initVars();
                ErrorFile = errfile;
                NormalFile = normalfile;
                AddDefaultLogs();
            }
            catch (Exception)
            {
               
            }
        }

        /// <summary>
        /// 指定错误日志路径，普通日志路径以及调试日志路径的构造函数
        /// </summary>
        /// <param name="errfile">错误日志路径</param>
        /// <param name="normalfile">普通日志路径</param>
        /// <param name="debug">是否启用调试</param>
        /// <param name="debugfile">调试文件</param>
        public SystemLog(string errfile, string normalfile, bool debug, string debugfile)
        {
            try
            {
                initVars();
                ErrorFile = errfile;
                NormalFile = normalfile;
                EnableDebug = debug;
                DebugFile = debugfile;
                AddDefaultLogs();
            }
            catch (Exception)
            {
                
            }
        }

        /// <summary>
        /// 指定错误日志路径，普通日志路径，调试日志路径和默认缓存大小的构造函数
        /// </summary>
        /// <param name="errfile">错误日志路径</param>
        /// <param name="normalfile">普通日志路径</param>
        /// <param name="debug">是否启用调试</param>
        /// <param name="debugfile">调试文件</param>
        /// <param name="buffersize">默认缓存大小</param>
        public SystemLog(string errfile, string normalfile, bool debug, string debugfile, int buffersize)
        {
            try
            {
                initVars();
                ErrorFile = errfile;
                NormalFile = normalfile;
                EnableDebug = debug;
                DebugFile = debugfile;
                BufferSize = buffersize;
                AddDefaultLogs();
            }
            catch (Exception)
            {
               
            }
        }


        /// <summary>
        /// 初始化默认值，可以从文件配置中读取
        /// </summary>
        private void initVars()
        {
            try
            {
                Dateformat = "yyyy-MM-dd HH:mm:ss";
                LogEncoding = System.Text.Encoding.Default;
                EnableDebug = false;
                ShowLineNo = true;
                ShowMethod = true;
                ShowFile = ShowFileType.none;
                BufferSize = 30 * 1024;
            }
            catch (Exception)
            {
                
            }
        }
        #endregion

        /*---------------------------------------------------
         * 写各种日志 
         * ------------------------------------------------*/
        #region ------------------写各种日志
        /// <summary>
        /// 写错误日志
        /// </summary>
        /// <param name="err">日志文本</param>
        public void Error(string err)
        {
            try
            {
                Log5Info loginf = GetLogInfo(_sys_err_log_name);
                if (loginf == null)
#if DEBUG
                    throw new Exception("LogInfo is not found.");
#else
				return;
#endif

                loginf.AddStr(err);
                loginf.CheckSize();
            }
            catch (Exception)
            {
                
            }
        }
        /// <summary>
        /// 写错误日志 对象级别
        /// </summary>
        /// <param name="err">日志文本</param>
        public void Errors(string err,System.Exception ex)
        {
            try
            {
                Log5Info loginf = GetLogInfo(_sys_err_log_name);
                if (loginf == null)
#if DEBUG
                    throw new Exception("LogInfo is not found.");
#else
				return;
#endif

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("异常说明:" + err + "\r\n");
                sb.Append("------------------------------------------\r\n");
                sb.Append("Type:" + ex.GetType() + "\r\n");
                sb.Append("Message:" + ex.Message + "\r\n");
                sb.Append("Source:" + ex.Source + "\r\n");
                sb.Append("StackTrace:" + ex.StackTrace + "\r\n");
                sb.Append("TargetSite:" + ex.TargetSite + "\r\n");

                if (ex != null)
                {
                    while (ex.InnerException != null)
                    {
                        ex = ex.InnerException;
                        sb.Append("------------------------------------------\r\n");
                        sb.Append("Type:" + ex.GetType() + "\r\n");
                        sb.Append("Message:" + ex.Message + "\r\n");
                        sb.Append("Source:" + ex.Source + "\r\n");
                        sb.Append("StackTrace:" + ex.StackTrace + "\r\n");
                        sb.Append("TargetSite:" + ex.TargetSite + "\r\n");
                    }
                }
                loginf.AddStr(sb.ToString());
                loginf.CheckSize();
            }
            catch (Exception)
            {
                
            }
        }

        /// <summary>
        /// 写含有参数的错误日志
        /// </summary>
        /// <param name="err">日志文本</param>
        /// <param name="args">参数</param>
        public void Error(string err, params Object[] args)
        {
            try
            {
                string lo = string.Format(err, args);
                Error(lo);
            }
            catch (Exception)
            {
                
            }
        }

        /// <summary>
        /// 写普通日志
        /// </summary>
        /// <param name="normalstr">日志文本</param>
        public void Normal(string normalstr)
        {
            try
            {
                Log5Info loginf = GetLogInfo(_sys_normal_log_name);
                if (loginf == null)
#if DEBUG
                    throw new Exception("LogInfo is not found.");
#else
				return;
#endif

                loginf.AddStr(normalstr);
                loginf.CheckSize();
            }
            catch (Exception)
            {
                
            }
        }

        /// <summary>
        /// 写含有参数的普通日志
        /// </summary>
        /// <param name="normalstr">日志文本</param>
        /// <param name="args">参数</param>
        public void Normal(string normalstr, params Object[] args)
        {
            try
            {
                string lo = string.Format(normalstr, args);
                Normal(lo);
            }
            catch (Exception ex)
            {
                
            }
        }

        /// <summary>
        /// 写调试日志
        /// </summary>
        /// <param name="debugstr">调试文本</param>
        public void Debug(string debugstr)
        {
            Log5Info loginf = GetLogInfo(_sys_debug_log_name);
            if (loginf == null)
#if DEBUG
                throw new Exception("LogInfo is not found.");
#else
				return;
#endif

            loginf.AddStr(debugstr);
            loginf.CheckSize();
        }

        /// <summary>
        /// 写含有参数的调试日志
        /// </summary>
        /// <param name="debugstr">调试文本</param>
        /// <param name="args">参数</param>
        public void Debug(string debugstr, params object[] args)
        {
            string lo = string.Format(debugstr, args);
            Debug(lo);
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="logname">已创建的日志名称</param>
        /// <param name="str">日志文本</param>
        public void Log(string logname, string str)
        {

            Log5Info loginf = GetLogInfo(logname);
            if (loginf == null)
#if DEBUG
                throw new Exception("LogInfo is not found.");
#else
				return;
#endif

            loginf.AddStr(str);
            loginf.CheckSize();
        }

        /// <summary>
        /// 写带有参数的日志
        /// </summary>
        /// <param name="logname">已创建的日志名称</param>
        /// <param name="fmt">日志文本</param>
        /// <param name="args">参数</param>
        public void Log(string logname, string fmt, params object[] args)
        {
            string lo = string.Format(fmt, args);
            Log(logname, lo);
        }
        #endregion

        /*---------------------------------------------------
         * 创建各种日志 
         * ------------------------------------------------*/
        #region ------------------创建各种日志
        /// <summary>
        /// 创建日志
        /// </summary>
        /// <param name="logname">日志名称</param>
        /// <param name="filepath">日志路径</param>
        public void CreateLog(string logname, string filepath)
        {
            CreateLog(logname, filepath, ShowLineNo, ShowFile, BufferSize, LogEncoding, Dateformat);
        }

        /// <summary>
        /// 创建日志
        /// </summary>
        /// <param name="logname">日志名称</param>
        /// <param name="filepath">日志路径</param>
        /// <param name="showlineno">是否添加行号</param>
        /// <param name="showfile">是否添加文件名</param>
        public void CreateLog(string logname, string filepath, bool showlineno, ShowFileType showfile)
        {
            CreateLog(logname, filepath, showlineno, showfile, BufferSize, LogEncoding, Dateformat);
        }

        /// <summary>
        /// 创建日志
        /// </summary>
        /// <param name="logname">日志名称</param>
        /// <param name="filepath">日志路径</param>
        /// <param name="showlineno">是否添加行号</param>
        /// <param name="showfile">是否添加文件名</param>
        /// <param name="buffsize">缓存大小</param>
        public void CreateLog(string logname, string filepath, bool showlineno, ShowFileType showfile, int buffsize)
        {
            CreateLog(logname, filepath, showlineno, showfile, buffsize, LogEncoding, Dateformat);
        }

        /// <summary>
        /// 创建日志
        /// </summary>
        /// <param name="logname">日志名称</param>
        /// <param name="filepath">日志路径</param>
        /// <param name="showlineno">是否添加行号</param>
        /// <param name="showfile">是否添加文件名</param>
        /// <param name="dateformat">日期格式</param>
        public void CreateLog(string logname, string filepath, bool showlineno, ShowFileType showfile, string dateformat)
        {
            CreateLog(logname, filepath, showlineno, showfile, BufferSize, LogEncoding, dateformat);
        }

        /// <summary>
        /// 创建日志
        /// </summary>
        /// <param name="logname">日志名称</param>
        /// <param name="filepath">日志路径</param>
        /// <param name="showlineno">是否添加行号</param>
        /// <param name="showfile">是否添加文件名</param>
        /// <param name="enc">日志编码</param>
        public void CreateLog(string logname, string filepath, bool showlineno, ShowFileType showfile, System.Text.Encoding enc)
        {
            CreateLog(logname, filepath, showlineno, showfile, BufferSize, enc, Dateformat);
        }

        /// <summary>
        /// 创建日志
        /// </summary>
        /// <param name="logname">日志名称</param>
        /// <param name="filepath">日志路径</param>
        /// <param name="showlineno">是否添加行号</param>
        /// <param name="showfile">是否添加文件名</param>
        /// <param name="buffsize">缓存大小</param>
        /// <param name="enc">日志编码</param>
        /// <param name="dateformat">日期格式</param>
        public void CreateLog(string logname, string filepath, bool showlineno, ShowFileType showfile, int buffsize, System.Text.Encoding enc, string dateformat)
        {
            Log5Info loginf = new Log5Info(logname, filepath);
            loginf.BufferSize = buffsize;
            loginf.Dateformat = dateformat;
            loginf.LogEncoding = enc;
            loginf.ShowFile = showfile;
            loginf.ShowLineNo = showlineno;
            if (_loghash.Contains(loginf.Logname))
            {
#if DEBUG
                throw new Exception("The log is alreay exist.");
#else
				return;
#endif
            }
            else
                _loghash.Add(loginf.Logname, loginf);


        }
        #endregion


        /*---------------------------------------------------
         * 设置各种参数 
         * ------------------------------------------------*/
        #region ------------------创建各种日志
        /// <summary>
        /// 自动生成文件名
        /// </summary>
        /// <param name="logname">已存在的日志名</param>
        /// <param name="bauto">打开或关闭自动生成</param>
        /// <param name="dir">日志目录</param>
        /// <param name="autoname">文件名格式</param>
        public void SetAutoGenerateFilename(string logname, bool bauto, string dir, string autoname)
        {
            Log5Info loginf = GetLogInfo(logname);
            if (loginf == null)
#if DEBUG
                throw new Exception("LogInfo is not found.");
#else
				return;
#endif
            loginf.AutoGenerateFilename = bauto;
            if (bauto && !string.IsNullOrEmpty(autoname))
            {
                loginf.AutoFileDir = dir;
                loginf.AutofilenameExpr = autoname;
            }
        }

        /// <summary>
        /// 设置日志缓存大小，单位字节
        /// </summary>
        /// <param name="logname">已存在的日志名</param>
        /// <param name="newbuffersize">缓存大小</param>
        public void SetBufferSize(string logname, int newbuffersize)
        {
            Log5Info loginf = GetLogInfo(logname);
            if (loginf == null)
#if DEBUG
                throw new Exception("LogInfo is not found.");
#else
				return;
#endif
            loginf.BufferSize = newbuffersize;
        }

        /// <summary>
        /// 设置日志是否显示行号
        /// </summary>
        /// <param name="logname">已存在的日志名</param>
        /// <param name="bshow">是否显示行号</param>
        public void SetShowLineNumber(string logname, bool bshow)
        {
            Log5Info loginf = GetLogInfo(logname);
            if (loginf == null)
#if DEBUG
                throw new Exception("LogInfo is not found.");
#else
				return;
#endif
            loginf.ShowLineNo = bshow;
        }

        /// <summary>
        /// 设置日志是否显示文件路径
        /// </summary>
        /// <param name="logname">已存在的日志名</param>
        /// <param name="bshow">是否显示路径</param>
        public void SetShowFilePath(string logname, ShowFileType show)
        {
            Log5Info loginf = GetLogInfo(logname);
            if (loginf == null)
#if DEBUG
                throw new Exception("LogInfo is not found.");
#else
				return;
#endif
            loginf.ShowFile = show;
        }

        /// <summary>
        /// 设置日志是否显示方法名
        /// </summary>
        /// <param name="logname">已存在的日志名</param>
        /// <param name="bshow">是否显示方法名</param>
        public void SetShowMethod(string logname, bool bshow)
        {
            Log5Info loginf = GetLogInfo(logname);
            if (loginf == null)
#if DEBUG
                throw new Exception("LogInfo is not found.");
#else
				return;
#endif
            loginf.ShowMethod = bshow;
        }

        /// <summary>
        /// 设置新的日志路径
        /// </summary>
        /// <param name="logname">已存在的日志名</param>
        /// <param name="newfile">新路径</param>
        public void SetLogFile(string logname, string newfile)
        {
            Log5Info loginf = GetLogInfo(logname);
            if (loginf == null)
#if DEBUG
                throw new Exception("LogInfo is not found.");
#else
				return;
#endif
            loginf.Logfile = newfile;
        }

        /// <summary>
        /// 设置日志的文件编码
        /// </summary>
        /// <param name="logname">已存在的日志名</param>
        /// <param name="enc">编码</param>
        public void SetLogEncoding(string logname, System.Text.Encoding enc)
        {
            Log5Info loginf = GetLogInfo(logname);
            if (loginf == null)
#if DEBUG
                throw new Exception("LogInfo is not found.");
#else
				return;
#endif
            loginf.LogEncoding = enc;
        }
        #endregion


        /// <summary>
        /// 获取一个日志实体
        /// </summary>
        /// <param name="logname"></param>
        /// <returns></returns>
        private Log5Info GetLogInfo(string logname)
        {
            return (Log5Info)_loghash[logname];
        }

        private void AddDefaultLogs()
        {
            // 预设三个日志。
            CreateLog(_sys_err_log_name, ErrorFile, true, ShowFileType.none);
            CreateLog(_sys_normal_log_name, NormalFile, false, ShowFileType.none);
            CreateLog(_sys_debug_log_name, DebugFile, true, ShowFileType.filename);
        }

        /// <summary>
        /// IDispose接口
        /// </summary>
        public void Dispose()
        {
            foreach (object key in _loghash.Keys)
            {
                ((Log5Info)_loghash[key]).Dispose();
            }
        }

    }
    class Log5Info : IDisposable
    {
        private string _logfile = "";
        private string _logdir = "";
        private static string _appSiteName = null;
        private static string _logfilepath = "E:\\00_ApiLog\\";
        private string _dateformat = "yyyy-MM-dd HH:mm:ss";
        private System.Text.Encoding _encoding = System.Text.Encoding.Default;

        
        /// <summary>
        /// 日志名称
        /// </summary>
        public string Logname { get; set; }
        /// <summary>
        /// 日志路径
        /// </summary>
        public string Logfile
        {
            get { return _logfile; }
            set
            {
                _logfile = ConvertPath(value);
            }
        }
        /// <summary>
        /// 缓存大小
        /// </summary>
        public int BufferSize { get; set; }
        /// <summary>
        /// 是否显示行号
        /// </summary>
        public bool ShowLineNo { get; set; }
        /// <summary>
        /// 显示文件名，文件路径，或者不显示
        /// </summary>
        public ShowFileType ShowFile { get; set; }
        /// <summary>
        /// 是否显示方法名
        /// </summary>
        public bool ShowMethod { get; set; }
        /// <summary>
        /// 日期格式
        /// </summary>
        public string Dateformat { get { return _dateformat; } set { _dateformat = value; } }

        /// <summary>
        /// 是否自动生成文件名，当为true时，Logfile无效
        /// </summary>
        public bool AutoGenerateFilename { get; set; }

        /// <summary>
        /// 自动生成文件名的格式
        /// </summary>
        public string AutofilenameExpr = "";
        /// <summary>
        /// 自动生成文件名是，文件的目录
        /// </summary>
        public string AutoFileDir
        {
            get { return _logdir; }
            set
            {
                _logdir = ConvertPath(value);
            }

        }

      

        /// <summary>
        /// 日志编码
        /// </summary>
        public System.Text.Encoding LogEncoding { get { return _encoding; } set { _encoding = value; } }

        /// <summary>
        /// 日志缓存
        /// </summary>
        private System.Collections.Generic.Queue<string> logqueue = new System.Collections.Generic.Queue<string>();

        private Object _lock = new Object();
        private long _length = 0;
        private DateTime logdate = DateTime.Now;

        /// <summary>
        /// 构造一个日志
        /// </summary>
        /// <param name="logname">日志名称</param>
        /// <param name="logfile">日志路径</param>
        public Log5Info(string logname, string logfile)
        {
            GetAppSiteName();
            BufferSize = 30 * 1024;
            ShowLineNo = true;
            ShowFile = ShowFileType.none;
            ShowMethod = true;
            Logname = logname;
            Logfile = logfile;
            AutoGenerateFilename = false;

        }
        /// <summary>
        /// 构造一个日志
        /// </summary>
        /// <param name="logname">日志名称</param>
        /// <param name="logdir">日志目录</param>
        /// <param name="filename">日志文件名，不含路径</param>
        public Log5Info(string logname, string logdir, string filename)
        {
            GetAppSiteName();
            BufferSize = 30 * 1024;
            ShowLineNo = true;
            ShowFile = ShowFileType.none;
            Logname = logname;
            Logfile = System.IO.Path.Combine(logdir, filename);
            AutoGenerateFilename = false;
        }
        public void AddStr(string str)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("=============================================================================\r\n");
            try
            {
                System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(ShowLineNo && ShowFile != ShowFileType.none);
                int lineno = 0;
                string file = null;
                string method = null;
                if (st.FrameCount >= 3)
                {
                    System.Diagnostics.StackFrame sf = st.GetFrame(2);
                    lineno = sf.GetFileLineNumber();
                    file = sf.GetFileName();
                    if (ShowMethod)
                        method = sf.GetMethod().Name;
                }
                sb.Append("DateTime:" + DateTime.Now.ToString(Dateformat) + " \r\n");
                sb.AppendFormat("Thread: 线程({0})处理, 当前线程数({1}) \r\n", AppDomain.GetCurrentThreadId(), System.Diagnostics.Process.GetCurrentProcess().Threads.Count);
                if (ShowLineNo && lineno > 0)
                    sb.AppendFormat("lineno:[{0:0000}] \r\n", lineno);
                if (ShowMethod && method != null)
                    sb.AppendFormat("method:[{0}] \r\n", method);
                if (ShowFile != ShowFileType.none && file != null)
                {
                    sb.AppendFormat("ShowFile:[{0}] \r\n", ShowFile == ShowFileType.filename ? System.IO.Path.GetFileName(file) : file);
                }
                sb.Append("------------------------------------------\r\n");
                sb.Append(str +"\r\n");
                sb.Append("=============================================================================\r\n\r\n");
                lock (_lock)
                {
                    if (logqueue.Count == 0)
                        logdate = DateTime.Now;
                    logqueue.Enqueue(sb.ToString());
                    System.Diagnostics.Trace.WriteLine(sb.ToString());
                    System.Threading.Interlocked.Add(ref _length, (long)sb.Length);
                }
            }
            catch
            {
            }

        }
        public void CheckSize()
        {
            if (_length > BufferSize)
            {
                Flush();
            }
        }
        public void Flush()
        {
            if (logqueue.Count == 0)
                return;
            string filename = Logfile;

            if (string.IsNullOrEmpty(filename) || filename.EndsWith("\\"))
            {
                filename = Logname + ".log";
            }

            if (AutoGenerateFilename && !string.IsNullOrEmpty(AutofilenameExpr))
            {
                filename = AutofilenameExpr.Replace("%f", Logname);
                filename = filename.Replace("%y", logdate.Year.ToString());
                filename = filename.Replace("%M", logdate.ToString("MM"));
                filename = filename.Replace("%d", logdate.ToString("dd"));
                filename = filename.Replace("%H", logdate.ToString("HH"));
                filename = filename.Replace("%m", logdate.ToString("mm"));
                filename = filename.Replace("%s", logdate.ToString("ss"));
                string dir = ConvertPath(AutoFileDir);
                filename = System.IO.Path.Combine(dir, filename);
                dir = System.IO.Path.GetDirectoryName(filename);
                if (!PrepareLogDir(dir))
                    throw new System.IO.DriveNotFoundException("The dir is not exist and cannot be created");
            }
            else
            {
                string dir = System.IO.Path.GetDirectoryName(Logfile);
                if (!PrepareLogDir(dir))
                    throw new System.IO.DriveNotFoundException("The dir is not exist and cannot be created");
            }
            lock (_lock)
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filename, true, LogEncoding))
                {
                    while (logqueue.Count != 0)
                    {
                        string logstr = logqueue.Dequeue();
                        if (logstr != null)
                            sw.WriteLine(logstr);
                    }
                    _length = 0;
                    sw.Close();
                }
            }
        }
        public override int GetHashCode()
        {
            return Logname.GetHashCode();
        }
        public static string ConvertPath(string path)
        {
            string syspath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

            if(_logfilepath!="")
            {
                syspath=_logfilepath + _appSiteName+"\\";
            }

            if (string.IsNullOrEmpty(path))	// 默认当前路径
            {
                return syspath;
            }

            path = path.Replace('/', '\\');

            if (path.IndexOf(':') == 1) // 如果是绝对路径，无需转换
                return path;
            return syspath + (path.StartsWith("\\") ? path.Substring(1, path.Length - 1) : path);

        }
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="dir"></param>
        /// <returns>是否成功</returns>
        public static bool PrepareLogDir(string dir)
        {
            if (System.IO.Directory.Exists(dir) == false)
            {
                System.IO.Directory.CreateDirectory(dir);
            }
            return System.IO.Directory.Exists(dir);

        }
        public static string GetAppPath()
        {
            return AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        }

        public static void GetAppSiteName()
        {
            if(string.IsNullOrWhiteSpace(_appSiteName))
            {
                string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                string[] patharr = path.Split('\\');
                foreach (string key in patharr)
                {
                    if(key!="")
                    _appSiteName= key;
                }
            }
        }

        public static string GetLogFilePath()
        {
            string _logfilepaths = "";
            if (string.IsNullOrWhiteSpace(_appSiteName))
            {
                string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                string[] patharr = path.Split('\\');
                foreach (string key in patharr)
                {
                    if (key != "")
                        _appSiteName = key;
                }
            }
            if (_logfilepath != "")
            {
                return _logfilepath + _appSiteName;
            }
            return _logfilepaths;
        }
        public void Dispose()
        {
            Flush();
        }


    }
    public enum ShowFileType
    {
        none = 0,
        filename = 1,
        filepath = 2,
    }

}
