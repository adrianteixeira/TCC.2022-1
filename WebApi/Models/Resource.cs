
namespace WebApi.Models;

public class Resource
{
    public List<LinkDTO> Links { get; set; } = new List<LinkDTO>();
}

public class LinkDTO
{
    public int? Id { get; private set; }
    public string Href { get; private set; }
    public string Rel { get; private set; }
    public string Method { get; private set; }
    public LinkDTO(string href, string rel, string method)
    {
        Href = href;
        Rel = rel;
        Method = method;
    }
}
