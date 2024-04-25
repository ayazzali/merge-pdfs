using System.Reflection;
using PdfSharp.Fonts;

namespace MergePdf;

class DemoFontResolver : IFontResolver
{
    public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        var name = familyName.ToLower();
 
        // Deal with the fonts we know.
        switch (name)
        {
            case "janitor":
                return new FontResolverInfo("Janitor#");
            case "arial":
                return new FontResolverInfo("Arial#");
        }
 
        // We pass all other font requests to the default handler.
        // When running on a web server without sufficient permission, you can return a default font at this stage.
        return PlatformFontResolver.ResolveTypeface(familyName, isBold, isItalic);
    }

    public byte[]? GetFont(string faceName)
    {switch (faceName)
        {
            case "Janitor#":
                return FontHelper.Janitor;
            case "Arial#":
                return FontHelper.Arial;
        }
 
        return null;
    }
}

public static class FontHelper
{
    public static byte[] Janitor
    {
        get { return LoadFontData("Janitor.otf"); }
    }
    
    public static byte[] Arial
    {
        get { return LoadFontData("arial.ttf"); }
    }
    static byte[] LoadFontData(string name)
    {
        var assembly = Assembly.GetExecutingAssembly();
 
        using (Stream stream = File.OpenRead(name))
        {
            if (stream == null)
                throw new ArgumentException("No resource with name " + name);
 
            int count = (int)stream.Length;
            byte[] data = new byte[count];
            stream.Read(data, 0, count);
            return data;
        }
    }
}