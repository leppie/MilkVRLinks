using System.IO;
using System.Text.RegularExpressions;

class Program
{
    static void Main(string[] args)
    {
        var settings = MilkVRLinks.Properties.Settings.Default;
        var searchpattern = settings.SearchPattern;
        var baseurl = settings.BaseUrl;

        if (args.Length > 0)
        {
            baseurl = args[0];

            if (args.Length > 1)
            {
                searchpattern = args[1];
            }
        }

        foreach (var f in Directory.EnumerateFiles(".", searchpattern))
        {
            Video.FromFile(Path.GetFileName(f), baseurl).ToMVRL();
        }
    }
}

class Video
{
    string Url, VideoType, AudioType;
    public string Title { get; private set; }

    public void ToMVRL()
    {
        using (var w = new StreamWriter($"{Title}.mvrl", false))
        {
            w.WriteLine(Url);
            w.WriteLine(VideoType);
            w.WriteLine(AudioType);
        }
    }

    static readonly Regex vtr = new Regex(@"(180x1[86]0_)?(squished_)?3d[vh]", RegexOptions.Compiled);

    public static Video FromFile(string filename, string baseurl)
    {
        var url = $"{baseurl.TrimEnd('/')}/{filename.Replace(" ", "%20")}";
        var fne = Path.GetFileNameWithoutExtension(filename);
        var vt = vtr.Match(fne)?.Value;

        if (!string.IsNullOrEmpty(vt))
        {
            fne = fne.Replace(vt, "");
        }

        fne = fne.TrimEnd('_');

        return new Video
        {
            Title = fne,
            Url = url,
            VideoType = vt,
            AudioType = ""
        };
    }
}