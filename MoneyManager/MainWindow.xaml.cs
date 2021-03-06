﻿using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Unity;
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

namespace MoneyManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IUnityContainer container;
         
        public MainWindow(IUnityContainer container,IEventAggregator events)
        {
            this.container = container;
            InitializeComponent();
            events.GetEvent<CloseTabEvent>().Subscribe((x) => tabs.Items.Remove(tabs.SelectedItem));
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["logins"] == "YES")
            {
                ShowLogins();
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var vc = container.Resolve<BudgetView>();
            var tabItem = new TabItem();
            tabItem.Header = "Budget";
            tabItem.Content = vc;
            tabs.Items.Add(tabItem);
            tabs.SelectedItem = tabs.Items[tabs.Items.Count - 1];
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {

            var vc = container.Resolve<RegisterView>();
            var tabItem = new TabItem();
            tabItem.Header = "Register";
            tabItem.Content = vc;
            tabs.Items.Add(tabItem);
            tabs.SelectedItem = tabs.Items[tabs.Items.Count - 1];
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            var vc = container.Resolve<CCTransView>();
            var tabItem = new TabItem();
            tabItem.Header = "Credit Card (Chrissi)";
            tabItem.Content = vc;
            tabs.Items.Add(tabItem);
            tabs.SelectedItem = tabs.Items[tabs.Items.Count - 1];
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            var vc = container.Resolve<AnalysisView>();
            var tabItem = new TabItem();
            tabItem.Header = "Analysis";
            tabItem.Content = vc;
            tabs.Items.Add(tabItem);
            tabs.SelectedItem = tabs.Items[tabs.Items.Count - 1];
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            ShowLogins();

        }

        private void ShowLogins()
        {
            var vm = container.Resolve<LoginsView>();
            var pass = new PasswordWindow();
            pass.ShowDialog();
            try
            {
                vm.Load(pass.Password);

                var tabItem = new TabItem();
                tabItem.Header = "Logins";
                tabItem.Content = vm;
                tabs.Items.Add(tabItem);
                tabs.SelectedItem = tabs.Items[tabs.Items.Count - 1];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            var vc = container.Resolve<SavingsView>();
            var tabItem = new TabItem();
            tabItem.Header = "Savings";
            tabItem.Content = vc;
            tabs.Items.Add(tabItem);
            tabs.SelectedItem = tabs.Items[tabs.Items.Count - 1];
        }

        private void MenuItem_Click_6(object sender, RoutedEventArgs e)
        {
            var vc = container.Resolve<CCTransView>();
            (vc.DataContext as CCTransViewVM).Type = 1;
            (vc.DataContext as CCTransViewVM).LoadItems();
            var tabItem = new TabItem();
            tabItem.Header = "Credit Card (David)";
            tabItem.Content = vc;
            tabs.Items.Add(tabItem);
            tabs.SelectedItem = tabs.Items[tabs.Items.Count - 1];
        }
    }
}
