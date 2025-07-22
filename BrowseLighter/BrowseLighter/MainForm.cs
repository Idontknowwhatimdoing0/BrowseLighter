/*
 * Created by SharpDevelop.
 * User: mateu
 * Date: 21.07.2025
 * Time: 17:00
 * 
 * Modified to enable IE11 mode and modern fonts for BrowseLighter
 * Modified to force Firefox User-Agent on navigation
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;

namespace BrowseLighter
{
    public partial class MainForm : Form
    {
        // Controls
        private WebBrowser webBrowser1;
        private TextBox textBoxUrl;
        private Button buttonGo;

        // Custom Firefox user agent string
        private const string customUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:115.0) Gecko/20100101 Firefox/115.0";

        public MainForm()
        {
            // Enable IE11 mode for the embedded browser before UI initialization
            SetBrowserFeatureControl();

            InitializeComponent();

            // Create controls manually (if not using designer)
            SetupControls();

            // Load custom new tab page at start
            webBrowser1.DocumentText = GetNewTabPageHtml();
        }

        private void SetupControls()
        {
            // Set form properties
            this.Text = "BrowseLighter";
            this.Width = 800;
            this.Height = 600;

            // WebBrowser control
            webBrowser1 = new WebBrowser();
            webBrowser1.Dock = DockStyle.Fill;
            this.Controls.Add(webBrowser1);

            // Panel at bottom for URL/search input and Go button
            Panel panelBottom = new Panel();
            panelBottom.Height = 30;
            panelBottom.Dock = DockStyle.Bottom;
            this.Controls.Add(panelBottom);

            // TextBox for URL/search
            textBoxUrl = new TextBox();
            textBoxUrl.Width = 700;
            textBoxUrl.Left = 5;
            textBoxUrl.Top = 3;
            textBoxUrl.Font = new Font("Segoe UI", 9f);
            panelBottom.Controls.Add(textBoxUrl);

            // Go button
            buttonGo = new Button();
            buttonGo.Text = "Go";
            buttonGo.Width = 50;
            buttonGo.Left = textBoxUrl.Right + 5;
            buttonGo.Top = 1;
            buttonGo.Font = new Font("Segoe UI", 9f);
            buttonGo.Click += ButtonGo_Click;
            buttonGo.FlatStyle = FlatStyle.System; // native modern style
            panelBottom.Controls.Add(buttonGo);

            // Pressing Enter triggers Go
            textBoxUrl.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    ButtonGo_Click(this, EventArgs.Empty);
                }
            };
        }

        private void ButtonGo_Click(object sender, EventArgs e)
        {
            string input = textBoxUrl.Text.Trim();

            if (string.IsNullOrEmpty(input))
                return;

            string urlToNavigate;

            // Simple check if input looks like URL
            if (Uri.IsWellFormedUriString(input, UriKind.Absolute))
            {
                if (!input.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                    !input.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    input = "http://" + input;
                }
                urlToNavigate = input;
            }
            else
            {
                urlToNavigate = "https://www.google.com/search?q=" + Uri.EscapeDataString(input);
            }

            // Add custom User-Agent header
            string headers = "User-Agent: " + customUserAgent + "\r\n";

            webBrowser1.Navigate(urlToNavigate, null, null, headers);
        }

        private string GetNewTabPageHtml()
        {
            return @"
                <html>
                <head>
                    <title>BrowseLighter - New Tab</title>
                    <style>
                        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; text-align: center; padding-top: 150px; background-color: #f0f0f0; }
                        h1 { color: #333; }
                        p { color: #666; font-size: 18px; }
                    </style>
                </head>
                <body>
                    <h1>Welcome to BrowseLighter!</h1>
                    <p>Type a URL or search term below and click Go.</p>
                </body>
                </html>";
        }

        private void SetBrowserFeatureControl()
        {
            try
            {
                string appName = System.IO.Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(
                    @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", RegistryKeyPermissionCheck.ReadWriteSubTree))
                {
                    key.SetValue(appName, 11001, RegistryValueKind.DWord); // IE11 edge mode
                }
            }
            catch
            {
                // ignore errors
            }
        }
    }
}
