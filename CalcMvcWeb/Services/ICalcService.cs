namespace CalcMvcWeb.Services
{
    public interface ICalcService
    {
        /// <summary>
        /// Evaluates a mathematical expression and returns the result as a string.
        /// </summary>
        /// <param name="expression">The input mathematical expression.</param>
        /// <returns>The calculated result or "Error" if evaluation fails.</returns>
        string EvaluateExpression(string expression);
    }
}
