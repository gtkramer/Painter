using System;
using Microsoft.Extensions.DependencyInjection;

namespace Painter.Download {
    public class SherwinWilliamsClientFactory {
        private readonly IServiceProvider _serviceProvider;

        public SherwinWilliamsClientFactory(IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
        }

        public SherwinWilliamsClient Create() {
            return _serviceProvider.GetRequiredService<SherwinWilliamsClient>();
        }
    }
}
