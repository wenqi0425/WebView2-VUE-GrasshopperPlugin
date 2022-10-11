using Grasshopper.Kernel.Types;
using Grasshopper.Kernel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginTwo
{
    public class SettersDefineComponent : GH_Component
    {
        // Component for building a HTML label input component.
        public SettersDefineComponent()
            : base("Define Set Values", "Define Set",
                "Define Values to Set in the UI.",
                "UI", "Set")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("IDs", "ids", "The ids of the label components.", GH_ParamAccess.list);
            pManager.AddTextParameter("Values", "vals", "The values of the label components.", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Setters", "set", "The commands for setting values in the web UI.",
                GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess da)
        {
            // get input from gh component inputs
            List<string> ids = new List<string>();
            List<string> vals = new List<string>();

            // don't compute if values and ids are not provided
            if (!da.GetDataList(0, ids)) return;
            if (!da.GetDataList(1, vals)) return;

            //List<DomSetValueModelGoo> setValueModels =
            //    ids.Select((t, i) => new DomSetValueModelGoo() {id = t, value = vals[i]}).ToList();

            // create a dictionary of ids and values to set
            // TODO: there's definitely a more elegant way than this stupid loop...
            Dictionary<string, string> setValueModels = new Dictionary<string, string>();
            for (int i = 0; i < ids.Count; i++)
            {
                setValueModels.Add(ids[i], vals[i]);
            }

            IGH_Goo dictionaryGoo = new GH_ObjectWrapper(setValueModels);

            da.SetData(0, dictionaryGoo);
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.define_setters;

        public override Guid ComponentGuid => new Guid("5e31613c-8036-4382-adeb-068caa1eff08");
    }
}
