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

namespace Superete.Main.Inventory
{
    /// <summary>
    /// Interaction logic for WAddFamille.xaml
    /// </summary>
    public partial class WAddFamille : Window
    {
        public WAddFamille(List<Famille> lf,WAddArticle ar,Famille f,WManageFamillies mf,int ww)
        {
            InitializeComponent();
            this.lf = lf;
            this.ar = ar;
            this.f = f;
            this.mf = mf;
            this.ww = ww;
            if (f.FamilleID != 0)
            {
                Header.Text = "Update Familly";
                AddButton.Content = "Update";
                FamillyName.Text = f.FamilleName;    
            }
        }
        List<Famille> lf; WAddArticle ar;Famille f; WManageFamillies mf;int ww;
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if(FamillyName.Text.Length==0)
            {
                MessageBox.Show("Le nom de la famille ne peut pas être vide.");
                return;
            }
            if(lf.Where(x=>x.FamilleName.ToLower()==FamillyName.Text.ToLower() && x.FamilleID!=f.FamilleID).ToList().Count>0)
            {
                MessageBox.Show("Une famille avec ce nom existe déjà.");
                return;
            }
            if (f.FamilleID == 0)
            {
                Famille ff = new Famille();
                ff.FamilleName = FamillyName.Text;
                ff.NbrArticle = 0;
                int id = await ff.InsertFamilleAsync();
                f.FamilleID = id;
                lf.Add(ff);
                if (ww == 1)
                {
                    mf.LoadFamillies(lf);
                }
                else
                {
                    ar.LoadFamillies(lf);
                }
                    
                this.Close();
            }
            else
            {
                f.FamilleName = FamillyName.Text;
                f.UpdateFamilleAsync();
                foreach(Famille fa in lf)
                {
                    if(fa.FamilleID==f.FamilleID)
                    {
                        fa.FamilleName = f.FamilleName;
                        break;
                    }
                }
                mf.LoadFamillies(lf);
                this.Close();
            }
            
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
