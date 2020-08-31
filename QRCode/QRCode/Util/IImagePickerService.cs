using System.Collections.Generic;
using System.Threading.Tasks;

namespace QRCode.Util
{
    public interface IImagePickerService
    {
        Task<string> PickImageAsync();
    }
}
