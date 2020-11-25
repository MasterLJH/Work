using System.ComponentModel;

namespace BingLibrary.hjb.mvvm
{
    public class DataSourceOriginal : INotifyPropertyChanged
    {
        public string LastPropertyChanged;

        protected void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

            LastPropertyChanged = propName;
        }

        public void NotifyPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
