using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QRCode.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScanPage : ContentPage
    {
        public ScanPage()
        {
            InitializeComponent();

        }

        /*
        private void SacnButton_Clicked(object sender, EventArgs e)
        {
            OnQRCodeButton();
        }

        /// <summary>
        /// 响应扫码按钮
        /// </summary>
        private async void OnQRCodeButton()
        {
            if (await CheckPermission())
            {
                var options = new ZXingScanOverlayOptions()
                {
                    ScanColor = Color.DeepSkyBlue, // 扫描框颜色
                    ShowFlash = true // 闪光灯
                };

                var overlay = new ZXingScanOverlay(options);
                var csPage = new ZXingScanPage(overlay);

                csPage.OnScanResult = (result) =>
                {
                    if (result != null)
                    {
                        string decodeText = Base64Helper.Base64Decode(result.Text);
                        //registerViewModel.Invitation = JsonConvert.DeserializeObject<InvitationInfo>(decodeText);
                        //Console.WriteLine(JsonConvert.DeserializeObject<InvitationInfo>(decodeText));

                        ProductInfo productInfo = JsonConvert.DeserializeObject<ProductInfo>(decodeText);
                        scanViewModel.ProductName = productInfo.ProductName;
                        scanViewModel.Weight = productInfo.Weight;
                        scanViewModel.RecipientName = productInfo.RecipientName;
                        scanViewModel.RecipientPhone = productInfo.RecipientPhone;
                        scanViewModel.RecipientAddress = productInfo.RecipientAddress;
                        scanViewModel.SenderName = productInfo.SenderName;
                        scanViewModel.SenderPhone = productInfo.SenderPhone;
                        scanViewModel.SenderAddress = productInfo.SenderAddress;
                    }
                };

                await Navigation.PushModalAsync(csPage);
            }
        }

        // 检查权限
        // 如果不做权限检查，当用户拒绝授权时页面会挂起
        private async Task<bool> CheckPermission()
        {
            var current = CrossPermissions.Current;

            // 检查权限
            var status = await current.CheckPermissionStatusAsync(Permission.Camera);

            if (status != PermissionStatus.Granted)
            {
                var results = await current.RequestPermissionsAsync(Permission.Camera);
                status = results[Permission.Camera];
            }

            return status == PermissionStatus.Granted;
        }
        */
    }
}