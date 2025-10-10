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
using System.Xml.Linq;

namespace Superete.Main.ProjectManagment
{
    /// <summary>
    /// Interaction logic for CMainP.xaml
    /// </summary>
    public partial class CMainP : UserControl
    {
        public CMainP(User u, MainWindow main)
        {
            InitializeComponent();
            this.u = u;
            this.main = main;

            // ✅ Set default filter selections
            TypeOperationFilter.SelectedIndex = 0;
            StatusFilter.SelectedIndex = 0;
            DateFilter.SelectedIndex = 0;
            SearchTypeMouvmentFilter.SelectedIndex = 0;
            TypeMouvmentFilter.SelectedIndex = 0;
            StatusMouvmenttFilter.SelectedIndex = 0;
            StatusMouvmentReversedFilter.SelectedIndex = 0;
            DateMouvmentFilter.SelectedIndex = 0;

            // ✅ Reset counters
            VenteCount = 0;
            AchatCount = 0;
            MouvmentVenteCount = 0;
            MouvmentAchatCount = 0;
            MouvmentFinanceCount = 0;
            Finance = 0;

            LoadStats();

            foreach (Role r in main.lr)
            {
                if (u.RoleID == r.RoleID)
                {
                    if (r.ViewOperation == false)
                    {
                        OperationsButton.IsEnabled = false;
                        OperationsContent.Visibility = Visibility.Collapsed;
                        MouvmentContent.Visibility = Visibility.Visible;
                        OperationsButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280"));
                        OperationsButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
                        StockButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E90FF"));
                        StockButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E90FF"));
                    }
                    break;
                }
            }

            foreach (Role r in main.lr)
            {
                if (u.RoleID == r.RoleID)
                {
                    if (r.ViewMouvment == false)
                    {
                        StockButton.IsEnabled = false;
                        MouvmentContent.Visibility = Visibility.Collapsed;
                    }
                    break;
                }
            }

            // ✅ Load data
            LoadOperations(main.lo);
            LoadMouvments(main.loa);
        }

        public User u; public MainWindow main; public int VenteCount; public int AchatCount; public Decimal Finance; public Decimal MouvmentVenteCount; public Decimal MouvmentAchatCount; public Decimal MouvmentFinanceCount;public int index=10; public int index2 = 10;
        public void LoadOperations(List<Operation> lo)
        {
            int i = 1;
            OperationsContainer.Children.Clear();
            foreach (Operation operation in lo)
            {
                if (i > index) break;
                i++;
                CSingleOperation wSingleOperation = new CSingleOperation(this, operation);
                OperationsContainer.Children.Add(wSingleOperation);
            }

        }
        public void LoadMouvments(List<OperationArticle> loa)
        {
            int i = 1;
            MouvmentContainer.Children.Clear();
            foreach (OperationArticle operationA in loa)
            {
                if (i > index2) break;
                i++;
                CSingleMouvment wSingleMouvment = new CSingleMouvment(this, operationA);
                MouvmentContainer.Children.Add(wSingleMouvment);
            }

        }
        public void LoadStats()
        {
            VenteCount = 0;
            AchatCount = 0;
            Finance = 0;
            MouvmentVenteCount = 0;
            MouvmentAchatCount = 0;
            MouvmentFinanceCount = 0;
            foreach (Operation operation in main.lo)
            {
                if (operation.Reversed) continue; // skip reversed ops

                if (operation.OperationType.StartsWith("V", StringComparison.OrdinalIgnoreCase))
                {
                    VenteCount++;
                    Finance += operation.PrixOperation;

                    // ✅ Only articles linked to this operation
                    foreach (OperationArticle oa in main.loa.Where(x => x.OperationID == operation.OperationID && !x.Reversed))
                    {
                        Article article = main.la.FirstOrDefault(a => a.ArticleID == oa.ArticleID);
                        if (article != null)
                        {
                            MouvmentFinanceCount += article.PrixVente * oa.QteArticle;
                            MouvmentVenteCount++;
                        }
                    }
                }
                else if (operation.OperationType.StartsWith("A", StringComparison.OrdinalIgnoreCase))
                {
                    AchatCount++;
                    Finance -= operation.PrixOperation;

                    foreach (OperationArticle oa in main.loa.Where(x => x.OperationID == operation.OperationID && !x.Reversed))
                    {
                        Article article = main.la.FirstOrDefault(a => a.ArticleID == oa.ArticleID);
                        if (article != null)
                        {
                            MouvmentFinanceCount -= article.PrixAchat * oa.QteArticle;
                            MouvmentAchatCount++;
                        }
                    }
                }
            }
            // ✅ Update UI labels once at the end
            VenteCountLabel.Text = VenteCount.ToString("0");
            AchatCountLabel.Text = AchatCount.ToString("0");
            FinanceLabel.Text = Finance.ToString("0.00") + " DH";
            MouvmentVente.Text = MouvmentVenteCount.ToString("0");
            MouvmentAchat.Text = MouvmentAchatCount.ToString("0");
            MouvmentFinance.Text = MouvmentFinanceCount.ToString("0.00") + " DH";
        }
        private void RetourButton_Click(object sender, RoutedEventArgs e)
        {
            main.load_main(u);
        }


