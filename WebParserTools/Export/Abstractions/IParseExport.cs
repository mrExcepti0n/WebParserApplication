using System;
using System.Collections.Generic;
using WebParserTools.Model;

namespace WebParserTools.Export.Abstractions
{
    public interface IParseExport
    {
        void OnParseComplete(Object sender, ParseCompletedEventArgs eventArgs);


        void SaveToSource(List<PostInformation> postInformation);

    }
}
