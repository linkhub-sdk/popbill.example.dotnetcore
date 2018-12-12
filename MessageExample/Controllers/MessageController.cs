using System;
using System.Collections.Generic;
using ControllerDI.Services;
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
            //문자 서비스 객체 생성
            _messageService = MSGinstance.messageService;

            //연동환경 설정값, 개발용(true), 상업용(false)
            _messageService.IsTest = true;
        }

        //팝빌 연동회원 사업자번호 (하이픈 '-' 없이 10자리)
        string corpNum = "1234567890";

        //팝빌 연동회원 아이디
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
         * 문자 발신번호 관리 팝업 URL을 반합니다.
         * - 반환된 URL은 보안정책에 따라 30초의 유효시간을 갖습니다.
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
         * 문자 발신번호 목록을 반환합니다.
         */
        public IActionResult GetSenderNumberList()
        {
            try
            {
                var response = _messageService.GetSenderNumberList(corpNum, userID);
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
         * SMS(단문)를 전송합니다.
         *  - 메시지 길이가 90 byte 이상인 경우, 길이를 초과하는 메시지 내용은 자동으로 제거됩니다.
         */
        public IActionResult SendSMS()
        {
            // 발신번호 
            string senderNum = "07043042992";

            // 발신자명
            string senderName = "발신자명";

            // 수신번호
            string receiverNum = "010111222";

            // 수신자명 
            string receiverName = "수신자명";

            // 메시지내용, 90byte초과된 내용은 삭제되어 전송됨. 
            string contents = "단문 문자 메시지 내용. 90byte 초과시 삭제되어 전송";

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;
            
            // 광고문자여부 (기본값 false)
            // [참고] "광고메시지 전송방법 안내” [ http://blog.linkhub.co.kr/2642/ ]
            bool adsYN = false;

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
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
         * [대량전송] SMS(단문)를 전송합니다.
         *  - 메시지 길이가 90 byte 이상인 경우, 길이를 초과하는 메시지 내용은 자동으로 제거됩니다.
         *  - 대량전송/부분전송에 대한 설명은 "[문자 API 연동매뉴얼] > 3.2.1 SendSMS(단문전송)"을 참조하시기 바랍니다.
         */
        public IActionResult SendSMS_Multi()
        {
            // 발신번호 
            string senderNum = "07043042992";

            // 메시지 구성
            List<Message> messages = new List<Message>();

            for (int i = 0; i < 100; i++)
            {
                Message msg = new Message();

                // 발신자명
                msg.senderName = "발신자명";

                // 수신번호
                msg.receiveNum = "010111222";

                // 수신자명
                msg.receiveName = "수신자명칭_" + i;

                // 메시지 내용, 90byte초과된 내용은 삭제되어 전송됨. 
                msg.content = "단문 문자메시지 내용, 각 메시지마다 개별설정 가능." + i;

                messages.Add(msg);
            }

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

            // 광고문자여부 (기본값 false)
            // [참고] "광고메시지 전송방법 안내” [ http://blog.linkhub.co.kr/2642/ ]
            bool adsYN = false;

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
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
         * [동보전송] SMS(단문)를 전송합니다.
         *  - 메시지 길이가 90 byte 이상인 경우, 길이를 초과하는 메시지 내용은 자동으로 제거됩니다.
         *  - 대량전송/부분전송에 대한 설명은 "[문자 API 연동매뉴얼] > 3.2.1 SendSMS(단문전송)"을 참조하시기 바랍니다.
         */
        public IActionResult SendSMS_Same()
        {
            // 발신번호 
            string senderNum = "07043042992";

            // (동보) 메시지내용, 90byte초과된 내용은 삭제되어 전송됨. 
            string contents = "단문 문자 메시지 내용. 90byte 초과시 삭제되어 전송";

            // 메시지 구성
            List<Message> messages = new List<Message>();

            for (int i = 0; i < 100; i++)
            {
                Message msg = new Message();

                // 발신자명
                msg.senderName = "발신자명";

                // 수신번호
                msg.receiveNum = "010111222";

                // 수신자명
                msg.receiveName = "수신자명칭_" + i;

                messages.Add(msg);
            }

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

            // 광고문자여부 (기본값 false)
            // [참고] "광고메시지 전송방법 안내” [ http://blog.linkhub.co.kr/2642/ ]
            bool adsYN = false;

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
            string requestNum = "";

            try
            {
                var receiptNum =
                    _messageService.SendSMS(corpNum, senderNum, contents, messages, sndDT, adsYN, requestNum);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        /*
         * LMS(장문)를 전송합니다.
         *  - 메시지 길이가 2,000 byte 이상인 경우, 길이를 초과하는 메시지 내용은 자동으로 제거됩니다.
         */
        public IActionResult SendLMS()
        {
            // 발신번호 
            string senderNum = "07043042992";

            // 발신자명
            string senderName = "발신자명";

            // 수신번호
            string receiverNum = "010111222";

            // 수신자명 
            string receiverName = "수신자명";

            // 메시지제목
            string subject = "장문 문자 메시지 제목";

            // 메시지내용, 최대 2000byte 초과된 내용은 삭제되어 전송됨.
            string contents = "장문 문자 메시지 내용. 최대 2000byte 초과된 내용은 삭제되어 전송.";

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

            // 광고문자여부 (기본값 false)
            // [참고] "광고메시지 전송방법 안내” [ http://blog.linkhub.co.kr/2642/ ]
            bool adsYN = false;

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
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
         * [대량전송] LMS(장문)를 전송합니다.
         *  - 메시지 길이가 2,000 byte 이상인 경우, 길이를 초과하는 메시지 내용은 자동으로 제거됩니다.
         *  - 대량전송/부분전송에 대한 설명은 "[문자 API 연동매뉴얼] > 3.2.2 SendLMS(장문전송)"을 참조하시기 바랍니다.
         */
        public IActionResult SendLMS_Multi()
        {
            // 발신번호 
            string senderNum = "07043042992";

            // 메시지 구성
            List<Message> messages = new List<Message>();

            for (int i = 0; i < 100; i++)
            {
                Message msg = new Message();

                // 발신자명
                msg.senderName = "발신자명";

                // 수신번호
                msg.receiveNum = "010111222";

                // 수신자명
                msg.receiveName = "수신자명칭_" + i;

                // 메시지 제목
                msg.subject = "메시지 제목" + i;

                // 메시지내용, 최대 2000byte 초과된 내용은 삭제되어 전송됨.
                msg.content = "장문 문자 메시지 내용. 최대 2000byte 초과된 내용은 삭제되어 전송." + i;

                messages.Add(msg);
            }

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

            // 광고문자여부 (기본값 false)
            // [참고] "광고메시지 전송방법 안내” [ http://blog.linkhub.co.kr/2642/ ]
            bool adsYN = false;

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
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
         * [동보전송] LNS(장문)를 전송합니다.
         *  - 메시지 길이가 2,000 byte 이상인 경우, 길이를 초과하는 메시지 내용은 자동으로 제거됩니다.
         *  - 대량전송/부분전송에 대한 설명은 "[문자 API 연동매뉴얼] > 3.2.2 SendLMS(장문전송)"을 참조하시기 바랍니다.
         */
        public IActionResult SendLMS_Same()
        {
            // 발신번호 
            string senderNum = "07043042992";

            // 메시지제목
            string subject = "장문 문자 메시지 제목";

            // (동보) 메시지내용, 최대 2000byte 초과된 내용은 삭제되어 전송됨.
            string contents = "장문 문자 메시지 내용. 최대 2000byte 초과된 내용은 삭제되어 전송.";

            // 메시지 구성
            List<Message> messages = new List<Message>();

            for (int i = 0; i < 100; i++)
            {
                Message msg = new Message();

                // 발신자명
                msg.senderName = "발신자명";

                // 수신번호
                msg.receiveNum = "010111222";

                // 수신자명
                msg.receiveName = "수신자명칭_" + i;

                messages.Add(msg);
            }

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

            // 광고문자여부 (기본값 false)
            // [참고] "광고메시지 전송방법 안내” [ http://blog.linkhub.co.kr/2642/ ]
            bool adsYN = false;

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
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
         * MMS(포토)를 전송합니다.
         *  - 메시지 길이가 2,000 byte 이상인 경우, 길이를 초과하는 메시지 내용은 자동으로 제거됩니다.
         *  - 이미지 파일의 크기는 최대 300Kbtye (JPEG), 가로/세로 1500px 이하 권장
         */
        public IActionResult SendMMS()
        {
            // 발신번호 
            string senderNum = "07043042992";

            // 발신자명
            string senderName = "발신자명";

            // 수신번호
            string receiverNum = "010111222";

            // 수신자명 
            string receiverName = "수신자명";

            // 메시지제목
            string subject = "포토 문자 메시지 제목";

            // 메시지내용, 최대 2000byte 초과된 내용은 삭제되어 전송됨.
            string contents = "포토 문자 메시지 내용. 최대 2000byte 초과된 내용은 삭제되어 전송.";

            // 첨부파일 경로
            string filePath = "C:\\popbill.example.dotnetcore\\MessageExample\\wwwroot\\images\\image.jpg";

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

            // 광고문자여부 (기본값 false)
            // [참고] "광고메시지 전송방법 안내” [ http://blog.linkhub.co.kr/2642/ ]
            bool adsYN = false;

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
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
         * [대랑전송] MMS(포토)를 전송합니다.
         *  - 메시지 길이가 2,000 byte 이상인 경우, 길이를 초과하는 메시지 내용은 자동으로 제거됩니다.
         *  - 이미지 파일의 크기는 최대 300Kbtye (JPEG), 가로/세로 1500px 이하 권장
         */
        public IActionResult SendMMS_Multi()
        {
            // 발신번호 
            string senderNum = "07043042992";

            // 메시지 구성
            List<Message> messages = new List<Message>();

            for (int i = 0; i < 100; i++)
            {
                Message msg = new Message();

                // 발신자명
                msg.senderName = "발신자명";

                // 수신번호
                msg.receiveNum = "010111222";

                // 수신자명
                msg.receiveName = "수신자명칭_" + i;

                // 메시지 제목
                msg.subject = "메시지 제목" + i;

                // 메시지내용, 최대 2000byte 초과된 내용은 삭제되어 전송됨.
                msg.content = "포토 문자 메시지 내용. 최대 2000byte 초과된 내용은 삭제되어 전송." + i;

                messages.Add(msg);
            }

            // 첨부파일 경로
            string filePath = "C:\\popbill.example.dotnetcore\\MessageExample\\wwwroot\\images\\image.jpg";

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

            // 광고문자여부 (기본값 false)
            // [참고] "광고메시지 전송방법 안내” [ http://blog.linkhub.co.kr/2642/ ]
            bool adsYN = false;

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
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
         * [동보전송] MMS(포토)를 전송합니다.
         *  - 메시지 길이가 2,000 byte 이상인 경우, 길이를 초과하는 메시지 내용은 자동으로 제거됩니다.
         *  - 이미지 파일의 크기는 최대 300Kbtye (JPEG), 가로/세로 1500px 이하 권장
         */
        public IActionResult SendMMS_Same()
        {
            // 발신번호 
            string senderNum = "07043042992";

            // 메시지제목
            string subject = "포토 문자 메시지 제목";

            // (동보) 메시지내용, 최대 2000byte 초과된 내용은 삭제되어 전송됨.
            string contents = "포토 문자 메시지 내용. 최대 2000byte 초과된 내용은 삭제되어 전송.";

            // 메시지 구성
            List<Message> messages = new List<Message>();

            for (int i = 0; i < 100; i++)
            {
                Message msg = new Message();

                // 발신자명
                msg.senderName = "발신자명";

                // 수신번호
                msg.receiveNum = "010111222";

                // 수신자명
                msg.receiveName = "수신자명칭_" + i;

                messages.Add(msg);
            }

            // 첨부파일 경로
            string filePath = "C:\\popbill.example.dotnetcore\\MessageExample\\wwwroot\\images\\image.jpg";

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

            // 광고문자여부 (기본값 false)
            // [참고] "광고메시지 전송방법 안내” [ http://blog.linkhub.co.kr/2642/ ]
            bool adsYN = false;

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
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
         * XMS(단문/장문 자동인식)를 전송합니다.
         *  - 메시지 내용의 길이(90byte)에 따라 SMS/LMS(단문/장문)를 자동인식하여 전송합니다.
         */
        public IActionResult SendXMS()
        {
            // 발신번호 
            string senderNum = "07043042992";

            // 발신자명
            string senderName = "발신자명";

            // 수신번호
            string receiverNum = "010111222";

            // 수신자명 
            string receiverName = "수신자명";

            // 메시지제목
            string subject = "단문/장문 문자 메시지 제목";

            // 메시지내용, 메시지 내용의 길이(90byte)에 따라 SMS/LMS(단문/장문)를 자동인식하여 전송됨.
            string contents = "단문/장문 문자 메시지 내용. 메시지 내용의 길이(90byte)에 따라 SMS/LMS(단문/장문)를 자동인식하여 전송됨";

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

            // 광고문자여부 (기본값 false)
            // [참고] "광고메시지 전송방법 안내” [ http://blog.linkhub.co.kr/2642/ ]
            bool adsYN = false;

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
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
         * [대량전송] XMS(단문/장문 자동인식)를 전송합니다.
         *  - 메시지 내용의 길이(90byte)에 따라 SMS/LMS(단문/장문)를 자동인식하여 전송합니다.
         *  - 대량전송/부분전송에 대한 설명은 "[문자 API 연동매뉴얼] > 3.2.4 SendXMS(단문/장문 자동인식 전송)"을 참조하시기 바랍니다.
         */
        public IActionResult SendXMS_Multi()
        {
            // 발신번호 
            string senderNum = "07043042992";

            // 메시지 구성
            List<Message> messages = new List<Message>();

            for (int i = 0; i < 100; i++)
            {
                Message msg = new Message();

                // 발신자명
                msg.senderName = "발신자명";

                // 수신번호
                msg.receiveNum = "010111222";

                // 수신자명
                msg.receiveName = "수신자명칭_" + i;

                // 메시지 제목
                msg.subject = "메시지 제목" + i;

                // 메시지내용, 메시지 내용의 길이(90byte)에 따라 SMS/LMS(단문/장문)를 자동인식하여 전송됨.
                msg.content = "단문/장문 문자 메시지 내용. 메시지 내용의 길이(90byte)에 따라 SMS/LMS(단문/장문)를 자동인식하여 전송됨" + i;

                messages.Add(msg);
            }

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

            // 광고문자여부 (기본값 false)
            // [참고] "광고메시지 전송방법 안내” [ http://blog.linkhub.co.kr/2642/ ]
            bool adsYN = false;

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
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
         * [동보전송] XMS(단문/장문 자동인식)를 전송합니다.
         *  - 메시지 내용의 길이(90byte)에 따라 SMS/LMS(단문/장문)를 자동인식하여 전송합니다.
         *  - 대량전송/부분전송에 대한 설명은 "[문자 API 연동매뉴얼] > 3.2.4 SendXMS(단문/장문 자동인식 전송)"을 참조하시기 바랍니다.
         */
        public IActionResult SendXMS_Same()
        {
            // 발신번호 
            string senderNum = "07043042992";

            // 메시지제목
            string subject = "단문/장문 문자 메시지 제목";

            // (동보) 메시지내용, 메시지 내용의 길이(90byte)에 따라 SMS/LMS(단문/장문)를 자동인식하여 전송됨.
            string contents = "단문/장문 문자 메시지 내용. 메시지 내용의 길이(90byte)에 따라 SMS/LMS(단문/장문)를 자동인식하여 전송됨";

            // 메시지 구성
            List<Message> messages = new List<Message>();

            for (int i = 0; i < 100; i++)
            {
                Message msg = new Message();

                // 발신자명
                msg.senderName = "발신자명";

                // 수신번호
                msg.receiveNum = "010111222";

                // 수신자명
                msg.receiveName = "수신자명칭_" + i;

                messages.Add(msg);
            }

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

            // 광고문자여부 (기본값 false)
            // [참고] "광고메시지 전송방법 안내” [ http://blog.linkhub.co.kr/2642/ ]
            bool adsYN = false;

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
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

        #endregion

        #region 전송내역조회/요약정보확인

        /*
         * 문자 전송내역 요약정보를 확인합니다.
         */
        public IActionResult GetStates()
        {
            // 요약정보 확인할 문자 접수번호 배열
            List<string> receiptNumList = new List<string>();
            receiptNumList.Add("018090410000000416");
            receiptNumList.Add("018090410000000395");

            try
            {
                var response = _messageService.GetStates(corpNum, receiptNumList, userID);
                return View("GetStates", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 검색조건을 사용하여 문자전송 내역을 조회합니다.
         * - 최대 검색기간 : 6개월 이내
         */
        public IActionResult Search()
        {
            // 최대 검색기간 : 6개월 이내
            // 시작일자, 날짜형식(yyyyMMdd)
            string SDate = "20181101";

            // 종료일자, 날짜형식(yyyyMMdd)
            string EDate = "20181126";

            // 전송상태값 배열, 1-대기, 2-성공, 3-실패, 4-취소
            string[] State = new string[4];
            State[0] = "1";
            State[1] = "2";
            State[2] = "3";
            State[3] = "4";

            // 검색대상 배열, SMS, LMS, MMS
            string[] Item = new string[3];
            Item[0] = "SMS";
            Item[1] = "LMS";
            Item[2] = "MMS";

            // 예약여부, true-예약전송건 조회, false-즉시전송건 조회 
            bool ReserveYN = false;

            // 개인조회여부 true-개인조회, false-전체조회 
            bool SenderYN = false;

            // 페이지 번호, 기본값 '1'
            int Page = 1;

            // 페이지당 검색개수, 기본값 '500', 최대 '1000' 
            int PerPage = 30;

            // 정렬방향, D-내림차순, A-오름차순
            string Order = "D";

            // 조회 검색어, 문자 전송시 기재한 수신자명 또는 발신자명 기재
            string QString = "";

            try
            {
                var response = _messageService.Search(corpNum, SDate, EDate, State, Item, ReserveYN, SenderYN, Page,
                    PerPage, Order, QString, userID);
                return View("Search", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        /*
         * 문자 전송내역 팝업 URL을 반환합니다.
         * - 반환된 URL은 보안정책에 따라 30초의 유효시간을 갖습니다.
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
         * 080 서비스 수신거부 목록을 확인합니다.
         */
        public IActionResult GetAutoDenyList()
        {
            try
            {
                var Response = _messageService.GetAutoDenyList(corpNum, userID);
                return View("GetAutoDenyList", Response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        #endregion

        #region 접수번호 관련 기능 (요청번호 미할당)

        /*
         * 문자전송요청시 발급받은 접수번호(receiptNum)로 전송상태를 확인합니다
         * - 응답항목에 대한 자세한 사항은 "[문자 API 연동매뉴얼] >  3.3.1. GetMessages (전송내역 확인)을 참조하시기 바랍니다.
         */
        public IActionResult GetMessages()
        {
            // 문자 전송요청시 발급받은 접수번호
            string receiptNum = "018112714000000020";

            try
            {
                var Response = _messageService.GetMessages(corpNum, receiptNum, userID);
                return View("GetMessages", Response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 문자전송요청시 발급받은 접수번호(receiptNum)로 예약문자 전송을 취소합니다.
         * - 예약취소는 예약전송시간 10분전까지만 가능합니다.
         */
        public IActionResult CancelReserve()
        {
            // 문자 전송요청시 발급받은 접수번호
            string receiptNum = "018112714000000020";

            try
            {
                var Response = _messageService.CancelReserve(corpNum, receiptNum, userID);
                return View("Response", Response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        #endregion

        #region 요청번호 할당 전송건 관련 기능

        /*
         * 문자전송요청시 할당한 전송요청번호(requestNum)로 전송상태를 확인합니다
         * - 응답항목에 대한 자세한 사항은 "[문자 API 연동매뉴얼] > 3.3.2. GetMessagesRN (전송내역 확인 - 요청번호 할당)을 참조하시기 바랍니다.
         */
        public IActionResult GetMessagesRN()
        {
            // 문자 전송요청시 할당한 요청번호
            string requestNum = "20181127141839";

            try
            {
                var Response = _messageService.GetMessagesRN(corpNum, requestNum, userID);
                return View("GetMessages", Response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 문자전송요청시 할당한 전송요청번호(requestNum)로 예약문자 전송을 취소합니다.
         * - 예예약취소는 예약전송시간 10분전까지만 가능합니다.
         */
        public IActionResult CancelReserveRN()
        {
            // 문자 전송요청시 할당한 요청번호
            string requestNum = "20181127141839";

            try
            {
                var Response = _messageService.CancelReserveRN(corpNum, requestNum, userID);
                return View("Response", Response);
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
                var result = _messageService.GetBalance(corpNum);
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
                var result = _messageService.GetChargeURL(corpNum, userID);
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
                var result = _messageService.GetPartnerBalance(corpNum);
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
                var result = _messageService.GetPartnerURL(corpNum, TOGO);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 문자서비스 전송단가를 확인합니다.
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
         * 문자서비스 API 서비스 과금정보를 확인합니다.
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

                var response = _messageService.CheckIsMember(corpNum, linkID);
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
                var response = _messageService.CheckID(checkID);
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
            
            // 아이디 6자이상 50자 미만
            joinInfo.ID = "userid_20181212"; 
            
            // 비밀번호 6자이상 20자 미만
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
                var response = _messageService.JoinMember(joinInfo);
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
         */
        public IActionResult GetCorpInfo()
        {
            try
            {
                var response = _messageService.GetCorpInfo(corpNum, userID);
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
                var response = _messageService.UpdateCorpInfo(corpNum, corpInfo, userID);
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
                var response = _messageService.RegistContact(corpNum, contactInfo, userID);
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
                var response = _messageService.ListContact(corpNum, userID);
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
                var response = _messageService.UpdateContact(corpNum, contactInfo, userID);
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