        private void ToutButton_Click(object sender, RoutedEventArgs e)
        {
            SearchInput.Text = "";

            TypeOperationFilter.SelectedIndex = 0;
            StatusFilter.SelectedIndex = 0;
            DateFilter.SelectedIndex = 0;
            SearchTypeFilter.SelectedIndex = 0;

            LoadOperations(main.lo);
        }

        private void OperationsButton_Click(object sender, RoutedEventArgs e)
        {
            OperationsContent.Visibility = Visibility.Visible;
            MouvmentContent.Visibility = Visibility.Collapsed;
            RapportContent.Visibility = Visibility.Collapsed;
            OperationsButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E90FF"));
            OperationsButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E90FF"));
            StockButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280"));
            StockButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
            Repportbtn.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280"));
            Repportbtn.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
        }

        private void StockButton_Click(object sender, RoutedEventArgs e)
        {
            OperationsContent.Visibility = Visibility.Collapsed;
            MouvmentContent.Visibility = Visibility.Visible;
            RapportContent.Visibility = Visibility.Collapsed;
            OperationsButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280"));
            OperationsButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
            StockButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E90FF"));
            StockButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E90FF"));
            Repportbtn.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280"));
            Repportbtn.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
        }
        private void RepportButton_Click(object sender, RoutedEventArgs e)
        {

            OperationsContent.Visibility = Visibility.Collapsed;
            MouvmentContent.Visibility = Visibility.Collapsed;
            RapportContent.Visibility = Visibility.Visible;
            Repportbtn.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E90FF"));
            Repportbtn.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E90FF"));
            StockButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280"));
            StockButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
            OperationsButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280"));
            OperationsButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
        }
        private void SearchInput_TextChanged(object sender, TextChangedEventArgs e)
    => ApplyFilters();

        private void TypeOperationFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => ApplyFilters();

        private void StatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => ApplyFilters();

        private void DateFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => ApplyFilters();

