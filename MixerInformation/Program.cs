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

    class NativeMethods
    {

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int LoadString(IntPtr hInstance, uint uID, StringBuilder lpBuffer, int nBufferMax);
    }
}
