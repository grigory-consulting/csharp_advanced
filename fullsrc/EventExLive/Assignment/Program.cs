// Assignment: Alarm System with Two Interfaces

// 1. define IAlarm with a method void OnAlarm(string message)

// 2. Define an interface IEventSubscriber with a method void SubscribeTo(MotionSensor sensor)
// (any subscriber class can register itself to the event)

// 3. Create a class MotionSensor which declares the event AlarmTriggered of type EventHandler<string>,
// The method DetectMotion prints "Motion detected" to the console 
// and raises an event with message "Intruder detected" 

// 4. Implement a class AlarmService which implements both IAlarm and IEventSubscriber

// 5. in Main()
// Simulate motion detection by calling DetectMotion()

using System;

public interface IAlarm
{
    void OnAlarm(string msg);
}
public interface IEventSubscriber
{
    void SubscribeTo(MotionSensor sensor);
}

public class MotionSensor
{
    public event EventHandler<string>? AlarmTriggered;

    public void DetectMotion()
    {
        Console.WriteLine("Motion detected!");
        AlarmTriggered?.Invoke(this, "Intruder detected!");
    }
}

public class AlarmService : IAlarm, IEventSubscriber
{
    private readonly string _name;
    public AlarmService(string name) => _name = name;

    public void OnAlarm(string msg)
        => Console.WriteLine($"[ALARM-{_name}] {msg}");

    public void SubscribeTo(MotionSensor sensor)
    {
        sensor.AlarmTriggered += (s, e) => OnAlarm(e);
    }
}

public static class Program
{
    public static void Main()
    {
        var sensor = new MotionSensor();

        var alarm1 = new AlarmService("A");
        var alarm2 = new AlarmService("B");

        alarm1.SubscribeTo(sensor);
        alarm2.SubscribeTo(sensor);

        sensor.DetectMotion();
        sensor.DetectMotion();
    }
}
