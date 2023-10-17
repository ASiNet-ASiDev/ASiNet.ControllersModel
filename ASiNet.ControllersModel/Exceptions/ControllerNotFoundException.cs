using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASiNet.ControllersModel.Exceptions;
public class ControllerNotFoundException : Exception
{
    public ControllerNotFoundException(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
}
