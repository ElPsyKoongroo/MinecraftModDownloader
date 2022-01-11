using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net;
using System.Diagnostics;
using System.IO;

namespace webScrapWPF;
public class Mod
{
    private string desc;
    public string name { get; }
    public string Desc { get{return desc;} }
    string versionsLink;
    public string iconLink;
    private bool versionScrap;
    public bool VersionsScrap { get { return VersionsScrap; } }

    public Dictionary<string, string> versions = new();
    public Dictionary<string, string> modStats = new(){
        {"downloads", ""},
        {"last version", ""},
        {"last update", ""},
        {"client side", ""},
        {"server side", ""},
        {"tags", ""}
    };

    public string VersionsLink
    {
        set { versionsLink = value; }
        get { return versionsLink; }
    }

    public Mod(string projectStr = "")
    {
        if(projectStr == "")
        {
            versionsLink    = "";
            iconLink        = "";
            name            = "";
        }
        string regPat = "<h2 class=\"title\".*?</h2>";
        string h2 = Regex.Match(projectStr, regPat).Value;

        string href = Regex.Match(h2, "/mod/([\\w-]+)(?=\")").Value;
        versionsLink = $"https://www.modrinth.com{href}/versions";

        string iconRegex = "class=\"icon\".+?<img src=\"(.+?)\"";

        iconLink = Regex.Match(projectStr, iconRegex).Groups[1].Value;

        name = Regex
                .Match(h2, "(<.+><.+>)(.+)(</a></h2>)")
                .Groups[2]
                .Value;
        versionScrap = false;
        desc = "";
    }

    public async Task<bool> GetVersions()
    {
        bool ret;
        if (versionScrap) return true;
        try
        {
            
            string body = (await Scraper.get(versionsLink));

            Match tbodyMatch = Regex.Match(body, "<tbody.*?</tbody>");

            string tbody = tbodyMatch.Value;

            List<string> tr = Regex
                                .Matches(tbody, "<tr.*?</tr>")
                                .Select(x => x.Value)
                                .ToList();

            foreach (string version in tr)
            {
                List<string> td = Regex
                                    .Matches(version, "<td.*?</td>")
                                    .Select(x => x.Value)
                                    .ToList();

                string ver  = Regex.Match(td[4], "(?<=>).*?(?=<)").         Value.Trim();
                string href = Regex.Match(td[0], "(?<=href=\").*?(?=\")").  Value.Trim();

                versions.TryAdd(ver, href);
            }
            GetStats(body);
            ret = true;
            versionScrap=true;
        }
        catch 
        {
            ret = false;
        }
        return ret;
    }

    public void GetStats(String body)
    {

        string          statsRegex          = "<div class=\"info\"(.*\n)*?.*?</div>";
        string          valueStatRegex      = "<p .+?>(.+?)</p>";
        MatchCollection stats               = Regex.Matches(body, statsRegex);

        string text = string.Join("", stats.Select(x => x.Value).ToArray());

        MatchCollection values              = Regex.Matches(text, valueStatRegex);

        // Fixed values hope to work
        this.desc                           = values[0].Groups[1].Value;
        this.modStats["downloads"]      = values[2].Groups[1].Value;
        this.modStats["last version"]   = values[4].Groups[1].Value;
        this.modStats["last update"]    = values[5].Groups[1].Value;
        this.modStats["client side"]    = values[6].Groups[1].Value;
        this.modStats["server side"]    = values[7].Groups[1].Value;
    }

    public void ShowVersions()
    {
        Console.WriteLine($"Mod: {name}");
        foreach(var pair in versions)
        {
            Console.WriteLine($"Version-> {pair.Key}\n\n");
        }
        Console.WriteLine("\n\n\n");
        
        foreach(var pair in modStats)
        {
            Console.WriteLine($"{pair.Key}: {pair.Value}");
        }
    }
}


