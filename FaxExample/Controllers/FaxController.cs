using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Popbill;
using Popbill.Fax;

namespace FaxExample.Controllers
{
    public class FaxController : Controller
    {
        private readonly FaxService _faxService;

        public FaxController(FaxInstance FAXinstance)
        {
            // 팩스 서비스 객체 생성
            _faxService = FAXinstance.faxService;
        }

        // 팝빌 연동회원 사업자번호 (하이픈 '-' 없이 10자리)
        string corpNum = "1234567890";

        // 팝빌 연동회원 아이디
        string userID = "testkorea";

        /*
         * 팩스 Index page (Fax/Index.cshtml)
         */
        public IActionResult Index()
        {
            return View();
        }

        #region 발신번호 사전등록

        /*
         * 팩스 발신번호 등록여부를 확인합니다.
         * - 발신번호 상태가 '승인'인 경우에만 리턴값 'Response'의 변수 'code'가 1로 반환됩니다.
         * - https://docs.popbill.com/fax/dotnetcore/api#CheckSenderNumber
         */
        public IActionResult CheckSenderNumber()
        {
            // 확인할 발신번호
            string senderNumber = "";

            try
            {
                var Response = _faxService.CheckSenderNumber(corpNum, senderNumber);
                return View("Response", Response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 발신번호를 등록하고 내역을 확인하는 팩스 발신번호 관리 페이지 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://docs.popbill.com/fax/dotnetcore/api#GetSenderNumberMgtURL
         */
        public IActionResult GetSenderNumberMgtURL()
        {
            try
            {
                var result = _faxService.GetSenderNumberMgtURL(corpNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌에 등록한 연동회원의 팩스 발신번호 목록을 확인합니다.
         * - https://docs.popbill.com/fax/dotnetcore/api#GetSenderNumberList
         */
        public IActionResult GetSenderNumberList()
        {
            try
            {
                var response = _faxService.GetSenderNumberList(corpNum);
                return View("GetSenderNumberList", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        #endregion

        #region 팩스 전송

        /*
         * 팩스 1건을 전송합니다. (최대 전송파일 개수: 20개)
         * - https://docs.popbill.com/fax/dotnetcore/api#SendFAX
         */
        public IActionResult SendFAX()
        {
            // 발신번호
            string senderNum = "";

            // 발신자명
            string senderName = "발신자명";

            // 수신번호
            string receiverNum = "";

            // 수신자명
            string receiverName = "수신자명";

            // 팩스전송 파일경로, 전송파일 최대 20개
            List<string> filePath = new List<string>();
            filePath.Add("C:\\popbill.example.dotnetcore\\FaxExample\\wwwroot\\images\\tax_image.png");
            filePath.Add("C:\\popbill.example.dotnetcore\\FaxExample\\wwwroot\\images\\tax_image.png");

            // 팩스제목
            string title = "팩스 제목";

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

            // 광고팩스 전송여부 , true / false 중 택 1
            // └ true = 광고 , false = 일반
            // └ 미입력 시 기본값 false 처리
            bool adsYN = false;

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
            string requestNum = "";

            try
            {
                var receiptNum = _faxService.SendFAX(corpNum, senderNum, senderName, receiverNum, receiverName,
                    filePath, title, sndDT, adsYN, requestNum);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 동일한 팩스파일을 다수의 수신자에게 전송하기 위해 팝빌에 접수합니다. (최대 전송파일 개수 : 20개) (최대 1,000건)
         * - https://docs.popbill.com/fax/dotnetcore/api#SendFAX_Same
         */
        public IActionResult SendFAX_Multi()
        {
            // 발신번호
            string senderNum = "";

            // 발신자명
            string senderName = "발신자명";

            // 수신자정보 배열 (최대 1000건)
            List<FaxReceiver> receivers = new List<FaxReceiver>();

            for (int i = 0; i < 10; i++)
            {
                FaxReceiver receiver = new FaxReceiver();

                // 수신번호
                receiver.receiveNum = "";

                // 수신자명
                receiver.receiveName = "수신자명칭_" + i;

                // 파트너 지정키, 대량전송시, 수신자 구별용 메모
                receiver.interOPRefKey = "2022527-" + i;

                receivers.Add(receiver);
            }

            // 팩스전송 파일경로, 전송파일 최대 20개
            List<string> filePath = new List<string>();
            filePath.Add("C:\\popbill.example.dotnetcore\\FaxExample\\wwwroot\\images\\tax_image.png");
            filePath.Add("C:\\popbill.example.dotnetcore\\FaxExample\\wwwroot\\images\\tax_image.png");

            // 팩스제목
            string title = "팩스 제목";

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

            // 광고팩스 전송여부 , true / false 중 택 1
            // └ true = 광고 , false = 일반
            // └ 미입력 시 기본값 false 처리
            bool adsYN = false;

            // 전송요청번호
            // 팝빌이 접수 단위를 식별할 수 있도록 파트너가 부여하는 식별번호.
            // 1~36자리로 구성. 영문, 숫자, 하이픈(-), 언더바(_)를 조합하여 팝빌 회원별로 중복되지 않도록 할당.
            string requestNum = "";

            try
            {
                var receiptNum = _faxService.SendFAX(corpNum, senderNum, senderName, receivers, filePath, title, sndDT,
                    adsYN, requestNum);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        /*
         * 팝빌에서 반환받은 접수번호를 통해 팩스 1건을 재전송합니다.
         * - 발신/수신 정보 미입력시 기존과 동일한 정보로 팩스가 전송되고, 접수일 기준 최대 60일이 경과되지 않는 건만 재전송이 가능합니다.
         * - 팩스 재전송 요청시 포인트가 차감됩니다. (전송실패시 환불처리)
         * - 변환실패 사유로 전송실패한 팩스 접수건은 재전송이 불가합니다.
         * - https://docs.popbill.com/fax/dotnetcore/api#ResendFAX
         */
        public IActionResult ResendFAX()
        {
            // 팩스전송 요청시 발급받은 접수번호
            string orgReceiptNum = "018120517165400001";

            // 발신번호, 공백으로 처리시 기존전송정보로 전송
            string senderNum = "";

            // 발신자명, 공백으로 처리시 기존전송정보로 전송
            string senderName = "";

            // 수신번호, 공백으로 처리시 기존전송정보로 전송
            string receiverNum = "";

            // 수신자명, 공백으로 처리시 기존전송정보로 전송
            string receiverName = "";

            // 팩스제목
            string title = "팩스 제목";

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

            // 재전송 팩스의 전송요청번호
            // 팝빌이 접수 단위를 식별할 수 있도록 파트너가 부여하는 식별번호.
            // 1~36자리로 구성. 영문, 숫자, 하이픈(-), 언더바(_)를 조합하여 팝빌 회원별로 중복되지 않도록 할당.
            string requestNum = "";

            try
            {
                var receiptNum = _faxService.ResendFAX(corpNum, orgReceiptNum, senderNum, senderName, receiverNum,
                    receiverName, title, sndDT, requestNum);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌에서 반환받은 접수번호를 통해 다수건의 팩스를 재전송합니다. (최대 전송파일 개수: 20개) (최대 1,000건)
         * - 발신/수신 정보 미입력시 기존과 동일한 정보로 팩스가 전송되고, 접수일 기준 최대 60일이 경과되지 않는 건만 재전송이 가능합니다.
         * - 팩스 재전송 요청시 포인트가 차감됩니다. (전송실패시 환불처리)
         * - 변환실패 사유로 전송실패한 팩스 접수건은 재전송이 불가합니다.
         * - https://docs.popbill.com/fax/dotnetcore/api#ResendFAX_Same
         */
        public IActionResult ResendFAX_Multi()
        {
            // 팩스전송 요청시 발급받은 접수번호
            string orgReceiptNum = "018120517165400001";

            // 발신번호, 공백으로 처리시 기존전송정보로 전송
            string senderNum = "";

            // 발신자명, 공백으로 처리시 기존전송정보로 전송
            string senderName = "";

            // 수신자정보 배열 (최대 1000건)
            List<FaxReceiver> receivers = new List<FaxReceiver>();

            for (int i = 0; i < 10; i++)
            {
                FaxReceiver receiver = new FaxReceiver();

                // 수신번호
                receiver.receiveNum = "";

                // 수신자명
                receiver.receiveName = "수신자명칭_" + i;

                // 파트너 지정키, 대량전송시, 수신자 구별용 메모
                receiver.interOPRefKey = "2022527-" + i;

                receivers.Add(receiver);
            }

            // 수신자정보를 변경하지 않고 기존 전송정보로 전송하는 경우
            // List<FaxReceiver> receivers = null;

            // 팩스제목
            string title = "팩스 제목";

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;


            // 재전송 팩스의 전송요청번호
            // 팝빌이 접수 단위를 식별할 수 있도록 파트너가 부여하는 식별번호.
            // 1~36자리로 구성. 영문, 숫자, 하이픈(-), 언더바(_)를 조합하여 팝빌 회원별로 중복되지 않도록 할당.
            string requestNum = "";

            try
            {
                var receiptNum = _faxService.ResendFAX(corpNum, orgReceiptNum, senderNum, senderName, receivers, title,
                    sndDT, requestNum);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        /*
         * 파트너가 할당한 전송요청 번호를 통해 팩스 1건을 재전송합니다.
         * - 발신/수신 정보 미입력시 기존과 동일한 정보로 팩스가 전송되고, 접수일 기준 최대 60일이 경과되지 않는 건만 재전송이 가능합니다.
         * - 팩스 재전송 요청시 포인트가 차감됩니다. (전송실패시 환불처리)
         * - 변환실패 사유로 전송실패한 팩스 접수건은 재전송이 불가합니다.
         * - https://docs.popbill.com/fax/dotnetcore/api#ResendFAXRN
         */
        public IActionResult ResendFAXRN()
        {
            // 팩스전송 요청시 할당한 전송요청번호
            string preRequestNum = "";

            // 발신번호, 공백으로 처리시 기존전송정보로 전송
            string senderNum = "";

            // 발신자명, 공백으로 처리시 기존전송정보로 전송
            string senderName = "";

            // 수신번호, 공백으로 처리시 기존전송정보로 전송
            string receiverNum = "";

            // 수신자명, 공백으로 처리시 기존전송정보로 전송
            string receiverName = "";

            // 팩스제목
            string title = "팩스 제목";

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

            // 재전송 팩스의 전송요청번호
            // 팝빌이 접수 단위를 식별할 수 있도록 파트너가 부여하는 식별번호.
            // 1~36자리로 구성. 영문, 숫자, 하이픈(-), 언더바(_)를 조합하여 팝빌 회원별로 중복되지 않도록 할당.
            string requestNum = "";

            try
            {
                var receiptNum = _faxService.ResendFAXRN(corpNum, preRequestNum, senderNum, senderName, receiverNum,
                    receiverName, title, sndDT, requestNum);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 파트너가 할당한 전송요청 번호를 통해 다수건의 팩스를 재전송합니다. (최대 전송파일 개수: 20개) (최대 1,000건)
         * - 발신/수신 정보 미입력시 기존과 동일한 정보로 팩스가 전송되고, 접수일 기준 최대 60일이 경과되지 않는 건만 재전송이 가능합니다.
         * - 팩스 재전송 요청시 포인트가 차감됩니다. (전송실패시 환불처리)
         * - 변환실패 사유로 전송실패한 팩스 접수건은 재전송이 불가합니다.
         * - https://docs.popbill.com/fax/dotnetcore/api#ResendFAXRN_Same
         */
        public IActionResult ResendFAXRN_multi()
        {
            // 팩스전송 요청시 할당한 요청번호
            string preRequestNum = "";

            // 발신번호, 공백으로 처리시 기존전송정보로 전송
            string senderNum = "";

            // 발신자명, 공백으로 처리시 기존전송정보로 전송
            string senderName = "";

            // 수신자정보 배열 (최대 1000건)
            List<FaxReceiver> receivers = new List<FaxReceiver>();

            for (int i = 0; i < 10; i++)
            {
                FaxReceiver receiver = new FaxReceiver();

                // 수신번호
                receiver.receiveNum = "";

                // 수신자명
                receiver.receiveName = "수신자명칭_" + i;

                // 파트너 지정키, 대량전송시, 수신자 구별용 메모
                receiver.interOPRefKey = "2022527-" + i;

                receivers.Add(receiver);
            }

            // 수신자정보를 변경하지 않고 기존 전송정보로 전송하는 경우
            // List<FaxReceiver> receivers = null;

            // 팩스제목
            string title = "팩스 제목";

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

            // 재전송 팩스의 전송요청번호
            // 팝빌이 접수 단위를 식별할 수 있도록 파트너가 부여하는 식별번호.
            // 1~36자리로 구성. 영문, 숫자, 하이픈(-), 언더바(_)를 조합하여 팝빌 회원별로 중복되지 않도록 할당.
            string requestNum = "";

            try
            {
                var receiptNum = _faxService.ResendFAXRN(corpNum, preRequestNum, senderNum, senderName, receivers,
                    title, sndDT, requestNum);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌에서 반환받은 접수번호를 통해 예약접수된 팩스 전송을 취소합니다. (예약시간 10분 전까지 가능)
         * - https://docs.popbill.com/fax/dotnetcore/api#CancelReserve
         */
        public IActionResult CancelReserve()
        {
            // 팩스전송 요청시 발급받은 접수번호
            string receiptNum = "018120517184000001";

            try
            {
                var Response = _faxService.CancelReserve(corpNum, receiptNum);
                return View("Response", Response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 파트너가 할당한 전송요청 번호를 통해 예약접수된 팩스 전송을 취소합니다. (예약시간 10분 전까지 가능)
         * - https://docs.popbill.com/fax/dotnetcore/api#CancelReserveRN
         */
        public IActionResult CancelReserveRN()
        {
            // 팩스 전송요청시 할당한 전송요청번호
            string requestNum = "";

            try
            {
                var Response = _faxService.CancelReserveRN(corpNum, requestNum);
                return View("Response", Response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        #endregion

        #region 정보조회

        /*
         * 팝빌에서 반환 받은 접수번호를 통해 팩스 전송상태 및 결과를 확인합니다.
         * - https://docs.popbill.com/fax/dotnetcore/api#GetFaxDetail
         */
        public IActionResult GetFaxDetail()
        {
            // 팩스전송 요청시 발급받은 접수번호
            string receiptNum = "018112714511700001";

            try
            {
                var Response = _faxService.GetFaxDetail(corpNum, receiptNum);
                return View("GetFaxDetail", Response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 파트너가 할당한 전송요청 번호를 통해 팩스 전송상태 및 결과를 확인합니다.
         * - https://docs.popbill.com/fax/dotnetcore/api#GetFaxDetailRN
         */
        public IActionResult GetFaxDetailRN()
        {
            // 팩스 전송요청시 할당한 전송요청번호
            string requestNum = "";

            try
            {
                var Response = _faxService.GetFaxDetailRN(corpNum, requestNum);
                return View("GetFaxDetailRN", Response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 검색조건에 해당하는 팩스 전송내역 목록을 조회합니다. (조회기간 단위 : 최대 2개월)
         * - 팩스 접수일시로부터 2개월 이내 접수건만 조회할 수 있습니다.
         * - https://docs.popbill.com/fax/dotnetcore/api#Search
         */
        public IActionResult Search()
        {
            // 최대 검색기간 : 6개월 이내
            // 시작일자, 날짜형식(yyyyMMdd)
            string SDate = "20211201";

            // 종료일자, 날짜형식(yyyyMMdd)
            string EDate = "20211230";

            // 전송상태 배열 ("1" , "2" , "3" , "4" 중 선택, 다중 선택 가능)
            // └ 1 = 대기 , 2 = 성공 , 3 = 실패 , 4 = 취소
            // - 미입력 시 전체조회
            string[] State = new string[4];
            State[0] = "1";
            State[1] = "2";
            State[2] = "3";
            State[3] = "4";

            // 예약여부 (false , true 중 택 1)
            // └ false = 전체조회, true = 예약전송건 조회
            // - 미입력시 기본값 false 처리
            bool ReserveYN = false;

            // 개인조회 여부 (false , true 중 택 1)
            // false = 접수한 팩스 전체 조회 (관리자권한)
            // true = 해당 담당자 계정으로 접수한 팩스만 조회 (개인권한)
            // 미입력시 기본값 false 처리
            bool SenderOnly = false;

            // 페이지 번호, 기본값 '1'
            int Page = 1;

            // 페이지당 검색개수, 기본값 '500', 최대 '1000'
            int PerPage = 30;

            // 정렬방향, D-내림차순, A-오름차순
            string Order = "D";

            // 조회하고자 하는 발신자명 또는 수신자명
            // - 미입력시 전체조회
            string QString = "";

            try
            {
                var response = _faxService.Search(corpNum, SDate, EDate, State, ReserveYN, SenderOnly, Page, PerPage,
                    Order, QString);
                return View("Search", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌 사이트와 동일한 팩스 전송내역 확인 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://docs.popbill.com/fax/dotnetcore/api#GetSentListURL
         */
        public IActionResult GetSentListURL()
        {
            try
            {
                var result = _faxService.GetSentListURL(corpNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팩스 미리보기 팝업 URL을 반환하며, 팩스전송을 위한 TIF 포맷 변환 완료 후 호출 할 수 있습니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://docs.popbill.com/fax/dotnetcore/api#GetPreviewURL
         */
        public IActionResult GetPreviewURL()
        {
            // 팩스전송 요청시 발급받은 접수번호
            string receiptNum = "018092922570100001";

            try
            {
                var result = _faxService.GetPreviewURL(corpNum, receiptNum, userID);
                return View("Result", result);
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
         * - https://docs.popbill.com/fax/dotnetcore/api#GetBalance
         */
        public IActionResult GetBalance()
        {
            try
            {
                var result = _faxService.GetBalance(corpNum);
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
         * - https://docs.popbill.com/fax/dotnetcore/api#GetChargeURL
         */
        public IActionResult GetChargeURL()
        {
            try
            {
                var result = _faxService.GetChargeURL(corpNum, userID);
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
         * - https://docs.popbill.com/fax/dotnetcore/api#GetPaymentURL
         */
        public IActionResult GetPaymentURL()
        {

            try
            {
                var result = _faxService.GetPaymentURL(corpNum, userID);
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
         * - https://docs.popbill.com/fax/dotnetcore/api#GetUseHistoryURL
         */
        public IActionResult GetUseHistoryURL()
        {

            try
            {
                var result = _faxService.GetUseHistoryURL(corpNum, userID);
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
         * - https://docs.popbill.com/fax/dotnetcore/api#GetPartnerBalance
         */
        public IActionResult GetPartnerBalance()
        {
            try
            {
                var result = _faxService.GetPartnerBalance(corpNum);
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
         * - https://docs.popbill.com/fax/dotnetcore/api#GetPartnerURL
         */
        public IActionResult GetPartnerURL()
        {
            // CHRG 포인트충전 URL
            string TOGO = "CHRG";

            try
            {
                var result = _faxService.GetPartnerURL(corpNum, TOGO);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팩스 전송시 과금되는 포인트 단가를 확인합니다.
         * - https://docs.popbill.com/fax/dotnetcore/api#GetUnitCost
         */
        public IActionResult GetUnitCost()
        {
            try
            {
                // 수신번호 유형, 일반 / 지능 중 택 1
                string receiveNumType = "일반";

                var result = _faxService.GetUnitCost(corpNum, receiveNumType);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌 팩스 API 서비스 과금정보를 확인합니다.
         * - https://docs.popbill.com/fax/dotnetcore/api#GetChargeInfo
         */
        public IActionResult GetChargeInfo()
        {
            try
            {
                // 수신번호 유형, 일반 / 지능 중 택 1
                string receiveNumType = "일반";

                var response = _faxService.GetChargeInfo(corpNum, receiveNumType);
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
         * - https://docs.popbill.com/fax/dotnetcore/api#CheckIsMember
         */
        public IActionResult CheckIsMember()
        {
            try
            {
                //링크아이디
                string linkID = "TESTER";

                var response = _faxService.CheckIsMember(corpNum, linkID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 사용하고자 하는 아이디의 중복여부를 확인합니다.
         * - https://docs.popbill.com/fax/dotnetcore/api#CheckID
         */
        public IActionResult CheckID()
        {
            //중복여부 확인할 팝빌 회원 아이디
            string checkID = "testkorea";

            try
            {
                var response = _faxService.CheckID(checkID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 사용자를 연동회원으로 가입처리합니다.
         * - https://docs.popbill.com/fax/dotnetcore/api#JoinMember
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
                var response = _faxService.JoinMember(joinInfo);
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
         * - https://docs.popbill.com/fax/dotnetcore/api#GetAccessURL
         */
        public IActionResult GetAccessURL()
        {
            try
            {
                var result = _faxService.GetAccessURL(corpNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 회사정보를 확인합니다.
         * - https://docs.popbill.com/fax/dotnetcore/api#GetCorpInfo
         */
        public IActionResult GetCorpInfo()
        {
            try
            {
                var response = _faxService.GetCorpInfo(corpNum);
                return View("GetCorpInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 회사정보를 수정합니다.
         * - https://docs.popbill.com/fax/dotnetcore/api#UpdateCorpInfo
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
                var response = _faxService.UpdateCorpInfo(corpNum, corpInfo);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 담당자(팝빌 로그인 계정)를 추가합니다.
         * - https://docs.popbill.com/fax/dotnetcore/api#RegistContact
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
                var response = _faxService.RegistContact(corpNum, contactInfo);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 정보을 확인합니다.
         * - https://docs.popbill.com/fax/dotnetcore/api#GetContactInfo
         */
        public IActionResult GetContactInfo()
        {
            // 확인할 담당자 아이디
            string contactID = "test0730";

            try
            {
                var contactInfo = _faxService.GetContactInfo(corpNum, contactID);
                return View("GetContactInfo", contactInfo);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 목록을 확인합니다.
         * - https://docs.popbill.com/fax/dotnetcore/api#ListContact
         */
        public IActionResult ListContact()
        {
            try
            {
                var response = _faxService.ListContact(corpNum);
                return View("ListContact", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 정보를 수정합니다.
         * - https://docs.popbill.com/fax/dotnetcore/api#UpdateContact
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
                var response = _faxService.UpdateContact(corpNum, contactInfo);
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
