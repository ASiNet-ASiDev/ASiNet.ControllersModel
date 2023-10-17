using System.Linq.Expressions;
using System.Reflection;
using ASiNet.ControllersModel.Attributes;
using ASiNet.ControllersModel.Exceptions;

namespace ASiNet.ControllersModel.Base;

public delegate object ExecuteDelegate(string methodName, object inst, params object[] parameters);

public class ControllerFactory
{
    public ControllerFactory(Type controllerType)
    {
        Init(controllerType);
    }


    private Func<object> _defaultConstructor = null!;
    private ExecuteDelegate _executeDelegate = null!;


    public Controller CreateNew()
    {
        var inst = _defaultConstructor();
        return new(inst, _executeDelegate);
    }

    private void Init(Type controllerType)
    {
        InitConstructor(controllerType);
        InitExecutable(controllerType);
    }

    private void InitConstructor(Type type)
    {
        try
        {
            _defaultConstructor = Expression.Lambda<Func<object>>(Expression.Convert(Expression.New(type), typeof(object))).Compile();
        }
        catch
        {
            throw new DefaultConstructorNotFoundException(type);
        }
    }

    private void InitExecutable(Type type)
    {
        var instParameter = Expression.Parameter(typeof(object), "instParam");
        var nameParameter = Expression.Parameter(typeof(string), "name");
        var parametersParameter = Expression.Parameter(typeof(object[]), "parameters");
        var resultV = Expression.Variable(typeof(object), "result");
        var inst = Expression.Variable(type, "inst");

        var parameters = new ParameterExpression[] { nameParameter, instParameter, parametersParameter};
        var variables = new ParameterExpression[] { resultV, inst };

        var instrustions = new List<Expression>
        {
            Expression.Assign(inst, Expression.Convert(instParameter, type)),
            ExecuteMethodsBuyName(type, resultV, nameParameter, inst, parametersParameter),
            resultV
        };

        var body = Expression.Block(variables, instrustions);

        var lambda = Expression.Lambda<ExecuteDelegate>(body, parameters);
        _executeDelegate = lambda.Compile();
    }

    private Expression ExecuteMethodsBuyName(Type type, Expression result, Expression name, Expression inst, Expression parameters)
    {
        var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Select(x => (Mi: x, Attr: x.GetCustomAttribute<ExecutableAttribute>()))
            .Where(x => x.Attr is not null);

        var cases = new List<SwitchCase>();
        foreach (var method in methods)
        {
            cases.Add(Expression.SwitchCase(
                ExecuteMethod(method.Mi, result, inst, parameters), 
                Expression.Constant(method.Attr!.Name)));
        }

        var r = Expression.Switch(name, 
            Expression.Throw(
                Expression.Constant(new MethodNotFoundException())), 
            cases.ToArray());
        return r;
    }

    private Expression ExecuteMethod(MethodInfo mi, Expression result, Expression inst, Expression parameters)
    {
        Expression eBlock = mi.ReturnType == typeof(void) ? 
            Expression.Block(
                Expression.Call(inst, mi, ConvertParameters(mi, parameters)),
                Expression.Assign(result, Expression.Constant(null, typeof(object))))
            : Expression.Assign(result, Expression.Convert(Expression.Call(inst, mi, ConvertParameters(mi, parameters)), typeof(object)));

        return Expression.IfThenElse(
            CompareParameters(mi, parameters),
            eBlock,
            Expression.Throw(
                Expression.Constant(new ParametersMismathException())));
    }

    private Expression CompareParameters(MethodInfo mi, Expression parameters)
    {
        var mp = mi.GetParameters();

        var compare = Expression.Equal(Expression.Constant(mp.Length), Expression.ArrayLength(parameters));

        var i = 0;
        foreach (var parameter in mp)
        {
            compare = Expression.And(compare, 
                Expression.TypeIs(
                    Expression.ArrayAccess(
                        parameters, 
                        Expression.Constant(i)
                        ),
                     parameter.ParameterType
                    )
                );
            i++;
        }

        return compare;
    }

    private IEnumerable<Expression> ConvertParameters(MethodInfo mi, Expression parameters)
    {
        var i = 0;
        foreach (var p in mi.GetParameters())
        {
            yield return Expression.Convert(Expression.ArrayAccess(parameters, Expression.Constant(i)), p.ParameterType);
            i++;
        }
    }
}
