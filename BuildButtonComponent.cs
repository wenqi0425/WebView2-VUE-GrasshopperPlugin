using Grasshopper.Kernel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginTwo.Components
{
    public class BuildButtonComponent : GH_Component
    {
        public BuildButtonComponent()
            : base("Create Button", "Button",
                "Create a HTML Button Input.",
                "UI", "Create")
        {
        }        

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "name", "The name of the button input component.", GH_ParamAccess.item,
                "button");
            pManager.AddTextParameter("ID", "id", "The id of the button input component.", GH_ParamAccess.item,
                "button");
            pManager.AddTextParameter("Value", "val", "The starting value of the button input component.",
                GH_ParamAccess.item, "Button");
            pManager.AddTextParameter("CSS", "css", "The `style` attribute to apply to the element and its children.",
                GH_ParamAccess.item,
                "");
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("HTML", "html", "The HTML code for the created button input.",
                GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess access)
        {
            // get input from gh component inputs
            string name = null;
            string id = null;
            string value = null;
            string cssStyle = null;

            access.GetData(0, ref name);
            access.GetData(1, ref id);
            access.GetData(2, ref value);
            access.GetData(3, ref cssStyle);

            // create a valid HTML string from the inputs for our button
            string buttonString =
                $"<input type='button' id='{id}' name='{name}' value='{value}' style='{cssStyle}'>";

            access.SetData(0, buttonString);
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.button;
        // public override Guid ComponentGuid => throw new NotImplementedException();
        public override Guid ComponentGuid => new Guid("2abba5cd-7c27-4443-8fca-df978295efbd");
    }
}
