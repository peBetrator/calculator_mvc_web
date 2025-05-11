using System;
using System.Data;
using System.Text.RegularExpressions;

namespace CalcMvcWeb.Services
{
    public class CalcService : ICalcService
    {
        public string EvaluateExpression(string expression)
        {
            try
            {
                // Handle advanced math functions and special cases first
                expression = ProcessSpecialOperators(expression);

                // Final basic math evaluation
                var result = new DataTable().Compute(expression, null);
                return result.ToString();
            }
            catch
            {
                return "Error";
            }
        }

        public string ProcessSpecialOperators(string expression)
        {
            expression = ProcessFunction(expression, "sqrt", Math.Sqrt);
            expression = ProcessFunction(expression, "sin", x => Math.Sin(ToRadians(x)));
            expression = ProcessFunction(expression, "cos", x => Math.Cos(ToRadians(x)));
            expression = ProcessFunction(expression, "tan", x => Math.Tan(ToRadians(x)));
            expression = ProcessFunction(expression, "log", Math.Log10);
            expression = ProcessFunction(expression, "ln", Math.Log);
            expression = ProcessFunction(expression, "abs", Math.Abs);
            expression = ProcessFunction(expression, "fact", Factorial);

            while (Regex.IsMatch(expression, @"(\d+(\.\d+)?)\^(\d+(\.\d+)?)"))
            {
                expression = Regex.Replace(expression, @"(\d+(\.\d+)?)\^(\d+(\.\d+)?)", match =>
                {
                    double baseNum = double.Parse(match.Groups[1].Value);
                    double exponent = double.Parse(match.Groups[3].Value);
                    return Math.Pow(baseNum, exponent).ToString();
                });
            }

            expression = expression.Replace("mod", "%");

            return expression;
        }

        private string ProcessFunction(string expression, string functionName, Func<double, double> func)
        {
            return Regex.Replace(expression, $@"{functionName}\(([^)]+)\)", match =>
            {
                var inner = match.Groups[1].Value;
                return TryCalculate(func, inner);
            });
        }

        private string TryCalculate(Func<double, double> operation, string input)
        {
            if (double.TryParse(input, out double value))
                return operation(value).ToString();
            return "0";
        }

        private double Factorial(double n)
        {
            if (n < 0) return double.NaN;
            if (n == 0) return 1;
            double result = 1;
            for (int i = 1; i <= (int)n; i++)
                result *= i;
            return result;
        }

        private double ToRadians(double angle) => angle * (Math.PI / 180);

    }
}
