using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using QRCode.Droid;
using QRCode.Util;
using Xamarin.Forms;

[assembly: Dependency(typeof(ImagePickerService))]
namespace QRCode.Droid
{
    public class ImagePickerService : IImagePickerService
    {
        public Task<string> PickImageAsync()
        {
            // Define the Intent for getting images
            //Intent intent = new Intent();
            //intent.SetType("image/*");
            //intent.SetAction(Intent.ActionGetContent);

            Intent intent = new Intent(Intent.ActionPick);
            intent.SetType("image/*");
            //intent.PutExtra(Intent.ExtraAllowMultiple, false);

            // Start the picture-picker activity (resumes in MainActivity.cs)
            MainActivity.Current.StartActivityForResult(
                Intent.CreateChooser(intent, "Select Photo"),
                MainActivity.PickImageId);

            // Save the TaskCompletionSource object as a MainActivity property
            MainActivity.Current.PickImageTaskCompletionSource = new TaskCompletionSource<string>();

            // Return Task object
            return MainActivity.Current.PickImageTaskCompletionSource.Task;
        }

    }
}