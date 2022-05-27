using System;
using System.Linq;
using UnityEngine.UIElements;

namespace Battlehub.UIElements.Bindings
{
    public class TwoWayBindingUxmlTraits<TViewModelsEnum> : OneWayBindingUxmlTraits<TViewModelsEnum> where TViewModelsEnum : struct, IConvertible
    {
    }

    public class TwoWayBinding<TViewModelsEnum> : OneWayBinding<TViewModelsEnum>
    {
        private Delegate m_callback;

        protected override void Bind()
        {
            base.Bind();

            var callbackEventHandler = view as CallbackEventHandler;
            if (callbackEventHandler != null)
            {
                Type eventType = GetEventType();

                m_callback = CreateCallback(eventType, nameof(TwoWayBinding<TViewModelsEnum>.OnCallback));

                m_callback.DynamicInvoke(new object[1] { Activator.CreateInstance(eventType) });
                RegisterCallback(eventType, m_callback);
            }
        }

        protected override void Unbind()
        {
            if (m_callback != null)
            {
                Type eventType = GetEventType();
                UnregisterCallback(eventType, m_callback);
            }

            base.Unbind();
        }

        private Type GetEventType()
        {
            return typeof(ChangeEvent<>).MakeGenericType(viewModelPropertyInfo.PropertyType);
        }

        public void OnCallback<T>(T v)
        {
            SyncWithView();
        }

        private void SyncWithView()
        {
            viewModelPropertyInfo.SetValue(viewModel, viewPropertyInfo.GetValue(view));
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

