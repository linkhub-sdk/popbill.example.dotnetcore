using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Popbill;
using Popbill.Kakao;

namespace KakaoExample.Controllers
{
    public class KakaoController : Controller
    {
        private readonly KakaoService _kakaoService;

        public KakaoController(KakaoInstance KKOinstance)
        {
            // 카카오톡 서비스 객체 생성
            _kakaoService = KKOinstance.kakaoService;

        }

        // 팝빌 연동회원 사업자번호 (하이픈 '-' 없이 10자리)
        string corpNum = "1234567890";

        // 팝빌 연동회원 아이디
        string userID = "testkorea";

        /*
         * 카카오톡 Index page (Kakao/Index.cshtml)
         */
        public IActionResult Index()
        {
            return View();
        }

        #region 플러스친구/발신번호/알림템플릿 관리

        /*
         * 플러스친구 계정관리 팝업 URL을 반환합니다.
         * - 반환된 URL은 보안정책에 따라 30초의 유효시간을 갖습니다.
         * - https://docs.popbill.com/kakao/dotnetcore/api#GetPlusFriendMgtURL
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
         * 팝빌에 등록된 플러스친구 계정목록을 반환합니다.
         * - https://docs.popbill.com/kakao/dotnetcore/api#ListPlusFriendID
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
         * 발신번호 관리 팝업 URL을 반환합니다.
         * - 반환된 URL은 보안정책에 따라 30초의 유효시간을 갖습니다.
         * - https://docs.popbill.com/kakao/dotnetcore/api#GetSenderNumberMgtURL
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
         * 팝빌에 등록된 발신번호 목록을 반환합니다.
         * - https://docs.popbill.com/kakao/dotnetcore/api#GetSenderNumberList
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
         * - https://docs.popbill.com/kakao/dotnetcore/api#GetATSTemplateMgtURL
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
         * (주)카카오로 부터 승인된 알림톡 템플릿 정보를 확인합니다.
         * - https://docs.popbill.com/kakao/dotnetcore/api#GetATSTemplate
         */
        public IActionResult GetATSTemplate()
        {
            // 템플릿 코드
            string templateCdoe = "021010000076";

            try
            {
                var templateInfo = _kakaoService.GetATSTemplate(corpNum, templateCdoe, userID);
                return View("GetATSTemplate", templateInfo);
             }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * (주)카카오로 부터 승인된 알림톡 템플릿 목록을 확인합니다.
         * - 반환항목중 템플릿코드(templateCode)는 알림톡 전송시 사용됩니다.
         * - https://docs.popbill.com/kakao/dotnetcore/api#ListATSTemplate
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

        #region 알림톡/친구톡 전송

        /*
         * 알림톡 전송을 요청합니다.
         * - 사전에 승인된 템플릿의 내용과 알림톡 전송내용(content)이 다를 경우 전송실패 처리됩니다.
         * - https://docs.popbill.com/kakao/dotnetcore/api#SendATS
         */
        public IActionResult SendATS_One()
        {
            // 알림톡 템플릿 코드, ListATSTemplate API의 templateCode 확인
            string templateCode = "019020000163";

            // 발신번호
            string senderNum = "07043042991";

            // 수신번호
            string receiverNum = "010111222";

            // 수신자명
            string receiverName = "수신자명";

            // 알림톡 템플릿 내용, 최대 1000자
            String content = "[ 팝빌 ]\n";
            content += "신청하신 #{템플릿코드}에 대한 심사가 완료되어 승인 처리되었습니다.\n";
            content += "해당 템플릿으로 전송 가능합니다.\n\n";
            content += "문의사항 있으시면 파트너센터로 편하게 연락주시기 바랍니다.\n\n";
            content += "팝빌 파트너센터 : 1600-8536\n";
            content += "support@linkhub.co.kr".Replace("\n", Environment.NewLine);

            // 대체문자 메시지 내용 (최대 2000byte)
            string altContent = "대체문자 메시지 내용";

            // 대체문자 유형, 공백-미전송, C-알림톡 내용, A-대체문자 내용
            string altSendType = "A";

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
            string requestNum = "";

            // 알림톡 버튼정보를 템플릿 신청시 기재한 버튼정보와 동일하게 전송하는 경우 null 처리.
            List<KakaoButton> buttons = null;


            // 알림톡 버튼 URL에 #{템플릿변수}를 기재한경우 템플릿변수 영역을 변경하여 버튼정보 구성
            /*
            List<KakaoButton> buttons = new List<KakaoButton>();
            
            KakaoButton btnInfo = new KakaoButton();
            // 버튼명
            btnInfo.n = "템플릿 안내";
            // 버튼유형 DS(-배송조회 / WL - 웹링크 / AL - 앱링크 / MD - 메시지전달 / BK - 봇키워드)
            btnInfo.t = "WL";
            // 버튼링크1 [앱링크] iOS / [웹링크] Mobile
            btnInfo.u1 = "https://www.popbill.com";
            // 버튼링크2 [앱링크] Android / [웹링크] PC URL
            btnInfo.u2 = "http://test.popbill.com";
            buttons.Add(btnInfo);
            */

            try
            {
                var receiptNum = _kakaoService.SendATS(corpNum, templateCode, senderNum, receiverNum, receiverName,
                    content, altContent, altSendType, sndDT, requestNum, userID, buttons);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * [대량전송] 알림톡 전송을 요청합니다.
         * - 사전에 승인된 템플릿의 내용과 알림톡 전송내용(content)이 다를 경우 전송실패 처리됩니다.
         * - https://docs.popbill.com/kakao/dotnetcore/api#SendATS_Multi
         */
        public IActionResult SendATS_Multi()
        {
            // 알림톡 템플릿 코드, ListATSTemplate API의 templateCode 확인
            string templateCode = "019020000163";

            // 알림톡 템플릿 내용, 최대 1000자
            String content = "[ 팝빌 ]\n";
            content += "신청하신 #{템플릿코드}에 대한 심사가 완료되어 승인 처리되었습니다.\n";
            content += "해당 템플릿으로 전송 가능합니다.\n\n";
            content += "문의사항 있으시면 파트너센터로 편하게 연락주시기 바랍니다.\n\n";
            content += "팝빌 파트너센터 : 1600-8536\n";
            content += "support@linkhub.co.kr".Replace("\n", Environment.NewLine);

            // 발신번호
            string senderNum = "01043245117";

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
                receiverInfo.msg = content;

                // 대체문자 내용 (최대 2000byte)
                receiverInfo.altmsg = "대체문자 내용입니다" + i;

                // 파트너 지정키, 수신자 구분용 메모
                receiverInfo.interOPRefKey = "20200805-"+i;

                receivers.Add(receiverInfo);
            }

            // 대체문자 유형, 공백-미전송, C-알림톡 내용, A-대체문자 내용
            string altSendType = "A";

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
            string requestNum = "";

            // 알림톡 버튼정보를 템플릿 신청시 기재한 버튼정보와 동일하게 전송하는 경우 null 처리.
            List<KakaoButton> buttons = null;


            // 알림톡 버튼 URL에 #{템플릿변수}를 기재한경우 템플릿변수 영역을 변경하여 버튼정보 구성
            /*
            List<KakaoButton> buttons = new List<KakaoButton>();
            
            KakaoButton btnInfo = new KakaoButton();
            // 버튼명
            btnInfo.n = "템플릿 안내";
            // 버튼유형 DS(-배송조회 / WL - 웹링크 / AL - 앱링크 / MD - 메시지전달 / BK - 봇키워드)
            btnInfo.t = "WL";
            // 버튼링크1 [앱링크] iOS / [웹링크] Mobile
            btnInfo.u1 = "https://www.popbill.com";
            // 버튼링크2 [앱링크] Android / [웹링크] PC URL
            btnInfo.u2 = "http://test.popbill.com";
            buttons.Add(btnInfo);
            */

            try
            {
                var receiptNum = _kakaoService.SendATS(corpNum, templateCode, senderNum, receivers, altSendType, sndDT,
                    requestNum, userID, buttons);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * [동보전송] 알림톡 전송을 요청합니다.
         * - 사전에 승인된 템플릿의 내용과 알림톡 전송내용(content)이 다를 경우 전송실패 처리됩니다.
         * - https://docs.popbill.com/kakao/dotnetcore/api#SendATS_Same
         */
        public IActionResult SendATS_Same()
        {
            // 알림톡 템플릿 코드, ListATSTemplate API의 templateCode 확인
            string templateCode = "019020000163";

            // 알림톡 템플릿 내용, 최대 1000자
            String content = "[ 팝빌 ]\n";
            content += "신청하신 #{템플릿코드}에 대한 심사가 완료되어 승인 처리되었습니다.\n";
            content += "해당 템플릿으로 전송 가능합니다.\n\n";
            content += "문의사항 있으시면 파트너센터로 편하게 연락주시기 바랍니다.\n\n";
            content += "팝빌 파트너센터 : 1600-8536\n";
            content += "support@linkhub.co.kr".Replace("\n", Environment.NewLine);

            // (동보) 대체문자 메시지 내용 (최대 2000byte)
            string altContent = "대체문자 메시지 내용";

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

                receivers.Add(receiverInfo);
            }

            // 대체문자 유형, 공백-미전송, C-알림톡 내용, A-대체문자 내용
            string altSendType = "A";

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
            string requestNum = "";

            // 알림톡 버튼정보를 템플릿 신청시 기재한 버튼정보와 동일하게 전송하는 경우 null 처리.
            List<KakaoButton> buttons = null;


            // 알림톡 버튼 URL에 #{템플릿변수}를 기재한경우 템플릿변수 영역을 변경하여 버튼정보 구성
            /*
            List<KakaoButton> buttons = new List<KakaoButton>();
            
            KakaoButton btnInfo = new KakaoButton();
            // 버튼명
            btnInfo.n = "템플릿 안내";
            // 버튼유형 DS(-배송조회 / WL - 웹링크 / AL - 앱링크 / MD - 메시지전달 / BK - 봇키워드)
            btnInfo.t = "WL";
            // 버튼링크1 [앱링크] iOS / [웹링크] Mobile
            btnInfo.u1 = "https://www.popbill.com";
            // 버튼링크2 [앱링크] Android / [웹링크] PC URL
            btnInfo.u2 = "http://test.popbill.com";
            buttons.Add(btnInfo);
            */

            try
            {
                var receiptNum = _kakaoService.SendATS(corpNum, templateCode, senderNum, content, altContent, receivers,
                    altSendType, sndDT, requestNum, userID, buttons);
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
         * - https://docs.popbill.com/kakao/dotnetcore/api#SendFTS
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
                u1 = "http://www.popbill.com", // [앱링크] iOS, [웹링크] Mobile
                u2 = "http://test.popbill.com" // [앱링크] Android, [웹링크] PC URL
            };
            buttons.Add(btnInfo);

            // 대체문자 유형, 공백-미전송, C-친구톡 내용, A-대체문자 내용
            string altSendType = "A";

            // 광고 전송여부
            bool adsYN = false;

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

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
         * - https://docs.popbill.com/kakao/dotnetcore/api#SendFTS_Multi
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
                receiverInfo.msg = "친구톡 내용입니다." + i;

                // 대체문자 내용 (최대 2000byte)
                receiverInfo.altmsg = "대체문자 내용입니다" + i;

                receivers.Add(receiverInfo);
            }

            // 버튼배열, 최대 5개
            List<KakaoButton> buttons = new List<KakaoButton>();
            KakaoButton btnInfo = new KakaoButton
            {
                n = "버튼이름", // 버튼명
                t = "WL", // 버튼 유형, WL-웹링크, AL-앱링크, MD-메시지 전달, BK-봇키워드
                u1 = "http://www.popbill.com", // [앱링크] iOS, [웹링크] Mobile
                u2 = "http://test.popbill.com" // [앱링크] Android, [웹링크] PC URL
            };
            buttons.Add(btnInfo);

            // 대체문자 유형, 공백-미전송, C-친구톡 내용, A-대체문자 내용
            string altSendType = "A";

            // 광고 전송여부
            bool adsYN = false;

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

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
         * - https://docs.popbill.com/kakao/dotnetcore/api#SendFTS_Same
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
                u1 = "http://www.popbill.com", // [앱링크] iOS, [웹링크] Mobile
                u2 = "http://test.popbill.com" // [앱링크] Android, [웹링크] PC URL
            };
            buttons.Add(btnInfo);

            // 대체문자 유형, 공백-미전송, C-친구톡 내용, A-대체문자 내용
            string altSendType = "A";

            // 광고 전송여부
            bool adsYN = false;

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
            string requestNum = "";

            try
            {
                var receiptNum = _kakaoService.SendFTS(corpNum, plusFriendID, senderNum, content, altContent, receivers,
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
         * - https://docs.popbill.com/kakao/dotnetcore/api#SendFMS
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

            // 친구톡 내용 (최대 400자)
            string content = "친구톡(이미지) 내용은 최대 400자 입니다.";

            // 대체문자 메시지 내용 (최대 2000byte)
            string altContent = "대체문자 내용";

            // 버튼배열, 최대 5개
            List<KakaoButton> buttons = new List<KakaoButton>();
            KakaoButton btnInfo = new KakaoButton
            {
                n = "버튼이름", // 버튼명
                t = "WL", // 버튼 유형, WL-웹링크, AL-앱링크, MD-메시지 전달, BK-봇키워드
                u1 = "http://www.popbill.com", // [앱링크] iOS, [웹링크] Mobile
                u2 = "http://test.popbill.com" // [앱링크] Android, [웹링크] PC URL
            };
            buttons.Add(btnInfo);

            // 대체문자 유형, 공백-미전송, C-친구톡 내용, A-대체문자 내용
            string altSendType = "A";

            // 광고 전송여부
            bool adsYN = false;

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

            // 이미지 링크 URL, 링크는 http(s)://... 형식으로 입력되어야 합니다.
            string imageURL = "https://www.popbill.com";

            // 이미지 파일경로
            string filePath = "C:\\popbill.example.dotnetcore\\KakaoExample\\wwwroot\\images\\image.jpg";

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
            string requestNum = "";

            try
            {
                var receiptNum = _kakaoService.SendFMS(corpNum, plusFriendID, senderNum, receiverNum, receiverName,
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
         * - https://docs.popbill.com/kakao/dotnetcore/api#SendFMS_Multi
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

                // 친구톡 내용 (최대 400자)
                receiverInfo.msg = "친구톡(이미지) 내용은 최대 400자 입니다." + i;

                // 대체문자 내용 (최대 2000byte)
                receiverInfo.altmsg = "대체문자 내용 입니다" + i;

                receivers.Add(receiverInfo);
            }

            // 버튼배열, 최대 5개
            List<KakaoButton> buttons = new List<KakaoButton>();
            KakaoButton btnInfo = new KakaoButton
            {
                n = "버튼이름", // 버튼명
                t = "WL", // 버튼 유형, WL-웹링크, AL-앱링크, MD-메시지 전달, BK-봇키워드
                u1 = "http://www.popbill.com", // [앱링크] iOS, [웹링크] Mobile
                u2 = "http://test.popbill.com" // [앱링크] Android, [웹링크] PC URL
            };
            buttons.Add(btnInfo);

            // 대체문자 유형, 공백-미전송, C-친구톡 내용, A-대체문자 내용
            string altSendType = "A";

            // 광고 전송여부
            bool adsYN = false;

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

            // 이미지 링크 URL, 링크는 http(s)://... 형식으로 입력되어야 합니다.
            string imageURL = "https://www.popbill.com";

            // 이미지 파일경로
            string filePath = "C:\\popbill.example.dotnetcore\\KakaoExample\\wwwroot\\images\\image.jpg";

            // 전송요청번호, 파트너가 전송요청에 대한 관리번호를 직접 할당하여 관리하는 경우 기재
            // 최대 36자리, 영문, 숫자, 언더바('_'), 하이픈('-')을 조합하여 사업자별로 중복되지 않도록 구성
            string requestNum = "";

            try
            {
                var receiptNum = _kakaoService.SendFMS(corpNum, plusFriendID, senderNum, receivers, buttons,
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
         * - https://docs.popbill.com/kakao/dotnetcore/api#SendFMS_Same
         */
        public IActionResult SendFMS_Same()
        {
            // 플러스친구 아이디, ListPlusFriendID API 의 plusFriendID 참고
            string plusFriendID = "@팝빌";

            // 발신번호
            string senderNum = "07043042991";

            // (동보) 친구톡 내용 (최대 400자)
            string content = "친구톡(이미지) 내용은 최대 400자 입니다.";

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
                u1 = "http://www.popbill.com", // [앱링크] iOS, [웹링크] Mobile
                u2 = "http://test.popbill.com" // [앱링크] Android, [웹링크] PC URL
            };
            buttons.Add(btnInfo);

            // 대체문자 유형, 공백-미전송, C-친구톡 내용, A-대체문자 내용
            string altSendType = "A";

            // 광고 전송여부
            bool adsYN = false;

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;
            
            // 이미지 링크 URL, 링크는 http(s)://... 형식으로 입력되어야 합니다.
            string imageURL = "https://www.popbill.com";

            // 이미지 파일경로
            string filePath = "C:\\popbill.example.dotnetcore\\KakaoExample\\wwwroot\\images\\image.jpg";

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

        /*
         * 알림톡/친구톡 전송요청시 발급받은 접수번호(receiptNum)로 예약전송건을 취소합니다.
         * - 예약취소는 예약전송시간 10분전까지만 가능합니다.
         * - https://docs.popbill.com/kakao/dotnetcore/api#CancelReserve
         */
        public IActionResult CancelReserve()
        {
            // 알림톡/친구톡 전송요청시 발급받은 접수번호
            string receiptNum = "018120515512000001";

            try
            {
                var Response = _kakaoService.CancelReserve(corpNum, receiptNum, userID);
                return View("Response", Response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 전송요청번호(requestNum)를 할당한 알림톡/친구톡 예약전송건을 취소합니다.
         * - 예약전송 취소는 예약시간 10분전까지만 가능합니다.
         * - https://docs.popbill.com/kakao/dotnetcore/api#CancelReserveRN
         */
        public IActionResult CancelReserveRN()
        {
            // 알림톡/친구톡 전송요청시 할당한 요청번호
            string requestNum = "20190115-001";

            try
            {
                var Response = _kakaoService.CancelReserveRN(corpNum, requestNum, userID);
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
         * 알림톡/친구톡 전송요청시 발급받은 접수번호(receiptNum)로 전송결과를 확인합니다.
         * - https://docs.popbill.com/kakao/dotnetcore/api#GetMessages
         */
        public IActionResult GetMessages()
        {
            // 알림톡/친구톡 전송요청시 발급받은 접수번호
            string receiptNum = "020080512164500002";

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
         * 전송요청번호(requestNum)를 할당한 알림톡/친구톡 전송내역 및 전송상태를 확인합니다.
         * - https://docs.popbill.com/kakao/dotnetcore/api#GetMessagesRN
         */
        public IActionResult GetMessagesRN()
        {
            // 알림톡/친구톡 전송요청시 할당한 요청번호
            string requestNum = "20190115-001";

            try
            {
                var Response = _kakaoService.GetMessagesRN(corpNum, requestNum, userID);
                return View("GetMessagesRN", Response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        /*
         * 검색조건을 사용하여 알림톡/친구톡 전송 내역을 조회합니다.
         * - 최대 검색기간 : 6개월 이내
         * - https://docs.popbill.com/kakao/dotnetcore/api#Search
         */
        public IActionResult Search()
        {
            // 최대 검색기한 : 6개월 이내
            // 시작일자, 날자형식(yyyyMMdd)
            string SDate = "20190101";

            // 종료일자, 날자형식(yyyyMMdd)
            string EDate = "20190115";

            // 전송상태 배열, 0-대기, 1-전송중, -2-성공, 3-대체, 4-실패, 5-취소
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

            // 개인조회여부, true-개인조회, false-전체조회
            bool SenderYN = false;

            // 페이지 번호, 기본값 '1'
            int Page = 1;

            // 페이지당 검색개수, 기본값 '500', 최대 '1000' 
            int PerPage = 30;

            // 정렬방향, D-내림차순, A-오름차순
            string Order = "D";

            // 조회 검색어, 카카오톡 전송시 기재한 수신자명 입력, 공백시 전체조회
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
         * 알림톡/친구톡 전송내역 팝업 URL을 반환합니다.
         * - 반환된 URL은 보안정책에 따라 30초의 유효시간을 갖습니다.
         * - https://docs.popbill.com/kakao/dotnetcore/api#GetSentListURL
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




        #endregion

        #region 포인트관리 

        /*
         * 연동회원 잔여포인트를 확인합니다.
         * - https://docs.popbill.com/kakao/dotnetcore/api#GetBalance
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
         * - https://docs.popbill.com/kakao/dotnetcore/api#GetChargeURL
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
         * - https://docs.popbill.com/kakao/dotnetcore/api#GetPartnerBalance
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
         * - https://docs.popbill.com/kakao/dotnetcore/api#GetPartnerURL
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
         * 연동회원 포인트 결재내역 URL을 반환합니다
         * - 반환된 URL은 보안정책에 따라 30초의 유효시간을 갖습니다.
         * - https://docs.popbill.com/kakao/dotnetcore/api#GetPaymentURL
         */
        public IActionResult GetPaymentURL()
        {

            try
            {
                var result = _kakaoService.GetPaymentURL(corpNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         *연동회원 포인트 사용내역 URL을 반환합니다
         * - 반환된 URL은 보안정책에 따라 30초의 유효시간을 갖습니다.
         * - https://docs.popbill.com/kakao/dotnetcore/api#GetUseHistoryURL
         */
        public IActionResult GetUseHistoryURL()
        {

            try
            {
                var result = _kakaoService.GetUseHistoryURL(corpNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 알림톡/친구톡 서비스 전송단가를 확인합니다.
         * - https://docs.popbill.com/kakao/dotnetcore/api#GetUnitCost
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
         * 알림톡/친구톡 서비스 API 서비스 과금정보를 확인합니다.
         * - https://docs.popbill.com/kakao/dotnetcore/api#GetChargeInfo
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
         * - https://docs.popbill.com/kakao/dotnetcore/api#CheckIsMember
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
         * 팝빌 회원아이디 중복여부를 확인합니다.
         * - https://docs.popbill.com/kakao/dotnetcore/api#CheckID
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
         * - https://docs.popbill.com/kakao/dotnetcore/api#JoinMember
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
            joinInfo.ContactEmail = "test@test.com";

            // 담당자 연락처 (최대 20자)
            joinInfo.ContactTEL = "070-4304-2992";

            // 담당자 휴대폰번호 (최대 20자)
            joinInfo.ContactHP = "010-111-222";

            // 담당자 팩스번호 (최대 20자)
            joinInfo.ContactFAX = "02-111-222";

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
         * - https://docs.popbill.com/kakao/dotnetcore/api#GetAccessURL
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
         * - https://docs.popbill.com/kakao/dotnetcore/api#GetCorpInfo
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
         * - https://docs.popbill.com/kakao/dotnetcore/api#UpdateCorpInfo
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
         * - https://docs.popbill.com/kakao/dotnetcore/api#RegistContact
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
            contactInfo.tel = "070-4304-2992";

            // 담당자 휴대폰번호 (최대 20자)
            contactInfo.hp = "010-111-222";

            // 담당자 팩스번호 (최대 20자)
            contactInfo.fax = "02-111-222";

            // 담당자 이메일 (최대 100자)
            contactInfo.email = "netcore@linkhub.co.kr";

            // 담당자 조회권한 설정, 1(개인권한), 2 (읽기권한), 3 (회사권한)
            contactInfo.searchRole = 1;

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
        * 연동회원의 담당자 정보를 확인합니다.
        * - https://docs.popbill.com/kakao/dotnetcore/api#GetContactInfo
        */
        public IActionResult GetContactInfo()
        {
            string contactID = "test0730";

            try
            {
                var contactInfo = _kakaoService.GetContactInfo(corpNum, contactID, userID);
                return View("GetContactInfo", contactInfo);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 담당자 목록을 확인합니다.
         * - https://docs.popbill.com/kakao/dotnetcore/api#ListContact
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
         * - https://docs.popbill.com/kakao/dotnetcore/api#UpdateContact
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

            // 담당자 이메일 (최대 100자)
            contactInfo.email = "netcore@linkhub.co.kr";

            // 담당자 조회권한 설정, 1(개인권한), 2 (읽기권한), 3 (회사권한)
            contactInfo.searchRole = 1;

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