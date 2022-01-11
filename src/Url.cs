using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;


namespace webScrapWPF;

public class Url
{

    private string baseUrl = "https://api.modrinth.com/api/v1/mod?";
    private int offset;
    private int length;

    public Url(int _offset, int _lenght)
    {
        this.offset = _offset;
        this.length = _lenght;
    }

    public string getFullUrl()
    {
        return $"{baseUrl}limit={length.ToString()}&offset={offset.ToString()}";
    }

    public static string getVersionsUrl(string modId)
    {
        return $"https://api.modrinth.com/api/v1/mod/{modId}/version";
    }

}

