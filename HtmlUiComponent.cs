﻿using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace PluginTwo
{
    public class HtmlUiComponent : GH_Component
    {
        public bool Initialized;

        // ModelessForm instance
        private WebWindow _webWindow;

        // Separate thread to run Ui on
        private Thread _uiThread;

        private string _oldPath;

        /// <summary>
        /// Launch a UI Window from a HTML file.
        /// </summary>
        public HtmlUiComponent()
            : base("Launch HTML UI", "HTML UI",
                "Launch a UI Window from a HTML file.",
                "UI", "Main")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("HTML Path", "path", "Where to look for the HTML interface.",
                GH_ParamAccess.item);
            pManager.AddBooleanParameter("Show Window", "show", "Toggle for showing/hiding the interface window.",
                GH_ParamAccess.item, false);
            pManager.AddTextParameter("Title", "title", "The title name for the UI window.",
                GH_ParamAccess.item, "UI");
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Input Values", "vals", "Value of HTML Inputs", GH_ParamAccess.list);
            pManager.AddTextParameter("Input Ids", "ids", "Ids of HTML Inputs", GH_ParamAccess.list);
            pManager.AddTextParameter("Input Names", "names", "Names of HTML Inputs", GH_ParamAccess.list);
            pManager.AddTextParameter("Input Types", "types", "Types of HTML Inputs", GH_ParamAccess.list);
            pManager.AddGenericParameter("Web Window", "web", "Web Window Instance", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess da)
        {
            // get input from gh component inputs
            string path = null;
            bool show = false;
            string title = null;

            // get input
            if (!da.GetData(0, ref path)) return;
            if (!da.GetData<bool>(1, ref show)) return;
            da.GetData(2, ref title);


            if (!show) return;

            if (Initialized)
            {
                // if there's a new path, navigate to it
                if (_oldPath != path)
                {
                    _webWindow.Navigate(path);
                    _oldPath = path;
                }

                da.SetDataList(0, _webWindow.InputValues);
                da.SetDataList(1, _webWindow.InputIds);
                da.SetDataList(2, _webWindow.InputNames);
                da.SetDataList(3, _webWindow.InputTypes);
                da.SetData(4, _webWindow);
            }
            else
            {
                LaunchWindow(path, title);
                Initialized = true;
                _oldPath = path;
            }

            GH_Document doc = OnPingDocument();
            doc?.ScheduleSolution(500, document => ExpireSolution(false));
        }

        private void LaunchWindow(string path, string title = "UI")
        {
            if (!(_uiThread is null) && _uiThread.IsAlive) return;
            _uiThread = new Thread(() =>
            {
                SynchronizationContext.SetSynchronizationContext(
                    new DispatcherSynchronizationContext(
                        Dispatcher.CurrentDispatcher));
                // The dialog becomes the owner responsible for disposing the objects given to it.
                _webWindow = new WebWindow(path);
                _webWindow.Closed += _webWindow_Closed;
                _webWindow.Show();
                _webWindow.Title = title;
                Dispatcher.Run();
            });

            _uiThread.SetApartmentState(ApartmentState.STA);
            _uiThread.IsBackground = true;
            _uiThread.Start();
        }

        private void _webWindow_Closed(object sender, EventArgs e)
        {
            Initialized = false;
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.web_window;

        public override Guid ComponentGuid => new Guid("1fd8ca75-b4ff-4b1e-9dd0-ac00ac4b6559");
    }
}
