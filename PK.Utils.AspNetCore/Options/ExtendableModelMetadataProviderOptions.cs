using System;
using PK.Utils.AspNetCore.Contexts;

namespace PK.Utils.AspNetCore.Options;

public class ExtendableModelMetadataProviderOptions
{
	public Action<GetMetadataForPropertiesContext> OnBeforeGetMetadataForProperties { get; set; }
	public Action<GetMetadataForPropertiesContext> OnAfterGetMetadataForProperties { get; set; }

	public Action<GetMetadataForParameterContext> OnBeforeGetMetadataForParameter { get; set; }
	public Action<GetMetadataForParameterContext> OnAfterGetMetadataForParameter { get; set; }

	public Action<GetMetadataForTypeContext> OnBeforeGetMetadataForType { get; set; }
	public Action<GetMetadataForTypeContext> OnAfterGetMetadataForType { get; set; }

	public Action<GetMetadataForPropertyContext> OnBeforeGetMetadataForProperty { get; set; }
	public Action<GetMetadataForPropertyContext> OnAfterGetMetadataForProperty { get; set; }

	public Action<GetMetadataForConstructorContext> OnBeforeGetMetadataForConstructor { get; set; }
	public Action<GetMetadataForConstructorContext> OnAfterGetMetadataForConstructor { get; set; }

	public Action<CreateModelMetadataContext> OnBeforeCreateModelMetadata { get; set; }
	public Action<CreateModelMetadataContext> OnAfterCreateModelMetadata { get; set; }

	public Action<CreatePropertyDetailsContext> OnBeforeCreatePropertyDetails { get; set; }
	public Action<CreatePropertyDetailsContext> OnAfterCreatePropertyDetails { get; set; }

	public Action<CreateTypeDetailsContext> OnBeforeCreateTypeDetails { get; set; }
	public Action<CreateTypeDetailsContext> OnAfterCreateTypeDetails { get; set; }

	public Action<CreateParameterDetailsContext> OnBeforeCreateParameterDetails { get; set; }
	public Action<CreateParameterDetailsContext> OnAfterCreateParameterDetails { get; set; }
}
