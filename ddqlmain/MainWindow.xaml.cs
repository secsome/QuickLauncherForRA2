using System;
using System.Windows;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace ddqlmain
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (File.Exists(PATH + "\\ddqlconfig.ini") == false)
            {
                MessageBox.Show("无法找到配置文件ddqlconfig.ini！程序将自动关闭。", "致命错误！", MessageBoxButton.OK);
                App.Current.Shutdown();
            }
            CampaignDataInitializer();
        }

        public String PATH = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        public String GameExecutableNames = "Syringe.exe";
        public String ExtraCommandLineParams = "";
        public String[] SideName = new String[4] { "盟军", "苏联", "厄普西隆", "焚风" };
        public String[] Spawn = new String[9]
        {
            ";This file was spawned automatically by Dissolving Dream Quick Launcher by SEC_SOME.",
            "[Settings]",
            "GameSpeed=1",
            "Firestorm=False",
            "CustomLoadScreen=Resources\\l600s01.pcx",
            "IsSinglePlayer=Yes",
            "SidebarHack=False",
            "Side=0",
            "BuildOffAlly=True"
        };

    public void CampaignDataInitializer()
        {
            try
            {
                ObservableCollection<Campaigns> campaigndata = new ObservableCollection<Campaigns>();
                iniclass ddqlconfig = new iniclass(PATH + "\\ddqlconfig.ini");
                int CampaignCount = int.Parse(ddqlconfig.Read("General", "CampaignCount"));
                for (int i = 1; i <= CampaignCount; ++i)
                {
                    String str = ddqlconfig.Read("General", i.ToString());
                    campaigndata.Add(new Campaigns
                    {
                        Name = ddqlconfig.Read(str, "NAME"),
                        ID = ddqlconfig.Read(str, "ID"),
                        Side = SideName[int.Parse(ddqlconfig.Read(str, "SIDE"))],
                        SCENERY = ddqlconfig.Read(str, "SCENERY")
                    });
                }
                CampaignDataGrid.DataContext = campaigndata;
            }
            
            catch (Exception ex)
            {
                MessageBox.Show("启动器调用CampaignDataInitializer时遇到问题：\n\r" + ex.Message, "致命错误", MessageBoxButton.OK);
                App.Current.Shutdown();
            }
        }

        public void ExtraCommandLineParamsChecker()
        {
            try
            {
                if (Set_SpeedControl.IsChecked == true)
                    if (Set_Log.IsChecked == true)
                        ExtraCommandLineParams = " \"gamemd.exe\" -SPAWN -CD -LOG -SPEEDCONTROL %";
                    else
                        ExtraCommandLineParams = " \"gamemd.exe\" -SPAWN -CD -SPEEDCONTROL %";
                else
                if (Set_Log.IsChecked == true)
                    ExtraCommandLineParams = " \"gamemd.exe\" -SPAWN -CD -LOG";
                else
                    ExtraCommandLineParams = " \"gamemd.exe\" -SPAWN -CD";
                return;
            }

            catch (Exception ex)
            {
                MessageBox.Show("启动器调用ExtraCommandLineParamsChecker时遇到问题：\n\r" + ex.Message, "错误", MessageBoxButton.OK);
            }
        }

        public void LaunchChecker()
        {
            try
            {
                StreamWriter streamWriter = new StreamWriter(PATH + "\\spawn.ini");
                for (int i = 0; i < 9; ++i) streamWriter.WriteLine(Spawn[i]);
                streamWriter.Close();
                iniclass spawn = new iniclass(PATH + "\\spawn.ini");
                iniclass ra2mo = new iniclass(PATH + "\\RA2MO.ini");

                if (Diff_Easy.IsChecked == true)
                {
                    spawn.Write("Settings", "DifficultyModeHuman", "0");
                    spawn.Write("Settings", "DifficultyModeComputer", "2");
                    ra2mo.Write("Options", "Difficulty", "0");
                }
                if (Diff_Medium.IsChecked == true)
                {
                    spawn.Write("Settings", "DifficultyModeHuman", "1");
                    spawn.Write("Settings", "DifficultyModeComputer", "1");
                    ra2mo.Write("Options", "Difficulty", "1");
                }
                if (Diff_Hard.IsChecked == true)
                {
                    spawn.Write("Settings", "DifficultyModeHuman", "2");
                    spawn.Write("Settings", "DifficultyModeComputer", "0");
                    ra2mo.Write("Options", "Difficulty", "2");
                }
                Campaigns tmp = (Campaigns)CampaignDataGrid.SelectedItem;
                spawn.Write("Settings", "Scenario", tmp.SCENERY + ".MAP");
                return;
            }

            catch (Exception ex)
            {
                MessageBox.Show("启动器调用LaunchChecker时遇到问题：\n\r" + ex.Message, "错误", MessageBoxButton.OK);
            }
        }

        public bool ProcessLaunch()
        {
            try
            {
                Process mainprocess = new Process();
                mainprocess.StartInfo= new ProcessStartInfo(GameExecutableNames, ExtraCommandLineParams.Trim());
                mainprocess.Start();
                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show("在游戏启动过程中遇到问题，原因可能是：\n\r" + ex.Message, "警告", MessageBoxButton.OK);
            }
            return false;
        }

        private void Button_Launch_Click(object sender, RoutedEventArgs e)
        {
            ExtraCommandLineParamsChecker();
            LaunchChecker();
            bool IsSucceed = ProcessLaunch();
            if (IsSucceed == true) App.Current.Shutdown();
            else MessageBox.Show("有什么错误发生了，请检查您是否已经正确地安装了游戏！", "错误", MessageBoxButton.OK);
            GC.Collect();
            return;
        }

        private void Button_Load_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(PATH + "\\Saved Games") == false) MessageBox.Show("无法读取Saved Games文件夹，请检查是否存在。", "错误", MessageBoxButton.OK);
            else
            {
                SavesWindow saveswindow = new SavesWindow();
                saveswindow.Owner = this;
                saveswindow.ShowDialog();
                GC.Collect();
            }
            return;
        }

        private void Button_Setting_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(PATH + "\\RA2MO.ini") == false)
            {
                MessageBox.Show("RA2MO.ini配置文件不存在，无法调整设置。", "错误", MessageBoxButton.OK);
                return;
            }
            if (File.Exists(PATH + "\\Resources\\Renderers.ini") == false)
            {
                MessageBox.Show("Resources\\Renderers.ini配置文件不存在，无法调整设置。", "错误", MessageBoxButton.OK);
                return;
            }
            SettingWindow settingwindow = new SettingWindow();
            settingwindow.Owner = this;
            settingwindow.ShowDialog();
            GC.Collect();
            return;
        }
    }
}
