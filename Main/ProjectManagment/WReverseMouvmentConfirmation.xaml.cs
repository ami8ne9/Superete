﻿using System;
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
using System.Windows.Shapes;

namespace Superete.Main.ProjectManagment
{
    /// <summary>
    /// Interaction logic for WReverseMouvmentConfirmation.xaml
    /// </summary>
    public partial class WReverseMouvmentConfirmation : Window
    {
        public WReverseMouvmentConfirmation(CSingleMouvment sm)
        {
            InitializeComponent();
            this.sm = sm;
            foreach (Role r in sm.main.main.lr)
            {
                if (sm.main.u.RoleID == r.RoleID)
                {
                    if (r.ReverseMouvment == false)
                    {
                        ContinueBtn.IsEnabled = false;
                    }
                    break;
                }
            }
        }
        CSingleMouvment sm;
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            sm.opa.Reversed = true;
            sm.opa.UpdateOperationArticleAsync();
            sm.main.LoadOperations(sm.main.main.lo);
            sm.main.LoadMouvments(sm.main.main.loa);
            sm.main.LoadStats();
            this.Close();
        }
    }
}
