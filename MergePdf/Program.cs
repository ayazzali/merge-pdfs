using System.Reflection;
using MergePdf;
using Microsoft.Extensions.Primitives;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

var infoVersion = Assembly.GetExecutingAssembly()?
    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
    .InformationalVersion;
Console.WriteLine($"app version:{infoVersion}");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.WebHost.UseKestrel(o => o.Limits.MaxRequestBodySize = null);

var app = builder.Build();

{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapGet("/", _ =>
    {
        _.Response.Redirect("/swagger");
        return Task.CompletedTask;
    });
}
app.MapPost("/mergePdfFiles", (HttpContext context) =>
    {
        context.Request.Form.TryGetValue("title", out StringValues titleVals);
        context.Request.Form.TryGetValue("secondTitle", out StringValues secondTitleVals);
        var pdfs = context.Request.Form.Files;
        var resultStream = new MemoryStream();
        using (var document = new PdfDocument())
        {
            if (titleVals.Any())
            {
                var title = titleVals.First();
                var page = document.AddPage();
                page.MediaBox = new PdfRectangle(new XRect(0, 0, 200, 100));
                var gfx = XGraphics.FromPdfPage(page);
                var font = new XFont("arial", 10, XFontStyleEx.BoldItalic);
                gfx.DrawString(title, font, XBrushes.DarkBlue, 5, 30);
                gfx.DrawString(secondTitleVals.FirstOrDefault() ?? "", font, XBrushes.DarkBlue, 5, 50);
            }

            foreach (var externalPdfFile in pdfs)
            {
                var externalPdf = PdfReader.Open(externalPdfFile.OpenReadStream(), PdfDocumentOpenMode.Import);
                if (externalPdf.Pages.Count < 1)
                    //log
                    continue;

                document.AddPage(externalPdf.Pages[0]);
            }

            document.Save(resultStream);
        }

        return Results.File(resultStream, "application/pdf", "ruvelt.pdf");
    })
    .WithName("MergePdfFiles")
    .WithOpenApi();

app.MapGet("/test", () =>
    Results.Content("""
                    <form enctype="multipart/form-data" method="post" action="/mergePdfFiles">
                        <input name="pdfs[0]" type="file">
                        <input name="pdfs[1]" type="file">
                        <button>Send</button>
                    </form>
                    """, "text/html")
);

GlobalFontSettings.FontResolver = new DemoFontResolver();

app.Run();


// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())

// app.UseAntiforgery();
// app.UseHttpsRedirection();

// app.MapGet("/", () =>
//     Results.Redirect("/swagger")
// );

// [FromForm] IEnumerable<IFormFile> pdfs 

// document.Info.Title = "Created with PDFsharp";
// document.Info.Subject = "Just a simple Hello-World program.";