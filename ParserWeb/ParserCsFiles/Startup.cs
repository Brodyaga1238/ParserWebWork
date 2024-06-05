namespace ParserWeb
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Здесь вы можете настроить сервисы, которые ваше приложение будет использовать
            Console.WriteLine("Parser Program start");
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}