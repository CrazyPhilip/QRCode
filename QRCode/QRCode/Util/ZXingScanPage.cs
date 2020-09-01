using System;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace QRCode.Util
{
    /// <summary>
    /// ZXingScanPage
    /// </summary>
    public class ZXingScanPage : ContentPage
    {
        private ZXingScannerView _zxing;
        private ZXingScanOverlay _overlay;

        public ZXingScanPage(ZXingScanOverlay overlay = null) : base()
        {
            _overlay = overlay ?? new ZXingScanOverlay();

            Title = "扫一扫";

            _zxing = new ZXingScannerView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                AutomationId = "zxingScannerView",
            };

            // 返回结果
            _zxing.OnScanResult += (result) =>
                Device.BeginInvokeOnMainThread(async () =>
                {
                    _zxing.IsAnalyzing = false;

                    await Navigation.PopModalAsync();

                    OnScanResult?.Invoke(result);
                });

            // 闪光灯
            _overlay.Options.FlashTappedAction = () =>
            {
                _zxing.IsTorchOn = !_zxing.IsTorchOn;
            };

            var grid = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            grid.Children.Add(_zxing);
            grid.Children.Add(_overlay);

            Content = grid;
        }

        // 扫描结果
        public Action<ZXing.Result> OnScanResult { get; set; }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            _zxing.IsScanning = true;

            if (_overlay != null && _overlay.Options.ShowScanAnimation)
                await _overlay.ScanAnimationAsync();
        }

        protected override void OnDisappearing()
        {
            _zxing.IsScanning = false;

            base.OnDisappearing();
        }
    }
}
