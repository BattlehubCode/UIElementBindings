using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine.UIElements;

namespace Battlehub.UIElements.Bindings
{
    public class OneWayBindingUxmlTraits<TViewModelsEnum> : BindingCoreElementTraits<TViewModelsEnum> where TViewModelsEnum : struct, IConvertible
    {
        private UxmlStringAttributeDescription m_viewModelPropertyName =
            new UxmlStringAttributeDescription { name = "view-model-property-name", defaultValue = "" };

        private UxmlStringAttributeDescription m_viewPropertyName =
            new UxmlStringAttributeDescription { name = "view-property-name", defaultValue = "" };

        public override IEnumerable<UxmlAttributeDescription> uxmlAttributesDescription
        {
            get { return base.uxmlAttributesDescription.Union(new UxmlAttributeDescription[] { m_viewModelPropertyName, m_viewPropertyName }); }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);

            OneWayBinding<TViewModelsEnum> oneWayBinding = ve as OneWayBinding<TViewModelsEnum>;
            oneWayBinding.viewModelPropertyName = m_viewModelPropertyName.GetValueFromBag(bag, cc);
            oneWayBinding.viewPropertyName = m_viewPropertyName.GetValueFromBag(bag, cc);
        }
    }

    public class OneWayBinding<TViewModelsEnum> : BindingCoreElement<TViewModelsEnum>
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

        protected object view
        {
            get;
            private set;
        }
        
        protected object viewModel
        {
            get;
            private set;
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

        public OneWayBinding()
        {
            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        private void OnAttachToPanel(AttachToPanelEvent e)
        {
            viewModel = knownViewModel.FindViewModelFor(this);
            if (viewModel == null)
            {
                return;
            }
            Bind();
        }

        private void OnDetachFromPanel(DetachFromPanelEvent e)
        {
            if (viewModel == null)
            {
                return;
            }

            Unbind();
        }


        protected virtual void Bind()
        {
            view = hierarchy.parent;
            viewPropertyInfo = view.GetType().GetProperty(viewPropertyName);
            viewModelPropertyInfo = viewModel.GetType().GetProperty(viewModelPropertyName);
            m_notifyPropertyChanged = viewModel as INotifyPropertyChanged;
            if (m_notifyPropertyChanged != null)
            {
                m_notifyPropertyChanged.PropertyChanged += OnPropertyChanged;
            }

            SyncWithViewModel();
        }

        protected virtual void Unbind()
        {
            viewModel = null;
            view = null;
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

