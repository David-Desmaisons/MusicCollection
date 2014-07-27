using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MusicCollection.Infra;
using System.ComponentModel;

namespace MusicCollectionWPF.ViewModelHelper
{
    public static class RelayCommand
    {
        #region Empty Command
        private class EmptyCommand : ICommand
        {
            public EmptyCommand()
            {
            }

            [DebuggerStepThrough]
            public bool CanExecute(object parameter)
            {
                return true;
            }

            [DebuggerStepThrough]
            public void Execute(object parameter)
            {
            }

            public event EventHandler CanExecuteChanged
            { add {} remove {} }
        }
        #endregion

        #region implementation class

        private class BasicRelayCommand : ICommand
        {
            readonly Action<object> _execute;

            public BasicRelayCommand(Action<object> execute)
            {
                _execute = execute;
            }

            public BasicRelayCommand(Action execute)
                : this((_) => execute())
            {
            }

            [DebuggerStepThrough]
            public bool CanExecute(object parameter)
            {
                return true;
            }

            [DebuggerStepThrough]
            public void Execute(object parameter)
            {
                _execute(parameter);
            }

            public event EventHandler CanExecuteChanged
            { add { CommandManager.RequerySuggested += value; } remove { CommandManager.RequerySuggested -= value; } }
        }

        private class BasicRelayCommandAutoRefresh : ICommand
        {
             readonly Action _execute;
             readonly Func<bool> _Canexecute;

            public BasicRelayCommandAutoRefresh(Action execute,Func<bool> Canexecute)
            {
                _execute = execute;
                _Canexecute = Canexecute;
            }

            [DebuggerStepThrough]
            public bool CanExecute(object parameter)
            {
                return _Canexecute();
            }

            [DebuggerStepThrough]
            public void Execute(object parameter)
            {
                _execute();
            }

            public event EventHandler CanExecuteChanged
            { add { CommandManager.RequerySuggested += value; } remove { CommandManager.RequerySuggested -= value; } }
        }

        private class BasicRelayCommand<T> : ICommand where T : class
        {
            readonly Action<T> _execute;
            readonly Func<T, bool> _canexecute;

            public BasicRelayCommand(Action<T> execute)
                : this(execute, t => true)
            {
            }

            public BasicRelayCommand(Action<T> execute, Func<T, bool> canexecute)
            {
                _execute = execute;
                _canexecute = canexecute;
            }

            [DebuggerStepThrough]
            public bool CanExecute(object parameter)
            {
                return _canexecute(parameter as T);
            }

            [DebuggerStepThrough]
            public void Execute(object parameter)
            {
                _execute(parameter as T);
            }

            public event EventHandler CanExecuteChanged
            { add { CommandManager.RequerySuggested += value; } remove { CommandManager.RequerySuggested -= value; } }
        }

        private abstract class DynamicBasicRelayCommandBase<T>  where T : class
        {
            readonly IFunction<bool> _canExecute;

            public DynamicBasicRelayCommandBase(Expression<Func<bool>> iCondition)
            {
                _canExecute = iCondition.CompileToObservable();
                _canExecute.PropertyChanged += OnPropertyChanged;
                CommandManager.RequerySuggested += CommandManager_RequerySuggested;
            }

            private void CommandManager_RequerySuggested(object sender, EventArgs e)
            {
                FireCanExecuteChanged();
            }

            private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                FireCanExecuteChanged();
            }

            [DebuggerStepThrough]
            public virtual bool CanExecute(object parameter)
            {
                return _canExecute.Value;
            }

            protected void FireCanExecuteChanged()
            {
                EventHandler cec = _CanExecuteChanged;
                if (cec != null)
                    cec(this, EventArgs.Empty);
            }

            private event EventHandler _CanExecuteChanged;
            public event EventHandler CanExecuteChanged
            { add { _CanExecuteChanged += value; } remove { _CanExecuteChanged -= value; } }

            public void Dispose()
            {
                CommandManager.RequerySuggested -= CommandManager_RequerySuggested;
                _canExecute.Dispose();
                _canExecute.PropertyChanged -= OnPropertyChanged;
            }
        }

