using PluginTwo.Classes;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace PluginTwo
{
    /// <summary>
    /// Interaction logic for WebWindow.xaml
    /// </summary>
    public partial class WebWindow : Window
    {
        private WebView2Wrapper _webView2WrapperInstance;

        // List of the current values of all the input elements in the DOM.
        public List<string> InputValues => _webView2WrapperInstance.InputValues;

        // List of input elements in the DOM.
        public List<string> InputIds => _webView2WrapperInstance.InputIds;
        public List<string> InputNames => _webView2WrapperInstance.InputNames;
        public List<string> InputTypes => _webView2WrapperInstance.InputTypes;

        public WebWindow()
        {

        }

        // The WPF Container for the WebBrowser element which renders the user's HTML.
        public WebWindow(string htmlPath)
        {
            InitializeComponent();
            _webView2WrapperInstance = new WebView2Wrapper(htmlPath, Dispatcher);
            _webView2WrapperInstance.InitializeWebView(Docker);
            _webView2WrapperInstance.SubscribeToHtmlChanged();
        }

        public void Navigate(string newPath)
        {
            _webView2WrapperInstance.Navigate(newPath);
        }

        public void HandleSetters(Dictionary<string, string> setters)
        {
            _webView2WrapperInstance.HandleValueSetters(setters);
        }
    }
}
