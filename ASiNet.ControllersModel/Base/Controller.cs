namespace ASiNet.ControllersModel.Base;
public class Controller
{
    public Controller(object inst, ExecuteDelegate executable)
    {
        _instance = inst;
        _executable = executable;
    }

    private object _instance;
    private ExecuteDelegate _executable;

    public object Execute(string methodName, params object[] parameters)
    {
        return _executable.Invoke(methodName, _instance, parameters);
    }
}
