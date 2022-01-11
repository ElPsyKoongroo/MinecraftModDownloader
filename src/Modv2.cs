using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace webScrapWPF;

public class ModInfo
{
    public string          mod_id;
    public string          slug;
    public string          author;
    public string          title;
    public string          description;
    public List<string>    categories;
    public List<string>    versions;
    public string          page_url;
    public string          icon_url;
    public string          author_url;
    public string          date_created;
    public string          date_modified;
    public string          latest_version;
    public string          license;
    public string          client_side;
    public string          server_side;
    public string          host;
    public int             follows;
    public int             downloads;
    public List<VersionInfo> versionsDetailled;
    public bool isLoaded = false;

    public async Task LoadVersions()
    {
        if (isLoaded) return;
        string jsonInStringFormat = await Scraper.get(Url.getVersionsUrl(mod_id.Replace("local-", "")));
        versionsDetailled = JsonSerializer.Deserialize<List<VersionInfo>>(jsonInStringFormat, new JsonSerializerOptions(){IncludeFields=true});
        isLoaded = true;
    }

     
}

public class VersionInfo
{
    public int              downloads;
    public bool             featured;
    public string           id;
    public string           mod_id;
    public string           author_id;
    public string           name;
    public string           version_number;
    public string           changelog;
    public string           changelog_url;
    public string           date_published;
    public string           version_type;
    public List<FileInfo>   files;
    public List<string>     dependencies;
    public List<string>     game_versions;
    public List<string>     loaders;
}
public class FileInfo
{
    public Dictionary<string, string>   hashes;
    public string                       url;
    public string                       filename;
    public bool                         primary;
}