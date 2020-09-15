using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SampleApp
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<object, IDictionary<string, string>>(this, MessagingCenterKey.Payed, (sender, args) => 
            {
                var strBuilder = new StringBuilder();
                foreach (var item in args.Keys)
                {
                    var resultKey = item;
                    var resultValue = args[item];
                    strBuilder.Append(resultKey);
                    strBuilder.Append(":");
                    strBuilder.Append(resultValue);
                    strBuilder.Append(",");
                }
                Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() => 
                {
                    PayReslutEditor.Text = strBuilder.ToString();
                });                
            });
        }

        private void PayBtn_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send(new object(), MessagingCenterKey.Pay);
        }
    }
}
