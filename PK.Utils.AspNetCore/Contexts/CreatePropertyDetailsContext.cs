using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace PK.Utils.AspNetCore.Contexts;

public class CreatePropertyDetailsContext: BaseContext
{
	public ModelMetadataIdentity Key { get; set; }
	public DefaultMetadataDetails[] Metadata { get; set; }

	/// <inheritdoc />
	public CreatePropertyDetailsContext(ExtendableModelMetadataProvider provider) : base(provider)
	{
	}
}
