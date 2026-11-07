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

            bool isNewInstance;
            _appMutex = new Mutex(true, "UniPinyin_EN_SingleInstance_Mutex_v1", out isNewInstance);

            if (!isNewInstance)
            {
                MessageBox.Show(
                    "UniPinyin is already running in the system tray (bottom-right corner of your screen)!\nClick the [中] or [EN] icon near the clock to use it.",
                    "UniPinyin Already Running",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            try
            {
                Settings settings = Settings.Load();
                TrayContext trayContext = new TrayContext(settings);

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
                MessageBox.Show(
                    "An error occurred while launching UniPinyin:\n" + ex.Message,
                    "Startup Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
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
