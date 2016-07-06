using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawablesGenerator
{
    public class DrawableException : Exception
    {
        public DrawableException()
        {
        }

        public DrawableException(string message) : base(message)
        {
        }
    }
}
