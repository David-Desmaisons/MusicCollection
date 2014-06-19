using System.Windows.Data;
using System;
using Microsoft.Scripting.Hosting;
using MusicCollection.Infra;

namespace PyBinding
{
    public interface IPyBinding
    {
        bool UnsetValueIsInvalid { get; set; }
        CompiledCode CompiledCode { get; }
        //DEM Two way Binding support
        CompiledCode CompiledCodeBack { get; }
    }

    public class PyBinding : MultiBinding, IPyBinding
    {
        #region Fields

        private string _script;
        private string _scriptBack;

        #endregion

        #region Constructor

        static PyBinding()
        {
            ScriptConverter = new ScriptConverter();
        }

        public PyBinding()
        {
            this.Mode = BindingMode.OneWay;
            this.StringFormat = "{0}"; // This is required for some reason.  Maybe type converters aren't picking it up?
        }

        public PyBinding(string script)
            : this()
        {
            this.Script = script;
        }

        #endregion

        #region Properties

        private static IMultiValueConverter ScriptConverter { get; set; }

        public bool UnsetValueIsInvalid { get; set; }

        public CompiledCode CompiledCode { get; private set; }

        public CompiledCode CompiledCodeBack { get; private set; }

        public bool IsAsync { get; set; }

        public new BindingMode Mode
        {
            get { return base.Mode; }
            set
            {
                if (value == base.Mode)
                    return;

                base.Mode = value;
                if (value == BindingMode.TwoWay)
                {
                    Bindings.Apply(b => (b as Binding).Mode = value);
                }
            }
        }

        public string Script
        {
            get { return _script; }
            set
            {
                _script = string.Intern(value);
                BuildBindingFromScript();
            }
        }

        //DEM Two ways binding support
        public string ScriptBack
        {
            get { return _scriptBack; }
            set
            {
                _scriptBack = string.Intern(value);
                if (string.IsNullOrEmpty(_scriptBack))
                    return;

                this.CompiledCodeBack = PythonEvaluator.Compile(_scriptBack);
                Mode = BindingMode.TwoWay;
            }
        }

        public override string ToString()
        {
            return string.Format("Mode:<{0}>  Script:<{1}>  ScriptBack:<{2}>",this.Mode,this.Script,this.ScriptBack);
        }


        //End DEM

        #endregion

        #region Methods

        private void BuildBindingFromScript()
        {
            if (string.IsNullOrEmpty(_script))
                return;

            var bindingPaths = BindingPathParser.GetUniqueMarkupBrackets(_script);

            foreach (var bindingPath in bindingPaths)
            {
                var intermediateBinding = BindingPathParser.BuildBinding(bindingPath);
                if (Mode == BindingMode.TwoWay)
                    intermediateBinding.Mode = BindingMode.TwoWay;
                intermediateBinding.IsAsync = this.IsAsync;
                this.Bindings.Add(intermediateBinding);
            }

            string finalScript = BindingPathParser.SetupPythonScriptWithReplacement(_script, PythonEvaluator.VariablePrefix);
            this.CompiledCode = PythonEvaluator.Compile(finalScript);
            this.ConverterParameter = this;
            this.Converter = ScriptConverter;
        }

        #endregion
    }
}
