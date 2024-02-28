/*
 * 팝빌 홈택스 현금영수증 API .NET Core SDK Example
 * .NET Core 연동 튜토리얼 안내 : https://developers.popbill.com/guide/htcashbill/dotnetcore/getting-started/tutorial
 * 
 * 업데이트 일자 : 2024-02-26
 * 연동 기술지원 연락처 : 1600 - 9854
 * 연동 기술지원 이메일 : code@linkhubcorp.com
 * 
 * <테스트 연동개발 준비사항>
 * 1) 홈택스 로그인 인증정보를 등록합니다. (부서사용자등록 / 공동인증서 등록)
 *   - 팝빌로그인 > [홈택스연동] > [환경설정] > [인증 관리] 메뉴
 *   - 홈택스연동 인증 관리 팝업 URL(GetCertificatePopUpURL API) 반환된 URL을 이용하여
 *     홈택스 인증 처리를 합니다.
*/
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
            // 홈택스연동(현금영수증) 서비스 객체 생성
            _htCashbillService = HTCashbill.htCashbillService;

        }

        // 팝빌 연동회원 사업자번호 (하이픈 '-' 없이 10자리)
        string corpNum = "1234567890";

        // 팝빌 연동회원 아이디
        string userID = "testkorea";

        /*
         * 홈택스연동(현금영수증) Index page (HTCashbill/Index.cshtml)
         */
        public IActionResult Index()
        {
            return View();
        }

        #region 홈택스 현금영수증 매입/매출 내역 수집

        /*
         * 홈택스에 신고된 현금영수증 매입/매출 내역 수집을 팝빌에 요청합니다. (조회기간 단위 : 최대 3개월)
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/job#RequestJob
         */
        public IActionResult RequestJob()
        {
            // 현금영수증 유형 SELL-매출, BUY-매입
            KeyType keyType = KeyType.SELL;

            // 시작일자, 표시형식(yyyyMMdd)
            string SDate = "20220501";

            // 종료일자, 표시형식(yyyyMMdd)
            string EDate = "20220527";

            try
            {
                var result = _htCashbillService.RequestJob(corpNum, keyType, SDate, EDate);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         *  수집 요청(RequestJob API) 함수를 통해 반환 받은 작업 아이디의 상태를 확인합니다.
         * - 수집 결과 조회(Search API) 함수 또는 수집 결과 요약 정보 조회(Summary API) 함수를 사용하기 전에
         *   수집 작업의 진행 상태, 수집 작업의 성공 여부를 확인해야 합니다.
         * - 작업 상태(jobState) = 3(완료)이고 수집 결과 코드(errorCode) = 1(수집성공)이면
         *   수집 결과 내역 조회(Search) 또는 수집 결과 요약 정보 조회(Summary)를 해야합니다.
         * - 작업 상태(jobState)가 3(완료)이지만 수집 결과 코드(errorCode)가 1(수집성공)이 아닌 경우에는
         *   오류메시지(errorReason)로 수집 실패에 대한 원인을 파악할 수 있습니다.
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/job#GetJobState
         */
        public IActionResult GetJobState()
        {
            // 수집 요청(requestJob API)시 반환반은 작업아이디(jobID)
            string jobID = "018112709000000001";

            try
            {
                var response = _htCashbillService.GetJobState(corpNum, jobID);
                return View("GetJobState", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 현금영수증 매입/매출 내역 수집요청에 대한 상태 목록을 확인합니다.
         * - 수집 요청 후 1시간이 경과한 수집 요청건은 상태정보가 반환되지 않습니다.
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/job#ListActiveJob
         */
        public IActionResult ListActiveJob()
        {
            try
            {
                var response = _htCashbillService.ListActiveJob(corpNum);
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
         * 함수 GetJobState(수집 상태 확인)를 통해 상태 정보 확인된 작업아이디를 활용하여 현금영수증 매입/매출 내역을 조회합니다.
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/search#Search
         */
        public IActionResult Search()
        {
            // 수집 요청(requestJob API)시 반환반은 작업아이디(jobID)
            string jobID = "018112709000000001";

            // 문서형태 배열 ("N" 와 "C" 중 선택, 다중 선택 가능)
            // └ N = 일반 현금영수증 , C = 취소현금영수증
            // - 미입력 시 전체조회
            string[] TradeType = {"N", "C"};

            // 거래구분 배열 ("P" 와 "C" 중 선택, 다중 선택 가능)
            // └ P = 소득공제용 , C = 지출증빙용
            // - 미입력 시 전체조회
            string[] TradeUsage = {"P", "C"};

            // 페이지 번호, 기본값 '1'
            int Page = 1;

            // 페이지당 검색개수, 기본값 '500', 최대 '1000'
            int PerPage = 30;

            // 정렬방향, A-오름차순, D-내림차순
            string Order = "D";

            try
            {
                var response = _htCashbillService.Search(corpNum, jobID, TradeType, TradeUsage, Page, PerPage, Order);
                return View("Search", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 수집 상태 확인(GetJobState API) 함수를 통해 상태 정보가 확인된 작업아이디를 활용하여 수집된 현금영수증 매입/매출 내역의 요약 정보를 조회합니다.
         * - 요약 정보 : 현금영수증 수집 건수, 공급가액 합계, 세액 합계, 봉사료 합계, 합계 금액
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/search#Summary
         */
        public IActionResult Summary()
        {
            // 수집 요청(requestJob API)시 반환반은 작업아이디(jobID)
            string jobID = "018112709000000001";

            // 문서형태 배열 ("N" 와 "C" 중 선택, 다중 선택 가능)
            // └ N = 일반 현금영수증 , C = 취소현금영수증
            // - 미입력 시 전체조회
            string[] TradeType = { "N", "C" };

            // 거래구분 배열 ("P" 와 "C" 중 선택, 다중 선택 가능)
            // └ P = 소득공제용 , C = 지출증빙용
            // - 미입력 시 전체조회
            string[] TradeUsage = { "P", "C" };

            try
            {
                var response = _htCashbillService.Summary(corpNum, jobID, TradeType, TradeUsage);
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
         * 홈택스연동 인증정보를 관리하는 페이지의 팝업 URL을 반환합니다.
         * - 인증방식에는 부서사용자/공인인증서 인증 방식이 있습니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/cert#GetCertificatePopUpURL
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
         * 홈택스연동 인증을 위해 팝빌에 등록된 인증서 만료일자를 확인합니다.
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/cert#GetCertificateExpireDate
         */
        public IActionResult GetCertificateExpireDate()
        {
            try
            {
                var result = _htCashbillService.GetCertificateExpireDate(corpNum);
                return View("Result", result.ToString("yyyyMMddHHmmss"));
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌에 등록된 인증서로 홈택스 로그인 가능 여부를 확인합니다.
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/cert#CheckCertValidation
         */
        public IActionResult CheckCertValidation()
        {
            try
            {
                var response = _htCashbillService.CheckCertValidation(corpNum);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 홈택스연동 인증을 위해 팝빌에 현금영수증 자료조회 부서사용자 계정을 등록합니다.
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/cert#RegistDeptUser
         */
        public IActionResult RegistDeptUser()
        {
            // 홈택스에서 생성한 현금영수증 부서사용자 아이디
            string deptUserID = "userid_test";

            // 홈택스에서 생성한 현금영수증 부서사용자 비밀번호
            string deptUserPWD = "passwd_test";

            try
            {
                var response = _htCashbillService.RegistDeptUser(corpNum, deptUserID, deptUserPWD);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 홈택스연동 인증을 위해 팝빌에 등록된 현금영수증 자료조회 부서사용자 계정을 확인합니다.
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/cert#CheckDeptUser
         */
        public IActionResult CheckDeptUser()
        {
            try
            {
                var response = _htCashbillService.CheckDeptUser(corpNum);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌에 등록된 현금영수증 자료조회 부서사용자 계정 정보로 홈택스 로그인 가능 여부를 확인합니다.
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/cert#CheckLoginDeptUser
         */
        public IActionResult CheckLoginDeptUser()
        {
            try
            {
                var response = _htCashbillService.CheckLoginDeptUser(corpNum);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌에 등록된 홈택스 현금영수증 자료조회 부서사용자 계정을 삭제합니다.
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/cert#DeleteDeptUser
         */
        public IActionResult DeleteDeptUser()
        {
            try
            {
                var response = _htCashbillService.DeleteDeptUser(corpNum);
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
         * 홈택스연동 정액제 서비스 신청 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/point#GetFlatRatePopUpURL
         */
        public IActionResult GetFlatRatePopUpURL()
        {
            try
            {
                var result = _htCashbillService.GetFlatRatePopUpURL(corpNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 홈택스연동 정액제 서비스 상태를 확인합니다.
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/point#GetFlatRateState
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

        /*
         * 연동회원의 잔여포인트를 확인합니다.
         * - 과금방식이 파트너과금인 경우 파트너 잔여포인트 확인(GetPartnerBalance API) 함수를 통해 확인하시기 바랍니다.
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/point#GetBalance
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
         * 연동회원 포인트 충전을 위한 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/point#GetChargeURL
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
         * 연동회원 포인트 결제내역 확인을 위한 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/point#GetPaymentURL
         */
        public IActionResult GetPaymentURL()
        {

            try
            {
                var result = _htCashbillService.GetPaymentURL(corpNum, userID);
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
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/point#GetUseHistoryURL
         */
        public IActionResult GetUseHistoryURL()
        {

            try
            {
                var result = _htCashbillService.GetUseHistoryURL(corpNum, userID);
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
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/point#GetPartnerBalance
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
         * 파트너 포인트 충전을 위한 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/point#GetPartnerURL
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
         * 팝빌 홈택스연동(현금) API 서비스 과금정보를 확인합니다.
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/point#GetChargeInfo
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
         * 연동회원 포인트를 환불 신청합니다.
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/point#Refund
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

                var response = _htCashbillService.Refund(corpNum, refundForm);
                return View("RefundResponse", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 포인트를 환불 신청합니다.
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/point#PaymentRequest
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

                var response = _htCashbillService.PaymentRequest(corpNum, paymentForm);
                return View("PaymentResponse", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 포인트 무통장 입금신청내역 1건을 확인합니다.
         *  - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/point#GetSettleResult
         */
        public IActionResult GetSettleResult()
        {
            // 정산코드
            var settleCode = "202301130000000026";

            try
            {
                var paymentHistory = _htCashbillService.GetSettleResult(corpNum, settleCode);

                return View("PaymentHistory", paymentHistory);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 포인트 사용내역을 확인합니다.
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/point#GetUseHistory
         */
        public IActionResult GetUseHistory()
        {
            // 조회 기간의 시작일자 (형식 : yyyyMMdd)
            var SDate = "20230102";

            // 조회 기간의 종료일자 (형식 : yyyyMMdd)
            var EDate = "20230131";

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
                var useHistoryResult = _htCashbillService.GetUseHistory(corpNum, SDate, EDate, Page,
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
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/point#GetPaymentHistory
         */
        public IActionResult GetPaymentHistory()
        {
            // 조회 기간의 시작일자 (형식 : yyyyMMdd)
            var SDate = "20230102";

            // 조회 기간의 종료일자 (형식 : yyyyMMdd)
            var EDate = "20230131";

            // 목록 페이지번호 (기본값 1)
            var Page = 1;

            // 페이지당 표시할 목록 개수 (기본값 500, 최대 1,000)
            var PerPage = 100;

            try
            {
                var paymentHistoryResult = _htCashbillService.GetPaymentHistory(corpNum, SDate, EDate,
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
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/point#GetRefundHistory
         */
        public IActionResult GetRefundHistory()
        {
            // 목록 페이지번호 (기본값 1)
            var Page = 1;

            // 페이지당 표시할 목록 개수 (기본값 500, 최대 1,000)
            var PerPage = 100;

            try
            {
                var refundHistoryResult = _htCashbillService.GetRefundHistory(corpNum, Page, PerPage);

                return View("RefundHistoryResult", refundHistoryResult);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        /*
         * 포인트 환불에 대한 상세정보 1건을 확인합니다.
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/point#GetRefundInfo
         */
        public IActionResult GetRefundInfo()
        {
            // 환불 코드
            var refundCode = "023040000017";

            try
            {
                var response = _htCashbillService.GetRefundInfo(corpNum, refundCode);
                return View("RefundHistory", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        /*
         * 환불 가능한 포인트를 확인합니다. (보너스 포인트는 환불가능포인트에서 제외됩니다.)
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/point#GetRefundableBalance
         */
        public IActionResult GetRefundableBalance()
        {
            try
            {
                var refundableBalance = _htCashbillService.GetRefundableBalance(corpNum);
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
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/member#CheckIsMember
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
         * 사용하고자 하는 아이디의 중복여부를 확인합니다.
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/member#CheckID
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
         * 사용자를 연동회원으로 가입처리합니다.
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/member#JoinMember
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
                var response = _htCashbillService.JoinMember(joinInfo);
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
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/member#GetAccessURL
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
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/member#GetCorpInfo
         */
        public IActionResult GetCorpInfo()
        {
            try
            {
                var response = _htCashbillService.GetCorpInfo(corpNum);
                return View("GetCorpInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 회사정보를 수정합니다.
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/member#UpdateCorpInfo
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
                var response = _htCashbillService.UpdateCorpInfo(corpNum, corpInfo);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 담당자(팝빌 로그인 계정)를 추가합니다.
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/member#RegistContact
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
                var response = _htCashbillService.RegistContact(corpNum, contactInfo);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
        * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 정보을 확인합니다.
        * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/member#GetContactInfo
        */
        public IActionResult GetContactInfo()
        {
            // 확인할 담당자 아이디
            string contactID = "test0730";

            try
            {
                var contactInfo = _htCashbillService.GetContactInfo(corpNum, contactID);
                return View("GetContactInfo", contactInfo);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 목록을 확인합니다.
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/member#ListContact
         */
        public IActionResult ListContact()
        {
            try
            {
                var response = _htCashbillService.ListContact(corpNum);
                return View("ListContact", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 정보를 수정합니다.
         * - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/member#UpdateContact
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
                var response = _htCashbillService.UpdateContact(corpNum, contactInfo);
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
         *  - https://developers.popbill.com/reference/htcashbill/dotnetcore/api/member#QuitMember
         */
        public IActionResult QuitMember()
        {
            // 탈퇴 사유
            var quitReason = "탈퇴사유";

            try
            {
                var response = _htCashbillService.QuitMember(corpNum, quitReason);
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
