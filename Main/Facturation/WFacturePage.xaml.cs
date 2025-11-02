using GestionComerce;
using GestionComerce.Main.Facturation;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
using System.Windows.Shapes;

namespace Superete.Main.Facturation
{
    /// <summary>
    /// Interaction logic for WFacturePage.xaml
    /// </summary>
    public partial class WFacturePage : Window
    {
        public CMainFa main;
        public WFacturePage(CMainFa main)
        {
            InitializeComponent();
            List<List<OperationArticle>> ListListArts = new List<List<OperationArticle>>();
            this.main = main;
            if(main.OperationContainer.Children[0] is CSingleOperation sop)
            {
                List<OperationArticle> ListArts = new List<OperationArticle>();
                bool skip=false;
                foreach (OperationArticle oa in main.main.loa)
                {
                    if (oa.OperationID == sop.op.OperationID)
                    {
                        ListArts.Add(oa);
                    }
                    if (ListArts.Count == 5 && skip==false) { 
                        ListListArts.Add(ListArts);
                        ListArts = new List<OperationArticle>();
                        skip = true;
                    }
                    if (ListArts.Count == 7)
                    {
                        ListListArts.Add(ListArts);
                        ListArts = new List<OperationArticle>();
                    }
                }
                if (ListArts.Count > 0)
                {
                    ListListArts.Add(ListArts);
                }
            }
            LoadArticles(ListListArts);

        }
        int PageCounte = 1;int TotalPageCount = 1;
        public void LoadArticles(List<List<OperationArticle>> ListListArts)
        {
            int index = 0;
            TotalPageCount = ListListArts.Count;
            TotalPageNbr.Text = "/" + ListListArts.Count.ToString();
            foreach (var list in ListListArts)
            {
                index++;
                if (ListListArts.Count == 1)
                {
                    //Load first template
                    Canvas mainCanvas = new Canvas
                    {
                        Height = 1000,
                        Width = 720
                    };

                    // Create Image
                    Image image = new Image
                    {
                        Source = new BitmapImage(new Uri("/Main/images/1.jpg", UriKind.Relative)),
                        Stretch = Stretch.Fill,
                        Height = 1000,
                        Width = 720
                    };
                    mainCanvas.Children.Add(image);

                    // Create StackPanel
                    StackPanel articlesContainer = new StackPanel
                    {
                        Orientation = Orientation.Vertical,
                        Background = new SolidColorBrush(Colors.AliceBlue),
                        Width = 562,
                        Height = 196,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Top,
                        Name = "ArticlesContainer"
                    };

                    // Set StackPanel position on Canvas
                    Canvas.SetLeft(articlesContainer, 82);
                    Canvas.SetTop(articlesContainer, 534);

                    // Add StackPanel to Canvas
                    mainCanvas.Children.Add(articlesContainer);

                    // Set the Canvas as the Window content
                    FacturesContainer.Children.Add(mainCanvas);


                    foreach (var oa in list)
                    {
                        foreach(var a in main.main.laa)
                        {
                            if(a.ArticleID == oa.ArticleID)
                            {
                                //Load articles in template
                                Grid articleRow = new Grid
                                {
                                    Width = 562,
                                    Height = 20,
                                    Background = new SolidColorBrush(Color.FromRgb(235, 245, 255)), // light background
                                    Margin = new Thickness(0, 5, 0, 5)
                                };

                                // Define 4 columns
                                articleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }); // Name
                                articleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Unit price
                                articleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Quantité
                                articleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Total

                                // Create TextBlocks
                                TextBlock nameBlock = new TextBlock
                                {
                                    Text = a.ArticleName,
                                    VerticalAlignment = VerticalAlignment.Center,
                                    Margin = new Thickness(5, 0, 0, 0)
                                };
                                TextBlock priceBlock = new TextBlock
                                {
                                    Text = a.PrixVente.ToString(),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    TextAlignment = TextAlignment.Center
                                };
                                TextBlock qtyBlock = new TextBlock
                                {
                                    Text = oa.QteArticle.ToString(),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    TextAlignment = TextAlignment.Center
                                };
                                TextBlock totalBlock = new TextBlock
                                {
                                    Text = (oa.QteArticle* a.PrixVente).ToString(),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    TextAlignment = TextAlignment.Center
                                };

                                // Assign grid positions
                                Grid.SetColumn(nameBlock, 0);
                                Grid.SetColumn(priceBlock, 1);
                                Grid.SetColumn(qtyBlock, 2);
                                Grid.SetColumn(totalBlock, 3);

                                // Add text blocks to grid
                                articleRow.Children.Add(nameBlock);
                                articleRow.Children.Add(priceBlock);
                                articleRow.Children.Add(qtyBlock);
                                articleRow.Children.Add(totalBlock);
                                Canvas.SetLeft(articleRow, 82);
                                Canvas.SetTop(articleRow, 434);
                                articlesContainer.Children.Add(articleRow);
                            }
                        }
                        
                    }

                }
                else if (ListListArts.Count == 2)

