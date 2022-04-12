using System.IO;
using CSharpFunctionalExtensions;

namespace DocumentUploader.UnitTests;

public class FileName
{
    private readonly string value;

    private FileName(string value)
        => this.value = value;

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


    // RuleFor(command => command)
    //     .Must(
    //     command =>
    // {
    //     try
    //     {
    //         command.GetFileExtension();
    //     }
    //     catch
    //     {
    //         return false;
    //     }
    //
    //     return true;
    // })
    // .WithName(nameof(FileName))
    //     .WithMessage("'FileName' must contains a valid file extension");
    //       
    // RuleFor(command => command)
    //     .Must(
    //     command =>
    // {
    //     try
    //     {
    //         command.GetFileNameWithoutExtension();
    //     }
    //     catch
    //     {
    //         return false;
    //     }
    //
    //     return true;
    // })
    // .WithName(nameof(FileName))
    //     .WithMessage("'FileName' must contains a valid file name");
}