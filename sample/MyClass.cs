using Microsoft.Extensions.Logging;

namespace sample
{
    public class MyClass
    {
        private readonly ILogger<MyClass> _logger;

        public MyClass(ILogger<MyClass> logger)
        {
            _logger = logger;
        }

        public void DoStuff()
        {
            _logger.LogInformation($"Hello from {nameof(DoStuff)}");
            _logger.LogWarning($"Hello from {nameof(DoStuff)}");
            _logger.LogError($"Hello from {nameof(DoStuff)}");
        }
    }
}
