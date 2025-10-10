using Superete.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Superete
{
    /// <summary>
    /// Logique d'interaction pour Login.xaml
    /// </summary>
    public partial class Login : UserControl
    {
        private StringBuilder passwordBuilder = new StringBuilder();

        public Login(MainWindow main)
        {
            InitializeComponent();
            this.main = main;
            Btn0.Click += NumericButton_Click;
            Btn1.Click += NumericButton_Click;
            Btn2.Click += NumericButton_Click;
            Btn3.Click += NumericButton_Click;
            Btn4.Click += NumericButton_Click;
            Btn5.Click += NumericButton_Click;
            Btn6.Click += NumericButton_Click;
            Btn7.Click += NumericButton_Click;
            Btn8.Click += NumericButton_Click;
            Btn9.Click += NumericButton_Click;
            BtnClear.Click += BtnClear_Click;
            BtnDelete.Click += BtnDelete_Click;
        }
        MainWindow main;

        private void NumericButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Content is string digit)
            {
                passwordBuilder.Append(digit);
                PasswordInput.Password = passwordBuilder.ToString();
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            passwordBuilder.Clear();
            PasswordInput.Password = string.Empty;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (passwordBuilder.Length > 0)
            {
                passwordBuilder.Remove(passwordBuilder.Length - 1, 1);
                PasswordInput.Password = passwordBuilder.ToString();
            }
        }
        private void PasswordInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Only allow digits
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        private void PasswordInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Block spaces
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }

            //// Block Ctrl+V (paste)
            //if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.V)
            //{
            //    e.Handled = true;
            //}
        }
        private async void BtnEnter_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordInput.Password == "")
            {
                MessageBox.Show("write a password");
                return;
            }
            User u = new User();
            List<User> lu=await u.GetUsersAsync();
            foreach (User user in lu)
            {
                if (user.Code == Convert.ToInt32(PasswordInput.Password))
                {

                    main.load_main(user);
                    return;
                }
            }
            PasswordInput.Clear();
            MessageBox.Show("Wrong Code");

        }
    }
}
