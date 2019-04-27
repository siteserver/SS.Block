using System;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Block.Core;

namespace SS.Block.Controllers
{
    public class BlockController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Query()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var siteId = request.GetQueryInt("siteId");
                var sessionId = request.GetQueryString("sessionId");
                var ipAddress = Utils.GetIpAddress();
                var configInfo = Main.GetConfig(siteId);
                var geoNameId = BlockManager.Instance.GetGeoNameId(ipAddress);
                var areaInfo = AreaManager.Instance.GetAreaInfo(geoNameId);
                var isAllowed = BlockManager.Instance.IsAllowed(siteId, configInfo, areaInfo, sessionId);
                var blockMethod = configInfo.BlockMethod;
                var redirectUrl = configInfo.RedirectUrl;
                var warning = configInfo.Warning;

                return Ok(new
                {
                    Value = isAllowed,
                    BlockMethod = blockMethod,
                    RedirectUrl = redirectUrl,
                    Warning = warning
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult Auth()
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var siteId = request.GetQueryInt("siteId");
                var password = request.GetPostString("password");

                var sessionId = string.Empty;
                var configInfo = Main.GetConfig(siteId);
                if (configInfo.Password == password)
                {
                    sessionId = Guid.NewGuid().ToString();
                    CacheUtils.Insert(sessionId, true, 1);
                }

                return Ok(new
                {
                    Value = configInfo.Password == password,
                    SessionId = sessionId
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