        private class DynamicBasicRelayCommand<T> : DynamicBasicRelayCommandBase<T> , IDynamicCommand where T : class
        {
            private readonly Action<T> _execute;
    
            public DynamicBasicRelayCommand(Action execute, Expression<Func<bool>> iCondition)
                : this((_) => execute(), iCondition)
            {
            }

            public DynamicBasicRelayCommand(Action<T> execute, Expression<Func<bool>> iCondition)
                : base(iCondition)
            {
                _execute = execute;
            }

            [DebuggerStepThrough]
            public void Execute(object parameter)
            {
                _execute(parameter as T);
            }
        }

        private abstract class CompleteDynamicBasicRelayCommandBase<T> : IDynamicCommand where T : class
        {
            readonly ISimpleFunction<T, bool> _canExecute;
            private T _CurrentElement;
            private bool _First;

            protected CompleteDynamicBasicRelayCommandBase(Expression<Func<T, bool>> iCondition)
            {
                _canExecute = iCondition.CompileToObservableFunction();
                _canExecute.ElementChanged += _canExecute_ElementChanged;
                _canExecute.ElementsChanged += _canExecute_ElementsChanged;
                _First = true;
                CommandManager.RequerySuggested += CommandManager_RequerySuggested;
            }

            private void CommandManager_RequerySuggested(object sender, EventArgs e)
            {
                FireCanExecuteChanged();
            }

            private void _canExecute_ElementsChanged(object sender, ObjectAttributesChangedArgs<T, bool> e)
            {
                FireCanExecuteChanged();
            }

            private void _canExecute_ElementChanged(object sender, ObjectAttributeChangedArgs<bool> e)
            {
                FireCanExecuteChanged();
            }


            [DebuggerStepThrough]
            public virtual bool CanExecute(object parameter)
            {
                T mynewelement = parameter as T;

                if (_First)
                {
                    _First = false;
                    this._canExecute.Register(mynewelement);
                }
                else if (!object.ReferenceEquals(mynewelement, _CurrentElement))
                {
                    this._canExecute.UnRegister(_CurrentElement);
                    this._canExecute.Register(mynewelement);
                }

                _CurrentElement = mynewelement;

                return _canExecute.GetCached(mynewelement);
            }

            protected void FireCanExecuteChanged()
            {
                EventHandler cec = _CanExecuteChanged;
                if (cec != null)
                    cec(this, EventArgs.Empty);
            }

            private event EventHandler _CanExecuteChanged;
            public event EventHandler CanExecuteChanged
            { add { _CanExecuteChanged += value; } remove { _CanExecuteChanged -= value; } }

            [DebuggerStepThrough]
            public abstract void Execute(object parameter);

            public void Dispose()
            {
                CommandManager.RequerySuggested -= CommandManager_RequerySuggested;
                if (_CurrentElement != null)
                {
                    _canExecute.UnRegister(_CurrentElement);
                }
                _canExecute.Dispose();
                _canExecute.ElementChanged -= _canExecute_ElementChanged;
                _canExecute.ElementsChanged -= _canExecute_ElementsChanged;
            }
        }

        private class CompleteDynamicBasicRelayCommand<T> : CompleteDynamicBasicRelayCommandBase<T>, IDynamicCommand where T : class
        {
            readonly Action<T> _execute;

            public CompleteDynamicBasicRelayCommand(Action<T> execute, Expression<Func<T, bool>> iCondition)
                : base(iCondition)
            {
                _execute = execute;
            }

            [DebuggerStepThrough]
            public override void Execute(object parameter)
            {
                _execute(parameter as T);
            }
        }

        private class AsyncBasicRelayCommand<T> : CompleteDynamicBasicRelayCommandBase<T>, IDynamicCommand where T : class
        {
            //readonly Action<T> _execute;
            readonly Func<T, Task> _execute;
            private bool _ReadyForExcecution = true;

            public AsyncBasicRelayCommand(Action<T> execute)
                : base((_) => true)
            {
                _execute = (t) => Task.Run(() => execute(t));
            }

            public AsyncBasicRelayCommand(Func<T, Task> execute)
                : base((_) => true)
            {
                _execute = execute;
            }

