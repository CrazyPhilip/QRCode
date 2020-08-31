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
                SelectImage();
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
                    ScanColor = Color.DeepSkyBlue, // 扫描框颜色
                    ShowFlash = true // 闪光灯
                };

                var overlay = new ZXingScanOverlay(options);
                var csPage = new ZXingScanPage(overlay);

                csPage.OnScanResult = (result) =>
                {
                    if (result != null)
                    {
                        Text = Base64Helper.Base64Decode(result.Text);
                        //registerViewModel.Invitation = JsonConvert.DeserializeObject<InvitationInfo>(decodeText);
                        //Console.WriteLine(JsonConvert.DeserializeObject<InvitationInfo>(decodeText));

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

                using (FileStream fs = new FileStream(image, FileMode.Open))
                {
                    byte[] bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, bytes.Length);

                    BarcodeReader reader = new BarcodeReader();
                    string text = reader.Decode(bytes).Text;
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        Text = text;
                    }
                    else
                    {
                        CrossToastPopUp.Current.ShowToastError("没有检测到二维码", ToastLength.Long);
                    }
                }
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
