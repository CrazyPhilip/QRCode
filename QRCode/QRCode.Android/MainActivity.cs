using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Threading.Tasks;
using Android.Content;
using CarouselView.FormsPlugin.Android;
using ZXing;
using Android.Graphics;
using ZXing.Common;
using Xamarin.Essentials;
using System.IO;
using Path = System.IO.Path;
using Xamarin.Forms;
using Plugin.Toast;
using ToastLength = Plugin.Toast.Abstractions.ToastLength;
using Android.Database;
using Android.Provider;
using Android.Net;

namespace QRCode.Droid
{
    [Activity(Label = "QRCode", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            Current = this;

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;


            MessagingCenter.Subscribe<object, string>(this, "Save", (sender, args) =>
            {
                var path = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/qrcode";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var file = Path.Combine(path, DateTime.Now.ToString("yyyyMMddhhmmss") + ".png");
                using (FileStream fs = new FileStream(file, FileMode.Create))
                {
                    BarcodeWriter<Bitmap> writer = new BarcodeWriter<Bitmap>();
                    writer.Format = BarcodeFormat.QR_CODE;
                    writer.Options = new EncodingOptions()
                    {
                        Height = 512,
                        Width = 512
                    };
                    writer.Renderer = new BitmapRenderer();
                    Bitmap bitmap = writer.Write(args);
                    //bitmap.Save(Path.Combine(FileSystem.AppDataDirectory, DateTime.Now.ToString()));
                    bitmap.Compress(Bitmap.CompressFormat.Png, 100, fs);
                    fs.Flush();
                }

                RunOnUiThread(() =>
                {
                    if (File.Exists(file))
                    {
                        CrossToastPopUp.Current.ShowToastSuccess("已保存到" + file, ToastLength.Long);
                    }
                    else
                    {
                        CrossToastPopUp.Current.ShowToastError("出现错误", ToastLength.Long);
                    }
                });
            });
            
            /*
            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.Kitkat)
            {                
                //透明状态栏                
                Window.AddFlags(WindowManagerFlags.TranslucentStatus);
                //透明导航栏                
                Window.AddFlags(WindowManagerFlags.TranslucentNavigation);            
            }*/
            //this.SetTheme(Android.Resource.Style.ThemeNoTitleBarFullScreen);//全屏并且无标题栏，必须在OnCreate前面设置。
            this.Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);//只设置本页全屏

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            CarouselViewRenderer.Init();
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: false);
            Xamarin.Essentials.Platform.Init(Application);
            ZXing.Net.Mobile.Forms.Android.Platform.Init();

            LoadApplication(new App());
        }

        // Field, properties, and method for Video Picker
        public static MainActivity Current { private set; get; }

        public static readonly int PickImageId = 1000;

        public TaskCompletionSource<string> PickImageTaskCompletionSource { set; get; }

        protected override void OnActivityResult(int requestCode, Android.App.Result resultCode, Intent intent)
        {
            base.OnActivityResult(requestCode, resultCode, intent);

            if (requestCode == PickImageId)
            {
                if ((resultCode == Android.App.Result.Ok) && (intent != null))
                {
                    string path = null;
                    // Set the filename as the completion of the Task
                    Android.Net.Uri uri = intent.Data;
                    ICursor cursor = ContentResolver.Query(uri, null, null, null, null);
                    if (cursor == null)
                    {
                        return;
                    }
                    if (cursor.MoveToFirst())
                    {
                        try
                        {
                            path = cursor.GetString(cursor.GetColumnIndex(MediaStore.MediaColumns.Data));
                        }
                        catch (Exception)
                        {
                        }
                    }
                    cursor.Close();
                    PickImageTaskCompletionSource.SetResult(path);
                }
                else
                {
                    PickImageTaskCompletionSource.SetResult(null);
                }
            }
        }

    }
}