using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace PK.Utils.AspNetCore.Contexts;

public class CreateTypeDetailsContext: BaseContext
{
	public ModelMetadataIdentity Key { get; set; }
	public DefaultMetadataDetails Metadata { get; set; }

	/// <inheritdoc />
	public CreateTypeDetailsContext(ExtendableModelMetadataProvider provider) : base(provider)
	{
	}
}
