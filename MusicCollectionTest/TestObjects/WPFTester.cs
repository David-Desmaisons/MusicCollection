using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using NUnit.Framework;
using System.Windows.Input;

namespace MusicCollectionTest.TestObjects
{
    public class WPFTester
    {
        #region Fields

        private static readonly DispatcherOperationCallback _exitFrameCallback;

        #endregion

        #region Constructor

        static WPFTester()
        {
            _exitFrameCallback = OnExitFrame;
        }

        #endregion

        #region Properties

        public bool IsInitialized
        {
            get { return (this.Window != null); }
        }

        public Window Window { get; private set; }

        #endregion

        #region Methods

        public void ShowWindow(Window window)
        {
            Initialize(window);
            ShowWindow();
        }

        public void ShowControl(FrameworkElement control)
        {
            Initialize(new Window());

            this.Window.Content = control;
            ShowWindow();
        }

        public void ShowControl(FrameworkElement control, ResourceDictionary dictionary)
        {
            Initialize(new Window());

            this.Window.Resources = dictionary;
            this.Window.Content = control;

            ShowWindow();
        }

        public void Close()
        {
            if (this.Window == null)
                return;

            this.Window.Content = null;
            this.Window.Close();
            this.Window.Dispatcher.UnhandledException -= OnDispatcherUnhandledException;
            this.Window = null;
            GC.Collect();
        }

        public void UpdateWindowLayout()
        {
            if (!this.IsInitialized)
                throw new InvalidOperationException("You must initialize WPFTester before you can update the window layout.");

            this.Window.UpdateLayout();
        }

        private void Initialize(Window window)
        {
            if (this.IsInitialized)
                return;

            if (window == null)
                throw new ArgumentNullException("window", "WPFTester must be initialized with a valid window");

            this.Window = window;
            this.Window.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        private void ShowWindow()
        {
            this.Window.Show();
            this.Window.BringIntoView();
        }

        private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Assert.Fail(e.Exception.ToString());
        }

        public T FindChildControl<T>(string name)
            where T : FrameworkElement
        {
            var target = this.Window.Content as FrameworkElement;
            return (target != null ? FindChildControl<T>(target, name) : null);
        }

        public T FindTemplateChildControl<T>(string name)
          where T : FrameworkElement
        {
            var target = this.Window.Content as Control;
            return (target != null ? FindTemplateChildControl<T>(target, name) : null);
        }

        public static ResourceDictionary LoadResourceDictionary(params string[] uriList)
        {
            var dictionary = new ResourceDictionary();

            // Load uriList into ResourceDictionary
            foreach (string uri in uriList)
            {
                dictionary.MergedDictionaries.Add(CreateResourceDictionary(uri));
            }
            return dictionary;
        }

        private static ResourceDictionary CreateResourceDictionary(string uri)
        {
            return Application.LoadComponent(new Uri(uri, UriKind.Relative)) as ResourceDictionary;
        }

        public static T FindChildControl<T>(FrameworkElement parent, string name)
            where T : FrameworkElement
        {
            return parent.FindName(name) as T;
        }

        public static T FindChildControl<T>(FrameworkTemplate template, FrameworkElement parent, string name)
            where T : FrameworkElement
        {
            return template.FindName(name, parent) as T;
        }

        public static T FindTemplateChildControl<T>(Control parent, string name)
          where T : FrameworkElement
        {
            return parent.Template.FindName(name, parent) as T;
        }

        public static T GetTemplateContent<T>(FrameworkTemplate template)
            where T : DependencyObject
        {
            return template.LoadContent() as T;
        }

        public static ContentPresenter GetContentPresenter(ListBoxItem item)
        {
            var border = VisualTreeHelper.GetChild(item, 0) as Border;
            if (border == null)
                return null;

            return border.Child as ContentPresenter;
        }

        public static T GetItemsControlPanel<T>(ItemsControl itemsControl)
            where T : Panel
        {
            var border = VisualTreeHelper.GetChild(itemsControl, 0) as Border;
            if (border == null)
                return null;

            var itemspresenter = (ItemsPresenter)border.Child;
            return VisualTreeHelper.GetChild(itemspresenter, 0) as T;
        }

        public static void MouseLeftButtonDown(UIElement element)
        {
            element.RaiseEvent(
                new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left)
                {
                    RoutedEvent = UIElement.MouseLeftButtonDownEvent
                });
        }

        public static void MouseEnter(Control control)
        {
            Mouse.Capture(control, CaptureMode.Element);
        }

        public static void MouseLeave(Control control)
        {
            Mouse.Capture(control, CaptureMode.None);
        }

        public static void ClickButton(Button button)
        {
            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        public static void CheckButton(ToggleButton button)
        {
            button.RaiseEvent(new RoutedEventArgs(ToggleButton.CheckedEvent));
        }

        public static void UncheckButton(ToggleButton button)
        {
            button.RaiseEvent(new RoutedEventArgs(ToggleButton.UncheckedEvent));
        }

        public static void ClickImage(Image image)
        {
            var routedEventArgs =
                new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left)
                {
                    RoutedEvent = UIElement.MouseUpEvent
                };
            image.RaiseEvent(routedEventArgs);
        }

