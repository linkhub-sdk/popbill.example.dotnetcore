/*
 * 팝빌 문자 API .NET Core SDK Example
 * .NET Core 연동 튜토리얼 안내 : https://developers.popbill.com/guide/sms/dotnetcore/getting-started/tutorial
 * 
 * 업데이트 일자 : 2024-02-26
 * 연동 기술지원 연락처 : 1600 - 9854
 * 연동 기술지원 이메일 : code @linkhubcorp.com
 * 
 * <테스트 연동개발 준비사항>
 * 1) 발신번호 사전등록을 합니다. (등록방법은 사이트/API 두가지 방식이 있습니다.)
 *   - 1. 팝빌 사이트 로그인 > [문자/팩스] > [문자] > [발신번호 사전등록] 메뉴에서 등록
 *   - 2. getSenderNumberMgtURL API를 통해 반환된 URL을 이용하여 발신번호 등록
*/
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Popbill;
using Popbill.Message;

namespace MessageExample.Controllers
{
    public class MessageController : Controller
    {
        private readonly MessageService _messageService;

        public MessageController(MessageInstance MSGinstance)
        {
            // 문자 서비스 객체 생성
            _messageService = MSGinstance.messageService;
        }

        // 팝빌 연동회원 사업자번호 (하이픈 '-' 없이 10자리)
        string corpNum = "1234567890";

        // 팝빌 연동회원 아이디
        string userID = "testkorea";

        /*
         * 문자 Index page (Message/Index.cshtml)
         */
        public IActionResult Index()
        {
            return View();
        }

        #region 발신번호 사전등록

