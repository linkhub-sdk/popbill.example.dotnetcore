using Microsoft.AspNetCore.Mvc;
using Popbill;
using Popbill.EasyFin;

namespace EasyFinBankExample.Controllers
{
    public class EasyFinBankController : Controller
    {
        private readonly EasyFinBankService _easyFinBankService;

        public EasyFinBankController(EasyFinBankInstance EasyFinBank)
        {
            // 계조조회 서비스 객체 생성
            _easyFinBankService = EasyFinBank.easyFinBankService;

        }

        // 팝빌 연동회원 사업자번호 (하이픈 '-' 없이 10자리)
        string corpNum = "1234567890";

        // 팝빌 연동회원 아이디
        string userID = "testkorea";

        public IActionResult Index()
        {
            return View();
        }

        /*
        * 계좌조회 서비스를 이용할 은행계좌를 등록한다.
        * - https://docs.popbill.com/easyfinbank/dotnetcore/api#RegistBankAccount
        */
        public IActionResult RegistBankAccount()
        {
            EasyFinBankAccountForm info = new EasyFinBankAccountForm();

            // [필수] 은행코드
            // 산업은행-0002 / 기업은행-0003 / 국민은행-0004 /수협은행-0007 / 농협은행-0011 / 우리은행-0020
            // SC은행-0023 / 대구은행-0031 / 부산은행-0032 / 광주은행-0034 / 제주은행-0035 / 전북은행-0037
            // 경남은행-0039 / 새마을금고-0045 / 신협은행-0048 / 우체국-0071 / KEB하나은행-0081 / 신한은행-0088 /씨티은행-0027
            info.BankCode = "";

            // [필수] 계좌번호, 하이픈('-') 제외
            info.AccountNumber = "";

            // [필수] 계좌비밀번호
            info.AccountPWD = "";

            // [필수] 계좌유형, "법인" 또는 "개인" 입력
            info.AccountType = "";

            // [필수] 예금주 식별정보 (‘-‘ 제외)
            // 계좌유형이 “법인”인 경우 : 사업자번호(10자리)
            // 계좌유형이 “개인”인 경우 : 예금주 생년월일 (6자리-YYMMDD)
            info.IdentityNumber = "";

            // 계좌 별칭
            info.AccountName = "";

            // 인터넷뱅킹 아이디 (국민은행 필수)
            info.BankID = "";

            // 조회전용 계정 아이디 (대구은행, 신협, 신한은행 필수)
            info.FastID = "";

            // 조회전용 계정 비밀번호 (대구은행, 신협, 신한은행 필수)
            info.FastPWD = "";

            // 결제기간(개월), 1~12 입력가능, 미기재시 기본값(1) 처리
            // - 파트너 과금방식의 경우 입력값에 관계없이 1개월 처리
            info.UsePeriod = "1";

            // 메모
            info.Memo = "";


            try
            {
                var response = _easyFinBankService.RegistBankAccount(corpNum, info);

                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌에 등록된 은행 계좌정보를 수정합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#UpdateBankAccount
         */
        public IActionResult UpdateBankAccount()
        {
            EasyFinBankAccountForm info = new EasyFinBankAccountForm();

            // [필수] 은행코드
            // 산업은행-0002 / 기업은행-0003 / 국민은행-0004 /수협은행-0007 / 농협은행-0011 / 우리은행-0020
            // SC은행-0023 / 대구은행-0031 / 부산은행-0032 / 광주은행-0034 / 제주은행-0035 / 전북은행-0037
            // 경남은행-0039 / 새마을금고-0045 / 신협은행-0048 / 우체국-0071 / KEB하나은행-0081 / 신한은행-0088 /씨티은행-0027
            info.BankCode = "";

            // [필수] 계좌번호, 하이픈('-') 제외
            info.AccountNumber = "";

            // [필수] 계좌비밀번호
            info.AccountPWD = "";

            // 계좌 별칭
            info.AccountName = "";

            // 인터넷뱅킹 아이디 (국민은행 필수)
            info.BankID = "";

            // 조회전용 계정 아이디 (대구은행, 신협, 신한은행 필수)
            info.FastID = "";

            // 조회전용 계정 비밀번호 (대구은행, 신협, 신한은행 필수)
            info.FastPWD = "";

            // 메모
            info.Memo = "";


            try
            {
                var response = _easyFinBankService.UpdateBankAccount(corpNum, info);

                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌에 등록된 은행 계좌정보를 확인합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#GetBankAccountInfo
         */
        public IActionResult GetBankAccountInfo()
        {
            // [필수] 은행코드
            // 산업은행-0002 / 기업은행-0003 / 국민은행-0004 /수협은행-0007 / 농협은행-0011 / 우리은행-0020
            // SC은행-0023 / 대구은행-0031 / 부산은행-0032 / 광주은행-0034 / 제주은행-0035 / 전북은행-0037
            // 경남은행-0039 / 새마을금고-0045 / 신협은행-0048 / 우체국-0071 / KEB하나은행-0081 / 신한은행-0088 /씨티은행-0027
            string BankCode = "";

            // [필수] 계좌번호, 하이픈('-') 제외
            string AccountNumber = "";

            try
            {
                var response = _easyFinBankService.GetBankAccountInfo(corpNum, BankCode, AccountNumber);
                return View("GetBankAccountInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌에 등록된 은행계좌의 정액제 해지를 요청합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#CloseBankAccount
         */
        public IActionResult CloseBankAccount()
        {
            // [필수] 은행코드
            // 산업은행-0002 / 기업은행-0003 / 국민은행-0004 /수협은행-0007 / 농협은행-0011 / 우리은행-0020
            // SC은행-0023 / 대구은행-0031 / 부산은행-0032 / 광주은행-0034 / 제주은행-0035 / 전북은행-0037
            // 경남은행-0039 / 새마을금고-0045 / 신협은행-0048 / 우체국-0071 / KEB하나은행-0081 / 신한은행-0088 /씨티은행-0027
            string BankCode = "";

            // [필수] 계좌번호, 하이픈('-') 제외
            string AccountNumber = "";

            // [필수] 해지유형, “일반”, “중도” 중 선택 기재
            // 일반해지 – 이용중인 정액제 사용기간까지 이용후 정지
            // 중도해지 – 요청일 기준으로 정지, 정액제 잔여기간은 일할로 계산되어 포인트 환불 (무료 이용기간 중 중도해지 시 전액 환불)
            string CloseType = "중도";


            try
            {
                var response = _easyFinBankService.CloseBankAccount(corpNum, BankCode, AccountNumber, CloseType);

                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        /*
         * 계좌 정액제 해지요청을 취소합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#RevokeCloseBankAccount
         */
        public IActionResult RevokeCloseBankAccount()
        {
            // [필수] 은행코드
            // 산업은행-0002 / 기업은행-0003 / 국민은행-0004 /수협은행-0007 / 농협은행-0011 / 우리은행-0020
            // SC은행-0023 / 대구은행-0031 / 부산은행-0032 / 광주은행-0034 / 제주은행-0035 / 전북은행-0037
            // 경남은행-0039 / 새마을금고-0045 / 신협은행-0048 / 우체국-0071 / KEB하나은행-0081 / 신한은행-0088 /씨티은행-0027
            string BankCode = "";

            // [필수] 계좌번호, 하이픈('-') 제외
            string AccountNumber = "";

            try
            {
                var response = _easyFinBankService.RevokeCloseBankAccount(corpNum, BankCode, AccountNumber);

                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        /*
         * 팝빌 계좌 관리 팝업 URL을 반환합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#GetBankAccountMgtURL
         */
        public IActionResult GetBankAccountMgtURL()
        {

            try
            {
                var result = _easyFinBankService.GetBankAccountMgtURL(corpNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌에 등록된 계좌 목록을 확인합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#ListBankAccount
         */
        public IActionResult LIstBankAccount()
        {
            try
            {
                var response = _easyFinBankService.ListBankAccount(corpNum, userID);
                return View("ListBankAccount", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 계좌의 거래내역 수집을 요청합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#RequestJob
         */
        public IActionResult RequestJob()
        {

            // 은행코드 
            string BankCode = "0048";

            // 계좌번호
            string AccountNumber = "131020538645";

            // 시작일자, 표시형식(yyyyMMdd)
            string SDate = "20191101";

            // 종료일자, 표시형식(yyyyMMdd)
            string EDate = "20200108";

            try
            {
                var result = _easyFinBankService.RequestJob(corpNum, BankCode, AccountNumber, SDate, EDate, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 수집 작업 상태를 확인합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#GetJobState
         */
        public IActionResult GetJobState()
        {
            // 수집 요청(requestJob API)시 반환반은 작업아이디(jobID)
            string jobID = "020010816000000002";

            try
            {
                var response = _easyFinBankService.GetJobState(corpNum, jobID, userID);
                return View("GetJobState", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 수집작업 목록을 확인합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#ListActiveJob
         */
        public IActionResult ListActiveJob()
        {
            try
            {
                var response = _easyFinBankService.ListActiveJob(corpNum, userID);
                return View("ListActiveJob", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 수집이 완료된 계좌의 거래내역을 조회합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#Search
         */
        public IActionResult Search()
        {
            // 수집 요청(requestJob API)시 반환반은 작업아이디(jobID)
            string jobID = "020010816000000002";

            // 거래유형 배열, I-입금, O-출금
            string[] TradeType = { "N", "M" };

            string SearchString = "";

            // 페이지 번호, 기본값 '1'
            int Page = 1;

            // 페이지당 검색개수, 기본값 '500', 최대 '1000' 
            int PerPage = 30;

            // 정렬방향, A-오름차순, D-내림차순
            string Order = "D";

            try
            {
                var response = _easyFinBankService.Search(corpNum, jobID, TradeType, SearchString, Page, PerPage, Order, userID);
                return View("Search", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 수집이 완료된 거래내역의 요약정보를 조회합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#Summary
         */
        public IActionResult Summary()
        {
            // 수집 요청(requestJob API)시 반환반은 작업아이디(jobID)
            string jobID = "020010816000000002";

            // 거래유형 배열, I-입금, O-출금
            string[] TradeType = { "N", "M" };

            string SearchString = "";

            try
            {
                var response = _easyFinBankService.Summary(corpNum, jobID, TradeType, SearchString, userID);
                return View("Summary", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 거래내역에 메모를 저장합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#SaveMemo
         */
        public IActionResult SaveMemo()
        {
            string tid = "01912181100000000120191231000001";

            string memo = "asp.net Core";

            try
            {
                var response = _easyFinBankService.SaveMemo(corpNum, tid, memo, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 계좌조회 정액제 신청 팝업 URL을 반환합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#GetFlatRatePopUpURL
         */
        public IActionResult GetFlatRatePopUpURL()
        {
            try
            {
                var result = _easyFinBankService.GetFlatRatePopUpURL(corpNum);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 계좌 정액제 상태를 확인합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#GetFlatRateState
         */
        public IActionResult GetFlatRateState()
        {
            // 은행코드 
            string BankCode = "0048";

            // 계좌번호
            string AccountNumber = "131020538645";
            try
            {
                var response = _easyFinBankService.GetFlatRateState(corpNum, BankCode, AccountNumber);
                return View("GetFlatRateState", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 해당 사업자의 파트너 연동회원 가입여부를 확인합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#CheckIsMember
         */
        public IActionResult CheckIsMember()
        {
            try
            {
                //링크아이디
                string linkID = "TESTER";

                var response = _easyFinBankService.CheckIsMember(corpNum, linkID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌 회원아이디 중복여부를 확인합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#CheckID
         */
        public IActionResult CheckID()
        {
            //중복여부 확인할 팝빌 회원 아이디
            string checkID = "testkorea";

            try
            {
                var response = _easyFinBankService.CheckID(checkID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 파트너의 연동회원으로 신규가입 처리합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#JoinMember
         */
        public IActionResult JoinMember()
        {
            JoinForm joinInfo = new JoinForm();

            // 링크아이디
            joinInfo.LinkID = "TESTER";

            // 아이디, 6자이상 50자 미만
            joinInfo.ID = "userid_20181212";

            // 비밀번호, 6자이상 20자 미만
            joinInfo.PWD = "12341234";

            // 사업자번호 "-" 제외
            joinInfo.CorpNum = "0000000001";

            // 대표자 성명 (최대 100자)
            joinInfo.CEOName = "대표자 성명";

            // 상호 (최대 200자)
            joinInfo.CorpName = "상호";

            // 주소 (최대 300자)
            joinInfo.Addr = "주소";

            // 업태 (최대 100자)
            joinInfo.BizType = "업태";

            // 종목 (최대 100자)
            joinInfo.BizClass = "종목";

            // 담당자 성명 (최대 100자)
            joinInfo.ContactName = "담당자명";

            // 담당자 이메일주소 (최대 100자)
            joinInfo.ContactEmail = "test@test.com";

            // 담당자 연락처 (최대 20자)
            joinInfo.ContactTEL = "070-4304-2992";

            // 담당자 휴대폰번호 (최대 20자)
            joinInfo.ContactHP = "010-111-222";

            // 담당자 팩스번호 (최대 20자)
            joinInfo.ContactFAX = "02-111-222";
            try
            {
                var response = _easyFinBankService.JoinMember(joinInfo);
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
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#GetAccessURL
         */
        public IActionResult GetAccessURL()
        {
            try
            {
                var result = _easyFinBankService.GetAccessURL(corpNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 회사정보를 확인합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#GetCorpInfo
         */
        public IActionResult GetCorpInfo()
        {
            try
            {
                var response = _easyFinBankService.GetCorpInfo(corpNum, userID);
                return View("GetCorpInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 회사정보를 수정합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#UpdateCorpInfo
         */
        public IActionResult UpdateCorpInfo()
        {
            CorpInfo corpInfo = new CorpInfo();

            // 대표자 성명 (최대 100자)
            corpInfo.ceoname = "대표자 성명 수정";

            // 상호 (최대 200자)
            corpInfo.corpName = "상호 수정";

            // 주소 (최대 300자)
            corpInfo.addr = "주소 수정";

            // 업태 (최대 100자)
            corpInfo.bizType = "업태 수정";

            // 종목 (최대 100자)
            corpInfo.bizClass = "종목 수정";

            try
            {
                var response = _easyFinBankService.UpdateCorpInfo(corpNum, corpInfo, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 담당자를 신규로 등록합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#RegistContact
         */
        public IActionResult RegistContact()
        {
            Contact contactInfo = new Contact();

            // 담당자 아이디, 6자 이상 50자 미만
            contactInfo.id = "testkorea_20181212";

            // 비밀번호, 6자 이상 20자 미만
            contactInfo.pwd = "user_password";

            // 담당자명 (최대 100자)
            contactInfo.personName = "코어담당자";

            // 담당자 연락처 (최대 20자)
            contactInfo.tel = "070-4304-2992";

            // 담당자 휴대폰번호 (최대 20자)
            contactInfo.hp = "010-111-222";

            // 담당자 팩스번호 (최대 20자)
            contactInfo.fax = "02-111-222";

            // 담당자 이메일 (최대 100자)
            contactInfo.email = "netcore@linkhub.co.kr";

            // 회사조회 권한여부, true(회사조회), false(개인조회)
            contactInfo.searchAllAllowYN = true;

            // 관리자 권한여부, true(관리자), false(사용자)
            contactInfo.mgrYN = false;

            try
            {
                var response = _easyFinBankService.RegistContact(corpNum, contactInfo, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 담당자 목록을 확인합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#ListContact
         */
        public IActionResult ListContact()
        {
            try
            {
                var response = _easyFinBankService.ListContact(corpNum, userID);
                return View("ListContact", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 담당자 정보를 수정합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#UpdateContact
         */
        public IActionResult UpdateContact()
        {
            Contact contactInfo = new Contact();

            // 담당자 아이디
            contactInfo.id = "testkorea";

            // 담당자명 (최대 100자)
            contactInfo.personName = "코어담당자";

            // 담당자 연락처 (최대 20자)
            contactInfo.tel = "070-4304-2992";

            // 담당자 휴대폰번호 (최대 20자)
            contactInfo.hp = "010-111-222";

            // 담당자 팩스번호 (최대 20자)
            contactInfo.fax = "02-111-222";

            // 담당자 이메일 (최대 10자)
            contactInfo.email = "test@linkhub.co.kr";

            // 회사조회 권한여부, true(회사조회), false(개인조회)
            contactInfo.searchAllAllowYN = true;

            // 관리자 권한여부, true(관리자), false(사용자)
            contactInfo.mgrYN = false;

            try
            {
                var response = _easyFinBankService.UpdateContact(corpNum, contactInfo, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 잔여포인트를 확인합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#GetBalance
         */
        public IActionResult GetBalance()
        {
            try
            {
                var result = _easyFinBankService.GetBalance(corpNum);
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
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#GetChargeURL
         */
        public IActionResult GetChargeURL()
        {
            try
            {
                var result = _easyFinBankService.GetChargeURL(corpNum, userID);
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
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#GetPartnerBalance
         */
        public IActionResult GetPartnerBalance()
        {
            try
            {
                var result = _easyFinBankService.GetPartnerBalance(corpNum);
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
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#GetPartnerURL
         */
        public IActionResult GetPartnerURL()
        {
            // CHRG 포인트충전 URL
            string TOGO = "CHRG";

            try
            {
                var result = _easyFinBankService.GetPartnerURL(corpNum, TOGO);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 홈택스연동 API 서비스 과금정보를 확인합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#GetChargeInfo
         */
        public IActionResult GetChargeInfo()
        {
            try
            {
                var response = _easyFinBankService.GetChargeInfo(corpNum);
                return View("GetChargeInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }
    }
}
