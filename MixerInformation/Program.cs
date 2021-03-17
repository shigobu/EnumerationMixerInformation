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
                Console.WriteLine("IconPathRaw:" + Environment.ExpandEnvironmentVariables(device.IconPath));

                //アイコンパス文字列の取得
                string[] iconPathSplited = device.IconPath.Split(',');
                if (iconPathSplited.Length >= 2)
                {
                    string expandPath = Environment.ExpandEnvironmentVariables(iconPathSplited.First());
                    int IDraw = int.Parse(iconPathSplited.Last());
                    uint ID = (uint)Math.Abs(IDraw);
                    Console.WriteLine("IconPath:" + GetStringFromDll(expandPath, ID));
                    int err = Marshal.GetLastWin32Error();
                }

                Console.WriteLine("");

                AudioSessionManager sessionManager = device.AudioSessionManager;
                var sessions = sessionManager.Sessions;
                for (int j = 0; j < sessions.Count; j++)
                {
                    if (sessions[j].DisplayName.First() == '@')
                    {
                        string[] displayNameSplited = sessions[j].DisplayName.Split(',');
                        string expandPath = Environment.ExpandEnvironmentVariables(displayNameSplited.First().Substring(1));
                        int IDraw = int.Parse(displayNameSplited.Last());
                        uint ID = (uint)Math.Abs(IDraw);
                        Console.WriteLine("DisplayName:" + GetStringFromDll(expandPath, ID));
                        int err = Marshal.GetLastWin32Error();
                    }
                    else
                    {
                        Console.WriteLine("DisplayName:" + sessions[j].DisplayName);
                    }

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

        /// <summary>
        /// DLLとリソースIDから、文字列を取得します。
        /// </summary>
        /// <param name="dllName"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        static string GetStringFromDll(string dllName, uint ID)
        {
            string temp = System.IO.Path.GetFileName(dllName);
            using(NativeLibraryOperation nativeLibrary = new NativeLibraryOperation(dllName))
            {
                StringBuilder stringBuilder = new StringBuilder(1024);
                int err = NativeMethods.LoadString(nativeLibrary.ModuleHandle, ID, stringBuilder, stringBuilder.Capacity);
                return stringBuilder.ToString();
            }
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
        public IntPtr ModuleHandle { get; }

        /// <summary>
        /// 指定のライブラリをロードし、インスタンスの初期化を行います。
        /// </summary>
        /// <param name="libraryName"></param>
        public NativeLibraryOperation(string libraryName)
        {
            LibraryName = libraryName;
            ModuleHandle = NativeMethods.LoadLibraryEx(libraryName.ToLower(), IntPtr.Zero, NativeMethods.LoadLibraryExFlags.DONT_RESOLVE_DLL_REFERENCES | NativeMethods.LoadLibraryExFlags.LOAD_LIBRARY_AS_DATAFILE);

            int err = Marshal.GetLastWin32Error();

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

                NativeMethods.FreeLibrary(ModuleHandle);

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
        [Flags]
        public enum LoadLibraryExFlags : UInt32
        {
            DONT_RESOLVE_DLL_REFERENCES = 0x0000001,
            LOAD_LIBRARY_AS_DATAFILE = 0x00000002
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, LoadLibraryExFlags dwFlags);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int LoadString(IntPtr hInstance, uint uID, StringBuilder lpBuffer, int nBufferMax);
    }
}
