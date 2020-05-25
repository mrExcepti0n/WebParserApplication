using System;
using System.Collections.Generic;
using System.Text;

namespace WebParserTools.Model
{
    public class ParseCompletedEventArgs : EventArgs
    {
        public ParseCompletedEventArgs(List<PostInformation> postInformations)
        {
            PostInformations = postInformations;
        }
        public List<PostInformation> PostInformations { get; set; }
    }
}
