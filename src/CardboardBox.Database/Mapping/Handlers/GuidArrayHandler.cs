namespace CardboardBox.Database.Mapping;

/// <summary>
/// Handles mapping <see cref="Guid"/> array results from string results.
/// </summary>
public class GuidArrayHandler : SqlMapper.TypeHandler<Guid[]>
{
    /// <summary>
    /// Parses the input value into a proper <see cref="Guid"/> array
    /// </summary>
    /// <param name="value">The value to parse</param>
    /// <returns>The parsed <see cref="Guid"/>[]</returns>
    public override Guid[] Parse(object value)
    {
        if (value is Guid[] guids)
            return guids;

        if (value is string[] array)
            return array.Select(Guid.Parse).ToArray();

        if (value is not string str)
            return [];

        return str
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(Guid.Parse)
            .ToArray();
    }

    /// <summary>
    /// Sets the database parameter value to the proper representation for a <see cref="Guid"/>[]
    /// </summary>
    /// <param name="parameter">The data parameter to set</param>
    /// <param name="value">The value to set it to</param>
    public override void SetValue(IDbDataParameter parameter, Guid[]? value)
    {
        parameter.Value = value;
    }
}
