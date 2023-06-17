using System;
using Microsoft.Extensions.DependencyInjection;

namespace Painter.Download {
    public class BenjaminMooreClientFactory {
        private readonly IServiceProvider _serviceProvider;

        public BenjaminMooreClientFactory(IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
        }

        public BenjaminMooreClient Create() {
            return _serviceProvider.GetRequiredService<BenjaminMooreClient>();
        }
    }
}
