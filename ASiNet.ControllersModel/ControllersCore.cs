using ASiNet.ControllersModel.Base;
using ASiNet.ControllersModel.Exceptions;

namespace ASiNet.ControllersModel;

public delegate void FillControllersDelegate(Dictionary<string, ControllerFactory> dist);

public class ControllersCore
{
    public ControllersCore(FillControllersDelegate? fillControllers = null)
    {
        _controllers = new(() => Init(fillControllers));
    }

    private Lazy<Dictionary<string, ControllerFactory>> _controllers;

    public Controller GetControllerByName(string name)
    {
        if(_controllers.Value.TryGetValue(name, out var factory))
            return factory.CreateNew();
        throw new ControllerNotFoundException(name);
    }


    public bool TryGetControllerByName(string name, out Controller? controller)
    {
        if (_controllers.Value.TryGetValue(name, out var factory))
        {
            controller = factory.CreateNew();
            return true;
        }
        controller = null;
        return false;
    }


    private Dictionary<string, ControllerFactory> Init(FillControllersDelegate? fcd)
    {
        var result = new Dictionary<string, ControllerFactory>();
        if(fcd == null)
            FindAllControllers.Fill(result);
        else
            fcd.Invoke(result);
        return result;
    }
}
