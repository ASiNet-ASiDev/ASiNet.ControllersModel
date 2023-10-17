using System.Reflection;
using ASiNet.ControllersModel.Attributes;

namespace ASiNet.ControllersModel.Base;
internal static class FindAllControllers
{
    public static void Fill(Dictionary<string, ControllerFactory> dist)
    {
        foreach (var ass in AppDomain.CurrentDomain.GetAssemblies()) 
        {
            foreach (var cinfo in ass.GetTypes().Select(x => (Type: x, Attr: x.GetCustomAttribute<ControllerAttribute>())).Where(x => x.Attr is not null))
            {
                var factory = new ControllerFactory(cinfo.Type);
                dist.Add(cinfo.Attr!.ControllerName, factory);
            }
        }
    }

}
