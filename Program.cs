using System;
using System.Threading;
using System.Windows.Forms;

namespace UniPinyin
{
    static class Program
    {
        private static Mutex _appMutex = null;

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Đảm bảo chỉ 1 phiên bản UniPinyin chạy cùng một lúc (giống UniKey)
            bool isNewInstance;
            _appMutex = new Mutex(true, "UniPinyin_App_SingleInstance_Mutex_v1", out isNewInstance);

            if (!isNewInstance)
            {
                MessageBox.Show("UniPinyin đang chạy ở khay hệ thống (góc dưới bên phải màn hình)!\nHãy nhấp chuột vào biểu tượng [中] hoặc [EN] ở khay đồng hồ để sử dụng.",
                    "UniPinyin Đang Chạy", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                Settings settings = Settings.Load();
                TrayContext trayContext = new TrayContext(settings);

                // Nếu có cờ --settings hoặc lần đầu chạy, mở bảng điều khiển
                bool openSettings = false;
                if (args != null)
                {
                    foreach (var arg in args)
                    {
                        if (arg.Equals("--settings", StringComparison.OrdinalIgnoreCase) ||
                            arg.Equals("-s", StringComparison.OrdinalIgnoreCase))
                        {
                            openSettings = true;
                            break;
                        }
                    }
                }

                if (openSettings)
                {
                    trayContext.ShowSettings();
                }

                Application.Run(trayContext);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra trong quá trình khởi động UniPinyin:\n" + ex.Message,
                    "Lỗi Khởi Động", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_appMutex != null)
                {
                    _appMutex.ReleaseMutex();
                    _appMutex.Dispose();
                }
            }
        }
    }
}
