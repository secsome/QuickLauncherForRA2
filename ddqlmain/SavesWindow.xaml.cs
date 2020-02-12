using System;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace ddqlmain
{
    /// <summary>
    /// SavesWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SavesWindow : Window
    {
        public SavesWindow()
        {
            InitializeComponent();
            SavesDataInitializer();
        }

        public String PATH = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        public String GameExecutableNames = "Syringe.exe";
        public String ExtraCommandLineParams = "";
        public String SpawnMap = "[Map]\nSize=0,0,50,50\nLocalSize=0,0,50,50";
        public String[] Spawn = new String[8]
        {
            ";This file was spawned automatically by Dissolving Dream Quick Launcher by SEC_SOME.",
            "[Settings]",
            "Scenario=spawnmap.ini",
            "SaveGameName=",
            "LoadSaveGame=Yes",
            "SidebarHack=False",
            "Firestorm=No",
            "GameSpeed=6"
        };

        public void SavesDataInitializer()
        {
            try
            {
                ObservableCollection<Saves> savesdata = new ObservableCollection<Saves>();
                DirectoryInfo folder = new DirectoryInfo(PATH + "\\Saved Games");
                FileInfo[] fileInfos = folder.GetFiles("*.SAV");
                foreach (FileInfo file in fileInfos)
                {
                    BinaryReader br = new BinaryReader(File.Open(PATH + "\\Saved Games\\" + file.Name, FileMode.Open, FileAccess.Read));
                    br.BaseStream.Position = 2256;
                    String saveGameName = String.Empty;
                    bool IsLastByteZero = false;
                    while (true)
                    {
                        byte characterByte = br.ReadByte();
                        if (characterByte == 0)
                        {
                            if (IsLastByteZero) break;
                            IsLastByteZero = true;
                        }
                        else
                        {
                            IsLastByteZero = false;
                            char character = Convert.ToChar(characterByte);
                            saveGameName += character;
                        }
                    }
                    br.Close();
                    savesdata.Add(new Saves
                    {
                        Name = file.Name,
                        UIName = saveGameName,
                        Time = file.LastWriteTime.ToString()
                    });
                }
                savesdata = new ObservableCollection<Saves>(savesdata.OrderByDescending(item => item.Time));
                SavesDataGrid.DataContext = savesdata;
            }

            catch (Exception ex)
            {
                MessageBox.Show("启动器调用SavesDataInitializer时遇到问题：\n\r" + ex.Message, "错误", MessageBoxButton.OK);
                this.Close();
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

        public bool SpawnFilesWriter()
        {
            try
            {
                StreamWriter spawnmap = new StreamWriter(PATH + "\\spawnmap.ini");
                spawnmap.Write(SpawnMap);
                spawnmap.Close();
                StreamWriter spawn = new StreamWriter(PATH + "\\spawn.ini");
                spawn.WriteLine(Spawn[0]);
                spawn.WriteLine(Spawn[1]);
                spawn.WriteLine(Spawn[2]);
                spawn.Write(Spawn[3]);
                Saves tmp = (Saves)SavesDataGrid.SelectedItem;
                spawn.WriteLine(tmp.Name);
                spawn.WriteLine(Spawn[4]);
                spawn.WriteLine(Spawn[5]);
                spawn.WriteLine(Spawn[6]);
                spawn.WriteLine(Spawn[7]);
                spawn.Close();
                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show("在编辑文件时出现错误，原因可能是：\n" + ex.Message, "错误", MessageBoxButton.OK);
                return false;
            }
        }

        public bool ProcessLaunch()
        {
            try
            {
                Process mainprocess = new Process();
                mainprocess.StartInfo = new ProcessStartInfo(GameExecutableNames, ExtraCommandLineParams.Trim());
                mainprocess.Start();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("在游戏启动过程中遇到问题，原因可能是：\n\r" + ex.Message, "警告", MessageBoxButton.OK);
            }
            return false;
        }

        private void Button_Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Load_Click(object sender, RoutedEventArgs e)
        {
            ExtraCommandLineParamsChecker();
            bool IsFileWritten=SpawnFilesWriter();
            if (IsFileWritten == false) return;
            bool IsSucceed = ProcessLaunch();
            if (IsSucceed == true) App.Current.Shutdown();
            else MessageBox.Show("有什么错误发生了，请检查您是否已经正确地安装了游戏！", "错误", MessageBoxButton.OK);
            GC.Collect();
            return;
        }
    }
}
