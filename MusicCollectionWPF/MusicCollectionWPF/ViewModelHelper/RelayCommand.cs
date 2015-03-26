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
        private class EmptyCommand : WPFDispatcher, ICommand
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
            { add { } remove { } }
        }
        #endregion

        #region implementation class

        private class BasicRelayCommand : WPFDispatcher, ICommand
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

        private class BasicRelayCommandAutoRefresh : WPFDispatcher, ICommand
        {
            readonly Action _execute;
            readonly Func<bool> _Canexecute;

            public BasicRelayCommandAutoRefresh(Action execute, Func<bool> Canexecute)
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

        private class BasicRelayCommand<T> : WPFDispatcher, ICommand where T : class
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

            public bool CanExecute(T parameter)
            {
                return _canexecute(parameter as T);
            }

            public void Excecute(T parameter)
            {
                _execute(parameter as T);
            }
        }

        private abstract class DynamicBasicRelayCommandBase<T> : WPFDispatcher where T : class
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

        private class DynamicBasicRelayCommand<T> : DynamicBasicRelayCommandBase<T>, IDynamicCommand where T : class
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

        private abstract class CompleteDynamicBasicRelayCommandBase<T> : WPFDispatcher, IDynamicCommand where T : class
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
            readonly Func<T, Task> _execute;
            private bool _ReadyForExcecution = true;
            private bool _UnavailableWhenExecuting = false;

            public AsyncBasicRelayCommand(Action<T> execute, bool iUnavailableWhenExecuting)
                : base((_) => true)
            {
                _UnavailableWhenExecuting = iUnavailableWhenExecuting;
                _execute = (t) => Task.Run(() => execute(t));
            }

            public AsyncBasicRelayCommand(Func<T, Task> execute, bool iUnavailableWhenExecuting)
                : base((_) => true)
            {
                _UnavailableWhenExecuting = iUnavailableWhenExecuting;
                _execute = execute;
            }

            public AsyncBasicRelayCommand(Func<T, Task> execute, Expression<Func<T, bool>> iCondition, bool iUnavailableWhenExecuting)
                : base(iCondition)
            {
                _UnavailableWhenExecuting = iUnavailableWhenExecuting;
                _execute = execute;
            }

            [DebuggerStepThrough]
            public override bool CanExecute(object parameter)
            {
                if ((_UnavailableWhenExecuting) && (_ReadyForExcecution == false))
                    return false;

                return base.CanExecute(parameter);
            }


            private void SetReadyForExcecution(bool ieex)
            {
                if (!_UnavailableWhenExecuting)
                    return;

                _ReadyForExcecution = ieex;
                FireCanExecuteChanged();
            }

            [DebuggerStepThrough]
            public override async void Execute(object parameter)
            {
                SetReadyForExcecution(false);
                await _execute(parameter as T);

                SetReadyForExcecution(true);
            }
        }

        private class AsyncBasicRelayCommand : DynamicBasicRelayCommandBase<object>, IDynamicCommand
        {
            readonly Func<Task> _execute;
            private bool _ReadyForExcecution = true;
            private bool _UnavailableWhenExecuting = false;

            public AsyncBasicRelayCommand(Action execute, bool iUnavailableWhenExecuting)
                : base(() => true)
            {
                _UnavailableWhenExecuting = iUnavailableWhenExecuting;
                _execute = () => Task.Run(execute);
            }

            public AsyncBasicRelayCommand(Func<Task> execute, bool iUnavailableWhenExecuting)
                : base(() => true)
            {
                _UnavailableWhenExecuting = iUnavailableWhenExecuting;
                _execute = execute;
            }

            public AsyncBasicRelayCommand(Func<Task> execute, Expression<Func<bool>> CanExcecute, bool iUnavailableWhenExecuting)
                : base(CanExcecute)
            {
                _UnavailableWhenExecuting = iUnavailableWhenExecuting;
                _execute = execute;
            }

            [DebuggerStepThrough]
            public override bool CanExecute(object parameter)
            {
                if ((_UnavailableWhenExecuting) && (_ReadyForExcecution == false))
                    return false;

                return base.CanExecute(parameter);
            }

            private void SetReadyForExcecution(bool ieex)
            {
                if (!_UnavailableWhenExecuting)
                    return;

                _ReadyForExcecution = ieex;
                FireCanExecuteChanged();
            }

            [DebuggerStepThrough]
            public async void Execute(object parameter)
            {
                SetReadyForExcecution(false);
                await _execute();
                SetReadyForExcecution(true);
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

        static public IDynamicCommand InstanciateAsync<T>(Action<T> execute, bool iUnavailableWhenExecuting=true) where T : class
        {
            return new AsyncBasicRelayCommand<T>(execute, iUnavailableWhenExecuting);
        }

        static public IDynamicCommand InstanciateAsync<T>(Func<T, Task> execute, bool iUnavailableWhenExecuting = true) where T : class
        {
            return new AsyncBasicRelayCommand<T>(execute, iUnavailableWhenExecuting);
        }

        static public IDynamicCommand InstanciateAsync<T>(Func<T, Task> execute, Expression<Func<T, bool>> iCondition, bool iUnavailableWhenExecuting = true) where T : class
        {
            return new AsyncBasicRelayCommand<T>(execute, iCondition, iUnavailableWhenExecuting);
        }

        static public IDynamicCommand InstanciateAsync(Func<Task> execute, bool iUnavailableWhenExecuting = true)
        {
            return new AsyncBasicRelayCommand(execute, iUnavailableWhenExecuting);
        }

        static public IDynamicCommand InstanciateAsync(Func<Task> execute, Expression<Func<bool>> iCondition, bool iUnavailableWhenExecuting = true)
        {
            return new AsyncBasicRelayCommand(execute, iCondition, iUnavailableWhenExecuting);
        }

        static public IDynamicCommand InstanciateAsync(Action execute, bool iUnavailableWhenExecuting = true)
        {
            return new AsyncBasicRelayCommand(execute, iUnavailableWhenExecuting);
        }


        static public ICommand Empty()
        {
            return new EmptyCommand();
        }
    }
}
