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
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using webScrapWPF.Pages;
using System.Windows.Forms;

namespace webScrapWPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
/// 
public partial class MainWindow : Window
{
    public static string downloadPath = "./";

    Dictionary<int, Page> pages;
    int actualPage = 1;

    List<TextBlock> modNames;
    List<Image> images;
    List<StackPanel> stackPanels;

    ModList modList;
    ModInfoPage modInfoPage;

    manyNames modNamesList;

    public MainWindow()
    {
        Scraper.setHeaders();
        InitializeComponent();
        pages       = new();

        modNames    = new();
        stackPanels = new();
        images      = new();
        modList     = new();
        modInfoPage = new();


        version.Text = "v1.3.1";
        //modList.pagesGrid.ShowGridLines = true;

        frame.Content = modList;

        //windowGrid.ShowGridLines = true;
        

        for (int i = 0; i < 20; i++)
        {
            modNames.Add(new TextBlock());
            stackPanels.Add(new StackPanel());
            images.Add(new Image());

            modNames[i].Name        = $"txtBlock{(i+1).ToString()}";
            modNames[i].Foreground  = Utilities.Colors.darkThemeTextColor;
            stackPanels[i].Name     = $"stackPanel{(i + 1).ToString()}";
            images[i].Name          = $"image{(i + 1).ToString()}";

            stackPanels[i].MouseLeftButtonDown += clickOnMod;
            stackPanels[i].Margin = new Thickness(10);

            stackPanels[i].HorizontalAlignment  = System.Windows.HorizontalAlignment.Center;
            stackPanels[i].VerticalAlignment    = VerticalAlignment.Center;
            stackPanels[i].Orientation = System.Windows.Controls.Orientation.Vertical;

            images[i].HorizontalAlignment       = System.Windows.HorizontalAlignment.Center;
            images[i].VerticalAlignment         = VerticalAlignment.Center;
            images[i].Height                    = 100;
            images[i].Margin = new Thickness(0, 20, 0, 0);

            modNames[i].HorizontalAlignment     = System.Windows.HorizontalAlignment.Center;
            modNames[i].VerticalAlignment       = VerticalAlignment.Center;

            modNames[i].FontSize = 20;
            modNames[i].MaxHeight = 40;
            modNames[i].TextWrapping = TextWrapping.Wrap;

            SolidColorBrush clr = new();
            stackPanels[i].Background = clr;

            stackPanels[i].Style = (Style)this.Resources["StackPanelStyle1"];
            stackPanels[i].Children.Add(images[i]);
            stackPanels[i].Children.Add(modNames[i]);
            stackPanels[i].MinHeight = 150;
            stackPanels[i].MinWidth = 200;


            modList.pagesGrid.Children.Add(stackPanels[i]);

            Grid.SetRow(stackPanels[i], i / 5);
            Grid.SetColumn(stackPanels[i], i % 5);
        }

        txtInputPages.Text = actualPage.ToString();

        Init();
    }

    private async void Init()
    {
        pages.Add(-1, new Page());
        await LoadPage(actualPage);
        await bigNameSearch();
    }

    private async void clickOnMod(object sender, MouseButtonEventArgs e)
    {
        StackPanel sen = (StackPanel)sender;

        int currentMod = int.Parse(sen.Name.Replace("stackPanel", ""))-1;

        txtAux.Text = pages[actualPage].mods[currentMod].title;

        //Task pageScrap = Task.Run(() => pages[actualPage].mods[currentMod].versions);
        //pageScrap.Start();

        frame.Content = modInfoPage;

        Task modTask = pages[actualPage].mods[currentMod].LoadVersions();

        modInfoPage.ChangeMod(modTask, pages[actualPage].mods[currentMod]);

        btnBack.IsEnabled = true;

        
    }

    private async void btnSearch_Click(object sender, RoutedEventArgs e)
    {
        int page;


        if (!int.TryParse(txtInputPages.Text, out page))
        {
            pages[-1] = new Page();
            await pages[-1].InitQuery(txtInputPages.Text);
            actualPage = -1;
        }
        else if (txtInputPages.Text == "" || int.Parse(txtInputPages.Text) <= 0)
        {
            txtAux.Text = "Pagina no encontrada";
            return;
        }
        else
        {
            actualPage = page;

            txtAux.Text = "Cargando";

        }
        await LoadPage(actualPage);
        txtAux.Text = "Terminado";
    }

    private async void btnSearchOnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            btnSearch_Click(sender, new RoutedEventArgs());
        }
    }

    private async Task LoadPage(int page)
    {
        if(!pages.ContainsKey(page))
        {
            Page auxPage = new();
            await auxPage.Init(page);
            pages.Add(page, auxPage);
        }

        for(int i = 0; i < pages[actualPage].mods.Count; i++)
        {

            stackPanels[i].Visibility = Visibility.Visible;
            stackPanels[i].IsEnabled = true;

            modNames[i].Text = pages[actualPage].mods[i].title;

            if (pages[actualPage].mods[i].icon_url == "")
                pages[actualPage].mods[i].icon_url = "pack://application:,,,/Resources/imageNotFound.png";
            images[i].Source = new BitmapImage(new Uri(pages[actualPage].mods[i].icon_url));
        }

        if(pages[actualPage].mods.Count < 20)
        {
            for(int i = pages[actualPage].mods.Count; i < 20; i++)
            {
                stackPanels[i].Visibility = Visibility.Hidden;
                stackPanels[i].IsEnabled = false;
            }
        }

    }

    private void btnBack_Click(object sender, RoutedEventArgs e)
    {
        frame.Content = modList;
        btnBack.IsEnabled = false;
    }

    private void btnFolder_Click(object sender, RoutedEventArgs e)
    {
        FolderBrowserDialog fbd = new FolderBrowserDialog();
        
        var x = fbd.ShowDialog();

        txtInputFolder.Text = fbd.SelectedPath;
        downloadPath = fbd.SelectedPath.Replace("\\", "/");
    }


    private async Task bigNameSearch()
    {
        string jsonInStringFormat = await Scraper.get(Url.getFullUrl(100, 0));
        modNamesList = JsonSerializer.Deserialize<manyNames>(jsonInStringFormat, new JsonSerializerOptions {IncludeFields=true});
    }

}