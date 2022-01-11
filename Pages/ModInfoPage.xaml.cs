using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using webScrapWPF;

namespace webScrapWPF.Pages
{
    /// <summary>
    /// Lógica de interacción para ModInfoPage.xaml
    /// </summary>
    public partial class ModInfoPage
    {
        ModInfo actualMod;
        BitmapImage downloadImage;
        public ModInfoPage()
        {
            InitializeComponent();
            actualMod = new ModInfo();
            downloadImage = new BitmapImage();
            downloadImage.BeginInit();
            downloadImage.UriSource = new Uri("U:/Programacion/C#/webScrapWPF/Resources/download.png");
            downloadImage.EndInit();
            //versionListGrid.ShowGridLines = true;
            
            //download.Source = "/Pages/download2.png";
        }
            
        public async void ChangeMod(Task modTask, ModInfo newMod)
        {
            actualMod = newMod;
            InitRefresh();
            await Task.Delay(1);
            await modTask;
            DataRefresh();
        }

        public void InitRefresh()
        {
            versionListGrid.Children.Clear();
            versionListGrid.RowDefinitions.Clear();

            versionListGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });

            TextBlock txtName = new();
            TextBlock downloads = new();
            TextBlock download = new();

            Grid.SetRow(txtName, 0);
            Grid.SetRow(downloads, 0);
            Grid.SetRow(download, 0);

            Grid.SetColumn(txtName, 0);
            Grid.SetColumn(downloads, 1);
            Grid.SetColumn(download, 2);


            txtName.FontSize    = 16;
            download.FontSize   = 16;
            downloads.FontSize  = 16;

            txtName.HorizontalAlignment = HorizontalAlignment.Center;
            txtName.VerticalAlignment = VerticalAlignment.Top;

            download.VerticalAlignment = VerticalAlignment.Top;
            download.HorizontalAlignment = HorizontalAlignment.Center;  

            downloads.HorizontalAlignment = HorizontalAlignment.Center;
            downloads.VerticalAlignment = VerticalAlignment.Top;

            txtName.Text = "Minecraft Version";
            downloads.Text = "Total Downloads";
            download.Text = "Download";

            txtName.Foreground = Utilities.Colors.darkThemeTextColor;
            download.Foreground = Utilities.Colors.darkThemeTextColor;
            downloads.Foreground = Utilities.Colors.darkThemeTextColor;

            versionListGrid.Children.Add(txtName);
            versionListGrid.Children.Add(download);
            versionListGrid.Children.Add(downloads);



            modIcon.Source = new BitmapImage(new Uri(actualMod.icon_url));
            modName.Text = actualMod.title;


            modDesc.Text = actualMod.description;
            timesDownloaded.Text = String.Format("DOWNLOADS\n{0:N0}", actualMod.downloads); //$"DOWNLOADS\n{(actualMod.downloads)} hola";
            lastVersion.Text = $"LASTEST VERSION\n{actualMod.latest_version}";
            clientSide.Text = $"CLIENT SIDE\n{string.Concat(actualMod.client_side[0].ToString().ToUpper(), actualMod.client_side.AsSpan(1))}";
            serverSide.Text = $"SERVER SIDE\n{string.Concat(actualMod.server_side[0].ToString().ToUpper(), actualMod.server_side.AsSpan(1))}";
            tags.Text = String.Join(", ", actualMod.categories.Select(x => string.Concat(x[0].ToString().ToUpper(), x.AsSpan(1))));
        }
        public void DataRefresh()
        {

            int i = 1;
            foreach(var version in actualMod.versionsDetailled)
            {
                Image download = new();

                download.MouseLeftButtonDown += downloadClicked;
                download.Source = downloadImage;
                download.Margin = new Thickness(8);
                download.Name   = $"icon{i}";
                versionListGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });

                TextBlock actualVersion = new TextBlock();
                TextBlock actualDownloads = new TextBlock();

                actualVersion.Text = String.Join(", ", version.game_versions);      // [1.18, 1.18.1]
                actualVersion.Foreground = Utilities.Colors.darkThemeTextColor;
                actualVersion.HorizontalAlignment = HorizontalAlignment.Center;
                actualVersion.VerticalAlignment = VerticalAlignment.Center;
                actualVersion.FontSize = 16;
                actualVersion.TextWrapping = TextWrapping.Wrap;

                actualDownloads.Text = version.downloads.ToString();
                actualDownloads.Foreground = Utilities.Colors.darkThemeTextColor;
                actualDownloads.FontSize = 16;
                actualDownloads.HorizontalAlignment = HorizontalAlignment.Center;
                actualDownloads.VerticalAlignment = VerticalAlignment.Center;

                Grid.SetColumn(actualVersion, 0);
                Grid.SetColumn(actualDownloads, 1);
                Grid.SetColumn(download, 2);

                Grid.SetRow(actualDownloads, i);
                Grid.SetRow(actualVersion, i);
                Grid.SetRow(download, i);

                versionListGrid.Children.Add(actualDownloads);
                versionListGrid.Children.Add(actualVersion);
                versionListGrid.Children.Add(download);

                i++;
            }
        }
        public async void downloadClicked(object sender, MouseButtonEventArgs e)
        {
            Image send = (Image)sender;
            int pos = int.Parse(send.Name.Replace("icon", ""))-1;

            string url = actualMod.versionsDetailled[pos].files[0].url;

            await Scraper.download(url, actualMod.versionsDetailled[pos].files[0].filename);
        }
    }
}
