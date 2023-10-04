using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PK.Utils.AspNetCore.Contexts;

public class GetMetadataForConstructorContext: BaseContext
{
	public ConstructorInfo ConstructorInfo { get; set; }
	public Type ModelType { get; set; }
	public ModelMetadata Metadata { get; set; }

	/// <inheritdoc />
	public GetMetadataForConstructorContext(ExtendableModelMetadataProvider provider) : base(provider)
	{
	}
}
