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

                return Ok(new
                {
                    Value = config,
                    Areas = areas,
                    BlockAreas = blockAreas
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
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

                var type = request.GetPostString("type");
                if (Utils.EqualsIgnoreCase(type, nameof(ConfigInfo.IsBlock)))
                {
                    config.IsBlock = request.GetPostBool(nameof(ConfigInfo.IsBlock));
                    Main.SetConfig(siteId, config);
                }
                else if (Utils.EqualsIgnoreCase(type, nameof(ConfigInfo.IsBlockAll)))
                {
                    config.IsBlockAll = request.GetPostBool(nameof(ConfigInfo.IsBlockAll));
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

                return Ok(new
                {
                    Value = config,
                    Areas = areas,
                    BlockAreas = blockAreas
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
