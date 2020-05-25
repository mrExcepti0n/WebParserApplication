using System.Collections.Generic;
using System.Threading.Tasks;
using WebParserTools.Model;

namespace WebParserTools.Abstractions
{
    public abstract class WebParser<T> where T: class
    {
        public abstract Task<List<T>> Parse();
    }
}
