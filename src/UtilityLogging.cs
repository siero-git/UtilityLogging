using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityLogging
{
    /// <summary>
    /// 指定のフォルダに日付を元にしたファイル名で出力
    /// </summary>
    public class UtilityLogging : UtilityLoggingBase
    {
        public UtilityLogging(string rootPath)
        {
            _rootPath = rootPath;

            Directory.CreateDirectory(_rootPath);
        }

        protected override void Output(string strLog)
        {
            lock (_saveLocker)
            {
                DateTime nowTime = DateTime.Now;
                string dateText = $"{nowTime:yyyyMMdd}";

                string fileName = string.IsNullOrEmpty(optAppendName) ?
                    $"{dateText}.{optFileExt}" :
                    $"{optAppendName}-{dateText}.{optFileExt}";

                string strPath = Path.Combine(_rootPath, fileName);

                try
                {
                    using (StreamWriter sw = new StreamWriter(strPath, true, optEncoding))
                    {
                        //時間情報を付与して出力テキストを生成
                        string strOut = $"{strLog}";

                        sw.WriteLine(strOut);
                    }
                }
                catch(Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }

            //Taskの結果を破棄して実行 => 処理を待機しない
            _ = DeleteLog();
        }

        protected override Task DeleteLog()
        {
            DateOnly nowDate = DateOnly.FromDateTime(DateTime.Now);

            //日付に差異がない場合は処理しない
            if (nowDate == _lastSaveDate) return Task.CompletedTask;

            lock (_delLocker)
            {
                //指定日数分マイナスした値を取得
                DateOnly delDate = nowDate.AddDays(-optDeleteSpan);

                //フォルダ内のファイルを取得
                DirectoryInfo dirInfo = new DirectoryInfo(_rootPath);
                FileInfo[] fileInfos = dirInfo.GetFiles();

                //最終更新日が指定日以前のファイルを列挙
                IEnumerable<FileInfo> delInfos = fileInfos.Where(x => DateOnly.FromDateTime(x.LastWriteTime) <= delDate);

                //対象のファイルを削除
                foreach (FileInfo info in delInfos)
                {
                    try
                    {
                        File.Delete(info.FullName);
                    }
                    catch (Exception ex)
                    {
                        Output($"[UtilLog] ファイル削除例外 msg:{ex.Message}");
                    }
                }
                _lastSaveDate = DateOnly.FromDateTime(DateTime.Now);
            }

            return Task.CompletedTask;
        }
    }
}
