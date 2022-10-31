namespace WebApi.Models;

public class Resource
{
    public List<LinkDTO> Links { get; set; } = new List<LinkDTO>();
}

public class LinkDTO
{
    public int Id { get; private set; }
    public string Href { get; private set; }
    public string Rel { get; private set; }
    public string Metodo { get; private set; }
    public LinkDTO(string href, string rel, string metodo)
    {
        Href = href;
        Rel = rel;
        Metodo = metodo;
    }
}
