using ControllerDI.Services;
using Microsoft.AspNetCore.Mvc;
using Popbill;
using Popbill.HomeTax;


namespace HTCashbillExample.Controllers
{
    public class HTCashbillController : Controller
    {
        private readonly HTCashbillService _htCashbillService;

        public HTCashbillController(HTCashbillInstance HTCashbill)
        {
            //홈택스 서비스 객체 생성
            _htCashbillService = HTCashbill.htCashbillService;

            //연동환경 설정값, 개발용(true), 상업용(false)
            _htCashbillService.IsTest = true;
        }

        //팝빌 연동회원 사업자번호 (하이픈 '-' 없이 10자리)
        string corpNum = "6798700433";

        //팝빌 연동회원 아이디
        string userID = "testkorea_linkhub";

        /*
         * 홈택스 Index page (HTCashbill/Index.cshtml)
         */
        public IActionResult Index()
        {
            return View();
        }

        #region 홈택스 현금영수증 매입/매출 내역 수집

        /*
         * 현금영수증 매출/매입 내역 수집을 요청합니다
         * - 홈택스연동 프로세스는 "[홈택스연동 (현금영수증) API 연동매뉴얼]> 1.1. 프로세스 흐름도" 를 참고하시기 바랍니다.
         * - 수집 요청후 반환받은 작업아이디(JobID)의 유효시간은 1시간 입니다.
         */
        public IActionResult RequestJob()
        {
            // 현금영수증 유형 SELL-매출, BUY-매입
            KeyType tiKeyType = KeyType.SELL;

            // 시작일자, 표시형식(yyyyMMdd)
            string SDate = "20180101";

            // 종료일자, 표시형식(yyyyMMdd)
            string EDate = "20181126";

            try
            {
                var result = _htCashbillService.RequestJob(corpNum, tiKeyType, SDate, EDate, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 수집 요청 상태를 확인합니다.
         * - 응답항목 관한 정보는 "[홈택스연동 (현금영수증) API 연동매뉴얼] > 3.1.2. GetJobState(수집 상태 확인)" 을 참고하시기 바랍니다.
         */
        public IActionResult GetJobState()
        {
            // 수집 요청(requestJob API)시 반환반은 작업아이디(jobID)
            string jobID = "018112709000000001";

            try
            {
                var response = _htCashbillService.GetJobState(corpNum, jobID, userID);
                return View("GetJobState", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 수집 요청건들에 대한 상태 목록을 확인합니다.
         * - 수집 요청 작업아이디(JobID)의 유효시간은 1시간 입니다.
         * - 응답항목에 관한 정보는 "[홈택스연동 (현금영수증) API 연동매뉴얼] > 3.1.3. ListActiveJob(수집 상태 목록 확인)" 을 참고하시기 바랍니다.
         */
        public IActionResult ListActiveJob()
        {
            try
            {
                var response = _htCashbillService.ListActiveJob(corpNum, userID);
                return View("ListActiveJob", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        #endregion

        #region 홈택스 현금영수증 매입/매출 내역 수집 결과 조회

        /*
         * 현금영수증 매입/매출 내역의 수집 결과를 조회합니다.
         * - 응답항목에 관한 정보는 "[홈택스연동 (현금영수증) API 연동매뉴얼] > 3.2.1. Search(수집 결과 조회)" 을 참고하시기 바랍니다.
         */
        public IActionResult Search()
        {
            // 수집 요청(requestJob API)시 반환반은 작업아이디(jobID)
            string jobID = "018112709000000001";

            // 현금영수증 형태 배열, N-일반 현금영수증, C-취소 현금영수증
            string[] TradeType = {"N", "C"};

            // 거래용도 배열, P-소득공제용, C-지출증빙용
            string[] TradeUsage = {"P", "C"};

            // 페이지 번호, 기본값 '1'
            int Page = 1;

            // 페이지당 검색개수, 기본값 '500', 최대 '1000' 
            int PerPage = 30;

            // 정렬방향, A-오름차순, D-내림차순
            string Order = "D";

            try
            {
                var response = _htCashbillService.Search(corpNum, jobID, TradeType, TradeUsage, Page, PerPage, Order,
                    userID);
                return View("Search", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 현금영수증 매입/매출 내역의 수집 결과 요약정보를 조회합니다.
         * - 응답항목에 관한 정보는 "[홈택스연동 (현금영수증) API 연동매뉴얼] > 3.2.2. Summary(수집 결과 요약정보 조회)" 을 참고하시기 바랍니다.
         */
        public IActionResult Summary()
        {
            // 수집 요청(requestJob API)시 반환반은 작업아이디(jobID)
            string jobID = "018112709000000001";
            
            // 현금영수증 형태 배열, N-일반 현금영수증, C-취소 현금영수증
            string[] TradeType = { "N", "C" };

            // 거래용도 배열, P-소득공제용, C-지출증빙용
            string[] TradeUsage = { "P", "C" };
            
            try
            {
                var response = _htCashbillService.Summary(corpNum, jobID, TradeType, TradeUsage, userID);
                return View("Summary", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        #endregion

        #region 홈택스 인증 관리

        /*
         * 홈택스연동에 이용할 공인인증서 등록 URL을 반환합니다.
         * - 반환된 URL은 보안정책에 따라 30초의 유효시간을 갖습니다.
         */
        public IActionResult GetCertificatePopUpURL()
        {
            try
            {
                var result = _htCashbillService.GetCertificatePopUpURL(corpNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 홈택스연동에 이용하는 공인인증서의 만료일자를 반환합니다.
         */
        public IActionResult GetCertificateExpireDate()
        {
            try
            {
                var result = _htCashbillService.GetCertificateExpireDate(corpNum, userID);
                return View("Result", result.ToString("yyyyMMddHHmmss"));
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌에 등록된 공인인증서의 홈택스 로그인을 테스트합니다.
         */
        public IActionResult CheckCertValidation()
        {
            try
            {
                var response = _htCashbillService.CheckCertValidation(corpNum, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 홈택스 전자세금계산서 부서사용자 계정을 등록합니다.
         */
        public IActionResult RegistDeptUser()
        {
            // 홈택스에서 생성한 현금영수증 부서사용자 아이디
            string deptUserID = "userid_test";

            // 홈택스에서 생성한 현금영수증 부서사용자 비밀번호
            string deptUserPWD = "passwd_test";
            
            try
            {
                var response = _htCashbillService.RegistDeptUser(corpNum, deptUserID, deptUserPWD, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌에 등록된 전자세금계산서 부서사용자 아이디를 확인합니다.
         */
        public IActionResult CheckDeptUser()
        {
            try
            {
                var response = _htCashbillService.CheckDeptUser(corpNum, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌에 등록된 전자세금계산서 부서사용자 계정정보를 이용하여 홈택스 로그인을 테스트합니다.
         */
        public IActionResult CheckLoginDeptUser()
        {
            try
            {
                var response = _htCashbillService.CheckLoginDeptUser(corpNum, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌에 등록된 전자세금계산서 부서사용자 계정정보를 삭제합니다.
         */
        public IActionResult DeleteDeptUser()
        {
            try
            {
                var response = _htCashbillService.DeleteDeptUser(corpNum, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        #endregion

        #region 포인트관리 / 정액제신청

        /*
         * 연동회원 잔여포인트를 확인합니다.
         */
        public IActionResult GetBalance()
        {
            try
            {
                var result = _htCashbillService.GetBalance(corpNum);
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
                var result = _htCashbillService.GetChargeURL(corpNum, userID);
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
                var result = _htCashbillService.GetPartnerBalance(corpNum);
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
            // CHRG 포인트충전 URL
            string TOGO = "CHRG";

            try
            {
                var result = _htCashbillService.GetPartnerURL(corpNum, TOGO);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 홈택스연동 API 서비스 과금정보를 확인합니다.
         */
        public IActionResult GetChargeInfo()
        {
            try
            {
                var response = _htCashbillService.GetChargeInfo(corpNum);
                return View("GetChargeInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 정액제 서비스 신청 URL을 반환합니다.
         * - 반환된 URL은 보안정책에 따라 30초의 유효시간을 갖습니다.
         */
        public IActionResult GetFlatRatePopUpURL()
        {
            try
            {
                var result = _htCashbillService.GetFlatRatePopUpURL(corpNum);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 정액제 서비스 상태를 확인합니다.
         */
        public IActionResult GetFlatRateState()
        {
            try
            {
                var response = _htCashbillService.GetFlatRateState(corpNum);
                return View("GetFlatRateState", response);
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

                var response = _htCashbillService.CheckIsMember(corpNum, linkID);
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
                var response = _htCashbillService.CheckID(checkID);
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

            joinInfo.LinkID = "TESTER"; // 링크아이디
            joinInfo.ID = "userid"; // 아이디 6자이상 50자 미만
            joinInfo.PWD = "12341234"; // 비밀번호 6자이상 20자 미만
            joinInfo.CorpNum = "0000000001"; // 사업자번호 "-" 제외
            joinInfo.CEOName = "대표자 성명"; // 대표자 성명 
            joinInfo.CorpName = "상호"; // 상호
            joinInfo.Addr = "주소"; // 주소
            joinInfo.BizType = "업태"; // 업태
            joinInfo.BizClass = "종목"; // 종목
            joinInfo.ContactName = "담당자명"; // 담당자 성명 
            joinInfo.ContactEmail = "test@test.com"; // 담당자 이메일주소         
            joinInfo.ContactTEL = "070-4304-2992"; // 담당자 연락처   
            joinInfo.ContactHP = "010-111-222"; // 담당자 휴대폰번호 
            joinInfo.ContactFAX = "02-111-222"; // 담당자 팩스번호

            try
            {
                var response = _htCashbillService.JoinMember(joinInfo);
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
                var result = _htCashbillService.GetAccessURL(corpNum, userID);
                return View("Result", result);
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
                var response = _htCashbillService.GetCorpInfo(corpNum, userID);
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

            corpInfo.ceoname = "대표자 성명 수정"; // 대표자 성명
            corpInfo.corpName = "상호 수정"; // 상호
            corpInfo.addr = "주소 수정"; // 주소
            corpInfo.bizType = "업태 수정"; // 업태 
            corpInfo.bizClass = "종목 수정"; // 종목

            try
            {
                var response = _htCashbillService.UpdateCorpInfo(corpNum, corpInfo, userID);
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

            contactInfo.id = "testkorea_20181108"; // 담당자 아이디; 6자 이상 50자 미만
            contactInfo.pwd = "user_password"; // 비밀번호, 6자 이상 20자 미만
            contactInfo.personName = "코어담당자"; // 담당자명
            contactInfo.tel = "070-4304-2992"; // 담당자연락처
            contactInfo.hp = "010-111-222"; // 담당자 휴대폰번호
            contactInfo.fax = "02-111-222"; // 담당자 팩스번호 
            contactInfo.email = "netcore@linkhub.co.kr"; // 담당자 메일주소
            contactInfo.searchAllAllowYN = true; // 회사조회 권한여부, true(회사조회), false(개인조회)
            contactInfo.mgrYN = false; // 관리자 권한여부 

            try
            {
                var response = _htCashbillService.RegistContact(corpNum, contactInfo, userID);
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
                var response = _htCashbillService.ListContact(corpNum, userID);
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

            contactInfo.id = "testkorea"; // 아이디
            contactInfo.personName = "담당자명"; // 담당자명 
            contactInfo.tel = "070-4304-2992"; // 연락처
            contactInfo.hp = "010-222-111"; // 휴대폰번호
            contactInfo.fax = "02-222-1110"; // 팩스번호
            contactInfo.email = "aspnetcore@popbill.co.kr"; // 이메일주소
            contactInfo.searchAllAllowYN = true; // 회사조회 권한여부, true(회사조회), false(개인조회)
            contactInfo.mgrYN = false; // 관리자 권한여부 

            try
            {
                var response = _htCashbillService.UpdateContact(corpNum, contactInfo, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        #endregion
    }
}