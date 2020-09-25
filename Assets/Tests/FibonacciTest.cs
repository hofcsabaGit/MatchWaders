using UnityEngine.TestTools;
using NUnit.Framework;

public class FibonacciTest {
	[Test]
	public void TestFibonacci ()
	{
		//setup
		int expectedNumber = 34; // 8th number in series;

		//run
		int outputNumber = FibonacciCalculation.CalculateFibonacci (8);

		//assert
		Assert.AreEqual (expectedNumber, outputNumber);
	}
}

