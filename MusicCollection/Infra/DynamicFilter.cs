using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Diagnostics;

using MusicCollection.ToolBox.LambdaExpressions;

//DEM Changes TR

namespace MusicCollection.Infra
{
    public sealed class  DynamicFilter<T> : IFunction<T, bool> where T : class
    {
        private ICompleteFunction<T, bool> _Wrapped;
        private ICompleteFunction<T, bool> _OldWrapped = null;
  
        private Expression<Func<T, bool>> _FI;

        public DynamicFilter(Expression<Func<T, bool>> Initial)
        {
            FilterExpression = Initial;
            FactorizeEvent = false;
        }

        public override string ToString()
        {
            return _FI.ToString();
        }

        private bool _FactorizeEvent;
        public bool FactorizeEvent
        {
            get { return _FactorizeEvent; }
            set
            {
                _FactorizeEvent = value;
                this._Wrapped.FactorizeEvent = value;
            }
        }

        public Expression<Func<T, bool>> FilterExpression
        {
            get { return _FI; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                if (value.IsTheSame(_FI))
                {
                    return;
                }

                _OldWrapped = _Wrapped;
                _FI = value;
                _Wrapped = _FI.CompileToObservable();
                _Wrapped.FactorizeEvent = FactorizeEvent;

   
                if (_OldWrapped != null)
                {
 
                    if (this.FactorizeEvent == false)
                    {
                        EventHandler<ObjectAttributeChangedArgs<bool>> ec = ElementChanged;
                        if (ec != null)
                        {
                            foreach (Tuple<T, bool> o in _OldWrapped.ListenedandCachedValue)
                            {
                                var newv = _Wrapped.RegisterAndGetValue(o.Item1);

                                if (o.Item2 != newv)
                                {
                                    ec(this, new ObjectAttributeChangedArgs<bool>(o.Item1, null, o.Item2, newv));
                                }
                            }
                        }
                    }
                    else
                    {
                        EventHandler<ObjectAttributesChangedArgs<T, bool>> esc = ElementsChanged;
                         if (esc != null)
                        {
                            ObjectAttributesChangedArgs<T, bool> willbesent = new ObjectAttributesChangedArgs<T, bool>(_OldWrapped.ListenedCount);

                            foreach (Tuple<T, bool> o in _OldWrapped.ListenedandCachedValue)
                            {
                                var newv = _Wrapped.RegisterAndGetValue(o.Item1);

                                if (o.Item2 != newv)
                                {
                                    willbesent.AddChanges(o.Item1, new ObjectAttributeChangedArgs<bool>(o.Item1, null, o.Item2, newv));
                                }
                            }                         

                            if (willbesent.HasChanged)
                                esc(this, willbesent);

                        }
                    }


                    _OldWrapped.ElementsChanged -= this.LetterEventGroupedListener;
                    _OldWrapped.ElementChanged -= this.LetterEventIndividualListener;

                    _OldWrapped.Dispose();
                    _OldWrapped = null;
                }

                _Wrapped.ElementsChanged += this.LetterEventGroupedListener;
                _Wrapped.ElementChanged += this.LetterEventIndividualListener;

            }
        }

        public Func<T, bool> EvaluateAndRegister
        {
            get { return _Wrapped.EvaluateAndRegister; }
        }

        public bool IsDynamic
        {
            get { return true; }
        }

        public bool IsParameterDynamic
        {
            get { return true; }
        }

        public bool IsConstantDynamic
        {
            get { return true; }
        }

        public Func<T, bool> Evaluate
        {
            get { return _Wrapped.Evaluate; }
        }

       

        public Func<T, bool> CurrentOrOldValueComputer
        {
            get
            {
                if (this._OldWrapped == null)
                    return _Wrapped.CurrentOrOldValueComputer;

                return _OldWrapped.CurrentOrOldValueComputer;
            }
        }

        public bool GetCached(T t)
        {
            return _Wrapped.GetCached(t);
        }

        public bool Register(T to)
        {
            return _Wrapped.Register(to);
        }

        public bool RegisterAndGetValue(T to)
        {
            return _Wrapped.RegisterAndGetValue(to);
        }

        public bool IsSingleRegistered(T item)
        {
            return _Wrapped.IsSingleRegistered(item);
        }



        public bool UnRegister(T to)
        {
            //_Observed.Remove(to);
            return _Wrapped.UnRegister(to);
        }

        //public event EventHandler<GroupedValueChangedArgs<T, bool>> AllChanged;

        public event EventHandler<ObjectAttributeChangedArgs<bool>> ElementChanged;

        private void LetterEventIndividualListener(object sender, ObjectAttributeChangedArgs<bool> oa)
        {
            if (ElementChanged != null)
            {
                ElementChanged(this, oa);
            }
        }

        private void LetterEventGroupedListener(object sender, ObjectAttributesChangedArgs<T, bool> oa)
        {
            if (ElementsChanged != null)
            {
                ElementsChanged(this, oa);
            }
        }

        public void UnListenAll()
        {
            _Wrapped.UnListenAll();
        }


        public IEnumerable<T> Listened
        {
            get { return _Wrapped.Listened; }
        }


        public void Dispose()
        {
            _Wrapped.Dispose();
            _Wrapped.ElementChanged -= this.LetterEventIndividualListener;
            _Wrapped.ElementsChanged -= this.LetterEventGroupedListener;
            //_Observed.Clear();
        }


        public event EventHandler<ObjectAttributesChangedArgs<T, bool>> ElementsChanged;
    }
}
