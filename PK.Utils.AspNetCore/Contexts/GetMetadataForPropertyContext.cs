using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PK.Utils.AspNetCore.Contexts;

public class GetMetadataForPropertyContext : BaseContext
{
	public PropertyInfo PropertyInfo { get; set; }
	public Type ModelType { get; set; }
	public ModelMetadata Metadata { get; set; }

	/// <inheritdoc />
	public GetMetadataForPropertyContext(ExtendableModelMetadataProvider provider) : base(provider)
	{
	}
}
