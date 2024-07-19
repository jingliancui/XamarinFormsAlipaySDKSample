using AlipaySDK_ApiDefinition;
using Foundation;
using System.Diagnostics;
using UIKit;

namespace SampleApp
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override bool OpenUrl(UIApplication application, NSUrl url, NSDictionary options)
        {
            AlipaySDK.DefaultService.ProcessOrderWithPaymentResult(url, resultDic => 
            {
                IDictionary<string, string> result = new Dictionary<string, string>();
                resultDic.Keys?.ToList().ForEach(key =>
                {
                    var getOk = resultDic.TryGetValue(key, out NSObject obj);
                    if (getOk)
                    {
                        result.Add(key.ToString(), obj.ToString());
                    }

                    Debug.WriteLine($"{result.Keys.FirstOrDefault()},{result.Values.FirstOrDefault()}");
                });
            });

            return true;
        }
    }
}
