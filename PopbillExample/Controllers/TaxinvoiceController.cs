using Microsoft.AspNetCore.Mvc;
using Popbill;
using Popbill.Taxinvoice;

namespace PopbillExample.Controllers
{
    public class TaxinvoiceController : Controller
    {
        //링크허브에서 발급받은 고객사 고객사 인증정보로 링크아이디(LinkID)와 비밀키(SecretKey) 값을 변경하시기 바랍니다.
        static string linkID = "TESTER";
        static string secretKey = "SwWxqU+0TErBXy/9TVjIPEnI0VTUMMSQZtJf3Ed8q3I=";

        private TaxinvoiceService taxinvoiceService;

        public TaxinvoiceController()
        {
            taxinvoiceService = new TaxinvoiceService(linkID, secretKey);
            taxinvoiceService.IsTest = true;
        }

        //팝빌 연동회원 사업자번호 (하이픈 '-' 없이 10자리)
        string corpNum = "1234567890";

        //팝빌 연동회원 아이디
        string userID = "testkorea";

        // GET
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CheckIsMember()
        {
            try
            {
                var response = taxinvoiceService.CheckIsMember(corpNum, linkID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        public IActionResult GetUnitCost()
        {
            try
            {
                var response = taxinvoiceService.GetUnitCost(corpNum, userID);
                return View("Result", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }
    }
}