using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using PK.Utils.AspNetCore.Options;

namespace PK.Utils.AspNetCore.Extensions;

public static class DependencyInjection
{
	/// <summary>
	/// Adds <see cref="ExtendableModelMetadataProvider"/> as <see cref="IModelMetadataProvider"/>, which allows to extend
	/// default metadata provider's virtual methods using Before/After extension points
	/// </summary>
	/// <param name="services">Service collection</param>
	/// <param name="configureAction">Action for configuring <see cref="ExtendableModelMetadataProviderOptions"/></param>
	/// <returns>Service collection</returns>
	public static IServiceCollection AddExtendableModelMetadataProvider(this IServiceCollection services, Action<ExtendableModelMetadataProviderOptions> configureAction = null)
	{
		services.Configure<ExtendableModelMetadataProviderOptions>(options => configureAction?.Invoke(options));
		services.AddSingleton<IModelMetadataProvider, ExtendableModelMetadataProvider>();
		return services;
	}
}