        private void ApplyFilters()
        {
            IEnumerable<Operation> lo = main.lo; // start with all operations

            // ✅ Search filter
            string searchText = SearchInput.Text?.Trim() ?? "";
            string searchType = (SearchTypeFilter.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (!string.IsNullOrEmpty(searchText))
            {
                if (searchType == "Operation ID")
                {
                    lo = lo.Where(op => op.OperationID.ToString().Contains(searchText));
                }
                else if (searchType == "Client")
                {
                    lo = lo.Where(op => main.lc.Any(c => c.ClientID == op.ClientID && c.Nom.Contains(searchText)));
                }
                else if (searchType == "Fournisseur")
                {
                    lo = lo.Where(op => main.lfo.Any(f => f.FournisseurID == op.FournisseurID && f.Nom.Contains(searchText)));
                }
                else if (searchType == "Utilisateur")
                {
                    lo = lo.Where(op => main.lu.Any(u => u.UserID == op.UserID && u.UserName.Contains(searchText)));
                }
            }

            // ✅ Type filter
            string type = (TypeOperationFilter.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (type == "Vente") lo = lo.Where(op => op.OperationType.StartsWith("V"));
            else if (type == "Achat") lo = lo.Where(op => op.OperationType.StartsWith("A"));
            else if (type == "Modification") lo = lo.Where(op => op.OperationType.StartsWith("M"));
            else if (type == "Suppression") lo = lo.Where(op => op.OperationType.StartsWith("D"));
            else if (type == "Payment Credit Client") lo = lo.Where(op =>  op.OperationType.StartsWith("P"));
            else if (type == "Payment Credit Fournisseur") lo = lo.Where(op => op.OperationType.StartsWith("S"));
            // "Tout" = skip

            // ✅ Status filter
            string status = (StatusFilter.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (status == "Normal") lo = lo.Where(op => !op.Reversed);
            else if (status == "Reversed") lo = lo.Where(op => op.Reversed);
            // "Tout" = skip

            // ✅ Date filter
            string dateFilter = (DateFilter.SelectedItem as ComboBoxItem)?.Content.ToString();
            DateTime today = DateTime.Today;

            if (dateFilter == "Today") lo = lo.Where(op => op.DateOperation > today.AddDays(-1));
            else if (dateFilter == "Week") lo = lo.Where(op => op.DateOperation > today.AddDays(-7));
            else if (dateFilter == "Month") lo = lo.Where(op => op.DateOperation > today.AddMonths(-1));
            else if (dateFilter == "6 month") lo = lo.Where(op => op.DateOperation > today.AddMonths(-6));
            else if (dateFilter == "Year") lo = lo.Where(op => op.DateOperation > today.AddYears(-1));
            // "Tout" = skip

            // ✅ Finally load
            LoadOperations(lo.ToList());
        }

        private void ApplyMouvmentFilters()
        {
            // Join OperationArticles with Operations (so we can filter directly)
            var query =
                from oa in main.loa
                join o in main.lo on oa.OperationID equals o.OperationID
                select new { OA = oa, O = o };

            // ✅ Search filter
            string searchText = SearchMouvmentInput.Text?.Trim() ?? "";
            string searchType = (SearchTypeMouvmentFilter.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (!string.IsNullOrEmpty(searchText))
            {
                string lowerSearch = searchText.ToLower();

                if (searchType == "Article")
                {
                    query = from q in query
                            join a in main.laa on q.OA.ArticleID equals a.ArticleID
                            where a.ArticleName != null && a.ArticleName.ToLower().Contains(lowerSearch)
                            select q;
                }
                else if (searchType == "Client")
                {
                    query = from q in query
                            join c in main.lc on q.O.ClientID equals c.ClientID
                            where c.Nom != null && c.Nom.ToLower().Contains(lowerSearch)
                            select q;
                }
                else if (searchType == "Fournisseur")
                {
                    query = from q in query
                            join f in main.lfo on q.O.FournisseurID equals f.FournisseurID
                            where f.Nom != null && f.Nom.ToLower().Contains(lowerSearch)
                            select q;
                }
                else if (searchType == "Utilisateur")
                {
                    query = from q in query
                            join u in main.lu on q.O.UserID equals u.UserID
                            where u.UserName != null && u.UserName.ToLower().Contains(lowerSearch)
                            select q;
                }
            }

            // ✅ Type filter
            string type = (TypeMouvmentFilter.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (type == "Vente") query = query.Where(q => q.O.OperationType.StartsWith("V", StringComparison.OrdinalIgnoreCase));
            else if (type == "Achat") query = query.Where(q => q.O.OperationType.StartsWith("A", StringComparison.OrdinalIgnoreCase));
            else if (type == "Modification") query = query.Where(q => q.O.OperationType.StartsWith("M", StringComparison.OrdinalIgnoreCase));
            else if (type == "Suppression") query = query.Where(q => q.O.OperationType.StartsWith("D", StringComparison.OrdinalIgnoreCase));
            // "Tout" = skip

            // ✅ Status filter
            string status = (StatusMouvmentReversedFilter.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (status == "Normal") query = query.Where(q => !q.OA.Reversed);
            else if (status == "Reversed") query = query.Where(q => q.OA.Reversed);
            // "Tout" = skip

            string status1 = (StatusMouvmenttFilter.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (status1 == "Normal")
            {
                query = query.Where(oa => main.laa.Any(a => a.ArticleID == oa.OA.ArticleID && a.Etat == true));
            }

            else if (status1 == "Supprime")
            {
                query = query.Where(oa => main.laa.Any(a => a.ArticleID == oa.OA.ArticleID && a.Etat == false));
            }

            // ✅ Date filter
            string dateFilter = (DateMouvmentFilter.SelectedItem as ComboBoxItem)?.Content.ToString();
            DateTime today = DateTime.Today;

            if (dateFilter == "Today") query = query.Where(q => q.O.DateOperation > today.AddDays(-1));
            else if (dateFilter == "Week") query = query.Where(q => q.O.DateOperation > today.AddDays(-7));
            else if (dateFilter == "Month") query = query.Where(q => q.O.DateOperation > today.AddMonths(-1));
            else if (dateFilter == "6 month") query = query.Where(q => q.O.DateOperation > today.AddMonths(-6));
            else if (dateFilter == "Year") query = query.Where(q => q.O.DateOperation > today.AddYears(-1));
            // "Tout" = skip

            // ✅ Finally load (only the OA back)
            LoadMouvments(query.Select(q => q.OA).ToList());
        }


        private void SearchMouvmentInput_TextChanged(object sender, TextChangedEventArgs e)
            => ApplyMouvmentFilters();

        private void TypeMouvmentFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        => ApplyMouvmentFilters();

        private void DateMouvmentFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        => ApplyMouvmentFilters();

        private void StatusMouvmentFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        => ApplyMouvmentFilters();

        private void ReversedMouvmentFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        => ApplyMouvmentFilters();
        private void ToutMouvmentButton_Click(object sender, RoutedEventArgs e)
        {
            SearchMouvmentInput.Text = "";

            TypeMouvmentFilter.SelectedIndex = 0;
            StatusMouvmenttFilter.SelectedIndex = 0;
            StatusMouvmentReversedFilter.SelectedIndex = 0;
            SearchTypeFilter.SelectedIndex = 0;

            LoadOperations(main.lo);
        }

        private void SearchTypeFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            index = index + 10;
            ApplyFilters();
        }
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            index2 = index2 + 10;
            ApplyMouvmentFilters();
        }

        //private void LoadFilters()
        //{
        //    // Load months
        //    for (int m = 1; m <= 12; m++) MonthFilter.Items.Add(m);
        //    MonthFilter.SelectedIndex = DateTime.Now.Month - 1;

        //    // Load years
        //    for (int y = 2020; y <= DateTime.Now.Year; y++) YearFilter.Items.Add(y);
        //    YearFilter.SelectedItem = DateTime.Now.Year;

        //    // TODO: Load Users from DB
        //    UserFilter.Items.Add("All Users");
        //    UserFilter.SelectedIndex = 0;
        //}

        //private async void GetStats_Click(object sender, RoutedEventArgs e)
        //{
        //    var operations = await new Operation().GetOperationsAsync();
        //    var opArticles = await new OperationArticle().GetOperationArticlesAsync();

        //    // Apply filters
        //    DateTime? selectedDate = DatePickerFilter.SelectedDate;
        //    int? selectedMonth = MonthFilter.SelectedItem as int?;
        //    int? selectedYear = YearFilter.SelectedItem as int?;

        //    var filteredOps = operations.Where(o => o.Etat);

        //    if (selectedDate.HasValue)
        //        filteredOps = filteredOps.Where(o => o.DateOperation.Date == selectedDate.Value.Date);

        //    if (selectedMonth.HasValue)
        //        filteredOps = filteredOps.Where(o => o.DateOperation.Month == selectedMonth.Value);

        //    if (selectedYear.HasValue)
        //        filteredOps = filteredOps.Where(o => o.DateOperation.Year == selectedYear.Value);

        //    // ===== Method 1: Simple Operation-based =====
        //    decimal totalSpendOp = filteredOps
        //        .Where(o => o.FournisseurID != null || o.OperationType == "Purchase")
        //        .Sum(o => o.PrixOperation);

        //    decimal totalProfitOp = filteredOps
        //        .Where(o => o.ClientID != null || o.OperationType == "Sale")
        //        .Sum(o => o.PrixOperation - o.Remise);

        //    TxtOperationSpend.Text = $"Spend (Ops): {totalSpendOp:C}";
        //    TxtOperationProfit.Text = $"Profit (Ops): {totalProfitOp:C}";
        //    TxtOperationCount.Text = $"Operations Count: {filteredOps.Count()}";

        //    // ===== Method 2: OperationArticle-based =====
        //    // ⚠️ You need to join Article table to get PrixVente & PrixAchat
        //    // For now let's assume you already have a method GetArticlePrice(id)
        //    decimal totalSpendArt = 0;
        //    decimal totalProfitArt = 0;

        //    foreach (var oa in opArticles.Where(a => filteredOps.Any(o => o.OperationID == a.OperationID)))
        //    {
        //        decimal prixVente = GetPrixVente(oa.ArticleID); // TODO: replace with DB call
        //        decimal prixAchat = GetPrixAchat(oa.ArticleID); // TODO: replace with DB call

        //        var operation = filteredOps.First(o => o.OperationID == oa.OperationID);
        //        decimal remise = operation.Remise;

        //        decimal revenue = (prixVente * oa.QteArticle) - remise;
        //        decimal cost = prixAchat * oa.QteArticle;

        //        totalSpendArt += cost;
        //        totalProfitArt += (revenue - cost);
        //    }

        //    TxtArticleSpend.Text = $"Spend (Articles): {totalSpendArt:C}";
        //    TxtArticleProfit.Text = $"Profit (Articles): {totalProfitArt:C}";
        //}

        //// Dummy methods (replace with actual Article class methods/DB)
        //private decimal GetPrixVente(int articleId)
        //{
        //    return 100; // Example value
        //}

        //private decimal GetPrixAchat(int articleId)
        //{
        //    return 70; // Example value
        //}

        private void UserFilter_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
        // Reports logic
        private void DayButton_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedButton(DayButton);
            ShowView(DayView);
        }

        private void MonthButton_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedButton(MonthButton);
            ShowView(MonthView);
            PopulateYearComboBox(MonthYearComboBox);
        }

        private void YearButton_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedButton(YearButton);
            ShowView(YearView);
            PopulateYearComboBox(YearComboBox);
        }

        private void CustomButton_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedButton(CustomButton);
            ShowView(CustomView);
        }

        private void SetSelectedButton(Button selected)
        {
            // Reset all buttons
            DayButton.Style = (Style)FindResource("FilterButtonStyle");
            MonthButton.Style = (Style)FindResource("FilterButtonStyle");
            YearButton.Style = (Style)FindResource("FilterButtonStyle");
            CustomButton.Style = (Style)FindResource("FilterButtonStyle");

            // Set selected button
            selected.Style = (Style)FindResource("SelectedFilterButtonStyle");
        }

        private void ShowView(FrameworkElement viewToShow)
        {
            DayView.Visibility = Visibility.Collapsed;
            MonthView.Visibility = Visibility.Collapsed;
            YearView.Visibility = Visibility.Collapsed;
            CustomView.Visibility = Visibility.Collapsed;

            viewToShow.Visibility = Visibility.Visible;
        }

        private void PopulateYearComboBox(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            int currentYear = DateTime.Now.Year;
            for (int i = currentYear; i >= currentYear - 10; i--)
            {
                comboBox.Items.Add(i);
            }
            comboBox.SelectedIndex = 0;
        }

        private void DatePicker_Changed(object sender, EventArgs e)
        {
            // Load statistics based on selected date range
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            // Your logic to load and display statistics
        }

    }

}