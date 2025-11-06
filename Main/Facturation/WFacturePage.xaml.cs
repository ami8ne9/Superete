using GestionComerce;
using GestionComerce.Main.Facturation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using Microsoft.Win32;
using System.Windows.Markup;

using System.Printing;

namespace Superete.Main.Facturation
{
    public partial class WFacturePage : Window
    {
        public CMainFa main;
        Dictionary<string, string> FactureInfo;
        public WFacturePage(CMainFa main, Dictionary<string, string> FactureInfo)
        {
            InitializeComponent();
            List<List<OperationArticle>> ListListArts = new List<List<OperationArticle>>();
            this.main = main;
            this.FactureInfo = FactureInfo;
            if (main.OperationContainer.Children[0] is CSingleOperation sop)
            {
                List<OperationArticle> ListArts = new List<OperationArticle>();
                bool skip = false;
                foreach (OperationArticle oa in main.main.loa)
                {
                    if (oa.OperationID == sop.op.OperationID)
                    {
                        if(main.EtatFacture.IsEnabled == true )
                        {
                            if(main.EtatFacture.Text == "Normal")
                            {
                                if(oa.Reversed == true)
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                if (oa.Reversed == false)
                                {
                                    continue;
                                }
                            }
                        }
                        ListArts.Add(oa);
                        if (ListArts.Count == 8 && skip == false)
                        {
                            ListListArts.Add(ListArts);
                            ListArts = new List<OperationArticle>();
                            skip = true;
                        } 
                    
                        if (ListArts.Count == 19)
                        {
                            ListListArts.Add(ListArts);
                            ListArts = new List<OperationArticle>();

                        }
                    }
                    
                }
                if (ListArts.Count > 0)
                {
                    ListListArts.Add(ListArts);
                }
            }

            LoadArticles(ListListArts);
        }

        int PageCounte = 1;
        int TotalPageCount = 1;

        public void LoadArticles(List<List<OperationArticle>> pages)
        {
            FacturesContainer.Children.Clear();
            TotalPageCount = pages.Count;
            TotalPageNbr.Text = "/" + TotalPageCount.ToString();

            // Template image map
            string[] templates = { "1.png", "2.png", "3.png", "4.png" };

            for (int i = 0; i < pages.Count; i++)
            {
                var currentPage = pages[i];
                string template = GetTemplateForPage(i, pages.Count, templates);
                bool isFirstPage = (i == 0);
                bool isLastPage = (i == pages.Count - 1);
                bool isSinglePage = (pages.Count == 1);

                // Create canvas
                Canvas mainCanvas = new Canvas
                {
                    Height = 1000,
                    Width = 720,
                    Name = $"Canvas{i + 1}",
                    Visibility = (i == 0) ? Visibility.Visible : Visibility.Collapsed
                };

                // Add background image
                Image image = new Image
                {
                    Source = new BitmapImage(new Uri($"/Main/images/{template}", UriKind.Relative)),
                    Stretch = Stretch.Fill,
                    Height = 1000,
                    Width = 720
                };
                mainCanvas.Children.Add(image);

                // Add first Grid (Header info) - if first page OR single page
                if (isFirstPage || isSinglePage)
                {
                    // Add logo
                    Grid logoContainer = CreateLogoPlaceholder();
                    mainCanvas.Children.Add(logoContainer);

                    Grid headerGrid = CreateHeaderGrid();
                    mainCanvas.Children.Add(headerGrid);

                    // Populate header data after adding to canvas
                    PopulateHeaderData(mainCanvas);
                }

                // Add second StackPanel (Summary info) - if single page OR last page
                if (isSinglePage || isLastPage)
                {
                    StackPanel summaryPanel = CreateSummaryPanel();
                    mainCanvas.Children.Add(summaryPanel);

                    // Populate summary data
                    PopulateSummaryData(summaryPanel);

                    // Add description panel to the left of summary
                    StackPanel descriptionPanel = CreateDescriptionPanel();
                    mainCanvas.Children.Add(descriptionPanel);

                    // Populate description
                    PopulateDescriptionData(descriptionPanel);
                }

                // Determine StackPanel position based on image
                (double top, double height) = GetStackPanelLayoutForTemplate(template);

                // Create StackPanel for articles
                StackPanel articlesContainer = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Background = new SolidColorBrush(Colors.White),
                    Width = 562,
                    Height = height,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top,
                    Name = "ArticlesContainer"
                };

                Canvas.SetLeft(articlesContainer, 82);
                Canvas.SetTop(articlesContainer, top);
                mainCanvas.Children.Add(articlesContainer);

                // Fill articles
                foreach (var oa in currentPage)
                {
                    var article = main.main.laa.FirstOrDefault(a => a.ArticleID == oa.ArticleID);
                    articlesContainer.Children.Add(
                        CreateArticleRow(article.ArticleName, (double)article.PrixVente, oa.QteArticle)
                    );
                }

                // Add footer StackPanel - on ALL pages
                StackPanel footerPanel = CreateFooterPanel();
                mainCanvas.Children.Add(footerPanel);

                // Populate footer data
                PopulateFooterData(footerPanel);

                // Add final canvas
                FacturesContainer.Children.Add(mainCanvas);
            }
        }

