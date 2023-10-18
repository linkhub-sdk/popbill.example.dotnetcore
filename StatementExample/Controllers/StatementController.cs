using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Popbill;
using Popbill.Statement;

namespace StatementExample.Controllers
{
    public class StatementController : Controller
    {
        private readonly StatementService _statementService;

        public StatementController(StatementInstance STMinstance)
        {
            // 전자명세서 서비스 객체 생성
            _statementService = STMinstance.statementService;

        }

        // 팝빌 연동회원 사업자번호 (하이픈 '-' 없이 10자리)
        string corpNum = "1234567890";

        // 팝빌 연동회원 아이디
        string userID = "testkorea";

        /*
         * 전자명세서 Index page (Statement/Index.cshtml)
         */
        public IActionResult Index()
        {
            return View();
        }

        #region 전자명세서 발행

        /*
         * 파트너가 전자명세서 관리 목적으로 할당하는 문서번호의 사용여부를 확인합니다.
         * - 이미 사용 중인 문서번호는 중복 사용이 불가하고, 전자명세서가 삭제된 경우에만 문서번호의 재사용이 가능합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/info#CheckMgtKeyInUse
         */
        public IActionResult CheckMgtKeyInUse()
        {
            try
            {
                // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
                int itemCode = 121;

                // 전자명세서 문서번호
                string mgtKey = "20220527-001";

                bool result = _statementService.CheckMgtKeyInUse(corpNum, itemCode, mgtKey);

                return View("result", result ? "사용중" : "미사용중");
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 작성된 전자명세서 데이터를 팝빌에 저장과 동시에 발행하여, "발행완료" 상태로 처리합니다.
         * - 팝빌 사이트 [전자명세서] > [환경설정] > [전자명세서 관리] 메뉴의 발행시 자동승인 옵션 설정을 통해 전자명세서를 "발행완료" 상태가 아닌 "승인대기" 상태로 발행 처리 할 수 있습니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/issue#RegistIssue
         */
        public IActionResult RegistIssue()
        {
            // 전자명세서 정보 객체
            Statement statement = new Statement();

            // 기재상 작성일자 날짜형식(yyyyMMdd)
            statement.writeDate = "20220527";

            // {영수, 청구, 없음} 중 기재
            statement.purposeType = "영수";

            // 과세형태, {과세, 영세, 면세} 중 기재
            statement.taxType = "과세";

            // 맞춤양식코드, 기본값을 공백('')으로 처리하면 기본양식으로 처리.
            statement.formCode = "";

            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
            statement.itemCode = 121;

            // 문서번호, 1~24자리 숫자, 영문, '-', '_' 조합으로 사업자별로 중복되지 않도록 구성
            statement.mgtKey = "20220527-001";


            /**************************************************************************
             *                             발신자 정보                                   *
             **************************************************************************/

            // 발신자 사업자번호
            statement.senderCorpNum = corpNum;

            // 종사업자 식별번호. 필요시 기재. 형식은 숫자 4자리.
            statement.senderTaxRegID = "";

            // 발신자 상호
            statement.senderCorpName = "발신자 상호";

            // 발신자 대표자성명
            statement.senderCEOName = "발신자 대표자 성명";

            // 발신자 주소
            statement.senderAddr = "발신자 주소";

            // 발신자 종목
            statement.senderBizClass = "발신자 종목";

            // 발신자 업태
            statement.senderBizType = "발신자 업태";

            // 발신자 종목
            statement.senderBizClass = "발신자 종목";

            // 발신자 성명
            statement.senderContactName = "발신자 담당자명";

            // 발신자 부서명
            statement.senderDeptName = "발신자 부서명";

            // 발신자 연락처
            statement.senderTEL = "";

            // 발신자 휴대전화
            statement.senderHP = "";

            // 발신자 이메일주소
            statement.senderEmail = "";

            // 발신자 팩스번호
            statement.senderFAX = "";

            /**************************************************************************
             *                               수신자 정보                                 *
             **************************************************************************/

            // 수신자 사업자번호
            statement.receiverCorpNum = "8888888888";

            // 수신자 상호
            statement.receiverCorpName = "수신자 상호";

            // 수신자 대표자성명
            statement.receiverCEOName = "수신자 대표자 성명";

            // 수신자 주소
            statement.receiverAddr = "수신자 주소";

            // 수신자 종목
            statement.receiverBizClass = "수신자 종목";

            // 수신자 업태
            statement.receiverBizType = "수신자 업태";

            // 수신자 종목
            statement.receiverBizClass = "수신자 종목";

            // 수신자 성명
            statement.receiverContactName = "수신자 담당자명";

            // 수신자 부서명
            statement.receiverDeptName = "수신자 부서명";

            // 수신자 연락처
            statement.receiverTEL = "";

            // 수신자 휴대전화
            statement.receiverHP = "";

            // 수신자 이메일주소
            // 팝빌 개발환경에서 테스트하는 경우에도 안내 메일이 전송되므로,
            // 실제 거래처의 메일주소가 기재되지 않도록 주의
            statement.receiverEmail = "";

            // 수신자 팩스번호
            statement.receiverFAX = "";


            /**************************************************************************
             *                           전자명세서 기재항목                               *
             **************************************************************************/

            // 공급가액 합계
            statement.supplyCostTotal = "200000";

            // 세액 합계
            statement.taxTotal = "20000";

            // 합계금액
            statement.totalAmount = "220000";

            // 기재상 일련번호 항목
            statement.serialNum = "123";

            // 기재상 비고 항목
            statement.remark1 = "비고1";
            statement.remark2 = "비고2";
            statement.remark3 = "비고3";

            // 사업자등록증 이미지 첨부여부 (true / false 중 택 1)
            // └ true = 첨부 , false = 미첨부(기본값)
            // - 팝빌 사이트 또는 인감 및 첨부문서 등록 팝업 URL (GetSealURL API) 함수를 이용하여 등록
            statement.businessLicenseYN = false;

            // 통장사본 이미지 첨부여부 (true / false 중 택 1)
            // └ true = 첨부 , false = 미첨부(기본값)
            // - 팝빌 사이트 또는 인감 및 첨부문서 등록 팝업 URL (GetSealURL API) 함수를 이용하여 등록
            statement.bankBookYN = false;

            // 문자 자동전송 여부 (true / false 중 택 1)
            // └ true = 전송 , false = 미전송(기본값)
            statement.smssendYN = false;

            // 상세항목(품목) 정보 객체
            statement.detailList = new List<StatementDetail>();

            StatementDetail detail = new StatementDetail();

            detail.serialNum = 1; // 일련번호 1부터 순차기재
            detail.purchaseDT = "20220527"; // 거래일자 작성형식 yyyyMMdd
            detail.itemName = "품목명"; // 품목명
            detail.spec = "규격"; // 규격
            detail.qty = "1"; // 수량
            detail.unitCost = "100000"; // 단가
            detail.supplyCost = "100000"; // 공급가액
            detail.tax = "10000"; // 세액
            detail.remark = "품목비고"; // 비고
            detail.spare1 = "spare1"; // 여분
            detail.spare2 = "spare2";
            detail.spare3 = "spare3";
            detail.spare4 = "spare4";
            detail.spare5 = "spare5";

            statement.detailList.Add(detail);

            detail = new StatementDetail();

            detail.serialNum = 2; // 일련번호 1부터 순차기재
            detail.purchaseDT = "20220527"; // 거래일자 작성형식 yyyyMMdd
            detail.itemName = "품목명"; // 품목명
            detail.spec = "규격"; // 규격
            detail.qty = "1"; // 수량
            detail.unitCost = "100000"; // 단가
            detail.supplyCost = "100000"; // 공급가액
            detail.tax = "10000"; // 세액
            detail.remark = "품목비고"; // 비고
            detail.spare1 = "spare1"; // 여분
            detail.spare2 = "spare2";
            detail.spare3 = "spare3";
            detail.spare4 = "spare4";
            detail.spare5 = "spare5";

            statement.detailList.Add(detail);

            /************************************************************
             * 전자명세서 추가속성
             * [https://developers.popbill.com/guide/statement/dotnetcore/introduction/statement-form#propertybag-table]
             ************************************************************/
            statement.propertyBag = new propertyBag();

            statement.propertyBag.Add("Balance", "15000"); // 전잔액
            statement.propertyBag.Add("Deposit", "5000"); // 입금액
            statement.propertyBag.Add("CBalance", "20000"); // 현잔액

            // 즉시발행
            string memo = "즉시발행 메모";

            // 발행 안내 메일 제목
            // - 미입력 시 팝빌에서 지정한 이메일 제목으로 전송
            string emailSubject = "";

            try
            {
                var STMIssueResponse = _statementService.RegistIssue(corpNum, statement, memo, userID, emailSubject);
                return View("STMIssueResponse", STMIssueResponse);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 작성된 전자명세서 데이터를 팝빌에 저장합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/issue#Register
         */
        public IActionResult Register()
        {
            // 전자명세서 정보 객체
            Statement statement = new Statement();

            // 기재상 작성일자 날짜형식(yyyyMMdd)
            statement.writeDate = "20220527";

            // {영수, 청구} 중 기재
            statement.purposeType = "영수";

            // 과세형태, {과세, 영세, 면세} 중 기재
            statement.taxType = "과세";

            // 맞춤양식코드, 기본값을 공백('')으로 처리하면 기본양식으로 처리.
            statement.formCode = "";

            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
            statement.itemCode = 121;

            // 문서번호, 1~24자리 숫자, 영문, '-', '_' 조합으로 사업자별로 중복되지 않도록 구성
            statement.mgtKey = "20220527-002";


            /**************************************************************************
             *                             발신자 정보                                   *
             **************************************************************************/

            // 발신자 사업자번호
            statement.senderCorpNum = corpNum;

            // 종사업자 식별번호. 필요시 기재. 형식은 숫자 4자리.
            statement.senderTaxRegID = "";

            // 발신자 상호
            statement.senderCorpName = "발신자 상호";

            // 발신자 대표자성명
            statement.senderCEOName = "발신자 대표자 성명";

            // 발신자 주소
            statement.senderAddr = "발신자 주소";

            // 발신자 종목
            statement.senderBizClass = "발신자 종목";

            // 발신자 업태
            statement.senderBizType = "발신자 업태";

            // 발신자 종목
            statement.senderBizClass = "발신자 종목";

            // 발신자 성명
            statement.senderContactName = "발신자 담당자명";

            // 발신자 부서명
            statement.senderDeptName = "발신자 부서명";

            // 발신자 연락처
            statement.senderTEL = "";

            // 발신자 휴대전화
            statement.senderHP = "";

            // 발신자 이메일주소
            statement.senderEmail = "";

            // 발신자 팩스번호
            statement.senderFAX = "";


            /**************************************************************************
             *                               수신자 정보                                 *
             **************************************************************************/

            // 수신자 사업자번호
            statement.receiverCorpNum = "8888888888";

            // 수신자 상호
            statement.receiverCorpName = "수신자 상호";

            // 수신자 대표자성명
            statement.receiverCEOName = "수신자 대표자 성명";

            // 수신자 주소
            statement.receiverAddr = "수신자 주소";

            // 수신자 종목
            statement.receiverBizClass = "수신자 종목";

            // 수신자 업태
            statement.receiverBizType = "수신자 업태";

            // 수신자 종목
            statement.receiverBizClass = "수신자 종목";

            // 수신자 성명
            statement.receiverContactName = "수신자 담당자명";

            // 수신자 부서명
            statement.receiverDeptName = "수신자 부서명";

            // 수신자 연락처
            statement.receiverTEL = "";

            // 수신자 휴대전화
            statement.receiverHP = "";

            // 수신자 이메일주소
            // 팝빌 개발환경에서 테스트하는 경우에도 안내 메일이 전송되므로,
            // 실제 거래처의 메일주소가 기재되지 않도록 주의
            statement.receiverEmail = "";

            // 수신자 팩스번호
            statement.receiverFAX = "";


            /**************************************************************************
             *                           전자명세서 기재항목                               *
             **************************************************************************/

            // 공급가액 합계
            statement.supplyCostTotal = "200000";

            // 세액 합계
            statement.taxTotal = "20000";

            // 합계금액
            statement.totalAmount = "220000";

            // 기재상 일련번호 항목
            statement.serialNum = "123";

            // 기재상 비고 항목
            statement.remark1 = "비고1";
            statement.remark2 = "비고2";
            statement.remark3 = "비고3";

            // 사업자등록증 이미지 첨부여부 (true / false 중 택 1)
            // └ true = 첨부 , false = 미첨부(기본값)
            // - 팝빌 사이트 또는 인감 및 첨부문서 등록 팝업 URL (GetSealURL API) 함수를 이용하여 등록
            statement.businessLicenseYN = false;

            // 통장사본 이미지 첨부여부 (true / false 중 택 1)
            // └ true = 첨부 , false = 미첨부(기본값)
            // - 팝빌 사이트 또는 인감 및 첨부문서 등록 팝업 URL (GetSealURL API) 함수를 이용하여 등록
            statement.bankBookYN = false;

            // 문자 자동전송 여부 (true / false 중 택 1)
            // └ true = 전송 , false = 미전송(기본값)
            statement.smssendYN = false;

            // 상세항목(품목) 정보 객체
            statement.detailList = new List<StatementDetail>();

            StatementDetail detail = new StatementDetail();

            detail.serialNum = 1; // 일련번호 1부터 순차기재
            detail.purchaseDT = "20220527"; // 거래일자 작성형식 yyyyMMdd
            detail.itemName = "품목명"; // 품목명
            detail.spec = "규격"; // 규격
            detail.qty = "1"; // 수량
            detail.unitCost = "100000"; // 단가
            detail.supplyCost = "100000"; // 공급가액
            detail.tax = "10000"; // 세액
            detail.remark = "품목비고"; // 비고
            detail.spare1 = "spare1"; // 여분
            detail.spare2 = "spare2";
            detail.spare3 = "spare3";
            detail.spare4 = "spare4";
            detail.spare5 = "spare5";

            statement.detailList.Add(detail);

            detail = new StatementDetail();

            detail.serialNum = 2; // 일련번호 1부터 순차기재
            detail.purchaseDT = "20220527"; // 거래일자 작성형식 yyyyMMdd
            detail.itemName = "품목명"; // 품목명
            detail.spec = "규격"; // 규격
            detail.qty = "1"; // 수량
            detail.unitCost = "100000"; // 단가
            detail.supplyCost = "100000"; // 공급가액
            detail.tax = "10000"; // 세액
            detail.remark = "품목비고"; // 비고
            detail.spare1 = "spare1"; // 여분
            detail.spare2 = "spare2";
            detail.spare3 = "spare3";
            detail.spare4 = "spare4";
            detail.spare5 = "spare5";

            statement.detailList.Add(detail);

            /************************************************************
             * 전자명세서 추가속성
             * [https://developers.popbill.com/guide/statement/dotnetcore/introduction/statement-form#propertybag-table]
             ************************************************************/
            statement.propertyBag = new propertyBag();

            statement.propertyBag.Add("Balance", "15000"); // 전잔액
            statement.propertyBag.Add("Deposit", "5000"); // 입금액
            statement.propertyBag.Add("CBalance", "20000"); // 현잔액

            try
            {
                var response = _statementService.Register(corpNum, statement);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * "임시저장" 상태의 전자명세서를 수정합니다.건의 전자명세서를 [수정]합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/issue#Update
         */
        public IActionResult Update()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 수정할 명세서 문서번호
            string mgtKey = "20220527-002";


            // 전자명세서 정보 객체
            Statement statement = new Statement();

            // 기재상 작성일자 날짜형식(yyyyMMdd)
            statement.writeDate = "20220527";

            // {영수, 청구} 중 기재
            statement.purposeType = "영수";

            // 과세형태, {과세, 영세, 면세} 중 기재
            statement.taxType = "과세";

            // 맞춤양식코드, 기본값을 공백('')으로 처리하면 기본양식으로 처리.
            statement.formCode = "";


            /**************************************************************************
             *                             발신자 정보                                   *
             **************************************************************************/

            // 발신자 사업자번호
            statement.senderCorpNum = corpNum;

            // 종사업자 식별번호. 필요시 기재. 형식은 숫자 4자리.
            statement.senderTaxRegID = "";

            // 발신자 상호
            statement.senderCorpName = "발신자 상호";

            // 발신자 대표자성명
            statement.senderCEOName = "발신자 대표자 성명";

            // 발신자 주소
            statement.senderAddr = "발신자 주소";

            // 발신자 종목
            statement.senderBizClass = "발신자 종목";

            // 발신자 업태
            statement.senderBizType = "발신자 업태";

            // 발신자 종목
            statement.senderBizClass = "발신자 종목";

            // 발신자 성명
            statement.senderContactName = "발신자 담당자명";

            // 발신자 부서명
            statement.senderDeptName = "발신자 부서명";

            // 발신자 연락처
            statement.senderTEL = "";

            // 발신자 휴대전화
            statement.senderHP = "";

            // 발신자 이메일주소
            statement.senderEmail = "";

            // 발신자 팩스번호
            statement.senderFAX = "";


            /**************************************************************************
             *                               수신자 정보                                 *
             **************************************************************************/

            // 수신자 사업자번호
            statement.receiverCorpNum = "8888888888";

            // 수신자 상호
            statement.receiverCorpName = "수신자 상호";

            // 수신자 대표자성명
            statement.receiverCEOName = "수신자 대표자 성명";

            // 수신자 주소
            statement.receiverAddr = "수신자 주소";

            // 수신자 종목
            statement.receiverBizClass = "수신자 종목";

            // 수신자 업태
            statement.receiverBizType = "수신자 업태";

            // 수신자 종목
            statement.receiverBizClass = "수신자 종목";

            // 수신자 성명
            statement.receiverContactName = "수신자 담당자명";

            // 수신자 부서명
            statement.receiverDeptName = "수신자 부서명";

            // 수신자 연락처
            statement.receiverTEL = "";

            // 수신자 휴대전화
            statement.receiverHP = "";

            // 수신자 이메일주소
            // 팝빌 개발환경에서 테스트하는 경우에도 안내 메일이 전송되므로,
            // 실제 거래처의 메일주소가 기재되지 않도록 주의
            statement.receiverEmail = "";

            // 수신자 팩스번호
            statement.receiverFAX = "";


            /**************************************************************************
             *                           전자명세서 기재항목                               *
             **************************************************************************/

            // 공급가액 합계
            statement.supplyCostTotal = "200000";

            // 세액 합계
            statement.taxTotal = "20000";

            // 합계금액
            statement.totalAmount = "220000";

            // 기재상 일련번호 항목
            statement.serialNum = "123";

            // 기재상 비고 항목
            statement.remark1 = "비고1";
            statement.remark2 = "비고2";
            statement.remark3 = "비고3";

            // 사업자등록증 이미지 첨부여부 (true / false 중 택 1)
            // └ true = 첨부 , false = 미첨부(기본값)
            // - 팝빌 사이트 또는 인감 및 첨부문서 등록 팝업 URL (GetSealURL API) 함수를 이용하여 등록
            statement.businessLicenseYN = false;

            // 통장사본 이미지 첨부여부 (true / false 중 택 1)
            // └ true = 첨부 , false = 미첨부(기본값)
            // - 팝빌 사이트 또는 인감 및 첨부문서 등록 팝업 URL (GetSealURL API) 함수를 이용하여 등록
            statement.bankBookYN = false;

            // 문자 자동전송 여부 (true / false 중 택 1)
            // └ true = 전송 , false = 미전송(기본값)
            statement.smssendYN = false;

            // 상세항목(품목) 정보 객체
            statement.detailList = new List<StatementDetail>();

            StatementDetail detail = new StatementDetail();

            detail.serialNum = 1; // 일련번호 1부터 순차기재
            detail.purchaseDT = "20220527"; // 거래일자 작성형식 yyyyMMdd
            detail.itemName = "품목명(수정)"; // 품목명
            detail.spec = "규격"; // 규격
            detail.qty = "1"; // 수량
            detail.unitCost = "100000"; // 단가
            detail.supplyCost = "100000"; // 공급가액
            detail.tax = "10000"; // 세액
            detail.remark = "품목비고"; // 비고
            detail.spare1 = "spare1"; // 여분
            detail.spare2 = "spare2";
            detail.spare3 = "spare3";
            detail.spare4 = "spare4";
            detail.spare5 = "spare5";

            statement.detailList.Add(detail);

            detail = new StatementDetail();

            detail.serialNum = 2; // 일련번호 1부터 순차기재
            detail.purchaseDT = "20220527"; // 거래일자 작성형식 yyyyMMdd
            detail.itemName = "품목명"; // 품목명
            detail.spec = "규격"; // 규격
            detail.qty = "1"; // 수량
            detail.unitCost = "100000"; // 단가
            detail.supplyCost = "100000"; // 공급가액
            detail.tax = "10000"; // 세액
            detail.remark = "품목비고"; // 비고
            detail.spare1 = "spare1"; // 여분
            detail.spare2 = "spare2";
            detail.spare3 = "spare3";
            detail.spare4 = "spare4";
            detail.spare5 = "spare5";

            statement.detailList.Add(detail);

            /************************************************************
             * 전자명세서 추가속성
             * [https://developers.popbill.com/guide/statement/dotnetcore/introduction/statement-form#propertybag-table]
             ************************************************************/
            statement.propertyBag = new propertyBag();

            statement.propertyBag.Add("Balance", "15000"); // 전잔액
            statement.propertyBag.Add("Deposit", "5000"); // 입금액
            statement.propertyBag.Add("CBalance", "20000"); // 현잔액
            try
            {
                var response = _statementService.Update(corpNum, itemCode, mgtKey, statement);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * "임시저장" 상태의 전자명세서를 발행하여, "발행완료" 상태로 처리합니다.
         * - 팝빌 사이트 [전자명세서] > [환경설정] > [전자명세서 관리] 메뉴의 발행시 자동승인 옵션 설정을 통해 전자명세서를 "발행완료" 상태가 아닌 "승인대기" 상태로 발행 처리 할 수 있습니다.
         * - 전자명세서 발행 함수 호출시 포인트가 과금되며, 수신자에게 발행 안내 메일이 발송됩니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/issue#Issue
         */
        public IActionResult Issue()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 발행처리할 명세서 문서번호
            string mgtKey = "20220527-001";

            // 발행 메모
            string memo = "발행 메모";

            try
            {
                var response = _statementService.Issue(corpNum, itemCode, mgtKey, memo);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 발신자가 발행한 전자명세서를 발행취소합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/issue#Cancel
         */
        public IActionResult Cancel()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서) 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 발행취소할 명세서 문서번호
            string mgtKey = "20220527-001";

            // 발행 메모
            string memo = "발행 메모";

            try
            {
                var response = _statementService.Cancel(corpNum, itemCode, mgtKey, memo);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 삭제 가능한 상태의 전자명세서를 삭제합니다.
         * - 삭제 가능한 상태: "임시저장", "취소", "승인거부", "발행취소"
         * - 전자명세서를 삭제하면 사용된 문서번호(mgtKey)를 재사용할 수 있습니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/issue#Delete
         */
        public IActionResult Delete()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서) 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 삭제처리할 명세서 문서번호
            string mgtKey = "20220527-001";

            try
            {
                var response = _statementService.Delete(corpNum, itemCode, mgtKey);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        #endregion

        #region 전자명세서 정보확인

        /*
         * 전자명세서의 1건의 상태 및 요약정보 확인합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/info#GetInfo
         */
        public IActionResult GetInfo()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서) 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 명세서 문서번호, 사업자별로 중복되지 않도록 문서번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20220527-001";

            try
            {
                var response = _statementService.GetInfo(corpNum, itemCode, mgtKey);
                return View("GetInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 다수건의 전자명세서 상태/요약 정보를 확인합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/info#GetInfos
         */
        public IActionResult GetInfos()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서) 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 조회할 전자명세서 문서번호 배열, (최대 1000건)
            List<string> mgtKeyList = new List<string>();
            mgtKeyList.Add("20220527-001");
            mgtKeyList.Add("20220527-002");

            try
            {
                var response = _statementService.GetInfos(corpNum, itemCode, mgtKeyList);
                return View("GetInfos", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 전자명세서 1건의 상세정보 확인합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/info#GetDetailInfo
         */
        public IActionResult GetDetailInfo()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서) 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 명세서 문서번호, 사업자별로 중복되지 않도록 문서번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20220527-001";
            try
            {
                var response = _statementService.GetDetailInfo(corpNum, itemCode, mgtKey);
                return View("GetDetailInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 검색조건에 해당하는 전자명세서를 조회합니다. (조회기간 단위 : 최대 6개월)
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/info#Search
         */
        public IActionResult Search()
        {
            // 일자 유형 ("R" , "W" , "I" 중 택 1)
            // └ R = 등록일자 , W = 작성일자 , I = 발행일자
            string DType = "W";

            // 시작일자, 날짜형식(yyyyMMdd)
            string SDate = "20220501";

            // 종료일자, 날짜형식(yyyyMMdd)
            string EDate = "20220531";

            // 전자명세서 상태코드 배열 (2,3번째 자리에 와일드카드(*) 사용 가능)
            // - 미입력시 전체조회
            string[] State = new string[4];
            State[0] = "100";
            State[1] = "2**";
            State[2] = "3**";
            State[3] = "4**";

            // 명세서 코드배열 - 121(거래명세서), 122(청구서), 123(견적서) 124(발주서), 125(입금표), 126(영수증)
            int[] itemCode = {121, 122, 123, 124, 125, 126};

            // 페이지 번호, 기본값 '1'
            int Page = 1;

            // 페이지당 검색개수, 기본값 '500', 최대 '1000'
            int PerPage = 50;

            // 정렬방향, D-내림차순, A-오름차순
            string Order = "D";

            // 통합검색어, 거래처 상호명 또는 거래처 사업자번호로 조회
            // - 미입력시 전체조회
            string QString = "";

            try
            {
                var response = _statementService.Search(corpNum, DType, SDate, EDate, State, itemCode, Page, PerPage,
                    Order, QString);
                return View("Search", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 전자명세서의 상태에 대한 변경이력을 확인합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/info#GetLogs
         */
        public IActionResult GetLogs()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서) 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 명세서 문서번호, 사업자별로 중복되지 않도록 문서번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20220527-001";
            try
            {
                var response = _statementService.GetLogs(corpNum, itemCode, mgtKey);
                return View("GetLogs", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 로그인 상태로 팝빌 사이트의 전자명세서 문서함 메뉴에 접근할 수 있는 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/info#GetURL
         */
        public IActionResult GetURL()
        {
            // 임시문서함(TBOX), 발행문서함(SBOX)
            string TOGO = "TBOX";

            try
            {
                var result = _statementService.GetURL(corpNum, TOGO, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        #endregion

        #region 전자명세서 보기/인쇄

        /*
         * 팝빌 사이트와 동일한 전자명세서 1건의 상세 정보 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/view#GetPopUpURL
         */
        public IActionResult GetPopUpURL()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서) 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 명세서 문서번호, 사업자별로 중복되지 않도록 문서번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20220527-001";

            try
            {
                var result = _statementService.GetPopUpURL(corpNum, itemCode, mgtKey, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌 사이트와 동일한 전자명세서 1건의 상세 정보 페이지(사이트 상단, 좌측 메뉴 및 버튼 제외)의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/view#GetViewURL
         */
        public IActionResult GetViewURL()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서) 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 명세서 문서번호, 사업자별로 중복되지 않도록 문서번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20220527-001";

            try
            {
                var result = _statementService.GetViewURL(corpNum, itemCode, mgtKey, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        /*
         * 전자명세서 1건을 인쇄하기 위한 페이지의 팝업 URL을 반환하며, 페이지내에서 인쇄 설정값을 "공급자" / "공급받는자" / "공급자+공급받는자"용 중 하나로 지정할 수 있습니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - 전자명세서의 공급자는 "발신자", 공급받는자는 "수신자"를 나타내는 용어입니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/view#GetPrintURL
         */
        public IActionResult GetPrintURL()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서) 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 명세서 문서번호, 사업자별로 중복되지 않도록 문서번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20220527-001";

            try
            {
                var result = _statementService.GetPrintURL(corpNum, itemCode, mgtKey, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * "공급받는자" 용 전자명세서 1건을 인쇄하기 위한 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - 전자명세서의 공급받는자는 "수신자"를 나타내는 용어입니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/view#GetEPrintURL
         */
        public IActionResult GetEPrintURL()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서) 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 명세서 문서번호, 사업자별로 중복되지 않도록 문서번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20220527-001";

            try
            {
                var result = _statementService.GetEPrintURL(corpNum, itemCode, mgtKey, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 다수건의 전자명세서를 인쇄하기 위한 페이지의 팝업 URL을 반환합니다. (최대 100건)
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/view#GetMassPrintURL
         */
        public IActionResult GetMassPrintURL()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서) 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 조회할 전자명세서 문서번호 배열, (최대 100건)
            List<string> mgtKeyList = new List<string>();
            mgtKeyList.Add("20220527-001");
            mgtKeyList.Add("20220527-002");

            try
            {
                var result = _statementService.GetMassPrintURL(corpNum, itemCode, mgtKeyList, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 전자명세서 안내메일의 상세보기 링크 URL을 반환합니다.
         * - 함수 호출로 반환 받은 URL에는 유효시간이 없습니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/view#GetMailURL
         */
        public IActionResult GetMailURL()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서) 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 명세서 문서번호, 사업자별로 중복되지 않도록 문서번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20220527-001";

            try
            {
                var result = _statementService.GetMailURL(corpNum, itemCode, mgtKey, userID);
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
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/member#GetAccessURL
         */
        public IActionResult GetAccessURL()
        {
            try
            {
                var result = _statementService.GetAccessURL(corpNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 전자명세서에 첨부할 인감, 사업자등록증, 통장사본을 등록하는 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/etc#GetSealURL
         */
        public IActionResult GetSealURL()
        {
            try
            {
                var result = _statementService.GetSealURL(corpNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * "임시저장" 상태의 명세서에 1개의 파일을 첨부합니다. (최대 5개)
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/etc#AttachFile
         */
        public IActionResult AttachFile()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서) 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 명세서 문서번호, 사업자별로 중복되지 않도록 문서번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20220527-002";

            // 파일경로
            string filePath = "C:/popbill.example.dotnetcore/StatementExample/wwwroot/images/tax_image.png";

            try
            {
                var response = _statementService.AttachFile(corpNum, itemCode, mgtKey, filePath);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * "임시저장" 상태의 전자명세서에 첨부된 1개의 파일을 삭제합니다.
         * - 파일을 식별하는 파일아이디는 첨부파일 목록(GetFiles API) 의 응답항목 중 파일아이디(AttachedFile) 값을 통해 확인할 수 있습니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/etc#DeleteFile
         */
        public IActionResult DeleteFile()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 명세서 문서번호, 사업자별로 중복되지 않도록 문서번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20220527-002";

            // 파일아이디, 첨부파일 목록(GetFiles API) 의 응답항목 중 파일아이디(AttachedFile) 값
            string fileID = "";

            try
            {
                var response = _statementService.DeleteFile(corpNum, itemCode, mgtKey, fileID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 전자명세서에 첨부된 파일목록을 확인합니다.
         * - 응답항목 중 파일아이디(AttachedFile) 항목은 파일삭제(DeleteFile API) 호출시 이용할 수 있습니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/etc#GetFiles
         */
        public IActionResult GetFiles()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 명세서 문서번호, 사업자별로 중복되지 않도록 문서번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20220527-002";

            try
            {
                var response = _statementService.GetFiles(corpNum, itemCode, mgtKey);
                return View("GetFiles", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * "승인대기", "발행완료" 상태의 전자명세서와 관련된 발행 안내 메일을 재전송 합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/etc#SendEmail
         */
        public IActionResult SendEmail()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 명세서 문서번호, 사업자별로 중복되지 않도록 문서번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20220527-002";

            // 수신자 이메일주소
            string receiver = "";

            try
            {
                var response = _statementService.SendEmail(corpNum, itemCode, mgtKey, receiver);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 전자명세서와 관련된 안내 SMS(단문) 문자를 재전송하는 함수로, 팝빌 사이트 [문자·팩스] > [문자] > [전송내역] 메뉴에서 전송결과를 확인 할 수 있습니다.
         * - 메시지는 최대 90byte까지 입력 가능하고, 초과한 내용은 자동으로 삭제되어 전송합니다. (한글 최대 45자)
         * - 함수 호출시 포인트가 과금됩니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/etc#SendSMS
         */
        public IActionResult SendSMS()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 명세서 문서번호, 사업자별로 중복되지 않도록 문서번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20220527-002";

            // 발신번호
            string sender = "";

            // 수신번호
            string receiver = "";

            // 문자메시지 내용, 90byte 초과시 길이가 조정되어 전송됨
            string contents = "알림문자 전송내용, 90byte 초과된 내용은 삭제되어 전송됨";

            try
            {
                var response = _statementService.SendSMS(corpNum, itemCode, mgtKey, sender, receiver, contents);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 전자명세서를 팩스로 전송하는 함수로, 팝빌 사이트 [문자·팩스] > [팩스] > [전송내역] 메뉴에서 전송결과를 확인 할 수 있습니다.
         * - 함수 호출시 포인트가 과금됩니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/etc#SendFAX
         */
        public IActionResult SendFAX()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 명세서 문서번호, 사업자별로 중복되지 않도록 문서번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20220527-002";

            // 발신번호
            string sender = "";

            // 수신번호
            string receiver = "";

            try
            {
                var response = _statementService.SendFAX(corpNum, itemCode, mgtKey, sender, receiver);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 전자명세서를 팩스로 전송하는 함수로, 팝빌에 데이터를 저장하는 과정이 없습니다.
         * - 팝빌 사이트 [문자·팩스] > [팩스] > [전송내역] 메뉴에서 전송결과를 확인 할 수 있습니다.
         * - 함수 호출시 포인트가 과금됩니다.
         * - 팩스 발행 요청시 작성한 문서번호는 팩스전송 파일명으로 사용됩니다.
         * - 팩스 전송결과를 확인하기 위해서는 선팩스 전송 요청 시 반환받은 접수번호를 이용하여 팩스 API의 전송결과 확인 (GetFaxResult) API를 이용하면 됩니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/etc#FAXSend
         */
        public IActionResult FAXSend()
        {
            // 전자명세서 정보 객체
            Statement statement = new Statement();

            // 기재상 작성일자 날짜형식(yyyyMMdd)
            statement.writeDate = "20220527";

            // {영수, 청구} 중 기재
            statement.purposeType = "영수";

            // 과세형태, {과세, 영세, 면세} 중 기재
            statement.taxType = "과세";

            // 맞춤양식코드, 기본값을 공백('')으로 처리하면 기본양식으로 처리.
            statement.formCode = "";

            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서) 124(발주서), 125(입금표), 126(영수증)
            statement.itemCode = 121;

            // 문서번호, 1~24자리 숫자, 영문, '-', '_' 조합으로 사업자별로 중복되지 않도록 구성
            statement.mgtKey = "20220527-002";


            /**************************************************************************
             *                             발신자 정보                                   *
             **************************************************************************/

            // 발신자 사업자번호
            statement.senderCorpNum = corpNum;

            // 종사업자 식별번호. 필요시 기재. 형식은 숫자 4자리.
            statement.senderTaxRegID = "";

            // 발신자 상호
            statement.senderCorpName = "발신자 상호";

            // 발신자 대표자성명
            statement.senderCEOName = "발신자 대표자 성명";

            // 발신자 주소
            statement.senderAddr = "발신자 주소";

            // 발신자 종목
            statement.senderBizClass = "발신자 종목";

            // 발신자 업태
            statement.senderBizType = "발신자 업태";

            // 발신자 종목
            statement.senderBizClass = "발신자 종목";

            // 발신자 성명
            statement.senderContactName = "발신자 담당자명";

            // 발신자 부서명
            statement.senderDeptName = "발신자 부서명";

            // 발신자 연락처
            statement.senderTEL = "";

            // 발신자 휴대전화
            statement.senderHP = "";

            // 발신자 이메일주소
            statement.senderEmail = "";

            // 발신자 팩스번호
            statement.senderFAX = "";


            /**************************************************************************
             *                               수신자 정보                                 *
             **************************************************************************/

            // 수신자 사업자번호
            statement.receiverCorpNum = "8888888888";

            // 수신자 상호
            statement.receiverCorpName = "수신자 상호";

            // 수신자 대표자성명
            statement.receiverCEOName = "수신자 대표자 성명";

            // 수신자 주소
            statement.receiverAddr = "수신자 주소";

            // 수신자 종목
            statement.receiverBizClass = "수신자 종목";

            // 수신자 업태
            statement.receiverBizType = "수신자 업태";

            // 수신자 종목
            statement.receiverBizClass = "수신자 종목";

            // 수신자 성명
            statement.receiverContactName = "수신자 담당자명";

            // 수신자 부서명
            statement.receiverDeptName = "수신자 부서명";

            // 수신자 연락처
            statement.receiverTEL = "";

            // 수신자 휴대전화
            statement.receiverHP = "";

            // 수신자 이메일주소
            // 팝빌 개발환경에서 테스트하는 경우에도 안내 메일이 전송되므로,
            // 실제 거래처의 메일주소가 기재되지 않도록 주의
            statement.receiverEmail = "";

            // 수신자 팩스번호
            statement.receiverFAX = "";


            /**************************************************************************
             *                           전자명세서 기재항목                               *
             **************************************************************************/

            // 공급가액 합계
            statement.supplyCostTotal = "200000";

            // 세액 합계
            statement.taxTotal = "20000";

            // 합계금액
            statement.totalAmount = "220000";

            // 기재상 일련번호 항목
            statement.serialNum = "123";

            // 기재상 비고 항목
            statement.remark1 = "비고1";
            statement.remark2 = "비고2";
            statement.remark3 = "비고3";

            // 사업자등록증 이미지 첨부여부 (true / false 중 택 1)
            // └ true = 첨부 , false = 미첨부(기본값)
            // - 팝빌 사이트 또는 인감 및 첨부문서 등록 팝업 URL (GetSealURL API) 함수를 이용하여 등록
            statement.businessLicenseYN = false;

            // 통장사본 이미지 첨부여부 (true / false 중 택 1)
            // └ true = 첨부 , false = 미첨부(기본값)
            // - 팝빌 사이트 또는 인감 및 첨부문서 등록 팝업 URL (GetSealURL API) 함수를 이용하여 등록
            statement.bankBookYN = false;

            // 문자 자동전송 여부 (true / false 중 택 1)
            // └ true = 전송 , false = 미전송(기본값)
            statement.smssendYN = false;

            // 상세항목(품목) 정보 객체
            statement.detailList = new List<StatementDetail>();

            StatementDetail detail = new StatementDetail
            {
                serialNum = 1, // 일련번호 1부터 순차기재
                purchaseDT = "20220527", // 거래일자 작성형식 yyyyMMdd
                itemName = "품목명", // 품목명
                spec = "규격", // 규격
                qty = "1", // 수량
                unitCost = "100000", // 단가
                supplyCost = "100000", // 공급가액
                tax = "10000", // 세액
                remark = "품목비고", // 비고
                spare1 = "spare1", // 여분
                spare2 = "spare2",
                spare3 = "spare3",
                spare4 = "spare4",
                spare5 = "spare5"
            };
            statement.detailList.Add(detail);

            detail = new StatementDetail
            {
                serialNum = 2, // 일련번호 1부터 순차기재
                purchaseDT = "20220527", // 거래일자 작성형식 yyyyMMdd
                itemName = "품목명", // 품목명
                spec = "규격", // 규격
                qty = "1", // 수량
                unitCost = "100000", // 단가
                supplyCost = "100000", // 공급가액
                tax = "10000", // 세액
                remark = "품목비고", // 비고
                spare1 = "spare1", // 여분
                spare2 = "spare2",
                spare3 = "spare3",
                spare4 = "spare4",
                spare5 = "spare5"
            };
            statement.detailList.Add(detail);

            /************************************************************
             * 전자명세서 추가속성
             * [https://developers.popbill.com/guide/statement/dotnetcore/introduction/statement-form#propertybag-table]
             ************************************************************/
            statement.propertyBag = new propertyBag();

            statement.propertyBag.Add("Balance", "15000"); // 전잔액
            statement.propertyBag.Add("Deposit", "5000"); // 입금액
            statement.propertyBag.Add("CBalance", "20000"); // 현잔액

            // 발신번호
            string sendNum = "";

            // 수신번호
            string receiveNum = "";

            try
            {
                var result = _statementService.FAXSend(corpNum, statement, sendNum, receiveNum);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 하나의 전자명세서에 다른 전자명세서를 첨부합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/etc#AttachStatement
         */
        public IActionResult AttachStatement()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 명세서 문서번호, 사업자별로 중복되지 않도록 문서번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20220527-001";

            // 첨부할 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
            int subItemCode = 121;

            // 첨부할 명세서 문서번호
            string subMgtKey = "20220527-002";

            try
            {
                var response =
                    _statementService.AttachStatement(corpNum, itemCode, mgtKey, subItemCode, subMgtKey);
                return View("response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 하나의 전자명세서에 첨부된 다른 전자명세서를 해제합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/etc#DetachStatement
         */
        public IActionResult DetachStatement()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 명세서 문서번호, 사업자별로 중복되지 않도록 문서번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20220527-001";

            // 첨부해제할 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
            int subItemCode = 121;

            // 첨부해제할 명세서 문서번호
            string subMgtKey = "20220527-002";

            try
            {
                var response =
                    _statementService.DetachStatement(corpNum, itemCode, mgtKey, subItemCode, subMgtKey);
                return View("response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 전자명세서 관련 메일 항목에 대한 발송설정을 확인합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/etc#ListEmailConfig
         */
        public IActionResult ListEmailConfig()
        {
            try
            {
                var result = _statementService.ListEmailConfig(corpNum);
                return View("ListEmailConfig", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 전자명세서 관련 메일 항목에 대한 발송설정을 수정합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/etc#UpdateEmailConfig
         *
         * 메일전송유형
         * SMT_ISSUE : 수신자에게 전자명세서가 발행 되었음을 알려주는 메일입니다.
         * SMT_ACCEPT : 발신자에게 전자명세서가 승인 되었음을 알려주는 메일입니다.
         * SMT_DENY : 발신자에게 전자명세서가 거부 되었음을 알려주는 메일입니다.
         * SMT_CANCEL : 수신자에게 전자명세서가 취소 되었음을 알려주는 메일입니다.
         * SMT_CANCEL_ISSUE : 수신자에게 전자명세서가 발행취소 되었음을 알려주는 메일입니다.
         */
        public IActionResult UpdateEmailConfig()
        {
            //메일전송유형
            string emailType = "SMT_ISSUE";

            //전송여부 (true-전송, false-미전송)
            bool sendYN = true;

            try
            {
                var response = _statementService.UpdateEmailConfig(corpNum, emailType, sendYN);
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
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/point#GetBalance
         */
        public IActionResult GetBalance()
        {
            try
            {
                var result = _statementService.GetBalance(corpNum);
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
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/point#GetChargeURL
         */
        public IActionResult GetChargeURL()
        {
            try
            {
                var result = _statementService.GetChargeURL(corpNum, userID);
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
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/point#GetPaymentURL
         */
        public IActionResult GetPaymentURL()
        {

            try
            {
                var result = _statementService.GetPaymentURL(corpNum, userID);
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
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/point#GetUseHistoryURL
         */
        public IActionResult GetUseHistoryURL()
        {

            try
            {
                var result = _statementService.GetUseHistoryURL(corpNum, userID);
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
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/point#GetPartnerBalance
         */
        public IActionResult GetPartnerBalance()
        {
            try
            {
                var result = _statementService.GetPartnerBalance(corpNum);
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
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/point#GetPartnerURL
         */
        public IActionResult GetPartnerURL()
        {
            // CHRG 포인트충전 URL
            string TOGO = "CHRG";

            try
            {
                var result = _statementService.GetPartnerURL(corpNum, TOGO);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 전자명세서 발행시 과금되는 포인트 단가를 확인합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/point#GetUnitCost
         */
        public IActionResult GetUnitCost()
        {
            try
            {
                // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
                int itemCode = 121;

                var result = _statementService.GetUnitCost(corpNum, itemCode);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌 전자명세서 API 서비스 과금정보를 확인합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/point#GetChargeInfo
         */
        public IActionResult GetChargeInfo()
        {
            try
            {
                // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
                int itemCode = 121;

                var response = _statementService.GetChargeInfo(corpNum, itemCode);
                return View("GetChargeInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

                /*
         * 연동회원 포인트를 환불 신청합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/point#Refund
         */
        public IActionResult Refund()
        {
            try
            {
                var refundForm = new RefundForm();

                // 담당자명
                refundForm.setContactName("담당자명");

                // 담당자 연락처
                refundForm.setTel("01077777777");

                // 환불 신청 포인트
                refundForm.setRequestPoint("10");

                // 은행명
                refundForm.setAccountBank("국민");

                // 계좌번호
                refundForm.setAccountNum("123123123-123");

                // 예금주명
                refundForm.setAccountName("예금주명");

                // 환불사유
                refundForm.setReason("환불사유");

                var response = _statementService.Refund(corpNum, refundForm);
                return View("RefundResponse", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 포인트를 환불 신청합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/point#PaymentRequest
         */
        public IActionResult PaymentRequest()
        {
            try
            {
                var paymentForm = new PaymentForm();

                // 담당자명
                paymentForm.setSettlerName("담당자명");

                // 담당자 이메일
                paymentForm.setSettlerEmail("test@test.com");

                // 담당자 휴대폰
                // └ 무통장 입금 승인 알림톡이 전송될 번호
                paymentForm.setNotifyHP("01012341234");

                // 입금자명
                paymentForm.setPaymentName("입금자명");

                // 결제금액
                paymentForm.setSettleCost("11000");

                var response = _statementService.paymentRequest(corpNum, paymentForm);
                return View("PaymentResponse", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 포인트 무통장 입금신청내역 1건을 확인합니다.
         *  - https://developers.popbill.com/reference/statement/dotnetcore/api/point#GetSettleResult
         */
        public IActionResult GetSettleResult()
        {
            // 정산코드
            var settleCode = "202301130000000026";

            try
            {
                var paymentHistory = _statementService.getSettleResult(corpNum, settleCode);

                return View("PaymentHistory", paymentHistory);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 포인트 사용내역을 확인합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/point#GetUseHistory
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
                var useHistoryResult = _statementService.getUseHistory(corpNum, SDate, EDate, Page,
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
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/point#GetPaymentHistory
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
                var paymentHistoryResult = _statementService.getPaymentHistory(corpNum, SDate, EDate,
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
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/point#GetRefundHistory
         */
        public IActionResult GetRefundHistory()
        {
            // 목록 페이지번호 (기본값 1)
            var Page = 1;

            // 페이지당 표시할 목록 개수 (기본값 500, 최대 1,000)
            var PerPage = 100;

            try
            {
                var refundHistoryResult = _statementService.getRefundHistory(corpNum, Page, PerPage);

                return View("RefundHistoryResult", refundHistoryResult);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        /*
         * 포인트 환불에 대한 상세정보 1건을 확인합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/point#GetRefundInfo
         */
        public IActionResult GetRefundInfo()
        {
            // 환불 코드
            var refundCode = "023040000017";

            try
            {
                var response = _statementService.getRefundInfo(corpNum, refundCode);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        /*
         * 환불 가능한 포인트를 확인합니다. (보너스 포인트는 환불가능포인트에서 제외됩니다.)
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/point#GetRefundableBalance
         */
        public IActionResult GetRefundableBalance()
        {
            try
            {
                var refundableBalance = _statementService.getRefundableBalance(corpNum);
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
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/member#CheckIsMember
         */
        public IActionResult CheckIsMember()
        {
            try
            {
                //링크아이디
                string linkID = "TESTER";

                var response = _statementService.CheckIsMember(corpNum, linkID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 사용하고자 하는 아이디의 중복여부를 확인합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/member#CheckID
         */
        public IActionResult CheckID()
        {
            //중복여부 확인할 팝빌 회원 아이디
            string checkID = "testkorea";

            try
            {
                var response = _statementService.CheckID(checkID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 사용자를 연동회원으로 가입처리합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/member#JoinMember
         */
        public IActionResult JoinMember()
        {
            JoinForm joinInfo = new JoinForm();

            // 링크아이디
            joinInfo.LinkID = "TESTER";

            // 비밀번호, 8자이상 20자 미만 (영문, 숫자, 특수문자 조합)
            joinInfo.Password = "asdfasdf123!@#";

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
            joinInfo.ContactEmail = "";

            // 담당자 연락처 (최대 20자)
            joinInfo.ContactTEL = "";

            try
            {
                var response = _statementService.JoinMember(joinInfo);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 회사정보를 확인합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/member#GetCorpInfo
         */
        public IActionResult GetCorpInfo()
        {
            try
            {
                var response = _statementService.GetCorpInfo(corpNum);
                return View("GetCorpInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 회사정보를 수정합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/member#UpdateCorpInfo
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
                var response = _statementService.UpdateCorpInfo(corpNum, corpInfo);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 담당자(팝빌 로그인 계정)를 추가합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/member#RegistContact
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
                var response = _statementService.RegistContact(corpNum, contactInfo);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 정보을 확인합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/member#GetContactInfo
         */
        public IActionResult GetContactInfo()
        {
            // 확인할 담당자 아이디
            string contactID = "test0730";

            try
            {
                var contactInfo = _statementService.GetContactInfo(corpNum, contactID);
                return View("GetContactInfo", contactInfo);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 목록을 확인합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/member#ListContact
         */
        public IActionResult ListContact()
        {
            try
            {
                var response = _statementService.ListContact(corpNum);
                return View("ListContact", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 정보를 수정합니다.
         * - https://developers.popbill.com/reference/statement/dotnetcore/api/member#UpdateContact
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

            // 담당자 이메일 (최대 100자)
            contactInfo.email = "";

            // 담당자 조회권한 설정, 1(개인권한), 2 (읽기권한), 3 (회사권한)
            contactInfo.searchRole = 3;

            try
            {
                var response = _statementService.UpdateContact(corpNum, contactInfo);
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
         *  - https://developers.popbill.com/reference/statement/dotnetcore/api/member#QuitMember
         */
        public IActionResult QuitMember()
        {
            // 탈퇴 사유
            var quitReason = "탈퇴사유";

            try
            {
                var response = _statementService.quitMember(corpNum, quitReason);
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
