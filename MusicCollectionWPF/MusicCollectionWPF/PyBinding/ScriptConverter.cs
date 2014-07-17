using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Windows.Markup;
using System.Windows.Data;
using System.Globalization;
using System.Diagnostics;
using System.Threading.Tasks;
using MusicCollection.Infra;

namespace PyBinding
{
    [MarkupExtensionReturnType(typeof(object))]
    public class ScriptConverter : IValueConverter, IMultiValueConverter
    {
        #region Constructors

        //static internal void Load()
        //{
        //}

        //static ScriptConverter()
        //{
        //    PythonEvaluator = new PythonEvaluator();
        //    DefaultValues = new Dictionary<Type, object>
        //                        {
        //                            {typeof (int), 0},
        //                            {typeof (string), null},
        //                            {typeof (double), 0.0D},
        //                            {typeof (decimal), 0.0M},
        //                            {typeof (bool), false},
        //                            {typeof (float), 0.0F},
        //                            {typeof (long), 0L},
        //                            {typeof (short), 0}
        //                        };
        //}

        private static Task _Loader;

        static public Task LoadAsync(ThreadProperties tp = null)
        {
            if (_Loader != null)
                return _Loader;

            _Loader = Task.Factory.StartNew(
                () =>
                {
                    using (tp.GetChanger())
                    {
                        PythonEvaluator = new PythonEvaluator();
                    }
                },
                    TaskCreationOptions.LongRunning);

            return _Loader;
        }

        static ScriptConverter()
        {
            DefaultValues = new Dictionary<Type, object>
                                {
                                    {typeof (int), 0},
                                    {typeof (string), null},
                                    {typeof (double), 0.0D},
                                    {typeof (decimal), 0.0M},
                                    {typeof (bool), false},
                                    {typeof (float), 0.0F},
                                    {typeof (long), 0L},
                                    {typeof (short), 0}
                                };
        }

        public ScriptConverter()
        {
            LoadAsync().Wait();
        }

        #endregion

        #region Properties

        private static PythonEvaluator PythonEvaluator { get; set; }

        private static IDictionary<Type, object> DefaultValues { get; set; }

        #endregion

        #region Methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(new[] { value }, targetType, parameter, culture);
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {            
            try
            {
                var pyBinding = parameter as IPyBinding;
                if (pyBinding == null || (pyBinding.UnsetValueIsInvalid && ContainsUnsetValue(values)))
                    return GetDefaultValue(targetType);

                var result = PythonEvaluator.ExecuteWithResult(pyBinding, values);
                return result ?? GetDefaultValue(targetType);
            }
            catch (Exception)
            {
                return GetDefaultValue(targetType);
            }
        }

        private static bool ContainsUnsetValue(object[] values)
        {
            for (int i = 0; i < values.Length; i++)
                if (values[i] == DependencyProperty.UnsetValue)
                    return true;

            return false;
        }

        //DEM Two Way Support
        private static object[] GetDefaultValue(Type[] targetTypes)
        {
            return targetTypes.Cast<Type>().Select(t => GetDefaultValue(t)).ToArray<object>();
        }
        //End DEM

        private static object GetDefaultValue(Type targetType)
        {
            if (targetType == null)
                return null;

            if (DefaultValues.ContainsKey(targetType))
                return DefaultValues[targetType];
            
            object result = null;

            if (targetType.IsValueType)
                result = Activator.CreateInstance(targetType);

            DefaultValues.Add(targetType, result);
            
            return result;
        }

        //DEM Two ways Binding support

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            try
            {
                var pyBinding = parameter as IPyBinding;
                if (pyBinding == null || (pyBinding.UnsetValueIsInvalid && value == DependencyProperty.UnsetValue))
                    return GetDefaultValue(targetTypes);

                if (pyBinding.CompiledCodeBack == null)
                    throw new NotSupportedException("ScriptBack PyBinding attribute should be not null to support two way binding ");

                var result = PythonEvaluator.ExecuteBackWithResult(pyBinding, new object[] { value }); 
                
                if (result == null)
                    return GetDefaultValue(targetTypes);

                int C = targetTypes.Length;  
                if (C==1)
                    return new object[] { result };

                if (!(result is IList<object>))
                {
                    Trace.WriteLine("PyBiding ConvertBack type mismatch");
                    return GetDefaultValue(targetTypes);
                }
               
                var t = (IList<object>)result;              
                if (t.Count != C)
                {
                    Trace.WriteLine("PyBiding ConvertBack expected different count collection");
                    return GetDefaultValue(targetTypes);
                }

                return t.ToArray<object>();               
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                if (e is NotSupportedException)
                    throw;
                return GetDefaultValue(targetTypes);
            }
           
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object[] res = ConvertBack(value, new Type[] { targetType }, parameter, culture);
            if ((res == null) || (res.Length != 1))
                throw new NotSupportedException("PyBinding Script Back inconsistency expected single value");

            return res[0];     
        }

        //End End

        #endregion
    }
}
