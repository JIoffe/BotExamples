using System;
using System.Collections.Generic;

namespace BIADTemplate.Model
{
    [Serializable]
    public class RichQnaResponse
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public IList<RichQnaButton> Buttons { get; set; }
    }
}