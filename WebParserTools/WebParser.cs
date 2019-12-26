using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebParserTools
{
    public abstract class WebParser<T> where T: class
    {
        public abstract Task<List<PostInformation>> Parse();
    }
}
