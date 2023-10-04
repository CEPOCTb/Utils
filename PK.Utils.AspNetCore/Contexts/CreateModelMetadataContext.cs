using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace PK.Utils.AspNetCore.Contexts;

public class CreateModelMetadataContext : BaseContext
{
	public DefaultMetadataDetails Entry { get; set; }
	public ModelMetadata Metadata { get; set; }

	/// <inheritdoc />
	public CreateModelMetadataContext(ExtendableModelMetadataProvider provider) : base(provider)
	{
	}
}
