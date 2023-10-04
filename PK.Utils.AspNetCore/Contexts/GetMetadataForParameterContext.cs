using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PK.Utils.AspNetCore.Contexts;

public class GetMetadataForParameterContext: BaseContext
{
	public ParameterInfo Parameter { get; set; }


	public Type ModelType { get; set; }

	public ModelMetadata Metadata { get; set; }

	/// <inheritdoc />
	public GetMetadataForParameterContext(ExtendableModelMetadataProvider provider) : base(provider)
	{
	}
}
