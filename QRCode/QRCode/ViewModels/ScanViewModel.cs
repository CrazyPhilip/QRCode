using Newtonsoft.Json.Linq;
using Plugin.Toast;
using Plugin.Toast.Abstractions;
using QRCode.Models;
using QRCode.Util;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing;
using ZXing.Common;
using System.Drawing;

namespace QRCode.ViewModels
{
    public class ScanViewModel : BaseViewModel
    {
        private string text;   //文字
        public string Text
        {
            get { return text; }
            set { SetProperty(ref text, value); }
        }

        private ObservableCollection<JsonItem> jsonList;   //键值对列表
        public ObservableCollection<JsonItem> JsonList
        {
            get { return jsonList; }
            set { SetProperty(ref jsonList, value); }
        }

        public Command ScanCommand { get; set; }
        public Command ParseCommand { get; set; }
        public Command CopyCommand { get; set; }
        public Command SelectImageCommand { get; set; }

        public ScanViewModel()
        {
            ScanCommand = new Command(() =>
            {
                OnQRCodeButton();
            }, () => { return true; });

            ParseCommand = new Command(() =>
            {
                if (!string.IsNullOrWhiteSpace(Text))
                {
                    if (!JsonHelper.IsJson(Text))
                    {
                        CrossToastPopUp.Current.ShowToastError("该文本不是Json格式", ToastLength.Long);
                        return;
                    }

                    JsonList = new ObservableCollection<JsonItem>();

                    JObject jObject = JObject.Parse(Text);
                    int i = 0;
                    foreach (JToken child in jObject.Children())
                    {
                        var property = child as JProperty;
                        if (property != null)
                        {
                            JsonList.Add(new JsonItem { Id = i, Key = property.Name, Value = property.Value.ToString() });
                            i++;
                        }
                    }
                }
                else
                {
                    CrossToastPopUp.Current.ShowToastError("空字符串，请检查", ToastLength.Long);
                    return;
                }
            }, () => { return true; });

            CopyCommand = new Command(async () =>
            {
                if (!string.IsNullOrWhiteSpace(Text))
                {
                    await Clipboard.SetTextAsync(Text);
                }
            }, () => { return true; });

            SelectImageCommand = new Command(() =>
            {
                CrossToastPopUp.Current.ShowToastMessage("暂未开放，感谢使用", ToastLength.Long);
                //SelectImage();
            }, () => { return true; });

        }

        /// <summary>
        /// 响应扫码按钮
        /// </summary>
        private async void OnQRCodeButton()
        {
            if (await GetCameraPermissionAsync())
            {
                var options = new ZXingScanOverlayOptions()
                {
                    ScanColor = Xamarin.Forms.Color.DeepSkyBlue, // 扫描框颜色
                    ShowFlash = true // 闪光灯
                };

                var overlay = new ZXingScanOverlay(options);
                var csPage = new ZXingScanPage(overlay);

                csPage.OnScanResult = (result) =>
                {
                    if (result != null)
                    {
                        Text = Base64Helper.IsBase64(result.Text) ? Base64Helper.Base64Decode(result.Text) : result.Text;
                    }
                };

                await Application.Current.MainPage.Navigation.PushModalAsync(csPage);
            }
        }

        /// <summary>
        /// 响应选择图片
        /// </summary>
        private async void SelectImage()
        {
            if (await GetReadPermissionAsync())
            {
                string image = await DependencyService.Get<IImagePickerService>().PickImageAsync();

                if (string.IsNullOrWhiteSpace(image))
                {
                    CrossToastPopUp.Current.ShowToastWarning("取消选择", ToastLength.Long);
                    return;
                }
                else
                {
                    Text = image;
                }

                //using (FileStream fs = new FileStream(image, FileMode.Open))
                //{
                //    byte[] bytes = new byte[fs.Length];
                //    fs.Read(bytes, 0, bytes.Length);

                //    BarcodeReader reader = new BarcodeReader()
                //    {
                //        Options = new ZXing.Common.DecodingOptions
                //        {
                //            TryHarder = false,
                //            PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE }
                //        }
                //    };
                //    reader.AutoRotate = true;
                //    reader.ResultFound += Reader_ResultFound;
                //    reader.Decode(bytes);
                //}

                //using (FileStream fs = new FileStream(image, FileMode.Open))
                //{
                //    Bitmap bitmap = (Bitmap)System.Drawing.Image.FromFile(image);

                //    int h = bitmap.Height;
                //    int w = bitmap.Width;

                //    byte[] bytes = new byte[fs.Length];
                //    fs.Read(bytes, 0, bytes.Length);



                //    RGBLuminanceSource rGBLuminanceSource = new RGBLuminanceSource(bytes, w, h);
                //    BinaryBitmap binaryBitmap = new BinaryBitmap(new HybridBinarizer(rGBLuminanceSource));
                //    Result result = new MultiFormatReader().decode(binaryBitmap);

                //    if (!string.IsNullOrWhiteSpace(result.Text))
                //    {
                //        Text = result.Text;
                //    }
                //    else
                //    {
                //        CrossToastPopUp.Current.ShowToastError("没有检测到二维码", ToastLength.Long);
                //    }

                //    bitmap.Dispose();
                //}
            }
        }

        private void Reader_ResultFound(Result obj)
        {
            if (!string.IsNullOrWhiteSpace(obj.Text))
            {
                Text = obj.Text;
            }
            else
            {
                CrossToastPopUp.Current.ShowToastError("没有检测到二维码", ToastLength.Long);
            }
        }

        /// <summary>
        /// 获取摄像头权限
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetCameraPermissionAsync()
        {
            var status = await PermissionHelper.CheckAndRequestPermissionAsync(new Permissions.Camera());
            if (status != PermissionStatus.Granted)
            {
                CrossToastPopUp.Current.ShowToastMessage("摄像头权限：" + status, ToastLength.Long);
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 获取存储权限
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetReadPermissionAsync()
        {
            var status = await PermissionHelper.CheckAndRequestPermissionAsync(new Permissions.StorageRead());
            if (status != PermissionStatus.Granted)
            {
                CrossToastPopUp.Current.ShowToastMessage("存储权限：" + status, ToastLength.Long);
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
