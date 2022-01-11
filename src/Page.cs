global using System;
global using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace webScrapWPF;

public class Page
{
    public List<ModInfo> hits;
    public List<ModInfo> mods { get { return hits; } }
    public int offset;
    public int limit;
    public int total_hits;

    public Page() { }
    public async Task Init(int pageNumber)
    {
        Url urlPrueba = new((pageNumber - 1) * 20, 20);
        var jsonInStringFormat = await Scraper.get(urlPrueba.getFullUrl());
        Page aux = JsonSerializer.Deserialize<Page>(jsonInStringFormat, new JsonSerializerOptions { IncludeFields = true});

        hits = aux.hits;
        offset = aux.offset;
        limit = aux.limit;
        total_hits = aux.total_hits;
    }
}

