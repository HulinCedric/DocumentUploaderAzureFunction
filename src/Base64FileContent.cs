using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace DocumentUploader;

public class Base64FileContent : ValueObject
{
    private Base64FileContent(string value)
        => Value = value;

    public string Value { get; }

    public byte[] GetDecodedValue()
        => Convert.FromBase64String(Value);
    
    public BinaryData GetBinaryValue()
        => BinaryData.FromBytes(GetDecodedValue());

    private static bool IsBase64String(string input)
    {
        var buffer = new Span<byte>(new byte[input.Length]);
        return Convert.TryFromBase64String(input, buffer, out var bytesParsed);
    }

    public static Result<Base64FileContent> Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Result.Failure<Base64FileContent>("Value must not be empty");

        var base64Value = input;

        if (!IsBase64String(base64Value))
            return Result.Failure<Base64FileContent>("Value must be a valid base 64 string");

        return new Base64FileContent(base64Value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}