namespace Battlehub.UIElements.Bindings
{
    [System.Serializable]
    public class BindingException : System.Exception
    {
        public BindingException() { }
        public BindingException(string message) : base(message) { }
        public BindingException(string message, System.Exception inner) : base(message, inner) { }
        protected BindingException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
