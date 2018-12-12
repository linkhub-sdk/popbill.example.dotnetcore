using System;
using System.Collections.Generic;
using ControllerDI.Services;
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
            //팩스 서비스 객체 생성
            _faxService = FAXinstance.faxService;

            //연동환경 설정값, 개발용(true), 상업용(false)
            _faxService.IsTest = true;
        }

        //팝빌 연동회원 사업자번호 (하이픈 '-' 없이 10자리)
        string corpNum = "1234567890";

        //팝빌 연동회원 아이디
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
         * 팩스 발신번호 관리 팝업 URL을 반합니다.
         * - 반환된 URL은 보안정책에 따라 30초의 유효시간을 갖습니다.
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
         * 팩스 발신번호 목록을 반환합니다.
         */
        public IActionResult GetSenderNumberList()
        {
            try
            {
                var response = _faxService.GetSenderNumberList(corpNum, userID);
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
         * 팩스를 전송합니다. (전송할 파일 개수는 최대 20개까지 가능)
         * - 팩스전송 문서 파일포맷 안내 : http://blog.linkhub.co.kr/2561
         */
        public IActionResult SendFAX()
        {
            // 발신번호 
            string senderNum = "07043042992";

            // 발신자명
            string senderName = "발신자명";

            // 수신번호
            string receiverNum = "010111222";

            // 수신자명 
            string receiverName = "수신자명";

            // 팩스전송 파일경로, 전송파일 최대 20개 
            List<string> filePath = new List<string>();
            filePath.Add("C:\\popbill.example.dotnetcore\\FaxExample\\wwwroot\\images\\tax_image.png");
            filePath.Add("C:\\popbill.example.dotnetcore\\FaxExample\\wwwroot\\images\\tax_image.png");

            // 팩스제목
            string title = "팩스 제목";

            // 예약전송일시(yyyyMMddHHmmss) ex) 20181126121206, null인 경우 즉시전송
            DateTime? sndDT = null;

            // 광고여부 (기본값 false)
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
         * [대량전송] 팩스를 전송합니다. (전송할 파일 개수는 최대 20개까지 가능)
         * - 팩스전송 문서 파일포맷 안내 : http://blog.linkhub.co.kr/2561
         */
        public IActionResult SendFAX_Multi()
        {
            // 발신번호 
            string senderNum = "07043042992";

            // 발신자명
            string senderName = "발신자명";

            // 수신자정보 배열 (최대 1000건)
            List<FaxReceiver> receivers = new List<FaxReceiver>();

            for (int i = 0; i < 10; i++)
            {
                FaxReceiver receiver = new FaxReceiver();

                // 수신번호
                receiver.receiveNum = "111-2222-3333";

                // 수신자명
                receiver.receiveName = "수신자명칭_" + i;
                receivers.Add(receiver);
            }

            // 팩스전송 파일경로, 전송파일 최대 20개 
            List<string> filePath = new List<string>();
            filePath.Add("C:\\popbill.example.dotnetcore\\FaxExample\\wwwroot\\images\\tax_image.png");
            filePath.Add("C:\\popbill.example.dotnetcore\\FaxExample\\wwwroot\\images\\tax_image.png");

            // 팩스제목
            string title = "팩스 제목";

            // 예약전송일시(yyyyMMddHHmmss) ex) 20181126121206, null인 경우 즉시전송
            DateTime? sndDT = null;

            // 광고여부 (기본값 false)
            bool adsYN = false;

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
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

        #endregion

        #region 접수번호 관련 기능 (요청번호 미할당)

        /*
         * 팩스전송요청시 발급받은 접수번호(receiptNum)로 전송결과를 확인합니다
         * - 응답항목에 대한 자세한 사항은 "[팩스 API 연동매뉴얼] >  3.3.1 GetFaxDetail (전송내역 및 전송상태 확인)을 참조하시기 바랍니다.
         */
        public IActionResult GetFaxDetail()
        {
            // 팩스전송 요청시 발급받은 접수번호
            string receiptNum = "018112714511700001";

            try
            {
                var Response = _faxService.GetFaxDetail(corpNum, receiptNum, userID);
                return View("GetFaxDetail", Response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팩스전송요청시 발급받은 접수번호(receiptNum)로 팩스 예약전송건을 취소합니다.
         * - 예약전송 취소는 예약전송시간 10분전까지 가능하며, 팩스변환 이후 가능합니다.
         */
        public IActionResult CancelReserve()
        {
            // 팩스전송 요청시 발급받은 접수번호
            string receiptNum = "018120517184000001";

            try
            {
                var Response = _faxService.CancelReserve(corpNum, receiptNum, userID);
                return View("Response", Response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팩스를 재전송합니다.
         * - 접수일로부터 60일이 경과된 경우 재전송할 수 없습니다.
         * - 팩스 재전송 요청시 포인트가 차감됩니다. (전송실패시 환불처리)
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

            // 예약전송일시(yyyyMMddHHmmss) ex) 20181126121206, null인 경우 즉시전송
            DateTime? sndDT = null;

            // 광고여부 (기본값 false)
            bool adsYN = false;

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
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
         * [대량전송] 팩스를 재전송합니다.
         * - 접수일로부터 60일이 경과된 경우 재전송할 수 없습니다.
         * - 팩스 재전송 요청시 포인트가 차감됩니다. (전송실패시 환불처리)
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
                receiver.receiveNum = "111-2222-3333";

                // 수신자명
                receiver.receiveName = "수신자명칭_" + i;
                receivers.Add(receiver);
            }

            // 수신자정보를 변경하지 않고 기존 전송정보로 전송하는 경우
            // List<FaxReceiver> receivers = null;

            // 팩스제목
            string title = "팩스 제목";

            // 예약전송일시(yyyyMMddHHmmss) ex) 20181126121206, null인 경우 즉시전송
            DateTime? sndDT = null;

            // 광고여부 (기본값 false)
            bool adsYN = false;

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
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

        #endregion

        #region 요청번호 할당 전송건 관련 기능

        /*
          * 팩스전송요청시 할당한 전송요청번호(requestNum)으로 전송결과를 확인합니다
          * - 응답항목에 대한 자세한 사항은 "[팩스 API 연동매뉴얼] >  3.3.2 GetFaxDetailRN (전송내역 및 전송상태 확인 - 요청번호 할당)을 참조하시기 바랍니다.
          */
        public IActionResult GetFaxDetailRN()
        {
            // 팩스 전송요청시 할당한 전송요청번호
            string requestNum = "20180929225651";

            try
            {
                var Response = _faxService.GetFaxDetailRN(corpNum, requestNum, userID);
                return View("GetFaxDetail", Response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팩스전송요청시 할당한 전송요청번호(requestNum)로 팩스 예약전송건을 취소합니다.
         * - 예약전송 취소는 예약전송시간 10분전까지 가능하며, 팩스변환 이후 가능합니다.
         */
        public IActionResult CancelReserveRN()
        {
            // 팩스 전송요청시 할당한 전송요청번호
            string requestNum = "20181205-002";

            try
            {
                var Response = _faxService.CancelReserveRN(corpNum, requestNum, userID);
                return View("Response", Response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 전송요청번호(requestNum)을 할당한 팩스를 재전송합니다.
         * - 접수일로부터 60일이 경과된 경우 재전송할 수 없습니다.
         * - 팩스 재전송 요청시 포인트가 차감됩니다. (전송실패시 환불처리)
         */
        public IActionResult ResendFAXRN()
        {
            // 팩스전송 요청시 할당한 요청번호
            string preRequestNum = "20181205-001";

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

            // 예약전송일시(yyyyMMddHHmmss) ex) 20181126121206, null인 경우 즉시전송
            DateTime? sndDT = null;

            // 광고여부 (기본값 false)
            bool adsYN = false;

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
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
         * [대량전송] 전송요청번호(requestNum)을 할당한 팩스를 재전송합니다.
         * - 접수일로부터 60일이 경과된 경우 재전송할 수 없습니다.
         * - 팩스 재전송 요청시 포인트가 차감됩니다. (전송실패시 환불처리)
         */
        public IActionResult ResendFAXRN_multi()
        {
            // 팩스전송 요청시 할당한 요청번호
            string preRequestNum = "20181205-001";

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
                receiver.receiveNum = "111-2222-3333";

                // 수신자명
                receiver.receiveName = "수신자명칭_" + i;
                receivers.Add(receiver);
            }

            // 수신자정보를 변경하지 않고 기존 전송정보로 전송하는 경우
            // List<FaxReceiver> receivers = null;

            // 팩스제목
            string title = "팩스 제목";

            // 예약전송일시(yyyyMMddHHmmss) ex) 20181126121206, null인 경우 즉시전송
            DateTime? sndDT = null;

            // 광고여부 (기본값 false)
            bool adsYN = false;

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
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

        #endregion

        #region 팩스전송 목록조회

        /*
         * 검색조건을 사용하여 팩스전송 내역을 조회합니다.
         * - 최대 검색기간 : 6개월 이내
         */
        public IActionResult Search()
        {
            // 최대 검색기간 : 6개월 이내 
            // 시작일자, 날짜형식(yyyyMMdd)
            string SDate = "20181101";

            // 종료일자, 날짜형식(yyyyMMdd)
            string EDate = "20181126";

            //전송상태 배열 1-대기, 2-성공, 3-실패, 4-취소
            string[] State = new string[4];
            State[0] = "1";
            State[1] = "2";
            State[2] = "3";
            State[3] = "4";

            // 예약여부, True-예약전송건 검색, False-즉시전송건 검색
            bool ReserveYN = false;

            // 개인조회여부, True-개인조회, False-전체조회
            bool SenderOnly = false;

            // 페이지 번호, 기본값 '1'
            int Page = 1;

            // 페이지당 검색개수, 기본값 '500', 최대 '1000' 
            int PerPage = 30;

            // 정렬방향, D-내림차순, A-오름차순
            string Order = "D";

            // 조회 검색어, 팩스 전송시 기재한 발신자명 또는 수신자명 기재
            string QString = "";

            try
            {
                var response = _faxService.Search(corpNum, SDate, EDate, State, ReserveYN, SenderOnly, Page, PerPage,
                    Order, QString, userID);
                return View("Search", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팩스 전송내역 팝업 URL을 반환합니다.
         * - 반환된 URL은 보안정책에 따라 30초의 유효시간을 갖습니다.
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
         * 접수한 팩스 전송건에 대한 미리보기 팝업 URL을 반환합니다.
         * - 반환된 URL은 보안정책에 따라 30초의 유효시간을 갖습니다.
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
         * 연동회원 잔여포인트를 확인합니다.
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
         * 팝빌 연동회원의 포인트충전 팝업 URL을 반환합니다.
         * - 반환된 URL은 보안정책에 따라 30초의 유효시간을 갖습니다.
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
         * 파트너의 잔여포인트를 확인합니다.
         * - 과금방식이 연동과금인 경우 연동회원 잔여포인트(GetBalance API)를 이용하시기 바랍니다.
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
         * 파트너 포인트 충전 팝업 URL을 반환합니다.
         * - 반환된 URL은 보안정책에 따라 30초의 유효시간을 갖습니다.
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
         * 팩스 발행단가를 확인합니다.
         */
        public IActionResult GetUnitCost()
        {
            try
            {
                var result = _faxService.GetUnitCost(corpNum);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팩스 API 서비스 과금정보를 확인합니다.
         */
        public IActionResult GetChargeInfo()
        {
            try
            {
                var response = _faxService.GetChargeInfo(corpNum);
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

                var response = _faxService.CheckIsMember(corpNum, linkID);
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
                var response = _faxService.CheckID(checkID);
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
            JoinForm joinInfo = new JoinForm
            {
                LinkID = "TESTER", // 링크아이디
                ID = "userid", // 아이디, 6자이상 50자 미만
                PWD = "12341234", // 비밀번호, 6자이상 20자 미만
                CorpNum = "0000000001", // 사업자번호 "-" 제외
                CEOName = "대표자 성명", // 대표자 성명 
                CorpName = "상호", // 상호
                Addr = "주소", // 주소
                BizType = "업태", // 업태
                BizClass = "종목", // 종목
                ContactName = "담당자명", // 담당자 성명 
                ContactEmail = "test@test.com", // 담당자 이메일주소         
                ContactTEL = "070-4304-2992", // 담당자 연락처   
                ContactHP = "010-111-222", // 담당자 휴대폰번호 
                ContactFAX = "02-111-222" // 담당자 팩스번호
            };

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
         * 팝빌에 로그인 상태로 접근할 수 있는 팝업 URL을 반환합니다.
         * - 반환된 URL은 보안정책에 따라 30초의 유효시간을 갖습니다.
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
         */
        public IActionResult GetCorpInfo()
        {
            try
            {
                var response = _faxService.GetCorpInfo(corpNum, userID);
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
            CorpInfo corpInfo = new CorpInfo
            {
                ceoname = "대표자 성명 수정", // 대표자 성명
                corpName = "상호 수정", // 상호
                addr = "주소 수정", // 주소
                bizType = "업태 수정", // 업태 
                bizClass = "종목 수정" // 종목
            };

            try
            {
                var response = _faxService.UpdateCorpInfo(corpNum, corpInfo, userID);
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
            Contact contactInfo = new Contact
            {
                id = "testkorea_20181108", // 담당자 아이디, 6자 이상 50자 미만
                pwd = "user_password", // 비밀번호, 6자 이상 20자 미만
                personName = "코어담당자", // 담당자명
                tel = "070-4304-2992", // 담당자연락처
                hp = "010-111-222", // 담당자 휴대폰번호
                fax = "02-111-222", // 담당자 팩스번호 
                email = "netcore@linkhub.co.kr", // 담당자 메일주소
                searchAllAllowYN = true, // 회사조회 권한여부, true(회사조회), false(개인조회)
                mgrYN = false // 관리자 권한여부 
            };

            try
            {
                var response = _faxService.RegistContact(corpNum, contactInfo, userID);
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
                var response = _faxService.ListContact(corpNum, userID);
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
            Contact contactInfo = new Contact
            {
                id = "testkorea", // 아이디
                personName = "담당자명", // 담당자명 
                tel = "070-4304-2992", // 연락처
                hp = "010-222-111", // 휴대폰번호
                fax = "02-222-1110", // 팩스번호
                email = "aspnetcore@popbill.co.kr", // 이메일주소
                searchAllAllowYN = true, // 회사조회 권한여부, true(회사조회), false(개인조회)
                mgrYN = false // 관리자 권한여부 
            };

            try
            {
                var response = _faxService.UpdateContact(corpNum, contactInfo, userID);
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