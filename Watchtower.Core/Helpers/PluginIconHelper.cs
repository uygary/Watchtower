using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace Watchtower.Core
{
    /// <summary>
    /// Contains static helper methods related to image handling.
    /// </summary>
    public class PluginIconHelper
    {
        /// <summary>
        /// Generates the plugin icon to be used in Watchtower.
        /// </summary>
        /// <param name="assemblyId">Assembly name.</param>
        /// <param name="imageResourcePath">Path to the image resource in project.</param>
        /// <returns></returns>
        public static BitmapImage GetPluginIcon(string assemblyId, string imageResourcePath)
        {
            string uriString = string.Format("pack://application:,,,/{0};component/{1}", assemblyId, imageResourcePath);
            Uri uri = new Uri(uriString);

            StreamResourceInfo imageResourceInfo = Application.GetResourceStream(uri);
            Stream imageStream = imageResourceInfo.Stream;

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = imageStream;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();

            return image;
        }
    }
}
