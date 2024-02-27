using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
        var pdfs = context.Request.Form.Files;
        var resultStream = new MemoryStream();
        using (var document = new PdfDocument())
        {
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