using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GWP2
{
    public class Media
    {
        public string Title { get; private set; }
        public string Path { get; private set; }
        public Media(string title, string path)
        {
            this.Title = title;
            this.Path = path;
        }

        public override string ToString()
        {
            return this.Title;
        }

    }
}