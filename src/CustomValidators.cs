using System;
using CSharpFunctionalExtensions;
using FluentValidation;

namespace DocumentUploader;

public static class CustomValidators
{
    public static IRuleBuilderOptions<T, string?> MustBeValueObject<T, TValueObject>(
        this IRuleBuilder<T, string?> ruleBuilder,
        Func<string, Result<TValueObject>> factoryMethod)
        => (IRuleBuilderOptions<T, string?>)ruleBuilder.Custom(
            (value, context) =>
            {
                if (string.IsNullOrWhiteSpace(value))
                    return;

                var result = factoryMethod(value);

                if (result.IsFailure)
                    context.AddFailure(result.Error);
            });
}