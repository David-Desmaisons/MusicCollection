using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

using MusicCollection.Infra;
using System.Linq.Expressions;
using MusicCollection.ToolBox.StringExpression;
using System.Collections.Specialized;
using System.Diagnostics;

namespace MusicCollectionWPF.Infra
{
    public class ListGrouper : DependencyObject, IDisposable
    {
        public ListGrouper AddGroup(string igroup)
        {
            GroupName = igroup;
            return this;
        }

        public string GroupName { get; set; }

        private void Unlisten()
        {
            if (_Collection == null)
                return;

            _Collection.CollectionChanged -= collection_CollectionChanged;
            IDisposable disp = _Collection as IDisposable;
            if (disp != null) disp.Dispose();
            _Collection = null;
        }

        private void Listen()
        {
            if (GroupName == null)
                return;

            try
            { 
                dynamic o = _ICollectionView.SourceCollection;
                Listen(o);
            }
            catch(Exception e)
            {
                Trace.WriteLine(string.Format("Problem Grouper behaviour {0}",e));
            }
        }

        private bool Listen(object io)
        {
            return false;
        }

        private bool Listen<T>(IList<T> io) where T : class
        {
            _Collection = io.LiveSelect<T, object>(GroupName);
            if (_Collection == null)
                return false;
            _Collection.CollectionChanged += collection_CollectionChanged;
            return true;
        }

        private void collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_ICollectionView != null)
            {
                _ICollectionView.GroupDescriptions.Clear();
                if (GroupName != null)
                    _ICollectionView.GroupDescriptions.Add(new PropertyGroupDescription(GroupName));
            }
        }

        private ICollectionView _ICollectionView = null;
        private INotifyCollectionChanged _Collection;
        public void Apply(ICollectionView icv)
        {
            if (_ICollectionView == icv)
                return;

            Unlisten();
            _ICollectionView = icv;

            icv.GroupDescriptions.Clear();
            icv.SortDescriptions.Clear();
            if (GroupName != null)
            {
                icv.GroupDescriptions.Add(new PropertyGroupDescription(GroupName));
                icv.SortDescriptions.Add(new SortDescription(GroupName, ListSortDirection.Ascending));
            }
            icv.Refresh();
            Listen();
        }


        public void Dispose()
        {
            Unlisten();
        }
    }
}
