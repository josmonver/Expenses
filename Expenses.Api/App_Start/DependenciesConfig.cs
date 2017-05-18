using Autofac;
using Autofac.Features.Variance;
using Autofac.Integration.WebApi;
using AutoMapper;
using Expenses.Data;
using Expenses.SharedKernel.Validation;
using FluentValidation;
using FluentValidation.WebApi;
using MediatR;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Validation;

namespace Expenses.Api
{
    public static class DependenciesConfig
    {
        public static IContainer RegisterDependencies(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();

            // Register MediatR
            RegisterMediatR(builder);

            // Register Validation
            RegisterFluentValidation(builder);

            // Register mappings
            RegisterMappings(builder);

            // Register custom services
            builder.RegisterAssemblyTypes(typeof(Startup).Assembly).AsImplementedInterfaces();
            builder.RegisterType<ExpensesDbContext>().As<IExpensesDbContext>().InstancePerRequest();

            // Register Web API controllers
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly()).InstancePerRequest();

            builder.RegisterWebApiFilterProvider(config);

            return builder.Build();
        }

        private static void RegisterMediatR(ContainerBuilder builder)
        {
            // Enables contravariant Resolve() for interfaces with single contravariant ("in") arg
            builder.RegisterSource(new ContravariantRegistrationSource());
            // Mediator itself
            builder.RegisterAssemblyTypes(typeof(IMediator).Assembly).AsImplementedInterfaces();

            // Request handlers
            builder.Register<SingleInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });
            // Notification handlers
            builder.Register<MultiInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => (IEnumerable<object>)c.Resolve(typeof(IEnumerable<>).MakeGenericType(t));
            });
        }

        private static void RegisterFluentValidation(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(Startup).Assembly)
                   .Where(t => t.Name.EndsWith("Validator"))
                   .AsImplementedInterfaces()
                   .InstancePerLifetimeScope();

            builder.RegisterType<FluentValidationModelValidatorProvider>().As<ModelValidatorProvider>();

            builder.RegisterType<AutofacValidatorFactory>().As<IValidatorFactory>().SingleInstance();
        }

        private static void RegisterMappings(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(Startup).Assembly).As<Profile>();

            builder.Register(context =>
            {
                var profiles = context.Resolve<IEnumerable<Profile>>();

                var config = new MapperConfiguration(x =>
                {
                    // Load in all our AutoMapper profiles that have been registered
                    foreach (var profile in profiles)
                    {
                        x.AddProfile(profile);
                    }
                });

                return config;
            }).SingleInstance() // We only need one instance
            .AutoActivate() // Create it on ContainerBuilder.Build()
            .AsSelf(); // Bind it to its own type

            // HACK: IComponentContext needs to be resolved again as 'tempContext' is only temporary. See http://stackoverflow.com/a/5386634/718053 
            builder.Register(tempContext =>
            {
                var ctx = tempContext.Resolve<IComponentContext>();
                var config = ctx.Resolve<MapperConfiguration>();

                // Create our mapper using our configuration above
                return config.CreateMapper(t => ctx.Resolve(t));
            }).As<IMapper>() // Bind it to the IMapper interface
            .InstancePerLifetimeScope();
        }
    }
}