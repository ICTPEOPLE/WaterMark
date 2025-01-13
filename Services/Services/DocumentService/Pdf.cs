
using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Font;

namespace Services.DocumentService
{
    public class Pdf
    {
        public void AddDiagonalWatermarks(string inputFile, string outputFile, string topText, string middleText)
        {
            try
            {
                using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(inputFile), new PdfWriter(outputFile)))
                using (Document doc = new Document(pdfDoc))
                {
                    string fontPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "MALGUN.TTF");
                    PdfFont font = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

                    PdfExtGState gs1 = new PdfExtGState().SetFillOpacity(0.3f);
                    
                    for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                    {
                        PdfPage pdfPage = pdfDoc.GetPage(i);
                        Rectangle pageSize = pdfPage.GetPageSize();
                        float centerX = pageSize.GetWidth() / 2;
                        float centerY = pageSize.GetHeight() / 2;
                        float topY = pageSize.GetHeight() - 10;

                        PdfCanvas over = new PdfCanvas(pdfPage);
                        over.SaveState();
                        over.SetExtGState(gs1);

                        Paragraph topParagraph = new Paragraph(topText)
                                .SetFont(font)
                                .SetFontSize(35)
                                .SetFontColor(iText.Kernel.Colors.ColorConstants.GRAY)
                                .SetOpacity(0.3f);

                        doc.ShowTextAligned(topParagraph, centerX, topY , i,TextAlignment.CENTER,VerticalAlignment.TOP,0);

                        Paragraph middleParagraph = new Paragraph(middleText)
                               .SetFont(font)
                               .SetFontSize(75)
                               .SetFontColor(iText.Kernel.Colors.ColorConstants.DARK_GRAY)
                               .SetOpacity(0.3f);

                        doc.ShowTextAligned(middleParagraph, centerX, centerY, i, TextAlignment.CENTER, VerticalAlignment.MIDDLE, 45);

                        over.RestoreState();
                    }
                }
                Console.WriteLine("워터마크 추가 완료.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("PDF 처리 중 오류 발생: " + ex.Message);
            }
        }

    }
}
