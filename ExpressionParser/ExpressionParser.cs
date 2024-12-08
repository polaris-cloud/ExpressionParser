using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ExpressionParser
{
    public abstract class ExpressionParser
    {

        
        private RegexOptions _options;
        private string _ukPattern;
        private string _opPattern;
        private string _pattern;
        private string _numPattern=@"[-]?\d+\.*\d*";




       
       protected  ExpressionParser(string opPtn = @"[/+-/*<>=]", string ukPtn = @"[-]?[a-z]+[0-9]*",RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Singleline)
        {

            _opPattern = opPtn;
            _ukPattern = ukPtn;
            _options = options;
            _pattern = @"[/(/)]|(?<=\S)(?<!=" + opPtn + ")" + opPtn + "|" + _numPattern +"|"+ _ukPattern;
        }




        /// <summary>
        ///   将表达式分解为运算元(变量、常量、运算符)
        /// </summary>
        /// <param name="rightExp"></param>
        /// <param name="pattern"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static  string[] AnalyseExpression(string rightExp, string pattern, RegexOptions options)
        {
            //去除换行符等字符
            string trim = Regex.Replace(rightExp, @"\s", "");
            var matches = Regex.Matches(trim, pattern, options);

            string[] tokens = new string[matches.Count];
            for (int i = 0; i < tokens.Length; ++i)
            {

                tokens[i] = matches[i].ToString();
            }
            return tokens;
        }


        /// <summary>
        /// 解析表达式
        /// </summary>
        /// <param name="rightExp"></param>
        /// <returns></returns>
        public string[] AnalyseExpression(string Exp)
        {
            return AnalyseExpression(Exp, _pattern, _options);
        }




        /// <summary>
        /// 获取变量
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private  List<string> GetVariables(string[] tokens)
        {
            List<string> token = new List<string>();

            for (int i = 0; i < tokens.Length; ++i)
            {
                if (Regex.IsMatch(tokens[i], _ukPattern, _options))
                {
                    token.Add(tokens[i]);
                }
            }
            return token;
        }





        protected bool IsNumber(string token)
        {
            double result = 0;
            if (double.TryParse(token, out result))
                return true;
            else
                return false;
        }

        protected bool IsUnknownNumber(string token)
        {
            if (Regex.IsMatch(token, _ukPattern, RegexOptions.IgnoreCase))
                return true;
            else
                return false;

        }

        /// <summary>
        ///  是否是运算符
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected  bool IsOperator(string token)
        {
            if (Regex.IsMatch(token, _opPattern, RegexOptions.IgnoreCase))
                return true;
            else
                return false;
        }


        /// <summary>
        ///  得到优先级
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected virtual int GetPriority(string token)
        {
            switch (token)
            {
                
                case "=":
                    return -1;
                case "+":
                case "-":
                case "<":
                case ">":
                    return 0;
                case "*":
                case "/":
                    return 1;

                default:
                    return 0;
            }

        }

        /// <summary>
        ///  若比运算符堆栈栈顶的运算符优先级高或相等，则直接存入运算符堆栈。
        /// 若比运算符堆栈栈顶的运算符优先级低，则输出栈顶运算符到操作数堆栈，并将当前运算符压入运算符堆栈。
        /// </summary>
        /// <param name="token"></param>
        /// <param name="stack"></param>
        /// <returns></returns>
        private bool IsPrior(string token, Stack<string> stack)
        {

            if (GetPriority(token) > GetPriority(stack.Peek()))
                return true;
            else return false;

        }

        protected string[] ConvertToRPN(string rightExp)
        {

            string[] tokens = AnalyseExpression(rightExp);
            Stack<string> numStack = new Stack<string>();
            Stack<string> opStack = new Stack<string>();
            int bra = 0; 
            foreach (string token in tokens)
            {
                if (IsNumber(token) || IsUnknownNumber(token))
                {
                    numStack.Push(token);  //操作数压入操作数栈中
                }
                else if (token == "(")
                {
                    opStack.Push(token);
                    ++bra;
                }
                else if (token == ")")
                {
                    if (bra == 0) return null;//左右括号不匹配 
                    while (opStack.Peek() != "(")
                        numStack.Push(opStack.Pop());
                    opStack.Pop();
                    --bra;
                }
                else if (IsOperator(token))
                {


                    while (opStack.Count != 0 && !IsPrior(token, opStack))
                    {
                        numStack.Push(opStack.Pop());  // 将比当前要比较运算优先级高的运算符压入操作数栈中 
                    }
                    opStack.Push(token);  // 将该低优先级运算符放入运算符栈中

                }
                else
                    return null;
            }
            //一个op栈中所有元素的转移至num栈中
            while (opStack.Count != 0)
                numStack.Push(opStack.Pop());  //将最低优先级的运算符最后移入操作数栈中
            //左右括号数量不匹配 
            if (bra != 0)
                return null;

            return numStack.ToArray();

        }


        
        







        /*
        /// <summary>
        /// 计算表达式
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="unKnownNuber"></param>
        /// <returns></returns>
        public double? EvalExpression(string rightexp, IDictionary<string, double?> unKnownNuber)
        {


            return EvalRPN(ConvertToRPN(rightexp), unKnownNuber);
        }
        */
    }






}
