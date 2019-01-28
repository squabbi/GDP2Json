using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.IO;

using Newtonsoft.Json;

using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace FactoryImage2Json
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private Task<List<FactoryImage>> factoryImages;

        private async void LoadTableBtn_Click(object sender, RoutedEventArgs e)
        {
            // Split text by commas
            string[] tableNos = tableNoTb.Text.Split(',');
            if (tableNos.Length > 1)
            {
                // More than one table
                int[] tables = Array.ConvertAll(tableNos, s => int.Parse(s));
                factoryImages = LoadFactoryImages(tables);
                // Load into Data Grid
                factoryImagesDg.ItemsSource = await factoryImages;
            }
            else
            {
                if (int.TryParse(tableNoTb.Text, out int tableNo))
                {
                    factoryImages = LoadFactoryImages(tableNo);
                    // Load into Data Grid
                    factoryImagesDg.ItemsSource = await factoryImages;
                }
                else
                {
                    MessageBox.Show("Please enter a valid number...");
                    tableNoTb.Clear();
                }
            }
            
        }

        private async Task<List<FactoryImage>> LoadFactoryImages(int tableNo)
        {
            List<FactoryImage> factoryImages = new List<FactoryImage>();

            // AngleSharp, rip table contents
            var config = Configuration.Default.WithDefaultLoader();
            var source = "https://developers.google.com/android/images";
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(source);

            // Locate all tables in the site (containing the factory images)
            var tables = document.QuerySelectorAll("table");
            // Locate the desired table
            var table = (IHtmlTableElement)tables[tableNo];
            var tableContents = table.Rows;
            // Go through each row after the first one (title row)
            for (int i = 1; i < tableContents.Length; i++)
            {
                var rowChildren = tableContents[i].Children;
                // Child 1 - Version, child 2 - Link (href), child 3 - Checksum
                string version = rowChildren[0].TextContent;
                string link = ((IHtmlAnchorElement)rowChildren[1].FirstChild).Href.ToString();
                string checksum = rowChildren[2].TextContent;

                // Split up version and link by '/' and '-' to get individual elements
                string[] versionSplit = version.Split(' ');
                string[] linkSplit = link.Split(new char[] { '/', '-' });

                // Create the Factory Image and add it to the list of factory images
                factoryImages.Add(new FactoryImage(linkSplit[6], versionSplit[0], linkSplit[7].ToUpper(), linkSplit[9].Substring(0,8), version, link, checksum));
            }

            return factoryImages;
        }

        private async Task<List<FactoryImage>> LoadFactoryImages(int[] tableNos)
        {
            List<FactoryImage> factoryImages = new List<FactoryImage>();

            // AngleSharp, rip table contents
            var config = Configuration.Default.WithDefaultLoader();
            var source = "https://developers.google.com/android/images";
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(source);

            // Locate all tables in the site (containing the factory images)
            var tables = document.QuerySelectorAll("table");
            // Locate the desired table
            foreach (int num in tableNos)
            {
                // Pick table
                var table = (IHtmlTableElement)tables[num];
                var tableContents = table.Rows;
                // Go through each row after the first one (title row)
                for (int i = 1; i < tableContents.Length; i++)
                {
                    var rowChildren = tableContents[i].Children;
                    // Child 1 - Version, child 2 - Link (href), child 3 - Checksum
                    string version = rowChildren[0].TextContent;
                    string link = ((IHtmlAnchorElement)rowChildren[1].FirstChild).Href.ToString();
                    string checksum = rowChildren[2].TextContent;

                    // Split up version and link by '/' and '-' to get individual elements
                    string[] versionSplit = version.Split(' ');
                    string[] linkSplit = link.Split(new char[] { '/', '-' });

                    // Create the Factory Image and add it to the list of factory images
                    factoryImages.Add(new FactoryImage(linkSplit[6], versionSplit[0], linkSplit[7].ToUpper(), linkSplit[9].Substring(0, 8), version, link, checksum));
                }
            }

            return factoryImages;
        }

        private async Task<List<FactoryImage>> LoadAllFactoryImages()
        {
            List<FactoryImage> factoryImages = new List<FactoryImage>();

            // AngleSharp, rip table contents
            var config = Configuration.Default.WithDefaultLoader();
            var source = "https://developers.google.com/android/images";
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(source);

            // Locate all tables in the site (containing the factory images)
            var tables = document.QuerySelectorAll("table");
            // Go through all tables
            foreach (IHtmlTableElement table in tables)
            {
                var tableContents = table.Rows;
                // Go through each row after the first one (title row)
                for (int i = 1; i < tableContents.Length; i++)
                {
                    var rowChildren = tableContents[i].Children;
                    // Child 1 - Version, child 2 - Link (href), child 3 - Checksum
                    string version = rowChildren[0].TextContent;
                    string link = ((IHtmlAnchorElement)rowChildren[1].FirstChild).Href.ToString();
                    string checksum = rowChildren[2].TextContent;

                    // Split up version and link by '/' and '-' to get individual elements
                    string[] versionSplit = version.Split(' ');
                    string[] linkSplit = link.Split(new char[] { '/', '-' });

                    // Create the Factory Image and add it to the list of factory images
                    factoryImages.Add(new FactoryImage(linkSplit[6], versionSplit[0], linkSplit[7].ToUpper(), linkSplit[9].Substring(0, 8), version, link, checksum));
                }
            }

            return factoryImages;
        }

        private void OpenSiteBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://developers.google.com/android/images");
        }

        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            string filename = filenameTb.Text;

            // serialize JSON directly to a file
            using (StreamWriter file = File.CreateText(filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, factoryImages.Result);
            }
        }

        private void OpenFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Environment.CurrentDirectory);
        }

        private async void LoadAllBtn_Click(object sender, RoutedEventArgs e)
        {
            factoryImages = LoadAllFactoryImages();
            // Load into Data Grid
            factoryImagesDg.ItemsSource = await factoryImages;
        }

        private void ClipboardBtn_Click(object sender, RoutedEventArgs e)
        {
            string json = JsonConvert.SerializeObject(factoryImages.Result);
            Clipboard.SetText(json);
        }
    }
}
