namespace CardboardBox.Database.Mapping;

/// <summary>
/// Handles mapping Enum results from string results.
/// </summary>
public class EnumHandler<T> : SqlMapper.TypeHandler<T>
    where T : struct, Enum
{
    /// <summary>
    /// Parses the enum into the proper value
    /// </summary>
    /// <param name="value">The value to parse</param>
    /// <returns>The parsed enum</returns>
    public override T Parse(object value)
    {
        return !Enum.TryParse(value?.ToString(), true, out T result)
            ? default
            : result;
    }

    /// <summary>
    /// Sets the database parameter value to the proper representation for the Enum
    /// </summary>
    /// <param name="parameter">The data parameter to set</param>
    /// <param name="value">The value to set it to</param>
    public override void SetValue(IDbDataParameter parameter, T value)
    {
        parameter.Value = (int)(object)value;
    }
}
