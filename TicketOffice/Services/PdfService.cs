using System.Globalization;
using TicketOffice.Models;
using Microsoft.AspNetCore.Mvc;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Writer;

namespace TicketOffice.Services;

public class PdfService
{
    // Generates and returns PDF representation of some ticket
    public FileStreamResult GetTicketPdf(Ticket ticket)
    {
        // Set culture info to be able to correctly convert date To String
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en");

        PdfDocumentBuilder builder = new PdfDocumentBuilder();

        byte[] robotoBytes = File.ReadAllBytes(
            $"{AppDomain.CurrentDomain.BaseDirectory}" +
            $"../../../wwwroot/fonts/Roboto-Regular.ttf");

        PdfDocumentBuilder.AddedFont roboto =
            builder.AddTrueTypeFont(robotoBytes);

        PdfPageBuilder page = builder.AddPage(PageSize.A4);

        PdfPoint topCenter = new PdfPoint(
            page.PageSize.Width / 2,
            page.PageSize.Top - 25);
        PdfPoint firstParagraphPoint =
            new PdfPoint(15, page.PageSize.Top - 50);
        int lineHeight = 25;

        page.AddText(
            $"BusRoutesBuddy – Ticket No.{ticket.Id}",
            16,
            topCenter.Translate(-75, 0),
            roboto);

        page.AddText(
            $"Route number:",
            14,
            firstParagraphPoint,
            roboto);
        page.AddText(
            $"No. {ticket.Route.Number}",
            14,
            firstParagraphPoint.Translate(250, 0),
            roboto);

        page.AddText(
            "Passenger, seat:",
            14,
            firstParagraphPoint.Translate(0, -lineHeight),
            roboto);
        page.AddText(
            $"{ticket.PassengerLastName} {ticket.PassengerFirstName}," +
                $" No. {ticket.PassengerPlace}",
            14,
            firstParagraphPoint.Translate(250, -lineHeight),
            roboto);

        page.AddText(
            "Date and city of Departure:",
            14,
            firstParagraphPoint.Translate(0, 2 * -lineHeight),
            roboto);
        page.AddText(
            $"{ticket.Cities.First().Name}," +
                $" {ticket.Cities.First().DepartureTime?.ToString("f").Split(",")[0].ToLower()}," +
                $" {ticket.Cities.First().DepartureTime?.ToString("dd.MM.yyyy")}," +
                $" {ticket.Cities.First().DepartureTime?.ToString("HH:mm")}",
            14,
            firstParagraphPoint.Translate(250, 2 * -lineHeight),
            roboto);

        page.AddText(
            "Date and city of arrival:",
            14,
            firstParagraphPoint.Translate(0, 3 * -lineHeight),
            roboto);
        page.AddText(
            $"{ticket.Cities.Last().Name}," +
                $" {ticket.Cities.Last().ArrivalTime?.ToString("f").Split(",")[0].ToLower()}," +
                $" {ticket.Cities.Last().ArrivalTime?.ToString("dd.MM.yyyy")}," +
                $" {ticket.Cities.Last().ArrivalTime?.ToString("HH:mm")}",
            14,
            firstParagraphPoint.Translate(250, 3 * -lineHeight),
            roboto);

        page.AddText(
            "Price:",
            14,
            firstParagraphPoint.Translate(0, 4 * -lineHeight),
            roboto);
        page.AddText(
            $"{ticket.GetTotalCost()}",
            14,
            firstParagraphPoint.Translate(250, 4 * -lineHeight),
            roboto);

        page.AddText(
            "Date of purchasing ticket:",
            14,
            firstParagraphPoint.Translate(0, 6 * -lineHeight),
            roboto);
        page.AddText(
            $"{ticket.OderDate.ToString("dd.MM.yyyy, HH:mm:ss")}",
            14,
            firstParagraphPoint.Translate(250, 6 * -lineHeight),
            roboto);

        page.AddText(
            "Date of printing ticket:",
            14,
            firstParagraphPoint.Translate(0, 7 * -lineHeight),
            roboto);
        page.AddText(
            $"{DateTime.Now.ToString("dd.MM.yyyy, HH:mm:ss")}",
            14,
            firstParagraphPoint.Translate(250, 7 * -lineHeight),
            roboto);

        byte[] document = builder.Build();

        //Saving the PDF to the MemoryStream
        MemoryStream stream = new MemoryStream();

        stream.Write(document);

        //Set the position as '0'.
        stream.Position = 0;

        //Download the PDF document in the browser
        FileStreamResult fileStreamResult =
            new FileStreamResult(stream, "application/pdf");

        fileStreamResult.FileDownloadName =
            $"BusRoutesBuddy – Ticket No.{ticket.Id}." +
                $" Route No.{ticket.RouteId}.pdf." +
                $" {ticket.Cities.First().DepartureTime?.ToString("dd.MM.yyyy")}";

        return fileStreamResult;
    }
}