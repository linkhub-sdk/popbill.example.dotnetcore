/*
 * 팝빌 홈택스 전자세금계산서 API .NET Core SDK Example
 * .NET Core 연동 튜토리얼 안내 : https://developers.popbill.com/guide/httaxinvoice/dotnetcore/getting-started/tutorial
 * 
 * 업데이트 일자 : 2025-09-29
 * 연동 기술지원 연락처 : 1600 - 9854
 * 연동 기술지원 이메일 : code@linkhubcorp.com
 * 
 * <테스트 연동개발 준비사항>
 * 1) 홈택스 로그인 인증정보를 등록합니다. (부서사용자등록 / 공동인증서 등록)
 *   - 팝빌로그인 > [홈택스수집] > [환경설정] > [인증 관리] 메뉴
 *   - 홈택스수집 인증 관리 팝업 URL(GetCertificatePopUpURL API) 반환된 URL을 이용하여
 *     홈택스 인증 처리를 합니다.
*/
using Microsoft.AspNetCore.Mvc;
using Popbill;
using Popbill.HomeTax;


namespace HTTaxinvoiceExample.Controllers
{
    public class HTTaxinvoiceController : Controller
    {
        private readonly HTTaxinvoiceService _htTaxinvoiceService;

        public HTTaxinvoiceController(HTTaxinvoiceInstance HTTaxinvoice)
        {
            // 홈택스수집(전자세금계산서) 서비스 객체 생성
            _htTaxinvoiceService = HTTaxinvoice.htTaxinvoiceService;

        }

        // 팝빌 연동회원 사업자번호 (하이픈 '-' 없이 10자리)
        string corpNum = "1234567890";

        // 팝빌 연동회원 아이디
        string userID = "testkorea";

        /*
         * 홈택스수집(전자세금계산서) Index page (HTTaxinvoice/Index.cshtml)
         */
        public IActionResult Index()
        {
            return View();
        }

        #region 홈택스 전자세금계산서 매입/매출 내역 수집

