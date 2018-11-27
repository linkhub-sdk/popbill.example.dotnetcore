using System;
using System.Collections.Generic;
using ControllerDI.Services;
using Microsoft.AspNetCore.Mvc;
using Popbill;
using Popbill.Kakao;

namespace KakaoExample.Controllers
{
    public class KakaoController : Controller
    {
        private readonly KakaoService _kakaoService;

        //링크허브에서 발급받은 고객사 고객사 인증정보로 링크아이디(LinkID)와 비밀키(SecretKey) 값을 변경하시기 바랍니다.
        private string linkID = "TESTER";
        private string secretKey = "SwWxqU+0TErBXy/9TVjIPEnI0VTUMMSQZtJf3Ed8q3I=";

        public KakaoController(KakaoInstance KKOinstance)
        {
            //카카오 서비스 객체 생성
            _kakaoService = KKOinstance.kakaoService;

            //연동환경 설정값, 개발용(true), 상업용(false)
            _kakaoService.IsTest = true;
        }

        //팝빌 연동회원 사업자번호 (하이픈 '-' 없이 10자리)
        string corpNum = "1234567890";

        //팝빌 연동회원 아이디
        string userID = "testkorea";

        /*
         * 카카오 Index page (Kakao/Index.cshtml)
         */
        public IActionResult Index()
        {
            return View();
        }

        #region 친구톡/알림톡/발신번호 관리

