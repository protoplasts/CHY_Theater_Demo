using CHY_Theater.Areas.Booking.Models.ViewModels;
using CHY_Theater_DataAcess.Data;
using CHY_Theater_Models.Models;
using Microsoft.AspNetCore.Mvc;

using System.Collections.Specialized;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace CHY_Theater.Areas.Booking.Services
{
    public class ECPayService : ICommerce
    {
        private readonly Theater_ProjectDbContext _context;
        private readonly IHttpContextAccessor _accessor;

        public IConfiguration Config { get; set; }

        public ECPayService(Theater_ProjectDbContext eCPAY_TestContext, IHttpContextAccessor accessor)
        {
            Config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
            _context = eCPAY_TestContext;
            _accessor = accessor;
        }
      
        public string GetCallBack(ECPayViewModel inModel)
        {
            var userId = _accessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var orderId = inModel.MerchantTradeNo;
            var booking = _context.PaymentTransactions.Where(m => m.MerchantTradeNo == orderId).FirstOrDefault();
            var tradeDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            if (booking != null)
            {

                booking.TradeDate = tradeDate;
                booking.RtnMsg = "Pending";
                booking.TradeNo = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);
                booking.PaymentTypeChargeFee = "free";
            }
            

            //var ecpModel = new PaymentTransaction
            //{
            //    MerchantTradeNo = orderId,
            //    MemberID = userIdString,
            //    RtnMsg = "Pending",
            //    TradeNo = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20),
            //    PaymentType = inModel.PayOption,
            //    PaymentTypeChargeFee = "free",
            //    TradeDate = tradeDate,
            //    TradeAmt = inModel.TotalAmount
            //};
            //// Save inModel to database
            //_context.PaymentTransactions.Add(ecpModel);
            _context.SaveChanges();
            //需填入 你的網址
            var website = $"{Config.GetSection("HostURL").Value}/ECP";

            var order = new Dictionary<string, object>
            {
                //特店交易編號
                { "MerchantTradeNo",  orderId},

                //特店交易時間 yyyy/MM/dd HH:mm:ss
                { "MerchantTradeDate",  tradeDate},

                //交易金額
                { "TotalAmount",  inModel.Amt},

                //交易描述
                { "TradeDesc",  inModel.ItemDesc},

                //商品名稱
                { "ItemName", inModel.ItemDesc},

                //允許繳費有效天數(付款方式為 ATM 時，需設定此值)
                { "ExpireDate",  "3"},

                //自訂名稱欄位1
                { "Email",  inModel.Email},

                //自訂名稱欄位2
                { "CustomField2",  ""},

                //自訂名稱欄位3
                { "CustomField3",  ""},

                //自訂名稱欄位4
                { "CustomField4",  ""},

                //完成後發通知
                { "ReturnURL",  $"{Config.GetSection("HostURL").Value}/Notify/CallbackNotify?option=ECPay"},

                //付款完成後導頁
                { "OrderResultURL", $"{Config.GetSection("HostURL").Value}/booking/ECP/CallbackReturn?option=ECPay"},


                //付款方式為 ATM 時，當使用者於綠界操作結束時，綠界回傳 虛擬帳號資訊至 此URL
                { "PaymentInfoURL",$"{Config.GetSection("HostURL").Value}/Home/CallbackCustomer?option=ECPay"},

                //付款方式為 ATM 時，當使用者於綠界操作結束時，綠界會轉址至 此URL。
                { "ClientRedirectURL",  $"{Config.GetSection("HostURL").Value}/Home/CallbackCustomer?option=ECPay"},

                //特店編號， 2000132 測試綠界編號
                { "MerchantID",  "3002599"},

                //忽略付款方式
                { "IgnorePayment",  "GooglePay#WebATM#CVS#BARCODE"},

                //交易類型 固定填入 aio
                { "PaymentType",  "aio"},

                //選擇預設付款方式 固定填入 ALL
                { "ChoosePayment",  "Credit"},

                //CheckMacValue 加密類型 固定填入 1 (SHA256)
                { "EncryptType",  "1"},
            };

            //檢查碼
            order["CheckMacValue"] = GetCheckMacValue(order);

            StringBuilder s = new StringBuilder();
            s.AppendFormat("<form id='payForm' action='{0}' method='post'>", "https://payment-stage.ecpay.com.tw/Cashier/AioCheckOut/V5");
            foreach (KeyValuePair<string, object> item in order)
            {
                s.AppendFormat("<input type='hidden' name='{0}' value='{1}' />", item.Key, item.Value);
            }

            s.Append("</form>");

            return s.ToString();
        }
        /// <summary>
        /// 取得 檢查碼
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private string GetCheckMacValue(Dictionary<string, object> order)
        {
            var param = order.Keys.OrderBy(x => x).Select(key => key + "=" + order[key]).ToList();

            var checkValue = string.Join("&", param);

            //測試用的 HashKey
            var hashKey = "spPjZn66i0OhqJsQ";

            //測試用的 HashIV
            var HashIV = "hT5OJckN45isQTTs";

            checkValue = $"HashKey={hashKey}" + "&" + checkValue + $"&HashIV={HashIV}";

            checkValue = HttpUtility.UrlEncode(checkValue).ToLower();

            checkValue = EncryptSHA256(checkValue);

            return checkValue.ToUpper();
        }

        private string GetCheckMacValue(Dictionary<string, string> order)
        {
            var param = order.Keys.OrderBy(x => x).Select(key => key + "=" + order[key]).ToList();

            var checkValue = string.Join("&", param);

            //測試用的 HashKey
            var hashKey = "dALAm7IGaqMq8ebH";

            //測試用的 HashIV
            var HashIV = "ZV1cJFulEf25mRCG";

            checkValue = $"HashKey={hashKey}" + "&" + checkValue + $"&HashIV={HashIV}";

            checkValue = HttpUtility.UrlEncode(checkValue).ToLower();

            checkValue = EncryptSHA256(checkValue);

            return checkValue.ToUpper();
        }

        public ECPResultViewModel GetCallbackResult(IFormCollection form)
        {
            var data = new Dictionary<string, string>();
            foreach (string key in form.Keys)
            {
                data.Add(key, form[key]);
            }
            string temp = form["MerchantTradeNo"]; //寫在LINQ(下一行)會出錯，
            var booking = _context.Bookings.Where(m => m.MerchantTradeNo == temp).FirstOrDefault();
            if (booking != null)
            {

                if (form["RtnMsg"] == "Succeeded") booking.BookingStatus = "已付款";

            }

            var ecpayOrder = _context.PaymentTransactions.Where(m => m.MerchantTradeNo == temp).FirstOrDefault();
            if (ecpayOrder != null)
            {
                ecpayOrder.RtnCode = int.Parse(form["RtnCode"]);
                if (form["RtnMsg"] == "Succeeded") ecpayOrder.RtnMsg = "已付款";
                ecpayOrder.PaymentDate = Convert.ToDateTime(form["PaymentDate"]);
                ecpayOrder.SimulatePaid = int.Parse(form["SimulatePaid"]);
                _context.SaveChanges();

            };

            // 接收參數
            StringBuilder receive = new StringBuilder();
            foreach (var item in form)
            {
                receive.AppendLine(item.Key + "=" + item.Value + "<br>");
            }
            var result = new ECPResultViewModel
            {
                ReceiveObj = receive.ToString(),
                MerchantTradeNo = temp,
                RtnMsg = form["RtnMsg"]
            };

            // 解密訊息
            IConfiguration Config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
            string HashKey = Config.GetSection("HashKey").Value;//API 串接金鑰
            string HashIV = Config.GetSection("HashIV").Value;//API 串接密碼
            string TradeInfoDecrypt = DecryptAESHex(form["TradeInfo"], HashKey, HashIV);
            NameValueCollection decryptTradeCollection = HttpUtility.ParseQueryString(TradeInfoDecrypt);

            receive.Length = 0;
            foreach (String key in decryptTradeCollection.AllKeys)
            {
                receive.AppendLine(key + "=" + decryptTradeCollection[key] + "<br>");
            }
            result.TradeInfo = receive.ToString();

            return result;
        }

        public string GetPeriodCallBack(ECPayViewModel inModel)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 16 進制字串解密
        /// </summary>
        /// <param name="source">加密前字串</param>
        /// <param name="cryptoKey">加密金鑰</param>
        /// <param name="cryptoIV">cryptoIV</param>
        /// <returns>解密後的字串</returns>
        public string DecryptAESHex(string source, string cryptoKey, string cryptoIV)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(source))
            {
                // 將 16 進制字串 轉為 byte[] 後
                byte[] sourceBytes = ToByteArray(source);

                if (sourceBytes != null)
                {
                    // 使用金鑰解密後，轉回 加密前 value
                    result = Encoding.UTF8.GetString(DecryptAES(sourceBytes, cryptoKey, cryptoIV)).Trim();
                }
            }

            return result;
        }
        /// <summary>
        /// 將16進位字串轉換為byteArray
        /// </summary>
        /// <param name="source">欲轉換之字串</param>
        /// <returns></returns>
        public byte[] ToByteArray(string source)
        {
            byte[] result = null;

            if (!string.IsNullOrWhiteSpace(source))
            {
                var outputLength = source.Length / 2;
                var output = new byte[outputLength];

                for (var i = 0; i < outputLength; i++)
                {
                    output[i] = Convert.ToByte(source.Substring(i * 2, 2), 16);
                }
                result = output;
            }

            return result;
        }

        /// <summary>
        /// 字串解密AES
        /// </summary>
        /// <param name="source">解密前字串</param>
        /// <param name="cryptoKey">解密金鑰</param>
        /// <param name="cryptoIV">cryptoIV</param>
        /// <returns>解密後字串</returns>
        public byte[] DecryptAES(byte[] source, string cryptoKey, string cryptoIV)
        {
            byte[] dataKey = Encoding.UTF8.GetBytes(cryptoKey);
            byte[] dataIV = Encoding.UTF8.GetBytes(cryptoIV);

            using (var aes = System.Security.Cryptography.Aes.Create())
            {
                aes.Mode = System.Security.Cryptography.CipherMode.CBC;
                // 智付通無法直接用PaddingMode.PKCS7，會跳"填補無效，而且無法移除。"
                // 所以改為PaddingMode.None並搭配RemovePKCS7Padding
                aes.Padding = System.Security.Cryptography.PaddingMode.None;
                aes.Key = dataKey;
                aes.IV = dataIV;

                using (var decryptor = aes.CreateDecryptor())
                {
                    byte[] data = decryptor.TransformFinalBlock(source, 0, source.Length);
                    int iLength = data[data.Length - 1];
                    var output = new byte[data.Length - iLength];
                    Buffer.BlockCopy(data, 0, output, 0, output.Length);
                    return output;
                }
            }
        }

        /// <summary>
        /// 加密後再轉 16 進制字串
        /// </summary>
        /// <param name="source">加密前字串</param>
        /// <param name="cryptoKey">加密金鑰</param>
        /// <param name="cryptoIV">cryptoIV</param>
        /// <returns>加密後的字串</returns>
        public string EncryptAESHex(string source, string cryptoKey, string cryptoIV)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(source))
            {
                var encryptValue = EncryptAES(Encoding.UTF8.GetBytes(source), cryptoKey, cryptoIV);

                if (encryptValue != null)
                {
                    result = BitConverter.ToString(encryptValue)?.Replace("-", string.Empty)?.ToLower();
                }
            }

            return result;
        }

        /// <summary>
        /// 字串加密AES
        /// </summary>
        /// <param name="source">加密前字串</param>
        /// <param name="cryptoKey">加密金鑰</param>
        /// <param name="cryptoIV">cryptoIV</param>
        /// <returns>加密後字串</returns>
        public byte[] EncryptAES(byte[] source, string cryptoKey, string cryptoIV)
        {
            byte[] dataKey = Encoding.UTF8.GetBytes(cryptoKey);
            byte[] dataIV = Encoding.UTF8.GetBytes(cryptoIV);

            using (var aes = System.Security.Cryptography.Aes.Create())
            {
                aes.Mode = System.Security.Cryptography.CipherMode.CBC;
                aes.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
                aes.Key = dataKey;
                aes.IV = dataIV;

                using (var encryptor = aes.CreateEncryptor())
                {
                    return encryptor.TransformFinalBlock(source, 0, source.Length);
                }
            }
        }

        /// <summary>
        /// 字串加密SHA256
        /// </summary>
        /// <param name="source">加密前字串</param>
        /// <returns>加密後字串</returns>
        public string EncryptSHA256(string source)
        {
            string result = string.Empty;

            using (System.Security.Cryptography.SHA256 algorithm = System.Security.Cryptography.SHA256.Create())
            {
                var hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(source));

                if (hash != null)
                {
                    result = BitConverter.ToString(hash)?.Replace("-", string.Empty)?.ToUpper();
                }

            }
            return result;
        }
    }
}
