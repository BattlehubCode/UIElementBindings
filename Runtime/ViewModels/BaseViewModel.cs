using System.ComponentModel;

namespace Battlehub.UIElements.ViewModels
{
    public class BaseViewModel : IViewModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void Awake()
        {

        }

        public virtual void OnDestroy()
        {
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void RaisePropertyChanged(PropertyChangedEventArgs propertyChangedArgs)
        {
            PropertyChanged?.Invoke(this, propertyChangedArgs);
        }
    }
}
