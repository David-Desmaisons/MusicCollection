using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

using MusicCollection.Fundation;
using MusicCollection.Properties;
using MusicCollection.Infra;
using MusicCollection.Implementation.Session;

namespace MusicCollection.SettingsManagement
{
    [Serializable]
    public class PersistentColumns : List<PersistentColumn>
    {
    }

    [Serializable]
    public class PersistentColumn
    {
        public PersistentColumn()
        {
        }

        public PersistentColumn(int iIndex)
        {
            Index = iIndex;
        }

        public bool Visibility { get; set; }

        public int DisplayIndex { get; set; }

        public double Width { get; set; }

        public int Index { get; set; }
    }

    internal class UIGridManagement : IUIGridManagement
    {
        private class PersistGrid : IPersistGrid
        {
            private IAparencyUserSettings _IAparencyUserSettings;
            internal PersistGrid(IAparencyUserSettings iAparencyUserSettings)
            {
                _IAparencyUserSettings = iAparencyUserSettings;
            }

            public void PersistChange(IList<DataGridColumn> columns)
            {
                var newgridperisted = new PersistentColumns();                               
                newgridperisted.AddRange(columns.Select((c, i) => new PersistentColumn(i) { Visibility = (c.Visibility == Visibility.Visible), DisplayIndex = c.DisplayIndex, Width = c.Width.DisplayValue }));
                _IAparencyUserSettings.TrackGrid = newgridperisted; 
                //Settings.Default.TrackGrid = newgridperisted; 

                ////columns.Select((c, i) => new PersistentColumn(i) { Visibility = (c.Visibility == Visibility.Visible), DisplayIndex = c.DisplayIndex, Width = c.Width.DisplayValue }).Apply(c => Settings.Default.TrackGrid.Add(c));
            }

            public void FromPersistance(IList<DataGridColumn> cols)
            {
                PersistentColumns pc = _IAparencyUserSettings.TrackGrid;

                if (pc == null)
                    return;

                var sorted = pc.OrderBy(i => i.DisplayIndex);
                foreach (var item in sorted)
                {
                    DataGridColumn dc = cols[item.Index];
                    dc.DisplayIndex = item.DisplayIndex;
                    dc.Visibility = item.Visibility ? Visibility.Visible : Visibility.Collapsed;
                    dc.Width = item.Width;
                }
            }
        }

        private PersistGrid _Unic;
        private IAparencyUserSettings _IAparencyUserSettings;

        internal UIGridManagement(IMusicSettings MS)
        {
            _IAparencyUserSettings = MS.AparencyUserSettings;
        }

        private PersistGrid Unic
        {
            get
            {
                if (_Unic == null)
                    _Unic = new PersistGrid(_IAparencyUserSettings);

                return _Unic;
            }
        }

        public IPersistGrid Default { get { return Unic; } }

        //public IPersistGrid GetFromName(string GriName)
        //{
        //    throw new NotImplementedException("Only One grid supported");
        //}
    }


}
