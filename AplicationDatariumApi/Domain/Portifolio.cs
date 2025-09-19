using System.ComponentModel.DataAnnotations;

namespace AplicationDatariumApi.Domain;

public class Portifolio
{
    public long Id { get; set; }

    [MaxLength(160)]
    public string Name { get; set; } = default!;

    public RiskProfile RiskProfile { get; set; } = RiskProfile.Conservative;

   
    public decimal TotalAmount { get; set; }

  
    public List<Asset> Assets { get; set; } = new();
}