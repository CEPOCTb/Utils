using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace PK.Utils.AspNetCore.Contexts;

public class CreateParameterDetailsContext: BaseContext
{
	public ModelMetadataIdentity Key { get; set; }
	public DefaultMetadataDetails Metadata { get; set; }

	/// <inheritdoc />
	public CreateParameterDetailsContext(ExtendableModelMetadataProvider provider) : base(provider)
	{
	}
}