        /*
         * 홈택스에 신고된 전자세금계산서 매입/매출 내역 수집을 팝빌에 요청합니다. (조회기간 단위 : 최대 3개월)
         * - 주기적으로 자체 DB에 세금계산서 정보를 INSERT 하는 경우, 조회할 일자 유형(DType) 값을 "S"로 하는 것을 권장합니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/api/job#RequestJob
         */
        public IActionResult RequestJob()
        {
            // 전자세금계산서 유형 SELL-매출, BUY-매입, TRUSTEE-위수탁
            KeyType keyType = KeyType.SELL;

            // 일자유형, W-작성일자, I-발행일자, S-전송일자
            string DType = "S";

            // 시작일자, 표시형식(yyyyMMdd)
            string SDate = "20250701";

            // 종료일자, 표시형식(yyyyMMdd)
            string EDate = "20250731";

            try
            {
                var result = _htTaxinvoiceService.RequestJob(corpNum, keyType, DType, SDate, EDate);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 수집 요청(RequestJob API) 함수를 통해 반환 받은 작업 아이디의 상태를 확인합니다.
         * - 수집 결과 조회(Search API) 함수 또는 수집 결과 요약 정보 조회(Summary API) 함수를 사용하기 전에
         *   수집 작업의 진행 상태, 수집 작업의 성공 여부를 확인해야 합니다.
         * - 작업 상태(jobState) = 3(완료)이고 수집 결과 코드(errorCode) = 1(수집성공)이면
         *   수집 결과 내역 조회(Search) 또는 수집 결과 요약 정보 조회(Summary)를 해야합니다.
         * - 작업 상태(jobState)가 3(완료)이지만 수집 결과 코드(errorCode)가 1(수집성공)이 아닌 경우에는
         *   오류메시지(errorReason)로 수집 실패에 대한 원인을 파악할 수 있습니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/api/job#GetJobState
         */
        public IActionResult GetJobState()
        {
            // 수집 요청(requestJob API)시 반환반은 작업아이디(jobID)
            string jobID = "018120517000000006";

            try
            {
                var response = _htTaxinvoiceService.GetJobState(corpNum, jobID);
                return View("GetJobState", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 전자세금계산서 매입/매출 내역 수집요청에 대한 상태 목록을 확인합니다.
         * - 수집 요청 후 1시간이 경과한 수집 요청건은 상태정보가 반환되지 않습니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/api/job#ListActiveJob
         */
        public IActionResult ListActiveJob()
        {
            try
            {
                var response = _htTaxinvoiceService.ListActiveJob(corpNum);
                return View("ListActiveJob", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        #endregion

        #region 홈택스 전자세금계산서 매입/매출 내역 수집 결과 조회

        /*
         * 함수 GetJobState(수집 상태 확인)를 통해 상태 정보가 확인된 작업아이디를 활용하여 수집된 전자세금계산서 매입/매출 내역을 조회합니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/api/search#Search
         */
        public IActionResult Search()
        {
            // 수집 요청(requestJob API)시 반환반은 작업아이디(jobID)
            string jobID = "018120517000000006";

            // 문서형태 배열 ("N" 와 "M" 중 선택, 다중 선택 가능)
            // └ N = 일반 , M = 수정
            // - 미입력 시 전체조회
            string[] Type = {"N", "M"};

            // 과세형태 배열 ("T" , "N" , "Z" 중 선택, 다중 선택 가능)
            // └ T = 과세, N = 면세, Z = 영세
            // - 미입력 시 전체조회
            string[] TaxType = {"T", "N", "Z"};

            // 발행목적 배열 ("R", "C", "N" 중 선택, 다중 선택 가능)
            // └ R = 영수, C = 청구, N = 없음
            // - 미입력 시 전체조회
            string[] PurposeType = {"R", "C", "N"};

            // 종사업장번호 유무 (null , "0" , "1" 중 택 1)
            // - null = 전체조회 , 0 = 없음, 1 = 있음
            string TaxRegIDYN = "";

            // 종사업장번호의 주체 ("S" , "B" , "T" 중 택 1)
            // - S = 공급자 , B = 공급받는자 , T = 수탁자
            string TaxRegIDType = "S";

            // 종사업장번호
            // - 다수기재 시 콤마(",")로 구분. ex) "0001,0002"
            // - 미입력 시 전체조회
            string TaxRegID = "";

            // 페이지 번호, 기본값 '1'
            int Page = 1;

            // 페이지당 검색개수, 기본값 '500', 최대 '1000'
            int PerPage = 30;

            // 정렬방향, A-오름차순, D-내림차순
            string Order = "D";

            // 거래처 상호 / 사업자번호 (사업자) / 주민등록번호 (개인) / "9999999999999" (외국인) 중 검색하고자 하는 정보 입력
            // - 사업자번호 / 주민등록번호는 하이픈('-')을 제외한 숫자만 입력
            // - 미입력시 전체조회
            string SearchString = "";

            try
            {
                var response = _htTaxinvoiceService.Search(corpNum, jobID, Type, TaxType, PurposeType, TaxRegIDYN,
                    TaxRegIDType, TaxRegID, Page, PerPage, Order, userID, SearchString);
                return View("Search", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 수집 상태 확인(GetJobState API) 함수를 통해 상태 정보가 확인된 작업아이디를 활용하여 수집된 전자세금계산서 매입/매출 내역의 요약 정보를 조회합니다.
         * - 요약 정보 : 전자세금계산서 수집 건수, 공급가액 합계, 세액 합계, 합계 금액
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/api/search#Summary
         */
        public IActionResult Summary()
        {
            // 수집 요청(requestJob API)시 반환반은 작업아이디(jobID)
            string jobID = "018120517000000006";

            // 문서형태 배열 ("N" 와 "M" 중 선택, 다중 선택 가능)
            // └ N = 일반 , M = 수정
            // - 미입력 시 전체조회
            string[] Type = {"N", "M"};

            // 과세형태 배열 ("T" , "N" , "Z" 중 선택, 다중 선택 가능)
            // └ T = 과세, N = 면세, Z = 영세
            // - 미입력 시 전체조회
            string[] TaxType = {"T", "N", "Z"};

            // 발행목적 배열 ("R" , "C", "N" 중 선택, 다중 선택 가능)
            // └ R = 영수, C = 청구, N = 없음
            // - 미입력 시 전체조회
            string[] PurposeType = {"R", "C", "N"};

            // 종사업장번호 유무 (null , "0" , "1" 중 택 1)
            // - null = 전체조회 , 0 = 없음, 1 = 있음
            string TaxRegIDYN = "";

            // 종사업장번호의 주체 ("S" , "B" , "T" 중 택 1)
            // - S = 공급자 , B = 공급받는자 , T = 수탁자
            string TaxRegIDType = "S";

            // 종사업장번호
            // - 다수기재 시 콤마(",")로 구분. ex) "0001,0002"
            // - 미입력 시 전체조회
            string TaxRegID = "";

            // 거래처 상호 / 사업자번호 (사업자) / 주민등록번호 (개인) / "9999999999999" (외국인) 중 검색하고자 하는 정보 입력
            // - 사업자번호 / 주민등록번호는 하이픈('-')을 제외한 숫자만 입력
            // - 미입력시 전체조회
            string SearchString = "";

            try
            {
                var response = _htTaxinvoiceService.Summary(corpNum, jobID, Type, TaxType, PurposeType, TaxRegIDYN,
                    TaxRegIDType, TaxRegID, userID, SearchString);
                return View("Summary", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 국세청 승인번호를 통해 수집한 전자세금계산서 1건의 상세정보를 반환합니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/api/search#GetTaxinvoice
         */
        public IActionResult GetTaxinvoice()
        {
            // 조회할 전자세금계산서 국세청 승인번호
            string ntsConfirmNum = "201812044100020300000c0a";

            try
            {
                var response = _htTaxinvoiceService.GetTaxinvoice(corpNum, ntsConfirmNum);
                return View("GetTaxinvoice", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 국세청 승인번호를 통해 수집한 전자세금계산서 1건의 상세정보를 XML 형태의 문자열로 반환합니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/api/search#GetXML
         */
        public IActionResult GetXML()
        {
            // 조회할 전자세금계산서 국세청 승인번호
            string ntsConfirmNum = "201812044100020300000c0a";

            try
            {
                var response = _htTaxinvoiceService.GetXML(corpNum, ntsConfirmNum);
                return View("GetXML", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 수집된 전자세금계산서 1건의 상세내역을 확인하는 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/api/search#GetPopUpURL
         */
        public IActionResult GetPopUpURL()
        {
            // 조회할 전자세금계산서 국세청 승인번호
            string ntsConfirmNum = "201812044100020300000c0a";

            try
            {
                var result = _htTaxinvoiceService.GetPopUpURL(corpNum, ntsConfirmNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 수집된 전자세금계산서 1건의 상세내역을 인쇄하는 페이지의 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/api/search#GetPrintURL
         */
        public IActionResult GetPrintURL()
        {
            // 조회할 전자세금계산서 국세청 승인번호
            string ntsConfirmNum = "201812044100020300000c0a";

            try
            {
                var result = _htTaxinvoiceService.GetPrintURL(corpNum, ntsConfirmNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        #endregion

        #region 홈택스수집 인증 관리

        /*
         * 홈택스수집 인증정보를 관리하는 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/api/cert#GetCertificatePopUpURL
         */
        public IActionResult GetCertificatePopUpURL()
        {
            try
            {
                var result = _htTaxinvoiceService.GetCertificatePopUpURL(corpNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌에 등록된 인증서 만료일자를 확인합니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/api/cert#GetCertificateExpireDate
         */
        public IActionResult GetCertificateExpireDate()
        {
            try
            {
                var result = _htTaxinvoiceService.GetCertificateExpireDate(corpNum);

                return View("Result", result.ToString("yyyyMMddHHmmss"));
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌에 등록된 인증서로 홈택스 로그인 가능 여부를 확인합니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/api/cert#CheckCertValidation
         */
        public IActionResult CheckCertValidation()
        {
            try
            {
                var response = _htTaxinvoiceService.CheckCertValidation(corpNum);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

		/*
         * 팝빌에 전자세금계산서 전용 부서사용자를 등록합니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/api/cert#RegistDeptUser
         */
		public IActionResult RegistDeptUser()
        {
            // 부서사용자 아이디
            string deptUserID = "userid_test";

            // 부서사용자 비밀번호
            string deptUserPWD = "passwd_test";

			// 부서사용자 대표자 주민번호
			string identityNum = "";

			// 팝빌회원 아이디
			string userID = "testkorea";

			try
            {
                var response = _htTaxinvoiceService.RegistDeptUser(corpNum, deptUserID, deptUserPWD, identityNum, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 홈택스수집 인증을 위해 팝빌에 등록된 전자세금계산서용 부서사용자 계정을 확인합니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/api/cert#CheckDeptUser
         */
        public IActionResult CheckDeptUser()
        {
            try
            {
                var response = _htTaxinvoiceService.CheckDeptUser(corpNum);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌에 등록된 전자세금계산서용 부서사용자 계정 정보로 홈택스 로그인 가능 여부를 확인합니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/api/cert#CheckLoginDeptUser
         */
        public IActionResult CheckLoginDeptUser()
        {
            try
            {
                var response = _htTaxinvoiceService.CheckLoginDeptUser(corpNum);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌에 등록된 홈택스 전자세금계산서용 부서사용자 계정을 삭제합니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/api/cert#DeleteDeptUser
         */
        public IActionResult DeleteDeptUser()
        {
            try
            {
                var response = _htTaxinvoiceService.DeleteDeptUser(corpNum);
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
         * 홈택스수집 정액제 서비스 신청 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/common-api/point#GetFlatRatePopUpURL
         */
        public IActionResult GetFlatRatePopUpURL()
        {
            try
            {
                var result = _htTaxinvoiceService.GetFlatRatePopUpURL(corpNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 홈택스수집 정액제 서비스 상태를 확인합니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/common-api/point#GetFlatRateState
         */
        public IActionResult GetFlatRateState()
        {
            try
            {
                var response = _htTaxinvoiceService.GetFlatRateState(corpNum);
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
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/common-api/point#GetBalance
         */
        public IActionResult GetBalance()
        {
            try
            {
                var result = _htTaxinvoiceService.GetBalance(corpNum);
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
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/common-api/point#GetChargeURL
         */
        public IActionResult GetChargeURL()
        {
            try
            {
                var result = _htTaxinvoiceService.GetChargeURL(corpNum, userID);
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
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/common-api/point#GetPaymentURL
         */
        public IActionResult GetPaymentURL()
        {

            try
            {
                var result = _htTaxinvoiceService.GetPaymentURL(corpNum, userID);
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
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/common-api/point#GetUseHistoryURL
         */
        public IActionResult GetUseHistoryURL()
        {

            try
            {
                var result = _htTaxinvoiceService.GetUseHistoryURL(corpNum, userID);
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
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/common-api/point#GetPartnerBalance
         */
        public IActionResult GetPartnerBalance()
        {
            try
            {
                var result = _htTaxinvoiceService.GetPartnerBalance(corpNum);
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
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/common-api/point#GetPartnerURL
         */
        public IActionResult GetPartnerURL()
        {
            // CHRG 포인트충전 URL
            string TOGO = "CHRG";

            try
            {
                var result = _htTaxinvoiceService.GetPartnerURL(corpNum, TOGO);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌 홈택스수집(세금) API 서비스 과금정보를 확인합니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/common-api/point#GetChargeInfo
         */
        public IActionResult GetChargeInfo()
        {
            try
            {
                var response = _htTaxinvoiceService.GetChargeInfo(corpNum);
                return View("GetChargeInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

                /*
         * 연동회원 포인트를 환불 신청합니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/common-api/point#Refund
         */
        public IActionResult Refund()
        {
            try
            {
                var refundForm = new RefundForm();
                
                // 담당자명
                refundForm.contactName= "담당자명";

                // 담당자 연락처
                refundForm.tel="01077777777";

                // 환불 신청 포인트
                refundForm.requestPoint = "10";

                // 은행명
                refundForm.accountBank ="국민";

                // 계좌번호
                refundForm.accountNum ="123123123-123" ;

                // 예금주명
                refundForm.accountName = "예금주명";

                // 환불사유
                refundForm.reason = "환불사유";

                var response = _htTaxinvoiceService.Refund(corpNum, refundForm);
                return View("RefundResponse", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 포인트를 환불 신청합니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/common-api/point#PaymentRequest
         */
        public IActionResult PaymentRequest()
        {
            try
            {
                var paymentForm = new PaymentForm();

                // 담당자명
                paymentForm.settlerName = "담당자명";

                // 담당자 이메일
                paymentForm.settlerEmail = "test@test.com";

                // 담당자 휴대폰
                // └ 무통장 입금 승인 알림톡이 전송될 번호
                paymentForm.notifyHP = "01012341234";

                // 입금자명
                paymentForm.paymentName = "입금자명";

                // 결제금액
                paymentForm.settleCost = "11000";

                var response = _htTaxinvoiceService.PaymentRequest(corpNum, paymentForm);
                return View("PaymentResponse", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 포인트 무통장 입금신청내역 1건을 확인합니다.
         *  - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/common-api/point#GetSettleResult
         */
        public IActionResult GetSettleResult()
        {
            // 정산코드
            var settleCode = "202301130000000026";

            try
            {
                var paymentHistory = _htTaxinvoiceService.GetSettleResult(corpNum, settleCode);

                return View("PaymentHistory", paymentHistory);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 포인트 사용내역을 확인합니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/common-api/point#GetUseHistory
         */
        public IActionResult GetUseHistory()
        {
            // 조회 기간의 시작일자 (형식 : yyyyMMdd)
            var SDate = "20250701";

            // 조회 기간의 종료일자 (형식 : yyyyMMdd)
            var EDate = "20250731";

            // 목록 페이지번호 (기본값 1)
            var Page = 1;

            // 페이지당 표시할 목록 개수 (기본값 500, 최대 1,000)
            var PerPage = 100;

            // 거래일자를 기준으로 하는 목록 정렬 방향 : "D" / "A" 중 택 1
            // └ "D" : 내림차순
            // └ "A" : 오름차순
            // ※ 미입력시 기본값 "D" 처리
            var Order = "D";

            try
            {
                var useHistoryResult = _htTaxinvoiceService.GetUseHistory(corpNum, SDate, EDate, Page,
                    PerPage, Order);

                return View("UseHistoryResult", useHistoryResult);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 포인트 결제내역을 확인합니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/common-api/point#GetPaymentHistory
         */
        public IActionResult GetPaymentHistory()
        {
            // 조회 기간의 시작일자 (형식 : yyyyMMdd)
            var SDate = "20250701";

            // 조회 기간의 종료일자 (형식 : yyyyMMdd)
            var EDate = "20250731";

            // 목록 페이지번호 (기본값 1)
            var Page = 1;

            // 페이지당 표시할 목록 개수 (기본값 500, 최대 1,000)
            var PerPage = 100;

            try
            {
                var paymentHistoryResult = _htTaxinvoiceService.GetPaymentHistory(corpNum, SDate, EDate,
                    Page, PerPage);

                return View("PaymentHistoryResult", paymentHistoryResult);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 포인트 환불신청내역을 확인합니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/common-api/point#GetRefundHistory
         */
        public IActionResult GetRefundHistory()
        {
            // 목록 페이지번호 (기본값 1)
            var Page = 1;

            // 페이지당 표시할 목록 개수 (기본값 500, 최대 1,000)
            var PerPage = 100;

            try
            {
                var refundHistoryResult = _htTaxinvoiceService.GetRefundHistory(corpNum, Page, PerPage);

                return View("RefundHistoryResult", refundHistoryResult);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        /*
         * 포인트 환불에 대한 상세정보 1건을 확인합니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/common-api/point#GetRefundInfo
         */
        public IActionResult GetRefundInfo()
        {
            // 환불 코드
            var refundCode = "023040000017";

            try
            {
                var response = _htTaxinvoiceService.GetRefundInfo(corpNum, refundCode);
                return View("RefundHistory", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        /*
         * 환불 가능한 포인트를 확인합니다. (보너스 포인트는 환불가능포인트에서 제외됩니다.)
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/common-api/point#GetRefundableBalance
         */
        public IActionResult GetRefundableBalance()
        {
            try
            {
                var refundableBalance = _htTaxinvoiceService.GetRefundableBalance(corpNum);
                return View("RefundableBalance", refundableBalance);
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
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/common-api/member#CheckIsMember
         */
        public IActionResult CheckIsMember()
        {
            try
            {
                //링크아이디
                string linkID = "TESTER";

                var response = _htTaxinvoiceService.CheckIsMember(corpNum, linkID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 사용하고자 하는 아이디의 중복여부를 확인합니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/common-api/member#CheckID
         */
        public IActionResult CheckID()
        {
            //중복여부 확인할 팝빌 회원 아이디
            string checkID = "testkorea";

            try
            {
                var response = _htTaxinvoiceService.CheckID(checkID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 사용자를 연동회원으로 가입처리합니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/common-api/member#JoinMember
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

            // 담당자 메일 (최대 100자)
            joinInfo.ContactEmail = "";

            // 담당자 휴대폰 (최대 20자)
            joinInfo.ContactTEL = "";

            try
            {
                var response = _htTaxinvoiceService.JoinMember(joinInfo);
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
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/common-api/member#GetAccessURL
         */
        public IActionResult GetAccessURL()
        {
            try
            {
                var result = _htTaxinvoiceService.GetAccessURL(corpNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 회사정보를 확인합니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/common-api/member#GetCorpInfo
         */
        public IActionResult GetCorpInfo()
        {
            try
            {
                var response = _htTaxinvoiceService.GetCorpInfo(corpNum);
                return View("GetCorpInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 회사정보를 수정합니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/common-api/member#UpdateCorpInfo
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
                var response = _htTaxinvoiceService.UpdateCorpInfo(corpNum, corpInfo);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 담당자(팝빌 로그인 계정)를 추가합니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/common-api/member#RegistContact
         */
        public IActionResult RegistContact()
        {
            Contact contactInfo = new Contact();

            // 담당자 아이디, 6자 이상 50자 미만
            contactInfo.id = "testkorea_20181212";

            // 비밀번호, 8자이상 20자 미만 (영문, 숫자, 특수문자 조합)
            contactInfo.Password = "asdfasdf123!@#";

            // 담당자 성명 (최대 100자)
            contactInfo.personName = "코어담당자";

            // 담당자 휴대폰 (최대 20자)
            contactInfo.tel = "";

            // 담당자 메일 (최대 100자)
            contactInfo.email = "";

            // 권한, 1(개인권한), 2 (읽기권한), 3 (회사권한)
            contactInfo.searchRole = 3;

            try
            {
                var response = _htTaxinvoiceService.RegistContact(corpNum, contactInfo);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 담당자를 삭제합니다.
         * - https://developers.popbill.com/reference/accountcheck/dotnetcore/common-api/member#DeleteContact
         */
        public IActionResult DeleteContact()
        {
            // 삭제할 담당자 아이디
            string targetUserID = "test";

            try
            {
                var response = _htTaxinvoiceService.DeleteContact(corpNum, targetUserID, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 정보을 확인합니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/common-api/member#GetContactInfo
         */
        public IActionResult GetContactInfo()
        {
            // 확인할 담당자 아이디
            string contactID = "test0730";

            try
            {
                var contactInfo = _htTaxinvoiceService.GetContactInfo(corpNum, contactID);
                return View("GetContactInfo", contactInfo);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 목록을 확인합니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/common-api/member#ListContact
         */
        public IActionResult ListContact()
        {
            try
            {
                var response = _htTaxinvoiceService.ListContact(corpNum);
                return View("ListContact", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 정보를 수정합니다.
         * - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/common-api/member#UpdateContact
         */
        public IActionResult UpdateContact()
        {
            Contact contactInfo = new Contact();

            // 아이디
            contactInfo.id = "testkorea";

            // 담당자 성명 (최대 100자)
            contactInfo.personName = "코어담당자";

            // 담당자 휴대폰 (최대 20자)
            contactInfo.tel = "";

            // 담당자 메일 (최대 10자)
            contactInfo.email = "";

            // 권한, 1(개인권한), 2 (읽기권한), 3 (회사권한)
            contactInfo.searchRole = 3;

            try
            {
                var response = _htTaxinvoiceService.UpdateContact(corpNum, contactInfo);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 가입된 연동회원의 탈퇴를 요청합니다.
         *  - 회원탈퇴 신청과 동시에 팝빌의 모든 서비스 이용이 불가하며, 관리자를 포함한 모든 담당자 계정도 일괄탈퇴 됩니다.
         *  - 회원탈퇴로 삭제된 데이터는 복원이 불가능합니다.
         *  - 관리자 계정만 사용 가능합니다.
         *  - https://developers.popbill.com/reference/httaxinvoice/dotnetcore/common-api/member#QuitMember
         */
        public IActionResult QuitMember()
        {
            // 탈퇴 사유
            var quitReason = "탈퇴사유";

            try
            {
                var response = _htTaxinvoiceService.QuitMember(corpNum, quitReason);
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
