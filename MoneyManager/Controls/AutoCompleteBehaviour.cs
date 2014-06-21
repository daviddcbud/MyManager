using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MoneyManager.Controls
{


    public static class AutoCompleteBoxBehavior
    {
        #region IsSelectedItemScrolledIntoView

        public static bool GetIsSelectedItemScrolledIntoView(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsSelectedItemScrolledIntoViewProperty);
        }

        public static void SetIsSelectedItemScrolledIntoView(DependencyObject obj, bool value)
        {
            obj.SetValue(IsSelectedItemScrolledIntoViewProperty, value);
        }

        public static readonly DependencyProperty IsSelectedItemScrolledIntoViewProperty =
            DependencyProperty.RegisterAttached("IsSelectedItemScrolledIntoView",
                                                 typeof(bool),
                                                 typeof(AutoCompleteBoxBehavior),
                                                 new UIPropertyMetadata(false, OnIsSelectedItemScrolledIntoViewChanged));

        static void OnIsSelectedItemScrolledIntoViewChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteBox autoCompleteBox = obj as AutoCompleteBox;

            if (autoCompleteBox == null)
                return;

            if (e.NewValue is bool == false)
                return;

            if ((bool)e.NewValue)
                autoCompleteBox.SelectionChanged += new SelectionChangedEventHandler(OnSelectionChanged);
            else
                autoCompleteBox.SelectionChanged -= new SelectionChangedEventHandler(OnSelectionChanged);
        }

        static void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AutoCompleteBox autoCompleteBox = e.OriginalSource as AutoCompleteBox;

            if (autoCompleteBox != null)
            {
                ListBox listBox = autoCompleteBox.Template.FindName("Selector", autoCompleteBox) as ListBox;

                if (listBox != null)
                    listBox.ScrollIntoView(autoCompleteBox.SelectedItem);
            }
        }

        #endregion

        //#region VirtualizationMode

        //public static VirtualizationMode GetVirtualizationMode(DependencyObject obj)
        //{
        //    return (VirtualizationMode)obj.GetValue(VirtualizationModeProperty);
        //}

        //public static void SetVirtualizationMode(DependencyObject obj, VirtualizationMode value)
        //{
        //    obj.SetValue(VirtualizationModeProperty, value);
        //}

        //public static readonly DependencyProperty VirtualizationModeProperty =
        //    DependencyProperty.RegisterAttached("VirtualizationMode",
        //                                         typeof(VirtualizationMode),
        //                                         typeof(AutoCompleteBoxBehavior),
        //                                         new UIPropertyMetadata(VirtualizationMode.Standard, OnVirtualizationModeChanged));

        //static void OnVirtualizationModeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        //{
        //    AutoCompleteBox autoCompleteBox = obj as AutoCompleteBox;

        //    if (autoCompleteBox == null)
        //        return;

        //    if (e.NewValue is VirtualizationMode == false)
        //        return;

        //    autoCompleteBox.Populated += new PopulatedEventHandler(autoCompleteBox_Populated);
        //}

        //#endregion

        //#region IsDeferredScrollingEnabled

        //public static bool GetIsDeferredScrollingEnabled(DependencyObject obj)
        //{
        //    return (bool)obj.GetValue(IsDeferredScrollingEnabledProperty);
        //}

        //public static void SetIsDeferredScrollingEnabled(DependencyObject obj, bool value)
        //{
        //    obj.SetValue(IsDeferredScrollingEnabledProperty, value);
        //}

        //public static readonly DependencyProperty IsDeferredScrollingEnabledProperty =
        //    DependencyProperty.RegisterAttached("IsDeferredScrollingEnabled",
        //                                         typeof(bool),
        //                                         typeof(AutoCompleteBoxBehavior),
        //                                         new UIPropertyMetadata(false, OnIsDeferredScrollingEnabledChanged));

        //static void OnIsDeferredScrollingEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        //{
        //    AutoCompleteBox autoCompleteBox = obj as AutoCompleteBox;

        //    if (autoCompleteBox == null)
        //        return;

        //    if (e.NewValue is bool == false)
        //        return;

        //    //if ((bool)e.NewValue)
        //    //    autoCompleteBox.Populated += new PopulatedEventHandler(autoCompleteBox_Populated);
        //    //else
        //    //    autoCompleteBox.Populated -= new PopulatedEventHandler(autoCompleteBox_Populated);

        //    autoCompleteBox.Populated += new PopulatedEventHandler(autoCompleteBox_Populated);
        //}

        //#endregion

        //static void autoCompleteBox_Populated(object sender, PopulatedEventArgs e)
        //{
        //    AutoCompleteBox autoCompleteBox = e.OriginalSource as AutoCompleteBox;

        //    if (autoCompleteBox != null)
        //    {
        //        ListBox listBox = autoCompleteBox.Template.FindName("Selector", autoCompleteBox) as ListBox;

        //        if (listBox != null)
        //        {
        //            VirtualizingStackPanel.SetVirtualizationMode(listBox, (VirtualizationMode)autoCompleteBox.GetValue(AutoCompleteBoxBehavior.VirtualizationModeProperty));
        //            ScrollViewer.SetIsDeferredScrollingEnabled(listBox, (bool)autoCompleteBox.GetValue(AutoCompleteBoxBehavior.IsDeferredScrollingEnabledProperty));
        //        }
        //    }
        //}
    }
}
