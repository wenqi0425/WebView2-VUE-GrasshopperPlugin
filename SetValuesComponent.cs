using Grasshopper.Kernel.Types;
using Grasshopper.Kernel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginTwo
{
    public class SetValuesComponent : GH_Component
    {
        /// <summary>
        /// Set Values in the UI.
        /// </summary>
        public SetValuesComponent()
            : base("Set Values", "Set",
                "Set Values in the UI.",
                "UI", "Set")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Web Window", "web", "Web Window Instance",
                GH_ParamAccess.item);
            pManager.AddGenericParameter("Setters", "set", "The commands for setting values in the web UI.",
                GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            //pManager.AddTextParameter("Input Values", "vals", "Value of HTML Inputs", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess da)
        {
            // get input from gh component inputs
            GH_ObjectWrapper settersGoo = null;
            GH_ObjectWrapper webWindowGoo = null;
            Dictionary<string, string> settersDictionary = new Dictionary<string, string>();

            // get setters
            if (!da.GetData(0, ref webWindowGoo)) return;
            if (da.GetData(1, ref settersGoo))
            {
                settersDictionary = settersGoo.Value as Dictionary<string, string>;
            }
            else
            {
                return;
            }

            WebWindow webWindow = (WebWindow)webWindowGoo.Value;
            webWindow.HandleSetters(settersDictionary);
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.define_setters;

        public override Guid ComponentGuid => new Guid("784b0509-70d6-4b0a-85f9-c89c9703c163");
    }
}
