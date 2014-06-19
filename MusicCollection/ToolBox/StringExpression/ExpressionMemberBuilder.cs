using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Diagnostics;


using MusicCollection.ToolBox.Collection;
using MusicCollection.ToolBox.StringExpression;

namespace MusicCollection.ToolBox.StringExpression
{
    public class ExpressionMemberBuilder<Tin,Tout>
    {
        public Expression<Func<Tin, Tout>> ExpressionResult
        {
            get;
            private set;
        }

        static private readonly PolyMorphDictionaryGeneric<string, ExpressionMemberBuilder<Tin, Tout>> _Cache = null;

        static ExpressionMemberBuilder()
        {
            _Cache = new PolyMorphDictionary<string, ExpressionMemberBuilder<Tin, Tout>>();
        }

        public string Name
        {
            get;
            private set;
        }

        public Exception BuildingException
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return String.Format("Name: {0} - Expression: {1} - Exception: {2}", Name, ExpressionResult,BuildingException);
        }

        public ExpressionMemberBuilder(string iName)
        {
            Name = iName;

            ExpressionMemberBuilder<Tin,Tout> myExpr = null;
            if (_Cache.TryGetValue(Name, out myExpr))
            {
                ExpressionResult = myExpr.ExpressionResult;
                BuildingException = myExpr.BuildingException;
                return;
            }

            StringTokenizer stn = new StringTokenizer(iName);

            var Elements = stn.GetTokens().ToList();

            if (stn.Error)
                return;

            if (Elements.Count != 1)
                return;

            try
            {
                ParameterExpression pe = Expression.Parameter(typeof(Tin), "x");

                SimpleToken st = ExpressionToken.GetToken(Elements[0], null, true,pe) as SimpleToken;
                if (st == null)
                    return;

                ExpressionResult = Expression.Lambda<Func<Tin, Tout>>(st.Builder.ConvertToType(typeof(Tout)), pe);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                BuildingException = e;
            }

            _Cache.Add(Name, this);
        }

        public static implicit operator Expression<Func<Tin, Tout>>( ExpressionMemberBuilder<Tin,Tout> emf)
        {
            return emf.ExpressionResult;
        }
    }
}
