

using System.Reflection;
using System.Reflection.PortableExecutable;


public class Secret
{
    private int pin = 1234;
    private string message = "Top Secret";

    public void Print()
    {
        Console.WriteLine($"{pin}: {message}");
    }

}

class Program
{
    static void Main()
    {
        var secret = new Secret();
        secret.Print();

        var t = typeof( Secret );

        // Change private int field

        var pinField = t.GetField("pin", BindingFlags.NonPublic | BindingFlags.Instance);
        pinField.SetValue(secret, 9999);
        
        var msgField= t.GetField("message", BindingFlags.NonPublic | BindingFlags.Instance);
        msgField.SetValue(secret, "Not So Secret");

        secret.Print() ;

    }
}