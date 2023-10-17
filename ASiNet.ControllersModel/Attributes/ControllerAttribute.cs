namespace ASiNet.ControllersModel.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ControllerAttribute : Attribute
{
    public ControllerAttribute(string name)
    {
        ControllerName = name;
    }


    public string ControllerName { get; set; }
}
