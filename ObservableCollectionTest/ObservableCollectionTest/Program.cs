using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservableCollectionTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ObservableCollection<string> strlist = new ObservableCollection<string>();
            strlist.Add("1");
                strlist.Add("1");

        }
    }
}
