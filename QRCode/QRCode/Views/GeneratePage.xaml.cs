using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;
using QRCode.Models;
using Newtonsoft.Json;
using QRCode.Util;
using QRCode.ViewModels;

namespace QRCode.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GeneratePage : ContentPage
    {
        GenerateViewModel generateViewModel = new GenerateViewModel();


        public GeneratePage()
        {
            InitializeComponent();

            BindingContext = generateViewModel;
        }

        private void ClearButton_Clicked(object sender, EventArgs e)
        {
            generateViewModel.ProductName = "";
            generateViewModel.Weight = "";
            generateViewModel.RecipientName = "";
            generateViewModel.RecipientPhone = "";
            generateViewModel.RecipientAddress = "";
            generateViewModel.SenderName = "";
            generateViewModel.SenderPhone = "";
            generateViewModel.SenderAddress = "";
        }

        private void GenerateButton_Clicked(object sender, EventArgs e)
        {
            ZXingBarcodeImageView barcode = new ZXingBarcodeImageView
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                BarcodeFormat = ZXing.BarcodeFormat.QR_CODE,
                WidthRequest = 200,
                HeightRequest = 200,
                Margin = 0,
                BarcodeOptions = { Height = 200, Width = 200, Margin = 0 }
            };

            ProductInfo productInfo = new ProductInfo
            {
                ProductName = generateViewModel.ProductName,
                Weight = generateViewModel.Weight,
                RecipientName = generateViewModel.RecipientName,
                RecipientPhone = generateViewModel.RecipientPhone,
                RecipientAddress = generateViewModel.RecipientAddress,
                SenderName = generateViewModel.SenderName,
                SenderPhone = generateViewModel.SenderPhone,
                SenderAddress = generateViewModel.SenderAddress
            };

            string value = JsonConvert.SerializeObject(productInfo);
            string base64 = Base64Helper.Base64Encode(value);
            //Console.WriteLine(base64);
            //barcode.BarcodeValue = base64;
            generateViewModel.BarCode = base64;

            //CodeStack.Children.Add(barcode);
        }
    }
}