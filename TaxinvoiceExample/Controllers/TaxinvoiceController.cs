using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Popbill;
using Popbill.Taxinvoice;

namespace TaxinvoiceExample.Controllers
{
    public class TaxinvoiceController : Controller
    {
        private readonly TaxinvoiceService _taxinvoiceService;

        public TaxinvoiceController(TaxinvoiceInstance TIinstance)
        {
            // 세금계산서 서비스 객체 생성
            _taxinvoiceService = TIinstance.taxinvoiceService;
        }

        // 팝빌 연동회원 사업자번호 (하이픈 '-' 없이 10자리)
        string corpNum = "1234567890";

        // 팝빌 연동회원 아이디
        string userID = "testkorea";

        /*
         * 전자세금계산서 Index page (Taxinvoice/Index.cshtml)
         */
        public IActionResult Index()
        {
            return View();
        }

        #region 세금계산서 발행/전송

        /*
         * 파트너가 세금계산서 관리 목적으로 할당하는 문서번호의 사용여부를 확인합니다.
         * - 이미 사용 중인 문서번호는 중복 사용이 불가하고, 세금계산서가 삭제된 경우에만 문서번호의 재사용이 가능합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/info#CheckMgtKeyInUse
         */
        public IActionResult CheckMgtKeyInUse()
        {
            try
            {
                // 세금계산서 문서번호
                string mgtKey = "20220527";

                // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
                MgtKeyType mgtKeyType = MgtKeyType.SELL;

                bool result = _taxinvoiceService.CheckMgtKeyInUse(corpNum, mgtKeyType, mgtKey);

                return View("result", result ? "사용중" : "미사용중");
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 작성된 세금계산서 데이터를 팝빌에 저장과 동시에 발행(전자서명)하여 "발행완료" 상태로 처리합니다.
         * - 세금계산서 국세청 전송 정책 [https://developers.popbill.com/guide/taxinvoice/dotnetcore/introduction/policy-of-send-to-nts]
         * - "발행완료"된 전자세금계산서는 국세청 전송 이전에 발행취소(CancelIssue API) 함수로 국세청 신고 대상에서 제외할 수 있습니다.
         * - 임시저장(Register API) 함수와 발행(Issue API) 함수를 한 번의 프로세스로 처리합니다.
         * - 세금계산서 발행을 위해서 공급자의 인증서가 팝빌 인증서버에 사전등록 되어야 합니다.
         *   └ 위수탁발행의 경우, 수탁자의 인증서 등록이 필요합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/issue#RegistIssue
         */
        public IActionResult RegistIssue()
        {
            // 세금계산서 정보 객체
            Taxinvoice taxinvoice = new Taxinvoice();

            // 기재상 작성일자, 날짜형식(yyyyMMdd)
            taxinvoice.writeDate = "20220527";

            // 과금방향, {정과금, 역과금}중 선택
            // - 정과금(공급자과금), 역과금(공급받는자과금)
            // - 역과금은 역발행 세금계산서를 발행하는 경우만 가능
            taxinvoice.chargeDirection = "정과금";

            // 발행형태, {정발행, 역발행, 위수탁} 중 기재
            taxinvoice.issueType = "정발행";

            // {영수, 청구, 없음} 중 기재
            taxinvoice.purposeType = "영수";

            // 과세형태, {과세, 영세, 면세} 중 기재
            taxinvoice.taxType = "과세";


            /*****************************************************************
             *                         공급자 정보                           *
             *****************************************************************/

            // 공급자 사업자번호, '-' 제외 10자리
            taxinvoice.invoicerCorpNum = corpNum;

            // 공급자 종사업자 식별번호. 필요시 기재. 형식은 숫자 4자리.
            taxinvoice.invoicerTaxRegID = "";

            // 공급자 상호
            taxinvoice.invoicerCorpName = "공급자 상호";

            // 공급자 문서번호, 숫자, 영문, '-', '_' 조합으로
            //  1~24자리까지 사업자번호별 중복없는 고유번호 할당
            taxinvoice.invoicerMgtKey = "20220527-003";

            // 공급자 대표자 성명
            taxinvoice.invoicerCEOName = "공급자 대표자 성명";

            // 공급자 주소
            taxinvoice.invoicerAddr = "공급자 주소";

            // 공급자 종목
            taxinvoice.invoicerBizClass = "공급자 종목";

            // 공급자 업태
            taxinvoice.invoicerBizType = "공급자 업태";

            // 공급자 담당자 성명
            taxinvoice.invoicerContactName = "공급자 담당자명";

            // 공급자 담당자 연락처
            taxinvoice.invoicerTEL = "";

            // 공급자 담당자 휴대폰번호
            taxinvoice.invoicerHP = "";

            // 공급자 담당자 메일주소
            taxinvoice.invoicerEmail = "";

            // 발행 안내 문자 전송여부 (true / false 중 택 1)
            // └ true = 전송 , false = 미전송
            // └ 공급받는자 (주)담당자 휴대폰번호 {invoiceeHP1} 값으로 문자 전송
            // - 전송 시 포인트 차감되며, 전송실패시 환불처리
            taxinvoice.invoicerSMSSendYN = false;


            /*********************************************************************
             *                         공급받는자 정보                           *
             *********************************************************************/

            // 공급받는자 구분, {사업자, 개인, 외국인} 중 기재
            taxinvoice.invoiceeType = "사업자";

            // 공급받는자 사업자번호
            // - {invoiceeType}이 "사업자" 인 경우, 사업자번호 (하이픈 ('-') 제외 10자리)
            // - {invoiceeType}이 "개인" 인 경우, 주민등록번호 (하이픈 ('-') 제외 13자리)
            // - {invoiceeType}이 "외국인" 인 경우, "9999999999999" (하이픈 ('-') 제외 13자리)
            taxinvoice.invoiceeCorpNum = "8888888888";

            // 공급받는자 상호
            taxinvoice.invoiceeCorpName = "공급받는자 상호";

            // [역발행시 필수] 공급받는자 문서번호, 숫자, 영문, '-', '_' 조합으로
            // 1~24자리까지 사업자번호별 중복없는 고유번호 할당
            taxinvoice.invoiceeMgtKey = "";

            // 공급받는자 대표자 성명
            taxinvoice.invoiceeCEOName = "공급받는자 대표자 성명";

            // 공급받는자 주소
            taxinvoice.invoiceeAddr = "공급받는자 주소";

            // 공급받는자 종목
            taxinvoice.invoiceeBizClass = "공급받는자 종목";

            // 공급받는자 업태
            taxinvoice.invoiceeBizType = "공급받는자 업태";

            // 공급받는자 주)담당자 성명
            taxinvoice.invoiceeContactName1 = "공급받는자 담당자명";

            // 공급받는자 주)담당자 연락처
            taxinvoice.invoiceeTEL1 = "";

            // 공급받는자 주)담당자 휴대폰번호
            taxinvoice.invoiceeHP1 = "";

            // 공급받는자 주)담당자 메일주소
            // 팝빌 개발환경에서 테스트하는 경우에도 안내 메일이 전송되므로,
            // 실제 거래처의 메일주소가 기재되지 않도록 주의
            taxinvoice.invoiceeEmail1 = "";

            // 역발행 요청시 알림문자 전송여부 (역발행에서만 사용가능)
            // - 공급자 담당자 휴대폰번호(invoicerHP)로 전송
            // - 전송시 포인트가 차감되며 전송실패하는 경우 포인트 환불처리
            taxinvoice.invoiceeSMSSendYN = false;


            /*********************************************************************
             *                          세금계산서 정보                          *
             *********************************************************************/

            // 공급가액 합계
            taxinvoice.supplyCostTotal = "100000";

            // 세액 합계
            taxinvoice.taxTotal = "10000";

            // 합계금액,  공급가액 합계 + 세액 합계
            taxinvoice.totalAmount = "110000";

            // 기재상 일련번호 항목
            taxinvoice.serialNum = "";

            // 기재상 현금 항목
            taxinvoice.cash = "";

            // 기재상 수표 항목
            taxinvoice.chkBill = "";

            // 기재상 어음 항목
            taxinvoice.note = "";

            // 기재상 외상미수금 항목
            taxinvoice.credit = "";

            // 비고
            // {invoiceeType}이 "외국인" 이면 remark1 필수
            // - 외국인 등록번호 또는 여권번호 입력
            taxinvoice.remark1 = "비고1";
            taxinvoice.remark2 = "비고2";
            taxinvoice.remark3 = "비고3";

            // 기재상 권 항목, 최대값 32767
            // 미기재시 taxinvoice.kwon = null;
            taxinvoice.kwon = 1;

            // 기재상 호 항목, 최대값 32767
            // 미기재시 taxinvoice.ho = null;
            taxinvoice.ho = 1;

            // 사업자등록증 이미지 첨부여부 (true / false 중 택 1)
            // └ true = 첨부 , false = 미첨부(기본값)
            // - 팝빌 사이트 또는 인감 및 첨부문서 등록 팝업 URL (GetSealURL API) 함수를 이용하여 등록
            taxinvoice.businessLicenseYN = false;

            // 통장사본 이미지 첨부여부 (true / false 중 택 1)
            // └ true = 첨부 , false = 미첨부(기본값)
            // - 팝빌 사이트 또는 인감 및 첨부문서 등록 팝업 URL (GetSealURL API) 함수를 이용하여 등록
            taxinvoice.bankBookYN = false;


            /**************************************************************************
             *        수정세금계산서 정보 (수정세금계산서 작성시에만 기재             *
             * - 수정세금계산서 관련 정보는 연동매뉴얼 또는 개발가이드 링크 참조      *
             * - [참고] 수정세금계산서 작성방법 안내 - https://developers.popbill.com/guide/taxinvoice/dotnetcore/introduction/modified-taxinvoice  *
             *************************************************************************/

            // 수정사유코드, 1~6까지 선택기재.
            taxinvoice.modifyCode = null;

            // 수정세금계산서 작성시 원본세금계산서의 국세청승인번호
            taxinvoice.orgNTSConfirmNum = "";


            /********************************************************************************
             *                         상세항목(품목) 정보                                  *
             * - 상세항목 정보는 세금계산서 필수기재사항이 아니므로 작성하지 않더라도       *
             *   세금계산서 발행이 가능합니다.                                              *
             * - 최대 99건까지 작성가능                                                     *
             ********************************************************************************/

            taxinvoice.detailList = new List<TaxinvoiceDetail>();

            TaxinvoiceDetail detail = new TaxinvoiceDetail();

            detail.serialNum = 1; // 일련번호; 1부터 순차기재
            detail.purchaseDT = "20220527"; // 거래일자
            detail.itemName = "품목명"; // 품목명
            detail.spec = "규격"; // 규격
            detail.qty = "1"; // 수량
            detail.unitCost = "50000"; // 단가
            detail.supplyCost = "50000"; // 공급가액
            detail.tax = "5000"; // 세액
            detail.remark = "품목비고"; //비고

            taxinvoice.detailList.Add(detail);

            detail = new TaxinvoiceDetail();

            detail.serialNum = 2; // 일련번호; 1부터 순차기재
            detail.purchaseDT = "20220527"; // 거래일자
            detail.itemName = "품목명"; // 품목명
            detail.spec = "규격"; // 규격
            detail.qty = "1"; // 수량
            detail.unitCost = "50000"; // 단가
            detail.supplyCost = "50000"; // 공급가액
            detail.tax = "5000"; // 세액
            detail.remark = "품목비고"; //비고

            taxinvoice.detailList.Add(detail);


            /*********************************************************************************
            *                           추가담당자 정보                                      *
            * - 세금계산서 발행안내 메일을 수신받을 공급받는자 담당자가 다수인 경우 담당자   *
            *   정보를 추가하여 발행안내메일을 다수에게 전송할 수 있습니다.                  *
            * - 최대 5개까지 기재가능                                                        *
            **********************************************************************************/

            taxinvoice.addContactList = new List<TaxinvoiceAddContact>();

            TaxinvoiceAddContact addContact = new TaxinvoiceAddContact();

            addContact.serialNum = 1; // 일련번호, 1부터 순차기재
            addContact.email = ""; // 추가담당자 메일주소
            addContact.contactName = "추가담당자명"; // 추가담당자 성명

            taxinvoice.addContactList.Add(addContact);

            addContact = new TaxinvoiceAddContact();

            addContact.serialNum = 2; // 일련번호, 1부터 순차기재
            addContact.email = ""; // 추가담당자 메일주소
            addContact.contactName = "추가담당자명"; // 추가담당자 성명

            taxinvoice.addContactList.Add(addContact);


            // 거래명세서 동시작성여부 (true / false 중 택 1)
            // └ true = 사용 , false = 미사용
            // - 미입력 시 기본값 false 처리
            bool writeSpecification = false;

            // 지연발행 강제여부  (true / false 중 택 1)
            // └ true = 가능 , false = 불가능
            // - 미입력 시 기본값 false 처리
            // - 발행마감일이 지난 세금계산서를 발행하는 경우, 가산세가 부과될 수 있습니다.
            // - 가산세가 부과되더라도 발행을 해야하는 경우에는 forceIssue의 값을
            //   true로 선언하여 발행(Issue API)를 호출하시면 됩니다.
            bool forceIssue = false;

            // {writeSpecification} = true인 경우, 거래명세서 문서번호 할당
            // - 미입력시 기본값 세금계산서 문서번호와 동일하게 할당
            string dealInvoiceMgtKey = "";

            // 메모
            string memo = "";

            // 발행안내메일 제목, 미기재시 기본양식으로 전송됨
            string emailSubject = "";

            try
            {
                var response = _taxinvoiceService.RegistIssue(corpNum, taxinvoice, writeSpecification, forceIssue,
                    dealInvoiceMgtKey, memo, emailSubject);
                return View("IssueResponse", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 최대 100건의 세금계산서 발행을 한번의 요청으로 접수합니다.
         * * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/issue#BulkSubmit
         */
        public IActionResult BulkSubmit()
        {

            // 제출아이디
            // 최대 36자리 영문, 숫자, '-'조합
            string submitID = "20220527-BULK";

            //세금계산서 객체정보 목록
            List<Taxinvoice> taxinvoiceList = new List<Taxinvoice>();

            for (int i = 0; i < 10; i++)
            {
                // 세금계산서 정보 객체
                Taxinvoice taxinvoice = new Taxinvoice();

                // 기재상 작성일자, 날짜형식(yyyyMMdd)
                taxinvoice.writeDate = "20220527";

                // 과금방향, {정과금, 역과금}중 선택
                // - 정과금(공급자과금), 역과금(공급받는자과금)
                // - 역과금은 역발행 세금계산서를 발행하는 경우만 가능
                taxinvoice.chargeDirection = "정과금";

                // 발행형태, {정발행, 역발행, 위수탁} 중 기재
                taxinvoice.issueType = "정발행";

                // {영수, 청구, 없음} 중 기재
                taxinvoice.purposeType = "영수";

                // 과세형태, {과세, 영세, 면세} 중 기재
                taxinvoice.taxType = "과세";


                /*****************************************************************
                 *                         공급자 정보                           *
                 *****************************************************************/

                // 공급자 사업자번호, '-' 제외 10자리
                taxinvoice.invoicerCorpNum = corpNum;

                // 공급자 종사업자 식별번호. 필요시 기재. 형식은 숫자 4자리.
                taxinvoice.invoicerTaxRegID = "";

                // 공급자 상호
                taxinvoice.invoicerCorpName = "공급자 상호";

                // 공급자 문서번호, 숫자, 영문, '-', '_' 조합으로
                //  1~24자리까지 사업자번호별 중복없는 고유번호 할당
                taxinvoice.invoicerMgtKey = submitID + "-" + i;

                // 공급자 대표자 성명
                taxinvoice.invoicerCEOName = "공급자 대표자 성명";

                // 공급자 주소
                taxinvoice.invoicerAddr = "공급자 주소";

                // 공급자 종목
                taxinvoice.invoicerBizClass = "공급자 종목";

                // 공급자 업태
                taxinvoice.invoicerBizType = "공급자 업태";

                // 공급자 담당자 성명
                taxinvoice.invoicerContactName = "공급자 담당자명";

                // 공급자 담당자 연락처
                taxinvoice.invoicerTEL = "";

                // 공급자 담당자 휴대폰번호
                taxinvoice.invoicerHP = "";

                // 공급자 담당자 메일주소
                taxinvoice.invoicerEmail = "";

                // 발행 안내 문자 전송여부 (true / false 중 택 1)
                // └ true = 전송 , false = 미전송
                // └ 공급받는자 (주)담당자 휴대폰번호 {invoiceeHP1} 값으로 문자 전송
                // - 전송 시 포인트 차감되며, 전송실패시 환불처리
                taxinvoice.invoicerSMSSendYN = false;


                /*********************************************************************
                 *                         공급받는자 정보                           *
                 *********************************************************************/

                // 공급받는자 구분, {사업자, 개인, 외국인} 중 기재
                taxinvoice.invoiceeType = "사업자";

                // 공급받는자 사업자번호
                // - {invoiceeType}이 "사업자" 인 경우, 사업자번호 (하이픈 ('-') 제외 10자리)
                // - {invoiceeType}이 "개인" 인 경우, 주민등록번호 (하이픈 ('-') 제외 13자리)
                // - {invoiceeType}이 "외국인" 인 경우, "9999999999999" (하이픈 ('-') 제외 13자리)
                taxinvoice.invoiceeCorpNum = "8888888888";

                // 공급받는자 상호
                taxinvoice.invoiceeCorpName = "공받자 Core";

                // [역발행시 필수] 공급받는자 문서번호, 숫자, 영문, '-', '_' 조합으로
                // 1~24자리까지 사업자번호별 중복없는 고유번호 할당
                taxinvoice.invoiceeMgtKey = "";

                // 공급받는자 대표자 성명
                taxinvoice.invoiceeCEOName = "공급받는자 대표자 성명";

                // 공급받는자 주소
                taxinvoice.invoiceeAddr = "공급받는자 주소";

                // 공급받는자 종목
                taxinvoice.invoiceeBizClass = "공급받는자 종목";

                // 공급받는자 업태
                taxinvoice.invoiceeBizType = "공급받는자 업태";

                // 공급받는자 주)담당자 성명
                taxinvoice.invoiceeContactName1 = "공급받는자 담당자명";

                // 공급받는자 주)담당자 연락처
                taxinvoice.invoiceeTEL1 = "";

                // 공급받는자 주)담당자 휴대폰번호
                taxinvoice.invoiceeHP1 = "";

                // 공급받는자 주)담당자 메일주소
                // 팝빌 개발환경에서 테스트하는 경우에도 안내 메일이 전송되므로,
                // 실제 거래처의 메일주소가 기재되지 않도록 주의
                taxinvoice.invoiceeEmail1 = "";

                // 역발행 요청시 알림문자 전송여부 (역발행에서만 사용가능)
                // - 공급자 담당자 휴대폰번호(invoicerHP)로 전송
                // - 전송시 포인트가 차감되며 전송실패하는 경우 포인트 환불처리
                taxinvoice.invoiceeSMSSendYN = false;


                /*********************************************************************
                 *                          세금계산서 정보                          *
                 *********************************************************************/

                // 공급가액 합계
                taxinvoice.supplyCostTotal = "100000";

                // 세액 합계
                taxinvoice.taxTotal = "10000";

                // 합계금액,  공급가액 합계 + 세액 합계
                taxinvoice.totalAmount = "110000";

                // 기재상 일련번호 항목
                taxinvoice.serialNum = "";

                // 기재상 현금 항목
                taxinvoice.cash = "";

                // 기재상 수표 항목
                taxinvoice.chkBill = "";

                // 기재상 어음 항목
                taxinvoice.note = "";

                // 기재상 외상미수금 항목
                taxinvoice.credit = "";

                // 비고
                // {invoiceeType}이 "외국인" 이면 remark1 필수
                // - 외국인 등록번호 또는 여권번호 입력
                taxinvoice.remark1 = "비고1";
                taxinvoice.remark2 = "비고2";
                taxinvoice.remark3 = "비고3";

                // 기재상 권 항목, 최대값 32767
                // 미기재시 taxinvoice.kwon = null;
                taxinvoice.kwon = 1;

                // 기재상 호 항목, 최대값 32767
                // 미기재시 taxinvoice.ho = null;
                taxinvoice.ho = 1;

                // 사업자등록증 이미지 첨부여부 (true / false 중 택 1)
                // └ true = 첨부 , false = 미첨부(기본값)
                // - 팝빌 사이트 또는 인감 및 첨부문서 등록 팝업 URL (GetSealURL API) 함수를 이용하여 등록
                taxinvoice.businessLicenseYN = false;

                // 통장사본 이미지 첨부여부 (true / false 중 택 1)
                // └ true = 첨부 , false = 미첨부(기본값)
                // - 팝빌 사이트 또는 인감 및 첨부문서 등록 팝업 URL (GetSealURL API) 함수를 이용하여 등록
                taxinvoice.bankBookYN = false;


                /********************************************************************************
                 *                         상세항목(품목) 정보                                  *
                 * - 상세항목 정보는 세금계산서 필수기재사항이 아니므로 작성하지 않더라도       *
                 *   세금계산서 발행이 가능합니다.                                              *
                 * - 최대 99건까지 작성가능                                                     *
                 ********************************************************************************/

                taxinvoice.detailList = new List<TaxinvoiceDetail>();

                TaxinvoiceDetail detail = new TaxinvoiceDetail();

                detail.serialNum = 1; // 일련번호; 1부터 순차기재
                detail.purchaseDT = "20220527"; // 거래일자
                detail.itemName = "품목명"; // 품목명
                detail.spec = "규격"; // 규격
                detail.qty = "1"; // 수량
                detail.unitCost = "50000"; // 단가
                detail.supplyCost = "50000"; // 공급가액
                detail.tax = "5000"; // 세액
                detail.remark = "품목비고"; //비고

                taxinvoice.detailList.Add(detail);

                detail = new TaxinvoiceDetail();

                detail.serialNum = 2; // 일련번호; 1부터 순차기재
                detail.purchaseDT = "20220527"; // 거래일자
                detail.itemName = "품목명"; // 품목명
                detail.spec = "규격"; // 규격
                detail.qty = "1"; // 수량
                detail.unitCost = "50000"; // 단가
                detail.supplyCost = "50000"; // 공급가액
                detail.tax = "5000"; // 세액
                detail.remark = "품목비고"; //비고

                taxinvoice.detailList.Add(detail);

                taxinvoiceList.Add(taxinvoice);
            }

            try
            {
                var bulkResponse = _taxinvoiceService.BulkSubmit(corpNum, submitID, taxinvoiceList);
                return View("BulkResponse", bulkResponse);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 초대량 발행 접수결과를 확인합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/issue#GetBulkResult
         */
        public IActionResult GetBulkResult()
        {
            // 초대량 발행 접수시 기재한 제출아이디
            // 최대 36자리 영문, 숫자, '-'조합
            string submitID = "20220527-BULK";

            try
            {
                var bulkTaxinvoiceResult = _taxinvoiceService.GetBulkResult(corpNum, submitID);
                return View("BulkTaxinvoiceResult", bulkTaxinvoiceResult);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 작성된 세금계산서 데이터를 팝빌에 저장합니다.
         * - "임시저장" 상태의 세금계산서는 발행(Issue) 함수를 호출하여 "발행완료" 처리한 경우에만 국세청으로 전송됩니다.
         * - 정발행 시 임시저장(Register)과 발행(Issue)을 한번의 호출로 처리하는 즉시발행(RegistIssue API) 프로세스 연동을 권장합니다.
         * - 역발행 시 임시저장(Register)과 역발행요청(Request)을 한번의 호출로 처리하는 즉시요청(RegistRequest API) 프로세스 연동을 권장합니다.
         * - 세금계산서 파일첨부 기능을 구현하는 경우, 임시저장(Register API) -> 파일첨부(AttachFile API) -> 발행(Issue API) 함수를 차례로 호출합니다.
         * - 역발행 세금계산서를 저장하는 경우, 객체 'Taxinvoice'의 변수 'chargeDirection' 값을 통해 과금 주체를 지정할 수 있습니다.
         *   └ 정과금 : 공급자 과금 , 역과금 : 공급받는자 과금
         * - 임시저장된 세금계산서는 팝빌 사이트 '임시문서함'에서 확인 가능합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/issue#Register
         */
        public IActionResult Register()
        {
            // 세금계산서 정보 객체
            Taxinvoice taxinvoice = new Taxinvoice();

            // 기재상 작성일자, 날짜형식(yyyyMMdd)
            taxinvoice.writeDate = "20220527";

            // 과금방향, {정과금, 역과금}중 선택
            // - 정과금(공급자과금), 역과금(공급받는자과금)
            // - 역과금은 역발행 세금계산서를 발행하는 경우만 가능
            taxinvoice.chargeDirection = "정과금";

            // 발행형태, {정발행, 역발행, 위수탁} 중 기재
            taxinvoice.issueType = "정발행";

            // {영수, 청구, 없음} 중 기재
            taxinvoice.purposeType = "영수";

            // 과세형태, {과세, 영세, 면세} 중 기재
            taxinvoice.taxType = "과세";


            /*****************************************************************
             *                         공급자 정보                           *
             *****************************************************************/

            // 공급자 사업자번호, '-' 제외 10자리
            taxinvoice.invoicerCorpNum = corpNum;

            // 공급자 종사업자 식별번호. 필요시 기재. 형식은 숫자 4자리.
            taxinvoice.invoicerTaxRegID = "";

            // 공급자 상호
            taxinvoice.invoicerCorpName = "공급자 상호";

            // 공급자 문서번호, 숫자, 영문, '-', '_' 조합으로
            //  1~24자리까지 사업자번호별 중복없는 고유번호 할당
            taxinvoice.invoicerMgtKey = "20220527-002";

            // 공급자 대표자 성명
            taxinvoice.invoicerCEOName = "공급자 대표자 성명";

            // 공급자 주소
            taxinvoice.invoicerAddr = "공급자 주소";

            // 공급자 종목
            taxinvoice.invoicerBizClass = "공급자 종목";

            // 공급자 업태
            taxinvoice.invoicerBizType = "공급자 업태";

            // 공급자 담당자 성명
            taxinvoice.invoicerContactName = "공급자 담당자명";

            // 공급자 담당자 연락처
            taxinvoice.invoicerTEL = "";

            // 공급자 담당자 휴대폰번호
            taxinvoice.invoicerHP = "";

            // 공급자 담당자 메일주소
            taxinvoice.invoicerEmail = "";

            // 발행 안내 문자 전송여부 (true / false 중 택 1)
            // └ true = 전송 , false = 미전송
            // └ 공급받는자 (주)담당자 휴대폰번호 {invoiceeHP1} 값으로 문자 전송
            // - 전송 시 포인트 차감되며, 전송실패시 환불처리
            taxinvoice.invoicerSMSSendYN = false;


            /*********************************************************************
             *                         공급받는자 정보                           *
             *********************************************************************/

            // 공급받는자 구분, {사업자, 개인, 외국인} 중 기재
            taxinvoice.invoiceeType = "사업자";

            // 공급받는자 사업자번호
            // - {invoiceeType}이 "사업자" 인 경우, 사업자번호 (하이픈 ('-') 제외 10자리)
            // - {invoiceeType}이 "개인" 인 경우, 주민등록번호 (하이픈 ('-') 제외 13자리)
            // - {invoiceeType}이 "외국인" 인 경우, "9999999999999" (하이픈 ('-') 제외 13자리)
            taxinvoice.invoiceeCorpNum = "8888888888";

            // 공급받는자 상호
            taxinvoice.invoiceeCorpName = "공급받는자 상호";

            // [역발행시 필수] 공급받는자 문서번호, 숫자, 영문, '-', '_' 조합으로
            // 1~24자리까지 사업자번호별 중복없는 고유번호 할당
            taxinvoice.invoiceeMgtKey = "";

            // 공급받는자 대표자 성명
            taxinvoice.invoiceeCEOName = "공급받는자 대표자 성명";

            // 공급받는자 주소
            taxinvoice.invoiceeAddr = "공급받는자 주소";

            // 공급받는자 종목
            taxinvoice.invoiceeBizClass = "공급받는자 종목";

            // 공급받는자 업태
            taxinvoice.invoiceeBizType = "공급받는자 업태";

            // 공급받는자 주)담당자 성명
            taxinvoice.invoiceeContactName1 = "공급받는자 담당자명";

            // 공급받는자 주)담당자 연락처
            taxinvoice.invoiceeTEL1 = "";

            // 공급받는자 주)담당자 휴대폰번호
            taxinvoice.invoiceeHP1 = "";

            // 공급받는자 주)담당자 메일주소
            taxinvoice.invoiceeEmail1 = "";

            // 역발행 요청시 알림문자 전송여부 (역발행에서만 사용가능)
            // - 공급자 담당자 휴대폰번호(invoicerHP)로 전송
            // - 전송시 포인트가 차감되며 전송실패하는 경우 포인트 환불처리
            taxinvoice.invoiceeSMSSendYN = false;


            /*********************************************************************
             *                          세금계산서 정보                          *
             *********************************************************************/

            // 공급가액 합계
            taxinvoice.supplyCostTotal = "100000";

            // 세액 합계
            taxinvoice.taxTotal = "10000";

            // 합계금액,  공급가액 합계 + 세액 합계
            taxinvoice.totalAmount = "110000";

            // 기재상 일련번호 항목
            taxinvoice.serialNum = "";

            // 기재상 현금 항목
            taxinvoice.cash = "";

            // 기재상 수표 항목
            taxinvoice.chkBill = "";

            // 기재상 어음 항목
            taxinvoice.note = "";

            // 기재상 외상미수금 항목
            taxinvoice.credit = "";

            // 비고
            // {invoiceeType}이 "외국인" 이면 remark1 필수
            // - 외국인 등록번호 또는 여권번호 입력
            taxinvoice.remark1 = "비고1";
            taxinvoice.remark2 = "비고2";
            taxinvoice.remark3 = "비고3";

            // 기재상 권 항목, 최대값 32767
            // 미기재시 taxinvoice.kwon = null;
            taxinvoice.kwon = 1;

            // 기재상 호 항목, 최대값 32767
            // 미기재시 taxinvoice.ho = null;
            taxinvoice.ho = 1;

            // 사업자등록증 이미지 첨부여부 (true / false 중 택 1)
            // └ true = 첨부 , false = 미첨부(기본값)
            // - 팝빌 사이트 또는 인감 및 첨부문서 등록 팝업 URL (GetSealURL API) 함수를 이용하여 등록
            taxinvoice.businessLicenseYN = false;

            // 통장사본 이미지 첨부여부 (true / false 중 택 1)
            // └ true = 첨부 , false = 미첨부(기본값)
            // - 팝빌 사이트 또는 인감 및 첨부문서 등록 팝업 URL (GetSealURL API) 함수를 이용하여 등록
            taxinvoice.bankBookYN = false;

            /**************************************************************************
             *        수정세금계산서 정보 (수정세금계산서 작성시에만 기재             *
             * - 수정세금계산서 관련 정보는 연동매뉴얼 또는 개발가이드 링크 참조      *
             * - [참고] 수정세금계산서 작성방법 안내 - https://developers.popbill.com/guide/taxinvoice/dotnetcore/introduction/modified-taxinvoice  *
             *************************************************************************/

            // 수정사유코드, 1~6까지 선택기재.
            taxinvoice.modifyCode = null;

            // 수정세금계산서 작성시 원본세금계산서의 국세청승인번호
            taxinvoice.orgNTSConfirmNum = "";


            /********************************************************************************
             *                         상세항목(품목) 정보                                  *
             * - 상세항목 정보는 세금계산서 필수기재사항이 아니므로 작성하지 않더라도       *
             *   세금계산서 발행이 가능합니다.                                              *
             * - 최대 99건까지 작성가능                                                     *
             ********************************************************************************/

            taxinvoice.detailList = new List<TaxinvoiceDetail>();

            TaxinvoiceDetail detail = new TaxinvoiceDetail();

            detail.serialNum = 1; // 일련번호; 1부터 순차기재
            detail.purchaseDT = "20220527"; // 거래일자
            detail.itemName = "품목명"; // 품목명
            detail.spec = "규격"; // 규격
            detail.qty = "1"; // 수량
            detail.unitCost = "50000"; // 단가
            detail.supplyCost = "50000"; // 공급가액
            detail.tax = "5000"; // 세액
            detail.remark = "품목비고"; //비고

            taxinvoice.detailList.Add(detail);

            detail = new TaxinvoiceDetail();

            detail.serialNum = 2; // 일련번호; 1부터 순차기재
            detail.purchaseDT = "20220527"; // 거래일자
            detail.itemName = "품목명"; // 품목명
            detail.spec = "규격"; // 규격
            detail.qty = "1"; // 수량
            detail.unitCost = "50000"; // 단가
            detail.supplyCost = "50000"; // 공급가액
            detail.tax = "5000"; // 세액
            detail.remark = "품목비고"; //비고

            taxinvoice.detailList.Add(detail);


            /*********************************************************************************
            *                           추가담당자 정보                                      *
            * - 세금계산서 발행안내 메일을 수신받을 공급받는자 담당자가 다수인 경우 담당자   *
            *   정보를 추가하여 발행안내메일을 다수에게 전송할 수 있습니다.                  *
            * - 최대 5개까지 기재가능                                                        *
            **********************************************************************************/

            taxinvoice.addContactList = new List<TaxinvoiceAddContact>();

            TaxinvoiceAddContact addContact = new TaxinvoiceAddContact();

            addContact.serialNum = 1; // 일련번호, 1부터 순차기재
            addContact.email = ""; // 추가담당자 메일주소
            addContact.contactName = "추가담당자명"; // 추가담당자 성명

            taxinvoice.addContactList.Add(addContact);

            addContact = new TaxinvoiceAddContact();

            addContact.serialNum = 2; // 일련번호, 1부터 순차기재
            addContact.email = ""; // 추가담당자 메일주소
            addContact.contactName = "추가담당자명"; // 추가담당자 성명

            taxinvoice.addContactList.Add(addContact);


            // 거래명세서 동시작성여부 (true / false 중 택 1)
            // └ true = 사용 , false = 미사용
            // - 미입력 시 기본값 false 처리
            bool writeSpecification = false;

            // {writeSpecification} = true인 경우, 거래명세서 문서번호 할당
            // - 미입력시 기본값 세금계산서 문서번호와 동일하게 할당
            string dealInvoiceMgtKey = "";

            try
            {
                var response = _taxinvoiceService.Register(corpNum, taxinvoice, writeSpecification, dealInvoiceMgtKey);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * "임시저장" 상태의 세금계산서를 수정합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/issue#Update
         */
        public IActionResult Update()
        {
            // 수정할 세금계산서 문서번호
            string mgtKey = "20220527-002";

            // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
            MgtKeyType mgtKeyType = MgtKeyType.SELL;


            // 세금계산서 정보 객체
            Taxinvoice taxinvoice = new Taxinvoice();

            // 기재상 작성일자, 날짜형식(yyyyMMdd)
            taxinvoice.writeDate = "20220527";

            // 과금방향, {정과금, 역과금}중 선택
            // - 정과금(공급자과금), 역과금(공급받는자과금)
            // - 역과금은 역발행 세금계산서를 발행하는 경우만 가능
            taxinvoice.chargeDirection = "정과금";

            // 발행형태, {정발행, 역발행, 위수탁} 중 기재
            taxinvoice.issueType = "정발행";

            // {영수, 청구, 없음} 중 기재
            taxinvoice.purposeType = "영수";

            // 과세형태, {과세, 영세, 면세} 중 기재
            taxinvoice.taxType = "과세";


            /*****************************************************************
             *                         공급자 정보                           *
             *****************************************************************/

            // 공급자 사업자번호, '-' 제외 10자리
            taxinvoice.invoicerCorpNum = corpNum;

            // 공급자 종사업자 식별번호. 필요시 기재. 형식은 숫자 4자리.
            taxinvoice.invoicerTaxRegID = "";

            // 공급자 상호
            taxinvoice.invoicerCorpName = "공급자 상호";

            // 공급자 문서번호, 숫자, 영문, '-', '_' 조합으로
            //  1~24자리까지 사업자번호별 중복없는 고유번호 할당
            taxinvoice.invoicerMgtKey = "";

            // 공급자 대표자 성명
            taxinvoice.invoicerCEOName = "공급자 대표자 성명";

            // 공급자 주소
            taxinvoice.invoicerAddr = "공급자 주소";

            // 공급자 종목
            taxinvoice.invoicerBizClass = "공급자 종목";

            // 공급자 업태
            taxinvoice.invoicerBizType = "공급자 업태";

            // 공급자 담당자 성명
            taxinvoice.invoicerContactName = "공급자 담당자명";

            // 공급자 담당자 연락처
            taxinvoice.invoicerTEL = "";

            // 공급자 담당자 휴대폰번호
            taxinvoice.invoicerHP = "";

            // 공급자 담당자 메일주소
            taxinvoice.invoicerEmail = "";

            // 발행 안내 문자 전송여부 (true / false 중 택 1)
            // └ true = 전송 , false = 미전송
            // └ 공급받는자 (주)담당자 휴대폰번호 {invoiceeHP1} 값으로 문자 전송
            // - 전송 시 포인트 차감되며, 전송실패시 환불처리
            taxinvoice.invoicerSMSSendYN = false;


            /*********************************************************************
             *                         공급받는자 정보                           *
             *********************************************************************/

            // 공급받는자 구분, {사업자, 개인, 외국인} 중 기재
            taxinvoice.invoiceeType = "사업자";

            // 공급받는자 사업자번호
            // - {invoiceeType}이 "사업자" 인 경우, 사업자번호 (하이픈 ('-') 제외 10자리)
            // - {invoiceeType}이 "개인" 인 경우, 주민등록번호 (하이픈 ('-') 제외 13자리)
            // - {invoiceeType}이 "외국인" 인 경우, "9999999999999" (하이픈 ('-') 제외 13자리)
            taxinvoice.invoiceeCorpNum = "8888888888";

            // 공급받는자 상호
            taxinvoice.invoiceeCorpName = "공급받는자 상호";

            // [역발행시 필수] 공급받는자 문서번호, 숫자, 영문, '-', '_' 조합으로
            // 1~24자리까지 사업자번호별 중복없는 고유번호 할당
            taxinvoice.invoiceeMgtKey = "";

            // 공급받는자 대표자 성명
            taxinvoice.invoiceeCEOName = "공급받는자 대표자 성명";

            // 공급받는자 주소
            taxinvoice.invoiceeAddr = "공급받는자 주소";

            // 공급받는자 종목
            taxinvoice.invoiceeBizClass = "공급받는자 종목";

            // 공급받는자 업태
            taxinvoice.invoiceeBizType = "공급받는자 업태";

            // 공급받는자 주)담당자 성명
            taxinvoice.invoiceeContactName1 = "공급받는자 담당자명";

            // 공급받는자 주)담당자 연락처
            taxinvoice.invoiceeTEL1 = "";

            // 공급받는자 주)담당자 휴대폰번호
            taxinvoice.invoiceeHP1 = "";

            // 공급받는자 주)담당자 메일주소
            // 팝빌 개발환경에서 테스트하는 경우에도 안내 메일이 전송되므로,
            // 실제 거래처의 메일주소가 기재되지 않도록 주의
            taxinvoice.invoiceeEmail1 = "";

            // 역발행 요청시 알림문자 전송여부 (역발행에서만 사용가능)
            // - 공급자 담당자 휴대폰번호(invoicerHP)로 전송
            // - 전송시 포인트가 차감되며 전송실패하는 경우 포인트 환불처리
            taxinvoice.invoiceeSMSSendYN = false;


            /*********************************************************************
             *                          세금계산서 정보                          *
             *********************************************************************/

            // 공급가액 합계
            taxinvoice.supplyCostTotal = "100000";

            // 세액 합계
            taxinvoice.taxTotal = "10000";

            // 합계금액,  공급가액 합계 + 세액 합계
            taxinvoice.totalAmount = "110000";

            // 기재상 일련번호 항목
            taxinvoice.serialNum = "";

            // 기재상 현금 항목
            taxinvoice.cash = "";

            // 기재상 수표 항목
            taxinvoice.chkBill = "";

            // 기재상 어음 항목
            taxinvoice.note = "";

            // 기재상 외상미수금 항목
            taxinvoice.credit = "";

            // 비고
            // {invoiceeType}이 "외국인" 이면 remark1 필수
            // - 외국인 등록번호 또는 여권번호 입력
            taxinvoice.remark1 = "비고1";
            taxinvoice.remark2 = "비고2";
            taxinvoice.remark3 = "비고3";

            // 기재상 권 항목, 최대값 32767
            // 미기재시 taxinvoice.kwon = null;
            taxinvoice.kwon = 1;

            // 기재상 호 항목, 최대값 32767
            // 미기재시 taxinvoice.ho = null;
            taxinvoice.ho = 1;

            // 사업자등록증 이미지 첨부여부 (true / false 중 택 1)
            // └ true = 첨부 , false = 미첨부(기본값)
            // - 팝빌 사이트 또는 인감 및 첨부문서 등록 팝업 URL (GetSealURL API) 함수를 이용하여 등록
            taxinvoice.businessLicenseYN = false;

            // 통장사본 이미지 첨부여부 (true / false 중 택 1)
            // └ true = 첨부 , false = 미첨부(기본값)
            // - 팝빌 사이트 또는 인감 및 첨부문서 등록 팝업 URL (GetSealURL API) 함수를 이용하여 등록
            taxinvoice.bankBookYN = false;


            /**************************************************************************
             *        수정세금계산서 정보 (수정세금계산서 작성시에만 기재             *
             * - 수정세금계산서 관련 정보는 연동매뉴얼 또는 개발가이드 링크 참조      *
             * - [참고] 수정세금계산서 작성방법 안내 - https://developers.popbill.com/guide/taxinvoice/dotnetcore/introduction/modified-taxinvoice  *
             *************************************************************************/

            // 수정사유코드, 1~6까지 선택기재.
            taxinvoice.modifyCode = null;

            // 수정세금계산서 작성시 원본세금계산서의 국세청승인번호
            taxinvoice.orgNTSConfirmNum = "";


            /********************************************************************************
             *                         상세항목(품목) 정보                                  *
             * - 상세항목 정보는 세금계산서 필수기재사항이 아니므로 작성하지 않더라도       *
             *   세금계산서 발행이 가능합니다.                                              *
             * - 최대 99건까지 작성가능                                                     *
             ********************************************************************************/

            taxinvoice.detailList = new List<TaxinvoiceDetail>();

            TaxinvoiceDetail detail = new TaxinvoiceDetail();

            detail.serialNum = 1; // 일련번호; 1부터 순차기재
            detail.purchaseDT = "20220527"; // 거래일자
            detail.itemName = "품목명(수정)"; // 품목명
            detail.spec = "규격"; // 규격
            detail.qty = "1"; // 수량
            detail.unitCost = "50000"; // 단가
            detail.supplyCost = "50000"; // 공급가액
            detail.tax = "5000"; // 세액
            detail.remark = "품목비고"; //비고

            taxinvoice.detailList.Add(detail);

            detail = new TaxinvoiceDetail();

            detail.serialNum = 2; // 일련번호; 1부터 순차기재
            detail.purchaseDT = "20220527"; // 거래일자
            detail.itemName = "품목명"; // 품목명
            detail.spec = "규격"; // 규격
            detail.qty = "1"; // 수량
            detail.unitCost = "50000"; // 단가
            detail.supplyCost = "50000"; // 공급가액
            detail.tax = "5000"; // 세액
            detail.remark = "품목비고"; //비고

            taxinvoice.detailList.Add(detail);


            /*********************************************************************************
            *                           추가담당자 정보                                      *
            * - 세금계산서 발행안내 메일을 수신받을 공급받는자 담당자가 다수인 경우 담당자   *
            *   정보를 추가하여 발행안내메일을 다수에게 전송할 수 있습니다.                  *
            * - 최대 5개까지 기재가능                                                        *
            **********************************************************************************/

            taxinvoice.addContactList = new List<TaxinvoiceAddContact>();

            TaxinvoiceAddContact addContact = new TaxinvoiceAddContact();

            addContact.serialNum = 1; // 일련번호, 1부터 순차기재
            addContact.email = ""; // 추가담당자 메일주소
            addContact.contactName = "추가담당자명(수정)"; // 추가담당자 성명

            taxinvoice.addContactList.Add(addContact);

            addContact = new TaxinvoiceAddContact();

            addContact.serialNum = 2; // 일련번호, 1부터 순차기재
            addContact.email = ""; // 추가담당자 메일주소
            addContact.contactName = "추가담당자명"; // 추가담당자 성명

            taxinvoice.addContactList.Add(addContact);

            try
            {
                var response = _taxinvoiceService.Update(corpNum, mgtKeyType, mgtKey, taxinvoice);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * "임시저장" 또는 "(역)발행대기" 상태의 세금계산서를 발행(전자서명)하며, "발행완료" 상태로 처리합니다.
         * - 세금계산서 국세청 전송정책 [https://developers.popbill.com/guide/taxinvoice/dotnetcore/introduction/policy-of-send-to-nts]
         * - "발행완료" 된 전자세금계산서는 국세청 전송 이전에 발행취소(CancelIssue API) 함수로 국세청 신고 대상에서 제외할 수 있습니다.
         * - 세금계산서 발행을 위해서 공급자의 인증서가 팝빌 인증서버에 사전등록 되어야 합니다.
         *   └ 위수탁발행의 경우, 수탁자의 인증서 등록이 필요합니다.
         * - 세금계산서 발행 시 공급받는자에게 발행 메일이 발송됩니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/issue#Issue
         */
        public IActionResult Issue()
        {
            // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
            MgtKeyType mgtKeyType = MgtKeyType.SELL;

            // 발행처리할 세금계산서 문서번호
            string mgtKey = "20220527-002";

            // 지연발행 강제여부  (true / false 중 택 1)
            // └ true = 가능 , false = 불가능
            // - 미입력 시 기본값 false 처리
            // - 발행마감일이 지난 세금계산서를 발행하는 경우, 가산세가 부과될 수 있습니다.
            // - 가산세가 부과되더라도 발행을 해야하는 경우에는 forceIssue의 값을
            //   true로 선언하여 발행(Issue API)를 호출하시면 됩니다.
            bool forceIssue = false;

            // 메모
            string memo = "발행메모";

            // 발행안내메일 제목, 미기재시 기본양식으로 전송됨
            string emailSubject = "";

            try
            {
                var response = _taxinvoiceService.Issue(corpNum, mgtKeyType, mgtKey, forceIssue, memo, emailSubject);
                return View("IssueResponse", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 국세청 전송 이전 "발행완료" 상태의 전자세금계산서를 "발행취소"하고 국세청 신고대상에서 제외합니다.
         * - Delete(삭제)함수를 호출하여 "발행취소" 상태의 전자세금계산서를 삭제하면, 문서번호 재사용이 가능합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/issue#CancelIssue
         */
        public IActionResult CancelIssue()
        {
            // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
            MgtKeyType mgtKeyType = MgtKeyType.SELL;

            // 발행취소할 세금계산서 문서번호
            string mgtKey = "20220527-002";

            // 메모
            string memo = "발행 취소 메모";

            try
            {
                var response = _taxinvoiceService.CancelIssue(corpNum, mgtKeyType, mgtKey, memo);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 공급받는자가 작성한 세금계산서 데이터를 팝빌에 저장하고 공급자에게 송부하여 발행을 요청합니다.
         * - 역발행 세금계산서 프로세스를 구현하기 위해서는 공급자/공급받는자가 모두 팝빌에 회원이여야 합니다.
         * - 발행 요청된 세금계산서는 "(역)발행대기" 상태이며, 공급자가 팝빌 사이트 또는 함수를 호출하여 발행한 경우에만 국세청으로 전송됩니다.
         * - 공급자는 팝빌 사이트의 "매출 발행 대기함"에서 발행대기 상태의 역발행 세금계산서를 확인할 수 있습니다.
         * - 임시저장(Register API) 함수와 역발행 요청(Request API) 함수를 한 번의 프로세스로 처리합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/issue#RegistRequest
         */
        public IActionResult RegistRequest()
        {
            // 세금계산서 정보 객체
            Taxinvoice taxinvoice = new Taxinvoice();

            // 기재상 작성일자, 날짜형식(yyyyMMdd)
            taxinvoice.writeDate = "20220527";

            // 과금방향, {정과금, 역과금}중 선택
            // - 정과금(공급자과금), 역과금(공급받는자과금)
            // - 역과금은 역발행 세금계산서를 발행하는 경우만 가능
            taxinvoice.chargeDirection = "정과금";

            // 발행형태, {정발행, 역발행, 위수탁} 중 기재
            taxinvoice.issueType = "역발행";

            // {영수, 청구, 없음} 중 기재
            taxinvoice.purposeType = "영수";

            // 과세형태, {과세, 영세, 면세} 중 기재
            taxinvoice.taxType = "과세";


            /*****************************************************************
             *                         공급자 정보                           *
             *****************************************************************/

            // 공급자 사업자번호, '-' 제외 10자리
            taxinvoice.invoicerCorpNum = "8888888888";

            // 공급자 종사업자 식별번호. 필요시 기재. 형식은 숫자 4자리.
            taxinvoice.invoicerTaxRegID = "";

            // 공급자 상호
            taxinvoice.invoicerCorpName = "공급자 상호";

            // 공급자 문서번호, 숫자, 영문, '-', '_' 조합으로
            //  1~24자리까지 사업자번호별 중복없는 고유번호 할당
            taxinvoice.invoicerMgtKey = "";

            // 공급자 대표자 성명
            taxinvoice.invoicerCEOName = "공급자 대표자 성명";

            // 공급자 주소
            taxinvoice.invoicerAddr = "공급자 주소";

            // 공급자 종목
            taxinvoice.invoicerBizClass = "공급자 종목";

            // 공급자 업태
            taxinvoice.invoicerBizType = "공급자 업태";

            // 공급자 담당자 성명
            taxinvoice.invoicerContactName = "공급자 담당자명";

            // 공급자 담당자 연락처
            taxinvoice.invoicerTEL = "";

            // 공급자 담당자 휴대폰번호
            taxinvoice.invoicerHP = "";

            // 공급자 담당자 메일주소
            taxinvoice.invoicerEmail = "";

            // 발행 안내 문자 전송여부 (true / false 중 택 1)
            // └ true = 전송 , false = 미전송
            // └ 공급받는자 (주)담당자 휴대폰번호 {invoiceeHP1} 값으로 문자 전송
            // - 전송 시 포인트 차감되며, 전송실패시 환불처리
            taxinvoice.invoicerSMSSendYN = false;


            /*********************************************************************
             *                         공급받는자 정보                           *
             *********************************************************************/

            // 공급받는자 구분, {사업자, 개인, 외국인} 중 기재
            taxinvoice.invoiceeType = "사업자";

            // 공급받는자 사업자번호
            // - {invoiceeType}이 "사업자" 인 경우, 사업자번호 (하이픈 ('-') 제외 10자리)
            // - {invoiceeType}이 "개인" 인 경우, 주민등록번호 (하이픈 ('-') 제외 13자리)
            // - {invoiceeType}이 "외국인" 인 경우, "9999999999999" (하이픈 ('-') 제외 13자리)
            taxinvoice.invoiceeCorpNum = corpNum;

            // 공급받는자 상호
            taxinvoice.invoiceeCorpName = "공급받는자 상호";

            // [역발행시 필수] 공급받는자 문서번호, 숫자, 영문, '-', '_' 조합으로
            // 1~24자리까지 사업자번호별 중복없는 고유번호 할당
            taxinvoice.invoiceeMgtKey = "20220527-003";

            // 공급받는자 대표자 성명
            taxinvoice.invoiceeCEOName = "공급받는자 대표자 성명";

            // 공급받는자 주소
            taxinvoice.invoiceeAddr = "공급받는자 주소";

            // 공급받는자 종목
            taxinvoice.invoiceeBizClass = "공급받는자 종목";

            // 공급받는자 업태
            taxinvoice.invoiceeBizType = "공급받는자 업태";

            // 공급받는자 주)담당자 성명
            taxinvoice.invoiceeContactName1 = "공급받는자 담당자명";

            // 공급받는자 주)담당자 연락처
            taxinvoice.invoiceeTEL1 = "";

            // 공급받는자 주)담당자 휴대폰번호
            taxinvoice.invoiceeHP1 = "";

            // 공급받는자 주)담당자 메일주소
            // 팝빌 개발환경에서 테스트하는 경우에도 안내 메일이 전송되므로,
            // 실제 거래처의 메일주소가 기재되지 않도록 주의
            taxinvoice.invoiceeEmail1 = "";

            // 역발행 요청시 알림문자 전송여부 (역발행에서만 사용가능)
            // - 공급자 담당자 휴대폰번호(invoicerHP)로 전송
            // - 전송시 포인트가 차감되며 전송실패하는 경우 포인트 환불처리
            taxinvoice.invoiceeSMSSendYN = false;


            /*********************************************************************
             *                          세금계산서 정보                          *
             *********************************************************************/

            // 공급가액 합계
            taxinvoice.supplyCostTotal = "100000";

            // 세액 합계
            taxinvoice.taxTotal = "10000";

            // 합계금액,  공급가액 합계 + 세액 합계
            taxinvoice.totalAmount = "110000";

            // 기재상 일련번호 항목
            taxinvoice.serialNum = "";

            // 기재상 현금 항목
            taxinvoice.cash = "";

            // 기재상 수표 항목
            taxinvoice.chkBill = "";

            // 기재상 어음 항목
            taxinvoice.note = "";

            // 기재상 외상미수금 항목
            taxinvoice.credit = "";

            // 비고
            // {invoiceeType}이 "외국인" 이면 remark1 필수
            // - 외국인 등록번호 또는 여권번호 입력
            taxinvoice.remark1 = "비고1";
            taxinvoice.remark2 = "비고2";
            taxinvoice.remark3 = "비고3";

            // 기재상 권 항목, 최대값 32767
            // 미기재시 taxinvoice.kwon = null;
            taxinvoice.kwon = 1;

            // 기재상 호 항목, 최대값 32767
            // 미기재시 taxinvoice.ho = null;
            taxinvoice.ho = 1;

            // 사업자등록증 이미지 첨부여부 (true / false 중 택 1)
            // └ true = 첨부 , false = 미첨부(기본값)
            // - 팝빌 사이트 또는 인감 및 첨부문서 등록 팝업 URL (GetSealURL API) 함수를 이용하여 등록
            taxinvoice.businessLicenseYN = false;

            // 통장사본 이미지 첨부여부 (true / false 중 택 1)
            // └ true = 첨부 , false = 미첨부(기본값)
            // - 팝빌 사이트 또는 인감 및 첨부문서 등록 팝업 URL (GetSealURL API) 함수를 이용하여 등록
            taxinvoice.bankBookYN = false;


            /**************************************************************************
             *        수정세금계산서 정보 (수정세금계산서 작성시에만 기재             *
             * - 수정세금계산서 관련 정보는 연동매뉴얼 또는 개발가이드 링크 참조      *
             * - [참고] 수정세금계산서 작성방법 안내 - https://developers.popbill.com/guide/taxinvoice/dotnetcore/introduction/modified-taxinvoice  *
             *************************************************************************/

            // 수정사유코드, 1~6까지 선택기재.
            taxinvoice.modifyCode = null;

            // 수정세금계산서 작성시 원본세금계산서의 국세청승인번호
            taxinvoice.orgNTSConfirmNum = "";


            /********************************************************************************
             *                         상세항목(품목) 정보                                  *
             * - 상세항목 정보는 세금계산서 필수기재사항이 아니므로 작성하지 않더라도       *
             *   세금계산서 발행이 가능합니다.                                              *
             * - 최대 99건까지 작성가능                                                     *
             ********************************************************************************/

            taxinvoice.detailList = new List<TaxinvoiceDetail>();

            TaxinvoiceDetail detail = new TaxinvoiceDetail();

            detail.serialNum = 1; // 일련번호; 1부터 순차기재
            detail.purchaseDT = "20220527"; // 거래일자
            detail.itemName = "품목명"; // 품목명
            detail.spec = "규격"; // 규격
            detail.qty = "1"; // 수량
            detail.unitCost = "50000"; // 단가
            detail.supplyCost = "50000"; // 공급가액
            detail.tax = "5000"; // 세액
            detail.remark = "품목비고"; //비고

            taxinvoice.detailList.Add(detail);

            detail = new TaxinvoiceDetail();

            detail.serialNum = 2; // 일련번호; 1부터 순차기재
            detail.purchaseDT = "20220527"; // 거래일자
            detail.itemName = "품목명"; // 품목명
            detail.spec = "규격"; // 규격
            detail.qty = "1"; // 수량
            detail.unitCost = "50000"; // 단가
            detail.supplyCost = "50000"; // 공급가액
            detail.tax = "5000"; // 세액
            detail.remark = "품목비고"; //비고

            taxinvoice.detailList.Add(detail);

            // 메모
            string memo = "";

            try
            {
                var response = _taxinvoiceService.RegistRequest(corpNum, taxinvoice, memo);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 공급받는자가 저장된 역발행 세금계산서를 공급자에게 송부하여 발행 요청합니다.
         * - 역발행 세금계산서 프로세스를 구현하기 위해서는 공급자/공급받는자가 모두 팝빌에 회원이여야 합니다.
         * - 역발행 요청된 세금계산서는 "(역)발행대기" 상태이며, 공급자가 팝빌 사이트 또는 함수를 호출하여 발행한 경우에만 국세청으로 전송됩니다.
         * - 공급자는 팝빌 사이트의 "매출 발행 대기함"에서 발행대기 상태의 역발행 세금계산서를 확인할 수 있습니다.
         * - 역발행 요청시 공급자에게 역발행 요청 메일이 발송됩니다.
         * - 공급자가 역발행 세금계산서 발행시 포인트가 과금됩니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/issue#Request
         */
        public IActionResult TIRequest()
        {
            // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
            MgtKeyType mgtKeyType = MgtKeyType.BUY;

            // 세금계산서 문서번호
            string mgtKey = "20220527-002";

            //메모
            string memo = "역발행요청 메모";

            try
            {
                var response = _taxinvoiceService.Request(corpNum, mgtKeyType, mgtKey, memo);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 공급자가 요청받은 역발행 세금계산서를 발행하기 전, 공급받는자가 역발행요청을 취소합니다.
         * - 함수 호출시 상태 값이 "취소"로 변경되고, 해당 역발행 세금계산서는 공급자에 의해 발행 될 수 없습니다.
         * - [취소]한 세금계산서의 문서번호를 재사용하기 위해서는 삭제 (Delete API) 함수를 호출해야 합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/issue#CancelRequest
         */
        public IActionResult CancelRequest()
        {
            // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
            MgtKeyType mgtKeyType = MgtKeyType.BUY;

            // 세금계산서 문서번호
            string mgtKey = "20220527-003";

            //메모
            string memo = "역발행 취소 메모";

            try
            {
                var response = _taxinvoiceService.CancelRequest(corpNum, mgtKeyType, mgtKey, memo);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 공급자가 공급받는자에게 역발행 요청 받은 세금계산서의 발행을 거부합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/issue#Refuse
         */
        public IActionResult Refuse()
        {
            // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
            MgtKeyType mgtKeyType = MgtKeyType.SELL;

            // 세금계산서 문서번호
            string mgtKey = "20220527-003";

            //메모
            string memo = "역발행 거부 메모";

            try
            {
                var response = _taxinvoiceService.Refuse(corpNum, mgtKeyType, mgtKey, memo);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 삭제 가능한 상태의 세금계산서를 삭제합니다.
         * - 삭제 가능한 상태: "임시저장", "발행취소", "역발행거부", "역발행취소", "전송실패"
         * - 세금계산서를 삭제해야만 문서번호(mgtKey)를 재사용할 수 있습니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/issue#Delete
         */
        public IActionResult Delete()
        {
            // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
            MgtKeyType mgtKeyType = MgtKeyType.SELL;

            // 삭제처리할 세금계산서 문서번호
            string mgtKey = "20220527-002";

            try
            {
                var response = _taxinvoiceService.Delete(corpNum, mgtKeyType, mgtKey);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 공급자가 "발행완료" 상태의 전자세금계산서를 국세청에 즉시 전송하며, 함수 호출 후 최대 30분 이내에 전송 처리가 완료됩니다.
         * - 국세청 즉시전송을 호출하지 않은 세금계산서는 발행일 기준 익일 오후 3시에 팝빌 시스템에서 일괄적으로 국세청으로 전송합니다.
         * - 익일전송시 전송일이 법정공휴일인 경우 다음 영업일에 전송됩니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/issue#SendToNTS
         */
        public IActionResult SendToNTS()
        {
            // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
            MgtKeyType mgtKeyType = MgtKeyType.SELL;

            // 세금계산서 문서번호
            string mgtKey = "20220527-001";

            try
            {
                var response = _taxinvoiceService.SendToNTS(corpNum, mgtKeyType, mgtKey);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        #endregion

        #region 세금계산서 정보확인

        /*
         * 세금계산서 1건의 상태 및 요약정보를 확인합니다.
         * 리턴값 'TaxinvoiceInfo'의 변수 'stateCode'를 통해 세금계산서의 상태코드를 확인합니다.
         * 세금계산서 상태코드 [https://developers.popbill.com/reference/taxinvoice/dotnetcore/response-code#state-code]
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/info#GetInfo
         */
        public IActionResult GetInfo()
        {
            // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
            MgtKeyType mgtKeyType = MgtKeyType.SELL;

            // 세금계산서 문서번호
            string mgtKey = "20220527-002";

            try
            {
                var response = _taxinvoiceService.GetInfo(corpNum, mgtKeyType, mgtKey);
                return View("GetInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
     * 다수건의 세금계산서 상태 및 요약 정보를 확인합니다. (1회 호출 시 최대 1,000건 확인 가능)
     * 리턴값 'TaxinvoiceInfo'의 변수 'stateCode'를 통해 세금계산서의 상태코드를 확인합니다.
     * 세금계산서 상태코드 [https://developers.popbill.com/reference/taxinvoice/dotnetcore/response-code#state-code]
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/info#GetInfos
         */
        public IActionResult GetInfos()
        {
            // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
            MgtKeyType mgtKeyType = MgtKeyType.SELL;

            // 조회할 세금계산서 문서번호 배열, (최대 1000건)
            List<string> mgtKeyList = new List<string>();
            mgtKeyList.Add("20220527-001");
            mgtKeyList.Add("20220527-002");

            try
            {
                var response = _taxinvoiceService.GetInfos(corpNum, mgtKeyType, mgtKeyList);
                return View("GetInfos", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 세금계산서 1건의 상세정보를 확인합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/info#GetDetailInfo
         */
        public IActionResult GetDetailInfo()
        {
            // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
            MgtKeyType mgtKeyType = MgtKeyType.SELL;

            // 세금계산서 문서번호
            string mgtKey = "20220527-002";

            try
            {
                var response = _taxinvoiceService.GetDetailInfo(corpNum, mgtKeyType, mgtKey);
                return View("GetDetailInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 세금계산서 1건의 상세정보를 XML로 반환합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/info#GetXML
         */
        public IActionResult GetXML()
        {
            // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
            MgtKeyType mgtKeyType = MgtKeyType.SELL;

            // 세금계산서 문서번호
            string mgtKey = "20220527-002";

            try
            {
                var response = _taxinvoiceService.GetXML(corpNum, mgtKeyType, mgtKey);
                return View("GetXML", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 검색조건에 해당하는 세금계산서를 조회합니다. (조회기간 단위 : 최대 6개월)
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/info#Search
         */
        public IActionResult Search()
        {
            // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
            MgtKeyType mgtKeyType = MgtKeyType.SELL;

            // 일자유형 ("R" , "W" , "I" 중 택 1)
            // - R = 등록일자 , W = 작성일자 , I = 발행일자
            string DType = "W";

            // 시작일자, 날자형식(yyyyMMdd)
            string SDate = "20220501";

            // 종료일자, 날자형식(yyyyMMdd)
            string EDate = "20220531";

            // 세금계산서 상태코드 배열 (2,3번째 자리에 와일드카드(*) 사용 가능)
            // - 미입력시 전체조회
            string[] state = new string[3];
            state[0] = "3**";
            state[1] = "4**";
            state[2] = "6**";

            // 문서유형 배열 ("N" , "M" 중 선택, 다중 선택 가능)
            // - N = 일반세금계산서 , M = 수정세금계산서
            // - 미입력시 전체조회
            string[] Type = new string[2];
            Type[0] = "N";
            Type[1] = "M";

            // 과세형태 배열 ("T" , "N" , "Z" 중 선택, 다중 선택 가능)
            // - T = 과세 , N = 면세 , Z = 영세
            // - 미입력시 전체조회
            string[] TaxType = new string[3];
            TaxType[0] = "T";
            TaxType[1] = "N";
            TaxType[2] = "Z";

            // 발행형태 배열 ("N" , "R" , "T" 중 선택, 다중 선택 가능)
            // - N = 정발행 , R = 역발행 , T = 위수탁발행
            // - 미입력시 전체조회
            string[] IssueType = new string[3];
            IssueType[0] = "N";
            IssueType[1] = "R";
            IssueType[2] = "T";

            // 등록유형 배열 ("P" , "H" 중 선택, 다중 선택 가능)
            // - P = 팝빌에서 등록 , H = 홈택스 또는 외부ASP 등록
            // - 미입력시 전체조회
            string[] RegType = new string[2];
            RegType[0] = "P";
            RegType[1] = "H";

            // 공급받는자 휴폐업상태 배열 ("N" , "0" , "1" , "2" , "3" , "4" 중 선택, 다중 선택 가능)
            // - N = 미확인 , 0 = 미등록 , 1 = 사업 , 2 = 폐업 , 3 = 휴업 , 4 = 확인실패
            // - 미입력시 전체조회
            string[] CloseDownState = new string[5];
            CloseDownState[0] = "N";
            CloseDownState[1] = "0";
            CloseDownState[2] = "1";
            CloseDownState[3] = "2";
            CloseDownState[4] = "3";

            // 지연발행 여부 (null , true , false 중 택 1)
            // - null = 전체조회 , true = 지연발행 , false = 정상발행
            bool? LateOnly = null;

            // 종사업장번호 유무 (null , "0" , "1" 중 택 1)
            // - null = 전체 , 0 = 없음, 1 = 있음
            string TaxRegIDYN = "";

            // 종사업장번호의 주체 ("S" , "B" , "T" 중 택 1)
            // └ S = 공급자 , B = 공급받는자 , T = 수탁자
            // - 미입력시 전체조회
            string TaxRegIDType = "S";

            // 종사업장번호, 콤마(",")로 구분하여 구성 ex) "0001,1234"
            string TaxRegID = "";

            // 페이지 번호, 기본값 '1'
            int Page = 1;

            // 페이지당 검색개수, 기본값 '500', 최대 '1000'
            int PerPage = 30;

            // 정렬방향, A-오름차순, D-내림차순
            string Order = "D";

            // 거래처 상호 / 사업자번호 (사업자) / 주민등록번호 (개인) / "9999999999999" (외국인) 중 검색하고자 하는 정보 입력
            // └ 사업자번호 / 주민등록번호는 하이픈('-')을 제외한 숫자만 입력
            // - 미입력시 전체조회
            string Qstring = "";

            // 문서번호 또는 국세청승인번호 조회 검색어
            string MgtKey = "";

            // 연동문서 여부 (null , "0" , "1" 중 택 1)
            // └ null = 전체조회 , 0 = 일반문서 , 1 = 연동문서
            // - 일반문서 : 팝빌 사이트를 통해 저장 또는 발행한 세금계산서
            // - 연동문서 : 팝빌 API를 통해 저장 또는 발행한 세금계산서
            string InterOPYN = "";

            try
            {
                var response = _taxinvoiceService.Search(corpNum, mgtKeyType, DType, SDate, EDate, state, Type, TaxType,
                    IssueType, LateOnly, TaxRegIDYN, TaxRegIDType, TaxRegID, Page, PerPage, Order, Qstring, InterOPYN,
                    userID, RegType, CloseDownState, MgtKey);
                return View("Search", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 세금계산서의 상태에 대한 변경이력을 확인합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/info#GetLogs
         */
        public IActionResult GetLogs()
        {
            // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
            MgtKeyType mgtKeyType = MgtKeyType.SELL;

            // 세금계산서 문서번호
            string mgtKey = "20220527-001";

            try
            {
                var response = _taxinvoiceService.GetLogs(corpNum, mgtKeyType, mgtKey);
                return View("GetLogs", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 로그인 상태로 팝빌 사이트의 전자세금계산서 임시문서함 메뉴에 접근할 수 있는 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/info#GetURL
         */
        public IActionResult GetURL()
        {
            // TBOX (임시문서함) , SBOX (매출문서함) , PBOX (매입문서함) ,
            // SWBOX (매출발행 대기함) , PWBOX (매입발행 대기함) , WRITE (정발행 작성)
            string TOGO = "SBOX";

            try
            {
                var result = _taxinvoiceService.GetURL(corpNum, TOGO, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        #endregion

        #region 세금계산서 보기/인쇄

        /*
         * 세금계산서 1건의 상세 정보 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/view#GetPopUpURL
         */
        public IActionResult GetPopUpURL()
        {
            // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
            MgtKeyType mgtKeyType = MgtKeyType.SELL;

            // 세금계산서 문서번호
            string mgtKey = "20220527-001";

            try
            {
                var result = _taxinvoiceService.GetPopUpURL(corpNum, mgtKeyType, mgtKey, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 세금계산서 1건의 상세정보 페이지(사이트 상단, 좌측 메뉴 및 버튼 제외)의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/view#GetViewURL
         */
        public IActionResult GetViewURL()
        {
            // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
            MgtKeyType mgtKeyType = MgtKeyType.SELL;

            // 세금계산서 문서번호
            string mgtKey = "20220527-001";

            try
            {
                var result = _taxinvoiceService.GetViewURL(corpNum, mgtKeyType, mgtKey, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        /*
         * 전자세금계산서 PDF 파일을 다운 받을 수 있는 URL을 반환합니다.
         * - 반환되는 URL은 보안정책상 30초의 유효시간을 갖으며, 유효시간 이후 호출시 정상적으로 페이지가 호출되지 않습니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/view#GetPDFURL
         */
        public IActionResult GetPDFURL()
        {
            // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
            MgtKeyType mgtKeyType = MgtKeyType.SELL;

            // 세금계산서 문서번호
            string mgtKey = "20220527-002";

            try
            {
                var result = _taxinvoiceService.GetPDFURL(corpNum, mgtKeyType, mgtKey, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        /*
         * 세금계산서 1건을 인쇄하기 위한 페이지의 팝업 URL을 반환하며, 페이지내에서 인쇄 설정값을 "공급자" / "공급받는자" / "공급자+공급받는자"용 중 하나로 지정할 수 있습니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/view#GetPrintURL
         */
        public IActionResult GetPrintURL()
        {
            // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
            MgtKeyType mgtKeyType = MgtKeyType.SELL;

            // 세금계산서 문서번호
            string mgtKey = "20220527-002";

            try
            {
                var result = _taxinvoiceService.GetPrintURL(corpNum, mgtKeyType, mgtKey, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 세금계산서 1건을 구버전 양식으로 인쇄하기 위한 페이지의 팝업 URL을 반환하며, 페이지내에서 인쇄 설정값을 "공급자" / "공급받는자" / "공급자+공급받는자"용 중 하나로 지정할 수 있습니다..
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/view#GetOldPrintURL
         */
        public IActionResult GetOldPrintURL()
        {
            // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
            MgtKeyType mgtKeyType = MgtKeyType.SELL;

            // 세금계산서 문서번호
            string mgtKey = "20220527-002";

            try
            {
                var result = _taxinvoiceService.GetOldPrintURL(corpNum, mgtKeyType, mgtKey, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * "공급받는자" 용 세금계산서 1건을 인쇄하기 위한 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/view#GetEPrintURL
         */
        public IActionResult GetEPrintURL()
        {
            // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
            MgtKeyType mgtKeyType = MgtKeyType.SELL;

            // 세금계산서 문서번호
            string mgtKey = "20220527-002";

            try
            {
                var result = _taxinvoiceService.GetEPrintURL(corpNum, mgtKeyType, mgtKey, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 다수건의 세금계산서를 인쇄하기 위한 페이지의 팝업 URL을 반환합니다. (최대 100건)
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/view#GetMassPrintURL
         */
        public IActionResult GetMassPrintURL()
        {
            // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
            MgtKeyType mgtKeyType = MgtKeyType.SELL;

            // 조회할 세금계산서 문서번호 배열, (최대 100건)
            List<string> mgtKeyList = new List<string>();
            mgtKeyList.Add("20220527-001");
            mgtKeyList.Add("20220527-002");

            try
            {
                var result = _taxinvoiceService.GetMassPrintURL(corpNum, mgtKeyType, mgtKeyList, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 전자세금계산서 안내메일의 상세보기 링크 URL을 반환합니다.
         * - 함수 호출로 반환 받은 URL에는 유효시간이 없습니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/view#GetMailURL
         */
        public IActionResult GetMailURL()
        {
            // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
            MgtKeyType mgtKeyType = MgtKeyType.SELL;

            // 세금계산서 문서번호
            string mgtKey = "20220527-002";

            try
            {
                var result = _taxinvoiceService.GetMailURL(corpNum, mgtKeyType, mgtKey, userID);
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
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/member#GetAccessURL
         */
        public IActionResult GetAccessURL()
        {
            try
            {
                var result = _taxinvoiceService.GetAccessURL(corpNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 세금계산서에 첨부할 인감, 사업자등록증, 통장사본을 등록하는 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/etc#GetSealURL
         */
        public IActionResult GetSealURL()
        {
            try
            {
                var result = _taxinvoiceService.GetSealURL(corpNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * "임시저장" 상태의 세금계산서에 1개의 파일을 첨부합니다. (최대 5개)
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/etc#AttachFile
         */
        public IActionResult AttachFile()
        {
            // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
            MgtKeyType mgtKeyType = MgtKeyType.SELL;

            // 세금계산서 문서번호
            string mgtKey = "20220527-002";

            // 파일경로
            string filePath = "C:/popbill.example.dotnetcore/TaxinvoiceExample/wwwroot/images/tax_image.png";

            // 첨부파일명
            string displayName = "DisplayName.png";

            try
            {
                var response = _taxinvoiceService.AttachFile(corpNum, mgtKeyType, mgtKey, filePath, null, displayName);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * "임시저장" 상태의 세금계산서에 첨부된 1개의 파일을 삭제합니다.
         * - 파일 식별을 위해 첨부 시 부여되는 'FileID'는 첨부파일 목록 확인(GetFiles API) 함수를 호출하여 확인합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/etc#DeleteFile
         */
        public IActionResult DeleteFile()
        {
            // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
            MgtKeyType mgtKeyType = MgtKeyType.SELL;

            // 세금계산서 문서번호
            string mgtKey = "20220527-002";

            // 팝빌이 첨부파일 관리를 위해 할당하는 식별번호
            // 첨부파일 목록 확인(getFiles API) 함수의 리턴 값 중 attachedFile 필드값 기재.
            string fileID = "F2A701E3-053B-40D8-AF28-FE1CBBE7FB53.PBF";

            try
            {
                var response = _taxinvoiceService.DeleteFile(corpNum, mgtKeyType, mgtKey, fileID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 세금계산서에 첨부된 파일목록을 확인합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/etc#GetFiles
         */
        public IActionResult GetFiles()
        {
            // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
            MgtKeyType mgtKeyType = MgtKeyType.SELL;

            // 세금계산서 문서번호
            string mgtKey = "20220527-002";

            try
            {
                var response = _taxinvoiceService.GetFiles(corpNum, mgtKeyType, mgtKey);
                return View("GetFiles", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 세금계산서와 관련된 안내 메일을 재전송 합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/etc#SendEmail
         */
        public IActionResult SendEmail()
        {
            // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
            MgtKeyType mgtKeyType = MgtKeyType.SELL;

            // 세금계산서 문서번호
            string mgtKey = "20220527-002";

            // 수신자 이메일주소
            string receiver = "";

            try
            {
                var response = _taxinvoiceService.SendEmail(corpNum, mgtKeyType, mgtKey, receiver);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 세금계산서와 관련된 안내 SMS(단문) 문자를 재전송하는 함수로, 팝빌 사이트 [문자·팩스] > [문자] > [전송내역] 메뉴에서 전송결과를 확인 할 수 있습니다.
         * - 메시지는 최대 90byte까지 입력 가능하고, 초과한 내용은 자동으로 삭제되어 전송합니다. (한글 최대 45자)
         * - 함수 호출시 포인트가 과금됩니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/etc#SendSMS
         */
        public IActionResult SendSMS()
        {
            // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
            MgtKeyType mgtKeyType = MgtKeyType.SELL;

            // 세금계산서 문서번호
            string mgtKey = "20220527-001";

            // 발신번호
            string sender = "";

            // 수신번호
            string receiver = "";

            // 문자메시지 내용, 90byte 초과시 길이가 조정되어 전송됨
            string contents = "알림문자 전송내용, 90byte 초과된 내용은 삭제되어 전송됨";

            try
            {
                var response =
                    _taxinvoiceService.SendSMS(corpNum, mgtKeyType, mgtKey, sender, receiver, contents);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 세금계산서를 팩스로 전송하는 함수로, 팝빌 사이트 [문자·팩스] > [팩스] > [전송내역] 메뉴에서 전송결과를 확인 할 수 있습니다.
         * - 함수 호출시 포인트가 과금됩니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/etc#SendFAX
         */
        public IActionResult SendFAX()
        {
            // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
            MgtKeyType mgtKeyType = MgtKeyType.SELL;

            // 세금계산서 문서번호
            string mgtKey = "20220527-001";

            // 발신번호
            string sender = "";

            // 수신번호
            string receiver = "";

            try
            {
                var response = _taxinvoiceService.SendFAX(corpNum, mgtKeyType, mgtKey, sender, receiver);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌 전자명세서 API를 통해 발행한 전자명세서를 세금계산서에 첨부합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/etc#AttachStatement
         */
        public IActionResult AttachStatement()
        {
            // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
            MgtKeyType mgtKeyType = MgtKeyType.SELL;

            // 세금계산서 문서번호
            string mgtKey = "20220527-001";

            // 첨부할 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
            int docItemCode = 121;

            // 첨부할 명세서 문서번호
            string docMgtKey = "20220527-002";

            try
            {
                var response = _taxinvoiceService.AttachStatement(corpNum, mgtKeyType, mgtKey, docItemCode, docMgtKey);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 세금계산서에 첨부된 전자명세서를 해제합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/etc#DetachStatement
         */
        public IActionResult DetachStatement()
        {
            // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
            MgtKeyType mgtKeyType = MgtKeyType.SELL;

            // 세금계산서 문서번호
            string mgtKey = "20220527-001";

            // 첨부해제할 명세서 코드 - 121(거래명세서), 122(청구서), 123(견적서), 124(발주서), 125(입금표), 126(영수증)
            int docItemCode = 121;

            // 첨부해제할 명세서 문서번호
            string docMgtKey = "20220527-002";

            try
            {
                var response = _taxinvoiceService.DetachStatement(corpNum, mgtKeyType, mgtKey, docItemCode, docMgtKey);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌 사이트를 통해 발행하였지만 문서번호가 존재하지 않는 세금계산서에 문서번호를 할당합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/etc#AssignMgtKey
         */
        public IActionResult AssignMgtKey()
        {
            // 세금계산서유형, SELL(매출), BUY(매입), TRUSTEE(위수탁)
            MgtKeyType mgtKeyType = MgtKeyType.SELL;

            // 세금계산서 아이템키, 목록조회(Search) API의 반환항목중 ItemKey 참조
            string itemKey = "018103016112000001";

            // 세금계산서에 할당할 문서번호
            string mgtKey = "20220527-100";

            try
            {
                var response = _taxinvoiceService.AssignMgtKey(corpNum, mgtKeyType, itemKey, mgtKey);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 세금계산서 관련 메일 항목에 대한 발송설정을 확인합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/etc#ListEmailConfig
         */
        public IActionResult ListEmailConfig()
        {
            try
            {
                var response = _taxinvoiceService.ListEmailConfig(corpNum);
                return View("ListEmailConfig", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /* 세금계산서 관련 메일 항목에 대한 발송설정을 수정합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/etc#UpdateEmailConfig
         *
         * 메일전송유형
         * [정발행]
         * TAX_ISSUE : 공급받는자에게 전자세금계산서가 발행 되었음을 알려주는 메일입니다.
         * TAX_ISSUE_INVOICER : 공급자에게 전자세금계산서가 발행 되었음을 알려주는 메일입니다.
         * TAX_CHECK : 공급자에게 전자세금계산서가 수신확인 되었음을 알려주는 메일입니다.
         * TAX_CANCEL_ISSUE : 공급받는자에게 전자세금계산서가 발행취소 되었음을 알려주는 메일입니다.
         *
         * [역발행]
         * TAX_REQUEST : 공급자에게 세금계산서를 전자서명 하여 발행을 요청하는 메일입니다.
         * TAX_CANCEL_REQUEST : 공급받는자에게 세금계산서가 취소 되었음을 알려주는 메일입니다.
         * TAX_REFUSE : 공급받는자에게 세금계산서가 거부 되었음을 알려주는 메일입니다.
         *
         * [위수탁발행]
         * TAX_TRUST_ISSUE : 공급받는자에게 전자세금계산서가 발행 되었음을 알려주는 메일입니다.
         * TAX_TRUST_ISSUE_TRUSTEE : 수탁자에게 전자세금계산서가 발행 되었음을 알려주는 메일입니다.
         * TAX_TRUST_ISSUE_INVOICER : 공급자에게 전자세금계산서가 발행 되었음을 알려주는 메일입니다.
         * TAX_TRUST_CANCEL_ISSUE : 공급받는자에게 전자세금계산서가 발행취소 되었음을 알려주는 메일입니다.
         * TAX_TRUST_CANCEL_ISSUE_INVOICER : 공급자에게 전자세금계산서가 발행취소 되었음을 알려주는 메일입니다.
         *
         * [처리결과]
         * TAX_CLOSEDOWN : 거래처의 휴폐업 여부를 확인하여 안내하는 메일입니다.
         * TAX_NTSFAIL_INVOICER : 전자세금계산서 국세청 전송실패를 안내하는 메일입니다.
         *
         * [정기발송]
         * ETC_CERT_EXPIRATION : 팝빌에서 이용중인 공인인증서의 갱신을 안내하는 메일입니다.
         */
        public IActionResult UpdateEmailConfig()
        {
            //메일전송유형
            string emailType = "TAX_ISSUE";

            //전송여부 (true-전송, false-미전송)
            bool sendYN = true;

            try
            {
                var response = _taxinvoiceService.UpdateEmailConfig(corpNum, emailType, sendYN);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 국세청 전송 옵션 설정 상태를 확인합니다.
         * - 팝빌 국세청 전송 정책 [https://developers.popbill.com/guide/taxinvoice/dotnetcore/introduction/policy-of-send-to-nts]
         * - 국세청 전송 옵션 설정은 팝빌 사이트 [전자세금계산서] > [환경설정] > [세금계산서 관리] 메뉴에서 설정할 수 있으며, API로 설정은 불가능 합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/etc#GetSendToNTSConfig
         */
        public IActionResult GetSendToNTSConfig()
        {

            try
            {
                bool config = _taxinvoiceService.GetSendToNTSConfig(corpNum);
                return View("result", config ? "발행 즉시 전송" : "익일 자동 전송");
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        #endregion

        #region 인증서 관리

        /*
         * 전자세금계산서 발행에 필요한 인증서를 팝빌 인증서버에 등록하기 위한 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - 인증서 갱신/재발급/비밀번호 변경한 경우, 변경된 인증서를 팝빌 인증서버에 재등록 해야합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/cert#GetTaxCertURL
         */
        public IActionResult GetTaxCertURL()
        {
            try
            {
                var result = _taxinvoiceService.GetTaxCertURL(corpNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌 인증서버에 등록된 인증서의 만료일을 확인합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/cert#GetCertificateExpireDate
         */
        public IActionResult GetCertificateExpireDate()
        {
            try
            {
                var result = _taxinvoiceService.GetCertificateExpireDate(corpNum);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌 인증서버에 등록된 인증서의 유효성을 확인합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/cert#CheckCertValidation
         */
        public IActionResult CheckCertValidation()
        {
            try
            {
                var response = _taxinvoiceService.CheckCertValidation(corpNum);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌 인증서버에 등록된 공동인증서의 정보를 확인합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/cert#GetTaxCertInfo
         */
        public IActionResult GetTaxCertInfo()
        {
            try
            {
                var response = _taxinvoiceService.GetTaxCertInfo(corpNum);
                return View("GetTaxCertInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        #endregion

        #region 포인트 관리

        /*
         * 연동회원의 잔여포인트를 확인합니다.
         * - 과금방식이 파트너과금인 경우 파트너 잔여포인트 확인(GetPartnerBalance API) 함수를 통해 확인하시기 바랍니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/point#GetBalance
         */
        public IActionResult GetBalance()
        {
            try
            {
                var result = _taxinvoiceService.GetBalance(corpNum);
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
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/point#GetChargeURL
         */
        public IActionResult GetChargeURL()
        {
            try
            {
                var result = _taxinvoiceService.GetChargeURL(corpNum, userID);
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
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/point#GetPaymentURL
         */
        public IActionResult GetPaymentURL()
        {

            try
            {
                var result = _taxinvoiceService.GetPaymentURL(corpNum, userID);
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
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/point#GetUseHistoryURL
         */
        public IActionResult GetUseHistoryURL()
        {

            try
            {
                var result = _taxinvoiceService.GetUseHistoryURL(corpNum, userID);
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
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/point#GetPartnerBalance
         */
        public IActionResult GetPartnerBalance()
        {
            try
            {
                var result = _taxinvoiceService.GetPartnerBalance(corpNum);
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
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/point#GetPartnerURL
         */
        public IActionResult GetPartnerURL()
        {
            // CHRG 포인트충전 URL
            string TOGO = "CHRG";

            try
            {
                var result = _taxinvoiceService.GetPartnerURL(corpNum, TOGO);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 전자세금계산서 발행단가를 확인합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/point#GetUnitCost
         */
        public IActionResult GetUnitCost()
        {
            try
            {
                var result = _taxinvoiceService.GetUnitCost(corpNum);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌 전자세금계산서 API 서비스 과금정보를 확인합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/point#GetChargeInfo
         */
        public IActionResult GetChargeInfo()
        {
            try
            {
                var response = _taxinvoiceService.GetChargeInfo(corpNum);
                return View("GetChargeInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        /*
         * 연동회원 포인트를 환불 신청합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/point#Refund
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
                refundForm.requestPoint = "1";

                // 은행명
                refundForm.accountBank ="국민";

                // 계좌번호
                refundForm.accountNum ="123123123-123" ;

                // 예금주명
                refundForm.accountName = "예금주명";

                // 환불사유
                refundForm.reason = "환불사유";

                var response = _taxinvoiceService.Refund(corpNum, refundForm);
                return View("RefundResponse", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 포인트를 환불 신청합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/point#PaymentRequest
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

                var response = _taxinvoiceService.PaymentRequest(corpNum, paymentForm);
                return View("PaymentResponse", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 포인트 무통장 입금신청내역 1건을 확인합니다.
         *  - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/point#GetSettleResult
         */
        public IActionResult GetSettleResult()
        {
            // 정산코드
            var settleCode = "202402260000000025";

            try
            {
                var paymentHistory = _taxinvoiceService.GetSettleResult(corpNum, settleCode);

                return View("PaymentHistory", paymentHistory);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }
        
        /*
         * 연동회원의 포인트 결제내역을 확인합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/point#GetPaymentHistory
         */
        public IActionResult GetPaymentHistory()
        {
            // 조회 기간의 시작일자 (형식 : yyyyMMdd)
            var SDate = "20240202";

            // 조회 기간의 종료일자 (형식 : yyyyMMdd)
            var EDate = "20240301";

            // 목록 페이지번호 (기본값 1)
            var Page = 1;

            // 페이지당 표시할 목록 개수 (기본값 500, 최대 1,000)
            var PerPage = 100;

            try
            {
                var paymentHistoryResult = _taxinvoiceService.GetPaymentHistory(corpNum, SDate, EDate,
                    Page, PerPage);

                return View("PaymentHistoryResult", paymentHistoryResult);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 포인트 사용내역을 확인합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/point#GetUseHistory
         */
        public IActionResult GetUseHistory()
        {
            // 조회 기간의 시작일자 (형식 : yyyyMMdd)
            var SDate = "20240102";

            // 조회 기간의 종료일자 (형식 : yyyyMMdd)
            var EDate = "20240131";

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
                var useHistoryResult = _taxinvoiceService.GetUseHistory(corpNum, SDate, EDate, Page,
                    PerPage, Order);

                return View("UseHistoryResult", useHistoryResult);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 포인트 환불신청내역을 확인합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/point#GetRefundHistory
         */
        public IActionResult GetRefundHistory()
        {
            // 목록 페이지번호 (기본값 1)
            var Page = 1;

            // 페이지당 표시할 목록 개수 (기본값 500, 최대 1,000)
            var PerPage = 100;

            try
            {
                var refundHistoryResult = _taxinvoiceService.GetRefundHistory(corpNum, Page, PerPage);

                return View("RefundHistoryResult", refundHistoryResult);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        /*
         * 포인트 환불에 대한 상세정보 1건을 확인합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/point#GetRefundInfo
         */
        public IActionResult GetRefundInfo()
        {
            // 환불 코드
            var refundCode = "023040000017";

            try
            {
                var response = _taxinvoiceService.GetRefundInfo(corpNum, refundCode);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        /*
         * 환불 가능한 포인트를 확인합니다. (보너스 포인트는 환불가능포인트에서 제외됩니다.)
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/point#GetRefundableBalance
         */
        public IActionResult GetRefundableBalance()
        {
            try
            {
                var refundableBalance = _taxinvoiceService.GetRefundableBalance(corpNum);
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
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/member#CheckIsMember
         */
        public IActionResult CheckIsMember()
        {
            try
            {
                //링크아이디
                string linkID = "TESTER";

                var response = _taxinvoiceService.CheckIsMember(corpNum, linkID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 사용하고자 하는 아이디의 중복여부를 확인합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/member#CheckID
         */
        public IActionResult CheckID()
        {
            //중복여부 확인할 팝빌 회원 아이디
            string checkID = "testkorea";

            try
            {
                var response = _taxinvoiceService.CheckID(checkID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 사용자를 연동회원으로 가입처리합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/member#JoinMember
         */
        public IActionResult JoinMember()
        {
            JoinForm joinInfo = new JoinForm();

            // 링크아이디
            joinInfo.LinkID = "TESTER";

            // 아이디, 6자이상 50자 미만
            joinInfo.ID = "userid";

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
                var response = _taxinvoiceService.JoinMember(joinInfo);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 회사정보를 확인합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/member#GetCorpInfo
         */
        public IActionResult GetCorpInfo()
        {
            try
            {
                var response = _taxinvoiceService.GetCorpInfo(corpNum);
                return View("GetCorpInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 회사정보를 수정합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/member#UpdateCorpInfo
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
                var response = _taxinvoiceService.UpdateCorpInfo(corpNum, corpInfo);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 담당자(팝빌 로그인 계정)를 추가합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/member#RegistContact
         */
        public IActionResult RegistContact()
        {
            Contact contactInfo = new Contact();

            // 담당자 아이디, 6자 이상 50자 미만
            contactInfo.id = "testkorea_20210165";

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
                var response = _taxinvoiceService.RegistContact(corpNum, contactInfo);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 정보을 확인합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/member#GetContactInfo
         */
        public IActionResult GetContactInfo()
        {
            // 확인할 담당자 아이디
            string contactID = "test0730";

            try
            {
                var contactInfo = _taxinvoiceService.GetContactInfo(corpNum, contactID);
                return View("GetContactInfo", contactInfo);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 목록을 확인합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/member#ListContact
         */
        public IActionResult ListContact()
        {
            try
            {
                var response = _taxinvoiceService.ListContact(corpNum);
                return View("ListContact", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 정보를 수정합니다.
         * - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/member#UpdateContact
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
                var response = _taxinvoiceService.UpdateContact(corpNum, contactInfo);
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
         *  - https://developers.popbill.com/reference/taxinvoice/dotnetcore/api/member#QuitMember
         */
        public IActionResult QuitMember()
        {
            // 탈퇴 사유
            var quitReason = "탈퇴사유";

            try
            {
                var response = _taxinvoiceService.QuitMember(corpNum, quitReason);
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
