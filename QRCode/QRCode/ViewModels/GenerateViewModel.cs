using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Plugin.Toast;
using Plugin.Toast.Abstractions;
using QRCode.Models;
using QRCode.Util;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace QRCode.ViewModels
{
    public class GenerateViewModel : BaseViewModel
    {
        private string jsonBarCode;   //Json二维码
        public string JsonBarCode
        {
            get { return jsonBarCode; }
            set { SetProperty(ref jsonBarCode, value); }
        }

        private string plainTextBarCode;   //自由文本二维码
        public string PlainTextBarCode
        {
            get { return plainTextBarCode; }
            set { SetProperty(ref plainTextBarCode, value); }
        }

        private ObservableCollection<JsonItem> jsonList;   //键值对列表
        public ObservableCollection<JsonItem> JsonList
        {
            get { return jsonList; }
            set { SetProperty(ref jsonList, value); }
        }

        private string plainText;   //自由文本
        public string PlainText
        {
            get { return plainText; }
            set { SetProperty(ref plainText, value); }
        }

        private int index { get; set; }

        public Command<int> DeleteCommand { get; set; }
        public Command ClearJsonCommand { get; set; }
        public Command ClearPlainTextCommand { get; set; }
        public Command GenerateJsonCommand { get; set; }
        public Command GeneratePlainTextCommand { get; set; }
        public Command AddRowCommand { get; set; }
        public Command<string> SaveCommand { get; set; }

        public GenerateViewModel()
        {
            //BarCode = string.Empty;
            index = 0;
            JsonList = new ObservableCollection<JsonItem>()
            {
                new JsonItem { Id = index, Checked = true, Key = string.Empty, Value = string.Empty }
            };

            DeleteCommand = new Command<int>((id) =>
            {
                if (JsonList.Count == 1) { return; }
                int i = 0;
                foreach (var item in JsonList)
                {
                    if (item.Id == id)
                    {
                        i = JsonList.IndexOf(item);
                    }
                }
                JsonList.RemoveAt(i);
                //JsonList.RemoveAt(id);
                //index--;
                //for (int i = 0; i < JsonList.Count; i++)
                //{
                //    JsonList[i].Id = i;
                //}
            }, (id) => { return true; });

            ClearJsonCommand = new Command(() =>
            {
                index = 0;
                JsonList = new ObservableCollection<JsonItem>()
                {
                    new JsonItem { Id = index, Checked = true, Key = string.Empty, Value = string.Empty }
                };
            }, () => { return true; });

            GenerateJsonCommand = new Command(() =>
            {
                GenerateJson();
            }, () => { return true; });

            AddRowCommand = new Command(() =>
            {
                index++;
                JsonList.Add(new JsonItem { Id = index, Checked = true, Key = string.Empty, Value = string.Empty });
            }, () => { return true; });

            ClearPlainTextCommand = new Command(async () =>
            {
                bool action = await Application.Current.MainPage.DisplayAlert("Warning", "请问要清空自由文本吗？", "确定", "取消");
                if (action)
                {
                    PlainText = string.Empty;
                }
            }, () => { return true; });

            GeneratePlainTextCommand = new Command(() =>
            {
                if (string.IsNullOrWhiteSpace(PlainText))
                {
                    CrossToastPopUp.Current.ShowToastWarning("空字符串，请检查", ToastLength.Long);
                    return;
                }
                else
                {
                    string base64 = Base64Helper.Base64Encode(Encoding.UTF8, PlainText);
                    PlainTextBarCode = base64;
                }
            }, () => { return true; });

            SaveCommand = new Command<string>(async (code) =>
            {
                if (await GetReadPermissionAsync())
                {
                    MessagingCenter.Send(new object(), "Save", code);
                }
            }, (code) => { return true; });

        }

        /// <summary>
        /// 生成二维码 json格式
        /// </summary>
        private void GenerateJson()
        {
            string value = "";
            int count = JsonList.Count;
            for (int i = 0; i < count; i++)
            {
                if (string.IsNullOrWhiteSpace(JsonList[i].Key) || !JsonList[i].Checked) { continue; }
                value += "\"" + JsonList[i].Key + "\":\"" + JsonList[i].Value + "\"";
                value += count - i == 1 ? "" : ",";
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                CrossToastPopUp.Current.ShowToastWarning("空字符串，请检查", ToastLength.Long);
                return;
            }
            else
            {
                value = "{" + value + "}";
            }

            string base64 = Base64Helper.Base64Encode(Encoding.UTF8, value);
            JsonBarCode = base64;

        }

        /// <summary>
        /// 获取存储权限
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetReadPermissionAsync()
        {
            var status = await PermissionHelper.CheckAndRequestPermissionAsync(new Permissions.StorageWrite());
            if (status != PermissionStatus.Granted)
            {
                CrossToastPopUp.Current.ShowToastMessage("写入权限：" + status, ToastLength.Long);
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
