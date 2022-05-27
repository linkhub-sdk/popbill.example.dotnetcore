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

        #region 계좌 관리

        /*
         * 계좌조회 서비스를 이용할 계좌를 팝빌에 등록합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#RegistBankAccount
         */
        public IActionResult RegistBankAccount()
        {
            EasyFinBankAccountForm info = new EasyFinBankAccountForm();

            // 기관코드
            // 산업은행-0002 / 기업은행-0003 / 국민은행-0004 /수협은행-0007 / 농협은행-0011 / 우리은행-0020
            // SC은행-0023 / 대구은행-0031 / 부산은행-0032 / 광주은행-0034 / 제주은행-0035 / 전북은행-0037
            // 경남은행-0039 / 새마을금고-0045 / 신협은행-0048 / 우체국-0071 / KEB하나은행-0081 / 신한은행-0088 /씨티은행-0027
            info.BankCode = "";

            // 계좌번호, 하이픈('-') 제외
            info.AccountNumber = "";

            // 계좌비밀번호
            info.AccountPWD = "";

            // 계좌유형, "법인" 또는 "개인" 입력
            info.AccountType = "";

            // 예금주 식별정보 (‘-‘ 제외)
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

            // 정액제 이용할 개월수, 1~12 입력가능
            // - 미입력시 기본값 1개월 처리
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
         * 팝빌에 등록된 계좌정보를 수정합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#UpdateBankAccount
         */
        public IActionResult UpdateBankAccount()
        {
            EasyFinBankAccountForm info = new EasyFinBankAccountForm();

            // 기관코드
            // 산업은행-0002 / 기업은행-0003 / 국민은행-0004 /수협은행-0007 / 농협은행-0011 / 우리은행-0020
            // SC은행-0023 / 대구은행-0031 / 부산은행-0032 / 광주은행-0034 / 제주은행-0035 / 전북은행-0037
            // 경남은행-0039 / 새마을금고-0045 / 신협은행-0048 / 우체국-0071 / KEB하나은행-0081 / 신한은행-0088 /씨티은행-0027
            info.BankCode = "";

            // 계좌번호, 하이픈('-') 제외
            info.AccountNumber = "";

            // 계좌비밀번호
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
         * 팝빌에 등록된 계좌 정보를 확인합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#GetBankAccountInfo
         */
        public IActionResult GetBankAccountInfo()
        {
            // 기관코드
            // 산업은행-0002 / 기업은행-0003 / 국민은행-0004 /수협은행-0007 / 농협은행-0011 / 우리은행-0020
            // SC은행-0023 / 대구은행-0031 / 부산은행-0032 / 광주은행-0034 / 제주은행-0035 / 전북은행-0037
            // 경남은행-0039 / 새마을금고-0045 / 신협은행-0048 / 우체국-0071 / KEB하나은행-0081 / 신한은행-0088 /씨티은행-0027
            string BankCode = "";

            // 계좌번호, 하이픈('-') 제외
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
         * 팝빌에 등록된 은행계좌 목록을 반환한다.
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
         * 계좌 등록, 수정 및 삭제할 수 있는 계좌 관리 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
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
         * 계좌의 정액제 해지를 요청합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#CloseBankAccount
         */
        public IActionResult CloseBankAccount()
        {
            // 기관코드
            // 산업은행-0002 / 기업은행-0003 / 국민은행-0004 /수협은행-0007 / 농협은행-0011 / 우리은행-0020
            // SC은행-0023 / 대구은행-0031 / 부산은행-0032 / 광주은행-0034 / 제주은행-0035 / 전북은행-0037
            // 경남은행-0039 / 새마을금고-0045 / 신협은행-0048 / 우체국-0071 / KEB하나은행-0081 / 신한은행-0088 /씨티은행-0027
            string BankCode = "";

            // 계좌번호, 하이픈('-') 제외
            string AccountNumber = "";

            // 해지유형, "일반", "중도" 중 택 1
            // 일반(일반해지) – 이용중인 정액제 기간 만료 후 해지
            // 중도(중도해지) – 해지 요청일 기준으로 정지되고 팝빌 담당자가 승인시 해지
            // └ 중도일 경우, 정액제 잔여기간은 일할로 계산되어 포인트 환불 (무료 이용기간 중 해지하면 전액 환불)
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
         * 신청한 정액제 해지요청을 취소합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#RevokeCloseBankAccount
         */
        public IActionResult RevokeCloseBankAccount()
        {
            // 기관코드
            // 산업은행-0002 / 기업은행-0003 / 국민은행-0004 /수협은행-0007 / 농협은행-0011 / 우리은행-0020
            // SC은행-0023 / 대구은행-0031 / 부산은행-0032 / 광주은행-0034 / 제주은행-0035 / 전북은행-0037
            // 경남은행-0039 / 새마을금고-0045 / 신협은행-0048 / 우체국-0071 / KEB하나은행-0081 / 신한은행-0088 /씨티은행-0027
            string BankCode = "";

            // 계좌번호, 하이픈('-') 제외
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
         * 등록된 계좌를 삭제합니다.
         * - 정액제가 아닌 종량제 이용 시에만 등록된 계좌를 삭제할 수 있습니다.
         * - 정액제 이용 시 정액제 해지요청(CloseBankAccount API) 함수를 사용하여 정액제를 해제할 수 있습니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#DeleteBankAccount
         */
        public IActionResult DeleteBankAccount()
        {
            // 기관코드
            // 산업은행-0002 / 기업은행-0003 / 국민은행-0004 /수협은행-0007 / 농협은행-0011 / 우리은행-0020
            // SC은행-0023 / 대구은행-0031 / 부산은행-0032 / 광주은행-0034 / 제주은행-0035 / 전북은행-0037
            // 경남은행-0039 / 새마을금고-0045 / 신협은행-0048 / 우체국-0071 / KEB하나은행-0081 / 신한은행-0088 /씨티은행-0027
            string BankCode = "";

            // 계좌번호, 하이픈('-') 제외
            string AccountNumber = "";

            try
            {
                var response = _easyFinBankService.DeleteBankAccount(corpNum, BankCode, AccountNumber);

                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        #endregion

        #region 계좌 거래내역 수집

        /*
         * 계좌 거래내역을 확인하기 위해 팝빌에 수집요청을 합니다. (조회기간 단위 : 최대 1개월)
         * - 조회일로부터 최대 3개월 이전 내역까지 조회할 수 있습니다.
         * - 반환 받은 작업아이디는 함수 호출 시점부터 1시간 동안 유효합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#RequestJob
         */
        public IActionResult RequestJob()
        {

            // 기관코드
            string BankCode = "";

            // 계좌번호
            string AccountNumber = "";

            // 시작일자, 표시형식(yyyyMMdd)
            string SDate = "20220501";

            // 종료일자, 표시형식(yyyyMMdd)
            string EDate = "20220527";

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
         * 수집 요청(RequestJob API) 함수를 통해 반환 받은 작업 아이디의 상태를 확인합니다.
         * - 거래 내역 조회(Search API) 함수 또는 거래 요약 정보 조회(Summary API) 함수를 사용하기 전에
         *   수집 작업의 진행 상태, 수집 작업의 성공 여부를 확인해야 합니다.
         * - 작업 상태(jobState) = 3(완료)이고 수집 결과 코드(errorCode) = 1(수집성공)이면
         *   거래 내역 조회(Search) 또는 거래 요약 정보 조회(Summary) 를 해야합니다.
         * - 작업 상태(jobState)가 3(완료)이지만 수집 결과 코드(errorCode)가 1(수집성공)이 아닌 경우에는
         *   오류메시지(errorReason)로 수집 실패에 대한 원인을 파악할 수 있습니다.
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
         * 수집 요청(RequestJob API) 함수를 통해 반환 받은 작업아이디의 목록을 확인합니다.
         * - 수집 요청 후 1시간이 경과한 수집 요청건은 상태정보가 반환되지 않습니다.
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

        #endregion

        #region 계좌 거래내역 조회

        /*
         * 수집 상태 확인(GetJobState API) 함수를 통해 상태 정보가 확인된 작업아이디를 활용하여 계좌 거래 내역을 조회합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#Search
         */
        public IActionResult Search()
        {
            // 수집 요청(RequestJob API) 함수 호출시 반환받은 작업아이디
            string jobID = "020080511000000001";

            // 거래유형 배열 ("I" 와 "O" 중 선택, 다중 선택 가능)
            // └ I = 입금 , O = 출금
            // - 미입력 시 전체조회
            string[] TradeType = { "I", "O" };

            // "입·출금액" / "메모" / "비고" 중 검색하고자 하는 값 입력
            // - 메모 = 거래내역 메모저장(SaveMemo API) 함수를 사용하여 저장한 값
            // - 비고 = EasyFinBankSearchDetail의 remark1, remark2, remark3 값
            // - 미입력시 전체조회
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
         * 수집 상태 확인(GetJobState API) 함수를 통해 상태 정보가 확인된 작업아이디를 활용하여 계좌 거래내역의 요약 정보를 조회합니다.
         * - 요약 정보 : 입·출 금액 합계, 입·출 거래 건수
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#Summary
         */
        public IActionResult Summary()
        {
            // 수집 요청(requestJob API)시 반환반은 작업아이디(jobID)
            string jobID = "020010816000000002";

            // 거래유형 배열 ("I" 와 "O" 중 선택, 다중 선택 가능)
            // └ I = 입금 , O = 출금
            // - 미입력 시 전체조회
            string[] TradeType = { "I", "O" };

            // "입·출금액" / "메모" / "비고" 중 검색하고자 하는 값 입력
            // - 메모 = 거래내역 메모저장(SaveMemo API) 함수를 사용하여 저장한 값
            // - 비고 = EasyFinBankSearchDetail의 remark1, remark2, remark3 값
            // - 미입력시 전체조회
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
         * 한 건의 거래 내역에 메모를 저장합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#SaveMemo
         */
        public IActionResult SaveMemo()
        {
            string tid = "";

            string memo = "memo";

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

        #endregion

        #region 정액제 신청 / 포인트 관리

        /*
         * 계좌조회 정액제 서비스 신청 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
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
         * 계좌조회 정액제 서비스 상태를 확인합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#GetFlatRateState
         */
        public IActionResult GetFlatRateState()
        {
            // 기관코드
            string BankCode = "";

            // 계좌번호
            string AccountNumber = "";
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
         * 연동회원의 잔여포인트를 확인합니다.
         * - 과금방식이 파트너과금인 경우 파트너 잔여포인트 확인(GetPartnerBalance API) 함수를 통해 확인하시기 바랍니다.
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
         * 연동회원 포인트 충전을 위한 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
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
        * 연동회원 포인트 결제내역 확인을 위한 페이지의 팝업 URL을 반환합니다.
        * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
        * - https://docs.popbill.com/easyfinbank/dotnetcore/api#GetPaymentURL
        */
        public IActionResult GetPaymentURL()
        {

            try
            {
                var result = _easyFinBankService.GetPaymentURL(corpNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 포인트 사용내역 확인을 위한 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#GetUseHistoryURL
         */
        public IActionResult GetUseHistoryURL()
        {

            try
            {
                var result = _easyFinBankService.GetUseHistoryURL(corpNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        /*
         * 파트너의 잔여포인트를 확인합니다.
         * - 과금방식이 연동과금인 경우 연동회원 잔여포인트 확인(GetBalance API) 함수를 이용하시기 바랍니다.
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
         * 파트너 포인트 충전을 위한 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
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
         * 팝빌 계좌조회 API 서비스 과금정보를 확인합니다.
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

        #endregion

        #region 회원정보

        /*
         * 사업자번호를 조회하여 연동회원 가입여부를 확인합니다.
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
         * 사용하고자 하는 아이디의 중복여부를 확인합니다.
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
         * 사용자를 연동회원으로 가입처리합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#JoinMember
         */
        public IActionResult JoinMember()
        {
            JoinForm joinInfo = new JoinForm();

            // 링크아이디
            joinInfo.LinkID = "TESTER";

            // 아이디, 6자이상 50자 미만
            joinInfo.ID = "userid_20181212";

            // 비밀번호, 8자이상 20자 미만 (영문, 숫자, 특수문자 조합)
            joinInfo.Password = "asdfasdf123!@#";

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
            joinInfo.ContactEmail = "";

            // 담당자 연락처 (최대 20자)
            joinInfo.ContactTEL = "";

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
         * 팝빌 사이트에 로그인 상태로 접근할 수 있는 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
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
         * 연동회원 사업자번호에 담당자(팝빌 로그인 계정)를 추가합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#RegistContact
         */
        public IActionResult RegistContact()
        {
            Contact contactInfo = new Contact();

            // 담당자 아이디, 6자 이상 50자 미만
            contactInfo.id = "testkorea_20181212";

            // 비밀번호, 8자이상 20자 미만 (영문, 숫자, 특수문자 조합)
            contactInfo.Password = "asdfasdf123!@#";

            // 담당자명 (최대 100자)
            contactInfo.personName = "코어담당자";

            // 담당자 연락처 (최대 20자)
            contactInfo.tel = "";

            // 담당자 이메일 (최대 100자)
            contactInfo.email = "";

            // 담당자 조회권한 설정, 1(개인권한), 2 (읽기권한), 3 (회사권한)
            contactInfo.searchRole = 3;

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
         * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 정보을 확인합니다.
         * - https://docs.popbill.com/easyfinbank/dotnetcore/api#GetContactInfo
         */
        public IActionResult GetContactInfo()
        {
            // 확인할 담당자 아이디
            string contactID = "test0730";

            try
            {
                var contactInfo = _easyFinBankService.GetContactInfo(corpNum, contactID, userID);
                return View("GetContactInfo", contactInfo);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 목록을 확인합니다.
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
         * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 정보를 수정합니다.
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
            contactInfo.tel = "";

            // 담당자 이메일 (최대 10자)
            contactInfo.email = "";

            // 담당자 조회권한 설정, 1(개인권한), 2 (읽기권한), 3 (회사권한)
            contactInfo.searchRole = 3;

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

        #endregion
    }
}
