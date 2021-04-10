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
using MahApps.Metro.Controls;

namespace VA
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
           
        }

        private void AdminSwitcher()
        {
            if (Login.Text == "Admin")
            {
                r1.IsEnabled = true;
                r2.IsEnabled = true;
                r3.IsEnabled = true;
                r4.IsEnabled = false;
                r5.IsEnabled = true;
            }
            else
            {
                r1.IsEnabled = true;
                r2.IsEnabled = false;
                r3.IsEnabled = false;
                r4.IsEnabled = true;
                r5.IsEnabled = false;
            }
        }
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AdminSwitcher();
        }
    }
}