using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Block.Core;

namespace SS.Block.Controllers.Pages
{
    [RoutePrefix("pages/settings")]
    public class PagesSettingsController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetConfig()
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var siteId = request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, Utils.PluginId)) return Unauthorized();

                var config = Main.GetConfig(siteId);
                var areas = AreaManager.Instance.GetAreaInfoList();
                var blockAreas = new List<KeyValuePair<int, string>>();
                if (config.BlockAreas != null && areas != null)
                {
                    blockAreas = areas.Where(x => config.BlockAreas.Contains(x.Key)).ToList();
                }

                var channels = new List<KeyValuePair<int, string>>();
                var channelIdList = Context.ChannelApi.GetChannelIdList(siteId);
                var isLastNodeArray = new bool[channelIdList.Count];
                foreach (var theChannelId in channelIdList)
                {
                    var channelInfo = Context.ChannelApi.GetChannelInfo(siteId, theChannelId);

                    var title = GetChannelListBoxTitle(siteId, channelInfo.Id, channelInfo.ChannelName, channelInfo.ParentsCount, channelInfo.LastNode, isLastNodeArray);
                    channels.Add(new KeyValuePair<int, string>(channelInfo.Id, title));
                }
                var blockChannels = new List<KeyValuePair<int, string>>();
                if (config.BlockChannels != null)
                {
                    blockChannels = channels.Where(x => config.BlockChannels.Contains(x.Key)).ToList();
                }

                return Ok(new
                {
                    Value = config,
                    Areas = areas,
                    BlockAreas = blockAreas,
                    Channels = channels,
                    BlockChannels = blockChannels
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private static string GetChannelListBoxTitle(int siteId, int channelId, string nodeName, int parentsCount, bool isLastNode, IList<bool> isLastNodeArray)
        {
            var str = string.Empty;
            if (channelId == siteId)
            {
                isLastNode = true;
            }
            if (isLastNode == false)
            {
                isLastNodeArray[parentsCount] = false;
            }
            else
            {
                isLastNodeArray[parentsCount] = true;
            }
            for (var i = 0; i < parentsCount; i++)
            {
                str = string.Concat(str, "　");
            }

            str = string.Concat(str, "└");
            str = string.Concat(str, Utils.MaxLengthText(nodeName, 8));

            return str;
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult SetConfig()
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var siteId = request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, Utils.PluginId)) return Unauthorized();

                var config = Main.GetConfig(siteId);
                var blockAreas = new List<KeyValuePair<int, string>>();
                var blockChannels = new List<KeyValuePair<int, string>>();

                var type = request.GetPostString("type");
                if (Utils.EqualsIgnoreCase(type, nameof(ConfigInfo.IsEnabled)))
                {
                    config.IsEnabled = request.GetPostBool(nameof(ConfigInfo.IsEnabled));
                    Main.SetConfig(siteId, config);
                }
                else if (Utils.EqualsIgnoreCase(type, nameof(ConfigInfo.IsAllChannels)))
                {
                    config.IsAllChannels = request.GetPostBool(nameof(ConfigInfo.IsAllChannels));
                    blockChannels = request.GetPostObject<List<KeyValuePair<int, string>>>(nameof(ConfigInfo.BlockChannels));
                    config.BlockChannels = blockChannels.Select(x => x.Key).ToList();
                    Main.SetConfig(siteId, config); 
                }
                else if (Utils.EqualsIgnoreCase(type, nameof(ConfigInfo.IsAllAreas)))
                {
                    config.IsAllAreas = request.GetPostBool(nameof(ConfigInfo.IsAllAreas));
                    blockAreas = request.GetPostObject<List<KeyValuePair<int, string>>>(nameof(ConfigInfo.BlockAreas));
                    config.BlockAreas = blockAreas.Select(x => x.Key).ToList();
                    Main.SetConfig(siteId, config);
                }
                else if (Utils.EqualsIgnoreCase(type, nameof(ConfigInfo.BlockMethod)))
                {
                    config.BlockMethod = request.GetPostString(nameof(ConfigInfo.BlockMethod));

                    config.RedirectUrl = request.GetPostString(nameof(ConfigInfo.RedirectUrl));
                    config.Warning = request.GetPostString(nameof(ConfigInfo.Warning));
                    config.Password = request.GetPostString(nameof(ConfigInfo.Password));

                    Main.SetConfig(siteId, config);
                }

                var areas = AreaManager.Instance.GetAreaInfoList();
                if (config.BlockAreas != null && areas != null)
                {
                    blockAreas = areas.Where(x => config.BlockAreas.Contains(x.Key)).ToList();
                }

                var channels = new List<KeyValuePair<int, string>>();
                var channelIdList = Context.ChannelApi.GetChannelIdList(siteId);
                var isLastNodeArray = new bool[channelIdList.Count];
                foreach (var theChannelId in channelIdList)
                {
                    var channelInfo = Context.ChannelApi.GetChannelInfo(siteId, theChannelId);

                    var title = GetChannelListBoxTitle(siteId, channelInfo.Id, channelInfo.ChannelName, channelInfo.ParentsCount, channelInfo.LastNode, isLastNodeArray);
                    channels.Add(new KeyValuePair<int, string>(channelInfo.Id, title));
                }
                
                if (config.BlockChannels != null)
                {
                    blockChannels = channels.Where(x => config.BlockChannels.Contains(x.Key)).ToList();
                }

                return Ok(new
                {
                    Value = config,
                    Areas = areas,
                    BlockAreas = blockAreas,
                    Channels = channels,
                    BlockChannels = blockChannels
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
