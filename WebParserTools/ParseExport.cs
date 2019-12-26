using System;
using System.Collections.Generic;
using System.Text;

namespace WebParserTools
{
    public abstract class ParseExport : IParseExport
    {
        public void OnParseEnd(object sender, List<PostInformation> postInformations)
        {
            SaveToSource(postInformations);
        }

        public abstract void SaveToSource(List<PostInformation> postInformation);
       
    }
}
