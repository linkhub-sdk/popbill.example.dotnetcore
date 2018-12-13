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
            //전자명세서 서비스 객체 생성
            _statementService = STMinstance.statementService;

        }

        //팝빌 연동회원 사업자번호 (하이픈 '-' 없이 10자리)
        string corpNum = "1234567890";

        //팝빌 연동회원 아이디
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
         * 전자명세서 관리번호 중복여부를 확인합니다.
         * - 관리번호는 1~24자리로 숫자, 영문 '-', '_' 조합으로 구성할 수 있습니다.
         */
        public IActionResult CheckMgtKeyInUse()
        {
            try
            {
                // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
                int itemCode = 121;

                // 전자명세서 문서관리번호
                string mgtKey = "20181030";

                bool result = _statementService.CheckMgtKeyInUse(corpNum, itemCode, mgtKey);

                return View("result", result ? "사용중" : "미사용중");
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 1건의 전자명세서를 [즉시발행]합니다.
         */
        public IActionResult RegistIssue()
        {
            // 전자명세서 정보 객체
            Statement statement = new Statement();

            // [필수], 기재상 작성일자 날짜형식(yyyyMMdd)
            statement.writeDate = "20181212";

            // [필수], {영수, 청구} 중 기재 
            statement.purposeType = "영수";

            // [필수], 과세형태, {과세, 영세, 면세} 중 기재
            statement.taxType = "과세";

            // 맞춤양식코드, 기본값을 공백('')으로 처리하면 기본양식으로 처리.
            statement.formCode = "";

            // [필수] 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
            statement.itemCode = 121;

            // [필수] 문서관리번호, 1~24자리 숫자, 영문, '-', '_' 조합으로 사업자별로 중복되지 않도록 구성
            statement.mgtKey = "20181212115601";


            /**************************************************************************
             *                             발신자 정보                                   *
             **************************************************************************/

            // [필수] 발신자 사업자번호
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
            statement.senderTEL = "070-7070-0707";

            // 발신자 휴대전화
            statement.senderHP = "010-000-2222";

            // 발신자 이메일주소 
            statement.senderEmail = "test@test.com";

            // 발신자 팩스번호 
            statement.senderFAX = "02-111-2222";


            /**************************************************************************
             *                               수신자 정보                                 *
             **************************************************************************/

            // 수신자 사업자번호
            statement.receiverCorpNum = "8888888888";

            // [필수] 수신자 상호
            statement.receiverCorpName = "공급받는자 상호";

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

            // [필수] 수신자 성명
            statement.receiverContactName = "수신자 담당자명";

            // 수신자 부서명
            statement.receiverDeptName = "수신자 부서명";

            // 수신자 연락처
            statement.receiverTEL = "070-7070-0707";

            // 수신자 휴대전화
            statement.receiverHP = "010-000-2222";

            // 수신자 이메일주소 
            statement.receiverEmail = "test@test.com";

            // 수신자 팩스번호 
            statement.receiverFAX = "02-111-2222";


            /**************************************************************************
             *                           전자명세서 기재항목                               *
             **************************************************************************/

            // [필수] 공급가액 합계
            statement.supplyCostTotal = "200000";

            // [필수] 세액 합계
            statement.taxTotal = "20000";

            // 합계금액
            statement.totalAmount = "220000";

            // 기재상 일련번호 항목
            statement.serialNum = "123";

            // 기재상 비고 항목
            statement.remark1 = "비고1";
            statement.remark2 = "비고2";
            statement.remark3 = "비고3";

            // 사업자등록증 첨부 여부
            statement.businessLicenseYN = false;

            // 통장사본 첨부 여부 
            statement.bankBookYN = false;

            // 문자 자동전송 여부
            statement.smssendYN = false;

            // 발행시 자동승인 여부
            statement.autoAcceptYN = false;

            // 상세항목(품목) 정보 객체
            statement.detailList = new List<StatementDetail>();

            StatementDetail detail = new StatementDetail();

            detail.serialNum = 1; // 일련번호 1부터 순차기재
            detail.purchaseDT = "20181212"; // 거래일자 작성형식 yyyyMMdd
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
            detail.purchaseDT = "20181212"; // 거래일자 작성형식 yyyyMMdd
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

            // 추가속성항목, 자세한사항은 "전자명세서 API 연동매뉴얼> 5.2 기본양식 추가속성 테이블" 참조. 
            statement.propertyBag = new propertyBag();

            statement.propertyBag.Add("Balance", "15000"); // 전잔액
            statement.propertyBag.Add("Deposit", "5000"); // 입금액
            statement.propertyBag.Add("CBalance", "20000"); // 현잔액

            // 즉시발행 
            string memo = "즉시발행 메모";

            try
            {
                var response = _statementService.RegistIssue(corpNum, statement, memo, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 1건의 전자명세서를 [임시저장]합니다.
         */
        public IActionResult Register()
        {
            // 전자명세서 정보 객체
            Statement statement = new Statement();

            // [필수], 기재상 작성일자 날짜형식(yyyyMMdd)
            statement.writeDate = "20181212";

            // [필수], {영수, 청구} 중 기재 
            statement.purposeType = "영수";

            // [필수], 과세형태, {과세, 영세, 면세} 중 기재
            statement.taxType = "과세";

            // 맞춤양식코드, 기본값을 공백('')으로 처리하면 기본양식으로 처리.
            statement.formCode = "";

            // [필수] 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
            statement.itemCode = 121;

            // [필수] 문서관리번호, 1~24자리 숫자, 영문, '-', '_' 조합으로 사업자별로 중복되지 않도록 구성
            statement.mgtKey = "20181212115621";


            /**************************************************************************
             *                             발신자 정보                                   *
             **************************************************************************/

            // [필수] 발신자 사업자번호
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
            statement.senderTEL = "070-7070-0707";

            // 발신자 휴대전화
            statement.senderHP = "010-000-2222";

            // 발신자 이메일주소 
            statement.senderEmail = "test@test.com";

            // 발신자 팩스번호 
            statement.senderFAX = "02-111-2222";


            /**************************************************************************
             *                               수신자 정보                                 *
             **************************************************************************/

            // 수신자 사업자번호
            statement.receiverCorpNum = "8888888888";

            // [필수] 수신자 상호
            statement.receiverCorpName = "공급받는자 상호";

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

            // [필수] 수신자 성명
            statement.receiverContactName = "수신자 담당자명";

            // 수신자 부서명
            statement.receiverDeptName = "수신자 부서명";

            // 수신자 연락처
            statement.receiverTEL = "070-7070-0707";

            // 수신자 휴대전화
            statement.receiverHP = "010-000-2222";

            // 수신자 이메일주소 
            statement.receiverEmail = "test@test.com";

            // 수신자 팩스번호 
            statement.receiverFAX = "02-111-2222";


            /**************************************************************************
             *                           전자명세서 기재항목                               *
             **************************************************************************/

            // [필수] 공급가액 합계
            statement.supplyCostTotal = "200000";

            // [필수] 세액 합계
            statement.taxTotal = "20000";

            // 합계금액
            statement.totalAmount = "220000";

            // 기재상 일련번호 항목
            statement.serialNum = "123";

            // 기재상 비고 항목
            statement.remark1 = "비고1";
            statement.remark2 = "비고2";
            statement.remark3 = "비고3";

            // 사업자등록증 첨부 여부
            statement.businessLicenseYN = false;

            // 통장사본 첨부 여부 
            statement.bankBookYN = false;

            // 문자 자동전송 여부
            statement.smssendYN = false;

            // 발행시 자동승인 여부
            statement.autoAcceptYN = false;

            // 상세항목(품목) 정보 객체
            statement.detailList = new List<StatementDetail>();

            StatementDetail detail = new StatementDetail();

            detail.serialNum = 1; // 일련번호 1부터 순차기재
            detail.purchaseDT = "20181212"; // 거래일자 작성형식 yyyyMMdd
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
            detail.purchaseDT = "20181212"; // 거래일자 작성형식 yyyyMMdd
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

            // 추가속성항목, 자세한사항은 "전자명세서 API 연동매뉴얼> 5.2 기본양식 추가속성 테이블" 참조. 
            statement.propertyBag = new propertyBag();

            statement.propertyBag.Add("Balance", "15000"); // 전잔액
            statement.propertyBag.Add("Deposit", "5000"); // 입금액
            statement.propertyBag.Add("CBalance", "20000"); // 현잔액

            try
            {
                var response = _statementService.Register(corpNum, statement, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 1건의 전자명세서를 [수정]합니다.
         * - [임시저장] 상태의 전자명세서만 수정할 수 있습니다.
         */
        public IActionResult Update()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 수정할 명세서 문서관리번호
            string mgtKey = "20181212115621";


            // 전자명세서 정보 객체
            Statement statement = new Statement();

            // [필수], 기재상 작성일자 날짜형식(yyyyMMdd)
            statement.writeDate = "20181212";

            // [필수], {영수, 청구} 중 기재 
            statement.purposeType = "영수";

            // [필수], 과세형태, {과세, 영세, 면세} 중 기재
            statement.taxType = "과세";

            // 맞춤양식코드, 기본값을 공백('')으로 처리하면 기본양식으로 처리.
            statement.formCode = "";


            /**************************************************************************
             *                             발신자 정보                                   *
             **************************************************************************/

            // [필수] 발신자 사업자번호
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
            statement.senderTEL = "070-7070-0707";

            // 발신자 휴대전화
            statement.senderHP = "010-000-2222";

            // 발신자 이메일주소 
            statement.senderEmail = "test@test.com";

            // 발신자 팩스번호 
            statement.senderFAX = "02-111-2222";


            /**************************************************************************
             *                               수신자 정보                                 *
             **************************************************************************/

            // 수신자 사업자번호
            statement.receiverCorpNum = "8888888888";

            // [필수] 수신자 상호
            statement.receiverCorpName = "공급받는자 상호";

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

            // [필수] 수신자 성명
            statement.receiverContactName = "수신자 담당자명";

            // 수신자 부서명
            statement.receiverDeptName = "수신자 부서명";

            // 수신자 연락처
            statement.receiverTEL = "070-7070-0707";

            // 수신자 휴대전화
            statement.receiverHP = "010-000-2222";

            // 수신자 이메일주소 
            statement.receiverEmail = "test@test.com";

            // 수신자 팩스번호 
            statement.receiverFAX = "02-111-2222";


            /**************************************************************************
             *                           전자명세서 기재항목                               *
             **************************************************************************/

            // [필수] 공급가액 합계
            statement.supplyCostTotal = "200000";

            // [필수] 세액 합계
            statement.taxTotal = "20000";

            // 합계금액
            statement.totalAmount = "220000";

            // 기재상 일련번호 항목
            statement.serialNum = "123";

            // 기재상 비고 항목
            statement.remark1 = "비고1";
            statement.remark2 = "비고2";
            statement.remark3 = "비고3";

            // 사업자등록증 첨부 여부
            statement.businessLicenseYN = false;

            // 통장사본 첨부 여부 
            statement.bankBookYN = false;

            // 문자 자동전송 여부
            statement.smssendYN = false;

            // 발행시 자동승인 여부
            statement.autoAcceptYN = false;

            // 상세항목(품목) 정보 객체
            statement.detailList = new List<StatementDetail>();

            StatementDetail detail = new StatementDetail();

            detail.serialNum = 1; // 일련번호 1부터 순차기재
            detail.purchaseDT = "20181212"; // 거래일자 작성형식 yyyyMMdd
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
            detail.purchaseDT = "20181212"; // 거래일자 작성형식 yyyyMMdd
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

            // 추가속성항목, 자세한사항은 "전자명세서 API 연동매뉴얼> 5.2 기본양식 추가속성 테이블" 참조. 
            statement.propertyBag = new propertyBag();

            statement.propertyBag.Add("Balance", "15000"); // 전잔액
            statement.propertyBag.Add("Deposit", "5000"); // 입금액
            statement.propertyBag.Add("CBalance", "20000"); // 현잔액
            try
            {
                var response = _statementService.Update(corpNum, itemCode, mgtKey, statement, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 1건의 [임시저장] 상태의 전자명세서를 [발행]합니다.
         */
        public IActionResult Issue()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 발행처리할 명세서 문서관리번호
            string mgtKey = "20181206-123";

            // 발행 메모
            string memo = "발행 메모";

            try
            {
                var response = _statementService.Issue(corpNum, itemCode, mgtKey, memo, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 1건의 전자명세서를 [발행취소]합니다.
         */
        public IActionResult Cancel()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서) 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 발행취소할 명세서 문서관리번호
            string mgtKey = "20181206-123";

            // 발행 메모
            string memo = "발행 메모";

            try
            {
                var response = _statementService.Cancel(corpNum, itemCode, mgtKey, memo, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 1건의 전자명세서를 [삭제]합니다.
         * - 전자명세서를 삭제하면 사용된 문서관리번호(mgtKey)를 재사용할 수 있습니다.
         * - 삭제가능한 문서 상태 : [임시저장], [발행취소]
         */
        public IActionResult Delete()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서) 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 삭제처리할 명세서 문서관리번호
            string mgtKey = "20181206-123";

            // 발행 메모
            string memo = "발행 메모";

            try
            {
                var response = _statementService.Delete(corpNum, itemCode, mgtKey, userID);
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
         * 1건의 전자명세서 상태/요약 정보를 확인합니다.
         * - 응답항목에 대한 자세한 정보는 "[전자명세서 API 연동매뉴얼] > 3.2.1. GetInfo (상태 확인)"을 참조하시기 바랍니다.
         */
        public IActionResult GetInfo()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서) 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 명세서 문서관리번호, 사업자별로 중복되지 않도록 관리번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20181124";

            try
            {
                var response = _statementService.GetInfo(corpNum, itemCode, mgtKey, userID);
                return View("GetInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 다수건의 전자명세서 상태/요약 정보를 확인합니다.
         * - 응답항목에 대한 자세한 정보는 "[전자명세서 API 연동매뉴얼] > 3.2.2. GetInfos (상태 대량 확인)"을 참조하시기 바랍니다.
         */
        public IActionResult GetInfos()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서) 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 조회할 전자명세서 문서관리번호 배열, (최대 1000건)
            List<string> mgtKeyList = new List<string>();
            mgtKeyList.Add("20181112-a003");
            mgtKeyList.Add("20181124224344");
            mgtKeyList.Add("20181112-a004");

            try
            {
                var response = _statementService.GetInfos(corpNum, itemCode, mgtKeyList, userID);
                return View("GetInfos", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 전자명세서 1건의 상세정보를 조회합니다.
         * - 응답항목에 대한 자세한 사항은 "[전자명세서 API 연동매뉴얼] > 4.1. 전자명세서 구성" 을 참조하시기 바랍니다.
         */
        public IActionResult GetDetailInfo()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서) 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 명세서 문서관리번호, 사업자별로 중복되지 않도록 관리번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20181124";
            try
            {
                var response = _statementService.GetDetailInfo(corpNum, itemCode, mgtKey, userID);
                return View("GetDetailInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 검색조건을 사용하여 전자명세서 목록을 조회합니다.
         * - 응답항목에 대한 자세한 사항은 "[전자명세서 API 연동매뉴얼] > 3.2.4. Search (목록 조회)" 를 참조하시기 바랍니다.
         */
        public IActionResult Search()
        {
            // [필수] 검색일자 유형, R-등록일자, W-작성일자, I-발행일자
            string DType = "W";

            // [필수] 시작일자, 날짜형식(yyyyMMdd)
            string SDate = "20181101";

            // [필수] 종료일자, 날짜형식(yyyyMMdd)
            string EDate = "20181124";

            // 전송상태값 배열, 미기재시 전체 상태조회, 상태코드(stateCode)값 3자리의 배열, 2,3번째 자리에 와일드카드 가능
            //  - 상태코드에 대한 자세한 사항은 "[전자명세서 API 연동매뉴얼] > 5.1 전자명세서 상태코드" 를 참조하시기 바랍니다. 
            string[] State = new string[4];
            State[0] = "100";
            State[1] = "2**";
            State[2] = "3**";
            State[3] = "4**";

            //명세서 종류코드, 121-거래명세서, 122-청구서, 123-견적서, 124-발주서, 125-입금표, 126-영수증
            int[] itemCode = {121, 122, 123, 124, 125, 126};

            // 페이지 번호, 기본값 '1'
            int Page = 1;

            // 페이지당 검색개수, 기본값 '500', 최대 '1000'
            int PerPage = 50;

            // 정렬방향, D-내림차순, A-오름차순
            string Order = "D";

            // 거래처 조회, 거래처 등록번호, 상호 조회, 공백시 전체조회
            string QString = "";

            try
            {
                var response = _statementService.Search(corpNum, DType, SDate, EDate, State, itemCode, Page, PerPage,
                    Order, QString, userID);
                return View("Search", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 전자명세서 상태 변경이력을 확인합니다.
         * - 상태 변경이력 확인(GetLogs API) 응답항목에 대한 자세한 정보는
         *  "[전자명세서 API 연동매뉴얼] > 3.2.5 GetLogs (상태 변경이력 확인)" 을 참조하시기 바랍니다.
         */
        public IActionResult GetLogs()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서) 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 명세서 문서관리번호, 사업자별로 중복되지 않도록 관리번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20181124";
            try
            {
                var response = _statementService.GetLogs(corpNum, itemCode, mgtKey, userID);
                return View("GetLogs", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌 전자명세서 문서함 관련 팝업 URL을 반환합니다.
         * - 반환된 URL은 보안정책에 따라 30초의 유효시간을 갖습니다.
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
         * 1건의 전자명세서 보기 팝업 URL을 반환합니다.
         * - 반환된 URL은 보안정책에 따라 30초의 유효시간을 갖습니다.
         */
        public IActionResult GetPopUpURL()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서) 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 명세서 문서관리번호, 사업자별로 중복되지 않도록 관리번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20181124";

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
         * 1건의 전자명세서 인쇄팝업 URL을 반환합니다. (공급자/공급받는자용)
         * - 반환된 URL은 보안정책에 따라 30초의 유효시간을 갖습니다.
         */
        public IActionResult GetPrintURL()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서) 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 명세서 문서관리번호, 사업자별로 중복되지 않도록 관리번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20181124";

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
         * 1건의 전자명세서 인쇄팝업 URL을 반환합니다. (공급받는자용)
         * - 반환된 URL은 보안정책에 따라 30초의 유효시간을 갖습니다.
         */
        public IActionResult GetEPrintURL()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서) 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 명세서 문서관리번호, 사업자별로 중복되지 않도록 관리번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20181124";

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
         * 다수건의 전자명세서 인쇄팝업 URL을 반환합니다. (최대 100건)
         * - 반환된 URL은 보안정책에 따라 30초의 유효시간을 갖습니다.
         */
        public IActionResult GetMassPrintURL()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서) 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 조회할 전자명세서 문서관리번호 배열, (최대 100건)
            List<string> mgtKeyList = new List<string>();
            mgtKeyList.Add("20181112103859");
            mgtKeyList.Add("20181124224344");
            mgtKeyList.Add("20181105-004");

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
         * 공급받는자 메일링크 URL을 반환합니다.
         * - 메일링크 URL은 유효시간이 존재하지 않습니다.
         */
        public IActionResult GetMailURL()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서) 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 명세서 문서관리번호, 사업자별로 중복되지 않도록 관리번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20181124";

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
         * 팝빌에 로그인 상태로 접근할 수 있는 팝업 URL을 반환합니다.
         * - 반환된 URL은 보안정책에 따라 30초의 유효시간을 갖습니다.
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
         * 전자명세서에 첨부파일을 등록합니다.
         * - 첨부파일 등록은 전자명세서가 [임시저장] 상태인 경우에만 가능합니다.
         * - 첨부파일은 최대 5개까지 등록할 수 있습니다.
         */
        public IActionResult AttachFile()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서) 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 명세서 문서관리번호, 사업자별로 중복되지 않도록 관리번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20181124225313";

            // 파일경로
            string filePath = "C:/popbill.example.dotnetcore/StatementExample/wwwroot/images/tax_image.png";

            try
            {
                var response = _statementService.AttachFile(corpNum, itemCode, mgtKey, filePath, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 전자명세서에 첨부된 파일을 삭제합니다.
         * - 파일을 식별하는 파일아이디는 첨부파일 목록(GetFiles API) 의 응답항목
         *   중 파일아이디(AttachedFile) 값을 통해 확인할 수 있습니다.
         */
        public IActionResult DeleteFile()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 명세서 문서관리번호, 사업자별로 중복되지 않도록 관리번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20181124225313";

            // 파일아이디, 첨부파일 목록(GetFiles API) 의 응답항목 중 파일아이디(AttachedFile) 값
            string fileID = "4D3B7765-1623-4FD1-AA94-9AB624B92A66.PBF";

            try
            {
                var response = _statementService.DeleteFile(corpNum, itemCode, mgtKey, fileID, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 전자명세서에 첨부된 파일의 목록을 확인합니다.
         * - 응답항목 중 파일아이디(AttachedFile) 항목은 파일삭제(DeleteFile API)
         *   호출시 이용할 수 있습니다.
         */
        public IActionResult GetFiles()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 명세서 문서관리번호, 사업자별로 중복되지 않도록 관리번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20181124225313";

            try
            {
                var response = _statementService.GetFiles(corpNum, itemCode, mgtKey, userID);
                return View("GetFiles", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 발행 안내메일을 재전송합니다.
         */
        public IActionResult SendEmail()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 명세서 문서관리번호, 사업자별로 중복되지 않도록 관리번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20181124";

            // 수신자 이메일주소
            string receiver = "test@test.com";

            try
            {
                var response = _statementService.SendEmail(corpNum, itemCode, mgtKey, receiver, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 알림문자를 전송합니다. (단문/SMS- 한글 최대 45자)
         * - 알림문자 전송시 포인트가 차감됩니다. (전송실패시 환불처리)
         * - 전송내역 확인은 "팝빌 로그인" > [문자 팩스] > [전송내역] 탭에서 전송결과를 확인할 수 있습니다.
         */
        public IActionResult SendSMS()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 명세서 문서관리번호, 사업자별로 중복되지 않도록 관리번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20181124";

            // 발신번호
            string sender = "070-4304-2992";

            // 수신번호
            string receiver = "010-111-222";

            // 문자메시지 내용, 90byte 초과시 길이가 조정되어 전송됨
            string contents = "알림문자 전송내용, 90byte 초과된 내용은 삭제되어 전송됨";

            try
            {
                var response = _statementService.SendSMS(corpNum, itemCode, mgtKey, sender, receiver, contents, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 전자명세서를 팩스전송합니다.
         * - 팩스 전송 요청시 포인트가 차감됩니다. (전송실패시 환불처리)
         * - 전송내역 확인은 "팝빌 로그인" > [문자 팩스] > [팩스] > [전송내역] 메뉴에서 전송결과를 확인할 수 있습니다.
         */
        public IActionResult SendFAX()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 명세서 문서관리번호, 사업자별로 중복되지 않도록 관리번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20181124";

            // 발신번호
            string sender = "070-4304-2992";

            // 수신번호
            string receiver = "070-111-222";

            try
            {
                var response = _statementService.SendFAX(corpNum, itemCode, mgtKey, sender, receiver, userID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌에 전자명세서를 등록하지 않고 공급받는자에게 팩스전송합니다.
         * - 팩스 전송 요청시 포인트가 차감됩니다. (전송실패시 환불처리)
         * - 팩스 발행 요청시 작성한 문서관리번호는 팩스전송 파일명으로 사용됩니다.
         * - 전송내역 확인은 "팝빌 로그인" > [문자 팩스] > [팩스] > [전송내역] 메뉴에서 전송결과를 확인할 수 있습니다.
         * - 팩스 전송결과를 확인하기 위해서는 선팩스 전송 요청 시 반환받은 접수번호를 이용하여
         *   팩스 API의 전송결과 확인 (GetFaxDetail) API를 이용하면 됩니다.
         */
        public IActionResult FAXSend()
        {
            // 전자명세서 정보 객체
            Statement statement = new Statement();

            // [필수], 기재상 작성일자 날짜형식(yyyyMMdd)
            statement.writeDate = "20181124";

            // [필수], {영수, 청구} 중 기재 
            statement.purposeType = "영수";

            // [필수], 과세형태, {과세, 영세, 면세} 중 기재
            statement.taxType = "과세";

            // 맞춤양식코드, 기본값을 공백('')으로 처리하면 기본양식으로 처리.
            statement.formCode = "";

            // [필수] 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서) 124(발주서), 125(입금표), 126(영수증)
            statement.itemCode = 121;

            // [필수] 문서관리번호, 1~24자리 숫자, 영문, '-', '_' 조합으로 사업자별로 중복되지 않도록 구성
            statement.mgtKey = "20181127112719";


            /**************************************************************************
             *                             발신자 정보                                   *
             **************************************************************************/

            // [필수] 발신자 사업자번호
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
            statement.senderTEL = "070-7070-0707";

            // 발신자 휴대전화
            statement.senderHP = "010-000-2222";

            // 발신자 이메일주소 
            statement.senderEmail = "test@test.com";

            // 발신자 팩스번호 
            statement.senderFAX = "02-111-2222";


            /**************************************************************************
             *                               수신자 정보                                 *
             **************************************************************************/

            // 수신자 사업자번호
            statement.receiverCorpNum = "8888888888";

            // [필수] 수신자 상호
            statement.receiverCorpName = "공급받는자 상호";

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

            // [필수] 수신자 성명
            statement.receiverContactName = "수신자 담당자명";

            // 수신자 부서명
            statement.receiverDeptName = "수신자 부서명";

            // 수신자 연락처
            statement.receiverTEL = "070-7070-0707";

            // 수신자 휴대전화
            statement.receiverHP = "010-000-2222";

            // 수신자 이메일주소 
            statement.receiverEmail = "test@test.com";

            // 수신자 팩스번호 
            statement.receiverFAX = "02-111-2222";


            /**************************************************************************
             *                           전자명세서 기재항목                               *
             **************************************************************************/

            // [필수] 공급가액 합계
            statement.supplyCostTotal = "200000";

            // [필수] 세액 합계
            statement.taxTotal = "20000";

            // 합계금액
            statement.totalAmount = "220000";

            // 기재상 일련번호 항목
            statement.serialNum = "123";

            // 기재상 비고 항목
            statement.remark1 = "비고1";
            statement.remark2 = "비고2";
            statement.remark3 = "비고3";

            // 사업자등록증 첨부 여부
            statement.businessLicenseYN = false;

            // 통장사본 첨부 여부 
            statement.bankBookYN = false;

            // 문자 자동전송 여부
            statement.smssendYN = false;

            // 발행시 자동승인 여부
            statement.autoAcceptYN = false;

            // 상세항목(품목) 정보 객체
            statement.detailList = new List<StatementDetail>();

            StatementDetail detail = new StatementDetail
            {
                serialNum = 1, // 일련번호 1부터 순차기재
                purchaseDT = "20181124", // 거래일자 작성형식 yyyyMMdd
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
                purchaseDT = "20181124", // 거래일자 작성형식 yyyyMMdd
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

            // 추가속성항목, 자세한사항은 "전자명세서 API 연동매뉴얼> 5.2 기본양식 추가속성 테이블" 참조. 
            statement.propertyBag = new propertyBag();

            statement.propertyBag.Add("Balance", "15000"); // 전잔액
            statement.propertyBag.Add("Deposit", "5000"); // 입금액
            statement.propertyBag.Add("CBalance", "20000"); // 현잔액

            // 발신번호
            string sendNum = "070-4304-2992";

            // 수신번호
            string receiveNum = "070-111-222";

            try
            {
                var result = _statementService.FAXSend(corpNum, statement, sendNum, receiveNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 전자명세서에 다른 전자명세서 1건을 첨부합니다.
         */
        public IActionResult AttachStatement()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 명세서 문서관리번호, 사업자별로 중복되지 않도록 관리번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20181124225313";

            // 첨부할 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
            int subItemCode = 121;

            // 첨부할 명세서 문서관리번호
            string subMgtKey = "20181124223932-001";

            try
            {
                var response =
                    _statementService.AttachStatement(corpNum, itemCode, mgtKey, subItemCode, subMgtKey, userID);
                return View("response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 전자명세서에 첨부된 다른 전자명세서를 첨부해제합니다.
         */
        public IActionResult DetachStatement()
        {
            // 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
            int itemCode = 121;

            // 명세서 문서관리번호, 사업자별로 중복되지 않도록 관리번호 할당
            // 1~24자리 영문,숫자,'-','_' 조합 구성
            string mgtKey = "20181124225313";

            // 첨부해제할 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
            int subItemCode = 121;

            // 첨부해제할 명세서 문서관리번호
            string subMgtKey = "20181124223932-001";

            try
            {
                var response =
                    _statementService.DetachStatement(corpNum, itemCode, mgtKey, subItemCode, subMgtKey, userID);
                return View("response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 전자명세서 관련 메일전송 항목에 대한 전송여부를 목록으로 반환한다.
         */
        public IActionResult ListEmailConfig()
        {
            try
            {
                var result = _statementService.ListEmailConfig(corpNum, userID);
                return View("ListEmailConfig", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 전자명세서 관련 메일전송 항목에 대한 전송여부를 수정한다.
         *
         * 메일전송유형
         * SMT_ISSUE : 공급받는자에게 전자명세서가 발행 되었음을 알려주는 메일입니다.
         * SMT_ACCEPT : 공급자에게 전자명세서가 승인 되었음을 알려주는 메일입니다.
         * SMT_DENY : 공급자에게 전자명세서가 거부 되었음을 알려주는 메일입니다.
         * SMT_CANCEL : 공급받는자에게 전자명세서가 취소 되었음을 알려주는 메일입니다.
         * SMT_CANCEL_ISSUE : 공급받는자에게 전자명세서가 발행취소 되었음을 알려주는 메일입니다.
         */
        public IActionResult UpdateEmailConfig()
        {
            //메일전송유형
            string emailType = "SMT_ISSUE";

            //전송여부 (true-전송, false-미전송)
            bool sendYN = true;

            try
            {
                var response = _statementService.UpdateEmailConfig(corpNum, emailType, sendYN, userID);
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
         * 연동회원 잔여포인트를 확인합니다.
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
         * 팝빌 연동회원의 포인트충전 팝업 URL을 반환합니다.
         * - 반환된 URL은 보안정책에 따라 30초의 유효시간을 갖습니다.
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
         * 파트너의 잔여포인트를 확인합니다.
         * - 과금방식이 연동과금인 경우 연동회원 잔여포인트(GetBalance API)를 이용하시기 바랍니다.
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
         * 파트너 포인트 충전 팝업 URL을 반환합니다.
         * - 반환된 URL은 보안정책에 따라 30초의 유효시간을 갖습니다.
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
         * 전자명세서 발행단가를 확인합니다.
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
         * 전자명세서 API 서비스 과금정보를 확인합니다.
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

                var response = _statementService.CheckIsMember(corpNum, linkID);
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
                var response = _statementService.CheckID(checkID);
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
            
            // 아이디, 6자이상 50자 미만
            joinInfo.ID = "userid_20181212"; 
            
            // 비밀번호, 6자이상 20자 미만
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
         */
        public IActionResult GetCorpInfo()
        {
            try
            {
                var response = _statementService.GetCorpInfo(corpNum, userID);
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
                var response = _statementService.UpdateCorpInfo(corpNum, corpInfo, userID);
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
                var response = _statementService.RegistContact(corpNum, contactInfo, userID);
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
                var response = _statementService.ListContact(corpNum, userID);
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
                var response = _statementService.UpdateContact(corpNum, contactInfo, userID);
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