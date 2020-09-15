using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Com.Alipay.Sdk.App;
using Com.Alipay.Sdk.Pay.Demo.Util;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SampleApp.Droid
{
    [Activity(Label = "SampleApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            Xamarin.Forms.MessagingCenter.Subscribe<object>(this, MessagingCenterKey.Pay, obj =>
            {                
                /*
		         * 这里只是为了方便直接向商户展示支付宝的整个支付流程；所以Demo中加签过程直接放在客户端完成；
		         * 真实App里，privateKey等数据严禁放在客户端，加签过程务必要放在服务端完成；
		         * 防止商户私密数据泄露，造成不必要的资金损失，及面临各种安全风险； 
		         * 
		         * orderInfo 的获取必须来自服务端；
		         */
                var appid = "";
                var rsa2 = true;
                var para = OrderInfoUtil2_0.BuildOrderParamMap(appid, rsa2);
                var orderParam = OrderInfoUtil2_0.BuildOrderParam(para);
                var privateKey = "";
                var sign = OrderInfoUtil2_0.GetSign(para, privateKey, rsa2);
                var orderInfo = orderParam + "&" + sign;

                // 必须异步调用
                Task.Run(() =>
                {
                    var alipay = new PayTask(this);
                    var result = alipay.PayV2(orderInfo, true);                    
                    MessagingCenter.Send(new object(), MessagingCenterKey.Payed, result);
                });                
            });

            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}