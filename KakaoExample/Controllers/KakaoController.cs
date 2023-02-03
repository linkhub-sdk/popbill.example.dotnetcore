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
         * 카카오톡 채널을 등록하고 내역을 확인하는 카카오톡 채널 관리 페이지 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/channel#GetPlusFriendMgtURL
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
         * 팝빌에 등록한 연동회원의 카카오톡 채널 목록을 확인합니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/channel#ListPlusFriendID
         */
        public IActionResult ListPlusFriendID()
        {
            try
            {
                var response = _kakaoService.ListPlusFriendID(corpNum);
                return View("ListPlusFriendID", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 카카오톡 발신번호 등록여부를 확인합니다.
         * - 발신번호 상태가 '승인'인 경우에만 리턴값 'Response'의 변수 'code'가 1로 반환됩니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/sendnum#CheckSenderNumber
         */
        public IActionResult CheckSenderNumber()
        {
            // 확인할 발신번호
            string senderNumber = "";

            try
            {
                var Response = _kakaoService.CheckSenderNumber(corpNum, senderNumber);
                return View("Response", Response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 발신번호를 등록하고 내역을 확인하는 카카오톡 발신번호 관리 페이지 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/sendnum#GetSenderNumberMgtURL
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
         * 팝빌에 등록한 연동회원의 카카오톡 발신번호 목록을 확인합니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/sendnum#GetSenderNumberList
         */
        public IActionResult GetSenderNumberList()
        {
            try
            {
                var response = _kakaoService.GetSenderNumberList(corpNum);
                return View("GetSenderNumberList", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 알림톡 템플릿을 신청하고 승인심사 결과를 확인하며 등록 내역을 확인하는 알림톡 템플릿 관리 페이지 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/template#GetATSTemplateMgtURL
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
         * 승인된 알림톡 템플릿 정보를 확인합니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/template#GetATSTemplate
         */
        public IActionResult GetATSTemplate()
        {
            // 템플릿 코드
            string templateCdoe = "021010000076";

            try
            {
                var templateInfo = _kakaoService.GetATSTemplate(corpNum, templateCdoe);
                return View("GetATSTemplate", templateInfo);
             }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 승인된 알림톡 템플릿 목록을 확인합니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/template#ListATSTemplate
         */
        public IActionResult ListATSTemplate()
        {
            try
            {
                var response = _kakaoService.ListATSTemplate(corpNum);
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
         * 승인된 템플릿의 내용을 작성하여 1건의 알림톡 전송을 팝빌에 접수합니다.
         * - 사전에 승인된 템플릿의 내용과 알림톡 전송내용(content)이 다를 경우 전송실패 처리됩니다.
         * - 전송실패 시 사전에 지정한 변수 'altSendType' 값으로 대체문자를 전송할 수 있고 이 경우 문자(SMS/LMS) 요금이 과금됩니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/send#SendATSOne
         */
        public IActionResult SendATS_One()
        {
            // 승인된 알림톡 템플릿코드
            // └ 알림톡 템플릿 관리 팝업 URL(GetATSTemplateMgtURL API) 함수, 알림톡 템플릿 목록 확인(ListATStemplate API) 함수를 호출하거나
            //   팝빌사이트에서 승인된 알림톡 템플릿 코드를  확인 가능.
            string templateCode = "019020000163";

            // 발신번호
            string senderNum = "";

            // 수신번호
            string receiverNum = "";

            // 수신자명
            string receiverName = "수신자명";

            // 알림톡 템플릿 내용, 최대 1000자
            string content = "[ 팝빌 ]\n";
            content += "신청하신 #{템플릿코드}에 대한 심사가 완료되어 승인 처리되었습니다.\n";
            content += "해당 템플릿으로 전송 가능합니다.\n\n";
            content += "문의사항 있으시면 파트너센터로 편하게 연락주시기 바랍니다.\n\n";
            content += "팝빌 파트너센터 : 1600-8536\n";
            content += "support@linkhub.co.kr".Replace("\n", Environment.NewLine);

            // 대체문자 제목
            // - 메시지 길이(90byte)에 따라 장문(LMS)인 경우에만 적용.
            string altSubject = "대체문자 제목";

            // 대체문자 유형(altSendType)이 "A"일 경우, 대체문자로 전송할 내용 (최대 2000byte)
            // └ 팝빌이 메시지 길이에 따라 단문(90byte 이하) 또는 장문(90byte 초과)으로 전송처리
            string altContent = "대체문자 메시지 내용";

            // 대체문자 유형 (null , "C" , "A" 중 택 1)
            // null = 미전송, C = 알림톡과 동일 내용 전송 , A = 대체문자 내용(altContent)에 입력한 내용 전송
            string altSendType = "A";

            // 예약전송일시, yyyyMMddHHmmss
            // - 분단위 전송, 미입력 시 즉시 전송
            DateTime? sndDT = null;

            // 전송요청번호
            // 팝빌이 접수 단위를 식별할 수 있도록 파트너가 부여하는 식별번호.
            // 1~36자리로 구성. 영문, 숫자, 하이픈(-), 언더바(_)를 조합하여 팝빌 회원별로 중복되지 않도록 할당.
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
                    content, altContent, altSendType, sndDT, requestNum, userID, buttons, altSubject);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 승인된 템플릿의 내용을 작성하여 다수건의 알림톡 전송을 팝빌에 접수하며, 수신자 별로 개별 내용을 전송합니다. (최대 1,000건)
         * - 사전에 승인된 템플릿의 내용과 알림톡 전송내용(content)이 다를 경우 전송실패 처리됩니다.
         * - 전송실패 시 사전에 지정한 변수 'altSendType' 값으로 대체문자를 전송할 수 있고, 이 경우 문자(SMS/LMS) 요금이 과금됩니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/send#SendATSMulti
         */
        public IActionResult SendATS_Multi()
        {
            // 승인된 알림톡 템플릿코드
            // └ 알림톡 템플릿 관리 팝업 URL(GetATSTemplateMgtURL API) 함수, 알림톡 템플릿 목록 확인(ListATStemplate API) 함수를 호출하거나
            //   팝빌사이트에서 승인된 알림톡 템플릿 코드를  확인 가능.
            string templateCode = "019020000163";

            // 알림톡 템플릿 내용, 최대 1000자
            string content = "[ 팝빌 ]\n";
            content += "신청하신 #{템플릿코드}에 대한 심사가 완료되어 승인 처리되었습니다.\n";
            content += "해당 템플릿으로 전송 가능합니다.\n\n";
            content += "문의사항 있으시면 파트너센터로 편하게 연락주시기 바랍니다.\n\n";
            content += "팝빌 파트너센터 : 1600-8536\n";
            content += "support@linkhub.co.kr".Replace("\n", Environment.NewLine);

            // 발신번호
            string senderNum = "";

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

                // 대체문자 제목
                receiverInfo.altsjt = "대체문자 제목입니다." + i;

                // 대체문자 내용 (최대 2000byte)
                receiverInfo.altmsg = "대체문자 내용입니다" + i;

                // 파트너 지정키, 수신자 구분용 메모
                receiverInfo.interOPRefKey = "20200805-"+i;

                receivers.Add(receiverInfo);
            }

            // 대체문자 유형 (null , "C" , "A" 중 택 1)
            // null = 미전송, C = 알림톡과 동일 내용 전송 , A = 대체문자 내용(altContent)에 입력한 내용 전송
            string altSendType = "A";

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

            // 전송요청번호
            // 팝빌이 접수 단위를 식별할 수 있도록 파트너가 부여하는 식별번호.
            // 1~36자리로 구성. 영문, 숫자, 하이픈(-), 언더바(_)를 조합하여 팝빌 회원별로 중복되지 않도록 할당.
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
         * 승인된 템플릿 내용을 작성하여 다수건의 알림톡 전송을 팝빌에 접수하며, 모든 수신자에게 동일 내용을 전송합니다. (최대 1,000건)
         * - 사전에 승인된 템플릿의 내용과 알림톡 전송내용(content)이 다를 경우 전송실패 처리됩니다.
         * - 전송실패시 사전에 지정한 변수 'altSendType' 값으로 대체문자를 전송할 수 있고, 이 경우 문자(SMS/LMS) 요금이 과금됩니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/send#SendATSSame
         */
        public IActionResult SendATS_Same()
        {
            // 승인된 알림톡 템플릿코드
            // └ 알림톡 템플릿 관리 팝업 URL(GetATSTemplateMgtURL API) 함수, 알림톡 템플릿 목록 확인(ListATStemplate API) 함수를 호출하거나
            //   팝빌사이트에서 승인된 알림톡 템플릿 코드를  확인 가능.
            string templateCode = "019020000163";

            // 알림톡 템플릿 내용, 최대 1000자
            string content = "[ 팝빌 ]\n";
            content += "신청하신 #{템플릿코드}에 대한 심사가 완료되어 승인 처리되었습니다.\n";
            content += "해당 템플릿으로 전송 가능합니다.\n\n";
            content += "문의사항 있으시면 파트너센터로 편하게 연락주시기 바랍니다.\n\n";
            content += "팝빌 파트너센터 : 1600-8536\n";
            content += "support@linkhub.co.kr".Replace("\n", Environment.NewLine);

            // 대체문자 유형(altSendType)이 "A"일 경우, 대체문자로 전송할 내용 (최대 2000byte)
            // └ 팝빌이 메시지 길이에 따라 단문(90byte 이하) 또는 장문(90byte 초과)으로 전송처리
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

            // 대체문자 제목
            // - 메시지 길이(90byte)에 따라 장문(LMS)인 경우에만 적용.
            // - 수신정보 배열에 대체문자 제목이 입력되지 않은 경우 적용.
            // - 모든 수신자에게 다른 제목을 보낼 경우 464번 라인에 있는 altsjt 를 이용.
            string altSubject = "";

            // 대체문자 유형 (null , "C" , "A" 중 택 1)
            // null = 미전송, C = 알림톡과 동일 내용 전송 , A = 대체문자 내용(altContent)에 입력한 내용 전송
            string altSendType = "A";

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

            // 전송요청번호
            // 팝빌이 접수 단위를 식별할 수 있도록 파트너가 부여하는 식별번호.
            // 1~36자리로 구성. 영문, 숫자, 하이픈(-), 언더바(_)를 조합하여 팝빌 회원별로 중복되지 않도록 할당.
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
                    altSendType, sndDT, requestNum, userID, buttons, altSubject);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 텍스트로 구성된 1건의 친구톡 전송을 팝빌에 접수합니다.
         * - 친구톡의 경우 야간 전송은 제한됩니다. (20:00 ~ 익일 08:00)
         * - 전송실패시 사전에 지정한 변수 'altSendType' 값으로 대체문자를 전송할 수 있고, 이 경우 문자(SMS/LMS) 요금이 과금됩니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/send#SendFTSOne
         */
        public IActionResult SendFTS_One()
        {
            // 팝빌에 등록된 카카오톡 검색용 아이디
            string plusFriendID = "@팝빌";

            // 발신번호
            string senderNum = "";

            // 수신번호
            string receiverNum = "";

            // 수신자명
            string receiverName = "수신자명";

            // 친구톡 내용 (최대 1000자)
            string content = "친구톡 내용";

            // 대체문자 유형(altSendType)이 "A"일 경우, 대체문자로 전송할 내용 (최대 2000byte)
            // └ 팝빌이 메시지 길이에 따라 단문(90byte 이하) 또는 장문(90byte 초과)으로 전송처리
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

            // 대체문자 유형 (null , "C" , "A" 중 택 1)
            // null = 미전송, C = 알림톡과 동일 내용 전송 , A = 대체문자 내용(altContent)에 입력한 내용 전송
            string altSendType = "A";

            // 광고성 메시지 여부 ( true , false 중 택 1)
            // └ true = 광고 , false = 일반
            // - 미입력 시 기본값 false 처리
            bool adsYN = false;

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

            // 전송요청번호
            // 팝빌이 접수 단위를 식별할 수 있도록 파트너가 부여하는 식별번호.
            // 1~36자리로 구성. 영문, 숫자, 하이픈(-), 언더바(_)를 조합하여 팝빌 회원별로 중복되지 않도록 할당.
            string requestNum = "";

            // 대체문자 제목
            // - 메시지 길이(90byte)에 따라 장문(LMS)인 경우에만 적용.
            string altSubject = "";

            try
            {
                var receiptNum = _kakaoService.SendFTS(corpNum, plusFriendID, senderNum, receiverNum, receiverName,
                    content, altContent, buttons, altSendType, adsYN, sndDT, requestNum, userID, altSubject);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 텍스트로 구성된 다수건의 친구톡 전송을 팝빌에 접수하며, 수신자 별로 개별 내용을 전송합니다. (최대 1,000건)
         * - 친구톡의 경우 야간 전송은 제한됩니다. (20:00 ~ 익일 08:00)
         * - 전송실패시 사전에 지정한 변수 'altSendType' 값으로 대체문자를 전송할 수 있고, 이 경우 문자(SMS/LMS) 요금이 과금됩니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/send#SendFTSMulti
         */
        public IActionResult sendFTS_Multi()
        {
            // 팝빌에 등록된 카카오톡 검색용 아이디
            string plusFriendID = "@팝빌";

            // 발신번호
            string senderNum = "";

            // 수신자정보 배열, 최대 1000건
            List<KakaoReceiver> receivers = new List<KakaoReceiver>();

            for (int i = 0; i < 5; i++)
            {
                KakaoReceiver receiverInfo = new KakaoReceiver();

                // 수신번호
                receiverInfo.rcv = "" + i;

                // 수신자명
                receiverInfo.rcvnm = "수신자명" + i;

                // 친구톡 내용 (최대 1000자)
                receiverInfo.msg = "친구톡 내용입니다." + i;

                // 대체문자 제목
                receiverInfo.altsjt = "대체문자 제목입니다." + i;

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

            // 대체문자 유형 (null , "C" , "A" 중 택 1)
            // null = 미전송, C = 친구톡과 동일 내용 전송 , A = 대체문자 내용(altContent)에 입력한 내용 전송
            string altSendType = "A";

            // 광고성 메시지 여부 ( true , false 중 택 1)
            // └ true = 광고 , false = 일반
            // - 미입력 시 기본값 false 처리
            bool adsYN = false;

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

            // 전송요청번호
            // 팝빌이 접수 단위를 식별할 수 있도록 파트너가 부여하는 식별번호.
            // 1~36자리로 구성. 영문, 숫자, 하이픈(-), 언더바(_)를 조합하여 팝빌 회원별로 중복되지 않도록 할당.
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
         * 텍스트로 구성된 다수건의 친구톡 전송을 팝빌에 접수하며, 모든 수신자에게 동일 내용을 전송합니다. (최대 1,000건)
         * - 친구톡의 경우 야간 전송은 제한됩니다. (20:00 ~ 익일 08:00)
         * - 전송실패시 사전에 지정한 변수 'altSendType' 값으로 대체문자를 전송할 수 있고, 이 경우 문자(SMS/LMS) 요금이 과금됩니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/send#SendFTSSame
         */
        public IActionResult sendFTS_Same()
        {
            // 팝빌에 등록된 카카오톡 검색용 아이디
            string plusFriendID = "@팝빌";

            // 발신번호
            string senderNum = "";

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
                receiverInfo.rcv = "" + i;

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

            // 대체문자 제목
            // - 메시지 길이(90byte)에 따라 장문(LMS)인 경우에만 적용.
            string altSubject = "";

            // 대체문자 유형 (null , "C" , "A" 중 택 1)
            // null = 미전송, C = 친구톡과 동일 내용 전송 , A = 대체문자 내용(altContent)에 입력한 내용 전송
            string altSendType = "A";

            // 광고성 메시지 여부 ( true , false 중 택 1)
            // └ true = 광고 , false = 일반
            // - 미입력 시 기본값 false 처리
            bool adsYN = false;

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

            // 전송요청번호
            // 팝빌이 접수 단위를 식별할 수 있도록 파트너가 부여하는 식별번호.
            // 1~36자리로 구성. 영문, 숫자, 하이픈(-), 언더바(_)를 조합하여 팝빌 회원별로 중복되지 않도록 할당.
            string requestNum = "";

            try
            {
                var receiptNum = _kakaoService.SendFTS(corpNum, plusFriendID, senderNum, content, altContent, receivers,
                    buttons, altSendType, adsYN, sndDT, requestNum, altSubject);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 이미지가 첨부된 1건의 친구톡 전송을 팝빌에 접수합니다.
         * - 친구톡의 경우 야간 전송은 제한됩니다. (20:00 ~ 익일 08:00)
         * - 전송실패시 사전에 지정한 변수 'altSendType' 값으로 대체문자를 전송할 수 있고, 이 경우 문자(SMS/LMS) 요금이 과금됩니다.
         * - 대체문자의 경우, 포토문자(MMS) 형식은 지원하고 있지 않습니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/send#SendFMSOne
         */
        public IActionResult SendFMS_One()
        {
            // 팝빌에 등록된 카카오톡 검색용 아이디
            string plusFriendID = "@팝빌";

            // 발신번호
            string senderNum = "";

            // 수신번호
            string receiverNum = "";

            // 수신자명
            string receiverName = "수신자명";

            // 친구톡 내용 (최대 400자)
            string content = "친구톡(이미지) 내용은 최대 400자 입니다.";

            // 대체문자 유형(altSendType)이 "A"일 경우, 대체문자로 전송할 내용 (최대 2000byte)
            // └ 팝빌이 메시지 길이에 따라 단문(90byte 이하) 또는 장문(90byte 초과)으로 전송처리
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

            // 대체문자 제목
            // - 메시지 길이(90byte)에 따라 장문(LMS)인 경우에만 적용.
            string altSubject = "";

            // 대체문자 유형 (null , "C" , "A" 중 택 1)
            // null = 미전송, C = 친구톡과 동일 내용 전송 , A = 대체문자 내용(altContent)에 입력한 내용 전송
            string altSendType = "A";

            // 광고성 메시지 여부 ( true , false 중 택 1)
            // └ true = 광고 , false = 일반
            // - 미입력 시 기본값 false 처리
            bool adsYN = false;

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

            // 이미지 링크 URL
            // └ 수신자가 친구톡 상단 이미지 클릭시 호출되는 URL
            // - 미입력시 첨부된 이미지를 링크 기능 없이 표시
            string imageURL = "https://www.popbill.com";

            // 첨부이미지 파일 경로
            // - 이미지 파일 규격: 전송 포맷 – JPG 파일 (.jpg, .jpeg), 용량 – 최대 500 Kbyte, 크기 – 가로 500px 이상, 가로 기준으로 세로 0.5~1.3배 비율 가능
            string filePath = "C:\\popbill.example.dotnetcore\\KakaoExample\\wwwroot\\images\\image.jpg";

            // 전송요청번호
            // 팝빌이 접수 단위를 식별할 수 있도록 파트너가 부여하는 식별번호.
            // 1~36자리로 구성. 영문, 숫자, 하이픈(-), 언더바(_)를 조합하여 팝빌 회원별로 중복되지 않도록 할당.
            string requestNum = "";

            try
            {
                var receiptNum = _kakaoService.SendFMS(corpNum, plusFriendID, senderNum, receiverNum, receiverName,
                    content, altContent, buttons, altSendType, adsYN, sndDT, imageURL, filePath, requestNum, altSubject);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 이미지가 첨부된 다수건의 친구톡 전송을 팝빌에 접수하며, 수신자 별로 개별 내용을 전송합니다. (최대 1,000건)
         * - 친구톡의 경우 야간 전송은 제한됩니다. (20:00 ~ 익일 08:00)
         * - 전송실패시 사전에 지정한 변수 'altSendType' 값으로 대체문자를 전송할 수 있고, 이 경우 문자(SMS/LMS) 요금이 과금됩니다.
         * - 대체문자의 경우, 포토문자(MMS) 형식은 지원하고 있지 않습니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/send#SendFMSMulti
         */
        public IActionResult SendFMS_Multi()
        {
            // 팝빌에 등록된 카카오톡 검색용 아이디
            string plusFriendID = "@팝빌";

            // 발신번호
            string senderNum = "";

            // 수신자정보 배열, 최대 1000건
            List<KakaoReceiver> receivers = new List<KakaoReceiver>();

            for (int i = 0; i < 5; i++)
            {
                KakaoReceiver receiverInfo = new KakaoReceiver();

                // 수신번호
                receiverInfo.rcv = "" + i;

                // 수신자명
                receiverInfo.rcvnm = "수신자명" + i;

                // 친구톡 내용 (최대 400자)
                receiverInfo.msg = "친구톡(이미지) 내용은 최대 400자 입니다." + i;

                // 대체문자 제목
                receiverInfo.altsjt = "대체문자 제목입니다." + i;

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

            // 대체문자 유형 (null , "C" , "A" 중 택 1)
            // null = 미전송, C = 친구톡과 동일 내용 전송 , A = 대체문자 내용(altContent)에 입력한 내용 전송
            string altSendType = "A";

            // 광고성 메시지 여부 ( true , false 중 택 1)
            // └ true = 광고 , false = 일반
            // - 미입력 시 기본값 false 처리
            bool adsYN = false;

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

            // 이미지 링크 URL
            // └ 수신자가 친구톡 상단 이미지 클릭시 호출되는 URL
            // - 미입력시 첨부된 이미지를 링크 기능 없이 표시
            string imageURL = "https://www.popbill.com";

            // 첨부이미지 파일 경로
            // - 이미지 파일 규격: 전송 포맷 – JPG 파일 (.jpg, .jpeg), 용량 – 최대 500 Kbyte, 크기 – 가로 500px 이상, 가로 기준으로 세로 0.5~1.3배 비율 가능
            string filePath = "C:\\popbill.example.dotnetcore\\KakaoExample\\wwwroot\\images\\image.jpg";

            // 전송요청번호
            // 팝빌이 접수 단위를 식별할 수 있도록 파트너가 부여하는 식별번호.
            // 1~36자리로 구성. 영문, 숫자, 하이픈(-), 언더바(_)를 조합하여 팝빌 회원별로 중복되지 않도록 할당.
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
         * 이미지가 첨부된 다수건의 친구톡 전송을 팝빌에 접수하며, 모든 수신자에게 동일 내용을 전송합니다. (최대 1,000건)
         * - 친구톡의 경우 야간 전송은 제한됩니다. (20:00 ~ 익일 08:00)
         * - 이미지 파일 규격: 전송 포맷 – JPG 파일 (.jpg, .jpeg), 용량 – 최대 500 Kbyte, 크기 – 가로 500px 이상, 가로 기준으로 세로 0.5~1.3배 비율 가능
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/send#SendFMSSame
         */
        public IActionResult SendFMS_Same()
        {
            // 팝빌에 등록된 카카오톡 검색용 아이디
            string plusFriendID = "@팝빌";

            // 발신번호
            string senderNum = "";

            // (동보) 친구톡 내용 (최대 400자)
            string content = "친구톡(이미지) 내용은 최대 400자 입니다.";

            // 대체문자 유형(altSendType)이 "A"일 경우, 대체문자로 전송할 내용 (최대 2000byte)
            // └ 팝빌이 메시지 길이에 따라 단문(90byte 이하) 또는 장문(90byte 초과)으로 전송처리
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

            // 대체문자 유형 (null , "C" , "A" 중 택 1)
            // null = 미전송, C = 친구톡과 동일 내용 전송 , A = 대체문자 내용(altContent)에 입력한 내용 전송
            string altSendType = "A";

            // 광고성 메시지 여부 ( true , false 중 택 1)
            // └ true = 광고 , false = 일반
            // - 미입력 시 기본값 false 처리
            bool adsYN = false;

            // 예약전송일시(yyyyMMddHHmmss), null인 경우 즉시전송
            // ex) DateTime sndDT = new DateTime(20181230120000);
            DateTime? sndDT = null;

            // 이미지 링크 URL
            // └ 수신자가 친구톡 상단 이미지 클릭시 호출되는 URL
            // - 미입력시 첨부된 이미지를 링크 기능 없이 표시
            string imageURL = "https://www.popbill.com";

            // 첨부이미지 파일 경로
            // - 이미지 파일 규격: 전송 포맷 – JPG 파일 (.jpg, .jpeg), 용량 – 최대 500 Kbyte, 크기 – 가로 500px 이상, 가로 기준으로 세로 0.5~1.3배 비율 가능
            string filePath = "C:\\popbill.example.dotnetcore\\KakaoExample\\wwwroot\\images\\image.jpg";

            // 전송요청번호
            // 팝빌이 접수 단위를 식별할 수 있도록 파트너가 부여하는 식별번호.
            // 1~36자리로 구성. 영문, 숫자, 하이픈(-), 언더바(_)를 조합하여 팝빌 회원별로 중복되지 않도록 할당.
            string requestNum = "";

            // 대체문자 제목
            // - 메시지 길이(90byte)에 따라 장문(LMS)인 경우에만 적용.
            string altSubject = "";

            try
            {
                var receiptNum = _kakaoService.SendFMS(corpNum, plusFriendID, senderNum, content, altContent, receivers,
                    buttons, altSendType, adsYN, sndDT, imageURL, filePath, requestNum, userID, altSubject);
                return View("ReceiptNum", receiptNum);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌에서 반환받은 접수번호를 통해 예약접수된 카카오톡을 전송 취소합니다. (예약시간 10분 전까지 가능)
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/send#CancelReserve
         */
        public IActionResult CancelReserve()
        {
            // 알림톡/친구톡 전송요청시 발급받은 접수번호
            string receiptNum = "018120515512000001";

            try
            {
                var Response = _kakaoService.CancelReserve(corpNum, receiptNum);
                return View("Response", Response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 파트너가 할당한 전송요청 번호를 통해 예약접수된 카카오톡을 전송 취소합니다. (예약시간 10분 전까지 가능)
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/send#CancelReserveRN
         */
        public IActionResult CancelReserveRN()
        {
            // 알림톡/친구톡 전송요청시 할당한 요청번호
            string requestNum = "";

            try
            {
                var Response = _kakaoService.CancelReserveRN(corpNum, requestNum);
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
         * 팝빌에서 반환받은 접수번호를 통해 알림톡/친구톡 전송상태 및 결과를 확인합니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/info#GetMessages
         */
        public IActionResult GetMessages()
        {
            // 알림톡/친구톡 전송요청시 발급받은 접수번호
            string receiptNum = "020080512164500002";

            try
            {
                var Response = _kakaoService.GetMessages(corpNum, receiptNum);
                return View("GetMessages", Response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 파트너가 할당한 전송요청 번호를 통해 알림톡/친구톡 전송상태 및 결과를 확인합니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/info#GetMessagesRN
         */
        public IActionResult GetMessagesRN()
        {
            // 알림톡/친구톡 전송요청시 할당한 요청번호
            string requestNum = "";

            try
            {
                var Response = _kakaoService.GetMessagesRN(corpNum, requestNum);
                return View("GetMessagesRN", Response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 검색조건에 해당하는 카카오톡 전송내역을 조회합니다. (조회기간 단위 : 최대 2개월)
         * - 카카오톡 접수일시로부터 6개월 이내 접수건만 조회할 수 있습니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/info#Search
         */
        public IActionResult Search()
        {
            // 최대 검색기한 : 6개월 이내
            // 시작일자, 날자형식(yyyyMMdd)
            string SDate = "20220501";

            // 종료일자, 날자형식(yyyyMMdd)
            string EDate = "20220531";

            // 전송상태 배열 ("0" , "1" , "2" , "3" , "4" , "5" 중 선택, 다중 선택 가능)
            // └ 0 = 전송대기 , 1 = 전송중 , 2 = 전송성공 , 3 = 대체문자 전송 , 4 = 전송실패 , 5 = 전송취소
            // - 미입력 시 전체조회
            string[] State = new string[6];
            State[0] = "0";
            State[1] = "1";
            State[2] = "2";
            State[3] = "3";
            State[4] = "4";
            State[5] = "5";

            // 검색대상 배열 ("ATS", "FTS", "FMS" 중 선택, 다중 선택 가능)
            // └ ATS = 알림톡 , FTS = 친구톡(텍스트) , FMS = 친구톡(이미지)
            // - 미입력 시 전체조회
            string[] Item = new string[3];
            Item[0] = "ATS";
            Item[1] = "FTS";
            Item[2] = "FMS";

            // 전송유형별 조회 (null , "0" , "1" 중 택 1)
            // └ null = 전체 , 0 = 즉시전송건 , 1 = 예약전송건
            // - 미입력 시 전체조회
            string ReserveYN = "";

            // 사용자권한별 조회 (true / false 중 택 1)
            // └ false = 접수한 카카오톡 전체 조회 (관리자권한)
            // └ true = 해당 담당자 계정으로 접수한 카카오톡만 조회 (개인권한)
            // - 미입력시 기본값 false 처리
            bool SenderYN = false;

            // 페이지 번호, 기본값 '1'
            int Page = 1;

            // 페이지당 검색개수, 기본값 '500', 최대 '1000'
            int PerPage = 30;

            // 정렬방향, D-내림차순, A-오름차순
            string Order = "D";

            // 조회하고자 하는 수신자명
            // - 미입력시 전체조회
            string QString = "";

            try
            {
                var response = _kakaoService.Search(corpNum, SDate, EDate, State, Item, ReserveYN, SenderYN, Page,
                    PerPage, Order, QString);
                return View("Search", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 팝빌 사이트와 동일한 카카오톡 전송내역을 확인하는 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/info#GetSentListURL
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
         * 연동회원의 잔여포인트를 확인합니다.
         * - 과금방식이 파트너과금인 경우 파트너 잔여포인트 확인(GetPartnerBalance API) 함수를 통해 확인하시기 바랍니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/point#GetBalance
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
         * 연동회원 포인트 충전을 위한 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/point#GetChargeURL
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
         * 연동회원 포인트 결제내역 확인을 위한 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/point#GetPaymentURL
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
         * 연동회원 포인트 사용내역 확인을 위한 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/point#GetUseHistoryURL
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
         * 파트너의 잔여포인트를 확인합니다.
         * - 과금방식이 연동과금인 경우 연동회원 잔여포인트 확인(GetBalance API) 함수를 이용하시기 바랍니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/point#GetPartnerBalance
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
         * 파트너 포인트 충전을 위한 페이지의 팝업 URL을 반환합니다.
         * - 반환되는 URL은 보안 정책상 30초 동안 유효하며, 시간을 초과한 후에는 해당 URL을 통한 페이지 접근이 불가합니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/point#GetPartnerURL
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
         * 카카오톡 전송시 과금되는 포인트 단가를 확인합니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/point#GetUnitCost
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
         * 팝빌 카카오톡 API 서비스 과금정보를 확인합니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/point#GetChargeInfo
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
         * 사업자번호를 조회하여 연동회원 가입여부를 확인합니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/member#CheckIsMember
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
         * 사용하고자 하는 아이디의 중복여부를 확인합니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/member#CheckID
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
         * 사용자를 연동회원으로 가입처리합니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/member#JoinMember
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
                var response = _kakaoService.JoinMember(joinInfo);
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
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/member#GetAccessURL
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
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/member#GetCorpInfo
         */
        public IActionResult GetCorpInfo()
        {
            try
            {
                var response = _kakaoService.GetCorpInfo(corpNum);
                return View("GetCorpInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 회사정보를 수정합니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/member#UpdateCorpInfo
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
                var response = _kakaoService.UpdateCorpInfo(corpNum, corpInfo);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 담당자(팝빌 로그인 계정)를 추가합니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/member#RegistContact
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
                var response = _kakaoService.RegistContact(corpNum, contactInfo);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 정보을 확인합니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/member#GetContactInfo
         */
        public IActionResult GetContactInfo()
        {
            // 확인할 담당자 아이디
            string contactID = "test0730";

            try
            {
                var contactInfo = _kakaoService.GetContactInfo(corpNum, contactID);
                return View("GetContactInfo", contactInfo);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 목록을 확인합니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/member#ListContact
         */
        public IActionResult ListContact()
        {
            try
            {
                var response = _kakaoService.ListContact(corpNum);
                return View("ListContact", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 정보를 수정합니다.
         * - https://developers.popbill.com/reference/kakaotalk/dotnetcore/api/member#UpdateContact
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
                var response = _kakaoService.UpdateContact(corpNum, contactInfo);
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
