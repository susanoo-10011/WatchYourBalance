using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WatchYourBalance.Utilities
{
    public static class WindowBehaviours
    {
        // Close the Window
        public static void SetClose(DependencyObject target, bool value)
        {
            target.SetValue(CloseProperty, value);
        }

        public static readonly DependencyProperty CloseProperty =
                                                  DependencyProperty.RegisterAttached("Close",
                                                  typeof(bool),
                                                  typeof(WindowBehaviours),
                                                  new UIPropertyMetadata(false, OnClose));

        private static void OnClose(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool && ((bool)e.NewValue))
            {
                Window window = GetWindow(sender);

                if (window != null)
                {
                    window.Close();
                }
            }
        }

        // Hide the Window
        public static void SetHide(DependencyObject target, bool value)
        {
            target.SetValue(HideProperty, value);
        }

        public static readonly DependencyProperty HideProperty =
                                                  DependencyProperty.RegisterAttached("Hide",
                                                  typeof(bool),
                                                  typeof(WindowBehaviours),
                                                  new UIPropertyMetadata(false, OnHide));

        private static void OnHide(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool && ((bool)e.NewValue))
            {
                Window window = GetWindow(sender);

                if (window != null)
                {
                    window.WindowState = WindowState.Minimized;
                }
            }
        }

        // Full the Window
        public static void SetFull(DependencyObject target, bool value)
        {
            target.SetValue(FullProperty, value);
        }

        public static readonly DependencyProperty FullProperty =
                                                  DependencyProperty.RegisterAttached("Full",
                                                  typeof(bool),
                                                  typeof(WindowBehaviours),
                                                  new UIPropertyMetadata(false, OnFull));

        private static void OnFull(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool && ((bool)e.NewValue))
            {
                Window window = GetWindow(sender);

                if (window != null)
                {
                    window.WindowState = WindowState.Maximized;
                }
            }
        }

        // Set the Window in Normal
        public static void SetNormal(DependencyObject target, bool value)
        {
            target.SetValue(NormalProperty, value);
        }

        public static readonly DependencyProperty NormalProperty =
                                                  DependencyProperty.RegisterAttached("Normal",
                                                  typeof(bool),
                                                  typeof(WindowBehaviours),
                                                  new UIPropertyMetadata(false, OnNormal));

        private static void OnNormal(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool && ((bool)e.NewValue))
            {
                Window window = GetWindow(sender);

                if (window != null)
                {
                    window.WindowState = WindowState.Normal;
                }
            }
        }

        // Get the Window
        private static Window GetWindow(DependencyObject sender)
        {
            Window window = null;

            if (sender is Window)
            {
                window = (Window)sender;
            }

            if (window == null)
            {
                window = Window.GetWindow(sender);
            }

            return window;
        }
    }
}
