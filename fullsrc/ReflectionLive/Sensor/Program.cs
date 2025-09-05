
using System.Reflection;


[Obsolete, InformativeAttribute("")]
public class InformativeAttributeAttribute : Attribute
{
    [InformativeAttribute("")]
    public string Info { get; }
    public InformativeAttributeAttribute(string info) => Info = info; 
}

[InformativeAttribute("Sensor class")]
public class Sensor
{
    public string SensorId { get; set; }
    public string Reading { get; set; }

    public Sensor(string sensorId, string reading)
    {
        SensorId = sensorId;
        Reading = reading;
    }
    [Obsolete]
    public void Calibrate(string msg)
    {
        Console.WriteLine($"Sensor {SensorId} has been calibrated");
    }
}

class Program
{
    static void Main()
    {
        // Get the Type object for Sensor
        Type type = typeof(Sensor);

        // list all public properties
        Console.WriteLine("Sensor properties");
        foreach (var prop in type.GetProperties())
        {
            Console.WriteLine($"{prop.Name} ({prop.PropertyType.Name})");
        }

        // list all public methods 
        Console.WriteLine("Sensor properties");
        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            Console.WriteLine($"{method.Name} ({method.ReturnType.Name})");
            var parameters = method.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                Console.WriteLine(parameters[i].Name);
            }
        }

        // create a Sensor instance
        object[] args = ["ID1", "23.5"];
        object sensor = Activator.CreateInstance(type, args);

        // Get the property values
        Console.WriteLine($"SensorId {type.GetProperty("SensorId").GetValue(sensor)}");
        Console.WriteLine($"Reading {type.GetProperty("Reading").GetValue(sensor)}");

        //Set the property values
        type.GetProperty("SensorId").SetValue(sensor, "ID2");
        type.GetProperty("Reading").SetValue(sensor, "19.5");

        // Invoke a method

        var calibrateMethod = type.GetMethod("Calibrate");
        object[] input_args = ["Message",];
        calibrateMethod.Invoke(sensor, input_args);


        // List all obsolete methods
        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            var obsolete = method.GetCustomAttribute<ObsoleteAttribute>();
            if (obsolete != null)
                Console.WriteLine($"Obsolete: {method.Name}");

        }

    }
}