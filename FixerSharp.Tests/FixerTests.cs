﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FixerSharp.Tests
{
	[TestClass]
	public class FixerTests
	{
		//Insert your own Fixer.IO API key to see if the tests are working 
		private static string testApiKey = "00000000000000000000000000000000";

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Invalid_From_Symbol_Throws_Exception()
		{
			Fixer.Convert(testApiKey, "ABC", "USD", 100);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Invalid_To_Symbol_ThrowsException()
		{
			Fixer.Convert(testApiKey, "GBP", "XYZ", 100);
		}

		[TestMethod]
		public void Conversion_With_Symbol_Returns_Value()
		{
			double converted = Fixer.Convert(testApiKey, Symbols.GBP, Symbols.USD, 100);

			Assert.AreNotEqual(converted, 0);
		}

		[TestMethod]
		public void Conversion_With_String_Returns_Value()
		{
			double converted = Fixer.Convert(testApiKey, "USD", "EUR", 100);

			Assert.AreNotEqual(converted, 0);
		}

		[TestMethod]
		public void Rate_With_Symbol_Returns_Value()
		{
			var rate = Fixer.Rate(testApiKey, Symbols.GBP, Symbols.DKK);

			Assert.AreNotEqual(rate, null);
		}

		[TestMethod]
		public void Rate_With_Strings_Returns_Value()
		{
			var rate = Fixer.Rate(testApiKey, "USD", "GBP");

			Assert.AreNotEqual(rate, null);
		}

		[TestMethod]
		public void Rate_Properties_Contain_Data()
		{
			var rate = Fixer.Rate(testApiKey, Symbols.GBP, Symbols.EUR);

			Assert.AreNotEqual(rate.From, "");
			Assert.AreNotEqual(rate.To, "");
			Assert.AreNotEqual(rate.Rate, 0);
			Assert.AreNotEqual(rate.Date, default(DateTime));
		}

		[TestMethod]
		public void Rates_For_Previous_Dates_Contain_Different_Data()
		{
			var rate1 = Fixer.Rate(testApiKey, Symbols.GBP, Symbols.EUR, new DateTime(2016, 10, 19));
			var rate2 = Fixer.Rate(testApiKey, Symbols.GBP, Symbols.EUR, new DateTime(2016, 10, 18));

			Assert.AreNotEqual(rate1.Rate, rate2.Rate);
			Assert.AreNotEqual(rate1.Date, rate2.Date);
		}

		[TestMethod]
		public void Conversion_From_Rate()
		{
			var rate = Fixer.Rate(testApiKey, Symbols.EUR, Symbols.GBP);

			Assert.AreNotEqual(rate.Convert(1), 0);
			Assert.AreNotEqual(rate.Convert(100), 0);
			Assert.AreNotEqual(rate.Convert(1000), 0);
			Assert.AreNotEqual(rate.Convert(10000), 0);
			Assert.AreNotEqual(rate.Convert(100000), 0);
			Assert.AreNotEqual(rate.Convert(10000000), 0);
			Assert.AreNotEqual(rate.Convert(100000000), 0);
		}
	}
}
