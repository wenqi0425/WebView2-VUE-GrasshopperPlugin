using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using PluginTwo.Classes.Models;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Core.DevToolsProtocolExtension;
using Newtonsoft.Json;
using Microsoft.Web.WebView2.Wpf;

namespace PluginTwo.Classes
{
    public class WebView2Wrapper
    {
        #region Private Attributes

        // The path of the HTML file which is being served as the user interface.
        private string _htmlPath;

        // The dispatcher handling the execution of the WPF host Window.
        private readonly Dispatcher _dispatcher;

        // The directory where the HTML file used for the UI lives.
        private string _directory 
            => _dispatcher.Invoke(() => Path.GetDirectoryName(_htmlPath));

        // A special "temp" folder where WebView2Wrapper does the execution.
        // This should be created in the Grasshopper/Libraries directory.
        private string _executingLocation 
            => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\temp";

        // The WebView2Wrapper instance which is being executed in this component.
        private Microsoft.Web.WebView2.Wpf.WebView2 _webView;

        // Class for watching file changes in the source HTML file.
        // Allows for reload triggering when the file is updated.
        private FileSystemWatcher _watcher;

        // A collection of DomInputModel classes - representing the relevant data from the HTML `input` elements. 
        private List<DomInputModel> _domInputModels;

        // Developer-focused tooling that exposes some more DOM-specific events and utilities.
        private DevToolsProtocolHelper _cdpHelper;

        // input parameter of WebView2Wrapper
        // A dictionary of the ids of the HTML elements to set the values for ids.
        private Dictionary<string, string> _oldSetters;

        #endregion

        #region List of input elements

        public List<string> InputIds
            => _domInputModels?.Select(s => s.id).ToList();

        public List<string> InputValues
            => _domInputModels?.Select(s => s.value).ToList();

        public List<string> InputNames 
            => _domInputModels?.Select(s => s.name).ToList();

        public List<string> InputTypes 
            => _domInputModels?.Select(s => s.type).ToList();

        #endregion


        #region Constructors
        public WebView2Wrapper()
        {
        }

        public WebView2Wrapper(string htmlPath, Dispatcher dispatcher)
        {
            _htmlPath = htmlPath;
            _dispatcher = dispatcher;
            _oldSetters = new Dictionary<string, string>();
        }
        #endregion

        #region Public Methods

        public void HandleValueSetters(Dictionary<string,string> newSetters)
        {
            // if the values haven't changed, do nothing
            bool same = Comparison.CompareDictionaries(_oldSetters, newSetters);
            if (same) return;

            // if the value has changed, execute the setter script
            _oldSetters = newSetters;
            _dispatcher.BeginInvoke(new Action(() =>
{
                foreach (KeyValuePair<string, string> s in newSetters)
                {
                    _webView.ExecuteScriptAsync($"setValues('{s.Key}','{s.Value}');");
                }
            }));
        }

        /// Initialize the DevTools helper class.
        public void InitializeDevToolsProtocolHelper()
        {
            if (_webView == null || _webView.CoreWebView2 == null)
            {
                throw new Exception("Initialize WebView before using DevToolsProtocolHelper.");
            }

            if (_cdpHelper == null)
            {
                _cdpHelper = _webView.CoreWebView2.GetDevToolsProtocolHelper();
            }
        }

