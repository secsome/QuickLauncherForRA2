using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ddqlmain
{
    /// <summary>
    /// SettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();
            InfoInitializer();
            SettingInitializer();
        }

        public String PATH = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        public String[] info = new String[]{
            "【基本信息】",
            "本启动器由SEC-SOME编写，仅供梦醒时分系列使用。其中",
            "部分代码参考了DTA Client，由于开发者水平有限，本启动",
            "器可能存在BUG，若发现请及时汇报！",
            "【常见问题解答】",
            "Q:窗口化运行时遇到需要16位色盘",
            "A:在gamemd.exe的属性中设置兼容性，启用16位色即可",
            "Q:FATAL string maneger failed toinitilaized properly.",
            "A.确保游戏路径中无中文名称，并设置gamemd.exe兼容性",
            "为Windows XP(SP2)",
            "Q:Could not run executable.请求的操作需要提升",
            "A:给本启动器及Syringe.exe设置管理员权限",
            "如果发现更多问题欢迎通过邮箱联系！",
            "联系邮箱：3179369262@qq.com",
            "bilibili:https://space.bilibili.com/143748001"
        };
        public bool[] SettingList_Video = new bool[3];
        public bool[] SettingList_Compatibility = new bool[3];
        public String[] SettingString_Video = new String[3]
        {
            "Video.Windowed",
            "NoWindowFrame",
            "VideoBackBuffer"
        };
        public String[] SettingString_Compatibility = new String[3]
        {
            "DisableDDrawAcceleration",
            "EnableWindows8Fix",
            "EnableCursorLock"
        };
        public List<String> Renderers = new List<String>();
        public int totalRendererNums = 0;
        public int ScreenWidth = (int)SystemParameters.PrimaryScreenWidth, ScreenHeight = (int)SystemParameters.PrimaryScreenHeight;
        ScreenResolution ScreenResolutions = new ScreenResolution();
        public Tuple<int, int>[] StdandardScreenResolutions = new Tuple<int, int>[16]
        {
            new Tuple<int, int>(800,600),
            new Tuple<int, int>(1024,768),
            new Tuple<int, int>(1152,864),
            new Tuple<int, int>(1280,600),
            new Tuple<int, int>(1280,720),
            new Tuple<int, int>(1280,768),
            new Tuple<int, int>(1280,800),
            new Tuple<int, int>(1280,960),
            new Tuple<int, int>(1280,1024),
            new Tuple<int, int>(1360,768),
            new Tuple<int, int>(1366,768),
            new Tuple<int, int>(1400,1050),
            new Tuple<int, int>(1440,900),
            new Tuple<int, int>(1600,900),
            new Tuple<int, int>(1680,1050),
            new Tuple<int, int>(1920,1080)
        };


        public void InfoInitializer()//There shouldn't have bugs existed
        {
            String Info = String.Empty;
            foreach (String str in info) Info += (str + '\n');
            Info_Text.Content = Info;
            return;
        }

        public bool SettingChecker(String str)//This too
        {
            return str == "true" || str == "True";
        }

        public String SettingWriterVideo(bool? bol)//This tooo
        {
            if (bol == true) return "True";
            else return "False";
        }

        public String SettingWriterCompatibility(bool? bol)//This toooo
        {
            if (bol == true) return "true";
            else return "false";
        }

        public void WindowedFrameChecker()//Also for this one
        {
            if (Set_Windowed.IsChecked == true) Set_Windowed_NoFrame.IsEnabled = true;
            else
            {
                Set_Windowed_NoFrame.IsChecked = false;
                Set_Windowed_NoFrame.IsEnabled = false;
            }
            return;
        }

        public void SettingInitializer()
        {
            try
            {
                iniclass ra2mo = new iniclass(PATH + "\\RA2MO.ini");
                iniclass ddqlconfig = new iniclass(PATH + "\\ddqlconfig.ini");
                iniclass renderer = new iniclass(PATH + "\\Resources\\Renderers.ini");

                //read RA2MO.ini
                for (int i = 0; i < 3; ++i)
                {
                    String str = ra2mo.Read("Video", SettingString_Video[i]);
                    SettingList_Video[i] = SettingChecker(str);
                }
                for (int i = 0; i < 3; ++i)
                {
                    String str = ra2mo.Read("Compatibility", SettingString_Compatibility[i]);
                    SettingList_Compatibility[i] = SettingChecker(str);
                }
                Set_Windowed.IsChecked = SettingList_Video[0];
                Set_Windowed_NoFrame.IsChecked = SettingList_Video[1];
                Set_BackBuffer.IsChecked = SettingList_Video[2];
                Set_DDrawAccelerateion.IsChecked = SettingList_Compatibility[0];
                Set_Windows8.IsChecked = SettingList_Compatibility[1];
                Set_CursorLock.IsChecked = SettingList_Compatibility[2];
                if (!SettingList_Video[0] && SettingList_Video[1])
                {
                    ra2mo.Write("Video", SettingString_Video[1], "False");
                    SettingList_Video[1] = false;
                    SettingString_Video[1] = "False";
                    Set_Windowed_NoFrame.IsChecked = false;
                }
                WindowedFrameChecker();

                //read Renderers.ini
                String RendererNow = ra2mo.Read("Compatibility", "Renderer");
                String s = renderer.Read("Renderers", totalRendererNums.ToString());
                while (s != String.Empty)
                {
                    Renderers.Add(s);
                    if (s == RendererNow) ComboBox_Renderers.SelectedIndex = totalRendererNums;
                    ++totalRendererNums;
                    s = renderer.Read("Renderers", totalRendererNums.ToString());
                }
                ComboBox_Renderers.ItemsSource = Renderers;

                //read Screen resolutions
                ScreenResolution_Custom_Width.Text = ddqlconfig.Read("Settings", "ScreenResolutionCustomWidth");
                ScreenResolution_Custom_Height.Text = ddqlconfig.Read("Settings", "ScreenResolutionCustomHeight");

                for (int i = 0; i < 16; ++i)
                    if (StdandardScreenResolutions[i].Item1 <= ScreenWidth && StdandardScreenResolutions[i].Item2 <= ScreenHeight)
                    {
                        ScreenResolutions.Item1.Add(StdandardScreenResolutions[i]);
                        ScreenResolutions.Item2.Add(StdandardScreenResolutions[i].Item1.ToString() + 'X' + StdandardScreenResolutions[i].Item2.ToString());
                    }
                ComboBox_ScreenResolution.ItemsSource = ScreenResolutions.Item2;
                ComboBox_ScreenResolution.SelectedIndex = int.Parse(ddqlconfig.Read("Settings", "ScreenResolutionIndex"));

                if (ddqlconfig.Read("Settings", "ScreenResolutionMode") == "0") ScreenResolution_Standard.IsChecked = true;
                else ScreenResolution_Custom.IsChecked = true;

                return;
            }

            catch (Exception ex)
            {
                MessageBox.Show("启动器调用SettingInitializer时遇到问题：\n\r" + ex.Message, "错误", MessageBoxButton.OK);
                this.Close();
            }
        }

        private void ScreenResolution_Custom_Checked(object sender, RoutedEventArgs e)
        {
            ComboBox_ScreenResolution.IsEnabled = false;
            ScreenResolution_Custom_Width.IsEnabled = true;
            ScreenResolution_Custom_Height.IsEnabled = true;
            ScreenResolution_Custom_X.IsEnabled = true;
            return;
        }

        private void ScreenResolution_Standard_Checked(object sender, RoutedEventArgs e)
        {
            ComboBox_ScreenResolution.IsEnabled = true;
            ScreenResolution_Custom_Width.IsEnabled = false;
            ScreenResolution_Custom_Height.IsEnabled = false;
            ScreenResolution_Custom_X.IsEnabled = false;
            return;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            return;
        }

        private void Button_Confirm_Click(object sender, RoutedEventArgs e)//I don't think a button have any problem but a debug is needed
        {
            try
            {
                ScreenResolution_Custom_Width.Text = ScreenResolution_Custom_Width.Text.Trim();
                ScreenResolution_Custom_Height.Text = ScreenResolution_Custom_Height.Text.Trim();
                iniclass ra2mo = new iniclass(PATH + "\\RA2MO.ini");
                iniclass ddqlconfig = new iniclass(PATH + "\\ddqlconfig.ini");
                ra2mo.Write("Video", SettingString_Video[0], SettingWriterVideo(Set_Windowed.IsChecked));
                ra2mo.Write("Video", SettingString_Video[1], SettingWriterVideo(Set_Windowed_NoFrame.IsChecked));
                ra2mo.Write("Video", SettingString_Video[2], SettingWriterVideo(Set_BackBuffer.IsChecked));
                if (ScreenResolution_Standard.IsChecked == true)
                {
                    ddqlconfig.Write("Settings", "ScreenResolutionMode", 0.ToString());
                    ra2mo.Write("Video", "ScreenWidth", ScreenResolutions.Item1[ComboBox_ScreenResolution.SelectedIndex].Item1.ToString());
                    ra2mo.Write("Video", "ScreenHeight", ScreenResolutions.Item1[ComboBox_ScreenResolution.SelectedIndex].Item2.ToString());
                }
                else
                {
                    ddqlconfig.Write("Settings", "ScreenResolutionMode", 1.ToString());
                    ra2mo.Write("Video", "ScreenWidth", ScreenResolution_Custom_Width.Text);
                    ra2mo.Write("Video", "ScreenHeight", ScreenResolution_Custom_Height.Text);
                }
                ra2mo.Write("Compatibility", SettingString_Compatibility[0], SettingWriterCompatibility(Set_DDrawAccelerateion.IsChecked));
                ra2mo.Write("Compatibility", SettingString_Compatibility[1], SettingWriterCompatibility(Set_Windows8.IsChecked));
                ra2mo.Write("Compatibility", SettingString_Compatibility[2], SettingWriterCompatibility(Set_CursorLock.IsChecked));
                ra2mo.Write("Compatibility", "Renderer", Renderers[ComboBox_Renderers.SelectedIndex]);
                ddqlconfig.Write("Settings", "ScreenResolutionCustomWidth", ScreenResolution_Custom_Width.Text);
                ddqlconfig.Write("Settings", "ScreenResolutionCustomHeight", ScreenResolution_Custom_Height.Text);
                ddqlconfig.Write("Settings", "ScreenResolutionIndex", ComboBox_ScreenResolution.SelectedIndex.ToString());
                this.Close();
                return;
            }

            catch (Exception ex)
            {
                MessageBox.Show("启动器调用Button_Confirm_Click时遇到问题：\n\r" + ex.Message, "错误", MessageBoxButton.OK);
            }
        }

        private void Set_Windowed_Click(object sender, RoutedEventArgs e)
        {
            WindowedFrameChecker();
            return;
        }
        private void Set_Windowed_NoFrame_Click(object sender, RoutedEventArgs e)
        {
            WindowedFrameChecker();
            return;
        }

        private void TextBoxNumberOnly(object sender,TextCompositionEventArgs e)//Number input ONLY
        {
            Regex re = new Regex("[^0-9]+");
            e.Handled = re.IsMatch(e.Text);
            return;
        }
    }
}
