namespace foodshop.Models
{

    public class Addon
    {

        public int Id { get; set; }


        public string Name { get; set; } = string.Empty;


        public int Price { get; set; }


        public bool IsActive { get; set; } = true;
    }
}