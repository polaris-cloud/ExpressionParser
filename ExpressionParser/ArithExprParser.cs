using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ExpressionParser
{
   public class ArithExprParser: ExpressionParser 
    {
        public ArithExprParser() : base() { }

        public ArithExprParser(string op, string uk,System.Text.RegularExpressions.RegexOptions options) : base(op, uk,options) { }


        protected virtual double Calculate(string token,double num1,double num2)
        {

            switch (token)
            {
                case "+":
                   return  num1 + num2;
                    
                case "-":
                  return num1 - num2;
                 
                case "*":
                  return  num1 * num2;
                 
                case "/":
                    return num1 / num2 ;
                case "<":
                    return num1 < num2 ? num1 : num2;
                case ">":
                    return num1 > num2 ? num1 : num2 ;
                case "=":
                    return num1 = num2;
                default:
                    throw new NotImplementedException();
            }


        }
        public  double? Eval(string expression, IDictionary<string, double?> unknownNumber)
        {
            string[] RPNTokens = ConvertToRPN(expression);
            Stack<double> nums = new Stack<double>();
            for (int i = RPNTokens.Length - 1; i >= 0; --i)
            {
                if (IsNumber(RPNTokens[i]))
                    nums.Push(double.Parse(RPNTokens[i]));
                else if (IsUnknownNumber(RPNTokens[i]))
                {
                    if (unknownNumber.ContainsKey(RPNTokens[i]))
                    {
                        double? num = unknownNumber[RPNTokens[i]];
                        if (num == null) return null;
                        nums.Push((double)unknownNumber[RPNTokens[i]]);
                    }
                    else
                        return null;
                }
                else if (IsOperator(RPNTokens[i]))
                {
                    double num2 = nums.Pop();
                    double num1 = nums.Pop();
                    nums.Push(Calculate(RPNTokens[i], num1, num2));
                }
            }
            return nums.Peek();
        }

            
    }
}