        /*
         * 문자 발신번호 등록여부를 확인합니다.
         * - 발신번호 상태가 '승인'인 경우에만 리턴값 'Response'의 변수 'code'가 1로 반환됩니다.
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/sendnum#CheckSenderNumber
         */
        public IActionResult CheckSenderNumber()
        {
            // 확인할 발신번호
            string senderNumber = "";

            try
            {
                var Response = _messageService.CheckSenderNumber(corpNum, senderNumber);
                return View("Response", Response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 발신번호를 등록하고 내역을 확인하는 문자 발신번호 관리 페이지 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/sendnum#GetSenderNumberMgtURL
         */
        public IActionResult GetSenderNumberMgtURL()
        {
            try
            {
                var result = _messageService.GetSenderNumberMgtURL(corpNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌에 등록한 연동회원의 문자 발신번호 목록을 확인합니다.
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/sendnum#GetSenderNumberList
         */
        public IActionResult GetSenderNumberList()
        {
            try
            {
                var response = _messageService.GetSenderNumberList(corpNum);
                return View("GetSenderNumberList", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        #endregion

        #region 문자전송

        /*
         * 최대 90byte의 단문(SMS) 메시지 1건 전송을 팝빌에 접수합니다.
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/send#SendSMSOne
         */
        public IActionResult SendSMS()
        {
            // 발신번호
            string senderNum = "";

            // 발신자명
            string senderName = "발신자명";

            // 수신번호
            string receiverNum = "";

            // 수신자명
            string receiverName = "수신자명";

            // 메시지내용, 90byte초과된 내용은 삭제되어 전송됨.
            string contents = "단문 문자 메시지 내용. 90byte 초과시 삭제되어 전송";

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(2025,01,01,12,00,00);
            DateTime? sndDT = null;

            // 광고성 메시지 여부 ( true , false 중 택 1)
            // └ true = 광고 , false = 일반
            // - 미입력 시 기본값 false 처리
            bool adsYN = false;

            // 전송요청번호
            // 팝빌이 접수 단위를 식별할 수 있도록 파트너가 부여하는 식별번호.
            // 1~36자리로 구성. 영문, 숫자, 하이픈(-), 언더바(_)를 조합하여 팝빌 회원별로 중복되지 않도록 할당.
            string requestNum = "";

            try
            {
                var receiptNum = _messageService.SendSMS(corpNum, senderNum, senderName, receiverNum, receiverName,
                    contents, sndDT, adsYN, requestNum);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 최대 90byte의 단문(SMS) 메시지 다수건 전송을 팝빌에 접수합니다. (최대 1,000건)
         * - 수신자마다 개별 내용을 전송할 수 있습니다(대량전송).
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/send#SendSMSMulti
         */
        public IActionResult SendSMS_Multi()
        {
            // 발신번호
            string senderNum = "";

            // 메시지 구성, 최대 1000건
            List<Message> messages = new List<Message>();

            for (int i = 0; i < 100; i++)
            {
                Message msg = new Message();

                // 발신자명
                msg.senderName = "발신자명";

                // 수신번호
                msg.receiveNum = "";

                // 수신자명
                msg.receiveName = "수신자명칭_" + i;

                // 메시지 내용, 90byte초과된 내용은 삭제되어 전송됨.
                msg.content = "단문 문자메시지 내용, 각 메시지마다 개별설정 가능." + i;

                // 파트너 지정키, 대량전송시, 수신자 구별용 메모
                msg.interOPRefKey = "2022527-" + i;

                messages.Add(msg);
            }

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(2025,01,01,12,00,00);
            DateTime? sndDT = null;

            // 광고성 메시지 여부 ( true , false 중 택 1)
            // └ true = 광고 , false = 일반
            // - 미입력 시 기본값 false 처리
            bool adsYN = false;

            // 전송요청번호
            // 팝빌이 접수 단위를 식별할 수 있도록 파트너가 부여하는 식별번호.
            // 1~36자리로 구성. 영문, 숫자, 하이픈(-), 언더바(_)를 조합하여 팝빌 회원별로 중복되지 않도록 할당.
            string requestNum = "";

            try
            {
                var receiptNum = _messageService.SendSMS(corpNum, senderNum, messages, sndDT, adsYN, requestNum);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 최대 90byte의 단문(SMS) 메시지 다수건 전송을 팝빌에 접수합니다. (최대 1,000건)
         * - 모든 수신자에게 동일한 내용을 전송합니다(동보전송).
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/send#SendSMSSame
         */
        public IActionResult SendSMS_Same()
        {
            // 발신번호
            string senderNum = "";

            // (동보) 메시지내용, 90byte초과된 내용은 삭제되어 전송됨.
            string contents = "단문 문자 메시지 내용. 90byte 초과시 삭제되어 전송";

            // 메시지 구성, 최대 1000건
            List<Message> messages = new List<Message>();

            for (int i = 0; i < 100; i++)
            {
                Message msg = new Message();

                // 발신자명
                msg.senderName = "발신자명";

                // 수신번호
                msg.receiveNum = "";

                // 수신자명
                msg.receiveName = "수신자명칭_" + i;

                // 파트너 지정키, 대량전송시, 수신자 구별용 메모
                msg.interOPRefKey = "2022527-" + i;

                messages.Add(msg);
            }

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(2025,01,01,12,00,00);
            DateTime? sndDT = null;

            // 광고성 메시지 여부 ( true , false 중 택 1)
            // └ true = 광고 , false = 일반
            // - 미입력 시 기본값 false 처리
            bool adsYN = false;

            // 전송요청번호
            // 팝빌이 접수 단위를 식별할 수 있도록 파트너가 부여하는 식별번호.
            // 1~36자리로 구성. 영문, 숫자, 하이픈(-), 언더바(_)를 조합하여 팝빌 회원별로 중복되지 않도록 할당.
            string requestNum = "";

            try
            {
                var receiptNum = _messageService.SendSMS(corpNum, senderNum, contents, messages, sndDT,
                    adsYN, requestNum);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        /*
         * 최대 2,000byte의 장문(LMS) 메시지 1건 전송을 팝빌에 접수합니다.
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/send#SendLMSOne
         */
        public IActionResult SendLMS()
        {
            // 발신번호
            string senderNum = "";

            // 발신자명
            string senderName = "발신자명";

            // 수신번호
            string receiverNum = "";

            // 수신자명
            string receiverName = "수신자명";

            // 메시지제목
            string subject = "장문 문자 메시지 제목";

            // 메시지내용, 최대 2000byte 초과된 내용은 삭제되어 전송됨.
            string contents = "장문 문자 메시지 내용. 최대 2000byte 초과된 내용은 삭제되어 전송.";

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(2025,01,01,12,00,00);
            DateTime? sndDT = null;

            // 광고성 메시지 여부 ( true , false 중 택 1)
            // └ true = 광고 , false = 일반
            // - 미입력 시 기본값 false 처리
            bool adsYN = false;

            // 전송요청번호
            // 팝빌이 접수 단위를 식별할 수 있도록 파트너가 부여하는 식별번호.
            // 1~36자리로 구성. 영문, 숫자, 하이픈(-), 언더바(_)를 조합하여 팝빌 회원별로 중복되지 않도록 할당.
            string requestNum = "";

            try
            {
                var receiptNum = _messageService.SendLMS(corpNum, senderNum, senderName, receiverNum, receiverName,
                    subject, contents, sndDT, adsYN, requestNum);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 최대 2,000byte의 장문(LMS) 메시지 다수건 전송을 팝빌에 접수합니다. (최대 1,000건)
         * - 수신자마다 개별 내용을 전송할 수 있습니다(대량전송).
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/send#SendLMSMulti
         */
        public IActionResult SendLMS_Multi()
        {
            // 발신번호
            string senderNum = "";

            // 메시지 구성, 최대 1000건
            List<Message> messages = new List<Message>();

            for (int i = 0; i < 100; i++)
            {
                Message msg = new Message();

                // 발신자명
                msg.senderName = "발신자명";

                // 수신번호
                msg.receiveNum = "";

                // 수신자명
                msg.receiveName = "수신자명칭_" + i;

                // 메시지 제목
                msg.subject = "메시지 제목" + i;

                // 메시지내용, 최대 2000byte 초과된 내용은 삭제되어 전송됨.
                msg.content = "장문 문자 메시지 내용. 최대 2000byte 초과된 내용은 삭제되어 전송." + i;

                // 파트너 지정키, 대량전송시, 수신자 구별용 메모
                msg.interOPRefKey = "2022527-" + i;

                messages.Add(msg);
            }

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(2025,01,01,12,00,00);
            DateTime? sndDT = null;

            // 광고성 메시지 여부 ( true , false 중 택 1)
            // └ true = 광고 , false = 일반
            // - 미입력 시 기본값 false 처리
            bool adsYN = false;

            // 전송요청번호
            // 팝빌이 접수 단위를 식별할 수 있도록 파트너가 부여하는 식별번호.
            // 1~36자리로 구성. 영문, 숫자, 하이픈(-), 언더바(_)를 조합하여 팝빌 회원별로 중복되지 않도록 할당.
            string requestNum = "";

            try
            {
                var receiptNum = _messageService.SendLMS(corpNum, senderNum, messages, sndDT, adsYN, requestNum);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        /*
         * 최대 2,000byte의 장문(LMS) 메시지 다수건 전송을 팝빌에 접수합니다. (최대 1,000건)
         * - 모든 수신자에게 동일한 내용을 전송합니다(동보전송).
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/send#SendLMSSame
         */
        public IActionResult SendLMS_Same()
        {
            // 발신번호
            string senderNum = "";

            // 메시지제목
            string subject = "장문 문자 메시지 제목";

            // (동보) 메시지내용, 최대 2000byte 초과된 내용은 삭제되어 전송됨.
            string contents = "장문 문자 메시지 내용. 최대 2000byte 초과된 내용은 삭제되어 전송.";

            // 메시지 구성, 최대 1000건
            List<Message> messages = new List<Message>();

            for (int i = 0; i < 100; i++)
            {
                Message msg = new Message();

                // 발신자명
                msg.senderName = "발신자명";

                // 수신번호
                msg.receiveNum = "";

                // 수신자명
                msg.receiveName = "수신자명칭_" + i;

                // 파트너 지정키, 대량전송시, 수신자 구별용 메모
                msg.interOPRefKey = "2022527-" + i;

                messages.Add(msg);
            }

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(2025,01,01,12,00,00);
            DateTime? sndDT = null;

            // 광고성 메시지 여부 ( true , false 중 택 1)
            // └ true = 광고 , false = 일반
            // - 미입력 시 기본값 false 처리
            bool adsYN = false;

            // 전송요청번호
            // 팝빌이 접수 단위를 식별할 수 있도록 파트너가 부여하는 식별번호.
            // 1~36자리로 구성. 영문, 숫자, 하이픈(-), 언더바(_)를 조합하여 팝빌 회원별로 중복되지 않도록 할당.
            string requestNum = "";

            try
            {
                var receiptNum = _messageService.SendLMS(corpNum, senderNum, subject, contents, messages, sndDT, adsYN,
                    requestNum);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        /*
         * 최대 2,000byte의 메시지와 이미지로 구성된 포토문자(MMS) 1건 전송을 팝빌에 접수합니다.
         * - 이미지 파일 포맷/규격 : 최대 300Kbyte(JPEG, JPG), 가로/세로 1,000px 이하 권장
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/send#SendMMSOne
         */
        public IActionResult SendMMS()
        {
            // 발신번호
            string senderNum = "";

            // 발신자명
            string senderName = "발신자명";

            // 수신번호
            string receiverNum = "";

            // 수신자명
            string receiverName = "수신자명";

            // 메시지제목
            string subject = "포토 문자 메시지 제목";

            // 메시지내용, 최대 2000byte 초과된 내용은 삭제되어 전송됨.
            string contents = "포토 문자 메시지 내용. 최대 2000byte 초과된 내용은 삭제되어 전송.";

            // 첨부파일 경로
            string filePath = "C:\\popbill.example.dotnetcore\\MessageExample\\wwwroot\\images\\image.jpg";

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송sssssss
            // ex) DateTime sndDT = new DateTime(2025,01,01,12,00,00);
            DateTime? sndDT = null;

            // 광고성 메시지 여부 ( true , false 중 택 1)
            // └ true = 광고 , false = 일반
            // - 미입력 시 기본값 false 처리
            bool adsYN = false;

            // 전송요청번호
            // 팝빌이 접수 단위를 식별할 수 있도록 파트너가 부여하는 식별번호.
            // 1~36자리로 구성. 영문, 숫자, 하이픈(-), 언더바(_)를 조합하여 팝빌 회원별로 중복되지 않도록 할당.
            string requestNum = "";

            try
            {
                var receiptNum = _messageService.SendMMS(corpNum, senderNum, senderName, receiverNum, receiverName,
                    subject, contents, filePath, sndDT, adsYN, requestNum);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 최대 2,000byte의 메시지와 이미지로 구성된 포토문자(MMS) 다수건 전송을 팝빌에 접수합니다. (최대 1,000건)
         * - 수신자마다 개별 내용을 전송할 수 있습니다(대량전송).
         * - 이미지 파일 포맷/규격 : 최대 300Kbyte(JPEG), 가로/세로 1,000px 이하 권장
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/send#SendMMSMulti
         */
        public IActionResult SendMMS_Multi()
        {
            // 발신번호
            string senderNum = "";

            // 메시지 구성, 최대 1000건
            List<Message> messages = new List<Message>();

            for (int i = 0; i < 100; i++)
            {
                Message msg = new Message();

                // 발신자명
                msg.senderName = "발신자명";

                // 수신번호
                msg.receiveNum = "";

                // 수신자명
                msg.receiveName = "수신자명칭_" + i;

                // 메시지 제목
                msg.subject = "메시지 제목" + i;

                // 메시지내용, 최대 2000byte 초과된 내용은 삭제되어 전송됨.
                msg.content = "포토 문자 메시지 내용. 최대 2000byte 초과된 내용은 삭제되어 전송." + i;

                // 파트너 지정키, 대량전송시, 수신자 구별용 메모
                msg.interOPRefKey = "2022527-" + i;

                messages.Add(msg);
            }

            // 첨부파일 경로
            string filePath = "C:\\popbill.example.dotnetcore\\MessageExample\\wwwroot\\images\\image.jpg";

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(2025,01,01,12,00,00);
            DateTime? sndDT = null;

            // 광고성 메시지 여부 ( true , false 중 택 1)
            // └ true = 광고 , false = 일반
            // - 미입력 시 기본값 false 처리
            bool adsYN = false;

            // 전송요청번호
            // 팝빌이 접수 단위를 식별할 수 있도록 파트너가 부여하는 식별번호.
            // 1~36자리로 구성. 영문, 숫자, 하이픈(-), 언더바(_)를 조합하여 팝빌 회원별로 중복되지 않도록 할당.
            string requestNum = "";

            try
            {
                var receiptNum =
                    _messageService.SendMMS(corpNum, senderNum, messages, filePath, sndDT, adsYN, requestNum);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 최대 2,000byte의 메시지와 이미지로 구성된 포토문자(MMS) 다수건 전송을 팝빌에 접수합니다. (최대 1,000건)
         * - 모든 수신자에게 동일한 내용을 전송합니다(동보전송).
         * - 이미지 파일 포맷/규격 : 최대 300Kbyte(JPEG), 가로/세로 1,000px 이하 권장
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/send#SendMMSSame
         */
        public IActionResult SendMMS_Same()
        {
            // 발신번호
            string senderNum = "";

            // 메시지제목
            string subject = "포토 문자 메시지 제목";

            // (동보) 메시지내용, 최대 2000byte 초과된 내용은 삭제되어 전송됨.
            string contents = "포토 문자 메시지 내용. 최대 2000byte 초과된 내용은 삭제되어 전송.";

            // 메시지 구성, 최대 1000건
            List<Message> messages = new List<Message>();

            for (int i = 0; i < 100; i++)
            {
                Message msg = new Message();

                // 발신자명
                msg.senderName = "발신자명";

                // 수신번호
                msg.receiveNum = "";

                // 수신자명
                msg.receiveName = "수신자명칭_" + i;

                // 파트너 지정키, 대량전송시, 수신자 구별용 메모
                msg.interOPRefKey = "2022527-" + i;

                messages.Add(msg);
            }

            // 첨부파일 경로
            string filePath = "C:\\popbill.example.dotnetcore\\MessageExample\\wwwroot\\images\\image.jpg";

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(2025,01,01,12,00,00);
            DateTime? sndDT = null;

            // 광고성 메시지 여부 ( true , false 중 택 1)
            // └ true = 광고 , false = 일반
            // - 미입력 시 기본값 false 처리
            bool adsYN = false;

            // 전송요청번호
            // 팝빌이 접수 단위를 식별할 수 있도록 파트너가 부여하는 식별번호.
            // 1~36자리로 구성. 영문, 숫자, 하이픈(-), 언더바(_)를 조합하여 팝빌 회원별로 중복되지 않도록 할당.
            string requestNum = "";

            try
            {
                var receiptNum = _messageService.SendMMS(corpNum, senderNum, subject, contents, messages, filePath,
                    sndDT, adsYN, requestNum);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 메시지 크기(90byte)에 따라 단문/장문(SMS/LMS)을 자동으로 인식하여 1건의 메시지를 전송을 팝빌에 접수합니다.
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/send#SendXMSOne
         */
        public IActionResult SendXMS()
        {
            // 발신번호
            string senderNum = "";

            // 발신자명
            string senderName = "발신자명";

            // 수신번호
            string receiverNum = "";

            // 수신자명
            string receiverName = "수신자명";

            // 메시지제목
            string subject = "단문/장문 문자 메시지 제목";

            // 메시지내용, 메시지 내용의 길이(90byte)에 따라 SMS/LMS(단문/장문)를 자동인식하여 전송됨.
            string contents = "단문/장문 문자 메시지 내용. 메시지 내용의 길이(90byte)에 따라 SMS/LMS(단문/장문)를 자동인식하여 전송됨";

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(2025,01,01,12,00,00);
            DateTime? sndDT = null;

            // 광고성 메시지 여부 ( true , false 중 택 1)
            // └ true = 광고 , false = 일반
            // - 미입력 시 기본값 false 처리
            bool adsYN = false;

            // 전송요청번호
            // 팝빌이 접수 단위를 식별할 수 있도록 파트너가 부여하는 식별번호.
            // 1~36자리로 구성. 영문, 숫자, 하이픈(-), 언더바(_)를 조합하여 팝빌 회원별로 중복되지 않도록 할당.
            string requestNum = "";

            try
            {
                var receiptNum = _messageService.SendXMS(corpNum, senderNum, senderName, receiverNum, receiverName,
                    subject, contents, sndDT, adsYN, requestNum);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 메시지 크기(90byte)에 따라 단문/장문(SMS/LMS)을 자동으로 인식하여 다수건의 메시지 전송을 팝빌에 접수합니다. (최대 1,000건)
         * - 수신자마다 개별 내용을 전송할 수 있습니다(대량전송).
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/send#SendXMSMulti
         */
        public IActionResult SendXMS_Multi()
        {
            // 발신번호
            string senderNum = "";

            // 메시지 구성, 최대 1000건
            List<Message> messages = new List<Message>();

            for (int i = 0; i < 100; i++)
            {
                Message msg = new Message();

                // 발신자명
                msg.senderName = "발신자명";

                // 수신번호
                msg.receiveNum = "";

                // 수신자명
                msg.receiveName = "수신자명칭_" + i;

                // 메시지 제목
                msg.subject = "메시지 제목" + i;

                // 메시지내용, 메시지 내용의 길이(90byte)에 따라 SMS/LMS(단문/장문)를 자동인식하여 전송됨.
                msg.content = "단문/장문 문자 메시지 내용. 메시지 내용의 길이(90byte)에 따라 SMS/LMS(단문/장문)를 자동인식하여 전송됨" + i;

                // 파트너 지정키, 대량전송시, 수신자 구별용 메모
                msg.interOPRefKey = "2022527-" + i;

                messages.Add(msg);
            }

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(2025,01,01,12,00,00);
            DateTime? sndDT = null;

            // 광고성 메시지 여부 ( true , false 중 택 1)
            // └ true = 광고 , false = 일반
            // - 미입력 시 기본값 false 처리
            bool adsYN = false;

            // 전송요청번호
            // 팝빌이 접수 단위를 식별할 수 있도록 파트너가 부여하는 식별번호.
            // 1~36자리로 구성. 영문, 숫자, 하이픈(-), 언더바(_)를 조합하여 팝빌 회원별로 중복되지 않도록 할당.
            string requestNum = "";

            try
            {
                var receiptNum = _messageService.SendXMS(corpNum, senderNum, messages, sndDT, adsYN, requestNum);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 메시지 크기(90byte)에 따라 단문/장문(SMS/LMS)을 자동으로 인식하여 다수건의 메시지 전송을 팝빌에 접수합니다. (최대 1,000건)
         * - 모든 수신자에게 동일한 내용을 전송합니다(동보전송).
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/send#SendXMSSame
         */
        public IActionResult SendXMS_Same()
        {
            // 발신번호
            string senderNum = "";

            // 메시지제목
            string subject = "단문/장문 문자 메시지 제목";

            // (동보) 메시지내용, 메시지 내용의 길이(90byte)에 따라 SMS/LMS(단문/장문)를 자동인식하여 전송됨.
            string contents = "단문/장문 문자 메시지 내용. 메시지 내용의 길이(90byte)에 따라 SMS/LMS(단문/장문)를 자동인식하여 전송됨";

            // 메시지 구성, 최대 1000건
            List<Message> messages = new List<Message>();

            for (int i = 0; i < 100; i++)
            {
                Message msg = new Message();

                // 발신자명
                msg.senderName = "발신자명";

                // 수신번호
                msg.receiveNum = "";

                // 수신자명
                msg.receiveName = "수신자명칭_" + i;

                // 파트너 지정키, 대량전송시, 수신자 구별용 메모
                msg.interOPRefKey = "2022527-" + i;

                messages.Add(msg);
            }

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(2025,01,01,12,00,00);
            DateTime? sndDT = null;

            // 광고성 메시지 여부 ( true , false 중 택 1)
            // └ true = 광고 , false = 일반
            // - 미입력 시 기본값 false 처리
            bool adsYN = false;

            // 전송요청번호
            // 팝빌이 접수 단위를 식별할 수 있도록 파트너가 부여하는 식별번호.
            // 1~36자리로 구성. 영문, 숫자, 하이픈(-), 언더바(_)를 조합하여 팝빌 회원별로 중복되지 않도록 할당.
            string requestNum = "";

            try
            {
                var receiptNum = _messageService.SendXMS(corpNum, senderNum, subject, contents, messages, sndDT, adsYN,
                    requestNum);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌에서 반환받은 접수번호를 통해 예약접수된 문자 메시지 전송을 취소합니다. (예약시간 10분 전까지 가능)
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/send#CancelReserve
         */
        public IActionResult CancelReserve()
        {
            // 문자 전송요청시 발급받은 접수번호
            string receiptNum = "018112714000000020";

            try
            {
                var Response = _messageService.CancelReserve(corpNum, receiptNum);
                return View("Response", Response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 파트너가 할당한 전송요청 번호를 통해 예약접수된 문자 전송을 취소합니다. (예약시간 10분 전까지 가능)
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/send#CancelReserveRN
         */
        public IActionResult CancelReserveRN()
        {
            // 문자 전송요청시 할당한 요청번호
            string requestNum = "";

            try
            {
                var Response = _messageService.CancelReserveRN(corpNum, requestNum);
                return View("Response", Response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }
        /*
         * 팝빌에서 반환받은 접수번호와 수신번호를 통해 예약접수된 문자 메시지 전송을 취소합니다. (예약시간 10분 전까지 가능)
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/send#CancelReservebyRCV
         */
        public IActionResult CancelReservebyRCV()
        {
            // 문자 전송요청시 발급받은 접수번호
            string receiptNum = "";

            // 문자 전송요청시 요청한 수신번호
            string receiveNum = "";

            try
            {
                var Response = _messageService.CancelReservebyRCV(corpNum, receiptNum, receiveNum);
                return View("Response", Response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 파트너가 할당한 전송요청 번호와 수신번호를 통해 예약접수된 문자 전송을 취소합니다. (예약시간 10분 전까지 가능)
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/send#CancelReserveRNbyRCV
         */
        public IActionResult CancelReserveRNbyRCV()
        {
            // 문자 전송요청시 할당한 요청번호
            string requestNum = "";

            // 문자 전송요청시 요청한 수신번호
            string receiveNum = "";

            try
            {
                var Response = _messageService.CancelReserveRNbyRCV(corpNum, requestNum, receiveNum);
                return View("Response", Response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        #endregion

        #region 정보확인

        /*
         * 팝빌에서 반환받은 접수번호를 통해 문자 전송상태 및 결과를 확인합니다.
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/info#GetMessages
         */
        public IActionResult GetMessages()
        {
            // 문자 전송요청시 발급받은 접수번호
            string receiptNum = "018112714000000020";

            try
            {
                var Response = _messageService.GetMessages(corpNum, receiptNum);
                return View("GetMessages", Response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 파트너가 할당한 전송요청 번호를 통해 문자 전송상태 및 결과를 확인합니다.
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/info#GetMessagesRN
         */
        public IActionResult GetMessagesRN()
        {
            // 문자 전송요청시 할당한 요청번호
            string requestNum = "";

            try
            {
                var Response = _messageService.GetMessagesRN(corpNum, requestNum);
                return View("GetMessagesRN", Response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 검색조건에 해당하는 문자 전송내역을 조회합니다. (조회기간 단위 : 최대 2개월)
         * 문자 접수일시로부터 6개월 이내 접수건만 조회할 수 있습니다.
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/info#Search
         */
        public IActionResult Search()
        {
            // 최대 검색기간 : 6개월 이내
            // 시작일자, 날짜형식(yyyyMMdd)
            string SDate = "20220501";

            // 종료일자, 날짜형식(yyyyMMdd)
            string EDate = "20220527";

            // 전송상태 배열 ("1" , "2" , "3" , "4" 중 선택, 다중 선택 가능)
            // └ 1 = 대기 , 2 = 성공 , 3 = 실패 , 4 = 취소
            // - 미입력 시 전체조회
            string[] State = new string[4];
            State[0] = "1";
            State[1] = "2";
            State[2] = "3";
            State[3] = "4";

            // 검색대상 배열 ("SMS" , "LMS" , "MMS" 중 선택, 다중 선택 가능)
            // └ SMS = 단문 , LMS = 장문 , MMS = 포토문자
            // - 미입력 시 전체조회
            string[] Item = new string[3];
            Item[0] = "SMS";
            Item[1] = "LMS";
            Item[2] = "MMS";

            // 예약여부 (false , true 중 택 1)
            // └ false = 전체조회, true = 예약전송건 조회
            // - 미입력시 기본값 false 처리
            bool ReserveYN = false;

            // 개인조회 여부 (false , true 중 택 1)
            // └ false = 접수한 문자 전체 조회 (관리자권한)
            // └ true = 해당 담당자 계정으로 접수한 문자만 조회 (개인권한)
            // - 미입력시 기본값 false 처리
            bool SenderYN = false;

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
                var response = _messageService.Search(corpNum, SDate, EDate, State, Item, ReserveYN, SenderYN, Page,
                    PerPage, Order, QString);
                return View("Search", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        /*
         * 문자 전송내역 확인 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/info#GetSentListURL
         */
        public IActionResult GetSentListURL()
        {
            try
            {
                var result = _messageService.GetSentListURL(corpNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 전용 080 번호에 등록된 수신거부 목록을 반환합니다.
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/info#GetAutoDenyList
         */
        public IActionResult GetAutoDenyList()
        {
            try
            {
                var Response = _messageService.GetAutoDenyList(corpNum);
                return View("GetAutoDenyList", Response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌회원에 등록된 080 수신거부 번호 정보를 확인합니다.
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/info#CheckAutoDenyNumber
         */
        public IActionResult CheckAutoDenyNumber()
        {
            try
            {
                var response = _messageService.CheckAutoDenyNumber(corpNum);
                return View("CheckAutoDenyNumber", response);
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
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/point#GetBalance
         */
        public IActionResult GetBalance()
        {
            try
            {
                var result = _messageService.GetBalance(corpNum);
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
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/point#GetChargeURL
         */
        public IActionResult GetChargeURL()
        {
            try
            {
                var result = _messageService.GetChargeURL(corpNum, userID);
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
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/point#GetPaymentURL
         */
        public IActionResult GetPaymentURL()
        {

            try
            {
                var result = _messageService.GetPaymentURL(corpNum, userID);
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
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/point#GetUseHistoryURL
         */
        public IActionResult GetUseHistoryURL()
        {

            try
            {
                var result = _messageService.GetUseHistoryURL(corpNum, userID);
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
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/point#GetPartnerBalance
         */
        public IActionResult GetPartnerBalance()
        {
            try
            {
                var result = _messageService.GetPartnerBalance(corpNum);
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
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/point#GetPartnerURL
         */
        public IActionResult GetPartnerURL()
        {
            // CHRG 포인트충전 URL
            string TOGO = "CHRG";

            try
            {
                var result = _messageService.GetPartnerURL(corpNum, TOGO);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 문자 전송시 과금되는 포인트 단가를 확인합니다.
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/point#GetUnitCost
         */
        public IActionResult GetUnitCost()
        {
            try
            {
                // 문자 전송유형 SMS(단문), LMS(장문), MMS(포토)
                MessageType msgType = MessageType.SMS;

                var result = _messageService.GetUnitCost(corpNum, msgType);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌 문자 API 서비스 과금정보를 확인합니다.
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/point#GetChargeInfo
         */
        public IActionResult GetChargeInfo()
        {
            try
            {
                // 문자 전송유형 SMS(단문), LMS(장문), MMS(포토)
                MessageType msgType = MessageType.SMS;

                var response = _messageService.GetChargeInfo(corpNum, msgType);
                return View("GetChargeInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 포인트를 환불 신청합니다.
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/point#Refund
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

                var response = _messageService.Refund(corpNum, refundForm);
                return View("RefundResponse", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 포인트를 환불 신청합니다.
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/point#PaymentRequest
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

                var response = _messageService.PaymentRequest(corpNum, paymentForm);
                return View("PaymentResponse", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 포인트 무통장 입금신청내역 1건을 확인합니다.
         *  - https://developers.popbill.com/reference/sms/dotnetcore/api/point#GetSettleResult
         */
        public IActionResult GetSettleResult()
        {
            // 정산코드
            var settleCode = "202301130000000026";

            try
            {
                var paymentHistory = _messageService.GetSettleResult(corpNum, settleCode);

                return View("PaymentHistory", paymentHistory);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 포인트 사용내역을 확인합니다.
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/point#GetUseHistory
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
                var useHistoryResult = _messageService.GetUseHistory(corpNum, SDate, EDate, Page,
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
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/point#GetPaymentHistory
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
                var paymentHistoryResult = _messageService.GetPaymentHistory(corpNum, SDate, EDate,
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
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/point#GetRefundHistory
         */
        public IActionResult GetRefundHistory()
        {
            // 목록 페이지번호 (기본값 1)
            var Page = 1;

            // 페이지당 표시할 목록 개수 (기본값 500, 최대 1,000)
            var PerPage = 100;

            try
            {
                var refundHistoryResult = _messageService.GetRefundHistory(corpNum, Page, PerPage);

                return View("RefundHistoryResult", refundHistoryResult);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        /*
         * 포인트 환불에 대한 상세정보 1건을 확인합니다.
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/point#GetRefundInfo
         */
        public IActionResult GetRefundInfo()
        {
            // 환불 코드
            var refundCode = "023040000017";

            try
            {
                var response = _messageService.GetRefundInfo(corpNum, refundCode);
                return View("RefundHistory", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        /*
         * 환불 가능한 포인트를 확인합니다. (보너스 포인트는 환불가능포인트에서 제외됩니다.)
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/point#GetRefundableBalance
         */
        public IActionResult GetRefundableBalance()
        {
            try
            {
                var refundableBalance = _messageService.GetRefundableBalance(corpNum);
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
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/member#CheckIsMember
         */
        public IActionResult CheckIsMember()
        {
            try
            {
                //링크아이디
                string linkID = "TESTER";

                var response = _messageService.CheckIsMember(corpNum, linkID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 사용하고자 하는 아이디의 중복여부를 확인합니다.
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/member#CheckID
         */
        public IActionResult CheckID()
        {
            //중복여부 확인할 팝빌 회원 아이디
            string checkID = "testkorea";

            try
            {
                var response = _messageService.CheckID(checkID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 사용자를 연동회원으로 가입처리합니다.
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/member#JoinMember
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
                var response = _messageService.JoinMember(joinInfo);
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
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/member#GetAccessURL
         */
        public IActionResult GetAccessURL()
        {
            try
            {
                var result = _messageService.GetAccessURL(corpNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 회사정보를 확인합니다.
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/member#GetCorpInfo
         */
        public IActionResult GetCorpInfo()
        {
            try
            {
                var response = _messageService.GetCorpInfo(corpNum);
                return View("GetCorpInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 회사정보를 수정합니다.
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/member#UpdateCorpInfo
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
                var response = _messageService.UpdateCorpInfo(corpNum, corpInfo);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 담당자(팝빌 로그인 계정)를 추가합니다.
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/member#RegistContact
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
                var response = _messageService.RegistContact(corpNum, contactInfo);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 정보을 확인합니다.
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/member#GetContactInfo
         */
        public IActionResult GetContactInfo()
        {
            // 확인할 담당자 아이디
            string contactID = "test0730";

            try
            {
                var contactInfo = _messageService.GetContactInfo(corpNum, contactID);
                return View("GetContactInfo", contactInfo);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 목록을 확인합니다.
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/member#ListContact
         */
        public IActionResult ListContact()
        {
            try
            {
                var response = _messageService.ListContact(corpNum);
                return View("ListContact", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 정보를 수정합니다.
         * - https://developers.popbill.com/reference/sms/dotnetcore/api/member#UpdateContact
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
                var response = _messageService.UpdateContact(corpNum, contactInfo);
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
         *  - https://developers.popbill.com/reference/sms/dotnetcore/api/member#QuitMember
         */
        public IActionResult QuitMember()
        {
            // 탈퇴 사유
            var quitReason = "탈퇴사유";

            try
            {
                var response = _messageService.QuitMember(corpNum, quitReason);
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
