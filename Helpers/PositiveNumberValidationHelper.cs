namespace BankAccountServices.Helpers;

public static class PositiveNumberValidationHelper
{
    public static void Validate(long value, string parameterName)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                value,
                "La valeur doit être strictement positive.");
        }
    }
}
