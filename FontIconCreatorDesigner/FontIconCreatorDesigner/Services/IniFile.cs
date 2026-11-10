using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

public class IniFile(string iniPath = null)
{
    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
    private readonly string _path = iniPath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.ini");
    public void Write(string section, string key, string value) => WritePrivateProfileString(section, key, value, _path);
    public string Read(string section, string key, string defaultValue = "")
    {
        var sb = new StringBuilder(500);
        GetPrivateProfileString(section, key, defaultValue, sb, sb.Capacity, _path);
        return sb.ToString();
    }
}
