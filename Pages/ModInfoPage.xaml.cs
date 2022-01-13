using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
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
using System.Xml.Linq;
using webScrapWPF;

namespace webScrapWPF.Pages
{
    /// <summary>
    /// Lógica de interacción para ModInfoPage.xaml
    /// </summary>
    public partial class ModInfoPage
    {
        ModInfo actualMod;
        Image downloadImage;
        Image downloadImage2;
        BitmapImage forgeImage;
        BitmapImage fabricImage;
        BitmapImage bitmapImage;
        public ModInfoPage()
        {
            InitializeComponent();
            actualMod = new ModInfo();
            downloadImage = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/download.png"))
            };
            

            

            downloadImage2 = new Image();
            System.Drawing.Bitmap downloadBitMap;

            downloadBitMap = Properties.Resources.download;
            bitmapImage = new BitmapImage();
            using (System.IO.MemoryStream coso = new())
            {
                downloadBitMap.Save(coso, ImageFormat.Png);
                coso.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = coso;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }


            forgeImage = new BitmapImage();
            forgeImage.BeginInit();
            forgeImage.UriSource = new Uri("pack://application:,,,/Resources/forge.png");
            forgeImage.EndInit();

            fabricImage = new BitmapImage();
            fabricImage.BeginInit();
            fabricImage.UriSource = new Uri("pack://application:,,,/Resources/fabric.png");
            fabricImage.EndInit();
            
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
            tags.Children.Clear();
            versionListGrid.Children.Clear();
            versionListGrid.RowDefinitions.Clear();

            versionListGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });

            TextBlock txtName = new();
            TextBlock download_count = new();
            TextBlock download = new();
            TextBlock modLoader = new();

            Grid.SetRow(txtName, 0);
            Grid.SetRow(download_count, 0);
            Grid.SetRow(download, 0);
            Grid.SetRow(modLoader, 0);

            Grid.SetColumn(txtName, 0);
            Grid.SetColumn(download_count, 1);
            Grid.SetColumn(modLoader, 2);
            Grid.SetColumn(download, 3);


            txtName.FontSize    = 16;
            download.FontSize   = 16;
            download_count.FontSize  = 16;
            modLoader.FontSize = 16;

            txtName.HorizontalAlignment = HorizontalAlignment.Center;
            txtName.VerticalAlignment = VerticalAlignment.Top;

            download.VerticalAlignment = VerticalAlignment.Top;
            download.HorizontalAlignment = HorizontalAlignment.Center;  

            download_count.HorizontalAlignment = HorizontalAlignment.Center;
            download_count.VerticalAlignment = VerticalAlignment.Top;

            modLoader.HorizontalAlignment = HorizontalAlignment.Center;
            modLoader.VerticalAlignment = VerticalAlignment.Top;

            txtName.Text = "Minecraft Version";
            download_count.Text = "Total Downloads";
            download.Text = "Download";
            modLoader.Text = "Mod Loader";

            txtName.Foreground = Utilities.Colors.darkThemeTextColor;
            download.Foreground = Utilities.Colors.darkThemeTextColor;
            download_count.Foreground = Utilities.Colors.darkThemeTextColor;
            modLoader.Foreground = Utilities.Colors.darkThemeTextColor;

            versionListGrid.Children.Add(txtName);
            versionListGrid.Children.Add(download);
            versionListGrid.Children.Add(download_count);
            versionListGrid.Children.Add(modLoader);

            modIcon.Source = new BitmapImage(new Uri(actualMod.icon_url));
            modName.Text = actualMod.title;

            downloadProgress.Visibility = Visibility.Hidden;
            downloadProgress.Tag        = "";

            modDesc.Text = actualMod.description;
            timesDownloaded.Text = String.Format("DOWNLOADS\n{0:N0}", actualMod.downloads); //$"DOWNLOADS\n{(actualMod.downloads)} hola";
            lastVersion.Text = $"LASTEST VERSION\n{actualMod.latest_version}";
            clientSide.Text = $"CLIENT SIDE\n{string.Concat(actualMod.client_side[0].ToString().ToUpper(), actualMod.client_side.AsSpan(1))}";
            serverSide.Text = $"SERVER SIDE\n{string.Concat(actualMod.server_side[0].ToString().ToUpper(), actualMod.server_side.AsSpan(1))}";

            List<string> icons = new(){ "adventure", "misc", "utility", "decoration", "fabric", "forge", "worldgen",
                                        "library", "equipment"};

            foreach (var category in actualMod.categories)
            {
                StackPanel tagFull = new StackPanel();
                tagFull.Orientation = Orientation.Horizontal;

                TextBlock categoryText = new TextBlock();
                categoryText.Text = string.Concat(category[0].ToString().ToUpper(), category.AsSpan(1));
                categoryText.Foreground = Utilities.Colors.darkThemeTextColor;
                categoryText.Margin = new Thickness(1, 2, 30, 0);
                categoryText.VerticalAlignment = VerticalAlignment.Top;
                categoryText.FontSize = 14;

                Image categoryImage = new();
                if (icons.Contains(category))
                    categoryImage.Source = new BitmapImage(new Uri(@"U:\Programacion\C#\webScrapWPF\Resources\" + category + ".png"));
                else categoryImage.Source = new BitmapImage(new Uri(@"U:\Programacion\C#\webScrapWPF\Resources\imageNotFound.png"));
                categoryImage.VerticalAlignment = VerticalAlignment.Top;
                categoryImage.Stretch = Stretch.Uniform;
                categoryImage.Height = 25;
                // U:\Programacion\C#\webScrapWPF\Resources\

                tagFull.Children.Add(categoryImage);
                tagFull.Children.Add(categoryText);

                tags.Children.Add(tagFull);
            }
        }
        public void DataRefresh()
        {

            downloadProgress.Visibility = Visibility.Hidden;
            downloadProgress.Tag = "";

            int i = 1;
            foreach(var version in actualMod.versionsDetailled)
            {
                Image download = new();
                Image loader = new();

                download.MouseLeftButtonDown += downloadClicked;
                download.Source = downloadImage.Source;
                download.Margin = new Thickness(8);
                download.Name   = $"icon{i}";
                versionListGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });

                loader.Margin = new Thickness(8);
                if(version.loaders[0] == "fabric")
                {
                    loader.Source = fabricImage;
                } else
                {
                    loader.Source = forgeImage;
                    loader.Margin = new Thickness(11);
                }


                TextBlock actualVersion = new TextBlock();
                TextBlock actualDownloads = new TextBlock();

                actualVersion.Text = String.Join(", ", version.game_versions);      
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
                Grid.SetColumn(loader, 2);
                Grid.SetColumn(download, 3);

                Grid.SetRow(actualDownloads, i);
                Grid.SetRow(actualVersion, i);
                Grid.SetRow(download, i);
                Grid.SetRow(loader, i);

                versionListGrid.Children.Add(actualDownloads);
                versionListGrid.Children.Add(actualVersion);
                versionListGrid.Children.Add(download);
                versionListGrid.Children.Add(loader);
                i++;
            }
        }
 
        public async void downloadClicked(object sender, MouseButtonEventArgs e)
        {
            Progress<int> progress = new();
            progress.ProgressChanged += changedProgress;

            downloadProgress.Value = 0;
            downloadProgress.Visibility = Visibility.Visible;
            Image send = (Image)sender;
            int pos = int.Parse(send.Name.Replace("icon", ""))-1;

            string url = actualMod.versionsDetailled[pos].files[0].url;
            await Scraper.download(url, actualMod.versionsDetailled[pos].files[0].filename, progress);
            downloadProgress.Value = 100;
            downloadProgress.Tag = "Done!";
        }
        private void changedProgress(object? sender, int newProgress)
        {
            downloadProgress.Value = newProgress;
            downloadProgress.Tag = (newProgress != 100) ? newProgress.ToString() + "%" : "Done!";
        }
    }
}
