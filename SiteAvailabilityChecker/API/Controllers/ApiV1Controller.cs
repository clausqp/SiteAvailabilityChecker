using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace SiteAvailabilityChecker.API.Controllers
{


    //[ApiAuthorize]
    //[RequireHttps]
    public class ApiV1Controller : ApiController
    {
        private static ILog logger = log4net.LogManager.GetLogger("ApiV1Controller");


        /// <summary>
        /// Returns the current api version
        /// </summary>
        [AllowAnonymous]
        [Route("api/v1/GetVersion")]
        [HttpGet]
        public HttpResponseMessage GetVersion()
        {
            logger.Debug($"GetVersion");
            return Request.CreateResponse(HttpStatusCode.OK, "1.0");
        }



        [AllowAnonymous]
        [Route("api/v1/GetState")]
        [HttpGet]
        public HttpResponseMessage GetState()
        {
            logger.Debug($"GetState");
            return Request.CreateResponse(HttpStatusCode.OK, CurrentState.StatusMessage);
        }




    }
}
