/*
 * 팝빌 예금주조회 API .NET Core SDK Example
 * .NET Core 연동 튜토리얼 안내 : https://developers.popbill.com/guide/accountcheck/dotnetcore/getting-started/tutorial
 * 
 * 업데이트 일자 : 2025-01-23
 * 연동 기술지원 연락처 : 1600 - 9854
 * 연동 기술지원 이메일 : code@linkhubcorp.com
*/
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Popbill;
using Popbill.AccountCheck;

namespace AccountCheckExample.Controllers
{
    public class AccountCheckController : Controller
    {
        private readonly AccountCheckService _accountCheckService;

        public AccountCheckController(AccountCheckInstance ACInstance)
        {
            // 예금주조회 서비스 객체 생성
            _accountCheckService = ACInstance.accountCheckService;

        }

        // 팝빌 연동회원 사업자번호 (하이픈 '-' 없이 10자리)
        string corpNum = "1234567890";

        // 팝빌 연동회원 아이디
        string userID = "testkorea";

        /*
         * 예금주조회 Index page (Closedown/Index.cshtml)
         */
        public IActionResult Index()
        {
            return View();
        }

        #region 예금주조회

        /*
         * 1건의 예금주성명을 조회합니다.
         * - https://developers.popbill.com/reference/accountcheck/dotnetcore/api/check#CheckAccountInfo
         */
        public IActionResult CheckAccountInfo()
        {
            try
            {
                // 기관코드
                string bankCode = "";

                // 계좌번호
                string accountNumber = "";


                var response = _accountCheckService.CheckAccountInfo(corpNum, bankCode, accountNumber);
                return View("CheckAccountInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 1건의 예금주실명을 조회합니다.
         * - https://developers.popbill.com/reference/accountcheck/dotnetcore/api/check#CheckDepositorInfo
         */
        public IActionResult CheckDepositorInfo()
        {
            try
            {
                // 기관코드
                string bankCode = "";

                // 계좌번호
                string accountNumber = "";

                // 등록번호 유형, P-개인, B-사업자
                string identityNumType = "P";

                // 등록번호
                // └ 등록번호 유형 값이 "B"인 경우 사업자번호(10 자리) 입력
                // └ 등록번호 유형 값이 "P"인 경우 생년월일(6 자리) 입력 (형식 : YYMMDD)
                // 하이픈 '-' 제외하고 입력
                string identityNum = "";

                var response = _accountCheckService.CheckDepositorInfo(corpNum, bankCode, accountNumber, identityNumType, identityNum);
                return View("CheckDepositorInfo", response);
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
         * - https://developers.popbill.com/reference/accountcheck/dotnetcore/api/point#GetBalance
         */
        public IActionResult GetBalance()
        {
            try
            {
                var result = _accountCheckService.GetBalance(corpNum);
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
         * - https://developers.popbill.com/reference/accountcheck/dotnetcore/api/point#GetChargeURL
         */
        public IActionResult GetChargeURL()
        {
            try
            {
                var result = _accountCheckService.GetChargeURL(corpNum, userID);
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
         * - https://developers.popbill.com/reference/accountcheck/dotnetcore/api/point#GetPaymentURL
         */
        public IActionResult GetPaymentURL()
        {

            try
            {
                var result = _accountCheckService.GetPaymentURL(corpNum, userID);
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
         * - https://developers.popbill.com/reference/accountcheck/dotnetcore/api/point#GetUseHistoryURL
         */
        public IActionResult GetUseHistoryURL()
        {

            try
            {
                var result = _accountCheckService.GetUseHistoryURL(corpNum, userID);
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
         * - https://developers.popbill.com/reference/accountcheck/dotnetcore/api/point#GetPartnerBalance
         */
        public IActionResult GetPartnerBalance()
        {
            try
            {
                var result = _accountCheckService.GetPartnerBalance(corpNum);
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
         * - https://developers.popbill.com/reference/accountcheck/dotnetcore/api/point#GetPartnerURL
         */
        public IActionResult GetPartnerURL()
        {
            // CHRG - 포인트충전 URL
            string TOGO = "CHRG";

            try
            {
                var result = _accountCheckService.GetPartnerURL(corpNum, TOGO);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 예금주조회시 과금되는 포인트 단가를 확인합니다.
         * - https://developers.popbill.com/reference/accountcheck/dotnetcore/api/point#GetUnitCost
         */
        public IActionResult GetUnitCost()
        {
            // 서비스 유형, 성명 / 실명 중 택 1
            string serviceType = "실명";

            try
            {
                var result = _accountCheckService.GetUnitCost(corpNum, serviceType);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 예금주조회 API 서비스 과금정보를 확인합니다.
         * - https://developers.popbill.com/reference/accountcheck/dotnetcore/api/point#GetChargeInfo
         */
        public IActionResult GetChargeInfo()
        {
            // 서비스 유형, 성명 / 실명 중 택 1
            string serviceType = "실명";

            try
            {
                var response = _accountCheckService.GetChargeInfo(corpNum, userID, serviceType);
                return View("GetChargeInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

                /*
         * 연동회원 포인트를 환불 신청합니다.
         * - https://developers.popbill.com/reference/accountcheck/dotnetcore/api/point#Refund
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

                var response = _accountCheckService.Refund(corpNum, refundForm);
                return View("RefundResponse", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 포인트를 환불 신청합니다.
         * - https://developers.popbill.com/reference/accountcheck/dotnetcore/api/point#PaymentRequest
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

                var response = _accountCheckService.PaymentRequest(corpNum, paymentForm);
                return View("PaymentResponse", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 포인트 무통장 입금신청내역 1건을 확인합니다.
         *  - https://developers.popbill.com/reference/accountcheck/dotnetcore/api/point#GetSettleResult
         */
        public IActionResult GetSettleResult()
        {
            // 정산코드
            var settleCode = "202301130000000026";

            try
            {
                var paymentHistory = _accountCheckService.GetSettleResult(corpNum, settleCode);

                return View("PaymentHistory", paymentHistory);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 포인트 사용내역을 확인합니다.
         * - https://developers.popbill.com/reference/accountcheck/dotnetcore/api/point#GetUseHistory
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
                var useHistoryResult = _accountCheckService.GetUseHistory(corpNum, SDate, EDate, Page,
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
         * - https://developers.popbill.com/reference/accountcheck/dotnetcore/api/point#GetPaymentHistory
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
                var paymentHistoryResult = _accountCheckService.GetPaymentHistory(corpNum, SDate, EDate,
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
         * - https://developers.popbill.com/reference/accountcheck/dotnetcore/api/point#GetRefundHistory
         */
        public IActionResult GetRefundHistory()
        {
            // 목록 페이지번호 (기본값 1)
            var Page = 1;

            // 페이지당 표시할 목록 개수 (기본값 500, 최대 1,000)
            var PerPage = 100;

            try
            {
                var refundHistoryResult = _accountCheckService.GetRefundHistory(corpNum, Page, PerPage);

                return View("RefundHistoryResult", refundHistoryResult);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        /*
         * 포인트 환불에 대한 상세정보 1건을 확인합니다.
         * - https://developers.popbill.com/reference/accountcheck/dotnetcore/api/point#GetRefundInfo
         */
        public IActionResult GetRefundInfo()
        {
            // 환불 코드
            var refundCode = "023040000017";

            try
            {
                var response = _accountCheckService.GetRefundInfo(corpNum, refundCode);
                return View("RefundHistory", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }


        /*
         * 환불 가능한 포인트를 확인합니다. (보너스 포인트는 환불가능포인트에서 제외됩니다.)
         * - https://developers.popbill.com/reference/accountcheck/dotnetcore/api/point#GetRefundableBalance
         */
        public IActionResult GetRefundableBalance()
        {
            try
            {
                var refundableBalance = _accountCheckService.GetRefundableBalance(corpNum);
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
         * - https://developers.popbill.com/reference/accountcheck/dotnetcore/api/member#CheckIsMember
         */
        public IActionResult CheckIsMember()
        {
            try
            {
                //링크아이디
                string linkID = "TESTER";

                var response = _accountCheckService.CheckIsMember(corpNum, linkID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 사용하고자 하는 아이디의 중복여부를 확인합니다.
         * - https://developers.popbill.com/reference/accountcheck/dotnetcore/api/member#CheckID
         */
        public IActionResult CheckID()
        {
            //중복여부 확인할 팝빌 회원 아이디
            string checkID = "testkorea";

            try
            {
                var response = _accountCheckService.CheckID(checkID);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 사용자를 연동회원으로 가입처리합니다.
         * - https://developers.popbill.com/reference/accountcheck/dotnetcore/api/member#JoinMember
         */
        public IActionResult JoinMember()
        {
            JoinForm joinInfo = new JoinForm();

            // 링크아이디
            joinInfo.LinkID = "TESTER";

            // 아이디, 6자이상 50자 미만
            joinInfo.ID = "userid_20181212";

            //// 비밀번호, 8자이상 20자 미만 (영문, 숫자, 특수문자 조합)
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
                var response = _accountCheckService.JoinMember(joinInfo);
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
         * - https://developers.popbill.com/reference/accountcheck/dotnetcore/api/member#GetAccessURL
         */
        public IActionResult GetAccessURL()
        {
            try
            {
                var result = _accountCheckService.GetAccessURL(corpNum, userID);
                return View("Result", result);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 회사정보를 확인합니다.
         * - https://developers.popbill.com/reference/accountcheck/dotnetcore/api/member#GetCorpInfo
         */
        public IActionResult GetCorpInfo()
        {
            try
            {
                var response = _accountCheckService.GetCorpInfo(corpNum);
                return View("GetCorpInfo", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원의 회사정보를 수정합니다.
         * - https://developers.popbill.com/reference/accountcheck/dotnetcore/api/member#UpdateCorpInfo
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
                var response = _accountCheckService.UpdateCorpInfo(corpNum, corpInfo);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 담당자(팝빌 로그인 계정)를 추가합니다.
         * - https://developers.popbill.com/reference/accountcheck/dotnetcore/api/member#RegistContact
         */
        public IActionResult RegistContact()
        {
            Contact contactInfo = new Contact();

            // 담당자 아이디, 6자 이상 50자 미만
            contactInfo.id = "testkorea_20181212";

            //// 비밀번호, 8자이상 20자 미만 (영문, 숫자, 특수문자 조합)
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
                var response = _accountCheckService.RegistContact(corpNum, contactInfo);
                return View("Response", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 정보을 확인합니다.
         * - https://developers.popbill.com/reference/accountcheck/dotnetcore/api/member#GetContactInfo
         */
        public IActionResult GetContactInfo()
        {
            // 확인할 담당자 아이디
            string contactID = "test0730";

            try
            {
                var contactInfo = _accountCheckService.GetContactInfo(corpNum, contactID);
                return View("GetContactInfo", contactInfo);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 목록을 확인합니다.
         * - https://developers.popbill.com/reference/accountcheck/dotnetcore/api/member#ListContact
         */
        public IActionResult ListContact()
        {
            try
            {
                var response = _accountCheckService.ListContact(corpNum);
                return View("ListContact", response);
            }
            catch (PopbillException pe)
            {
                return View("Exception", pe);
            }
        }

        /*
         * 연동회원 사업자번호에 등록된 담당자(팝빌 로그인 계정) 정보를 수정합니다.
         * - https://developers.popbill.com/reference/accountcheck/dotnetcore/api/member#UpdateContact
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
                var response = _accountCheckService.UpdateContact(corpNum, contactInfo);
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
         *  - https://developers.popbill.com/reference/accountcheck/dotnetcore/api/member#QuitMember
         */
        public IActionResult QuitMember()
        {
            // 탈퇴 사유
            var quitReason = "탈퇴사유";

            try
            {
                var response = _accountCheckService.QuitMember(corpNum, quitReason);
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