                {
                    //Load first template
                    if (index == 1)
                    {
                        Canvas mainCanvas = new Canvas
                        {
                            Height = 1000,
                            Width = 720,
                            Name = $"Canvas{1}"
                        };

                        // Create Image
                        Image image = new Image
                        {
                            Source = new BitmapImage(new Uri("/Main/images/2.jpg", UriKind.Relative)),
                            Stretch = Stretch.Fill,
                            Height = 1000,
                            Width = 720
                        };
                        mainCanvas.Children.Add(image);

                        // Create StackPanel
                        StackPanel articlesContainer = new StackPanel
                        {
                            Orientation = Orientation.Vertical,
                            Background = new SolidColorBrush(Colors.AliceBlue),
                            Width = 562,
                            Height = 196,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Top,
                            Name = "ArticlesContainer"
                        };

                        // Set StackPanel position on Canvas
                        Canvas.SetLeft(articlesContainer, 82);
                        Canvas.SetTop(articlesContainer, 534);

                        // Add StackPanel to Canvas
                        mainCanvas.Children.Add(articlesContainer);

                        // Set the Canvas as the Window content
                        FacturesContainer.Children.Add(mainCanvas);


                        foreach (var oa in list)
                        {
                            foreach (var a in main.main.laa)
                            {
                                if (a.ArticleID == oa.ArticleID)
                                {
                                    //Load articles in template
                                    Grid articleRow = new Grid
                                    {
                                        Width = 562,
                                        Height = 20,
                                        Background = new SolidColorBrush(Color.FromRgb(235, 245, 255)), // light background
                                        Margin = new Thickness(0, 5, 0, 5)
                                    };

                                    // Define 4 columns
                                    articleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }); // Name
                                    articleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Unit price
                                    articleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Quantité
                                    articleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Total

                                    // Create TextBlocks
                                    TextBlock nameBlock = new TextBlock
                                    {
                                        Text = a.ArticleName,
                                        VerticalAlignment = VerticalAlignment.Center,
                                        Margin = new Thickness(5, 0, 0, 0)
                                    };
                                    TextBlock priceBlock = new TextBlock
                                    {
                                        Text = a.PrixVente.ToString(),
                                        VerticalAlignment = VerticalAlignment.Center,
                                        TextAlignment = TextAlignment.Center
                                    };
                                    TextBlock qtyBlock = new TextBlock
                                    {
                                        Text = oa.QteArticle.ToString(),
                                        VerticalAlignment = VerticalAlignment.Center,
                                        TextAlignment = TextAlignment.Center
                                    };
                                    TextBlock totalBlock = new TextBlock
                                    {
                                        Text = (oa.QteArticle * a.PrixVente).ToString(),
                                        VerticalAlignment = VerticalAlignment.Center,
                                        TextAlignment = TextAlignment.Center
                                    };

                                    // Assign grid positions
                                    Grid.SetColumn(nameBlock, 0);
                                    Grid.SetColumn(priceBlock, 1);
                                    Grid.SetColumn(qtyBlock, 2);
                                    Grid.SetColumn(totalBlock, 3);

                                    // Add text blocks to grid
                                    articleRow.Children.Add(nameBlock);
                                    articleRow.Children.Add(priceBlock);
                                    articleRow.Children.Add(qtyBlock);
                                    articleRow.Children.Add(totalBlock);
                                    Canvas.SetLeft(articleRow, 82);
                                    Canvas.SetTop(articleRow, 434);
                                    articlesContainer.Children.Add(articleRow);
                                }
                            }

                        }
                    }
                    else if(index ==2)
                    {
                        Canvas mainCanvas = new Canvas
                        {
                            Height = 1000,
                            Width = 720,
                            Visibility = Visibility.Collapsed,
                            Name = $"Canvas{2}"
                        };

                        // Create Image
                        Image image = new Image
                        {
                            Source = new BitmapImage(new Uri("/Main/images/4.jpg", UriKind.Relative)),
                            Stretch = Stretch.Fill,
                            Height = 1000,
                            Width = 720
                        };
                        mainCanvas.Children.Add(image);

                        // Create StackPanel
                        StackPanel articlesContainer = new StackPanel
                        {
                            Orientation = Orientation.Vertical,
                            Background = new SolidColorBrush(Colors.AliceBlue),
                            Width = 562,
                            Height = 196,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Top,
                            Name = "ArticlesContainer"
                        };

                        // Set StackPanel position on Canvas
                        Canvas.SetLeft(articlesContainer, 82);
                        Canvas.SetTop(articlesContainer, 434);

                        // Add StackPanel to Canvas
                        mainCanvas.Children.Add(articlesContainer);

                        // Set the Canvas as the Window content
                        FacturesContainer.Children.Add(mainCanvas);


                        foreach (var oa in list)
                        {
                            foreach (var a in main.main.laa)
                            {
                                if (a.ArticleID == oa.ArticleID)
                                {
                                    //Load articles in template
                                    Grid articleRow = new Grid
                                    {
                                        Width = 562,
                                        Height = 20,
                                        Background = new SolidColorBrush(Color.FromRgb(235, 245, 255)), // light background
                                        Margin = new Thickness(0, 5, 0, 5)
                                    };

                                    // Define 4 columns
                                    articleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }); // Name
                                    articleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Unit price
                                    articleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Quantité
                                    articleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Total

                                    // Create TextBlocks
                                    TextBlock nameBlock = new TextBlock
                                    {
                                        Text = a.ArticleName,
                                        VerticalAlignment = VerticalAlignment.Center,
                                        Margin = new Thickness(5, 0, 0, 0)
                                    };
                                    TextBlock priceBlock = new TextBlock
                                    {
                                        Text = a.PrixVente.ToString(),
                                        VerticalAlignment = VerticalAlignment.Center,
                                        TextAlignment = TextAlignment.Center
                                    };
                                    TextBlock qtyBlock = new TextBlock
                                    {
                                        Text = oa.QteArticle.ToString(),
                                        VerticalAlignment = VerticalAlignment.Center,
                                        TextAlignment = TextAlignment.Center
                                    };
                                    TextBlock totalBlock = new TextBlock
                                    {
                                        Text = (oa.QteArticle * a.PrixVente).ToString(),
                                        VerticalAlignment = VerticalAlignment.Center,
                                        TextAlignment = TextAlignment.Center
                                    };

                                    // Assign grid positions
                                    Grid.SetColumn(nameBlock, 0);
                                    Grid.SetColumn(priceBlock, 1);
                                    Grid.SetColumn(qtyBlock, 2);
                                    Grid.SetColumn(totalBlock, 3);

                                    // Add text blocks to grid
                                    articleRow.Children.Add(nameBlock);
                                    articleRow.Children.Add(priceBlock);
                                    articleRow.Children.Add(qtyBlock);
                                    articleRow.Children.Add(totalBlock);
                                    Canvas.SetLeft(articleRow, 82);
                                    Canvas.SetTop(articleRow, 434);
                                    articlesContainer.Children.Add(articleRow);
                                }
                            }

                        }
                    }


                }
                else{
                    //Load first template
                    if (index == 1)
                    {
                        Canvas mainCanvas = new Canvas
                        {
                            Height = 1000,
                            Width = 720,
                            Name = $"Canvas{1}"
                        };

                        // Create Image
                        Image image = new Image
                        {
                            Source = new BitmapImage(new Uri("/Main/images/2.jpg", UriKind.Relative)),
                            Stretch = Stretch.Fill,
                            Height = 1000,
                            Width = 720
                        };
                        mainCanvas.Children.Add(image);

                        // Create StackPanel
                        StackPanel articlesContainer = new StackPanel
                        {
                            Orientation = Orientation.Vertical,
                            Background = new SolidColorBrush(Colors.AliceBlue),
                            Width = 562,
                            Height = 196,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Top,
                            Name = "ArticlesContainer"
                        };

                        // Set StackPanel position on Canvas
                        Canvas.SetLeft(articlesContainer, 82);
                        Canvas.SetTop(articlesContainer, 534);

                        // Add StackPanel to Canvas
                        mainCanvas.Children.Add(articlesContainer);

                        // Set the Canvas as the Window content
                        FacturesContainer.Children.Add(mainCanvas);


                        foreach (var oa in list)
                        {
                            foreach (var a in main.main.laa)
                            {
                                if (a.ArticleID == oa.ArticleID)
                                {
                                    //Load articles in template
                                    Grid articleRow = new Grid
                                    {
                                        Width = 562,
                                        Height = 20,
                                        Background = new SolidColorBrush(Color.FromRgb(235, 245, 255)), // light background
                                        Margin = new Thickness(0, 5, 0, 5)
                                    };

                                    // Define 4 columns
                                    articleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }); // Name
                                    articleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Unit price
                                    articleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Quantité
                                    articleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Total

                                    // Create TextBlocks
                                    TextBlock nameBlock = new TextBlock
                                    {
                                        Text = a.ArticleName,
                                        VerticalAlignment = VerticalAlignment.Center,
                                        Margin = new Thickness(5, 0, 0, 0)
                                    };
                                    TextBlock priceBlock = new TextBlock
                                    {
                                        Text = a.PrixVente.ToString(),
                                        VerticalAlignment = VerticalAlignment.Center,
                                        TextAlignment = TextAlignment.Center
                                    };
                                    TextBlock qtyBlock = new TextBlock
                                    {
                                        Text = oa.QteArticle.ToString(),
                                        VerticalAlignment = VerticalAlignment.Center,
                                        TextAlignment = TextAlignment.Center
                                    };
                                    TextBlock totalBlock = new TextBlock
                                    {
                                        Text = (oa.QteArticle * a.PrixVente).ToString(),
                                        VerticalAlignment = VerticalAlignment.Center,
                                        TextAlignment = TextAlignment.Center
                                    };

                                    // Assign grid positions
                                    Grid.SetColumn(nameBlock, 0);
                                    Grid.SetColumn(priceBlock, 1);
                                    Grid.SetColumn(qtyBlock, 2);
                                    Grid.SetColumn(totalBlock, 3);

                                    // Add text blocks to grid
                                    articleRow.Children.Add(nameBlock);
                                    articleRow.Children.Add(priceBlock);
                                    articleRow.Children.Add(qtyBlock);
                                    articleRow.Children.Add(totalBlock);
                                    Canvas.SetLeft(articleRow, 82);
                                    Canvas.SetTop(articleRow, 434);
                                    articlesContainer.Children.Add(articleRow);
                                }
                            }

                        }
                    }
                    else if (index == ListListArts.Count)
                    {
                        Canvas mainCanvas = new Canvas
                        {
                            Height = 1000,
                            Width = 720,
                            Visibility = Visibility.Collapsed,
                            Name = $"Canvas{index}"
                        };

                        // Create Image
                        Image image = new Image
                        {
                            Source = new BitmapImage(new Uri("/Main/images/4.jpg", UriKind.Relative)),
                            Stretch = Stretch.Fill,
                            Height = 1000,
                            Width = 720
                        };
                        mainCanvas.Children.Add(image);

                        // Create StackPanel
                        StackPanel articlesContainer = new StackPanel
                        {
                            Orientation = Orientation.Vertical,
                            Background = new SolidColorBrush(Colors.AliceBlue),
                            Width = 562,
                            Height = 196,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Top,
                            Name = "ArticlesContainer"
                        };

                        // Set StackPanel position on Canvas
                        Canvas.SetLeft(articlesContainer, 82);
                        Canvas.SetTop(articlesContainer, 434);

                        // Add StackPanel to Canvas
                        mainCanvas.Children.Add(articlesContainer);

                        // Set the Canvas as the Window content
                        FacturesContainer.Children.Add(mainCanvas);


                        foreach (var oa in list)
                        {
                            foreach (var a in main.main.laa)
                            {
                                if (a.ArticleID == oa.ArticleID)
                                {
                                    //Load articles in template
                                    Grid articleRow = new Grid
                                    {
                                        Width = 562,
                                        Height = 20,
                                        Background = new SolidColorBrush(Color.FromRgb(235, 245, 255)), // light background
                                        Margin = new Thickness(0, 5, 0, 5)
                                    };

                                    // Define 4 columns
                                    articleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }); // Name
                                    articleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Unit price
                                    articleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Quantité
                                    articleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Total

                                    // Create TextBlocks
                                    TextBlock nameBlock = new TextBlock
                                    {
                                        Text = a.ArticleName,
                                        VerticalAlignment = VerticalAlignment.Center,
                                        Margin = new Thickness(5, 0, 0, 0)
                                    };
                                    TextBlock priceBlock = new TextBlock
                                    {
                                        Text = a.PrixVente.ToString(),
                                        VerticalAlignment = VerticalAlignment.Center,
                                        TextAlignment = TextAlignment.Center
                                    };
                                    TextBlock qtyBlock = new TextBlock
                                    {
                                        Text = oa.QteArticle.ToString(),
                                        VerticalAlignment = VerticalAlignment.Center,
                                        TextAlignment = TextAlignment.Center
                                    };
                                    TextBlock totalBlock = new TextBlock
                                    {
                                        Text = (oa.QteArticle * a.PrixVente).ToString(),
                                        VerticalAlignment = VerticalAlignment.Center,
                                        TextAlignment = TextAlignment.Center
                                    };

                                    // Assign grid positions
                                    Grid.SetColumn(nameBlock, 0);
                                    Grid.SetColumn(priceBlock, 1);
                                    Grid.SetColumn(qtyBlock, 2);
                                    Grid.SetColumn(totalBlock, 3);

                                    // Add text blocks to grid
                                    articleRow.Children.Add(nameBlock);
                                    articleRow.Children.Add(priceBlock);
                                    articleRow.Children.Add(qtyBlock);
                                    articleRow.Children.Add(totalBlock);
                                    Canvas.SetLeft(articleRow, 82);
                                    Canvas.SetTop(articleRow, 634);
                                    articlesContainer.Children.Add(articleRow);
                                }
                            }

                        }
                    }
                    else
                    {
                        Canvas mainCanvas = new Canvas
                        {
                            Height = 1000,
                            Width = 720,
                            Visibility = Visibility.Collapsed,
                            Name = $"Canvas{index}"
                        };

                        // Create Image
                        Image image = new Image
                        {
                            Source = new BitmapImage(new Uri("/Main/images/3.jpg", UriKind.Relative)),
                            Stretch = Stretch.Fill,
                            Height = 1000,
                            Width = 720
                        };
                        mainCanvas.Children.Add(image);

                        // Create StackPanel
                        StackPanel articlesContainer = new StackPanel
                        {
                            Orientation = Orientation.Vertical,
                            Background = new SolidColorBrush(Colors.AliceBlue),
                            Width = 562,
                            Height = 196,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Top,
                            Name = "ArticlesContainer"
                        };

                        // Set StackPanel position on Canvas
                        Canvas.SetLeft(articlesContainer, 82);
                        Canvas.SetTop(articlesContainer, 434);

                        // Add StackPanel to Canvas
                        mainCanvas.Children.Add(articlesContainer);

                        // Set the Canvas as the Window content
                        FacturesContainer.Children.Add(mainCanvas);


                        foreach (var oa in list)
                        {
                            foreach (var a in main.main.laa)
                            {
                                if (a.ArticleID == oa.ArticleID)
                                {
                                    //Load articles in template
                                    Grid articleRow = new Grid
                                    {
                                        Width = 562,
                                        Height = 20,
                                        Background = new SolidColorBrush(Color.FromRgb(235, 245, 255)), // light background
                                        Margin = new Thickness(0, 5, 0, 5)
                                    };

                                    // Define 4 columns
                                    articleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }); // Name
                                    articleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Unit price
                                    articleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Quantité
                                    articleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Total

                                    // Create TextBlocks
                                    TextBlock nameBlock = new TextBlock
                                    {
                                        Text = a.ArticleName,
                                        VerticalAlignment = VerticalAlignment.Center,
                                        Margin = new Thickness(5, 0, 0, 0)
                                    };
                                    TextBlock priceBlock = new TextBlock
                                    {
                                        Text = a.PrixVente.ToString(),
                                        VerticalAlignment = VerticalAlignment.Center,
                                        TextAlignment = TextAlignment.Center
                                    };
                                    TextBlock qtyBlock = new TextBlock
                                    {
                                        Text = oa.QteArticle.ToString(),
                                        VerticalAlignment = VerticalAlignment.Center,
                                        TextAlignment = TextAlignment.Center
                                    };
                                    TextBlock totalBlock = new TextBlock
                                    {
                                        Text = (oa.QteArticle * a.PrixVente).ToString(),
                                        VerticalAlignment = VerticalAlignment.Center,
                                        TextAlignment = TextAlignment.Center
                                    };

                                    // Assign grid positions
                                    Grid.SetColumn(nameBlock, 0);
                                    Grid.SetColumn(priceBlock, 1);
                                    Grid.SetColumn(qtyBlock, 2);
                                    Grid.SetColumn(totalBlock, 3);

                                    // Add text blocks to grid
                                    articleRow.Children.Add(nameBlock);
                                    articleRow.Children.Add(priceBlock);
                                    articleRow.Children.Add(qtyBlock);
                                    articleRow.Children.Add(totalBlock);
                                    Canvas.SetLeft(articleRow, 82);
                                    Canvas.SetTop(articleRow, 434);
                                    articlesContainer.Children.Add(articleRow);
                                }
                            }

                        }
                    }


                }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (PageCounte == TotalPageCount)
            {
                return;
            } 
            PageCounte++;
            PageNbr.Text = PageCounte.ToString();
            foreach (UIElement child in FacturesContainer.Children)
            {
                if (child is Canvas canvas)
                {
                    if (canvas.Name == $"Canvas{PageCounte}")
                    {
                        canvas.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        canvas.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (PageCounte == 1)
            {
                return;
            }
            PageCounte--;
            PageNbr.Text = PageCounte.ToString();
            foreach (UIElement child in FacturesContainer.Children)
            {
                if (child is Canvas canvas)
                {
                    if (canvas.Name == $"Canvas{PageCounte}")
                    {
                        canvas.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        canvas.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }
    }
}
