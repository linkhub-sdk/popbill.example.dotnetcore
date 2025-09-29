/*
 * 팝빌 현금영수증 API .NET Core SDK Example
 * .NET Core 연동 튜토리얼 안내 : https://developers.popbill.com/guide/cashbill/dotnetcore/getting-started/tutorial
 * 
 * 업데이트 일자 : 2025-09-29
 * 연동 기술지원 연락처 : 1600 - 9854
 * 연동 기술지원 이메일 : code@linkhubcorp.com
*/
using Microsoft.AspNetCore.Mvc;
using Popbill;
using Popbill.Cashbill;
using System.Collections.Generic;

namespace CashbillExample.Controllers
{
    public class CashbillController : Controller
    {
        private readonly CashbillService _cashbillService;

        public CashbillController(CashbillInstance CBinstance)
        {
            //현금영수증 서비스 객체 생성
            _cashbillService = CBinstance.cashbillService;

        }

        //팝빌 연동회원 사업자번호 (하이픈 '-' 없이 10자리)
        string corpNum = "1234567890";

        //팝빌 연동회원 아이디
        string userID = "testkorea";

        /*
         * 현금영수증 Index page (Cashbill/Index.cshtml)
         */
        public IActionResult Index()
        {
            return View();
        }

        #region 현금영수증 / 취소현금영수증 발행

