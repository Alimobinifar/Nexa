using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Cmmon.Dtos.ResultAndResponse
{

    public class ResponseDto<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }

        public ResponseDto()
        {
        }

        public ResponseDto(T data, string message = "", int statusCode = 200)
        {
            IsSuccess = true;
            Data = data;
            Message = message;
            StatusCode = statusCode;
        }

        public ResponseDto(string message, int statusCode, List<string>? errors = null)
        {
            IsSuccess = false;
            Message = message;
            StatusCode = statusCode;
            Errors = errors;
        }


        public static ResponseDto<T> Success(T data, string message = "عملیات با موفقیت انجام شد")
            => new(data, message, 200);

        public static ResponseDto<T> Error(string message, int statusCode = 400, List<string>? errors = null)
            => new(message, statusCode, errors);

        public static ResponseDto<T> NotFound(string message = "موردی یافت نشد")
            => new(message, 404);

        public static ResponseDto<T> Unauthorized(string message = "دسترسی غیرمجاز")
            => new(message, 401);
    }
}