        /// <summary>
        /// Creates the logo placeholder
        /// </summary>
        private Grid CreateLogoPlaceholder()
        {
            Grid logoContainer = new Grid
            {
                Width = 100,
                Height = 100,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top
            };

            Canvas.SetRight(logoContainer, 40);
            Canvas.SetTop(logoContainer, 40);

            // Background border with default text
            Border border = new Border
            {
                Name = "logoBorder",
                Width = 100,
                Height = 100,
                Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
                BorderThickness = new Thickness(1)
            };

            TextBlock defaultText = new TextBlock
            {
                Text = "Logo",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(150, 150, 150)),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            border.Child = defaultText;
            logoContainer.Children.Add(border);

            // Image on top (set source when needed)
            Image logoImage = new Image
            {
                Name = "imgLogo",
                Width = 100,
                Height = 100,
                Stretch = Stretch.Uniform
            };

            // Set logo if exists
            string logoPath = GetDictionaryValue("Logo");
            if (!string.IsNullOrEmpty(logoPath))
            {
                try
                {
                    logoImage.Source = new BitmapImage(new Uri(logoPath, UriKind.RelativeOrAbsolute));
                    // Hide the default border when image is loaded
                    border.Visibility = Visibility.Collapsed;
                }
                catch
                {
                    // If loading fails, keep the default "Logo" text visible
                }
            }

            logoContainer.Children.Add(logoImage);

            return logoContainer;
        }

        /// <summary>
        /// Helper method to get value from dictionary with default
        /// </summary>
        private string GetDictionaryValue(string key, string defaultValue = "")
        {
            return FactureInfo.ContainsKey(key) ? FactureInfo[key] : defaultValue;
        }

