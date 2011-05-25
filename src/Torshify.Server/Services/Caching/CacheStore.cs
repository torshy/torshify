using System.Collections.Generic;

namespace Torshify.Server.Services.Caching
{
    public class CacheStore<TDto, TTorshifyEntity>
    {
        private readonly Dictionary<TTorshifyEntity, TDto> _torshifyToDtoCache = new Dictionary<TTorshifyEntity, TDto>();
        private readonly Dictionary<TDto, TTorshifyEntity> _dtoToTorshifyCache = new Dictionary<TDto, TTorshifyEntity>();

        private static readonly CacheStore<TDto, TTorshifyEntity> _instance = new CacheStore<TDto, TTorshifyEntity>();

        public static CacheStore<TDto, TTorshifyEntity> Instance
        {
            get { return _instance; }
        }

        public bool Contains(TTorshifyEntity entity)
        {
            return _torshifyToDtoCache.ContainsKey(entity);
        }

        public TDto Get(TTorshifyEntity entity)
        {
            TDto dto;
            _torshifyToDtoCache.TryGetValue(entity, out dto);
            return dto;
        }

        public TTorshifyEntity Get(TDto dto)
        {
            TTorshifyEntity entity;
            _dtoToTorshifyCache.TryGetValue(dto, out entity);
            return entity;
        }

        public void Put(TTorshifyEntity entity, TDto dto)
        {
            _torshifyToDtoCache[entity] = dto;
            _dtoToTorshifyCache[dto] = entity;
        }
    }
}