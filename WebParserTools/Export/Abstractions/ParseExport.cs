using System.Collections.Generic;
using WebParserTools.Model;

namespace WebParserTools.Export.Abstractions
{
    public abstract class ParseExport : IParseExport
    {
        public void OnParseComplete(object sender, ParseCompletedEventArgs eventArgs)
        {
            SaveToSource(eventArgs.PostInformations);
        }

        public abstract void SaveToSource(List<PostInformation> postInformation);
       
    }
}
