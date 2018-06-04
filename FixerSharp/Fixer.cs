using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FixerSharp
{
	public class Fixer
	{
		private const string BaseUri = "http://data.fixer.io/api/";

		public static double Convert(string apiKey, string from, string to, double amount, DateTime? date = null)
		{
			return GetRate(apiKey, from, to, date).Convert(amount);
		}

		public static async Task<double> ConvertAsync(string apiKey, string from, string to, double amount, DateTime? date = null)
		{
			return (await GetRateAsync(apiKey, from, to, date)).Convert(amount);
		}

		public static ExchangeRate Rate(string apiKey, string from, string to, DateTime? date = null)
		{
			return GetRate(apiKey, from, to, date);
		}

		public static async Task<ExchangeRate> RateAsync(string apiKey, string from, string to, DateTime? date = null)
		{
			return await GetRateAsync(apiKey, from, to, date);
		}

		private static ExchangeRate GetRate(string apiKey, string from, string to, DateTime? date = null)
		{
			from = from.ToUpper();
			to = to.ToUpper();

			if (!Symbols.IsValid(from))
				throw new ArgumentException("Symbol not found for provided currency", "from");

			if (!Symbols.IsValid(to))
				throw new ArgumentException("Symbol not found for provided currency", "to");

			var url = GetFixerUrl(apiKey, date);

			using (var client = new HttpClient())
			{
				var response = client.GetAsync(url).Result;
				response.EnsureSuccessStatusCode();

				return ParseData(response.Content.ReadAsStringAsync().Result, from, to);
			}
		}

		private static async Task<ExchangeRate> GetRateAsync(string apiKey, string from, string to, DateTime? date = null)
		{
			from = from.ToUpper();
			to = to.ToUpper();

			if (!Symbols.IsValid(from))
				throw new ArgumentException("Symbol not found for provided currency", "from");

			if (!Symbols.IsValid(to))
				throw new ArgumentException("Symbol not found for provided currency", "to");

			var url = GetFixerUrl(apiKey, date);

			using (var client = new HttpClient())
			{
				var response = await client.GetAsync(url);
				response.EnsureSuccessStatusCode();

				return ParseData(await response.Content.ReadAsStringAsync(), from, to);
			}
		}

		private static ExchangeRate ParseData(string data, string from, string to)
		{
			// Parse JSON
			var root = JObject.Parse(data);

			var rates = root.Value<JObject>("rates");
			var fromRate = rates.Value<double>(from);
			var toRate = rates.Value<double>(to);

			var rate = toRate / fromRate;

			// Parse returned date
			// Note: This may be different to the requested date as Fixer will return the closest available
			var returnedDate = DateTime.ParseExact(root.Value<string>("date"), "yyyy-MM-dd",
				System.Globalization.CultureInfo.InvariantCulture);

			return new ExchangeRate(from, to, rate, returnedDate);
		}

		private static string GetFixerUrl(string apiKey, DateTime? date = null)
		{
			var dateString = date.HasValue ? date.Value.ToString("yyyy-MM-dd") : "latest";

			return $"{BaseUri}{dateString}?access_key={apiKey}";
		}

	}
}
