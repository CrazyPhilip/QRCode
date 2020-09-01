using System;
using Android.App;
using Android.Content.PM;
using Android.Views;
using Android.OS;
using System.Threading.Tasks;
using Android.Content;
using CarouselView.FormsPlugin.Android;
using ZXing;
using Android.Graphics;
using ZXing.Common;
using System.IO;
using Path = System.IO.Path;
using Xamarin.Forms;
using Plugin.Toast;
using ToastLength = Plugin.Toast.Abstractions.ToastLength;
using Result = ZXing.Result;
using System.Collections.Generic;
using File = System.IO.File;

namespace QRCode.Droid
{
    [Activity(MainLauncher = false, Label = "QRCode", Icon = "@mipmap/icon", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
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
                        Width = 512,
                        Margin = 10
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
                    //switch (intent.Scheme)
                    //{
                    //    case "file":
                    //        PickImageTaskCompletionSource.SetResult(intent.Data.EncodedPath);
                    //        break;

                    //    case "content":
                    //        string path = null;
                    //        // Set the filename as the completion of the Task
                    //        Android.Net.Uri uri = intent.Data;
                    //        ICursor cursor = ContentResolver.Query(uri, null, null, null, null);
                    //        if (cursor == null)
                    //        {
                    //            return;
                    //        }
                    //        if (cursor.MoveToFirst())
                    //        {
                    //            try
                    //            {
                    //                path = cursor.GetString(cursor.GetColumnIndex(MediaStore.MediaColumns.Data));
                    //            }
                    //            catch (Exception)
                    //            {
                    //            }
                    //        }
                    //        cursor.Close();
                    //        PickImageTaskCompletionSource.SetResult(path);
                    //        break;

                    //    default:
                    //        PickImageTaskCompletionSource.SetResult(null);
                    //        break;
                    //}

                    Android.Net.Uri uri = intent.Data;
                    using (FileStream stream = new FileStream(intent.Data.EncodedPath, FileMode.Open))
                    {
                        //Bitmap bitmap = BitmapFactory.DecodeStream(stream);

                        //int h = bitmap.Height;
                        //int w = bitmap.Width;

                        byte[] bytes = new byte[stream.Length];
                        stream.Read(bytes, 0, bytes.Length);

                        RGBLuminanceSource rGBLuminanceSource = new RGBLuminanceSource(bytes, 512, 512, RGBLuminanceSource.BitmapFormat.RGB24);
                        //BinaryBitmap binaryBitmap = new BinaryBitmap(new HybridBinarizer(rGBLuminanceSource));
                        ////MultiFormatReader reader = new MultiFormatReader();
                        //
                        //IDictionary<DecodeHintType, object> hints = new Dictionary<DecodeHintType, object>
                        //{
                        //    { DecodeHintType.CHARACTER_SET, "utf-8" }, // 设置二维码内容的编码
                        //    {DecodeHintType.TRY_HARDER, Boolean.True },
                        //    { DecodeHintType.POSSIBLE_FORMATS, BarcodeFormat.QR_CODE}
                        //};
                        //QRCodeReader reader = new QRCodeReader();
                        //Result result = reader.decode(binaryBitmap, hints);

                        BarcodeReader reader = new BarcodeReader()
                        {
                            Options = new DecodingOptions
                            {
                                TryHarder = true,
                                CharacterSet = "utf-8",
                                PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE }
                            }
                        };
                        reader.AutoRotate = true;
                        Result result = reader.Decode(rGBLuminanceSource);

                        //bitmap.Dispose();
                        if (!string.IsNullOrWhiteSpace(result?.Text))
                        {
                            PickImageTaskCompletionSource.SetResult(result.Text);
                        }
                    }
                }
                else
                {
                    PickImageTaskCompletionSource.SetResult(null);
                }
            }
        }

        //   /**
        //* 扫描二维码图片的方法,返回结果
        //*
        //* @param path
        //* @return 返回结果
        //*/
        //   public Result scanningImage(string path)
        //   {
        //       if (string.IsNullOrWhiteSpace(path))
        //       {
        //           return null;
        //       }
        //       Dictionary<DecodeHintType, string> hints = new Dictionary<DecodeHintType, string>();
        //       hints.Add(DecodeHintType.TRY_HARDER, "UTF8"); // 设置二维码内容的编码
        //       BitmapFactory.Options options = new BitmapFactory.Options();
        //       options.InJustDecodeBounds = true; // 先获取原大小
        //       Bitmap scanBitmap = BitmapFactory.DecodeFile(path, options);
        //       options.InJustDecodeBounds = false; // 获取新的大小
        //       int sampleSize = (int)(options.OutHeight / (float)100);
        //       if (sampleSize <= 0)
        //           sampleSize = 1;
        //       options.InSampleSize = sampleSize;

        //       //获取到bitmap对象(相册图片对象通过path)
        //       scanBitmap = BitmapFactory.DecodeFile(path, options);
        //       //输入bitmap解析的二值化结果(就是图片的二进制形式)
        //       RGBLuminanceSource source = new RGBLuminanceSource(scanBitmap);
        //       //再把图片的二进制形式转换成,图片bitmap对象
        //       BinaryBitmap bitmap1 = new BinaryBitmap(new HybridBinarizer(source));
        //       //CodaBarReader codaBarReader= new CodaBarReader();    //codaBarReader  二维码
        //       try
        //       {
        //           /**创建MultiFormatReader对象,调用decode()获取我们想要的信息,比如条形码的code,二维码的数据等等.这里的MultiFormatReader可以理解为就是一个读取获取数据的类,最核心的就是decode()方法 */
        //           return new MultiFormatReader().decode(bitmap1, (IDictionary<DecodeHintType, object>)hints);      //识别条形码

        //       }
        //       catch (Exception e)
        //       {
        //           e.printStackTrace();
        //       }
        //       return null;
        //   }

    }
}