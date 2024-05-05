namespace TernakSepatu.Dtos
{
    public class BrandDto
    {
        public int Id { get; set; }

        public string BrandName { get; set; }
        public IFormFile? ImageURL { get; set; }

    }
}
