using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace StarFlintSaver.Windows.Behaviours
{
    public static class ListBoxBehavior
    {
        public static bool GetScrollSelectedIntoView(ListBox listBox)
        {
            return (bool)listBox.GetValue(ScrollSelectedIntoViewProperty);
        }

        public static void SetScrollSelectedIntoView(ListBox listBox, bool value)
        {
            listBox.SetValue(ScrollSelectedIntoViewProperty, value);
        }

        public static readonly DependencyProperty ScrollSelectedIntoViewProperty =
            DependencyProperty.RegisterAttached("ScrollSelectedIntoView", typeof(bool), typeof(ListBoxBehavior),
                                                new UIPropertyMetadata(false, OnScrollSelectedIntoViewChanged));

        private static void OnScrollSelectedIntoViewChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (!(dependencyObject is Selector selector))
            {
                return;
            }

            if (eventArgs.NewValue is bool == false)
            {
                return;
            }

            if ((bool)eventArgs.NewValue)
            {
                selector.AddHandler(Selector.SelectionChangedEvent, new RoutedEventHandler(ListBoxSelectionChangedHandler));
            }
            else
            {
                selector.RemoveHandler(Selector.SelectionChangedEvent, new RoutedEventHandler(ListBoxSelectionChangedHandler));
            }
        }

        private static void ListBoxSelectionChangedHandler(object sender, RoutedEventArgs e)
        {
            if (!(sender is ListBox))
            {
                return;
            }

            var listBox = sender as ListBox;
            if (listBox.SelectedItem != null)
            {
                listBox.Dispatcher.BeginInvoke(
                    (Action)(() =>
                    {
                        listBox.UpdateLayout();
                        if (listBox.SelectedItem != null)
                        {
                            listBox.ScrollIntoView(listBox.SelectedItem);
                        }
                    }));
            }
        }
    }
}
