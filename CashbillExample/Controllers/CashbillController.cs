using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Popbill;
using Popbill.Cashbill;

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
         * - 문서번호는 최대 24자리, 영문, 숫자 '-', '_'를 조합하여 사업자별로 중복되지 않도록 구성
         * - https://docs.popbill.com/cashbill/dotnetcore/api#CheckMgtKeyInUse
         */
        public IActionResult CheckMgtKeyInUse()
        {
            try
            {
                // 현금영수증 문서번호
                string mgtKey = "20181030";

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
         * - 현금영수증 국세청 전송 정책 : https://docs.popbill.com/cashbill/ntsSendPolicy?lang=donetcore
         * - https://docs.popbill.com/cashbill/dotnetcore/api#RegistIssue
         */
        public IActionResult RegistIssue()
        {
            // 현금영수증 정보 객체 
            Cashbill cashbill = new Cashbill();

            // [필수] 문서번호, 사업자별로 중복되지 않도록 문서번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            cashbill.mgtKey = "20211227-CORE002";

            // [취소거래시 필수] 원본 현금영수증 국세청승인번호
            cashbill.orgConfirmNum = "";

            // [취소거래시 필수] 원본 현금영수증 거래일자
            cashbill.orgTradeDate = "";

            // [필수] 문서형태, { 승인거래, 취소거래 } 중 기재
            cashbill.tradeType = "승인거래";

            // [필수] 거래구분, { 소득공제용, 지출증빙용 } 중 기재
            cashbill.tradeUsage = "소득공제용";

            // 거래유형, { 일반, 도서공연, 대중교통 } 중 기재
            cashbill.tradeOpt = "일반";

            // [필수] 과세형태, { 과세, 비과세 } 중 기재
            cashbill.taxationType = "과세";

            // [필수] 거래금액 ( 공급가액 + 세액 + 봉사료 ) 
            cashbill.totalAmount = "11000";

            // [필수] 공급가액
            cashbill.supplyCost = "10000";

            // [필수] 부가세 
            cashbill.tax = "1000";

            // [필수] 봉사료
            cashbill.serviceFee = "0";

            // [필수] 가맹점 사업자번호
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
            cashbill.franchiseTEL = "070-1234-1234";

            // [필수] 식별번호
            // 거래구분(tradeUsage) - '소득공제용' 인 경우 주민등록/휴대폰/카드번호 기재 가능
            // 거래구분(tradeUsage) - '지출증빙용' 인 경우 사업자번호/주민등록/휴대폰/카드번호 기재 가능 
            cashbill.identityNum = "0101112222";

            // 주문자명
            cashbill.customerName = "주문자명";

            // 주문상품명
            cashbill.itemName = "주문상품명";

            // 주문번호
            cashbill.orderNumber = "주문번호";

            // 주문자 이메일
            // 팝빌 개발환경에서 테스트하는 경우에도 안내 메일이 전송되므로,
            // 실제 거래처의 메일주소가 기재되지 않도록 주의
            cashbill.email = "test@test.com";

            // 주문자 휴대폰
            cashbill.hp = "010-111-222";

            // 주문자 팩스번호
            cashbill.fax = "02-6442-9700";

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
         * 1건의 현금영수증을 [임시저장]합니다.
         * - [임시저장] 상태의 현금영수증은 발행(Issue API)을 호출해야만 국세청에 전송됩니다.
         */
        public IActionResult Register()
        {
            // 현금영수증 정보 객체 
            Cashbill cashbill = new Cashbill();

            // [필수] 문서번호, 사업자별로 중복되지 않도록 문서번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            cashbill.mgtKey = "20211227-CoreR001";

            // [취소거래시 필수] 원본 현금영수증 국세청승인번호
            cashbill.orgConfirmNum = "";

            // [취소거래시 필수] 원본 현금영수증 거래일자
            cashbill.orgTradeDate = "";

            // [필수] 문서형태, { 승인거래, 취소거래 } 중 기재
            cashbill.tradeType = "승인거래";

            // [필수] 거래구분, { 소득공제용, 지출증빙용 } 중 기재
            cashbill.tradeUsage = "소득공제용";

            // 거래유형, { 일반, 도서공연, 대중교통 } 중 기재
            cashbill.tradeOpt = "도서공연";

            // [필수] 과세형태, { 과세, 비과세 } 중 기재
            cashbill.taxationType = "과세";

            // [필수] 거래금액 ( 공급가액 + 세액 + 봉사료 ) 
            cashbill.totalAmount = "11000";

            // [필수] 공급가액
            cashbill.supplyCost = "10000";

            // [필수] 부가세 
            cashbill.tax = "1000";

            // [필수] 봉사료
            cashbill.serviceFee = "0";

            // [필수] 가맹점 사업자번호
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
            cashbill.franchiseTEL = "070-1234-1234";

            // [필수] 식별번호
            // 거래구분(tradeUsage) - '소득공제용' 인 경우 주민등록/휴대폰/카드번호 기재 가능
            // 거래구분(tradeUsage) - '지출증빙용' 인 경우 사업자번호/주민등록/휴대폰/카드번호 기재 가능 
            cashbill.identityNum = "0101112222";

            // 주문자명
            cashbill.customerName = "주문자명";

            // 주문상품명
            cashbill.itemName = "주문상품명";

            // 주문번호
            cashbill.orderNumber = "주문번호";

            // 주문자 이메일
            // 팝빌 개발환경에서 테스트하는 경우에도 안내 메일이 전송되므로,
            // 실제 거래처의 메일주소가 기재되지 않도록 주의
            cashbill.email = "test@test.com";

            // 주문자 휴대폰
            cashbill.hp = "010-111-222";

            // 주문자 팩스번호
            cashbill.fax = "02-6442-9700";

            // 발행시 알림문자 전송여부
            cashbill.smssendYN = false;

            try
            {
                var response = _cashbillService.Register(corpNum, cashbill, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 1건의 현금영수증을 [수정]합니다.
         * - [임시저장] 상태의 현금영수증만 수정할 수 있습니다.
         * - 국세청에 신고된 현금영수증은 수정할 수 없으며, 취소 현금영수증을 발행하여 취소처리 할 수 있습니다.
         */
        public IActionResult Update()
        {
            // 수정할 현금영수증 문서번호
            string mgtKey = "20211222-002";

            // 현금영수증 정보 객체 
            Cashbill cashbill = new Cashbill();

            // [취소거래시 필수] 원본 현금영수증 국세청승인번호
            cashbill.orgConfirmNum = "";

            // [취소거래시 필수] 원본 현금영수증 거래일자
            cashbill.orgTradeDate = "";

            // [필수] 문서형태, { 승인거래, 취소거래 } 중 기재
            cashbill.tradeType = "승인거래";

            // [필수] 거래구분, { 소득공제용, 지출증빙용 } 중 기재
            cashbill.tradeUsage = "소득공제용";

            // 거래유형, { 일반, 도서공연, 대중교통 } 중 기재
            cashbill.tradeOpt = "도서공연";

            // [필수] 과세형태, { 과세, 비과세 } 중 기재
            cashbill.taxationType = "과세";

            // [필수] 거래금액 ( 공급가액 + 세액 + 봉사료 ) 
            cashbill.totalAmount = "11000";

            // [필수] 공급가액
            cashbill.supplyCost = "10000";

            // [필수] 부가세 
            cashbill.tax = "1000";

            // [필수] 봉사료
            cashbill.serviceFee = "0";

            // [필수] 가맹점 사업자번호
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
            cashbill.franchiseTEL = "070-1234-1234";

            // [필수] 식별번호
            // 거래구분(tradeUsage) - '소득공제용' 인 경우 주민등록/휴대폰/카드번호 기재 가능
            // 거래구분(tradeUsage) - '지출증빙용' 인 경우 사업자번호/주민등록/휴대폰/카드번호 기재 가능 
            cashbill.identityNum = "0101112222";

            // 주문자명
            cashbill.customerName = "주문자명";

            // 주문상품명
            cashbill.itemName = "주문상품명";

            // 주문번호
            cashbill.orderNumber = "주문번호";

            // 주문자 이메일
            // 팝빌 개발환경에서 테스트하는 경우에도 안내 메일이 전송되므로,
            // 실제 거래처의 메일주소가 기재되지 않도록 주의
            cashbill.email = "test@test.com";

            // 주문자 휴대폰
            cashbill.hp = "010-111-222";

            // 주문자 팩스번호
            cashbill.fax = "02-6442-9700";

            // 발행시 알림문자 전송여부
            cashbill.smssendYN = false;

            try
            {
                var response = _cashbillService.Update(corpNum, mgtKey, cashbill, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 1건의 [임시저장] 현금영수증을 [발행]합니다.
         * - 현금영수증 국세청 전송 정책 : https://docs.popbill.com/cashbill/ntsSendPolicy?lang=dotnetcore
         */
        public IActionResult Issue()
        {
            // 발행처리할 현금영수증 문서번호
            string mgtKey = "20211227-CoreR001";

            // 메모
            string memo = "발행 메모";

            try
            {
                var response = _cashbillService.Issue(corpNum, mgtKey, memo, userID);
                return View("IssueResponse", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 국세청 전송 이전 "발행완료" 상태의 현금영수증을 "발행취소"하고 국세청 신고 대상에서 제외합니다.
         * - Delete(삭제)함수를 호출하여 "발행취소" 상태의 현금영수증을 삭제하면, 문서번호 재사용이 가능합니다.
         * - https://docs.popbill.com/cashbill/dotnetcore/api#CancelIssue
         */
        public IActionResult CancelIssue()
        {
            // 발행취소할 현금영수증 문서번호
            string mgtKey = "20211201-001";

            // 메모
            string memo = "발행취소 메모";

            try
            {
                var response = _cashbillService.CancelIssue(corpNum, mgtKey, memo, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 삭제 가능한 상태의 현금영수증을 삭제합니다.
         * - 삭제 가능한 상태: "임시저장", "발행취소", "전송실패"
         * - 현금영수증을 삭제하면 사용된 문서번호(mgtKey)를 재사용할 수 있습니다.
         * - https://docs.popbill.com/cashbill/dotnetcore/api#Delete
         */
        public IActionResult Delete()
        {
            // 삭제처리할 현금영수증 문서번호
            string mgtKey = "20211201-001";

            try
            {
                var response = _cashbillService.Delete(corpNum, mgtKey, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 취소 현금영수증 데이터를 팝빌에 저장과 동시에 발행하여 "발행완료" 상태로 처리합니다.
         * - 현금영수증 국세청 전송 정책 : https://docs.popbill.com/cashbill/ntsSendPolicy?lang=dotnetcore
         * - https://docs.popbill.com/cashbill/dotnetcore/api#RevokeRegistIssue
         */
        public IActionResult RevokeRegistIssue()
        {
            // 현금영수증 문서번호, 사업자별로 중복되지 않도록 문서번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20211227-Revoke001";

            // 원본 현금영수증 국세청승인번호
            string orgConfirmNum = "TB0000015";

            // 원본현금영수증 거래일자 (날짜형식yyyyMMdd)
            string orgTradeDate = "20211224";

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
         * - 현금영수증 국세청 전송 정책 : https://docs.popbill.com/cashbill/ntsSendPolicy?lang=dotnetcore
         * - https://docs.popbill.com/cashbill/dotnetcore/api#RevokeRegistIssue
         */
        public IActionResult RevokeRegistIssue_part()
        {
            // 현금영수증 문서번호, 사업자별로 중복되지 않도록 문서번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20211227-Revoke002";

            // 원본 현금영수증 국세청승인번호
            string orgConfirmNum = "TB0000015";

            // 원본현금영수증 거래일자 (날짜형식yyyyMMdd)
            string orgTradeDate = "20211224";

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

            try
            {
                var response = _cashbillService.RevokeRegistIssue(corpNum, mgtKey, orgConfirmNum, orgTradeDate,
                    smssendYN, memo, isPartCancel, cancelType, totalAmount, supplyCost, tax, serviceFee, userID);
                return View("IssueResponse", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 1건의 취소현금영수증을 [임시저장]합니다.
         * - [임시저장] 상태의 현금영수증은 발행(Issue API)을 호출해야만 국세청에 전송됩니다.
         */
        public IActionResult RevokeRegister()
        {
            // 현금영수증 문서번호, 사업자별로 중복되지 않도록 문서번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20211201-101";

            // 원본 현금영수증 국세청승인번호
            string orgConfirmNum = "158814020";

            // 원본현금영수증 거래일자 (날짜형식yyyyMMdd)
            string orgTradeDate = "20211201";

            try
            {
                var response = _cashbillService.RevokeRegister(corpNum, mgtKey, orgConfirmNum, orgTradeDate);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 1건의 (부분)취소현금영수증을 [임시저장]합니다.
         * - [임시저장] 상태의 현금영수증은 발행(Issue API)을 호출해야만 국세청에 전송됩니다.
         */
        public IActionResult RevokeRegister_part()
        {
            // 현금영수증 문서번호,사업자별로 중복되지 않도록 문서번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20211201-100";

            // 원본 현금영수증 국세청승인번호
            string orgConfirmNum = "158814020";

            // 원본현금영수증 거래일자 (날짜형식yyyyMMdd)
            string orgTradeDate = "20211201";

            // 발행 안내문자 전송여부           
            bool smssendYN = false;

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

            try
            {
                var response = _cashbillService.RevokeRegister(corpNum, mgtKey, orgConfirmNum, orgTradeDate,
                    smssendYN, isPartCancel, cancelType, totalAmount, supplyCost, tax, serviceFee, userID);
                return View("Response", response);
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
         * - https://docs.popbill.com/cashbill/dotnetcore/api#GetInfo
         */
        public IActionResult GetInfo()
        {
            // 현금영수증 문서번호
            string mgtKey = "20210518-003";

            try
            {
                var response = _cashbillService.GetInfo(corpNum, mgtKey, userID);
                return View("GetInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 다수건의 현금영수증 상태 및 요약 정보를 확인합니다. (1회 호출 시 최대 1,000건 확인 가능)
         * - https://docs.popbill.com/cashbill/dotnetcore/api#GetInfos
         */
        public IActionResult GetInfos()
        {
            // 조회할 현금영수증 문서번호 배열, (최대 1000건)
            List<string> mgtKeyList = new List<string>();
            mgtKeyList.Add("20210518-001");
            mgtKeyList.Add("20210518-002");
            mgtKeyList.Add("20210518-003");

            try
            {
                var response = _cashbillService.GetInfos(corpNum, mgtKeyList, userID);
                return View("GetInfos", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 현금영수증 1건의 상세정보를 확인합니다.
         * - https://docs.popbill.com/cashbill/dotnetcore/api#GetDetailInfo
         */
        public IActionResult GetDetailInfo()
        {
            // 현금영수증 문서번호
            string mgtKey = "20210518-003";

            try
            {
                var response = _cashbillService.GetDetailInfo(corpNum, mgtKey, userID);
                return View("GetDetailInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 검색조건에 해당하는 현금영수증을 조회합니다. (조회기간 단위 : 최대 6개월)
         * - https://docs.popbill.com/cashbill/dotnetcore/api#Search
         */
        public IActionResult Search()
        {
            // 검색일자 유형, R-등록일자, T-거래일자, I-발행일자
            string DType = "T";

            // 시작일자, 날짜형식(yyyyMMdd)
            string SDate = "20211201";

            // 종료일자, 날짜형식(yyyyMMdd)
            string EDate = "20211220";

            // 상태코드 배열, 미기재시 전체 상태조회, 상태코드(stateCode)값 3자리의 배열, 2,3번째 자리에 와일드카드 가능
            // - 상태코드에 대한 자세한 사항은 "[현금영수증 API 연동매뉴얼] > 5.1 현금영수증 상태코드" 를 참조하시기 바랍니다. 
            string[] State = new string[3];
            State[0] = "1**";
            State[1] = "3**";
            State[2] = "4**";

            // 문서형태 배열, N-일반 현금영수증, C-취소 현금영수증
            string[] TradeType = new string[2];
            TradeType[0] = "N";
            TradeType[1] = "C";

            // 거래구분 배열, P-소득공제용, C-지출증빙용
            string[] TradeUsage = new string[2];
            TradeUsage[0] = "P";
            TradeUsage[1] = "C";

            // 거래유형 배열, N-일반, B-도서공연, T-대중교통
            string[] TradeOpt = new string[3];
            TradeOpt[0] = "N";
            TradeOpt[1] = "B";
            TradeOpt[2] = "T";

            // 과세형태 배열, T-과세, N-비과세 
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

            // 가맹점 종사업장 번호, 다수건 검색시 콤마(",")로 구분. 예) 1234,1000
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
         * 현금영수증의 상태에 대한 변경이력을 확인합니다.
         * - https://docs.popbill.com/cashbill/dotnetcore/api#GetLogs
         */
        public IActionResult GetLogs()
        {
            // 현금영수증 문서번호
            string mgtKey = "20210518-001";

            try
            {
                var response = _cashbillService.GetLogs(corpNum, mgtKey, userID);
                return View("GetLogs", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 로그인 상태로 팝빌 사이트의 현금영수증 문서함 메뉴에 접근할 수 있는 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://docs.popbill.com/cashbill/dotnetcore/api#GetURL
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
         * 팝빌 사이트와 동일한 현금영수증 1건의 상세 정보 페이지의 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://docs.popbill.com/cashbill/dotnetcore/api#GetPopUpURL
         */
        public IActionResult GetPopUpURL()
        {
            // 현금영수증 문서번호
            string mgtKey = "20210518-003";

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
         * 팝빌 사이트와 동일한 현금영수증 1건의 상세 정보 페이지(사이트 상단, 좌측 메뉴 및 버튼 제외)의 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://docs.popbill.com/cashbill/dotnetcore/api#GetViewURL
         */
        public IActionResult GetViewURL()
        {
            // 현금영수증 문서번호
            string mgtKey = "20210518-003";

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
         * - https://docs.popbill.com/cashbill/dotnetcore/api#GetPDFURL
         */
        public IActionResult GetPDFURL()
        {
            // 현금영수증 문서번호
            string mgtKey = "20210518-003";
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
         * - https://docs.popbill.com/cashbill/dotnetcore/api#GetPrintURL
         */
        public IActionResult GetPrintURL()
        {
            // 현금영수증 문서번호
            string mgtKey = "20210518-003";
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
         * - https://docs.popbill.com/cashbill/dotnetcore/api#GetMassPrintURL
         */
        public IActionResult GetMassPrintURL()
        {
            // 조회할 현금영수증 문서번호 배열, (최대 100건)
            List<string> MgtKeyList = new List<string>();
            MgtKeyList.Add("20210518-003");
            MgtKeyList.Add("20210518-002");
            MgtKeyList.Add("20210518-001");

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
         * - https://docs.popbill.com/cashbill/dotnetcore/api#GetMailURL
         */
        public IActionResult GetMailURL()
        {
            // 현금영수증 문서번호
            string mgtKey = "20210518-003";
            
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
         * - https://docs.popbill.com/cashbill/dotnetcore/api#GetAccessURL
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
         * - https://docs.popbill.com/cashbill/dotnetcore/api#SendEmail
         */
        public IActionResult SendEmail()
        {
            // 현금영수증 문서번호
            string mgtKey = "20210518-003";

            // 수신자 이메일주소
            string receiveEmail = "test@test.com";

            try
            {
                var response = _cashbillService.SendEmail(corpNum, mgtKey, receiveEmail, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 현금영수증과 관련된 안내 SMS(단문) 문자를 재전송하는 함수로, 팝빌 사이트 [문자·팩스] > [문자] > [전송내역] 메뉴에서 전송결과를 확인 할 수 있습니다.
         * - 알림문자 전송시 포인트가 차감됩니다. (전송실패시 환불처리)
         * - https://docs.popbill.com/cashbill/dotnetcore/api#SendSMS
         */
        public IActionResult SendSMS()
        {
            // 현금영수증 문서번호
            string mgtKey = "20210518-003";

            // 발신자 번호
            string sender = "070-4304-2991";

            // 수신자 번호
            string receiver = "010-111-222";

            // 메시지 내용, 90byte 초과시 길이가 조정되어 전송됨
            string contents = "문자 메시지 내용은 90byte초과시 길이가 조정되어 전송됩니다.";

            try
            {
                var response = _cashbillService.SendSMS(corpNum, mgtKey, sender, receiver, contents, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 현금영수증을 팩스로 전송하는 함수로, 팝빌 사이트 [문자·팩스] > [팩스] > [전송내역] 메뉴에서 전송결과를 확인 할 수 있습니다.
         * - 팩스 전송 요청시 포인트가 차감됩니다. (전송실패시 환불처리)
         * - https://docs.popbill.com/cashbill/dotnetcore/api#SendFAX
         */
        public IActionResult SendFAX()
        {
            // 현금영수증 문서번호
            string mgtKey = "20210518-003";

            // 발신번호
            string sender = "070-4304-2991";

            // 수신번호
            string receiver = "010-111-222";

            try
            {
                var response = _cashbillService.SendFAX(corpNum, mgtKey, sender, receiver, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌 사이트를 통해 발행하였지만 문서번호가 존재하지 않는 현금영수증에 문서번호를 할당합니다.
         * * - https://docs.popbill.com/cashbill/dotnetcore/api#AssignMgtKey
         */
        public IActionResult AssignMgtKey()
        {
            // 현금영수증 아이템키, 목록조회(Search) API의 반환항목중 ItemKey 참조
            string itemKey = "020080513514100001";

            // 현금영수증에 할당할 문서번호
            string mgtKey = "20210518-100";

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
         * - https://docs.popbill.com/cashbill/dotnetcore/api#ListEmailConfig
         */
        public IActionResult ListEmailConfig()
        {
            try
            {
                var result = _cashbillService.ListEmailConfig(corpNum, userID);
                return View("ListEmailConfig", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 현금영수증 관련 메일 항목에 대한 발송설정을 수정합니다.
         * - https://docs.popbill.com/cashbill/dotnetcore/api#UpdateEmailConfig
         *
         * 메일전송유형
         * CSH_ISSUE : 고객에게 현금영수증이 발행 되었음을 알려주는 메일 입니다.
         * CSH_CANCEL : 고객에게 현금영수증이 발행취소 되었음을 알려주는 메일 입니다.
         * - https://docs.popbill.com/cashbill/dotnetcore/api#UpdateEmailConfig
         */
        public IActionResult UpdateEmailConfig()
        {
            //메일전송유형
            string emailType = "CSH_ISSUE";

            //전송여부 (true-전송, false-미전송)
            bool sendYN = true;

            try
            {
                var response = _cashbillService.UpdateEmailConfig(corpNum, emailType, sendYN, userID);
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
         * - 과금방식이 파트너과금인 경우 파트너 잔여포인트(GetPartnerBalance API)를 통해 확인하시기 바랍니다.
         * - https://docs.popbill.com/cashbill/dotnetcore/api#GetBalance
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
         * - https://docs.popbill.com/cashbill/dotnetcore/api#GetChargeURL
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
         * - https://docs.popbill.com/cashbill/dotnetcore/api#GetPaymentURL
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
         * - https://docs.popbill.com/cashbill/dotnetcore/api#GetUseHistoryURL
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
         * - 과금방식이 연동과금인 경우 연동회원 잔여포인트(GetBalance API)를 이용하시기 바랍니다.
         * - https://docs.popbill.com/cashbill/dotnetcore/api#GetPartnerBalance
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
         * - https://docs.popbill.com/cashbill/dotnetcore/api#GetPartnerURL
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
         * - https://docs.popbill.com/cashbill/dotnetcore/api#GetUnitCost
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
         * - https://docs.popbill.com/cashbill/dotnetcore/api#GetChargeInfo
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

        #endregion

        #region 회원정보

        /*
         * 사업자번호를 조회하여 연동회원 가입여부를 확인합니다.
         * - https://docs.popbill.com/cashbill/dotnetcore/api#CheckIsMember
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
         * - https://docs.popbill.com/cashbill/dotnetcore/api#CheckID
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
         * - https://docs.popbill.com/cashbill/dotnetcore/api#JoinMember
         */
        public IActionResult JoinMember()
        {
            JoinForm joinInfo = new JoinForm();

            // 링크아이디
            joinInfo.LinkID = "TESTER";

            // 아이디, 6자이상 50자 미만
            joinInfo.ID = "userid_20181212";

            //// 비밀번호, 8자이상 20자 미만 (영문, 숫자, 특수문자 조합)
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
            joinInfo.ContactEmail = "test@test.com";

            // 담당자 연락처 (최대 20자)
            joinInfo.ContactTEL = "070-4304-2992";

            // 담당자 휴대폰번호 (최대 20자)
            joinInfo.ContactHP = "010-111-222";

            // 담당자 팩스번호 (최대 20자)
            joinInfo.ContactFAX = "02-111-222";

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
         * - https://docs.popbill.com/cashbill/dotnetcore/api#GetCorpInfo
         */
        public IActionResult GetCorpInfo()
        {
            try
            {
                var response = _cashbillService.GetCorpInfo(corpNum, userID);
                return View("GetCorpInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 회사정보를 수정합니다.
         * - https://docs.popbill.com/cashbill/dotnetcore/api#UpdateCorpInfo
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
                var response = _cashbillService.UpdateCorpInfo(corpNum, corpInfo, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 담당자(팝빌 로그인 계정)를 추가합니다.
         * - https://docs.popbill.com/cashbill/dotnetcore/api#RegistContact
         */
        public IActionResult RegistContact()
        {
            Contact contactInfo = new Contact();

            // 담당자 아이디, 6자 이상 50자 미만
            contactInfo.id = "testkorea_20181212";

            //// 비밀번호, 8자이상 20자 미만 (영문, 숫자, 특수문자 조합)
            contactInfo.Password = "asdfasdf123!@#";

            // 담당자명 (최대 100자)
            contactInfo.personName = "코어담당자";

            // 담당자 연락처 (최대 20자)
            contactInfo.tel = "070-4304-2992";

            // 담당자 휴대폰번호 (최대 20자)
            contactInfo.hp = "010-111-222";

            // 담당자 팩스번호 (최대 20자)
            contactInfo.fax = "02-111-222";

            // 담당자 이메일 (최대 100자)
            // 팝빌 개발환경에서 테스트하는 경우에도 안내 메일이 전송되므로,
            // 실제 거래처의 메일주소가 기재되지 않도록 주의
            contactInfo.email = "netcore@linkhub.co.kr";

            // 담당자 조회권한 설정, 1(개인권한), 2 (읽기권한), 3 (회사권한)
            contactInfo.searchRole = 3;

            try
            {
                var response = _cashbillService.RegistContact(corpNum, contactInfo, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
        * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 정보을 확인합니다.
        * - https://docs.popbill.com/cashbill/dotnetcore/api#GetContactInfo
        */
        public IActionResult GetContactInfo()
        {
            // 확인할 담당자 아이디
            string contactID = "test0730";

            try
            {
                var contactInfo = _cashbillService.GetContactInfo(corpNum, contactID, userID);
                return View("GetContactInfo", contactInfo);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 목록을 확인합니다.
         * - https://docs.popbill.com/cashbill/dotnetcore/api#ListContact
         */
        public IActionResult ListContact()
        {
            try
            {
                var response = _cashbillService.ListContact(corpNum, userID);
                return View("ListContact", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 정보를 수정합니다.
         * - https://docs.popbill.com/cashbill/dotnetcore/api#UpdateContact
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
            // 팝빌 개발환경에서 테스트하는 경우에도 안내 메일이 전송되므로,
            // 실제 거래처의 메일주소가 기재되지 않도록 주의
            contactInfo.email = "netcore@linkhub.co.kr";

            // 담당자 조회권한 설정, 1(개인권한), 2 (읽기권한), 3 (회사권한)
            contactInfo.searchRole = 3;

            try
            {
                var response = _cashbillService.UpdateContact(corpNum, contactInfo, userID);
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