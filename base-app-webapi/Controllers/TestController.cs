using System;
using System.IO;
using System.Threading.Tasks;
using base_app_common;
using base_app_common.dto.user;
using base_app_common.dto.utilities;
using base_app_service;
using base_app_webapi.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace base_app_webapi.Controllers {
    [Route ("api/[controller]")]
    [ApiController]

    public class TestController : BaseController {
        private readonly IMailer mailer;
        private readonly IPrinter printer;

        public TestController (IServiceManager serviceManager, ILogger<BaseController> logger, IMailer mailer, IPrinter printer) : base (serviceManager, logger) {
            this.mailer = mailer;
            this.printer = printer;
        }

        // POST: api/Test/Mail/
        [HttpPost ("Mail")]
        public async Task<GenericResponse> TestMail (MailDto mailDto) {
            // Deneme amaçlı eklendi
            //ServiceResult service = await mailer.SendAsync(new string[]{"umutyilmaz44@gmail.com","uyilmaz@vhselektronik.com.tr"}, null, null, "test","denemem eerer", null);

            ServiceResult result = await mailer.SendAsync (mailDto.recipients, mailDto.bccList, mailDto.ccList, mailDto.subject, mailDto.body, mailDto.attachments);
            if (result.Success)
                return GenericResponse.Ok ();
            else
                return GenericResponse.Error (ResultType.Error, result.Error, "TC_TM_01", StatusCodes.Status500InternalServerError);
        }

        // POST: api/Test/Print/
        [HttpPost ("CompileHtml")]
        public async Task<GenericResponse<string>> TestCompileHtml () {
            // Deneme amaçlı eklendi
            string templateName = "example";
            UserDto userDto = new UserDto () {
                FirstName = "Mühüttün",
                LastName = "GANDAK"
            };
            ServiceResult<string> result = await printer.CompileHtmlAsync (templateName, userDto);
            if (result.Success)
                return GenericResponse<string>.Ok (result.Data);
            else
                return GenericResponse<string>.Error (ResultType.Error, result.Error, "TC_TM_01", StatusCodes.Status500InternalServerError);
        }

        // POST: api/Test/Print/
        [HttpPost ("Print")]
        public async Task<ActionResult> TestPrint () {
            // Deneme amaçlı eklendi
            string templateName = "user_info";
            UserDto userDto = new UserDto () {
                FirstName = "Mühüttün",
                LastName = "GANDAK"
            };
            ServiceResult<string> result = await printer.CompileHtmlAsync (templateName, userDto);
            if (result.Success) {
                try {
                    string htmlFilePath = result.Data; 
                    ServiceResult<string> pdfResult = await printer.ConvertHtmlToPdfAsync (htmlFilePath);
                    if (pdfResult.Success) {
                        string pdfFilePath = pdfResult.Data;
                        string pdfFileName = templateName + ".pdf";
                                                   
                        return File (pdfFilePath, "application/pdf", pdfFileName);
                    } else {
                        return BadRequest (pdfResult.Error);
                    }
                } catch (Exception ex) {
                    return BadRequest (ex.Message);
                }
            } else
                return BadRequest (result.Error);
        }
    }
}