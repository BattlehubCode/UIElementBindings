using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Battlehub.UIElements.Bindings
{
    public class BaseBindingUxmlTraits<TViewModelsEnum> : BindingCoreElementTraits<TViewModelsEnum> where TViewModelsEnum : struct, IConvertible
    {
        protected UxmlStringAttributeDescription m_viewName =
            new UxmlStringAttributeDescription { name = "view-name" };

        public override IEnumerable<UxmlAttributeDescription> uxmlAttributesDescription
        {
            get
            {
                yield return m_viewName;
                yield return m_knownViewModelName;
            }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);

            BaseBinding<TViewModelsEnum> baseBinding = ve as BaseBinding<TViewModelsEnum>;
            baseBinding.viewName = m_viewName.GetValueFromBag(bag, cc);
        }
    }

    public abstract class BaseBinding<TViewModelsEnum> : BindingCoreElement<TViewModelsEnum>
    {
        public string viewName
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

        private VisualElement GetRoot()
        {
            VisualElement element = hierarchy.parent;
            if (element == null)
            {
                return this;
            }

            while (element.parent != null)
            {
                element = element.parent;
            }

            return element;
        }

        protected VisualElement FindView()
        {
            VisualElement view;
            if (string.IsNullOrEmpty(viewName))
            {
                view = hierarchy.parent;
            }
            else
            {
                VisualElement root = GetRoot();
                view = root.Q(viewName);
            }
            return view;
        }

        public BaseBinding()
        {
            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        protected virtual void OnAttachToPanel(AttachToPanelEvent e)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            viewModel = knownViewModel.FindViewModelFor(this);
            if (viewModel == null)
            {
                LogError($"ViewModel {KnownViewModelsUtils.EnumToTypeName(knownViewModel.ToString())} not found");
                return;
            }

            view = FindView();
            if (view == null)
            {
                LogError($"View {viewName} not found");
                return;
            }

            try
            {
                Bind();
            }
            catch (BindingException exc)
            {
                LogError(exc.Message);
            }
        }

        protected virtual void OnDetachFromPanel(DetachFromPanelEvent e)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (viewModel == null)
            {
                return;
            }

            if (view == null)
            {
                return;
            }

            Unbind();

            viewModel = null;
            view = null;
        }

        protected abstract void Bind();

        protected abstract void Unbind();
    }
}