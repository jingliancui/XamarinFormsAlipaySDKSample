using System;
using System.Collections.Generic;
using System.Linq;
using AlipaySDKBinding;
using DemoUtilsBinding;
using Foundation;
using UIKit;
using Xamarin.Forms;

namespace SampleApp.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            MessagingCenter.Subscribe<object>(this, MessagingCenterKey.Pay, obj =>
            {
                var util = new DemoUtils();
                var tradeNo = util.GenerateTradeNO;

                //将商品信息赋予AlixPayOrder的成员变量
                var order = new APOrderInfo
                {
                    // NOTE: app_id设置
                    App_id = "",

                    // NOTE: 支付接口名称
                    Method = "alipay.trade.app.pay",

                    // NOTE: 参数编码格式
                    Charset = "utf-8",

                    // NOTE: 当前时间点
                    Timestamp = string.Format("yyyy-MM-dd HH:mm:ss", DateTime.Now),

                    // NOTE: 支付版本
                    Version = "1.0",

                    //NOTE: sign_type设置
                    Sign_type = "RSA",

                    // NOTE: 商品数据
                    Biz_content = new APBizContent
                    {
                        Body = "我是测试数据",
                        Subject = "1",
                        Out_trade_no = tradeNo,
                        Timeout_express = "30ms",//超时时间设置
                        Total_amount = string.Format("%.2f", 0.01)//商品价格
                    }                    
                };

                //将商品信息拼接成字符串
                var orderInfo = "";
                var orderinfoEncoded = "";
                orderInfo = order.OrderInfoEncoded(false);
                orderinfoEncoded = order.OrderInfoEncoded(true);

                // NOTE: 获取私钥并将商户信息签名，外部商户的加签过程请务必放在服务端，防止公私钥数据泄露；
                //       需要遵循RSA签名规范，并将签名字符串base64编码和UrlEncode

                var privateKey = "";

                var signer = new APRSASigner(privateKey);

                var signerStr = signer.SignString(orderInfo, true);

                // NOTE: 如果加签成功，则继续执行支付
                if (!string.IsNullOrEmpty(signerStr))
                {
                    //应用注册scheme,在AliSDKDemo-Info.plist定义URL types
                    var appScheme = "alisdkdemo";

                    // NOTE: 将签名成功字符串格式化为订单字符串,请严格按照该格式
                    var orderString = string.Format("%@&sign=%@", orderinfoEncoded, signerStr);

                    // NOTE: 调用支付结果开始支付
                    AlipaySDK.DefaultService.PayOrder(orderString, appScheme, resultDic =>
                    {
                        var result = resultDic;

                        //ios9.0以及后续新版本需要在下方重载OpenUrl中获取回调信息
                    }); 
                }
            });

            return base.FinishedLaunching(app, options);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            AlipaySDK.DefaultService.ProcessOrderWithPaymentResult(url, resultDic =>
            {
                IDictionary<string, string> result = new Dictionary<string, string>();
                resultDic.Keys?.ToList().ForEach(key => 
                {                   
                    var getOk=resultDic.TryGetValue(key, out NSObject obj);
                    if (getOk)
                    {
                        result.Add(key.ToString(), obj.ToString());
                    }
                });

                MessagingCenter.Send(new object(), MessagingCenterKey.Payed, result);
            });
            //return base.OpenUrl(app, url, options);
            return true;
        }
    }
}
