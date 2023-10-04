using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Options;
using PK.Utils.AspNetCore.Contexts;
using PK.Utils.AspNetCore.Options;

namespace PK.Utils.AspNetCore;

/// <summary>
/// Extends default <see cref="DefaultModelMetadataProvider"/> metadata provider's virtual methods using Before/After extension points
/// </summary>
[PublicAPI]
public class ExtendableModelMetadataProvider : DefaultModelMetadataProvider
{
	private readonly IOptions<ExtendableModelMetadataProviderOptions> _options;

	public Func<DefaultMetadataDetails, ModelMetadata> CreateModelMetadataFunc => CreateModelMetadata;
	public Func<ModelMetadataIdentity, DefaultMetadataDetails[]> CreatePropertyDetailsFunc => CreatePropertyDetails;
	public Func<ModelMetadataIdentity, DefaultMetadataDetails> CreateTypeDetailsFunc => CreateTypeDetails;
	public Func<ModelMetadataIdentity, DefaultMetadataDetails> CreateParameterDetailsFunc => CreateParameterDetails;

	/// <inheritdoc />
	public ExtendableModelMetadataProvider(ICompositeMetadataDetailsProvider detailsProvider, IOptions<ExtendableModelMetadataProviderOptions> options) : base(detailsProvider)
	{
		_options = options ?? throw new ArgumentNullException(nameof(options));
	}

	/// <inheritdoc />
	public ExtendableModelMetadataProvider(ICompositeMetadataDetailsProvider detailsProvider, IOptions<MvcOptions> optionsAccessor, IOptions<ExtendableModelMetadataProviderOptions> options) : base(detailsProvider, optionsAccessor)
	{
		_options = options ?? throw new ArgumentNullException(nameof(options));
	}

	/// <inheritdoc />
	public override IEnumerable<ModelMetadata> GetMetadataForProperties(Type modelType)
	{
		var context = new GetMetadataForPropertiesContext(this)
		{
			ModelType = modelType
		};

		_options?.Value.OnBeforeGetMetadataForProperties?.Invoke(context);

		var metadata = context.Handled && context.Metadata != null
			? context.Metadata
			: base.GetMetadataForProperties(context.ModelType);

		context.Metadata = metadata;

		_options?.Value.OnAfterGetMetadataForProperties?.Invoke(context);

		return context.Handled ? context.Metadata : metadata;

	}

	/// <inheritdoc />
	public override ModelMetadata GetMetadataForParameter(ParameterInfo parameter)
	{
		var context = new GetMetadataForParameterContext(this)
		{
			Parameter = parameter
		};

		_options?.Value.OnBeforeGetMetadataForParameter?.Invoke(context);

		var metadata = context.Handled && context.Metadata != null
			? context.Metadata
			: base.GetMetadataForParameter(context.Parameter);

		context.Metadata = metadata;

		_options?.Value.OnAfterGetMetadataForParameter?.Invoke(context);

		return context.Handled ? context.Metadata : metadata;
	}

	/// <inheritdoc />
	public override ModelMetadata GetMetadataForParameter(ParameterInfo parameter, Type modelType)
	{
		var context = new GetMetadataForParameterContext(this)
		{
			Parameter = parameter,
			ModelType = modelType
		};

		_options?.Value.OnBeforeGetMetadataForParameter?.Invoke(context);

		var metadata = context.Handled && context.Metadata != null
			? context.Metadata
			: base.GetMetadataForParameter(context.Parameter, context.ModelType);

		context.Metadata = metadata;

		_options?.Value.OnAfterGetMetadataForParameter?.Invoke(context);

		return context.Handled ? context.Metadata : metadata;
	}

	/// <inheritdoc />
	public override ModelMetadata GetMetadataForType(Type modelType)
	{
		var context = new GetMetadataForTypeContext(this)
		{
			ModelType = modelType
		};

		_options?.Value.OnBeforeGetMetadataForType?.Invoke(context);

		var metadata = context.Handled && context.Metadata != null
			? context.Metadata
			: base.GetMetadataForType(context.ModelType);

		context.Metadata = metadata;

		_options?.Value.OnAfterGetMetadataForType?.Invoke(context);

		return context.Handled ? context.Metadata : metadata;
	}

