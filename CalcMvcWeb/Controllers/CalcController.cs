using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalcMvcWeb.Models;
using CalcMvcWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace CalcMvcWeb.Controllers
{
    public class CalcController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(new CalcViewModel());
        }

        private string AutoCloseBrackets(string expression)
        {
            int openCount = expression.Count(c => c == '(');
            int closeCount = expression.Count(c => c == ')');

            int missingClosures = openCount - closeCount;

            if (missingClosures > 0)
            {
                expression += new string(')', missingClosures);
            }

            return expression;
        }

        private bool HasMismatchedBrackets(string expression)
        {
            int balance = 0;
            foreach (var c in expression)
            {
                if (c == '(') balance++;
                else if (c == ')') balance--;

                if (balance < 0) return true; // Closing before opening
            }
            return balance != 0; // Unbalanced brackets
        }

        [HttpPost]
        public IActionResult Calculate(CalcViewModel model, string button)
        {
            var cs = new CalcService();

            if (button == "C")
            {
                model.Expression = string.Empty;
                model.Result = "0";
            }
            else if (button == "⌫")
            {
                if (!string.IsNullOrEmpty(model.Expression))
                    model.Expression = model.Expression[..^1];
            }
            else if (button == "=")
            {
                model.Expression = AutoCloseBrackets(model.Expression);
                model.Result = cs.EvaluateExpression(model.Expression);
                model.Expression = model.Result;
            }
            else if (button == "+/-")
            {
                if (double.TryParse(model.Expression, out double value))
                    model.Expression = (-value).ToString();
            }
            else if (button == "MC")
            {
                model.Memory = 0;
            }
            else if (button == "MR")
            {
                model.Expression += model.Memory.ToString();
            }
            else if (button == "M+")
            {
                if (double.TryParse(model.Result, out double value))
                    model.Memory += value;
            }
            else if (button == "M-")
            {
                if (double.TryParse(model.Result, out double value))
                    model.Memory -= value;
            }
            else if (button == "MS")
            {
                if (double.TryParse(model.Result, out double value))
                    model.Memory = value;
            }
            else
            {
                if (model.Expression == "Error")
                    model.Expression = button;
                else
                    model.Expression += button;
            }

            ViewData["InvalidBrackets"] = HasMismatchedBrackets(model.Expression);
            return View("Index", model);
        }

    }
}