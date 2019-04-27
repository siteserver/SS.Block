using System;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Block.Core;

namespace SS.Block.Controllers.Pages
{
    [RoutePrefix("pages/ip")]
    public class PagesIpController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetIpAddress()
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var siteId = request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, Utils.PluginId)) return Unauthorized();

                return Ok(new
                {
                    Value = Utils.GetIpAddress()
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult Query()
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var siteId = request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, Utils.PluginId)) return Unauthorized();

                var configInfo = Main.GetConfig(siteId);
                var ipAddress = Utils.GetIpAddress();
                var geoNameId = BlockManager.Instance.GetGeoNameId(ipAddress);
                var areaInfo = AreaManager.Instance.GetAreaInfo(geoNameId);
                var isAllowed = BlockManager.Instance.IsAllowed(siteId, configInfo, areaInfo, string.Empty);

                return Ok(new
                {
                    Value = isAllowed,
                    AreaInfo = areaInfo
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
