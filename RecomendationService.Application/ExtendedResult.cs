using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecomendationService.Application
{
    public class Result<T> : Result
    {
        public T? Value { get; set; }
        public static Result<T> Success(T value, int statusCode = 200)
        {
            return new Result<T> { Value = value, StatusCode = statusCode };
        }
        public static Result<T> Failure(int statusCode, string errorMessage, T? value = default)
        {
            return new Result<T> { ErrorMessage = errorMessage, StatusCode = statusCode, Value = value };
        }
    }
}
