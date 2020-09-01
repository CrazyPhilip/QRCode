using HouseSource.Themes;
using Plugin.Toast;
using Plugin.Toast.Abstractions;
using QRCode.Util;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace QRCode.ViewModels
{
    public class SettingViewModel : BaseViewModel
    {
        private bool darkModeIsToggled;   //Comment
        public bool DarkModeIsToggled
        {
            get { return darkModeIsToggled; }
            set { SetProperty(ref darkModeIsToggled, value); }
        }

        private string rate;   //Comment
        public string Rate
        {
            get { return rate; }
            set { SetProperty(ref rate, value); }
        }

        private string currentVersion;   //Comment
        public string CurrentVersion
        {
            get { return currentVersion; }
            set { SetProperty(ref currentVersion, value); }
        }

        private string newestVersion;   //Comment
        public string NewestVersion
        {
            get { return newestVersion; }
            set { SetProperty(ref newestVersion, value); }
        }

        public Command ThemeCommand { get; set; }
        public Command ClearCacheCommand { get; set; }
        public Command UpdateCommand { get; set; }
        public Command MoreCommand { get; set; }

        public SettingViewModel()
        {
            DarkModeIsToggled = GlobalVariables.DarkMode;
            CurrentVersion = AppInfo.VersionString;

            ThemeCommand = new Command(() =>
            {
                GlobalVariables.DarkMode = DarkModeIsToggled;
                Theme theme = GlobalVariables.DarkMode ? Theme.Dark : Theme.Light;

                ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
                if (mergedDictionaries != null)
                {
                    mergedDictionaries.Clear();

                    switch (theme)
                    {
                        case Theme.Dark:
                            mergedDictionaries.Add(new DarkTheme());
                            break;
                        case Theme.Light:
                        default:
                            mergedDictionaries.Add(new LightTheme());
                            break;
                    }
                }
            }, () => { return true; });

            ClearCacheCommand = new Command(() =>
            {
                //LocalDatabaseService localDatabaseService = new LocalDatabaseService();
                //int total = await localDatabaseService.ClearAllData();
                //if (total > 0)
                //{
                //    CrossToastPopUp.Current.ShowToastSuccess("清理完成，共清理" + total.ToString() + "条数据", ToastLength.Long);
                //}
                CrossToastPopUp.Current.ShowToastMessage("下次更新，感谢使用", ToastLength.Long);
            }, () => { return true; });

            UpdateCommand = new Command(() =>
            {
                //await CheckAppVersionAsync();
                CrossToastPopUp.Current.ShowToastMessage("请到应用商店更新，感谢使用", ToastLength.Long);
            }, () => { return true; });

            MoreCommand = new Command(async () =>
            {
                await Browser.OpenAsync("http://www.crazyphilip.space/", new BrowserLaunchOptions
                {
                    LaunchMode = BrowserLaunchMode.SystemPreferred,
                    TitleMode = BrowserTitleMode.Show,
                    PreferredToolbarColor = Color.FromHex("#2196F3"),
                    PreferredControlColor = Color.FromHex("#2196F3")
                });
            }, () => { return true; });

        }

        /*
        private async Task CheckAppVersionAsync()
        {
            try
            {
                if (!Tools.IsNetConnective())
                {
                    CrossToastPopUp.Current.ShowToastError("无网络连接，请检查网络。", ToastLength.Long);
                    return;
                }

                string result = await RestSharpService.GetNewestVersion();

                JObject jObject = JObject.Parse(result);
                if (jObject["Msg"].ToString() == "failed")
                {
                    CrossToastPopUp.Current.ShowToastError("获取失败", ToastLength.Short);
                    return;
                }

                bool action = await Application.Current.MainPage.DisplayAlert("更新", "最新版本：" + jObject["VersionCode"].ToString() + "\n" + jObject["Remarks"].ToString(), "确定", "取消");
                if (action)
                {
                    await Browser.OpenAsync(jObject["DownloadUrl"].ToString(), BrowserLaunchMode.SystemPreferred);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }*/
    }
}
