using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tagpic
{
    public class ImageWithTags
    {
        public Image Image { get; set; }
        public List<string> Tags { get; set; }
        public string fileName { get; set; }

        public ImageWithTags(Image image, string fileName)
        {
            this.Image = image;
            string name = Path.GetFileNameWithoutExtension(fileName);
            this.Tags = name.Split('_').Skip(2).ToList();
            this.fileName = fileName;
        }
    }
}
