
namespace WexinTest
{
    public class WexinUserProfile
    {
        /// <summary>
        /// 指出用戶是否有關注公眾號，0表時沒有關注，將會擷取不到其他的用戶資料
        /// </summary>
        public string subscribe { get; set; }
        /// <summary>
        /// 對目前公眾號的唯一id
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// 用戶匿稱
        /// </summary>
        public string nickname { get; set; }
        /// <summary>
        /// 用戶性別
        /// </summary>
        public string sex { get; set; }
        /// <summary>
        /// 用戶所在城市
        /// </summary>
        public string city { get; set; }
        /// <summary>
        /// 用戶所國家
        /// </summary>
        public string country { get; set; }
        /// <summary>
        /// 用戶所在省份
        /// </summary>
        public string province { get; set; }
        /// <summary>
        /// 用戶使用語言
        /// <para>簡體中文微zh_CN</para>
        /// </summary>
        public string language { get; set; }
        /// <summary>
        /// 用戶頭像url
        /// </summary>
        public string headimgurl { get; set; }
        /// <summary>
        /// 用戶關注時間
        /// <para>此為時間戳記，從1979/1/1 00:00:00起算的秒數</para>
        /// </summary>
        public string subscribe_time { get; set; }
        /// <summary>
        /// 微信開放平台識別id
        /// <para>只有公眾號綁定到微信開放平台帳號之後，才會出現unionid</para>
        /// </summary>
        public string unionid { get; set; }
    }
}
