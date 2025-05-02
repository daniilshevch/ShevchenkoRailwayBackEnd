public class ImportanceAttribute : Attribute
{
    public string Description { get; set; }
    public ImportanceAttribute(string description = "")
    {
        Description = description;
    }
}
public class NotInUseAttribute : ImportanceAttribute { }
public class CrucialAttribute : ImportanceAttribute
{
    public CrucialAttribute(string description = "") : base(description) { }
}
public class ExecutiveAttribute : ImportanceAttribute
{
    public ExecutiveAttribute(string description = "") : base(description) { }
}
public class PeripheralAttribute : ImportanceAttribute
{
    public PeripheralAttribute(string description = "") : base(description) { }
}
public class ArchievedAttribute : ImportanceAttribute
{
    public ArchievedAttribute(string description = "") : base(description) { }
}
public class DestinationAttribute : Attribute
{

}
public class InternalAttribute : DestinationAttribute { }
public class ExternalAttribute : DestinationAttribute { }
public class AlgorithmAttribute : Attribute
{
    public string Algorithm { get; set; }
    public AlgorithmAttribute(string algorithm)
    {
        Algorithm = algorithm;
    }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class StatusAttribute : Attribute
{
    public string Version { get; set; }
    public string Date { get; set; }
    public StatusAttribute(string version, string date)
    {
        Version = version;
        Date = date;
    }
}
public class CheckedAttribute : StatusAttribute
{
    public CheckedAttribute(string date) : base("v1.0", date) { }
}
public class RefactoredAttribute : StatusAttribute
{
    public RefactoredAttribute(string version, string date) : base(version, date) { }
}
public class ReengineeredAttribute : StatusAttribute
{
    public ReengineeredAttribute(string version, string date) : base(version, date) { }
}
public class OptimizedAttribute : StatusAttribute
{
    public OptimizedAttribute(string version, string date) : base(version, date) { }
}
