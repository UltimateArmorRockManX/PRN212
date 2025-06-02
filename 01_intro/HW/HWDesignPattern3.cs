using System;
using System.Collections.Generic;
using System.Linq;

namespace DesignPatterns.Homework
{
	#region Observer Pattern Interfaces

	// The subject interface that all weather stations must implement
	public interface IWeatherStation
	{
		// Methods to manage observers
		void RegisterObserver(IWeatherObserver observer);
		void RemoveObserver(IWeatherObserver observer);
		void NotifyObservers();

		// Weather data properties
		float Temperature { get; }
		float Humidity { get; }
		float Pressure { get; }
	}

	// The observer interface that all display devices must implement
	public interface IWeatherObserver
	{
		void Update(float temperature, float humidity, float pressure);
	}

	#endregion

	#region Weather Station Implementation

	// Concrete implementation of a weather station
	public class WeatherStation : IWeatherStation
	{
		// List to store all registered observers
		private List<IWeatherObserver> _observers;

		// Weather data
		private float _temperature;
		private float _humidity;
		private float _pressure;

		// Constructor
		public WeatherStation()
		{
			_observers = new List<IWeatherObserver>();
		}

		// Methods to register and remove observers
		public void RegisterObserver(IWeatherObserver observer)
		{
			_observers.Add(observer);
			Console.WriteLine("Observer has been registered.");
		}

		public void RemoveObserver(IWeatherObserver observer)
		{
			_observers.Remove(observer);
			Console.WriteLine("Observer has been removed.");
		}

		// Method to notify all observers when weather data changes
		public void NotifyObservers()
		{
			Console.WriteLine("Notifying observers...");
			foreach (var observer in _observers)
			{
				observer.Update(_temperature, _humidity, _pressure);
			}
		}

		// Properties to access weather data
		public float Temperature => _temperature;
		public float Humidity => _humidity;
		public float Pressure => _pressure;

		// Method to update weather data and notify observers
		public void SetMeasurements(float temperature, float humidity, float pressure)
		{
			Console.WriteLine("\n--- Weather Station: Weather measurements updated ---");

			// Update weather data
			_temperature = temperature;
			_humidity = humidity;
			_pressure = pressure;

			Console.WriteLine($"Temperature: {_temperature}ÅãC");
			Console.WriteLine($"Humidity: {_humidity}%");
			Console.WriteLine($"Pressure: {_pressure} hPa");

			// Notify observers of the change
			NotifyObservers();
		}
	}

	#endregion

	#region Observer Implementations

	// Current Conditions Display
	public class CurrentConditionsDisplay : IWeatherObserver
	{
		private float _temperature;
		private float _humidity;
		private float _pressure;
		private IWeatherStation _weatherStation;

		public CurrentConditionsDisplay(IWeatherStation weatherStation)
		{
			_weatherStation = weatherStation;
			_weatherStation.RegisterObserver(this);
		}

		public void Update(float temperature, float humidity, float pressure)
		{
			_temperature = temperature;
			_humidity = humidity;
			_pressure = pressure;
		}

		public void Display()
		{
			Console.WriteLine("\n--- Current Conditions Display ---");
			Console.WriteLine($"Temperature: {_temperature}ÅãC");
			Console.WriteLine($"Humidity: {_humidity}%");
			Console.WriteLine($"Pressure: {_pressure} hPa");
		}
	}

	// Statistics Display
	public class StatisticsDisplay : IWeatherObserver
	{
		private List<float> _temperatures;
		private IWeatherStation _weatherStation;

		public StatisticsDisplay(IWeatherStation weatherStation)
		{
			_weatherStation = weatherStation;
			_temperatures = new List<float>();
			_weatherStation.RegisterObserver(this);
		}

		public void Update(float temperature, float humidity, float pressure)
		{
			_temperatures.Add(temperature);
		}

		public void Display()
		{
			if (_temperatures.Count == 0)
			{
				Console.WriteLine("\n--- Statistics Display ---");
				Console.WriteLine("No temperature data available.");
				return;
			}

			float minTemp = _temperatures.Min();
			float maxTemp = _temperatures.Max();
			float avgTemp = _temperatures.Average();

			Console.WriteLine("\n--- Statistics Display ---");
			Console.WriteLine($"Min Temperature: {minTemp}ÅãC");
			Console.WriteLine($"Max Temperature: {maxTemp}ÅãC");
			Console.WriteLine($"Average Temperature: {avgTemp}ÅãC");
		}
	}

	// Forecast Display
	public class ForecastDisplay : IWeatherObserver
	{
		private float _lastPressure;
		private IWeatherStation _weatherStation;

		public ForecastDisplay(IWeatherStation weatherStation)
		{
			_weatherStation = weatherStation;
			_weatherStation.RegisterObserver(this);
		}

		public void Update(float temperature, float humidity, float pressure)
		{
			if (_lastPressure == 0)
			{
				_lastPressure = pressure;
				return;
			}

			string forecast = "Same weather";

			if (pressure > _lastPressure)
			{
				forecast = "Improving weather";
			}
			else if (pressure < _lastPressure)
			{
				forecast = "Cooler, rainy weather";
			}

			_lastPressure = pressure;
			Console.WriteLine("\n--- Forecast Display ---");
			Console.WriteLine($"Forecast: {forecast}");
		}
	}

	#endregion

	#region Application

	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Observer Pattern Homework - Weather Station\n");

			try
			{
				// Create the weather station (subject)
				WeatherStation weatherStation = new WeatherStation();

				// Create display devices (observers)
				Console.WriteLine("Creating display devices...");
				CurrentConditionsDisplay currentDisplay = new CurrentConditionsDisplay(weatherStation);
				StatisticsDisplay statisticsDisplay = new StatisticsDisplay(weatherStation);
				ForecastDisplay forecastDisplay = new ForecastDisplay(weatherStation);

				// Simulate weather changes
				Console.WriteLine("\nSimulating weather changes...");

				// Initial weather
				weatherStation.SetMeasurements(25.2f, 65.3f, 1013.1f);

				// Display information from all displays
				Console.WriteLine("\n--- Displaying Information ---");
				currentDisplay.Display();
				statisticsDisplay.Display();
				forecastDisplay.Display();

				// Weather change 1
				weatherStation.SetMeasurements(28.5f, 70.2f, 1012.5f);

				// Display updated information
				Console.WriteLine("\n--- Displaying Updated Information ---");
				currentDisplay.Display();
				statisticsDisplay.Display();
				forecastDisplay.Display();

				// Weather change 2
				weatherStation.SetMeasurements(22.1f, 90.7f, 1009.2f);

				// Display updated information again
				Console.WriteLine("\n--- Displaying Final Information ---");
				currentDisplay.Display();
				statisticsDisplay.Display();
				forecastDisplay.Display();

				// Test removing an observer
				Console.WriteLine("\nRemoving CurrentConditionsDisplay...");
				weatherStation.RemoveObserver(currentDisplay);

				// Weather change after removing an observer
				weatherStation.SetMeasurements(24.5f, 80.1f, 1010.3f);

				// Display information without the removed observer
				Console.WriteLine("\n--- Displaying Information After Removal ---");
				statisticsDisplay.Display();
				forecastDisplay.Display();

				Console.WriteLine("\nObserver Pattern demonstration complete.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An error occurred: {ex.Message}");
			}

			Console.WriteLine("\nPress any key to exit...");
			Console.ReadKey();
		}
	}

	#endregion
}