        /// <summary>
        /// Populates header grid with data from FactureInfo
        /// </summary>
        private void PopulateHeaderData(Canvas canvas)
        {
            SetTextBlockValue(canvas, "txtDisplayNom", GetDictionaryValue("NomC"));
            SetTextBlockValue(canvas, "txtDisplayICE", GetDictionaryValue("ICEC"));
            SetTextBlockValue(canvas, "txtDisplayVAT", GetDictionaryValue("VATC"));
            SetTextBlockValue(canvas, "txtDisplayTelephone", GetDictionaryValue("TelephoneC"));
            SetTextBlockValue(canvas, "txtDisplayEtatJuridique", GetDictionaryValue("EtatJuridiqueC"));
            SetTextBlockValue(canvas, "txtDisplayIdSociete", GetDictionaryValue("IdSocieteC"));
            SetTextBlockValue(canvas, "txtDisplaySiegeSociete", GetDictionaryValue("SiegeEntrepriseC"));
            SetTextBlockValue(canvas, "txtDisplayAdresse", GetDictionaryValue("AdressC"));

            SetTextBlockValue(canvas, "txtDisplayFacture", GetDictionaryValue("NFacture"));
            SetTextBlockValue(canvas, "txtDisplayDate", GetDictionaryValue("Date"));
            SetTextBlockValue(canvas, "txtDisplayEtatFacture", GetDictionaryValue("Reversed")); // FIXED: Now shows EtatFacture from "Reversed" key
            SetTextBlockValue(canvas, "txtDisplayDevice", GetDictionaryValue("Device"));
            SetTextBlockValue(canvas, "txtDisplayType", GetDictionaryValue("Type"));
            SetTextBlockValue(canvas, "txtDisplayIndex", GetDictionaryValue("IndexDeFacture"));
        }

        /// <summary>
        /// Populates summary panel with data from FactureInfo
        /// </summary>
        private void PopulateSummaryData(StackPanel panel)
        {
            if (panel.Children.Count >= 5) // FIXED: Now checking for 5 children (including Remise)
            {
                // Remise (first - at the top) - with minus prefix
                if (panel.Children[0] is TextBlock tb0)
                    tb0.Text = "- " + GetDictionaryValue("Remise", "0.00");

                // Montant Total
                if (panel.Children[1] is TextBlock tb1)
                    tb1.Text = GetDictionaryValue("MontantTotal", "0.00");

                // TVA Rate
                if (panel.Children[2] is TextBlock tb2)
                    tb2.Text = GetDictionaryValue("TVA", "0.00") + " %";

                // Montant TVA
                if (panel.Children[3] is TextBlock tb3)
                    tb3.Text = GetDictionaryValue("MontantTVA", "0.00");

                // Montant Apres TVA
                if (panel.Children[4] is TextBlock tb4)
                    tb4.Text = GetDictionaryValue("MontantApresTVA", "0.00");
            }
        }

        /// <summary>
        /// Populates description panel with data from FactureInfo
        /// </summary>
        private void PopulateDescriptionData(StackPanel panel)
        {
            string description = GetDictionaryValue("Description");

            // Hide the panel if description is empty
            if (string.IsNullOrWhiteSpace(description))
            {
                panel.Visibility = Visibility.Collapsed;
                return;
            }

            foreach (UIElement child in panel.Children)
            {
                if (child is TextBlock tb && tb.Name == "txtDescription")
                {
                    tb.Text = description;
                    break;
                }
            }
        }

        /// <summary>
        /// Populates footer panel with data from FactureInfo (User info)
        /// </summary>
        private void PopulateFooterData(StackPanel panel)
        {
            SetTextBlockValue(panel, "txtDisplayNomU", GetDictionaryValue("NomU"));
            SetTextBlockValue(panel, "txtDisplayICEU", GetDictionaryValue("ICEU"));
            SetTextBlockValue(panel, "txtDisplayVATU", GetDictionaryValue("VATU"));
            SetTextBlockValue(panel, "txtDisplayTelephoneU", GetDictionaryValue("TelephoneU"));
            SetTextBlockValue(panel, "txtDisplayEtatJuridiqueU", GetDictionaryValue("EtatJuridiqueU"));
            SetTextBlockValue(panel, "txtDisplayIdSocieteU", GetDictionaryValue("IdSocieteU"));
            SetTextBlockValue(panel, "txtDisplaySeigeU", GetDictionaryValue("SiegeEntrepriseU"));
            SetTextBlockValue(panel, "txtDisplayAdresseU", GetDictionaryValue("AdressU"));
        }

