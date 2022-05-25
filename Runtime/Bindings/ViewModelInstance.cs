using Battlehub.UIElements.ViewModels;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Battlehub.UIElements.Bindings
{
    public class ViewModelInstanceUxmlTraits<TViewModelsEnum> : BindingCoreElementTraits<TViewModelsEnum> where TViewModelsEnum : struct, IConvertible
    {
    }

    public class ViewModelInstance<TViewModelsEnum> : BindingCoreElement<TViewModelsEnum>
    {
        public object instance
        {
            get;
            set;
        }
        
        public ViewModelInstance()
        {
            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        private void OnAttachToPanel(AttachToPanelEvent e)
        {
            if(!Application.isPlaying)
            {
                return;
            }

            instance = knownViewModel.Instantiate();
            IViewModel viewModel = instance as IViewModel;
            if(viewModel != null)
            {
                viewModel.Awake();
            }
        }

        private void OnDetachFromPanel(DetachFromPanelEvent e)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            IViewModel viewModel = instance as IViewModel;
            if(viewModel != null)
            {
                viewModel.OnDestroy();
            }

            IDisposable disposable = instance as IDisposable;
            if(disposable != null)
            {
                disposable.Dispose();
            }

            instance = null;
        }
    }
}
