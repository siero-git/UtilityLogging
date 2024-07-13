using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityLogging
{
    public abstract class UtilityLoggingBase
    {
        #region 継承可能プロパティ
        /// <summary>
        /// 排他ロック用オブジェクト
        /// </summary>
        protected object _saveLocker = new object();
        protected object _delLocker = new object();
        /// <summary>
        /// 最終保存日時
        /// </summary>
        protected DateOnly _lastSaveDate = DateOnly.FromDateTime(DateTime.Now);

        /// <summary>
        /// ログ出力ルートパス
        /// </summary>
        protected string _rootPath = string.Empty;
        #endregion

        #region 公開プロパティ
        /// <summary>
        /// ログ種別
        /// 例:logtype-xxx.yyy
        /// </summary>
        public string optAppendName = string.Empty;
        public int optDeleteSpan { get; set; } = 30;
        public Encoding optEncoding { get; set; } = Encoding.UTF8;
        public string optFileExt { get; set; } = "txt";
        #endregion


        #region 公開メソッド
        /// <summary>
        /// ログ出力処理
        /// </summary>
        /// <param name="logText"></param>
        public void SetLog(string logText)
        {
            Output(logText);
        }

        /// <summary>
        /// 日付情報を付加したログ出力処理
        /// </summary>
        /// <param name="logText"></param>
        public void SetLogAddDateTime(string logText)
        {
            Output($"[{DateTime.Now:HH:mm:ss.fff}],{logText}");
        }

        #endregion

        /// <summary>
        /// 出力処理
        /// </summary>
        /// <param name="logText"></param>
        protected abstract void Output(string logText);


        /// <summary>
        /// 削除処理
        /// </summary>
        protected abstract Task DeleteLog();
    }
}
