using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System;

namespace ddqlmain
{
    class iniclass
    {
        /// <summary>
        /// 配置文件 .ini操作类
        /// </summary>
        private class IniFileUtils
        {
            /// <summary>
            /// 写入INI文件
            /// </summary>
            /// <param name="section">节点名称[如TypeName]</param>
            /// <param name="key">键</param>
            /// <param name="val">值</param>
            /// <param name="filepath">文件路径</param>
            /// <returns></returns>
            [DllImport("kernel32")]
            private static extern long WritePrivateProfileString(string section, string key, string val, string filepath);
            /// <summary>
            /// 读取INI文件
            /// </summary>
            /// <param name="section">节点名称</param>
            /// <param name="key">键</param>
            /// <param name="def">值</param>
            /// <param name="retval">stringbulider对象</param>
            /// <param name="size">字节大小</param>
            /// <param name="filePath">文件路径</param>
            /// <returns></returns>
            [DllImport("kernel32")]
            private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retval, int size, string filePath);

            /// <summary>
            /// 写入或更新.ini配置文件属性值
            /// </summary>
            /// <param name="section">区域（节点）</param>
            /// <param name="key">key键属性名称</param>
            /// <param name="value">key键对应属性值param>
            /// <param name="path">.ini文件所在路径</param>
            public static void WriteContentValue(string section, string key, string value, string path)
            {
                //判断文件是或否存在
                if (File.Exists(path))
                {
                    WritePrivateProfileString(section, key, value, path);
                }
            }

            /// <summary>
            /// 读取.ini配置文件属性值
            /// </summary>
            /// <param name="Section">区域(节点)</param>
            /// <param name="key">key键属性名称</param>
            /// <param name="path">.ini文件所在路径</param>
            /// <returns></returns>
            public static string ReadContentValue(string Section, string key, string path)
            {
                StringBuilder temp = new StringBuilder(1024);
                //判断文件是或否存在
                if (File.Exists(path))
                {
                    GetPrivateProfileString(Section, key, "", temp, 1024, path);
                }
                return temp.ToString();

            }
        }
        private string filepath;
        public iniclass(String _path)
        {
            filepath = _path;
        }

        public string Read(string section,string key)
        {
            return IniFileUtils.ReadContentValue(section, key, filepath);
        }

        public void Write(string section,string key,string content)
        {
            IniFileUtils.WriteContentValue(section, key, content, filepath);
            return;
        }
    }
}
