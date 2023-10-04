using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PK.Utils.AspNetCore.Contexts;

public class GetMetadataForPropertiesContext: BaseContext
{
	public Type ModelType { get; set; }

	public IEnumerable<ModelMetadata> Metadata { get; set; }

	/// <inheritdoc />
	public GetMetadataForPropertiesContext(ExtendableModelMetadataProvider provider) : base(provider)
	{
	}
}
