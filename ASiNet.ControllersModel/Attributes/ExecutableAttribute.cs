namespace ASiNet.ControllersModel.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class ExecutableAttribute : Attribute
{
    public ExecutableAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
}
