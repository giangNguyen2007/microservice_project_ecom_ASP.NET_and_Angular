using System.Text;

namespace SharedLibrary.JwtSetting;

public static class JwtSetting
{
    public static byte[] Key = Encoding.UTF8.GetBytes("096c0a72c31f9a2d65126d8e8a401a2ab2f2e21d0a282a6ffe6642bbef65ffd9");
    public static string Issuer = "MyApp";
    public static string Audience ="MyAppUsers";
}