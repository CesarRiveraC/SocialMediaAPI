using System;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using SocialMedia.Core.Interfaces;
using SocialMedia.Core.Services;
using SocialMedia.Infrastructure.Data;
using SocialMedia.Infrastructure.Filters;
using SocialMedia.Infrastructure.Repositories;

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
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            services.AddDbContext<SocialMediaContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SocialMedia")));

            /*Cada vez que en el programa se haga uso de esta abastraccion/interface(IPostRepository),
             * yo le voy a entregar(resolver) a esa clase una instancia de esta implementacion(PostRepository).
             */
            services.AddTransient<IPostService, PostService>();
            services.AddTransient<IPostRepository, PostRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            //
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