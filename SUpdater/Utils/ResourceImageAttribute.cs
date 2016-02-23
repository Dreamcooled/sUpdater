using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SUpdater.Model;

namespace SUpdater.Utils
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ResourceImageAttribute : Attribute
    {
        private readonly ImageSource _source;
        public ResourceImageAttribute(string uri)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("pack://application:,,,/sUpdater;component/"+uri);
            image.EndInit();
            _source = image;
        }

        public static ImageSource GetImageSource(Enum value)
        {
            Type enumType = value.GetType();

            foreach (FieldInfo f in enumType.GetFields())
            {
                if (!f.IsStatic)
                    continue;

                object[] attributes = f.GetCustomAttributes(typeof(ResourceImageAttribute), true);

                if (Object.Equals(value, f.GetValue(f)))
                {
                    if (attributes.Length > 0)
                    {
                        return ((ResourceImageAttribute)attributes[0])._source;
                    }
                    else
                    {
                        break;
                    }
                }
            }

           // throw new Exception("No ResourceImageAttribute found for this enumeration-value!");
            return null;
        }
        
    }
}
