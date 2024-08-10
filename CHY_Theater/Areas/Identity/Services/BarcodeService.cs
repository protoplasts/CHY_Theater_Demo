using ZXing;
using ZXing.Common;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace CHY_Theater.Areas.Identity.Services
{
	public class BarcodeService
	{
		public byte[] GenerateUserIdBarcode(string userId)
		{
			var writer = new BarcodeWriterPixelData
			{
				Format = BarcodeFormat.CODE_128,
				Options = new EncodingOptions
				{
					Width = 300,
					Height = 100,
					Margin = 10
				}
			};

			var pixelData = writer.Write(userId);

			using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
			{
				using (var ms = new MemoryStream())
				{
					var bitmapData = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
					try
					{
						System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
					}
					finally
					{
						bitmap.UnlockBits(bitmapData);
					}
					bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
					return ms.ToArray();
				}
			}
		}
	}
}
