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
    List<TextBlock> modAuthor;
    List<Image> images;
    List<StackPanel> stackPanels;

    ModList modList;
    ModInfoPage modInfoPage;

    manyNames modNamesList;

    public MainWindow()
    {
        InitializeComponent();
        Scraper.setHeaders();

        pages       = new();
        modNames    = new();
        modAuthor   = new();
        stackPanels = new();
        images      = new();
        modList     = new();
        modInfoPage = new();
       

        version.Text = "v1.3.3";
        //modList.pagesGrid.ShowGridLines = true;

        frame.Content = modList;

        //windowGrid.ShowGridLines = true;
        

        for (int i = 0; i < 20; i++)
        {
            modNames.Add(new TextBlock());
            modAuthor.Add(new TextBlock());
            images.Add(new Image());

            Grid Card = new();
            Grid middleGrid = new();
            //Card.ShowGridLines = true;

            Card.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(125) });
            Card.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(300) });
            Card.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0,0) });

            middleGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0,0)});
            middleGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, 0)});

            modNames[i].Name        = $"txtBlock{i + 1}";
            modNames[i].Foreground  = Utilities.Colors.darkThemeTextColor;

            modAuthor[i].Name       = $"txtBlock{i + 1}";
            modAuthor[i].Foreground = Utilities.Colors.darkThemeTextColor;
            
            images[i].Name          = $"image{i + 1}";
            images[i].HorizontalAlignment       = System.Windows.HorizontalAlignment.Left;
            images[i].VerticalAlignment         = VerticalAlignment.Top;
            images[i].Height                    = 100;
            Grid.SetColumn(images[i], 0);

            modNames[i].FontSize = 20;
            modNames[i].MaxHeight = 40;

            modAuthor[i].FontSize = 14;
            modAuthor[i].MaxHeight = 40;

            modNames[i].TextAlignment       = System.Windows.TextAlignment.Left;
            modNames[i].VerticalAlignment   = System.Windows.VerticalAlignment.Center;
            modAuthor[i].TextAlignment      = System.Windows.TextAlignment.Left;
            modAuthor[i].VerticalAlignment  = System.Windows.VerticalAlignment.Center;  

            StackPanel nameAndAuthor = new() { Orientation = System.Windows.Controls.Orientation.Horizontal} ;

            nameAndAuthor.Children.Add(modNames[i]);
            nameAndAuthor.Children.Add(modAuthor[i]);
            
            Grid.SetColumn(nameAndAuthor, 0);

           
            Grid.SetColumn(middleGrid, 1);
            Grid.SetRow(middleGrid, 0);
            
            middleGrid.Children.Add(nameAndAuthor);
            

            Card.Children.Add(middleGrid);
            Card.Children.Add(images[i]);
            
            Card.Margin = new Thickness(0, 0, 0, 15);

            Card.MouseLeftButtonDown += clickOnMod;
            Card.Name = $"Card{i + 1}";
            modList.CardPanel.Children.Add(Card);

        }

        txtInputPages.Text = actualPage.ToString();

        Init();
    }

    private Grid getElement(int position)
    {
        return modList.CardPanel.Children.OfType<Grid>().ElementAt(position);
    }

    private async void Init()
    {
        pages.Add(-1, new Page());
        await LoadPage(actualPage);
        await bigNameSearch();
    }

    private async void clickOnMod(object sender, MouseButtonEventArgs e)
    {
        Grid sen = (Grid)sender;

        int currentMod = int.Parse(sen.Name.Replace("Card", ""))-1;

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

        if (txtInputPages.Text == "" || int.Parse(txtInputPages.Text) <= 0)
        {
            txtAux.Text = "Pagina no encontrada";
            return;
        }

        else if (!int.TryParse(txtInputPages.Text, out page))
        {
            pages[-1] = new Page();
            await pages[-1].InitQuery(txtInputPages.Text);
            actualPage = -1;
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

            getElement(i).Visibility = Visibility.Visible;
            getElement(i).IsEnabled = true;

            modNames[i].Text = pages[actualPage].mods[i].title;
            modAuthor[i].Text = "   by  " + pages[actualPage].mods[i].author;
            if (pages[actualPage].mods[i].icon_url == "")
                pages[actualPage].mods[i].icon_url = "pack://application:,,,/Resources/imageNotFound.png";
            images[i].Source = new BitmapImage(new Uri(pages[actualPage].mods[i].icon_url));
        }

        if(pages[actualPage].mods.Count < 20)
        {
            for(int i = pages[actualPage].mods.Count; i < 20; i++)
            {
                getElement(i).Visibility = Visibility.Hidden;
                getElement(i).IsEnabled = false;
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