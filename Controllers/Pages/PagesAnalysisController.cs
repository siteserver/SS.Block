using System;
using System.Linq;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Block.Core;

namespace SS.Block.Controllers.Pages
{
    [RoutePrefix("pages/analysis")]
    public class PagesAnalysisController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetAnalysis()
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var siteId = request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, Utils.PluginId)) return Unauthorized();

                var blockedList = Main.BlockRepository.GetMonthlyBlockedList(siteId);
                var labels = blockedList.Select(x => x.Key).ToList();
                var data = blockedList.Select(x => x.Value).ToList();

                return Ok(new
                {
                    Value = true,
                    Days = labels,
                    Count = data
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
