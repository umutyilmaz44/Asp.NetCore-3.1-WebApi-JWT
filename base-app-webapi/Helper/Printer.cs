using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using base_app_common;
using base_app_common.dto.user;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using PuppeteerSharp;
using RazorLight;
using PuppeteerSharp.Media;
using System.Text;

namespace base_app_webapi.Helper {
    public interface IPrinter {
       
        string PrintOutputs { get; }
        Task<ServiceResult<string>> CompileHtmlAsync (string templateName, object model);
        Task<ServiceResult<string>> CompileHtmlAsync<T> (string templateName, T model);

        
        Task<ServiceResult<string>> ConvertHtmlToPdfAsync (string htmlFilePath);
    }
    
    public class Printer : IPrinter {
        private readonly IWebHostEnvironment env;
        private readonly RazorLightEngine engine;
        private readonly string browserExecutablePath;

        public string PrintOutputs{ get {return Path.Combine (env.ContentRootPath, "Print_Outputs"); }}

        private string templateFolderPath; 

        public Printer (IWebHostEnvironment env) {
            this.env = env;

            templateFolderPath = Path.Combine (env.ContentRootPath, "Print_Templates");           
            engine = new RazorLightEngineBuilder ()
                                            // required to have a default RazorLightProject type,
                                            // but not required to create a template from string.
                                            .UseFileSystemProject(templateFolderPath)
                                            .UseMemoryCachingProvider ()
                                            .Build ();
            var bfOptions = new BrowserFetcherOptions();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                bfOptions.Path = Path.GetTempPath();
            }
            var bf = new BrowserFetcher(bfOptions);
            bf.DownloadAsync(BrowserFetcher.DefaultRevision).Wait();
            browserExecutablePath = bf.GetExecutablePath(BrowserFetcher.DefaultRevision);
            //revisionInfo = (new BrowserFetcher()).DownloadAsync(BrowserFetcher.DefaultRevision).Result;            
        }

        public async Task<ServiceResult<string>> CompileHtmlAsync<T> (string templateName, T model) {
            try {                
                string htmlResult = await engine.CompileRenderAsync<T>(templateName + "/view.cshtml", model);

                string htmlFilePath = SaveHtmlDocument(templateName, htmlResult);

                return new ServiceResult<string> (htmlFilePath, true, "");
            } 
            catch (Exception ex) {
                return new ServiceResult<string> ("", false, ex.ToString ());
            }
        }
        
        public async Task<ServiceResult<string>> CompileHtmlAsync (string templateName, object model) {
            try {                
                string htmlResult = await engine.CompileRenderAsync(templateName + "/view.cshtml", model);
                
                string htmlFilePath = SaveHtmlDocument(templateName, htmlResult);

                return new ServiceResult<string> (htmlFilePath, true, "");
            } 
            catch (Exception ex) {
                return new ServiceResult<string> ("", false, ex.ToString ());
            }
        }

        private string SaveHtmlDocument(string templateName, string htmlContent)
        {
            string outputFolderPath = Path.Combine(PrintOutputs, Guid.NewGuid().ToString());
            string htmlFilePath = Path.Combine(outputFolderPath, "templateName.html");
            string cssFilePath = Path.Combine(outputFolderPath, "templateName.css");
            string jsFilePath = Path.Combine(outputFolderPath, "templateName.js");
            
            Directory.CreateDirectory(outputFolderPath);

            using(StreamWriter sw = new StreamWriter(htmlFilePath, false, Encoding.UTF8))
            {
                sw.Write(htmlContent);
            }

            string srcJsFilePath = Path.Combine(templateFolderPath, templateName, "view.js");
            string srcCssFilePath = Path.Combine(templateFolderPath, templateName, "view.css");

            if(File.Exists(srcJsFilePath))
                File.Copy(srcJsFilePath, jsFilePath);
            
            if(File.Exists(srcCssFilePath))
                File.Copy(srcCssFilePath, cssFilePath);

            return htmlFilePath;
        }

        public async Task<ServiceResult<string>> ConvertHtmlToPdfAsync (string htmlFilePath) {
            try
            {
                FileInfo htmlFileInfo = new FileInfo(htmlFilePath);
                if(!htmlFileInfo.Exists)
                    throw new Exception("Html File Not Found!");
                
                string pdfFilePath = Path.Combine(htmlFileInfo.DirectoryName, htmlFileInfo.Name.Replace(htmlFileInfo.Extension,"") + ".pdf");

                LaunchOptions options = new LaunchOptions {                                
                    Headless = true,
                    ExecutablePath = browserExecutablePath                    
                };                
                PdfOptions pdfOptions = new PdfOptions(){
                    Format = PaperFormat.A4,
                    PrintBackground = true
                };
                using (var browser = await Puppeteer.LaunchAsync(options))
                {
                    using (var page = await browser.NewPageAsync())
                    {                        
                        Response response = await page.GoToAsync("file://" + htmlFilePath);
                        if(response.Ok){
                            await page.PdfAsync(pdfFilePath, pdfOptions);
                            return new ServiceResult<string>(pdfFilePath, true, "");                            
                        }
                        else{
                            return new ServiceResult<string>(null, false, response.StatusText);
                        }
                        
                    }
                }
            }
            catch(Exception ex)
            {
                return new ServiceResult<string>(null, false, ex.Message);
            }
        }
    }
}