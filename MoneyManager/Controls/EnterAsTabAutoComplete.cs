using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MoneyManager.Controls
{
    public class EnterAsTabAutoComplete : AutoCompleteBox
    {
        public bool IgnoreSendTab
        {
            get { return (bool)GetValue(IgnoreSendTabProperty); }
            set { SetValue(IgnoreSendTabProperty, value); }
        }

        public static readonly DependencyProperty IgnoreSendTabProperty =
            DependencyProperty.Register("IgnoreSendTab", typeof(bool), typeof(EnterAsTabAutoComplete));

        public ICommand EnterCommand
        {
            get { return (ICommand)GetValue(EnterCommandProperty); }
            set { SetValue(EnterCommandProperty, value); }
        }
        public static readonly DependencyProperty EnterCommandProperty =
           DependencyProperty.Register("EnterCommand", typeof(ICommand), typeof(EnterAsTabAutoComplete));

        public EnterAsTabAutoComplete()
        {
            this.GotFocus += EnterAsTabAutoComplete_GotFocus;
        }
        public void SetupDefaults(string defaultValue, object itemsSource, object selectedItem)
        {
            this.Height = 26;
            this.MinWidth = 200;
            this.MinimumPrefixLength = 0;
            this.FilterMode = AutoCompleteFilterMode.Contains;
            this.MaxDropDownHeight = 250;
            this.IsTextCompletionEnabled = true;
            this.ItemsSource = itemsSource as System.Collections.IEnumerable;
            this.SelectedItem = selectedItem;
            this.ValueMemberPath = "Name";


            this.Text = defaultValue;
            var highlight = new HighlightTextBlock();
            var dt = new DataTemplate();
           AutoCompleteBoxBehavior.SetIsSelectedItemScrolledIntoView(this, true);

            this.ItemTemplate = FindResource("highlightBoxName") as DataTemplate;
        }
        void EnterAsTabAutoComplete_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            {
                this.IsDropDownOpen = true;
            }

        }


        public static void Send(Key key)
        {
            if (Keyboard.PrimaryDevice != null)
            {
                if (Keyboard.PrimaryDevice.ActiveSource != null)
                {
                    var e = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, key)
                    {
                        RoutedEvent = Keyboard.KeyDownEvent
                    };
                    InputManager.Current.ProcessInput(e);

                    // Note: Based on your requirements you may also need to fire events for:
                    // RoutedEvent = Keyboard.PreviewKeyDownEvent
                    // RoutedEvent = Keyboard.KeyUpEvent
                    // RoutedEvent = Keyboard.PreviewKeyUpEvent
                }
            }
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.Key == Key.Enter)
            {

                if (!IgnoreSendTab)
                {
                    Send(Key.Tab);
                }
                else
                {
                    if (EnterCommand != null) EnterCommand.Execute(null);
                }

            }
            else
            {
                if (string.IsNullOrEmpty(Text))
                {
                    this.IsDropDownOpen = true;
                }
            }
        }

    }
}
