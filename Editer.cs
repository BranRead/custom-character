using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Diagnostics;

using System.Reflection;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Assignment9
{
    internal class Editer
    {
        public void Previous_Image(Image image, Image imageResult, string part, ref int index, int numOfPhotos)
        {
            index--;
            if (index < 1)
            {
                index = numOfPhotos;
            }

            image.Source = new BitmapImage(new Uri("/images/" + part + index + ".png", UriKind.Relative));
            imageResult.Source = new BitmapImage(new Uri("/images/" + part + index + ".png", UriKind.Relative));

        }

        public void Next_Image(Image image, Image imageResult, string part, ref int index, int numOfPhotos)
        {
            index++;
            if (index > numOfPhotos)
            {
                index = 1;
            }

            image.Source = new BitmapImage(new Uri("/images/" + part + index + ".png", UriKind.Relative));
            imageResult.Source = new BitmapImage(new Uri("/images/" + part + index + ".png", UriKind.Relative));
        }

        public void Random_Image(Image image, Image imageResult, string part, ref int index, int numOfPhotos)
        {
            Random rng = new Random();
            index = rng.Next(1, numOfPhotos);
            image.Source = new BitmapImage(new Uri("/images/" + part + index + ".png", UriKind.Relative));
            imageResult.Source = new BitmapImage(new Uri("/images/" + part + index + ".png", UriKind.Relative));
        }

        public void Set_Base_Face(int option, Image image) 
        {
            image.Source = new BitmapImage(new Uri("/images/base_face_" + option + ".png", UriKind.Relative));
        }

        public void Update_Face(string part, Image image, int index) 
        {
            image.Source = new BitmapImage(new Uri("/images/" + part + index + ".png", UriKind.Relative));
        }
    }
}
