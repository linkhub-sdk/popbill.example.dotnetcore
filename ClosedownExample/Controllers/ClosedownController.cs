using System.Collections.Generic;
using ControllerDI.Services;
using Microsoft.AspNetCore.Mvc;
using Popbill;
using Popbill.Closedown;

namespace ClosedownExample.Controllers
{
    public class ClosedownController : Controller
    {
        private readonly ClosedownService _closedownService;

        public ClosedownController(ClosedownInstance CDinstance)
        {
            //휴폐업조회 서비스 객체 생성
            _closedownService = CDinstance.closedownService;

            //연동환경 설정값, 개발용(true), 상업용(false)
            _closedownService.IsTest = true;
        }

        //팝빌 연동회원 사업자번호 (하이픈 '-' 없이 10자리)
        string corpNum = "1234567890";

        //팝빌 연동회원 아이디
        string userID = "testkorea";

        /*
         * 휴폐업조회 Index page (Closedown/Index.cshtml)
         */
        public IActionResult Index()
        {
            return View();
        }


        #region 휴폐업조회

        /*
         * 1건의 휴폐업여부를 조회합니다.
         */
        public IActionResult CheckCorpNum()
        {
            try
            {
                //조회할 사업자번호
                string targetCorpNum = "6798700433";

                var response = _closedownService.checkCorpNum(corpNum, targetCorpNum, userID);
                return View("CheckCorpNum", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 다수의 사업자에 대한 휴폐업여부를 조회합니다. (최대 1000건)
         */
        public IActionResult CheckCorpNums()
        {
            try
            {
                //조회할 사업자번호 배열, 최대 1000건
                List<string> targetCorpNums = new List<string>();
                targetCorpNums.Add("1234567890");
                targetCorpNums.Add("6798700433");
                targetCorpNums.Add("401-03-94930");

                var response = _closedownService.checkCorpNums(corpNum, targetCorpNums, userID);
                return View("CheckCorpNums", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        #endregion

        #region 포인트관리 

        /*
         * 연동회원 잔여포인트를 확인합니다.
         */
        public IActionResult GetBalance()
        {
            try
            {
                var result = _closedownService.GetBalance(corpNum);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌 연동회원의 포인트충전 팝업 URL을 반환합니다.
         * - 반환된 URL은 보안정책에 따라 30초의 유효시간을 갖습니다.
         */
        public IActionResult GetChargeURL()
        {
            try
            {
                var result = _closedownService.GetChargeURL(corpNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 파트너의 잔여포인트를 확인합니다.
         * - 과금방식이 연동과금인 경우 연동회원 잔여포인트(GetBalance API)를 이용하시기 바랍니다.
         */
        public IActionResult GetPartnerBalance()
        {
            try
            {
                var result = _closedownService.GetPartnerBalance(corpNum);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 파트너 포인트 충전 팝업 URL을 반환합니다.
         * - 반환된 URL은 보안정책에 따라 30초의 유효시간을 갖습니다.
         */
        public IActionResult GetPartnerURL()
        {
            // CHRG - 포인트충전 URL
            string TOGO = "CHRG";

            try
            {
                var result = _closedownService.GetPartnerURL(corpNum, TOGO);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 휴폐업조회 발행단가를 확인합니다.
         */
        public IActionResult GetUnitCost()
        {
            try
            {
                var result = _closedownService.GetUnitCost(corpNum);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 휴폐업조회 API 서비스 과금정보를 확인합니다.
         */
        public IActionResult GetChargeInfo()
        {
            try
            {
                var response = _closedownService.GetChargeInfo(corpNum);
                return View("GetChargeInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        #endregion

        #region 회원정보

        /*
         * 해당 사업자의 파트너 연동회원 가입여부를 확인합니다.
         */
        public IActionResult CheckIsMember()
        {
            try
            {
                //링크아이디
                string linkID = "TESTER";

                var response = _closedownService.CheckIsMember(corpNum, linkID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌 회원아이디 중복여부를 확인합니다.
         */
        public IActionResult CheckID()
        {
            //중복여부 확인할 팝빌 회원 아이디
            string checkID = "testkorea";

            try
            {
                var response = _closedownService.CheckID(checkID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 파트너의 연동회원으로 신규가입 처리합니다.
         */
        public IActionResult JoinMember()
        {
            JoinForm joinInfo = new JoinForm();

            // 링크아이디
            joinInfo.LinkID = "TESTER"; 
            
            // 아이디 6자이상 50자 미만
            joinInfo.ID = "userid_20181212"; 
            
            // 비밀번호 6자이상 20자 미만
            joinInfo.PWD = "12341234"; 
            
            // 사업자번호 "-" 제외
            joinInfo.CorpNum = "0000000001"; 
            
            // 대표자 성명
            joinInfo.CEOName = "대표자 성명";  
            
            // 상호
            joinInfo.CorpName = "상호"; 
            
            // 주소
            joinInfo.Addr = "주소"; 
            
            // 업태
            joinInfo.BizType = "업태"; 
            
            // 종목
            joinInfo.BizClass = "종목"; 
            
            // 담당자 성명
            joinInfo.ContactName = "담당자명";  
            
            // 담당자 이메일주소
            joinInfo.ContactEmail = "test@test.com";          
            
            // 담당자 연락처
            joinInfo.ContactTEL = "070-4304-2992";    
            
            // 담당자 휴대폰번호
            joinInfo.ContactHP = "010-111-222";  
            
            // 담당자 팩스번호
            joinInfo.ContactFAX = "02-111-222"; 

            try
            {
                var response = _closedownService.JoinMember(joinInfo);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 회사정보를 확인합니다.
         */
        public IActionResult GetCorpInfo()
        {
            try
            {
                var response = _closedownService.GetCorpInfo(corpNum, userID);
                return View("GetCorpInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 회사정보를 수정합니다
         */
        public IActionResult UpdateCorpInfo()
        {
            CorpInfo corpInfo = new CorpInfo();

            // 대표자 성명
            corpInfo.ceoname = "대표자 성명 수정"; 
            
            // 상호
            corpInfo.corpName = "상호 수정"; 
            
            // 주소
            corpInfo.addr = "주소 수정"; 
            
            // 업태
            corpInfo.bizType = "업태 수정";  
            
            // 종목
            corpInfo.bizClass = "종목 수정"; 

            try
            {
                var response = _closedownService.UpdateCorpInfo(corpNum, corpInfo, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 담당자를 신규로 등록합니다.
         */
        public IActionResult RegistContact()
        {
            Contact contactInfo = new Contact();

            // 담당자 아이디, 6자 이상 50자 미만
            contactInfo.id = "testkorea_20181212";
            
            // 비밀번호, 6자 이상 20자 미만
            contactInfo.pwd = "user_password";
            
            // 담당자명
            contactInfo.personName = "코어담당자";
            
            // 담당자연락처
            contactInfo.tel = "070-4304-2992";
            
            // 담당자 휴대폰번호
            contactInfo.hp = "010-111-222";
            
            // 담당자 팩스번호
            contactInfo.fax = "02-111-222";
            
            // 담당자 메일주소
            contactInfo.email = "netcore@linkhub.co.kr";
            
            // 회사조회 권한여부, true(회사조회), false(개인조회)
            contactInfo.searchAllAllowYN = true;
            
            // 관리자 권한여부
            contactInfo.mgrYN = false;

            try
            {
                var response = _closedownService.RegistContact(corpNum, contactInfo, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 담당자 목록을 확인합니다.
         */
        public IActionResult ListContact()
        {
            try
            {
                var response = _closedownService.ListContact(corpNum, userID);
                return View("ListContact", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 담당자 정보를 수정합니다.
         */
        public IActionResult UpdateContact()
        {
            Contact contactInfo = new Contact();

            // 아이디
            contactInfo.id = "testkorea";
            
            // 담당자명
            contactInfo.personName = "담당자명";
            
            // 연락처
            contactInfo.tel = "070-4304-2992";
            
            // 휴대폰번호
            contactInfo.hp = "010-222-111";
            
            // 팩스번호
            contactInfo.fax = "02-222-1110";
            
            // 이메일주소
            contactInfo.email = "aspnetcore@popbill.co.kr";
            
            // 회사조회 권한여부, true(회사조회), false(개인조회)
            contactInfo.searchAllAllowYN = true;
            
            // 관리자 권한여부
            contactInfo.mgrYN = false;

            try
            {
                var response = _closedownService.UpdateContact(corpNum, contactInfo, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌에 로그인 상태로 접근할 수 있는 팝업 URL을 반환합니다.
         * - 반환된 URL은 보안정책에 따라 30초의 유효시간을 갖습니다.
         */
        public IActionResult GetAccessURL()
        {
            try
            {
                var result = _closedownService.GetAccessURL(corpNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        #endregion
    }
}