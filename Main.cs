using System.Collections.Generic;
using SiteServer.Plugin;
using SS.Block.Core;

namespace SS.Block
{
    public class Main : PluginBase
    {
        public static BlockRepository BlockRepository { get; private set; }

        public override void Startup(IService service)
        {
            //var config = GetConfig(0);
            //var isAllowed = IPAddressManager.IsVisitAllowed(config);

            BlockRepository = new BlockRepository();

            service
                .AddSiteMenu(siteId => new Menu
                {
                    Text = "IP定位拦截",
                    Href = "pages/settings.html",
                    IconClass = "fa fa-exclamation-triangle",
                    Menus = new List<Menu>
                    {
                        new Menu
                        {
                            Text = "拦截设置",
                            Href = "pages/settings.html",
                        },
                        new Menu
                        {
                            Text = "拦截统计",
                            Href = "pages/analysis.html",
                        },
                        new Menu
                        {
                            Text = "IP地址查询",
                            Href = "pages/ip.html",
                        }
                    }
                })
                .AddDatabaseTable(BlockRepository.TableName, BlockRepository.TableColumns)
                ;

            service.AfterStlParse += Service_AfterStlParse;
        }

        private void Service_AfterStlParse(object sender, ParseEventArgs e)
        {
            var configInfo = GetConfig(e.SiteId);
            if (!configInfo.IsEnabled) return;

            var isChannel = false;
            if (configInfo.IsAllChannels)
            {
                isChannel = true;
            }
            else
            {
                if (configInfo.BlockChannels != null && configInfo.BlockChannels.Contains(e.ChannelId))
                {
                    isChannel = true;
                }
            }

            if (!isChannel) return;

            e.ContentBuilder.Replace("<body", @"<body style=""display: none""");

            var pluginUrl = Context.PluginApi.GetPluginUrl(Utils.PluginId);
            e.HeadCodes[Utils.PluginId] = $@"
<script src=""{pluginUrl}/assets/lib/es6-promise.auto.min.js"" type=""text/javascript""></script>
<script src=""{pluginUrl}/assets/lib/axios-0.18.0.min.js"" type=""text/javascript""></script>
<script src=""{pluginUrl}/assets/lib/sweetalert2-7.28.4.all.min.js"" type=""text/javascript""></script>
<script src=""{pluginUrl}/assets/js/swal2.js"" type=""text/javascript""></script>
<script src=""{pluginUrl}/assets/block.js"" data-api-url=""{Context.Environment.ApiUrl}"" data-site-id=""{e.SiteId}"" type=""text/javascript""></script>
";
        }

        private static string GetCacheKey(int siteId)
        {
            return $"{Utils.PluginId}.{siteId}";
        }

        public static void SetConfig(int siteId, ConfigInfo configInfo)
        {
            CacheUtils.Remove(GetCacheKey(siteId));
            Context.ConfigApi.SetConfig(Utils.PluginId, siteId, configInfo);
        }

        public static ConfigInfo GetConfig(int siteId)
        {
            var cacheKey = GetCacheKey(siteId);
            var configInfo = CacheUtils.Get<ConfigInfo>(cacheKey);
            if (configInfo != null) return configInfo;

            configInfo = Context.ConfigApi.GetConfig<ConfigInfo>(Utils.PluginId, siteId) ?? new ConfigInfo
            {
                IsAllChannels = true
            };
            CacheUtils.Insert(cacheKey, configInfo, 24);

            return configInfo;
        }
    }
}