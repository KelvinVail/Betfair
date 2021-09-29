using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace Betfair.Domain
{
    public sealed class DomainError : ValueObject
    {
        internal DomainError(string code, string message)
        {
            Code = code;
            Message = message;
        }

        public string Code { get; }

        public string Message { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Code;
        }
    }
}
