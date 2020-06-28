using Idoneus.Converters.Base;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Idoneus.Converters
{
    public class ImageConverter : BaseValueConverter<ImageConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Icon ico = (Icon)value;
            using Bitmap bmp = ico.ToBitmap();
            var stream = new MemoryStream();
            bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            ImageSource imageSource = BitmapFrame.Create(stream);
            return imageSource;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
