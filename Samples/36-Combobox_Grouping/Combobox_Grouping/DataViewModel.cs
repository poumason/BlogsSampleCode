using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Combobox_Grouping
{
    public class DataViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private List<ComboxTiem>? _comboxList;
        public List<ComboxTiem>? ComboBoxList
        {
            get
            {
                return _comboxList;
            }
            set
            {
                _comboxList = value;
                OnPropertyChanged();
            }
        }

        private ListCollectionView? _lcv;
        public ListCollectionView Lcv
        {
            get
            {
                return _lcv;
            }
            set
            {
                _lcv = value;
                OnPropertyChanged();
            }
        }

        public DataViewModel()
        {
            List<Item> items = new List<Item>() {
            new Item() {Name="pou", Group="father", Age = 37},
            new Item() {Name="pou1", Group="father", Age = 20},
            new Item() {Name="emily", Group="monther", Age = 27},
            new Item() {Name="emily1", Group="monther", Age = 17},
            new Item() {Name="emily2", Group="monther", Age = 7},
            new Item() {Name="xiao", Group="children", Age = 3},
            };

            ListCollectionView lcv = new ListCollectionView(items);
            lcv.GroupDescriptions.Add(new PropertyGroupDescription("Group"));

            List<ComboxTiem> cbList = new List<ComboxTiem>();
            ComboxTiem cb = new ComboxTiem();
            cb.Items = lcv;
            cbList.Add(cb);
            ComboBoxList = cbList;

            this.Lcv = lcv;
        }

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class Item
    {
        public string? Name { get; set; }

        public string? Group { get; set; }

        public int Age { get; set; } = 0;
    }

    public class ComboxTiem
    {
        public ListCollectionView? Items { get; set; }
    }
}
