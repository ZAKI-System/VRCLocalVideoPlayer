using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCLocalVideoPlayer
{
    public class ExtensionModel
    {
        public string StoreId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string OptionsUri { get; set; }
        public string PopupUri { get; set; }

        public override string ToString()
        {
            return $"{this.Name} ({this.Id} / store:{this.StoreId})";
        }
    }
}
