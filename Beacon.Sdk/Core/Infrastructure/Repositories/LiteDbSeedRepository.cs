using System;
using LiteDB;

namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Entities;
    using Domain.Interfaces.Data;
    using Microsoft.Extensions.Logging;

    // Todo: Add secure storage
    public class LiteDbSeedRepository : BaseLiteDbRepository<SeedEntity>, ISeedRepository
    {
        private const string CollectionName = "Seed";

        public LiteDbSeedRepository(ILogger<LiteDbSeedRepository> logger, RepositorySettings settings)
            : base(logger, settings)
        {
        }

        public async Task<SeedEntity> CreateAsync(string seed)
        {
            var func = new Func<LiteCollection<SeedEntity>, Task<SeedEntity>>(col =>
            {
                var data = new SeedEntity
                {
                    Seed = seed
                };

                col.Insert(data);

                return Task.FromResult(data);
            });

            try
            {
                return await InConnection(CollectionName, func);
            }
            catch
            {
                await DropAsync(CollectionName);

                return await InConnection(CollectionName, func);
            }
        }

        public Task<SeedEntity?> TryReadAsync() =>
            InConnectionNullable(CollectionName, col =>
            {
                SeedEntity? seedEntity = col.FindAll().FirstOrDefault();
                return Task.FromResult(seedEntity ?? null);
            });

        public Task<List<SeedEntity>> ReadAllAsync() =>
            InConnection(CollectionName, col =>
            {
                var seedEntities = col.FindAll().ToList();

                return Task.FromResult(seedEntities);
            });
    }
}