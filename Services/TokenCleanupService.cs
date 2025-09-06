using CatalogoBackend.Data.IA;

namespace CatalogoBackend.Services
{
    public class TokenCleanupService : BackgroundService
    {
        private readonly IServiceProvider _sp;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);
        private readonly ILogger<TokenCleanupService> _logger;

        public TokenCleanupService(IServiceProvider sp, ILogger<TokenCleanupService> logger)
        {
            _sp = sp;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    //* Se crea un scope que obtiene el servicio necesario dentro del BackgroundService
                    using var scope = _sp.CreateScope();
                    var tokenDao = scope.ServiceProvider.GetService<ITokenDao>();

                    //* Se marcan como tokens inactivos los ya expirados
                    await tokenDao.RefreshTokens(stoppingToken);
                    _logger.LogInformation("Los tokens se han refrescado correctamente");
                }
                catch
                {
                    _logger.LogError("Error al refrescar los tokens");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
