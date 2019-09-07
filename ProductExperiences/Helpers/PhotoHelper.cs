using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProductExperiences.Helpers
{
    public class PhotoHelper
    {
        
        public static string SaveImageAndReturnUniqueFileName(IFormFile photo, IHostingEnvironment hostingEnvironment, string folder)
        {
            string uniqueFileName = null;

            if (photo != null)
            {
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, folder);
                uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                photo.CopyTo(new FileStream(filePath, FileMode.Create));

            }

            return uniqueFileName;
        }
    }
}
