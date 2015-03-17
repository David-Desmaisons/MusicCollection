using System;
using System.IO;
using System.Linq;
using System.Windows;
using IronPython.Compiler;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System.Collections.Generic;
using Microsoft.Scripting.Runtime;
using System.Reflection;
using System.Diagnostics;

namespace PyBinding
{
    public class PythonEvaluator
    {
        #region Constants

        public const string VariablePrefix = "var_";

        private const string STARTER_SCRIPT_FILENAME = "StartupScript.py";

        #endregion

        #region Constructors

        static PythonEvaluator()
        {
            //TODO: bring back configuration via config file
            var options = new Dictionary<string, object>();
            options["LightweightScopes"] = false;
            options["Optimize"] = true;
            
            Engine = IronPython.Hosting.Python.CreateEngine(options);
            // Allow Optimized option to be set. We are caching compiled code anyway, so pinning for the sake of performance is ok. 
            DefaultCompilerOptions = new PythonCompilerOptions(ModuleOptions.Optimized | ModuleOptions.ModuleBuiltins);

            CodeCache = new Dictionary<string, CompiledCode>();
        }

        public PythonEvaluator()
            : this(STARTER_SCRIPT_FILENAME)
        {
        }
        
        public PythonEvaluator(string starterFile)
        {
            LoadAssembliesIntoEngine();

            this.ScriptScope = Engine.CreateScope();

            //DEM Embedding Py file
            string Res = null;
            using (Stream stream = Assembly.GetAssembly(typeof(PythonEvaluator)).GetManifestResourceStream("MusicCollectionWPF.PyBinding.Resource." + STARTER_SCRIPT_FILENAME))
            {
                using (StreamReader textStreamReader = new StreamReader(stream))
                {
                    Res = textStreamReader.ReadToEnd();
                }
            }
             
            var importSource = Engine.CreateScriptSourceFromString(Res);
            ExecuteSafely(() => importSource.Compile(DefaultCompilerOptions).Execute(this.ScriptScope));
        }

        #endregion

        #region Properties

        private ScriptScope ScriptScope { get; set; }

        private static PythonCompilerOptions DefaultCompilerOptions { get; set; }

        private static ScriptEngine Engine { get; set; }

        private static IDictionary<String, CompiledCode> CodeCache { get; set; }

        #endregion

        #region Methods

        public void SetVariable(ScriptScope scope, string name, object value)
        {
            scope.SetVariable(name, value);
        }

        public static CompiledCode Compile(string script)
        {
            CompiledCode compiledCode;
            
            if (!CodeCache.TryGetValue(script, out compiledCode))
            {
                //specify kind so that IronPython does not have to parse the script and determine kind
                var source = Engine.CreateScriptSourceFromString(script, SourceCodeKind.Expression);
                compiledCode = (CompiledCode)ExecuteSafely(() => source.Compile(DefaultCompilerOptions));
                CodeCache.Add(script, compiledCode);
            }

            return compiledCode;
        }

        //DEM new changes
        public object ExecuteWithResult(IPyBinding binding, object[] values)
        {
            SetVariables(this.ScriptScope, values);
            SetVariable(this.ScriptScope, "Parameter", binding.Parameter);
            return ExecuteSafely(() => binding.CompiledCode.Execute(this.ScriptScope));
        }

        public object ExecuteBackWithResult(IPyBinding binding, object[] values)
        {
            SetVariables(this.ScriptScope, values);
            SetVariable(this.ScriptScope, "Parameter", binding.Parameter);
            return ExecuteSafely(() => binding.CompiledCodeBack.Execute(this.ScriptScope));
        }


        
        ////DEM new changes

        public void SetVariables(ScriptScope scope, object[] values)
        {
            if (values == null)
                return;

            for (int i = 0; i < values.Length; i++)
                SetVariable(scope, VariablePrefix + i, values[i]);
        }

        private static object ExecuteSafely(Func<object> executionBlock)
        {
            try
            {
                return executionBlock();
            }
            //DEM Mod1 for Debug facility-Init
            catch (Exception e)
            {
                ExceptionOperations eo = Engine.GetService<ExceptionOperations>();
                if (eo!=null)
                    Trace.WriteLine(eo.FormatException(e));
                else
                    Trace.WriteLine(e);
                return null;
            }
        }

        private static void LoadAssembliesIntoEngine()
        {
            var domain = AppDomain.CurrentDomain;
            if (domain == null) return;

            var runtime = Engine.Runtime;
            var assemblies = domain.GetAssemblies();
            foreach (Assembly assembly in assemblies.Where(ass => !ass.IsDynamic))
            {
                runtime.LoadAssembly(assembly);
            }
        }

        #endregion
    }
}
