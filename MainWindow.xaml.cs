using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using Microsoft.Win32;
using ProcessMGR;
using System.Windows.Threading;
using Path = System.IO.Path;

namespace ModInjector
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public DispatcherTimer timer_neWindow = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(100) };
        public DispatcherTimer timer_injectMod = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(100) };
        public String neteaseDownloadpath;
        public String ahpxModPath;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Netease\MCLauncher");
            neteaseDownloadpath = registryKey.GetValue("DownloadPath").ToString();
            ahpxModPath = neteaseDownloadpath + @"\AHpx";
            Directory.CreateDirectory(ahpxModPath);

            foreach (var item in Directory.GetFiles(ahpxModPath))
            {
                ListBox_Modlist.Items.Add(item);
            }

            timer_neWindow.Tick += (Object,RoutedEventArgs) => Timer_Tick_NeWindow();
            timer_injectMod.Tick += (Object, RoutedEventArgs) => Timer_Tick_ModInject();
            timer_neWindow.Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Core.KillProcess("javaw");
        }

        private void Timer_Tick_NeWindow()
        {
            if (Core.IsWindowExist("《我的世界》-游戏启动"))
            {
                Console.WriteLine("Launching window exist");
                timer_injectMod.Start();
            }
        }
        private void Timer_Tick_ModInject()
        {
            if (timer_neWindow.IsEnabled)
            {
                timer_neWindow.Stop();
            }
            if (!Core.IsWindowExist("《我的世界》-游戏启动"))
            {
                try
                {
                    foreach (var item in Directory.GetFiles(ahpxModPath))
                    {
                        Console.WriteLine(item + " copied to " + neteaseDownloadpath + @"\Game\.minecraft\mods\" + Path.GetFileName(item));
                        File.Copy(item, neteaseDownloadpath + @"\Game\.minecraft\mods\" + Path.GetFileName(item));
                    }
                    
                    timer_injectMod.Stop();
                }catch (Exception){}
            }
        }


        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox.IsChecked.Value)
            {
                timer_neWindow.Start();
                timer_injectMod.Stop();
            }
            else
            {
                timer_neWindow.Stop();
                timer_injectMod.Stop();
            }
        }
    }
}
