using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using CSharpFunctionalExtensions;

namespace DocumentUploader;

public class FileName : ValueObject
{
    private FileName(string value)
        => Value = value;

    public string Value { get; }

    private static string GetFileExtension(string input)
        => Path.GetExtension(input);

    private static string GetFileNameWithoutExtension(string input)
        => Path.GetFileNameWithoutExtension(input);

    public static Result<FileName> Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Result.Failure<FileName>("Value must not be empty");

        var fileName = input.Trim().ToLowerInvariant();

        var fileExtension = GetFileExtension(fileName);
        if (string.IsNullOrEmpty(fileExtension))
            return Result.Failure<FileName>("Value must contain an extension");

        var fileNameWithoutExtension = GetFileNameWithoutExtension(fileName);
        if (string.IsNullOrEmpty(fileNameWithoutExtension))
            return Result.Failure<FileName>("Value must contain a name");

        return new FileName(fileName);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public string GetRandomizedValue()
        => $"{GetHashString(Value)}{GetFileExtension(Value)}";

    private static byte[] GetHash(string input)
    {
        using HashAlgorithm algorithm = SHA256.Create();
        return algorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
    }

    private static string GetHashString(string input)
    {
        var sb = new StringBuilder();
        foreach (var b in GetHash(input))
            sb.Append(b.ToString("X2"));

        return sb.ToString().ToLowerInvariant();
    }

    public override string ToString()
        => Value;
}