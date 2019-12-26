using System;
using System.Collections.Generic;
using System.Text;

namespace WebParserTools
{
    public interface IParseExport
    {
        void OnParseEnd(Object sender, List<PostInformation> postInformations);


        void SaveToSource(List<PostInformation> postInformation);

    }
}
