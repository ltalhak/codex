using System;
using System.Windows;
using System.Windows.Input;

namespace SolarOrderQuiz.Behaviors
{
    public static class DragDropBehavior
    {
        public static readonly DependencyProperty DragDataProperty = DependencyProperty.RegisterAttached(
            "DragData",
            typeof(object),
            typeof(DragDropBehavior),
            new PropertyMetadata(null, OnDragDataChanged));

        public static readonly DependencyProperty DropCommandProperty = DependencyProperty.RegisterAttached(
            "DropCommand",
            typeof(ICommand),
            typeof(DragDropBehavior),
            new PropertyMetadata(null, OnDropCommandChanged));

        private const string DataFormat = "SolarOrderQuiz.Planet";
        private static readonly DependencyProperty DragStartPointProperty = DependencyProperty.RegisterAttached(
            "DragStartPoint",
            typeof(Point?),
            typeof(DragDropBehavior));

        public static void SetDragData(DependencyObject element, object value) => element.SetValue(DragDataProperty, value);

        public static object? GetDragData(DependencyObject element) => element.GetValue(DragDataProperty);

        public static void SetDropCommand(DependencyObject element, ICommand? value) => element.SetValue(DropCommandProperty, value);

        public static ICommand? GetDropCommand(DependencyObject element) => (ICommand?)element.GetValue(DropCommandProperty);

        private static void OnDragDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement element)
            {
                return;
            }

            element.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
            element.PreviewMouseMove -= OnPreviewMouseMove;

            if (e.NewValue is not null)
            {
                element.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
                element.PreviewMouseMove += OnPreviewMouseMove;
            }
        }

        private static void OnDropCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not UIElement element)
            {
                return;
            }

            element.AllowDrop = e.NewValue is not null;
            element.DragEnter -= OnDragEnter;
            element.DragOver -= OnDragOver;
            element.Drop -= OnDrop;

            if (e.NewValue is ICommand)
            {
                element.DragEnter += OnDragEnter;
                element.DragOver += OnDragOver;
                element.Drop += OnDrop;
            }
        }

        private static void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is DependencyObject d)
            {
                d.SetValue(DragStartPointProperty, e.GetPosition(null));
            }
        }

        private static void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || sender is not FrameworkElement element)
            {
                return;
            }

            if (element.GetValue(DragDataProperty) is null)
            {
                return;
            }

            if (element.GetValue(DragStartPointProperty) is not Point startPoint)
            {
                return;
            }

            var currentPoint = e.GetPosition(null);
            if (Math.Abs(currentPoint.X - startPoint.X) < SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(currentPoint.Y - startPoint.Y) < SystemParameters.MinimumVerticalDragDistance)
            {
                return;
            }

            var data = element.GetValue(DragDataProperty);
            if (data is null)
            {
                return;
            }

            var dataObject = new DataObject(DataFormat, data);
            DragDrop.DoDragDrop(element, dataObject, DragDropEffects.Move);
            element.SetValue(DragStartPointProperty, null);
        }

        private static void OnDragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormat))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        private static void OnDragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormat))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }

            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        private static void OnDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormat))
            {
                return;
            }

            if (sender is not DependencyObject d)
            {
                return;
            }

            var command = GetDropCommand(d);
            var data = e.Data.GetData(DataFormat);

            if (command?.CanExecute(data) == true)
            {
                command.Execute(data);
            }
        }
    }
}
