using System.ComponentModel.DataAnnotations;

public class Career
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category is required")]
    public string Category { get; set; } = string.Empty;

    [Required(ErrorMessage = "Required skills are mandatory")]
    public string RequiredSkills { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "Salary must be positive")]
    public decimal AverageSalary { get; set; }
}