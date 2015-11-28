using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvc4gw7.Models
{
    public class AppColors
    {
        private List <int> _value = new List <int> {
            0xce1e45,
            0xde5810,
            0x487222,
            0x22726d,
            0xe3f170,
            0x49182f,
            0xba4444,
            0x0fa29d,
            0x4c3875,
            0x553d34,
            0xffda2c,
            0x0bc30b,
            0x000f4f,
            0x960000,
            0x80876e,
            0xab578a,
            0xba6411,
            0x084bc5,
            0x56cb7c,
            0xd5b357,
            0x3b0202,
            0x006005,
            0xff973b,
            0xf20909,
            0x618aff,
            0xffdc7e,
            0x9eca97
            };

        public List<int> value
        {
            get
            {
                return _value;
            }
        }
    }


    public enum MediaType : int
    {
        Photo,
        Graphic,
        Video
    }

    public class MediaElement
    {
        public int Id { get; set; }
        [ForeignKey("MediaCollection")]
        public int CollectionId { get; set; }
        public string File { get; set; }

        public virtual MediaCollection MediaCollection { get; set; }
    }

    public class MediaCollection
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        [Display(Name = "Краткое название")]
        public string ShortTitle { get; set; }
        [Display(Name = "Полное название")]
        public string Title { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }
        public string Location { get; set; }
        public MediaType MediaType { get; set; }

        public virtual ICollection<MediaElement> MediaElements { get; set; }
    }

    public class VideoMediaElement
    {
        public string ImageFile { get; set; }
        public string VideoFile { get; set; }
    }

}