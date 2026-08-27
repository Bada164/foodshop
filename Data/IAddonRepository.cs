using foodshop.Models;

namespace foodshop.Data
{
    public interface IAddonRepository
    {
        IEnumerable<Addon> GetActiveAddons();
    }
}