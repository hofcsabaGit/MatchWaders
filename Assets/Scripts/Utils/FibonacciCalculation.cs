public static class FibonacciCalculation {
	// public for unit test
	public static int CalculateFibonacci (int n)
	{
		int a = 1;
		int b = 1;

		for (int i = 0; i < n; i++) {
			int temp = a;
			a = b;
			b = temp + b;
		}

		return a;
	}

}
