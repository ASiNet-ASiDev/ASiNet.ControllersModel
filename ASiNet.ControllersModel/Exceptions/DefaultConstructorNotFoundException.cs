namespace ASiNet.ControllersModel.Exceptions;
public class DefaultConstructorNotFoundException : Exception
{
    public DefaultConstructorNotFoundException(Type type)
    {
        InstanceType = type;
    }

    public Type InstanceType { get; }
}