	/// <inheritdoc />
	public override ModelMetadata GetMetadataForProperty(PropertyInfo propertyInfo, Type modelType)
	{
		var context = new GetMetadataForPropertyContext(this)
		{
			PropertyInfo = propertyInfo,
			ModelType = modelType
		};

		var metadata = context.Handled && context.Metadata != null
			? context.Metadata
			: base.GetMetadataForProperty(context.PropertyInfo, context.ModelType);

		context.Metadata = metadata;

		_options?.Value.OnAfterGetMetadataForProperty?.Invoke(context);

		return context.Handled ? context.Metadata : metadata;
	}

	/// <inheritdoc />
	public override ModelMetadata GetMetadataForConstructor(ConstructorInfo constructorInfo, Type modelType)
	{
		var context = new GetMetadataForConstructorContext(this)
		{
			ConstructorInfo = constructorInfo,
			ModelType = modelType
		};

		_options?.Value.OnBeforeGetMetadataForConstructor?.Invoke(context);

		var metadata = context.Handled && context.Metadata != null
			? context.Metadata
			: base.GetMetadataForConstructor(context.ConstructorInfo, context.ModelType);

		context.Metadata = metadata;

		_options?.Value.OnAfterGetMetadataForConstructor?.Invoke(context);

		return context.Handled ? context.Metadata : metadata;
	}

	/// <inheritdoc />
	protected override ModelMetadata CreateModelMetadata(DefaultMetadataDetails entry)
	{
		var context = new CreateModelMetadataContext(this)
		{
			Entry = entry
		};

		_options?.Value.OnBeforeCreateModelMetadata?.Invoke(context);

		var metadata = context.Handled && context.Metadata != null
			? context.Metadata
			: base.CreateModelMetadata(context.Entry);

		context.Metadata = metadata;

		_options?.Value.OnAfterCreateModelMetadata?.Invoke(context);

		return context.Handled ? context.Metadata : metadata;
	}

	/// <inheritdoc />
	protected override DefaultMetadataDetails[] CreatePropertyDetails(ModelMetadataIdentity key)
	{
		var context = new CreatePropertyDetailsContext(this)
		{
			Key = key
		};

		_options?.Value.OnBeforeCreatePropertyDetails?.Invoke(context);

		var metadata = context.Handled && context.Metadata != null
			? context.Metadata
			: base.CreatePropertyDetails(context.Key);

		context.Metadata = metadata;

		_options?.Value.OnAfterCreatePropertyDetails?.Invoke(context);

		return context.Handled ? context.Metadata : metadata;
	}

	/// <inheritdoc />
	protected override DefaultMetadataDetails CreateTypeDetails(ModelMetadataIdentity key)
	{
		var context = new CreateTypeDetailsContext(this)
		{
			Key = key
		};

		_options?.Value.OnBeforeCreateTypeDetails?.Invoke(context);

		var metadata = context.Handled && context.Metadata != null
			? context.Metadata
			: base.CreateTypeDetails(context.Key);

		context.Metadata = metadata;

		_options?.Value.OnAfterCreateTypeDetails?.Invoke(context);

		return context.Handled ? context.Metadata : metadata;
	}

	/// <inheritdoc />
	protected override DefaultMetadataDetails CreateParameterDetails(ModelMetadataIdentity key)
	{
		var context = new CreateParameterDetailsContext(this)
		{
			Key = key
		};

		_options?.Value.OnBeforeCreateParameterDetails?.Invoke(context);

		var metadata = context.Handled && context.Metadata != null
			? context.Metadata
			: base.CreateParameterDetails(context.Key);

		context.Metadata = metadata;

		_options?.Value.OnAfterCreateParameterDetails?.Invoke(context);

		return context.Handled ? context.Metadata : metadata;
	}
}
