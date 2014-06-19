using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace MusicCollectionTest.TestObjects
{
    public interface IObject
    {
        string Name
        {
            set;
            get;
        }

        IObject MyFriend
        {
            set;
            get;
        }

        int Value
        {
            set;
            get;
        }

        int ID
        {
            get;
        }

        //ObservableCollection<MyObject> MyFriends
        //{
        //    get;
        //}
    }
}
