using System.Windows;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Win32;
using System;

namespace FactoryImage2Json
{
    /// <summary>
    /// Interaction logic for ClientJson.xaml
    /// </summary>
    public partial class ClientJson : Window
    {
        public ClientJson()
        {
            InitializeComponent();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            Toolkit toolkit = new Toolkit(versionTb.Text, versionCodeTb.Text, updateLinkTb.Text, xdaThreadLinkTb.Text, noteLinkTb.Text);
            Images images = new Images(versionTextTb.Text, versionIntTb.Text);

            Client client = new Client(toolkit, images);

            if ((bool)toClipboardCb.IsChecked)
            {
                string json = JsonConvert.SerializeObject(client);
                Clipboard.SetText(json);
            }
            else
            {
                // serialize JSON directly to a file
                using (StreamWriter file = File.CreateText("client.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, client);
                }
            }
        }

        private void LoadBtn_Click(object sender, RoutedEventArgs e)
        {
            // Get filepath of client.json
            string jsonPath;

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };
            if (openFileDialog.ShowDialog() == true)
            {
                jsonPath = openFileDialog.FileName;

                try
                {
                    string json = File.ReadAllText(jsonPath);
                    Client client = JsonConvert.DeserializeObject<Client>(json);
                    // Populate text boxes
                    LoadClientObject(client);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void LoadClientObject(Client client)
        {
            // Fill out the textboxes
            versionTb.Text = client.Toolkit.Version;
            versionCodeTb.Text = client.Toolkit.VersionCode;
            updateLinkTb.Text = client.Toolkit.UpdateLink;
            xdaThreadLinkTb.Text = client.Toolkit.XdaThreadLink;
            noteLinkTb.Text = client.Toolkit.Note;

            versionTextTb.Text = client.Images.Version;
            versionIntTb.Text = client.Images.VersionCode;
        }
    }
}