        /*
         * 파트너가 현금영수증 관리 목적으로 할당하는 문서번호 사용여부를 확인합니다.
         * - 이미 사용 중인 문서번호는 중복 사용이 불가하고, 현금영수증이 삭제된 경우에만 문서번호의 재사용이 가능합니다.
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/api/info#CheckMgtKeyInUse
         */
        public IActionResult CheckMgtKeyInUse()
        {
            try
            {
                // 현금영수증 문서번호
                string mgtKey = "20220527-001";

                bool result = _cashbillService.CheckMgtKeyInUse(corpNum, mgtKey);

                return View("result", result ? "사용중" : "미사용중");
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 작성된 현금영수증 데이터를 팝빌에 저장과 동시에 발행하여 "발행완료" 상태로 처리합니다.
         * - 현금영수증 국세청 전송 정책 : https://developers.popbill.com/guide/cashbill/dotnetcore/introduction/policy-of-send-to-nts
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/api/issue#RegistIssue
         */
        public IActionResult RegistIssue()
        {
            // 현금영수증 정보 객체
            Cashbill cashbill = new Cashbill();

            // 문서번호, 사업자별로 중복되지 않도록 문서번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            cashbill.mgtKey = "20220527-001";

            // 거래일시, 날짜(yyyyMMddHHmmss)
            // 당일, 전일만 가능 미입력시 기본값 발행일시 처리
            cashbill.tradeDT = "";

            // 문서형태, 승인거래 기재
            cashbill.tradeType = "승인거래";

            // 거래구분, { 소득공제용, 지출증빙용 } 중 기재
            cashbill.tradeUsage = "소득공제용";

            // 거래유형, { 일반, 도서공연, 대중교통 } 중 기재
            // - 미입력 시 기본값 "일반" 처리
            cashbill.tradeOpt = "일반";

            // 과세형태, { 과세, 비과세 } 중 기재
            cashbill.taxationType = "과세";

            // 거래금액 ( 공급가액 + 세액 + 봉사료 )
            cashbill.totalAmount = "11000";

            // 공급가액
            cashbill.supplyCost = "10000";

            // 부가세
            cashbill.tax = "1000";

            // 봉사료
            cashbill.serviceFee = "0";

            // 가맹점 사업자번호
            cashbill.franchiseCorpNum = corpNum;

            // 가맹점 종사업장 식별번호
            cashbill.franchiseTaxRegID = "";

            // 가맹점 상호
            cashbill.franchiseCorpName = "가맹점 상호";

            // 가맹점 대표자 성명
            cashbill.franchiseCEOName = "가맹점 대표자";

            // 가맹점 주소
            cashbill.franchiseAddr = "가맹점 주소";

            // 가맹점 전화번호
            cashbill.franchiseTEL = "";

            // 식별번호, 거래구분에 따라 작성
            // └ 소득공제용 - 주민등록/휴대폰/카드번호(현금영수증 카드)/자진발급용 번호(010-000-1234) 기재가능
            // └ 지출증빙용 - 사업자번호/주민등록/휴대폰/카드번호(현금영수증 카드) 기재가능
            // └ 주민등록번호 13자리, 휴대폰번호 10~11자리, 카드번호 13~19자리, 사업자번호 10자리 입력 가능
            cashbill.identityNum = "0100001234";

            // 구매자 성명
            cashbill.customerName = "주문자명";

            // 주문상품명
            cashbill.itemName = "주문상품명";

            // 주문번호
            cashbill.orderNumber = "주문번호";

            // 구매자 이메일
            // 팝빌 개발환경에서 테스트하는 경우에도 안내 메일이 전송되므로,
            // 실제 거래처의 메일주소가 기재되지 않도록 주의
            cashbill.email = "";

            // 구매자 휴대폰
            // - {smssendYN} 의 값이 true 인 경우 아래 휴대폰번호로 안내 문자 전송
            cashbill.hp = "";

            // 발행시 알림문자 전송여부
            cashbill.smssendYN = false;

            // 현금영수증 발행 메모
            string memo = "현금영수증 즉시발행 메모";

            // 메일제목, 공백처리시 기본양식으로 전송
            string emailSubject = "";

            try
            {
                var response = _cashbillService.RegistIssue(corpNum, cashbill, memo, userID, emailSubject);
                return View("IssueResponse", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 최대 100건의 현금영수증 발행을 한번의 요청으로 접수합니다.
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/api/issue#BulkSubmit
         */
        public IActionResult BulkSubmit()
        {
            // 제출아이디
            string submitID = "20221109-BULK";

            // 현금영수증 객체정보 목록
            List<Cashbill> cashbillList = new List<Cashbill>();

            for (int i = 0; i < 5; i++)
            {
                Cashbill cashbill = new Cashbill();

                // 문서번호, 최대 24자리, 영문, 숫자 '-', '_'를 조합하여 사업자별로 중복되지 않도록 구성
                cashbill.mgtKey = submitID + "-" + i;

                // [취소거래시 필수] 당초 승인 현금영수증 국세청승인번호
                cashbill.orgConfirmNum = "";

                // [취소거래시 필수] 당초 승인 현금영수증 거래일자
                cashbill.orgTradeDate = "";

                // 거래일시, 날짜(yyyyMMddHHmmss)
                // 당일, 전일만 가능 미입력시 기본값 발행일시 처리
                cashbill.tradeDT = "";

                // 문서형태, { 승인거래, 취소거래 } 중 기재
                cashbill.tradeType = "승인거래";

                // 거래구분, { 소득공제용, 지출증빙용 } 중 기재
                cashbill.tradeUsage = "소득공제용";

                // 거래유형, { 일반, 도서공연, 대중교통 } 중 기재
                cashbill.tradeOpt = "일반";

                // 과세형태, { 과세, 비과세 } 중 기재
                cashbill.taxationType = "과세";

                // 거래금액 ( 공급가액 + 세액 + 봉사료 )
                cashbill.totalAmount = "11000";

                // 공급가액
                cashbill.supplyCost = "10000";

                // 부가세
                cashbill.tax = "1000";

                // 봉사료
                cashbill.serviceFee = "0";

                // 가맹점 사업자번호
                cashbill.franchiseCorpNum = corpNum;

                // 가맹점 종사업장 식별번호
                cashbill.franchiseTaxRegID = "";

                // 가맹점 상호
                cashbill.franchiseCorpName = "가맹점 상호";

                // 가맹점 대표자 성명
                cashbill.franchiseCEOName = "가맹점 대표자 성명";

                // 가맹점 주소
                cashbill.franchiseAddr = "가맹점 주소";

                // 가맹점 전화번호
                cashbill.franchiseTEL = "";

                // 식별번호, 거래구분에 따라 작성
                // └ 소득공제용 - 주민등록/휴대폰/카드번호(현금영수증 카드)/자진발급용 번호(010-000-1234) 기재가능
                // └ 지출증빙용 - 사업자번호/주민등록/휴대폰/카드번호(현금영수증 카드) 기재가능
                // └ 주민등록번호 13자리, 휴대폰번호 10~11자리, 카드번호 13~19자리, 사업자번호 10자리 입력 가능
                cashbill.identityNum = "0100001234";

                // 구매자 성명
                cashbill.customerName = "주문자명";

                // 주문상품명
                cashbill.itemName = "주문상품명";

                // 주문번호
                cashbill.orderNumber = "주문번호";

                // 구매자 메일
                // 팝빌 개발환경에서 테스트하는 경우에도 안내 메일이 전송되므로,
                // 실제 거래처의 메일주소가 기재되지 않도록 주의
                cashbill.email = "";

                // 구매자 휴대폰
                // - {smssendYN} 의 값이 true 인 경우 아래 휴대폰번호로 안내 문자 전송
                cashbill.hp = "";

                // 발행시 알림문자 전송여부
                cashbill.smssendYN = false;

                cashbillList.Add(cashbill);
            }

            try
            {
                var bulkResponse = _cashbillService.BulkSubmit(corpNum, submitID, cashbillList);
                return View("BulkResponse", bulkResponse);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 접수시 기재한 SubmitID를 사용하여 현금영수증 접수결과를 확인합니다.
         * - 개별 현금영수증 처리상태는 접수상태(txState)가 완료(2) 시 반환됩니다.
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/api/issue#GetBulkResult
         */
        public IActionResult GetBulkResult()
        {
            // 초대량 발행 접수시 기재한 제출아이디
            // 최대 36자리 영문, 숫자, '-'조합
            string submitID = "20221109-BULK";

            try
            {
                var bulkCashbillResult = _cashbillService.GetBulkResult(corpNum, submitID);
                return View("BulkCashbillResult", bulkCashbillResult);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 삭제 가능한 상태의 현금영수증을 삭제합니다.
         * - 삭제 가능한 상태: "전송실패"
         * - 현금영수증을 삭제하면 사용된 문서번호(mgtKey)를 재사용할 수 있습니다.
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/api/issue#Delete
         */
        public IActionResult Delete()
        {
            // 삭제처리할 현금영수증 문서번호
            string mgtKey = "20220527-001";

            try
            {
                var response = _cashbillService.Delete(corpNum, mgtKey);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 취소 현금영수증 데이터를 팝빌에 저장과 동시에 발행하여 "발행완료" 상태로 처리합니다.
         * - 현금영수증 국세청 전송 정책 : https://developers.popbill.com/guide/cashbill/dotnetcore/introduction/policy-of-send-to-nts
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/api/issue#RevokeRegistIssue
         */
        public IActionResult RevokeRegistIssue()
        {
            // 현금영수증 문서번호, 사업자별로 중복되지 않도록 문서번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20220527-002";

            // 당초 승인 현금영수증 국세청승인번호
            string orgConfirmNum = "TB0000015";

            // 당초 승인 현금영수증 거래일자 (날짜형식yyyyMMdd)
            string orgTradeDate = "20220501";

            try
            {
                var response = _cashbillService.RevokeRegistIssue(corpNum, mgtKey, orgConfirmNum, orgTradeDate);
                return View("IssueResponse", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 작성된 (부분)취소 현금영수증 데이터를 팝빌에 저장과 동시에 발행하여 "발행완료" 상태로 처리합니다.
         * - 취소 현금영수증의 금액은 원본 금액을 넘을 수 없습니다.
         * - 현금영수증 국세청 전송 정책 : https://developers.popbill.com/guide/cashbill/dotnetcore/introduction/policy-of-send-to-nts
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/api/issue#RevokeRegistIssue
         */
        public IActionResult RevokeRegistIssue_part()
        {
            // 현금영수증 문서번호, 사업자별로 중복되지 않도록 문서번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20220527-003";

            // 당초 승인 현금영수증 국세청승인번호
            string orgConfirmNum = "TB0000104";

            // 당초 승인 현금영수증 거래일자 (날짜형식yyyyMMdd)
            string orgTradeDate = "20221108";

            // 발행 안내문자 전송여부
            bool smssendYN = false;

            // 메모
            string memo = "부분 취소발행 메모";

            // 부분취소여부, true-부분취소, false-전체취소
            bool isPartCancel = true;

            // 취소사유, 1-거래취소, 2-오류발급취소, 3-기타
            int cancelType = 1;

            // [취소] 거래금액 ( 공급가액 + 세액 + 봉사료 )
            string totalAmount = "3300";

            // [취소] 공급가액
            string supplyCost = "3000";

            // [취소] 부가세
            string tax = "300";

            // [취소] 봉사료
            string serviceFee = "";

            // 안내메일 제목, 공백처리시 기본양식으로 전송
            string emailSubject = "메일제목 테스트";

            // 거래일시, 날짜(yyyyMMddHHmmss)
            // 당일, 전일만 가능 미입력시 기본값 발행일시 처리
            string tradeDT = "";

            try
            {
                var response = _cashbillService.RevokeRegistIssue(corpNum, mgtKey, orgConfirmNum, orgTradeDate,
                    smssendYN, memo, isPartCancel, cancelType, totalAmount, supplyCost, tax, serviceFee, userID, emailSubject, tradeDT);
                return View("IssueResponse", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        #endregion

        #region 현금영수증 정보확인

        /*
         * 현금영수증 1건의 상태 및 요약정보를 확인합니다.
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/api/info#GetInfo
         */
        public IActionResult GetInfo()
        {
            // 현금영수증 문서번호
            string mgtKey = "20220527-001";

            try
            {
                var response = _cashbillService.GetInfo(corpNum, mgtKey);
                return View("GetInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 다수건의 현금영수증 상태 및 요약 정보를 확인합니다. (1회 호출 시 최대 1,000건 확인 가능)
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/api/info#GetInfos
         */
        public IActionResult GetInfos()
        {
            // 조회할 현금영수증 문서번호 배열, (최대 1000건)
            List<string> mgtKeyList = new List<string>();
            mgtKeyList.Add("20220527-001");
            mgtKeyList.Add("20220527-002");

            try
            {
                var response = _cashbillService.GetInfos(corpNum, mgtKeyList);
                return View("GetInfos", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 현금영수증 1건의 상세정보를 확인합니다.
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/api/info#GetDetailInfo
         */
        public IActionResult GetDetailInfo()
        {
            // 현금영수증 문서번호
            string mgtKey = "20220527-001";

            try
            {
                var response = _cashbillService.GetDetailInfo(corpNum, mgtKey);
                return View("GetDetailInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 검색조건에 해당하는 현금영수증을 조회합니다. (조회기간 단위 : 최대 6개월)
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/api/info#Search
         */
        public IActionResult Search()
        {
            // 일자 유형 ("R" , "T" , "I" 중 택 1)
            // └ R = 등록일자 , T = 거래일자 , I = 발행일자
            string DType = "T";

            // 시작일자, 날짜형식(yyyyMMdd)
            string SDate = "20250701";

            // 종료일자, 날짜형식(yyyyMMdd)
            string EDate = "20250731";

            // 상태코드 배열 (2,3번째 자리에 와일드카드(*) 사용 가능)
            // - 미입력시 전체조회
            string[] State = new string[3];
            State[0] = "3**";

            // 문서형태 배열 ("N" , "C" 중 선택, 다중 선택 가능)
            // - N = 일반 현금영수증 , C = 취소 현금영수증
            // - 미입력시 전체조회
            string[] TradeType = new string[2];
            TradeType[0] = "N";
            TradeType[1] = "C";

            // 거래구분 배열 ("P" , "C" 중 선택, 다중 선택 가능)
            // - P = 소득공제용 , C = 지출증빙용
            // - 미입력시 전체조회
            string[] TradeUsage = new string[2];
            TradeUsage[0] = "P";
            TradeUsage[1] = "C";

            // 거래유형 배열 ("N" , "B" , "T" 중 선택, 다중 선택 가능)
            // - N = 일반 , B = 도서공연 , T = 대중교통
            // - 미입력시 전체조회
            string[] TradeOpt = new string[3];
            TradeOpt[0] = "N";
            TradeOpt[1] = "B";
            TradeOpt[2] = "T";

            // 과세형태 배열 ("T" , "N" 중 선택, 다중 선택 가능)
            // - T = 과세 , N = 비과세
            // - 미입력시 전체조회
            string[] TaxationType = new string[2];
            TaxationType[0] = "T";
            TaxationType[1] = "N";

            // 페이지 번호, 기본값 '1'
            int Page = 1;

            // 페이지당 검색개수, 기본값 '500', 최대 '1000'
            int PerPage = 30;

            // 정렬방향, D-내림차순, A-오름차순
            string Order = "D";

            // 식별번호 조회, 공백시 전체조회
            string QString = "";

            // 가맹점 종사업장 번호
            // └ 다수건 검색시 콤마(",")로 구분. 예) "1234,1000"
            // └ 미입력시 전제조회
            string FranchiseTaxRegID = "";

            try
            {
                var response = _cashbillService.Search(corpNum, DType, SDate, EDate, State, TradeType, TradeUsage,
                    TradeOpt, TaxationType, Page, PerPage, Order, QString, userID, FranchiseTaxRegID);
                return View("Search", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 로그인 상태로 팝빌 사이트의 현금영수증 문서함 메뉴에 접근할 수 있는 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/api/info#GetURL
         */
        public IActionResult GetURL()
        {
            // TBOX(임시문서함), PBOX(발행문서함), WRITE(현금영수증 작성)
            string TOGO = "WRITE";

            try
            {
                var result = _cashbillService.GetURL(corpNum, TOGO, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        #endregion

        #region 현금영수증 보기/인쇄

        /*
         * 현금영수증 1건의 상세 정보 페이지의 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/api/view#GetPopUpURL
         */
        public IActionResult GetPopUpURL()
        {
            // 현금영수증 문서번호
            string mgtKey = "20220527-003";

            try
            {
                var result = _cashbillService.GetPopUpURL(corpNum, mgtKey, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 현금영수증 1건의 상세 정보 페이지(사이트 상단, 좌측 메뉴 및 버튼 제외)의 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/api/view#GetViewURL
         */
        public IActionResult GetViewURL()
        {
            // 현금영수증 문서번호
            string mgtKey = "20220527-003";

            try
            {
                var result = _cashbillService.GetViewURL(corpNum, mgtKey, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 현금영수증 PDF 파일을 다운 받을 수 있는 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/api/view#GetPDFURL
         */
        public IActionResult GetPDFURL()
        {
            // 현금영수증 문서번호
            string mgtKey = "20220527-003";
            try
            {
                var result = _cashbillService.GetPDFURL(corpNum, mgtKey, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 현금영수증 1건을 인쇄하기 위한 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/api/view#GetPrintURL
         */
        public IActionResult GetPrintURL()
        {
            // 현금영수증 문서번호
            string mgtKey = "20220527-003";
            try
            {
                var result = _cashbillService.GetPrintURL(corpNum, mgtKey, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 다수건의 현금영수증을 인쇄하기 위한 페이지의 팝업 URL을 반환합니다. (최대 100건)
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/api/view#GetMassPrintURL
         */
        public IActionResult GetMassPrintURL()
        {
            // 조회할 현금영수증 문서번호 배열, (최대 100건)
            List<string> MgtKeyList = new List<string>();
            MgtKeyList.Add("20220527-001");
            MgtKeyList.Add("20220527-002");

            try
            {
                var result = _cashbillService.GetMassPrintURL(corpNum, MgtKeyList, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 구매자가 수신하는 현금영수증 안내 메일의 하단에 버튼 URL 주소를 반환합니다.
         * - 함수 호출로 반환 받은 URL에는 유효시간이 없습니다.
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/api/view#GetMailURL
         */
        public IActionResult GetMailURL()
        {
            // 현금영수증 문서번호
            string mgtKey = "20220527-003";

            try
            {
                var result = _cashbillService.GetMailURL(corpNum, mgtKey, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        #endregion

        #region 부가기능

        /*
         * 팝빌 사이트에 로그인 상태로 접근할 수 있는 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/common-api/member#GetAccessURL
         */
        public IActionResult GetAccessURL()
        {
            try
            {
                var result = _cashbillService.GetAccessURL(corpNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 현금영수증과 관련된 안내 메일을 재전송 합니다.
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/api/etc#SendEmail
         */
        public IActionResult SendEmail()
        {
            // 현금영수증 문서번호
            string mgtKey = "20220527-003";

            // 수신자 이메일주소
            string receiveEmail = "";

            try
            {
                var response = _cashbillService.SendEmail(corpNum, mgtKey, receiveEmail);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 현금영수증과 관련된 안내 SMS(단문) 문자를 재전송하는 함수로, 팝빌 사이트 [문자·팩스] > [문자] > [전송내역] 메뉴에서 전송결과를 확인 할 수 있습니다.
         * - 메시지는 최대 90byte까지 입력 가능하고, 초과한 내용은 자동으로 삭제되어 전송합니다. (한글 최대 45자)
         * - 함수 호출 시 포인트가 과금됩니다. (전송실패시 환불처리)
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/api/etc#SendSMS
         */
        public IActionResult SendSMS()
        {
            // 현금영수증 문서번호
            string mgtKey = "20220527-003";

            // 발신자 번호
            string sender = "";

            // 수신자 번호
            string receiver = "";

            // 메시지 내용, 90byte 초과시 길이가 조정되어 전송됨
            string contents = "문자 메시지 내용은 90byte초과시 길이가 조정되어 전송됩니다.";

            try
            {
                var response = _cashbillService.SendSMS(corpNum, mgtKey, sender, receiver, contents);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 현금영수증을 팩스로 전송하는 함수로, 팝빌 사이트 [문자·팩스] > [팩스] > [전송내역] 메뉴에서 전송결과를 확인 할 수 있습니다.
         * - 함수 호출 시 포인트가 과금됩니다. (전송실패시 환불처리
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/api/etc#SendFAX
         */
        public IActionResult SendFAX()
        {
            // 현금영수증 문서번호
            string mgtKey = "20220527-003";

            // 발신번호
            string sender = "";

            // 수신번호
            string receiver = "";

            try
            {
                var response = _cashbillService.SendFAX(corpNum, mgtKey, sender, receiver);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌 사이트를 통해 발행하였지만 문서번호가 존재하지 않는 현금영수증에 문서번호를 할당합니다.
         * * - https://developers.popbill.com/reference/cashbill/dotnetcore/api/etc#AssignMgtKey
         */
        public IActionResult AssignMgtKey()
        {
            // 현금영수증 아이템키, 목록조회(Search API) 함수의 반환항목중 ItemKey 참조
            string itemKey = "020080513514100001";

            // 할당할 문서번호, 숫자, 영문, '-', '_' 조합으로
            // 1~24자리까지 사업자번호별 중복없는 고유번호 할당
            string mgtKey = "20220527-100";

            try
            {
                var response = _cashbillService.AssignMgtKey(corpNum, itemKey, mgtKey);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        /*
         * 현금영수증 관련 메일 항목에 대한 발송설정을 확인합니다.
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/api/etc#ListEmailConfig
         */
        public IActionResult ListEmailConfig()
        {
            try
            {
                var result = _cashbillService.ListEmailConfig(corpNum);
                return View("ListEmailConfig", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 현금영수증 관련 메일 항목에 대한 발송설정을 수정합니다.
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/api/etc#UpdateEmailConfig
         */
        public IActionResult UpdateEmailConfig()
        {
            //메일전송유형
            // CSH_ISSUE : 고객에게 현금영수증이 발행 되었음을 알려주는 메일 입니다.
            string emailType = "CSH_ISSUE";

            //전송여부 (true-전송, false-미전송)
            bool sendYN = true;

            try
            {
                var response = _cashbillService.UpdateEmailConfig(corpNum, emailType, sendYN);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        #endregion

        #region 포인트관리

        /*
         * 연동회원의 잔여포인트를 확인합니다.
         * - 과금방식이 파트너과금인 경우 파트너 잔여포인트 확인(GetPartnerBalance API) 함수를 통해 확인하시기 바랍니다.
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/common-api/point#GetBalance
         */
        public IActionResult GetBalance()
        {
            try
            {
                var result = _cashbillService.GetBalance(corpNum);
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
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/common-api/point#GetChargeURL
         */
        public IActionResult GetChargeURL()
        {
            try
            {
                var result = _cashbillService.GetChargeURL(corpNum, userID);
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
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/common-api/point#GetPaymentURL
         */
        public IActionResult GetPaymentURL()
        {

            try
            {
                var result = _cashbillService.GetPaymentURL(corpNum, userID);
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
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/common-api/point#GetUseHistoryURL
         */
        public IActionResult GetUseHistoryURL()
        {

            try
            {
                var result = _cashbillService.GetUseHistoryURL(corpNum, userID);
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
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/common-api/point#GetPartnerBalance
         */
        public IActionResult GetPartnerBalance()
        {
            try
            {
                var result = _cashbillService.GetPartnerBalance(corpNum);
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
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/common-api/point#GetPartnerURL
         */
        public IActionResult GetPartnerURL()
        {
            // CHRG 포인트충전 URL
            string TOGO = "CHRG";

            try
            {
                var result = _cashbillService.GetPartnerURL(corpNum, TOGO);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 현금영수증 발행시 과금되는 포인트 단가를 확인합니다.
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/common-api/point#GetUnitCost
         */
        public IActionResult GetUnitCost()
        {
            try
            {
                var result = _cashbillService.GetUnitCost(corpNum);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌 현금영수증 API 서비스 과금정보를 확인합니다.
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/common-api/point#GetChargeInfo
         */
        public IActionResult GetChargeInfo()
        {
            try
            {
                var response = _cashbillService.GetChargeInfo(corpNum);
                return View("GetChargeInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

                /*
         * 연동회원 포인트를 환불 신청합니다.
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/common-api/point#Refund
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

                var response = _cashbillService.Refund(corpNum, refundForm);
                return View("RefundResponse", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 포인트를 환불 신청합니다.
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/common-api/point#PaymentRequest
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

                var response = _cashbillService.PaymentRequest(corpNum, paymentForm);
                return View("PaymentResponse", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 포인트 무통장 입금신청내역 1건을 확인합니다.
         *  - https://developers.popbill.com/reference/cashbill/dotnetcore/common-api/point#GetSettleResult
         */
        public IActionResult GetSettleResult()
        {
            // 정산코드
            var settleCode = "202301130000000026";

            try
            {
                var paymentHistory = _cashbillService.GetSettleResult(corpNum, settleCode);

                return View("PaymentHistory", paymentHistory);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 포인트 사용내역을 확인합니다.
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/common-api/point#GetUseHistory
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
                var useHistoryResult = _cashbillService.GetUseHistory(corpNum, SDate, EDate, Page,
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
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/common-api/point#GetPaymentHistory
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
                var paymentHistoryResult = _cashbillService.GetPaymentHistory(corpNum, SDate, EDate,
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
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/common-api/point#GetRefundHistory
         */
        public IActionResult GetRefundHistory()
        {
            // 목록 페이지번호 (기본값 1)
            var Page = 1;

            // 페이지당 표시할 목록 개수 (기본값 500, 최대 1,000)
            var PerPage = 100;

            try
            {
                var refundHistoryResult = _cashbillService.GetRefundHistory(corpNum, Page, PerPage);

                return View("RefundHistoryResult", refundHistoryResult);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        /*
         * 포인트 환불에 대한 상세정보 1건을 확인합니다.
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/common-api/point#GetRefundInfo
         */
        public IActionResult GetRefundInfo()
        {
            // 환불 코드
            var refundCode = "023040000017";

            try
            {
                var response = _cashbillService.GetRefundInfo(corpNum, refundCode);
                return View("RefundHistory", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        /*
         * 환불 가능한 포인트를 확인합니다. (보너스 포인트는 환불가능포인트에서 제외됩니다.)
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/common-api/point#GetRefundableBalance
         */
        public IActionResult GetRefundableBalance()
        {
            try
            {
                var refundableBalance = _cashbillService.GetRefundableBalance(corpNum);
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
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/common-api/member#CheckIsMember
         */
        public IActionResult CheckIsMember()
        {
            try
            {
                //링크아이디
                string linkID = "TESTER";

                var response = _cashbillService.CheckIsMember(corpNum, linkID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 사용하고자 하는 아이디의 중복여부를 확인합니다.
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/common-api/member#CheckID
         */
        public IActionResult CheckID()
        {
            //중복여부 확인할 팝빌 회원 아이디
            string checkID = "testkorea";

            try
            {
                var response = _cashbillService.CheckID(checkID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 사용자를 연동회원으로 가입처리합니다.
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/common-api/member#JoinMember
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

            // 담당자 메일주소 (최대 100자)
            joinInfo.ContactEmail = "";

            // 담당자 휴대폰 (최대 20자)
            joinInfo.ContactTEL = "";

            try
            {
                var response = _cashbillService.JoinMember(joinInfo);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 회사정보를 확인합니다.
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/common-api/member#GetCorpInfo
         */
        public IActionResult GetCorpInfo()
        {
            try
            {
                var response = _cashbillService.GetCorpInfo(corpNum);
                return View("GetCorpInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 회사정보를 수정합니다.
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/common-api/member#UpdateCorpInfo
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
                var response = _cashbillService.UpdateCorpInfo(corpNum, corpInfo);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 담당자(팝빌 로그인 계정)를 추가합니다.
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/common-api/member#RegistContact
         */
        public IActionResult RegistContact()
        {
            Contact contactInfo = new Contact();

            // 아이디, 6자 이상 50자 미만
            contactInfo.id = "testkorea_20181212";

            // 비밀번호, 8자이상 20자 미만 (영문, 숫자, 특수문자 조합)
            contactInfo.Password = "asdfasdf123!@#";

            // 담당자 성명 (최대 100자)
            contactInfo.personName = "코어담당자";

            // 담당자 휴대폰 (최대 20자)
            contactInfo.tel = "";

            // 담당자 메일 (최대 100자)
            contactInfo.email = "";

            // 권한 1(개인권한), 2 (읽기권한), 3 (회사권한)
            contactInfo.searchRole = 3;

            try
            {
                var response = _cashbillService.RegistContact(corpNum, contactInfo);
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
                var response = _cashbillService.DeleteContact(corpNum, targetUserID, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
        * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 정보을 확인합니다.
        * - https://developers.popbill.com/reference/cashbill/dotnetcore/common-api/member#GetContactInfo
        */
        public IActionResult GetContactInfo()
        {
            // 확인할 담당자 아이디
            string contactID = "test0730";

            try
            {
                var contactInfo = _cashbillService.GetContactInfo(corpNum, contactID);
                return View("GetContactInfo", contactInfo);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 목록을 확인합니다.
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/common-api/member#ListContact
         */
        public IActionResult ListContact()
        {
            try
            {
                var response = _cashbillService.ListContact(corpNum);
                return View("ListContact", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 정보를 수정합니다.
         * - https://developers.popbill.com/reference/cashbill/dotnetcore/common-api/member#UpdateContact
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
                var response = _cashbillService.UpdateContact(corpNum, contactInfo);
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
         *  - https://developers.popbill.com/reference/cashbill/dotnetcore/common-api/member#QuitMember
         */
        public IActionResult QuitMember()
        {
            // 탈퇴 사유
            var quitReason = "탈퇴사유";

            try
            {
                var response = _cashbillService.QuitMember(corpNum, quitReason);
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
