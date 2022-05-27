using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine.UIElements;

namespace Battlehub.UIElements.Bindings
{
    public class OneWayBindingUxmlTraits<TViewModelsEnum> : BaseBindingUxmlTraits<TViewModelsEnum> where TViewModelsEnum : struct, IConvertible
    {
        private UxmlStringAttributeDescription m_viewModelPropertyName =
            new UxmlStringAttributeDescription { name = "view-model-property-name", defaultValue = "" };

        private UxmlStringAttributeDescription m_viewPropertyName =
            new UxmlStringAttributeDescription { name = "view-property-name", defaultValue = "" };

        public override IEnumerable<UxmlAttributeDescription> uxmlAttributesDescription
        {
            get 
            {
                yield return m_viewName;
                yield return m_viewPropertyName;
                yield return m_knownViewModelName;
                yield return m_viewModelPropertyName;   
            }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);

            OneWayBinding<TViewModelsEnum> oneWayBinding = ve as OneWayBinding<TViewModelsEnum>;
            oneWayBinding.viewModelPropertyName = m_viewModelPropertyName.GetValueFromBag(bag, cc);
            oneWayBinding.viewPropertyName = m_viewPropertyName.GetValueFromBag(bag, cc);
        }
    }

    public class OneWayBinding<TViewModelsEnum> : BaseBinding<TViewModelsEnum>
    {
        public string viewModelPropertyName
        {
            get;
            set;
        }

        public string viewPropertyName
        {
            get;
            set;
        }

        protected PropertyInfo viewPropertyInfo
        {
            get;
            private set;
        }

        protected PropertyInfo viewModelPropertyInfo
        {
            get;
            private set;
        }

        private INotifyPropertyChanged m_notifyPropertyChanged;

        protected object viewModelPropertyValue
        {
            get { return viewModelPropertyInfo.GetValue(viewModel); }
        }

        protected override void Bind()
        {
            viewPropertyInfo = view.GetType().GetProperty(viewPropertyName);
            if(viewPropertyInfo == null)
            {
                throw new BindingException($"view property {viewPropertyName} not found");
            }

            viewModelPropertyInfo = viewModel.GetType().GetProperty(viewModelPropertyName);
            if (viewModelPropertyInfo == null)
            {
                throw new BindingException($"view model property {viewModelPropertyName} not found");
            }

            m_notifyPropertyChanged = viewModel as INotifyPropertyChanged;
            if (m_notifyPropertyChanged != null)
            {
                m_notifyPropertyChanged.PropertyChanged += OnPropertyChanged;
            }

            SyncWithViewModel();
        }

        protected override void Unbind()
        {
            viewPropertyInfo = null;
            viewModelPropertyInfo = null;
            if (m_notifyPropertyChanged != null)
            {
                m_notifyPropertyChanged.PropertyChanged -= OnPropertyChanged;
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == viewModelPropertyName)
            {
                SyncWithViewModel();
            }
        }

        private void SyncWithViewModel()
        {
            viewPropertyInfo.SetValue(view, viewModelPropertyValue);
        }
    }
}

