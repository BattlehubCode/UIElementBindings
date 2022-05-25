using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.UIElements;

namespace Battlehub.UIElements.Bindings
{
    public class EventBindingUxmlTraits<TViewModelsEnum> : BindingCoreElementTraits<TViewModelsEnum> where TViewModelsEnum : struct, IConvertible
    {
        private UxmlStringAttributeDescription m_viewModelMethodName =
            new UxmlStringAttributeDescription { name = "view-model-method-name", defaultValue = "" };

        public override IEnumerable<UxmlAttributeDescription> uxmlAttributesDescription
        {
            get { return base.uxmlAttributesDescription.Union(new UxmlAttributeDescription[] { m_viewModelMethodName }); }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);

            EventBinding<TViewModelsEnum> eventBinding = ve as EventBinding<TViewModelsEnum>;
            eventBinding.viewModelMethodName = m_viewModelMethodName.GetValueFromBag(bag, cc);
        }
    }

    public class EventBinding<TViewModelsEnum> : BindingCoreElement<TViewModelsEnum> where TViewModelsEnum : struct, IConvertible
    {
        public string viewModelMethodName
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

        protected MethodInfo viewModelMethodInfo
        {
            get;
            private set;
        }

        protected Delegate callback
        {
            get;
            private set;
        }

        public EventBinding()
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
            viewModelMethodInfo = viewModel.GetType().GetMethod(viewModelMethodName);
         
            var callbackEventHandler = view as CallbackEventHandler;
            if (callbackEventHandler != null)
            {
                Type eventType = GetEventType();

                callback = CreateCallback(eventType, nameof(OnCallback));
                RegisterCallback(eventType, callback);
            }
        }

        protected virtual void Unbind()
        {
            if (callback != null)
            {
                Type eventType = GetEventType();
                UnregisterCallback(eventType, callback);
            }
        }

        private Type GetEventType()
        {
            return typeof(ClickEvent);
        }

        public void OnCallback<T>(T _)
        {
            viewModelMethodInfo.Invoke(viewModel, null);
        }

        protected Delegate CreateCallback(Type eventType, string methodName)
        {
            var callbackMethod = GetType().GetMethod(methodName).MakeGenericMethod(eventType);  
            var eventCallbackType = typeof(EventCallback<>).MakeGenericType(eventType);
            return Delegate.CreateDelegate(eventCallbackType, this, callbackMethod);
        }

        protected void RegisterCallback(Type eventType, Delegate callback)
        {
            string methodName = nameof(CallbackEventHandler.RegisterCallback);

            var registerCallbackMethod = view.GetType().GetMethods().Where(mi => mi.Name == methodName).First();
            registerCallbackMethod = registerCallbackMethod.MakeGenericMethod(eventType);
            registerCallbackMethod.Invoke(view, new object[] { callback, TrickleDown.NoTrickleDown });
        }

        protected void UnregisterCallback(Type eventType, Delegate callback)
        {
            string methodName = nameof(CallbackEventHandler.UnregisterCallback);
            var unregisterCallbackMethod = view.GetType().GetMethods().Where(mi => mi.Name == methodName).First();
            unregisterCallbackMethod = unregisterCallbackMethod.MakeGenericMethod(eventType);
            unregisterCallbackMethod.Invoke(view, new object[] { callback, TrickleDown.NoTrickleDown });
        }
    }
}

