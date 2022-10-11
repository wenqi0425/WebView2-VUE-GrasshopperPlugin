using Grasshopper.Kernel;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginTwo
{
    public class GHUInfo : GH_AssemblyInfo
    {
        public override string Name => "GHUI";

        public override Bitmap Icon =>
            //Return a 24x24 pixel bitmap to represent this GHA library.
            null;

        public override string Description =>
            //Return a short string describing the purpose of this GHA library.
            "Web-based user interface building.";

        public override Guid Id => new Guid("a875ab72-8a03-4c2b-9e6c-8793b8513a5d");

        public override string AuthorName =>
            //Return a string identifying you or your company.
            "Wenqi@Stykka";

        public override string AuthorContact =>
            //Return a string representing your preferred contact details.
            "wenqi100@gmail.com | @wenqi0425 on GitHub";
    }
}
