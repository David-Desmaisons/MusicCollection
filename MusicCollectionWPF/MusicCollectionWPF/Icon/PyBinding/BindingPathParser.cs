using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows;
using System.Windows.Markup;
using System.Xaml;

namespace PyBinding
{
    public static class BindingPathParser
    {
        private static readonly IDictionary<string, BindingDescriptor> BindingCache;
        private static readonly IDictionary<string, ScriptDescriptor> ScriptCache;

        //These regexes took lots of time to get right, and include support
        //  for referencing resource, and x:Static.  However, these break blend
        //  support, so the expanded form of PyBinding must be used to reference
        //  things in resources.
        //  
        //  Please leave the regexes as is, as the Blend team knows about these
        //  issues and will hopefully be fixing their support for custom
        //  markup extensions.

        private static readonly Regex _bracketRegex = new Regex(@"
                    \$\[
                    (?<path>
                        [^][]*
                        (?:
                            (?:(?<Open>\[)[^][]*)+
                            (?:(?<Close-Open>\])[^][]*)+)*
                            (?(Open)(?!)
                        )
                    )
                    \]",
                    RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);


//                    (?<cast>\([^)]*\))? 
        private static readonly Regex _bindingPathRegex = new Regex(@"                    
                    (?:(?<dataContext>\.) | (?<markupSource>\{[^}]*\}) | (?<xName>\w+)) 
                    \.?
                    (?<propertyPath>
                        (?:
                            (?:
                                [][\w]+[/]? |
                                \((?:\w+:)?\w+\.\w+\)
                            )
                            \.?
                        )+
                    )?",
                    //(?::(?<converter>\w+))?
                    //(?:\#(?<converterParameter>.*))?",
                    RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        private static readonly Regex _selfExtensionRegex = new Regex(@"
                    {\s*Self\s*}",
                    RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        //DEM Template Parent support
        private static readonly Regex _templateParentExtensionRegex = new Regex(@"
                    {\s*TemplatedParent\s*}",
                  RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        private static readonly Regex _RelativeAncestorExtensionRegex = new Regex(@"
                    {\s*FindAncestor\[(?<Type>[^,]+)(,(?<Level>\d+))?\]\s*}",
                 RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
        //End DEM


        //        private static readonly Regex _attachedPropertyRegex = new Regex(@"
        //                    (?:
        //                        \(
        //                            (?<propertyPath>
        //                                (?:\w+:)?\w+\.\w+
        //                            )
        //                        \)
        //                    )",
        //                    RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        //        private static Regex _staticExtensionRegex = new Regex(@"
        //                    {\s*
        //                    x:Static\s+ 
        //                    (?<typeName>
        //	                    (?:\w+:)?
        //	                    \w+
        //                    )
        //                    \.
        //                    (?<member>\w+)
        //                    \s*}",
        //                    RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        //        private static Regex _staticResourceExtensionRegex = new Regex(@"
        //                    {\s*
        //                    StaticResource\s+ 
        //                    (?<resourceKey>
        //	                    \w+
        //                    )
        //                    \s*}",
        //                    RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        //        private static Regex _findAncestorExtensionRegex = new Regex(@"
        //                    {\s*FindAncestor\s+
        //                    (?<typeName>
        //	                    (?:\w+:)?
        //	                    \w+
        //                    )
        //                    \s*}",
        //                    RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        static BindingPathParser()
        {
            BindingCache = new Dictionary<string, BindingDescriptor>();
            ScriptCache = new Dictionary<string, ScriptDescriptor>();
        }

      

        public static Binding BuildBinding(string path)
        {
            BindingDescriptor bindingDescriptor;

            if (BindingCache.TryGetValue(path, out bindingDescriptor))
                return bindingDescriptor.ToBinding();

            bindingDescriptor = new BindingDescriptor();

            var match = _bindingPathRegex.Match(path);

            if (match.Groups["propertyPath"].Success)
            {
                bindingDescriptor.PropertyPath = match.Groups["propertyPath"].Value;
            }

            if (!match.Groups["dataContext"].Success &&
                !match.Groups["markupSource"].Success &&
                !match.Groups["xName"].Success)
            {
                throw new System.Windows.Markup.XamlParseException("Could not find source in: " + path);
            }

            if (match.Groups["xName"].Success)
            {
                bindingDescriptor.ElementName = match.Groups["xName"].Value;
            }

            if (match.Groups["markupSource"].Success)
            {
                string markupSourcePath = match.Groups["markupSource"].Value;

                Match selfExtensionMatch = _selfExtensionRegex.Match(markupSourcePath);

                if (selfExtensionMatch.Success)
                {
                    bindingDescriptor.RelativeSource = RelativeSource.Self;
                }
                //DEM Added TemplatedParent and FindAncestor support
                else
                {
                    Match parenttemplateExtensionMatch = _templateParentExtensionRegex.Match(markupSourcePath);
                    if (parenttemplateExtensionMatch.Success)
                    {
                        bindingDescriptor.RelativeSource = RelativeSource.TemplatedParent;
                    }
                    else
                    {
                        Match RelativeAncestorExtensionMatch = _RelativeAncestorExtensionRegex.Match(markupSourcePath);
                        if (RelativeAncestorExtensionMatch.Success)
                        {
                           // RelativeSource nr = new RelativeSource(RelativeSourceMode.FindAncestor);
                            RelativeSource nr = new RelativeSource();
                            nr.AncestorType = TypeFromStringSolver.FromString(RelativeAncestorExtensionMatch.Groups["Type"].Value);
                            if (RelativeAncestorExtensionMatch.Groups["Level"].Success) 
                            {
                                nr.AncestorLevel = int.Parse(RelativeAncestorExtensionMatch.Groups["Level"].Value);
                            }
                            bindingDescriptor.RelativeSource = nr;
                        }
                    }
                }
                //End DEM

            }

            BindingCache.Add(path, bindingDescriptor);

            return bindingDescriptor.ToBinding();
        }

        public static IEnumerable<string> GetUniqueMarkupBrackets(string source)
        {
            ScriptDescriptor scriptDescriptor;
            if (GetOrCreateScript(source, out scriptDescriptor) && scriptDescriptor.UniqueMarkupBrackets != null)
            {
                return scriptDescriptor.UniqueMarkupBrackets;
            }

            var paths = new LinkedList<string>();

            var matches = _bracketRegex.Matches(source);
            foreach (Match match in matches)
            {
                string value = match.Groups["path"].Value;
                if (paths.Count == 0 || !paths.Contains(value))
                    paths.AddLast(value);
            }

            scriptDescriptor.UniqueMarkupBrackets = paths;

            return paths;
        }

        public static string SetupPythonScriptWithReplacement(string scriptSource, string pythonEvaluatorVariablePrefix)
        {
            ScriptDescriptor scriptDescriptor;
            if (GetOrCreateScript(scriptSource, out scriptDescriptor) && !string.IsNullOrEmpty(scriptDescriptor.FinalScript))
            {
                return scriptDescriptor.FinalScript;
            }

            int lastVariableIndex = 0;
            var variables = new Dictionary<string, string>();

            string script = _bracketRegex.Replace(scriptSource, match =>
            {
                string value = match.Value;
                if (variables.ContainsKey(value))
                    return variables[value];

                string variableName = pythonEvaluatorVariablePrefix + lastVariableIndex;
                lastVariableIndex++;
                variables.Add(value, variableName);

                return variableName;
            });

            script = script.Trim();

            scriptDescriptor.FinalScript = script;

            return script;
        }

        private static bool GetOrCreateScript(string scriptSource, out ScriptDescriptor scriptDescriptor)
        {
            if (!ScriptCache.TryGetValue(scriptSource, out scriptDescriptor))
            {
                scriptDescriptor = new ScriptDescriptor();
                ScriptCache.Add(scriptSource, scriptDescriptor);
                return false;
            }
            return true;
        }

        public static bool ContainsPythonCode(string finalScript)
        {
            var match = Regex.Match(finalScript, @"^\s*" + PythonEvaluator.VariablePrefix + @"0\s*$");

            return !match.Success;
        }

        private class ScriptDescriptor
        {
            public string FinalScript { get; set; }
            public LinkedList<string> UniqueMarkupBrackets { get; set; }

        }

        private struct BindingDescriptor
        {
            public string PropertyPath { private get; set; }
            public string ElementName { private get; set; }
            public RelativeSource RelativeSource { private get; set; }

            public Binding ToBinding()
            {
                var binding = new Binding();

                if (!string.IsNullOrEmpty(this.PropertyPath))
                    binding.Path = new PropertyPath(this.PropertyPath);

                if (!string.IsNullOrEmpty(this.ElementName))
                    binding.ElementName = this.ElementName;

                if (this.RelativeSource != null)
                    binding.RelativeSource = this.RelativeSource;

                return binding;
            }
        }
    }
}
