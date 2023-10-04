using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PK.Utils.AspNetCore.Contexts;

public class GetMetadataForTypeContext : BaseContext
{
	public Type ModelType { get; set; }
	public ModelMetadata Metadata { get; set; }

	/// <inheritdoc />
	public GetMetadataForTypeContext(ExtendableModelMetadataProvider provider) : base(provider)
	{
	}
}
