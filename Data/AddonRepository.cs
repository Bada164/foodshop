using foodshop.Models;

namespace foodshop.Data
{
    public class AddonRepository : IAddonRepository
    {
        DataContext _entityFramework;

        public AddonRepository(IConfiguration config)
        {
            _entityFramework = new DataContext(config);
        }

        public IEnumerable<Addon> GetActiveAddons()
        {
            return _entityFramework.Addons
                .Where(a => a.IsActive)
                .ToList();
        }
    }
}