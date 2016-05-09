namespace Prism.Properties
{
    public static class Resources
    {
        public static readonly string CannotRegisterCompositeCommandInItself =
            "Cannot register a CompositeCommand in itself.";

        public static readonly string CannotRegisterSameCommandTwice =
            "Cannot register the same command twice in the same CompositeCommand.";

        public static readonly string DefaultDebugLoggerPattern = "{1}: {2}. Priority: {3}. Timestamp:{0:u}.";

        public static readonly string DelegateCommandDelegatesCannotBeNull =
            "Neither the executeMethod nor the canExecuteMethod delegates can be null.";

        public static readonly string DelegateCommandInvalidGenericPayloadType =
            "T for DelegateCommand<T> is not an object nor Nullable.";

        public static readonly string EventAggregatorNotConstructedOnUIThread =
            "To use the UIThread option for subscribing, the EventAggregator must be constructed on the UI thread.";

        public static readonly string InvalidDelegateRerefenceTypeException =
            "Invalid Delegate Reference Type Exception";

        public static readonly string InvalidPropertyNameException =
            "The entity does not contain a property with that name";

        public static readonly string PropertySupport_ExpressionNotProperty_Exception =
            "The member access expression does not access a property.";

        public static readonly string PropertySupport_NotMemberAccessExpression_Exception =
            "The expression is not a member access expression.";

        public static readonly string PropertySupport_StaticExpression_Exception =
            "The referenced property is a static property.";
    }
}
