namespace ClassService.DTOs;

// Bruges når der skal oprettes eller opdateres en holdtemplate via API'et.
public class CreateClassTemplateDTO
{
    // Disse dataoplysninger der sendes ind.
    public string ClassName { get; set; } = "";
    public string ClassDescription { get; set; } = "";
    public string ClassType { get; set; } = "";
}