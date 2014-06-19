using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicCollection.Infra;
using System.Collections.ObjectModel;

namespace MusicCollectionTest.TestObjects
{
    public class CompleteObservableCollectionImpl<T> : ObservableCollection<T>, ICompleteObservableCollection<T>
    {
    }
}