        public static void ComboBoxSelectionChanged(ComboBox comboBox)
        {
            comboBox.RaiseEvent(new RoutedEventArgs(Selector.SelectionChangedEvent));
        }

        public static void SendPreviewTextInput(UIElement element, string resultText, out bool handled)
        {
            TextCompositionEventArgs args = CreateTextCompositionEventArgs(resultText);
            args.RoutedEvent = UIElement.PreviewTextInputEvent;
            element.RaiseEvent(args);
            handled = args.Handled;
        }

        public static void SendPreviewKeyDown(UIElement element, Key key, out bool handled)
        {
            KeyEventArgs args = CreateKeyEventArgs(key);
            args.RoutedEvent = UIElement.PreviewKeyDownEvent;
            element.RaiseEvent(args);
            handled = args.Handled;
        }

        public static T GetContainerFromIndex<T>(ItemsControl itemsControl, int i)
            where T : DependencyObject
        {
            return itemsControl.ItemContainerGenerator.ContainerFromIndex(i) as T;
        }

        public static void AssertStyleSetsExpectedValueToAProperty(Style style, string propertyName, object value)
        {
            Setter targetSetter = null;
            foreach (Setter setter in style.Setters)
            {
                if (setter.Property.Name == propertyName)
                {
                    targetSetter = setter;
                    break;
                }
            }

            Assert.IsNotNull(targetSetter);
            Assert.AreEqual(value, targetSetter.Value);
        }

        public static void AssertStyleIsAppliedToControl(Style style, FrameworkElement element)
        {
            foreach (SetterBase setterBase in style.Setters)
            {
                var setter = setterBase as Setter;

                if (setter == null)
                    continue;

                object expected = setter.Value;

                // We can't resolve a binding in the actual style until its
                // been applied to the actual object at which point we're
                // only able to pick up the resolved object and not the actual
                // Binding.
                if (expected is Binding)
                    continue;

                if (expected is MultiBinding)
                    continue;

                object actual = element.GetValue(setter.Property);

                Assert.AreEqual(expected, actual,
                    string.Format("Invalid value for property '{0}.{1}' as defined by style",
                                    setter.Property.OwnerType.Name, setter.Property.Name));
            }
        }

        public static void AssertStyleIsAppliedToContentControl(Style style, FrameworkContentElement element)
        {
            foreach (SetterBase setterBase in style.Setters)
            {
                var setter = setterBase as Setter;

                if (setter == null)
                    continue;

                string propertyName = setter.Property.Name;

                object expected = setter.Value;
                object actual = element.GetType().GetProperty(propertyName).GetValue(element, null);

                Assert.AreEqual(expected,
                                actual,
                                string.Format("Invalid value for property '{0}' as defined by style", propertyName));
            }
        }

        public static void EnsureBindings(FrameworkElement element, DependencyProperty property)
        {
            BindingExpression expression = element.GetBindingExpression(property);
            if (expression == null)
                return;

            element.SetBinding(property, expression.ParentBinding.Path.Path);
        }

        private static TextCompositionEventArgs CreateTextCompositionEventArgs(string resultText)
        {
            InputDevice inputDevice = Keyboard.PrimaryDevice;
            return new TextCompositionEventArgs(inputDevice, new TextComposition(InputManager.Current, inputDevice.Target, resultText));
        }

        private static KeyEventArgs CreateKeyEventArgs(Key key)
        {
            return new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, Environment.TickCount, key);
        }

        public static T CreateFromXaml<T>(string xaml) where T : class
        {
            var stringReader = new StringReader(xaml);

            XmlReader reader = XmlReader.Create(stringReader);
            return XamlReader.Load(reader) as T;
        }

        #region DoEvents

        public static void DoEvents()
        {
            // Create new nested message pump.
            var nestedFrame = new DispatcherFrame();

            // Dispatch a callback to the current message queue, when getting called,
            // this callback will end the nested message loop.
            // The priority of this callback should be lower than the that of UI event messages.
            DispatcherOperation exitOperation =
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                                                        _exitFrameCallback, nestedFrame);

            // Pump the nested message loop, the nested message loop will
            // immediately process the messages left inside the message queue.
            Dispatcher.PushFrame(nestedFrame);

            // If the "OnExitFrame" callback doesn't get finished, Abort it.
            if (exitOperation.Status != DispatcherOperationStatus.Completed)
            {
                exitOperation.Abort();
            }
        }

        private static object OnExitFrame(Object state)
        {
            var frame = state as DispatcherFrame;
            if (frame != null)
            {
                // Exit the nested message loop.
                frame.Continue = false;
            }
            return null;
        }

        #endregion

        public static T FindFirstVisualChild<T>(FrameworkElement parent, Func<T, bool> predicate) where T : class
        {
            if (parent == null)
                return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i) as FrameworkElement;
                if (child == null)
                    continue;

                var childT = child as T;
                if (childT != null && predicate(childT))
                    return childT;

                var grandChild = FindFirstVisualChild(child, predicate);
                if (grandChild != null)
                    return grandChild;
            }

            return null;
        }

        #endregion
    }
}
