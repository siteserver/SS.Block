using System;
using System.Text.RegularExpressions;
using System.Web;

namespace SS.Block.Core
{
    public static class Utils
    {
        public const string PluginId = "SS.Block";

        public const int LocalGeoNameId = 10000;

        public const string LocalAreaEn = "Local IP";

        public const string LocalAreaCn = "内网地址";

        public static bool EqualsIgnoreCase(string a, string b)
        {
            if (a == b) return true;
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return false;
            return string.Equals(a.Trim().ToLower(), b.Trim().ToLower());
        }

        public static int ToInt(string intStr, int defaultValue = 0)
        {
            if (!int.TryParse(intStr?.Trim(), out var i))
            {
                i = defaultValue;
            }
            return i;
        }

        private static bool IsIpAddress(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        public static string GetIpAddress()
        {
            var result = string.Empty;

            try
            {
                result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!string.IsNullOrEmpty(result))
                {
                    if (result.IndexOf(".", StringComparison.Ordinal) == -1)
                        result = null;
                    else
                    {
                        if (result.IndexOf(",", StringComparison.Ordinal) != -1)
                        {
                            result = result.Replace("  ", "").Replace("'", "");
                            var temporary = result.Split(",;".ToCharArray());
                            foreach (var t in temporary)
                            {
                                if (IsIpAddress(t) && t.Substring(0, 3) != "10." && t.Substring(0, 7) != "192.168" && t.Substring(0, 7) != "172.16.")
                                {
                                    result = t;
                                }
                            }
                            var str = result.Split(',');
                            if (str.Length > 0)
                                result = str[0].Trim();
                        }
                        else if (IsIpAddress(result))
                            return result;
                    }
                }

                if (string.IsNullOrEmpty(result))
                    result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                if (string.IsNullOrEmpty(result))
                    result = HttpContext.Current.Request.UserHostAddress;
                if (string.IsNullOrEmpty(result))
                    result = "127.0.0.1";

                if (result == "::1")
                {
                    result = "127.0.0.1";
                }
            }
            catch
            {
                // ignored
            }

            return result;
        }
    }
}
