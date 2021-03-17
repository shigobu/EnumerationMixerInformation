using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;

namespace MixerInformation
{
    class Program
    {
        static void Main(string[] args)
        {
            MMDevice device = null;
            try
            {
                //ウマ娘セッションの検索
                using (MMDeviceEnumerator DevEnum = new MMDeviceEnumerator())
                {
                    device = DevEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                }
                //master情報
                Console.WriteLine("FriendlyName:" + device.FriendlyName);
                Console.WriteLine("DeviceFriendlyName:" + device.DeviceFriendlyName);
                Console.WriteLine("IconPath:" + Environment.ExpandEnvironmentVariables(device.IconPath));
                Console.WriteLine("");

                AudioSessionManager sessionManager = device.AudioSessionManager;
                var sessions = sessionManager.Sessions;
                for (int j = 0; j < sessions.Count; j++)
                {
                    Console.WriteLine("DisplayName:" + sessions[j].DisplayName);
                    Console.WriteLine("IconPath:" + sessions[j].IconPath);
                    Console.WriteLine("");
                }
            }
            finally
            {
                if (device != null)
                {
                    device.Dispose();
                }
            }

            Console.Read();

        }
    }

    /// <summary>
    /// ライブラリを読み込み、開放する方法を提供します。
    /// </summary>
    class NativeLibraryOperation : IDisposable
    {
        /// <summary>
        /// ライブラリ名
        /// </summary>
        private string LibraryName { get; }

        /// <summary>
        /// ライブラリハンドル
        /// </summary>
        private IntPtr HModule { get; }

        /// <summary>
        /// 指定のライブラリをロードし、インスタンスの初期化を行います。
        /// </summary>
        /// <param name="libraryName"></param>
        NativeLibraryOperation(string libraryName)
        {
            LibraryName = libraryName;
            HModule = NativeMethods.LoadLibrary(libraryName);
        }

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)。
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。

                NativeMethods.FreeLibrary(HModule);

                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        ~NativeLibraryOperation()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(false);
        }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            GC.SuppressFinalize(this);
        }
        #endregion
    }

    class NativeMethods
    {

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int LoadString(IntPtr hInstance, uint uID, StringBuilder lpBuffer, int nBufferMax);
    }
}
