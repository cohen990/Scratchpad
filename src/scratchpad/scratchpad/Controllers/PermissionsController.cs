using System.Web.Mvc;

namespace scratchpad.Controllers
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Models;
    using Services;

    public class PermissionsController : Controller
    {
        private readonly PermissionsService _service;

        public PermissionsController()
        {
            _service = new PermissionsService(new HttpClient());
        }

        // GET: Permissinos
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> RequestPermissions()
        {
            InteractionModel<PermissionsServiceModel> result = await _service.RequestPermissionsAsync();

            if (!result.IsSuccessful)
            {
                ModelState.AddModelError("0", result.Errors.FirstOrDefault());
                return View(result.Data);
            }

            return
                Redirect(string.Format(
                    "https://www.sandbox.paypal.com/cgi-bin/webscr?cmd=_grant-permission&request_token={0}",
                    result.Data.Token));
        }

        public async Task<ActionResult> Success(string request_token, string verification_code)
        {
            if (string.IsNullOrEmpty(request_token))
                throw new ArgumentNullException("request_token");
            if (string.IsNullOrEmpty(verification_code))
                throw new ArgumentNullException("verification_code");

            var result = await _service.GetAccessTokenAsync(request_token, verification_code);

            if (!result.IsSuccessful)
            {
                ModelState.AddModelError("0", result.Errors.FirstOrDefault());
                return View(result.Data);
            }

            return View(result.Data);
        }
    }
}