using System;
using System.Collections.Generic;
using Painter.Data;
using Painter.Domain;
using Painter.Download;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Painter.CLI {
    public class DownloadRunner {
        public static void Execute(DownloadOptions opts) {
            ServiceCollection services = new();
            services.AddHttpClient<BenjaminMooreClient>();
            services.AddHttpClient<SherwinWilliamsClient>();
            services.AddSingleton<BenjaminMooreClientFactory>();
            services.AddSingleton<SherwinWilliamsClientFactory>();
            ServiceProvider serviceProvider = services.BuildServiceProvider();

            List<IColorClient> colorClients = new();
            if (opts.HasBenjaminMoore) {
                colorClients.Add(serviceProvider.GetService<BenjaminMooreClientFactory>().Create());
            }
            if (opts.HasSherwinWilliams) {
                colorClients.Add(serviceProvider.GetService<SherwinWilliamsClientFactory>().Create());
            }

            ConcurrentBag<ColorSwatch> colorSwatches = new();
            List<Task> tasks = new();
            foreach (IColorClient colorClient in colorClients) {
                foreach (string url in colorClient.GetUrls()) {
                    tasks.Add(new Task(() => colorClient.PopulateColors(url, colorSwatches)));
                }
            }
            Parallel.ForEach(tasks,
                new ParallelOptions {MaxDegreeOfParallelism = 20},
                task => {
                    task.Start();
                    task.Wait();
                }
            );

            SaveColors(colorSwatches, opts.DbFile);
        }

        private static void SaveColors(IEnumerable<ColorSwatch> colorSwatches, string dbFile) {
            using ColorContext colorContext = new(dbFile);
            colorContext.Database.EnsureCreated();
            foreach (ColorSwatch colorSwatch in colorSwatches) {
                if (!colorContext.TryAddNewColorSwatch(colorSwatch)) {
                    Console.WriteLine("Already added " + colorSwatch.Name + " " + colorSwatch.ColorNumbers[0].Number);
                }
            }
        }
    }
}