/*
    ENCONTRAR LO SIGUIENTE EN ProjectCard.version                   5 <- No le eches cuenta a este numero

    <tbody data-v-1b482dd2="">
        TEXXXXXXXXXXXXXXXXXXXXXXXXXXXXXTO
    </tbody> 

    Cuando encuentres esto, dentro va a tener unos cuantos <tr bla bla bla> texto </tr>
                                                            ^^^^^^^^^^^^^^^^^^^^^^^^^^
                                                            Estos son las diferentes versiones del mod

    Dentro de los <tr> hay unos cuantos <td bla bla bla> texto </td>
    Nos interesan el primero (0) y el quinto(4)

    En el primero esta el href con el link al archivo .jar

        
                                                                            AQUI
        <td data-v-1b482dd2="">         VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV      
            <a data-v-1b482dd2="" href="https://cdn.modrinth.com/data/AZomiSrC/versions/mc1.17.1-0.3.1/hydrogen-fabric-mc1.17.1-0.3.jar" class="download">
                <svg data-v-1b482dd2="" width="24" height="24" viewBox="0 0 24 24" fill="none" class="">
                    <path data-v-1b482dd2="" d="M4 16L4 17C4 18.6569 5.34315 20 7 20L17 20C18.6569 20 20 18.6569 20 17L20 16M16 12L12 16M12 16L8 12M12 16L12 4" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                    </path>
                </svg>
            </a>
        </td>


    En el quinto esta la version del mod 1.18, 1.17, 1.16, lo que sea



    <td data-v-1b482dd2="">1.17, 1.17.1</td>
                           ^^^^^^^^^^^^
                               ESTO


    no hace falta que guardemos nada porque es muy lento, just podemos guardar todo el texto y cuando el usuario quiera el mod hacemos todo lo de buscarlo
    mi coso en python va relativamente rapido porque solo hace 1 o 2 request como mucho

    es solo cuando dices que descargues X mod que hace mas requests

    y cada request que hago lo guardo, yo tengo una clase Page que tiene una lista con 20 Mod (que Mod es otra clase yeah)

    y pongo algo talque:
        self.pages[actual_page].get_mod(user_input).download()

    yeah, el download realmente lo que hace es mostrar las versiones

       0  1.18
       1  1.17
       2  etc

    ya metes el input y se descarga
    la clase page tiene un bool que es cargada = True/False
    yeah
    {
        0: Page
        5: Page
        1: Page
    }

    asi las puedo tener desordenadas
    mah o menoh

    

*/