        /*
         * 플러스친구 계정관리 팝업 URL을 반환합니다.
         * - 반환된 URL은 보안정책에 따라 30초의 유효시간을 갖습니다.
         */
        public IActionResult GetPlusFriendMgtURL()
        {
            try
            {
                var result = _kakaoService.GetPlusFriendMgtURL(corpNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌에 등록된 플러스친구 계정목록을 확인합니다.
         */
        public IActionResult ListPlusFriendID()
        {
            try
            {
                var response = _kakaoService.ListPlusFriendID(corpNum, userID);
                return View("ListPlusFriendID", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 발신번호 관리 팝업 URL을 반합니다.
         * - 반환된 URL은 보안정책에 따라 30초의 유효시간을 갖습니다.
         */
        public IActionResult GetSenderNumberMgtURL()
        {
            try
            {
                var result = _kakaoService.GetSenderNumberMgtURL(corpNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌에 등록된 발신번호 목록을 확인합니다.
         */
        public IActionResult GetSenderNumberList()
        {
            try
            {
                var response = _kakaoService.GetSenderNumberList(corpNum, userID);
                return View("GetSenderNumberList", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 알림톡 템플릿관리 팝업 URL을 반환합니다.
         * - 반환된 URL은 보안정책에 따라 30초의 유효시간을 갖습니다.
         */
        public IActionResult GetATSTemplateMgtURL()
        {
            try
            {
                var result = _kakaoService.GetATSTemplateMgtURL(corpNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * (주)카카오로 부터 승인된 알림톡 템플릿 목록을 확인합니다.
         * - 반환항목중 템플릿코드(templateCode)는 알림톡 전송시 사용됩니다.
         */
        public IActionResult ListATSTemplate()
        {
            try
            {
                var response = _kakaoService.ListATSTemplate(corpNum, userID);
                return View("ListATSTemplate", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        #endregion

        #region 친구톡/알림톡 전송

        /*
         * 알림톡 전송을 요청합니다.
         * 사전에 승인된 템플릿의 내용과 알림톡 전송내용(altMsg)이 다를 경우 전송실패 처리됩니다.
         */
        public IActionResult SendATS_One()
        {
            // 알림톡 템플릿 코드, ListATSTemplate API의 templateCode 확인
            string templateCode = "018030000066";

            // 발신번호
            string senderNum = "07043042992";

            // 수신번호
            string receiverNum = "010111222";

            // 수신자명
            string receiverName = "수신자명";

            // 알림톡 템플릿 내용 (최대 1000자)
            string content = "[테스트] 테스트 템플릿입니다.";

            // 대체문자 메시지 내용 (최대 2000byte)
            string altContent = "대체문자 메시지 내용";

            // 대체문자 유형, 공백-미전송, C-알림톡 내용, A-대체문자 내용
            string altSendType = "A";

            // 예약전송일시(yyyyMMddHHmmss) ex) 20181126121206, null인 경우 즉시전송
            DateTime sndDT = new DateTime(20181126121925);

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
            string requestNum = "";

            try
            {
                var receiptNum = _kakaoService.SendATS(corpNum, templateCode, senderNum, receiverNum, receiverName,
                    content, altContent, altSendType, sndDT, requestNum);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * [대량전송] 알림톡 전송을 요청합니다.
         * 사전에 승인된 템플릿의 내용과 알림톡 전송내용(altMsg)이 다를 경우 전송실패 처리됩니다.
         */
        public IActionResult SendATS_Multi()
        {
            // 알림톡 템플릿 코드, ListATSTemplate API의 templateCode 확인
            string templateCode = "018030000066";

            // 발신번호
            string senderNum = "07043042992";

            // 수신자정보 배열, 최대 1000건
            List<KakaoReceiver> receivers = new List<KakaoReceiver>();
            for (int i = 0; i < 5; i++)
            {
                KakaoReceiver receiverInfo = new KakaoReceiver();

                // 수신번호
                receiverInfo.rcv = "01011122" + i;

                // 수신자명
                receiverInfo.rcvnm = "수신자명" + i;

                // 알림톡 템플릿 내용 (최대 1000자)
                receiverInfo.msg = "[테스트] 테스트 템플릿입니다.";

                // 대체문자 내용 (최대 2000byte)
                receiverInfo.altmsg = "대체문자 내용입니다";

                receivers.Add(receiverInfo);
            }

            // 대체문자 유형, 공백-미전송, C-알림톡 내용, A-대체문자 내용
            string altSendType = "A";

            // 예약전송일시(yyyyMMddHHmmss) ex) 20181126121206, null인 경우 즉시전송
            DateTime sndDT = new DateTime(20181126121925);

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
            string requestNum = "";

            try
            {
                var receiptNum = _kakaoService.SendATS(corpNum, templateCode, senderNum, receivers, altSendType, sndDT,
                    requestNum);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * [동보전송] 알림톡 전송을 요청합니다.
         * 사전에 승인된 템플릿의 내용과 알림톡 전송내용(altMsg)이 다를 경우 전송실패 처리됩니다.
         */
        public IActionResult SendATS_Same()
        {
            // 알림톡 템플릿 코드, ListATSTemplate API의 templateCode 확인
            string templateCode = "018030000066";

            // 발신번호
            string senderNum = "07043042992";

            // (동보) 알림톡 템플릿 내용 (최대 1000자)
            string content = "[테스트] 테스트 템플릿입니다.";

            // (동보) 대체문자 메시지 내용 (최대 2000byte)
            string altContent = "대체문자 메시지 내용";

            // 수신자정보 배열, 최대 1000건
            List<KakaoReceiver> receivers = new List<KakaoReceiver>();
            for (int i = 0; i < 5; i++)
            {
                KakaoReceiver receiverInfo = new KakaoReceiver();

                // 수신번호
                receiverInfo.rcv = "01011122" + i;

                // 수신자명
                receiverInfo.rcvnm = "수신자명" + i;

                receivers.Add(receiverInfo);
            }

            // 대체문자 유형, 공백-미전송, C-알림톡 내용, A-대체문자 내용
            string altSendType = "A";

            // 예약전송일시(yyyyMMddHHmmss) ex) 20181126121206, null인 경우 즉시전송
            DateTime sndDT = new DateTime(20181126121925);

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
            string requestNum = "";

            try
            {
                var receiptNum = _kakaoService.SendATS(corpNum, templateCode, senderNum, content, altContent, receivers,
                    altSendType, sndDT, requestNum);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 친구톡(텍스트) 전송을 요청합니다.
         * - 친구톡은 심야 전송(20:00~08:00)이 제한됩니다.
         */
        public IActionResult SendFTS_One()
        {
            // 플러스친구 아이디, ListPlusFriendID API 의 plusFriendID 참고
            string plusFriendID = "@팝빌";

            // 발신번호
            string senderNum = "07043042991";

            // 수신번호
            string receiverNum = "010111222";

            // 수신자명
            string receiverName = "수신자명";

            // 친구톡 내용 (최대 1000자)
            string content = "친구톡 내용";

            // 대체문자 메시지 내용 (최대 2000byte)
            string altContent = "대체문자 내용";

            // 버튼배열, 최대 5개
            List<KakaoButton> buttons = new List<KakaoButton>();
            KakaoButton btnInfo = new KakaoButton
            {
                n = "버튼이름", // 버튼명
                t = "WL", // 버튼 유형, WL-웹링크, AL-앱링크, MD-메시지 전달, BK-봇키워드
                u1 = "http://www.popbill.com", // [앱링크] Android, [웹링크] Mobile
                u2 = "http://test.popbill.com" // [앱링크] IOS, [웹링크] PC URL
            };
            buttons.Add(btnInfo);

            // 대체문자 유형, 공백-미전송, C-알림톡 내용, A-대체문자 내용
            string altSendType = "A";

            // 광고전송여부
            bool adsYN = false;

            // 예약전송일시(yyyyMMddHHmmss) ex) 20181126121206, null인 경우 즉시전송
            DateTime sndDT = new DateTime(20181126121925);

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
            string requestNum = "";

            try
            {
                var receiptNum = _kakaoService.SendFTS(corpNum, plusFriendID, senderNum, receiverNum, receiverName,
                    content, altContent, buttons, altSendType, adsYN, sndDT, requestNum);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * [대량전송] 친구톡(텍스트) 전송을 요청합니다.
         * - 친구톡은 심야 전송(20:00~08:00)이 제한됩니다.
         */
        public IActionResult sendFTS_Multi()
        {
            // 플러스친구 아이디, ListPlusFriendID API 의 plusFriendID 참고
            string plusFriendID = "@팝빌";

            // 발신번호
            string senderNum = "07043042991";

            // 수신자정보 배열, 최대 1000건
            List<KakaoReceiver> receivers = new List<KakaoReceiver>();

            for (int i = 0; i < 5; i++)
            {
                KakaoReceiver receiverInfo = new KakaoReceiver();

                // 수신번호
                receiverInfo.rcv = "01011122" + i;

                // 수신자명
                receiverInfo.rcvnm = "수신자명" + i;

                // 친구톡 내용 (최대 1000자)
                receiverInfo.msg = "친구톡 내용입니다.";

                // 대체문자 내용 (최대 2000byte)
                receiverInfo.altmsg = "대체문자 내용입니다";

                receivers.Add(receiverInfo);
            }

            // 버튼배열, 최대 5개
            List<KakaoButton> buttons = new List<KakaoButton>();
            KakaoButton btnInfo = new KakaoButton
            {
                n = "버튼이름", // 버튼명
                t = "WL", // 버튼 유형, WL-웹링크, AL-앱링크, MD-메시지 전달, BK-봇키워드
                u1 = "http://www.popbill.com", // [앱링크] Android, [웹링크] Mobile
                u2 = "http://test.popbill.com" // [앱링크] IOS, [웹링크] PC URL
            };
            buttons.Add(btnInfo);

            // 대체문자 유형, 공백-미전송, C-알림톡 내용, A-대체문자 내용
            string altSendType = "A";

            // 광고전송여부
            bool adsYN = false;

            // 예약전송일시(yyyyMMddHHmmss) ex) 20181126121206, null인 경우 즉시전송
            DateTime sndDT = new DateTime(20181126121925);

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
            string requestNum = "";

            try
            {
                var receiptNum = _kakaoService.SendFTS(corpNum, plusFriendID, senderNum, receivers, buttons,
                    altSendType, adsYN, sndDT, requestNum);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * [동보전송] 친구톡(텍스트) 전송을 요청합니다.
         * - 친구톡은 심야 전송(20:00~08:00)이 제한됩니다.
         */
        public IActionResult sendFTS_Same()
        {
            // 플러스친구 아이디, ListPlusFriendID API 의 plusFriendID 참고
            string plusFriendID = "@팝빌";

            // 발신번호
            string senderNum = "07043042991";

            // (동보) 친구톡 내용 (최대 1000자)
            string content = "친구톡 내용 입니다.";

            // (동보) 대체문자 메시지 내용 (최대 2000byte)
            string altContent = "대체문자 메시지 내용";

            // 수신자정보 배열, 최대 1000건
            List<KakaoReceiver> receivers = new List<KakaoReceiver>();
            for (int i = 0; i < 5; i++)
            {
                KakaoReceiver receiverInfo = new KakaoReceiver();

                // 수신번호
                receiverInfo.rcv = "01011122" + i;

                // 수신자명
                receiverInfo.rcvnm = "수신자명" + i;

                receivers.Add(receiverInfo);
            }

            // 버튼배열, 최대 5개
            List<KakaoButton> buttons = new List<KakaoButton>();
            KakaoButton btnInfo = new KakaoButton
            {
                n = "버튼이름", // 버튼명
                t = "WL", // 버튼 유형, WL-웹링크, AL-앱링크, MD-메시지 전달, BK-봇키워드
                u1 = "http://www.popbill.com", // [앱링크] Android, [웹링크] Mobile
                u2 = "http://test.popbill.com" // [앱링크] IOS, [웹링크] PC URL
            };
            buttons.Add(btnInfo);

            // 대체문자 유형, 공백-미전송, C-알림톡 내용, A-대체문자 내용
            string altSendType = "A";

            // 광고전송여부
            bool adsYN = false;

            // 예약전송일시(yyyyMMddHHmmss) ex) 20181126121206, null인 경우 즉시전송
            DateTime sndDT = new DateTime(20181126121925);

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
            string requestNum = "";

            try
            {
                var receiptNum = _kakaoService.SendFMS(corpNum, plusFriendID, senderNum, content, altContent, receivers,
                    buttons, altSendType, adsYN, sndDT, requestNum);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 친구톡(이미지) 전송을 요청합니다.
         * - 친구톡은 심야 전송(20:00~08:00)이 제한됩니다.
         * - 이미지 전송규격 / jpg 포맷, 용량 최대 500KByte, 이미지 높이/너비 비율 1.333 이하, 1/2 이상
         */
        public IActionResult SendFMS_One()
        {
            // 플러스친구 아이디, ListPlusFriendID API 의 plusFriendID 참고
            string plusFriendID = "@팝빌";

            // 발신번호
            string senderNum = "07043042991";

            // 수신번호
            string receiverNum = "010111222";

            // 수신자명
            string receiverName = "수신자명";

            // 친구톡 내용 (최대 1000자)
            string content = "친구톡 내용";

            // 대체문자 메시지 내용 (최대 2000byte)
            string altContent = "대체문자 내용";

            // 버튼배열, 최대 5개
            List<KakaoButton> buttons = new List<KakaoButton>();
            KakaoButton btnInfo = new KakaoButton
            {
                n = "버튼이름", // 버튼명
                t = "WL", // 버튼 유형, WL-웹링크, AL-앱링크, MD-메시지 전달, BK-봇키워드
                u1 = "http://www.popbill.com", // [앱링크] Android, [웹링크] Mobile
                u2 = "http://test.popbill.com" // [앱링크] IOS, [웹링크] PC URL
            };
            buttons.Add(btnInfo);

            // 대체문자 유형, 공백-미전송, C-알림톡 내용, A-대체문자 내용
            string altSendType = "A";

            // 광고전송여부
            bool adsYN = false;

            // 예약전송일시(yyyyMMddHHmmss) ex) 20181126121206, null인 경우 즉시전송
            DateTime sndDT = new DateTime(20181126121925);

            // 이미지 링크 URL
            string imageURL = "https://www.popbill.com";

            // 이미지 파일경로
            string filePath = "";

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
            string requestNum = "";

            try
            {
                var receiptNum = _kakaoService.SendFTS(corpNum, plusFriendID, senderNum, receiverNum, receiverName,
                    content, altContent, buttons, altSendType, adsYN, sndDT, imageURL, filePath, requestNum);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * [대량전송] 친구톡(이미지) 전송을 요청합니다.
         * - 친구톡은 심야 전송(20:00~08:00)이 제한됩니다.
         * - 이미지 전송규격 / jpg 포맷, 용량 최대 500KByte, 이미지 높이/너비 비율 1.333 이하, 1/2 이상
         */
        public IActionResult SendFMS_Multi()
        {
            // 플러스친구 아이디, ListPlusFriendID API 의 plusFriendID 참고
            string plusFriendID = "@팝빌";

            // 발신번호
            string senderNum = "07043042991";

            // 수신자정보 배열, 최대 1000건
            List<KakaoReceiver> receivers = new List<KakaoReceiver>();

            for (int i = 0; i < 5; i++)
            {
                KakaoReceiver receiverInfo = new KakaoReceiver();

                // 수신번호
                receiverInfo.rcv = "01011122" + i;

                // 수신자명
                receiverInfo.rcvnm = "수신자명" + i;

                // 친구톡 내용 (최대 1000자)
                receiverInfo.msg = "친구톡 내용입니다.";

                // 대체문자 내용 (최대 2000byte)
                receiverInfo.altmsg = "대체문자 내용입니다";

                receivers.Add(receiverInfo);
            }

            // 버튼배열, 최대 5개
            List<KakaoButton> buttons = new List<KakaoButton>();
            KakaoButton btnInfo = new KakaoButton
            {
                n = "버튼이름", // 버튼명
                t = "WL", // 버튼 유형, WL-웹링크, AL-앱링크, MD-메시지 전달, BK-봇키워드
                u1 = "http://www.popbill.com", // [앱링크] Android, [웹링크] Mobile
                u2 = "http://test.popbill.com" // [앱링크] IOS, [웹링크] PC URL
            };
            buttons.Add(btnInfo);

            // 대체문자 유형, 공백-미전송, C-알림톡 내용, A-대체문자 내용
            string altSendType = "A";

            // 광고전송여부
            bool adsYN = false;

            // 예약전송일시(yyyyMMddHHmmss) ex) 20181126121206, null인 경우 즉시전송
            DateTime sndDT = new DateTime(20181126121925);

            // 이미지 링크 URL
            string imageURL = "https://www.popbill.com";

            // 이미지 파일경로
            string filePath = "";

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
            string requestNum = "";

            try
            {
                var receiptNum = _kakaoService.SendFTS(corpNum, plusFriendID, senderNum, receivers, buttons,
                    altSendType, adsYN, sndDT, imageURL, filePath, requestNum);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * [동보전송] 친구톡(이미지) 전송을 요청합니다.
         * - 친구톡은 심야 전송(20:00~08:00)이 제한됩니다.
         * - 이미지 전송규격 / jpg 포맷, 용량 최대 500KByte, 이미지 높이/너비 비율 1.333 이하, 1/2 이상
         */
        public IActionResult SendFMS_Same()
        {
            // 플러스친구 아이디, ListPlusFriendID API 의 plusFriendID 참고
            string plusFriendID = "@팝빌";

            // 발신번호
            string senderNum = "07043042991";

            // (동보) 친구톡 내용 (최대 1000자)
            string content = "친구톡 내용 입니다.";

            // (동보) 대체문자 메시지 내용 (최대 2000byte)
            string altContent = "대체문자 메시지 내용";

            // 수신자정보 배열, 최대 1000건
            List<KakaoReceiver> receivers = new List<KakaoReceiver>();
            for (int i = 0; i < 5; i++)
            {
                KakaoReceiver receiverInfo = new KakaoReceiver();

                // 수신번호
                receiverInfo.rcv = "01011122" + i;

                // 수신자명
                receiverInfo.rcvnm = "수신자명" + i;

                receivers.Add(receiverInfo);
            }

            // 버튼배열, 최대 5개
            List<KakaoButton> buttons = new List<KakaoButton>();
            KakaoButton btnInfo = new KakaoButton
            {
                n = "버튼이름", // 버튼명
                t = "WL", // 버튼 유형, WL-웹링크, AL-앱링크, MD-메시지 전달, BK-봇키워드
                u1 = "http://www.popbill.com", // [앱링크] Android, [웹링크] Mobile
                u2 = "http://test.popbill.com" // [앱링크] IOS, [웹링크] PC URL
            };
            buttons.Add(btnInfo);

            // 대체문자 유형, 공백-미전송, C-알림톡 내용, A-대체문자 내용
            string altSendType = "A";

            // 광고전송여부
            bool adsYN = false;

            // 예약전송일시(yyyyMMddHHmmss) ex) 20181126121206, null인 경우 즉시전송
            DateTime sndDT = new DateTime(20181126121925);

            // 이미지 링크 URL
            string imageURL = "https://www.popbill.com";

            // 이미지 파일경로
            string filePath = "";

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
            string requestNum = "";

            try
            {
                var receiptNum = _kakaoService.SendFMS(corpNum, plusFriendID, senderNum, content, altContent, receivers,
                    buttons, altSendType, adsYN, sndDT, imageURL, filePath, requestNum);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        #endregion

        #region 정보확인

        /*
         * 검색조건을 사용하여 카카오톡전송 내역을 조회합니다.
         * - 최대 검색기간 : 6개월 이내
         */
        public IActionResult Search()
        {
            // 최대 검색기한 : 6개월 이내
            // 시작일자, 날자형식(yyyyMMdd)
            string SDate = "20180601";

            // 종료일자, 날자형식(yyyyMMdd)
            string EDate = "20180731";

            // 전송상태값 배열, 0-대기, 1-전송중, -2-성공, 3-대체, 4-실패, 5-취소
            string[] State = new string[6];
            State[0] = "0";
            State[1] = "1";
            State[2] = "2";
            State[3] = "3";
            State[4] = "4";
            State[5] = "5";

            // 검색대상 배열, ATS-알림톡, FTS-친구톡 텍스트, FMS-친구톡 이미지
            string[] Item = new string[3];
            Item[0] = "ATS";
            Item[1] = "FTS";
            Item[2] = "FMS";

            // 예약여부, 공백-전체조회, 1-예약전송건 조회, 0-즉시전송 조회
            string ReserveYN = "";

            // 개인조회여부 true-개인조회, false-전체조회 
            bool SenderYN = false;

            // 페이지 번호, 기본값 '1'
            int Page = 1;

            // 페이지당 검색개수, 기본값 '500', 최대 '1000' 
            int PerPage = 30;

            // 정렬방향, D-내림차순, A-오름차순
            string Order = "D";

            // 조회 검색어, 카카오톡 전송시 기재한 수신자명 입력
            string QString = "";

            try
            {
                var response = _kakaoService.Search(corpNum, SDate, EDate, State, Item, ReserveYN, SenderYN, Page,
                    PerPage, Order, QString, userID);
                return View("Search", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 카카오톡 전송내역 팝업 URL을 반환합니다.
         * - 반환된 URL은 보안정책에 따라 30초의 유효시간을 갖습니다.
         */
        public IActionResult GetSentListURL()
        {
            try
            {
                var result = _kakaoService.GetSentListURL(corpNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 카카오톡 전송요청시 발급받은 접수번호(receiptNum)로 전송결과를 확인합니다.
         */
        public IActionResult GetMessages()
        {
            // 알림톡/친구톡 전송요청시 발급받은 접수번호
            string receiptNum = "018112717221800001";

            try
            {
                var Response = _kakaoService.GetMessages(corpNum, receiptNum, userID);
                return View("GetMessages", Response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 카카오톡 전송요청시 발급받은 접수번호(receiptNum)로 예약전송건을 취소합니다.
         * - 예약취소는 예약전송시간 10분전까지만 가능합니다.
         */
        public IActionResult CancelReserve()
        {
            // 알림톡/친구톡 전송요청시 발급받은 접수번호
            string receiptNum = "018112715514300001";

            try
            {
                var Response = _kakaoService.CancelReserve(corpNum, receiptNum, userID);
                return View("GetMessages", Response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 전송요청번호(requestNum)를 할당한 알림톡/친구톡 전송내역 및 전송상태를 확인합니다,
         */
        public IActionResult GetMessagesRN()
        {
            // 알림톡/친구톡 전송요청시 할당한 요청번호
            string requestNum = "20181127155110";

            try
            {
                var Response = _kakaoService.GetMessagesRN(corpNum, requestNum, userID);
                return View("GetMessages", Response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 전송요청번호(requestNum)를 할당한 알림톡/친구톡 예약전송건을 취소합니다.
         * - 예약전송 취소는 예약시간 10분전까지만 가능합니다.
         */
        public IActionResult CancelReserveRN()
        {
            // 알림톡/친구톡 전송요청시 할당한 요청번호
            string requestNum = "20181127155110";

            try
            {
                var Response = _kakaoService.CancelReserveRN(corpNum, requestNum, userID);
                return View("GetMessages", Response);
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
                var result = _kakaoService.GetBalance(corpNum);
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
                var result = _kakaoService.GetChargeURL(corpNum, userID);
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
                var result = _kakaoService.GetPartnerBalance(corpNum);
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
                var result = _kakaoService.GetPartnerURL(corpNum, TOGO);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 카카오서비스 발행단가를 확인합니다.
         */
        public IActionResult GetUnitCost()
        {
            try
            {
                // 카카오톡 전송유형 ATS(알림톡), FTS(친구톡텍스트), FMS(친구톡이미지)
                KakaoType msgType = KakaoType.ATS;

                var result = _kakaoService.GetUnitCost(corpNum, msgType);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 카카오서비스 API 서비스 과금정보를 확인합니다.
         */
        public IActionResult GetChargeInfo()
        {
            try
            {
                // 카카오톡 전송유형 ATS(알림톡), FTS(친구톡텍스트), FMS(친구톡이미지)
                KakaoType msgType = KakaoType.ATS;

                var response = _kakaoService.GetChargeInfo(corpNum, msgType);
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

                var response = _kakaoService.CheckIsMember(corpNum, linkID);
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
        public IActionResult CheckID()
        {
            //중복여부 확인할 팝빌 회원 아이디
            string checkID = "testkorea";

            try
            {
                var response = _kakaoService.CheckID(checkID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 파트너의 연동회원으로 회원가입을 요청합니다.
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
                var response = _kakaoService.JoinMember(joinInfo);
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
                var result = _kakaoService.GetAccessURL(corpNum, userID);
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
                var response = _kakaoService.GetCorpInfo(corpNum, userID);
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
                var response = _kakaoService.UpdateCorpInfo(corpNum, corpInfo, userID);
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
                var response = _kakaoService.RegistContact(corpNum, contactInfo, userID);
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
                var response = _kakaoService.ListContact(corpNum, userID);
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
                var response = _kakaoService.UpdateContact(corpNum, contactInfo, userID);
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