        public async void InitializeWebView(DockPanel docker)
        {
            _webView = new WebView2();

            // clear everything in the WPF dock panel container
            docker.Children.Clear();
            docker.Children.Add(_webView);

            // initialize the webview2 instance
            try
            {
                _webView.CoreWebView2InitializationCompleted += OnWebViewInitializationCompleted;
                CoreWebView2Environment env = await CoreWebView2Environment.CreateAsync(null, _executingLocation);
                await _webView.EnsureCoreWebView2Async(env);

                _webView.WebMessageReceived += OnWebViewInteraction;
                
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        // Navigate to a new HTML file path.
        // The file path of the new HTML file to load.
        public void Navigate(string newPath)
        {
            //if (_htmlPath == newPath) return;
            _dispatcher.BeginInvoke(new Action(() =>
            {
                _htmlPath = newPath;
                _webView.Source = new Uri(_htmlPath);
            }));
        }
        #endregion

        #region Private Methods

        // What to do when WebView is initialized: 
        // Navigate to the source, and add JS scripts/functions which need to be defined at startup.
        private void OnWebViewInitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (_webView?.CoreWebView2 == null) return;
            _webView.Source = new Uri(_htmlPath);
            _webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(
            Properties.Resources.AddDocumentClickListener);
            _webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(
                Properties.Resources.QueryInputElementsInDOM);
            _webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(
                Properties.Resources.SetValuesInDom);
        }

        // Run the DOM Query script (JS) to get all the input elements.
        private async Task RunDomInputQuery()
        {
            // get the results of the DOM 'input' element query script, and abort if none found
            string scriptResult = await _webView.ExecuteScriptAsync("queryInputElements();");
            dynamic deserializedDomModels = JsonConvert.DeserializeObject(scriptResult);
            if (deserializedDomModels == null) return;

            _domInputModels = new List<DomInputModel>();
            foreach (dynamic s in deserializedDomModels)
            {
                DomInputModel domInputModel = JsonConvert.DeserializeObject<DomInputModel>(s.ToString());
                _domInputModels.Add(domInputModel);
            }
        }

        private async void RunDomInputQuery(DomClickModel clickModel)
        {
            await RunDomInputQuery();
            // handle output for buttons
            if (clickModel.targetType == "button")
            {
                HandleButtonClick(clickModel);
            }
        }
        
        // What to do when the listener script returns a value.
        private void OnWebViewInteraction(object sender, CoreWebView2WebMessageReceivedEventArgs msg)
        {
            DomClickModel clickData = JsonConvert.DeserializeObject<DomClickModel>(msg.WebMessageAsJson);
            _dispatcher.BeginInvoke(new Action(() =>
            {
                RunDomInputQuery(clickData);
            }));
        }

        private void HandleButtonClick(DomClickModel clickModel)
        {
            //if (clickModel.targetType != "button") return;
            // TODO: need to ensure that there is a unique id for each button, even when users
            // are not using the id/name feature correctly. for now we loop over all the possible buttons
            var clickedButtons = _domInputModels.Where(m => m.type == clickModel.targetType &&
                                                            m.id == clickModel.targetId ||
                                                            m.name == clickModel.targetName);
            //if (clickedButtons == null) return;
            foreach (DomInputModel domInput in clickedButtons)
            {
                domInput.value = "true";
            }
        }

        // Subscribe to the DocumentUpdated event of WebView2Wrapper.
        public async void SubscribeToDocumentUpdated()
        {
            await _cdpHelper.DOM.EnableAsync();
            _cdpHelper.DOM.DocumentUpdated += OnDocumentUpdated;
        }

        // What to do when the Document is updated.
        // Currently somewhat redundant functionality with the FileWatcher doing the same essentially.
        private void OnDocumentUpdated(object sender, DOM.DocumentUpdatedEventArgs args)
        {
        }

        // Initialize a file-watcher object on the HTML file being used.
        public void SubscribeToHtmlChanged()
        {
            _watcher = new FileSystemWatcher(_directory)
            {
                NotifyFilter = NotifyFilters.LastAccess
                               | NotifyFilters.LastWrite
                               | NotifyFilters.FileName
                               | NotifyFilters.CreationTime
                               | NotifyFilters.Size
                               | NotifyFilters.DirectoryName
                               | NotifyFilters.Attributes
                               | NotifyFilters.Security,
                //Filter = "*.html"
            };
            _watcher.Changed += OnHtmlChanged;
            _watcher.EnableRaisingEvents = true;
            _watcher.IncludeSubdirectories = true;
        }

        // Method handler for when a change is detected in the HTML file.
        // This method will trigger a reload on the HTML file.
        private void OnHtmlChanged(object source, FileSystemEventArgs e)
        {
            _dispatcher.BeginInvoke(new Action(() =>
            {
                //RunDomInputQuery();
                _webView.Reload();
            }));
        }
        #endregion
    }
}
