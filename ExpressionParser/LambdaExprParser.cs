using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
namespace ExpressionParser
{
    public class ExprParseException : Exception
    {
        public ExprParseException(string mes) : base(mes) { }
        public ExprParseException(string mes,Exception inner) :base(mes ,inner) { }
    
        
    }





   public  class LambdaExprParser:ExpressionParser
    {
        public LambdaExprParser() : base(@"((<|>)=)|(==)|[+\-*/<>=:]|(&&)") { }

        public LambdaExprParser(string op, string uk,System.Text.RegularExpressions.RegexOptions options) : base(op, uk,options) { }





        protected  MemberExpression CreateMemberExpr(Expression objexp, string Token)
        {
            return Expression.Property(objexp,Token);
        }

        protected ConstantExpression CreateConstantExpr(string token) 
        {
            return Expression.Constant(double.Parse(token));
        }




        protected virtual ExpressionType GetExprType(string token)
        {
            switch (token)
            {
                case "<":
                    return ExpressionType.LessThan;
                case "=":
                    return ExpressionType.Assign;
                case "<=":
                    return ExpressionType.LessThanOrEqual;
                case ">":
                    return ExpressionType.GreaterThan;
                case ">=":
                    return ExpressionType.GreaterThanOrEqual;
                case "==":
                    return ExpressionType.Equal; 
                case "+":
                    return ExpressionType.Add;
                case "-":
                    return ExpressionType.Subtract;
                case "*":
                    return ExpressionType.Multiply;
                case "/":
                    return ExpressionType.Divide;
                case "&&":
                    return ExpressionType.AndAlso;
                case ".":
                    return ExpressionType.MemberAccess;
                case ":":
                    return ExpressionType.ArrayIndex;

                default:throw new NotImplementedException(); 
                    
            }
        
        }


        protected override int GetPriority(string token)
        {
            switch (token)
            {

                case "=":
                    return -2;
                case "&&":
                    return -1;
                case "==":
                    return 0;
                case "<":
                case ">":
                case ">=":
                case "<=":
                    return 1;
                case "+":
                case "-":
                    return 2;
                case "*":
                case "/":
                    return 3;
                case ".":
                case ":":
                    return 4; 
                default:
                    return 0;
            }

        }





        /// <summary>
        /// 将string转换为简单的委托
        /// </summary>
        /// <typeparam name="TObj"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Func<TObj, bool> GetConditionDelegate<TObj>(string expression)
        {
            try
            {
                string[] RPNTokens = ConvertToRPN(expression);
                ParameterExpression para = Expression.Parameter(typeof(TObj), "TObj");
                Stack<Expression> exprs = new Stack<Expression>();
                for (int i = RPNTokens.Length - 1; i >= 0; --i)
                {
                    string token = RPNTokens[i];
                    if (IsNumber(token))
                        exprs.Push(CreateConstantExpr(token));
                    else if (IsUnknownNumber(token))
                    {
                        exprs.Push(CreateMemberExpr(para, token));
                    }
                    else if (IsOperator(token))
                    {
                        Expression expr2 = exprs.Pop();
                        Expression expr1 = exprs.Pop();

                        if (expr1.Type.BaseType == typeof(Array))
                            exprs.Push(Expression.MakeBinary(GetExprType(token), expr1, Expression.Convert(expr2, typeof(Int32))));
                        else if (expr1.Type == typeof(Boolean))
                            exprs.Push(Expression.MakeBinary(GetExprType(token), expr1, expr2));
                        else exprs.Push(Expression.MakeBinary(GetExprType(token), Expression.Convert(expr1, typeof(double)), Expression.Convert(expr2, typeof(double))));

                      
                       
                    }
                }

                Expression<Func<TObj, bool>> expt = Expression.Lambda<Func<TObj, bool>>(exprs.Peek(), true, para);
                return expt.Compile();
            } catch 
            {
                throw new ExprParseException("Parse Fail"); 
            }
        }

       




    }
}
