using ASiNet.ControllersModel;
using ASiNet.ControllersModel.Attributes;

var cc = new ControllersCore();

var c = cc.GetControllerByName("test1");

Console.WriteLine(c.Execute("sum", 10, 10));

Console.WriteLine(c.Execute("sum", 50, 20));

Console.ReadLine();


[Controller("test1")]
public class Test1
{

    [Executable("sum")]
    public int Sum(int a, int b)
    {
        return a + b;
    }
}