        /// <summary>
        /// Helper method to set TextBlock value by name
        /// </summary>
        private void SetTextBlockValue(DependencyObject parent, string name, string value)
        {
            TextBlock textBlock = FindVisualChild<TextBlock>(parent, name);
            if (textBlock != null)
            {
                textBlock.Text = value;
            }
        }

        /// <summary>
        /// Helper method to find child controls by name
        /// </summary>
        private T FindVisualChild<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T typedChild && (child as FrameworkElement)?.Name == name)
                {
                    return typedChild;
                }

                var result = FindVisualChild<T>(child, name);
                if (result != null)
                    return result;
            }
            return null;
        }

        /// <summary>
        /// Creates the header Grid with company information
        /// </summary>
        private Grid CreateHeaderGrid()
        {
            Grid headerGrid = new Grid
            {
                Background = new SolidColorBrush(Colors.White),
                Width = 544,
                Height = 170,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Name = "ArticlesContainer"
            };

            Canvas.SetLeft(headerGrid, 91);
            Canvas.SetTop(headerGrid, 173);

            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.88, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Left Column (Client Info)
            StackPanel leftPanel = new StackPanel { Margin = new Thickness(0) };
            Grid.SetColumn(leftPanel, 0);

            leftPanel.Children.Add(CreateInfoRow("Nom : ", "txtDisplayNom", 44));
            leftPanel.Children.Add(CreateInfoRow("ICE : ", "txtDisplayICE", 33));
            leftPanel.Children.Add(CreateInfoRow("VAT : ", "txtDisplayVAT", 41));
            leftPanel.Children.Add(CreateInfoRow("Téléphone : ", "txtDisplayTelephone", 85));
            leftPanel.Children.Add(CreateInfoRow("État Juridique : ", "txtDisplayEtatJuridique", 107));
            leftPanel.Children.Add(CreateInfoRow("ID de Société : ", "txtDisplayIdSociete", 99));
            leftPanel.Children.Add(CreateInfoRow("Siège de Société : ", "txtDisplaySiegeSociete", 122));
            leftPanel.Children.Add(CreateInfoRowWithWrap("Adresse : ", "txtDisplayAdresse", 65, 290));

            headerGrid.Children.Add(leftPanel);

            // Right Column (Invoice Info)
            StackPanel rightPanel = new StackPanel { Margin = new Thickness(0) };
            Grid.SetColumn(rightPanel, 1);

            rightPanel.Children.Add(CreateInfoRow("N° Facture : ", "txtDisplayFacture", 90, true));
            rightPanel.Children.Add(CreateInfoRow("Date : ", "txtDisplayDate", 50, true));
            rightPanel.Children.Add(CreateInfoRow("État Facture : ", "txtDisplayEtatFacture", 90, true));
            rightPanel.Children.Add(CreateInfoRow("Device : ", "txtDisplayDevice", 60, true));
            rightPanel.Children.Add(CreateInfoRow("Type : ", "txtDisplayType", 50, true));
            rightPanel.Children.Add(CreateInfoRow("Index : ", "txtDisplayIndex", 55, true));

            headerGrid.Children.Add(rightPanel);

            return headerGrid;
        }

        /// <summary>
        /// Creates the summary StackPanel
        /// </summary>
        private StackPanel CreateSummaryPanel()
        {
            StackPanel summaryPanel = new StackPanel
            {
                Background = new SolidColorBrush(Colors.White),
                Width = 141,
                Height = 145,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };

            Canvas.SetLeft(summaryPanel, 560);
            Canvas.SetTop(summaryPanel, 670);

            // Remise (first - at the top)
            summaryPanel.Children.Add(new TextBlock
            {
                Text = "0.00 DH",
                FontSize = 14,
                Foreground = new SolidColorBrush(Color.FromRgb(45, 55, 72)),
                Width = 150,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 3)
            });

            // Montant Total
            summaryPanel.Children.Add(new TextBlock
            {
                Text = "0.00 DH",
                FontSize = 14,
                Foreground = new SolidColorBrush(Color.FromRgb(45, 55, 72)),
                Width = 150,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 7)
            });

            // TVA Rate
            summaryPanel.Children.Add(new TextBlock
            {
                Text = "0.00 %",
                FontSize = 14,
                Foreground = new SolidColorBrush(Color.FromRgb(45, 55, 72)),
                Width = 150,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 3)
            });

            // Montant TVA
            summaryPanel.Children.Add(new TextBlock
            {
                Text = "0.00 DH",
                FontSize = 14,
                Foreground = new SolidColorBrush(Color.FromRgb(45, 55, 72)),
                Width = 150,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 3)
            });

            // Montant Apres TVA (Total - bold)
            summaryPanel.Children.Add(new TextBlock
            {
                Text = "0.00 DH",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(45, 55, 72)),
                Width = 150,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 0)
            });

            return summaryPanel;
        }

        /// <summary>
        /// Creates the description StackPanel (appears to the left of summary)
        /// </summary>
        private StackPanel CreateDescriptionPanel()
        {
            StackPanel descriptionPanel = new StackPanel
            {
                Background = new SolidColorBrush(Colors.White),
                Width = 350,
                Height = 240,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };

            Canvas.SetLeft(descriptionPanel, 82);
            Canvas.SetTop(descriptionPanel, 670);

            descriptionPanel.Children.Add(new TextBlock
            {
                Text = "Description : ",
                FontWeight = FontWeights.Bold,
                FontSize = 15,
                Foreground = new SolidColorBrush(Colors.Black),
                Width = 350,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(5)
            });

            descriptionPanel.Children.Add(new TextBlock
            {
                Name = "txtDescription",
                Text = "",
                FontSize = 12,
                Foreground = new SolidColorBrush(Colors.Black),
                Width = 350,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(5)
            });

            return descriptionPanel;
        }

        /// <summary>
        /// Creates the footer StackPanel (appears on all pages)
        /// </summary>
        private StackPanel CreateFooterPanel()
        {
            StackPanel footerPanel = new StackPanel
            {
                Background = new SolidColorBrush(Colors.White),
                Width = 642,
                Height = 59,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            Canvas.SetLeft(footerPanel, 49);
            Canvas.SetTop(footerPanel, 940);

            // First Row
            StackPanel firstRow = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            firstRow.Children.Add(CreateFooterInfoSection("Nom : ", "txtDisplayNomU", 30));
            firstRow.Children.Add(CreateFooterInfoSection("| ICE : ", "txtDisplayICEU", 25));
            firstRow.Children.Add(CreateFooterInfoSection("| VAT : ", "txtDisplayVATU", 23));
            firstRow.Children.Add(CreateFooterInfoSection("| Telephone : ", "txtDisplayTelephoneU", 55));

            footerPanel.Children.Add(firstRow);

            // Second Row
            StackPanel secondRow = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            secondRow.Children.Add(CreateFooterInfoSection("Etat Juridique : ", "txtDisplayEtatJuridiqueU", 69));
            secondRow.Children.Add(CreateFooterInfoSection("| Id Societe : ", "txtDisplayIdSocieteU", 57));
            secondRow.Children.Add(CreateFooterInfoSection("| Seige D'entreprise : ", "txtDisplaySeigeU", 100));

            footerPanel.Children.Add(secondRow);

            // Third Row (Address)
            StackPanel thirdRow = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top
            };

            TextBlock addressLabel = new TextBlock
            {
                Text = "Adresse : ",
                FontSize = 10,
                Foreground = new SolidColorBrush(Color.FromRgb(45, 55, 72)),
                Width = 42,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0)
            };

            TextBlock addressValue = new TextBlock
            {
                Name = "txtDisplayAdresseU",
                Text = "",
                FontSize = 10,
                Foreground = new SolidColorBrush(Color.FromRgb(45, 55, 72)),
                VerticalAlignment = VerticalAlignment.Top,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0),
                MaxWidth = 600,
                MaxHeight = 20,
                TextTrimming = TextTrimming.CharacterEllipsis
            };

            thirdRow.Children.Add(addressLabel);
            thirdRow.Children.Add(addressValue);

            footerPanel.Children.Add(thirdRow);

            return footerPanel;
        }

        /// <summary>
        /// Helper to create info rows
        /// </summary>
        private StackPanel CreateInfoRow(string label, string textBlockName, double labelWidth, bool wrap = false)
        {
            StackPanel sp = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0)
            };

            sp.Children.Add(new TextBlock
            {
                Text = label,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(45, 55, 72)),
                Width = labelWidth,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0)
            });

            TextBlock valueBlock = new TextBlock
            {
                Name = textBlockName,
                Text = "",
                FontSize = 14,
                Foreground = new SolidColorBrush(Color.FromRgb(45, 55, 72)),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0),
                TextTrimming = TextTrimming.CharacterEllipsis
            };

            if (wrap)
            {
                valueBlock.TextWrapping = TextWrapping.Wrap;
                valueBlock.MaxWidth = 150;
            }

            sp.Children.Add(valueBlock);

            return sp;
        }

        /// <summary>
        /// Helper to create info rows with text wrapping and width
        /// </summary>
        private StackPanel CreateInfoRowWithWrap(string label, string textBlockName, double labelWidth, double valueWidth)
        {
            StackPanel sp = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0),
                VerticalAlignment = VerticalAlignment.Top
            };

            sp.Children.Add(new TextBlock
            {
                Text = label,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(45, 55, 72)),
                Width = labelWidth,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0)
            });

            sp.Children.Add(new TextBlock
            {
                Name = textBlockName,
                Text = "",
                FontSize = 14,
                Foreground = new SolidColorBrush(Color.FromRgb(45, 55, 72)),
                VerticalAlignment = VerticalAlignment.Top,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0),
                Width = valueWidth,
                MaxHeight = 60,
                TextTrimming = TextTrimming.CharacterEllipsis
            });

            return sp;
        }

        /// <summary>
        /// Helper to create footer info sections
        /// </summary>
        private StackPanel CreateFooterInfoSection(string label, string textBlockName, double labelWidth)
        {
            StackPanel sp = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0)
            };

            sp.Children.Add(new TextBlock
            {
                Text = label,
                FontSize = 10,
                Foreground = new SolidColorBrush(Color.FromRgb(45, 55, 72)),
                Width = labelWidth,
                VerticalAlignment = VerticalAlignment.Center
            });

            sp.Children.Add(new TextBlock
            {
                Name = textBlockName,
                FontSize = 10,
                Foreground = new SolidColorBrush(Color.FromRgb(45, 55, 72)),
                VerticalAlignment = VerticalAlignment.Center,
                TextTrimming = TextTrimming.CharacterEllipsis,
                MaxWidth = 150
            });

            return sp;
        }

        /// <summary>
        /// Chooses which facture image template to use.
        /// </summary>
        private string GetTemplateForPage(int index, int totalPages, string[] templates)
        {
            if (totalPages == 1) return templates[0];
            if (index == 0) return templates[1];
            if (index == totalPages - 1) return templates[3];
            return templates[2];
        }

        /// <summary>
        /// Returns (top, height) of StackPanel depending on facture image template.
        /// </summary>
        private (double top, double height) GetStackPanelLayoutForTemplate(string template)
        {
            switch (template)
            {
                case "1.png": // Single page facture
                    return (400, 250);
                case "2.png": // First page
                    return (465, 250);
                case "3.png": // Middle page
                    return (190, 570);
                case "4.png": // Last page
                    return (75, 570);
                default:
                    return (100, 700);
            }
        }

        /// <summary>
        /// Creates a single article row.
        /// </summary>
        private Grid CreateArticleRow(string name, double price, double qty)
        {
            Grid articleRow = new Grid
            {
                Width = 562,
                Height = 20,
                Margin = new Thickness(0, 5, 0, 5)
            };

            articleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            articleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            articleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            articleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            TextBlock nameBlock = new TextBlock
            {
                Text = name,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5, 0, 0, 0)
            };
            TextBlock priceBlock = new TextBlock
            {
                Text = price.ToString("0.00"),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center
            };
            TextBlock qtyBlock = new TextBlock
            {
                Text = qty.ToString("0.##"),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center
            };
            TextBlock totalBlock = new TextBlock
            {
                Text = (qty * price).ToString("0.00"),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center
            };

            Grid.SetColumn(nameBlock, 0);
            Grid.SetColumn(priceBlock, 1);
            Grid.SetColumn(qtyBlock, 2);
            Grid.SetColumn(totalBlock, 3);

            articleRow.Children.Add(nameBlock);
            articleRow.Children.Add(priceBlock);
            articleRow.Children.Add(qtyBlock);
            articleRow.Children.Add(totalBlock);

            return articleRow;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (PageCounte == TotalPageCount)
                return;

            PageCounte++;
            PageNbr.Text = PageCounte.ToString();

            foreach (UIElement child in FacturesContainer.Children)
            {
                if (child is Canvas canvas)
                    canvas.Visibility = (canvas.Name == $"Canvas{PageCounte}") ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (PageCounte == 1)
                return;

            PageCounte--;
            PageNbr.Text = PageCounte.ToString();

            foreach (UIElement child in FacturesContainer.Children)
            {
                if (child is Canvas canvas)
                    canvas.Visibility = (canvas.Name == $"Canvas{PageCounte}") ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        // Replace these methods in your WFacturePage class:

        // COMPLETE REPLACEMENT - Copy this entire code

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    FixedDocument fixedDoc = new FixedDocument();
                    int originalPage = PageCounte;

                    // Make all canvases visible temporarily and force layout
                    List<Canvas> allCanvases = new List<Canvas>();
                    foreach (UIElement child in FacturesContainer.Children)
                    {
                        if (child is Canvas c)
                        {
                            allCanvases.Add(c);
                            c.Visibility = Visibility.Visible;
                        }
                    }

                    FacturesContainer.UpdateLayout();
                    this.UpdateLayout();
                    Application.Current.Dispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Render);

                    // Render each canvas
                    foreach (Canvas canvas in allCanvases)
                    {
                        FixedPage fixedPage = new FixedPage
                        {
                            Width = 720,
                            Height = 1000,
                            Background = Brushes.White
                        };

                        canvas.Measure(new Size(720, 1000));
                        canvas.Arrange(new Rect(0, 0, 720, 1000));
                        canvas.UpdateLayout();

                        RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                            720, 1000, 96d, 96d, PixelFormats.Pbgra32);
                        renderBitmap.Render(canvas);

                        Image img = new Image
                        {
                            Source = renderBitmap,
                            Width = 720,
                            Height = 1000,
                            Stretch = Stretch.Fill
                        };
                        fixedPage.Children.Add(img);

                        PageContent pageContent = new PageContent();
                        ((IAddChild)pageContent).AddChild(fixedPage);
                        fixedDoc.Pages.Add(pageContent);
                    }

                    // Restore visibility
                    foreach (Canvas canvas in allCanvases)
                    {
                        canvas.Visibility = (canvas.Name == $"Canvas{originalPage}") ? Visibility.Visible : Visibility.Collapsed;
                    }

                    // Print document
                    printDialog.PrintDocument(fixedDoc.DocumentPaginator, $"Facture - {GetDictionaryValue("NFacture")}");
                }
            }
            catch (Exception ex)
            {
                // Optional: log or show error if needed
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Close the window no matter what
                this.Close();
            }
        }



    }
}