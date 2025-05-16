using System;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CalcMvcWeb.Services
{
    public class CalcService : ICalcService
    {
        public string EvaluateExpression(string expression)
        {
            try
            {
                expression = ProcessSpecialOperators(expression);

                // Handle Power (^) before evaluating with DataTable
                expression = ProcessPowerOperator(expression);

                var result = new DataTable().Compute(expression, null);
                return FormatResult(result);
            }
            catch
            {
                return "Error";
            }
        }

        private string ProcessSpecialOperators(string expression)
        {
            expression = ApplyDirectFunction(expression, "π", Math.PI);
            expression = ApplyDirectFunction(expression, "e", Math.E);

            expression = ProcessFunction(expression, "sqrt", Math.Sqrt);
            expression = ProcessFunction(expression, "sin", x => Math.Sin(ToRadians(x)));
            expression = ProcessFunction(expression, "cos", x => Math.Cos(ToRadians(x)));
            expression = ProcessFunction(expression, "tan", x => Math.Tan(ToRadians(x)));
            expression = ProcessFunction(expression, "log", Math.Log10);
            expression = ProcessFunction(expression, "ln", Math.Log);
            expression = ProcessFunction(expression, "abs", Math.Abs);
            expression = ProcessFunction(expression, "fact", Factorial);

            expression = expression.Replace("mod", "%");
            return expression;
        }

        private string ProcessPowerOperator(string expression)
        {
            while (Regex.IsMatch(expression, @"(\d+(\.\d+)?)\^(\d+(\.\d+)?)"))
            {
                expression = Regex.Replace(expression, @"(\d+(\.\d+)?)\^(\d+(\.\d+)?)", match =>
                {
                    double baseNum = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                    double exponent = double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
                    return Math.Pow(baseNum, exponent).ToString(CultureInfo.InvariantCulture);
                });
            }
            return expression;
        }

        private string ProcessFunction(string expression, string functionName, Func<double, double> func)
        {
            // Example: sqrt(9) => 3
            return Regex.Replace(expression, $@"{functionName}\(([^)]+)\)", match =>
            {
                var inner = match.Groups[1].Value;
                return TryCalculate(func, inner);
            });
        }

        private string ApplyDirectFunction(string expression, string constantSymbol, double constantValue)
        {
            return expression.Replace(constantSymbol, constantValue.ToString(CultureInfo.InvariantCulture));
        }

        private string TryCalculate(Func<double, double> operation, string input)
        {
            if (double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                return operation(value).ToString(CultureInfo.InvariantCulture);
            return "0";
        }

        private string FormatResult(object result)
        {
            if (double.TryParse(result.ToString(), out double numericResult))
                return numericResult.ToString("G15", CultureInfo.InvariantCulture);
            return result.ToString();
        }

        private double Factorial(double n)
        {
            if (n < 0 || n != Math.Floor(n)) return double.NaN;
            double result = 1;
            for (int i = 2; i <= (int)n; i++)
                result *= i;
            return result;
        }

        private double ToRadians(double angle) => angle * (Math.PI / 180);
    }
}
