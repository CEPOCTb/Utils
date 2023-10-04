namespace PK.Utils.AspNetCore.Contexts;

public abstract class BaseContext
{
	public ExtendableModelMetadataProvider Provider { get; }
	public bool Handled { get; set; }

	protected BaseContext(ExtendableModelMetadataProvider provider)
	{
		Provider = provider;
	}
}