            public AsyncBasicRelayCommand(Func<T, Task> execute, Expression<Func<T, bool>> iCondition)
                : base(iCondition)
            {
                _execute = execute;
            }

            [DebuggerStepThrough]
            public override bool CanExecute(object parameter)
            {
                if (_ReadyForExcecution == false)
                    return false;

                return base.CanExecute(parameter);
            }

            [DebuggerStepThrough]
            public override void Execute(object parameter)
            {
                _ReadyForExcecution = false;
                FireCanExecuteChanged();
                _execute(parameter as T).ContinueWith(
                    _ =>
                    {
                        _ReadyForExcecution = true;
                        FireCanExecuteChanged();
                    }
                    );
            }
        }

        private class AsyncBasicRelayCommand : DynamicBasicRelayCommandBase<object>, IDynamicCommand
        {
            readonly Func<Task> _execute;
            private bool _ReadyForExcecution = true;

            public AsyncBasicRelayCommand(Action execute)
                : base(() => true)
            {
                _execute = () => Task.Run(execute);
            }

            public AsyncBasicRelayCommand(Func<Task> execute)
                : base(() => true)
            {
                _execute = execute;
            }

            public AsyncBasicRelayCommand(Func<Task> execute,Expression<Func<bool>> CanExcecute)
                : base(CanExcecute)
            {
                _execute = execute;
            }

            [DebuggerStepThrough]
            public override bool CanExecute(object parameter)
            {
                if (_ReadyForExcecution == false)
                    return false;

                return base.CanExecute(parameter);
            }

            [DebuggerStepThrough]
            public void Execute(object parameter)
            {

                _ReadyForExcecution = false;
                FireCanExecuteChanged();
                _execute().ContinueWith(
                    _ =>
                    {
                        _ReadyForExcecution = true;
                        FireCanExecuteChanged();
                    } );
            }
        }

        #endregion

        static public ICommand Instanciate<T>(Action<T> execute) where T : class
        {
            return new BasicRelayCommand<T>(execute);
        }

        static public ICommand Instanciate(Action<object> execute)
        {
            return new BasicRelayCommand(execute);
        }

        static public ICommand Instanciate(Action execute)
        {
            return new BasicRelayCommand(execute);
        }

        static public ICommand InstanciateStatic(Action execute, Func<bool> Canexecute)
        {
            return new BasicRelayCommandAutoRefresh(execute, Canexecute);
        }

        static public IDynamicCommand Instanciate(Action execute, Expression<Func<bool>> canExecute)
        {
            return new DynamicBasicRelayCommand<object>(execute, canExecute);
        }

        static public IDynamicCommand Instanciate<T>(Action<T> execute, Expression<Func<bool>> canExecute) where T : class
        {
            return new DynamicBasicRelayCommand<T>(execute, canExecute);
        }

        static public IDynamicCommand Instanciate<T>(Action<T> execute, Expression<Func<T, bool>> canExecute) where T : class
        {
            return new CompleteDynamicBasicRelayCommand<T>(execute, canExecute);
        }

        static public IDynamicCommand InstanciateAsync<T>(Action<T> execute) where T : class
        {
            return new AsyncBasicRelayCommand<T>(execute);
        }

        static public IDynamicCommand InstanciateAsync<T>(Func<T, Task> execute) where T : class
        {
            return new AsyncBasicRelayCommand<T>(execute);
        }

        static public IDynamicCommand InstanciateAsync<T>(Func<T, Task> execute, Expression<Func<T, bool>> iCondition) where T : class
        {
            return new AsyncBasicRelayCommand<T>(execute, iCondition);
        }

        static public IDynamicCommand InstanciateAsync(Func<Task> execute, Expression<Func<bool>> iCondition)
        {
            return new AsyncBasicRelayCommand(execute, iCondition);
        }

        static public IDynamicCommand InstanciateAsync(Action execute)
        {
            return new AsyncBasicRelayCommand(execute);
        }

        static public IDynamicCommand InstanciateAsync(Func<Task> execute)
        {
            return new AsyncBasicRelayCommand(execute);
        }

        static public ICommand Empty()
        {
            return new EmptyCommand();
        }
    }
}
