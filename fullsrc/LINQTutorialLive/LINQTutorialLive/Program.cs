using Bogus;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Tracing;
using System.Threading.Tasks.Dataflow;


namespace LINQTutorial
{
    [EventSource(Name = "LINQTutorial")]
    public class SensorEventSource : EventSource
    {
        public static readonly SensorEventSource Log = new SensorEventSource();

        [Event(1, Message = "Starting LINQ analytics")]
        public void LinqStart() => WriteEvent(1);

        [Event(2, Message = "Finished LINQ analytics")]
        public void LinqEnd () => WriteEvent(2);

        [Event(4, Level = EventLevel.Critical,Message = "Anomaly")]
        public void BigJump(int sensorId, double delta) => WriteEvent(4, sensorId, delta);

        [Event(5, Level = EventLevel.Informational, Message = "Batch of size {0} processed")]
        public void BatchProcessed(int BatchSize) => WriteEvent(5, BatchSize);

    }



    public class SensorReading
    {
        public int SensorId { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double CO2 { get; set; }
        public double Photosynthesis { get; set; }
        public double Radiation { get; set; }
        public DateTime TimeStamp { get; set; }

    }

    public class SensorLocation
    {
        public int SensorId { get; set; }
        public string City { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }


    class Program
    {
        static void Main()
        {

            var faker = new Faker<SensorReading>()
                .RuleFor(s => s.SensorId, f => f.Random.Int(1, 5))
                .RuleFor(s => s.Temperature, f => f.Random.Double(15.0, 35.0))
                .RuleFor(s => s.Humidity, f => f.Random.Double(30.0, 90.0))
                .RuleFor(s => s.CO2, f => f.Random.Double(400.0, 1200.0)) // ppm
                .RuleFor(s => s.Photosynthesis, f => f.Random.Double(0.0, 25.0))
                .RuleFor(s => s.Radiation, f => f.Random.Double(0, 1200.0))
                .RuleFor(s => s.TimeStamp, f => f.Date.Recent(1));

            var faker2 = new Faker<SensorLocation>()
                .RuleFor(s => s.SensorId, f => f.IndexFaker + 1)
                .RuleFor(s => s.City, f => f.Address.City())
                .RuleFor(s => s.Latitude, f=> f.Address.Latitude())
                .RuleFor(s => s.Longitude, f => f.Address.Longitude());


            List<SensorReading> readings = faker.Generate(1_000);
            List<SensorLocation> locations = faker2.Generate(5);


            SensorEventSource.Log.LinqStart();

            // LINQ = SQL-like query ... apply to any collection that implements IEnumerable<T> interface 

            // Average Temperature per Sensor

            var avgTemp = readings.GroupBy(r=>r.SensorId).Select(v => new {SensorId = v.Key, AverageTemperature = v.Average(r  => r.Temperature)});

            foreach (var sensor in avgTemp.OrderBy(r => r.SensorId))
            {
                Console.WriteLine($"{sensor.SensorId}: {sensor.AverageTemperature} °C");
            }


            foreach(var group in readings.GroupBy(r => r.SensorId)) 
                Console.WriteLine($"{group.Count()}:{group.Key}");
            // Filter and Projections

            var hotReadings = readings.Where(r => r.Temperature > 30);
            bool highCO2 = readings.Any(r => r.CO2 > 1000);
            bool allHumid = readings.All(r => r.Humidity > 40);
            var rads = readings.Select(r => new { r.TimeStamp, r.Radiation });
            var alerts = readings.Where(r => r.Temperature > 32 || r.Humidity < 35);

            // Time Filtering

            var yesterday = DateTime.Now.AddDays(-1);
            var lastDayReading = readings.Where(r => r.TimeStamp >= yesterday); // last 24 hours 

            // Detecting Big Changes 

            var bigJumps = readings
                .AsParallel().WithDegreeOfParallelism(4)
                .OrderBy(r => r.SensorId)
                .ThenBy(r => r.TimeStamp)
                .GroupBy(r => r.SensorId)
                .SelectMany(group => group.Zip(group.Skip(1), (prev, curr) => new
                {
                    SensorId = curr.SensorId,
                    Prev = prev,
                    Curr = curr,
                    Delta = Math.Abs(curr.Temperature - prev.Temperature)
                })
                .Where(x => x.Delta > 5.0));

            foreach (var jump in bigJumps)
            {
                SensorEventSource.Log.BigJump(jump.SensorId, jump.Delta);
            }

            var readingWithLocation = from r in readings  
                                      join location in locations on r.SensorId equals location.SensorId
                                      select new {
                                          r.SensorId,
                                          location.City,
                                          location.Latitude,
                                          location.Longitude,
                                          r.TimeStamp,
                                          r.Temperature,
                                          r.CO2,
                                          r.Humidity,
                                          r.Photosynthesis,
                                          r.Radiation
                                      };

            ConcurrentBag<(int SensorId, double Result)> results = new();
            Parallel.ForEach(readings, (reading) =>
            {

                double result = Math.Sqrt(reading.Temperature * reading.Humidity);
                results.Add((reading.SensorId, result));

            });


            // say CO2 > 1200 then trigger alarm. Count the number of alarms per City 

            var alarmsPerCity = readingWithLocation
                .Where(r => r.CO2 > 1200)
                .GroupBy(r => r.City)
                .Select(group => new { City = group.Key, AlarmCount = group.Count() });

            // For each sensor compute earliest and latest timestamp and its duration (max - min)

            var durations = readings
                .GroupBy(r => r.SensorId)
                .Select(group => new
                {
                    SensorId = group.Key,
                    MinTime = group.Min(r => r.TimeStamp),
                    MaxTime = group.Max(r => r.TimeStamp),
                    Duration = group.Max(r => r.TimeStamp) - group.Min(r => r.TimeStamp),

                });


            // How many readings did each sensor report per hour

            var readingsPerHour = readings
                .GroupBy(r => new { r.SensorId, Hour = r.TimeStamp.Hour })
                .Select(group => new
                {
                    group.Key.SensorId,
                    group.Key.Hour,
                    Count = group.Count()
                });


            // Processing the data in chunks. Important in Machine Learning 

            foreach(var batch in readingWithLocation.Chunk(1024))
            {
                SensorEventSource.Log.BatchProcessed(1024);
            }


            SensorEventSource.Log.LinqEnd();


        }
    }

}