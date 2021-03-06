using System;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using SocialMedia.Core.CustomEntities;
using SocialMedia.Core.Interfaces;
using SocialMedia.Core.Services;
using SocialMedia.Infrastructure.Data;
using SocialMedia.Infrastructure.Filters;
using SocialMedia.Infrastructure.Interfaces;
using SocialMedia.Infrastructure.Repositories;
using SocialMedia.Infrastructure.Services;

namespace SocialMedia.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Inyeccion de dependencias

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //Se utiliza esta libreria AddNewtonsoftJson para manejar el error de referencias circulares en el llamado de los objetos
            services.AddControllers(options =>
                {
                    options.Filters.Add<GlobalExceptionFilter>();
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });

            services.Configure<PaginationOptions>(Configuration.GetSection("Pagination"));
            services.AddDbContext<SocialMediaContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SocialMedia"));
            });

            /*Cada vez que en el programa se haga uso de esta abastraccion/interface(IPostRepository),
             * yo le voy a entregar(resolver) a esa clase una instancia de esta implementacion(PostRepository).
             */
            //services.AddTransient<IPostRepository, PostRepository>();
            //services.AddTransient<IUserRepository, UserRepository>();

            //Se definió un IPostService para conectar con el IPostRepository a su vez conectado al baseRepository
            services.AddTransient<IPostService, PostService>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            //Esta dependencia se resuelve como singleton debido a que se va a manejar una unica instancia para toda la app,
            //este servicio no maneja estado,solamente una entrada y genera una salida, no crea una instancia cada vez que se hace una solicitud
            services.AddSingleton<IUriService>(provider =>
            {
                var accesor = provider.GetRequiredService<IHttpContextAccessor>();
                var request = accesor.HttpContext.Request;
                var absoluteUri = string.Concat(request.Scheme, "://",request.Host.ToUriComponent());
                return new UriService(absoluteUri);
            });

            services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));

            //Permite validar los campos de los modelos utilizando la clase ValidationFilter como filtro.
            services.AddMvc(options => { options.Filters.Add<ValidationFilter>(); }).AddFluentValidation(options =>
            {
                options.RegisterValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}