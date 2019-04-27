using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SiteServer.Plugin;

namespace SS.Block.Core
{
    public sealed class AreaManager
    {
        private static readonly Lazy<AreaManager> Lazy = new Lazy<AreaManager>(() => new AreaManager());

        public static AreaManager Instance => Lazy.Value;

        private readonly List<AreaInfo> _areaList = new List<AreaInfo>();

        public List<AreaInfo> AreaInfoList => _areaList;

        private AreaManager()
        {
            var locationsEn =
                Context.PluginApi.GetPluginPath(Utils.PluginId, 
                    "assets/GeoLite2-Country-CSV_20190423/GeoLite2-Country-Locations-en.csv");
            var locationsCn =
                Context.PluginApi.GetPluginPath(Utils.PluginId,
                    "assets/GeoLite2-Country-CSV_20190423/GeoLite2-Country-Locations-zh-CN.csv");
            var enCsv = File.ReadAllLines(locationsEn);
            var cnCsv = File.ReadAllLines(locationsCn);

            for (var i = 0; i < enCsv.Length; i++)
            {
                if (i == 0) continue;

                var enSplits = enCsv[i].Split(',');
                var cnSplits = cnCsv[i].Split(',');

                var geoNameIdEn = Utils.ToInt(enSplits[0]);
                var areaEn = enSplits[5].Trim('"');
                var geoNameIdCn = Utils.ToInt(cnSplits[0]);
                var areaCn = cnSplits[5].Trim('"');

                if (geoNameIdEn == geoNameIdCn && !string.IsNullOrEmpty(areaEn) && !string.IsNullOrEmpty(areaCn))
                {
                    _areaList.Add(new AreaInfo
                    {
                        GeoNameId = geoNameIdEn,
                        AreaEn = areaEn,
                        AreaCn = areaCn
                    });
                }
            }

            _areaList = _areaList.OrderBy(x => x.AreaEn).ToList();

            _areaList.Insert(0, new AreaInfo
            {
                GeoNameId = Utils.LocalGeoNameId,
                AreaEn = Utils.LocalAreaEn,
                AreaCn = Utils.LocalAreaCn
            });
        }

        public List<KeyValuePair<int, string>> GetAreaInfoList()
        {
            return _areaList.Select(x => new KeyValuePair<int, string>(x.GeoNameId, $"{x.AreaEn}({x.AreaCn})"))
                .ToList();
        }

        public AreaInfo GetAreaInfo(int geoNameId)
        {
            return _areaList.FirstOrDefault(x => x.GeoNameId == geoNameId);
        }
    }
}
