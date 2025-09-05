using System;

// Simulating temperature sensor and then if the temperature is high then alert.

namespace Exercises
{

    public class TemperatureEventArgs : EventArgs
    {

        public double Value { get; }
        public TemperatureEventArgs(double value) => Value = value;
    
    }

    public class TemperatureSensor
    {
        public event EventHandler<TemperatureEventArgs>? ThresholdExceeded;

        private readonly double _threshold;
        private readonly Random _rnd = new();

        public TemperatureSensor(double threshold) => _threshold = threshold;

        public void Run()
        {

            for (int i = 0; i < 100; i++)
            {
                var temp = 15 + _rnd.NextDouble() * 20; // 15 - 35 degrees
                Console.WriteLine($"Measured: {temp:F1} °C");

                if (temp > _threshold)
                {
                    ThresholdExceeded?.Invoke(this, new TemperatureEventArgs(temp));
                }

                System.Threading.Thread.Sleep(500); // 500 ms sleep

            }

        }


    }


    public static class ProgramEventEx1
    {
        public static void Main(string[] args)
        {
            var sensor = new TemperatureSensor(threshold: 30.0);
            // Subscriber 
            sensor.ThresholdExceeded += (s, e) => Console.WriteLine($"Alert: High temperature {e.Value:F1} °C!");
            sensor.Run();
        }

    }
}