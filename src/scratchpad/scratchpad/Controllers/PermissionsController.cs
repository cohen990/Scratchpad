using System.Web.Mvc;

namespace scratchpad.Controllers
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Services;

    public class PermissionsController : Controller
    {
        private readonly PayPalService _service;

        public PermissionsController()
        {
            _service = new PayPalService(true);
        }

        // GET: Permissinos
        public ActionResult Index()
        {
            return View();
        }

        public async Task<RedirectResult> RequestPermissions()
        {
            var result = await _service.RequestPermissions();

            const string token = "hello";

            return
                Redirect(string.Format(
                    "https://www.sandbox.paypal.com/cgi-bin/webscr?cmd=_grant-permission&request_token={0}",
                    token));
        }
    }
}