/*

           <div data-v-e8c4e07a="" class="info">
               <h4 data-v-e8c4e07a="">
                   Downloads
               </h4> 
               <p data-v-e8c4e07a="" class="value">
                   124,824
               </p>
           </div>

           <p bla bla bla > TEXTO_IMPORTANTE </p>



           <div data-v-e8c4e07a="" class="mod-stats section"><div data-v-e8c4e07a="" class="stat"><svg data-v-e8c4e07a="" width="24" height="24" viewBox="0 0 24 24" fill="none" class=""><path data-v-e8c4e07a="" d="M4 16L4 17C4 18.6569 5.34315 20 7 20L17 20C18.6569 20 20 18.6569 20 17L20 16M16 12L12 16M12 16L8 12M12 16L12 4" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"></path></svg> <div data-v-e8c4e07a="" class="info"><h4 data-v-e8c4e07a="">Downloads</h4> <p data-v-e8c4e07a="" class="value">124,824</p></div></div> <div data-v-e8c4e07a="" class="stat"><svg data-v-e8c4e07a="" width="24" height="24" viewBox="0 0 24 24" fill="none" class=""><path data-v-e8c4e07a="" d="M8 7V3M16 7V3M7 11H17M5 21H19C20.1046 21 21 20.1046 21 19V7C21 5.89543 20.1046 5 19 5H5C3.89543 5 3 5.89543 3 7V19C3 20.1046 3.89543 21 5 21Z" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"></path></svg> <div data-v-e8c4e07a="" class="info"><h4 data-v-e8c4e07a="">Created</h4> <p data-v-e8c4e07a="" class="value has-tooltip" data-original-title="null">
              a year ago
            </p></div></div> <div data-v-e8c4e07a="" class="stat"><svg data-v-e8c4e07a="" width="24" height="24" viewBox="0 0 24 24" fill="none" class=""><path data-v-e8c4e07a="" d="M7 7H7.01M7 3H12C12.5119 2.99999 13.0237 3.19525 13.4142 3.58579L20.4143 10.5858C21.1953 11.3668 21.1953 12.6332 20.4143 13.4142L13.4142 20.4142C12.6332 21.1953 11.3668 21.1953 10.5858 20.4142L3.58579 13.4142C3.19526 13.0237 3 12.5118 3 12V7C3 4.79086 4.79086 3 7 3Z" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"></path></svg> <div data-v-e8c4e07a="" class="info"><h4 data-v-e8c4e07a="">Available For</h4> <p data-v-e8c4e07a="" class="value">
              1.18
            </p></div></div> <div data-v-e8c4e07a="" class="stat"><svg data-v-e8c4e07a="" width="24" height="24" viewBox="0 0 24 24" fill="none" class=""><path data-v-e8c4e07a="" d="M11 5H6C4.89543 5 4 5.89543 4 7V18C4 19.1046 4.89543 20 6 20H17C18.1046 20 19 19.1046 19 18V13M17.5858 3.58579C18.3668 2.80474 19.6332 2.80474 20.4142 3.58579C21.1953 4.36683 21.1953 5.63316 20.4142 6.41421L11.8284 15H9L9 12.1716L17.5858 3.58579Z" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"></path></svg> <div data-v-e8c4e07a="" class="info"><h4 data-v-e8c4e07a="">Updated</h4> <p data-v-e8c4e07a="" class="value has-tooltip" data-original-title="null">
              a month ago
            </p></div></div> <div data-v-e8c4e07a="" class="stat"><svg data-v-e8c4e07a="" fill="none" viewBox="0 0 24 24" stroke="currentColor" class=""><path data-v-e8c4e07a="" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.75 17L9 20l-1 1h8l-1-1-.75-3M3 13h18M5 17h14a2 2 0 002-2V5a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"></path></svg> <div data-v-e8c4e07a="" class="info"><h4 data-v-e8c4e07a="">Client Side</h4> <p data-v-e8c4e07a="" class="value capitalize">required</p></div></div> <div data-v-e8c4e07a="" class="stat"><svg data-v-e8c4e07a="" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class=""><line data-v-e8c4e07a="" x1="22" y1="12" x2="2" y2="12"></line><path data-v-e8c4e07a="" d="M5.45 5.11L2 12v6a2 2 0 0 0 2 2h16a2 2 0 0 0 2-2v-6l-3.45-6.89A2 2 0 0 0 16.76 4H7.24a2 2 0 0 0-1.79 1.11z"></path><line data-v-e8c4e07a="" x1="6" y1="16" x2="6.01" y2="16"></line><line data-v-e8c4e07a="" x1="10" y1="16" x2="10.01" y2="16"></line></svg> <div data-v-e8c4e07a="" class="info"><h4 data-v-e8c4e07a="">Server Side</h4> <p data-v-e8c4e07a="" class="value capitalize">unsupported</p></div></div> <div data-v-e8c4e07a="" class="stat"><svg data-v-e8c4e07a="" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-file-text"><path data-v-e8c4e07a="" d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path><polyline data-v-e8c4e07a="" points="14 2 14 8 20 8"></polyline><line data-v-e8c4e07a="" x1="16" y1="13" x2="8" y2="13"></line><line data-v-e8c4e07a="" x1="16" y1="17" x2="8" y2="17"></line><polyline data-v-e8c4e07a="" points="10 9 9 9 8 9"></polyline></svg> <div data-v-e8c4e07a="" class="info"><h4 data-v-e8c4e07a="">License</h4> <p data-v-e8c4e07a="" class="value ellipsis has-tooltip" data-original-title="null"><a data-v-e8c4e07a="" href="https://cdn.modrinth.com/licenses/lgpl-3.txt">
                LGPL-3</a></p></div></div> <div data-v-e8c4e07a="" class="stat"><svg data-v-e8c4e07a="" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor" class=""><path data-v-e8c4e07a="" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 20l4-16m4 4l4 4-4 4M6 16l-4-4 4-4"></path></svg> <div data-v-e8c4e07a="" class="info"><h4 data-v-e8c4e07a="">Project ID</h4> <p data-v-e8c4e07a="" class="value">AANobbMI</p></div></div> <div data-v-b7cea926="" data-v-e8c4e07a="" class="categories"><!----> <!----> <!----> <!----> <!----> <p data-v-b7cea926=""><svg data-v-b7cea926="" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" class=""><rect data-v-b7cea926="" x="2" y="7" width="20" height="14" rx="2" ry="2"></rect><path data-v-b7cea926="" d="M16 21V5a2 2 0 0 0-2-2h-4a2 2 0 0 0-2 2v16"></path></svg>
    Utility
  </p> <!----> <!----> <!----> <!----> <!----> <!----> <!----> <!----></div></div>


       (?<=<p.+?>).+?(?=</p>)



       */