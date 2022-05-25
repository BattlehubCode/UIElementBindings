using UnityEngine.UIElements;

namespace Battlehub.UIElements.Bindings
{
    public abstract class BindingCoreElement<TViewModelsEnum> : VisualElement
    {
        public TViewModelsEnum knownViewModel
        {
            get;
            set;
        }
    }
}
