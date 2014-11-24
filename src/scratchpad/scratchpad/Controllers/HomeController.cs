namespace scratchpad.Controllers
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Models;
    using Services;

    public class HomeController : Controller
    {
        private readonly PayPalService _service;
        private readonly HttpClient _client;

        public HomeController()
        {
            _service = new PayPalService(true);
            _client = new HttpClient();
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> GoToPayPal()
        {
            var checkout = await _service.SetExpressCheckoutAsync(_client);

            var payPalUri =
                string.Format("https://www.sandbox.paypal.com/cgi-bin/webscr?cmd=_express-checkout&token={0}",
                    checkout.Token);

            return Redirect(payPalUri);
        }

        public async Task<ActionResult> PayPalSuccess(string token, string payerId)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException("token");
            if (string.IsNullOrEmpty(payerId))
                throw new ArgumentNullException("payerId");

            var checkout = await _service.GetExpressCheckoutDetailsAsync(_client, token);

            return View(checkout);
        }

        public async Task<ActionResult> PayPalConfirmed(string token, string payerId)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException("token");
            if (string.IsNullOrEmpty(payerId))
                throw new ArgumentNullException("payerId");

            await _service.DoExpressCheckoutPaymentAsync(_client, token, payerId);

            return View();
        }
    }
}