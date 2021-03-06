using System;
using System.Diagnostics;
using System.Linq.Expressions;
using NRules.Utilities;

namespace NRules.Rete
{
    internal interface IAlphaCondition
    {
        bool IsSatisfiedBy(Fact fact);
    }

    [DebuggerDisplay("{_expression.ToString()}")]
    internal class AlphaCondition : IAlphaCondition, IEquatable<AlphaCondition>
    {
        private readonly LambdaExpression _expression;
        private readonly Func<object[], bool> _compiledExpression;

        public AlphaCondition(LambdaExpression expression)
        {
            _expression = expression;
            _compiledExpression = FastDelegate.Create<Func<object[], bool>>(expression);
        }

        public bool IsSatisfiedBy(Fact fact)
        {
            try
            {
                return _compiledExpression(new[] {fact.Object});
            }
            catch (Exception e)
            {
                throw new RuleConditionEvaluationException("Failed to evaluate condition", _expression, fact, e);
            }
        }

        public bool Equals(AlphaCondition other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ExpressionComparer.AreEqual(_expression, other._expression);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AlphaCondition)obj);
        }

        public override int GetHashCode()
        {
            return _expression.GetHashCode();
        }
    }
}