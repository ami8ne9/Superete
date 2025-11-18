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
                        if (main.EtatFacture.IsEnabled == true)
                        {
                            if (main.EtatFacture.Text == "Normal")
                            {
                                if (oa.Reversed == true)
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

        private TextBlock CreateTopRightHeader()
        {
            TextBlock header = new TextBlock
            {
                Name = "txtDisplayType",
                Text = GetDictionaryValue("txtDisplayType", ""),
                FontSize = 50,
                Background = new SolidColorBrush(Colors.White),
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                Width = 400,
                Height = 100,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                TextAlignment = TextAlignment.Left
            };

            Canvas.SetLeft(header, 80);
            Canvas.SetTop(header, 50);

            return header;
        }

        public void LoadArticles(List<List<OperationArticle>> pages)
        {
            FacturesContainer.Children.Clear();
            TotalPageCount = pages.Count;
            TotalPageNbr.Text = "/" + TotalPageCount.ToString();

            string[] templates = { "1.png", "2.png", "3.png", "4.png" };

            for (int i = 0; i < pages.Count; i++)
            {
                var currentPage = pages[i];
                string template = GetTemplateForPage(i, pages.Count, templates);
                bool isFirstPage = (i == 0);
                bool isLastPage = (i == pages.Count - 1);
                bool isSinglePage = (pages.Count == 1);

                Canvas mainCanvas = new Canvas
                {
                    Height = 1000,
                    Width = 720,
                    Name = $"Canvas{i + 1}",
                    Visibility = (i == 0) ? Visibility.Visible : Visibility.Collapsed
                };

                Image image = new Image
                {
                    Source = new BitmapImage(new Uri($"/Main/images/{template}", UriKind.Relative)),
                    Stretch = Stretch.Fill,
                    Height = 1000,
                    Width = 720
                };
                mainCanvas.Children.Add(image);

                if (isFirstPage || isSinglePage)
                {
                    TextBlock topRightHeader = CreateTopRightHeader();
                    mainCanvas.Children.Add(topRightHeader);

                    Grid logoContainer = CreateLogoPlaceholder();
                    mainCanvas.Children.Add(logoContainer);

                    Grid headerGrid = CreateHeaderGrid();
                    mainCanvas.Children.Add(headerGrid);

                    PopulateHeaderData(mainCanvas);
                }

                if (isSinglePage || isLastPage)
                {
                    StackPanel summaryPanel = CreateSummaryPanel();
                    mainCanvas.Children.Add(summaryPanel);
                    PopulateSummaryData(summaryPanel);

                    StackPanel descriptionPanel = CreateDescriptionPanel();
                    mainCanvas.Children.Add(descriptionPanel);
                    PopulateDescriptionData(descriptionPanel);
                }

                (double top, double height) = GetStackPanelLayoutForTemplate(template);

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

                foreach (var oa in currentPage)
                {
                    var article = main.main.laa.FirstOrDefault(a => a.ArticleID == oa.ArticleID);
                    double tvaRate = (double)article.tva;

                    articlesContainer.Children.Add(
                        CreateArticleRow(article.ArticleName, (double)article.PrixVente, oa.QteArticle, tvaRate)
                    );
                }

                StackPanel footerPanel = CreateFooterPanel();
                mainCanvas.Children.Add(footerPanel);
                PopulateFooterData(footerPanel);

                FacturesContainer.Children.Add(mainCanvas);
            }
        }

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

            Image logoImage = new Image
            {
                Name = "imgLogo",
                Width = 100,
                Height = 100,
                Stretch = Stretch.Uniform
            };

            string logoPath = GetDictionaryValue("Logo");
            if (!string.IsNullOrEmpty(logoPath))
            {
                try
                {
                    logoImage.Source = new BitmapImage(new Uri(logoPath, UriKind.RelativeOrAbsolute));
                    border.Visibility = Visibility.Collapsed;
                }
                catch
                {
                }
            }

            logoContainer.Children.Add(logoImage);
            return logoContainer;
        }

        private string GetDictionaryValue(string key, string defaultValue = "")
        {
            return FactureInfo.ContainsKey(key) ? FactureInfo[key] : defaultValue;
        }

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
            SetTextBlockValue(canvas, "txtDisplayEtatFacture", GetDictionaryValue("Reversed"));
            SetTextBlockValue(canvas, "txtDisplayDevice", GetDictionaryValue("Device"));
            SetTextBlockValue(canvas, "txtDisplayType", GetDictionaryValue("Type"));
            SetTextBlockValue(canvas, "txtDisplayIndex", GetDictionaryValue("IndexDeFacture"));
        }

        private void PopulateSummaryData(StackPanel panel)
        {
            if (panel.Children.Count >= 6)
            {
                // Prix HT (uses MontantTotal from dictionary) - NO DH
                if (panel.Children[0] is StackPanel sp0 && sp0.Children.Count >= 2 && sp0.Children[1] is TextBlock tb0)
                    tb0.Text = GetDictionaryValue("MontantTotal", "0.00");

                // TVA Rate - NO DH
                if (panel.Children[1] is StackPanel sp1 && sp1.Children.Count >= 2 && sp1.Children[1] is TextBlock tb1)
                    tb1.Text = GetDictionaryValue("TVA", "0.00") + " %";

                // Valeur TVA (uses MontantTVA from dictionary) - NO DH
                if (panel.Children[2] is StackPanel sp2 && sp2.Children.Count >= 2 && sp2.Children[1] is TextBlock tb2)
                    tb2.Text = GetDictionaryValue("MontantTVA", "0.00");

                // Prix TTC (uses MontantApresTVA from dictionary) - NO DH
                if (panel.Children[3] is StackPanel sp3 && sp3.Children.Count >= 2 && sp3.Children[1] is TextBlock tb3)
                    tb3.Text = GetDictionaryValue("MontantApresTVA", "0.00");

                // Remise - KEEPS DH
                if (panel.Children[4] is StackPanel sp4 && sp4.Children.Count >= 2 && sp4.Children[1] is TextBlock tb4)
                    tb4.Text = "- " + GetDictionaryValue("Remise", "0.00") + " DH";

                // Prix Apres Remise (Final Total from dictionary) - NO DH
                if (panel.Children[5] is StackPanel sp5 && sp5.Children.Count >= 2 && sp5.Children[1] is TextBlock tb5)
                    tb5.Text = GetDictionaryValue("MontantApresRemise", "0.00");
            }
        }

        private void PopulateDescriptionData(StackPanel panel)
        {
            string description = GetDictionaryValue("Description");

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

        private void SetTextBlockValue(DependencyObject parent, string name, string value)
        {
            TextBlock textBlock = FindVisualChild<TextBlock>(parent, name);
            if (textBlock != null)
            {
                textBlock.Text = value;
            }
        }

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

            StackPanel rightPanel = new StackPanel { Margin = new Thickness(0) };
            Grid.SetColumn(rightPanel, 1);

            rightPanel.Children.Add(CreateInfoRow("N° Facture : ", "txtDisplayFacture", 90, true));
            rightPanel.Children.Add(CreateInfoRow("Date : ", "txtDisplayDate", 50, true));
            rightPanel.Children.Add(CreateInfoRow("État Facture : ", "txtDisplayEtatFacture", 90, true));
            rightPanel.Children.Add(CreateInfoRow("Device : ", "txtDisplayDevice", 60, true));
            rightPanel.Children.Add(CreateInfoRow("Index : ", "txtDisplayIndex", 55, true));

            headerGrid.Children.Add(rightPanel);

            return headerGrid;
        }

        private StackPanel CreateSummaryPanel()
        {
            StackPanel summaryPanel = new StackPanel
            {
                Background = new SolidColorBrush(Colors.White),
                Width = 180,
                Height = 140,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };

            Canvas.SetLeft(summaryPanel, 470);
            Canvas.SetTop(summaryPanel, 670);

            // Prix HT (Montant Total)
            summaryPanel.Children.Add(CreateSummaryRow("Prix HT :", "0.00 DH", false));

            // TVA Rate
            summaryPanel.Children.Add(CreateSummaryRow("TVA :", "0.00 %", false));

            // Valeur TVA (Montant TVA)
            summaryPanel.Children.Add(CreateSummaryRow("Valeur TVA :", "0.00 DH", false));

            // Prix TTC (Montant Apres TVA)
            summaryPanel.Children.Add(CreateSummaryRow("Prix TTC :", "0.00 DH", false));

            // Remise
            summaryPanel.Children.Add(CreateSummaryRow("Remise :", "- 0.00 DH", false));

            // Prix Apres Remise (Total - bold)
            summaryPanel.Children.Add(CreateSummaryRow("Total :", "0.00 DH", true));

            return summaryPanel;
        }

        private StackPanel CreateSummaryRow(string label, string value, bool isBold)
        {
            StackPanel row = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 5)
            };

            TextBlock labelBlock = new TextBlock
            {
                Text = label,
                FontSize = 13,
                FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal,
                Foreground = new SolidColorBrush(Color.FromRgb(45, 55, 72)),
                Width = 90,
                VerticalAlignment = VerticalAlignment.Center
            };

            TextBlock valueBlock = new TextBlock
            {
                Text = value,
                FontSize = 13,
                FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal,
                Foreground = new SolidColorBrush(Color.FromRgb(45, 55, 72)),
                Width = 90,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Right
            };

            row.Children.Add(labelBlock);
            row.Children.Add(valueBlock);

            return row;
        }

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

            StackPanel secondRow = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            secondRow.Children.Add(CreateFooterInfoSection("Etat Juridique : ", "txtDisplayEtatJuridiqueU", 69));
            secondRow.Children.Add(CreateFooterInfoSection("| Id Societe : ", "txtDisplayIdSocieteU", 57));
            secondRow.Children.Add(CreateFooterInfoSection("| Seige D'entreprise : ", "txtDisplaySeigeU", 100));

            footerPanel.Children.Add(secondRow);

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

        private string GetTemplateForPage(int index, int totalPages, string[] templates)
        {
            if (totalPages == 1) return templates[0];
            if (index == 0) return templates[1];
            if (index == totalPages - 1) return templates[3];
            return templates[2];
        }

        private (double top, double height) GetStackPanelLayoutForTemplate(string template)
        {
            switch (template)
            {
                case "1.png":
                    return (400, 250);
                case "2.png":
                    return (465, 250);
                case "3.png":
                    return (190, 570);
                case "4.png":
                    return (75, 570);
                default:
                    return (100, 700);
            }
        }

        private Grid CreateArticleRow(string name, double price, double qty, double tvaRate)
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
            articleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.8, GridUnitType.Star) });
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
            TextBlock tvaBlock = new TextBlock
            {
                Text = tvaRate.ToString("0.##") + "%",
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
            Grid.SetColumn(tvaBlock, 3);
            Grid.SetColumn(totalBlock, 4);

            articleRow.Children.Add(nameBlock);
            articleRow.Children.Add(priceBlock);
            articleRow.Children.Add(qtyBlock);
            articleRow.Children.Add(tvaBlock);
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

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    FixedDocument fixedDoc = new FixedDocument();
                    int originalPage = PageCounte;

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

                    foreach (Canvas canvas in allCanvases)
                    {
                        canvas.Visibility = (canvas.Name == $"Canvas{originalPage}") ? Visibility.Visible : Visibility.Collapsed;
                    }

                    printDialog.PrintDocument(fixedDoc.DocumentPaginator, $"Facture - {GetDictionaryValue("NFacture")}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Close();
            }
        }
    }
}