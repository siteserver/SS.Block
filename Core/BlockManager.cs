using System;
using System.Text.RegularExpressions;
using MaxMind.GeoIP2;
using SiteServer.Plugin;

namespace SS.Block.Core
{
    public sealed class BlockManager
    {
        private static readonly Lazy<BlockManager> Lazy = new Lazy<BlockManager>(() => new BlockManager());

        public static BlockManager Instance => Lazy.Value;

        private readonly DatabaseReader _reader;

        private BlockManager()
        {
            var filePath = Context.PluginApi.GetPluginPath(Utils.PluginId,
                "assets/GeoLite2-Country_20190423/GeoLite2-Country.mmdb");
            _reader = new DatabaseReader(filePath);
        }

        public int GetGeoNameId(string ipAddress)
        {
            if (IsLocalIp(ipAddress)) return Utils.LocalGeoNameId;
            return _reader.TryCountry(ipAddress, out var response) ? response.Country.GeoNameId ?? 0 : 0;
        }

        private static bool IsLocalIp(string ipAddress)
        {
            return ipAddress == "127.0.0.1" || Regex.IsMatch(ipAddress,
                @"(^192\.168\.([0-9]|[0-9][0-9]|[0-2][0-5][0-5])\.([0-9]|[0-9][0-9]|[0-2][0-5][0-5])$)|(^172\.([1][6-9]|[2][0-9]|[3][0-1])\.([0-9]|[0-9][0-9]|[0-2][0-5][0-5])\.([0-9]|[0-9][0-9]|[0-2][0-5][0-5])$)|(^10\.([0-9]|[0-9][0-9]|[0-2][0-5][0-5])\.([0-9]|[0-9][0-9]|[0-2][0-5][0-5])\.([0-9]|[0-9][0-9]|[0-2][0-5][0-5])$)");
        }

        public bool IsAllowed(int siteId, ConfigInfo config, AreaInfo areaInfo, string sessionId)
        {
            if (!config.IsBlock) return true;

            if (!string.IsNullOrEmpty(sessionId))
            {
                if (CacheUtils.Exists(sessionId))
                {
                    return true;
                }
            }
            
            var isMatch = false;
            if (areaInfo != null)
            {
                if (config.BlockAreas != null && config.BlockAreas.Contains(areaInfo.GeoNameId))
                {
                    isMatch = true;
                }
            }

            bool isAllowed;
            if (config.IsBlockAll)
            {
                isAllowed = isMatch;
            }
            else
            {
                isAllowed = !isMatch;
            }

            if (!isAllowed)
            {
                Main.BlockRepository.AddBlock(siteId);
            }

            return isAllowed;
        }
    }
}
